using UnityEngine;
using System.Collections;
using  System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using System.Text;

public class GuardScript : MonoBehaviour {
	private GameController gameController;
	private Text guardText;
	// Use this for initialization
	void Start () {
		gameController = GameObject.FindWithTag ("GameController").GetComponent<GameController> ();
		guardText = GameObject.Find ("GuardText").GetComponent<Text> ();
		guardText.text = "\"Greetings My Liege\"";
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void GetFalseInfo(){
		guardText.text = "";

		AddQuote ();

		if (gameController.IsItDay ()) {
			guardText.text += "\n\nRecent movements suggest there's a ";
			guardText.text += Random.Range(0,100) + "% chance that a village";
			guardText.text += " poisoned the well.";
		} else {
			guardText.text += "\n\nIntel suggests there's a ";
			guardText.text += Random.Range(0,100) + "% chance that a village";
			guardText.text += " will drink from the well tomorrow.";
		}
	}

	public void GetRealInfo (){
		guardText.text = "";
		float trueInfo = 1.0f;
		List<GameObject> opponentList = gameController.GetOpponentList ();
		for(int i = 0; i<opponentList.Count; i++){
			trueInfo = trueInfo * (1.0f-((opponentList [i].GetComponent<VillageScript> ().GetNextDecisionProbability ())/100.0f)); 
		}
		Debug.Log ("TRUE INFOd " + (1.0f-trueInfo)*100.0f);
 
		int trueInfoInt = Mathf.FloorToInt((1.0f-(trueInfo))*100.0f);
	
	
		AddQuote ();

		if (gameController.IsItDay ()) {
			guardText.text += "\n\nRecent movements suggest there's a ";
			guardText.text += trueInfoInt.ToString () + "% chance that a village";
			guardText.text += " poisoned the well.";
		} else {
			guardText.text += "\n\nIntel suggests there's a ";
			guardText.text += trueInfoInt.ToString () + "% chance that a village";
			guardText.text += " will drink from the well tomorrow.";
		}
	
	}

	private void AddQuote(){
		//generate quote
		int quote = Random.Range (0, 10);
		if (quote < 1) {
			guardText.text += "\"Well butter my back and call me a biscuit...\"";
		} else if (quote < 2) {
			guardText.text += "\"I could go for some baby back ribs.\"";
		} else if (quote < 3) {
			guardText.text += "\"My Liege!\"";
		} else if (quote < 4) {
			guardText.text += "\"...hup two three four...\"";
		} else if (quote < 5) {
			guardText.text += "\"Such pain, such loss, and for what?\"";
		} else if (quote < 6) {
			guardText.text += "\"The fiends will taste our wrath!\"";
		} else if (quote < 7) {
			guardText.text += "\"HOLY HELL!\"";
		} else if (quote < 8) {
			guardText.text += "\"By the holy lance!\"";
		} else if (quote < 9) {
			guardText.text += "\"Reporting in!\"";
		} else if (quote < 10) {
			guardText.text += "\"Need me some gumbo!\"";
		} else {
			guardText.text += "\"Fire up that grill!\"";
		}

	}
}
