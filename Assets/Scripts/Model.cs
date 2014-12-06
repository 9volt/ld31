using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Model : MonoBehaviour {
	Level level;
	Slider bossHealth;
	Slider partyHealth;

	GameObject p1;
	GameObject p2;
	GameObject p3;
	GameObject p4;

	GameObject b1;
	GameObject b2;
	GameObject b3;
	GameObject b4;


	Person man = new Person("sword_man", 3, 6, "strong_sword");
	Person boy = new Person("heal_boy", 1, 0, "heal");
	Person witch = new Person("witch", 4, 0, "chain_break");
	Person archer = new Person("archer", 3, 2, "interrupt");

	// Use this for initialization
	void Start () {
		level = new Level();
		bossHealth = GameObject.FindGameObjectWithTag("BossHealth").GetComponent<Slider>();
		partyHealth = GameObject.FindGameObjectWithTag("PartyHealth").GetComponent<Slider>();

		p1 = GameObject.FindGameObjectWithTag("p1");
		p1.GetComponent<PartyMember>().me = man;
		p2 = GameObject.FindGameObjectWithTag("p2");
		p2.GetComponent<PartyMember>().me = boy;
		p3 = GameObject.FindGameObjectWithTag("p3");
		p3.GetComponent<PartyMember>().me = witch;
		p4 = GameObject.FindGameObjectWithTag("p4");
		p4.GetComponent<PartyMember>().me = archer;

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

public class Attack {
	public float max_percent;
	public float min_percent;
	public float chance;
	public int priority;
	public int damage;
	public string status;

	public Attack(float max, float min, float cha, int prio, int dam, string sta){
		max_percent = max;
		min_percent = min;
		chance = cha;
		priority = prio;
		damage = dam;
		status = sta;
	}
}

public class Person {
	string name;
	int basic_attack;
	int special_attack;
	string special_attack_type;

	public Person(string n, int ba, int sa, string sat){
		name = n;
		basic_attack = ba;
		special_attack = sa;
		special_attack_type = sat;
	}
}