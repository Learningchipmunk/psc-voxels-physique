using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ice : MonoBehaviour
{

    // Rigidbody stored in a variable
    private Rigidbody _body;



    // temperature (kelvin)
    public float T = 270;

    // Specific heat capacities

    private float _c = 2;

    private float _startingTime;
    //extinction time
    private float _extinctionTime = 8f;

    // Giving a random factor to time.
    private float _p = 0.5f;    

    void TimeCheck()
    {
        if(Time.time - _startingTime >= _extinctionTime)
        {
            float p =  Random.Range(0.0f, 1.0f);
            if(p <= _p)
            {
                Destroy(gameObject); 
            }
            _p += 0.1f;
            _extinctionTime += 0.5f;
        }else if(Time.time - _startingTime >= 8.5 && _p == 1)
        {
            Destroy(gameObject); 
        }
    }

    void Awake()
    {
        

        // Getting the Rigidbody
        _body = gameObject.GetComponent<Rigidbody>();


        _startingTime = Time.time;
    }

    void OnCollisionEnter(Collision collisionInfo){

        if(collisionInfo.collider.tag.Contains("Metal"))
        { 
            float massmet = collisionInfo.rigidbody.mass;

            // We only get the thermBody once to reduce the computation time
            ThermoBody collisionThermoBody = collisionInfo.gameObject.GetComponent<ThermoBody>();
            float tempmet = collisionThermoBody.GetT();
            float cmet = collisionThermoBody.Getc();

            // We compute the new temp after the collision with a simple thermodynamics equation : C1 * T1 + C2 * T2 = (C1+C2) * T  
            float newtemp = (_body.mass*_c*T + massmet*cmet*tempmet)/(_body.mass*_c + massmet*cmet);

            // We then change the temp of the voxel :
            collisionThermoBody.ChangeT(newtemp);
            T = newtemp;

            // Propagates the temp to other voxels :
            collisionThermoBody.Propagation();
            
            // To slow, TO DO : find bettter constants (Ronan)
            // Debug.Log(newtemp);
        }
    }

    void FixedUpdate(){
        if(T>273) Destroy(gameObject);
        TimeCheck();
    }
}
