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

    bool isCompletelyCorroded = false;

    //Probility of being a solvant
    public const float P = 0.56f;

    // Number of atoms in the metal
    public const int N = 100;

    // Number of oxidized atoms in the metal
    public int no = 0;

    // Number of acid on the surface of the metal
    public int sa = 0;

    // Maximum number of acid on the surface of the metal
    int msa = 20;

    // The speed of the reaction
    float deltaT = 0.3f;

    // The time of last reaction
    public float lastReaction = 0f;


    public const float arrCube = 12f;

    public const float raideurInitial = 1000f;
    const float raideurOx = 900f;    
    public const float distanceInitialCoef = 1.25f;
    public const float distanceOxCoef = 1.1f;


    public float lambdaM = 10.2f;
    public float lambdaA;




    

    public void UpdateTexture()
    {
        // if(3 * no > N && no < N)
        // {
        //     visual.material = ErodedTexture;
        // }
        // else if(no == N && !isCompletelyCorroded)
        // {
        //     float p =  Random.Range(0.0f, 1.0f);
        //     if(p <= P)
        //     {
        //         visual.enabled = false;
        //         Destroy(gameObject);
        //     }
        //     else{
        //         visual.material = CompletelyEroded;
        //         isCompletelyCorroded = true;
        //     }
        // }
        if(!isCompletelyCorroded)
        {
            //Get the Renderer component from the new cube
            var cubeRenderer = visual;

            //Get the proportion of the non-eroded atoms.
            float prop = (float)no/N;
            
            //for prop = 1, the triplet (R, G, B) defines the color Dark Brown
            float R = 1 - prop * (1.0f - 0.296f);
            float G = 1 - prop * (1.0f - 0.163f);
            float B = 1 - prop * (1.0f - 0.029f);

            //Call SetColor using the shader property name "_Color" and setting the color to (R, G, B)
            cubeRenderer.material.SetColor("_Color", new Color(R, G, B, prop/2));

            //Random Draw that decides if the acid is destroyed or passivated.    
            if(no == N)
            {
                isCompletelyCorroded = true;
                
                //Uniform distribution
                float p =  Random.Range(0.0f, 1.0f);
                if(p <= P)
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
        this.referenceVoxel.breakingDistanceCoef = (no * (distanceOxCoef - distanceInitialCoef) + (N - no) * distanceInitialCoef) / N;
    }

    public void UpdateRaideur()
    {
        this.referenceVoxel.k = (no * (raideurOx - raideurInitial) + (N - no) * raideurInitial) / N;
    }

    public int UpdateNo(int sa, float lambdaA)
    {
        // if sa == 0 there is no reaction
        if(sa == 0)return 0;

        // We store the reaction time
        lastReaction = Time.time;
        // Mesure the Delta
        int delta = no;

        //Typical model.
        //no = Mathf.Min(N, no + np);

        //Lambda model by Nono.
        no = Mathf.Min(N, no + (sa) / (int)(lambdaM - lambdaA) + 1);

        // Compute the delta
        delta = no - delta;


        // Remove the acid particules that reacted with the metal
        return delta;

    }

    // Decreases the number of acids that are around the metal
    public void UpdateSa(int decrease)
    {
        sa = Mathf.Max(0 , sa - decrease);
    }

    public int FillSa(int np, float lambdaA)
    {
        // We record the time of the first collision to monitor the reaction
        if(sa == 0)lastReaction = Time.time;


        // Assigning lambdaA to the metal
        this.lambdaA = lambdaA;

        // We strore the initial value of sa to know how much acid was absorbed
        int valInitiale = sa;

        // There can't be more acid on the surface (absorbed) than msa
        sa = Mathf.Min(msa, sa + np);

        // We return the amount of acid absorbed
        return sa - valInitiale;
    }

    
    public int GetN()
    {
        return N;
    }
    public int GetNo()
    {
        return no;
    }

    public int GetSa()
    {
        return sa;
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
        UpdateSa(UpdateNo(sa, lambdaA));
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
        // 0 atoms have been eroded initialy
        no = 0;

    }

    public void UpdateState(float T, float P){
   
    }

    public void UpdateMetal()
    {
        if(Time.time >= lastReaction + deltaT && sa > 0)
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
