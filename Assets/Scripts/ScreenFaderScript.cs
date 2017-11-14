using UnityEngine;
using System.Collections;

public class ScreenFaderScript : MonoBehaviour {
	public Texture2D fadeOutTexture;
	public float fadeSpeed = 0.8f;

	private int drawDepth = -1000; //texture's order in the draw hierarchy
	private float alpha = 1.0f;
	private int fadeDir = -1;

	private void OnGUI(){
		//fade out/in the alpha using a direction, speed, and Time.deltatime to convert the operation to seconds
		alpha+= fadeDir * fadeSpeed * Time.deltaTime;
		//force (clamp) the number between 0 and 1 because GUI.color uses alpha values between 0 and 1
		alpha = Mathf.Clamp01(alpha);

		//set color of our GUI 
		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
		GUI.depth = drawDepth;
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);
		if (alpha <= 0) {
			gameObject.SetActive(false);
		}
	}

	//sets fadeDir to the direction parameter
	public float BeginFade(int direction){
		fadeDir = direction;
		return (fadeSpeed);
	}

	//OnLevelWasLoaded is called when a level is loaded
	public void FadeFromBlack(){
		BeginFade (-1);
		alpha = 1.0f;
	}

	public void FadeToBlack(){
		BeginFade (1);
		alpha = 0.0f;
	}

	public IEnumerator Wait(){
		yield return new WaitForSeconds (5);
	}

}
