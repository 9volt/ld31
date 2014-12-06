﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class Model : MonoBehaviour {
	Level level;
	public int floor_num;
	public Slider bossHealth;
	public Slider partyHealth;
	public Text boss_cooldown;
	public Text win_text;
	public Image boss_image;

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
	public List<Person> party_members;

	// Use this for initialization
	void Start () {
		PopulateFloors();

		p1.GetComponent<PartyMember>().me = man;
		p2.GetComponent<PartyMember>().me = boy;
		p3.GetComponent<PartyMember>().me = witch;
		p4.GetComponent<PartyMember>().me = archer;

		party_members = new List<Person>();
		party_members.Add(man);
		party_members.Add(boy);
		party_members.Add(witch);
		party_members.Add(archer);

		LoadFloor(floor_num);
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
		if(level.current_party_health <= 0){
			win_text.text = "YOU LOSE :(";
			win_text.gameObject.SetActive(true);
		}
	}

	public void Restart(){
		win_text.gameObject.SetActive(false);
		floor_num = 0;
		LoadFloor(floor_num);
	}

	void LoadFloor(int num){
		if(floor_num >= floors.Count) {
			win_text.text = "YOU WIN!!";
			win_text.gameObject.SetActive(true);
			return;
		}
		level = new Level(floors[num]);

		Sprite boss_sprite = Resources.Load<Sprite>(level.floor.name);
		boss_image.sprite = boss_sprite;

		// reset party cooldowns
		foreach(Person p in party_members){
			p.current_delay = 0;
		}
		// reset boss cooldowns
		foreach(Attack a in level.floor.attacks){
			a.cur_cooldown = 0;
		}

		bossHealth.maxValue = level.total_boss_health;
		partyHealth.maxValue = level.total_party_health;
	}

	void PopulateFloors(){
		floors = new List<Floor>();
		// Declare Floors and Boss Attacks
		Floor f = new Floor("trash1_lizard", 50);
		f.AddAttack(new Attack(.25f, 0.0f, 1.0f, 1, 20, "delayed attack", 1, 5));
		f.AddAttack(new Attack(0.5f, 0.0f, 0.5f, 2, 15, "strong attack", 0, 1));
		f.AddAttack(new Attack(1.0f, 0.0f, 1.0f, 5, 5, "basic attack", 0, 0));
		floors.Add(f);
		f = new Floor("boss1_lizardperson", 100);
		f.AddAttack(new Attack(.25f, 0.0f, 1.0f, 1, 50, "delayed attack", 2, 5));
		f.AddAttack(new Attack(0.5f, 0.0f, 0.5f, 2, 35, "strong attack", 0, 1));
		f.AddAttack(new Attack(1.0f, 0.0f, 1.0f, 5, 5, "basic attack", 0, 0));
		floors.Add(f);
		f = new Floor("trash2_mimic", 100);
		f.AddAttack(new Attack(.25f, 0.0f, 1.0f, 1, 50, "delayed attack", 2, 5));
		f.AddAttack(new Attack(0.5f, 0.0f, 0.5f, 2, 35, "strong attack", 0, 1));
		f.AddAttack(new Attack(1.0f, 0.0f, 1.0f, 5, 5, "basic attack", 0, 0));
		floors.Add(f);
		f = new Floor("boss2_bandit", 100);
		f.AddAttack(new Attack(.25f, 0.0f, 1.0f, 1, 50, "delayed attack", 2, 5));
		f.AddAttack(new Attack(0.5f, 0.0f, 0.5f, 2, 35, "strong attack", 0, 1));
		f.AddAttack(new Attack(1.0f, 0.0f, 1.0f, 5, 5, "basic attack", 0, 0));
		floors.Add(f);
		f = new Floor("trash3_snakes", 100);
		f.AddAttack(new Attack(.25f, 0.0f, 1.0f, 1, 50, "delayed attack", 2, 5));
		f.AddAttack(new Attack(0.5f, 0.0f, 0.5f, 2, 35, "strong attack", 0, 1));
		f.AddAttack(new Attack(1.0f, 0.0f, 1.0f, 5, 5, "basic attack", 0, 0));
		floors.Add(f);
		f = new Floor("boss3_snake_gorgon_girl", 100);
		f.AddAttack(new Attack(.25f, 0.0f, 1.0f, 1, 50, "delayed attack", 2, 5));
		f.AddAttack(new Attack(0.5f, 0.0f, 0.5f, 2, 35, "strong attack", 0, 1));
		f.AddAttack(new Attack(1.0f, 0.0f, 1.0f, 5, 5, "basic attack", 0, 0));
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
		if(level.current_party_health > 0){
			int damage = 0;
			damage += HandleAttack(p1);
			damage += HandleAttack(p2);
			damage += HandleAttack(p3);
			damage += HandleAttack(p4);
			level.DamageBoss(damage);
			if(level.current_boss_health <= 0){
				floor_num++;
				LoadFloor(floor_num);
			}
			level.BossAttack();
		}
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
				float cur_percent = (float)current_boss_health / (float)total_boss_health;
				float rand = Random.value;
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
	public List<Attack> attacks;
	public string name;
	public int health;

	public Floor(string n, int h){
		name = n;
		health = h;
		attacks = new List<Attack>();
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
	public float max_percent;
	public float min_percent;
	public float chance;
	public int priority;
	public int damage;
	public string status;
	public int delay;
	public int cooldown;
	public int cur_cooldown = 0;

	public Attack(float max, float min, float cha, int prio, int dam, string sta, int del, int cool){
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