using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class slock : MonoBehaviour {
	public PartyMember pm;
	public Toggle special;
	bool locked = false;
	bool was_witch = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(locked && pm.me.name == "witch" && !was_witch){
			SetUnlock();
			was_witch = true;
			special.interactable = true;
		} else if(was_witch && locked && pm.me.name == "witch"){
			special.interactable = true;
		} else if(was_witch && locked && pm.me.name != "witch"){
			was_witch = false;
			SetLock();
		} else if(locked || pm.me.current_delay != 0){
			special.interactable = false;
		} else {
			special.interactable = true;
		}

	}

	void SetLock(){
		gameObject.GetComponent<Animator>().SetTrigger("lock");

	}

	void SetUnlock(){
		gameObject.GetComponent<Animator>().SetTrigger("break");

	}

	public void Lock(){
		locked = true;
		SetLock();
	}

	public void Unlock(){
		locked = false;
		SetUnlock();
	}
}
