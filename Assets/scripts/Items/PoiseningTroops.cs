using UnityEngine;
using UnityEngine.Serialization;

public class PoiseningTroops : Troop
{
    public float poisonDamage;
    public float poisonDuration;
    
    protected override void Attack()
    {
        print("Attack normal");
        AudioManager.instance.PlaySoundRandowPitch(AudioManager.instance.attackUnit,1.1f);

        if (target && target.TryGetComponent<ITargetable>(out var t) && t.CanBeAttacked)
        {
            if (t is Troop troop)
            {
                troop.GetPoisoned(poisonDuration, poisonDamage);
                troop.currentHP.color = Color.purple;
            }
        }
    
        StopAllCoroutines();
        StartCoroutine(Attacking());
    }
}
