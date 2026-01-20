using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CurplingPlayerScript : MonoBehaviour
{
    public GameObject palet;
    public Transform target;
    public Image jauge;

    public float powerAmount;
    bool ButtonPressed = false;
    public float score;
    
    public bool played = false;

    void Start()
    {
        jauge.fillAmount = 0;
    }
    
    public void ButtonPress(InputAction.CallbackContext context)
    {
        if(CurlingGameManager.instance.inGame)
        {
            ButtonPressed = true;
            StartCoroutine(ButtonPressedCoroutine());
        }
    }

    public void ButtonRelease(InputAction.CallbackContext context)
    {
        ButtonPressed = false;
    }

    IEnumerator ButtonPressedCoroutine()
    {
        float elapsedTime = 0f;
        while (ButtonPressed)
        {
            elapsedTime += Time.fixedDeltaTime;
            powerAmount = Mathf.Clamp(powerAmount + CurlingGameManager.instance.jaugeFillingSpeed, 0f, 1f);
            jauge.fillAmount = powerAmount;
            yield return new WaitForFixedUpdate();
        }

        Vector3 desiredPos = (palet.transform.position - target.position).normalized * powerAmount;
        while (Vector3.Distance(palet.transform.position, desiredPos) > 0.1f)
        {
            palet.transform.position = Vector3.Lerp(palet.transform.position, desiredPos, Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
        palet.transform.position = desiredPos;
        
        score = Vector3.Distance(palet.transform.position, target.position);
        played = true;
    }
    
    

}
