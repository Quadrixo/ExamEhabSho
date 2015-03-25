using UnityEngine;
using System.Collections.Generic;

public class Pickeditem : Movement
{

    public GameObject m_obj;

    public ItemProperties m_itemInfo;

    public List<Pickeditem> m_merge = new List<Pickeditem>();

    private bool m_remove = false;

    private bool oldPosNeg = false;

    private Powers mergeType = Powers.None;

    private Vector3 pickupScale = new Vector3(0.05f, 0.05f, 0.05f);

    public Pickeditem(ref GameObject _obj)
    {
        m_pickup = true;
        _obj.layer = LayerMask.NameToLayer("Picked");
        m_obj = _obj;

        if (_obj.GetComponent<ItemProperties>().hasItem != null)
            m_merge = _obj.GetComponent<ItemProperties>().hasItem;

        if (m_obj.GetComponent<BoxCollider>())
            m_obj.GetComponent<BoxCollider>().isTrigger = true;
        else if (m_obj.GetComponent<SphereCollider>())
            m_obj.GetComponent<SphereCollider>().isTrigger = true;
        else
            m_obj.GetComponent<CapsuleCollider>().isTrigger = true;

        GameObject.DestroyImmediate(_obj.GetComponent<Rigidbody>());

        setMoveTo = Positions.face;
        setScaleTo = Scaling.PickupScale;

        m_itemInfo = _obj.GetComponent<ItemProperties>();
        if (hasMerged)
        {
            mergeType = m_merge[0].mergeType;
            oldValue = m_itemInfo.itemOldValue;
        }
        else
            oldValue = m_itemInfo.itemValue;

        oldPosNeg = m_itemInfo.isNegative;
        if (m_itemInfo.TextValue == null)
        {
            m_itemInfo.TextValue = (GameObject)GameObject.Instantiate(GlobalItems.g_listOfObj[4], _obj.transform.position, Quaternion.identity);
            m_itemInfo.TextValue.transform.parent = m_obj.transform;
            m_itemInfo.TextValue.GetComponent<TextMesh>().transform.localScale = new Vector3(2, 2, 2);
        }

        m_itemInfo.SpawnHitParticles();

    }

    public Pickeditem(Pickeditem _obj)
    {
        m_obj = (GameObject)GameObject.Instantiate(_obj.m_obj);
        m_itemInfo = m_obj.GetComponent<ItemProperties>();


        GameObject.DestroyImmediate(m_obj.GetComponent<Rigidbody>());
        if (m_obj.GetComponent<BoxCollider>())
            m_obj.GetComponent<BoxCollider>().isTrigger = true;
        else if (m_obj.GetComponent<SphereCollider>())
            m_obj.GetComponent<SphereCollider>().isTrigger = true;
        else
        {
            mergeType = Powers.Convert;
            m_itemInfo.setPartValue(_obj.m_itemInfo.p1, _obj.m_itemInfo.p2);
            m_obj.GetComponent<CapsuleCollider>().isTrigger = true;
        }

        m_obj.layer = LayerMask.NameToLayer("Picked");
        m_obj.transform.localScale = Vector3.zero; // Start skala

        setMoveTo = Positions.AddSub;
        setScaleTo = Scaling.PickupScale;


        if (_obj.m_itemInfo.hasItem != null)
        {
            m_merge = _obj.m_itemInfo.hasItem;
            m_itemInfo.setValue = _obj.m_itemInfo.setValue;
            mergeType = Powers.Division;
        }

        m_itemInfo.setValue = _obj.m_itemInfo.setValue;
        oldValue = m_itemInfo.setValue;
        oldPosNeg = m_itemInfo.isNegative;
        StartMS(true, true);
        m_itemInfo.SpawnHitParticles();
    }

