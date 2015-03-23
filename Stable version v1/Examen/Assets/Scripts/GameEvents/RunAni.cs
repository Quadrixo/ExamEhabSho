using UnityEngine;
using System.Collections;

public class RunAni : MonoBehaviour {
    Animation a;


    public float playSpeed = 0.3f;

    public bool PlayerDist = false;
    public float Distance = 0.0f;

    public bool Grow = false;
    private Vector3 standardSize;

    public bool keepRotating = false;

    public AnimationClip clip;

    float currentTime = 0;

    GameObject player;



	// Use this for initialization
	void Start () {
        a = GetComponent<Animation>();
        player = GameObject.Find("First Person Controller");
        animation[clip.name].time = clip.length - 0.01f;
        a.Play(clip.name);
        animation[clip.name].speed = 0;
        
        if (Grow)
        {
            standardSize = this.transform.localScale;
            this.transform.localScale = Vector3.zero;

        }
	}
	
	// Update is called once per frame
	void Update () {

        if(PlayerDist)
        {
            if (Vector3.Distance(player.transform.position, this.transform.position) < Distance)
            {
                playSpeed = playSpeed * clip.length;
                animation[clip.name].speed = -playSpeed;
                a.Play(clip.name);
                animation[clip.name].time = clip.length/3 - 0.01f;

                PlayerDist = !PlayerDist;
            }
        }
        else if(a.isPlaying)
        {
            if (currentTime <= 1 && Grow)
            {

                currentTime += Time.deltaTime * playSpeed;
                this.transform.localScale = Vector3.Slerp(Vector3.zero, standardSize, currentTime);
                if(currentTime >= 1)
                    this.enabled = false;

            }
        }
        else if(!a.isPlaying && keepRotating)
            transform.Rotate(new Vector3(0, 1, 0), 2);

	}
    

    void OnTriggerEnter(Collider coll)
    {
        if (!PlayerDist)
            if (coll.gameObject == player.gameObject)
            {

                playSpeed = playSpeed * clip.length;
                
                animation[clip.name].speed = -playSpeed;
                a.Play(clip.name);
            }
    }

    void OnTriggerExit(Collider coll)
    {
        //if (coll.gameObject == player.gameObject)
        //{
        //    animation["PlusAnimation"].speed = playSpeed * 2;
        //    a.Play("PlusAnimation");
        //}
    }

}
