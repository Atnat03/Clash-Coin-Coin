using System;
using System.Collections;
using UnityEngine;

public class PaveExplosif : MonoBehaviour, IPave
{
    private float damage = 10;
    public float throwDuration = 0.6f;
    public float arcHeight = 2f;
    public ParticleSystem throwParticles;
    public float explosionZone = 1.5f;
    public bool playerOneProperty;
    private Collider mine;
    
    public void Throw(Vector3 startPos, Vector3 targetPos, bool playerOneProperty, float damage, Collider mine)
    {
        if (throwDuration <= 0f)
        {
            transform.position = targetPos;
            OnImpact();
            return;
        }
        
        this.damage = damage;
        this.mine = mine;

        this.playerOneProperty = playerOneProperty;
        StartCoroutine(ThrowCoroutine(startPos, targetPos));
    }

    
    IEnumerator ThrowCoroutine(Vector3 startPos, Vector3 targetPos)
    {
        float elapsed = 0f;
        
        while (elapsed < throwDuration)
        {
            float t = elapsed / throwDuration;

            Vector3 pos = Vector3.Lerp(startPos, targetPos, t);

            float height = 4f * arcHeight * t * (1f - t);
            pos.y += height;

            transform.position = pos;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        OnImpact();
    }

    void OnImpact()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionZone);

        foreach (Collider hit in hits)
        {
            if (hit.GetComponent<Item>())
            {
                Item troop = hit.GetComponent<Item>();
                if (troop != null && playerOneProperty != troop.playerOneProperty)
                {
                    print("touché une troupe");
                
                    troop.TakeDamage(damage);
                
                    break;
                }
            }
            else if (hit.GetComponent<Nexus>())
            {
                Nexus troop = hit.GetComponent<Nexus>();
                if (troop != null && playerOneProperty != troop.playerOneProperty)
                {
                    print("touché une troupe");
                
                    troop.TakeDamage(damage);
                
                    break;
                }
            }

        }
        
        Instantiate(throwParticles, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionZone);
    }
}
