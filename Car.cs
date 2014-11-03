using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Car
{
    // direction: 0Horizontal, 1Vertical
    public bool vertical;
    public int Length, Xstart, Ystart, Index;
    public char C;
    public Car(char c, int x, int y)
    {
        C = c;
        Xstart = x;
        Ystart = y;
        Length = 1;
        Index = -1;
        vertical = false;
    }
    public Step getStep(int d)
    {
        Direction dir;
        if(d==-1 && !vertical) //L
            dir = Direction.l;
        else if(d==1 && vertical) //D
            dir = Direction.d;

        else if(d==-1 && vertical) //U
            dir = Direction.u;
        else  //R
            dir = Direction.r;

        return new Step(C, dir);
    }
}

