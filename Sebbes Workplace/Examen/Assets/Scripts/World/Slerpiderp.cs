using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Slerpiderp : MonoBehaviour {

    public Transform sunrise;
    public Transform sunset;
    public float journeyTime = 1.0F;
    private float startTime;
    private bool begin = true;
    float fracComplete;
    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        if (begin)
        {
            Vector3 center = (sunrise.position + sunset.position) * 0.5F;
            center -= new Vector3(0, 0, -2f);
            Vector3 riseRelCenter = sunrise.position - center;
            Vector3 setRelCenter = sunset.position - center;
            fracComplete = (Time.time - startTime) / journeyTime;
            transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, fracComplete);
            transform.position += center;
            if (transform.position == sunset.position)
                begin = false;
        }
    }

    public void Begin()
    {
        if (!begin)
        {
            begin = true;
            startTime = Time.time;
        }
    }

    public bool beginText
    {
        get
        {
            return (fracComplete > 0.3f);
        }
    }
}



