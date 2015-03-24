using UnityEngine;
using System.Collections;

public class ObjFader : MonoBehaviour {

    

    public float fadeSpeed = 0.25f;		// the fading speed

    private float alpha = 1f;
    private float fixedAlpha;
    private int fadeDir;

    private bool fade = false;
    private bool fixFade = false;

	void Update () {

        if (fade)
        {
            alpha += fadeDir * fadeSpeed * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);
            GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, alpha);
            fade = (alpha != fadeDir);
            if (!fade)
                gameObject.SetActive(false);
        }
        else if (fixFade)
        {
            GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, alpha);
            fixFade = false;
        }
    }

    public void SetBackground(float value)
    {
        fixFade = true;
        alpha = value;
    }
    
	// sets fadeDir to the direction parameter making the scene fade in if -1 and out if 1
	public float BeginFade (int direction)
	{
        fade = true;
		fadeDir = direction;
        
		return (fadeSpeed);
	}

    public float backGroundAlpha
    {
        get
        {
            return alpha;
        }
    }

    public bool fading
    {
        get
        {
            return (fade||fixFade);
        }
    }
}
