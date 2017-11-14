using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WellScript : MonoBehaviour {
	public GameObject cursor;
	private string collectionResults;
	private Text popText;
	private Text groText;
	private Text newText;
	private Text waterText;
	private Text notificationText;

	public AudioSource moveDefaultSound;
	private AudioClip myAudioClip;
	private AudioClip poisonSoundEffect;
	private AudioClip drinkSoundEffect;
	private AudioClip errorSoundEffect;

	private float soundLevel = .18f;
	private int counter = 2;

	private Text selectionText;

	private Text promptText;
	private Text controlText;

	private bool poisonConfirmationPanelUp;
	private bool panelUp;

	private GameObject promptPanel;
	private GameObject errorPanel;

	private int poisonLevel;
	private bool isSelected;
	private bool canAction;
	private GameController gameController;
	// Use this for initialization
	void Start () {
		notificationText = GameObject.Find ("NotificationText").GetComponent<Text>();
		groText = GameObject.Find ("GroText").GetComponent<Text> ();
		waterText = GameObject.Find ("WaterLevelText").GetComponent<Text> ();
		popText = GameObject.Find ("PopulationText").GetComponent<Text> ();
		newText = GameObject.Find ("NewText").GetComponent<Text> ();

		moveDefaultSound = gameObject.GetComponent<AudioSource> ();
		myAudioClip = (AudioClip)Resources.Load<AudioClip> ("Sound/confirm");
		poisonSoundEffect = (AudioClip)Resources.Load<AudioClip> ("Sound/poison");
		drinkSoundEffect = (AudioClip)Resources.Load<AudioClip> ("Sound/drink");
		errorSoundEffect = (AudioClip)Resources.Load<AudioClip> ("Sound/error");

		errorPanel = GameObject.Find ("ErrorPanel");
		promptPanel = GameObject.Find ("PromptPanel");
		//promptPanel.SetActive (false);

		selectionText = GameObject.Find ("SelectionText").GetComponent<Text>();
		promptText = GameObject.Find ("PromptText").GetComponent<Text>();
		promptText.text = "";

		controlText = GameObject.Find ("ControlText").GetComponent<Text> ();
		controlText.text = "";
		isSelected = false;
		canAction = true;


		//establish reference to PoisonWellConfirmationPanel and DrawWaterConfirmationPanel
		panelUp = false;

		//establish reference to Game Controller
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null) {
			gameController = gameControllerObject.GetComponent<GameController> ();
		} else {
			Debug.Log ("Cannot find game controller script");
		}
		poisonLevel = 0;
	}
	// Update is called once per frame
	void Update () {
			CheckInteraction ();
	}
	public void ShootNotification(){
		notificationText.text += collectionResults;
		collectionResults = "";
	}

	public void processPoison(){
		if (poisonLevel > 0) {
			poisonLevel = poisonLevel -1;
		}
	}
	void updatePoisonLevel(int x){
		poisonLevel = poisonLevel + x;
	}
	void OnTriggerEnter2D(Collider2D other){
		isSelected = true;
	}
	void OnTriggerExit2D(Collider2D other){
		counter = 2;
		isSelected = false;
		panelUp = false;
		promptText.text = "";
		controlText.text = "";
		errorPanel.SetActive (false);
		promptPanel.SetActive (false);
	}

	void DrawWaterFromWell(){
		if(poisonLevel < 1){
			
			gameController.GetPlayerVillage().GetComponent<PlayerVillageScript>().RefillWater();


			if (gameController.GetPlayerVillage ().GetComponent<PlayerVillageScript> ().GetGro () >= 0) {
				gameController.GetPlayerVillage ().GetComponent<PlayerVillageScript> ().UpGro ();
				collectionResults += "You have collected pure water.\nYour GRO increases by +1.";
			} else {
				collectionResults += "You replace your tainted supply with clean water.\nYour GRO is now +1.";
				gameController.GetPlayerVillage ().GetComponent<PlayerVillageScript> ().SetGro (.1f);
			}
			canAction = false;
		}else{
			gameController.GetPlayerVillage().GetComponent<PlayerVillageScript>().RefillWater();
			if (gameController.GetPlayerVillage ().GetComponent<PlayerVillageScript> ().GetGro () >= 0) {
				collectionResults += poisonLevel + " units of poison in the well corrupted your supply.\nYour GRO is now -" + poisonLevel + ".";
				gameController.GetPlayerVillage ().GetComponent<PlayerVillageScript> ().SetGro (-poisonLevel * .1f);
			} else {
				collectionResults += "Your supply is still tainted.\nYour GRO decreases by 1."; 
				gameController.GetPlayerVillage ().GetComponent<PlayerVillageScript> ().DownGro ();
			}
		}
		collectionResults += "\n\n";
	}
	public void PoisonWell(){
		updatePoisonLevel(1);
	}
	public int GetPoisonLevel(){
		return poisonLevel;
	}

	private void CheckInteraction(){
		controlText.color = Color.white;
		if (isSelected) {
			Debug.Log ("well selected");
			if (counter > 0) {
				counter--;
				groText.text = "GRO: N/A";
				waterText.text = "WAT: infinite";
				newText.text = "NEW: 13+0";
				popText.text = "POP: 13";
				selectionText.text = "The Holy Well";
			}
			if (!gameController.IsItDay ()) {
				controlText.text = "SPACE: poison well";
				if (gameController.GetPlayerVillage ().GetComponent<PlayerVillageScript> ().GetWaterLevel () < 1000) {
					controlText.text = "NOT ENOUGH WATER TO POISON WELL"; 
					controlText.color = Color.red;
				}
				if (Input.GetAxisRaw ("Action") != 0 && canAction && !panelUp) {
					if (gameController.GetPlayerVillage ().GetComponent<PlayerVillageScript> ().GetWaterLevel () > 999) {

						Debug.Log ("PlaySound");
						moveDefaultSound.PlayOneShot (myAudioClip, soundLevel);
						promptPanel.SetActive (true);
						GameObject cursor = GameObject.FindWithTag ("Cursor");
						promptPanel.transform.position = new Vector3 (gameObject.transform.position.x + 2f, gameObject.transform.position.y + 1, 0);
						promptText.text = "poison the well for 1000 water?";
						panelUp = true;
						canAction = false;
					} else {
						errorPanel.SetActive (true);
						errorPanel.transform.position = new Vector3 (gameObject.transform.position.x + 2f, gameObject.transform.position.y + 1, 0);
						moveDefaultSound.PlayOneShot (drinkSoundEffect, soundLevel+.1f);
						canAction = false;
					}

					if (Input.GetAxisRaw ("Cancel") != 0) {
						promptText.text = "";
						promptPanel.SetActive (false);
						errorPanel.SetActive (false);
						panelUp = false;
						canAction = false;
					}
				}
			}

				if (gameController.IsItDay ()) {
					controlText.text = "SPACE: draw water";
					Debug.Log ("HERE");
					if (Input.GetAxisRaw ("Action") != 0 && !panelUp &&canAction) {
					
						moveDefaultSound.PlayOneShot (myAudioClip, soundLevel);
						promptPanel.SetActive (true);
						GameObject cursor = GameObject.FindWithTag ("Cursor");
						promptPanel.transform.position = new Vector3 (gameObject.transform.position.x + 2f, gameObject.transform.position.y + 1, 0);

						promptText.text = "draw water from the well?";

						panelUp = true;
						canAction = false;
					
					}

					if (Input.GetAxisRaw ("Cancel") != 0) {
						promptText.text = "";
						promptPanel.SetActive (false);
						errorPanel.SetActive (false);
						panelUp = false;
						canAction = false;
					}
				}

				if (Input.GetAxisRaw ("Action") != 0 && canAction && panelUp) {
					promptText.text = "";
					promptPanel.SetActive (false);
					errorPanel.SetActive (false);

					panelUp = false;

					if (gameController.IsItDay ()) {
						moveDefaultSound.PlayOneShot (drinkSoundEffect, soundLevel+.1f);

						DrawWaterFromWell ();
						gameController.GetPlayerVillage ().GetComponent<PlayerVillageScript> ().SetPlayerVisit (true);
					} else { //poisoning well
						if (gameController.GetPlayerVillage ().GetComponent<PlayerVillageScript> ().GetWaterLevel () > 0) {
							moveDefaultSound.PlayOneShot (poisonSoundEffect, soundLevel);
							PoisonWell ();
							gameController.GetPlayerVillage ().GetComponent<PlayerVillageScript> ().LoseThousandWater ();
							gameController.GetPlayerVillage ().GetComponent<PlayerVillageScript> ().SetPlayerVisit (true);
						}
					}
					StartCoroutine (gameController.ChangeTime ());
					canAction = false;
				}

				if (Input.GetAxisRaw ("Action") == 0) {
					canAction = true;
				}
					

	}
		
}
}