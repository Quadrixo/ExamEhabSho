using UnityEngine;
using System.Collections;

public class HandProperties : MonoBehaviour
{

    public GameObject particleSystem;
    public LayerMask ObjsToHit;
    
    //private Color c1 = Color.yellow;
    //private Color c2 = Color.red;

    public int Distance = 5;
    //private LineRenderer lineRenderer;

    enum pickupMode { Normal, Combine };

    private pickupMode m_currentMode = pickupMode.Normal;

    private Ray m_Aim;

    public float Strength;

    private bool
        m_throwed = false;

    private float
        m_distanceCounter = 2f;

    private Rigidbody m_currentPickupObj;

    private float grabSpeed = 4f;

    private RaycastHit m_hitInfoFromObject;

    private string nameOfItem = "";

    private float m_pickupTimer = 0f;

    private UIScreen screen;

    // initialization
    void Start()
    {
        screen = GameObject.Find("UI").GetComponent<UIScreen>();
        //lineRenderer = gameObject.AddComponent<LineRenderer>();
        //lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        //lineRenderer.SetColors(c1, c2);
        //lineRenderer.SetWidth(0.025F, 0.025F);
        //lineRenderer.SetVertexCount(lengthOfLineRenderer);
        //lineRenderer.material.renderQueue = 2001;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        canHit(Distance);
        if (m_pickupTimer >= 0)
        {
            screen.playWindow.AimImageSwitcher(true, false);
            m_pickupTimer -= Time.deltaTime;
        }
        else
            switch (m_currentMode)
            {
                case pickupMode.Normal:
                    if (PickupMode())
                        MoveMode();
                    break;
                default:
                    break;
            }
    }

    void OnGUI() // Info
    {
        //GUI.Label(new Rect(100, 200, 200, 200), m_Aim.GetPoint(0).normalized.ToString());

        //GUI.Label(new Rect(300, 100, 200, 200), this.transform.worldToLocalMatrix.m00.ToString());
        //GUI.Label(new Rect(200, 100, 200, 200), transform.forward.ToString());

        //GUI.Label(new Rect(300, 100, 200, 200), Camera.main.transform.rotation.ToString());
        //GUI.Label(new Rect(300, 200, 200, 200), nameOfItem);

        GUI.Button(new Rect(Screen.width / 2, Screen.height / 2, 10, 10), "");

        GUI.Label(new Rect(300, 300, 100, 100), GetComponentInParent<PlayerPower>().Freeze.ToString());
    }

    private bool PickupMode()
    {
        
        if (m_throwed && Input.GetMouseButtonUp(0))
        {
            m_pickupTimer = 0.1f;
            m_throwed = false;
            return true;
        }
        else if (!m_throwed)
        {
            
            if (Input.GetMouseButton(0))
            {
                handHit(Distance);

                ItemController();

                return false;
            }
            else if ((Input.GetMouseButtonUp(0) || Input.GetKeyDown(KeyCode.Q))) // släpper objektet och kastar även iväg det om du har fart
            {
                if (m_currentPickupObj)
                {
                    dropItem();
                }
                return false;
            }
            else if (m_currentPickupObj)
            {
                dropItem();
            }
            return true;
        }
        else
            return true;

    }

    private void MoveMode()
    {
        if (Input.GetMouseButtonDown(1) )
        {
            Debug.Log("tetst");

            if(handHit(Distance))
                HitSpark();
            if (m_currentPickupObj)
            {
                Vector3 dir = Camera.main.transform.forward + new Vector3(0, 0.5f, 0);
                Vector3 directionLerp = new Vector3(
                Mathf.Lerp(-2.5f, 2.5f, dir.x),
                Mathf.Lerp(-2.5f, 2.5f, dir.y),//makes the cubes move too
                Mathf.Lerp(-2.5f, 2.5f, dir.z));

                m_currentPickupObj.AddForce(dir * Strength);
            }
            nameOfItem = "";
            m_currentPickupObj = null;
        }
    }

    private void dropItem()
    {
        m_pickupTimer = 0.1f;
        Vector3 pickupPos = handDirection(ItemSizeDistance());
        FreezePlayerMouse(false);
        Vector3 travelDir = (pickupPos - m_currentPickupObj.gameObject.transform.position);
        Vector3 movement = (this.gameObject.GetComponentInParent<CharacterMotor>().movement.velocity * Time.deltaTime) * 2;
        m_currentPickupObj.AddForce(-movement + 150 * travelDir / m_currentPickupObj.mass);
        nameOfItem = "";
        ResetItem();

    }

