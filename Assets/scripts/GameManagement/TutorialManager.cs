using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutoPanel;
    public EventSystem eventSystem;
    public GameObject buttonRight;
    
    public List<GameObject> tutorialObjects = new List<GameObject>();
    int index = 0;

    void Start()
    {
        foreach (GameObject panel in tutorialObjects)
        {
            panel.SetActive(false);
        }
        tutorialObjects[index].SetActive(true);
        tutoPanel.SetActive(false);
    }

    public void OpenCloseTutorial()
    {
        Debug.Log("OpenCloseTutorial");
        tutoPanel.SetActive(!tutoPanel.activeSelf);
        if (tutoPanel.activeSelf)
        {
            eventSystem.SetSelectedGameObject(buttonRight);
        }
    }

    public void NavigateRight()
    {
        tutorialObjects[index].SetActive(false);
        index++;
        index = Mathf.Clamp(index, 0, tutorialObjects.Count - 1);
        tutorialObjects[index].SetActive(true);
    }
    
    public void NavigateLeft()
    {
        tutorialObjects[index].SetActive(false);
        index--;
        index = Mathf.Clamp(index, 0, tutorialObjects.Count - 1);
        tutorialObjects[index].SetActive(true);
    }
}
