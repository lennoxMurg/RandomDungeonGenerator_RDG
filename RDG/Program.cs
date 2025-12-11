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
                            throw new ArgumentException("\nDie Breite muss größer als 10 und kleiner als 50 sein .\n");
                        }

                    }


                    Console.WriteLine("Bitte die Höhe Eingeben!");
                    hoehe = Convert.ToInt32(Console.ReadLine());

                    if (hoehe < höhe_minimum || hoehe > höhe_maximum)
                    {
                        throw new ArgumentException("\nDie Höhe muss größer als 10 und kleiner als 25 sein.\n");
                    }
                    break;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);

                }

            }
            while (true);


            char[,] dungeonFeld = new char[breite, hoehe];


            


            InitialisiereDungeon(dungeonFeld, WALL);


           


     
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




    }


        }


    
