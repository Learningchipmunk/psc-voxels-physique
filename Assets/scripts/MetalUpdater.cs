using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalUpdater : MonoBehaviour
{
    // Number of entries
    public int n = 0;

    // The list of metals
    HashSet<Metal> mList = new HashSet<Metal>();




    // -------------------- Functions for metal list ------------------------- //

    public void addMetal(Metal m)
    {
        if(!mList.Contains(m))
        {
            mList.Add(m);
            n += 1;
        }
    }

    // Removes the Metal that has 0 acid on its surface
    public void removeMetal(Metal m)
    {
        mList.Remove(m);
        n -= 1;
    }

    // Update the list of metals
    void updateAllMetals()
    {
        // this Variable only use is to iterate over a HashSet, we remove from mList directly
        var clone = new HashSet<Metal>(mList, mList.Comparer);
        foreach(Metal entry in clone)
        {
            Metal M = entry;

            M.UpdateMetal();
            
            // if balance is reached, than there is no need to update the metal afterwards
            if(M.EqReached())
            {
                // So we remove the metal from the list of updates
                removeMetal(M);
            }
        }
    }

   
    

    // Update is called once per frame
    void FixedUpdate()
    {
        updateAllMetals();
    }
}
