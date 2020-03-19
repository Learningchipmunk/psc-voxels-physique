using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThermoBody : MonoBehaviour
{
    // Temperature (Kelvin)
    public float T;

    // Temperature of the atmosphere (Kelvin)
    public float Tatm = 300f;


    // characteristics of the material
        // Thermal conductivity
        public float k = 20;

        // specific heat capacity
        public float c = 1;

        // density
        public float rho = 1; // when Paul's modelisation is ready, it will be possible to compute the mass of the object thanks to rho
        
        // Thermal diffusivity (square meter by second)
        private float _d;
    
    
    public void UpdateT()
    {
        // Heat equation
    }

    void Start()
    {
        _d = k / (c*rho);
        
        // initially everything temperature is equal to Tatm
        T = Tatm;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateT();
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
    }
}
