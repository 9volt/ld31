using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PartyMember : MonoBehaviour {
	public Person me;
	public Toggle special_toggle;
	public Toggle attack_toggle;
	public Text cooldown;
	public Image avatar;
	public Image attack_image;
	public Image special_image;
	// Use this for initialization
	void Start () {
		
	}

	public void SetPerson(Person p){
		me = p;
		avatar.sprite = Resources.Load<Sprite>(me.name);
		attack_image.sprite = Resources.Load<Sprite>(me.name + "_attack");
		special_image.sprite = Resources.Load<Sprite>(me.special_attack_type);
	}
	
	// Update is called once per frame
	void Update () {
		special_toggle.interactable = me.current_delay == 0;
		if(special_toggle.interactable == false) {
			cooldown.text = me.current_delay.ToString();
			attack_toggle.isOn = true;
			special_toggle.isOn = false;
		} else {
			cooldown.text = "";
		}
	}
}
