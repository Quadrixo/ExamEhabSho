using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TagInfoHandler
{



    private GameObject obj;

    private Image background;
    private Text content;

    private float Clock = (float)1 / 60;
    private float fraction = 0;

    private bool changeColor = false;

    float amount = 0.7f;

    public enum state { Begin, End };
    private state currentState = state.End;

    public TagInfoHandler(GameObject _obj)
    {
        obj = _obj;

        foreach (Transform t in _obj.transform)
        {
            if (t.gameObject.GetComponent<Image>())
                background = t.gameObject.GetComponent<Image>();
            else
                content = t.gameObject.GetComponent<Text>();
        }


        Color c = background.color;
        background.color = new Color(c.r, c.g, c.b, 0);
        c = content.color;
        content.color = new Color(c.r, c.g, c.b, 0);
        background.transform.localScale
            = content.transform.localScale
            = new Vector3(0, 0, 1);
    }

    public void Initiate(string[] value)
    {
        changeColor = true;
        fraction = 0;
        if (value != null)
        {
            content.text = "";

            currentState = state.Begin;
            for (int i = 0; i < value.Length; i++)
                content.text += value[i] + "\n";
        }
        else
            currentState = state.End;
    }

    public void End()
    {
        fraction = 0;
        changeColor = true;
        currentState = state.End;
    }

    public void Update()
    {
        if (changeColor)
        {
            fraction += Clock;

            switch (currentState)
            {
                case state.Begin:
                    background.color = alpiTest(background.color, amount * 2, amount);
                    content.color = alpiTest(content.color, amount * 2, 0.7f);
                    background.transform.localScale =
                        content.transform.localScale =
                        Vector3.Lerp(background.transform.localScale, new Vector3(1, 1, 1), fraction / amount);
                    
                    break;
                case state.End:
                    background.color = alpiTest(background.color, -amount * 2, amount);
                    content.color = alpiTest(content.color, -amount * 2, amount);
                    background.transform.localScale =
                        content.transform.localScale =
                        Vector3.Lerp(background.transform.localScale, new Vector3(0, 0, 1), fraction / amount);
                    if (fraction >= 1)
                    {
                        content.text = "";
                    }
                    break;
            }
            if (fraction >= 1)
            {
                changeColor = false;
            }
        }
    }

    private Color alpiTest(Color col, float dir, float _amount)
    {
        float alpha = col.a + dir * Clock;
        alpha = Mathf.Clamp(alpha, 0, _amount);
        return new Color(col.r, col.g, col.b, alpha);
    }
    public bool activated
    {
        get
        {
            return (currentState == state.Begin);
        }
    }
}
