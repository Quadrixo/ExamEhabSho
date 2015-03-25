using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class PauseMeny : MonoBehaviour
{


    List<GameObject> children = new List<GameObject>();
    int amountOfChildrens = 0;
    float pauseTime = (float)1 / 60;
    float timer;

    public float TransSpeed = 1f;
    GlobalItems lod;

    private bool Change = false;


    public enum PauseState { Begin, Main, Alternative, Continue, Exit };
    private PauseState state = PauseState.Continue;


    // Use this for initialization
    void Start()
    {
        Color c = GetComponent<Image>().color;

        GetComponent<Image>().color = new Color(c.r, c.g, c.b, 0);

        foreach (Transform t in transform)
            children.Add(t.gameObject);

        foreach (RectTransform t in children[0].transform)
        {
            GlobalItems.NewSize(t);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Change)
        {
            timer += pauseTime;
            switch (state)
            {
                case PauseState.Begin:
                    GetComponent<Image>().color = alpiTest(GetComponent<Image>().color, 1, 1);
                    for (int i = 0; i < amountOfChildrens; i++)
                    {
                        GameObject go = children[0].transform.GetChild(i).gameObject;

                        if (go.GetComponent<Text>())
                        {
                            if(i == 0)
                                go.GetComponent<Text>().color = alpiTest(go.GetComponent<Text>().color, 0.7f, 0.68f);
                            go.GetComponent<Text>().color = alpiTest(go.GetComponent<Text>().color, 0.7f, 1);
                        }
                        if (go.gameObject.GetComponent<Image>())
                        {
                            go.GetComponent<Image>().color = alpiTest(go.GetComponent<Image>().color, 0.7f, 0.25f);
                        }

                        if (timer >= 0.5f)
                            children[0].SetActive(true);
                    }

                    if (timer >= 1)
                    {
                        Change = false;
                    }
                    break;
                case PauseState.Continue:
                    GetComponent<Image>().color = alpiTest(GetComponent<Image>().color, -1, 1);
                    for (int i = 0; i < amountOfChildrens; i++)
                    {
                        GameObject go = children[0].transform.GetChild(i).gameObject;

                        if (go.GetComponent<Text>())
                        {
                            go.GetComponent<Text>().color = alpiTest(go.GetComponent<Text>().color, -0.7f, 1);
                        }
                        if (go.gameObject.GetComponent<Image>())
                        {
                            go.GetComponent<Image>().color = alpiTest(go.GetComponent<Image>().color, -0.7f, 1);
                        }



                        else if (go.GetComponent<Image>())
                            if (timer >= 0.3f)
                            {
                                go.SetActive(false);
                                go.GetComponent<Image>().color = alpiTest(go.GetComponent<Image>().color, -50.7f, 1);
                            }

                    }
                    if (timer >= 1)
                    {
                        foreach (GameObject g in children)
                            g.SetActive(false);
                        Change = false;
                    }

                    break;
            }
        }

    }

    private Color alpiTest(Color col, float dir, float amount)
    {
        float alpha = col.a + dir * pauseTime;
        alpha = Mathf.Clamp(alpha, 0, amount);
        return new Color(col.r, col.g, col.b, alpha);
    }

    public void Continue()
    {
        state = PauseState.Continue;
        timer = 0;
        Change = true;
    }

    public void setPauseWindow(PauseState _value)
    {
        state = _value;
        switch (state)
        {
            case PauseState.Begin:

                amountOfChildrens = 0;
                foreach (GameObject g in children)
                    g.SetActive(true);

                foreach (Transform g in children[0].transform)
                {
                    amountOfChildrens++;
                    g.gameObject.SetActive(true);
                    Color c;
                    if (g.gameObject.GetComponent<Text>())
                    {
                        c = g.gameObject.GetComponent<Text>().color;
                        g.gameObject.GetComponent<Text>().color = new Color(c.r, c.g, c.b, 0);
                    }
                    if (g.gameObject.GetComponent<Image>())
                    {
                        c = g.gameObject.GetComponent<Image>().color;
                        g.gameObject.GetComponent<Image>().color = new Color(c.r, c.g, c.b, 0);
                    }
                }
                break;

        }
        timer = 0;
        Change = true;
    }

    public bool isPaused
    {
        get
        {
            return (state != PauseState.Continue);
        }
    }
}
