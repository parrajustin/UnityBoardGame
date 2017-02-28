using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class pieceController : MonoBehaviour {

	#region piece variables
		public Vector3 moveToLocation;
		private GameController gc = null;
		public GameController controller {
			set {
				gc = value;
			}
		}

		public int X;
		public int x {
			get {
				return X;
			}
			set {
				X = value;
			}
		}
		public int Z;
		public int z {
			get {
				return Z;
			}
			set {
				Z = value;
			}
		}

		private int lastTurn = -1;
		private bool mustDie = false;
	#endregion
	
	//     mmmmmmm    mmmmmmm      ooooooooooo vvvvvvv           vvvvvvv eeeeeeeeeeee    
	//   mm:::::::m  m:::::::mm  oo:::::::::::oov:::::v         v:::::vee::::::::::::ee  
	//  m::::::::::mm::::::::::mo:::::::::::::::ov:::::v       v:::::ve::::::eeeee:::::ee
	//  m::::::::::::::::::::::mo:::::ooooo:::::o v:::::v     v:::::ve::::::e     e:::::e
	//  m:::::mmm::::::mmm:::::mo::::o     o::::o  v:::::v   v:::::v e:::::::eeeee::::::e
	//  m::::m   m::::m   m::::mo::::o     o::::o   v:::::v v:::::v  e:::::::::::::::::e 
	//  m::::m   m::::m   m::::mo::::o     o::::o    v:::::v:::::v   e::::::eeeeeeeeeee  
	//  m::::m   m::::m   m::::mo::::o     o::::o     v:::::::::v    e:::::::e           
	//  m::::m   m::::m   m::::mo:::::ooooo:::::o      v:::::::v     e::::::::e          
	//  m::::m   m::::m   m::::mo:::::::::::::::o       v:::::v       e::::::::eeeeeeee  
	//  m::::m   m::::m   m::::m oo:::::::::::oo         v:::v         ee:::::::::::::e  
	//  mmmmmm   mmmmmm   mmmmmm   ooooooooooo            vvv            eeeeeeeeeeeeee  
	/** Move this piece based on the dir, 0 = Positive Y, 1 = Postive X, ...etc */
	/// <summary>
	/// Move this piece a certain direction
	/// </summary>
	/// <param name="dir">the direction to move the piece, 0 = Positive Z, 1 = Positive X, 2 = Negative Z, 3 = Negative X</param>
	/// <returns>boolean if it was able to move</returns>
	public bool move(int dir, ref pieceController[] pieces) {
		pieceController[] piece_array = (pieces != null? pieces : new pieceController[]{});

		if( lastTurn == gc.currentTurn ) { // I already took a turn
			pieces = piece_array;
			return false;
		} else
			lastTurn = gc.currentTurn;

		moveToLocation = this.gameObject.transform.position; // Moved here because it was causing a bug where it was before

		int x = this.x;
		int z = this.z;

		// set how the piece is gonna move
		if( dir == 0 )
			z += 1;
		else if( dir == 1 )
			x += 1;
		else if( dir == 2 )
			z -= 1;
		else
			x -= 1;

		pieceController temp = null;
		if( gc.store.TryGetValue(new Vector2(x, z).ToString(), out temp) && !(x < 0 || z < 0 || x >= gc.gameBoardSize || z >= gc.gameBoardSize) ) {
			// team 4 are the white structures that can't move
			// also move the other object first see if it can
			if( temp.gameObject.tag == "team4" || !temp.move(dir, ref piece_array) ) {
				pieces = piece_array;
				return false;
			}
		}

		if( dir == 0 )
			moveToLocation += (new Vector3(0.0f, 0.0f, 1.25f));
		else if( dir == 1 )
			moveToLocation += (new Vector3(1.25f, 0.0f, 0.0f));
		else if( dir == 2 )
			moveToLocation += (-new Vector3(0.0f, 0.0f, 1.25f));
		else
			moveToLocation += (-new Vector3(1.25f, 0.0f, 0.0f));

		gc.store.Remove(new Vector2(this.x, this.z).ToString()); // remove this entry from the array

		this.x = x; // update with new locations
		this.z = z;
		
		if( this.x < 0 || this.z < 0 || this.x >= gc.gameBoardSize || this.z >= gc.gameBoardSize ) { // the object was pushed out of bounds and needs to die
			int teamTemp = -1;
			if( int.TryParse(this.gameObject.tag.Split('m')[1], out teamTemp) ) 
				gc.reduceLives(teamTemp); 
			// Destroy(this.gameObject);
			mustDie = true;
		} else 
			gc.store.Add(new Vector2(this.x, this.z).ToString(), this);

		// add this pieceController to the pieceController array so that we can move all the pieces in unison
		Array.Resize<pieceController>(ref piece_array, piece_array.Length + 1);
		piece_array[piece_array.Length-1] = this;
		pieces = piece_array;
		return true;
	}

	/// <summary>
	/// Returns the Vector3 of where this piece is going to move to
	/// </summary>
	/// <returns>next location</returns>
	public Vector3 getMoveTo() {
		return moveToLocation;
	}

	/// <summary>
	/// If this piece needs to die
	/// </summary>
	public void finishMove() {
		if( mustDie ) {
			this.gameObject.tag = "dead";
			Destroy(this.gameObject);
		}
	}
}
