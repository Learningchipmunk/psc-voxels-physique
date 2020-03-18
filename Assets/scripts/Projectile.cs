using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody projectile1;
    public Rigidbody projectile2;
    public Rigidbody ice;
    public Transform spawnPoint;
    public float bulletForce1;
    public float bulletForce2;
    public float bulletForceIce;
    public Camera fpsCam;
    public RectTransform target;
    private float rayonCone = 0.2f;
    private bool allowfire = true;
    public float invertRate1 = 0f;
    public float invertRate2 = 0f;

    public float invertRateIce = 0f;

    private int projectilechose = 1;

    void Update()
    {
        if (Input.GetMouseButton(0)&&(allowfire))
        {
            StartCoroutine(Fire1());            
        }


        if (Input.GetKeyDown("a"))
        {
            if(projectilechose==1) projectilechose = 2;
            else if(projectilechose==2) projectilechose = 3;
            else projectilechose = 1;
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
            if(projectilechose == 1){
            clone = Instantiate(projectile1, spawnVector, spawnPoint.rotation);
            clone.AddForce(bulletForce1 * (dirProj + compCone));}
            else if(projectilechose == 2){
            clone = Instantiate(projectile2, spawnVector, spawnPoint.rotation);
            clone.AddForce(bulletForce2 * (dirProj + compCone));}
            else{
            clone = Instantiate(ice, spawnVector, spawnPoint.rotation);
            clone.AddForce(bulletForceIce * (dirProj + compCone));}
            
        }


        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            target.sizeDelta += new Vector2(10, 10);
            rayonCone += 0.02f;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            target.sizeDelta -= new Vector2(10, 10);
            rayonCone -= 0.02f;
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
        if(projectilechose == 1){
        clone = Instantiate(projectile1, spawnVector, spawnPoint.rotation);
        clone.AddForce(bulletForce1 * (dirProj + compCone));
        yield return new WaitForSeconds(invertRate1);}

        else if(projectilechose == 2){
        clone = Instantiate(projectile2, spawnVector, spawnPoint.rotation);
        clone.AddForce(bulletForce2 * (dirProj + compCone));
        yield return new WaitForSeconds(invertRate2); }

        else{
        clone = Instantiate(ice, spawnVector, spawnPoint.rotation);
        clone.AddForce(bulletForceIce * (dirProj + compCone));
        yield return new WaitForSeconds(invertRateIce);
        }
        allowfire = true;
    }

}

