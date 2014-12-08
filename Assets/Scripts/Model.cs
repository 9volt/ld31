using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class Model : MonoBehaviour {
	public bool god_mode;
	public int god_strength = 1000;
	public bool animating;
	public GameObject last_attacker;
	public int last_attacker_num;
	Level level;
	public int floor_num;
	public Slider bossHealth;
	public Slider partyHealth;
	public GameObject attack_button;
	public Text boss_cooldown;
	public AttackName boss_attack;
	public Text win_text;
	public Image boss_image;
	public Animator boss_animator;
	public Animator party_status_animator;
	public bool blocking = false;
	public bool poisoned = false;
	public bool time_warp = false;
	public int poison_damage = 20;
	public Image poison_icon;
	public Text level_counter;
	public Text boss_percent;
	public Text party_health;

	public GameObject normal_music;
	public GameObject boss_music;
	public GameObject victory_music;


	public GameObject p1;
	public GameObject p2;
	public GameObject p3;
	public GameObject p4;

	public GameObject b1;
	public GameObject b2;
	public GameObject b3;
	public GameObject b4;


	Person sword_man = new Person("sword_man", 3, 6, "strong_sword", 3);
	Person healer = new Person("healer", 1, 0, "heal", 2);
	Person witch = new Person("witch", 4, 0, "chain_break", 3);
	Person archer = new Person("archer", 3, 4, "interrupt", 2);
	Person mage = new Person("time_mage", 4, 0, "time_freeze", 6);
	Person druid = new Person("druid", 3, 0, "cleanse", 3);
	Person shield_man = new Person("shield_man", 3, 0, "block", 4);
	Person rogue = new Person("rogue", 4, 12, "expose_armor", 8);

	public List<Floor> floors;
	public List<Person> party_members;

	// Use this for initialization
	void Start () {
		PopulateFloors();

		p1.GetComponent<PartyMember>().SetPerson(sword_man);
		p2.GetComponent<PartyMember>().SetPerson(healer);
		p3.GetComponent<PartyMember>().SetPerson(witch);
		p4.GetComponent<PartyMember>().SetPerson(archer);

		b1.GetComponent<PartyMember>().SetPerson(mage);
		b2.GetComponent<PartyMember>().SetPerson(shield_man);
		b3.GetComponent<PartyMember>().SetPerson(druid);
		b4.GetComponent<PartyMember>().SetPerson(rogue);
		
		party_members = new List<Person>();
		party_members.Add(sword_man);
		party_members.Add(healer);
		party_members.Add(witch);
		party_members.Add(archer);
		party_members.Add(mage);
		party_members.Add(shield_man);
		party_members.Add(druid);
		party_members.Add(rogue);
		//floor_num = 4;
		LoadFloor(floor_num);
	}
	
	// Update is called once per frame
	void Update () {
		// GOd mode for debugging
		if(god_mode){
			sword_man.special_attack = god_strength;
		}
		poison_icon.enabled = poisoned;
		bossHealth.value = level.current_boss_health;
		if(level.current_boss_health <= 0){
			boss_percent.text = "0/" + level.total_boss_health.ToString() + " (0%)";
		} else {
			boss_percent.text = level.current_boss_health.ToString() + "/" + level.total_boss_health.ToString() + " (" +(((float)level.current_boss_health / (float)level.total_boss_health) * 100).ToString("0") + "%)";
		}
		partyHealth.value = level.current_party_health;

		if(level.current_party_health <= 0){
			party_health.text = "0/" + level.total_party_health.ToString();
		} else {
			party_health.text = level.current_party_health.ToString() + "/" + level.total_party_health.ToString();
		}

		if(level.current_boss_delay > 0){
			boss_cooldown.text = level.current_boss_delay.ToString();
		} else {
			boss_cooldown.text = null;
		}
		if(level.current_party_health <= 0){
			win_text.text = "Try Again :(";
			win_text.gameObject.SetActive(true);
		}

		if(animating){
			if(last_attacker.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("slide_right_p1")){
				if(last_attacker_num == 1 && p2.activeSelf){
					last_attacker = p2;
					last_attacker_num = 2;
					level.DamageBoss(HandleAttack(p2));
					last_attacker.GetComponent<Animator>().SetTrigger("attack");
				} else if(last_attacker_num == 2 && p3.activeSelf){
					last_attacker = p3;
					last_attacker_num = 3;
					level.DamageBoss(HandleAttack(p3));
					last_attacker.GetComponent<Animator>().SetTrigger("attack");
				} else if(last_attacker_num == 3 && p4.activeSelf){
					last_attacker = p4;
					last_attacker_num = 4;
					level.DamageBoss(HandleAttack(p4));
					last_attacker.GetComponent<Animator>().SetTrigger("attack");
				} else {
					if(time_warp){
						party_status_animator.SetTrigger("time_warp");
						foreach(Person pers in party_members){
							if(pers.name != "time_mage"){
								pers.current_delay = 0;
							}
						}
						time_warp = false;
					}
					if(level.current_boss_health <= 0){
						floor_num++;
						animating = false;
						LoadFloor(floor_num);
					} else {
						level.BossAttack();
					}
					if(poisoned){
						level.DamagePlayers(poison_damage);
					}
					animating = false;
					blocking = false;
				}
			}
		}
		attack_button.SetActive(!animating);
	}

	public void Restart(){
		win_text.gameObject.SetActive(false);
		if(floor_num >= floors.Count){
			floor_num = 0;
			Application.LoadLevel("one_screen");
		}
		LoadFloor(floor_num);
	}

	void LoadFloor(int num){
		level_counter.text = (num + 1).ToString () + "/" + floors.Count.ToString();
		switch(num){
		case 2:
			p2.SetActive(true);
			break;
		case 4:
			p3.SetActive(true);
			break;

		case 6:
			p4.SetActive(true);
			break;

		case 8:
			b1.SetActive(true);
			break;

		case 10:
			b2.SetActive(true);
			break;

		case 12:
			b3.SetActive(true);
			break;

		case 14:
			b4.SetActive(true);
			break;

		}
		if(num + 1 == floors.Count){
			normal_music.SetActive(false);
			boss_music.SetActive(true);
		}
		if(floor_num >= floors.Count) {
			boss_music.SetActive(false);
			victory_music.SetActive(true);
			win_text.text = "YOU WIN!!";
			win_text.gameObject.SetActive(true);
			return;
		}
		level = new Level(floors[num], this);

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

		// reset slocks
		foreach(GameObject go in GameObject.FindGameObjectsWithTag("slock")){
			go.GetComponent<slock>().Reset();
		}

		bossHealth.maxValue = level.total_boss_health;
		partyHealth.maxValue = level.total_party_health;
		level.current_party_health = level.total_party_health;
		blocking = false;
		poisoned = false;
	}

	void PopulateFloors(){
		floors = new List<Floor>();
		// Declare Floors and Boss Attacks
		Floor f = new Floor("trash1_lizard", 20);
		f.AddAttack(new Attack(1.0f, 0.0f, 1.0f, 5, 10, "Bite", 0, 0));
		floors.Add(f);
		f = new Floor("boss1_lizardperson", 30);
		f.AddAttack(new Attack(0.5f, 0.0f, 0.5f, 2, 20, "Claw Swipe", 0, 3));
		f.AddAttack(new Attack(1.0f, 0.0f, 1.0f, 5, 10, "Bite", 0, 0));
		floors.Add(f);
		f = new Floor("trash2_mimic", 40);
		f.AddAttack(new Attack(1.0f, 0.0f, 1.0f, 1, 80, "It's a trap", 0, 99));
		f.AddAttack(new Attack(0.5f, 0.0f, 0.5f, 2, 30, "Bite", 0, 2));
		f.AddAttack(new Attack(1.0f, 0.0f, 1.0f, 5, 10, "Boxxed", 0, 0));
		floors.Add(f);
		f = new Floor("boss2_bandit", 70);
		f.AddAttack(new Attack(1.0f, 0.0f, 1.0f, 1, 80, "Ambushed", 0, 99));
		f.AddAttack(new Attack(0.5f, 0.0f, 0.5f, 2, 50, "Ninja Star", 0, 4));
		f.AddAttack(new Attack(1.0f, 0.0f, 1.0f, 5, 10, "Stab", 0, 0));
		floors.Add(f);
		f = new Floor("trash3_snakes", 100);
		f.AddAttack(new Attack(.75f, 0.25f, 1.0f, 1, 30, "Paralyzed", 0, 4, "slock"));
		f.AddAttack(new Attack(0.5f, 0.0f, 0.5f, 2, 20, "Snake Swarm", 0, 2));
		f.AddAttack(new Attack(1.0f, 0.0f, 1.0f, 5, 10, "Bite", 0, 0));
		floors.Add(f);
		f = new Floor("boss3_snake_gorgon_girl", 125);
		f.AddAttack(new Attack(.75f, 0.0f, 1.0f, 1, 25, "Petrifying Gaze", 0, 6, "slock"));
		f.AddAttack(new Attack(0.5f, 0.0f, 0.75f, 2, 40, "Leer", 0, 3));
		f.AddAttack(new Attack(1.0f, 0.0f, 1.0f, 5, 10, "Swipe", 0, 0));
		floors.Add(f);
		f = new Floor("trash4_bull", 75);
		f.AddAttack(new Attack(.75f, 0.25f, 1.0f, 1, 70, "Charging...", 1, 4, "", "Bull Rush"));
		f.AddAttack(new Attack(0.5f, 0.0f, 0.5f, 2, 20, "Trample", 0, 2));
		f.AddAttack(new Attack(1.0f, 0.0f, 1.0f, 5, 10, "Kick", 0, 0));
		floors.Add(f);
		f = new Floor("boss4_minotaur", 150);
		f.AddAttack(new Attack(1.0f, 0.0f, 1.0f, 1, 60, "Kick Over", 0, 15));
		f.AddAttack(new Attack(1.0f, 0.0f, 1.0f, 2, 100, "Charging...", 2, 5, "", "Minotaur Rush"));
		f.AddAttack(new Attack(1.0f, 0.0f, 1.0f, 3, 60, "Sucker Punch", 0, 15));
		f.AddAttack(new Attack(1.0f, 0.0f, 1.0f, 4, 35, "Gore", 0, 3));
		f.AddAttack(new Attack(1.0f, 0.0f, 1.0f, 5, 10, "Punch", 0, 0));
		floors.Add(f);
		f = new Floor("trash5_cheetah", 100);
		f.AddAttack(new Attack(.25f, 0.0f, 1.0f, 1, 25, "Frenzied Claws", 0, 0));
		f.AddAttack(new Attack(0.5f, 0.0f, 0.5f, 2, 20, "Deep Swipe", 0, 2));
		f.AddAttack(new Attack(1.0f, 0.0f, 1.0f, 5, 10, "Swipe", 0, 0));
		floors.Add(f);
		f = new Floor("boss5_catgirl", 150);
		f.AddAttack(new Attack(.15f, 0.0f, 1.0f, 1, 40, "Frenzied Onslaught", 0, 0));
		f.AddAttack(new Attack(.75f, 0.0f, 1.0f, 2, 30, "Brain rattle", 0, 6, "slock"));
		f.AddAttack(new Attack(0.5f, 0.15f, 0.75f, 3, 80, "Bounds back...", 1, 4, "", "Pounce"));
		f.AddAttack(new Attack(0.5f, 0.0f, 0.5f, 4, 30, "Bite", 0, 2));
		f.AddAttack(new Attack(1.0f, 0.0f, 1.0f, 5, 10, "Hiss", 0, 0));
		floors.Add(f);
		f = new Floor("trash6_ogre", 100);
		f.AddAttack(new Attack(1.0f, 0.0f, 1.0f, 1, 90, "90 Ton Hammer", 0, 99));
		f.AddAttack(new Attack(0.5f, 0.0f, 0.5f, 2, 20, "Rock Throw", 0, 2));
		f.AddAttack(new Attack(1.0f, 0.0f, 1.0f, 5, 10, "Bash", 0, 0));
		floors.Add(f);
		f = new Floor("boss6_cyclops", 200);
		f.AddAttack(new Attack(1.0f, 0.0f, 1.0f, 1, 1000, "1000 Ton Hammer", 0, 99));
		f.AddAttack(new Attack(0.5f, 0.0f, 1.0f, 2, 1000, "Holds hammer up high...", 1, 6, "", "1000 Ton Hammer"));
		f.AddAttack(new Attack(0.5f, 0.15f, 1.0f, 3, 25, "Earthshattering Stomp", 0, 4, "slock"));
		f.AddAttack(new Attack(0.5f, 0.0f, 0.5f, 4, 70, "Stomp", 0, 2));
		f.AddAttack(new Attack(1.0f, 0.0f, 1.0f, 5, 10, "Punch", 0, 0));
		floors.Add(f);
		f = new Floor("trash7_spider", 150);
		f.AddAttack(new Attack(1.0f, 0.0f, 1.0f, 1, 10, "Spider Bite - POISONED", 0, 4, "poison"));
		f.AddAttack(new Attack(1.0f, 0.0f, 0.5f, 2, 30, "Web", 0, 4, "slock"));
		f.AddAttack(new Attack(0.5f, 0.0f, 0.5f, 3, 20, "Spider Swarm", 0, 2));
		f.AddAttack(new Attack(1.0f, 0.0f, 1.0f, 5, 10, "Nibble", 0, 0));
		floors.Add(f);
		f = new Floor("boss7_spider_girl", 200);
		f.AddAttack(new Attack(.50f, 0.0f, 1.0f, 1, 25, "Envenomed Fangs - POISONED", 0, 4, "poison"));
		f.AddAttack(new Attack(0.75f, 0.0f, 1.0f, 2, 40, "Sticky Web", 0, 8, "slock"));
		f.AddAttack(new Attack(0.75f, 0.0f, 1.0f, 3, 25, "Feed", 0, 8));
		f.AddAttack(new Attack(0.75f, 0.0f, 1.0f, 4, 40, "Stickier Web", 0, 8, "slock"));
		f.AddAttack(new Attack(0.5f, 0.0f, 0.5f, 5, 25, "Bite", 0, 2));
		f.AddAttack(new Attack(1.0f, 0.0f, 1.0f, 6, 10, "Bite", 0, 0));
		floors.Add(f);
		f = new Floor("trash8_drake", 200);
		f.AddAttack(new Attack(0.25f, 0.0f, 1.0f, 1, 40, "Flame Breath", 0, 0));
		f.AddAttack(new Attack(0.5f, 0.25f, 1.0f, 2, 100, "Flies up...", 1, 4, "", "Rake"));
		f.AddAttack(new Attack(0.5f, 0.25f, 1.0f, 3, 100, "Breaths in...", 1, 4, "", "Englufing Flames"));
		f.AddAttack(new Attack(0.75f, 0.0f, 1.0f, 4, 20, "Poison Spit - POISONED", 0, 5, "poison"));
		f.AddAttack(new Attack(0.5f, 0.0f, 0.5f, 5, 20, "Drake Claws", 0, 2));
		f.AddAttack(new Attack(1.0f, 0.0f, 1.0f, 6, 10, "Bite", 0, 0));
		floors.Add(f);
		f = new Floor("boss8_dragon", 350);
		f.AddAttack(new Attack(0.15f, 0.0f, 1.0f, 1, 50, "Dragon's Fire", 0, 0));
		f.AddAttack(new Attack(0.5f, 0.25f, 1.0f, 2, 40, "Ferocious Bite", 0, 4));
		f.AddAttack(new Attack(0.5f, 0.25f, 1.0f, 3, 100, "Takes a deep breath...", 2, 4, "", "World in Flames"));
		f.AddAttack(new Attack(0.75f, 0.35f, 1.0f, 4, 20, "Poisoned Claws - POISONED", 0, 5, "poison"));
		f.AddAttack(new Attack(0.75f, 0.0f, 1.0f, 5, 20, "Terrifying Roar", 0, 4, "slock"));
		f.AddAttack(new Attack(0.5f, 0.15f, 0.5f, 6, 60, "Dragon's Bite", 0, 3));
		f.AddAttack(new Attack(1.0f, 0.0f, 1.0f, 7, 15, "Rend", 0, 0));
		floors.Add(f);
	}

	int HandleAttack(GameObject p){
		string attack_type = p.GetComponent<ToggleGroup>().GetActive().name;
		PartyMember pm = p.GetComponent<PartyMember>();
		Person person = pm.me;
	
		int damage = 0;
		// Lower delay
		if(person.current_delay > 0) {
			person.current_delay--;
		}
		// Determine attack type
		if(attack_type == "Attack") {
			damage = person.basic_attack;
			pm.SetDamage(damage.ToString());
		} else if(attack_type == "Special") {
			damage = person.special_attack;
			switch(person.special_attack_type){
			case "heal":
				party_status_animator.SetTrigger("heal");
				pm.SetDamage("+50");
				level.DamagePlayers(-50);
				break;
			case "interrupt":
				pm.SetDamage("Interrupt - " + damage.ToString());
				level.current_boss_delay = 0;
				level.delayed_attack = null;
				boss_animator.SetBool("delay", false);
				break;
			case "chain_break":
				pm.SetDamage("Free Mind");
				foreach(GameObject go in GameObject.FindGameObjectsWithTag("slock")){
					go.GetComponent<slock>().Unlock();
				}
				break;
			case "time_freeze":
				pm.SetDamage("Time Warp");
				time_warp = true;
				break;
			case "block":
				party_status_animator.SetTrigger("block");
				pm.SetDamage("Block");
				blocking = true;
				break;
			case "cleanse":
				pm.SetDamage("Cleanse");
				poisoned = false;
				break;
			default:
				pm.SetDamage(damage.ToString());
				break;
			}
			person.current_delay = person.delay;
		}
		return damage;
	}

	public void Execute(){
		if(level.current_party_health > 0){
			level.DamageBoss(HandleAttack(p1));
			p1.GetComponent<Animator>().SetTrigger("attack");
			animating = true;
			last_attacker = p1;
			last_attacker_num = 1;
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
	Model mod;

	public Level(Floor f, Model m){
		boss_name = f.name;
		total_boss_health = f.health;
		current_boss_health = f.health;
		floor = f;
		total_party_health = 100;
		current_party_health = 100;
		mod = m;
	}

	public void BossAttack(){
		floor.LowerCooldowns();
		if(current_boss_delay > 0){
			current_boss_delay--;
			if(current_boss_delay == 0){
				mod.boss_image.GetComponent<Animator>().SetTrigger("jitter");
				mod.boss_animator.SetTrigger("white");
				mod.boss_animator.SetBool("delay", false);

				if(!mod.blocking){
					mod.boss_attack.SetText(delayed_attack.delayed_name + " *" + delayed_attack.damage.ToString() + "*");
					DamagePlayers(delayed_attack.damage);
				} else {
					mod.boss_attack.SetText(delayed_attack.delayed_name + " - BLOCKED");
				}
				delayed_attack = null;
			}
		} else {
			foreach(Attack a in floor.attacks){
				float cur_percent = (float)current_boss_health / (float)total_boss_health;
				float rand = Random.value;
				if(cur_percent <= a.max_percent && cur_percent >= a.min_percent && rand < a.chance && a.cur_cooldown == 0){
					mod.boss_animator.SetBool("poison", false);
					switch(a.status){
					case "slock":
						foreach(GameObject go in GameObject.FindGameObjectsWithTag("slock")){
							go.GetComponent<slock>().Lock();
						}
						break;
					case "poison":
						mod.boss_animator.SetBool("poison", true);
						if(!mod.blocking){
							mod.poisoned = true;
						}
						break;
					}
					a.cur_cooldown = a.cooldown;
					if(a.delay > 0){
						mod.boss_animator.SetBool("delay", true);
						delayed_attack = a;
						current_boss_delay = a.delay;
						mod.boss_attack.SetText(a.name);
					} else {
						mod.boss_animator.SetTrigger("white");
						mod.boss_image.GetComponent<Animator>().SetTrigger("jitter");
						if(!mod.blocking){
							mod.boss_attack.SetText(a.name + " *" + a.damage.ToString() + "*");
							DamagePlayers(a.damage);
						} else {
							mod.boss_attack.SetText(a.name + " - BLOCKED");
						}
					}
					return;
				}
			}
		}
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
	public string name;
	public string status;
	public int delay;
	public int cooldown;
	public int cur_cooldown = 0;
	public string delayed_name;

	public Attack(float max, float min, float cha, int prio, int dam, string nam, int del, int cool, string sta = "", string del_nam = ""){
		max_percent = max;
		min_percent = min;
		chance = cha;
		priority = prio;
		damage = dam;
		status = sta;
		delay = del;
		cooldown = cool;
		name = nam;
		delayed_name = del_nam;
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