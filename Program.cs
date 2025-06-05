using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using System.Linq;
using System.Xml.Linq;

namespace lab4opt4
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Parser pagesParser = new Parser("https://www.susu.ru/ru/structure/");
            await pagesParser.Parse(5, 35, 0, 0);
            
            Console.WriteLine("=================================================");
            
            foreach (var texxxt in pagesParser.dateBase) // Перебор спарсенных данных
            {
                foreach (var txte in texxxt.data)
                {
                    Console.WriteLine(txte);
                    Console.WriteLine("---------------------------------------------");
                }
            }

            foreach (var sss in pagesParser.links)
            {
                Console.WriteLine(sss); // sss - перебор 
            }

            pagesParser.SaveToCsv("/Users/egorvasilev/Documents/Институт/C#institut/OOP/lab4opt4/fileXML.csv");

        }
    }
    
    
    
    
//=================================================================================================================================================================================================




    public class Parser
    {
        public Parser(string url)
        {
            /* Помещаем главную страницу в список */
            links = new List<string> { url };
            dateBase = new List<TierDataSet>(); // Хранение и номеров, и адресов

            clasPhoneNumber = new PhoneNumber();
            clasAdreses = new Adreses();
        }

        public List<string> links; // Сюда будут помещаться ссылки для дальнейшего исследования

        public Adreses clasAdreses; // Список для адреесов
        public PhoneNumber clasPhoneNumber; // Список для номеров телефонов

        public List<TierDataSet> dateBase;

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public async Task Parse(short maxTier, short maxTransitionsOfJumps, short tier, short transitionsOfJumps)
        {
            TierDataSet tData = new TierDataSet();
            Console.WriteLine("Ссылка333 \n" + links[0]);
            /* Загружаем код страницы */
            var config = Configuration.Default.WithDefaultLoader();
            var URLdocument = await new BrowsingContext(config).OpenAsync(links[0]);
            links.RemoveAt(0); // Удаляем URL загруженной страницы из очереди
            string textOfPageHtml = URLdocument.Body.InnerHtml;

            //Console.WriteLine(textOfPageHtml); // Тут был для проверки вывод страниц

            /* Добавление ссылок для перехода */
            string regularString = @"/ru/structure/[\w\d-./]+(?(?="">))"; // /ru/structure/yuridicheskiy-institut
            List<string> proxyLinks = Regex.Matches(textOfPageHtml, regularString, RegexOptions.Singleline)
                .Select(match => match.Groups[0].Value)
                .ToList();
            foreach (string proxy in proxyLinks)
            {
                if (links.Contains("https://www.susu.ru/" + proxy) == false)
                {
                    links.Add("https://www.susu.ru/" + proxy);
                    //Console.WriteLine("https://www.susu.ru/" + proxy); // Проверка ссылок
                }
            }

            //Console.WriteLine(textOfPageHtml);

            /* Рекурсивный вызов функции с увеличением глубины и количества переходов */
            if ((tier < maxTier) && (transitionsOfJumps < maxTransitionsOfJumps))
                await Parse(maxTier, maxTransitionsOfJumps, (short)(tier + 1), (short)(transitionsOfJumps + 1));
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------    
            clasAdreses.RegularStringCheck(textOfPageHtml, tData, tier);
            clasPhoneNumber.RegularStringCheck(textOfPageHtml, tData, tier);
            
            dateBase.Add(tData);
            
        }
        
        
        public async Task SaveToCsv(string filePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Запись заголовка
                    writer.WriteLine("Tier,Content");

                    foreach (var tierData in dateBase)
                    {
                        foreach (var data in tierData.data)
                        {
                            // Запись строки в формате CSV
                            writer.WriteLine($"{new string('|', tierData.tier).Replace("|", "|---|")}\"{data.Replace("\"", "\"\"")}\"");
                        }
                    }
                }

                Console.WriteLine($"Данные успешно сохранены в файл: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении данных в CSV: {ex.Message}");
            }
        }
        
    }
//=================================================================================================================================================================================================

    public abstract class Datasets // Базовый(абстрактный) класс 
/*              Datasets
            /     \
       Adreses    PhoneNumber         */
    {

        public List<string> baseData;

        public void Sorted() // Сортировка сисков номеров телефонов или адресов
        {
            baseData.Sort();
        }

        public void PreRegularStringCheck(string html, string regularExpressionsForAdress1)
        {
            baseData.AddRange(Regex.Matches(html, regularExpressionsForAdress1, RegexOptions.Singleline)
            .Select(match => string.Concat(Enumerable.Range(1, match.Length) // Группы от 1 до 3
                .Select(i => match.Groups[i].Value)))
            .ToList());


            // foreach (var b in baseData)
            // {
            //     Console.WriteLine(b);
            // }

        }

    }
    
//=================================================================================================================================================================================================

             /*Адреса*/
    public class Adreses : Datasets 
    {
        public Adreses()
        {
            regularExpressions = new string[2] // Иницылизация массива регулярных выражений
            {
                @"(?=title=""\w{1,5}"">)*(Челябинск, [\s\w]+, \d{1,3})(?=</a><br>)",
                @"(Адрес: \d{5,7}, г. Челябинск, пр. Ленина, \d{1,3})",
                /*Адрес: 454080, г. Челябинск, пр. Ленина, 76<br>
                 "location":{"id":661420,"name":"Челябинск, р-н Ленинский","namePrepositional":"Челябинске"},*/
            };
            baseData = new List<string> { };
        }

        private string[] regularExpressions; // Создание массива регулярных выражений

        //--------------------------------------------------------------------------        

        public void RegularStringCheck(string html, TierDataSet OaddData, short tier)
        {
            OaddData.tier = tier;
            foreach (string regularExpression in regularExpressions)
            {
                PreRegularStringCheck(html, regularExpression); // Проверка с помощью регулярного выражения
            }
            OaddData.data.AddRange(baseData);
        }

    }

//=================================================================================================================================================================================================

            /*Номера телефонов*/
    public class PhoneNumber : Datasets 
    {
        public PhoneNumber()
        {
            regularExpressions = @"(\+7)*(\s*\(*\d{3}\)\s\d{3}-\d{2}-\d{2})"; // Иницылизация строки регулярного выражения
            baseData = new List<string> { };
        }

        public string regularExpressions; // Создание строки регулярного выражения

    //--------------------------------------------------------------------------        

        public void RegularStringCheck(string html, TierDataSet OaddData , short tier)
        {
            OaddData.tier = tier;
            PreRegularStringCheck(html, regularExpressions); // Проверка с помощью регулярного выражения
            OaddData.data.AddRange(baseData);
        }
        
    }
    
    //=================================================================================================================================================================================================

    public class TierDataSet
    {
        public List<string> data = new List<string>();
        public short tier;
    }
    
    
    
    
}