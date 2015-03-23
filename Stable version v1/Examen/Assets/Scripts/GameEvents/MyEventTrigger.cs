using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyEventTrigger : MonoBehaviour {


    public Intro.TypeEvent type;

    public List<string> Mess = new List<string>();
    private int TextPos;

    public List<GameObject> ActivationList = new List<GameObject>();

    private Intro intro;
    private GameObject player;


    public AudioClip TalkClip;

    private bool shake = true;

    void Awake()
    {
        intro = GameObject.Find("IntrotManager").GetComponent<Intro>();
        player = GameObject.Find("First Person Controller");
        
        switch (type)
        {
            case Intro.TypeEvent.Interaction:
                Mess.Clear();
                Mess.Add("Tryck Enter");
                TextPos = 2;
                break;
            case Intro.TypeEvent.Talking:

                break;
            case Intro.TypeEvent.Message:
                Mess.Add("m");
                TextPos = 0;
                break;
            default:
                break;
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if(coll.gameObject == player.gameObject)
        {
            if (intro.IsReady)
            {
                EngageEvent();
                this.gameObject.SetActive(false);
            }
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject == player.gameObject)
        {
            if (intro.IsReady)
            {
                EngageEvent();
                this.gameObject.SetActive(false);
            }
        }
    }

    void OnTriggerStay(Collider coll)
    {
        if (coll.gameObject == player.gameObject)
        {
            if (intro.IsReady)
            {
                EngageEvent();
                this.gameObject.SetActive(false);
            }
        }
    }

    public void EngageEvent()
    {
        switch(type)
        {
            case Intro.TypeEvent.Message:
            case Intro.TypeEvent.Interaction:
                intro.InfoEvent(type, ActivationList, TextPos, Mess);
                break;
            case Intro.TypeEvent.Talking:
                intro.TalkEvent(TalkClip);
                break;
            default:
                break;
        }
        this.gameObject.SetActive(false);
    }

    void Update()
    {
        if(shake)
        {
            shake = intro.ShakePlayer();
        }
    }
}
