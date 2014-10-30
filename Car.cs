using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Car
{
    // direction: 0Horizontal, 1Vertical
    public int Direction, Length, Xstart, Ystart;
    public char C;
    public Car(char c, int x, int y)
    {
        C = c;
        Xstart = x;
        Ystart = y;
        Length = 1;
        Direction = 0;
    }
}

