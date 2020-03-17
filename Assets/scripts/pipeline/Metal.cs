using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metal : MonoBehaviour
{

    // Stores the Script Ref of MetalUpdater
    MetalUpdater referenceScript;

    // Stores the Script Ref of Voxel
    Voxel referenceVoxel;

    private MeshRenderer visual;

    //Probability of being a solvant
    public float p_solv = 0.5f;

    // chemical caracteristics
        // metal concentration (mole per cubic meter)
        public float c_m = 500.0f;

        // amount of metal (mole)
        private float _nm;
    
        // current amount of oxidised metal (mole); <= _nm
        private float _nox;

    // mecanical constants
        public const float raideurInitial = 1000f;
        public const float raideurOx = 900f;    
        public const float distanceInitialCoef = 1.25f;
        public const float distanceOxCoef = 1.1f;

    // thermodynamic constants (some will be replaced by an abstract class)
        // atmospheric temperature (kelvin)
        public const float Tatm = 300f;
        
        // temperature (kelvin)
        public float temp ;

        // amount of oxidised metal at thermodynamic balance (mole); <= _nm
        private float _neq;

    // cinetic constants
        // range of the evolution of cinetic
        private const float Tmin = 250f;
        private const float Tmax = 500f;
        
        // duration between two steps of the reaction (second)
        private const float DeltaT = 0.2f;

        // The time of the last reaction
        private float _lastReaction = 0f;

        // Frac and Fracmin enable to compute the reaction progress
        // prog is the percentage of progress at each step
        private float _prog = 0.5f; // prog is the percentage of progress at each step
        private const float Progmin = 0.3f;
        private const float Progmax = 0.9f;
        private const float Progatm = 0.5f;
        private const float Rate_prog1 = (Progatm - Progmin)/(Tatm - Tmin);
        private const float Rate_prog2 = (Progmax - Progatm)/(Tmax - Tatm);

        public const float Fracmin = 0.01f; // Fracmin is the minimum part of _nm oxidised at each step 

    
    private void Awake() 
    {
        // Getting the meshrenderer of this gameobject
        visual = this.GetComponent<MeshRenderer>();
        // name = "Iron";

        // tagging the metal
        tag = "Metal";
        
        // loading textures in C# file
        Material Iron = Resources.Load("Iron", typeof(Material)) as Material;
        visual.material = Iron;
    }

    void Start()
    {

        // Gets the script Voxel
        this.referenceVoxel = this.GetComponent<Voxel>();

        // To find the platform that stores the lists of metals
        referenceScript = GameObject.FindWithTag("Platform").GetComponent<MetalUpdater>();
        
        // volume (cubic meter) and amount of metal
        float v = this.transform.localScale.x * this.transform.localScale.y * this.transform.localScale.z ;
        _nm = c_m*v;

        // 0 atoms have been eroded initially
        _nox = 0f;
        
        // 0 acid initially
        _neq = 0f;

        // temp = Tatm
        temp = Tatm;
    }

    public void UpdateProg() 
    {
        if(temp <= Tmin)
        {
            _prog = Progmin;
        }
        else if (temp <= Tatm)
        {
            _prog = Progmin + Rate_prog1*(temp - Tmin);
        }
        else if (temp <= Tmax)
        {
            _prog = Progatm + Rate_prog2*(temp - Tatm);
        }
        else
        {
            _prog = Progmax;
        }
    }

    public void UpdateTexture()
    {
        //Get the Renderer component from the new cube
        var cubeRenderer = visual;

        //Get the proportion of the non-eroded atoms.
        float prop = (float)_nox/_nm;
        
        //for prop = 1, the triplet (R, G, B) defines the color Dark Brown
        float R = 1 - prop * (1.0f - 0.296f);
        float G = 1 - prop * (1.0f - 0.163f);
        float B = 1 - prop * (1.0f - 0.029f);

        //Call SetColor using the shader property name "_Color" and setting the color to (R, G, B)
        cubeRenderer.material.SetColor("_Color", new Color(R, G, B, prop/2));

        //Random Draw that decides if the metal is destroyed or passivated.    
        if(IsCompletelyCorroded())
        {
            //Uniform distribution
            float p =  Random.Range(0.0f, 1.0f);
            if(p <= p_solv)
            {
                // When destoryed it remove itself from the list of metals in Platform
                referenceScript.removeMetal(this.GetComponent<Metal>());

                // Removes the Mesh
                visual.enabled = false;

                // Destroys the game object
                Destroy(gameObject);
            }
        }   
    }


    public void UpdateDistance()
    {
        this.referenceVoxel.breakingDistanceCoef = (_nox * (distanceOxCoef - distanceInitialCoef) + (_nm - _nox) * distanceInitialCoef) / _nm;
    }

    public void UpdateRaideur()
    {
        this.referenceVoxel.k = (_nox * (raideurOx - raideurInitial) + (_nm - _nox) * raideurInitial) / _nm;
    }

    public void UpdateNox() {
        // recording that a new reaction starts
        _lastReaction = Time.time;
        
        // step is the reaction progress made at each time step
        // Fracmin*_nm ensures that the reaction ends in a finite time
        float step = Mathf.Max(_prog * ( _neq - _nox), Fracmin*_nm);
        
        // ensures that _nox <= eq
        _nox = Mathf.Min(_nox + step, _neq);
    }

    public void UpdateNeq(float delta) {
        _neq = Mathf.Min(_neq + delta, _nm);
    }

    public void UpdateMetal()
    {
        //if(Time.time >= lastReaction + _deltaT && s_a > 0)
        if(Time.time >= _lastReaction + DeltaT && !this.EqReached())
        {
            UpdateProg();
            
            // Trigger a reaction
            UpdateNox();

            // Updates the values of the parameters of the metal
            UpdateDistance();
            UpdateRaideur();

            // Updating the texture of the metal
            UpdateTexture();
        }
    }

    public float GetNm()
    {
        return _nm;
    }

    public float GetNeq() 
    {
        return _neq;
    }

    public bool EqReached()
    {
        return _nox >= _neq;
    }

    public bool IsCompletelyCorroded()
    {
        return _nox >= _nm;
    }

}
