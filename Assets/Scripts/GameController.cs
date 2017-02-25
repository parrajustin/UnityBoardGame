// using System.Collections.Generic;
using UnityEngine;
using System;

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
		private BTree store;
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
		// setup bTree storage
		store = new BTree(5);

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
		spawn(4, UnityEngine.Random.Range(1, gameBoardSize-1), UnityEngine.Random.Range(1, gameBoardSize-1));
		spawn(4, UnityEngine.Random.Range(1, gameBoardSize-1), UnityEngine.Random.Range(1, gameBoardSize-1));
		spawn(4, UnityEngine.Random.Range(1, gameBoardSize-1), UnityEngine.Random.Range(1, gameBoardSize-1));

		store.dscOrder(store.getHead());
	}

	// Spawn something with a certain Team identifier
	private void spawn(int team = -1, int x = -1, int y = -1) {
		if( team == -1) { 
			for( int i = 0; i < 4; i++ ) {
				Vector3 temp = floorArray[(i % 2) * (gameBoardSize - 1), (i / 2) * (gameBoardSize - 1)].transform.position;

				GameObject spawned = Instantiate(piecePrefab, temp, Quaternion.identity) as GameObject;
				spawned.GetComponentInChildren<MeshRenderer>().material = floorArray[(i % 2) * (gameBoardSize - 1), (i / 2) * (gameBoardSize - 1)].GetComponent<MeshRenderer>().material;
				spawned.tag = "team" + (i).ToString();
				spawned.name = "team" + (i).ToString();

				pieceController pc = spawned.GetComponent<pieceController>();
				pc.x = (i % 2) * (gameBoardSize - 1);
				pc.y = (i / 2) * (gameBoardSize - 1);

				store.insert(pc.x * gameBoardSize + pc.y, spawned);
			}
		} else if(team < 0 || team > 4)
			Debug.LogError("Team Number not correct!");
		else if( team != -1 && x != -1 && y != -1 ){
			GameObject spawned = Instantiate(piecePrefab, floorArray[x, y].transform.position, Quaternion.identity) as GameObject;
			spawned.tag = "team" + (team).ToString();
			spawned.name = "team" + (team).ToString();

			pieceController pc = spawned.GetComponent<pieceController>();
			pc.x = x;
			pc.y = y;

			store.insert(pc.x * gameBoardSize + pc.y, spawned);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if( Input.GetKeyDown(KeyCode.W) ) {
			foreach(GameObject obj in GameObject.FindGameObjectsWithTag("team1")) {
				pieceController pc = obj.GetComponent<pieceController>();
				// Vector3 pos = obj.transform.position;
				// store.dscOrder(store.getHead());
				// Debug.Log((pos.x * gameBoardSize + pos.y));
				pc.y = pc.y + 1;
				obj.transform.Translate(new Vector3(0.0f, 0.0f, 1.25f));
				Debug.Log(store.findK((int) (pc.x * gameBoardSize + pc.y)));
			}
		} else if( Input.GetKeyDown(KeyCode.S) ) {
			foreach(GameObject obj in GameObject.FindGameObjectsWithTag("team1")) {
				pieceController pc = obj.GetComponent<pieceController>();
				pc.y = pc.y - 1;
				obj.transform.Translate(-new Vector3(0.0f, 0.0f, 1.25f));
				Debug.Log(store.findK((int) (pc.x * gameBoardSize + pc.y)));
			}
		} else if( Input.GetKeyDown(KeyCode.A) ) {
			foreach(GameObject obj in GameObject.FindGameObjectsWithTag("team1")) {
				pieceController pc = obj.GetComponent<pieceController>();
				pc.x = pc.x - 1;
				obj.transform.Translate(-new Vector3(1.25f, 0.0f, 0.0f));
				Debug.Log(store.findK((int) (pc.x * gameBoardSize + pc.y)));
			}
		} else if( Input.GetKeyDown(KeyCode.D) ) {
			foreach(GameObject obj in GameObject.FindGameObjectsWithTag("team1")) {
				pieceController pc = obj.GetComponent<pieceController>();
				pc.x = pc.x + 1;
				obj.transform.Translate(new Vector3(1.25f, 0.0f, 0.0f));
				Debug.Log(store.findK((int) (pc.x * gameBoardSize + pc.y)));
			}
		}
	}

	void move() {

	}
}


class BTreeNode {
	/** The seperating keys */
	public int[] key;
	/** The next bTree nodes */
	public BTreeNode[] childNodes;
	/** the values pointed at by the keys */
	public GameObject[] values;
	/** Is this node a leaf */
	public bool isLeaf;
	/** The number of key => value pairs in this node */
	public int count;
	/** Each node has at least maxNodes-1 and at most 2maxNodes-1 keys */
	private int maxNodes; 
	 	
