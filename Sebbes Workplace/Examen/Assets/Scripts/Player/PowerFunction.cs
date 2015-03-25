using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerFunction
{

    private List<List<Pickeditem>> m_items = new List<List<Pickeditem>>();
    private Pickeditem RestProdukt;

    private bool ready = false;
    private bool checkResult = false;
    private bool stop = false;
    private bool updateResult = true;

    private int countSize;
    private int PowerStepCounter = 0;

    private Powers Functions = Powers.None;

    public PowerFunction()
    { }

    public void InitiateBasicValues(ref List<Pickeditem> _items)
    {
        if (m_items.Count == 0)
        {
            countSize = 0;
            isReady = false;
            stop = false;
            checkResult = false;
            int TopdeciCount = _items[0].m_itemInfo.decimalCount();
            int BottomdeciCount = _items[1].m_itemInfo.decimalCount();

            for (int i = 0; i < _items.Count; i++)
            {
                List<Pickeditem> addList = new List<Pickeditem>();
                string value = _items[i].m_itemInfo.setValue.ToString();
                if (i == 0 && TopdeciCount < BottomdeciCount) // fixar decimaler
                {
                    for (int k = 1; k < BottomdeciCount - TopdeciCount; k++)
                        addList.Add(new Pickeditem("0", _items[i].itemPosition, i, (int)_items[i].m_itemInfo.TypeID));
                    if (TopdeciCount == 0)
                        addList.Add(new Pickeditem(".", _items[i].itemPosition, i, (int)_items[i].m_itemInfo.TypeID));
                }
                else if (i == 1 && BottomdeciCount < TopdeciCount) // fixar decimaler
                {
                    for (int k = 1; k < TopdeciCount - BottomdeciCount; k++)
                        addList.Add(new Pickeditem("0", _items[i].itemPosition, i, (int)_items[i].m_itemInfo.TypeID));
                    if (BottomdeciCount == 0)
                        addList.Add(new Pickeditem(".", _items[i].itemPosition, i, (int)_items[i].m_itemInfo.TypeID));
                }

                for (int pos = value.Length - 1; pos >= 0; pos--) // lägger till värderna
                {
                    if (value[pos] != '-')
                        addList.Add(new Pickeditem(value[pos].ToString(), _items[i].itemPosition, i, (int)_items[i].m_itemInfo.TypeID));
                }

                if (_items[i].m_itemInfo.isNegative)
                    foreach (Pickeditem p in addList)
                        p.negativeItem = true;


                if (addList.Count > countSize)
                    countSize = addList.Count;

                m_items.Add(addList);
                _items[i].m_obj.SetActive(false);
                _items[i].m_itemInfo.TextValue.SetActive(false);
            }
            //Result list
            m_items.Add(new List<Pickeditem>());
        }
    }

    public void InitiateDiviValues(ref List<Pickeditem> _items)
    {
        if (m_items.Count == 0)
        {
            countSize = 0;
            isReady = false;
            stop = false;
            checkResult = false;
            int width = 0;

            for (int i = 0; i < _items.Count; i++)
            {
                List<Pickeditem> addList = new List<Pickeditem>();

                addList.Add(new Pickeditem(_items[i].m_itemInfo.itemValue.ToString(), _items[i].itemPosition, i, (int)_items[i].m_itemInfo.TypeID));

                if (width < _items[i].m_itemInfo.setValue.ToString().Length)
                    width = _items[i].m_itemInfo.setValue.ToString().Length;


                _items[i].m_obj.SetActive(false);
                _items[i].m_itemInfo.TextValue.SetActive(false);

                addList[0].m_obj.SetActive(true);
                addList[0].m_itemInfo.TextValue.SetActive(true);
                addList[0].negativeItem = _items[i].negativeItem;
                addList[0].setMoveTo = Positions.Div;
                m_items.Add(addList);
            }
            float temp = 0;
            temp = _items[0].m_itemInfo.getNormalValue / _items[1].m_itemInfo.getNormalValue;
            if (temp - (int)temp != 0)
            {
                countSize++;
            }
            countSize += (int)temp;

            //Result list
            m_items.Add(new List<Pickeditem>());
        }
    }

    public bool Update()
    {
        bool check = true;
        updateResult = true;
        for (int i = 0; i < m_items.Count; i++)
            for (int k = 0; k < m_items[i].Count; k++)
            {
                if (Functions == Powers.Add || Functions == Powers.Substract)
                {
                    if (i != 2)
                        check = m_items[i][k].Update(k + m_items[2].Count, i, countSize);
                    else
                        check = m_items[i][k].Update(k, i, countSize);
                }
                else if (Functions == Powers.Division)
                {
                    if (i != 2)
                        check = m_items[i][k].Update(k, i, 1);
                    else
                        check = m_items[i][k].Update(1, i, 1);
                }
                else if (Functions == Powers.Multiplication)
                {

                }

                if (updateResult)
                    updateResult = check;

                if (check && m_items[i][k].Remove)
                {
                    m_items[i][k].Delete();
                    m_items[i].RemoveAt(k);
                }

            }
        if (RestProdukt != null)
        {
            if(RestProdukt.setMoveTo != Positions.Swap)
                updateResult = RestProdukt.Update(0, 0, 0);
            else
                updateResult = RestProdukt.Update(RestProdukt.target.pos + m_items[2].Count, 0.5f, countSize);
        }
        return updateResult;
    }

    public bool SortBasicNumbers() // fixa talens värden innan den mergar eller backar
    {
        if (RestProdukt == null)
        {
            for (int i = 0; i < m_items.Count; i++)
                for (int k = 0; k < m_items[i].Count; k++)
                {

                    if (m_items[i].Count - 1 == k) // om den sista värdeRegler
                    {
                        if (m_items[i][k].m_itemInfo.setValue == 0 && m_items[i][k].m_itemInfo.texten != ".") // nollans regler
                        {
                            if (m_items[i].Count > 1) // om det finns flera värden
                            {
                                if (m_items[i][k - 1].m_itemInfo.texten != ".")//om värdet innan visar sig inte vara ett comma
                                {
                                    m_items[i][k].Delete();
                                    m_items[i].RemoveAt(k);
                                    countSize--;
                                    return false;
                                }
                            }
                        }
                    }

                    if (i == 0) // översta talen
                    {
                        #region Negative
                        if (m_items[i][k].negativeItem)
                        {
                            if (m_items[0].Count - 1 > k) // om det inte är det sista talet
                            {
                                Debug.Log(m_items[i][k].m_itemInfo.setValue);

                                if (m_items[i][k].m_itemInfo.setValue < -9)
                                {
                                    if (m_items[i][k + 1].m_itemInfo.texten == ".")
                                    {
                                        if (m_items[i][k + 2].negativeItem)
                                        {
                                            Debug.Log("0");
                                            CreateRestProdukt("1", m_items[i][k], m_items[i][k + 2]);
                                            m_items[i][k].m_itemInfo.setValue += 10;

                                            return false;
                                        }
                                        else
                                        {
                                            Debug.Log("0");
                                            CreateRestProdukt("10", m_items[i][k], m_items[i][k + 2]);
                                            m_items[i][k + 2].m_itemInfo.setValue -= 1;

                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        if (m_items[i][k + 1].negativeItem)
                                        {
                                            Debug.Log("0");
                                            CreateRestProdukt("1", m_items[i][k], m_items[i][k + 1]);
                                            m_items[i][k].m_itemInfo.setValue += 10;

                                            return false;
                                        }
                                        else
                                        {
                                            Debug.Log("0");
                                            CreateRestProdukt("10", m_items[i][k], m_items[i][k + 1]);
                                            m_items[i][k + 1].m_itemInfo.setValue -= 1;

                                            return false;
                                        }
                                    }
                                }
                                else if(!m_items[i][k+1].m_itemInfo.isNegative)
                                {

                                    if (m_items[i][k + 1].m_itemInfo.texten == "." && !m_items[i][k + 2].negativeItem)
                                    {
                                        Debug.Log("0");
                                        CreateRestProdukt("1", m_items[i][k+2], m_items[i][k]);
                                        m_items[i][k+2].m_itemInfo.setValue -= 10;

                                        return false;
                                    }
                                    else
                                    {
                                        Debug.Log("0");
                                        CreateRestProdukt("10", m_items[i][k+1], m_items[i][k]);
                                        m_items[i][k+1].m_itemInfo.setValue -= 1;

                                        return false;
                                    }
                                }
                            }
                            else if (m_items[i][k].m_itemInfo.setValue < -9)
                            {
                                Debug.Log("0");
                                m_items[i][k].m_itemInfo.setValue += 10;
                                Pickeditem temp = new Pickeditem("1", m_items[i][k].m_obj.transform.position, 0, (int)m_items[i][k].m_itemInfo.TypeID);
                                temp.negativeItem = m_items[i][k].negativeItem;
                                countSize++;

                                temp.setMoveTo = Positions.AddSub;
                                m_items[0].Add(temp);
                                Debug.Log("3");
                                return false;
                            }

                        }
                        #endregion
                        else
                        #region Positive
                        {
                            if (m_items[i].Count - 1 > k) // om det inte är det sista talet
                            {
                                if (m_items[i][k].m_itemInfo.setValue > 9)
                                {
                                    Debug.Log("te");
                                    if (m_items[i][k + 1].m_itemInfo.texten == ".")
                                    {
                                        if (!m_items[i][k + 2].negativeItem)
                                        {
                                            Debug.Log("0");
                                            CreateRestProdukt("1", m_items[i][k], m_items[i][k + 2]);
                                            m_items[i][k].m_itemInfo.setValue -= 10;

                                            return false;
                                        }
                                        else
                                        {
                                            Debug.Log("0");
                                            CreateRestProdukt("10", m_items[i][k + 2], m_items[i][k]);
                                            m_items[i][k + 2].m_itemInfo.setValue += 1;

                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        if (!m_items[0][k].negativeItem)
                                        {
                                            Debug.Log("0");
                                            CreateRestProdukt("1", m_items[i][k], m_items[i][k + 1]);
                                            m_items[i][k].m_itemInfo.setValue -= 10;

                                            return false;
                                        }
                                        else
                                        {
                                            Debug.Log("0");
                                            CreateRestProdukt("10", m_items[i][k + 1], m_items[i][k]);
                                            m_items[i][k + 1].m_itemInfo.setValue += 1;

                                            return false;
                                        }
                                    }
                                }
                                else if (m_items[i][k + 1].m_itemInfo.isNegative)
                                {

                                    if (m_items[i][k + 1].m_itemInfo.texten == "." && !m_items[i][k + 2].negativeItem)
                                    {
                                        Debug.Log("0");
                                        CreateRestProdukt("1", m_items[i][k + 2], m_items[i][k]);
                                        m_items[i][k + 2].m_itemInfo.setValue += 10;

                                        return false;
                                    }
                                    else
                                    {
                                        Debug.Log("0");
                                        CreateRestProdukt("10", m_items[i][k + 1], m_items[i][k]);
                                        m_items[i][k + 1].m_itemInfo.setValue += 1;

                                        return false;
                                    }
                                }
                            }
                            else if (m_items[i][k].m_itemInfo.setValue > 9)
                            {
                                Debug.Log("0");
                                m_items[i][k].m_itemInfo.setValue -= 10;
                                Pickeditem temp = new Pickeditem("1", m_items[i][k].m_obj.transform.position, 0, (int)m_items[i][k].m_itemInfo.TypeID);
                                temp.negativeItem = m_items[i][k].negativeItem;
                                countSize++;

                                temp.setMoveTo = Positions.AddSub;
                                m_items[0].Add(temp);
                                Debug.Log("3");
                                return false;
                            }
                        }
                        #endregion
                    }

                    else if (i == 2) // resultatet
                    {
                        #region Negative
                        if (m_items[i][k].negativeItem) //Om detta värdet är negativt == röd i detta fallet
                        {
                            if (m_items[0].Count > 0) // om det finns items i det övre fältet
                            {
                                if (m_items[0][0].m_itemInfo.texten == ".")
                                {
                                    if (m_items[0][1].negativeItem)
                                    {
                                        if (m_items[i][k].m_itemInfo.setValue < -9)
                                        {
                                            Debug.Log("0");
                                            CreateRestProdukt("1", m_items[i][k], m_items[0][1]);
                                            m_items[i][k].m_itemInfo.setValue += 10;
                                            return false;
                                        }
                                    }
                                    else if (m_items[0][1].m_itemInfo.setValue != 0)
                                    {

                                        CreateRestProdukt("10", m_items[0][1], m_items[i][k]);
                                        Debug.Log(m_items[0][1].m_itemInfo.setValue);
                                        m_items[0][1].m_itemInfo.setValue -= 1;
                                        Debug.Log(m_items[0][1].m_itemInfo.setValue);
                                        return false;
                                    }
                                }
                                else
                                {
                                    if (m_items[0][0].negativeItem)
                                    {
                                        if (m_items[i][k].m_itemInfo.setValue < -9)
                                        {
                                            Debug.Log("0");
                                            CreateRestProdukt("1", m_items[0][0], m_items[i][k]);
                                            m_items[0][0].m_itemInfo.setValue += 10;

                                            return false;
                                        }
                                    }
                                    else if (m_items[0][0].m_itemInfo.setValue != 0)
                                    {
                                        Debug.Log("0");
                                        Debug.Log(m_items[0][0].m_itemInfo.setValue);
                                        m_items[0][0].m_itemInfo.setValue -= 1;
                                        Debug.Log(m_items[0][0].m_itemInfo.setValue);
                                        CreateRestProdukt("10", m_items[0][0], m_items[i][k]);
                                        return false;
                                    }
                                }
                            }
                            else if (m_items[i][k].m_itemInfo.setValue < -9)
                            {
                                Debug.Log("0");
                                m_items[i][k].m_itemInfo.setValue -= 10;
                                Pickeditem temp = new Pickeditem("1", m_items[i][k].m_obj.transform.position, 0, (int)m_items[i][k].m_itemInfo.TypeID);
                                countSize++;

                                temp.setMoveTo = Positions.AddSub;
                                temp.negativeItem = m_items[i][k].negativeItem;
                                m_items[0].Add(temp);
                                Debug.Log("3");
                                return false;
                            }
                        }
                        #endregion
                        else
                        #region Positive
                        {
                            if (m_items[0].Count > 0)
                            {
                                if (m_items[0][0].m_itemInfo.texten == ".")
                                {
                                    if (!m_items[0][1].negativeItem)
                                    {
                                        if (m_items[i][k].m_itemInfo.setValue > 9)
                                        {
                                            Debug.Log("0");
                                            CreateRestProdukt("1", m_items[i][k], m_items[0][1]);
                                            m_items[i][k].m_itemInfo.setValue -= 10;
                                            return false;
                                        }
                                    }
                                    else if (m_items[i][k].m_itemInfo.setValue != 0)
                                    {
                                        CreateRestProdukt("10", m_items[0][1], m_items[i][k]);
                                        m_items[0][1].m_itemInfo.setValue += 1;
                                        return false;
                                    }
                                }
                                else
                                {
                                    if (!m_items[0][0].negativeItem)
                                    {
                                        if (m_items[i][k].m_itemInfo.setValue > 9)
                                        {
                                            Debug.Log("0");
                                            m_items[i][k].m_itemInfo.setValue -= 10;
                                            CreateRestProdukt("1", m_items[i][k], m_items[0][0]);
                                            return false;
                                        }
                                    }
                                    else if (m_items[i][k].m_itemInfo.setValue != 0)
                                    {
                                        Debug.Log("0");
                                        m_items[0][0].m_itemInfo.setValue -= 1;
                                        CreateRestProdukt("10", m_items[0][0], m_items[i][k]);
                                        return false;
                                    }
                                }
                            }

                            else if (m_items[i][k].m_itemInfo.setValue > 9) // lägg till en ny item
                            {
                                m_items[i][k].m_itemInfo.setValue -= 10;
                                Pickeditem temp = new Pickeditem("1", m_items[i][k].m_obj.transform.position, 0, (int)m_items[i][k].m_itemInfo.TypeID);
                                countSize++;

                                temp.setMoveTo = Positions.AddSub;
                                m_items[0].Add(temp);
                                Debug.Log("3");
                                return false;
                            }


                        }
                        #endregion

                        if (checkResult)
                        {
                            ready = ResultBasicController();
                            return ready;
                        }

                    }
                    //Radera onödiga siffrer/märken
                }


            return true;
        }
        else if (!RestProdukt.isMoving)
        {
            if (RestProdukt.setMoveTo == Positions.Target)
            {

                if (RestProdukt.setTarget.negativeItem == RestProdukt.negativeItem)
                {
                    RestProdukt.setTarget.m_itemInfo.setValue += RestProdukt.m_itemInfo.setValue;
                }
                else
                {

                    RestProdukt.setTarget.m_itemInfo.setValue += RestProdukt.m_itemInfo.setValue;
                    RestProdukt.setTarget.negativeItem = RestProdukt.negativeItem;
                }
                RestProdukt.Delete();
                RestProdukt = null;
            }
            return false;
        }
        else
            return false;
    }

    private void CreateRestProdukt(string _value, Pickeditem _item, Pickeditem _target)
    {
        RestProdukt = miniCopy(_value, _item);
        RestProdukt.setTarget = _target;
        RestProdukt.negativeItem = _item.negativeItem;
        RestProdukt.setMoveTo = Positions.Swap;
    }

    private Pickeditem miniCopy(string _value, Pickeditem _item)
    {
        return new Pickeditem(_value, _item.m_obj.transform.position, 0, (int)_item.m_itemInfo.TypeID);
    }

    public bool SortDivideNumbers()
    {
        // Enklaste versionen av divisionen
        if (checkResult)
        {
            ready = checkResult;
            return ready;
        }
        if (RestProdukt != null)
        {
            if (!RestProdukt.isMoving)
            {
                if (RestProdukt.setMoveTo == Positions.Target)
                {
                    if (m_items[0].Count > 0)
                        if (RestProdukt.setTarget == m_items[0][0])
                        {
                            m_items[0][0].m_itemInfo.getNormalValue += RestProdukt.m_itemInfo.getNormalValue;
                        }
                    if (m_items[2].Count > 0)
                        if (RestProdukt.setTarget == m_items[2][0])
                        {
                            RestProdukt.setTarget.m_itemInfo.getNormalValue += 1;
                            RestProdukt.setTarget.m_merge.Add(RestProdukt);
                        }

                    RestProdukt.Delete();
                    RestProdukt = null;
                }
                return true;
            }
            else
                return false;
        }
        else
            return true;

    }

    private bool ResultBasicController()
    {
        if (!ready && RestProdukt == null)
        {
            int i = m_items[2].Count - 1;
            int k = i - 1;
            while (k >= 0)
            {
                if (m_items[2][i].m_itemInfo.texten != ".")
                {
                    if (m_items[2][i].negativeItem != m_items[2][k].negativeItem)
                    {
                        if (m_items[2][k].m_itemInfo.texten == ".")
                        {
                            if (m_items[2][i].negativeItem != m_items[2][k - 1].negativeItem && m_items[2][i].m_itemInfo.setValue != 0)
                            {
                                RestProdukt = new Pickeditem("10", m_items[2][i].m_obj.transform.position, 0, (int)m_items[2][i].m_itemInfo.TypeID);
                                RestProdukt.setTarget = m_items[2][k - 1];
                                RestProdukt.setMoveTo = Positions.Swap;
                                RestProdukt.setMoveTimer = 1.25f;
                                if (m_items[2][i].negativeItem)
                                {

                                    m_items[2][i].m_itemInfo.setValue += 1;
                                    RestProdukt.negativeItem = m_items[2][i].negativeItem;

                                    Debug.Log("RR");
                                    return false;
                                }
                                else if (m_items[2][i].m_itemInfo.setValue != 0)
                                {
                                    m_items[2][i].m_itemInfo.setValue -= 1;
                                    RestProdukt.negativeItem = m_items[2][i].negativeItem;

                                    Debug.Log("RR");
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            Debug.Log("RR");
                            if (m_items[2][i].negativeItem != m_items[2][k].negativeItem && m_items[2][i].m_itemInfo.setValue != 0)
                            {
                                RestProdukt = new Pickeditem("10", m_items[2][i].m_obj.transform.position, 0, (int)m_items[2][i].m_itemInfo.TypeID);
                                RestProdukt.setTarget = m_items[2][k];
                                RestProdukt.setMoveTo = Positions.Swap;
                                RestProdukt.setMoveTimer = 1.25f;
                                if (m_items[2][i].negativeItem)
                                {

                                    m_items[2][i].m_itemInfo.setValue += 1;
                                    RestProdukt.negativeItem = m_items[2][i].negativeItem;

                                    Debug.Log("RR");
                                    return false;
                                }
                                else
                                {
                                    m_items[2][i].m_itemInfo.setValue -= 1;
                                    RestProdukt.negativeItem = m_items[2][i].negativeItem;

                                    Debug.Log("RR");
                                    return false;
                                }
                            }
                        }
                    }
                }

                i--;
                k--;
            }

            foreach (Pickeditem p in m_items[2])
            {
                p.StartMS(true, false);
                p.setMoveTo = Positions.Middle;
            }
            return true;
        }
        return false;
    }

    private bool ResultDivideController()
    {
        Debug.Log("Inväntar");
        return false;
    }

    private int Values(int value, int size, int position)
    {
        if (size == position)
            return 0;
        else
            return Values(value, size, --position);
    }

    public void MergeBasic(Powers _pow)
    {
        if (PowerStepCounter > m_items[2].Count)
        {
            if (m_items[0].Count > 0 && m_items[1].Count > 0) // om det finns något både där uppe och nere
            {
                m_items[0][0].MergeBasicPower(m_items[1][0], _pow); // lägg ihop understa talet med det översta sparar sig själv i cuben
                m_items[1].RemoveAt(0); // ta bort den understa blocket

                m_items[2].Add(m_items[0][0]); // lägg det till resultatet
                m_items[0].RemoveAt(0);

            }
            else if (m_items[0].Count > 0)
            {



                m_items[2].Add(m_items[0][0]);
                m_items[0].RemoveAt(0);


            }
            else if (m_items[1].Count > 0)
            {
                switch (_pow)
                {
                    case Powers.Substract:
                        if (!m_items[1][0].negativeItem)
                            m_items[1][0].negativeItem = true;
                        break;
                    case Powers.Add:
                        if (m_items[1][0].negativeItem)
                            m_items[1][0].negativeItem = false;
                        break;
                    default:
                        break;
                }
                Debug.Log(m_items[1][0].negativeItem);

                m_items[2].Add(m_items[1][0]);
                m_items[1].RemoveAt(0);
            }
            else
            {
                PowerStepCounter--;
            }
        }
        else if (m_items[0].Count == 0 && m_items[1].Count == 0)
        {
            checkResult = true;
        }
    }

    public void MergeDivi()
    {
        if (m_items[0].Count > 0 && m_items[1].Count > 0)
        {
            if (m_items[0][0].setMoveTo == Positions.result)
            {
                m_items[0][0].MergeNewDivi(m_items[1][0]);

                m_items[1].RemoveAt(0);
                countSize--;
                checkResult = true;
            }
        }
        else if (m_items[0].Count == 0)
        {
            if (m_items[1].Count > 0)
                m_items[1][0].Remove = true;
            checkResult = true;
        }
    }

    public void RedoDiviCheck()
    {
        if (m_items[2].Count > 0)
            if (m_items[2][0].hasMerged)
                Debug.Log(m_items[2][0].m_merge.Count);
    }

    public void RedoBasicCheck()
    {
        if (PowerStepCounter < m_items[2].Count && 0 <= PowerStepCounter) // när du backar
        {
            if (m_items[2][m_items[2].Count - 1].hasMerged) // om den har mergat
            {
                m_items[1].Insert(0, m_items[2][m_items[2].Count - 1].popMergeObj());

                m_items[1][0].StartMS(true, false);
                m_items[1][0].setMoveTo = Positions.AddSub;
            }
            #region Default
            if (m_items[2][m_items[2].Count - 1].pos == 0)
            {
                m_items[0].Insert(0, m_items[2][m_items[2].Count - 1]);
                m_items[0][0].StartMS(true, false);
                m_items[0][0].setMoveTo = Positions.AddSub;
            }
            else
            {
                m_items[1].Insert(0, m_items[2][m_items[2].Count - 1]);
                m_items[1][0].StartMS(true, false);
                m_items[1][0].setMoveTo = Positions.AddSub;
            }
            #endregion
            m_items[2].RemoveAt(m_items[2].Count - 1);
        }
    }

    public void BasicStop(ref List<Pickeditem> _items)
    {

        if (_items.Count > 0 && !stop)
        {
            for (int i = 0; i < m_items.Count; i++)
                for (int k = 0; k < m_items[i].Count; k++)
                {
                    m_items[i][k].setMoveTo = Positions.Middle;
                    m_items[i][k].StartMS(true, false);
                }
            stop = true;
        }
        else if (_items.Count > 0 && stop)
        {
            if (updateResult)
            {
                if (ready)
                {
                    Debug.Log(_items[1].m_merge.Count);
                    _items[0].MergeBasicPower(_items[1], Functions);
                    Debug.Log(_items[1].m_merge.Count);
                    _items.RemoveAt(1);

                    _items[0].m_obj.SetActive(true);
                    _items[0].m_itemInfo.TextValue.SetActive(true);
                    _items[0].StartMS(true, false);
                    _items[0].setMoveTo = Positions.Picked;
                    _items[0].m_obj.transform.position = m_items[2][0].itemPosition;
                    _items[0].m_itemInfo.TextValue.transform.position = m_items[2][0].itemPosition;
                }
                else
                    foreach (Pickeditem p in _items)
                    {
                        p.m_obj.SetActive(true);
                        p.m_itemInfo.TextValue.SetActive(true);
                        p.StartMS(true, false);
                        p.setMoveTo = Positions.Picked;
                        p.m_obj.transform.position = m_items[0][0].itemPosition;
                    }

                int temp = m_items.Count;
                for (int i = 0; i < temp; i++)
                {

                    int temp_Count = m_items[0].Count;
                    for (int k = 0; k < temp_Count; k++)
                    {
                        m_items[0][0].Delete(); // tar bort innehållet i item
                        m_items[0].RemoveAt(0); // Tar bort item från listan
                    }
                    m_items.RemoveAt(0); // Tar bort listAllokeringen
                }
                if (m_items.Count > 0)
                    m_items.RemoveAt(0); // ta bort resultatListan
            }
        }
        if (RestProdukt != null)
        {
            RestProdukt.Delete();
            RestProdukt = null;
        }
    }

    public void DiviStop(ref List<Pickeditem> _items) // <<<<<<<<<<<<<<<
    {
        if (_items.Count > 0 && !stop)
        {
            for (int i = 0; i < m_items.Count; i++)
                for (int k = 0; k < m_items[i].Count; k++)
                {
                    m_items[i][k].setMoveTo = Positions.Middle;
                    m_items[i][k].StartMS(true, false);
                }
            stop = true;
        }
        else if (_items.Count > 0 && stop)
        {
            if (updateResult)
            {
                if (ready)
                {
                    checkResult = false;
                    if (m_items[0].Count == 0 && m_items[1].Count == 0) // == det har gått jämnt ut
                    {
                        _items[0].MergeBasicPower(_items[1], Functions);
                        Debug.Log("ssa");
                        _items.RemoveAt(1);

                        _items[0].StartMS(true, false);
                        _items[0].setMoveTo = Positions.Picked;
                        _items[0].m_obj.transform.position = m_items[2][0].itemPosition;
                        _items[0].m_itemInfo.TextValue.transform.position = m_items[2][0].itemPosition;

                        _items[0].m_obj.SetActive(true);
                        _items[0].m_itemInfo.TextValue.SetActive(true);
                    }
                    else if (m_items[0].Count == 1 && m_items[1].Count == 0)
                    {
                        _items[0].Delete();
                        m_items[2][0].MergeDivi(_items[1]);

                        _items[1].Delete();
                        _items.RemoveAt(0);
                        _items.RemoveAt(0);


                        if (m_items[2][0].hasMerged)
                            Debug.Log(m_items[2][0].m_merge[0].m_itemInfo);

                        _items.Add(new Pickeditem(m_items[0][0]));
                        _items.Add(new Pickeditem(m_items[2][0]));

                        if (_items[1].hasMerged)
                            Debug.Log(_items[1].m_merge[0].m_itemInfo.setValue);
                        foreach (Pickeditem p in _items)
                        {
                            p.setMoveTo = Positions.Picked;
                        }
                    }
                }
                else
                    foreach (Pickeditem p in _items)
                    {
                        p.m_obj.SetActive(true);
                        p.m_itemInfo.TextValue.SetActive(true);
                        p.StartMS(true, false);
                        p.setMoveTo = Positions.Picked;
                        p.m_obj.transform.position = m_items[0][0].itemPosition;
                    }

                int temp = m_items.Count;
                for (int i = 0; i < temp; i++)
                {
                    int temp_Count = m_items[0].Count;
                    for (int k = 0; k < temp_Count; k++)
                    {
                        m_items[0][0].Delete(); // tar bort innehållet i item
                        m_items[0].RemoveAt(0); // Tar bort item från listan
                    }
                    m_items.RemoveAt(0); // Tar bort listAllokeringen
                }

                if (m_items.Count > 0)
                {
                    if (m_items[0].Count > 0)
                        m_items[0][0].Delete();
                    m_items.RemoveAt(0); // ta bort resultatListan
                }
                if (RestProdukt != null)
                {
                    RestProdukt.Delete();
                    RestProdukt = null;
                }
            }

        }
    }

    public Powers CurrentPower
    {
        get
        {
            return Functions;
        }
        set
        {
            Functions = value;
        }
    }

    public bool HaveItems
    {
        get
        {
            return (m_items.Count != 0);
        }
    }

    public bool isdone
    {
        get
        {
            return updateResult;
        }
    }

    public int PowerStep // Förflyttning
    {
        set
        {
            if (updateResult)
            {
                if (value == 0)
                    PowerStepCounter = value;
                else if ((value > 0 && getItemMaxSize > PowerStepCounter) ||
                            (value < 0 && PowerStepCounter > -1))
                {

                    PowerStepCounter += value;
                    if (value > 0)
                    {
                        if (Functions == Powers.Add || Functions == Powers.Substract)
                        {
                            if (m_items[0].Count > 0)
                            {
                                m_items[0][0].StartMS(true, false);
                                m_items[0][0].setMoveTo = Positions.result;
                            }
                            if (m_items[1].Count > 0)
                            {
                                m_items[1][0].StartMS(true, false);
                                m_items[1][0].setMoveTo = Positions.result;
                            }
                        }
                        else if (Functions == Powers.Division)
                        {
                            DivisionAddSection();
                        }
                    }
                    else
                    {
                        if (Functions == Powers.Add || Functions == Powers.Substract)
                        {
                            if (m_items[0].Count > 0)
                            {
                                m_items[0][0].StartMS(true, false);
                                m_items[0][0].setMoveTo = Positions.AddSub;
                            }
                            if (m_items[1].Count > 0)
                            {
                                m_items[1][0].StartMS(true, false);
                                m_items[1][0].setMoveTo = Positions.AddSub;
                            }
                        }
                        else if (Functions == Powers.Division)
                        {
                            DivisionBackSection();
                        }
                    }
                }
            }
        }

        get
        {
            return PowerStepCounter;
        }
    }

    private void DivisionAddSection()
    {
        if (m_items[0].Count == 0)
        {
            updateResult = true;
        }

        if (m_items[0].Count > 0 && m_items[2].Count == 0)
        {
            if (m_items[0][0].m_itemInfo.getNormalValue - m_items[1][0].m_itemInfo.getNormalValue >= 0)
            {
                decimal fix = (decimal)m_items[0][0].m_itemInfo.getNormalValue;
                fix -= (decimal)m_items[1][0].m_itemInfo.getNormalValue;
                m_items[0][0].m_itemInfo.getNormalValue = (float)fix;
                RestProdukt = new Pickeditem(m_items[1][0].m_itemInfo.getNormalValue.ToString(), m_items[0][0].m_obj.transform.position, 0, (int)m_items[0][0].m_itemInfo.TypeID);

                RestProdukt.setMoveTo = Positions.ToResult;
                RestProdukt.negativeItem = m_items[0][0].negativeItem;
                m_items[2].Add(RestProdukt);

                if (m_items[0][0].m_itemInfo.getNormalValue == 0)
                {
                    m_items[0][0].Remove = true;
                    m_items[1][0].Remove = true;
                }
            }
        }
        else if (m_items[0][0].m_itemInfo.getNormalValue - m_items[1][0].m_itemInfo.getNormalValue >= 0)
        {
            decimal fix = (decimal)m_items[0][0].m_itemInfo.getNormalValue;
            fix -= (decimal)m_items[1][0].m_itemInfo.getNormalValue;
            m_items[0][0].m_itemInfo.getNormalValue = (float)fix;
            RestProdukt = new Pickeditem(m_items[1][0].m_itemInfo.getNormalValue.ToString(), m_items[0][0].m_obj.transform.position, 0, (int)m_items[0][0].m_itemInfo.TypeID);

            RestProdukt.setTarget = m_items[2][0];
            RestProdukt.negativeItem = m_items[0][0].negativeItem;

            if (m_items[0][0].m_itemInfo.getNormalValue == 0)
            {
                m_items[0][0].Delete();
                m_items[0].RemoveAt(0);
            }
        }
        else
        {
            if (m_items[0].Count > 0)
            {
                m_items[0][0].StartMS(true, false);
                m_items[0][0].setMoveTo = Positions.result;
            }
            if (m_items[1].Count > 0)
            {
                m_items[1][0].StartMS(true, false);
                m_items[1][0].setMoveTo = Positions.result;
            }

        }

    }

    private void DivisionBackSection()
    {

        if (m_items[2].Count > 0)
        {
            if (m_items[2][0].hasMerged)
            {
                RestProdukt = m_items[2][0].popDiviObj(m_items[1][0], (int)m_items[0][0].m_itemInfo.TypeID);
                RestProdukt.setTarget = m_items[0][0];
            }
            else
            {
                CreateRestProdukt(m_items[2][0].m_itemInfo.itemValue.ToString(), m_items[2][0], m_items[0][0]);
                RestProdukt.m_itemInfo.getNormalValue = m_items[1][0].m_itemInfo.getNormalValue;

                m_items[2].RemoveAt(0);
            }
        }
    }

    public int getItemMaxSize
    {
        get
        {
            return countSize;
        }
    }

    public bool isReady
    {
        get
        {
            return ready;
        }
        set
        {
            ready = value;
        }
    }
}
