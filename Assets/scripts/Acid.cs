using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Acid : MonoBehaviour
{
    // Reference to the platform that stores the metal encountered
    GameObject referenceObject;

    // Stores the Script Ref
    MetalUpdater referenceScript;

    // To store the MeshRenderer and change the color of the acid
    private MeshRenderer visual;

    // volume (cubic meter)
    public float V;
    public float lambdaA = 7.2f;

    // acid concentration (mole per cubic meter)
    public const float C_Ini = 10000.0f;

    // acid amount (mole) : initialised with C_ini and V
    public float na;


    // Creating a timer to destroy the acid after 5 seconds of creation.
    float startingTime;
    // EvaporationTime 
    float evaporationTime = 8f;

    // Giving a random factor to time.
    float P = 0.5f;


    // Removing the acid that was absorbed by the metal
    void Update_na(float used) {
        na = Mathf.Max(0, na - used);
    }

    // Destroys the acid if it has been created for more than 6 seconds
    void TimeCheck()
    {
        // Normal acid evaporation.

        // if(Time.time - startingTime >= 6)
        // {
        //     Destroy(gameObject); 
        // }
        
        // Randomisation of the acid's destruction.
        if(Time.time - startingTime >= evaporationTime)
        {
            float p =  Random.Range(0.0f, 1.0f);
            if(p <= P)
            {
                Destroy(gameObject); 
            }
            P += 0.1f;
            evaporationTime += 0.5f;
        }else if(Time.time - startingTime >= 8.5 && P == 1)
        {
            Destroy(gameObject); 
        }
    }

    void OnCollisionEnter(Collision collisionInfo) {
        if(collisionInfo.collider.tag.Contains("Metal"))
        {
            
            // Getting info on the metal.
            Metal M = collisionInfo.gameObject.GetComponent<Metal>();
            float nox = M.GetNox();
            float Nm = M.GetNm();

            // Adding the metal to the metal list
            referenceScript.addMetal(M);

            //Udating the parameters of the corrosion.
            Update_na(M.FillS_a(na, lambdaA));
            M.UpdateDistance();
            M.UpdateRaideur();

            // Updating the texture of the metal
            M.UpdateTexture();

            // Checking if the metal was completely correded. If so we destroy the acid.
            bool isCompletelyCorroded = M.getIsCompletelyCorroded();// to move if not realistic            
            
            // Destroys the Acid
            if(isCompletelyCorroded || na == 0)
            {
                visual.enabled = false;
                Destroy(gameObject);
            }
            //movement.enabled = false;
        }
    }

    void OnCollisionStay(Collision collisionInfo) 
    {
        foreach (ContactPoint contact in collisionInfo.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }


        if(collisionInfo.collider.tag.Contains("voxel"))
        {
            // Getting the info on the Metal 
            Metal M = collisionInfo.gameObject.GetComponent<Metal>();
            
            // filling the acid tank
            Update_na(M.FillS_a(na, lambdaA));
            
            // Adding the metal to the metal list
            referenceScript.addMetal(M);


            // Checking if the metal was completely correded. If so we destroy the acid.
            bool isCompletelyCorroded = M.getIsCompletelyCorroded();// to move if not realistic            
            
            // Destroys the Acid
            if(isCompletelyCorroded || na == 0)
            {
                visual.enabled = false;
                Destroy(gameObject);
            }
        }
    }

    private void Awake() 
    {
        // mf = this.GetComponent<MeshFilter>();
        
        // var theMesh = new MeshFilter();
        // GameObject go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        // theMesh = go.GetComponent<MeshFilter>();
        // Mesh mesh = Instantiate (theMesh.mesh) as Mesh;
        // if(mesh == null)
        // {
        //       Debug.Log("theMesh is null"); ;
        // }
        // else
        // {
        //       Debug.Log("GOT THE MESH!");
        // }
        // mf.sharedMesh = mesh;
        // Destroy(go);


        // Changes the scale of the acid :
        //this.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

    }

    void Start()
    {
        // Get the platform by its name
        // referenceObject = GameObject.Find("Platform");

        //Get the platform by its tag
        referenceObject = GameObject.FindWithTag("Platform");


        // Get The metal updater Script
        referenceScript = referenceObject.GetComponent<MetalUpdater>();

        // Get the MeshRenderer and storing it
        visual = GetComponent<MeshRenderer>();

        // Setting the color of the acid to Green in (R, G, B)
        // float R = 0.05f;
        // float G = 0.8f;
        // float B = 0.05f;

        // Call SetColor using the shader property name "_Color" and setting the color to Green
        // visual.material.SetColor("_Color", new Color(R, G, B, 1f));


        // Starting the timer to track the lifetime of the acid
        startingTime = Time.time;

        // Tagging the acid
        tag = "Acid";

        // Calculating the volum of the acid
        V = this.transform.localScale.x * this.transform.localScale.y * this.transform.localScale.z ;
        na = V*C_Ini;
    }

    void FixedUpdate()
    {
        TimeCheck();
    }

}
