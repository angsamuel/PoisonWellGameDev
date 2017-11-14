using UnityEngine;
using System.Collections;

public class ScreenFadeOutSquareScript : MonoBehaviour {
	float t;
	private bool f;

	private float trueMinimum = 0.0f;
	public float trueMaximum = 1f;

	public float minimum;
	public float maximum;

	public float duration = 1.0f;
	private float startTime;

	private bool fadeIn = true;
	public SpriteRenderer sprite;

	void Start() {
		f = true;
		t = 0;
		minimum = trueMinimum;
		maximum = trueMaximum;

		startTime = Time.time;
		sprite = gameObject.GetComponent<SpriteRenderer> ();
	}
	void Update() {
		t = (Time.time - startTime)/2;

		sprite.color = new Color (1f, 1f, 1f, Mathf.SmoothStep (minimum,  maximum, t));
	}
	public void ScreenRev(){
		startTime = Time.time;
	}
}