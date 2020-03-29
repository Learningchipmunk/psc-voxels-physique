using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metal_Features : MonoBehaviour
{   
    // concentration de metal (moles / m3)
    public const float C_m = 100000f;

    public float C_mboost = 0.005f;

    public float GetC_m() {
        return C_m * C_mboost;
    }

    // Probability of being a solvant
    public float p_solv = 0.5f;

    // mecanical constants
        public float raideurInitial = 1000f;
        public float raideurOx = 900f;    
        public float distanceInitialCoef = 1.25f;
        public float distanceOxCoef = 1.1f;

    // cinetic constants
        public float T1 = 250f;
        public float T2 = 300f;
        public float T3 = 500f;
        public float P1 = 0.05f;
        public float P2 = 0.4f;
        public float P3 = 0.95f;
        
        // duration between two steps of the reaction (second)
        public float DeltaT = 0.2f;

        // Fracmin is the minimum part of _nm oxidised at each step
        public float Fracmin = 0.01f;
    
    }
