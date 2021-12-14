using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using Pullenti;
using Pullenti.Morph;
using Pullenti.Ner;
using Pullenti.Ner.Core;
using Spectre.Console;

namespace Pullenti_Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            InitPullenti();

            // Анализируемый текст
            string txt = "Система разрабатывается с 2011 года российским программистом Михаилом Жуковым, проживающим в Москве на Красной площади в доме номер один на втором этаже. " +
                         "Конкурентов у него много: Abbyy, Yandex, ООО \"Russian Context Optimizer\" (RCO) и другие компании. " +
                         "Он планирует продать SDK за 1.120.000.001,99 (миллиард сто двадцать миллионов один рубль 99 копеек) рублей, без НДС.";
            string txt1 = "Глокая куздра штеко будланула бокра и курдячит бокрёнка";
            string textForAnalys = txt;

            AnsiConsole.MarkupLine("[bold yellow]Текст для анализа[/]: {0}", textForAnalys);
            AnsiConsole.WriteLine();

            var analyzers = new Tree("[yellow bold]Доступные анализаторы[/]");
            foreach (var analyzer in ProcessorService.Analyzers)
                analyzers.AddNode(analyzer.Caption);
            AnsiConsole.Write(analyzers);
            AnsiConsole.WriteLine();

            AnalysisResult are = ProcessorService.StandardProcessor.Process(new SourceOfAnalysis(textForAnalys));

            var root = new Tree("[bold yellow]Результат анализатора[/]").Style("red");

            var entities = root.AddNode("Entities");
            foreach (var areEntity in are.Entities)
            {
                var entity = entities.AddNode($"[blue bold]{areEntity.InstanceOf.Caption}[/]: ${areEntity}");
                foreach (var areEntitySlot in areEntity.Slots)
                    entity.AddNode(areEntitySlot.ToString());
            }

            // var numberTokens = root.AddNode("Числовые токены");
            for (var t = are.FirstToken; t != null; t = t.Next)
            {
                // может, номер задан явно цифрами или прописью
                NumberToken num = t as NumberToken;

                Token t1 = null; // ссылка на слово "сессия"
                if (num != null) t1 = t.Next;
                else
                {
                    // пробуем выделить римское число
                    num = NumberHelper.TryParseRoman(t);
                    if (num != null)
                    {
                        // поскольку токен num не встроен в общую цепочку, а BeginToken\EndToken
                        // указывают на первый и последний токены цепочки, то следующий не num.Next,
                        // а именно num.EndToken.Next
                        t1 = num.EndToken.Next;
                    }
                }
                if (t1 == null || num == null)
                    continue;
                if (!t1.IsValue("СЕССИЯ"))
                    continue;

// нашли
                AnsiConsole.Write("\r\nSession {0} on position {1}", num.Value, t.BeginChar);
                t = t1;
            }

            AnsiConsole.Write(root);
            AnsiConsole.WriteLine();
            
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