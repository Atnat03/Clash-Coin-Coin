using System;
using System.Collections;
using UnityEngine;

public class Trap : Build
{
    public float damage = 50;
    public float detectionZone = 2;
    public float explosionZone = 1.5f;
    bool alreadyTriggered = false;

    public Renderer modelRenderer;
    public Material matDefault;
    public Material matExplose;
    public GameObject explosionEffect;
    
    public new bool CanBeAttacked => false;
    
    public Trap(int id, string name, float maxPV) : base(id, name, maxPV)
    {
    }

    private void Start()
    {
        GetComponent<SphereCollider>().radius = detectionZone;
    }

    void Update()
    {
        if (alreadyTriggered) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionZone);

        foreach (Collider hit in hits)
        {
            if (hit.GetComponent<Troop>() != null)
            {
                Debug.Log("TriggerExplosion");
                alreadyTriggered = true;
                Explosed();
                break;
            }
        }
    }

    void Explosed()
    {
        print("Explosion");
        
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionZone);

        foreach (Collider hit in hits)
        {
            Troop troop = hit.GetComponent<Troop>();
            if (troop != null && playerOneProperty != troop.playerOneProperty)
            {
                print("touché une troupe");

                GameObject newPart = Instantiate(explosionEffect, transform.position + Vector3.up * 0.5f, Quaternion.identity);
                Destroy(newPart,2f);
                
                troop.TakeDamage(damage);
            }
        }
        
        Destroy(gameObject);
    }
    

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionZone);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, explosionZone);
    }
}
