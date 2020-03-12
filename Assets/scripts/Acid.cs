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

    // chemical constants
        // acid concentration (mole per cubic meter)
        public float c_ini = 10000.0f;

        // acid amount (mole) : initialised with C_ini
        private float _na_ini;
        private float _na;
        
        // maximum fraction of acid transferred
        public  const float Fracmax = 0.4f;

    // acid deleting constants
        // Creating a timer to destroy the acid after 5 seconds of creation.
        private float _startingTime;
        // EvaporationTime 
        private float _evaporationTime = 8f;

        // Giving a random factor to time.
        private float _p = 0.5f;


    // Removing the acid that was absorbed by the metal
    void Update_na(float used) {
        _na = Mathf.Max(0, _na - used);
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
        if(Time.time - _startingTime >= _evaporationTime)
        {
            float p =  Random.Range(0.0f, 1.0f);
            if(p <= _p)
            {
                Destroy(gameObject); 
            }
            _p += 0.1f;
            _evaporationTime += 0.5f;
        }else if(Time.time - _startingTime >= 8.5 && _p == 1)
        {
            Destroy(gameObject); 
        }
    }

    void OnCollisionEnter(Collision collisionInfo) {
        if(collisionInfo.collider.tag.Contains("Metal"))
        {
            
            // Getting info on the metal.
            Metal M = collisionInfo.gameObject.GetComponent<Metal>();
            float neq = M.GetNeq();
            float nm = M.GetNm();

            float delta_na = Mathf.Min(nm - neq, Fracmax*_na_ini, _na);

            Update_na(delta_na);
            // triggering corroding updates
            M.UpdateNeq(delta_na);
            referenceScript.addMetal(M);
            
            if(_na == 0)
            {
                visual.enabled = false;
                Destroy(gameObject);
            }
            
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
            float neq = M.GetNeq();
            float nm = M.GetNm();

            float delta_na = Mathf.Min(nm - neq, Fracmax*_na_ini, _na);

            Update_na(delta_na);
            // triggering corroding updates
            M.UpdateNeq(delta_na);
            referenceScript.addMetal(M);
            
            // Destroys the Acid
            if(_na == 0)
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
        _startingTime = Time.time;

        // Tagging the acid
        tag = "Acid";

        // Calculating the volum (cubic meter) of the acid
        float v = this.transform.localScale.x * this.transform.localScale.y * this.transform.localScale.z ;
        _na_ini = c_ini*v;
        _na = _na_ini; 
    }

    void FixedUpdate()
    {
        TimeCheck();
    }

}
