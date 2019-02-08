using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public delegate void CollectHandler (GameObject crate);
	public event CollectHandler OnCollect;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D (Collider2D otherCollider) {
		//Debug.Log (otherCollider.gameObject.name);
		if (otherCollider.gameObject.tag == "Crate") {
			if (OnCollect != null) {
				OnCollect (otherCollider.gameObject);
			}
			//Destroy (otherCollider.gameObject);
		}
	}
}
