using UnityEngine;
using System.Collections;

public class Movement {

    protected float m_TravelTime;
    protected float journeyTime = (float)1/60;
    private float Speed = 0.8f;

    protected bool m_moving = false;
    protected bool m_scaling = false;
    protected bool m_mergeOk = true;
    public bool m_pickup;

    protected int p_pickedPos;

    protected string theType;

    protected Vector3
    pickupPos = new Vector3(0.6f, 0.8f, 0.5f),
    itemPos = Vector3.zero,
    itemCenter = new Vector3(0.7f, 0.8f, 0.5f),
    divCenter = new Vector3(0.5f, 0.8f, 0.5f);


    protected float m_leftSide = 0.1f;
    protected float m_RightSide = 0.2f;

    public Pickeditem target;

    private Positions MoveToValue = Positions.Picked;

    private Scaling ScaleToValue = Scaling.NormalSize;


    public void StartMS(bool _move, bool _scale)
    {
        if (!m_moving && _move)
        {
            m_moving = _move;
            m_TravelTime = 0;
        }

        if (!m_scaling && _scale)
        {
            m_scaling = _scale;
            m_TravelTime = 0;
        }
    }

    public bool MovementUpdate(GameObject _obj,int _pos, float _height,int _size)
    {
        m_TravelTime += journeyTime;

        if (m_moving) // Om den ska röra på sig
        {
            _obj.transform.position = Vector3.Lerp(_obj.transform.position, getMoveTo(_pos, _height, _size), m_TravelTime);

            if (m_TravelTime > Speed)
                m_moving = false;

            if (_obj.transform.position == getMoveTo(_pos, _height, _size))
                m_moving = false;
        }
        else
        {
            _obj.transform.position = getMoveTo(_pos, _height, _size);
        }

        if (m_scaling) // Om den ska ändra storlek
        {
            if (m_TravelTime > Speed)
                m_scaling = false;

            _obj.transform.localScale = Vector3.Lerp(_obj.transform.localScale, getScaleTo(), m_TravelTime);
            if (_obj.transform.localScale == getScaleTo())
                m_scaling = false;
        }
        
        return (!m_scaling && !m_moving);
    }

    private Vector3 AddSubPos(int _pos,int _size, float _height) // Gör default fast tvärtom
    {
        return itemCenter + new Vector3(-0.2f + (m_leftSide * _size / 3) - ((m_RightSide / ((float)_size / 1.5f)) * (_pos + _size)), -m_RightSide * _height, _size * 0.05f);
    }

    private Vector3 DivisionPos(int _pos,int _size, float _height)
    {
        return divCenter + new Vector3(-0.2f * _pos, _height * -0.2f, 0.1f);
        //return itemCenter + new Vector3(0,0, 0.1f);

    }

    private Vector3 DefaultPickup(float _pos, int _size) // Default pickup + placering baserat på antal items
    {
        return pickupPos + new Vector3(-m_leftSide * _size + (m_RightSide * _pos), 0, _size * 0.1f);
    }

    private float constantZPos(int _size)
    {
        return _size * 0.1f;
    }

    private Vector3 getMoveTo(int _pos,float _height, int _size)
    {
        switch (MoveToValue)
        {
            case Positions.AddSub:
                return Camera.main.ViewportToWorldPoint(AddSubPos(_pos - _size, _size, _height));
            case Positions.Bottom:
                return Vector3.zero;
            case Positions.Div:
                return Camera.main.ViewportToWorldPoint(DivisionPos(_pos, _size, _height));
            case Positions.Middle:
                return Camera.main.ViewportToWorldPoint(DefaultPickup(0, 1));
            case Positions.TopLeft:
                return Camera.main.ViewportToWorldPoint(AddSubPos(_pos - _size, _size, _height));
            case Positions.ToResult:
                return Camera.main.ViewportToWorldPoint(DivisionPos(_pos - _size - 1, _size, 0.5f));
            case Positions.face:
                return Camera.main.transform.position + Camera.main.transform.forward + new Vector3(0,0.1f,0);
            case Positions.result:
                return Camera.main.ViewportToWorldPoint(AddSubPos(_pos - _size, _size, 0.5f));
            case Positions.resultdivi:
                return Camera.main.ViewportToWorldPoint(DivisionPos(_pos - _size - 1, _size, 0.5f));
            case Positions.Picked:
                return Camera.main.ViewportToWorldPoint(DefaultPickup(_pos, _size));
            case Positions.Target:
                return target.m_obj.transform.position;
            case Positions.Swap:
                return Camera.main.ViewportToWorldPoint(AddSubPos(_pos - _size, _size, _height) + new Vector3(0, 0.2f,0));
            default:
                return Vector3.zero;
        }
    }

    private Vector3 getScaleTo()
    {
        switch (ScaleToValue)
        {
            case Scaling.NormalSize:
                return new Vector3(1, 1, 1);
            case Scaling.Minimize:
                return Vector3.zero;
            case Scaling.Merging:
                return Vector3.zero;
            case Scaling.PickupScale:
                return new Vector3(0.05f, 0.05f, 0.05f);
            default:
                return new Vector3(1, 1, 1);
        }

    }

    public int pos
    {
        get
        {
            return p_pickedPos;
        }
    }

    public Pickeditem setTarget
    {
        get
        {
            return target;
        }
        set
        {
            MoveToValue = Positions.Target;
            StartMS(true, false);
            target = value;
        }
    }

    public Positions setMoveTo
    {
        set
        {
            
            MoveToValue = value;
        }
        get
        {
            return MoveToValue;
        }
    }

    public Scaling setScaleTo
    {
        set
        {
            ScaleToValue = value;
        }
    }

    public bool isMoving
    {
        get
        {
            return m_moving;
        }

    }

    public bool isScaling
    {
        get
        {
            return m_scaling;
        }
    }

    public float setMoveTimer
    {
        set
        {
            journeyTime = value;
        }
    }
}
