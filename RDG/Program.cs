using System;

namespace Projekt
{
    class Program
    {
        static void Main(string[] args)
        {
            const int breite_minimum = 10;
            const int breite_maximum = 50;

            const int höhe_minimum = 10;
            const int höhe_maximum = 25;

            int breite = 0;
            int hoehe = 0;
            const char WALL = '#';
            const char START = 'S';
            const char END = 'E';

            do
            {
                try
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


                    Console.WriteLine("Bitte die Höhe Eingeben!");
                    hoehe = Convert.ToInt32(Console.ReadLine());

                    if (hoehe < höhe_minimum || hoehe > höhe_maximum)
                    {
                        hoehe = 0;
                        throw new ArgumentException("\nDie Höhe muss größer als 10 und kleiner als 25 sein.\n");
                    }
                    


                    if(breite != 0 && hoehe != 0)
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



            // F.2.1 Datenstruktur: Zweidimensionales char-Array für die Dungeon-Karte
            char[,] dungeonFeld = new char[SIZE, SIZE];

            // Für die zufällige Platzierung von Start und Ende
            Random zufall = new Random();

            // --- F.2.2 Initialisierung: Array vollständig mit WALL füllen ---
            InitialisiereDungeon(dungeonFeld, WALL);

            // --- F.2.3 Start- und Endpunkt: Zufällige, nicht-randständige Platzierung ---
            PlatziereStartUndEnde(dungeonFeld, zufall, START, END);

            // --- Ausgabe: Das gesamte Dungeon-Array ausgeben ---
            GibDungeonAus(dungeonFeld);

            Console.ReadKey();
        }

        /// <summary>
        /// Füllt das gesamte Dungeon-Array mit einem bestimmten Zeichen (z.B. der Wand).
        /// </summary>
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

        /// <summary>
        /// Platziert Start (S) und Ende (E) an zufälligen, nicht-randständigen Positionen.
        /// </summary>
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

        /// <summary>
        /// Gibt das gesamte Dungeon-Feld zeilenweise in der Konsole aus.
        /// </summary>
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
    }
}
        
        
        
        
        }


    }
}