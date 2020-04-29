using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thermo_Features : MonoBehaviour
{
    // Temperature of the atmosphere (Kelvin)
    public float Tatm = 300f;

    // Time in between each heat exchange
    public float deltaT = 0.15f;

    // specific heat capacity kJ/(K*kg)
    public const float C = 0.5f;

    public float CBoost = 1f;

    public float GetC() {
        return C * CBoost;
    }

    // thermal diffusivity
    public const float D = 0.00001f;

    public float DBoost = 1f;

    public float GetD() {
        return D*DBoost;
    }

    // adapted newton-coefficient : h / (rho*C)
    public const float h = 0.002f;
    public float hBoost = 1f;

    public float GetH() {
        return h*hBoost;
    }

    // thermo-chemical constants
    public const float deltar_H = -111f; //kJ.mol-1
    public float deltar_Hboost = 0.5f;

    public float Getdeltar_H(){
        return deltar_H * deltar_Hboost;
    }
}
