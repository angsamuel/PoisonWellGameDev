using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;

public class VillageScript : BaseVillageScript {
	

	//UI
	private Text waterLevelText;

	//unique village death varaible
	public Sprite deathSprite;
	public bool markedForDeath;

	//double buffered decision probabilities
	private float nextDecisionProbability = 0.0f;
	private float currentDecisionProbability = 0.0f;
	//personality int corresponds to function call
	private int personality;	

	//poison management
	private bool poisoned;

	//water resources
	//private int waterLevel;
	//private int maxWaterLevel = 10000;


	//well connection
	private WellScript wellScript;


	// Use this for initialization
	void Start () {
		base.Start ();
		villageNameText = GameObject.Find ("SelectionText").GetComponent<Text>();

		//set up population

		poisoned = false;

		waterLevelText = GameObject.Find ("WaterLevelText").GetComponent<Text> ();

		//find well
		GameObject wellObject = GameObject.FindWithTag ("Well");
		wellScript = wellObject.GetComponent<WellScript> ();

		waterLevel = maxWaterLevel / 2;
		//Debug.Log ("WATER LEVEL " + waterLevel);

		//find Game Controller


		//determine personality
		personality = Random.Range (0, 7);

		// pick a random color
		float R = Random.Range(.4f, .9f);
		float G = Random.Range (.4f, .9f);
		float B = Random.Range (.4f, .9f);
		Color newColor = new Color(R, G, B, 1.0f );

		// apply it on current object's material
		GetComponent<Renderer>().material.color = newColor; 

		//opponent villages are not selected
		isSelected = false;
		markedForDeath = false;


	}
	void Update(){
		if (counter > 0 && isSelected) {
			villageNameText.color = GetComponent<Renderer> ().material.color;
			counter--;
		}
	}
		
	//GET METHODS
	public string GetName(){
		return villageName;
	}
	public bool isPoisoned(){
		return poisoned;
	}
	public int GetWaterLevel(){
		return waterLevel;
	}
	public bool IsMarkedForDeath(){
		return markedForDeath;
	}
	public int GetPopulation(){
		return population;
	}
		
	//SET METHODS
	public void SetName(string newName){
		villageName = newName;
	}
	public void MarkForDeath(){
		markedForDeath = true;
	}
		
	//HELPER FUNCTIONS AND MAIN FUNCTIONALITYs

	public void VillageCollectsWater(){
		waterLevel = maxWaterLevel+1;
		if (wellScript.GetPoisonLevel() < 1) {
			if (GetGro() >= 0) {
				UpGro ();
			} else {
				SetGro (.1f);
			}
		} else {
			waterLevel = maxWaterLevel;
			if (GetGro () >= 0) {
				SetGro (-wellScript.GetPoisonLevel() * .1f);
			} else {
				DownGro ();
			}
		}
	}

	public float GetCurrentDecisionProbability(){
		if (currentDecisionProbability > 100.0f) {
			return 100.0f;
		}else if (currentDecisionProbability >= 0) {
			return currentDecisionProbability;
		} else {
			return 0;
		}
	}
	public float GetNextDecisionProbability(){
		if (nextDecisionProbability > 100.0f) {
			return 100.0f;
		}else if (nextDecisionProbability >= 0) {
			return nextDecisionProbability;
		} else {
			return 0;
		}
	}
	public void swapProbabilities(){
		currentDecisionProbability = nextDecisionProbability;
	}

