using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIScreen : MonoBehaviour {

    InteractionIntro Interaction = new InteractionIntro();

    List<GameObject> EventList = new List<GameObject>();

    ObjFader background; // Bakgrunden
    PlayerPower Player; // Spelaren
    GameObject Talker;

    PauseMeny pause;
    PlayWindow play;

    public enum TextPositions { Left, Right, Middle, TopLeft }

    private TextPositions currentPos;


	void Start () {

        Player = GameObject.Find("First Person Controller").GetComponent<PlayerPower>();
        pause = GameObject.Find("Pause").GetComponent<PauseMeny>();
        play = GameObject.Find("Play").GetComponent<PlayWindow>();
        Talker = GameObject.Find("Fee");
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Y))
            PauseMeny(!pause.isPaused);
	}

    public void PauseMeny(bool activate)
    {
        Debug.Log("ss");
        if (activate)
            pause.setPauseWindow(global::PauseMeny.PauseState.Begin);
        else
            pause.Continue();

        Player.Freeze = activate;
    }

    public PauseMeny pauseMeny
    {
        get
        {
            if (!pause)
                return GameObject.Find("Pause").GetComponent<PauseMeny>();
            return pause;
        }
    }

    public PlayWindow playWindow
    {
        get
        {
            return play;
        }
    }


}
