using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using System.Linq;
using System.Text;

// public enum Step { Plus, Min }
public class State
{
    public int[] CarState;
    public int Depth;
    public Step ChainTail;
    public State(int[] cs, int depth)
    {
        CarState = cs;
        ChainTail = null;
        Depth = depth;
    }
    public State(int[] cs, Step PrevChainT, Step ThisStep, int depth)
    {
        CarState = cs;
        ChainTail = ThisStep;
        ChainTail.AddToChain(PrevChainT);
        Depth = depth;

    }
    public bool DoesItExistYet(Trie t)
    {
        //checks if it is n the Trie already?
        return t.T_Exists_F_Add(CarState);
    }
    public bool DoesItExistYet(ConcurrentDictionary<int, bool> CD)
    {
        int h = this.GetHashCode();
        //checks if it is n the Trie already?
        if (CD.ContainsKey(h))
            return true;
        else
        {
            CD.AddOrUpdate(h, true, new Func<int, bool, bool>((i, b) => true));
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
    public bool XreachesTarget()
    {
        Car x = RushHour.GetCar('x');

        if (x.vertical && x.Xstart == RushHour.TargetX)
        {
            //verticaal en dezelfde kolom
            int tarY = RushHour.TargetY;
            int carY = CarState[x.Index];
            return carY <= tarY && tarY < carY + x.Length;
        }
        else if (!x.vertical && x.Ystart == RushHour.TargetY)
        {
            //verticaal en dezelfde kolom
            int tarX = RushHour.TargetX;
            int carX = CarState[x.Index];
            return carX <= tarX && tarX < carX + x.Length;
        }
        else return false;
    }
    public bool DoesItNotCollide()
    {
        //checks if the cars stay within the grid and or their is a carcollision
        for (int i = 0; i < CarState.Length; i++)
        {
            Car current_car = RushHour.GetCar(i);
            int options;
            if (current_car.vertical)
                options = RushHour.Height - current_car.Length;
            else options = RushHour.Width - current_car.Length;
            if (CarState[i] < 0 || options < CarState[i])
                return false;

            for (int j = i + 1; j < CarState.Length; j++)
            {
                Car temp_car = RushHour.GetCar(j);

                //start posities van de autos 
                int curXA = current_car.Xstart;
                int curYA = current_car.Ystart;
                int tempXA = temp_car.Xstart;
                int tempYA = temp_car.Ystart;
                int curXB = curXA + 1;
                int curYB = curYA + 1;
                int tempXB = tempXA+ 1;
                int tempYB = tempYA + 1;
                if (current_car.vertical)
                {
                    curYA = CarState[i];
                    curYB = curYA + current_car.Length;
                }
                else
                {
                    curXA = CarState[i];
                    curXB = curXA + current_car.Length;
                }
                if (temp_car.vertical)
                {
                    tempYA = CarState[j];
                    tempYB = tempYA + temp_car.Length;
                }
                else
                {
                    tempXA = CarState[j];
                    tempXB = tempXA + temp_car.Length;
                }
                
                // kijkt of er een cllision is, zo ja, geeft direct false terug;
                if ((curXA <= tempXA && tempXA < curXB) || (tempXA <= curXA && curXA < tempXB))
                {
                    if ((curYA <= tempYA && tempYA < curYB) || (tempYA <= curYA && curYA < tempYB))
                    {
                        return false;
                    }
                }
            }
        }
        // if nothing collides
        return true;
    }

    public void GetAllPossibleNextSteps(ConcurrentDictionary<int,bool> CD)
    {
        for (int i = 0; i < CarState.Length; i++)
        {
            int curP = CarState[i];
            CarState[i] = curP + 1;
            State s = new State((int[])CarState.Clone(), ChainTail, RushHour.GetCar(i).getStep(1), Depth+1);
            if (s.DoesItNotCollide() && !s.DoesItExistYet(CD))
            {
                // kijkt of de state ook de target vangt
                if (s.XreachesTarget())
                {
                    RushHour.FoundResult(s);
                    break;
                }
                // zo niet, voeg to aan mogelijkheden
                RushHour.AddToStatesTree(s, s.Depth);
            }
            CarState[i] = curP - 1;
            s = new State((int[])CarState.Clone(), ChainTail, RushHour.GetCar(i).getStep(-1), Depth + 1);
            if (s.DoesItNotCollide() && !s.DoesItExistYet(CD))
            {
                // kijkt of de state ook de target vangt
                if (s.XreachesTarget())
                {
                    RushHour.FoundResult(s);
                    break;
                }
                // zo niet, voeg to aan mogelijkheden
                RushHour.AddToStatesTree(s, s.Depth);
            }
            // zet waarde van Deze state terug op zichzelf
            CarState[i] = curP;
        }
    }
}