    private bool ItemController()
    {
        if (m_currentPickupObj)
        {
            //if (!m_currentPickupObj.renderer.isVisible)
            //{
            //    dropItem();
            //    return false;
            //}
            screen.playWindow.AimImageSwitcher(false, true);
            m_currentPickupObj.velocity = Vector3.zero;
            m_currentPickupObj.freezeRotation = true;
            Vector3 pickupPos = handDirection(ItemSizeDistance());

            Vector3 travelDir = (pickupPos - m_currentPickupObj.position) * Time.deltaTime;
            Vector3 movementVelocity = (this.gameObject.GetComponentInParent<CharacterMotor>().movement.velocity * Time.deltaTime);


            // Förhindrar objectet att fastna/försvinna i onödan
            m_currentPickupObj.gameObject.GetComponent<Rigidbody>().AddForce(0, 0.01f, 0);
            m_currentPickupObj.gameObject.transform.position += (travelDir * grabSpeed) + movementVelocity;


            if (Input.GetKey(KeyCode.F)) // Roterar objektet samt låser kameran
            {

                m_currentPickupObj.freezeRotation = false;
                m_currentPickupObj.transform.Rotate(new Vector3(-Input.GetAxis("Mouse X") * 4, -Input.GetAxis("Mouse Y") * 4, 0));
                FreezePlayerMouse(true);
            }
            else if (Input.GetKeyUp(KeyCode.F)) // frigör ovanstående
            {
                FreezePlayerMouse(false);
            }

            if (Input.GetMouseButtonDown(1)) // Skjuter iväg objektet dit du siktar
            {
                m_throwed = true;
                m_currentPickupObj.AddForce(Strength * (Camera.main.transform.forward) * m_currentPickupObj.mass);
                ResetItem();
                return false;
            }

            if (m_distanceCounter < 3f && Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                m_distanceCounter += Input.GetAxis("Mouse ScrollWheel") * 2;
            }
            else if (m_distanceCounter > 1.5f && Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                m_distanceCounter += Input.GetAxis("Mouse ScrollWheel") * 2;
            }

            // Rita ut resultatet

            //GrabRay();

            return true;
        }
        return false;
    }

    void FreezePlayerMouse(bool value)
    {
        GetComponentInParent<PlayerPower>().Freeze = value;
    }

    private void ResetItem()
    {
        //lineRenderer.enabled = false;
        Camera.main.GetComponent<MouseLook>().mouseLook = true;
        m_currentPickupObj.freezeRotation = false;
        m_currentPickupObj.useGravity = true;
        m_currentPickupObj = null;
        m_distanceCounter = 2f;
    }

    private float ItemSizeDistance()
    {
        return m_distanceCounter +  // 1f som standard där senare storleken avgör distancen
            (m_currentPickupObj.GetComponent<Collider>().transform.localScale.x +
            m_currentPickupObj.GetComponent<Collider>().transform.localScale.y +
            m_currentPickupObj.GetComponent<Collider>().transform.localScale.z) / 3;
    }

    private bool isMoving()
    {
        Vector3 velocity = GetComponentInParent<CharacterMotor>().movement.velocity;
        if (velocity.x != 0 || velocity.z != 0)
            return true;
        return false;
    }

    //private void ChangeRayColor(int _type)
    //{

    //    switch (_type)
    //    {
    //        case (int)ColorMode.Pickup:
    //            c1 = Color.yellow;
    //            c2 = Color.red;
    //            break;
    //        case (int)ColorMode.Push:
    //            c1 = Color.blue;
    //            c2 = Color.cyan;
    //            break;
    //        default:
    //            c1 = Color.yellow;
    //            c2 = Color.red;
    //            break;
    //    }

    //}

    //private void GrabRay()
    //{
    //    Vector3 itemdistance = WeponDirection(ItemSizeDistance());
    //    Vector3 beginRay = WeponDirection(1f + m_currentPickupObj.transform.localScale.x);
    //    Vector3 startPos = this.transform.position;

    //    Vector3 objDirection = (startPos - m_currentPickupObj.position) / lengthOfLineRenderer;

    //    Vector3 rayBend = (startPos - beginRay) / lengthOfLineRenderer;

    //    lineRenderer = GetComponent<LineRenderer>();
    //    lineRenderer.SetColors(c1, c2);

