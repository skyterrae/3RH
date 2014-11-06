using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Linq;
using System.Text;

class RushHour
{
    static List<State>[] States = new List<State>[4];
    static State Result;
    static public void AddToStatesTree(State state, int depth)
    {
        lock (States)
        {
            // if needed resize the list[]
            int l = States.Length;
            if (l <= depth)
            {
                while (l <= depth)
                {
                    l = l * 2;
                }
                Array.Resize(ref States, l);
            }

            // add state to list[]
            if (States[depth] == null)
                States[depth] = new List<State>();
            States[depth].Add(state);
        }
    }

    static void MakeTreeStart()
    {
        State root = GetRootState();
        //Trie t = new Trie(getRootLevels());
        Taboo = new ConcurrentDictionary<uint, bool>();
        States = new List<State>[5];
        AddToStatesTree(root, 0);
    }
    static public ConcurrentDictionary<uint, bool> Taboo;

    static void DivideComputeState(int depth)
    {
        //IEnumerator<State> e = States[depth].GetEnumerator();
        lock (QueLock)
        {
            QueCount = States[depth].Count;
        }
        //while (e.MoveNext())
        //{
            //State state = e.Current;
        foreach(State state in States[depth])
        {
            ThreadPool.QueueUserWorkItem( new WaitCallback(ComputeState),state);
        }
        while (QueCount > 0) ; // wait for the que to finish
    }
    static public void ComputeState(object obj)
    {
        State state = (State)obj;
        state.GetAllPossibleNextSteps();
        lock (QueLock)
        {
            QueCount = QueCount - 1;
        }

    }
    static int QueCount;
    static object QueLock = new Object();
    static void Main(string[] args)
    {
        ReadInput();
        MakeTreeStart();
        int kijkDiepte = 0;
        while (Result == null && kijkDiepte < States.Length)
        {
            //kijkt of de lijst voor dit depth wel bestaat
            if (States[kijkDiepte] != null && States[kijkDiepte].Count>0)
            {
                DivideComputeState(kijkDiepte);
            }   
            // afgehandelde lijsten weggooien om ruimte te besparen
            if (kijkDiepte > 2 && States[kijkDiepte - 2] != null)
            {
                 //States[kijkDiepte - 2].Clear();
            }
            //kijkdiepte ophogen   
            kijkDiepte++;  
        }
        if (Result != null)
        {
             if (u == 0)
                Result.ChainTail.CountToConsole();
             else
                Result.ChainTail.StepsToConsole();
        }
        else Console.WriteLine("Geen oplossing gevonden");
        Console.ReadLine();
    }
    public static void FoundResult(State state)
{
    Result = state;
}

    static Dictionary<char, Car> Cars = new Dictionary<char,Car>(); 
    static int indexCount = 0;
    static Dictionary<int,char> IndexToCarC = new Dictionary<int,char>();
    static Dictionary<char,int> CarCToIndex = new Dictionary<char,int>();

    public static void AddCar(Car c, char keyChar)
    {
        // voegt een auto toe
        Cars.Add(keyChar, c);
        CarCToIndex.Add(keyChar,indexCount);
        c.Index = indexCount;
        IndexToCarC.Add(indexCount,keyChar);
        indexCount++;
    }
    public static Car GetCar(int index)
    {
        return Cars[IndexToCarC[index]];
    }
    public static Car GetCar(char carName)
    {
        return Cars[carName];
    }

    public static int[] getRootLevels()
    {
        int[] I = new int[Cars.Count];
        for (int m = 0; m < indexCount; m++)
        {
            Car c = Cars[IndexToCarC[m]];
            if (!c.vertical) I[m] = w - c.Length;
            else I[m] = h - c.Length;
        }
        return I;
    }
    public static State GetRootState()
    {
       ushort[] I = new ushort[Cars.Count];
       for (int m = 0; m < indexCount; m++)
       {
           Car c = Cars[IndexToCarC[m] ];
           if(!c.vertical) I[m] = c.Xstart;
           else I[m] = c.Ystart;
       }
       return new State(I,0);
    }

    static int u, w, h, x, y, s;
    static public int TargetX
    {
        get { return x; }
    }
    static public int TargetY
    {
        get { return y; }
    }

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
        for (ushort j = 0; j < h; j++)
        {
            //lees regel in als charArray
            LineC = Console.ReadLine().ToCharArray();
            for (ushort i = 0; i < w; i++)
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
