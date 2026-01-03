using System;
using System.Data;
using System.IO;

namespace Projekt
{
    class Program
    {
        //Public variablen für die Konstanten

        // Mindestabstand zwischen Start- und Endpunkt
        public const int START_END_ABSTAND = 10;

        // Festlegung der Symbole für die Kartenelemente
        public const char WAND_SYMBOL = '#';
        public const char START_SYMBOL = 'S';
        public const char END_SYMBOL = 'E';

        // Modulare Benutzereingabe
        public const int BREITE_MINIMUM = 10;
        public const int BREITE_MAXIMUM = 50;
        public const int HOEHE_MINIMUM = 10;
        public const int HOEHE_MAXIMUM = 25;


        static void Main(string[] args)
        {
            int breite = 0, hoehe = 0;
            string aktuelle_eingabe;

            // Wiederholt die Abfrage, bis gültige Werte eingegeben wurden
            do
            {
                try
                {
                    // Aufruf der Methoden zur Breiteneingabe und Höheneingabe
                    //breite = breite_eingabe(breite);
                    //hoehe = hoehe_eingeben(hoehe);
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

            for (int i = 0; i < 10; i++)
            {
                // Erstellung der Datenstruktur (2D-Array) basierend auf Eingabe
                char[,] dungeonFeld = new char[breite, hoehe];

                // Initialisierung des Zufallsgenerators
                Random zufall = new Random();

                // Das Array wird initial komplett mit dem WAND-Zeichen gefüllt
                InitialisiereDungeon(dungeonFeld);

                // Zufällige Platzierung von S und E (innerhalb der Spielfeldgrenzen)
                PlatziereStartUndEnde(dungeonFeld, zufall, breite, hoehe);

                // Zeichnet das Array farbig in die Konsole
                GibDungeonAus(dungeonFeld, breite, hoehe);
            }


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

        // Ermittelt zwei unterschiedliche Zufallspositionen für Start und Ende.
        // Der Rand (Index 0 und Max-1) wird dabei ausgespart.
        static void PlatziereStartUndEnde(char[,] dungeonFeld, Random zufall, int breite, int hoehe)
        {
            // Startpunkt setzen
            int startZeile = zufall.Next(1, breite - 1);
            int startSpalte = zufall.Next(1, hoehe - 1);

            dungeonFeld[startZeile, startSpalte] = START_SYMBOL;


            // Endpunkt setzen -> mit Prüfung auf Dopplung und Mindestabstand
            int endeZeile, endeSpalte;
            do
            {
                endeZeile = zufall.Next(1, breite - 1);
                endeSpalte = zufall.Next(1, hoehe - 1);
            }
            while ((endeZeile == startZeile && endeSpalte == startSpalte) || Math.Abs(endeZeile - startZeile) + Math.Abs(endeSpalte - startSpalte) < START_END_ABSTAND);

            dungeonFeld[endeZeile, endeSpalte] = END_SYMBOL;
        }

        static void pfadgenerierung(char[,] dungeon_feld)
        {
            //Wähle startpunkt aus
            //bewege in eine nicht randständige position
            //setze .  // Weg symbol
            //ziehe einen kleinen 'kreis' um den neuen punkt 
            //wähle zufällig eine richtung aus die noch nicht belegt ist und bei der kein weg ist
            //wiederhole bis ende erreicht ist


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