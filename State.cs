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
            Car current_car =  RushHour.GetCar(i);
            int options;
            if(current_car.vertical)
                options = RushHour.Height - current_car.Length;
            else options = RushHour.Width - current_car.Length;
            //carstate[i] < options veranderd naar <= 
            b = b && CarState[i] >= 0 && CarState[i] <= options;

            if( i != CarState.Length - 1)
                for (int j = i + 1; j < CarState.Length; j++)
                {
                    Car temp_car = RushHour.GetCar(j);

                    int curX = current_car.Xstart, curY = current_car.Ystart, tempX = temp_car.Xstart, tempY = temp_car.Ystart;

                    if (current_car.vertical)
                    {
                        if (temp_car.vertical)
                        {
                            if (curX == tempX)
                            {
                                if (curY < tempY)
                                    b = b && curY + current_car.Length - 1 < tempY;
                                else if (curY > tempY)
                                    b = b && tempY + temp_car.Length - 1 < curY;
                                else
                                    return false;
                            }
                            //zo niet hoeven we niks te doen, beide zijn verticaal en niet in de zelfde kolom
                        }
                        else
                        {
                            //als startx van current binnen de lengte van temp zit en starty van temp binnen 
                            //de lengte van current zit dan is er een collision
                            if (curX >= tempX)
                                if (curY <= tempY)
                                    if (tempX + temp_car.Length - 1 >= curX)
                                        if (curY + current_car.Length - 1 >= tempY)
                                            return false;
                        }
                    }
                    else
                    {
                        if (temp_car.vertical)
                        {
                            //als startx van temp binnen de lengte van current zit en starty van current binnen 
                            //de lengte van temp zit dan is er een collision
                            if (tempX >= curX)
                                if (tempY <= curY)
                                    if (curX + current_car.Length - 1 >= tempX)
                                        if (tempY + temp_car.Length - 1 >= curY)
                                            return false;
                        }
                        else
                        {
                            if (curY == tempY)
                            {
                                if (curX < tempX)
                                    b = b && curX + current_car.Length -1 < tempX;
                                else if (curX > tempX)
                                    b = b && tempX + temp_car.Length - 1 < curX;
                                else
                                    return false;
                            }
                            //zo niet hoeven we niks te doen, beide zijn horizontaal en niet in de zelfde rij
                        }
                    }

                    //if b is already false, there is no point to continue checking any of the remaining cars
                    if (!b)
                        return false;
                }

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
            bool nocollide = s.DoesItNotCollide();
            if (nocollide && !s.DoesItExistYet(CD))
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
