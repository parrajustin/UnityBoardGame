using System.Collections.Generic;
using UnityEngine;
// using System;
using UnityEngine.UI;

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
		public SortedList<string, pieceController> store;
	#endregion

	public Text lives;
	public Text currentTurn;
	public Text spawnCountDown;

	public int teamTurnNumber = 0;

	/// <summary>
	/// The defined board size
	/// </summary>
	public int gameBoardSize = 7; 

	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Start () {
	}

	/// <summary>
	/// This runs before any start function in any other script
	/// </summary>
	void Awake() {
		// setup bTree storage
		store = new SortedList<string, pieceController>();

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
		spawn(4, gameBoardSize / 2, gameBoardSize / 2);
		spawn(4, gameBoardSize / 2, -1);
		spawn(4, -1, gameBoardSize / 2);
		spawn(4, gameBoardSize, gameBoardSize / 2);
		spawn(4, gameBoardSize / 2, gameBoardSize);
		// spawn(4, UnityEngine.Random.Range(1, gameBoardSize-1), UnityEngine.Random.Range(1, gameBoardSize-1));
		// spawn(4, UnityEngine.Random.Range(1, gameBoardSize-1), UnityEngine.Random.Range(1, gameBoardSize-1));
		// spawn(4, UnityEngine.Random.Range(1, gameBoardSize-1), UnityEngine.Random.Range(1, gameBoardSize-1));
	}

	// Spawn something with a certain Team identifier
	private void spawn(int team = -1, int x = -1, int z = -1) {
		if( team == -1) 
			for( int i = 0; i < 4; i++ ) {
				this.spawn(i, (int)(i % 2) * (gameBoardSize - 1), (int)(i / 2) * (gameBoardSize - 1));
			}	
		else if(team < 0 || team > 4)
			Debug.LogError("Team Number not correct!");
		else {
			Vector3 pos = new Vector3(
				(x) * 1.25f - ((gameBoardSize - 1) / 2.0f) * 1.25f, 
				0.0f, 
				(z) * 1.25f - ((gameBoardSize - 1) / 2.0f) * 1.25f
			);

			GameObject spawned = Instantiate(piecePrefab, pos, Quaternion.identity) as GameObject;
			spawned.tag = "team" + (team).ToString();
			spawned.name = "team" + (team).ToString();

			switch(team) {
				case 0:
					spawned.GetComponentInChildren<MeshRenderer>().material = Blue;
					break;
				case 1:
					spawned.GetComponentInChildren<MeshRenderer>().material = Red;
					break;
				case 2:
					spawned.GetComponentInChildren<MeshRenderer>().material = Green;
					break;
				case 3:
					spawned.GetComponentInChildren<MeshRenderer>().material = Yellow;
					break;
			}

			pieceController pc = spawned.GetComponent<pieceController>();
			pc.x = x;
			pc.z = z;
			pc.controller = this;

			// if( pc.x != -1 && pc.y != -1 )
				store.Add(new Vector2(pc.x, pc.z).ToString(), pc);
			// else {
			// 	store.Add()
			// }
		}
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	void Update () {
		if( teamTurnNumber == 1 ) {
			if( Input.GetKeyDown(KeyCode.W) ) {
				foreach(GameObject obj in GameObject.FindGameObjectsWithTag("team1")) {
					pieceController pc = obj.GetComponent<pieceController>();
					pc.move(0);
				}
				teamTurnNumber = (teamTurnNumber + 1) % 4;
			} else if( Input.GetKeyDown(KeyCode.S) ) {
				foreach(GameObject obj in GameObject.FindGameObjectsWithTag("team1")) {
					pieceController pc = obj.GetComponent<pieceController>();
					pc.move(2);
				}
				teamTurnNumber = (teamTurnNumber + 1) % 4;
			} else if( Input.GetKeyDown(KeyCode.A) ) {
				foreach(GameObject obj in GameObject.FindGameObjectsWithTag("team1")) {
					pieceController pc = obj.GetComponent<pieceController>();
					pc.move(3);
				}
				teamTurnNumber = (teamTurnNumber + 1) % 4;
			} else if( Input.GetKeyDown(KeyCode.D) ) {
				foreach(GameObject obj in GameObject.FindGameObjectsWithTag("team1")) {
					pieceController pc = obj.GetComponent<pieceController>();
					pc.move(1);
				}
				teamTurnNumber = (teamTurnNumber + 1) % 4;
			}
		} else {
			foreach(GameObject obj in GameObject.FindGameObjectsWithTag("team" + teamTurnNumber.ToString())) {
				pieceController pc = obj.GetComponent<pieceController>();
				Dictionary<int, int> temp = new Dictionary<int, int>();

				if( pc.x != 0 )
					temp.Add(temp.Count, 3);
				if( pc.x != gameBoardSize - 1 )
					temp.Add(temp.Count, 1);
				if( pc.z != 0 ) 
					temp.Add(temp.Count, 2);
				if( pc.z != gameBoardSize - 1 ) 
					temp.Add(temp.Count, 0);

				int dir = 0;
				temp.TryGetValue(Random.Range(0, temp.Count), out dir);
				pc.move(dir);
			}

			teamTurnNumber = (teamTurnNumber + 1) % 4;
		}
	}
}