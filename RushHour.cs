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
    static Dictionary<char, Car> Cars = new Dictionary<char,Car>(); // ander datatype?
    static int indexCount = 0;
    static Dictionary<int,char> IndexToCarC = new Dictionary<int,char>();
    static Dictionary<char,int> CarCToIndex = new Dictionary<char,int>();

    public static void AddCar(Car c, char keyChar)
    {
        Cars.Add(keyChar, c);
        CarCToIndex.Add(keyChar,indexCount);
        IndexToCarC.Add(indexCount,keyChar);
        indexCount++;
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

        // leest grid in
        int[,] GridI = new int[w, h];
        char[] LineC;
        for (int j = 0; j < h; j++)
        {
            //lees regel in als charArray
            LineC = Console.ReadLine().ToCharArray();
            for (int i = 0; i < w; i++)
            {
                if (LineC[i] != '.')
                {
                    GridI[i, j] = (int)LineC[i] - (int)'a';
                    // voeg toe if new
                    if (!Cars.ContainsKey(LineC[i]))
                        AddCar(new Car(LineC[i], i, j), LineC[i]);
                    // kijk if horizontal
                    else if (i > 0 && GridI[i - 1, j] == GridI[i, j])
                    {
                        Cars[LineC[i]].Direction = 0;
                        Cars[LineC[i]].Length++;
                    }
                    else // car is verticaal
                    {
                        Cars[LineC[i]].Direction = 1;
                        Cars[LineC[i]].Length++;
                    }
                }
            }
        }
    }
}