    //    float step = 1f / lengthOfLineRenderer;

    //    for (int i = 0; i < lengthOfLineRenderer; i++) //Ritar ut linjen fint
    //    {
    //        Vector3 dir = (rayBend * (1f - (step * i))) + (objDirection * (step * i));

    //        Vector3 pos = startPos - (dir * i)
    //            + new Vector3(0, 0, Mathf.Sin(i + (Time.time * 15)) / 20);
    //        lineRenderer.SetPosition(i, pos);
    //    }

    //    particleTimer -= Time.deltaTime;
    //    if (particleTimer <= 0)
    //    {
    //        particleTimer = 0.05f;

    //        Vector3 particlePos = (m_hitInfoFromObject.rigidbody) ?
    //            m_hitInfoFromObject.point :
    //            startPos - ((objDirection * (step * lengthOfLineRenderer / 1.2f)) * lengthOfLineRenderer / 1.2f);

    //        GameObject temp = (GameObject)Instantiate(particleSystem, particlePos, Quaternion.identity);
    //        temp.particleSystem.startSize = Mathf.Lerp(0.05f, 1.5f, m_currentPickupObj.transform.localScale.x / 80);
    //        temp.transform.LookAt(this.transform);

    //    }
    //}

    //private void ResetPower()
    //{
    //    //cube.SetActive(false);
    //    //sphere.SetActive(false);
    //    //m_gun.SetActive(false);
    //}

    //private void MissRay()
    //{
    //    Vector3 miss = WeponDirection(lengthOfLineRenderer);
    //    Vector3 startPos = this.transform.position;
    //    Vector3 direction = new Vector3(
    //          startPos.x - miss.x,
    //          startPos.y - miss.y,
    //          startPos.z - miss.z) / lengthOfLineRenderer;

    //    lineRenderer.SetColors(c1, c2);
    //    for (int i = 0; i < lengthOfLineRenderer; i++) //Ritar ut linjen fint
    //    {
    //        Vector3 pos = startPos - (direction * i) + new Vector3(0, 0, Mathf.Sin(i + (Time.time * 15)) / 20);
    //        //new Vector3(i * 0.5F, Mathf.Sin(i + Time.time), 0);
    //        lineRenderer.SetPosition(i, pos);

    //    }
    //}

    private void HitSpark()
    {
        int i = 0;
        while (i < 5)
        {
            GameObject temp = (GameObject)Instantiate(particleSystem, m_hitInfoFromObject.point, Quaternion.identity);
            temp.GetComponent<ParticleSystem>().startSize = Mathf.Lerp(0.05f, 1.5f, m_hitInfoFromObject.transform.localScale.x / 80);
            temp.transform.LookAt(GameObject.Find("Player").transform);
            i++;
        }
    }

    public Vector3 handDirection(float _distance)
    {
        return Camera.main.transform.position + _distance * Camera.main.transform.forward;
    }

    private bool handHit(float distance)
    {
        Vector3 testDistance = Camera.main.transform.position + distance * Camera.main.transform.forward;

        if (Physics.Linecast(Camera.main.transform.position, testDistance,
             out m_hitInfoFromObject, ~(1 << LayerMask.NameToLayer("Player"))))
        {
            if (m_hitInfoFromObject.collider.gameObject.layer == LayerMask.NameToLayer("Item"))
            {
                nameOfItem = m_hitInfoFromObject.collider.gameObject.name;
                if (!m_currentPickupObj)
                {
                    m_currentPickupObj = m_hitInfoFromObject.rigidbody;
                    return true;
                }
            }
        }
        return false;
    }

    private void canHit(float distance)
    {
        Vector3 testDistance = Camera.main.transform.position + distance * Camera.main.transform.forward;

        if (Physics.Linecast(Camera.main.transform.position, testDistance,
             out m_hitInfoFromObject, ~(1 << LayerMask.NameToLayer("Player"))))
        {
            if (m_hitInfoFromObject.collider.gameObject.layer == LayerMask.NameToLayer("Item"))
            {
                if (!m_currentPickupObj)
                    screen.playWindow.AimImageSwitcher(true, false);
            }
            else
            {
                screen.playWindow.AimImageSwitcher(false, false);
            }
        }
        else
        {
            screen.playWindow.AimImageSwitcher(false, false);
        }
    }

        public bool objektLifted
    {
        get
        {
            return m_currentPickupObj;
        }
    }
}
