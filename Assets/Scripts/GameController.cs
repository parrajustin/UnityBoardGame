// using System.Collections.Generic;
using UnityEngine;
// using System;

public class GameController : MonoBehaviour {

	#region Loaded Resources
		Material Red;
		Material Blue;
		Material Green;
		Material Yellow;
		private GameObject floorPrefab; // Reference to the floor prefab
		private GameObject piecePrefab; // Reference to the game piece prefab
	#endregion

	#region Create Game pieces managment
		private GameObject[,] floorArray; // 2d array to hold the floor peices
	#endregion

	public int gameBoardSize = 7; //doesn't do anything right now

	// Use this for initialization
	void Start () {
		// Vector3 pos = new Vector3((Random.Range(0, 2) == 1? -1.0f: 1.0f) * Random.Range(1, 4) * 1.25f, 0.0f, (Random.Range(0,2) == 1? -1.0f: 1.0f) * Random.Range(1, 4) * 1.25f);
		// Debug.Log(pos);
		// other = (GameObject) Instantiate(Resources.Load("Prefab/PieceParent") as GameObject, pos, Quaternion.identity);

		// setUpBoard();
	}

	// This runs before any start function in any other script
	void Awake() {
		// Load up the prefabs that are used in this game
		floorPrefab = Resources.Load("Prefab/gameFloor") as GameObject;
		piecePrefab = Resources.Load("Prefab/gamePiece") as GameObject;
		
		Red = Resources.Load("Materials/Red") as Material;
		Blue = Resources.Load("Materials/Blue") as Material;
		Green = Resources.Load("Materials/Green") as Material;
		Yellow = Resources.Load("Materials/Yellow") as Material;

		// initialize teh floor array
		floorArray = new GameObject[gameBoardSize, gameBoardSize];

		// initialize a parent for the floor pieces
		GameObject parent = new GameObject();
		parent.transform.position = Vector3.zero;
		parent.name = "floorPiecesParent";

		// now we want to spawn the peices for the floor
		for(int i = 0; i < gameBoardSize; i++ ) {
			for(int j = 0; j < gameBoardSize; j++ ) {
				floorArray[i,j] = Instantiate(
						floorPrefab, 
						new Vector3(
							i * 1.25f - ((gameBoardSize - 1) / 2.0f) * 1.25f, 
							0.0f, 
							j * 1.25f - ((gameBoardSize - 1) / 2.0f) * 1.25f
						), 
						Quaternion.identity, 
						parent.transform
					) as GameObject;
			}
		}

		// now assign to materials to the corner floor pieces
		floorArray[0, gameBoardSize - 1].GetComponent<MeshRenderer>().material = Green;
		floorArray[0, 0].GetComponent<MeshRenderer>().material = Blue;
		floorArray[gameBoardSize - 1, 0].GetComponent<MeshRenderer>().material = Red;
		floorArray[gameBoardSize - 1, gameBoardSize - 1].GetComponent<MeshRenderer>().material = Yellow;

		// Now create the default Pieces
		spawn();

		// Spawn some barriers
		spawn(4, Random.Range(1, gameBoardSize-1), Random.Range(1, gameBoardSize-1));
		spawn(4, Random.Range(1, gameBoardSize-1), Random.Range(1, gameBoardSize-1));
		spawn(4, Random.Range(1, gameBoardSize-1), Random.Range(1, gameBoardSize-1));
	}

	// Spawn something with a certain Team identifier
	private void spawn(int team = -1, int x = -1, int y = -1) {
		if( team == -1) { 
			for( int i = 0; i < 4; i++ ) {
				GameObject spawned = Instantiate(piecePrefab, floorArray[(i % 2) * (gameBoardSize - 1), (i / 2) * (gameBoardSize - 1)].transform.position, Quaternion.identity) as GameObject;
				spawned.GetComponentInChildren<MeshRenderer>().material = floorArray[(i % 2) * (gameBoardSize - 1), (i / 2) * (gameBoardSize - 1)].GetComponent<MeshRenderer>().material;
				spawned.tag = "team" + (i).ToString();
				spawned.name = "team" + (i).ToString();
			}
		} else if(team < 0 || team > 4)
			Debug.LogError("Team Number not correct!");
		else if( team != -1 && x != -1 && y != -1 ){
			GameObject spawned = Instantiate(piecePrefab, floorArray[x, y].transform.position, Quaternion.identity) as GameObject;
			spawned.tag = "team" + (team).ToString();
			spawned.name = "team" + (team).ToString();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if( Input.GetKeyDown(KeyCode.W) ) {
			foreach(GameObject obj in GameObject.FindGameObjectsWithTag("team1")) {
				obj.transform.Translate(new Vector3(0.0f, 0.0f, 1.25f));
			}
		} else if( Input.GetKeyDown(KeyCode.S) ) {
			foreach(GameObject obj in GameObject.FindGameObjectsWithTag("team1")) {
				obj.transform.Translate(-new Vector3(0.0f, 0.0f, 1.25f));
			}
		} else if( Input.GetKeyDown(KeyCode.A) ) {
			foreach(GameObject obj in GameObject.FindGameObjectsWithTag("team1")) {
				obj.transform.Translate(-new Vector3(1.25f, 0.0f, 0.0f));
			}
		} else if( Input.GetKeyDown(KeyCode.D) ) {
			foreach(GameObject obj in GameObject.FindGameObjectsWithTag("team1")) {
				obj.transform.Translate(new Vector3(1.25f, 0.0f, 0.0f));
			}
		}
	}
}