using System;
using UnityEngine;
using UnityEngine.UI;

public class UiEnd : MonoBehaviour
{
    public Image image;
    public Sprite[] sprites;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetUp(int spriteID)
    {
        image.sprite = sprites[spriteID];
    }
}
