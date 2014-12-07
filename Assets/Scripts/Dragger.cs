using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Dragger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
	public Transform target;
	private bool isMouseDown = false;
	private Vector3 startMousePosition;
	private Vector3 startPosition;
	public bool shouldReturn;
	GameObject[] drag_targets;
	Camera camera;
	PartyMember pm;
	
	// Use this for initialization
	void Start () {
		drag_targets = GameObject.FindGameObjectsWithTag("drag_target");
		pm = gameObject.GetComponentInParent<PartyMember>();
		camera = Camera.main;
	}
	
	public void OnPointerDown(PointerEventData dt) {
		isMouseDown = true;
		startPosition = target.position;
		startMousePosition = Input.mousePosition;
	}
	
	public void OnPointerUp(PointerEventData dt) {
		isMouseDown = false;

		foreach(GameObject go in drag_targets){
			RectTransform rc = go.GetComponent<RectTransform>();
			if(RectTransformUtility.RectangleContainsScreenPoint(rc, Input.mousePosition, camera)){
				pm.SetPerson(go.GetComponent<PartyMember>().Swap(pm.me));
			}
		}

 		if (shouldReturn) {
			target.position = startPosition;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (isMouseDown) {
			Vector3 mouse_pos = camera.ScreenToWorldPoint(Input.mousePosition);
			mouse_pos.z = target.position.z;
			target.position = mouse_pos;
		}
	}
}