using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class QTEPlayerScript : MonoBehaviour
{
    public int score = 0;
    public Image imageNextInput;
    public Image jaugeScore;
    
    public Queue<QTEGameManager.ButtonDirection> inputList = new Queue<QTEGameManager.ButtonDirection>();
    
    void Start()
    {
        InitInpuList();
    }

    private int rnd = 0;
    private void InitInpuList()
    {
        for (int i = 0; i < 150; i++)
        {
            rnd = Random.Range(0, 4);
            inputList.Enqueue((QTEGameManager.ButtonDirection)rnd);
        }

        imageNextInput.sprite = QTEGameManager.instance.buttonSprites[inputList.Peek()];
    }

    public void RegisterInput(QTEGameManager.ButtonDirection input)
    {
        if (input == inputList.Peek())
        {
            inputList.Dequeue();
            score++;
            imageNextInput.sprite = QTEGameManager.instance.buttonSprites[inputList.Peek()];
        }
        else
        {
            score--;
        }
        
        jaugeScore.fillAmount = (float)score/QTEGameManager.instance.scoreMax;
    }

    public void NorthButtonPressed(InputAction.CallbackContext context)
    {
        if(QTEGameManager.instance.inGame && context.performed)RegisterInput(QTEGameManager.ButtonDirection.north);
    }
    
    public void SouthButtonPressed(InputAction.CallbackContext context)
    {
        if(QTEGameManager.instance.inGame && context.performed)RegisterInput(QTEGameManager.ButtonDirection.south);
    }
    public void EastButtonPressed(InputAction.CallbackContext context)
    {
        if(QTEGameManager.instance.inGame && context.performed)RegisterInput(QTEGameManager.ButtonDirection.east);
    }
    public void WestButtonPressed(InputAction.CallbackContext context)
    {
        if(QTEGameManager.instance.inGame && context.performed)RegisterInput(QTEGameManager.ButtonDirection.west);
    }
    
    
    
}
