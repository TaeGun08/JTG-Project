using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    private Image fadeImage;
    [SerializeField] private bool fadeOut = false;
    [SerializeField] private float fadeTimer = 3.0f;
    private Color fadeColor;
    private bool stop = false;
    //private uint rate = 144;

    private void Awake()
    {
        fadeImage = GetComponent<Image>();

        //Screen.SetResolution(1920, 1080, FullScreenMode.Windowed, new RefreshRate() { numerator = rate });       
    }

    private void Update()
    {
        fadeColor = fadeImage.color;

        if (fadeOut == false && fadeColor.a != 0.0f)
        {
            fadeColor.a -= Time.deltaTime / fadeTimer;

            if (fadeColor.a < 0.0f)
            {
                fadeColor.a = 0.0f;
            }

            fadeImage.color = fadeColor;
        }
        else if (fadeOut == true && fadeColor.a != 1.0f)
        {
            fadeColor.a += Time.deltaTime / fadeTimer;

            if (fadeColor.a > 1.0f)
            {
                fadeColor.a = 1.0f;
            }

            fadeImage.color = fadeColor;
        }
    }

    public void FadeInOut(bool _fade)
    {
        if (stop == false)
        {
            fadeOut = _fade;
            stop = true;
        }
    }
}
