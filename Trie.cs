using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Trie
{
    // leuk begin van een Trie object, maar hij werkt niet
    // negeer maar

    int[] LevelWidth;
    TrieNode Root;
    public Trie(int[] lvlW)
    {
        LevelWidth = lvlW;
        Root = new TrieNode(LevelWidth[0], 0);
    }
    public bool T_Exists_F_Add(int[] state)
    {
        TrieItem TI = Root;
        int lvl = 0;
        bool found = true;
        lock (Root)
        {
            while (!(TI is TrieLeaf))
            {
                TrieNode TN = TI as TrieNode;
                if (!TN.Possible(state[lvl]))
                {
                    return true;
                }
                if (!found && TN.Exists(state[lvl]))
                {
                    TI = TN.Get(state[lvl]);
                }
                else
                {
                    if (lvl == LevelWidth.Length - 1)
                        TI = TN.AddLeaf(state[lvl]);
                    else TI = TN.AddNode(state[lvl], LevelWidth[lvl]);
                    found = false;
                }
                lvl++;
            }
            return found;
        }
    }
}
public abstract class TrieItem
{
    protected int car;
}
public class TrieLeaf : TrieItem
{
    public TrieLeaf()
    {
        car = -1;   
    }
}
public class TrieNode : TrieItem
{
    TrieItem[] Triebranches;

    public TrieNode(int items, int c)
    {
        Triebranches = new TrieItem[items];
        car = c;
    }
    public TrieItem AddNode(int index, int width)
    {
        Triebranches[index] = new TrieNode(width,car+1);
        return Triebranches[index];
    }
    public TrieItem AddLeaf(int index)
    {
        Triebranches[index] = new TrieLeaf();
        return Triebranches[index];

    }
    public TrieItem Get(int i)
    {
        return Triebranches[i];
    }
    public bool Exists(int i)
    { 
        return Triebranches[i] != null; 
    }
    public bool Possible(int i)
    {
        return i>=0&& i< Triebranches.Length;
    }
}
