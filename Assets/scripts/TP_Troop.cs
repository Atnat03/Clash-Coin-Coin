using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TP_Troop : MonoBehaviour, ITargetable
{
    public Transform destination;
    public Vector3 offset;

    public bool isPlayer1 = false;
    public bool IsMovementTarget => false;

    private Queue<Troop> waitingTroops = new Queue<Troop>();
    private bool isTeleporting = false;

    public ParticleSystem teleportEffect;

    private void Start()
    {
        playerOneProperty = isPlayer1;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Troop>(out Troop troop))
            return;

        if (!troop.enabled)
            return;

        if (troop.playerOneProperty == playerOneProperty)
            return;

        if (troop.alreadyTakeTP)
            return;

        if (waitingTroops.Contains(troop))
            return;

        waitingTroops.Enqueue(troop);

        if (!isTeleporting)
            StartCoroutine(ProcessQueue());
    }

    IEnumerator ProcessQueue()
    {
        isTeleporting = true;

        while (waitingTroops.Count > 0)
        {
            Troop currentTroop = waitingTroops.Dequeue();

            if (currentTroop == null || !currentTroop.enabled)
                continue;

            yield return StartCoroutine(TeleportTroop(currentTroop));
        }

        isTeleporting = false;
    }

    IEnumerator TeleportTroop(Troop troop)
    {
        troop.isFrozen = true;

        float t = 0f;
        float duration = 0.25f;

        Vector3 startScale = troop.transform.localScale;

        while (t < duration)
        {
            if(troop == null)
                yield break;
            
            troop.transform.localScale =
                Vector3.Lerp(startScale, Vector3.zero, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        teleportEffect.Play();

        troop.transform.localScale = Vector3.zero;

        troop.transform.position = destination.position + offset;

        t = 0f;

        while (t < duration)
        {
            if(troop == null)
                yield break;
            
            troop.transform.localScale =
                Vector3.Lerp(Vector3.zero, startScale, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        troop.transform.localScale = startScale;

        troop.isFrozen = false;
        troop.alreadyTakeTP = true;
        troop.ForceRecalculatePath(ignoreLastTarget: true);
    }

    public bool playerOneProperty { get; set; }
    public bool CanBeAttacked => false;
    public void TakeDamage(float damage) { }
    public void GetPoisoned(float duration, float damage) { }
}
