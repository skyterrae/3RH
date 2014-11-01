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
    public void toConsole()
    {
        Step s = this.GetChain();
        int i = 1;
        while (s != null)
        {
            if (s.Next !=null && s.Next.car == s.car && s.Next.d == s.d)
                i++;
            else
            {
                Console.WriteLine(car.ToString() + d.ToString() + i.ToString());
                i = 1;
            }
            s = s.Next;
        }
    }
}