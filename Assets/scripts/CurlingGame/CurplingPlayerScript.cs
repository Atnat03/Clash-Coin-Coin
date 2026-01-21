using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CurplingPlayerScript : MonoBehaviour
{
    public GameObject palet;
    public Transform target;
    public Image jauge;

    public float powerAmount;
    public float score;

    public bool played = false;

    private bool buttonHeld = false;
    private bool isMoving = false;

    private Vector3 desiredPos;

    public float powerMultiplyFactor = 2;

    void Start()
    {
        jauge.fillAmount = 0f;
        powerAmount = 0f;
    }
    

    public void ButtonPress(InputAction.CallbackContext context)
    {
        if(played)return;
        
        if (!context.performed || !CurlingGameManager.instance.inGame)
            return;
        
        buttonHeld = true;
        
        StartCoroutine(ButtonPressedCoroutine());

        powerAmount = 0f;
        jauge.fillAmount = 0f;
    }

    public void ButtonRelease(InputAction.CallbackContext context)
    {
        if (!context.canceled || !buttonHeld)
            return;
        
        buttonHeld = false;

        desiredPos = palet.transform.position +
                     (palet.transform.position - target.position).normalized * powerAmount;

        isMoving = true;
    }
    
    IEnumerator ButtonPressedCoroutine()
    {
        float elapsedTime = 0f;
        while (buttonHeld)
        {
            elapsedTime += Time.fixedDeltaTime;
            powerAmount = Mathf.Clamp(powerAmount + CurlingGameManager.instance.jaugeFillingSpeed, 0f, 1f);
            jauge.fillAmount = powerAmount;
            yield return new WaitForFixedUpdate();
        }

        Vector3 direction = (target.position - palet.transform.position).normalized;
        Vector3 desiredPos = palet.transform.position + direction * powerAmount * powerMultiplyFactor;

        while (Vector3.Distance(palet.transform.position, desiredPos) > 0.1f)
        {
            palet.transform.position = Vector3.Lerp(palet.transform.position, desiredPos, Time.fixedDeltaTime*1.5f);
            yield return new WaitForFixedUpdate();
        }
        palet.transform.position = desiredPos;

        score = Vector3.Distance(palet.transform.position, target.position);
        played = true;
    }
}
