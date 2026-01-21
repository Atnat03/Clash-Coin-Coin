using System.Collections;
using UnityEngine;

public class LanceTroop : Troop
{
    public GameObject throwBoulePrefab;
    public Transform throwPos;

    public override void Attack()
    {
        StartCoroutine(Attacking());
    }
    
    protected override IEnumerator Attacking()
    {
        isAttacking = true;

        yield return new WaitForSeconds(attackCooldown);

        if (target &&
            target.TryGetComponent<ITargetable>(out var t) &&
            t.CanBeAttacked)
        {
            IPave b = Instantiate(throwBoulePrefab,  throwPos.position, Quaternion.identity).GetComponent<IPave>();
            b.Throw(throwPos.position, target.position, target.GetComponent<Item>().playerOneProperty);
        }

        isAttacking = false;
    }
}
