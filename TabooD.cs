using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TabooD
{
    Dictionary<uint, bool> D;
    public TabooD()
    {
        D = new Dictionary<uint, bool>();
    }
    public bool TryAdd(uint ui, bool b = false)
    {
        lock (D)
        {
            if (D.ContainsKey(ui))
                return false;
            else
            {
                D.Add(ui, b);
                return true;
            }
        }
    }
}