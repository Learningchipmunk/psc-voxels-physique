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
    public Text ProjectileChangeIndication;
    private int projectilechose;


    private Rigidbody projectile;
    public Rigidbody acid;
    public Rigidbody stone;
    public Rigidbody ice;
    

    private float bulletForce;
    public float bulletForceAcid;
    public float bulletForceStone;
    public float bulletForceIce;


    
    private float rayonCone = 0.16f;
    private bool allowfire = true;

    private float invertRate;
    public float invertRateAcid = 0f;
    public float invertRateStone = 1f;
    public float invertRateIce = 0f;

    

    void Awake(){
        projectile = acid;
        bulletForce = bulletForceAcid;
        invertRate = invertRateAcid;
        ProjectileType.text = "Projectile : Acide";
        ProjectileChangeIndication.text = "Press 'a' to switch";
        projectilechose = 1;

    }
    void Update()
    {
        if (Input.GetMouseButton(0)&&(allowfire))
        {
            StartCoroutine(Fire1());            
        }


        if (Input.GetKeyDown("a"))
        {
            ChangeProjectile();
        }
        if (Input.GetButtonDown("Fire2"))
        {
            Vector3 spawnVector = spawnPoint.position;
            Vector3 dirProj = spawnPoint.transform.position - fpsCam.transform.position;
            Rigidbody clone;
            Vector3 u = Vector3.Cross(dirProj, Vector3.up).normalized;
            Vector3 v = Vector3.Cross(dirProj, u).normalized;
            float angle = Mathf.PI * 2 * Random.value;
            float rayon = rayonCone * Random.value;
            Vector3 compCone = rayon * (Mathf.Cos(angle) * u + Mathf.Sin(angle) * v);
            clone = Instantiate(projectile, spawnVector, spawnPoint.rotation);
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
        Vector3 spawnVector = spawnPoint.position;
        Vector3 dirProj = spawnPoint.transform.position - fpsCam.transform.position;
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
            invertRate = invertRateStone;
            ProjectileType.text = "Projectile : Stone";
            projectilechose = 2;}
        else if(projectilechose==2){
            projectile = ice;
            bulletForce = bulletForceIce;
            invertRate = invertRateIce;
            ProjectileType.text = "Projectile : Ice";
            projectilechose = 3;}
        else{
            projectile = acid;
            bulletForce = bulletForceAcid;
            invertRate = invertRateAcid;
            ProjectileType.text = "Projectile : Acid";
            projectilechose = 1;}
    }

}

