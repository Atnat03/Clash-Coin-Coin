using System;
using UnityEngine;

public class Turret : Build, ITargetable
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
        if (t < shootRate)
        {
            t += Time.deltaTime;
        }
        else
        {
            print("Shoot");
            
            Collider[] hits = Physics.OverlapSphere(transform.position, shootRange);

            foreach (Collider hit in hits)
            {
                Troop troop = hit.GetComponent<Troop>();
                if (troop != null && playerOneProperty != troop.playerOneProperty)
                {
                    print("touché une troupe");

                    Vector3 dir = troop.transform.position - headTurret.position;
                    dir.y = 0;
                    headTurret.localRotation = Quaternion.LookRotation(dir);



                    Bullet b = Instantiate(bulletPrefab, shootPos.position, Quaternion.identity);
                    b.SetUp(troop.transform, GetComponent<Collider>());
                    Destroy(b.gameObject,3f);
                    break;
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
