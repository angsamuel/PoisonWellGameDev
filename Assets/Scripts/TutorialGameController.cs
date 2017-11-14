using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;

public class TutorialGameController : MonoBehaviour {
	private List<GameObject> opponentList;

	private GameObject[] trees;

	public Sprite[] villageSprites;
	private int[,] mapOccupation = new int[3,3];
	int opponents = 1;

	//player village
	private GameObject myPlayerVillage;
	private PlayerVillageScript myPlayerVillageScript;
	//cursor Object
	private GameObject myCursor; 
	private CursorScript myCursorScript;


	// Use this for initialization
	void Start () {
		CreateTutorialWorld ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void CreateTutorialWorld(){
		float offset = -.60f;
		List<Vector3> freeCoordinates = new List<Vector3> ();
		List<Vector3> freeTileCoordinates = new List<Vector3> ();
		//create coordinate list
		for (int i = 0; i < 3; ++i) {
			for (int j = 0; j < 3; ++j) {
				if (i != 1 || j != 1) {
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

		if (opponents > villageSprites.Length) {
			opponents = villageSprites.Length - 1;
		}
		if (opponents < 0) {
			opponents = 0;
		}

		//Create Tiles
		mapOccupation [1, 1] = 1;

		for (int i = 0; i < freeTileCoordinates.Count; i++) {
			Instantiate (tile, freeTileCoordinates[i], Quaternion.identity);
		}

		//spawn player
		int spriteSelect = Random.Range (0, villageSprites.Length);
		myPlayerVillage = Instantiate (playerVillage, freeCoordinates [0], Quaternion.identity) as GameObject;
		villageSpritesInUse [spriteSelect] = 1;
		myPlayerVillage.GetComponent<SpriteRenderer> ().sprite = villageSprites [spriteSelect];
		myPlayerVillage.GetComponent<PlayerVillageScript> ().SetName ("");

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

			spawnVillage.GetComponent<SpriteRenderer> ().sprite = villageSprites [spriteSelect];

			//assign random name
			spawnVillage.gameObject.GetComponent<VillageScript> ().SetName ("(((them)))");
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

		//find all trees for changing color during season
	}
}