	void OnTriggerEnter2D(Collider2D other){
		leaderText.text = "LED: " + villageLeaderName;
		villageNameText.text = villageName;
		villageNameText.color = GetComponent<Renderer> ().material.color;
		isSelected = true;
		popText.text = "POP: " + population; 
		groText.text = "GRO: "; 
		if (Mathf.FloorToInt (k * 10)>0) {
			groText.text += "+";
		}
		groText.text += Mathf.FloorToInt (k * 10);
		waterLevelText.text = "WAT: ???";
		if ((Mathf.FloorToInt (Mathf.Exp (k) * population) - population) > 0) {
			newText.text = "NEW: " + population + " + " + (Mathf.FloorToInt (Mathf.Exp (k) * population) - population);
		} else {
			newText.text = "NEW: " + population + " - " + Mathf.Abs((Mathf.FloorToInt (Mathf.Exp (k) * population) - population));
		}
	}
	void OnTriggerExit2D(Collider2D other){
		isSelected = false;
		villageNameText.color = Color.white;
		counter = 7;
	}
	//THIS IS WHERE DECISIONS ARE MADE
	public void MakeBufferedDecision(){
		float decision = 0.0f;
		if (gameController.IsItDay()) {
			//water levels
			float waterBias = (1.0f - (waterLevel / maxWaterLevel)) * 40.0f; 
			float survivorBias = (gameController.GetOpponentList ().Count / gameController.GetOriginialOpponentCount ()) * 15.0f;
			//how many poisoned
			float poisonedBias = (gameController.GetNumberOfVillagesPoisoned ()) * -4.0f;
			float timeBias = -1 * gameController.GetDayNumber () * 0.5f;
			float popBias = GetPopulation () * 0.01f;

			float groBias = 0;
			if (GetGro () > 0) {
				groBias = (GetGro () -1) * -5.0f;
			}

			nextDecisionProbability = waterBias + survivorBias + poisonedBias + timeBias + popBias + groBias;
			//Debug.Log (nextDecisionProbability + " day");
		} else {
			//water levels
			float waterBias = (waterLevel / maxWaterLevel) * 10.0f; 
			float survivorBias = (1 -(gameController.GetOpponentList ().Count / gameController.GetOriginialOpponentCount ())) * 15.0f;
			//how many poisoned
			float poisonedBias = ((gameController.GetNumberOfVillagesPoisoned ())) * -12.0f;
			float timeBias = gameController.GetDayNumber () * 0.5f;
			float popBias = GetPopulation () * 0.01f;
			nextDecisionProbability = waterBias + survivorBias + poisonedBias + timeBias + popBias;
			//Debug.Log (nextDecisionProbability + " night");
		}

		//a little spice
		nextDecisionProbability += Random.Range (-8, 5);


		//OLD BEHAVIOR
		/*
		if (waterLevel > population) {
			int dayNumber = gameController.GetDayNumber ();
			int isolationBiasPoison = 50 - (gameController.GetDayNumber ()*2);
			int isolationBiasDrink = 65 - (gameController.GetDayNumber ()*2);
			if (waterLevel < 40) {
				if (gameController.IsItDay ()) {
					nextDecisionProbability = 100.0f;
				}
			} else {
				if (personality < 1) {
					MakeBufferedDecisionDefault ();
				} else if (personality < 2) {
					MakeBufferedDecisionGenocideArtist ();
				} else if (personality < 3) {
					MakeBufferedDecisionPacifist ();
				} else if (personality < 4) {
					MakeBufferedDecisionIsolationist ();
				} else if (personality < 5) {
					MakeBufferedDecisionNaive ();
				} else if (personality < 6) {
					MakeBufferedDecisionSadist ();
				} else if (personality < 7) {
					MakeBufferedDecisionInsane ();
				} else {
					MakeBufferedDecisionDefault ();
				}

				if (gameController.IsItDay ()) {
					nextDecisionProbability += (GetGro () * -1.0f) *5.0f;
				}
				if (!gameController.IsItDay ()) {
					Debug.Log ("wow it's night apparently");
					nextDecisionProbability = nextDecisionProbability;
				}
				if (gameController.IsItDay ()) {
					nextDecisionProbability = nextDecisionProbability;
				}
				//sin
				if (nextDecisionProbability <= 0) {
					nextDecisionProbability = 0 + Random.Range (1, 5);
				}
				//regret
				if (nextDecisionProbability >= 100) {
					nextDecisionProbability = 100 - Random.Range (1, 5);
				}
			}
		} else if (gameController.IsItDay ()) {
			nextDecisionProbability = 75.0f;
		} else {
			nextDecisionProbability = 0.0f;
		}
		if (waterLevel < 1000 && !gameController.IsItDay ()) {
			nextDecisionProbability = 0.0f;
		}
		*/
	}
	public float DrinkByLowWater(){
		if(waterLevel < 1){return 100.0f;}
		return ((1.0f - (waterLevel * 2.0f / maxWaterLevel * 1.0f))) * 100.0f;
	}
	public float PoisonByHighWater(){
		return ((waterLevel*1.0f)/(maxWaterLevel*10.0f))*100.0f;
	}
	public float DrinkByManyVillages(){
		if(waterLevel < 1){return 100.0f;}
		return ((gameController.GetOpponentList().Count*1.0f)/(gameController.GetOriginialOpponentCount()*8.0f)) * 100f;
	}
	public float DrinkByFewVillages(){
		if(waterLevel < 1){return 100.0f;}
		return (1.0f - ((gameController.GetOpponentList().Count*2.0f)/(gameController.GetOriginialOpponentCount()*1.0f))) *100f;
	}
	public float PoisonByManyVillages(){
		return ((gameController.GetOpponentList().Count*1.0f)/(gameController.GetOriginialOpponentCount()*10.0f)) * 100f;
	}
	public float PoisonByFewVillages(){
		return (1.0f - ((gameController.GetOpponentList().Count*4.0f)/(gameController.GetOriginialOpponentCount()*1.0f))) * 100f;
	}
	public float PoisonByLowGlobalWaterLevel(){
		List<GameObject> opponentList = gameController.GetOpponentList ();
		int globalMaxSupply = 10 * opponentList.Count ();
		int globalCurrentSupply = 0;
		foreach (GameObject opponent in opponentList) {
			globalCurrentSupply += opponent.GetComponent<VillageScript>().GetWaterLevel ();
		}
		return (1.0f-((globalCurrentSupply+GameObject.FindGameObjectWithTag ("PlayerVillage").GetComponent<PlayerVillageScript>().GetWaterLevel()*4.0f)/((globalMaxSupply+10000)*1.0f))) * 100.0f;
	}
	public float DrinkByRequirement(){
		if (waterLevel < 20) {
			return 100.0f;
		} else {
			return 0.0f;
		}
	}
		
