using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public enum Step { Plus, Min }
public class State
{
    public int[] CarState;
    public State(int[] cs)
    {
        CarState = cs;
    }
    public bool DoesItExistYet(Trie t)
    {
        //checks if it is n the Trie already?
        return t.T_Exists_F_Add(CarState);
    }
    public bool IsItPossible()
    {
        //checks if the cars stay within the grid and or their is a carcollision

        // do something
        return false;
    }
    public Dictionary<Tuple<int, Step>, State> GetAllPossibleNextSteps(Trie t)
    {
        Dictionary<Tuple<int, Step>, State> D = new Dictionary<Tuple<int, Step>, State>();
        for (int i = 0; i < CarState.Length; i++)
        {
            int curP = CarState[i];
            CarState[i] = curP + 1;
            if (IsItPossible() && DoesItExistYet(t))
                D.Add(new Tuple<int, Step>(i, Step.Plus), new State(CarState));
            CarState[i] = curP - 1;
            if (IsItPossible() && DoesItExistYet(t))
                D.Add(new Tuple<int, Step>(i, Step.Min), new State(CarState));
            CarState[i] = curP;
        }
        return D;
    }
}
