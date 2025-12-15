using System;

namespace Projekt
{
    class Program
    {
        static void Main(string[] args)
        {

            const char WAND = '#';
            const char START = 'S';
            const char ENDE = 'E';

            const int BREITE_MINIMUM = 10;
            const int BREITE_MAXIMUM = 50;
            const int HOEHE_MINIMUM = 10;               //NOCH GROßSCHREIBEN
            const int HOEHE_MAXIMUM = 25;




            int breite = 0;
            int hoehe = 0;

            do
            {
                try
                {

                    breite = breite_eingabe(breite, BREITE_MAXIMUM, BREITE_MINIMUM);

                    hoehe = hoehe_eingeben(hoehe, HOEHE_MAXIMUM, HOEHE_MINIMUM);


                    if (breite != 0 && hoehe != 0)
                    {
                        break;
                    }

                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch
                {
                    Console.WriteLine("Es ist ein Unerwarteter Fehler aufgetreten!\n");
                }

            }
            while (true);

            Console.Clear();


            // F.2.1 Datenstruktur: Zweidimensionales char-Array für die Dungeon-Karte
            char[,] dungeonFeld = new char[breite, hoehe];

            // Für die zufällige Platzierung von Start und Ende
            Random zufall = new Random();

            // --- F.2.2 Initialisierung: Array vollständig mit WALL füllen ---
            InitialisiereDungeon(dungeonFeld, WAND);

            // --- F.2.3 Start- und Endpunkt: Zufällige, nicht-randständige Platzierung ---
            PlatziereStartUndEnde(dungeonFeld, zufall, START, ENDE);

            // --- Ausgabe: Das gesamte Dungeon-Array ausgeben ---
            GibDungeonAus(dungeonFeld);

            Console.ReadKey();

        }

        static int breite_eingabe(int breite, int breite_maximum, int breite_minimum)       // Eingabe Methode für breite
        {
            if (breite == 0)
            {
                Console.WriteLine("Bitte die Breite Eingeben!");
                breite = Convert.ToInt32(Console.ReadLine());

                if (breite < breite_minimum || breite > breite_maximum)
                {
                    breite = 0;
                    throw new ArgumentException("\nDie Breite muss größer als 10 und kleiner als 50 sein.\n");
                }

            }
            return breite;
        }

        static int hoehe_eingeben(int hoehe, int hoehe_maximum, int hoehe_minimum)      // Eingabe Methode für Höhe
        {
            Console.WriteLine("Bitte die Höhe Eingeben!");
            hoehe = Convert.ToInt32(Console.ReadLine());
            if (hoehe < hoehe_minimum || hoehe > hoehe_maximum)
            {
                hoehe = 0;
                throw new ArgumentException("\nDie Höhe muss größer als 10 und kleiner als 25 sein.\n");
            }
            return hoehe;
        }

        static void InitialisiereDungeon(char[,] feld, char fuellZeichen)
        {
            int zeilen = feld.GetLength(0);
            int spalten = feld.GetLength(1);

            for (int i = 0; i < zeilen; i++)
            {
                for (int j = 0; j < spalten; j++)
                {
                    feld[i, j] = fuellZeichen;
                }
            }
        }

        static void PlatziereStartUndEnde(char[,] feld, Random zufall, char startZeichen, char endeZeichen)
        {
            int maxZeilen = feld.GetLength(0);
            int maxSpalten = feld.GetLength(1);

            // Zufallsbereich für nicht-randständige Positionen: [1, max-2]
            // Zeile: zufall.Next(1, maxZeilen - 1)
            // Spalte: zufall.Next(1, maxSpalten - 1)

            // Startpunkt (S) platzieren
            int startZeile = zufall.Next(1, maxZeilen - 1);
            int startSpalte = zufall.Next(1, maxSpalten - 1);
            feld[startZeile, startSpalte] = startZeichen;

            // Endpunkt (E) platzieren, muss ein anderer Ort sein
            int endeZeile, endeSpalte;
            do
            {
                // Generiere neue zufällige Positionen
                endeZeile = zufall.Next(1, maxZeilen - 1);
                endeSpalte = zufall.Next(1, maxSpalten - 1);
            }
            // Wiederhole, falls Endpunkt gleich Startpunkt ist
            while (endeZeile == startZeile && endeSpalte == startSpalte);

            feld[endeZeile, endeSpalte] = endeZeichen;
        }

        static void GibDungeonAus(char[,] feld)
        {
            Console.WriteLine("--- ZUFALLS-DUNGEON ---");
            Console.WriteLine();

            int zeilen = feld.GetLength(0);
            int spalten = feld.GetLength(1);

            for (int i = 0; i < zeilen; i++)
            {
                for (int j = 0; j < spalten; j++)
                {
                    // Gib das Zeichen an der aktuellen Position aus
                    Console.Write(feld[i, j]);
                    // Optional: Fügen Sie ein Leerzeichen für eine bessere Lesbarkeit hinzu
                    Console.Write(' ');
                }
                // Nach Abschluss einer Zeile: Neue Zeile einfügen
                Console.WriteLine();
            }
        }

        
        static void SpeichernInTextdatei(char[,] dungeon, int breite, int hoehe)    // Dungeon in eine Textdatei speichern
        {
        Console.Write("Geben Sie den Dateinamen ein (mit .txt): ");
        string dateiname = Console.ReadLine();

        try
        {
            using (StreamWriter sw = new StreamWriter(dateiname))
            {
                for (int y = 0; y < hoehe; y++)
                {
                    for (int x = 0; x < breite; x++)
                    {
                        sw.Write(dungeon[y, x]);
                    }
                    sw.WriteLine();
                }
            }
            Console.WriteLine($"Dungeon erfolgreich in '{dateiname}' gespeichert.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Speichern der Datei: {ex.Message}");
        }
        }
}

    }
}