    public Pickeditem(string _value, Vector3 _pos, int _itemPos, int _type)
    {
        m_obj = (GameObject)GameObject.Instantiate(GlobalItems.g_listOfObj[_type], _pos, Quaternion.identity);

        m_itemInfo = m_obj.GetComponent<ItemProperties>();
        GameObject.DestroyImmediate(m_obj.GetComponent<Rigidbody>());

        if (m_obj.GetComponent<BoxCollider>())
            m_obj.GetComponent<BoxCollider>().isTrigger = true;
        else if (m_obj.GetComponent<SphereCollider>())
            m_obj.GetComponent<SphereCollider>().isTrigger = true;
        else if (m_obj.GetComponent<CapsuleCollider>())
            m_obj.GetComponent<CapsuleCollider>().isTrigger = true;

        m_obj.layer = LayerMask.NameToLayer("Picked");
        m_obj.transform.localScale = Vector3.zero; // Start skala

        setMoveTo = Positions.AddSub;
        setScaleTo = Scaling.PickupScale;

        p_pickedPos = pos;


        if (m_itemInfo.GetComponent<ItemProperties>().hasItem != null)
            m_merge = m_itemInfo.GetComponent<ItemProperties>().hasItem;

        if (m_itemInfo.TextValue == null)
        {
            m_itemInfo.TextValue = (GameObject)GameObject.Instantiate(GlobalItems.g_listOfObj[4], _pos, Quaternion.identity);
            m_itemInfo.TextValue.transform.parent = m_obj.transform;
            m_itemInfo.TextValue.GetComponent<TextMesh>().transform.localScale = new Vector3(2, 2, 2);

        }

        if (_value != ".")
            m_itemInfo.setValue = float.Parse(_value);
        else
            m_itemInfo.texten = _value;

        oldValue = m_itemInfo.setValue;
        oldPosNeg = m_itemInfo.isNegative;
        StartMS(true, true);
        m_itemInfo.SpawnHitParticles();
    }

    public void MergeBasicPower(Pickeditem _obj, Powers _pow)
    {
        if (_obj.m_itemInfo.texten != ".")
        {
            decimal to = (decimal)m_itemInfo.setValue;
            decimal from = (decimal)_obj.m_itemInfo.setValue;
            switch (_pow)
            {
                case Powers.Add:
                    to += from;
                    _obj.mergeType = Powers.Add;
                    break;
                case Powers.Substract:
                    to -= from;
                    _obj.mergeType = Powers.Substract;
                    break;
                case Powers.Multiplication:
                    to *= from;
                    _obj.mergeType = Powers.Multiplication;
                    break;
                case Powers.Division:
                    to /= from;
                    _obj.mergeType = Powers.Division;
                    break;
                default:
                    break;
            }
            m_itemInfo.setValue = (float)to;
        }
        else
        {
            _obj.mergeType = Powers.None;
            m_itemInfo.texten = _obj.m_itemInfo.texten;
        }
        _obj.mergeType = _pow;
        m_merge.Add(_obj);

        GameObject.DestroyImmediate(_obj.m_obj.GetComponent<ItemProperties>().TextValue);
        GameObject.DestroyImmediate(_obj.m_obj);
    }

    public void MergeNewDivi(Pickeditem _obj)
    {
        decimal temp = (decimal)m_itemInfo.getNormalValue;

        Delete();
        m_obj = (GameObject)GameObject.Instantiate(GlobalItems.g_listOfObj[2]);
        m_obj.layer = LayerMask.NameToLayer("Picked");
        GameObject.Destroy(m_obj.GetComponent<Rigidbody>());
        m_obj.transform.localScale = new Vector3(0.05f, 0.05f, 0.1f);
        if (m_itemInfo == null)
        {
            m_itemInfo = m_obj.GetComponent<ItemProperties>();
        }

        m_itemInfo.setPartValue((float)temp, _obj.m_itemInfo.getNormalValue);
        mergeType = Powers.Convert;

        GameObject.DestroyImmediate(_obj.m_obj.GetComponent<ItemProperties>().TextValue);
        GameObject.DestroyImmediate(_obj.m_obj);
    }

    public void MergeDivi(Pickeditem _obj)
    {
        oldValue = _obj.m_itemInfo.getNormalValue * m_itemInfo.getNormalValue;
        m_merge.Add(_obj);
        m_merge[0].m_itemInfo = new ItemProperties();
        m_merge[0].m_itemInfo.getNormalValue = _obj.m_itemInfo.getNormalValue;

        m_merge[0].mergeType = Powers.Division;

        GameObject.DestroyImmediate(_obj.m_obj.GetComponent<ItemProperties>().TextValue);
        GameObject.DestroyImmediate(_obj.m_obj);
    }

