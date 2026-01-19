using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerTest : MonoBehaviour
{
    public float speed = 3;
    private Vector2 m_moveAmt;
    
    public void OnMove(InputAction.CallbackContext context)
    {
        m_moveAmt = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        transform.Translate(new Vector3(m_moveAmt.x, 0, m_moveAmt.y) * speed * Time.deltaTime, Space.World);
    }
}
