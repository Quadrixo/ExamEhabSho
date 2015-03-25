using UnityEngine;
public class SingelObjLifter : MonoBehaviour
{

    public GameObject ObjToControll;

    private Intro intro;
    private GameObject player;

    public GameObject NextEvent;

    private Vector3 firstCheck;
    private Vector3 getAway;
    private Vector3 hoverValue;

    private Vector3 itemScale;

    private Vector3 to;
    private Vector3 from;

    private Vector3 CheatColor = new Vector3(1, 0.95f, 0);

    private bool start = false;
    private bool Hover = false;
    private float hoverTime = 5f;

    private float journeyTime = 1f;
    private float fadeTime = 10f;
    private float startTime;
    private float time;
    float fracComplete;

    void Awake()
    {
        intro = GameObject.Find("IntrotManager").GetComponent<Intro>();
        player = GameObject.Find("First Person Controller");

        firstCheck = transform.position + new Vector3(0, 1, 0);
        getAway = firstCheck + new Vector3(0, 15, 0);
        from = ObjToControll.transform.position;
        hoverValue = firstCheck - new Vector3(0, 0.2f, 0);

        itemScale = ObjToControll.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            if (Hover)
            {
                if (Hovering())
                {
                    GetsAway();
                }
            }
            else
                firstDestination();
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (!start)
            if (coll.gameObject == player.gameObject)
            {
                start = true;
                NewDestination(firstCheck, journeyTime);
            }
    }

    bool Hovering()
    {
        if (time >= hoverTime)
            return true;
        else
            time += Time.deltaTime;

        fracComplete = (Time.time - startTime) / journeyTime;

        ObjToControll.transform.position = Vector3.Slerp(from, to, fracComplete);

        if (ObjToControll.transform.position == hoverValue)
            NewDestination(firstCheck, journeyTime);

        else if (ObjToControll.transform.position == firstCheck)
            NewDestination(hoverValue, journeyTime);

        if (time >= hoverTime)
            NewDestination(getAway,5f);
       
        return false;
    }

    private void NewDestination(Vector3 _to, float _time)
    {
        journeyTime = _time;
        to = _to;
        from = ObjToControll.transform.position;
        startTime = Time.time;
    }

    void GetsAway()
    {

        fracComplete = (Time.time - startTime) / journeyTime;

        ObjToControll.transform.position = Vector3.Lerp(from, to, fracComplete);
        ObjToControll.transform.localScale = Vector3.Lerp(itemScale, Vector3.zero, fracComplete);

        fracComplete = (Time.time - startTime) / fadeTime;

        Vector3 colro = Vector3.Slerp(new Vector3(0, 0, 0), CheatColor, fracComplete);
        intro.ChangeWorldColor(colro.x, colro.y);

        if (colro == CheatColor)
        {
            ObjToControll = null;
            start = false;
            Debug.Log("Success");
            if (NextEvent)
            {
                NextEvent.SetActive(true);
                
            }
            Destroy(GameObject.Find("TriggEvent0M"));
        }
    }
    
    void firstDestination()
    {
        fracComplete = (Time.time - startTime) / journeyTime;

        ObjToControll.transform.position = Vector3.Slerp(from, to, fracComplete);

        if (ObjToControll.transform.position == to)
        {
            NewDestination(hoverValue, 0.8f);
            Hover = true;
        }
    }
}