    public Pickeditem popMergeObj()
    {
        Pickeditem itemPop;
        if (m_merge[m_merge.Count - 1].m_itemInfo.texten == ".")
            itemPop = new Pickeditem(m_merge[m_merge.Count - 1].m_itemInfo.texten, itemPosition, m_merge[m_merge.Count - 1].pos, (int)m_merge[m_merge.Count - 1].m_itemInfo.TypeID);
        else
        {
            itemPop = new Pickeditem(m_merge[m_merge.Count - 1].m_itemInfo.setValue.ToString(), itemPosition, m_merge[m_merge.Count - 1].pos, (int)m_merge[m_merge.Count - 1].m_itemInfo.TypeID);
            itemPop.negativeItem = m_merge[m_merge.Count - 1].negativeItem;
            itemPop.mergeType = m_merge[m_merge.Count - 1].mergeType;
            itemPop.m_merge = m_merge[m_merge.Count - 1].m_merge;

            decimal from = (decimal)itemPop.m_itemInfo.setValue;
            decimal to = (decimal)m_itemInfo.setValue;
            switch (itemPop.mergeType)//reverse
            {
                case Powers.Add:
                    to -= from;
                    break;
                case Powers.Substract:
                    to += from;
                    break;
                case Powers.Multiplication:
                    to /= from;
                    break;
                case Powers.Division:
                    to *= from;
                    break;
                default:
                    break;
            }

            m_itemInfo.setValue = (float)to;
            itemPop.mergeType = Powers.None;
        }
        m_merge.RemoveAt(m_merge.Count - 1);

        return itemPop;
    }

    public Pickeditem popDiviObj(Pickeditem _reference,int _type)
    {
        Pickeditem itemPop = new Pickeditem(_reference.m_itemInfo.setValue.ToString(), itemPosition, _reference.pos, _type);
        itemPop.negativeItem = _reference.negativeItem;

        m_itemInfo.setValue -= 1;
        itemPop.mergeType = Powers.None;

        m_merge.RemoveAt(0);

        return itemPop;
    }

    public void Delete()
    {
        if (m_obj)
        {
            GameObject.DestroyImmediate(m_obj.GetComponent<ItemProperties>().TextValue);
            GameObject.DestroyImmediate(m_obj);
            m_itemInfo = null;
        }
    }

    public Pickeditem getObj()
    {
        Pickeditem itemPop;
        if (m_merge.Count > 0 && !m_remove)
        {
            if (m_merge[0].m_itemInfo.texten == ".")
                itemPop = new Pickeditem(m_merge[0].m_itemInfo.texten, itemPosition, m_merge[0].pos, (int)m_merge[0].m_itemInfo.TypeID);
            else
            {
                Debug.Log(m_merge[m_merge.Count - 1].m_itemInfo.itemValue);
                if (m_merge[m_merge.Count - 1].mergeType == Powers.Division)
                {
                    decimal fix = (decimal)oldValue * (decimal)m_merge[m_merge.Count - 1].m_itemInfo.setValue;
                    oldValue = (float)fix;
                    itemPop = new Pickeditem(m_merge[m_merge.Count - 1].m_itemInfo.getNormalValue.ToString(), itemPosition, m_merge[m_merge.Count - 1].pos, (int)m_merge[m_merge.Count - 1].m_itemInfo.TypeID);
                    itemPop.negativeItem = m_merge[m_merge.Count - 1].negativeItem;

                    itemPop.mergeType = Powers.None;

                    m_itemInfo.isNegative = oldPosNeg;
                    m_itemInfo.getNormalValue = oldValue;
                }
                else
                {
                    itemPop = new Pickeditem(m_merge[m_merge.Count - 1].oldValue.ToString(), itemPosition, m_merge[m_merge.Count - 1].pos, (int)m_merge[m_merge.Count - 1].m_itemInfo.TypeID);
                    itemPop.negativeItem = m_merge[m_merge.Count - 1].negativeItem;

                    itemPop.mergeType = Powers.None;

                    m_itemInfo.isNegative = oldPosNeg;
                    m_itemInfo.getNormalValue = oldValue;
                }
            }
            m_merge.RemoveAt(0);
        }
        else
        {

            m_obj.transform.localScale = new Vector3(1, 1, 1);
            m_obj.layer = LayerMask.NameToLayer("Item");

            m_obj.AddComponent<Rigidbody>();

            if (m_obj.GetComponent<BoxCollider>())
                m_obj.GetComponent<BoxCollider>().isTrigger = false;
            else if (m_obj.GetComponent<SphereCollider>())
                m_obj.GetComponent<SphereCollider>().isTrigger = false;
            else if (m_obj.GetComponent<CapsuleCollider>())
            {
                m_obj.transform.localScale = new Vector3(1, 0.5662833f, 1);
                m_obj.GetComponent<CapsuleCollider>().isTrigger = false;
            }
            return this;
        }
        return itemPop;
    }


