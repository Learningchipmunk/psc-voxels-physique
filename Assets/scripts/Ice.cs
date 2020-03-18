using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ice : MonoBehaviour
{
    // temperature (kelvin)
        public float _temp = 270;

    // Specific heat capacities

        private float _capterm_ice = 2060;
        private float _capterm_met = 400;



    void OnCollisionEnter(Collision collisionInfo){
        if(collisionInfo.collider.tag.Contains("Metal")){
            Metal M = collisionInfo.gameObject.GetComponent<Metal>(); 
            float massmet = collisionInfo.rigidbody.mass;
            float newtemp = (gameObject.GetComponent<Rigidbody>().mass*_capterm_ice*_temp + massmet*_capterm_met*M._temp)/(gameObject.GetComponent<Rigidbody>().mass*_capterm_ice + massmet*_capterm_met);
            Debug.Log(newtemp);
            M._temp = newtemp;
            _temp = newtemp;
        }
    }

    void FixedUpdate(){
        if(_temp>273) Destroy(gameObject);
    }
}
