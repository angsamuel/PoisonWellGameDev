using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
	int origOpponents;
	int numberOfOpponentsPoisoned = 0;

	//counter for all deaths that take place
	public static int deathToll = 0;

	//number of days each season takes
	public int seasonLength;

	//UI elements
	private GameObject tutorialPanel;
	private Text tutorialText;
	private Text guardText; 
	private Text seasonText;
	private Text deathTollText;

	//weather
	private GameObject springWeather;
	private GameObject summerWeather;
	private GameObject fallWeather;
	private GameObject winterWeather;

	//weather music
	private GameObject springMusicBoxObject;
	private GameObject summerMusicBoxObject;
	private GameObject fallMusicBoxObject;
	private GameObject winterMusicBoxObject;
	private GameObject nightMusicBoxObject;

	//music players
	private SpringMusicBoxScript springMusicBoxScript;
	private SpringMusicBoxScript summerMusicBoxScript;
	private SpringMusicBoxScript fallMusicBoxScript;
	private SpringMusicBoxScript winterMusicBoxScript;
	private SpringMusicBoxScript nightMusicBoxScript;

	//list of trees, used for changing color during seasons
	private GameObject[] trees;

	//advisors
	private GuardScript guardScript;
	private WitchScript witchScript;

	//notification text
	private Text notificationText;
	string tempText;

	//transition time variables
	public int transitionLength = 1;
	private int timer = 0;
	public float duration = 4.0F;

	//number of opponents
	public int opponents;

	//well
	private GameObject well;
	private WellScript wellScript;

	//player village
	private GameObject myPlayerVillage;
	private PlayerVillageScript myPlayerVillageScript;

	//cursor Object
	private GameObject myCursor; 
	private CursorScript myCursorScript;

	//Day Night Resources
	public Color nightColor;
	public Color dayColor;
	public Color springColor;
	public Color summerColor;
	public Color fallColor;
	public Color winterColor;

	//tree colors
	public Color springSummerTreeColor;
	public Color fallTreeColor;
	public Color winterTreeColor;


	//camera
	public Camera camera;
	public GameObject screenFader;
	private ScreenFaderScript screenFaderScript;

	//isDay tells us if were are in day or night state
	private bool isDay;

	//used for world generation
	private int[,] mapOccupation = new int[7,7];
	private List<GameObject> opponentList;
	public Sprite[] villageSprites;

	//day and season varaibles for UI
	private Text dayNumberText;
	public static int dayNumber = 0;
	public int dayNumberSeason = 0;

	//Word Lists
	List<string> villageNounList = new List<string>();
	List<string> namesList = new List<string>();
	List<string> adjectiveList = new List<string>();
	List<string> nounList = new List<string>();
	List<string> maleNameList = new List<string> ();
	List<string> femaleNameList = new List<string>();
	List<string> lastNameList = new List<string>();

	string[] villageNounArr;
	string[] namesArr;
	string[] adjectiveArr;
	string[] nounArr;
	string[] maleNamesArr;
	string[] femaleNamesArr;
	string[] lastNamesArr;


	//variable telling us if we need notification text
	private bool needText = false;

	// Use this for initialization
	void Start () {
		//instantiate weather effects and music
		deathTollText = GameObject.Find ("DeathTollText").GetComponent<Text> ();
		deathTollText.text = "0 have died";
		winterWeather = GameObject.Find ("WinterWeather");
		winterWeather.SetActive (false);
		springWeather = GameObject.Find ("SpringWeather");
		springWeather.SetActive (false);
		summerWeather = GameObject.Find ("SummerWeather");
		summerWeather.SetActive (false);
		fallWeather = GameObject.Find ("FallWeather");
		fallWeather.SetActive (false);
		fallMusicBoxObject = GameObject.Find ("FallMusicBox");
		springMusicBoxObject = GameObject.Find ("SpringMusicBox");
		summerMusicBoxObject = GameObject.Find ("SummerMusicBox");
		winterMusicBoxObject = GameObject.Find ("WinterMusicBox");
		nightMusicBoxObject = GameObject.Find ("NightMusicBox");
		springMusicBoxScript = springMusicBoxObject.GetComponent<SpringMusicBoxScript> ();
		summerMusicBoxScript = summerMusicBoxObject.GetComponent<SpringMusicBoxScript> ();
		fallMusicBoxScript = fallMusicBoxObject.GetComponent<SpringMusicBoxScript> ();
		winterMusicBoxScript = winterMusicBoxObject.GetComponent<SpringMusicBoxScript> ();
		nightMusicBoxScript = nightMusicBoxObject.GetComponent<SpringMusicBoxScript> ();

		//find advisors
		guardScript = GameObject.Find ("Guard").GetComponent<GuardScript>();
		witchScript = GameObject.Find ("Witch").GetComponent<WitchScript>();

		//find notification text
		notificationText = GameObject.Find ("NotificationText").GetComponent<Text> ();
		notificationText.text = "";

		//Load in txt files to word lists 
		loadWordLists ();

		//find the screen fader
		screenFader = GameObject.Find ("ScreenFader");
		screenFaderScript = screenFader.gameObject.GetComponent<ScreenFaderScript> ();

		//write season text
		seasonText = GameObject.Find ("SeasonText").GetComponent<Text> ();
		seasonText.text = "spring";

		//set us to night
		isDay = false;

		//create list fo opponents
		opponentList = new List<GameObject>();

		//find the day counter in UI
		dayNumberText = GameObject.Find ("DayNumberText").GetComponent<Text>();

		//create the world with opponents, player, and decorations (cursor too)
		CreateWorld ();

		//establish reference to well
		well = GameObject.FindGameObjectWithTag("Well");
		wellScript = well.GetComponent<WellScript> ();

		//establish reference to player village
		myPlayerVillage = GameObject.FindGameObjectWithTag ("PlayerVillage");
		myPlayerVillageScript = myPlayerVillage.gameObject.GetComponent<PlayerVillageScript> ();

		//Camera background
		camera.clearFlags = CameraClearFlags.SolidColor;
		destroyWordLists ();
		nightMusicBoxScript.FadeInMusic ();

		//color trees
		for (int i = 0; i < trees.Length; i++) {
			trees [i].GetComponent<Renderer>().material.color = springSummerTreeColor;
		}

		//set day number
		dayNumberText.text = "night " + dayNumber;
		origOpponents = opponents;
	}
	public int GetDayNumber(){
		return dayNumber;
	}

	public GameObject GetPlayerVillage(){
		return myPlayerVillage;
	}

	public List<GameObject> GetOpponentList(){
		return opponentList;
	}
	public int GetOriginialOpponentCount(){
		return origOpponents;
	}

	public void FadeFromBlack(){
		screenFaderScript.gameObject.SetActive (true);
		screenFaderScript.FadeFromBlack();
	}

	public void FadeToBlack(){
		screenFaderScript.gameObject.SetActive (true);
		screenFaderScript.FadeToBlack();
	}
		
	public bool IsItDay(){
		return isDay;
	}

	//change day to night and vice versa
	//run through opponent list and ensure all village take an action
	//if day, display the villages that have perished
	//if night, lower poison level of well by 1
	//water levels of all villages, and player village decrease by one
	public IEnumerator ChangeTime(){
			
			needText = false;
			springMusicBoxScript.FadeOutMusic ();
			summerMusicBoxScript.FadeOutMusic ();
			fallMusicBoxScript.FadeOutMusic ();
			winterMusicBoxScript.FadeOutMusic ();
			nightMusicBoxScript.FadeOutMusic ();

			//fadeOut;
			FadeToBlack ();

			//freeze player movement;
			Vector3 oldCursorPosition = myCursor.transform.position;
			myCursor.GetComponent<CursorScript> ().Freeze ();
			myCursor.transform.position = new Vector3 (-1000, -1000, -1000);

			//PAUSE THE GAME, changes to visuals should take place after this point
			yield return new WaitForSeconds (duration);
			
			notificationText.text = "";
			wellScript.ShootNotification ();

			//notification text
			tempText = "";
			
			if (myPlayerVillageScript.GetPlayerVisit ()) {
				if (isDay) {
					tempText += "while gathering water, agents from the following\nvillages were spotted:\n ";
				} else {
				tempText += "while poisoning the well, agents from the\nfollowing villages were spotted:\n ";
				}
			}
			if (dayNumber == 0 && !isDay) {
				OpponentsMakeDecisionsNight ();
			}
			if (isDay) {
				CommitDecisionsDay ();
			} else if (!isDay) {
				CommitDecisionsNight ();
			}
			
			FadeFromBlack ();
			

			//update dayNumber, bring the sun back up, etc.
			if (!isDay) {
				dayNumber++;
				dayNumberSeason++;
			if (dayNumberSeason >= seasonLength*4) {
					dayNumberSeason = 0;
				}
			}
			//update day text
			if (!isDay) {
				dayNumberText.text = "day " + dayNumber;
			} else {
				dayNumberText.text = "night " + dayNumber;
			}

		//NOW OPERATING ON NEXT STAGE
		isDay = !isDay;

		//consume water before updating population according to growth
		if (!isDay) {
			foreach (GameObject opponent in opponentList) {
				VillageScript opponentScript = opponent.GetComponent<VillageScript> ();
				opponentScript.ConsumeWater ();
				opponentScript.UpdatePopulation ();
			}
			//player consume water
			myPlayerVillageScript.ConsumeWater ();
			myPlayerVillageScript.UpdatePopulation ();
		}
		//remove destroyed villages from the list
		for (int j = opponentList.Count - 1; j > -1; --j) {
			if (opponentList [j].GetComponent<VillageScript> ().GetPopulation()<20) {
				//add in proper death later
				needText = true;
				//tempText += opponentList[j].GetComponent<VillageScript>().GetName() + " has perished.\n"; 

				//Destroy (opponentList [j]);
				opponentList[j].GetComponent<VillageScript>().ActiviateDeathSprite();
				opponentList.RemoveAt(j);

				opponents--;
			}
		} 

			//advisors grab stuff here, poison chance
			if (isDay) {
				if (witchScript.ReadTheStars () > Random.Range (0, 100)) {
					guardScript.GetRealInfo ();
				} else {
					guardScript.GetFalseInfo ();
				}
				witchScript.UpdateWitchText ();
			}

			//villages make decision for next day
			if (isDay) {
				OpponentsMakeDecisionsDay ();
			} else if (!isDay) {
				OpponentsMakeDecisionsNight ();
				wellScript.processPoison ();
			}


			//advisors grab stuff here, drink chance
			if (!isDay) {
				if (witchScript.ReadTheStars () > Random.Range (0, 100)) {
					guardScript.GetRealInfo ();
				} else {
					guardScript.GetRealInfo ();
					guardScript.GetFalseInfo ();
				}
				witchScript.UpdateWitchText ();
			}
			
			//change background color according to season
			if (isDay) {
			if (dayNumberSeason / seasonLength == 0 || dayNumberSeason <= seasonLength) {
					camera.backgroundColor = springColor;
					springWeather.SetActive (true);
					winterWeather.SetActive (false);	
					springMusicBoxScript.FadeInMusic ();
					seasonText.text = "spring";
					for (int i = 0; i < trees.Length; i++) {
						trees [i].GetComponent<Renderer> ().material.color = springSummerTreeColor;
					}
			} else if ((dayNumberSeason) / seasonLength == 1) {
					summerWeather.SetActive (true);
					winterWeather.SetActive (false);	
					summerMusicBoxScript.FadeInMusic ();
					camera.backgroundColor = summerColor;
					seasonText.text = "summer";
					for (int i = 0; i < trees.Length; i++) {
						trees [i].GetComponent<Renderer> ().material.color = springSummerTreeColor;
					}
			} else if ((dayNumberSeason) / seasonLength == 2) {
					camera.backgroundColor = fallColor;
					winterWeather.SetActive (false);	
					seasonText.text = "fall";
					fallMusicBoxScript.FadeInMusic ();
					fallWeather.SetActive (true);
					for (int i = 0; i < trees.Length; i++) {
						trees [i].GetComponent<Renderer> ().material.color = fallTreeColor;
					}
			} else if ((dayNumberSeason) / seasonLength == 3) {
					camera.backgroundColor = winterColor;
					winterWeather.SetActive (false);	
					seasonText.text = "winter";
					winterWeather.SetActive (true);
					winterMusicBoxScript.FadeInMusic ();
					for (int i = 0; i < trees.Length; i++) {
						trees [i].GetComponent<Renderer> ().material.color = winterTreeColor;
					}
				}
			} else {
				springWeather.SetActive (false);
				summerWeather.SetActive (false);
				fallWeather.SetActive (false);
				nightMusicBoxScript.FadeInMusic ();
				camera.backgroundColor = nightColor;
			}
			if (opponents < 1) {
				SceneManager.LoadScene ("Victory");
			}
			myPlayerVillageScript.SetPlayerVisit (false);
			notificationText.text += tempText;
			
		//restore player movement
		myCursor.GetComponent<CursorScript> ().UnFreeze ();
		myCursor.transform.position = oldCursorPosition;
		//check for dying of thirst in opponentList
		if (!isDay) {
			for (int i = opponentList.Count - 1; i > -1; --i) {
				if (opponentList [i].GetComponent<VillageScript> ().GetWaterLevel () < 20) {
					//add in proper death later
					opponentList [i].GetComponent<VillageScript> ().MarkForDeath ();
				}
			} 
			//check Decimation in Player
			if (!isDay) {
				if (myPlayerVillageScript.GetPopulation ()<20){
					//add proper death later
					SceneManager.LoadScene ("GameOver"); 
				}
			}
		}
		//update death toll
		deathTollText.text = deathToll + " have died";
		numberOfOpponentsPoisoned = 0;
		for (int i = 0; i < opponentList.Count; ++i) {
			if (opponentList [i].GetComponent<VillageScript> ().GetGro () < 0) {
				numberOfOpponentsPoisoned++;
			}
		}
		if (myPlayerVillageScript.GetGro () < 0) {
			numberOfOpponentsPoisoned++;
		}
	}
	void OpponentsMakeDecisionsDay(){

		//create probability of taking action in each agent
		for (int i = 0; i < opponentList.Count; ++i) {
			VillageScript opponentScript = opponentList [i].GetComponent<VillageScript> ();
			opponentScript.MakeBufferedDecision ();
		}
	}

	private void CommitDecisionsDay(){
		needText = false;
		//execute decisions simultaneously
		for (int i = 0; i < opponentList.Count; ++i) {
			VillageScript opponentScript = opponentList [i].GetComponent<VillageScript> ();
			opponentScript.swapProbabilities ();
			int randomInt = Random.Range (0, 100);
			//Debug.Log(opponentScript.GetNextDecisionProbability () + " | " + randomInt);
			if (opponentScript.GetCurrentDecisionProbability () > randomInt) {
				opponentScript.VillageCollectsWater ();
				if (myPlayerVillageScript.GetPlayerVisit ()) {
					needText = true;
					tempText += "\t-" + opponentScript.GetName () + "\n"; 
				}
			}
		}



		if (!needText) {
			tempText = "";
		}
	}

	private void CommitDecisionsNight(){
		//Debug.Log ("Poison Level is: " + wellScript.GetPoisonLevel());
		//execute decisions simultaneously
		for (int i = 0; i < opponentList.Count; ++i) {
			VillageScript opponentScript = opponentList [i].GetComponent<VillageScript> ();
			opponentScript.swapProbabilities ();			
			int randomInt = Random.Range (0, 100);
			//Debug.Log ("Decision made with: " + randomInt);
			//Debug.Log(opponentScript.GetNextDecisionProbability () + " | " + randomInt);
			if (opponentScript.GetCurrentDecisionProbability () > randomInt) {
				wellScript.PoisonWell();
				//opponentScript.ConsumeWater ();
				if (myPlayerVillageScript.GetPlayerVisit ()) {
					needText = true;
					tempText += "\t-" + opponentScript.GetName () + "\n";
				}
			}
		}

		if (!needText) {
			tempText = "";
		}


	}

	void OpponentsMakeDecisionsNight(){
		bool needText = false;
		for (int i = 0; i < opponentList.Count; ++i) {
			VillageScript opponentScript = opponentList [i].GetComponent<VillageScript> ();
			opponentScript.MakeBufferedDecision ();
		}
	}

	private void loadWordLists(){

		TextAsset villageNounsAsset = Resources.Load ("Words/village_nouns") as TextAsset;
		TextAsset nounsAsset = Resources.Load ("Words/nouns") as TextAsset;
		TextAsset adjectivesAsset = Resources.Load ("Words/adj") as TextAsset;
		TextAsset namesAsset = Resources.Load ("Words/all_names") as TextAsset;
		TextAsset maleNamesAsset = Resources.Load ("Words/male_names") as TextAsset;
		TextAsset femaleNamesAsset = Resources.Load ("Words/female_names") as TextAsset;
		TextAsset lastNamesAsset = Resources.Load ("Words/last_names") as TextAsset;

		char[] archDelim = new char[] { '\r', '\n' };

		villageNounArr = villageNounsAsset.text.Split(archDelim, System.StringSplitOptions.RemoveEmptyEntries); 
		nounArr = nounsAsset.text.Split(archDelim, System.StringSplitOptions.RemoveEmptyEntries); 
		adjectiveArr = adjectivesAsset.text.Split(archDelim, System.StringSplitOptions.RemoveEmptyEntries); 
		namesArr = namesAsset.text.Split(archDelim, System.StringSplitOptions.RemoveEmptyEntries); 
		maleNamesArr = maleNamesAsset.text.Split(archDelim, System.StringSplitOptions.RemoveEmptyEntries); 
		femaleNamesArr = femaleNamesAsset.text.Split(archDelim, System.StringSplitOptions.RemoveEmptyEntries); 
		lastNamesArr = lastNamesAsset.text.Split(archDelim, System.StringSplitOptions.RemoveEmptyEntries); 



	}

	//destroys huge word lists
	private void destroyWordLists(){

		namesArr = new string[0];
		nounArr = new string[0];
		adjectiveArr = new string[0];
		villageNounArr = new string[0];
		maleNamesArr = new string[0];
		femaleNamesArr = new string[0];
		lastNamesArr = new string[0];

		Resources.UnloadUnusedAssets ();
	}

	private string RandomNoun(){
		return nounArr [Random.Range (0, nounArr.Length)];
	}
	private string RandomName(){
		return namesArr [Random.Range (0, namesArr.Length)];
	}
	private string RandomAdjective(){
		return adjectiveArr [Random.Range (0, adjectiveArr.Length)];
	}
	private string RandomVillageNoun(){
		return villageNounArr [Random.Range (0, villageNounArr.Length)];
	}
	private string RandomMaleName(){
		return maleNamesArr [Random.Range (0, maleNamesArr.Length)];
	}
	private string RandomFemaleName(){
		return femaleNamesArr [Random.Range (0, femaleNamesArr.Length)];
	}
	private string RandomLastName(){
		return lastNamesArr [Random.Range (0, lastNamesArr.Length)];
	}

	private string GenerateLeaderName(){
		int x = Random.Range (0, 100);
		if (x <= 50) {
			return RandomFemaleName () + " " + RandomLastName();
		} else {
			return RandomMaleName () + " " + RandomLastName ();
		}
	}

	private string GenerateVillageName(){
		string returnString = "";
		int format = Random.Range (0, 16);

		//add prefix
		int prefix = Random.Range (0, 2);
		if (prefix < 1) {
			returnString += "";
		} else if (prefix < 2) {
			returnString += "The ";
		}


		if (format < 1) {
			returnString = RandomName () + "'s " + RandomVillageNoun ();
		} else if (format < 2) {
			returnString += RandomAdjective () + " " + RandomNoun () + " " + RandomVillageNoun ();
		} else if (format < 3) {
			returnString += RandomNoun () + " of " + RandomName ();
		} else if (format < 4) {
			returnString += RandomAdjective () + " " + RandomVillageNoun ();
		} else if (format < 5) {
			returnString += RandomNoun () + " " + RandomAdjective ();
		} else if (format < 6) {
			returnString += RandomVillageNoun () + " of " + RandomNoun ();
		} else if (format < 7) {
			string noun1 = RandomNoun();
			string noun2 = RandomNoun ();
			if (noun1.Substring (noun1.Length - 1, 1) == "y") {
				noun1 = noun1.Substring(0, noun1.Length-1) + "ies";
			} else if (noun1.Substring (noun1.Length - 1, 1) != "s") {
				noun1 = noun1 + "s";
			}
			if (noun2.Substring (noun2.Length - 1, 1) == "y") {
				noun2 = noun2.Substring(0, noun2.Length-1) + "ies";
			} else if (noun2.Substring (noun2.Length - 1, 1) != "s") {
				noun2 = noun2 + "s";
			}

			returnString += RandomVillageNoun() + " of " + noun1 + " and " + noun2;
		} else if (format < 8) {
			returnString += RandomAdjective () + " " + RandomAdjective ();
		} else if (format < 9) {
			returnString += RandomNoun ();
		} else if (format < 10) {
			returnString = RandomAdjective () + " " + RandomVillageNoun () + " of " + RandomName ();
		} else if (format < 11) {
			returnString = RandomNoun ();
			int postFix = Random.Range (0, 4);
			if (postFix < 1) {
				returnString += "ville";
			} else if (postFix < 2) {
				returnString += "ton";
			} else if (postFix < 3) {
				returnString = "Santa " + returnString;
			} else if (postFix < 4) {
				returnString += "ham";
			}
		} else if (format < 12) {
			returnString = RandomName ();
		} else if (format < 13) {
			returnString += RandomAdjective ();
		} else if (format < 14) {
			returnString += RandomVillageNoun () + " of " + RandomName (); 
		} else if (format < 15) {
			returnString += RandomAdjective () + " " + RandomVillageNoun ();
		} else if (format < 16) {
			returnString = RandomAdjective () + " " + RandomVillageNoun () + " of " + RandomNoun ();
		}else {
			returnString += RandomAdjective () + " " + RandomNoun () + " " + RandomVillageNoun ();
		}

		return returnString;

	}

	public int GetNumberOfVillagesPoisoned(){
		return numberOfOpponentsPoisoned;
	}

	void CreateWorld(){
		//offset for nice looking tile
		float offset = -.60f;
		List<Vector3> freeCoordinates = new List<Vector3> ();
		List<Vector3> freeTileCoordinates = new List<Vector3> ();
		//create coordinate list
		for (int i = 0; i < 7; ++i) {
			for (int j = 0; j < 7; ++j) {
				if (i != 3 || j != 3) {
					freeCoordinates.Add (new Vector3 (i+offset, j - 3, 0));
				}
				freeTileCoordinates.Add (new Vector3 (i+offset, j - 3, 1));
			}
		}

		//shuffle list
		for (int i = 0; i < freeCoordinates.Count; i++) {
			Vector3 temp = freeCoordinates [i];
			int randomIndex = Random.Range (0, freeCoordinates.Count);
			freeCoordinates [i] = freeCoordinates [randomIndex];
			freeCoordinates [randomIndex] = temp;
		}

		//GRAB ALL OF THE PREFABS
		int[] villageSpritesInUse = new int[villageSprites.Length];
		GameObject shrine = Resources.Load<GameObject> ("Prefabs/Environment/Shrine");
		GameObject graves = Resources.Load<GameObject> ("Prefabs/Environment/Graves");
		GameObject forrest = Resources.Load<GameObject> ("Prefabs/Environment/Forrest");
		GameObject ruins = Resources.Load<GameObject> ("Prefabs/Environment/Ruins");
		GameObject tile = Resources.Load<GameObject> ("Prefabs/Tile");
		GameObject playerVillage = Resources.Load<GameObject> ("Prefabs/PlayerVillage");
		GameObject village = Resources.Load<GameObject> ("Prefabs/AI/Village");
		GameObject cursor = Resources.Load<GameObject> ("Prefabs/Cursor");
		GameObject goddessTree = Resources.Load<GameObject> ("Prefabs/Environment/GaiaTree");
		GameObject shrineYard = Resources.Load<GameObject> ("Prefabs/Environment/ShrineYard");
		GameObject umbralShard = Resources.Load<GameObject> ("Prefabs/Environment/UmbralShard");
		GameObject plain = Resources.Load<GameObject> ("Prefabs/Environment/Plains");
		GameObject plainEnhancement = Resources.Load<GameObject> ("Prefabs/Environment/PlainEnhancement");

		//only as many opponents as sprites
		if (opponents > villageSprites.Length) {
			opponents = villageSprites.Length - 1;
		}
		//can't have negative opponents
		if (opponents < 0) {
			opponents = 0;
		}

		//Create Tiles
		mapOccupation [3, 3] = 1;
		for (float y = 0; y < 7; y += 1) {
			for (float x = 0; x < 7; x += 1) {
				//Instantiate (tile, new Vector3 (x, y - 3, 0), Quaternion.identity);
			}
		}
		for (int i = 0; i < freeTileCoordinates.Count; i++) {
			Instantiate (tile, freeTileCoordinates[i], Quaternion.identity);
		}
			
		//spawn player
		int spriteSelect = Random.Range (0, villageSprites.Length);
		myPlayerVillage = Instantiate (playerVillage, freeCoordinates [0], Quaternion.identity) as GameObject;
		villageSpritesInUse [spriteSelect] = 1;
		myPlayerVillage.GetComponent<SpriteRenderer> ().sprite = villageSprites [spriteSelect];
	    myPlayerVillage.GetComponent<PlayerVillageScript> ().SetLeaderName (GenerateLeaderName ());
		myPlayerVillage.GetComponent<PlayerVillageScript> ().SetName (GenerateVillageName ());

		//spawn cursor
		myCursor = Instantiate (cursor, new Vector3(freeCoordinates [0].x+.0005f,freeCoordinates [0].y+.001f,freeCoordinates [0].z) , Quaternion.identity) as GameObject;
		myCursorScript = myCursor.GetComponent<CursorScript> ();
		//FIX CAST AS INT
		myCursorScript.locX = (int)(freeCoordinates [0].x+1f);
		myCursorScript.locY = (int)(freeCoordinates [0].y + 3f);
		freeCoordinates.RemoveAt (0);

		//spawn villages
		for (int i = 0; i < opponents; ++i) {
			spriteSelect = Random.Range (0, villageSprites.Length);
			GameObject spawnVillage = Instantiate (village, freeCoordinates [0], Quaternion.identity) as GameObject;

			bool foundNewSprite = false;
			while(!foundNewSprite){
				spriteSelect = Random.Range (0, villageSprites.Length);
				if(villageSpritesInUse[spriteSelect] != 1){
					foundNewSprite = true;
					villageSpritesInUse [spriteSelect]=1;
				}
			}
			//assign sprite
			spawnVillage.GetComponent<SpriteRenderer> ().sprite = villageSprites [spriteSelect];

			//assign random leader name
			spawnVillage.gameObject.GetComponent<VillageScript> ().SetLeaderName (GenerateLeaderName ());

			//assign random name
			spawnVillage.gameObject.GetComponent<VillageScript> ().SetName (GenerateVillageName ());
			villageSpritesInUse [spriteSelect] = 1;
			opponentList.Add (spawnVillage);
			freeCoordinates.RemoveAt (0);
		}


		//spawn forests
		int forrestNumber = Random.Range (8, 12);
		for (int i = 0; i < forrestNumber; ++i) {
			GameObject spawnForrest = Instantiate (forrest, freeCoordinates [0], Quaternion.identity) as GameObject;
			freeCoordinates.RemoveAt (0);
		}
		trees = GameObject.FindGameObjectsWithTag("Trees");

		//spawn graveyard
		int gravesNumber = Random.Range (2, 4);
		for (int i = 0; i < gravesNumber; ++i) {
			GameObject spawnGraves = Instantiate (graves, freeCoordinates [0], Quaternion.identity) as GameObject;
			freeCoordinates.RemoveAt (0);
		}
			
		//spawn shrine
		GameObject spawnShrine = Instantiate (shrine, freeCoordinates[0], Quaternion.identity) as GameObject;
		freeCoordinates.RemoveAt (0);

		//spawn ruins
		int ruinsNumber = Random.Range (8,12);
		for (int i = 0; i < ruinsNumber; ++i) {
			GameObject spawnRuins = Instantiate (ruins, freeCoordinates [0], Quaternion.identity) as GameObject;
			freeCoordinates.RemoveAt (0);
		}
			
		//spawn goddess tree
		GameObject spawnGoddessTree = Instantiate (goddessTree, freeCoordinates[0], Quaternion.identity) as GameObject;
		freeCoordinates.RemoveAt (0);

		//spawn shrine yard
		int shrineYardNumber = Random.Range (1,3);
		for (int i = 0; i < shrineYardNumber; ++i) {
			GameObject spawnShrineYard = Instantiate (shrineYard, freeCoordinates[0], Quaternion.identity) as GameObject;
			freeCoordinates.RemoveAt (0);
		}
			
		//spawn umbral shard
		GameObject spawnUmbralShard = Instantiate (umbralShard, freeCoordinates[0], Quaternion.identity) as GameObject;
		freeCoordinates.RemoveAt (0);
	
		//fill remaining squares with plains
		for (int i = 0; i < freeCoordinates.Count; ++i) {
			GameObject spawnPlains = Instantiate (plain, freeCoordinates [i], Quaternion.identity) as GameObject;
			GameObject spawnPlainEnhancement = Instantiate (plainEnhancement, freeCoordinates [i], Quaternion.identity) as GameObject;
		}

	}
}