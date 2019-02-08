using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : MonoBehaviour {

	public float speed = 0.8f;
	private List<Node> currentPath;
	private Node targetNode;
	private Quaternion targetRotation;
	// Use this for initialization
	void Start () {
		currentPath = new List<Node> ();
	}
	
	// Update is called once per frame
	void Update () {
		// Determine the target node
		if (targetNode == null && currentPath.Count > 0) {
			targetNode = currentPath [0];
			currentPath.Remove (targetNode);
		}

		// Move towards target node
		if (targetNode != null) {
			Vector3 direction = (targetNode.value.transform.position - transform.position).normalized;
			transform.position += direction * speed * Time.deltaTime;

			float angle = Mathf.Atan2 (direction.y, direction.x);
			targetRotation = Quaternion.Euler (0, 0, 90 + angle * Mathf.Rad2Deg);
			transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, Time.deltaTime * 5);

			if (Vector3.Distance (transform.position, targetNode.value.transform.position) < 0.12f) {
				transform.position = targetNode.value.transform.position;
				targetNode = null;
			}
		}
	}

	public void Move (List<Node> path) {
		currentPath.Clear ();
		foreach (Node node in path) {
			currentPath.Add (node);
		}
	}
}
