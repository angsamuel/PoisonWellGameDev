using UnityEngine;
using System.Collections;

public class SpringMusicBoxScript : MonoBehaviour {
	public AudioSource source;
	private bool fadingOut = false;
	public float maxVol = .25f;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void FadeInMusic(){
		source.Play ();
		StartCoroutine ("FadeSoundIn");
	}

	public void FadeOutMusic(){
		StartCoroutine ("FadeSoundOut");
	}

	public IEnumerator FadeSoundIn(){
		while(source.volume < maxVol && !fadingOut){
			source.volume += Time.deltaTime / 10.0f;
			yield return null;
		}
	}

	public IEnumerator FadeSoundOut(){
		fadingOut = true;
		while(source.volume > 0.0f){
			source.volume -= Time.deltaTime / 10.0f;
			yield return null;
		}
		fadingOut = false;
	}
}
