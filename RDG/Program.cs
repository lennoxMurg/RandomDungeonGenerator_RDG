using System;
using System.Data;
using System.IO;
using System.Collections.Generic;       //Für lists und stacks


namespace Projekt
{
    class Program
    {
        //  Public variablen für die Konstanten 

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

            // Zufällige Platzierung von S und E || Wichtig für DFS Algorithmus
            (int start_zeile, int start_spalte, int end_zeile, int end_spalte) = PlatziereStartUndEnde(dungeonFeld, zufall, breite, hoehe);

            // Erstellt Pfade im Dungeon nach recursive backtracking muster
            Dungeongenerierung_v3(dungeonFeld, start_zeile, start_spalte, end_zeile, end_spalte, zufall);

            // Zeichnet das Array farbig in die Konsole
            GibDungeonAus(dungeonFeld, breite, hoehe);


            Console.ReadKey();
        }


        // Eingabe methode für breite und höhe des dungeons
        static int eingabe_dungeon_groesse(int dungeon_groeße, int andere_groesse, string aktuelle_eingabe)
        {
            int eingabe = dungeon_groeße;

            //Eingabe für die Breite
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

            //Eingabe für die Höhe
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

        // Platziert Start- und Endposition zufällig im Dungeonfeld
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

        //Dritte methode um einen Dungeon zu generieren (Mit Recursive backtracking)
        static void Dungeongenerierung_v3(char[,] dungeonfeld, int startZ, int startS, int endZ, int endS, Random rnd)
        {
            int breite = dungeonfeld.GetLength(0);
            int hoehe = dungeonfeld.GetLength(1);

            // Startposition auf ungerade Koordinaten zwingen
            int sx = (startZ % 2 == 0) ? startZ + 1 : startZ;
            int sy = (startS % 2 == 0) ? startS + 1 : startS;

            // Sicherheitscheck
            if (sx <= 0 || sx >= breite - 1) sx = 1;
            if (sy <= 0 || sy >= hoehe - 1) sy = 1;

            dungeonfeld[sx, sy] = WEG_SYMBOL;

            // Richtungen: unten, oben, rechts, links (2er Schritte!)
            int[] dx = { 0, 0, 2, -2 };
            int[] dy = { 2, -2, 0, 0 };

            void DepthFirstSearch(int x, int y)
            {
                // Richtungsreihenfolge zufällig mischen
                List<int> richtungen = new List<int> { 0, 1, 2, 3 };
                for (int i = 0; i < richtungen.Count; i++)
                {
                    int tauschen = rnd.Next(i, richtungen.Count);
                    (richtungen[i], richtungen[tauschen]) = (richtungen[tauschen], richtungen[i]);
                }

                foreach (int dir in richtungen)
                {
                    int nx = x + dx[dir];
                    int ny = y + dy[dir];

                    if (nx > 0 && nx < breite - 1 &&
                        ny > 0 && ny < hoehe - 1 &&
                        dungeonfeld[nx, ny] == WAND_SYMBOL)
                    {
                        // Wand entfernen
                        dungeonfeld[x + dx[dir] / 2, y + dy[dir] / 2] = WEG_SYMBOL;
                        dungeonfeld[nx, ny] = WEG_SYMBOL;

                        DepthFirstSearch(nx, ny);
                    }
                }
            }

            // DFS starten
            DepthFirstSearch(sx, sy);

            // Start & Ende setzen (am Schluss!)
            dungeonfeld[startZ, startS] = START_SYMBOL;
            dungeonfeld[endZ, endS] = END_SYMBOL;
        }

        // Gibt das Spielfeld in der Konsole aus. Start/Ende werden farbig hervorgehoben.
        static void GibDungeonAus(char[,] dungeonFeld, int breite, int hoehe)
        {
            Console.WriteLine("--- ZUFALLS-DUNGEON ---");
            Console.WriteLine();

            // Darstellung der Matrix durch verschachtelte Schleifen
            for (int j = 0; j < hoehe; j++)
            {
                for (int i = 0; i < breite; i++)
                {
                    char aktuellesZeichen = dungeonFeld[i, j];

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

    }
}