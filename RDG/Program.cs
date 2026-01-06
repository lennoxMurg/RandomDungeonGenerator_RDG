using System;
using System.Data;
using System.IO;

namespace Projekt
{
    class Program
    {
        //  Public variablen für die Konstanten

        // Dichte des Dungeons (je kleiner der Wert, desto dichter / Mehr Wege)
        public const int DUNGEON_DICHTE = 8;

        // Mindestabstand zwischen Start und Ende
        public const int START_END_ABSTAND = 4;

        // Festlegung der Symbole für die Kartenelemente
        public const char WAND_SYMBOL = '#';
        public const char WEG_SYMBOL = '.';
        public const char START_SYMBOL = 'S';
        public const char END_SYMBOL = 'E';

        // Modulare Benutzereingabe
        public const int BREITE_MINIMUM = 10;
        public const int BREITE_MAXIMUM = 50;
        public const int HOEHE_MINIMUM = 10;
        public const int HOEHE_MAXIMUM = 25;


        static void Main(string[] args)
        {
            // Anzahl der zu generierenden Dungeons erstmal als Testwert 
            int dungeon_anzahl = 100;

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

        Nacheingabe:

            for (int i = 0; i < dungeon_anzahl; i++)
            {
                // Erstellung der Datenstruktur (2D-Array) basierend auf Eingabe
                char[,] dungeonFeld = new char[breite, hoehe];

                // Das Array wird initial komplett mit dem WAND-Zeichen gefüllt
                InitialisiereDungeon(dungeonFeld);

                // Zufällige Platzierung von S und E (innerhalb der Spielfeldgrenzen)
                (int start_zeile, int start_spalte, int end_zeile, int end_spalte) = PlatziereStartUndEnde(dungeonFeld, zufall, breite, hoehe);

                // Pfadgenerierung zwischen Start und Ende
                Pfadgenerierung(dungeonFeld, start_zeile, start_spalte, end_zeile, end_spalte);

                // Erstellt weitere Pfade im Dungeon
                Dungeongenerierung(dungeonFeld, zufall);


                // Zeichnet das Array farbig in die Konsole
                GibDungeonAus(dungeonFeld, breite, hoehe);

            }

            Console.ReadKey();
            Console.Clear();
            goto Nacheingabe;

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


        static (int startZeile, int startSpalte, int endeZeile, int endeSpalte) PlatziereStartUndEnde(char[,] dungeonfeld, Random zufall, int breite, int hoehe)
        {
            int startZeile, startSpalte;
            int endeZeile, endeSpalte;

            // Start position zufällig suchen
            startZeile = zufall.Next(1, breite - 1);
            startSpalte = zufall.Next(1, hoehe - 1);

            do
            {
                // Ende position zufällig suchen
                endeZeile = zufall.Next(1, breite - 1);
                endeSpalte = zufall.Next(1, hoehe - 1);
            }
            while (Math.Abs(endeZeile - startZeile) + Math.Abs(endeSpalte - startSpalte) < START_END_ABSTAND);

            // Start und Ende setzen
            dungeonfeld[startZeile, startSpalte] = START_SYMBOL;
            dungeonfeld[endeZeile, endeSpalte] = END_SYMBOL;

            return (startZeile, startSpalte, endeZeile, endeSpalte);
        }


        // Generiert einen geraden Pfad zwischen Start- und Endpunkt
        // erstellt L-förmigen Pfad || Nur temporär oder als toggle option?
        static void Pfadgenerierung(char[,] dungeon_feld, int start_zeile, int start_spalte, int end_zeile, int end_spalte)
        {
            int breite = dungeon_feld.GetLength(0);
            int hoehe = dungeon_feld.GetLength(1);

            int zeile = start_zeile;
            int spalte = start_spalte;


            while (zeile != end_zeile)
            {
                zeile += (end_zeile > zeile) ? 1 : -1;
                if (dungeon_feld[zeile, spalte] == WAND_SYMBOL)
                {
                    dungeon_feld[zeile, spalte] = WEG_SYMBOL;
                }
            }

            while (spalte != end_spalte)
            {
                spalte += (end_spalte > spalte) ? 1 : -1;
                if (dungeon_feld[zeile, spalte] == WAND_SYMBOL)
                {
                    dungeon_feld[zeile, spalte] = WEG_SYMBOL;
                }
            }
        }

        static void Dungeongenerierung(char[,] dungeon_feld, Random zufall, int start_zeile, int start_spalte, int end_zeile, int end_spalte)
        {
            int breite = dungeon_feld.GetLength(0);
            int hoehe = dungeon_feld.GetLength(1);

            // Zusätzliche Pfade generieren
            for (int i = 0; i < breite * hoehe / DUNGEON_DICHTE; i++) // Anzahl der zusätzlichen Pfade basierend auf der Größe
            {
                int x = zufall.Next(1, breite - 1);
                int y = zufall.Next(1, hoehe - 1);

                // Erstelle einen kleinen Raum oder Korridor
                for (int j = 0; j < 5; j++) // Länge des Korridors
                {
                    if (x > 0 && x < breite - 1 && y > 0 && y < hoehe - 1)
                    {
                        if (dungeon_feld[x, y] == WAND_SYMBOL)
                        {
                            dungeon_feld[x, y] = WEG_SYMBOL;
                        }

                        // Zufällige Richtung wählen
                        int richtung = zufall.Next(4);
                        switch (richtung)
                        {
                            case 0: x++; break; // Rechts
                            case 1: x--; break; // Links
                            case 2: y++; break; // Unten
                            case 3: y--; break; // Oben
                        }
                    }
                }
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