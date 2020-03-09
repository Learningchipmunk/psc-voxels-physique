using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel : MonoBehaviour
{
    public GameObject[] voxelNeighbors = new GameObject[6]; // (+x,-x, +y, -y, +z, -z) 
    public int neighborsNumber;
    public float k; // constant in the hook law

    // public float alphaRotation = 10f;
    public float alphaTranslation;
    
    public float breakingDistanceCoef;


    // ------------------ Awake and Update function --------------- //

void Awake(){
    this.k = 1000f;
    this.alphaTranslation = 10f;
    this.breakingDistanceCoef = 1.1f;
}

private void FixedUpdate() {
    // update the applied force to this object (so we can move other object with us)
    if (this.neighborsNumber != 0){
        this.HookeForce();
    }
}

public void ResetNeighbors(){
    int k = 0;
    Debug.Log(this.name + "( parent : " + this.transform.parent.name + " ) is now alone");
    foreach (GameObject neigh in this.voxelNeighbors){
        if (neigh!=null){
            neigh.GetComponent<Voxel>().SetNeighbor(f(k),null);
            neigh.GetComponent<Voxel>().neighborsNumber = neigh.GetComponent<Voxel>().neighborsNumber - 1 ;
        }
        k=k+1;
    }
    this.voxelNeighbors = new GameObject[6];
    this.neighborsNumber = 0;
}

public void ResetNeighbor(int k){
    // k : indice of the neighbor
    // this function is a new one that allows to only break a link between the voxel and one neighbor, not all of them.
    this.voxelNeighbors[k].GetComponent<Voxel>().SetNeighbor(f(k),null);
    this.voxelNeighbors[k].GetComponent<Voxel>().neighborsNumber = this.voxelNeighbors[k].GetComponent<Voxel>().neighborsNumber - 1 ;
    this.voxelNeighbors[k] = null;
    this.neighborsNumber = this.neighborsNumber - 1;
}
// f gives the position of "this" in the neighbor.voxelNeighbors gameObject[] so we can delete it in this array.
int f(int i){
    if (i==0){
        return 1;
    }
    else if(i==1){
        return 0;
    }
    else if(i==2){
        return 3;
    }
    else if(i==3){
        return 2;
    }
    else if(i==4){
        return 5;
    }
    else {
        return 4;
    }
}


// used in VoxelsJoint.cs
public void SetNeighbor(int pos, GameObject neighbor){
    // init already done
    if (neighbor == null){
        this.voxelNeighbors[pos] = null;
    }
    else{
        if (this.voxelNeighbors[pos] == null){
            this.voxelNeighbors[pos] = neighbor;
            this.neighborsNumber +=1;
        }
        else {
            // we have to compare if it's closer than the current neighbor to the current voxel or not.
            
            Vector3 dPos1;
            Vector3 dPos2;
            // simpler :
            dPos1 = neighbor.transform.position - this.transform.position;
            dPos2 = this.voxelNeighbors[pos].transform.position - this.transform.position; 

            /*
            float dPos1 = 0;
            float dPos2 = 0;
            
            if (pos < 2){
                dPos1 = Mathf.Abs(neighbor.transform.position.x - this.transform.position.x);
                dPos2 = Mathf.Abs(this.voxelNeighbors[pos].transform.position.x - this.transform.position.x); 
            }
            else if (pos < 4){
                dPos1 = Mathf.Abs(neighbor.transform.position.y - this.transform.position.y);
                dPos2 = Mathf.Abs(this.voxelNeighbors[pos].transform.position.y - this.transform.position.y); 
            }
            else {
                dPos1 = Mathf.Abs(neighbor.transform.position.z - this.transform.position.z);
                dPos2 = Mathf.Abs(this.voxelNeighbors[pos].transform.position.z - this.transform.position.z); 
            } */

            // we check which neighbor is closer to the current voxel.
            if ( dPos1.magnitude < dPos2.magnitude ){ // the new neighbor is closer than the previous one. So we replace the current neighbor by the new one.
                this.voxelNeighbors[pos] = neighbor;
            }
        }   
    }
}
public int GetNeighborsNumber(){
    return this.neighborsNumber;
}

void HookeForce(){
    int k = 0;
    foreach (GameObject neigh in this.voxelNeighbors){
        if (neigh != null){
            Vector3 hookForce = this.HookForce(neigh,k);
            this.GetComponent<Rigidbody>().AddForce(hookForce + this.TranslationFriction());
        }
        k++;
    }
}

    // ---------------- Physics Functions (addForce etc.) --------------- //

    

    // we only use the hookforce, no pendulum torsion.
    public Vector3 HookForce(GameObject neighbor, int k){
        // maybe it would be good to set an offset to the HookForce so there is less force when the 2 bodies are really close even if so far it's good. But that would make for better behaviors.
        // return voxel1.GetComponent<Voxel>().k * (voxel2.transform.position - voxel1.transform.position);
        Vector3 dPos = (neighbor.transform.position - this.transform.position);
        if (Mathf.Abs(dPos.x) > this.breakingDistanceCoef*this.transform.localScale.x || Mathf.Abs(dPos.y) > this.breakingDistanceCoef*this.transform.localScale.y || Mathf.Abs(dPos.z) > this.breakingDistanceCoef*this.transform.localScale.z){
            this.ResetNeighbor(k);
            Debug.Log(this.name + " " + this.transform.parent.name + " was too far from " + neighbor.name + " " + neighbor.transform.parent.name);
            return new Vector3(0,0,0);
        }
        float avg_k = this.k*neighbor.GetComponent<Voxel>().k / (this.k+neighbor.GetComponent<Voxel>().k);
        return avg_k * dPos;
    }

    // public Vector3 TorsionPendulum(GameObject voxel1, GameObject voxel2){
    //     Vector3 currentRotation = voxel2.transform.eulerAngles - voxel1.transform.eulerAngles;
    //     // return voxel1.GetComponent<Voxel>().C * currentRotation;
    //     return this.C * currentRotation;
    // }

    public Vector3 TranslationFriction(){
        return - this.alphaTranslation * this.GetComponent<Rigidbody>().velocity;
    }
//     public Vector3 RotationFriction(GameObject voxel){
//         return - this.alphaRotation * voxel.GetComponent<Rigidbody>().angularVelocity;
//     }

}
