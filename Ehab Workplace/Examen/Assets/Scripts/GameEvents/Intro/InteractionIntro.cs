using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class InteractionIntro {

    List<string> mess = new List<string>();
    // n = nextEvent, i = press´action, s = Wait till next event, u = no input 
    // l = lookat, n = nextEvent, u = No input

    private int CurrentText = 0;
    private int CurrentLine = 0;

    Color lowColor = new Color(0.15f, 0.15f, 0.15f);
    Color HighColor = new Color(0.72f, 0.72f, 0.72f);


    public float fadeSpeed = 2f;
    public float wait = 1.5f;
    private float fixedAlpha = 0.0f;

    private float alpha = 0f;
    private int fadeDir = 1;
    
    private float timer;
    private float startTime;

    private bool fadeIn = false;
    private bool fadeOut = false;

    public bool UpdateInteraction(Text text)
    {
        if (fadeIn)
        {
            
            alpha += fadeDir * 1.3f * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            if (alpha == fadeDir)
                fadeIn = !fadeIn;
        }
        else if (fadeOut)
        {
            timer = (Time.time - startTime) / wait;
            if (timer > 1)
            {
                alpha += fadeDir * 0.4f * Time.deltaTime;
                alpha = Mathf.Clamp01(alpha);
                text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
                if (alpha == 1 + fadeDir)
                {
                    fadeOut = !fadeOut;
                    GameObject.Find("FadeCube").SetActive(false);
                }
            }
        }

        return !(fadeIn || fadeOut);
    }

    public bool isInteraction
    {
        get
        {
            return (mess[CurrentText] == "p");
        }
    }

    public bool isLast
    {
        get
        {
            return (CurrentText == mess.Count - 1);
        }
    }

    public void fixColor(Text t,float value)
    {
        fixedAlpha = value;
        t.color = lowColor + (HighColor * value);
        t.color = new Color(t.color.r, t.color.g, t.color.b, alpha);
    }
    
    public void SetInteractionText(Text t, List<string> _value)
    {
        
        t.color = new Color(t.color.r, t.color.g, t.color.b, 0);
        alpha = 0;
        mess = _value;
        t.text = mess[CurrentText];
        fadeIn = true;
        fadeDir = 1;
    }  

    public void endFade()
    {
        fadeDir = -1;
        fadeOut = true;
    }

    public bool doneFading
    {
        get
        {
            return !fadeOut;
        }
    }

    public void HideText(Text t)
    {
        t.color = new Color(t.color.r, t.color.g, t.color.b, 0);
    }

    public void ResetCounter()
    {
        CurrentText = 0;
    }
}
