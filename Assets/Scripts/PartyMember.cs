using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PartyMember : MonoBehaviour {
	public Person me;
	public Toggle special_toggle;
	public Toggle attack_toggle;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		special_toggle.interactable = me.current_delay == 0;
		if(special_toggle.interactable == false) {
			attack_toggle.isOn = true;
			special_toggle.isOn = false;
		}
	}
}
