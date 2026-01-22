using System.Collections;
using UnityEngine;

public class LanceTroop : Troop
{
    public GameObject throwBoulePrefab;
    public Transform throwPos;

    protected override void Attack()
    {
        print("Attack lancer");
        StopAllCoroutines();
        StartCoroutine(Attacking());
    }
    
    protected override IEnumerator Attacking()
    {
        if (throwBoulePrefab == null || throwPos == null || target == null)
        {
            Debug.LogWarning("Impossible d'attaquer, prefab, position ou target manquant");
            isAttacking = false;
            yield break;
        }

        IPave b = Instantiate(throwBoulePrefab, throwPos.position, Quaternion.identity).GetComponent<IPave>();
        if (b != null)
        {
            Vector3 targetPos = target.position;
            b.Throw(throwPos.position, targetPos, playerOneProperty, Damage, GetComponent<Collider>());
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
