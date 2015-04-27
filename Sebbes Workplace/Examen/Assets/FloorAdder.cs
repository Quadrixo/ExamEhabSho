using UnityEngine;
using System.Collections;

public class FloorAdder : MonoBehaviour {



    MeshRenderer render;
    BoxCollider coll;

    Transform player;

    Collider[] grannar;

    public bool Spread = false;

    public float distance = 5;

    public float speed = 0.4f;

    private float fraction = 0;

    private float Clock = (float)1 / 60;

    private bool doh = false;


	// Use this for initialization
	void Start () {

        render = GetComponent<MeshRenderer>();
        coll = GetComponent<BoxCollider>();

        player = GameObject.Find("Player").transform;


        coll.enabled = false;
        render.enabled = false;
        transform.localScale = new Vector3(0, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {

        if (Spread)
            Spreading();
        else
            Carrying();


        if (doh && fraction <= 1)
        {
            fraction += Clock;
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, 1, 1), fraction * 2);
            if(fraction >=1)
            {
                GetComponent<FloorAdder>().enabled = false;
                coll.enabled = false;
            }
        }
        else if (!doh && coll.enabled)
        {

            fraction += Clock;
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0, 0, 0), fraction * 2);
            if (fraction >= 0.4f)
            {
                coll.enabled = false;
                render.enabled = false;
            }
        }
	
	}

    private void Spreading()
    {

        if (distance > Vector3.Distance(this.transform.position, player.position) && !coll.enabled)
        {
            doh = true;
            fraction = 0;
            coll.enabled = true;
            render.enabled = true;
        }
        else if (!coll.enabled)
        {
            grannar = Physics.OverlapSphere(transform.position, 1);
            foreach (Collider c in grannar)
            {
                if (c.gameObject.layer == LayerMask.NameToLayer("Floor"))
                    if(c.GetComponent<FloorAdder>())
                    {
                        if (c.GetComponent<FloorAdder>().doh && c.transform.localScale.x >= 0.5f)
                        {
                            doh = true;
                            fraction = 0;
                            coll.enabled = true;
                            render.enabled = true;
                        }
                    }
            }
        }
    }

    private void Carrying()
    {
        if (distance > Vector3.Distance(this.transform.position, player.position) && !coll.enabled)
        {
            fraction = 0;
            coll.enabled = true;
            render.enabled = true;
            doh = true;
        }
        else if (distance < Vector3.Distance(this.transform.position, player.position) && doh && )
        {
            fraction = 0;
            doh = false;
        }

        if (doh && fraction <= 1)
        {
            fraction += Clock;
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, 1, 1), fraction * 2);
        }
        else if (!doh && coll.enabled)
        {

            fraction += Clock;
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0, 0, 0), fraction * 2);
            if (fraction >= 0.4f)
            {
                coll.enabled = false;
                render.enabled = false;
            }
        }
    }
    public bool holymoly
    {
        get
        {
            return doh;
        }
    }
}
