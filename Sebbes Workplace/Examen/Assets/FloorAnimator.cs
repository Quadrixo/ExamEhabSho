using UnityEngine;
using System.Collections;

public class FloorAnimator : MonoBehaviour {



    public bool ActivateOnActive;

    public Vector3 startSize;
    public Vector3 endSize;
    public float GrowSpeed;
    public float ActivationDistance;


    private float amount = 0;
    private GameObject player;
    private Animator animator;


	// Use this for initialization
	void Start () {

        animator = GetComponent<Animator>();

        if (ActivateOnActive)
        {
            animator.Play("Build");
            animator.SetBool("Is_building", true);
            ActivateOnActive = false;
        }

        if (ActivationDistance > 0)
            player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {


        if(ActivationDistance > 0 && !animator.GetBool("Is_building"))
        {
            if(Vector3.Distance(this.transform.position,player.transform.position) < ActivationDistance)
                animator.SetBool("Is_building", true);
        }

        if (animator.GetBool("Is_building"))
        {
            if (GrowSpeed != 0 && amount < 1)
            {
                amount += Time.deltaTime * GrowSpeed/2;
                transform.localScale = Vector3.Slerp(startSize, endSize, amount);
            }
        }
	}
}
