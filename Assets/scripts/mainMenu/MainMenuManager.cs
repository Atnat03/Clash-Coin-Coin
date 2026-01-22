using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public Animation animAndSFXButton;
    public IEnumerator AnimAndSFXStartbutton()
    {
        animAndSFXButton.Play();
        AudioManager.instance.PlaySound(AudioManager.instance.buttonPressed);
        AudioManager.instance.PlaySound(AudioManager.instance.gongStart);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("MainScene");
    }
    public void PlayGame()
    {
        StartCoroutine(AnimAndSFXStartbutton());
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
}
