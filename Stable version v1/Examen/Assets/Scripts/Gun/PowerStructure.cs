using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PowerStructure : MonoBehaviour
{

    // Use this for initialization

    public enum type { Sphere, Cube };
    public type shootType;

    public GameObject textValue;

    private bool isFired = false;

    private RaycastHit rayHit;

    private Vector3 m_TargetScale = Vector3.zero;
    private Vector3 m_Direction = Vector3.zero;
    private Vector3 m_tempScale = Vector3.zero;


    public void InitiatieValues(Vector3 _scale, Vector3 _direction, bool _isFired)
    {
        m_Direction = _direction;
        isFired = _isFired;
        m_TargetScale = _scale;
        //textValue = (GameObject)Instantiate(GameObject.Find("ValueText"),
        //    transform.position + new Vector3(-1.2f,1.8f,0),
        //    Quaternion.Euler(Vector3.RotateTowards(transform.position,Camera.main.transform.position, 5,5)));
        textValue = (GameObject)Instantiate(textValue);
        textValue.gameObject.GetComponent<TextMesh>().text = this.gameObject.transform.localScale.x.ToString();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isFired)
        {
            #region Gravitation for Objects
            Collider[] cols = Physics.OverlapSphere(transform.position, 50);
            List<Rigidbody> rbs = new List<Rigidbody>();

            foreach (Collider c in cols)
            {
                if (c.gameObject.layer == LayerMask.NameToLayer("Item"))
                {
                    Rigidbody rb = c.attachedRigidbody;
                    if (rb != null && rb != rigidbody && !rbs.Contains(rb))
                    {
                        rbs.Add(rb);

                        Vector3 offset = transform.position - c.transform.position;
                        rb.AddForce(offset / offset.sqrMagnitude * rigidbody.mass * 40);
                    }
                }
            }
            #endregion

            #region ExtraCollCheck
            RaycastHit hitInfoFromObject;

            if (Physics.Linecast(transform.position, ObjectDirection(),
                           out hitInfoFromObject, ~(1 << LayerMask.NameToLayer("Player"))))
                if (hitInfoFromObject.collider.gameObject.layer == LayerMask.NameToLayer("WorldPiece"))
                {
                    rigidbody.useGravity = true;
                    rigidbody.isKinematic = false;
                    collider.isTrigger = false;
                    isFired = false;
                }
            #endregion
        }

        textValue.transform.rotation = Quaternion.LookRotation(LookAtPlayerTextRot());

        textValue.transform.position = transform.position;
    }

    private Vector3 LookAtPlayerTextRot()
    {
        return Vector3.RotateTowards(Camera.main.transform.forward, transform.position - Camera.main.transform.position, 100, 0.0f);
    }

    private Vector3 ObjectDirection()
    {
        return transform.position + ((transform.localScale.x + 0.05f) * Camera.main.transform.forward) + new Vector3(0, 0.9f, 0);
    }

    void OnTriggerEnter(Collider coll)
    {
        Debug.Log(coll.gameObject.layer.ToString());
        if (isFired)
        {

            if (coll.gameObject.layer == LayerMask.NameToLayer("Item"))
            {
                Debug.Log("derp");
                transform.localScale += coll.transform.localScale;
                textValue.gameObject.GetComponent<TextMesh>().text = this.gameObject.transform.localScale.x.ToString();
                Destroy(coll.gameObject);
            }

            if (coll.gameObject.layer == LayerMask.NameToLayer("WorldPiece"))
            {

                rigidbody.useGravity = true;
                rigidbody.isKinematic = false;
                collider.isTrigger = false;
                isFired = false;
            }


        }
    }

    void OnTriggerExit(Collider coll)
    {
        Debug.Log(coll.gameObject.layer.ToString());
    }
}

