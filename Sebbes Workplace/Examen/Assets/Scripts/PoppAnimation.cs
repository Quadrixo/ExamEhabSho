using UnityEngine;
using System.Collections;

public class PoppAnimation : MonoBehaviour {


    private Transform player;
    private Animator ani;

    public bool StartWithAnimation = false;
    public float animationSpeed = 1;

    public float growSpeed = 0;
    public Vector3 startSize = new Vector3(0,0,0);
    public Vector3 endSize = new Vector3(1,1,1);


    public float activationDistance = 0;
    public bool outOfRangeRemove = false;
    public bool CanReActivate = false;


    private float amount =  0;
    private bool activate = true;




	// Use this for initialization
	void Start () {

        ani = GetComponent<Animator>();


        ani.speed = 0;
        if(StartWithAnimation)
        {
            ani.speed = animationSpeed;
            ani.Play(0);
        }
        else
            growSpeed *= -1;

        if(activationDistance > 0)
            player = GameObject.Find("Player").transform;
        
	}
	
	// Update is called once per frame
	void Update () {


        if(activationDistance > 0)
        {
            if(Vector3.Distance(this.transform.position,player.position) < activationDistance)
            {
                
               if(ani.speed == 0 && activate)
               {
                   Debug.Log("s");
                   ani.speed = animationSpeed;
                   ani.Play(0);
                   activate = false;
               }

               if (growSpeed < 0)
                   growSpeed *= -0.5f;
            }
            else if(outOfRangeRemove)
            {
                if (growSpeed > 0)
                {
                    growSpeed *= -2;
                    ani.speed = 0;
                }
            }
        }

        if(growSpeed != 0 && amount >= 0 && amount <= 1)
        {
            amount += Time.deltaTime * growSpeed;
            amount = Mathf.Clamp(amount, 0, 1);

            transform.localScale = Vector3.Slerp(startSize, endSize, amount);

            if (CanReActivate && amount == 0)
                activate = true;

        }



	
	}
}
