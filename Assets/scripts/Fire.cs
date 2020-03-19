using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{

    public float energy = 100f;
    
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

    void Start()
    {
        _startingTime = Time.time;
    }
    void OnCollisionEnter(Collision collisionInfo){

        if(collisionInfo.collider.tag.Contains("Metal"))
        { 
            // We only get the thermBody once to reduce the computation time
            ThermoBody collisionThermoBody = collisionInfo.gameObject.GetComponent<ThermoBody>();

            float massmet = collisionInfo.rigidbody.mass;
            float tempmet = collisionThermoBody.GetT();
            float cmet = collisionThermoBody.Getc();
            float newtemp = tempmet + (energy)/(massmet*cmet);
            collisionThermoBody.ChangeT(newtemp);
            
            
            //Debug.Log(newtemp);
            
            // Destroys the fire on impact !
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        TimeCheck();
    }
}
