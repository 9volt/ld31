using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

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


	Person man = new Person("sword_man", 3, 6, "strong_sword", 3);
	Person boy = new Person("heal_boy", 1, 0, "heal", 2);
	Person witch = new Person("witch", 4, 0, "chain_break", 5);
	Person archer = new Person("archer", 3, 2, "interrupt", 2);

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

	int HandleAttack(GameObject p){
		string attack_type = p.GetComponent<ToggleGroup>().GetActive().name;
		Person person = p.GetComponent<PartyMember>().me;
		Debug.Log(attack_type);
		int damage = 0;
		// Lower delay
		if(person.current_delay > 0) {
			person.current_delay--;
		}
		// Determine attack type
		if(attack_type == "Attack") {
			damage = person.basic_attack;
		} else if(attack_type == "Special") {
			damage = person.special_attack;
			switch(person.special_attack_type){
			case "heal":
				Debug.Log("Heal");
				level.DamagePlayers(-25);
				break;
			default:
				Debug.Log("Special");
				break;
			}
			person.current_delay = person.delay;
		}
		return damage;
	}

	public void Execute(){
		Debug.Log("Executing");
		int damage = 0;
		damage += HandleAttack(p1);
		damage += HandleAttack(p2);
		damage += HandleAttack(p3);
		damage += HandleAttack(p4);
		level.DamageBoss(damage);
		level.DamagePlayers(5);
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
		total_boss_health = 100;
		current_boss_health = 100;
		total_party_health = 50;
		current_party_health = 50;
	}

	public void DamageBoss(int damage){
		current_boss_health -= damage;
	}

	public void DamagePlayers(int damage){
		current_party_health -= damage;
		if(current_party_health > total_party_health){
			current_party_health = total_party_health;
		}
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
	public string name;
	public int basic_attack;
	public int special_attack;
	public string special_attack_type;
	public int delay;
	public int current_delay = 0;

	public Person(string n, int ba, int sa, string sat, int d){
		name = n;
		basic_attack = ba;
		special_attack = sa;
		special_attack_type = sat;
		delay = d;
	}
}

// Thank you http://answers.unity3d.com/questions/809412/is-there-a-better-way-to-access-the-single-active.html
public static class ToggleGroupExtension{
	public static Toggle GetActive(this ToggleGroup aGroup){
		return aGroup.ActiveToggles().FirstOrDefault();
	}
}