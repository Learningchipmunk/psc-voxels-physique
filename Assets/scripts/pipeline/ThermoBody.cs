using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThermoBody : MonoBehaviour
{
    // Temperature (Kelvin)
    public float T;

    // Temperature à t+1 (Kelvin)
    public float Tnew;    

    // The time of the last heat exchange
    private float _lastReaction = 0f;

    // Neighbors of the voxel :
    GameObject [] neighbors;

    // Metal class for mesh update :
    private Metal _metal;

    // Stores the Script Ref of TreeUpdater :
    private TreeUpdater _refTreeUpdater;

    // Stores the name of the game Object in order to recognize it:
    private string _name;



    // characteristics of the material
        
        // Temperature of the atmosphere (Kelvin)
        private float _Tatm;
        
        // Time in between each heat exchange
        private float _deltaT;

        // specific heat capacity
        private float _c;

        //standard reaction enthalpy (assuming Ellingham's approximation) (kJ/mol-1)
        private float delta_r_H0; //the minus is a caracteristic for an exothermic reaction

        
        // Thermal diffusivity (square meter by second)
        private float _d;

        // adapted Newton coefficient
        private float _h;

    public void ComputeNewTemp()
    {
        // Keeps track of the last heat calc
        _lastReaction = Time.time;

        // This explicit method is known to be numerically stable and convergent when  _d * _deltaT / (deltaZ ** 2) <= 1/2

        Tnew = 0;

        // Distance between voxels is the size of the voxel(we assume that if they are neighbors, they are colliding
        // Therefore deltaX depends on the dimension of the Voxel        
        float deltaX = transform.localScale.x ;
        float deltaY = transform.localScale.y;
        float deltaZ = transform.localScale.z;


       
        int dir = 0; // (+x,-x, +y, -y, +z, -z)

        foreach (GameObject neigh in neighbors) 
        { 
            
            
            if(neigh == null) // in this direction the neighbour is outside the structure
            {
                // Case where the neighbor is in (+x,-x)
                if(dir <= 1)
                {
                    Tnew += _h * (_Tatm - T) / deltaX;
                }// Case where the neighbor is in (+y, -y)
                else if(dir <= 3)
                {
                    Tnew += _h * (_Tatm - T) / deltaY;
                }// Case where the neighbor is in (+z, -z)
                else if(dir <= 5)
                {
                    Tnew += _h * (_Tatm - T) / deltaZ;
                }
            }      
            
            else // in this direction the neighbour is inside the structure
            {
                //float tempNeigh = neigh.GetComponent<ThermoBody>().GetT();

                float tempNeigh = 0;

                // Getting the temp of the neigh from the tree
                foreach (ThermoBody neigh1 in _refTreeUpdater.GetNeighbors(gameObject)) 
                {
                    if(neigh1.GetName() == neigh.name)tempNeigh = neigh1.GetT();
                }


                // Case where the neighbor is in (+x,-x)
                if(dir <= 1)
                {
                    Tnew += _d * (tempNeigh - T) / (deltaX * deltaX);
                }// Case where the neighbor is in (+y, -y)
                else if(dir <= 3)
                {
                    Tnew += _d * (tempNeigh - T) / (deltaY * deltaY);
                }// Case where the neighbor is in (+z, -z)
                else if(dir <= 5)
                {
                    Tnew += _d * (tempNeigh - T) / (deltaZ * deltaZ);
                }
            }
            // Update neighbour direction
            dir += 1;
        }
        
        // complete the equation
        Tnew = _deltaT * Tnew + T;
            
            /*
            // Temperature of the neighbor
            float tempNeigh;
            
            // If he has no neighbor on a certain direction, we consider him neighboring a voxel with T = Tatm
            if (neigh == null) 
            {
                tempNeigh = _Tatm;
            }
            else
            {
                tempNeigh = neigh.GetComponent<ThermoBody>().GetT();
            }

            // Case where the neighbor is in (+x,-x)
            if(i <= 1)
            {
                Tnew += _d * _deltaT * tempNeigh / (deltaX * deltaX);
            }// Case where the neighbor is in (+y, -y)
            else if(i <= 3)
            {
                Tnew += _d * _deltaT * tempNeigh / (deltaY * deltaY);
            }// Case where the neighbor is in (+z, -z)
            else if(i <= 5)
            {
                Tnew += _d * _deltaT * tempNeigh / (deltaZ * deltaZ);
            } 

            i += 1;
        }

        // We add the last expression to Tnew
        Tnew += (1 - 2 * _d * _deltaT * ( 1/(deltaX * deltaX) + 1/(deltaY * deltaY) + 1/ (deltaZ * deltaZ) )) * T;*/

    }

    public void Propagation()
    {
        // We are using hashSets because the .contains is executed in constant time
        HashSet<GameObject> visited = new HashSet<GameObject>();

        // Using a queue to store the visited Vertices is optimal in BreadthFirst graph traversal
        Queue queue = new Queue();

        //ThermoBody initialVertex = gameObject.GetComponent<ThermoBody>();

        GameObject initialVertex = gameObject;

        queue.Enqueue(initialVertex);
        visited.Add(initialVertex);

        while (queue.Count != 0) {

            // We get the first vertex in line and we visit his unvisited neighbors
            //ThermoBody vertex = (ThermoBody)queue.Dequeue();
            GameObject vertex = (GameObject)queue.Dequeue();
            
            // We get the neighbors of the current visited  Voxel vertex
            //LinkedList<ThermoBody> vNeighbors = _refTreeUpdater.GetNeighbors(vertex.gameObject);
            GameObject [] vNeighbors = vertex.GetComponent<Voxel>().voxelNeighbors;

            foreach (GameObject neigh in vNeighbors) 
            {

                // If we find an unvisited vertex we compute the new Temp, mark it and add it to the queue
                if (neigh != null && !visited.Contains(neigh)) 
                {
                    visited.Add(neigh);
                    queue.Enqueue(neigh);

                    neigh.GetComponent<ThermoBody>().ComputeNewTemp();
                    //neigh.ComputeNewTemp();

                    // Debug.Log("Nom du Voxel visité : " + neigh.name + " par " + vertex.name);
                }
            }
        }

        // We then Set all the voxels to their new Temp

        // var clone = new HashSet<Metal>(mList, mList.Comparer);
        foreach(GameObject entry in visited)
        {
            //entry.MajT();
            entry.GetComponent<ThermoBody>().MajT();
        }
    }

    public void MajT()
    {
        T = Tnew;
    }
    
    public void UpdateT()
    {
        // Calcul la nouvelle Temp à t+1
        ComputeNewTemp();

        // Remplace la temp actuelle par la nouvelle
        MajT();
    }

    void Start()
    {
        // Stores the name:
        _name = gameObject.name;
        
        // Getting the Tree Updater script:
        _refTreeUpdater = GameObject.FindWithTag("Platform").GetComponent<TreeUpdater>();

        // Getting the Metal script:
        _metal = GetComponent<Metal>();

        // Getting the thermo features:
        Thermo_Features tf = GetComponent<Thermo_Features>();

        _d = tf.GetD();
        _deltaT = tf.deltaT;
        _Tatm = tf.Tatm;
        _c = tf.GetC();
        delta_r_H0 = tf.Getdeltar_H();
        _h = tf.GetH();
        
        // initially everything temperature is equal to Tatm
        T = _Tatm;
        Tnew = _Tatm;

        // We get the neighbors of the current visited  Voxel
        neighbors = gameObject.GetComponent<Voxel>().voxelNeighbors;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Performance optimizer, does not compute for all metals !
        // if((T > (_Tatm + 0.1) && T > (_Tatm - 0.1)) && (Tnew > (_Tatm + 0.1) && Tnew > (_Tatm - 0.1)))
        {// Ice mat must change temp > 0.2 K otherwise no update will be done...
            if(Time.time >= _lastReaction + _deltaT)
            {
                UpdateT();
                _lastReaction = Time.time;

                // Updates the texture if we are displaying Temp
                if(_metal.isThermDisplayed)_metal.UpdateTextureThermals();
            }
            
            // Ensures that _delta is bigger than the frame rate !
            _deltaT = Mathf.Max(Time.deltaTime, 0.15f);
        }
    }

    public float GetT() 
    {
        return T;
    }

    public float Getc() 
    {
        return _c;
    }

    public float Getdeltar_H0() 
    {
        return delta_r_H0;
    }

    public string GetName()
    {
        return _name;
    }

    public void ChangeT(float temp){
        T = temp;
        Tnew = T;
    }
}
