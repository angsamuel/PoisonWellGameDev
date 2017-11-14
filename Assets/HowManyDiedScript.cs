using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;

public class HowManyDiedScript : MonoBehaviour {
	private Text deadText;
	private Text daysText;
	// Use this for initialization
	void Start () {
		deadText = GameObject.Find ("DeadText").GetComponent<Text>();
		deadText.text = GameController.deathToll + " died along the way";
		daysText = GameObject.Find("TimeText").GetComponent<Text>();
		daysText.text = "your struggle lasted " + GameController.dayNumber + " days";
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
