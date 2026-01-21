using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MetronomePlayerScript : MonoBehaviour
{
    public Slider slider;

    public float cursorSpeed;
    public int points;
    
    public bool ingame;
    public int playerID;
    bool alreadyClicked;

    public Animator animClic;
    public AudioClip clickSound;
    public float volume;
    public AudioSource SFXSource;
    public void Start()
    {
        ingame = false;
        cursorSpeed = MetronomeGameManager.instance.cursorDefaultSpeed;
        MetronomeGameManager.instance.BeginGame += BeginGame;
    }

    public void BeginGame()
    {
        ingame = true;
        StartCoroutine(PlayGame());
    }

    private float elapsedTime = 0f;
    
    public IEnumerator PlayGame()
    {
        elapsedTime = 0f;
        while (elapsedTime < MetronomeGameManager.instance.gameLength)
        {
            elapsedTime += Time.deltaTime;
            slider.value = Mathf.Sin(elapsedTime * cursorSpeed);
            
            if (slider.value > 0.99f)
            {
                float phase = elapsedTime * cursorSpeed;
        
                cursorSpeed *= MetronomeGameManager.instance.cursorAccelerationFactor;
        
                elapsedTime = phase / cursorSpeed;
            }

            if (Mathf.Abs(slider.value) > 0.99f)
            {
                alreadyClicked = false;
            }
            
            yield return null;
        }
    }
    public void PlaySoundRandowPitch(AudioClip clip, float volume)
    {
        SFXSource.pitch = Random.Range(0.8f, 1.2f);
        SFXSource.PlayOneShot(clip, volume);

    }

    public Image[] feedBackSuccess;
    public void PlayerPressedA(InputAction.CallbackContext context)
    {
        if (!context.performed || alreadyClicked)
            return;
        
        animClic.SetTrigger("Clic");
        
        if (!(Mathf.Abs(slider.value) <= MetronomeGameManager.instance.SliderTolerence && points < MetronomeGameManager.instance.pointsToScore))
        {
            foreach (Image img in feedBackSuccess)
            {
                img.color = Color.red;
            }
        }
        else
        {
            PlaySoundRandowPitch(clickSound,volume);
            foreach (Image img in feedBackSuccess)
            {
                img.color = Color.green;
            }
        }
        
        
        if (ingame)
        {
            alreadyClicked = true;
            if (Mathf.Abs(slider.value) <= MetronomeGameManager.instance.SliderTolerence && points < MetronomeGameManager.instance.pointsToScore)
            {
                ScorePoint();
            }
        }

        StartCoroutine(ReturnToWhite());
    }

    IEnumerator ReturnToWhite()
    {
        yield return new WaitForSeconds(0.3f);
            foreach (Image img in feedBackSuccess)
            {
                img.color = Color.white;
            }
    }

    public void ScorePoint()
    {
        if (playerID == 1) MetronomeGameManager.instance.cursorPosition--;
        if (playerID == 2) MetronomeGameManager.instance.cursorPosition++;
    }

}
