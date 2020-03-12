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

    private bool isCompletelyCorroded = false;

    //Probility of being a solvant
    public float P_solv = 0.56f;


    // Number of acid on the surface of the metal
    private float s_a;

    // Maximum number of acid on the surface of the metal
    private float M_sa;

    // volume (cubic meter)
    private float V;
    
    // metal concentration (mole per cubic meter)
    public float C_m = 500.0f;

    // amount of metal (mole)
    private float _nm;
    
    // amount of oxidised metal (mole); <= nm
    private float _nox;

    // The time of last reaction
    private float lastReaction = 0f;

    public const float raideurInitial = 1000f;
    public const float raideurOx = 900f;    
    public const float distanceInitialCoef = 1.25f;
    public const float distanceOxCoef = 1.1f;

    // cinetic constants
    public const float Frac = 0.01f;
    
    private float _neq;
    public float lambdaM = 10.2f;
    public float lambdaA;


    // atmospheric temperature (kelvin)
    public const float Tatm = 300f;
    // temperature (kelvin)
    private float _temp ;

    // The speed of the reaction (second)
    private float _deltaT;

    // useful constants to update _deltaT
    public const float Tmin = 240f;
    public const float Tmax = 350f;
    public const float DeltaTmin = 0.05f;
    public const float DeltaTatm = 0.3f;
    public const float Rate = (DeltaTatm - DeltaTmin)/(Tatm - Tmin);
    public const float DeltaTmax = Rate*(Tmax - Tmin) +DeltaTmin;
    
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
        if(!isCompletelyCorroded)
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

            //Random Draw that decides if the acid is destroyed or passivated.    
            if(_nox >= _nm)
            {
                isCompletelyCorroded = true;
                
                //Uniform distribution
                float p =  Random.Range(0.0f, 1.0f);
                if(p <= P_solv)
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

    public float UpdateNox(float lambdaA) {
        // if sa == 0 there is no reaction
        if(s_a <= 0)return 0;

        // We store the reaction time
        lastReaction = Time.time;
        // Mesure the Delta
        float delta = _nox;

        //Typical model.
        //no = Mathf.Min(N, no + np);

        //Lambda model by Nono.
        _nox = Mathf.Min(_nm, _nox + s_a / (int)(lambdaM - lambdaA) + 1);

        // Compute the delta >=0
        delta = _nox - delta;


        // Remove the acid particules that reacted with the metal
        return delta;
    }

    // Decreases the number of acids that are around the metal
    public void UpdateS_a(float decrease)
    {
        s_a = Mathf.Max(0 , s_a - decrease);
    }

    public float FillS_a(float na, float lambdaA)
    {
        // We record the time of the first collision to monitor the reaction
        if(s_a <= 0)lastReaction = Time.time;


        // Assigning lambdaA to the metal
        this.lambdaA = lambdaA;

        // We store the initial value of sa to know how much acid was absorbed
        float v_ini = s_a;

        // There can't be more acid on the surface (absorbed) than msa
        s_a = Mathf.Min(M_sa, s_a + na);

        // We return the amount of acid absorbed
        return s_a - v_ini;
    }

    
    public float GetNm()
    {
        return _nm;
    }
    public float GetNox()
    {
        return _nox;
    }

    public float GetS_a()
    {
        return s_a;
    }

    public float getLambdaM()
    {
        return lambdaM;
    }

    public bool getIsCompletelyCorroded()
    {
        return isCompletelyCorroded;
    }

    void Oxidisation()
    {
        UpdateS_a(UpdateNox(lambdaA));
    }

    


    private void Awake() 
    {
        // Getting the meshrenderer of this gameobject
        visual = this.GetComponent<MeshRenderer>();
        // name = "Iron";

        // tagging the metal
        tag = "Metal";
        
        // Changing the size
        //this.transform.localScale = new Vector3(arrCube, arrCube, arrCube);

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
        
        // volume and amount of metal
        V = this.transform.localScale.x * this.transform.localScale.y * this.transform.localScale.z ;
        _nm = V*C_m;
        M_sa = _nm/5;

        // 0 atoms have been eroded initially
        _nox = 0f;
        
        // 0 acid initially
        //_neq = 0f;
        s_a = 0f;

        // temp = Tatm
        _temp = Tatm;
        _deltaT = DeltaTatm;
    }

    public void UpdateMetal()
    {
        if(Time.time >= lastReaction + _deltaT && s_a > 0)
        //if(Time.time >= lastReaction + _deltaT && _neq-_nox > 0)
        {
            // Debug.Log(lastReaction);
            // Trigger a reaction
            Oxidisation();

            // Updates the values of the parameters of the metal
            UpdateDistance();
            UpdateRaideur();

            // Updating the texture of the metal
            UpdateTexture();
        }
    }


}
