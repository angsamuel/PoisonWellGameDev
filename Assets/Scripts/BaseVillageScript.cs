using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;

public class BaseVillageScript : MonoBehaviour {
	//POPULATION DLC PACK!
	protected float k = 0.1f;
	protected int waterLevel;
	protected int maxWaterLevel = 10000;
	protected string villageLeaderName;

	protected int population;
	protected Text popText;
	protected Text kText;

	//UI elements
	protected Text villageNameText;
	protected Text groText;
	protected Text newText;
	protected Text leaderText;

	protected bool isSelected;

	protected string villageName;

	//counter to avoid UI update race conditions
	protected int counter = 7;
	protected int newPopulation = 0;

	//game controller connection
	protected GameController gameController;


	protected void Start(){
		leaderText = GameObject.Find ("LeaderText").GetComponent<Text>();
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		gameController = gameControllerObject.GetComponent<GameController> ();

		waterLevel = maxWaterLevel / 2;
		popText = GameObject.Find ("PopulationText").GetComponent<Text> ();
		popText.text = "";
		groText = GameObject.Find ("GroText").GetComponent<Text> ();
		groText.text = "";
		newText = GameObject.Find ("NewText").GetComponent<Text> ();
		newText.text = "";

		population = Random.Range(800, 1200);

	}

	public void SetLeaderName(string newLeaderName){
		villageLeaderName = newLeaderName;
	}

	public void ConsumeWater(){
		int diedFromFighting;
		int diedFromThirst = 0;
		if (waterLevel >= population) {
			waterLevel = waterLevel - population;
		} else {//REVOLUTION BOYOS
			diedFromFighting = (population / Random.Range (2, 5));
			GameController.deathToll += diedFromFighting;
			population = population - diedFromFighting;
			if (population > waterLevel) {
				diedFromThirst = population - waterLevel;
				GameController.deathToll += diedFromThirst;
				population = population - diedFromThirst;
				waterLevel = 0;
			} else {
				waterLevel = waterLevel - population;
			}
			string revoltMessage = villageName + " has revolted due to water shortages.";
			revoltMessage += "\n\t-" + diedFromThirst + " died without water.";
			revoltMessage += "\n\t-" + diedFromFighting + " died in the chaos\n\n";
			GameObject.Find ("NotificationText").GetComponent<Text> ().text += revoltMessage;
			Debug.Log("REVOLUTION: " + revoltMessage);

		}
	}

	public void UpdatePopulation(){
		int newPops = CalculateNewPops ();
		if (newPops < 0) {
			GameController.deathToll += Mathf.Abs (newPops);
		}
		population = population + newPops;
	}

	public int CalculateNewPops(){
		return Mathf.FloorToInt(Mathf.Exp (k)*population)-population;
	}

	public string GetNewTextString(){
		return "NEW: " + population + " + " + CalculateNewPops ();
	}
	public int GetPopulation(){
		return population;
	}
	public void UpGro(){
		k = k + 0.1f;
	}
	public void DownGro(){
		k = k - 0.1f;
	}
	public void FirstPoisonGro(){
		k = -0.3f;
	}
	public void SetGro(float newGro){
		k = newGro;
	}
	public float GetGro(){
		return k;
	}
	public void SetPop(int newPop){
		population = newPop;
	}
}
