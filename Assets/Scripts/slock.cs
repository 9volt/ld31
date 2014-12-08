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
		if(locked && pm.me.name == "witch"){
			SetUnlock();
			was_witch = true;
			if(pm.me.current_delay == 0){
				special.interactable = true;
			} else {
				special.interactable = false;
			}
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

	public void Reset(){
		gameObject.GetComponent<Animator>().SetTrigger("reset");
	}

	public void Lock(){
		SetLock();
		locked = true;
	}

	public void Unlock(){
		SetUnlock();
		locked = false;
	}
}
