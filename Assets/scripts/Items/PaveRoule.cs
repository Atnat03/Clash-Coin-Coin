using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaveRoule : MonoBehaviour, IPave
{
    private float damage = 10;
    public float rollDistance = 5f;
    public float rollSpeed = 8f;
    public ParticleSystem impactParticles;
    public bool playerOneProperty;
    public float DamageDistance = 1f;

    private Collider mine;
    
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();
    
    public void Throw(Vector3 startPos, Vector3 targetPos, bool playerOneProperty, float damage, Collider mine)
    {
        this.damage = damage;
        this.playerOneProperty = playerOneProperty;
        
        transform.position = startPos;
        
        Vector3 rollDirection = (targetPos - startPos).normalized;
        rollDirection.y = 0;
        rollDirection.Normalize();
        
        this.mine = mine;
        
        StartCoroutine(RollCoroutine(rollDirection));
    }
    
    IEnumerator RollCoroutine(Vector3 direction)
    {
        float distanceTraveled = 0f;
        
        while (distanceTraveled < rollDistance)
        {
            float moveDistance = rollSpeed * Time.deltaTime;
            transform.position += direction * moveDistance;
            distanceTraveled += moveDistance;
            
            transform.Rotate(Vector3.right, rollSpeed * 100f * Time.deltaTime);
            
            Collider[] hits = Physics.OverlapSphere(transform.position, DamageDistance);
            
            foreach (Collider hit in hits)
            {
                if (hitTargets.Contains(hit.gameObject))
                    continue;
                
                if(mine == hit)
                    continue;
                
                if (hit.GetComponent<Item>())
                {
                    Item troop = hit.GetComponent<Item>();
                    if (troop != null && playerOneProperty != troop.playerOneProperty)
                    {
                        print("Pavé roulant a touché une troupe : " + troop.name);
                        troop.TakeDamage(damage);
                        hitTargets.Add(hit.gameObject);
                    }
                }
                else if (hit.GetComponent<Nexus>())
                {
                    Nexus nexus = hit.GetComponent<Nexus>();
                    if (nexus != null && playerOneProperty != nexus.playerOneProperty)
                    {
                        print("Pavé roulant a touché le Nexus");
                        nexus.TakeDamage(damage);
                        hitTargets.Add(hit.gameObject);
                    }
                }
            }
            
            yield return null;
        }
        
        DestroyPave();
    }

    void DestroyPave()
    {
        if (impactParticles != null)
        {
            Instantiate(impactParticles, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, DamageDistance);
    }
}