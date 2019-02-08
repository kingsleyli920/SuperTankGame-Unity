using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that holds information about
// certain position, so it's used in a 
// pathfinding algorithm.
public class Node{

	// Node has X and Y positions
	public int posX;
	public int posY;

	// G is a basic cost value to go from one node to another
	public int g = int.MaxValue;

	// H is heuristic that estimates the cost of the closest path
	public int f = int.MaxValue;

	// Node has references to other nodes so it is possible to build a path
	public Node parent = null;

	// The value of the node
	public NavTile value = null;

	// Constructor
	public Node (int posX, int posY){
		this.posX = posX;
		this.posY = posY;

	}

}

public class AStar : MonoBehaviour {

	// Public variables
	public GameObject backgroundContainer;

	// Constants
	private int mapWidth;
	private int mapHeight;

	// Variables
	private List<string> map;
	private Node[,] nodeMap; // Node[,] means 2D array

	// Use this for initialization
	public List<Node> FindPath (GameObject obj1, GameObject obj2) {
		
		mapHeight = backgroundContainer.transform.childCount;
		mapWidth = backgroundContainer.transform.GetChild (0).childCount;
		// Parse the map
		nodeMap = new Node[mapWidth, mapHeight];
		Node start = null;
		Node goal = null;

		//Debug.Log (backgroundContainer.transform.childCount);

		for (int y = 0; y < backgroundContainer.transform.childCount; y++) {
			Transform backgroundRow = backgroundContainer.transform.GetChild (y);
			for (int x = 0; x < backgroundRow.childCount; x++) {
				NavTile tile = backgroundRow.GetChild (x).GetComponent<NavTile> ();
				Node node = new Node (x, y);
				node.value = tile;
				//Debug.Log ("Tile: " + tile);
				//Debug.Log(backgroundRow.GetChild (x));
				nodeMap [x, y] = node;
			}
		}

		start = FindNode (obj1);
		goal = FindNode (obj2);
		//Debug.Log ("Player is on " + start.posX + "," + start.posY);
		//Debug.Log ("Enemy is on " + goal.posX + "," + goal.posY);
		// Execute the A Star algorithm
		List<Node> nodePath = ExecuteAStar (start, goal);
		nodePath.Reverse ();
		return nodePath;
	}

	private Node FindNode (GameObject obj){
		Collider2D[] collidingObjects = Physics2D.OverlapCircleAll (obj.transform.position, .2f);
		foreach (Collider2D collidingObj in collidingObjects) {
			//Debug.Log (collidingObj.name);
			if(collidingObj.gameObject.GetComponents<NavTile> () != null) {

				// The tile the player is on
				NavTile tile = collidingObj.gameObject.GetComponent<NavTile> ();

				// Find the node which contains the tile
				for (int y = 0; y < mapHeight; y++) {
					for (int x = 0; x < mapWidth; x++) {
						Node node = nodeMap [x, y];
						if (node.value == tile) {
							//Debug.Log (node.value);
							return node;
						}
					}
				}

			}
		}
		return  null;
	}
	// Update is called once per frame
	void Update () {
		
	}

	private List<Node> ExecuteAStar (Node start, Node goal){

		// list holds potential best path nodes that should 
		// be visited. It always starts with the origin
		List<Node> openList = new List<Node> () { start };

		// List keeps track of all nodes that have been visited
		List<Node> closeList = new List<Node> ();

		// Initialize the start node
		// Note: f = g + h
		start.g = 0;
		start.f = start.g + CalculateHeuristicValue(start, goal);

		// Main algorithm
		while (openList.Count > 0) {
			// Get the node with lowest estimated cost to reach target
			Node current = openList [0];
			foreach (Node node in openList) {
				if (node.f < current.f) {
					current = node;
				}
			}

			// Check if the target has been reached
			if (current == goal) {
				return BuildPath (goal);
			}

			// Make sure current node will not visit again
			openList.Remove (current);
			closeList.Add (current);

			// Execute the algorithm in the current node's neighbours
			List<Node> neighbours = GetNeighbourNodes (current);
			foreach (Node neighbour in neighbours) {
				if (closeList.Contains(neighbour)) {
					// If the neighbour has already been visited, ignore it
					continue;
				}

				if (!openList.Contains (neighbour)) {
					// If the neighbour has't been scheduled for visiting, add it for visiting later
					openList.Add (neighbour);
				}

				// Caluculate a new G value and verify if this value 
				// is better than whatever is stored in the neighbour
				int candidateG = current.g + 1;
				if (candidateG >= neighbour.g) {
					// If the G value is greater or equal, then this doesn't belong 
					// to a good path (there was a better path caculated)
					continue;
				} else {
					// Otherwise, there is a better way to reach this neighbour
					// Initialize its values
					neighbour.parent = current;
					neighbour.g = candidateG;
					neighbour.f = neighbour.g + CalculateHeuristicValue (neighbour, goal);
				}
			}
		}

		// If reach this point, it means that there are 
		// no more nodes to search, the algorithm failed.
		return new List<Node> ();

	}

	private List<Node> GetNeighbourNodes (Node node) {
		List<Node> neighbours = new List<Node> ();

		// Verify all possible neighbours
		if (node.value.canMoveLeft && node.posX - 1 >= 0) {
			Node candidate = nodeMap [node.posX - 1, node.posY];
			if (candidate.value.navigable) {
				neighbours.Add (candidate);
			}
		}

		if (node.value.canMoveRight && node.posX + 1 <= mapWidth - 1) {
			Node candidate = nodeMap [node.posX + 1, node.posY];
			if (candidate.value.navigable) {
				neighbours.Add (candidate);
			}
		}

		if (node.value.canMoveUp && node.posY - 1 >= 0) {
			Node candidate = nodeMap [node.posX, node.posY - 1];
			if (candidate.value.navigable) {
				neighbours.Add (candidate);
			}
		}

		if (node.value.canMoveDown && node.posY + 1 <= mapHeight - 1) {
			Node candidate = nodeMap [node.posX, node.posY + 1];
			if (candidate.value.navigable) {
				neighbours.Add (candidate);
			}
		}

		return neighbours;

	}

	// A simple estimate of the distance 
	// Uses the Manhattan Distance
	private int CalculateHeuristicValue (Node node1, Node node2){
		return Mathf.Abs (node1.posX - node2.posX) + Mathf.Abs (node1.posY - node2.posY);
	}

	private List<Node> BuildPath (Node node) {
		List<Node> path = new List<Node> () { node };
		while (node.parent != null) {
			node = node.parent;
			path.Add (node);
		}
		return path;
	}
}
