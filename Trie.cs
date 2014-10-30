using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Trie
{
    int[] LevelWidth;
    TrieNode Root;
    public Trie(int[] lvlW)
    {
        LevelWidth = lvlW;
        Root = new TrieNode(LevelWidth[0]);
    }
    public bool T_Exists_F_Add(int[] state)
    {
        TrieNode TN = Root;
        int lvl = 0;
        while (lvl < LevelWidth.Length)
        {
            if(TN.Exists(state[lvl]))
            {
                TN = TN.Get(state[lvl]);
                lvl++;
            }
            else
            {
                TN.Add(state[lvl],LevelWidth[lvl]);
                return false;
            }
        }
        return true;
    }
}
public class TrieNode
{
    TrieNode[] Triebranches;

    public TrieNode(int items)
    {
        Triebranches = new TrieNode[items];
    }
    public void Add(int index, int width)
    {
        Triebranches[index] = new TrieNode(width);
    }
    public TrieNode Get(int i)
    {
        return Triebranches[i];
    }
    public bool Exists(int i)
    { 
        return Triebranches[i] != null; 
    }
}
