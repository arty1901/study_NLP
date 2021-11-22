using System.Text.RegularExpressions;

namespace ProjectFromBook
{
    /// <summary>
    /// МОРФОЛОГИЧЕСКИЙ АНАЛИЗ ТЕКСТОВ
    /// Используется утилита Yandex Mystem - морфологический анализатор русского языка с поддержкой снятия морфологической неоднозначности
    /// Стр 63
    /// </summary>
    public class MorphologicalAnalysis
    {
        private Regex regex;
        private const string outputFilePath = @"C:/Users/arty1/RiderProjects/study_NLP/ProjectFromBook/files/output.txt";
        private const string inputFilePath = @"C:/Users/arty1/RiderProjects/study_NLP/ProjectFromBook/files/input.txt";

        public MorphologicalAnalysis(Regex regex)
        {
            this.regex = regex;

        }

        public async void AnalysTextWithMyStem()
        {
            string inputFilename = string.Empty;

            var ofd = new OpenFileDialog();
            ofd.Filter = "Приложения|*.exe";
            ofd.Title = "Выберите файл программы mystem";

            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            System.Diagnostics.Process command = new System.Diagnostics.Process();
            command.StartInfo.FileName = ofd.FileName;

            // Использованные ключи
            // -i - Печатать грамматическую информацию, расшифровка ниже.
            // -n - Построчный режим; каждое слово печатается на новой строке.
            // -g - Склеивать информацию словоформ при одной лемме (только при включенной опции -i).
            command.StartInfo.Arguments = "-ign " + inputFilePath + " " + outputFilePath;
            await Task.Factory.StartNew(() => 
            { 
                command.Start(); 
            });

            MessageBox.Show("Анализ закончен");
        }
    }
}
