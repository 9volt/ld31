using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class Model : MonoBehaviour {
	Level level;
	public Slider bossHealth;
	public Slider partyHealth;
	public Text boss_cooldown;

	public GameObject p1;
	public GameObject p2;
	public GameObject p3;
	public GameObject p4;

	GameObject b1;
	GameObject b2;
	GameObject b3;
	GameObject b4;


	Person man = new Person("sword_man", 3, 6, "strong_sword", 3);
	Person boy = new Person("heal_boy", 1, 0, "heal", 2);
	Person witch = new Person("witch", 4, 0, "chain_break", 5);
	Person archer = new Person("archer", 3, 2, "interrupt", 2);

	public List<Floor> floors;

	// Use this for initialization
	void Start () {
		PopulateFloors();
		level = new Level(floors[0]);

		p1.GetComponent<PartyMember>().me = man;
		p2.GetComponent<PartyMember>().me = boy;
		p3.GetComponent<PartyMember>().me = witch;
		p4.GetComponent<PartyMember>().me = archer;

		bossHealth.maxValue = level.total_boss_health;
		partyHealth.maxValue = level.total_party_health;
	}
	
	// Update is called once per frame
	void Update () {
		bossHealth.value = level.current_boss_health;
		partyHealth.value = level.current_party_health;
		if(level.current_boss_delay > 0){
			boss_cooldown.text = level.current_boss_delay.ToString();
		} else {
			boss_cooldown.text = null;
		}
	}

	void PopulateFloors(){
		floors = new List<Floor>();
		// Declare Floors and Boss Attacks
		Floor f = new Floor("test", 100);
		f.AddAttack(new Attack(.25, 0.0, 1.0, 1, 50, "delayed attack", 2, 5));
		f.AddAttack(new Attack(0.5, 0.0, 0.5, 2, 15, "strong attack", 0, 1));
		f.AddAttack(new Attack(1.0, 0.0, 1.0, 5, 5, "basic attack", 0, 0));
		floors.Add(f);
	}

	int HandleAttack(GameObject p){
		string attack_type = p.GetComponent<ToggleGroup>().GetActive().name;
		Person person = p.GetComponent<PartyMember>().me;
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
				level.DamagePlayers(-25);
				break;
			case "interrupt":
				level.current_boss_delay = 0;
				level.delayed_attack = null;
				break;
			default:
				break;
			}
			person.current_delay = person.delay;
		}
		return damage;
	}

	public void Execute(){
		int damage = 0;
		damage += HandleAttack(p1);
		damage += HandleAttack(p2);
		damage += HandleAttack(p3);
		damage += HandleAttack(p4);
		level.DamageBoss(damage);
		level.BossAttack();
	}
}


public class Level {
	public string boss_name;
	public int total_boss_health;
	public int current_boss_health;
	public int current_boss_delay;
	public Attack delayed_attack;

	public int total_party_health;
	public int current_party_health;

	public Floor floor;

	public Level(Floor f){
		boss_name = f.name;
		total_boss_health = f.health;
		current_boss_health = f.health;
		floor = f;
		total_party_health = 50;
		current_party_health = 50;
	}

	public void BossAttack(){
		if(current_boss_delay > 0){
			current_boss_delay--;
			if(current_boss_delay == 0){
				DamagePlayers(delayed_attack.damage);
				delayed_attack = null;
			}
		} else {
			foreach(Attack a in floor.attacks){
				double cur_percent = (double)current_boss_health / (double)total_boss_health;
				double rand = .2;
				if(cur_percent <= a.max_percent && cur_percent >= a.min_percent && rand < a.chance && a.cur_cooldown == 0){
					Debug.Log(a.status);
					a.cur_cooldown = a.cooldown;
					if(a.delay > 0){
						delayed_attack = a;
						current_boss_delay = a.delay;
					} else {
						DamagePlayers(a.damage);
					}
					return;
				}
			}
		}
		floor.LowerCooldowns();
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

public class Floor{
	public List<Attack> attacks = new List<Attack>();
	public string name;
	public int health;

	public Floor(string n, int h){
		name = n;
		health = h;
	}

	public void AddAttack(Attack a){
		attacks.Add(a);
		attacks = attacks.OrderBy(x => x.priority).ToList();
	}

	public void LowerCooldowns(){
		foreach(Attack a in attacks){
			if(a.cur_cooldown > 0){
				a.cur_cooldown--;
			}
		}
	}
}

public class Attack {
	public double max_percent;
	public double min_percent;
	public double chance;
	public int priority;
	public int damage;
	public string status;
	public int delay;
	public int cooldown;
	public int cur_cooldown = 0;

	public Attack(double max, double min, double cha, int prio, int dam, string sta, int del, int cool){
		max_percent = max;
		min_percent = min;
		chance = cha;
		priority = prio;
		damage = dam;
		status = sta;
		delay = del;
		cooldown = cool;
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