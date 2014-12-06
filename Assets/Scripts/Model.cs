using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Model : MonoBehaviour {
	Level level;
	Slider bossHealth;
	Slider partyHealth;


	// Use this for initialization
	void Start () {
		level = new Level();
		bossHealth = GameObject.FindGameObjectWithTag("BossHealth").GetComponent<Slider>();
		partyHealth = GameObject.FindGameObjectWithTag("PartyHealth").GetComponent<Slider>();
		bossHealth.maxValue = level.total_boss_health;
		partyHealth.maxValue = level.total_party_health;
	}
	
	// Update is called once per frame
	void Update () {
		bossHealth.value = level.current_boss_health;
		partyHealth.value = level.current_party_health;
	}

	public void Execute(){
		Debug.Log("Executing");
		level.current_boss_health = level.current_boss_health - 1;
	}
}


public class Level {
	public string boss_name;
	public int total_boss_health;
	public int current_boss_health;

	public int total_party_health;
	public int current_party_health;

	public Level(){
		boss_name = "Test";
		total_boss_health = 5;
		current_boss_health = 4;
		total_party_health = 10;
		current_party_health = 8;
	}
}