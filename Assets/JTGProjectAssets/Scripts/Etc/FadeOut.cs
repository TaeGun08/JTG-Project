using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    private Image fadeOut;
    [SerializeField] private bool fadeIn = false;
    [SerializeField] private float fadeTimer = 3.0f;
    private Color fadeColor;
    private uint rate = 144;

    private void Awake()
    {
        fadeOut = GetComponent<Image>();

        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed, new RefreshRate() { numerator = rate });
    }

    private void Update()
    {
        fadeColor = fadeOut.color;

        if (fadeIn == true && fadeColor.a != 0.0f)
        {
            fadeColor.a -= Time.deltaTime / fadeTimer;

            if (fadeColor.a < 0.0f)
            {
                fadeColor.a = 0.0f;
            }

            fadeOut.color = fadeColor;
        }
        else if (fadeIn == false && fadeColor.a != 1.0f)
        {
            fadeColor.a += Time.deltaTime / fadeTimer;

            if (fadeColor.a > 1.0f)
            {
                fadeColor.a = 1.0f;
            }

            fadeOut.color = fadeColor;
        }
    }
}
