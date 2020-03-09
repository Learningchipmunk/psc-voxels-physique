using UnityEditor;
using UnityEngine;


//// <summary>
//// Vizualisation helper for generator - classe pour générer
//// </summary>

[CustomEditor(typeof(generator))] // The CustomEditor attribute informs Unity which component it should act as an editor for
public class pipe_generator : Editor {


    // pour dire que ça dérive de la classe éditor
    // éditeur gère ce qui se passe dans le moteur mais pas dans le jeu
    // les outils de pipeline facilite la création

    // 1 ==== PRIMARY FUNCTIONS ===
    public override void OnInspectorGUI(){ 
        // executer quand on fait qqchose dans le moteur (à chaque frame)
        DrawDefaultInspector(); // on appelle la fonction mère et on lui dit fait comme d'hab
        generator my_target = (generator) target; // target= pté d'éditor

        if (GUILayout.Button("Add the body " + my_target.bodyName)){
           
            GameObject voxel_container = new GameObject(my_target.bodyName); // should contain all the voxels

            voxel_container.transform.parent = my_target.transform;
            voxel_container.transform.position = my_target.position;
            voxel_container.AddComponent(typeof(VoxelsJoint));
            voxel_container.GetComponent<VoxelsJoint>().neighbors = new GameObject[my_target.x,my_target.y,my_target.z];

            for (int i=0; i< my_target.x; i++){
                for (int j=0; j< my_target.y; j++){
                    for (int k=0; k< my_target.z; k++){

                        // instantiate the new object based on the prefab
                        voxel_container.GetComponent<VoxelsJoint>().neighbors[i,j,k]= (GameObject) PrefabUtility.InstantiatePrefab(my_target.modele); 

                        // place it, accordingly to those who come before.
                        voxel_container.GetComponent<VoxelsJoint>().Place(voxel_container.GetComponent<VoxelsJoint>().neighbors[i,j,k], voxel_container.GetComponent<VoxelsJoint>().NewCoordinates(i,j,k,voxel_container.GetComponent<VoxelsJoint>().neighbors), voxel_container);
                
                        voxel_container.GetComponent<VoxelsJoint>().neighbors[i,j,k].tag = "voxel";
                        voxel_container.GetComponent<VoxelsJoint>().neighbors[i,j,k].name =  i.ToString() + "." + j.ToString() + "." + k.ToString(); // x then y then z
                    }
                }
            }  
            //voxel_container.GetComponent<VoxelsJoint>().Init(my_target.x,my_target.y,my_target.z);
            //voxel_container.GetComponent<VoxelsJoint>().InitNeighbors();
        }
    }
}   
