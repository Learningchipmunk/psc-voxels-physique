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

    
    
    // thermodynamic constants (will be replaced by an abstract class)
        // atmospheric temperature (kelvin)
        public const float Tatm = 300f;
        
        // temperature (kelvin)
        public float _temp ;

        // amount of oxidised metal at thermodynamic balance (mole); <= _nm
        private float _neq;

    // cinetic constants
        // The speed of the reaction (second)
        private float _deltaT;

        // The time of last reaction
        private float _lastReaction = 0f;

        // Frac and Fracmin enable to compute the reaction progress
        public const float Frac = 0.5f;
        public const float Fracmin = 0.01f;

        // useful constants to update _deltaT
        public const float Tmin = 240f;
        public const float Tmax = 350f;
        public const float DeltaTmin = 0.05f;
        public const float DeltaTatm = 0.3f;
        public const float Rate = (DeltaTatm - DeltaTmin)/(Tatm - Tmin);
        public const float DeltaTmax = Rate*(Tmax - Tmin) +DeltaTmin;
    
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
        _temp = Tatm;
        _deltaT = DeltaTatm;
    }


    public void UpdateDeltaT()
    {
        if(_temp <= Tmin)
        {
            _deltaT = DeltaTmin;
        }

        else if (_temp <= Tmax)
        {
            _deltaT = Rate*(_temp - Tmin) +DeltaTmin;
        }
        else
        {
            _deltaT = DeltaTmax;
        }
    }

    public void UpdateTexture()
    {
        if(!this.IsCompletelyCorroded())
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
            if(_nox >= _nm)
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
        _lastReaction = Time.time;
        // step is equivalent to the reaction progress
        float step = Mathf.Max(Frac*(_neq - _nox), Fracmin*_nm);
        _nox = Mathf.Min(_nox + step, _nm);
    }

    public void UpdateNeq(float delta) {
        _neq = Mathf.Min(_neq + delta, _nm);
    }

    public void UpdateMetal()
    {
        //if(Time.time >= lastReaction + _deltaT && s_a > 0)
        if(Time.time >= _lastReaction + _deltaT && !this.EqReached())
        {
            // Debug.Log(lastReaction);
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
