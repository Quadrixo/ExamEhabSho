using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextTest : MonoBehaviour {

    Text text;

    void Awake()
    {
        text = GetComponent<Text>();
    }

    public float fadeSpeed = 0.5f;		// the fading speed

    private int drawDepth = -1000;		// the texture's order in the draw hierarchy: a low number means it renders on top
    private float alpha = 0f;			// the texture's alpha value between 0 and 1
    private int fadeDir = 1;			// the direction to fade: in = -1 or out = 1

    void Update()
    {
        alpha += fadeDir * fadeSpeed * Time.deltaTime;
        alpha = Mathf.Clamp01(alpha);
        text.color = new Color(0, 0, 0, alpha);
    }

    // sets fadeDir to the direction parameter making the scene fade in if -1 and out if 1
    public float BeginFade(int direction)
    {
        fadeDir = direction;

        return (fadeSpeed);
    }

    // OnLevelWasLoaded is called when a level is loaded. It takes loaded level index (int) as a parameter so you can limit the fade in to certain scenes.
    void OnLevelWasLoaded()
    {
        // alpha = 1;		// use this if the alpha is not set to 1 by default
        BeginFade(1);		// call the fade in function
    }
}