	/** Create BtreeNode */
	public BTreeNode(int max){
		maxNodes = max;
		isLeaf = true;
		key = new int[2 * maxNodes - 1];
		values = new GameObject[2 * maxNodes - 1];
		childNodes = new BTreeNode[2 * maxNodes];
		count = 0;
   }
	
	/**
	 * Checks if the Btree Node is full
	 * @return boolean
	 */
	public bool isFull(){
		return count==(2*maxNodes-1);
	}
	
	/**
	 * Modified insert that doesn't allow duplicates
	 * @param newKey
	 * @return
	 */
	public bool insert(int newKey, GameObject insertee){
		// Instert new key to current node
		// We make sure that the current node is not full by checking and
		// splitting if necessary before descending to node

		// we need to check if the key already exists
		for( int x = 0; x < count; x++ )
			if( key[x] == newKey ) 
				return false;
		
		int i = count - 1;
		if (isLeaf) { //if this node is a leaf we need to insert this key into one of the avaliable spaces
			while ((i >= 0) && (newKey < key[i])) { // move all keys to the right until we find where this new key can fit
				key[i+1] = key[i];
				values[i+1] = values[i];
				i--;
			}
			count++;
			key[i + 1] = newKey;
			values[i + 1] = insertee;
		} else{
			while ((i >= 0)&& (newKey < key[i])) { // find where this key fits
				i--;
			}

			int insertChild = i + 1;  // Subtree where new key must be inserted
			if (childNodes[insertChild].isFull()){
				// The root of the subtree where new key will be inserted has to be split
				// We promote the mediand of that root to the current node and
				// update keys and references accordingly
			
				//System.out.println("This is the full node we're going to break ");// Debugging code
				//c[insertChild].printNodes();
				//System.out.println("going to promote " + c[insertChild].key[T-1]);
				count++;
				childNodes[count]=childNodes[count-1];
				for(int j = count - 1; j > insertChild; j--){
					childNodes[j] =childNodes[j-1];
					key[j] = key[j-1];
				}
				key[insertChild]= childNodes[insertChild].key[maxNodes-1];
				childNodes[insertChild].count = maxNodes-1;
				
				BTreeNode newNode = new BTreeNode(maxNodes);
				for(int k = 0; k < maxNodes - 1; k++){
					newNode.childNodes[k] = childNodes[insertChild].childNodes[k + maxNodes];
					newNode.key[k] = childNodes[insertChild].key[k + maxNodes];
				}
 
				newNode.childNodes[maxNodes - 1] = childNodes[insertChild].childNodes[2 * maxNodes - 1];
				newNode.count = maxNodes - 1;
				newNode.isLeaf = childNodes[insertChild].isLeaf;
				childNodes[insertChild + 1] = newNode;
						
				if (newKey <key[insertChild])
					return childNodes[insertChild].insert(newKey, insertee);
				else
					return childNodes[insertChild + 1].insert(newKey, insertee);
			} else
				return childNodes[insertChild].insert(newKey, insertee);
		}
		return true;
	}
		
}

/* This is the Master Btree class, I took the code from my java implmentation */
class BTree{
	private BTreeNode root;
	private int T; //2T is the maximum number of childen a node can have
	private int height;
	
	private int keys;
	private int nodes;
	
	public BTree(int t){
		root = new BTreeNode(t);
		T = t;
		height = 0;
		keys = 0;
		nodes = 1;
	}
	
	/**
	 * inserts a new key into the Btree
	 * @param newKey
	 */
	public void insert(int newKey, GameObject inserteee){
		Debug.LogWarning("INSERTING: " + newKey.ToString());
		if (root.isFull()){//Split root;
			split();
			height++;
		}
		if( root.insert(newKey, inserteee) )
			keys++;
		nodes = calculateNumberOfNodes(root);
	}
	
