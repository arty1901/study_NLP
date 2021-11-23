namespace ProjectFromBook
{
    public delegate void Display(string message);

    /// <summary>
    /// КЛАСТЕРНЫЙ АНАЛИЗ ТЕКСТОВ
    /// Шаги алгоритма
    /// 1) Инициализация (стр 76)
    /// 2) Вычисление центров кластеров (стр 77)
    /// 3) Формируется новая матрица принадлежности с учетом вычисленных на предыдущем шаге центров кластеров (стр 77)
    /// 4) Вычисляется значение целевой функции, и полученное значение сравнивается со значением на предыдущей итерации
    /// </summary>
    public class ClusterAnalysis
    {
        #region Properties/Fields
        private Random random = new Random();

        /// <summary>
        /// Массив степеней принадлежностей
        /// </summary>
        private float[,] _stPr;

        /// <summary>
        /// массив характеристик объектов
        /// </summary>
        public int[] Objecti { 
            get => objecti;
            set { 
                objecti = value;
                _numberOfObjects = objecti.Length;
            }
        }

        private int[] objecti;

        /// <summary>
        /// параметр q
        /// </summary>
        private float mi;

        /// <summary>
        /// Степень точности (параметр сходимости алгоритма).
        /// Когда разность значений целевых функций текущей и 
        /// предыдущей итераций достигнет этого уровня, считается, что кластеризация завершена
        /// </summary>
        private float ei;

        /// <summary>
        /// Количество итераций алгоритма.
        /// Задается во избежание зависания алгоритма 
        /// при невозможности достижения уровня точности
        /// </summary>
        private int _numberOfIterations;

        /// <summary>
        /// количество кластеров (C)
        /// </summary>
        private int _numberOfClusters;

        /// <summary>
        /// количество объектов (N)
        /// </summary>
        private int _numberOfObjects;

        /// <summary>
        /// массив центров кластеров
        /// </summary>
        private float[] _centerClusters;

        event Display _printMessage;
        #endregion

        // 1 шаг - инициализация
        public ClusterAnalysis(float mi, float ei, int numberOfIterations, int numberOfClusters, Display displayDelegate)
        {           
            _centerClusters = new float[_numberOfClusters];
            _numberOfClusters = numberOfClusters;

            this.mi = mi;
            this.ei = ei;
            _numberOfIterations = numberOfIterations;

            _printMessage += displayDelegate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool CheckStPr()
        {
            for (int i = 0; i < _numberOfObjects; i++)
            {
                int t = -1;
                for (int j = 0; j < _numberOfClusters; j++)
                {
                    if (_stPr[i, j] == 1) { t = j; break; }
                }

                if (t >= 0)
                {
                    for (int j = 0; j < _numberOfClusters; j++)
                    {
                        if (j != t) _stPr[i, j] = 0;
                    }
                }

                float sum = 0;

                for (int j = 0; j < _numberOfClusters; j++)
                {
                    sum = sum + (_stPr[i, j]);
                }

                while (sum - 1 >= 0.01)
                {
                    float max = 0;
                    int ti = 0;
                    for (int j = 0; j < _numberOfClusters; j++)
                    {
                        if (_stPr[i, j] > max) { max = _stPr[i, j]; ti = j; }
                    }
                    float razn = max - sum + 1;
                    if (razn >= 0) { _stPr[i, ti] = razn; sum = 0; }
                    if (razn < 0) { _stPr[i, ti] = 0; sum = sum - max; }
                }

                sum = 0;

                for (int j = 0; j < _numberOfClusters; j++)
                {
                    sum = sum + (_stPr[i, j]);
                }

                if (sum > 1.01)
                {
                    _printMessage("Error!!!!!");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Первоначальная случайная генерация матрицы принадлежности объектов кластерам
        /// </summary>
        private void InitRandomMatrix()
        {
            _stPr = new float[_numberOfObjects, _numberOfClusters];

            for (int i = 0; i < _numberOfObjects; i++)
                for (int j = 0; j < _numberOfClusters; j++)
                    _stPr[i, j] = (float)random.NextDouble();

            CheckStPr();
        }

        /// <summary>
        /// Рассчет расстояния между двумя точками на прямой характеристик
        /// </summary>
        /// <param name="x"></param>
        /// <param name="c"></param>
        /// <returns>Расстояние между двумя точками</returns>
        private float CalcDistance(float x, float c) => Math.Abs(x - c);

        /// <summary>
        /// Расчет оценочной функции
        /// </summary>
        /// <param name="Znach"></param>
        /// <param name="Klast"></param>
        /// <returns></returns>
        private float Klaster_CountOcF(int Znach, int Klast)
        {
            float sum = 0f;
            for (int i = 0; i < Znach; i++)
                for (int j = 0; j < Klast; j++)
                {
                    mi = sum + (float)(Math.Pow(_stPr[i, j], mi) * CalcDistance(objecti[i], _centerClusters[j]));
                }
            return sum;
        }

        /// <summary>
        /// Расчет U значения
        /// </summary>
        /// <param name="t"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        private float CountUValue(int t, int j)
        {
            float u = 0f;
            if (CalcDistance(objecti[t], _centerClusters[j]) == 0)
                return 1;
            else
            {
                for (int i = 0; i < _numberOfClusters; i++)
                {
                    if (CalcDistance(objecti[t], _centerClusters[i]) == 0)
                    { return 1; }

                    else u = u + (float)Math.Pow(CalcDistance
                    (objecti[t], _centerClusters[j]) / CalcDistance(objecti[t], _centerClusters[i]), 2 / (mi - 1));
                }
                _printMessage("!!!U in Klaster!!!:" + u.ToString());
                if (u != 0) u = 1 / u;
                return u;
            }
        }

        /// <summary>
        /// Метод расчета центров кластеров
        /// </summary>
        /// <param name="j"></param>
        /// <returns></returns>
        float CalcCentersOfClusters(int j)
        {
            float c = 0f;
            float c1 = 0f;
            for (int i = 0; i < _numberOfObjects; i++)
            {
                c = c + (float)(Math.Pow(_stPr[i, j], mi)) * objecti[i];
                c1 = c1 + (float)(Math.Pow(_stPr[i, j], mi));
            }
            return c / c1;
        }

        public string Work(Dictionary<string, int> objects)
        {
            int Shag = 0;
            float epx = 1000f;
            float OldF = 0f;
            float NewF = 0f;

            _printMessage("Klaster");

            InitRandomMatrix();

            _printMessage("InitF");

            for (int j = 0; j < _numberOfClusters; j++)
            {
                _centerClusters[j] = CalcCentersOfClusters(j);
            }

            Klaster_CountOcF(_numberOfObjects, _numberOfClusters);

            while ((Shag < _numberOfIterations) && (epx > ei)) //main cicle
            {
                _printMessage(Shag.ToString());
                Shag++;
                for (int j = 0; j < _numberOfClusters; j++)
                {
                    _centerClusters[j] = CalcCentersOfClusters(j);
                }
                for (int ii = 0; ii < _numberOfObjects; ii++)
                    for (int j = 0; j < _numberOfClusters; j++)
                    {
                        _stPr[ii, j] = CountUValue(ii, j);
                    }

                if (!CheckStPr()) return "Function error";

                NewF = Klaster_CountOcF(_numberOfObjects, _numberOfClusters);

                if (Shag > 1)
                    epx = Math.Abs(OldF - NewF);

                OldF = NewF;
            } //main cicle

            _printMessage("Itog");

            for (int i = 0; i < _numberOfObjects; i++)
            {
                float maxStPr = _stPr[i, 0];
                int numk = 0;
                for (int j = 0; j < _numberOfClusters; j++)
                {
                    if (maxStPr < _stPr[i, j]) { maxStPr = _stPr[i, j]; numk = j; }
                    _printMessage("Klaster" + j + ":" +
                    getName(objects, objecti[i], j == 1) + objecti[i] +
                    " " + _stPr[i, j]);
                    _printMessage("---------");
                }
            }

            return "Finish";
        }

        string getName(Dictionary<string, int> o, int g, bool del)
        {
            string s = o.Where(x => x.Value == g).FirstOrDefault().Key;
            if (del)
                o.Remove(s);
            return s;
        }
    }
}
