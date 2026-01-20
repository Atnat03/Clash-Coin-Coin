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
        if (other.GetComponent<Troop>() && other.GetComponent<Troop>().enabled)
        {
            currentTroop = other.GetComponent<Troop>();
            StartCoroutine(TP());
        }
    }

    IEnumerator TP()
    {
        currentTroop.isFrozen = true;

        float t = 0;
        float duration = 0.25f;

        while (t <= duration)
        {
            currentTroop.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t/duration);
            t += Time.deltaTime;
            yield return null;
        }

        currentTroop.transform.position = destination.position + offset;
        
        t = 0;
        
        while (t <= duration)
        {
            currentTroop.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t/duration);
            t += Time.deltaTime;
            yield return null;
        }

        currentTroop.transform.localScale = Vector3.one;
        
        currentTroop.isFrozen = false;
        currentTroop.ForceRecalculatePath();
    }

    public bool playerOneProperty { get; set; }
    public void TakeDamage(float damage)
    {
        throw new NotImplementedException();
    }

    public void GetPoisoned(float duration, float damage)
    {
        throw new NotImplementedException();
    }
    
    public bool CanBeAttacked => false;
}
