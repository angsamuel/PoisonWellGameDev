using UnityEngine;
using System.Collections;

public class NewGameButtonScript : MonoBehaviour {
	public GameObject screenFader;
	public IEnumerator LoadScene (string level) {
		GameObject.Find ("MusicBox").GetComponent<SpringMusicBoxScript> ().FadeOutMusic ();
		screenFader.SetActive (true);
		screenFader.GetComponent<ScreenFaderScript> ().FadeToBlack ();
		yield return new WaitForSeconds(3);
		Application.LoadLevel (level);

	}

	public void StartGame(){
		StartCoroutine(LoadScene("overworld"));
	}

	public void BackToMain(){
		StartCoroutine(LoadScene("MainMenu"));
	}
	public void ViewTutorial(){
		StartCoroutine(LoadScene("Tutorial"));
	}

	public void StraightToMenu(){
		Application.LoadLevel ("MainMenu");
	}

}
