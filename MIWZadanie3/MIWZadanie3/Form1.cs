using System.Text;

namespace MIWZadanie3
{
    public class Form1 : Form
    {
        private TextBox textBox1;

        const int iloscBitowLBnP = 8;
        const int liczbaParametrow = 9;
        const int wielkoscPopulacji = 13;
        const int iteracje = 100;
        const int wielkoscTurnieju = 3;

        Random rand = new Random();

        List<(double x1, double x2, double expected)> XOR = new()
        {
            (0, 0, 0),
            (0, 1, 1),
            (1, 0, 1),
            (1, 1, 0)
        };

        public Form1()
        {
            InitializeComponent();
            Program();
        }

        private void InitializeComponent()
        {
            textBox1 = new TextBox();
            SuspendLayout();

            textBox1.Multiline = true;
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.Dock = DockStyle.Fill;
            textBox1.ReadOnly = true;

            ClientSize = new Size(500, 500);
            Controls.Add(textBox1);
            Text = "MIW Zadanie 3";
            ResumeLayout(false);
            PerformLayout();
        }

        private void Program()
        {
            List<string> populacja = [];
            for (int i = 0; i < wielkoscPopulacji; i++)
            {
                string chromosom = "";
                for (int j = 0; j < iloscBitowLBnP * liczbaParametrow; j++)
                    chromosom += rand.Next(2).ToString();
                populacja.Add(chromosom);
            }

            for (int i = 0; i < iteracje; i++)
            {
                List<(string chromosom, double przystosowanie)> populacjaPrzeliczona = [];
                double najlepszePrzystosowanie = 99999;
                string najlepszyOsobnik = "";

                foreach (var osobnik in populacja)
                {
                    double przystosowanie = funkcjaPrzystosowania(osobnik);
                    populacjaPrzeliczona.Add((osobnik, przystosowanie));

                    if (przystosowanie < najlepszePrzystosowanie)
                    {
                        najlepszePrzystosowanie = przystosowanie;
                        najlepszyOsobnik = osobnik;
                    }
                }

                double sumaPrzystosowania = 0.0;
                foreach (var osobnik in populacjaPrzeliczona)
                {
                    sumaPrzystosowania += osobnik.przystosowanie;
                }

                double sredniaPrzystosowania = sumaPrzystosowania / populacjaPrzeliczona.Count;
                textBox1.AppendText($"Iteracja {i + 1}: Najlepsze = {najlepszePrzystosowanie:F4}, Średnie = {sredniaPrzystosowania:F4}{Environment.NewLine}");

                List<string> nowaPopulacja = new();
                string tymczasowyOsobnik1 = "";
                string tymczasowyOsobnik2 = "";
                for (int j = 0; j < populacja.Count-1; j++)
                {
                    var osobnik = turniej(populacjaPrzeliczona);
                    nowaPopulacja.Add(osobnik);
                }
                (tymczasowyOsobnik1, tymczasowyOsobnik2) = krzyzowanie(nowaPopulacja[0], nowaPopulacja[1]);
                nowaPopulacja[0] = tymczasowyOsobnik1;
                nowaPopulacja[1] = tymczasowyOsobnik2;
                
                (tymczasowyOsobnik1, tymczasowyOsobnik2) = krzyzowanie(nowaPopulacja[2], nowaPopulacja[3]);
                nowaPopulacja[2] = tymczasowyOsobnik1;
                nowaPopulacja[3] = tymczasowyOsobnik2;
                
                (tymczasowyOsobnik1, tymczasowyOsobnik2) = krzyzowanie(nowaPopulacja[8], nowaPopulacja[9]);
                nowaPopulacja[8] = tymczasowyOsobnik1;
                nowaPopulacja[9] = tymczasowyOsobnik2;
                
                (tymczasowyOsobnik1, tymczasowyOsobnik2) = krzyzowanie(nowaPopulacja[10], nowaPopulacja[11]);
                nowaPopulacja[10] = tymczasowyOsobnik1;
                nowaPopulacja[11] = tymczasowyOsobnik2;
                for (int j = 4; j < populacja.Count-1; j++)
                {
                    var dziecko = mutacja(nowaPopulacja[j]);
                    nowaPopulacja[j] = dziecko;
                }

                nowaPopulacja.Add(najlepszyOsobnik);
                populacja = new(nowaPopulacja);
            }
        }

        private double funkcjaPrzystosowania(string chromosom)
        {
            double[] wagi = dekodowanieChromosomu(chromosom);
            double blad = 0;

            foreach (var (x1, x2, expected) in XOR)
            {
                double wynik = uruchomSiec(x1, x2, wagi);
                blad += Math.Pow(expected - wynik, 2);
            }

            return blad;
        }

        private double[] dekodowanieChromosomu(string chromosom)
        {
            double[] wagi = new double[liczbaParametrow];

            for (int i = 0; i < liczbaParametrow; i++)
            {
                string bity = chromosom.Substring(i * iloscBitowLBnP, iloscBitowLBnP);
                int liczba = Convert.ToInt32(bity, 2);
                wagi[i] = -10.0 + (20.0 * liczba / (Math.Pow(2, iloscBitowLBnP) - 1));
            }

            return wagi;
        }

        private double uruchomSiec(double x1, double x2, double[] w)
        {
            double bias = 1.0;
            
            double h1 = sigmoid(w[0] * x1 + w[1] * x2 + w[2] * bias);
            double h2 = sigmoid(w[3] * x1 + w[4] * x2 + w[5] * bias);
            
            double output = sigmoid(w[6] * h1 + w[7] * h2 + w[8] * bias);
            return output;
        }

        private double sigmoid(double x) => 1.0 / (1.0 + Math.Exp(-x));

        private string turniej(List<(string chromosom, double przystosowanie)> populacjaPrzeliczona)
        {
            string najlepszy = "";
            double najlepszePrzystosowanie = double.MaxValue;

            for (int i = 0; i < wielkoscTurnieju; i++)
            {
                int index = rand.Next(populacjaPrzeliczona.Count);
                string chromosom = populacjaPrzeliczona[index].chromosom;
                double przystosowanie = populacjaPrzeliczona[index].przystosowanie;
                if (przystosowanie < najlepszePrzystosowanie)
                {
                    najlepszePrzystosowanie = przystosowanie;
                    najlepszy = chromosom;
                }
            }
            return najlepszy;
        }

        private (string dziecko1, string dziecko2) krzyzowanie(string rodzic1, string rodzic2)
        {
            int punktKrzyzowania = rand.Next(1, rodzic1.Length-2);
            string dziecko1 = "";
            string dziecko2 = "";

            for (int i = 0; i < rodzic1.Length; i++)
            {
                if (i <= punktKrzyzowania)
                {
                    dziecko1 += rodzic1[i];
                    dziecko2 += rodzic2[i];
                }
                else
                {
                    dziecko1 += rodzic2[i];
                    dziecko2 += rodzic1[i];
                }
            }

            return (dziecko1, dziecko2);
        }

        private string mutacja(string chromosom)
        {
            char[] bity = chromosom.ToCharArray();
            int indexBitu = rand.Next(chromosom.Length);
            for (int i = indexBitu; i < chromosom.Length; i++)
            {
                if (bity[i] == '0')
                    bity[i] = '1';
                else
                    bity[i] = '0';
            }
            return new string(bity);
        }
    }
}
