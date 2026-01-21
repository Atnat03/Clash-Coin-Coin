using System;
using System.Collections;
using UnityEngine;

public class TP_Troop : MonoBehaviour, ITargetable
{
    public Transform destination;

    private Troop currentTroop;
    public Vector3 offset;
    
    public bool isPlayer1 = false;

    public bool IsMovementTarget => false;
    
    private void Start()
    {
        playerOneProperty = isPlayer1;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Troop>() && other.GetComponent<Troop>().enabled && other.GetComponent<Troop>().playerOneProperty != playerOneProperty)
        {
            currentTroop = other.GetComponent<Troop>();
            StartCoroutine(TP());
        }
    }

    IEnumerator TP()
    {
        currentTroop.isFrozen = true;

        float t = 0f;
        float duration = 0.25f;

        // Disparaît
        while (t <= duration)
        {
            currentTroop.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        // Déplacement
        currentTroop.transform.position = destination.position + offset;

        t = 0f;

        // Réapparition
        while (t <= duration)
        {
            currentTroop.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        currentTroop.transform.localScale = Vector3.one;
        
        print(currentTroop.target);
    
        currentTroop.isFrozen = false;
        currentTroop.alreadyTakeTP = true;

        currentTroop.ForceRecalculatePath(ignoreLastTarget: true);

    }

    public bool playerOneProperty { get; set; }
    public void TakeDamage(float damage)
    { }

    public void GetPoisoned(float duration, float damage)
    { }
    
    public bool CanBeAttacked => false;
}
