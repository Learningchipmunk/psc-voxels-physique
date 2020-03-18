using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pluie : MonoBehaviour
{
    int pluie = 0;
    public Rigidbody projectile;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown("b"))
        {
            pluie = - pluie + 1;
        }

        if(pluie==1){
        float x = 16*Random.value-6;
        float z = 16*Random.value-6;
        Rigidbody clone;
        Vector3 spawnVector = new Vector3(x,20,z);
        clone = Instantiate(projectile, spawnVector, new Quaternion(0,0,0,0));
        }
    }
}
