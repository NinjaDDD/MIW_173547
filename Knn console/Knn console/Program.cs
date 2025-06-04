class Program
{
    const int liczbaSasiadow = 3;

    static void Main()
    {
        List<double[]> wszystkieDane = WczytajDaneZPliku("iris_data.txt");

        NormalizujDane(wszystkieDane);

        string[] metryki = { "euklidesowa", "manhattan", "chebyshev", "minkowski", "logarytmiczna" };

        foreach (string aktualnaMetryka in metryki)
        {
            int ilePoprawnych = 0;

            for (int i = 0; i < wszystkieDane.Count; i++)
            {
                double[] testowanyWiersz = wszystkieDane[i];
                List<double[]> zbiorTreningowy = new List<double[]>();

                for (int j = 0; j < wszystkieDane.Count; j++)
                {
                    if (i != j)
                        zbiorTreningowy.Add(wszystkieDane[j]);
                }

                int przewidzianaKlasa = KlasyfikujWiersz(testowanyWiersz, zbiorTreningowy, liczbaSasiadow, aktualnaMetryka);
                int prawdziwaKlasa = (int)testowanyWiersz[testowanyWiersz.Length - 1];

                if (przewidzianaKlasa == prawdziwaKlasa)
                    ilePoprawnych++;
            }

            double dokladnosc = (double)ilePoprawnych / wszystkieDane.Count * 100;
            Console.WriteLine($"Metryka: {aktualnaMetryka} | Dokładność: {dokladnosc:F2}%");
        }
    }


    static List<double[]> WczytajDaneZPliku(string nazwaPliku)
    {
        List<double[]> dane = new List<double[]>();
        string[] linie = File.ReadAllLines(nazwaPliku);

        foreach (string linia in linie)
        {
            string[] czesci = linia.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
            double[] wiersz = new double[czesci.Length];

            for (int i = 0; i < czesci.Length; i++)
            {
                string zamienione = czesci[i].Replace('.', ',');
                wiersz[i] = double.Parse(zamienione);
            }

            dane.Add(wiersz);
        }

        return dane;
    }

    static void NormalizujDane(List<double[]> dane)
    {
        int ileCecha = dane[0].Length - 1;
        double[] min = new double[ileCecha];
        double[] max = new double[ileCecha];

        for (int i = 0; i < ileCecha; i++)
        {
            min[i] = double.MaxValue;
            max[i] = double.MinValue;
        }

        foreach (var wiersz in dane)
        {
            for (int i = 0; i < ileCecha; i++)
            {
                if (wiersz[i] < min[i]) min[i] = wiersz[i];
                if (wiersz[i] > max[i]) max[i] = wiersz[i];
            }
        }

        foreach (var wiersz in dane)
        {
            for (int i = 0; i < ileCecha; i++)
                wiersz[i] = (wiersz[i] - min[i]) / (max[i] - min[i]);
        }
    }

    static int KlasyfikujWiersz(double[] test, List<double[]> trening, int k, string metryka)
    {
        List<Tuple<double, int>> odleglosci = new List<Tuple<double, int>>();

        foreach (var wierszTreningowy in trening)
        {
            double odleglosc = PoliczOdleglosc(test, wierszTreningowy, metryka);
            int etykieta = (int)wierszTreningowy[wierszTreningowy.Length - 1];
            odleglosci.Add(Tuple.Create(odleglosc, etykieta));
        }

        // Sortowanie bąbelkowe
        for (int i = 0; i < odleglosci.Count - 1; i++)
        {
            for (int j = 0; j < odleglosci.Count - i - 1; j++)
            {
                if (odleglosci[j].Item1 > odleglosci[j + 1].Item1)
                {
                    var temp = odleglosci[j];
                    odleglosci[j] = odleglosci[j + 1];
                    odleglosci[j + 1] = temp;
                }
            }
        }

        Dictionary<int, int> glosy = new Dictionary<int, int>();

        for (int i = 0; i < k; i++)
        {
            int etykieta = odleglosci[i].Item2;
            if (!glosy.ContainsKey(etykieta))
                glosy[etykieta] = 0;
            glosy[etykieta]++;
        }

        int najwiecejGlosow = -1;
        int przewidywanaKlasa = -1;

        foreach (var para in glosy)
        {
            if (para.Value > najwiecejGlosow)
            {
                najwiecejGlosow = para.Value;
                przewidywanaKlasa = para.Key;
            }
        }

        return przewidywanaKlasa;
    }

    static double PoliczOdleglosc(double[] a, double[] b, string metryka)
    {
        double suma = 0.0;
        int cechy = a.Length - 1;

        if (metryka == "euklidesowa")
        {
            for (int i = 0; i < cechy; i++)
                suma += (a[i] - b[i]) * (a[i] - b[i]);
            return Math.Sqrt(suma);
        }
        else if (metryka == "manhattan")
        {
            for (int i = 0; i < cechy; i++)
                suma += Math.Abs(a[i] - b[i]);
            return suma;
        }
        else if (metryka == "chebyshev")
        {
            double max = 0.0;
            for (int i = 0; i < cechy; i++)
            {
                double roznica = Math.Abs(a[i] - b[i]);
                if (roznica > max) max = roznica;
            }
            return max;
        }
        else if (metryka == "minkowski")
        {
            double p = 3.0;
            for (int i = 0; i < cechy; i++)
                suma += Math.Pow(Math.Abs(a[i] - b[i]), p);
            return Math.Pow(suma, 1.0 / p);
        }
        else if (metryka == "logarytmiczna")
        {
            for (int i = 0; i < cechy; i++)
                suma += Math.Log(1 + Math.Abs(a[i] - b[i]));
            return suma;
        }
        else
        {
            Console.WriteLine("Nieznana metryka!");
            return -1;
        }
    }

}
