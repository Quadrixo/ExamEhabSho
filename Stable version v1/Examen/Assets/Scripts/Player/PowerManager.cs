using UnityEngine;
using System.Collections.Generic;

public class PowerManager
{


    private List<Pickeditem>
        m_items = new List<Pickeditem>();

    private bool NextStep = false;

    private PowerFunction powerFunction = new PowerFunction();

    private Vector3
    pickupPos = new Vector3(0.6f, 0.8f, 1f),
    itemPos = Vector3.zero,
    itemCenter = new Vector3(0.5f, 0.8f, 1f);


    //1:0.5
    //2:0.4, 0.6
    //3:0.3, 0,5 0,7
    //Om man kommer på en ekvation, använd den då.

    //--Fuskekvation--
    // 0.6-0.1, 0.6-0.2, 0.6-(count*0.1)+(i*0.2f)

    // 0.6-(count*0.1)+(i*0.2f)
    // count = 1, i = 0, sum = 0.5
    // count = 2, i = 0, sum = 0.4
    // count = 2, i = 1, sum = 0.6
    // count = 3, i = 0, sum = 0.3
    // count = 3, i = 1, sum = 0.5
    // count = 3, i = 0, sum = 0.7

    private Powers m_power = Powers.Substract;


    private bool m_usePower = false;

    private bool m_split = false;

    public PowerManager()
    { }

    public void addItem(GameObject _obj)
    {
        if (m_items.Count > 2)
        {
            m_items[0].StartMS(true, false); // ber den första obj att flytta på sig
        }

        if (!(m_items.Count > 1)) // får bara ha två obj
        {
            m_items.Add(new Pickeditem(ref _obj));
            m_items[m_items.Count - 1].StartMS(true, true);
            //Spara och uppdatera positionen i realtime
        }
        m_usePower = false;
    }

    public void popItem()
    {
        m_items[m_items.Count - 1].Remove = true;
        m_items[m_items.Count - 1].setScaleTo = Scaling.Minimize;
    }

    public void ChangePower(int _value)
    {
        if (!m_usePower)
        {
            m_power += _value;

            if (m_power < Powers.None)//0 min
            {
                m_power = Powers.Convert;
            }
            else if (m_power > Powers.Convert)//5 max
            {
                m_power = Powers.None;
            }
        }
    }

    public void UsePower()
    {
        if (!m_usePower && m_items.Count > 1)
        {
            if (UnlockedPower())
            {
                powerFunction.PowerStep = 0;
                powerFunction.isReady = false;
                powerFunction.CurrentPower = m_power;
                m_usePower = true;

                foreach (Pickeditem p in m_items)
                    p.StartMS(true, false);
            }
        }
    }

    private bool UnlockedPower()
    {
        if (m_items[0].itemPower == Powers.Convert || m_items[1].itemPower == Powers.Convert)
            return false;
       
        switch(m_power)
        {
            case Powers.Division:
                if (m_items[0].m_itemInfo.itemValue < m_items[1].m_itemInfo.itemValue)
                    return false;
                break;
            case Powers.Multiplication:
                if (m_items[0].m_itemInfo.itemValue < m_items[1].m_itemInfo.itemValue)
                    return false;
                break;
            default:
                break;
        }
        return true;
    }

    public void PowerStep(int _value)
    {
        if (!NextStep && powerFunction.isdone)
        {
            NextStep = true;
            powerFunction.PowerStep = _value;
        }
    }

    public void UndoItem()
    {
        if (m_items.Count == 1 && m_items[0].hasMerged)
            m_split = true;

    }

