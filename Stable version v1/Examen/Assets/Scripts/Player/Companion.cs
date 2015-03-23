using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Companion : MonoBehaviour
{

    Color newColour = new Color(0, 0, 0);

    Vector3 Origin = Vector3.zero;
    Vector3 from = Vector3.zero;
    Vector3 HoverTo = Vector3.zero;

    private List<ParticleSystem> sparks = new List<ParticleSystem>();
    private List<Transform> body = new List<Transform>();
    public List<AudioClip> clips = new List<AudioClip>();


    private List<Vector3> bodySize = new List<Vector3>();

    private GameObject objToCatch;

    private AudioSource audio;


    float Colortimer = 6f;
    float bodyTimer = 10f;
    float Hovertimer = 1f;

    bool newCatch = true;

    float startTime = 0.0f;
    float catchTime = 0.35f;

    float fracComplete = 0.0f;

    void Awake()
    {
        Origin = transform.position;
        HoverTo = Origin;
        from = Origin;
        audio = GetComponent<AudioSource>();
    }

    void Start()
    {
        foreach (Transform t in transform)
        {
            if (t.GetComponent<ParticleSystem>())
                sparks.Add(t.GetComponent<ParticleSystem>());
            if (t.gameObject.name[0] == 'B')
            {
                body.Add(t.transform);
                bodySize.Add(new Vector3(0,0,0));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Voice();
        Form();
        Hover();
    }

    void Voice()
    {

        if (audio.clip && !audio.isPlaying)
        {
            audio.Play();
        }
        else if (clips.Count > 0 && !audio.clip && !audio.isPlaying)
        {
            audio.clip = clips[0];
            clips.RemoveAt(0);
        }

        if (audio.clip)
        {
            if (audio.clip.length == audio.time)
            {
                audio.clip = null;
            }
        }
    }

    void Form()
    {

        light.color = Color.Lerp(light.color, newColour, Colortimer * Time.deltaTime);
        foreach (ParticleSystem p in sparks)
            p.startColor = light.color;

        for (int i = 0; i < body.Count; i++)
        {
            body[i].localScale = Vector3.Lerp(body[i].localScale, bodySize[i], bodyTimer * Time.deltaTime);
            body[i].renderer.material.color = Color.Lerp(light.color, newColour, Colortimer * Time.deltaTime);
            if (body[i].localScale == bodySize[i])
                bodySize[i] = new Vector3(Random.Range(0f, 0.1f), Random.Range(0f, 0.1f), Random.Range(0f, 0.1f));
        }

        if (light.color == newColour)
        {
            newColour = GenerateNewColor();
        }
    }

    void Hover()
    {
        if (objToCatch)
        {
            float elapsedTime = (Time.time - startTime) / catchTime;
            transform.position = Vector3.Slerp(Origin, objToCatch.transform.position, elapsedTime);
            Debug.Log(elapsedTime);
            if (elapsedTime >= 1)
            {
                startTime = Time.time;
                objToCatch.rigidbody.velocity = Vector3.zero;
                from = transform.position;
                objToCatch = null;
            }
        }
        else if (!newCatch)
        {
            float elapsedTime = (Time.time - startTime) / catchTime;
            transform.position = Vector3.Slerp(from, Origin, elapsedTime);
            if (elapsedTime >= 1)
            {
                from = this.transform.position;
                newCatch = true;
            }
        }
        else
        {
            fracComplete = (Time.time - startTime) / Hovertimer;

            transform.position = Vector3.Slerp(from, HoverTo, fracComplete);
            if (fracComplete >= 1)
                NewDestination();
        }
    }

    private void NewDestination()
    {

        if (HoverTo == Origin)
            HoverTo = Origin + new Vector3(0f, 0.3f, 0f);
        else
            HoverTo = Origin;

        from = transform.position;
        startTime = Time.time;
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer("Item"))
            if (newCatch)   
                 newCatch = ControllCatch(coll, coll.gameObject.transform.rigidbody.velocity);
    }

    bool ControllCatch(Collider coll, Vector3 _velocity)
    {
        objToCatch = coll.gameObject;
        startTime = Time.time;
        if (_velocity.x > 1 || _velocity.x < -1)
            return false;
        if (_velocity.y > 1 || _velocity.y < -20)
            return false;
        if (_velocity.z > 1 || _velocity.z < -1)
            return false;


        objToCatch = null;
        return true;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Miai");
    }

    Color GenerateNewColor()
    {
        Vector3 temp = new Vector3((float)Random.Range(1, 500) / 1000, (float)Random.Range(1, 500) / 1000, (float)Random.Range(1, 500) / 1000);
        return new Color(temp.x, temp.y, temp.z);
    }
}
