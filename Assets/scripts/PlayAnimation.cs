using UnityEngine;

public class PlayAnimation : MonoBehaviour
{
    public void Attack()
    {
        transform.parent.GetComponent<Troop>().Attack();
    }
}
