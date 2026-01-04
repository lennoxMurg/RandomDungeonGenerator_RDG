using System;
using System.Data;
using System.IO;

namespace Projekt
{
    class Program
    {
        //  Public variablen für die Konstanten

        // Mindestabstand zwischen Start- und Endpunkt
        public const int START_END_ABSTAND = 4;

        // Festlegung der Symbole für die Kartenelemente
        public const char WAND_SYMBOL = '#';
        public const char BODEN_SYMBOL = '.';
        public const char START_SYMBOL = 'S';
        public const char END_SYMBOL = 'E';

        // Modulare Benutzereingabe
        public const int BREITE_MINIMUM = 10;
        public const int BREITE_MAXIMUM = 50;
        public const int HOEHE_MINIMUM = 10;
        public const int HOEHE_MAXIMUM = 25;


        static void Main(string[] args)
        {
            // Initialisierung des Zufallsgenerators
            Random zufall = new Random();

            int breite = 0, hoehe = 0;
            string aktuelle_eingabe;

            // Wiederholt die Abfrage, bis gültige Werte eingegeben wurden
            do
            {
                try
                {
                    // Aufruf der Methode zur Breiten- und Höheneingabe

                    aktuelle_eingabe = "breite";
                    breite = eingabe_dungeon_groesse(breite, hoehe, aktuelle_eingabe);
                    aktuelle_eingabe = "hoehe";
                    hoehe = eingabe_dungeon_groesse(hoehe, breite, aktuelle_eingabe);

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

            // Das Array wird initial komplett mit dem WAND-Zeichen gefüllt
            InitialisiereDungeon(dungeonFeld);

            // Zufällige Platzierung von S und E (innerhalb der Spielfeldgrenzen)
            PlatziereStartUndEnde(dungeonFeld, zufall, breite, hoehe);

            pfadgenerierung(dungeonFeld);

            // Zeichnet das Array farbig in die Konsole
            GibDungeonAus(dungeonFeld, breite, hoehe);



            Console.ReadKey();

            // Schreibt das Ergebnis in eine vom Benutzer benannte Datei    Sorgt noch für Probleme beim Testen/ausführen
            //SpeichernInTextdatei(dungeonFeld, breite, hoehe);
        }


        // Eingabe methode für breite und höhe des dungeons
        static int eingabe_dungeon_groesse(int dungeon_groeße, int andere_groesse, string aktuelle_eingabe)
        {
            int eingabe = dungeon_groeße;

            if (aktuelle_eingabe == "breite")
            {
                if (eingabe == 0)
                {
                    Console.WriteLine($"Bitte die Breite eingeben! ({BREITE_MINIMUM} - {BREITE_MAXIMUM})");
                    eingabe = Convert.ToInt32(Console.ReadLine());

                    if (eingabe < BREITE_MINIMUM || eingabe > BREITE_MAXIMUM)
                    {
                        eingabe = 0;
                        throw new ArgumentException($"\nDie Breite muss größer als {BREITE_MINIMUM} und kleiner als {BREITE_MAXIMUM} sein.\n");
                    }
                }
            }
            else if (aktuelle_eingabe == "hoehe")
            {
                if (eingabe == 0)
                {
                    Console.WriteLine($"Bitte die Höhe eingeben! ({HOEHE_MINIMUM} - {HOEHE_MAXIMUM})");
                    eingabe = Convert.ToInt32(Console.ReadLine());
                    if (eingabe < HOEHE_MINIMUM || eingabe > HOEHE_MAXIMUM)
                    {
                        eingabe = 0;
                        throw new ArgumentException($"\nDie Höhe muss größer als {HOEHE_MINIMUM} und kleiner als {HOEHE_MAXIMUM} sein.\n");
                    }
                }
            }

            return eingabe;
        }

        // Durchläuft das gesamte Array und setzt jedes Feld auf das angegebene Füllzeichen.
        static void InitialisiereDungeon(char[,] dungeonFeld)
        {
            int zeilen = dungeonFeld.GetLength(0);
            int spalten = dungeonFeld.GetLength(1);

            for (int i = 0; i < zeilen; i++)
            {
                for (int j = 0; j < spalten; j++)
                {
                    dungeonFeld[i, j] = WAND_SYMBOL;
                }
            }
        }

        // Ermittelt zwei unterschiedliche Zufallspositionen für Start und Ende --- Wichtig: Zeile = Breite & Spalte = Höhe
        // Der Rand wird ignoriert
        static void PlatziereStartUndEnde(char[,] dungeonFeld, Random zufall, int breite, int hoehe)
        {
            int endeZeile, endeSpalte;

            bool wiederholen = false;
            int versuche = 0;

            do
            {
                // Startpunkt setzen        
                int startZeile = zufall.Next(1, breite - 1);
                int startSpalte = zufall.Next(1, hoehe - 1);

                dungeonFeld[startZeile, startSpalte] = START_SYMBOL;


                do
                {
                    endeZeile = zufall.Next(1, breite - 1);
                    endeSpalte = zufall.Next(1, hoehe - 1);

                    versuche = versuche + 1;
                    if (versuche > 5)
                    {
                        wiederholen = true;
                        break;
                    }

                } while (Math.Abs(endeZeile - startZeile) + Math.Abs(endeSpalte - startSpalte) < START_END_ABSTAND);

            } while (wiederholen == true);

            dungeonFeld[endeZeile, endeSpalte] = END_SYMBOL;
        }


        static void pfadgenerierung(char[,] dungeon_feld)
        {
            int breite = dungeon_feld.GetLength(0);
            int hoehe = dungeon_feld.GetLength(1);

            // Positionen von Start und Ende finden
            int startX = -1, startY = -1, endX = -1, endY = -1;

            for (int zaehler_breite = 0; zaehler_breite < breite; zaehler_breite++)
            {
                for (int zaehler_hoehe = 0; zaehler_hoehe < hoehe; zaehler_hoehe++)
                {
                    if (dungeon_feld[zaehler_breite, zaehler_hoehe] == START_SYMBOL)
                    {
                        startX = zaehler_breite;
                        startY = zaehler_hoehe;
                    }
                    else if (dungeon_feld[zaehler_breite, zaehler_hoehe] == END_SYMBOL)
                    {
                        endX = zaehler_breite;
                        endY = zaehler_hoehe;
                    }
                }
            }

            // Einfache Pfadgenerierung: horizontal und vertikal verbinden
            int x = startX;
            int y = startY;

            while (x != endX)
            {
                x += (endX > x) ? 1 : -1;
                if (dungeon_feld[x, y] == WAND_SYMBOL)
                    dungeon_feld[x, y] = '.';
            }

            while (y != endY)
            {
                y += (endY > y) ? 1 : -1;
                if (dungeon_feld[x, y] == WAND_SYMBOL)
                    dungeon_feld[x, y] = '.';
            }
        }


        // Gibt das Spielfeld in der Konsole aus. Start/Ende werden farbig hervorgehoben.
        static void GibDungeonAus(char[,] dungeon_feld, int breite, int hoehe)
        {
            Console.WriteLine("--- ZUFALLS-DUNGEON ---");
            Console.WriteLine();

            // Darstellung der Matrix durch verschachtelte Schleifen
            for (int j = 0; j < hoehe; j++)
            {
                for (int i = 0; i < breite; i++)
                {
                    char aktuellesZeichen = dungeon_feld[i, j];

                    // Farbwechsel je nach Symbol
                    if (aktuellesZeichen == START_SYMBOL)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else if (aktuellesZeichen == END_SYMBOL)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else
                    {
                        Console.ResetColor();
                    }

                    Console.Write(aktuellesZeichen);
                    Console.Write(' ');     //Abstand zwischen zeichen
                }
                Console.ResetColor();
                Console.WriteLine(); // Zeilenumbruch nach jeder vollständigen Zeile
            }
        }

        // Erstellt eine Textdatei und schreibt das Dungeon-Muster hinein.
        static void SpeichernInTextdatei(char[,] dungeonFeld, int breite, int hoehe)
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
                            sw.Write(dungeonFeld[y, x]);
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