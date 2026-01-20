using System;
using UnityEngine;

public class Turret : Build
{
    public Bullet bulletPrefab;
    public float shootRange;
    public Transform shootPos;
    public Transform headTurret;
    public float shootRate = 1f;
    private float t = 0;
    
    public Turret(int id, string name, float maxPV) : base(id, name, maxPV)
    {
        
    }

    private void Update()
    {
        if (t < shootRange)
        {
            t += Time.deltaTime;
        }
        else
        {
            RaycastHit hit;

            if (Physics.SphereCast(shootPos.position, shootRange, transform.forward, out hit, 10))
            {
                if (hit.transform.GetComponent<Troop>() != null)
                {
                    headTurret.transform.LookAt(hit.transform);
                
                    Bullet b = Instantiate(bulletPrefab, shootPos.transform.position, Quaternion.identity);
                    b.SetUp(hit.transform);
                }
            }

            t = 0;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }
}
