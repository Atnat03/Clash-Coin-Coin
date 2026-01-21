using UnityEngine;

public class DestoyOnLoad : MonoBehaviour
{
    private float timer = 1.5f;
    void Start()
    {
        Destroy(gameObject, timer);
    }
}
