using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class MessageIntro
{
    //Dictionary<int, List<string>> message = new Dictionary<int,List<string>>();

    List<string> mess = new List<string>();
    // n = nextEvent, i = press´action, s = Wait till next event, u = no input 
    // l = lookat, n = nextEvent, u = No input

    private Text theText;

    public MessageIntro(Text t)
    {
        theText = t;
    }

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

    private bool StayOnCommand = false;

    public bool UpdateMessage()
    {
        if(fadeIn)
        {
            alpha += fadeDir * fadeSpeed * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);
            theText.color = new Color(theText.color.r, theText.color.g, theText.color.b, alpha);  
            if(alpha == fadeDir)
            {
                startTime = Time.time;
                if (!StayOnCommand)
                {
                    fadeDir = -fadeDir;
                    fadeIn = !fadeIn;
                    fadeOut = !fadeIn;
                }
            }
        }
        else if(fadeOut)
        {
            timer = (Time.time - startTime) / wait;
            if(timer > 1)
            {
                alpha += fadeDir * fadeSpeed * Time.deltaTime;
                alpha = Mathf.Clamp01(alpha);
                theText.color = new Color(theText.color.r, theText.color.g, theText.color.b, alpha);
                if (alpha == 1+fadeDir)
                    fadeOut = !fadeOut;
            }
        }

        if (!fadeIn && !fadeOut)
        {

            if (isLast)
            {
                SetMessageText(mess, false);
            }
            else
            {
                theText.text = "";
            }
        }
        return !(fadeIn||fadeOut);
    }

    public bool isLast
    {
        get
        {
            return (1 <= mess.Count);
        }
    }

    public void fixColor(float value)
    {
        fixedAlpha = value;
        theText.color = lowColor + (HighColor * value);
        theText.color = new Color(theText.color.r, theText.color.g, theText.color.b, alpha);
    }

    public void SetMessageText(List<string> _value, bool _stay)
    {
        theText.color = new Color(theText.color.r, theText.color.g, theText.color.b, 0);
        alpha = 0;
        mess = _value;
        theText.text = mess[0];
        mess.RemoveAt(0);
        Debug.Log(theText.text);
        fadeIn = true;
        fadeDir = 1;

        StayOnCommand = _stay;
    }

    public void SetMessageText(string _value, bool _stay)
    {
        theText.color = new Color(theText.color.r, theText.color.g, theText.color.b, 0);
        alpha = 0;
        theText.text = _value;
        fadeIn = true;
        fadeDir = 1;

        StayOnCommand = _stay;
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

    public void HideText()
    {
        theText.color = new Color(theText.color.r, theText.color.g, theText.color.b, 0);
    }

    public Text textis
    {
        get
        {

            return theText;
        }
    }

    public Color textColor
    {

        set
        {
            theText.color = value;
        }
        get
        {
            return theText.color;
        }
    }
}
