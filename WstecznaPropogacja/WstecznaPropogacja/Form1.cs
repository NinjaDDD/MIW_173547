namespace WstecznaPropogacja
{
    public partial class Form1 : Form
    {
        double Beta = 1.0;
        double współczynnikUczenia = 0.3;
        int maksymalnaLiczbaEpok = 1000;

        double[][] Wejście = {
            new double[] { 0, 0 },
            new double[] { 0, 1 },
            new double[] { 1, 0 },
            new double[] { 1, 1 }
        };

        double[] Wyjscie = { 0, 1, 1, 0 };

        double[] wagiUkryte1 = new double[2];
        double[] wagiUkryte2 = new double[2];
        double[] wagiWyjściowe = new double[2];

        double poprawkaUkryta1, poprawkaUkryta2, poprawkaWyjściowy;
        
        TextBox textBox1;

        public Form1()
        {
            InitializeComponent();
            textBox1 = new TextBox();
            textBox1.Multiline = true;
            textBox1.Width = 500;
            textBox1.Height = 300;
            textBox1.Location = new System.Drawing.Point(10, 10);
            Controls.Add(textBox1);
            UczSieć();
        }

        double FunkcjaSigmoidalna(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-Beta * x));
        }

        double PochodnaSigmoidalna(double x)
        {
            double sx = FunkcjaSigmoidalna(x);
            return Beta * sx * (1 - sx);
        }

        void UczSieć()
        {
            Random losowanie = new Random();
            for (int i = 0; i < 2; i++)
            {
                wagiUkryte1[i] = losowanie.NextDouble() * 10 - 5;
                wagiUkryte2[i] = losowanie.NextDouble() * 10 - 5;
                wagiWyjściowe[i] = losowanie.NextDouble() * 10 - 5;
            }
            poprawkaUkryta1 = losowanie.NextDouble() * 10 - 5;
            poprawkaUkryta2 = losowanie.NextDouble() * 10 - 5;
            poprawkaWyjściowy = losowanie.NextDouble() * 10 - 5;
            int epoka = 0;

            while (epoka < maksymalnaLiczbaEpok)
            {
                for (int i = 0; i < Wejście.Length; i++)
                {
                    double[] x = Wejście[i];
                    double d = Wyjscie[i];
                    
                    double sumaH1 = wagiUkryte1[0] * x[0] + wagiUkryte1[1] * x[1] + poprawkaUkryta1;
                    double sumaH2 = wagiUkryte2[0] * x[0] + wagiUkryte2[1] * x[1] + poprawkaUkryta2;

                    double h1 = FunkcjaSigmoidalna(sumaH1);
                    double h2 = FunkcjaSigmoidalna(sumaH2);
                    double sumaY = wagiWyjściowe[0] * h1 + wagiWyjściowe[1] * h2 + poprawkaWyjściowy;
                    double y = FunkcjaSigmoidalna(sumaY);
                    double błąd = d - y;
                    
                    double deltaY = błąd * PochodnaSigmoidalna(sumaY);
                    double deltaH1 = deltaY * wagiWyjściowe[0] * PochodnaSigmoidalna(sumaH1);
                    double deltaH2 = deltaY * wagiWyjściowe[1] * PochodnaSigmoidalna(sumaH2);
                    
                    wagiWyjściowe[0] += współczynnikUczenia * deltaY * h1;
                    wagiWyjściowe[1] += współczynnikUczenia * deltaY * h2;
                    poprawkaWyjściowy += współczynnikUczenia * deltaY;
                    
                    for (int j = 0; j < 2; j++)
                    {
                        wagiUkryte1[j] += współczynnikUczenia * deltaH1 * x[j];
                        wagiUkryte2[j] += współczynnikUczenia * deltaH2 * x[j];
                    }

                    poprawkaUkryta1 += współczynnikUczenia * deltaH1;
                    poprawkaUkryta2 += współczynnikUczenia * deltaH2;
                }
                epoka++;
            }
            textBox1.AppendText($"Uczenie zakończone po {epoka} epokach.{Environment.NewLine}");
            textBox1.AppendText($"Wynik:{Environment.NewLine}");
            for (int i = 0; i < Wejście.Length; i++)
            {
                double[] x = Wejście[i];

                double h1 = FunkcjaSigmoidalna(wagiUkryte1[0] * x[0] + wagiUkryte1[1] * x[1] + poprawkaUkryta1);
                double h2 = FunkcjaSigmoidalna(wagiUkryte2[0] * x[0] + wagiUkryte2[1] * x[1] + poprawkaUkryta2);

                double y = FunkcjaSigmoidalna(wagiWyjściowe[0] * h1 + wagiWyjściowe[1] * h2 + poprawkaWyjściowy);

                double błąd = Math.Abs(Wyjscie[i] - y);
                if (błąd > 0.3)
                    textBox1.AppendText($"Siec nie nauczyla neuronu {x[0]} {x[1]} błąd przekracza 0,3{Environment.NewLine}");
                textBox1.AppendText($"{x[0]} {x[1]} => {Math.Round(y, 4)} (Wyjście: {Wyjscie[i]}) | Różnica: {Math.Round(błąd, 4)}{Environment.NewLine}");
            }
        }
    }
}
