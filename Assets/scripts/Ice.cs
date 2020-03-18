using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ice : MonoBehaviour
{
    // temperature (kelvin)
        public float _T = 270;

    // Specific heat capacities

        private float c = 2;

    void OnCollisionEnter(Collision collisionInfo){

        if(collisionInfo.collider.tag.Contains("Metal")){ 
            float massmet = collisionInfo.rigidbody.mass;
            float tempmet = collisionInfo.gameObject.GetComponent<ThermoBody>().GetT();
            float cmet = collisionInfo.gameObject.GetComponent<ThermoBody>().Getc();
            float newtemp = (gameObject.GetComponent<Rigidbody>().mass*c*_T + massmet*cmet*tempmet)/(gameObject.GetComponent<Rigidbody>().mass*c + massmet*cmet);
            collisionInfo.gameObject.GetComponent<ThermoBody>().ChangeT(newtemp);
            _T = newtemp;
        }
    }

    void FixedUpdate(){
        if(_T>273) Destroy(gameObject);
    }
}
