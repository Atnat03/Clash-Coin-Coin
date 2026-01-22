using System.Collections;
using UnityEngine;

public class AnimFlip : MonoBehaviour
{

    public bool did = false;

    public void TriggerAnim()
    {
        StartCoroutine(FlipAnim());
    }

    IEnumerator FlipAnim()
    {
        Transform child = transform.GetChild(0);
        Vector3 originalPos = child.localPosition;
        Vector3 elevatedPos = originalPos + Vector3.up * 2f;

        float duration = 0.2f;
        float t = 0f;

        // Montée
        while (t < duration)
        {
            child.localPosition = Vector3.Lerp(originalPos, elevatedPos, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        child.localPosition = elevatedPos;

        // Rotation 360°
        t = 0f;
        while (t < duration)
        {
            child.Rotate(0, 360f * (Time.deltaTime / duration), 0);
            t += Time.deltaTime;
            yield return null;
        }

        // Descente
        t = 0f;
        while (t < duration)
        {
            child.localPosition = Vector3.Lerp(elevatedPos, originalPos, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        child.localPosition = originalPos;

        did = true;
    }
}
