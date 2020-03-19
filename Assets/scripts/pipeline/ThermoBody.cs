using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThermoBody : MonoBehaviour
{
    // Temperature (Kelvin)
    public float T;

    // Temperature à t+1 (Kelvin)
    public float Tnew;


    // Temperature of the atmosphere (Kelvin)
    public float Tatm = 300f;

    // Time in between each heat exchange
    private const float _deltaT = 0.15f;

    // The time of the last heat exchange
    private float _lastReaction = 0f;


    // characteristics of the material
        
        // Thermal conductivity
        public float k = 20;

        // specific heat capacity
        public float c = 1;

        // density
        public float rho = 1; // when Paul's modelisation is ready, it will be possible to compute the mass of the object thanks to rho
        
        // Thermal diffusivity (square meter by second)
        private float _d;
    

    public void ComputeNewTemp()
    {
        // Keeps track of the last heat calc
        _lastReaction = Time.time;

        // This explicit method is known to be numerically stable and convergent when  _d * _deltaT / (deltaZ ** 2) <= 1/2

        Tnew = 0;

        // Distance between voxels is the size of the voxel divided by 2(we assume that if they are neighbors, they are colliding
        // Therefore deltaX depends on the dimension of the Voxel        
        float deltaX = transform.localScale.x ;
        float deltaY = transform.localScale.y;
        float deltaZ = transform.localScale.z;


        GameObject [] neighbors = gameObject.GetComponent<Voxel>().voxelNeighbors;

        int i = 0; // (+x,-x, +y, -y, +z, -z) 

        foreach (GameObject neigh in neighbors) 
        { 
            // Temperature of the neighbor
            float tempNeigh;

            // If he has no neighbor on a certain direction, we consider him neighboring a voxel with T = Tatm
            if (neigh == null) 
            {
                tempNeigh = Tatm;
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
        Tnew += (1 - 2 * _d * _deltaT * ( 1/(deltaX * deltaX) + 1/(deltaY * deltaY) + 1/ (deltaZ * deltaZ) )) * T;

    }

    public void Propagation()
    {
        // We are using hashSets because the .contains is executed in constant time
        HashSet<GameObject> visited = new HashSet<GameObject>();

        // Using a queue to store the visited Vertices is optimal in BreadthFirst graph traversal
        Queue queue = new Queue();

        queue.Enqueue(gameObject);
        visited.Add(gameObject);

        while (queue.Count != 0) {

            // We get the first vertex in line and we visit his unvisited neighbors
            GameObject vertex = (GameObject)queue.Dequeue();
            
            // We get the neighbors of the current visited  Voxel vertex
            GameObject [] neighbors = vertex.GetComponent<Voxel>().voxelNeighbors;

            foreach (GameObject neigh in neighbors) 
            {

                // If we find an unvisited vertex we compute the new Temp, mark it and add it to the queue
                if (neigh != null && !visited.Contains(neigh)) 
                {
                    visited.Add(neigh);
                    queue.Enqueue(neigh);

                    neigh.GetComponent<ThermoBody>().ComputeNewTemp();

                    // Debug.Log("Nom du Voxel visité : " + neigh.name + " par " + vertex.name);
                }
            }
        }

        // We then Set all the voxels to their new Temp

        // var clone = new HashSet<Metal>(mList, mList.Comparer);
        foreach(GameObject entry in visited)
        {
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
        _d = k / (c*rho);

        // Augmentation de Rho pour l'équation de la chaleur, normalement les coef de diffusion thermique en m^2/s est du 10^-5 
        _d /= 1000; // à enlever quand c'est reglé
        
        // initially everything temperature is equal to Tatm
        T = Tatm;
        Tnew = Tatm;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Time.time >= _lastReaction + _deltaT)
        {
            UpdateT();
            _lastReaction = Time.time;
        }
    }

    public float GetT() 
    {
        return T;
    }

    public float Getc() 
    {
        return c;
    }

    public void ChangeT(float temp){
        T = temp;
        Tnew = T;
    }
}
