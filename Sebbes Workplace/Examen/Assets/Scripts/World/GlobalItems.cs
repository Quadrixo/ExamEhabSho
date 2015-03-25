using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GlobalItems : MonoBehaviour
{
    public static bool Rezised;

    public GameObject Particle;
    public static GameObject g_Particles;

    public List<GameObject> ListOfObj = new List<GameObject>();
    public static List<GameObject> g_listOfObj = new List<GameObject>();

    float currentTime = 0;
    float Timer = (float)1 / 2;

    public static Resolution currentRes = new Resolution();

    void Awake()
    {
        g_Particles = Particle;
        g_listOfObj = ListOfObj;
        
        currentRes.width = Screen.width;
        currentRes.height = Screen.height;

    }

    void Update()
    {

    }

    void LateUpdate()
    {
        //if (Rezised)
        //{
        //    currentTime += Timer;
        //    if (currentTime >= 1)
        //    {
        //        Rezised = !Rezised;
        //        oldSizeX = currentRes.width;
        //        oldSizeY = currentRes.height;
        //    }
        //}
    }

    public static void NewSize(RectTransform t)
    {
        Vector3 scale = t.localScale;
        Vector2 positio = t.anchoredPosition;

        float tempX = ((float)currentRes.width / 870);
        float tempY = ((float)currentRes.height / 489);


        scale = scale - new Vector3((tempX * scale.x) , (tempY * scale.y) , scale.z);
        positio = positio - new Vector2((tempX * positio.x), (tempY * positio.y) );
        t.anchoredPosition -= positio;
        t.localScale -= scale;
    }   

}

/// <summary>
/// 
/// 100x100
/// 50
/// 100-120/50/60
/// 50x50
/// 25
/// 
/// 75x75
/// 37.5
/// 
/// 
/// 
/// 
/// 
/// </summary>


public enum Positions
{

    AddSub, // 0
    Div,
    Bottom, //1      
    Middle, //2 
    TopLeft, //3     
    ToResult, //4
    result,//5
    resultdivi,
    face,//6
    Picked,//7
    Left, //8
    Right, //9
    Swap,
    Target
};

public enum Scaling
{
    Minimize, //0     
    PickupScale,
    Merging,//1      
    NormalSize // 2 
};

public enum Powers
{
    None, //0            Inget händer
    Add, //1             Ställer upp värden att addera bitar
    Substract, //2       ----------^----------- subtrahera(?) bitar
    Multiplication, //3  Multiplicerar talen
    Division, //4        Delar
    Convert //5        Roten ur ett singulärt tal
};
