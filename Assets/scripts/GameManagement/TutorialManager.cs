using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject tutoPanel;
    public EventSystem eventSystem;
    public GameObject buttonRight;

    [Header("Tutorial Pages")]
    public List<GameObject> tutorialObjects = new List<GameObject>();
    int index = 0;

    [Header("Input")]
    public InputActionReference openCloseTutorial;

    [Header("Stick Navigation")]
    public float stickThreshold = 0.6f;
    public float firstRepeatDelay = 0.4f;
    public float repeatDelay = 0.15f;

    float nextMoveTime = 0f;
    bool stickInUse = false;

    void Start()
    {
        foreach (GameObject panel in tutorialObjects)
        {
            panel.SetActive(false);
        }

        tutorialObjects[index].SetActive(true);
        tutoPanel.SetActive(false);
    }

    void OnEnable()
    {
        openCloseTutorial.action.performed += OnOpenClose;
        openCloseTutorial.action.Enable();
    }

    void OnDisable()
    {
        openCloseTutorial.action.performed -= OnOpenClose;
    }

    void Update()
    {
        if (!tutoPanel.activeSelf) return;

        float x = VariablesManager.instance.players[1].aimInput.x;

        // Zone morte du stick
        if (Mathf.Abs(x) < stickThreshold)
        {
            stickInUse = false;
            return;
        }

        // Navigation avec gestion du délai
        if (Time.time >= nextMoveTime)
        {
            if (x > stickThreshold)
            {
                NavigateRight();
            }
            else if (x < -stickThreshold)
            {
                NavigateLeft();
            }

            nextMoveTime = Time.time + (stickInUse ? repeatDelay : firstRepeatDelay);
            stickInUse = true;
        }
    }

    void OnOpenClose(InputAction.CallbackContext ctx)
    {
        OpenCloseTutorial();
    }

    public void OpenCloseTutorial()
    {
        AudioManager.instance.PlaySound(AudioManager.instance.buttonPressed);
        Debug.Log("OpenCloseTutorial");
        tutoPanel.SetActive(!tutoPanel.activeSelf);

        if (tutoPanel.activeSelf)
        {
            eventSystem.SetSelectedGameObject(buttonRight);
        }
    }

    public void NavigateRight()
    {
        AudioManager.instance.PlaySound(AudioManager.instance.tutoCard);
        tutorialObjects[index].SetActive(false);
        index++;
        index = Mathf.Clamp(index, 0, tutorialObjects.Count - 1);
        tutorialObjects[index].SetActive(true);
    }

    public void NavigateLeft()
    {
        AudioManager.instance.PlaySound(AudioManager.instance.tutoCard);
        tutorialObjects[index].SetActive(false);
        index--;
        index = Mathf.Clamp(index, 0, tutorialObjects.Count - 1);
        tutorialObjects[index].SetActive(true);
    }
}
