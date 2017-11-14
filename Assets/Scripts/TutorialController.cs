using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class TutorialController : MonoBehaviour {
	private int pageNum;
	private GameObject tutorialPage;
	private Vector3 frameLocation;
	AudioSource audioSource;
	public AudioClip audioClip;

	// Use this for initialization
	void Start () {
		pageNum = 0;
		tutorialPage = GameObject.Find ("Tutorial" + pageNum);
		frameLocation = tutorialPage.transform.position;
		audioSource = GetComponent<AudioSource> ();
	}


	public void TurnToNextPage(){
		pageNum++;
		if (pageNum == 43) {
			GameObject musicBox = GameObject.Find ("MusicBox");
			musicBox.GetComponent<AudioSource> ().Pause();
		}
		if (pageNum == 45) {
			audioSource.Play ();
		}
		if (pageNum == 46) {
			GameObject musicBox = GameObject.Find ("MusicBox");
			musicBox.GetComponent<AudioSource> ().UnPause();
		}
		if (pageNum == 47) {
			SceneManager.LoadScene ("MainMenu");
		}
		tutorialPage.transform.position = new Vector3 (-1000, -1000, 0);
		tutorialPage = GameObject.Find ("Tutorial" + pageNum);
		tutorialPage.transform.position = frameLocation;
	}

	public void TurnToPrevPage(){
		if (pageNum == 0) {
			SceneManager.LoadScene ("MainMenu");
		}
		pageNum--;
		tutorialPage.transform.position = new Vector3 (-1000, -1000, 0);
		tutorialPage = GameObject.Find ("Tutorial" + pageNum);
		tutorialPage.transform.position = frameLocation;
	}

	public void PlayWitchSound(){
		audioSource.Play ();
		
	}

	// Update is called once per frame
	void Update () {
	
	}
}
