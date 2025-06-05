using System.Text.RegularExpressions;
using AngleSharp;
using AngleSharp.Dom;

namespace lab4opt4
{

    public class nnnnn
    {
        public static async Task funct()
        {
            IConfiguration config = Configuration.Default.WithDefaultLoader();
            IDocument document = await new BrowsingContext(config).OpenAsync("https://www.susu.ru");
            var html = document.Body.OuterHtml;
            
            var menuPattern = @"Адрес: \d{5,7}, г. Челябинск, пр. Ленина, \d{1,3}";
            List<string> massivSilok = Regex.Matches(html, menuPattern, RegexOptions.Singleline)
                .Select(match => match.Groups[1].Value)
                .ToList();




            for (int i = 0; i < massivSilok.Count; i++)
            {
                Console.Write(i.ToString() + "\t");
                Console.WriteLine(massivSilok[i]);
            }


            Console.WriteLine(html);
        }
    }
}


/*
class Program
{
    static async Task Main(string[] args)
    {
        // URL для парсинга
        string url = "https://www.susu.ru";

        // Конфигурация AngleSharp
        var config = Configuration.Default.WithDefaultLoader();

        // Загрузка страницы
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(url);

        // Извлечение текста страницы
        string pageText = document.Body.TextContent;

        // Вывод текста
        Console.WriteLine("Текст страницы:");
        Console.WriteLine(pageText);
    }
}
}*/