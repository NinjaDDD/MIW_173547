using System.Text;

namespace ZmutowanyDywanik
{
    public class Form1 : Form
    {
        private TextBox textBox1;

        const int iloscBitowLBnP = 3;
        const int wielkoscPopulacji = 11;
        const int iteracje = 20;
        const double prawdopodobienstoMutacji = 0.01;
        const int wielkoscTurnieju = 2;

        Random rand = new Random();

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
            Text = "MIW Zadanie 1";
            ResumeLayout(false);
            PerformLayout();
        }

        private void Program()
        {
            int dlugoscChromosomu = iloscBitowLBnP * 2;
            List<string> populacja = [];
            for (int i = 0; i < wielkoscPopulacji; i++)
            {
                string chromosom = "";
                for (int j = 0; j < dlugoscChromosomu; j++)
                    chromosom += rand.Next(2).ToString();
                populacja.Add(chromosom);
            }
            
            for (int i = 0; i <= iteracje; i++)
            {
                List<(string chromosom, double przystosowanie)> populacjaPrzeliczona = [];
                double najlepszePrzystosowanie = 0;

                foreach (string osobnik in populacja)
                {
                    double przystosowanie = funkcjaPrzystosowania(osobnik);
                    populacjaPrzeliczona.Add((osobnik, przystosowanie));
                    if (przystosowanie > najlepszePrzystosowanie)
                    {
                        najlepszePrzystosowanie = przystosowanie;
                    }
                }

                double sumaPrzystosowania = 0;
                string najlepszyOsobnik = "";
                foreach (var osobnik in populacjaPrzeliczona)
                {
                    sumaPrzystosowania += osobnik.przystosowanie;
                }
                
                double sredniaWartoscPrzystosowania = sumaPrzystosowania / populacjaPrzeliczona.Count;
                
                foreach (var osobnik in populacjaPrzeliczona)
                {
                    if (osobnik.przystosowanie == najlepszePrzystosowanie)
                    {
                        najlepszyOsobnik = osobnik.chromosom;
                        break;
                    }
                }
                
                textBox1.AppendText($"Iteracja {i} - Najlepszy: {najlepszePrzystosowanie:F4}, Średni: {sredniaWartoscPrzystosowania:F4}{Environment.NewLine}");
                if (i == iteracje)
                    break;

                List<string> nowaPopulacja = new List<string>();

                while (nowaPopulacja.Count < wielkoscPopulacji - 1)
                {
                    string zwyciezca = turniej(populacjaPrzeliczona, wielkoscTurnieju);
                    string nowyOsobnik = mutacja(zwyciezca, prawdopodobienstoMutacji);
                    nowaPopulacja.Add(nowyOsobnik);
                }

                nowaPopulacja.Add(najlepszyOsobnik);

                populacja = new List<string>(nowaPopulacja);
            }
        }

        private (double x1, double x2) kodowanieCtmp(string chromosom)
        {
            string x1Bity = chromosom.Substring(0, iloscBitowLBnP);
            string x2Bity = chromosom.Substring(iloscBitowLBnP);

            double max = Math.Pow(2, iloscBitowLBnP) - 1;
            double x1 = Convert.ToInt32(x1Bity, 2) * max;
            double x2 = Convert.ToInt32(x2Bity, 2) * max;

            return (x1, x2);
        }

        private double funkcjaPrzystosowania(string chromosom)
        {
            (double x1, double x2) = kodowanieCtmp(chromosom);

            double f = Math.Sin(0.05 * x1) + Math.Sin(0.05 * x2) +
                       0.4 * Math.Sin(0.15 * x1) * Math.Sin(0.15 * x2);

            return f;
        }

        private string turniej(List<(string chromosom, double funkcjaPrzystosowania)> populacja, int wielkoscTurnieju)
        {
            string najlepszyChromosom = populacja[rand.Next(populacja.Count)].chromosom;
            double najlepszePrzystosowanie = double.MinValue;
            for (int i = 0; i < wielkoscTurnieju; i++)
            {
                var osobnik = populacja[rand.Next(populacja.Count)];
                if (osobnik.funkcjaPrzystosowania > najlepszePrzystosowanie)
                {
                    najlepszyChromosom = osobnik.chromosom;
                    najlepszePrzystosowanie = osobnik.funkcjaPrzystosowania;
                }
            }
            return najlepszyChromosom;
        }

        private string mutacja(string chromosom, double prawdopodobienstwoMutacji)
        {
            char[] bity = chromosom.ToCharArray();
            for (int i = 0; i < bity.Length; i++)
            {
                if (rand.NextDouble() < prawdopodobienstwoMutacji)
                {
                    if (bity[i] == '0')
                        bity[i] = '1';
                    else
                        bity[i] = '0';
                }
            }
            return new string(bity);
        }
    }
}
