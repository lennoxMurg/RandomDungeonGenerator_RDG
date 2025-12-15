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
            const int HOEHE_MINIMUM = 10;
            const int HOEHE_MAXIMUM = 25;


            int breite = 0;
            int hoehe = 0;

            do
            {
                try
                {
                    // Breiten- und Höhenabfrage
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
                catch (FormatException)
                {
                    Console.WriteLine("\nFehler: Bitte geben Sie eine gültige ganze Zahl ein!\n");
                    // Zurücksetzen der Werte bei Formatfehler, um die Schleife zu wiederholen
                    breite = 0;
                    hoehe = 0;
                }
                catch
                {
                    Console.WriteLine("Es ist ein Unerwarteter Fehler aufgetreten!\n");
                }

            }
            while (true);

            // Datenstruktur: Zweidimensionales char-Array für die Dungeon-Karte
            char[,] dungeonFeld = new char[breite, hoehe];

            // Für die zufällige Platzierung von Start und Ende
            Random zufall = new Random();

            // Initialisierung: Array vollständig mit WALL füllen
            InitialisiereDungeon(dungeonFeld, WAND);

            // Start- und Endpunkt: Zufällige, nicht-randständige Platzierung
            PlatziereStartUndEnde(dungeonFeld, zufall, START, ENDE);

            // Ausgabe: Das gesamte Dungeon-Array ausgeben (mit Farbanpassung)
            GibDungeonAus(dungeonFeld, START, ENDE); // START und ENDE werden für die Farbsteuerung übergeben

            Console.ReadKey();

        }

        static int breite_eingabe(int breite, int breite_maximum, int breite_minimum)       // Eingabe Methode für breite
        {
            // Wenn breite noch nicht gesetzt wurde (Wert 0), Eingabe fordern
            if (breite == 0)
            {
                Console.WriteLine("Bitte die Breite Eingeben!");
                breite = Convert.ToInt32(Console.ReadLine());

                if (breite < breite_minimum || breite > breite_maximum)
                {
                    breite = 0;
                    throw new ArgumentException($"\nDie Breite muss größer als {breite_minimum} und kleiner als {breite_maximum} sein.\n");
                }
            }
            return breite;
        }

        static int hoehe_eingeben(int hoehe, int hoehe_maximum, int hoehe_minimum)      // Eingabe Methode für Höhe
        {
            // Abfrage immer durchführen, da sie erst nach erfolgreicher Breiten-Eingabe erreicht wird.
            Console.WriteLine("Bitte die Höhe Eingeben!");
            hoehe = Convert.ToInt32(Console.ReadLine());

            if (hoehe < hoehe_minimum || hoehe > hoehe_maximum)
            {
                hoehe = 0;
                throw new ArgumentException($"\nDie Höhe muss größer als {hoehe_minimum} und kleiner als {hoehe_maximum} sein.\n");
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
          
            // Wir verwenden die Indizes wie sie deklariert wurden (i=Breite, j=Höhe)
            int maxBreite = feld.GetLength(0); // I
            int maxHoehe = feld.GetLength(1);  // J

            // Zufallsbereich für nicht-randständige Positionen: [1, max-1] (exklusiv max)
            // Startpunkt (S) platzieren
            int startBreite = zufall.Next(1, maxBreite - 1);
            int startHoehe = zufall.Next(1, maxHoehe - 1);
            feld[startBreite, startHoehe] = startZeichen;

            // Endpunkt (E) platzieren, muss ein anderer Ort sein
            int endeBreite, endeHoehe;
            do
            {
                // Generiere neue zufällige Positionen
                endeBreite = zufall.Next(1, maxBreite - 1);
                endeHoehe = zufall.Next(1, maxHoehe - 1);
            }
            // Wiederhole, falls Endpunkt gleich Startpunkt ist
            while (endeBreite == startBreite && endeHoehe == startHoehe);

            feld[endeBreite, endeHoehe] = endeZeichen;
        }

        static void GibDungeonAus(char[,] feld, char startZeichen, char endeZeichen)
        {
            Console.WriteLine("\n--- ZUFALLS-DUNGEON ---");
            Console.WriteLine();

         
            

            int maxBreite = feld.GetLength(0); // Erste Dimension
            int maxHoehe = feld.GetLength(1);  // Zweite Dimension

            // Äußere Schleife: Höhe (Zeilen)
            for (int j = 0; j < maxHoehe; j++)
            {
                // Innere Schleife: Breite (Spalten)
                for (int i = 0; i < maxBreite; i++)
                {
                    char aktuellesZeichen = feld[i, j];

                    // Prüfen, ob das Zeichen Start oder Ende ist
                    if (aktuellesZeichen == startZeichen)
                    {
                        Console.ForegroundColor = ConsoleColor.Green; // Grün für Start
                    }
                    else if (aktuellesZeichen == endeZeichen)
                    {
                        Console.ForegroundColor = ConsoleColor.Red; // Rot für Ende 
                    }
                    else
                    {
                        Console.ResetColor(); // Standardfarbe für Wände
                    }

                    // Gib das Zeichen an der aktuellen Position aus
                    Console.Write(aktuellesZeichen);

                    Console.Write(' '); // Abstand zwischen den Zeichen
                }

                // Am Ende jeder Zeile: Farbe zurücksetzen und neue Zeile einfügen
                Console.ResetColor();
                Console.WriteLine();
            }
        }
    }
}