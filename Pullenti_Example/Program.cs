using System.Diagnostics;
using Pullenti;
using Pullenti.Morph;
using Pullenti.Ner;
using Spectre.Console;

namespace Pullenti_Example
{
    /// <summary>
    /// Пример использования SDK Pullenti
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            InitPullenti();

            // Анализируемый текст
            string txt = "Система разрабатывается с 2011 года российским программистом Михаилом Жуковым, проживающим в Москве на Красной площади в доме номер один на втором этаже. " +
                         "Конкурентов у него много: Abbyy, Yandex, ООО \"Russian Context Optimizer\" (RCO), A-120 и другие компании. " +
                         "Он планирует продать SDK за 1.120.000.001,99 (миллиард сто двадцать миллионов один рубль 99 копеек) рублей, без НДС.";
            string txt1 = "Глокая куздра штеко будланула бокра и курдячит бокрёнка";
            string txt2 = "Я шел домой по незнакомой улице";
            string txt3 =
                "Теория пределов – это один из разделов математического анализа. Вопрос решения пределов является достаточно обширным, " +
                "поскольку существуют десятки приемов решений пределов различных видов. Существуют десятки нюансов и хитростей, " +
                "позволяющих решить тот или иной предел. Тем не менее, мы все-таки попробуем разобраться в основных типах пределов, " +
                "которые наиболее часто встречаются на практике. Yandex";
            string textForAnalys = txt3;

            AnsiConsole.MarkupLine("[bold yellow]Текст для анализа[/]: {0}", textForAnalys);
            AnsiConsole.WriteLine();

            var analyzers = new Tree("[yellow bold]Доступные анализаторы, которые используются по умолчанию[/]");
            foreach (var analyzer in ProcessorService.Analyzers)
                analyzers.AddNode(analyzer.Caption);
            AnsiConsole.Write(analyzers);
            AnsiConsole.WriteLine();

            AnalysisResult are = ProcessorService.StandardProcessor.Process(new SourceOfAnalysis(textForAnalys));

            var root = new Tree("[bold yellow]Результат анализатора[/]").Style("red");

            // Отображение сущностей в предложении
            if (are.Entities.Count == 0)
                AnsiConsole.MarkupLine("[red bold]Сущности не выделены[/]");
            else
            {
                var entities = root.AddNode("Сущности");
                foreach (var areEntity in are.Entities)
                {
                    var entity = entities.AddNode($"[blue bold]{areEntity.InstanceOf.Caption}[/]: ${areEntity}");
                    foreach (var areEntitySlot in areEntity.Slots)
                        entity.AddNode(areEntitySlot.ToString());
                }

                AnsiConsole.Write(root);
                AnsiConsole.WriteLine();
            }
            
            PrintInTable(MorphologyService.Process(textForAnalys));
        }

        /// <summary>
        /// Перед использованием функций SDK необходимо один раз произвести инициализацию.
        /// Она занимает несколько секунд, во время которых происходит распаковка словарей и подготовка данных.
        /// </summary>
        private static void InitPullenti()
        {
            Stopwatch sw = Stopwatch.StartNew();
            // инициализация - необходимо проводить один раз до обработки текстов
            AnsiConsole.MarkupLine("[bold red]Initializing SDK Pullenti ver {0} ({1}) ... [/]", Sdk.Version, Sdk.VersionDate);
            // инициализируются движок и все имеющиеся анализаторы
            Sdk.InitializeAll();
            sw.Stop();
            AnsiConsole.MarkupLine("[green underline]OK (by {0} ms), version {1}[/]", (int)sw.ElapsedMilliseconds, ProcessorService.Version);
            AnsiConsole.WriteLine();
        }

        /// <summary>
        /// Морфологический анализ
        /// </summary>
        /// <param name="processor"></param>
        private static void PrintInTable(List<MorphToken> processor)
        {
            var table = new Table
            {
                Title = new TableTitle("[yellow bold]Морфологический анализ текста[/]"),
                Border = TableBorder.Rounded
            };

            // добавил столбцы
            table.AddColumn(new TableColumn("Слово").Centered());
            table.AddColumn(new TableColumn("Слово (инф)").Centered());
            table.AddColumn(new TableColumn("Часть речи").Centered());
            table.AddColumn(new TableColumn("Число").Centered());
            table.AddColumn(new TableColumn("Род").Centered());
            table.AddColumn(new TableColumn("Падеж").Centered());
            table.AddColumn(new TableColumn("Время").Centered());
            table.AddColumn(new TableColumn("Залог").Centered());
            
            foreach (var morphToken in processor)
            {
                var row = new List<string>();

                if (morphToken.WordForms.Count == 0)
                {
                    table.AddRow(new Markup($"[red]{morphToken.Term}[/]"));
                    continue;
                }
                row.Add(morphToken.Term);

                var wordForm = morphToken.WordForms[0];
                
                // Началь форма (для глаголов инф)
                row.Add(wordForm.NormalCase);
                // часть речи
                row.Add(wordForm.Class.ToString());
                // Число
                row.Add(GetNumberDescription(wordForm.Number));
                // род
                row.Add(wordForm.Class.IsNoun ? GetGenderDescription(wordForm.Gender) : "-");
                // Падеж
                row.Add(wordForm.Case.IsUndefined ? "-" : wordForm.Case.ToString());
                // Время (для глаголов)
                row.Add(GetTensDescription(wordForm.Misc.Tense));
                // Залог (для глаголов)
                row.Add(GetVoiceDescription(wordForm.Misc.Voice));

                table.AddRow(row.ToArray());
            }
            
            AnsiConsole.Write(table);
        }

        private static string GetNumberDescription(MorphNumber number) => number switch
        {
            MorphNumber.Plural => "Множественное",
            MorphNumber.Singular => "Единственное",
            MorphNumber.Undefined => "-",
            _ => "Некорректное значение"
        };
        
        private static string GetTensDescription(MorphTense tens) => tens switch
        {
            MorphTense.Future => "Будущее",
            MorphTense.Past => "Прошедшее",
            MorphTense.Present => "Настоящее",
            MorphTense.Undefined => "-",
            _ => "Некорректное значение"
        };

        private static string GetVoiceDescription(MorphVoice voice) => voice switch
        {
            MorphVoice.Active => "Активный",
            MorphVoice.Middle => "Средний",
            MorphVoice.Passive => "Страдательный",
            MorphVoice.Undefined => "-",
            _ => "Некорректное значение"
        };

        private static string GetGenderDescription(MorphGender gender) => gender switch
        {
            MorphGender.Undefined => "-",
            MorphGender.Masculine => "Мужской",
            MorphGender.Feminie => "Женский",
            MorphGender.Neuter => "Средний",
            _ => "Некорректное значение"
        };
    }
}