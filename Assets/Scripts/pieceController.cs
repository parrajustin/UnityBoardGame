using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class pieceController : MonoBehaviour {

	#region piece variables
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
	public bool move(int dir = 0) {
		if( lastTurn == gc.currentTurn )
			return false;
		else
			lastTurn = gc.currentTurn;

		int x = this.x;
		int z = this.z;

		if( dir == 0 )
			z += 1;
		else if( dir == 1 )
			x += 1;
		else if( dir == 2 )
			z -= 1;
		else
			x -= 1;

		pieceController temp = null;
		gc.store.TryGetValue(new Vector2(x, z).ToString(), out temp);
		if( temp != null && !(x < 0 || z < 0 || x >= gc.gameBoardSize || z >= gc.gameBoardSize) ) {
			if( temp.gameObject.tag == "team4" ) return false; // team 4 are the white structures that can't move
			if( !temp.move(dir) ) return false; // move the other object first
		}

		if( dir == 0 )
			this.transform.Translate(new Vector3(0.0f, 0.0f, 1.25f));
		else if( dir == 1 )
			this.transform.Translate(new Vector3(1.25f, 0.0f, 0.0f));
		else if( dir == 2 )
			this.transform.Translate(-new Vector3(0.0f, 0.0f, 1.25f));
		else
			this.transform.Translate(-new Vector3(1.25f, 0.0f, 0.0f));

		gc.store.Remove(new Vector2(this.x, this.z).ToString());

		this.x = x;
		this.z = z;
		
		if( this.x < 0 || this.z < 0 || this.x >= gc.gameBoardSize || this.z >= gc.gameBoardSize ) { // the object was pushed out of bounds and needs to die
			// later add the kill animation here
			int teamTemp = -1;
			if( this.gameObject.tag != "dead" ) {
				int.TryParse(this.gameObject.tag.Split('m')[1], out teamTemp);
				gc.reduceLives(teamTemp);
			}
			Destroy(this.gameObject);
		} else {
			try {
				gc.store.Add(new Vector2(this.x, this.z).ToString(), this);
			} catch( Exception e ) {
				Debug.LogError(temp);
				Debug.LogError(e.ToString());
				Debug.LogError(new Vector2(this.x, this.z).ToString());
			}
		}

		return true;
	}

}
