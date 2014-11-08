using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Linq;
using System.Text;

class RushHour
{
    #region static membervariables
    static int u, w, h, x, y, s;
    static List<State>[] states = new List<State>[25];
    static public TabooD Taboo = new TabooD();
    static Dictionary<char, Car> cars = new Dictionary<char, Car>();
    static int indexCount = 0;
    static Dictionary<int, char> indexToChar = new Dictionary<int, char>();
    static Dictionary<char, int> charToIndex = new Dictionary<char, int>();
    static State result;
    static int queCount, kijkDiepte = 0;
    static object queLock = new Object();
    #endregion

    static void Main(string[] args)
    {
        readInput();
        setRootState();
        computeAnswer();
        writeOutput();

        //keep console open
        Console.ReadLine();
    }
    
    static void readInput()
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
                    if (!cars.ContainsKey(LineC[i]))
                        AddCar(new Car(LineC[i], i, j), LineC[i]);
                    // kijk if horizontal
                    else if (i > 0 && GridI[i - 1, j] == GridI[i, j])
                    {
                        cars[LineC[i]].vertical = false;
                        cars[LineC[i]].Length++;
                    }
                    else // car is verticaal
                    {
                        cars[LineC[i]].vertical = true;
                        cars[LineC[i]].Length++;
                    }
                }
            }
        }
    }
    static void AddCar(Car c, char keyChar)
    {
        // voegt een auto toe aan de Dic<char,Car> Cars
        // also inits a carindex, and adds it to the translators (char->int en int->char)
        cars.Add(keyChar, c);
        charToIndex.Add(keyChar,indexCount);
        c.Index = indexCount;
        indexToChar.Add(indexCount,keyChar);
        indexCount++;
    }
    static void setRootState()
    {
       ushort[] I = new ushort[cars.Count];
       for (int m = 0; m < indexCount; m++)
       {
           Car c = cars[indexToChar[m] ];
           if(!c.vertical) I[m] = c.Xstart;
           else I[m] = c.Ystart;
       }
       AddToStatesTree(new State(I,0), 0);
    }
    static void computeAnswer()
    {
        while (result == null && kijkDiepte < states.Length)
        {
            //kijkt of de lijst voor dit depth wel bestaat
            if (states[kijkDiepte] != null)
            {
                List<State> Ls = states[kijkDiepte];
                states[kijkDiepte] = null;
                computeListStates(Ls, kijkDiepte);
            }
            else kijkDiepte++;
        }
    }
    static void computeListStates(List<State> stateL, int depth)
    {
        //hoog kijkdiepte op
        kijkDiepte = depth + 1;

        lock (queLock)
        {
            // set how many items the que must wait for
            queCount = stateL.Count;
        }
        foreach(State state in stateL)
        {
            // add all states inside stateL to the que
            ThreadPool.QueueUserWorkItem( new WaitCallback(computeSingleState),state);
        }
        while (queCount > 0) ; // wait for the que to finish     
    }
    static void computeSingleState(object obj)
    {
        //used as the delegate WaitCallBack in computeListStates

        // compute next steps from this single state
        State state = (State)obj;
        state.GetAllPossibleNextSteps();
        //lower the quecount
        lock (queLock)
        {
            queCount = queCount - 1;
        }

    }
    static void writeOutput()
    {
        //writes output to the console
        if (result != null)
        {
            if (SolveMode)
                result.ChainTail.StepsToConsole();
            else result.ChainTail.CountToConsole();
        }
        else
        {
            if (SolveMode)
                Console.WriteLine("Geen oplossing gevonden");
            else Console.WriteLine("-1");
        }
    }

    #region static properties to be called from other classes
    public static Car GetCar(int index)
    {
        return cars[indexToChar[index]];
    }
    public static Car GetCar(char carName)
    {
        return cars[carName];
    }
    public static int TargetX
    {
        get { return x; }
    }
    public static int TargetY
    {
        get { return y; }
    }
    public static int Height
    {
        get { return h; }
    }
    public static int Width
    {
        get { return w; }
    }
    public static bool Heuristical
    {
        get { return s == 1; }
    }
    public static bool SolveMode
    {
        get { return u == 1; }
    }
    #endregion

    #region static methods to be called from other classes
    public static void FoundResult(State state)
    {
        // public method to invoke when a state realises it has solved the puzzle
        if(result == null || result.Depth < state.Depth)
            result = state;
    }
    public static void AddToStatesTree(State state, int depth)
    {
        // add a state to the array<list<State>> States
        lock (states)
        {
            // if needed resize the list[]
            int l = states.Length;
            if (l <= depth)
            {
                while (l <= depth)
                {
                    l = l * 2;
                }
                Array.Resize(ref states, l);
            }

            // add state to list[]
            if (states[depth] == null)
                states[depth] = new List<State>();
            states[depth].Add(state);

            // zorgen dat in de volgende ronde van stateverwerking 
            // wordt begonnen bij een lijst van diepte depth 
            if (depth < kijkDiepte)
                kijkDiepte = depth; //laagste heuristiek == beste heuristiek
        }
    }
    #endregion
}
