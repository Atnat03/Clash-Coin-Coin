using UnityEngine;
using UnityEngine.TestTools;

public class Bullet : MonoBehaviour
{
    public float speed = 25f;
    public Transform target;
    public float damage = 10f;
    public Collider col;
    
    public void SetUp(Transform target, Collider col)
    {
        this.col = col;
        this.target = target;
    }
    
    public void Update()
    {
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ITargetable>() != null && other != col)
        {
            other.GetComponent<ITargetable>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
