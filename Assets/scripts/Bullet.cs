using UnityEngine;
using UnityEngine.TestTools;

public class Bullet : MonoBehaviour
{
    public float speed = 25f;
    public Transform target;
    public float damage = 10f;
    public Collider col;
    
    private Rigidbody rb;
    
    private void Awake()
    {
        // S'assurer qu'il y a un Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true; // Kinematic car on contrôle le mouvement manuellement
        rb.useGravity = false;
        
        // S'assurer qu'il y a un Collider trigger
        Collider bulletCollider = GetComponent<Collider>();
        if (bulletCollider != null)
        {
            bulletCollider.isTrigger = true;
        }
    }
    
    public void SetUp(Transform target, Collider col, float damage)
    {
        this.col = col;
        this.target = target;
        this.damage = damage;
        print(damage);
    }
    
    private void Update()
    {
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            
            print(Vector3.Distance(transform.position, target.position));

            if (Vector3.Distance(transform.position, target.position) < 0.1f)
            {
                OnReachTarget();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void OnReachTarget()
    {
        if (target != null)
        {
            Troop troop = target.GetComponent<Troop>();
            if (troop != null)
            {
                print(damage + " : rerggedsddfgdse");
                troop.TakeDamage(40f);
            }
        }
        Destroy(gameObject);
    }
}
