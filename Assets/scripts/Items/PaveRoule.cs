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

    public void OnTriggerEnter(Collider collision)
    {
        print("collision");
        
        if (collision.GetComponent<ITargetable>() != null)
        {
            if(collision == mine)
                return;
            
            
            ITargetable troop = collision.GetComponent<ITargetable>();
            
            print(playerOneProperty + " / " + troop + " / " + troop.playerOneProperty);
            
            if (troop != null && playerOneProperty != troop.playerOneProperty)
            {
                print("touché une troupe");
                
                troop.TakeDamage(damage);
            }
        }
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
    
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, DamageDistance);

        Collider[] hits = Physics.OverlapSphere(transform.position, DamageDistance);

        foreach (Collider hit in hits)
        {
            Vector3 center = hit.bounds.center;

            if (mine != null && hit == mine)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(center, 0.1f);
                continue;
            }

            ITargetable target = hit.GetComponentInParent<ITargetable>();

            if (target == null)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawSphere(center, 0.08f);
                continue;
            }

            if (target.playerOneProperty == playerOneProperty)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(center, 0.1f);
                continue;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(center, 0.15f);
        }
    }
#endif

}