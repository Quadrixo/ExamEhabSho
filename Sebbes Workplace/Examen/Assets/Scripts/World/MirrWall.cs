using UnityEngine;
using System.Collections;

public class MirrWall : MonoBehaviour {

    public GameObject mirror;

    public enum Face { Front, Back, Left, Right, Top, Bottom };

    public Face theSide;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}





    void OnTriggerEnter(Collider coll)
    {
        Debug.Log("t");
        switch(theSide)
        {
            case Face.Front:
                coll.gameObject.transform.position = new Vector3(coll.gameObject.transform.position.x, coll.gameObject.transform.position.y, mirror.transform.position.z - 2);
                break;
            case Face.Back:
                coll.gameObject.transform.position = new Vector3(coll.gameObject.transform.position.x, coll.gameObject.transform.position.y, mirror.transform.position.z + 2);
                break;
            case Face.Left:
                coll.gameObject.transform.position = new Vector3(mirror.transform.position.x + 2, coll.gameObject.transform.position.y, coll.gameObject.transform.position.z);
                break;
            case Face.Right:
                coll.gameObject.transform.position = new Vector3(mirror.transform.position.x - 2, coll.gameObject.transform.position.y, coll.gameObject.transform.position.z);
                break;
            case Face.Top:
                coll.gameObject.transform.position = new Vector3(coll.transform.position.x, mirror.gameObject.transform.position.y + 2, coll.gameObject.transform.position.z);
                break;
            case Face.Bottom:
                coll.gameObject.transform.position = new Vector3(coll.transform.position.x, mirror.gameObject.transform.position.y - 2, coll.gameObject.transform.position.z);
                break;
            default:
                break;
        }
    }
}
