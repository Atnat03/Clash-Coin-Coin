using UnityEngine;
using UnityEngine.TestTools;

public class Bullet : MonoBehaviour
{
    public float speed = 25f;
    public Transform target;
    public Collider col;
    
    private Rigidbody rb;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true;
        rb.useGravity = false;
        
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
    }
    
    private void Update()
    {
        if (target != null)
        {
            Vector3 targetPos = target.position + Vector3.up;
            
            transform.position = Vector3.MoveTowards(transform.position,targetPos, speed * Time.deltaTime);
            
            transform.LookAt(target);

            transform.Rotate(Vector3.right * 10000f * Time.deltaTime, Space.Self);
            
            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
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
                troop.TakeDamage(40f);
            }
        }
        Destroy(gameObject);
    }
}
