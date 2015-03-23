using UnityEngine;
using System.Collections;

public class RotateAround : MonoBehaviour
{

    public GameObject orbitRound;
    public float rotateSpeed = 0.5f;
    public Vector3 angle = new Vector3(0.0f, 0.0f, 1.0f);
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.RotateAround(orbitRound.transform.position, angle, rotateSpeed / 4.0f);
    }
}
