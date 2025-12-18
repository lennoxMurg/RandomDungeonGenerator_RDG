using System;
using System.IO;

namespace Projekt
{
    class Program
    {
        static void Main(string[] args)
        {
            
            // Festlegung der Symbole für die Kartenelemente
            const char WAND = '#';
            const char START = 'S';
            const char ENDE = 'E';

            // Modulare Benutzereingabe
            const int BREITE_MINIMUM = 10;
            const int BREITE_MAXIMUM = 50;
            const int HOEHE_MINIMUM = 10;
            const int HOEHE_MAXIMUM = 25;

            int breite = 0;
            int hoehe = 0;

            
            // Wiederholt die Abfrage, bis gültige Werte eingegeben wurden
            do
            {
                try
                {
                    // Aufruf der Methoden zur Breiteneingabe und Höheneingabe
                    breite = breite_eingabe(breite, BREITE_MAXIMUM, BREITE_MINIMUM);
                    hoehe = hoehe_eingeben(hoehe, HOEHE_MAXIMUM, HOEHE_MINIMUM);

                    // Wenn beide Werte erfolgreich gesetzt wurden, Schleife verlassen
                    if (breite != 0 && hoehe != 0)
                    {
                        break;
                    }
                }
                catch (ArgumentException ex)
                {
                    // Gibt Fehlermeldungen aus der Eingabe aus
                    Console.WriteLine(ex.Message);
                }
                catch
                {
                    // Fängt unvorhergesehene Fehler ab (z.B. falsches Format bei der Eingabe)
                    Console.WriteLine("Es ist ein Unerwarteter Fehler aufgetreten!\n");
                }

            }
            while (true);

            Console.Clear();

            
            // Erstellung der Datenstruktur (2D-Array) basierend auf Eingabe
            char[,] dungeonFeld = new char[breite, hoehe];

            // Initialisierung des Zufallsgenerators
            Random zufall = new Random();

            // Das Array wird initial komplett mit dem WAND-Zeichen gefüllt
            InitialisiereDungeon(dungeonFeld, WAND);

            // Zufällige Platzierung von S und E (innerhalb der Spielfeldgrenzen)
            PlatziereStartUndEnde(dungeonFeld, zufall, START, ENDE);

            
            // Zeichnet das Array farbig in die Konsole
            GibDungeonAus(dungeonFeld, breite, hoehe, START, ENDE);

            Console.ReadKey();

            
            // Schreibt das Ergebnis in eine vom Benutzer benannte Datei
            SpeichernInTextdatei(dungeonFeld, breite, hoehe);
        }

        

        
        // Fragt die Breite ab und prüft, ob sie im erlaubten Bereich liegt.
        static int breite_eingabe(int breite, int breite_maximum, int breite_minimum)
        {
            if (breite == 0)
            {
                Console.WriteLine($"Bitte die Breite eingeben! ({breite_minimum} - {breite_maximum})");
                breite = Convert.ToInt32(Console.ReadLine());

                if (breite < breite_minimum || breite > breite_maximum)
                {
                    breite = 0;
                    throw new ArgumentException($"\nDie Breite muss größer als {breite_minimum} und kleiner als {breite_maximum} sein.\n");
                }
            }
            return breite;
        }

        // Fragt die Höhe ab und prüft, ob sie im erlaubten Bereich liegt.
        static int hoehe_eingeben(int hoehe, int hoehe_maximum, int hoehe_minimum)
        {
            Console.WriteLine($"Bitte die Höhe eingeben! ({hoehe_minimum} - {hoehe_maximum})");
            hoehe = Convert.ToInt32(Console.ReadLine());
            if (hoehe < hoehe_minimum || hoehe > hoehe_maximum)
            {
                hoehe = 0;
                throw new ArgumentException($"\nDie Höhe muss größer als {hoehe_minimum} und kleiner als {hoehe_maximum} sein.\n");
            }
            return hoehe;
        }
        
        // Durchläuft das gesamte Array und setzt jedes Feld auf das angegebene Füllzeichen.
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

        // Ermittelt zwei unterschiedliche Zufallspositionen für Start und Ende.
        // Der Rand (Index 0 und Max-1) wird dabei ausgespart.
        static void PlatziereStartUndEnde(char[,] feld, Random zufall, char startZeichen, char endeZeichen)
        {
            int maxZeilen = feld.GetLength(0);
            int maxSpalten = feld.GetLength(1);

            // Startpunkt setzen
            int startZeile = zufall.Next(1, maxZeilen - 1);
            int startSpalte = zufall.Next(1, maxSpalten - 1);
            feld[startZeile, startSpalte] = startZeichen;

            // Endpunkt setzen (mit Prüfung auf Dopplung)
            int endeZeile, endeSpalte;
            do
            {
                endeZeile = zufall.Next(1, maxZeilen - 1);
                endeSpalte = zufall.Next(1, maxSpalten - 1);
            }
            while (endeZeile == startZeile && endeSpalte == startSpalte);

            feld[endeZeile, endeSpalte] = endeZeichen;
        }

        // Gibt das Spielfeld in der Konsole aus. Start/Ende werden farbig hervorgehoben.
        static void GibDungeonAus(char[,] feld, int breite, int hoehe, char START, char ENDE)
        {
            Console.WriteLine("--- ZUFALLS-DUNGEON ---");
            Console.WriteLine();

            // Darstellung der Matrix durch verschachtelte Schleifen
            for (int j = 0; j < hoehe; j++)
            {
                for (int i = 0; i < breite; i++)
                {
                    char aktuellesZeichen = feld[i, j];

                    // Farbwechsel je nach Symbol
                    if (aktuellesZeichen == START)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else if (aktuellesZeichen == ENDE)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else
                    {
                        Console.ResetColor();
                    }

                    Console.Write(aktuellesZeichen);
                    Console.Write(' '); 
                }
                Console.ResetColor();
                Console.WriteLine(); // Zeilenumbruch nach jeder vollständigen Zeile
            }
        }

        // Erstellt eine Textdatei und schreibt das Dungeon-Muster hinein.
        static void SpeichernInTextdatei(char[,] dungeon, int breite, int hoehe)
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