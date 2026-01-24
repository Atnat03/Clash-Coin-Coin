using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UiEnd : MonoBehaviour
{
    public Image image;
    public Sprite[] sprites;

    public GameObject replayButton;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Reset()
    {
        gameObject.SetActive(false);
        replayButton.SetActive(false);
    }

    public void SetUp(int spriteID)
    {
        image.sprite = sprites[spriteID];
        StartCoroutine(EnableReplayButtonCoroutine());
    }

    IEnumerator EnableReplayButtonCoroutine()
    {
        yield return new WaitForSeconds(3f);
        replayButton.SetActive(true);
    }
}