	/**
	 * splits the root into 3 nodes
	 */
	public void split(){
		// Splits the root into three nodes.
		// The median element becomes the only element in the root
		// The left subtree contains the elements that are less than the median
		// The right subtree contains the elements that are larger than the median
		// The height of the tree is increased by one

		//System.out.println("Before splitting root");
		//root.printNodes(); // Code used for debugging
		BTreeNode leftChild = new BTreeNode(T);
		BTreeNode rightChild = new BTreeNode(T);
		leftChild.isLeaf = root.isLeaf;
 		rightChild.isLeaf = root.isLeaf;
		leftChild.count = T-1;
		rightChild.count = T-1;
		int median = T-1;
		for (int i = 0;i<T-1;i++){
			leftChild.childNodes[i] = root.childNodes[i];
			leftChild.key[i] = root.key[i];
		}
		leftChild.childNodes[median]= root.childNodes[median];
		for (int i = median+1;i<root.count;i++){
			rightChild.childNodes[i-median-1] = root.childNodes[i];
			rightChild.key[i-median-1] = root.key[i];
		}
		rightChild.childNodes[median]=root.childNodes[root.count];
		root.key[0]=root.key[median];
		root.count = 1;
		root.childNodes[0]=leftChild;
		root.childNodes[1]=rightChild;
		root.isLeaf = false;
		//System.out.println("After splitting root");
		//root.printNodes();
	}
	
	/**
	 * Calculates the total number of nodes in the BTree
	 * @param H
	 * @return
	 */
	private int calculateNumberOfNodes(BTreeNode H) {
		int count = 1;
		if( !H.isLeaf ) {
			for( int i = 0; i <= H.count; i++ )
				count += calculateNumberOfNodes(H.childNodes[i]);
		}
		return count;
	}

	public BTreeNode getHead() {
		return root;
	}
	
	/**
	 * recursive printer that prints out in a descending order
	 * @param H The Balanced Tree node
	 */
	public void dscOrder(BTreeNode H) {
		if(H.isLeaf) {
			for(int i = H.count-1; i>= 0; i-- ) {
				Debug.Log(H.key[i] + " ");
			}
		} else {
			for( int i = H.count; i >= 0; i-- ) {
				if( i < H.count ) {
					Debug.Log(H.key[i] + " ");
				}
				dscOrder(H.childNodes[i]);
			}
		}
	}
	
	// /**
	//  * number of nodes at depth d
	//  * @param d depth to go to
	//  * @return int number of nodes
	//  */
	// public int DepthNodes(int d) {
	// 	if( d < 0 || d > height)
	// 		return 0;
	// 	return recursiveDepthNodes(root,d);
	// }
	
	// /**
	//  * Number of nodes at depth d
	//  * @param H BTreeNode
	//  * @param d depth to get nodes
	//  * @return int number of nodes
	//  */
	// private int recursiveDepthNodes(BTreeNode H, int d) {
	// 	if( d == 0 )
	// 		return 1;
	// 	else if( d != 0 && H.isLeaf )
	// 		return 0;
	// 	else {
	// 		int count = 0;
	// 		for( int i = 0; i <= H.count; i++ )
	// 			count += recursiveDepthNodes(H.childNodes[i], d-1);
	// 		return count;
	// 	}
	// }
	
	// /**
	//  * will return the number of nodes with n keys
	//  * @param n keys we want to find
	//  * @return int number of nodes having n keys
	//  */
	// public int nNodes(int n) {
	// 	if( n > root.key.Length || ( n < ((root.key.Length+1)/2-1) && root.count != n) )
	// 		return 0;
	// 	if( n < ((root.key.Length+1)/2-1) && root.count == n)
	// 		return 1;
	// 	return recursiveNNodes(root,n);
	// }
	
	// /**
	//  * recursive element of n number of key nodes method
	//  * @param H BTreeNode
	//  * @param n int number of keys in node we want to find
	//  * @return int number of nodes that fit n
	//  */
	// private int recursiveNNodes(BTreeNode H, int n) {
	// 	int count = 0;
	// 	if( H.count == n )
	// 		count++;
	// 	if( !H.isLeaf )
	// 		for( int i = 0; i <= H.count; i++ )
	// 			count += recursiveNNodes(H.childNodes[i], n);
	// 	return count;
	// }
	
	// /**
	//  * return the number of nodes with the minimum number of Keys
	//  * @return int number of nodes with min keys
	//  */
	// public int minimumNodes() {
	// 	if( keys == 0 )
	// 		return 0;
	// 	return recursiveMin(root);
	// }
	
	// /**
	//  * recursive element of minimum number of keys method
	//  * @param H BTreeNode
	//  * @return int number of keys
	//  */
	// private int recursiveMin(BTreeNode H) {
	// 	int count = 0;
	// 	if(H.count == ((H.key.Length+1)/2-1))
	// 		count ++;
	// 	if(!H.isLeaf)
	// 		for( int i = 0; i<=H.count; i++ )
	// 			count += recursiveMin(H.childNodes[i]);
	// 	return count;
			
	// }
	
	// /**
	//  * Will return the number of full nodes in the tree
	//  * @return int number of full nodes
	//  */
	// public int fullNodes() {
	// 	if( keys == 0 )
	// 		return 0;
	// 	return recursiveFullNodes(root);
	// }
	