    public void Update()
    {
        if (DoPower)
        {
            switch (m_power)
            {
                case Powers.None:
                    break;
                case Powers.Add:
                case Powers.Substract:
                    BasicPower();
                    break;
                case Powers.Multiplication:
                    MultiPower();
                    break;
                case Powers.Division:
                    DiviPower();
                    break;
                case Powers.Convert:
                    break;
                default:
                    break;
            }
            if (powerFunction.PowerStep == -1 || powerFunction.isReady)
            {
                if (m_power == Powers.Add || m_power == Powers.Substract)
                    powerFunction.BasicStop(ref m_items);
                else if (m_power == Powers.Division)
                    powerFunction.DiviStop(ref m_items);

                m_usePower = false;
                powerFunction.isReady = false;
            }

        }
        else //De en/två objekt man har plockat upp
        {
            for (int i = 0; i < m_items.Count; i++)
            {
                if (m_usePower)
                {
                    MoveToMerge(m_items[i]);
                }
                else
                {
                    if (m_split)
                    {
                        m_items.Add(m_items[0].popMergeObj());
                        foreach (Pickeditem p in m_items)
                        {
                            p.StartMS(true, false);
                        }
                        m_items[1].setMoveTo = Positions.Picked;
                        m_split = false;
                        break;
                    }
                    if (RemoveSection(m_items[i]))
                        break;
                    m_usePower = false;
                }
                m_items[i].Update(i, 0.5f, m_items.Count);
            }
        }
    }

    private bool DoPower
    {
        get
        {
            if (m_items.Count > 1)
                return !(m_items[0].m_obj.activeSelf && m_items[1].m_obj.activeSelf);
            else if (m_items.Count == 1)
                return !(m_items[0].m_obj.activeSelf);
            else
                return false;
        }
    }

    private bool RemoveSection(Pickeditem _item)
    {
        if (_item.Remove)//Om Remove har blivit tryckt
        {
            if (!_item.isScaling)
            {
                GameObject obj = _item.getObj().m_obj;

                obj.transform.position = Camera.main.transform.position + Camera.main.transform.forward + new Vector3(0,-0.2f,0);

                obj.rigidbody.AddForce( Camera.main.transform.forward);

                if (_item.hasMerged)
                    obj.GetComponent<ItemProperties>().hasItem = _item.m_merge;

                _item.Remove = false;
                m_items.RemoveAt(m_items.Count - 1);

                obj = null;

                if (m_items.Count == 1)
                    m_items[0].StartMS(true, false);

                return true;
            }
        }
        return false;
    }

    private void MoveToMerge(Pickeditem _item)
    {
        if (_item.m_obj.activeSelf)
        {
            _item.setMoveTo = Positions.Middle;
            if (!_item.isMoving)
            {
                _item.m_itemInfo.TextValue.SetActive(false);
                _item.m_obj.SetActive(false);
            }
        }
    }

    private void BasicPower()
    {
        // 1: Kontrollera vad för tal det är,
        // 2: dela upp entalen, tior, hundratalen för sig. // Skapa fler cuber
        // 3: placera talen som ska plussas över det talet den ska plussas på.
        // 4: för ihop talen stegvis och beräkna tsm med eventuella rester
        // 5: Slå ihop talen från vänster till höger eftersom man läser det så. 1->5->4->2 = 1542// etttusenfemhundrafyrtiotvå

        if (!powerFunction.HaveItems)
            powerFunction.InitiateBasicValues(ref m_items);
        else
        {
            if (powerFunction.Update())
            {
                NextStep = false;// hindrar spamm
                if (powerFunction.SortBasicNumbers())
                {
                    powerFunction.MergeBasic(m_power);
                    powerFunction.RedoBasicCheck();
                    
                }
            }
        }
    }

    private void MultiPower()
    {
        if (!powerFunction.HaveItems)
            powerFunction.InitiateDiviValues(ref m_items);
        else
        {
            if (powerFunction.Update())
            {
                NextStep = false;

                if (powerFunction.SortDivideNumbers())
                {
                    powerFunction.MergeDivi();
                }
            }
        }
    }

    private void DiviPower()
    {
        if (!powerFunction.HaveItems)
            powerFunction.InitiateDiviValues(ref m_items);
        else
        {
            if (powerFunction.Update())
            {
                NextStep = false;

                if (powerFunction.SortDivideNumbers())
                {
                    powerFunction.MergeDivi();
                    powerFunction.RedoDiviCheck();
                }
            }
        }
    }

    public bool HaveItems
    {
        get
        {
            return (m_items.Count != 0);
        }
    }

    public int CountList
    {
        get
        {
            return m_items.Count;
        }
    }

    public string PowerName
    {
        get
        {
            return m_power.ToString();
        }
    }

    public bool PowerIsActive
    {
        get
        {
            return m_usePower;
        }
    }

    public string[] PickedValue(int pos)
    {
        return m_items[pos].getInfoForTag();
    }

}
