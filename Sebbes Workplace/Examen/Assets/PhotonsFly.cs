using UnityEngine;
using System.Collections;

public class PhotonsFly : MonoBehaviour {


    public Vector3 MaxPlayGround;
    public Vector3 MinPlayGround;

    private Vector3 moveToPos;

    private Vector3 Flexing = Vector3.zero;

    public float Maxspeed;
    public float minSpeed;
    private float speedTimer;

    private float timer = 0;

    private Vector3 from;

    private GameObject player;


	// Use this for initialization
	void Start () {

        player = GameObject.Find("Player").gameObject;
        NewMoveDestination();
	}
	
	// Update is called once per frame
	void Update () {
	

        timer += Time.deltaTime;

        Moving();
	}

    private bool Moving()
    {

        float amount = timer / speedTimer;

        Vector3 center = player.transform.position+(from + moveToPos) * 0.5F;
        center -= Flexing;
        Vector3 fromRelCenter = from - center;
        Vector3 toRelCenter = moveToPos - center;
        transform.position = Vector3.Slerp(fromRelCenter, toRelCenter, amount);
        transform.position += center;

        if (amount >= 1)
        {
            timer = 0;
            NewMoveDestination();
            return true;
        }   
        else
            return false;
    }

    private void NewMoveDestination()
    {
        from = transform.position;



        moveToPos = new Vector3((float)Random.Range(MinPlayGround.x,MaxPlayGround.x),
            (float)Random.Range(MinPlayGround.y,MaxPlayGround.y),
            (float)Random.Range(MinPlayGround.z,MaxPlayGround.z)) + player.transform.position;

        Flexing = new Vector3(Random.Range(0f, 3f), Random.Range(0f, 3f), Random.Range(0f, 3f));

        speedTimer = Random.Range(minSpeed, Maxspeed);

        timer = 0f;
    }

}
