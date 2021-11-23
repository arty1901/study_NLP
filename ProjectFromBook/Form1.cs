using System.Drawing.Drawing2D;
using System.Text;
using System.Text.RegularExpressions;

namespace ProjectFromBook
{
    /// <summary>
    /// Реализация приложения из книги "Разработка приложений для анализа - С#
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// Стоп-слова
        /// </summary>
        private string[] stopWords = { "в", "на", "по", "из", "за" };
        
        /// <summary>
        /// Символы, 
        /// </summary>
        private char[] splitChars = { '\n','\r', ' ', '.', ',' };

        /// <summary>
        /// Регулярное выражение для разделения входного текста на токены
        /// </summary>
        private Regex regex = new Regex("\\W");
        private Bitmap _bitmap;
        private Graphics _graphics;

        private MorphologicalAnalysis morphologicalAnalysis;
        private ClusterAnalysis clusterAnalysis;

        public Form1()
        {
            InitializeComponent();

            morphologicalAnalysis = new MorphologicalAnalysis(regex);

            clusterAnalysis = new ClusterAnalysis(
                numberOfIterations: 1000, 
                numberOfClusters: 2, 
                ei: 0.001f, 
                mi: 1.6f, 
                displayDelegate: DisplayMessage);
        }

        /// <summary>
        /// Статистический метод подсчета частот - метод прямого подсчета частоты n-словий
        /// Стр. 33
        /// </summary>
        /// <param name="masSt"></param>
        /// <returns>Результатом его работы является множество пар F(x)=<x,count_x></returns>
        private Dictionary<string, int> GetFrequencyOfWord(string[] masSt)
        {
            outputTextBox.Text = string.Empty;
            var freqDic = new Dictionary<string, int>();

            foreach (string s in masSt)
                if (freqDic.ContainsKey(s.ToUpper()))
                    freqDic[s.ToUpper()]++;
                else
                    freqDic.Add(s.ToUpper(), 1);
            
            return freqDic;
        }

        /// <summary>
        /// Расчет и отображение подсчета частот
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void freqButton_Click(object sender, EventArgs e)
        {
            string[] tokens = regex.Split(InputTextBox.Text);
            
            var dict = GetFrequencyOfWord(tokens);

            PrintDictionaryToOutputTextBox(dict);
            DrawGrahic(dict);
        }

        /// <summary>
        /// Mutual Information
        /// Это метод применяется только к двусловиям и 
        /// предполагает вычисление коэффициента взаимной информации по формуле (формула описана на стр 37)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bigramValueButton_Click(object sender, EventArgs e)
        {
            var inputStr = InputTextBox.Text;

            foreach (string s in stopWords)
                inputStr.Replace(s, string.Empty);

            string[] masSt = regex.Split(inputStr);

            Dictionary<string, int> FreqDic = GetFrequencyOfWord(masSt);

            PrintDictionaryToOutputTextBox(FreqDic);

            string ModS = InputTextBox.Text.Replace(BigramTextBox.Text.Trim(), "");
            string[] masSt2 = ModS.Split(new char[] { ' ', '.', ',' }, StringSplitOptions.RemoveEmptyEntries);


            double fxy = (masSt.Length - masSt2.Length) / 2;
            string[] masBigr = BigramTextBox.Text.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            int fx = FreqDic.Where(x => x.Key.Contains(masBigr[0].ToUpper())).FirstOrDefault().Value;
            int fy = FreqDic.Where(x => x.Key.Contains(masBigr[1].ToUpper())).FirstOrDefault().Value;
            double MI = Math.Log((fxy * masSt.Length) / (fx * fy), 2);

            outputTextBox.Text += Environment.NewLine + "fx:" + fx + " fy:" + fy;
            outputTextBox.Text += Environment.NewLine + "MI: " + MI;

            if (MI < 0)
                outputTextBox.Text += Environment.NewLine + "Каждое из слов встречается лишь в тех позициях, в которых невстречается другое";
            if (MI > 1)
                outputTextBox.Text += Environment.NewLine + "Значимо";
            if ((MI > 0) && (MI < 1)) 
                outputTextBox.Text += Environment.NewLine + "Не значимо";

            DrawGrahic(FreqDic);
        }

