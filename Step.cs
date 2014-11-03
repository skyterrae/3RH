using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum Direction { r, u, l, d }
public class Step
{
    char car;
    Direction d;
    Step Prev, Next;
    public void CountToConsole()
    {
        int i = 0;
        Step st = this;
        while (st.Prev != null)
        {
            i = i + 1;
            st = st.Prev;
        }
        Console.WriteLine(i);
    }

    public Step(char c, Direction D)
    {
        car = c;
        d = D;
        Prev = null;
        Next = null;
    }
    public void AddToChain(Step chainTail)
    {
        Prev = chainTail;
    }
    private Step GetChain()
    {
        if (Prev == null)
            return this;
        else
        {
            Prev.Next = this;
            return Prev.GetChain();
        }
    }
    public void StepsToConsole()
    {
        Step s = this.GetChain();
        int i = 1;
        StringBuilder sb = new StringBuilder();
        while (s != null)
        {
            //kijkt of dezelfde auto nu nogeens dezelfde richting upgaat
            if (s.Next !=null && s.Next.car == s.car && s.Next.d == s.d)
                i++;
            else if(s.Next !=null)
            {
                //andere auto/richting dan vorige step
                sb.Append(s.car.ToString() + s.d.ToString() + i.ToString()+", ");
                i = 1;
            }
            else if (s.Next == null)
            {
                // laatste append hoeft geen komma op het eidn
                sb.Append(s.car.ToString() + s.d.ToString() + i.ToString());
            }
            s = s.Next;
        }
        Console.Write(sb.ToString());
    }
}