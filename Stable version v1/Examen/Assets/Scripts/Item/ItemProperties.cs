using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemProperties : MonoBehaviour {

    RaycastHit rayi;

    private Rigidbody rigid;

    public GameObject hitParticle;
    public GameObject TextValue;

    private List<Pickeditem> m_HasItem = new List<Pickeditem>();

    public float itemValue;

    private float part1Value;
    private float part2Value;

    private bool negative = false;

    private float oldValue;

    public Collider hitMerge;

    private string m_textValue;

    private bool reSize = false;
    private float reSizeTimer;

    private int m_maxParticles = 10;
    private int m_currentParticles = 0;

    private float particleTimer = 0.1f;

    public enum Type { Cube, Globe, Capsule };
    public Type TypeID = Type.Cube;

	// Use this for initialization
	void Start () {
        hitParticle = GlobalItems.g_Particles;
        if (itemValue < 0)
        {
            itemValue *= -1;
            isNegative = true;
        }
	}
	
	// Update is called once per frame
	void Update () {

        if(reSize)
        {
            float fraction = (Time.time - reSizeTimer) / 1;
            transform.localScale = Vector3.Slerp(Vector3.zero, new Vector3(.1f, .1f, .1f), fraction);
            if (fraction >= 1)
                reSize = false;
        }


        if (!TextValue)
        {
            TextValue = (GameObject)GameObject.Instantiate(GlobalItems.g_listOfObj[4], transform.position + new Vector3(0,.2f,0), Quaternion.identity);
            TextValue.transform.parent = this.transform;
            TextValue.GetComponent<TextMesh>().transform.localScale = new Vector3(.1f, .1f, .1f);

        }


        if (gameObject.layer == LayerMask.NameToLayer("Item"))
        {

            if(TypeID == Type.Capsule)
            {
                
                TextValue.GetComponent<TextMesh>().text = part1Value + "\n--\n" + part2Value;
            }
            else
            {
                TextValue.GetComponent<TextMesh>().text = itemValue.ToString();

            }
            TextValue.GetComponent<TextMesh>().transform.localScale = new Vector3(.1f, .1f, .1f);
            TextValue.GetComponent<TextMesh>().color = (negative) ? Color.red : Color.green;

        }
        else
        {

            if (TypeID == Type.Capsule)
            {
                TextValue.GetComponent<TextMesh>().text = part1Value + "\n--\n" + part2Value;
            }
            else
            {
                TextValue.GetComponent<TextMesh>().text = m_textValue;

                if (texten == ".")
                    TextValue.GetComponent<TextMesh>().text = texten;
                else
                    TextValue.GetComponent<TextMesh>().text = itemValue.ToString();
            }
            TextValue.GetComponent<TextMesh>().transform.localScale = new Vector3(.15f, .15f, .15f);

            TextValue.transform.position = transform.position;
            TextValue.GetComponent<TextMesh>().color = (negative) ? Color.red : Color.green;

        }
        TextValue.transform.LookAt(2 * transform.position - Camera.main.transform.position);
        
        if (particleTimer > 0)
            particleTimer -= Time.time;
        else if (m_currentParticles != 0)
        {
            particleTimer = 0.1f;
            m_currentParticles -= 1;
        }
            
    }

    public string texten
    {
        get
        {
            return m_textValue;
        }
        set
        {
            m_textValue = value;
        }
    }

    public bool isNegative
    {
        get
        {
            return negative;
        }
        set
        {
            negative = value;
        }
    }

    public List<Pickeditem> hasItem
    {
        get
        {
        return m_HasItem;
        }
        set
        {
            m_HasItem = value;
        }
    }

    public float setValue
    {
        set {
              itemValue = value;
              if (itemValue < 0)
              {
                  negative = true;
                  itemValue *= -1;
              }
              else
                  negative = false;
        }
        get
        {
            return (negative) ? -itemValue : itemValue;
        }
    }

    public void setPartValue(float top, float bottom)
    {
        part1Value = top;
        part2Value = bottom;
    }

    public float getNormalValue
    {
        get
        {
            return itemValue;
        }
        set
        {
            itemValue = value;
        }
    }

    public void RemakeItem(ref ItemProperties _prop)
    {
        if (!TextValue)
            TextValue = (GameObject)GameObject.Instantiate(GlobalItems.g_listOfObj[4], transform.position, Quaternion.identity);
        itemValue = _prop.itemValue;
        gameObject.layer = LayerMask.NameToLayer("Picked");
        _prop.TextValue = TextValue;
    }

    void OnCollisionEnter(Collision coll)
    {
        
        if (coll.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            if (this.transform.localScale.x < coll.transform.localScale.x)
            {
                SpawnHitParticles(coll);
            }
        }
        else
            SpawnHitParticles(coll);

        if (coll.gameObject.layer == LayerMask.NameToLayer("Picked"))
        {
            SpawnHitParticles(coll);
            hitMerge = coll.collider;
        }
    }

    private void SpawnHitParticles(Collision coll)
    {
        foreach (ContactPoint p in coll.contacts)
        {
            Vector3 pos = p.point;
            for (int i = 0; i < (int)coll.relativeVelocity.magnitude/6; i++)
            {
                if (m_currentParticles < m_maxParticles)
                {
                    GameObject temp = (GameObject)Instantiate(hitParticle, pos, Quaternion.identity);
                    temp.particleSystem.startSize = (this.transform.localScale.x);
                    temp.transform.LookAt(this.transform);
                }
            }
        }
    }
    
    public void SpawnHitParticles()
    {
        for (int i = 0; i < 5; i++)
        {
            if (m_currentParticles < m_maxParticles)
            {
                GameObject temp = (GameObject)Instantiate(hitParticle, transform.position, Quaternion.identity);
                temp.particleSystem.startSize = (this.transform.localScale.x);
                temp.transform.LookAt(GameObject.Find("Main Camera").transform);
            }
        }

    }
    
    public float itemOldValue
    {
        get
        {
            return oldValue;
        }
        set
        {
            oldValue = value;
        }
    }

    public float p1
    {
        get
        {
            return part1Value;
        }
    }
   
    public float p2
    {
        get
        {
            return part2Value;
        }
    }

    
    public bool doResize
    {
        set
        {
            reSizeTimer = Time.time;
            reSize = value;
        }
    }

    public int decimalCount()
    {
        int temp = 0;
        if ((itemValue - (int)itemValue != 0))
        {
            string checki = itemValue.ToString();
            for (int pos = 0; pos < checki.Length; pos++)
            {
                if (checki[pos] == '.' || temp > 0)
                    temp++;
            }
        }
        return temp;
    }
}
