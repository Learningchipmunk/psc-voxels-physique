using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
<summary>
    This method creates a tree that stores all the neighboring metals
    according to The voxel script in a dictionary.
    This makes it po
</summary>
*/

public class TreeUpdater : MonoBehaviour
{
    // The tree that contains the graph of all neighboring voxels:
    private Dictionary<GameObject, LinkedList<ThermoBody>> _tree = new Dictionary<GameObject, LinkedList<ThermoBody>>();
    
     // ------------------------------------ Functions for trees ------------------------- //

    // Adds b to the neighbors of a and vice versa
    public void AddNeighbors(GameObject a, GameObject b)
    {
        AddNeighbor(a ,b);
        AddNeighbor(b, a);
    }

    // Only adds b to the list of neighbors of a
    private void AddNeighbor(GameObject a, GameObject b)
    {
        if (_tree.ContainsKey(a)) 
        { 
  
            // Getting the list of neighbors of a
            LinkedList<ThermoBody> l;
            _tree.TryGetValue(a, out l);

            // Adding b to the list
            l.AddFirst(b.GetComponent<ThermoBody>());

            // Replacing the new list in the HashMap
            _tree.Remove(a);
            _tree.Add(a, l); 
        } 
        else 
        { 

            // Creating the list of neighbors of a
            LinkedList<ThermoBody> l = new LinkedList<ThermoBody>();

            // Adding b to the list
            l.AddFirst(b.GetComponent<ThermoBody>());

            // Adding the couple (a, l) to the HashMap
            _tree.Add(a, l); 
        }
    }

    public void RemoveNeighbors(GameObject a, GameObject b)
    {
        RemoveNeighbor(a ,b);
        RemoveNeighbor(b, a);
    }

    private void RemoveNeighbor(GameObject a, GameObject b)
    {
        if (_tree.ContainsKey(a)) 
        { 
  
            // Getting the list of neighbors of a
            LinkedList<ThermoBody> l;
            _tree.TryGetValue(a, out l);

            // Adding b to the list
            if(l.Remove(b.GetComponent<ThermoBody>())) Debug.Log("Successfull! " + b.name + " was indeed a neighbor.");else Debug.Log("Not successfull...");

            // Replacing the new list in the HashMap
            _tree.Remove(a);
            _tree.Add(a, l); 
        }else
        {
            Debug.Log("You tried to remove a neighbor from a game object not in the tree.");
        }
    }

    public LinkedList<ThermoBody> GetNeighbors(GameObject a)
    {
        if (_tree.ContainsKey(a)) 
        { 
            // Getting the list of neighbors of a
            LinkedList<ThermoBody> l;
            _tree.TryGetValue(a, out l);
            return l;
        }else
        {
            Debug.Log("You tried to Get the neighbors of a game object that is not in the tree.");
            return null;
        }
    }

    public void printTree()
    {
        foreach(KeyValuePair<GameObject, LinkedList<ThermoBody>> entry in _tree) 
        { 
            Debug.Log(entry.Key + " -> ");
            
            // We get the Adjacency List of the entry Key
            LinkedList<ThermoBody> l = entry.Value;

            // We then print the Adjacency List
            Debug.Log(" neighbor list : ");
            foreach(ThermoBody gObj in l)
            {
                Debug.Log(gObj.GetName());
            }
        } 
    }
    
    private void awake()
    {
        if(_tree==null)_tree = new Dictionary<GameObject, LinkedList<ThermoBody>>();
    }

    // Update is called once per frame
    void Update()
    {
        // if(Time.time >= 1 && Time.time <= 1.1f)
        // {
        //     printTree();
        // }
    }
}
