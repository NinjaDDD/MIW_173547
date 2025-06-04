namespace MIWZadanie2
{
    public class Form1 : Form
    {
        private TextBox textBox1;

        const int iloscBitowLBnP = 4;
        const int liczbaParametrow = 3;
        const int wielkoscPopulacji = 13;
        const int iteracje = 100;
        const int wielkoscTurnieju = 3;

        Random rand = new Random();
        
        List<(double x, double y)> probki = new List<(double, double)>
        {
            (-1.0, 0.59554),
            (-0.8, 0.58813),
            (-0.6, 0.64181),
            (-0.4, 0.68587),
            (-0.2, 0.44783),
            (0.0, 0.40836),
            (0.2, 0.38241),
            (0.4, -0.05933),
            (0.6, -0.12478),
            (0.8, -0.36847),
            (1.0, -0.39935),
            (1.2, -0.50881),
            (1.4, -0.63435),
            (1.6, -0.59979),
            (1.8, -0.64107),
            (2.0, -0.51808),
            (2.2, -0.38127),
            (2.4, -0.12349),
            (2.6, -0.09624),
            (2.8, 0.27893),
            (3.0, 0.48965),
            (3.2, 0.33089),
            (3.4, 0.70615),
            (3.6, 0.53342),
            (3.8, 0.43321),
            (4.0, 0.64790),
            (4.2, 0.48834),
            (4.4, 0.18440),
            (4.6, -0.02389),
            (4.8, -0.10261),
            (5.0, -0.33594),
            (5.2, -0.35101),
            (5.4, -0.62027),
            (5.6, -0.55719),
            (5.8, -0.66377),
            (6.0, -0.62740)
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
            Text = "MIW Zadanie 2";
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

                double sumaPrzystosowania = 0;
                foreach (var osobnik in populacjaPrzeliczona)
                {
                    sumaPrzystosowania += osobnik.przystosowanie;
                }
                double sredniaPrzystosowania = sumaPrzystosowania / populacjaPrzeliczona.Count;

                textBox1.AppendText($"Iteracja {i + 1} - Najlepsze przystosowanie: {najlepszePrzystosowanie:F4}, Średnie przystosowanie: {sredniaPrzystosowania:F4}{Environment.NewLine}");
                
                List<string> nowaPopulacja = new List<string>();
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

                populacja = new List<string>(nowaPopulacja);
            }
        }
        
        private double funkcjaPrzystosowania(string chromosom)
        {
            (double pa, double pb, double pc) = kodowanieChromosomu(chromosom);
            double błąd = 0;

            foreach (var probka in probki)
            {
                double x = probka.x;
                double y = probka.y;
                double f = pa * Math.Sin(pb * x + pc);
                błąd += Math.Pow(y - f, 2);
            }

            return błąd;
        }

        private (double, double, double) kodowanieChromosomu(string chromosom)
        {
            string paBity = chromosom.Substring(0, iloscBitowLBnP);
            string pbBity = chromosom.Substring(iloscBitowLBnP, iloscBitowLBnP);
            string pcBity = chromosom.Substring(2 * iloscBitowLBnP, iloscBitowLBnP);
            
            double pa = Convert.ToInt32(paBity, 2) * 3 / (Math.Pow(2, iloscBitowLBnP) - 1);
            double pb = Convert.ToInt32(pbBity, 2) * 3 / (Math.Pow(2, iloscBitowLBnP) - 1);
            double pc = Convert.ToInt32(pcBity, 2) * 3 / (Math.Pow(2, iloscBitowLBnP) - 1);

            return (pa, pb, pc);
        }

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