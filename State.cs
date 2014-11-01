using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using System.Linq;
using System.Text;


// public enum Step { Plus, Min }
public class State
{
    public int[] CarState;
    public Step ChainTail;
    public State(int[] cs)
    {
        CarState = cs;
        ChainTail = null;
    }
    public State(int[] cs, Step PrevChainT, Step ThisStep)
    {
        CarState = cs;
        ChainTail = ThisStep;
        ChainTail.AddToChain(PrevChainT);
    }
    public bool DoesItExistYet(Trie t)
    {
        //checks if it is n the Trie already?
        return t.T_Exists_F_Add(CarState);
    }
    public bool DoesItExistYet(ConcurrentDictionary<State, bool> CD)
    {
        //checks if it is n the Trie already?
        if (CD.ContainsKey(this))
            return true;
        else
        {
            CD.AddOrUpdate(this, true,new Func<State,bool,bool>((s,b)=>true));
            return false;
        }
    }
    public override int GetHashCode()
    {
        int hash = 0;
        for (int i = 0; i < CarState.Length; i++)
        {
            hash = hash + CarState[i] * RushHour.HashHelper[i];
        }
        return hash;
    }
    public bool DoesItNotCollide()
    {
        //checks if the cars stay within the grid and or their is a carcollision
        bool b = true;
        for (int i = 0; i < CarState.Length; i++)
        {
            Car c =  RushHour.GetCar(i);
            int options;
            if(c.vertical)
                options = RushHour.Height - c.Length;
            else options = RushHour.Width - c.Length;
            b = b && CarState[i] >= 0 && CarState[i] < options;
        }
        // do something
        return b;
    }

    public List<State> GetAllPossibleNextSteps(Trie t)
    {
        List<State> L = new List<State>();
        for (int i = 0; i < CarState.Length; i++)
        {
            int curP = CarState[i];
            CarState[i] = curP + 1;
            if (DoesItNotCollide() && !DoesItExistYet(t))
                L.Add(new State((int[])CarState.Clone(), ChainTail,RushHour.GetCar(i).getStep(1)));
            CarState[i] = curP - 1;
            if (DoesItNotCollide() && !DoesItExistYet(t))
                L.Add(new State((int[])CarState.Clone(), ChainTail, RushHour.GetCar(i).getStep(-1)));
            CarState[i] = curP;
        }
        return L;
    }
    public List<State> GetAllPossibleNextSteps(ConcurrentDictionary<State,bool> CD)
    {
        List<State> L = new List<State>();
        for (int i = 0; i < CarState.Length; i++)
        {
            int curP = CarState[i];
            CarState[i] = curP + 1;
            State s = new State((int[])CarState.Clone(), ChainTail, RushHour.GetCar(i).getStep(1));
            if (s.DoesItNotCollide() && !s.DoesItExistYet(CD))
                L.Add(s);
            CarState[i] = curP - 1;
            s = new State((int[])CarState.Clone(), ChainTail, RushHour.GetCar(i).getStep(-1));
            if (s.DoesItNotCollide() && !s.DoesItExistYet(CD))
                L.Add(s);
            CarState[i] = curP;
        }
        return L;
    }
}
