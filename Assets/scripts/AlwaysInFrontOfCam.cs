using UnityEngine;

public class AlwaysInFrontOfCam : MonoBehaviour
{
    public Transform target;
    
    private void LateUpdate()
    {
        Vector3 direction = target.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(-direction);
    }
}
