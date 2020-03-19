using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Camera fpsCam;
    public RectTransform target;
    public Transform spawnPoint;
    public Text ProjectileType;

    public Slider FiringRate;
    public Text ProjectileChangeIndication;
    private int projectilechose;


    private Rigidbody projectile;
    public Rigidbody acid;
    public Rigidbody stone;
    public Rigidbody ice;
    public Rigidbody fire;
    

    private float bulletForce;
    public float bulletForceAcid;
    public float bulletForceStone;
    public float bulletForceIce;
    public float bulletForceFire;


    
    private float rayonCone = 0.16f;
    private bool allowfire = true;

    private float invertRate = 0.5f;


    void Awake(){
        projectile = acid;
        bulletForce = bulletForceAcid;
        ProjectileType.text = "Projectile : Acid";
        ProjectileChangeIndication.text = "'E' to switch";
        FiringRate.value = 1-invertRate;
        projectilechose = 1;

    }
    void FixedUpdate()
    {
        if (Input.GetMouseButton(0)&&(allowfire))
        {
            StartCoroutine(Fire1());            
        }

        if (Input.GetKeyDown("e"))
        {
            ChangeProjectile();
        }

        if (Input.GetKey("r")){
            if(invertRate<0.999){
            invertRate += 0.015f;
            FiringRate.value = 1-invertRate;}
        }

        if (Input.GetKey("t")){
            if(invertRate>0.001){
            invertRate -= 0.015f;
            FiringRate.value = 1-invertRate;}
        }

        if (Input.GetButtonDown("Fire2"))
        {
            Vector3 spawnVector = fpsCam.transform.position + 2*fpsCam.transform.forward;
            Vector3 dirProj = 2*fpsCam.transform.forward;
            Rigidbody clone;
            Vector3 u = Vector3.Cross(dirProj, Vector3.up).normalized;
            Vector3 v = Vector3.Cross(dirProj, u).normalized;
            float angle = Mathf.PI * 2 * Random.value;
            float rayon = rayonCone * Random.value;
            Vector3 compCone = rayon * (Mathf.Cos(angle) * u + Mathf.Sin(angle) * v);
            clone = Instantiate(projectile, spawnVector, new Quaternion(0,0,0,0));
            clone.AddForce(bulletForce * (dirProj + compCone));
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            target.sizeDelta += new Vector2(10, 10);
            rayonCone += 0.02f;
        }
        if ((Input.GetAxis("Mouse ScrollWheel") > 0)&&(rayonCone > 0.001))
        {
            target.sizeDelta -= new Vector2(10, 10);
            rayonCone -= 0.02f;
            Debug.Log(rayonCone);
        }

    }

    IEnumerator Fire1()
    {
        allowfire = false;
        Vector3 spawnVector = fpsCam.transform.position + 2*fpsCam.transform.forward;
        Vector3 dirProj = 2*fpsCam.transform.forward;
        Rigidbody clone;
        Vector3 u = Vector3.Cross(dirProj, Vector3.up).normalized;
        Vector3 v = Vector3.Cross(dirProj, u).normalized;
        float angle = Mathf.PI * 2 * Random.value;
        float rayon = rayonCone * Random.value;
        Vector3 compCone = rayon * (Mathf.Cos(angle) * u + Mathf.Sin(angle) * v);
        clone = Instantiate(projectile, spawnVector, spawnPoint.rotation);
        clone.AddForce(bulletForce * (dirProj + compCone));
        yield return new WaitForSeconds(invertRate);
        allowfire = true;
    }

    void ChangeProjectile(){
        if(projectilechose==1){
            projectile = stone;
            bulletForce = bulletForceStone;
            ProjectileType.text = "Projectile : Stone";
            projectilechose = 2;}
        else if(projectilechose==2){
            projectile = fire;
            bulletForce = bulletForceFire;
            ProjectileType.text = "Projectile : Fire";
            projectilechose = 3;}
        else if(projectilechose==3){
            projectile = ice;
            bulletForce = bulletForceIce;
            ProjectileType.text = "Projectile : Ice";
            projectilechose = 1;}
        else{
            projectile = acid;
            bulletForce = bulletForceAcid;
            ProjectileType.text = "Projectile : Acid";
            projectilechose = 1;}

    }

}

