using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Popper : MonoBehaviour
{

    public List<GameObject> ObjToPopUp;

    private List<Vector3> originSize;

    public List<GameObject> ActivateNextEvents;

    public bool ShootUp;
    public float Strength;

    public float Speed = 1f;
    private float timeSpammer;

    private float timer;

    public bool GiveGravity;

    
    public GameObject SingelMultiPop;
    public Transform dropPosition;
    private Vector3 sMPopOrigin;
    
    public List<float> values;
    private int valueCounter =0;

    private float SpawnSpeed;
    
    public int amount;
    // Use this for initialization
    void Start()
    {
        originSize = new List<Vector3>();
        sMPopOrigin = SingelMultiPop.transform.localScale;
        SingelMultiPop.transform.localScale = Vector3.zero;

        if (amount > 0)
            SpawnSpeed = Speed / amount;
        foreach (GameObject g in ObjToPopUp)
        {
            originSize.Add(g.transform.localScale);
            g.transform.localScale = Vector3.zero;
            if (g.GetComponent<Rigidbody>())
                g.GetComponent<Rigidbody>().useGravity = false;
        }
        timeSpammer = Time.time;
        timer = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        float fraction = (Time.time - timer) / Speed;

        for (int i = 0; i < originSize.Count; i++)
        {
            ObjToPopUp[i].transform.localScale = Vector3.Slerp(Vector3.zero, originSize[i], fraction);
        }

        if (amount > 0 && SingelMultiPop)
        {
            fraction = (Time.time - timeSpammer) / SpawnSpeed;
            if (fraction >= 1)
            {
                timeSpammer = Time.time;
                GameObject temp = (GameObject)GameObject.Instantiate(SingelMultiPop, dropPosition.position, Quaternion.identity);
                temp.GetComponent<ItemProperties>().doResize = true;
                if (ShootUp)
                    temp.GetComponent<Rigidbody>().AddForce(Random.Range(0, 100), Strength, Random.Range(0, 100));

                if (valueCounter < values.Count)
                {
                    temp.GetComponent<ItemProperties>().itemValue = values[valueCounter];
                    valueCounter++;
                }
                else if (amount >= 1)
                {
                    temp.GetComponent<ItemProperties>().itemValue = values[Random.Range(0, values.Count - 1)];
                }
                
                amount--;
            }
        }

       


        if (fraction >= 1 )
        {
            for (int i = 0; i < originSize.Count; i++)
            {
                ObjToPopUp[i].transform.localScale = originSize[i];
                if (ObjToPopUp[i].GetComponent<Rigidbody>())
                    ObjToPopUp[i].GetComponent<Rigidbody>().useGravity = true;
            }
            if (ActivateNextEvents.Count > 0)
                foreach (GameObject g in ActivateNextEvents)
                    g.SetActive(true);
        }


    }

}
