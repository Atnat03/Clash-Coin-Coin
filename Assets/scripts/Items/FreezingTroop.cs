using UnityEngine;

public class FreezingTroop : Troop
{
    public override void Attack()
    {
        print("Attack normal");
    
        if (target && target.TryGetComponent<ITargetable>(out var t) && t.CanBeAttacked)
        {
            if (t is Troop troop)
            {
                troop.isFrozen = true;
                troop.currentHP.color = Color.blue;
            }
        }
    
        StopAllCoroutines();
        StartCoroutine(Attacking());
    }
}
