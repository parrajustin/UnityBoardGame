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
		Material Grey;
		public Material getGrey{
			get{
				return Grey;
			}
		}
		private GameObject floorPrefab; // Reference to the floor prefab
		private GameObject piecePrefab; // Reference to the game piece prefab
		private GameObject teamPrefab; // Reference to the team prefab
	#endregion

	#region Create Game pieces managment
		private GameObject[,] floorArray; // 2d array to hold the floor peices
		public SortedList<string, pieceController> store;
	#endregion

	#region ui variables
		private Text livesText;
		private Text currentTurnText;
		private Text spawnCountDownText;
	#endregion

	#region team variables
		/// <summary>
		/// The amount of lives a team has left
		/// </summary>
		private int[] teamLives;
		/// <summary>
		/// Time left till a team spawns a new unit
		/// </summary>
		private int[] teamSpawnCountDown;
		/// <summary>
		/// The current turn number
		/// </summary>
		public int currentTurn;
		/// <summary>
		/// Which team went last
		/// </summary>
		private int lastTeamTurn;
		/// <summary>
		/// References to each team's controller
		/// </summary>
		private teamController[] teams;
		/// <summary>
		/// Is a team currently taking a turn
		/// </summary>
		private bool teamTakingTurn = false;
	#endregion

	/// <summary>
	/// The defined board size
	/// </summary>
	public int gameBoardSize = 7; 

	//                                                                                                      
	//                            tttt                                                        tttt          
	//                         ttt:::t                                                     ttt:::t          
	//                         t:::::t                                                     t:::::t          
	//                         t:::::t                                                     t:::::t          
	//      ssssssssss   ttttttt:::::ttttttt      aaaaaaaaaaaaa  rrrrr   rrrrrrrrr   ttttttt:::::ttttttt    
	//    ss::::::::::s  t:::::::::::::::::t      a::::::::::::a r::::rrr:::::::::r  t:::::::::::::::::t    
	//  ss:::::::::::::s t:::::::::::::::::t      aaaaaaaaa:::::ar:::::::::::::::::r t:::::::::::::::::t    
	//  s::::::ssss:::::stttttt:::::::tttttt               a::::arr::::::rrrrr::::::rtttttt:::::::tttttt    
	//   s:::::s  ssssss       t:::::t              aaaaaaa:::::a r:::::r     r:::::r      t:::::t          
	//     s::::::s            t:::::t            aa::::::::::::a r:::::r     rrrrrrr      t:::::t          
	//        s::::::s         t:::::t           a::::aaaa::::::a r:::::r                  t:::::t          
	//  ssssss   s:::::s       t:::::t    tttttta::::a    a:::::a r:::::r                  t:::::t    tttttt
	//  s:::::ssss::::::s      t::::::tttt:::::ta::::a    a:::::a r:::::r                  t::::::tttt:::::t
	//  s::::::::::::::s       tt::::::::::::::ta:::::aaaa::::::a r:::::r                  tt::::::::::::::t
	//   s:::::::::::ss          tt:::::::::::tt a::::::::::aa:::ar:::::r                    tt:::::::::::tt
	//    sssssssssss              ttttttttttt    aaaaaaaaaa  aaaarrrrrrr                      ttttttttttt  
	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Start () {
		GameObject[] uiobjs = GameObject.FindGameObjectsWithTag("ui.text");

		foreach(GameObject obj in uiobjs) {
			if( obj.gameObject.name == "lives" )
				livesText = obj.GetComponent<Text>();
			else if( obj.gameObject.name == "currTurn" )
				currentTurnText = obj.GetComponent<Text>();
			else
				spawnCountDownText = obj.GetComponent<Text>();
		}

		teamLives = new int[]{5, 5, 5, 5};
		teamSpawnCountDown = new int[]{5, 5, 5, 5};
		currentTurn = 0;
	}

	//                                                                          kkkkkkkk                            
	//                                                                          k::::::k                            
	//                                                                          k::::::k                            
	//                                                                          k::::::k                            
	//    aaaaaaaaaaaaawwwwwww           wwwww           wwwwwwwaaaaaaaaaaaaa    k:::::k    kkkkkkk eeeeeeeeeeee    
	//    a::::::::::::aw:::::w         w:::::w         w:::::w a::::::::::::a   k:::::k   k:::::kee::::::::::::ee  
	//    aaaaaaaaa:::::aw:::::w       w:::::::w       w:::::w  aaaaaaaaa:::::a  k:::::k  k:::::ke::::::eeeee:::::ee
	//             a::::a w:::::w     w:::::::::w     w:::::w            a::::a  k:::::k k:::::ke::::::e     e:::::e
	//      aaaaaaa:::::a  w:::::w   w:::::w:::::w   w:::::w      aaaaaaa:::::a  k::::::k:::::k e:::::::eeeee::::::e
	//    aa::::::::::::a   w:::::w w:::::w w:::::w w:::::w     aa::::::::::::a  k:::::::::::k  e:::::::::::::::::e 
	//   a::::aaaa::::::a    w:::::w:::::w   w:::::w:::::w     a::::aaaa::::::a  k:::::::::::k  e::::::eeeeeeeeeee  
	//  a::::a    a:::::a     w:::::::::w     w:::::::::w     a::::a    a:::::a  k::::::k:::::k e:::::::e           
	//  a::::a    a:::::a      w:::::::w       w:::::::w      a::::a    a:::::a k::::::k k:::::ke::::::::e          
	//  a:::::aaaa::::::a       w:::::w         w:::::w       a:::::aaaa::::::a k::::::k  k:::::ke::::::::eeeeeeee  
	//   a::::::::::aa:::a       w:::w           w:::w         a::::::::::aa:::ak::::::k   k:::::kee:::::::::::::e  
	//    aaaaaaaaaa  aaaa        www             www           aaaaaaaaaa  aaaakkkkkkkk    kkkkkkk eeeeeeeeeeeeee  
	/// <summary>
	/// This runs before any start function in any other script
	/// </summary>
	void Awake() {
		// setup bTree storage
		store = new SortedList<string, pieceController>();

		// Load up the prefabs that are used in this game
		floorPrefab = Resources.Load("Prefab/gameFloor") as GameObject;
		piecePrefab = Resources.Load("Prefab/gamePiece") as GameObject;
		teamPrefab = Resources.Load("Prefab/teamController") as GameObject;
		
		Red = Resources.Load("Materials/Red") as Material;
		Blue = Resources.Load("Materials/Blue") as Material;
		Green = Resources.Load("Materials/Green") as Material;
		Yellow = Resources.Load("Materials/Yellow") as Material;
		Grey = Resources.Load("Materials/Grey") as Material;

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

		// Create team masters
		teams = new teamController[4];
		for( int i = 0; i < teams.Length; i++ ) {
			GameObject temp = Instantiate(teamPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			teamController t = temp.GetComponent<teamController>();

			teams[i] = t;
			t.team = i;
			t.name = "teamController_" + i.ToString();
		}
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

	public void reduceLives(int team) {
		teamLives[team] -= 1;
	}
	
	//                                                   dddddddd                                                            
	//                                                   d::::::d                          tttt                              
	//                                                   d::::::d                       ttt:::t                              
	//                                                   d::::::d                       t:::::t                              
	//                                                   d:::::d                        t:::::t                              
	//  uuuuuu    uuuuuu ppppp   ppppppppp       ddddddddd:::::d   aaaaaaaaaaaaa  ttttttt:::::ttttttt        eeeeeeeeeeee    
	//  u::::u    u::::u p::::ppp:::::::::p    dd::::::::::::::d   a::::::::::::a t:::::::::::::::::t      ee::::::::::::ee  
	//  u::::u    u::::u p:::::::::::::::::p  d::::::::::::::::d   aaaaaaaaa:::::at:::::::::::::::::t     e::::::eeeee:::::ee
	//  u::::u    u::::u pp::::::ppppp::::::pd:::::::ddddd:::::d            a::::atttttt:::::::tttttt    e::::::e     e:::::e
	//  u::::u    u::::u  p:::::p     p:::::pd::::::d    d:::::d     aaaaaaa:::::a      t:::::t          e:::::::eeeee::::::e
	//  u::::u    u::::u  p:::::p     p:::::pd:::::d     d:::::d   aa::::::::::::a      t:::::t          e:::::::::::::::::e 
	//  u::::u    u::::u  p:::::p     p:::::pd:::::d     d:::::d  a::::aaaa::::::a      t:::::t          e::::::eeeeeeeeeee  
	//  u:::::uuuu:::::u  p:::::p    p::::::pd:::::d     d:::::d a::::a    a:::::a      t:::::t    tttttte:::::::e           
	//  u:::::::::::::::uup:::::ppppp:::::::pd::::::ddddd::::::dda::::a    a:::::a      t::::::tttt:::::te::::::::e          
	//   u:::::::::::::::up::::::::::::::::p  d:::::::::::::::::da:::::aaaa::::::a      tt::::::::::::::t e::::::::eeeeeeee  
	//    uu::::::::uu:::up::::::::::::::pp    d:::::::::ddd::::d a::::::::::aa:::a       tt:::::::::::tt  ee:::::::::::::e  
	//      uuuuuuuu  uuuup::::::pppppppp       ddddddddd   ddddd  aaaaaaaaaa  aaaa         ttttttttttt      eeeeeeeeeeeeee  
	//                    p:::::p                                                                                            
	//                    p:::::p                                                                                            
	//                   p:::::::p                                                                                           
	//                   p:::::::p                                                                                           
	//                   p:::::::p                                                                                           
	//                   ppppppppp                                                                                           
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	void Update () {
		guiUpdate(); // This updates the gui every update frame

		if( !teamTakingTurn ) {
			teamTakingTurn = true;

			int teamTurn; // the team that will be going
			teamUpdates(out teamTurn); // handles a team spawns and live
			takeTurn(teamTurn);
		}

		// // set this to a team number to allow for player control
		// if( teamTurn == 9 ) {
			// if( Input.GetKeyDown(KeyCode.W) ) {
			// 	foreach(GameObject obj in GameObject.FindGameObjectsWithTag("team1")) {
			// 		pieceController pc = obj.GetComponent<pieceController>();
			// 		pc.move(0);
			// 	}
		// 		teamSpawnCountDown[teamTurn] -= 1;
		// 		currentTurn++;
		// 	} else if( Input.GetKeyDown(KeyCode.S) ) {
		// 		foreach(GameObject obj in GameObject.FindGameObjectsWithTag("team1")) {
		// 			pieceController pc = obj.GetComponent<pieceController>();
		// 			pc.move(2);
		// 		}
		// 		teamSpawnCountDown[teamTurn] -= 1;
		// 		currentTurn++;
		// 	} else if( Input.GetKeyDown(KeyCode.A) ) {
		// 		foreach(GameObject obj in GameObject.FindGameObjectsWithTag("team1")) {
		// 			pieceController pc = obj.GetComponent<pieceController>();
		// 			pc.move(3);
		// 		}
		// 		teamSpawnCountDown[teamTurn] -= 1;
		// 		currentTurn++;
		// 	} else if( Input.GetKeyDown(KeyCode.D) ) {
		// 		foreach(GameObject obj in GameObject.FindGameObjectsWithTag("team1")) {
		// 			pieceController pc = obj.GetComponent<pieceController>();
		// 			pc.move(1);
		// 		}
		// 		teamSpawnCountDown[teamTurn] -= 1;
		// 		currentTurn++;
		// 	}
		// } else {
		// 	int dir = -1;

		// 	foreach(GameObject obj in GameObject.FindGameObjectsWithTag("team" + teamTurn.ToString())) {
		// 		pieceController pc = obj.GetComponent<pieceController>();
		// 		if( dir == -1 ) {
		// 			Dictionary<int, int> temp = new Dictionary<int, int>();

		// 			if( pc.x != 0 )
		// 				temp.Add(temp.Count, 3);
		// 			if( pc.x != gameBoardSize - 1 )
		// 				temp.Add(temp.Count, 1);
		// 			if( pc.z != 0 ) 
		// 				temp.Add(temp.Count, 2);
		// 			if( pc.z != gameBoardSize - 1 ) 
		// 				temp.Add(temp.Count, 0);

		// 			dir = 0;
		// 			temp.TryGetValue(Random.Range(0, temp.Count), out dir);
		// 			teamSpawnCountDown[teamTurn] -= 1;
		// 			pc.move(dir);
		// 		} else {
		// 			teamSpawnCountDown[teamTurn] -= 1;
		// 			pc.move(dir);
		// 		}
		// 	}

		// 	currentTurn++; //todo: fix this because the team spawns below affects the next team not this team.
		// }
	}

	//      ffffffffffffffff                                                                         
	//     f::::::::::::::::f                                                                        
	//    f::::::::::::::::::f                                                                       
	//    f::::::fffffff:::::f                                                                       
	//    f:::::f       ffffffuuuuuu    uuuuuunnnn  nnnnnnnn        cccccccccccccccc    ssssssssss   
	//    f:::::f             u::::u    u::::un:::nn::::::::nn    cc:::::::::::::::c  ss::::::::::s  
	//   f:::::::ffffff       u::::u    u::::un::::::::::::::nn  c:::::::::::::::::css:::::::::::::s 
	//   f::::::::::::f       u::::u    u::::unn:::::::::::::::nc:::::::cccccc:::::cs::::::ssss:::::s
	//   f::::::::::::f       u::::u    u::::u  n:::::nnnn:::::nc::::::c     ccccccc s:::::s  ssssss 
	//   f:::::::ffffff       u::::u    u::::u  n::::n    n::::nc:::::c                s::::::s      
	//    f:::::f             u::::u    u::::u  n::::n    n::::nc:::::c                   s::::::s   
	//    f:::::f             u:::::uuuu:::::u  n::::n    n::::nc::::::c     cccccccssssss   s:::::s 
	//   f:::::::f            u:::::::::::::::uun::::n    n::::nc:::::::cccccc:::::cs:::::ssss::::::s
	//   f:::::::f             u:::::::::::::::un::::n    n::::n c:::::::::::::::::cs::::::::::::::s 
	//   f:::::::f              uu::::::::uu:::un::::n    n::::n  cc:::::::::::::::c s:::::::::::ss  
	//   fffffffff                uuuuuuuu  uuuunnnnnn    nnnnnn    cccccccccccccccc  sssssssssss    
	//                                                                                                          
	/// <summary>
	/// Handles all the gui updates
	/// </summary>
	private void guiUpdate() {
		if( currentTurnText.text != "Current turn:" +  (currentTurn % 4).ToString() ) {
			currentTurnText.text = "Current turn:" +  (currentTurn % 4).ToString();
		}
		
		if( livesText.text != "Lives left: " + teamLives[1].ToString() ) {
			livesText.text = "Lives left: " + teamLives[1].ToString();
		}
		
		if( spawnCountDownText.text != "Next Spawn: " + teamSpawnCountDown[1].ToString() ) {
			spawnCountDownText.text = "Next Spawn: " + teamSpawnCountDown[1].ToString();
		}
	}

	/// <summary>
	///  Handles team actions such as checking if a team still has lives as well as spawning new pieces
	/// </summary>
	/// <param name="teamNumber">Which team should go</param>
	private void teamUpdates(out int teamNumber) {
		int tempTeam = (lastTeamTurn + 1) % 4; // who should be going next

		// find the team that should go next
		while( teamLives[tempTeam] <= 0) {
			if( tempTeam == lastTeamTurn ) {
				Debug.Break();
				break;
			}

			// we need to edit every game peice belonging to the team since it is dead
			foreach(GameObject obj in GameObject.FindGameObjectsWithTag("team" + tempTeam.ToString())) {
				obj.GetComponentInChildren<MeshRenderer>().material = getGrey; // turn the object a grey color
				obj.tag = "dead"; // set its tag to dead so it can't be found by searching for its tag
			}
			tempTeam = (tempTeam + 1) % 4; // add to who should be going next since this team is dead
		}

		// Check if the team going can spawn a game piece
		if( teamSpawnCountDown[tempTeam] <= 0 ) { // if a count down is 0 or less spawn a unit
			Vector2 query = new Vector2(((tempTeam % 2) * (gameBoardSize - 1)), ((tempTeam / 2) * (gameBoardSize - 1)));
			if( !store.Keys.Contains(query.ToString()) ) { // if there is a piece on on the spawn location we can't spawn this piece
				teamSpawnCountDown[tempTeam] = teamLives[tempTeam];
				this.spawn(tempTeam, (int)query.x, (int)query.y); // spawn the piece
			}
		}

		teamNumber = tempTeam; // a cooler way to return a value, orginally this function returned a bool so this is just here because I don't 
													 // want to change the other code or whatever
	}

	public void finishedTurn(int teamTurn) {
		teamSpawnCountDown[teamTurn] -= 1; // Reduce countdown
		lastTeamTurn = teamTurn; // set this team as the last team to take a turn
		currentTurn++; // increate the turn counter
		teamTakingTurn = false;
	}

	/// <summary>
	/// This lets a team take a turn whether it is ai or player
	/// </summary>
	/// <param name="teamNumber">Which team should go</param>
	private void takeTurn(int teamNumber) {
		teams[teamNumber].turn();	// tell the team controller that the team needs to take its turn
	}
}