	// /**
	//  * recursive elment of the number of full nodes method
	//  * @param H BTreeNode
	//  * @return int number of full nodes
	//  */
	// private int recursiveFullNodes(BTreeNode H) {
	// 	int count = 0;
	// 	if( H.count == H.key.Length )
	// 		count ++;
	// 	if( !H.isLeaf )
	// 		for( int i = 0; i < H.count; i++ )
	// 			count += recursiveFullNodes(H.childNodes[i]);
	// 	return count;
	// }
	
	// /**
	//  * will return the total number of leaves in the tree
	//  * @return int number of leaves
	//  */
	// public int numOfLeaves() {
	// 	return recursiveLeaves(root);
	// }
	
	// /**
	//  * recursive element of the number of leaves method
	//  * @param H BTreeNode
	//  * @return num of leaves
	//  */
	// private int recursiveLeaves(BTreeNode H) {
	// 	if( H.isLeaf )
	// 		return 1;
	// 	else {
	// 		int count = 0;
	// 		for( int i = 0; i <= H.count; i++ )
	// 			count += recursiveLeaves(H.childNodes[i]);
	// 		return count;
	// 	}
	// }
	
	// /**
	//  * Returns the sum of all keys in the tree
	//  * @return int sum of keys
	//  */
	// public int sumOfKeys() {
	// 	return recursiveSum(root);
	// }
	
	// /**
	//  * recursive element of the sum of keys method
	//  * @param H BTreeNode
	//  * @return int sum of all keys
	//  */
	// private int recursiveSum(BTreeNode H) {
	// 	int count = 0;
	// 	if( H.isLeaf ) {
	// 		for( int i = 0; i < H.count; i++ )
	// 			count += H.key[i];
	// 		return count;
	// 	} else {
	// 		for( int i = 0; i <= H.count; i++ ) {
	// 			if( i < H.count )
	// 				count+= H.key[i];
	// 			count+= recursiveSum(H.childNodes[i]);
	// 		}
	// 		return count;
	// 	}
	// }
	
	// /**
	//  * will return the number of keys in the tree
	//  * @return int number of keys
	//  */
	// public int numberOfKeys() {
	// 	return keys;
	// }
	
	// /**
	//  * will return the number of nodes in the tree
	//  * @return int number of nodes
	//  */
	// public int numberOfNodes() {
	// 	return nodes;
	// }
	
	// /**
	//  * will find the largest element in the tree
	//  * @return int - element
	//  */
	// public int maximumElement() {
	// 	if( keys == 0 )
	// 		return -1;
	// 	return recursiveMaximum(root);
	// }
	
	// /**
	//  * recursive element of the maximum element finder
	//  * @param H BtreeNode
	//  * @return int of the element
	//  */
	// private int recursiveMaximum(BTreeNode H) {
	// 	if(H.isLeaf)
	// 		return H.key[H.count-1];
	// 	else
	// 		return recursiveMaximum(H.childNodes[H.count]);
	// }
	
	// /**
	//  * will find the smallest element in the tree
	//  * @return int - element
	//  */
	// public int minimumElement() {
	// 	if( keys == 0 )
	// 		return -1;
	// 	return recursiveMinimum(root);
	// }
	
	// /**
	//  * recursive element of the minimum element finder
	//  * @param H BtreeNode
	//  * @return int of the element
	//  */
	// private int recursiveMinimum(BTreeNode H) {
	// 	if(H.isLeaf)
	// 		return H.key[0];
	// 	else
	// 		return recursiveMinimum(H.childNodes[0]);
	// }
	
	/**
	 * Will find a element K in the tree
	 * @param k the int to find
	 * @return True if K is found False otherwise
	 */
	public bool findK(int k) {
		if( keys == 0 )
			return false;
		return recursiveK(root, k);
	}
	
	/**
	 * The recursive method that takes in an BTreeNode to search
	 * @param H BTreeNode
	 * @param k int
	 * @return boolean true if found false otherwise
	 */
	private bool recursiveK(BTreeNode H, int k) {
		if( H.isLeaf ) {
			for( int i =0; i < H.count; i++ ) {
				if( H.key[i] > k )
					return false;
				else if( H.key[i] == k )
					return true;
			}
		} else {
			for( int i = 0; i < H.count; i++ ) {
				if(H.key[i] == k )
					return true;
				else if( H.key[i] > k )
					return recursiveK(H.childNodes[i], k);
			}
			return recursiveK(H.childNodes[H.count], k);
		}
		return false;
	}
}