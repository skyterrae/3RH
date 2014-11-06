using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using System.Linq;
using System.Text;

// public enum Step { Plus, Min }
public class State
{
    public ushort[] CarState;
    public int Depth;
    public Step ChainTail;
    public State(ushort[] cs, int depth)
    {
        // constructor with empty chainTail (for the root state)
        CarState = cs;
        ChainTail = null;
        Depth = depth;
    }
    public State(ushort[] cs, Step PrevChainT, Step ThisStep, int depth)
    {
        // constructor with adding to the chainTail
        CarState = cs;
        ChainTail = ThisStep;
        ChainTail.AddToChain(PrevChainT);
        Depth = depth;
    }
    public bool DoesItExistYet(Trie t)
    {
        // looks if it already in the Trie
        // return t.T_Exists_F_Add(CarState);
        return false;
    }
    public bool DoesNotExistYet()
    {
        // try add the value. If it is already inthere, it gives a false
        uint h = this.GetHashCode();
        return RushHour.Taboo.TryAdd(h, false);
    }
    public new uint GetHashCode()
    {
        //hash code from :
        // http://stackoverflow.com/questions/3404715/c-sharp-hashcode-for-array-of-ints
        int hc = CarState.Length;
        for (int i = 0; i < CarState.Length; ++i)
        {
            hc = unchecked(hc * 314159 + CarState[i]);
        }
        return (uint)hc;
    }
    public bool XreachesTarget()
    {
        // checks or an answer to the RushHour is found
        // check or x is on its target position
        Car x = RushHour.GetCar('x');

        if (x.vertical && x.Xstart == RushHour.TargetX)
        {
            //verticaal en dezelfde kolom
            int tarY = RushHour.TargetY;
            int carY = CarState[x.Index];
            return carY == tarY;
        }
        else if (!x.vertical && x.Ystart == RushHour.TargetY)
        {
            //verticaal en dezelfde kolom
            int tarX = RushHour.TargetX;
            int carX = CarState[x.Index];
            return carX == tarX;
        }
        else return false;
    }
    public bool DoesNotCollide()
    {
        //checks if the cars stay within the grid and or their is a carcollision
        for (int i = 0; i < CarState.Length; i++)
        {
            Car current_car = RushHour.GetCar(i);
            // checks car inside of grid
            int options;
            if (current_car.vertical)
                options = RushHour.Height - current_car.Length;
            else options = RushHour.Width - current_car.Length;
            if (CarState[i] < 0 || options < CarState[i])
                return false;

            //check collision with other cars
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
                int tempXB = tempXA + 1;
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

    public void GetAllPossibleNextSteps()
    {
        // add all posible next states to the Taboolist and to the list of nextstates States[Depth+1]
        for (int i = 0; i < CarState.Length; i++)
        {
            //remember current state
            ushort curP = CarState[i];
            // move car up/right
            int k = 1;
            CarState[i] = (ushort)(curP + k);
            State s = new State((ushort[])CarState.Clone(), ChainTail, RushHour.GetCar(i).getStep(1), Depth+1);
            // moves it up one step as long as it does not collide or fall out of the grid
            while (s.DoesNotCollide())
            {
                //checks if the current state is already in Taboo
                if (s.DoesNotExistYet())
                {
                    // kijkt of de state ook de target vangt
                    if (s.XreachesTarget())
                    {
                        // zo ja, geef het resultaat terug
                        RushHour.FoundResult(s);
                        break;
                    }
                    // zo niet, voeg to aan mogelijkheden
                    RushHour.AddToStatesTree(s, s.Depth);
                }
                // ga een stap verder
                k++;
                CarState[i] = (ushort)(curP + k);
                s = new State((ushort[])CarState.Clone(), ChainTail, RushHour.GetCar(i).getStep(k), Depth + 1);
            }
            //doe hetzelfde voor stappen naar beneden/links
            k = 1;
            CarState[i] = (ushort)(curP - k);
            s = new State((ushort[])CarState.Clone(), ChainTail, RushHour.GetCar(i).getStep(-1), Depth + 1);
            while (s.DoesNotCollide())
            {
                if (s.DoesNotExistYet())
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
                k++;
                CarState[i] = (ushort)(curP - k);
                s = new State((ushort[])CarState.Clone(), ChainTail, RushHour.GetCar(i).getStep(-k), Depth + 1);
            }
            // zet waarde van Deze state terug op zichzelf
            CarState[i] = curP;
        }
    }
}
