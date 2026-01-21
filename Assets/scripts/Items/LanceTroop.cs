using System.Collections;
using UnityEngine;

public class LanceTroop : Troop
{
    public GameObject throwBoulePrefab;
    public Transform throwPos;

    public override void Attack()
    {
        if (isAttacking) return;
        
        print("Attack lancer");
        StopAllCoroutines();
        StartCoroutine(Attacking());
    }
    
    protected override IEnumerator Attacking()
    {
        isAttacking = true;

        if (throwBoulePrefab == null || throwPos == null || target == null)
        {
            Debug.LogWarning("Impossible d'attaquer, prefab, position ou target manquant");
            isAttacking = false;
            yield break;
        }

        IPave b = Instantiate(throwBoulePrefab, throwPos.position, Quaternion.identity).GetComponent<IPave>();
        if (b != null)
        {
            // Décalage devant la cible
            Vector3 forwardOffset = target.forward * 1f; // 1 unité devant la target
            Vector3 targetPos = target.position + forwardOffset;

            ITargetable targetItem = target.GetComponent<ITargetable>();
            if (targetItem != null)
            {
                b.Throw(throwPos.position, targetPos, targetItem.playerOneProperty);
            }
            else
            {
                Debug.LogWarning("La target n'a pas de composant Item");
            }
        }
        else
        {
            Debug.LogWarning("Le prefab instancié n'a pas de composant IPave !");
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
        print("End attack");
    }
}
