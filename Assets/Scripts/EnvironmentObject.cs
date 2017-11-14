using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnvironmentObject : MonoBehaviour {
	//name to be displayed
	protected string objectName;
	int population;

	//GUI text
	protected Text selectionText;
	protected Text popText;
	protected Text waterLevelText;
	protected Text groText;
	protected Text newText;
	protected Text leaderText;


	protected void Start(){
		leaderText = GameObject.Find ("LeaderText").GetComponent<Text>();
		waterLevelText = GameObject.Find ("WaterLevelText").GetComponent<Text> ();
		selectionText = GameObject.Find ("SelectionText").GetComponent<Text>();
		popText = GameObject.Find ("PopulationText").GetComponent<Text> ();
		groText = GameObject.Find ("GroText").GetComponent<Text> ();
		newText = GameObject.Find ("NewText").GetComponent<Text> ();
		population = Random.Range (0, 10);

	}
		
	void Update(){}

	protected void OnTriggerEnter2D(Collider2D other){
		leaderText.text = "LED: N/A";
		selectionText.text = objectName;
		popText.text = "POP: " + population;
		waterLevelText.text = "WAT: N/A";
		groText.text = "GRO: N/A";
		newText.text = "NEW: " + population + " + 0";
	}
}