	public void MakeBufferedDecisionDefault(){//draws water when supplies get low, poisons when high
		if (gameController.IsItDay ()) { //day
			nextDecisionProbability = DrinkByLowWater();
		}else {
			nextDecisionProbability = PoisonByHighWater();
		}
	}
	public void MakeBufferedDecisionGenocideArtist(){
		if (gameController.IsItDay ()) {
			nextDecisionProbability = DrinkByLowWater ();
		} else {
			nextDecisionProbability = PoisonByLowGlobalWaterLevel ();
		}
	}

	public void MakeBufferedDecisionPacifist(){
		if (gameController.IsItDay ()) { //day
				nextDecisionProbability = DrinkByLowWater();
		} else { //night
			nextDecisionProbability = 0.0f;
		}
	}
	public void MakeBufferedDecisionIsolationist(){
		if (gameController.IsItDay ()) {
			nextDecisionProbability = DrinkByRequirement ();
		} else {
			nextDecisionProbability = 0;
		}
	}
	public void MakeBufferedDecisionThirsty(){
		if (gameController.IsItDay ()) {
			nextDecisionProbability = 100;
		} else {
			nextDecisionProbability = PoisonByHighWater ();
		}
	}
	public void MakeBufferedDecisionNaive(){
		if (gameController.IsItDay ()) {
			nextDecisionProbability = DrinkByManyVillages ();
		} else {
			nextDecisionProbability = PoisonByFewVillages ();
		}
	}
	public void MakeBufferedDecisionSadist(){
		if (gameController.IsItDay ()) {
			nextDecisionProbability = DrinkByFewVillages ();
		} else {
			nextDecisionProbability = PoisonByManyVillages ();
		}
	}
	public void MakeBufferedDecisionInsane(){
		nextDecisionProbability = Random.Range (0, 100);
	}

	public void ActiviateDeathSprite(){
		GetComponent<SpriteRenderer> ().sprite = deathSprite;
		villageName += "\n(DECIMATED)";
		SetGro (0);
		SetPop (0);
		villageLeaderName += " (EXILED)";
	}
}

//spoke to someone, asking for microsoft activation code