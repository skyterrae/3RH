using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class RushHour
{
    static void Main(string[] args)
    {
        ReadInput();
        Console.ReadLine();
    }
    static Dictionary<int, Car> Cars; // ander datatype?
    public static void AddCar(Car c, int index)
    {
        Cars.Add(index, c);
    }
    static int u, w, h, x, y, s;
    public static void ReadInput()
    {
        // line 1: telmodus0, oplosmodus1
        u = int.Parse(Console.ReadLine());
        // line 2: gridgrootte
        string[] line = Console.ReadLine().Split();
        w = int.Parse(line[0]);
        h = int.Parse(line[1]);
        // line 3: targetvakje
        line = Console.ReadLine().Split();
        x = int.Parse(line[0]);
        y = int.Parse(line[1]);
        // line 4: A* heuristiek
        s = int.Parse(Console.ReadLine());

        // maakt car collectie aan
        Cars = new Dictionary<int, Car>();
        // leest grid in
        int[,] GridI = new int[w, h];
        char[] LineC;
        for (int i = 0; i < h; i++)
        {
            LineC = Console.ReadLine().ToCharArray();
            for (int j = 0; j < w; j++)
            {
                if (LineC[j] != '.')
                {
                    GridI[j, i] = (int)LineC[j] - (int)'a';
                    // voeg toe if new
                    if (!Cars.ContainsKey(GridI[j, i]))
                        AddCar(new Car(LineC[j], j, i), GridI[j, i]);
                    // kijk if horizontal
                    else if (j > 0 && GridI[j - 1, i] == GridI[j, i])
                    {
                        Cars[GridI[j, i]].Direction = 0;
                        Cars[GridI[j, i]].Length++;
                    }
                    else // car is verticaal
                    {
                        Cars[GridI[j, i]].Direction = 1;
                        Cars[GridI[j, i]].Length++;
                    }
                }
            }
        }
    }
}
public class Car
{
    // direction: 0Horizontal, 1Vertical
    public int Direction, Length, Xstart, Ystart;
    public char C;
    public Car(char c, int x, int y)
    {
        C = c;
        Xstart = x;
        Ystart = y;
        Length = 1;
        Direction = 0;
    }
}