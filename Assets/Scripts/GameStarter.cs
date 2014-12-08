using UnityEngine;
using System.Collections;

public class GameStarter : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void GameStart(){
		PlayerPrefs.SetInt("hard_mode", 0);
		Application.LoadLevel("one_screen");
	}

	public void GameStartHard(){
		PlayerPrefs.SetInt("hard_mode", 1);
		Application.LoadLevel("one_screen");
	}
}
