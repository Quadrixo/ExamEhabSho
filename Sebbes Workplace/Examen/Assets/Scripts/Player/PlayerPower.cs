using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerPower : MonoBehaviour
{

    private Ray m_Aim;

    public float
        rotationSpeed = 4.0f;

    public float
        shootSpeed,
        defRange = 2f;

    private float
        m_distanceCounter = 1f;

    
    private RaycastHit m_hitInfoFromObject;
    private Rigidbody m_currentPickupObj;

    public bool Freeze;
    
    PowerManager m_pManager = new PowerManager();
    UIScreen UI;
    public bool canLift = false;


    void Awake()
    {
        UI = GameObject.Find("UI").GetComponent<UIScreen>();

    }

    void Start()
    {
       //GetComponent<CharacterMotor>().enabled = false;
    }

    void OnGUI() // Info
    {
        if (canLift)
        {
            GUI.Label(new Rect(400, 100, 200, 200), m_pManager.PowerName);
            GUI.Label(new Rect(400, 200, 200, 200), "nr: " + m_pManager.CountList.ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (canLift)
        {
            FreezePlayerMouse(Freeze);

            m_pManager.Update();

            PowerController();
        }
    }
    
    void PowerController()
    {
        #region PowerSwitch
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (m_pManager.CountList > 0)
               UI.playWindow.leftInfo(m_pManager.PickedValue(0));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (m_pManager.CountList > 1)
                UI.playWindow.rightInfo(m_pManager.PickedValue(1));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //ändra positiv/negativ på 1/2
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (!m_pManager.PowerIsActive)
            {
                
                m_pManager.UndoItem();
                ResetTagInfo(false);
            }
            // Ångra första obj och lägg till andra på nästkommande plats
        }

        if(!GetComponent<HandProperties>().objektLifted)
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                m_pManager.ChangePower(1);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                m_pManager.ChangePower(-1);
            }


        #endregion

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (grabHit(6f))
            {
                m_pManager.addItem(m_currentPickupObj.gameObject);
            }
        }
        #region HaveItems
        if (m_pManager.HaveItems)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (m_pManager.PowerIsActive)
                    m_pManager.PowerStep(-1);
                else if (m_pManager.HaveItems)
                {
                    m_pManager.popItem();
                    ResetTagInfo(false);
                }
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                if (!m_pManager.PowerIsActive)
                {
                    m_pManager.UsePower();
                    ResetTagInfo(true);
                }
                else
                    m_pManager.PowerStep(1);

            }
        }


        #endregion
    }

    public void ResetTagInfo(bool all)
    {

        if (!all)
        {
            if (m_pManager.CountList > 1)
                UI.playWindow.rightInfo(null);
            else if (m_pManager.CountList > 0)
                UI.playWindow.leftInfo(null);
        }
        else
        {
            UI.playWindow.rightInfo(null);
            UI.playWindow.leftInfo(null);
        }
    }

    private void createPower()
    {
        //if(sphere.activeSelf)// roterande sphere
        //{
        //    sphere.transform.Rotate(new Vector3(Mathf.PingPong(4, 7), Mathf.PingPong(4, 7), 0)+Camera.main.transform.forward);
        //    if(HitCheck(20f, LayerMask.NameToLayer("WorldPiece")))
        //    {
        //        //temp = (GameObject)Instantiate(sphere, AimDirection(2f), sphere.transform.rotation);
        //    }
        //}

        //if(Input.GetMouseButton(0))
        //{
        //    if (sphere.activeSelf)
        //    {
        //        sphere.transform.Rotate(new Vector3(-Input.GetAxis("Mouse X") * rotationSpeed, -Input.GetAxis("Mouse Y") * rotationSpeed, 0));
        //        FreezePlayerMouse(true);
        //    }
        //    else if (cube.activeSelf)
        //    {
        //        cube.transform.Rotate(new Vector3(-Input.GetAxis("Mouse X") * rotationSpeed, -Input.GetAxis("Mouse Y") * rotationSpeed, 0));
        //        FreezePlayerMouse(true);
        //    }

        //}

        //if (Input.GetMouseButtonDown(1))
        //{
        //    if (sphere.activeSelf)
        //    {
        //        temp = (GameObject)Instantiate(sphere, AimDirection(defRange), sphere.transform.rotation);
        //        GObjectSetup(ref temp);
        //        sphere.SetActive(false);
        //    }
        //    else if (cube.activeSelf)
        //    {
        //        temp = (GameObject)Instantiate(cube, AimDirection(defRange), cube.transform.rotation);
        //        GObjectSetup(ref temp);
        //        cube.SetActive(false);
        //    }
        //}
    }
    //Låser musen
    public void FreezePlayerMouse(bool value)
    {
        if (UI.pauseMeny)
        {
            if (UI.pauseMeny.isPaused)
            {

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                GetComponent<CharacterMotor>().enabled = !value;
                GetComponent<MouseLook>().mouseLook = !value;
                Camera.main.GetComponent<MouseLook>().mouseLook = !value;
                Debug.Log(value);

            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                GetComponent<CharacterMotor>().enabled = !value;
                GetComponent<MouseLook>().mouseLook = !value;
                Camera.main.GetComponent<MouseLook>().mouseLook = !value;
            }
        }
    }

    void GObjectSetup(ref GameObject temp)
    {
        temp.GetComponent<PowerStructure>().InitiatieValues(new Vector3(1, 1, 1), AimDirection(defRange), true);
        temp.GetComponent<Rigidbody>().useGravity = false;
        temp.GetComponent<Rigidbody>().isKinematic = false;
        temp.GetComponent<Rigidbody>().AddForce(shootSpeed * (Camera.main.transform.forward));
    }

    private Vector3 AimDirection(float distance)
    {
        return Camera.main.transform.position + distance * Camera.main.transform.forward;   
    }

    private void ResetPower()
    {
        //cube.SetActive(false);
        //sphere.SetActive(false);
        //m_gun.SetActive(false);
    }

    private float ItemSizeDistance()
    {
        return m_distanceCounter +  // 1f som standard där senare storleken avgör distancen
            (m_currentPickupObj.GetComponent<Collider>().transform.localScale.x +
            m_currentPickupObj.GetComponent<Collider>().transform.localScale.y +
            m_currentPickupObj.GetComponent<Collider>().transform.localScale.z) / 3;
    }

    private float ItemSizeDistance(GameObject obj)
    {
        return m_distanceCounter +  // 1f som standard där senare storleken avgör distancen
    (obj.GetComponent<Collider>().transform.localScale.x +
    obj.GetComponent<Collider>().transform.localScale.y +
    obj.GetComponent<Collider>().transform.localScale.z) / 3;
    }

    public bool grabHit(float distance)
    {
        Vector3 testDistance = Camera.main.transform.position + distance * Camera.main.transform.forward;

        if (Physics.Linecast(Camera.main.transform.position, testDistance,
             out m_hitInfoFromObject, ~(1 << LayerMask.NameToLayer("Player"))))
        {
            if (m_hitInfoFromObject.collider.gameObject.layer == LayerMask.NameToLayer("Item"))
            {
                m_currentPickupObj = m_hitInfoFromObject.collider.GetComponent<Rigidbody>();
                //Camera.main.transform.position + m_hitInfoFromObject.distance * Camera.main.transform.forward;
                return true;
            }
        }
        return false;
        // return Camera.main.transform.position + distance * Camera.main.transform.forward;
    }
}


