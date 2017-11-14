using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WitchScript : MonoBehaviour {
	private int chanceOfCorrect;
	private GameController gameController;
	private Text witchText;

	// Use this for initialization
	void Start () {
		chanceOfCorrect = 0;
		witchText = GameObject.Find ("WitchText").GetComponent<Text> ();
		witchText.text = "\"Greetings Master\"";
	}

	public int ReadTheStars(){
		chanceOfCorrect = Random.Range (0, 100);
		return chanceOfCorrect;
	}

	public void UpdateWitchText(){
		int s = Random.Range (0, 12);
		if (s < 1) {
			witchText.text = "\"The stars align!\"";
		} else if (s < 2) {
			witchText.text = "\"KA KA!\"";
		} else if (s < 3) {
			witchText.text = "\"Such lovely weather...\"";
		} else if (s < 4) {
			witchText.text = "\"Well hello beautiful!\"";
		} else if (s < 5) {
			witchText.text = "\"Oh what delicious temptations...\"";
		} else if (s < 6) {
			witchText.text = "\"Oh sweet congregation of flesh...\"";
		} else if (s < 7) {
			witchText.text = "\"No love, no hope, only bones.\"";
		} else if (s < 8) {
			witchText.text = "\"OH GOD THERE'S NOTHING BUT SALT!\"";
		} else if (s < 9) {
			witchText.text = "\"LET ME OUT! LET ME OUT OF THIS BOX!\"";
		} else if (s < 10) {
			witchText.text = "\"Let us join the feast of limbs.\"";
		} else if (s < 11) {
			witchText.text = "\"What say the bones?\"";
		} else if (s < 12) {
			witchText.text = "\"Ding Dong!\"";
		}
		witchText.text += "\n\nThere is a " + chanceOfCorrect.ToString() + "% chance his answers are informed.";
	}
}
