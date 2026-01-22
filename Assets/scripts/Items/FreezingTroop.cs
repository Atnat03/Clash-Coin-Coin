using UnityEngine;

public class FreezingTroop : Troop
{
    private float freezingDuration;
    protected override void Attack()
    {
        print("Attack normal");
    
        if (target && target.TryGetComponent<ITargetable>(out var t) && t.CanBeAttacked)
        {
            if (t is Troop troop)
            {
                troop.GetFrozen(freezingDuration);
                troop.currentHP.color = Color.blue;
            }
        }
    
        StopAllCoroutines();
        StartCoroutine(Attacking());
    }
}
