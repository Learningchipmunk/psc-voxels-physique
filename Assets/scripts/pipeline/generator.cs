using UnityEngine;
// using myClass.cs;
//// <summary>
//// Place the voxel at the provided location
//// </summary>

// isues here : 
//  - we recreate at each frame a lots of things/
//  - no breakforce yet
//  - how to deal with multiple created class in the voxels boss ??
public class generator : MonoBehaviour {
     // trying this time to create some feature from scratch.
    // public Vector3 size; // works in 3D _ lenght wanted.
    public int x,y,z; // y is the hight
    public GameObject modele; // the prefab
    public Vector3 position; // initial position

    // since we do not have a better alternative so far, we're going to use a int matrix just to know if there is a link to neighbors
    
    // for myjoints
    public string bodyName; 
    // public Vector3 orientation;
       

    void Start(){
        this.InitAllNeighbors();
    }
    public void InitAllNeighbors(){
        // we are going to init all neighbors. To do that, we first need to select all voxels. 1st : select the children structures, 2nd : select the children of thoses structures
        foreach (Transform childTransform in this.transform){
           // Debug.Log(childTransform.gameObject.name); 
            foreach(Transform voxelTransform in childTransform){
             //   Debug.Log(voxelTransform.gameObject.name);
                InitNeighbors(voxelTransform); // we add the neighbors of the given child tranform.
            }
        }
    }


    public void InitNeighbors(Transform transform){
        
        // the issue with such a radius (max(Scale)) is that it does not work for no-cubic voxel (or does not work as well as it could)
        //Collider[] hitColliders = Physics.OverlapBox(transform.position, 2*transform.localScale); // lossyScale is the global scale of the object (read only)
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.60f * Mathf.Max(transform.localScale.x,transform.localScale.y,transform.localScale.z));

        
        foreach (Collider neighColliders in hitColliders){

            // we have to know the type of the neighbor. 
            // so we are looking for neighbors of the same tag as the current voxel.         
            if (neighColliders.gameObject.tag == transform.gameObject.tag){
                
                Vector3 dPosition = neighColliders.transform.position - transform.position; // difference between the two tranform.position.
                
                // the axis along which we have the biggest difference is the one we are interested into. 
                int axisInd = SelectInd(dPosition); 
                // axisInd = -1, if the collider is the one of the current voxel. See script below.
                if (axisInd >=0 ){
                    // we print what we are doing (easier to sport if there is a pb)
                    Debug.Log(neighColliders.gameObject.name + " " + neighColliders.transform.parent.name + " is added to " + transform.gameObject.name + " " + transform.parent.name + " in position " + axisInd);
                    // we can have a problem if there is already a neighbors which has been detected and added. This issue is solved directly by the voxel (script SetNeighbor of Voxel.cs)
                    transform.gameObject.GetComponent<Voxel>().SetNeighbor(axisInd, neighColliders.gameObject);
                }
            }
        }
    }

    public static int SelectInd(Vector3 dPosition){
        // the goal is simply to select the right indice so we add the new neighbor to the correct coordinate of the current voxel neighborsList
        // TODO: this function could be added to the Voxel.cs script. But it's not mandatory.
        int i=-1;
        float max = Mathf.Max(Mathf.Abs(dPosition.x),Mathf.Abs(dPosition.y),Mathf.Abs(dPosition.z));
        if (max != 0){
            for (int k = 0; k<3; k++){
                // we select the direction 
                if (Mathf.Abs(dPosition[k]) == max){
                    // and then if it's + or - this direction.
                    if (dPosition[k] > 0 ){
                        return 2*k;
                    }
                    else {
                        return 2*k+1;
                    }
                } 
            }
        }
        return i;
    }
}
