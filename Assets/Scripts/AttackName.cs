using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AttackName : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetText(string text){
		gameObject.GetComponent<Text>().CrossFadeAlpha(1.0f, 0.0f, true);
		gameObject.GetComponent<Text>().text = text;
		gameObject.GetComponent<Text>().CrossFadeAlpha(0.0f, 2.0f, true);
	}
}
