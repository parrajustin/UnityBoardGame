using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GameController : MonoBehaviour {

	public List<GameObject> pieces = new List<GameObject>();

	
	Material Red;
	Material Blue;
	Material Green;
	Material Yellow;
	private GameObject floorPrefab; // Reference to the floor prefab
	private GameObject piecePrefab; // Reference to the game piece prefab
	private GameObject[,] floorArray; // 2d array to hold the floor peices


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
		floorArray = new GameObject[7,7];

		// initialize a parent for the floor pieces
		GameObject parent = new GameObject();
		parent.transform.position = Vector3.zero;
		parent.name = "floorPiecesParent";

		// now we want to spawn the peices for the floor
		for(int i = 0; i < 7; i++ ) {
			for(int j = 0; j < 7; j++ ) {
				floorArray[i,j] = Instantiate(floorPrefab, new Vector3(i*1.25f - 3.0f*1.25f, 0.0f, j*1.25f - 3.0f*1.25f), Quaternion.identity, parent.transform) as GameObject;
			}
		}

		// now assign to materials to the corner floor pieces
		floorArray[0,6].GetComponent<MeshRenderer>().material = Green;
		floorArray[0,0].GetComponent<MeshRenderer>().material = Blue;
		floorArray[6,0].GetComponent<MeshRenderer>().material = Red;
		floorArray[6,6].GetComponent<MeshRenderer>().material = Yellow;

		// Now create the default Pieces and some random blocking pieces
		pieces.Add(Instantiate(piecePrefab, floorArray[6,0].transform.position, Quaternion.identity) as GameObject);
	}

	
	// Update is called once per frame
	void Update () {
		// if( Input.GetKeyDown(KeyCode.W) ) {
		// 	piece.transform.Translate(new Vector3(0.0f, 0.0f, 1.25f));
		// } else if( Input.GetKeyDown(KeyCode.S) ) {
		// 	piece.transform.Translate(-new Vector3(0.0f, 0.0f, 1.25f));
		// } else if( Input.GetKeyDown(KeyCode.A) ) {
		// 	piece.transform.Translate(-new Vector3(1.25f, 0.0f, 0.0f));
		// } else if( Input.GetKeyDown(KeyCode.D) ) {
		// 	piece.transform.Translate(new Vector3(1.25f, 0.0f, 0.0f));
		// }
	}
}


