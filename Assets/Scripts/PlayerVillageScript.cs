using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerVillageScript : BaseVillageScript {


	private float soundLevel = .18f;
	public AudioSource audioSource;
	private AudioClip confirmAudioClip;
	private AudioClip waitSoundEffect;

	bool isPoisoned = false;

	int counter = 2;
	private bool playerVisit;

	private Text controlText;

	private Text selectionText;

	//water resources
	//private int maxWaterLevel = 10000;
	//private int waterLevel;

	//player interaction
	private bool isSelected;
	bool canAction = true;

	//UI connection
	private Text waterLevelText;
	bool panelUp;


	private GameObject promptPanel;
	private Text promptText;

	private Text warningText;
	// Use this for initialization
	void Start () {
		//population
		base.Start ();


		audioSource = gameObject.GetComponent<AudioSource> ();
		confirmAudioClip = (AudioClip)Resources.Load<AudioClip> ("Sound/confirm");
		waitSoundEffect = (AudioClip)Resources.Load<AudioClip> ("Sound/abstain");

		controlText = GameObject.Find ("ControlText").GetComponent<Text> ();
		controlText.text = "";

		promptText = GameObject.Find ("PromptText").GetComponent<Text> ();
		promptPanel = GameObject.Find ("PromptPanel"); 
		promptPanel.SetActive (false);

		warningText = GameObject.Find ("WarningText").GetComponent<Text> ();
		warningText.text = "";

		playerVisit = false;

		selectionText = GameObject.Find ("SelectionText").GetComponent<Text>();
		selectionText.text = "";

		canAction = true;
		isSelected = true;
		waterLevel = maxWaterLevel/2;

		panelUp = false;

		waterLevelText = GameObject.Find("WaterLevelText").GetComponent<Text>();

		waterLevelText.text = "WAT: " + waterLevel;
	}
	
	// Update is called once per frame
	void Update () {
			if (isSelected) {
				if (counter > 0) {
					counter--;
					leaderText.text = "LED: " + villageLeaderName;
					controlText.text = "SPACE: take no action";
					waterLevelText.text = "WAT: " + waterLevel;
					if (waterLevel < population) {
						warningText.text = "REVOLT imminent!";

					if (waterLevel < 20) {
						warningText.text = "TOTAL ANNIHILATION IMMINENT";
					} else if (population < 40) {
						warningText.text = "DEADLY REVOLT imminent!";
					}
					} else {
						warningText.text = "";
					}
				}	
				groText.text = "GRO: "; 
				if (Mathf.FloorToInt (k * 10)>0) {
					groText.text += "+";  
				}
				groText.text += Mathf.FloorToInt (k * 10);
				waterLevelText.text = "WAT: " + waterLevel + "/10000";
				popText.text = "POP: " + population;
				newText.text = "NEW: " + population;
				int tempNew = (Mathf.FloorToInt(Mathf.Exp (k)*population)-population);
				if(tempNew < 0){
					newText.text += " - ";
				}else{ 
					newText.text += " + ";
				}
				newText.text += Mathf.Abs(tempNew);

				selectionText.text = villageName + "\nYour Home";
				if (Input.GetAxisRaw ("Action") != 0 && canAction && !panelUp) {
				Debug.Log ("action");
				audioSource.PlayOneShot (confirmAudioClip, soundLevel);
					promptText.text = "take no action?";
					promptPanel.SetActive (true);
					MakePromptAppear ();

					panelUp = true;
					canAction = false;
				}
				if (Input.GetAxisRaw ("Cancel") != 0) {
					promptText.text = "";
					promptPanel.SetActive (false);
					panelUp = false;
					canAction = false;
				}
				if (Input.GetAxisRaw ("Action") != 0 && canAction) {
					
					promptText.text = "";
					promptPanel.SetActive (false);
					panelUp = false;
					audioSource.PlayOneShot (waitSoundEffect, soundLevel+.1f);
					StartCoroutine (gameController.ChangeTime ());
					canAction = false;
				}
				if (Input.GetAxisRaw ("Action") == 0) {
					canAction = true;
				}
		}
	}

	void MakePromptAppear(){
		int flippery = 1;
		int flipperx = 1;
		if (gameObject.transform.position.x > 3) {
			flipperx = -flipperx;
		}
		if (gameObject.transform.position.y > 0) {
			flippery = -flippery;
		}
		GameObject cursor = GameObject.FindWithTag("Cursor");
		promptPanel.transform.position = new Vector3 (gameObject.transform.position.x+(2f*flipperx), gameObject.transform.position.y+(1*flippery), 0);
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
		promptPanel.SetActive (false);
		warningText.text = "";
	}

	//GET FUNCTIONS
	public int GetWaterLevel(){
		return waterLevel;
	}
	public bool GetPlayerVisit(){
		return playerVisit;
	}

	//SET FUNCTIONS
	public void SetPlayerVisit(bool x){
		playerVisit = x;
	}
	public void SetName(string newName){
		villageName = newName;
	}

		
	//MAIN FUNCTIONALITY
	public void UpdatePopulation(){
		population = (int)(population * Mathf.Exp (k));
	}

	public void RefillWater(){
		//Debug.Log ("REEEEEEFILLLLL!");
		waterLevel = maxWaterLevel;
	}

	public void LoseThousandWater(){
		waterLevel = waterLevel - 1000;
	}

	public void PoisonPlayer(){
		isPoisoned = true;
	}
	public bool PlayerIsPoisoned(){
		return isPoisoned;
	}

}
