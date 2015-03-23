using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayWindow : MonoBehaviour {


    List<MessageIntro> text = new List<MessageIntro>();
    InteractionIntro Interaction = new InteractionIntro();
    List<GameObject> children = new List<GameObject>();

    List<GameObject> EventList = new List<GameObject>();

    TagInfoHandler left, right;

    ObjFader background; // Bakgrunden

    Image aim;
    Image close;

    private List<float> timers = new List<float>();


    private float speed = 2f;

    public enum TextPositions { Left, Right, Middle, TopLeft }

    private TextPositions currentPos;


    void Start()
    {
        aim = GameObject.Find("AimImage").GetComponent<Image>();
        close = GameObject.Find("CloseImage").GetComponent<Image>();

        foreach (Transform t in transform)
            children.Add(t.gameObject);

        for (int i = 0; i < children.Count; i++)
        {
            if (children[i].GetComponent<Text>())
            {
                text.Add(new MessageIntro(children[i].GetComponent<Text>()));
                text[text.Count - 1].textColor = new Color(text[text.Count - 1].textColor.r, text[text.Count - 1].textColor.g, text[text.Count - 1].textColor.b, 0f);

                timers.Add(0);
            }
            if (children[i].layer == LayerMask.NameToLayer("UILeftTag"))
                left = new TagInfoHandler(children[i]);
            if (children[i].layer == LayerMask.NameToLayer("UIRightTag"))
                right = new TagInfoHandler(children[i]);
        }

        aim.color = new Color(aim.color.r, aim.color.g, aim.color.b, 0);
        close.color = new Color(close.color.r, close.color.g, close.color.b, 0);

        foreach (GameObject g in children)
        {
            if(g.GetComponent<RectTransform>())
                GlobalItems.NewSize(g.GetComponent<RectTransform>());
        }
    }


    // Update is called once per frame
    void Update()
    {
        foreach (MessageIntro m in text)
            m.UpdateMessage();
        left.Update();
        right.Update();
    }

    public void leftInfo(string[] _value)
    {
        if (!left.activated)
            left.Initiate(_value);
        else
            left.End();

    }

    public void rightInfo(string[] _value)
    {
        if (!right.activated)
            right.Initiate(_value);
        else
            right.End();
    }
    

    public void AimImageSwitcher(bool _aim, bool _lifted)
    {
        if (_lifted)
        {
            close.color = alpiTest(close.color, 1, 0.7f);
            aim.color = alpiTest(aim.color, -1, 0.7f);
        }
        else if (_aim)
        {
            close.color = alpiTest(close.color, -1, 0.7f);
            aim.color = alpiTest(aim.color, 1, 0.7f);
        }
        else
        {
            close.color = alpiTest(close.color, -1, 0.7f);
            aim.color = alpiTest(aim.color, -1, 0.7f);
        }
    }

    private Color alpiTest(Color col, int dir, float amount)
    {
        float alpha = col.a + dir * 9 * Time.deltaTime;
        alpha = Mathf.Clamp(alpha, 0, amount);
        return new Color(col.r, col.g, col.b, alpha);
    }

    public void setText(TextPositions pos, string _value)
    {
        timers[(int)pos] = Time.time;

        text[(int)pos].SetMessageText(_value, false);
        currentPos = pos;
    }
}
