using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Linq;
using System.Text;

class RushHour
{
    List<State>[] States = new List<State>[3];
    int CurrStateDept;

    static void Main(string[] args)
    {
        ReadInput();

        State s = GetRootState();
        //Trie t = new Trie(getRootLevels());
        ConcurrentDictionary<State, bool> CD = new ConcurrentDictionary<State, bool>();
        List<State> D = s.GetAllPossibleNextSteps(CD);
        List<State>[] Ds = new List<State>[D.Count];
        int i = 0;
        foreach (State St in D)
        {
            // do something with the Dictionary :\
            Ds[i] = St.GetAllPossibleNextSteps(CD);
            i++;
        }
        Ds[2].ElementAt<State>(2).ChainTail.toConsole();
        Console.ReadLine();
    }
    static Dictionary<char, Car> Cars = new Dictionary<char,Car>(); // ander datatype?
    static int indexCount = 0;
    static Dictionary<int,char> IndexToCarC = new Dictionary<int,char>();
    static Dictionary<char,int> CarCToIndex = new Dictionary<char,int>();

    public static void AddCar(Car c, char keyChar)
    {
        // voegt een auto toe
        Cars.Add(keyChar, c);
        CarCToIndex.Add(keyChar,indexCount);
        IndexToCarC.Add(indexCount,keyChar);
        indexCount++;
    }
    public static Car GetCar(int index)
    {
        return Cars[IndexToCarC[index]];
    }
    public static int[] getRootLevels()
    {
        int[] I = new int[Cars.Count];
        for (int m = 0; m < indexCount; m++)
        {
            Car c = Cars[IndexToCarC[m]];
            if (!c.vertical) I[m] = w -c.Length;
            else I[m] = h -c.Length;
        }
        return I;
    }
    public static State GetRootState()
    {
       int[] I = new int[Cars.Count];
       for (int m = 0; m < indexCount; m++)
       {
           Car c = Cars[IndexToCarC[m] ];
           if(!c.vertical) I[m] = c.Xstart;
           else I[m] = c.Ystart;
       }
       return new State(I);
    }

    static int u, w, h, x, y, s;
    static public int Height
    {
        get { return h; }
    }
    static public int Width
    {
        get { return w; }
    }
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
                        Cars[LineC[i]].vertical = false;
                        Cars[LineC[i]].Length++;
                    }
                    else // car is verticaal
                    {
                        Cars[LineC[i]].vertical = true;
                        Cars[LineC[i]].Length++;
                    }
                }
            }
        }
    }
    public static int[] HashHelper = new int[26]{2,3,5,7,11,13,17,19
        ,23,29,31,37,41,43,47,53,59,61,67,71,73,79,83,89,97,101};
}
