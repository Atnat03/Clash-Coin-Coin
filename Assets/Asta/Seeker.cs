using System;
using UnityEngine;

public class Seeker : MonoBehaviour
{
    [SerializeField] GridManager gridManager;
    [SerializeField] float speed = 1;

    void Update()
    {
        if (gridManager.path != null)
        {
            if (gridManager.path.Count > 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, gridManager.path[0].worldPosition, speed * Time.deltaTime);
            }
        }
    }

}