using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class QTEPlayerScript : MonoBehaviour
{
    public int score = 0;
    public Image imageNextInput;
    public Image jaugeScore;
    public AudioClip clickSound;
    public float volume;
    public AudioSource SFXSource;
    
    public int playerID;
    
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
        
        if(playerID == 1)
            imageNextInput.sprite = QTEGameManager.instance.buttonSpritesP1[inputList.Peek()];
        else
            imageNextInput.sprite = QTEGameManager.instance.buttonSpritesP2[inputList.Peek()];

    }
    public void PlaySoundRandowPitch(AudioClip clip, float volume)
    {
        SFXSource.pitch = Random.Range(0.8f, 1.2f);
        SFXSource.PlayOneShot(clip, volume);
    }
    public void RegisterInput(QTEGameManager.ButtonDirection input)
    {
        if (input == inputList.Peek())
        {
            PlaySoundRandowPitch(clickSound,volume);
            inputList.Dequeue();
            score++;
            
            if(playerID == 1)
                imageNextInput.sprite = QTEGameManager.instance.buttonSpritesP1[inputList.Peek()];
            else
                imageNextInput.sprite = QTEGameManager.instance.buttonSpritesP2[inputList.Peek()];        
        }
        else
        {
            if(score > 0)score--;
        }
        
        jaugeScore.fillAmount = (float)score/QTEGameManager.instance.scoreMax;

        if(playerID == 1) QTEGameManager.instance.score1 = score;
        if(playerID == 2) QTEGameManager.instance.score2 = score;
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