    public bool Update(int _pos, float _height, int _size)
    {
        if (MovementUpdate(m_obj, _pos, _height, _size))
        {
            if (setMoveTo == Positions.face)
                setMoveTo = Positions.Picked;
            else if (setMoveTo == Positions.TopLeft || setMoveTo == Positions.Left)
                setMoveTo = Positions.AddSub;

            if (setMoveTo == Positions.Swap)
            {
                setMoveTo = Positions.Target;
                StartMS(true, false);
            }
            if (setMoveTo == Positions.ToResult)
            {
                setMoveTo = Positions.resultdivi;
                m_itemInfo.getNormalValue = 1;
            }

            return true;
            // om det ska hända något när dem har slutat röra sig
        }
        else
            return false;
    }

    public float oldValue
    {
        get
        {
            return m_itemInfo.itemOldValue;
        }
        set
        {
            m_itemInfo.itemOldValue = value;
        }
    }

    public bool isDeleted
    {
        get
        {
            return (m_obj == null);
        }
    }

    public bool hasMerged
    {
        get
        {
            return (m_merge.Count > 0) ? true : false;
        }
    }

    public Vector3 itemPosition
    {
        set
        {
            m_obj.transform.position = Camera.main.ViewportToWorldPoint(value);
        }
        get
        {
            return m_obj.transform.position;
        }

    }

    public bool negativeItem
    {
        set
        {
            m_itemInfo.isNegative = value;
        }
        get
        {
            return m_itemInfo.isNegative;
        }
    }

    public bool Remove
    {
        get
        {
            return m_remove;
        }

        set
        {
            setScaleTo = Scaling.Minimize;
            StartMS(false, true);
            m_remove = value;
        }
    }

    public Powers itemPower
    {
        get
        {
            return mergeType;
        }
        set
        {
            mergeType = value;
        }
    }


    public string[] getInfoForTag()
    {
        string[] temp = new string[1 + m_merge.Count];

        if (m_merge.Count > 0)
        {
            
            switch (m_merge[m_merge.Count - 1].mergeType)//reverse
            {
                case Powers.Add:
                    temp[0] = m_itemInfo.setValue.ToString();
                    temp[0] += " = " + (m_itemInfo.setValue - m_merge[m_merge.Count - 1].m_itemInfo.setValue).ToString();;
                    temp[0] += " + " + m_merge[m_merge.Count - 1].m_itemInfo.setValue.ToString();
                    break;

                case Powers.Substract:
                    temp[0] = m_itemInfo.setValue.ToString();
                    temp[0] += " = " + (m_itemInfo.setValue + m_merge[m_merge.Count - 1].m_itemInfo.setValue).ToString();;
                    temp[0] += " - " + m_merge[m_merge.Count - 1].m_itemInfo.setValue.ToString();
                    break;

                case Powers.Multiplication:
                    temp[0] = m_itemInfo.setValue.ToString();
                    temp[0] += " = " + (m_itemInfo.setValue / m_merge[m_merge.Count - 1].m_itemInfo.setValue).ToString();;
                    temp[0] += " * " + m_merge[m_merge.Count - 1].m_itemInfo.setValue.ToString();
                    break;

                case Powers.Division:
                    temp[0] = m_itemInfo.setValue.ToString();
                    temp[0] += " = " + (m_itemInfo.setValue * m_merge[m_merge.Count - 1].m_itemInfo.setValue).ToString();;
                    temp[0] += " / " + m_merge[m_merge.Count - 1].m_itemInfo.setValue.ToString();
                    break;

                default:
                    break;
            }
            if (m_merge.Count > 1)
                temp[0] += "\n (+" + (m_merge.Count-1) + ")   ";

        }
        else
            temp[0] = m_itemInfo.setValue.ToString();

        return temp;


    }

}
