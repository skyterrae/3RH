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
    int amount;
    public void CountToConsole()
    {
        int i = 1;
        Step st = this;
        while (st != null)
        {
            if(st.Prev != null && (st.Prev.car != st.car || st.Prev.d != st.d))
                i++;
            st = st.Prev;
        }
        Console.WriteLine(i);
    }

    public Step(char c, Direction D, int am)
    {
        car = c;
        d = D;
        amount = am;
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
        StringBuilder sb = new StringBuilder();
        while (s != null)
        {
            if(s.Next !=null)
            {
                //andere auto/richting dan vorige step
                sb.Append(s.car.ToString() + s.d.ToString() + s.amount.ToString()+", ");
            }
            else if (s.Next == null)
            {
                // laatste append hoeft geen komma op het eidn
                sb.Append(s.car.ToString() + s.d.ToString() + s.amount.ToString());
            }
            s = s.Next;
        }
        Console.Write(sb.ToString());
    }
}