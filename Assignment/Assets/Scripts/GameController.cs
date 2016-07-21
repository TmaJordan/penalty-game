using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//Ensure AudioSource is also attached to GameObject
[RequireComponent (typeof (AudioSource))]
public class GameController : MonoBehaviour {

	//UI Elements for displaying info to the player
	public Text goalCounter;
	public Text shotCounter;
	public Text gameOverText;

	//Used for the power bar
	public Vector2 pos = new Vector2(20,10);
	public Vector2 size = new Vector2(100,20);
	public Texture2D emptyTex;
	public Texture2D fullTex;

	//Sounds used for goals and misses
	public AudioClip goalSound;
	public AudioClip missSound;

	//Make static so values persist across levels
	private static int goals = 0;
	private static int shots = 0;
	private static int level = 1;

	//Internal flags and variables
	private int shotsPerLevel = 5;
	private float powerValue;
	private string gameOverString;

	//Flags to disable further counting after game over or result
	private bool gameOver = false;
	private bool turnOver = false;

	private AudioSource source;
	
	void Start () {
		//Get AudioSource component from GameObject
		source = GetComponent<AudioSource> ();

		//Initialise game over text as invisible
		gameOverText.enabled = false;
		//Store the start value of the game over text element so I can append score to it.
		gameOverString = gameOverText.text;
		UpdateGUI ();
		Debug.Log (Application.levelCount);
	}
	
	void Update() {
		//Player has indicated restart by pressing 'r' so set initial conditions back to the beginning
		if (gameOver && Input.GetKeyDown (KeyCode.R)) {
			shots = 0;
			goals = 0;
			level = 1;
			gameOverText.enabled = false;
			gameOver = false;
			UpdateLevel();
			UpdateGUI();
		}
	}

	//Update level and score info
	void UpdateGUI() {
		//Update goal and shot UI elements
		goalCounter.text = goals + " Goals";
		shotCounter.text = shots + "/" + shotsPerLevel + " Shots";
	}
	
	void UpdateLevel() {
		if (shots == shotsPerLevel) {
			//Time to go up a level
			shots = 0;
			level++;
		}

		//Display level or game over if reached last level
		if (level > Application.levelCount) {
			//Player has reached end of levels so show game over with player score
			gameOverText.text = gameOverString + "\n" + goals + " Goals Out of " + (shotsPerLevel * Application.levelCount);
			gameOverText.enabled = true;
			gameOver = true;
			shots = shotsPerLevel;
		} 
		else if (!gameOver) {
			//Set flag to true so goals, shots and misses aren't counted multiple times in the same turn
			turnOver = true;
			//Load the level based on the format Level{number}
			//Use Coroutine as I want to implement delay
			StartCoroutine(LoadAfterWait("Level" + level));
		}
	}

	//Used to display power for kick
	public void UpdatePower(float power, float maxPower) {
		//Ensure that powerValue is between 0 & 1 so power bar renders correctly.
		powerValue = (power) / maxPower;
	}

	//Called when goal is scored
	public void ScoreGoal() {
		//Stop counting goals when game or turn is over
		if (!gameOver && !turnOver) {
			source.PlayOneShot (goalSound);
			goals++;
			shots++;
			UpdateLevel ();
			UpdateGUI ();
		}
	}

	//Called when ball hits other trigger collider
	public void Miss() {
		//Stop counting misses when game or turn is over
		if (!gameOver && !turnOver) {
			source.PlayOneShot (missSound);
			shots++;
			UpdateLevel ();
			UpdateGUI ();
		}
	}

	void OnGUI() {
		//Draw the power bar container:
		GUI.Box(new Rect((Screen.width/2) + pos.x, Screen.height - pos.y, size.x, size.y), emptyTex);
		//Draw the power bar fill
		GUI.Box(new Rect((Screen.width/2) + pos.x, Screen.height - pos.y, size.x * powerValue, size.y), fullTex);
	}

	IEnumerator LoadAfterWait(string levelName) {
		//Delay next turn for 2 seconds so sounds can play
		yield return new WaitForSeconds(2f); // wait 2 seconds
		//Set flag back to false, not sure if necessary due to reloading level but just in case
		turnOver = false;
		Application.LoadLevel(levelName);
		
	}
}
