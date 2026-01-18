using System;
using System.Collections.Generic;       //Für lists benötigt


namespace Projekt
{
    class Program
    {
        // Mindestabstand zwischen Start und Ende
        const int START_END_ABSTAND = 4;

        // Festlegung der Symbole für die Kartenelemente
        const char WAND_SYMBOL = '#';
        const char WEG_SYMBOL = '.';
        const char START_SYMBOL = 'S';
        const char END_SYMBOL = 'E';

        // Modulare Benutzereingabe
        const int BREITE_MINIMUM = 10;
        const int BREITE_MAXIMUM = 50;
        const int HOEHE_MINIMUM = 10;
        const int HOEHE_MAXIMUM = 25;


        static void Main(string[] args)
        {
            // Initialisierung des Zufallsgenerators
            Random zufall = new Random();


            // Variable zum wiederholen der Generierung
            bool wiederholen = false;

            while (wiederholen == false)
            {
                int breite = 0, hoehe = 0;
                string aktuelle_eingabe = " ";

                // Wiederholt die Abfrage, bis gültige Werte eingegeben wurden
                do
                {
                    try
                    {
                        // Aufruf der Methode zur Breiten- und Höheneingabe

                        aktuelle_eingabe = "breite";
                        breite = eingabe_dungeon_groesse(breite, aktuelle_eingabe);

                        aktuelle_eingabe = "hoehe";
                        hoehe = eingabe_dungeon_groesse(hoehe, aktuelle_eingabe);

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

                Console.WriteLine("\n\n");
                Console.WriteLine("Wollen sie noch einen Dungeon Generieren?    (Ja/Nein)");
                string antwort = Console.ReadLine().ToUpper();
                if (antwort == "JA" || antwort == "J")
                {
                    wiederholen = false;
                    Console.Clear();
                }
                else
                {
                    wiederholen = true;
                }
            }
        }


        // Eingabe methode für breite und höhe des dungeons
        static int eingabe_dungeon_groesse(int dungeon_groeße, string aktuelle_eingabe)
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


            return (startZeile, startSpalte, endeZeile, endeSpalte);
        }

        // Methode um einen Dungeon zu generieren (Mit Recursive backtracking)
        static void Dungeongenerierung_v3(char[,] dungeonfeld, int startZeile, int startSpalte, int endZeile, int endSpalte, Random rnd)
        {
            int breite = dungeonfeld.GetLength(0);
            int hoehe = dungeonfeld.GetLength(1);

            int start_x, start_y;

            // Startposition auf ungerade Koordinaten zwingen
            if (startZeile % 2 == 0)
            {
                start_x = startZeile + 1;
            }
            else
            {
                start_x = startZeile;
            }

            if (startSpalte % 2 == 0)
            {
                start_y = startSpalte + 1;
            }
            else
            {
                start_y = startSpalte;
            }


            // Sicherheitscheck
            if (start_x <= 0 || start_x >= breite - 1)
            {
                start_x = 1;
            }

            if (start_y <= 0 || start_y >= hoehe - 1)
            {
                start_y = 1;
            }

            dungeonfeld[start_x, start_y] = WEG_SYMBOL;

            // Richtungen: unten, oben, rechts, links (2er Schritte!)
            int[] richtung_x = { 0, 0, 2, -2 };
            int[] richtung_y = { 2, -2, 0, 0 };

            void DepthFirstSearch(int start_x, int start_y)
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
                    int punkt_x = start_x + richtung_x[dir];
                    int punkt_y = start_y + richtung_y[dir];

                    if (punkt_x > 0 && punkt_x < breite - 1 && punkt_y > 0 && punkt_y < hoehe - 1 && dungeonfeld[punkt_x, punkt_y] == WAND_SYMBOL)
                    {
                        // Wand entfernen
                        dungeonfeld[start_x + richtung_x[dir] / 2, start_y + richtung_y[dir] / 2] = WEG_SYMBOL;
                        dungeonfeld[punkt_x, punkt_y] = WEG_SYMBOL;

                        DepthFirstSearch(punkt_x, punkt_y);
                    }
                }
            }

            // DFS starten
            DepthFirstSearch(start_x, start_y);

            // Start & Ende setzen (am Schluss!)
            dungeonfeld[startZeile, startSpalte] = START_SYMBOL;
            dungeonfeld[endZeile, endSpalte] = END_SYMBOL;
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