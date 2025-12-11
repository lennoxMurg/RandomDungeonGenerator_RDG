using System;

namespace Projekt
{
    class Program
    {
        static void Main(string[] args)
        {
            const char wand = '#';
            const char start = 'S';
            const char ende = 'E';

            const int breite_minimum = 10;
            const int breite_maximum = 50;

            const int hoehe_minimum = 10;
            const int hoehe_maximum = 25;

            int breite = 0;
            int hoehe = 0;

            do
            {
                try
                {

                    breite = breite_eingabe(breite, breite_maximum, breite_minimum);

                    Console.Clear();

                    hoehe = hoehe_eingeben(hoehe, hoehe_maximum, hoehe_minimum);


                    if (breite != 0 && hoehe != 0)
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


        }

        static int breite_eingabe(int breite, int breite_maximum, int breite_minimum)
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
            return breite;
        }

        static int hoehe_eingeben(int hoehe, int hoehe_maximum, int hoehe_minimum)
        {
            Console.WriteLine("Bitte die Höhe Eingeben!");
            hoehe = Convert.ToInt32(Console.ReadLine());
            if (hoehe < hoehe_minimum || hoehe > hoehe_maximum)
            {
                hoehe = 0;
                throw new ArgumentException("\nDie Höhe muss größer als 10 und kleiner als 25 sein.\n");
            }
            return hoehe;
        }
    }
}
