using UnityEngine;

public class PlayAnimation : MonoBehaviour
{
    public void Attack()
    {
        print("Attack animation");
        transform.parent.GetComponent<Troop>().Attack();
    }
}
