using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Intro : MonoBehaviour {

    public float fadeSpeed = 0.8f;
    public enum TypeEvent {Message, Interaction, Talking, None };
    private TypeEvent CurrentEvent = TypeEvent.None;

    MessageIntro[] text = new MessageIntro[4];
    InteractionIntro Interaction = new InteractionIntro();

    //GameObject floor;

    List<GameObject> EventList = new List<GameObject>();

    //ObjFader background; // Bakgrunden
    Slerpiderp light; // TextLjuset
    PlayerPower Player; // Spelaren
    GameObject Talker;

    public float waitTime;
    private float timer;

    void Awake()
    {
        for (int i = 0; i < text.Length; i++)
        {
            text[i] =  new MessageIntro(GameObject.Find("Text" + (i + 1)).GetComponent<Text>());
        }
        Player = GameObject.Find("First Person Controller").GetComponent<PlayerPower>();

        //light = GameObject.Find("TextLight").GetComponent<Slerpiderp>();

        //background = GameObject.Find("FadeCube").GetComponent<ObjFader>();
        //background.SetBackground(1);
        //floor = GameObject.Find("Ground");
        Talker = GameObject.Find("Fee");

        

        Wait(0f);
    }
    void Start()
    {
        Player.FreezePlayerMouse(true);
    }

	void Update () {
        if (EventHandler())
        {
            if (EventList.Count > 0)
            {
                Debug.Log("miau");
                foreach (GameObject g in EventList)
                    g.SetActive(true);
                EventList.Clear();
            }
        }
    }

    public bool IsReady
    {
        get
        {
            return (EventList.Count == 0);
        }
    }

    public void ChangeWorldColor(float _back, float _ground)
    {
        Color background = new Color(_back, _back, _back);
        Color ground = new Color(_ground, _ground, _ground);

        RenderSettings.fogColor = background;
        Camera.main.backgroundColor = background;
        //floor.renderer.material.color = ground;

        foreach (MessageIntro t in text)
            t.fixColor(_back);
    }

    bool EventHandler()
    {
        if ((Time.time - timer) / waitTime >= 1)
            switch (CurrentEvent)
            {
                case TypeEvent.Message:
                    return MessageUpdate(posi);

                case TypeEvent.Interaction: // Måste klicka på något för att du ska kunna gå vidare
                    return InteractionUpdate(posi);

                case TypeEvent.Talking:
                    return true;

                case TypeEvent.None:
                default:
                    return true;
            }
        else
            return false;
    }

    private int posi = 0;
    
    public void InfoEvent(TypeEvent _type,List<GameObject> _activation, int pos, List<string> message)
    {
        Debug.Log(_type);
        
        CurrentEvent = _type;
        EventList = _activation;
        posi = pos;
        //switch (_type) // vet ej vad jag ska göra med denna, återstår att se :)
        //{
        //    case TypeEvent.Message:
        //        Message.SetMessageText(text[pos], message);
        //        break;
        //    case TypeEvent.Interaction:
        //        Interaction.SetInteractionText(text[pos], message);
        //        break;
        //    default:
        //        break;
        //}

    }

    public void TalkEvent(AudioClip _clip)
    {
        Talker.GetComponent<AudioSource>().clip = _clip;
    }

    public bool ShakePlayer()
    {
        if (Player)
        {
            Player.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0.1f, 0));
            return false;
        }
        else
            return true;
    }

    bool MessageUpdate(int pos)
    {
        //bool check = true;
        //if (background.backGroundAlpha > 0 && !text[pos].isLast)
        //{
        //    if (light.beginText && !text[pos].isLast)
        //    {
        //        check = text[pos].UpdateMessage();
        //    }
        //    if (check && !text[pos].isLast)
        //        light.Begin();
        //}
        //else
        //{
        //    Debug.Log("asa");
        //    if (!text[pos].isLast)
        //        text[pos].UpdateMessage();
        //    else
        //    {
        //        Debug.Log("sa");
        //        CurrentEvent = TypeEvent.None;
        //        light.gameObject.SetActive(false);
        //    }

        //}
        return text[pos].isLast;
    }

    bool InteractionUpdate(int pos)
    {
        //if (Interaction.UpdateInteraction(text[pos].textis));
        //    if (Input.GetKeyDown(KeyCode.Return))
        //    {
        //        if (background.backGroundAlpha == 1)
        //            background.BeginFade(-1);

        //        Interaction.endFade();
        //        Player.GetComponent<CharacterMotor>().enabled = true;
        //        Player.FreezePlayerMouse(false);
        //    }

        //if (background.backGroundAlpha == 0)
        //{
        //    if (Interaction.doneFading)
        //    {
        //        //if (Interaction.InteractionMess)
        //        //    NextEvent(TypeEvent.Message, 0, Interaction.NextEventVaule());
        //        //else if (Interaction.InteractionWorld)
        //        //    NextEvent(TypeEvent.WorldEvent, 0, Interaction.NextEventVaule());
        //        //else
        //        //{
        //        //    CurrentEvent = TypeEvent.None;
        //        //}
        //        Interaction.ResetCounter();
        //        return true;
        //    }
        //    else
        //        return false;
        //}
        //else
        //{
        //    //foreach (Text t in text)
        //    //    Interaction.fixColor(t, background.backGroundAlpha);
        //    return false;
        //}
        return false;
    }

    private void Wait(float amount)
    {
        waitTime = amount;
        timer = Time.time;
    }

}
