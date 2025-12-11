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




            static char[,] InitialisiereDungeonArray(int breite, int hoehe)
            {
                char[,] dungeon = new char[hoehe, breite];

                for (int y = 0; y < hoehe; y++)
                {
                    for (int x = 0; x < breite; x++)
                    {
                        dungeon[y, x] = '#';
                    }
                }

                return dungeon;
            }


        }


    }
}