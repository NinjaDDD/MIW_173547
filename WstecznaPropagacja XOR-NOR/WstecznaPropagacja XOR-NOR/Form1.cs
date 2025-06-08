namespace WstecznaPropagacja_XOR_NOR
{
    public partial class Form1 : Form
    {
        double Beta = 1;
        double współczynnikUczenia = 0.3;
        int maksEpok = 10000;
        double prógBłędu = 0.4;

        double[][] wejścia = new double[][]
        {
            new double[] { 0, 0 },
            new double[] { 0, 1 },
            new double[] { 1, 0 },
            new double[] { 1, 1 }
        };

        double[][] wyjścia = new double[][]
        {
            new double[] { 0, 1 },
            new double[] { 1, 0 },
            new double[] { 1, 0 },
            new double[] { 0, 0 }
        };

        double[,] wagi12 = new double[2, 2];
        double[,] wagi23 = new double[2, 2];
        double[,] wagi34 = new double[2, 2];

        double[] bias2 = new double[2];
        double[] bias3 = new double[2];
        double[] bias4 = new double[2];

        TextBox textBox1;

        public Form1()
        {
            InitializeComponent();
            textBox1 = new TextBox
            {
                Multiline = true,
                Width = 600,
                Height = 400,
                Location = new System.Drawing.Point(10, 10),
                ScrollBars = ScrollBars.Vertical
            };
            Controls.Add(textBox1);
            UczSieć();
        }

        double Sigmoid(double x) => 1.0 / (1.0 + Math.Exp(-Beta * x));

        void UczSieć()
        {
            Random rand = new Random();

            void InitWagi(double[,] wagi)
            {
                for (int i = 0; i < wagi.GetLength(0); i++)
                    for (int j = 0; j < wagi.GetLength(1); j++)
                        wagi[i, j] = rand.NextDouble() * 2 - 1;
            }

            InitWagi(wagi12);
            InitWagi(wagi23);
            InitWagi(wagi34);

            for (int i = 0; i < 2; i++)
            {
                bias2[i] = rand.NextDouble() * 2 - 1;
                bias3[i] = rand.NextDouble() * 2 - 1;
                bias4[i] = rand.NextDouble() * 2 - 1;
            }

            for (int epoka = 0; epoka < maksEpok; epoka++)
            {
                for (int p = 0; p < wejścia.Length; p++)
                {
                    double[] x = wejścia[p];
                    double[] d = wyjścia[p];

                    double[] h1 = new double[2];
                    double[] h2 = new double[2];
                    double[] y = new double[2];

                    for (int i = 0; i < 2; i++)
                        h1[i] = Sigmoid(x[0] * wagi12[0, i] + x[1] * wagi12[1, i] + bias2[i]);

                    for (int i = 0; i < 2; i++)
                        h2[i] = Sigmoid(h1[0] * wagi23[0, i] + h1[1] * wagi23[1, i] + bias3[i]);

                    for (int i = 0; i < 2; i++)
                        y[i] = Sigmoid(h2[0] * wagi34[0, i] + h2[1] * wagi34[1, i] + bias4[i]);

                    double[] deltaY = new double[2];
                    double[] deltaH2 = new double[2];
                    double[] deltaH1 = new double[2];

                    for (int i = 0; i < 2; i++)
                        deltaY[i] = (d[i] - y[i]) * y[i] * (1 - y[i]);

                    for (int i = 0; i < 2; i++)
                    {
                        double sum = 0;
                        for (int j = 0; j < 2; j++)
                            sum += deltaY[j] * wagi34[i, j];
                        deltaH2[i] = sum * h2[i] * (1 - h2[i]);
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        double sum = 0;
                        for (int j = 0; j < 2; j++)
                            sum += deltaH2[j] * wagi23[i, j];
                        deltaH1[i] = sum * h1[i] * (1 - h1[i]);
                    }

                    for (int i = 0; i < 2; i++)
                        for (int j = 0; j < 2; j++)
                            wagi34[i, j] += współczynnikUczenia * deltaY[j] * h2[i];

                    for (int i = 0; i < 2; i++)
                        for (int j = 0; j < 2; j++)
                            wagi23[i, j] += współczynnikUczenia * deltaH2[j] * h1[i];

                    for (int i = 0; i < 2; i++)
                        for (int j = 0; j < 2; j++)
                            wagi12[i, j] += współczynnikUczenia * deltaH1[j] * x[i];

                    for (int i = 0; i < 2; i++)
                    {
                        bias4[i] += współczynnikUczenia * deltaY[i];
                        bias3[i] += współczynnikUczenia * deltaH2[i];
                        bias2[i] += współczynnikUczenia * deltaH1[i];
                    }
                }
            }

            textBox1.AppendText("=== Testowanie sieci po uczeniu ===\r\n");

            for (int p = 0; p < wejścia.Length; p++)
            {
                double[] x = wejścia[p];
                double[] d = wyjścia[p];

                double[] h1 = new double[2];
                double[] h2 = new double[2];
                double[] y = new double[2];

                for (int i = 0; i < 2; i++)
                    h1[i] = Sigmoid(x[0] * wagi12[0, i] + x[1] * wagi12[1, i] + bias2[i]);

                for (int i = 0; i < 2; i++)
                    h2[i] = Sigmoid(h1[0] * wagi23[0, i] + h1[1] * wagi23[1, i] + bias3[i]);

                for (int i = 0; i < 2; i++)
                    y[i] = Sigmoid(h2[0] * wagi34[0, i] + h2[1] * wagi34[1, i] + bias4[i]);

                textBox1.AppendText($"{x[0]} {x[1]} => wy1: {Math.Round(y[0], 3)} wy2: {Math.Round(y[1], 3)} (oczekiwane: {d[0]} {d[1]})\r\n");

                if (Math.Abs(d[0] - y[0]) > prógBłędu || Math.Abs(d[1] - y[1]) > prógBłędu)
                    textBox1.AppendText($"Błąd przekracza {prógBłędu} dla próbki {x[0]} {x[1]}\r\n");
            }
        }
    }
}
