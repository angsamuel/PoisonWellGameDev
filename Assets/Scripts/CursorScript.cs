using UnityEngine;
using System.Collections;

public class CursorScript : MonoBehaviour {
	private float soundLevel = .18f;

	//blink variable
	private float blinkVariable;
	private float moveBlinkPause;
	private float blinkFreq;

	//movement
	public float speed;
	public int locX;
	public int locY;

	//locks for directional movement
	private bool canMoveUp;
	private bool canMoveDown;
	private bool canMoveRight;
	private bool canMoveLeft;

	//lock for all movement
	private bool frozen;

	public AudioSource moveDefaultSound;
	private AudioClip myAudioClip;
	
	// Use this for initialization
	void Start () {
		moveDefaultSound = gameObject.GetComponent<AudioSource> ();
		myAudioClip = (AudioClip)Resources.Load<AudioClip> ("Sound/select");
		canMoveUp = true;
		canMoveDown = true;
		frozen = false;
		moveBlinkPause = -.1f;
		blinkVariable = 0;
		blinkFreq = 0.4f;
	}
	
	// Update is called once per frame
	void Update () {
		//blinking
		blinkVariable += Time.deltaTime;
			if (blinkVariable > blinkFreq) {
				CursorAnimateSwap ();
				blinkVariable = 0;
			}

		//Movement
		if (Input.GetAxisRaw("Up") != 0 && locY < 6 && canMoveUp) {
			transform.Translate (0, 1, 0, Space.World);
			++locY;
			canMoveUp = false;
			GetComponent<SpriteRenderer>().enabled = true;
			blinkVariable = moveBlinkPause;
			if (myAudioClip == null) {
				Debug.Log ("clip should not play");
			}

			moveDefaultSound.PlayOneShot (myAudioClip, soundLevel);
		}
		
		if (Input.GetAxisRaw("Down") != 0 && locY > 0 && canMoveDown) {
			transform.Translate (0, -1, 0, Space.World);
			--locY;
			canMoveDown = false;
			GetComponent<SpriteRenderer>().enabled = true;
			blinkVariable = moveBlinkPause;
			moveDefaultSound.PlayOneShot (myAudioClip, soundLevel);
		}

		if (Input.GetAxisRaw("Right") != 0 && locX < 6 && canMoveRight) {
			transform.Translate (1, 0, 0, Space.World);
			++locX;
			canMoveRight = false;
			GetComponent<SpriteRenderer>().enabled = true;
			blinkVariable = moveBlinkPause;
			moveDefaultSound.PlayOneShot (myAudioClip, soundLevel);
		}
		if (Input.GetAxisRaw("Left") != 0 && locX > 0 && canMoveLeft) {
			transform.Translate (-1, 0, 0, Space.World);
			--locX;
			canMoveLeft = false;
			GetComponent<SpriteRenderer>().enabled = true;
			blinkVariable = moveBlinkPause;
			moveDefaultSound.PlayOneShot (myAudioClip, soundLevel);
		}
		if (!frozen) {
			if (Input.GetAxisRaw ("Up") == 0) {
				canMoveUp = true;
			}
			if (Input.GetAxisRaw ("Down") == 0) {
				canMoveDown = true;
			}
			if (Input.GetAxisRaw ("Right") == 0) {
				canMoveRight = true;
			}
			if (Input.GetAxisRaw ("Left") == 0) {
				canMoveLeft = true;
			}
		}
	}

	//blinking cursor
	void CursorAnimateSwap(){
		GetComponent<SpriteRenderer>().enabled = !GetComponent<SpriteRenderer>().enabled;
	}

	//freeze all movement
	public void Freeze(){
		canMoveUp = false;
		canMoveDown = false;
		canMoveRight = false;
		canMoveLeft = false;
		frozen = true;
	}

	//unfreeze all movement
	public void UnFreeze(){
		canMoveUp = true;
		canMoveDown = true;
		canMoveRight = true;
		canMoveLeft = true;
		frozen = false;
	}
}
