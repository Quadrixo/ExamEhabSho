using UnityEngine;
using System.Collections;

public class UIAspect {


    private Vector3 size;
    private Vector3 pos;

    public UIAspect(GameObject obj)
    {

        size = obj.transform.localScale;
        pos = obj.transform.position;
    }


    public Vector3 oldSize
    {
        get
        {
            return size;
        }
    }

    public Vector3 oldpos
    {
        get
        {
            return pos;
        }
    }



}
