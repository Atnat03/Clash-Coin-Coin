using System.Collections;
using UnityEngine;

public class PaveRoule : MonoBehaviour, IPave
{
    private float damage = 10;
    public float rollDistance = 5f; // Distance de roulement
    public float rollSpeed = 8f; // Vitesse de roulement
    public ParticleSystem impactParticles;
    public bool playerOneProperty;
    public float DamageDistance = 1f;
    
    public void Throw(Vector3 startPos, Vector3 targetPos, bool playerOneProperty, float damage)
    {
        this.damage = damage;
        this.playerOneProperty = playerOneProperty;
        
        transform.position = startPos;
        
        Vector3 rollDirection = (targetPos - startPos).normalized;
        rollDirection.y = 0;
        rollDirection.Normalize();
        
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
                if (hit.GetComponent<Item>())
                {
                    Item troop = hit.GetComponent<Item>();
                    if (troop != null && playerOneProperty != troop.playerOneProperty)
                    {
                        print("Pavé roulant a touché une troupe");
                        troop.TakeDamage(damage);
                        OnDestroy();
                        yield break;
                    }
                }
                else if (hit.GetComponent<Nexus>())
                {
                    Nexus nexus = hit.GetComponent<Nexus>();
                    if (nexus != null && playerOneProperty != nexus.playerOneProperty)
                    {
                        print("Pavé roulant a touché le Nexus");
                        nexus.TakeDamage(damage);
                        OnDestroy();
                        yield break;
                    }
                }
            }
            
            yield return null;
        }
        
        OnDestroy();
    }

    void OnDestroy()
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
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}