        private void PrintDictionaryToOutputTextBox(Dictionary<string, int> dict)
        {
            var builder = new StringBuilder();
            foreach (var s in dict.OrderByDescending(x => x.Value))
                builder.Append($"{s.Key} {s.Value} {Environment.NewLine}");

            outputTextBox.Text = builder.ToString();
        }

        /// <summary>
        /// Отрисовка графика частот слов
        /// </summary>
        /// <param name="dict"></param>
        private void DrawGrahic(Dictionary<string, int> dict)
        {
            _bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            _graphics = Graphics.FromImage(_bitmap);

            int maxX = dict.Values.Count;
            int maxY = dict.Max(x => x.Value);
            int max = pictureBox1.Height - 25;

            float cdx = (pictureBox1.Width - 90) / maxX;
            float cdy = (pictureBox1.Height - 50) / maxY;

            Pen p = new Pen(Brushes.Green, 3);
            p.EndCap = LineCap.ArrowAnchor;
            _graphics.DrawLine(p, 25, pictureBox1.Height - 25, 25, 0);
            _graphics.DrawLine(p, 0, 0, pictureBox1.Width - 25, 0);

            List<string> ks = dict.Keys.ToList();

            for (int x = 0; x < maxX; x++)
            {
                _graphics.DrawString(ks[x].ToString(), new Font("Arial", 6), Brushes.Green, (x * cdx) + 25, max + 10 + ((x % 2 == 0) ? 7 : 0));
                _graphics.DrawLine(Pens.Green, (x * cdx) + 25, pictureBox1.Height - 25, (x * cdx) + 25, 0);
            }

            for (float y = 1; y <= maxY + 1; y++)
            {
                _graphics.DrawString(y.ToString(), new Font("Arial", 10), Brushes.Green, 0, (max - y * cdy));
                _graphics.DrawLine(Pens.Green, 0, max - (y * cdy), pictureBox1.Width - 25, max - (y * cdy));
            }

            for (int i = 0; i < ks.Count(); i++)
                _graphics.FillRectangle(Brushes.Red, (int)(i * cdx) + 25, (int)(max - dict[ks[i]] * cdy), 10, (int)(dict[ks[i]] * cdy));

            pictureBox1.Image = _bitmap;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            //pictureBox1.Update();
        }

        private void openFileButton_Click(object sender, EventArgs e)
        {
            string[] tokens = regex.Split(GetContentFromFile());
            GetFrequencyOfWord(tokens);
        }

        private string GetContentFromFile()
        {
            var fileContent = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Set filter
                openFileDialog.Filter = "txt files (*.txt)|*.txt";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();
                    }
                }
            }

            return fileContent;
        }

        private void fileAnalysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            morphologicalAnalysis.AnalysTextWithMyStem();
        }

        public void DisplayMessage(string message)
        {
            outputTextBox.Text += message + Environment.NewLine;
        }

        /// <summary>
        /// Посчитать кластеризацию из поля ввода
        /// </summary>
        private void inputTextBoxSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] tokens = regex.Split(InputTextBox.Text.ToUpper());
            DoClusterWork(tokens);
        }

        /// <summary>
        /// Посчитать кластеризацию из файла
        /// </summary>
        private void fileSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] tokens = regex.Split(GetContentFromFile().ToUpper());
            DoClusterWork(tokens);
        }

        private void DoClusterWork(string[] tokens)
        {
            var dict = GetFrequencyOfWord(tokens);
            clusterAnalysis.Objecti = dict.Values.ToArray();
            DisplayMessage(clusterAnalysis.Work(dict));
        }
    }
}