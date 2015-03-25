using UnityEngine;
using System.Collections;

public class Fading {

	private Texture2D fadeOutTexture;	// the texture that will overlay the screen. This can be a black image or a loading graphic
	private float fadeSpeed = 0.8f;		// the fading speed

	private int drawDepth = -1000;		// the texture's order in the draw hierarchy: a low number means it renders on top
	private float alpha = 1.0f;			// the texture's alpha value between 0 and 1
	private int fadeDir = -1;			// the direction to fade: in = -1 or out = 1

    private bool fade = false;


    public Fading(Texture2D _texture, float _fadeSpeed)
    {
        fadeOutTexture = _texture;
        fadeSpeed = _fadeSpeed;
    }
    
    void Fade()
    {
        if (fade)
        {
            alpha += fadeDir * fadeSpeed * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
            GUI.depth = drawDepth;																
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);
            
            fade = (alpha != fadeDir);
        }
    }

	// sets fadeDir to the direction parameter making the scene fade in if -1 and out if 1
	public float BeginFade (int direction)
	{
        fade = true;

		fadeDir = direction;
        
		return (fadeSpeed);
	}

	// OnLevelWasLoaded is called when a level is loaded. It takes loaded level index (int) as a parameter so you can limit the fade in to certain scenes.
}
