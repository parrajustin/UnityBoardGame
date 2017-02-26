using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pieceController : MonoBehaviour {

	#region piece variables
		private GameController gc = null;
		public GameController controller {
			set {
				gc = value;
			}
		}

		private int X;
		public int x {
			get {
				return X;
			}
			set {
				X = value;
			}
		}
		private int Y;
		public int y {
			get {
				return Y;
			}
			set {
				Y = value;
			}
		}
	#endregion
	
	/** Move this piece based on the dir, 0 = Positive Y, 1 = Postive X, ...etc */
	public bool move(int dir = 0) {
		int x = this.x;
		int y = this.y;

		if( dir == 0 )
			y += 1;
		else if( dir == 1 )
			x += 1;
		else if( dir == 2 )
			y -= 1;
		else
			x -= 1;

		pieceController temp = null;
		gc.store.TryGetValue(x * gc.gameBoardSize + y, out temp);
		if( temp != null && !(this.x < 0 || this.y < 0 || this.x >= gc.gameBoardSize || this.y >= gc.gameBoardSize) ) {
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

		gc.store.Remove(this.x * gc.gameBoardSize + this.y);

		this.x = x;
		this.y = y;

		if( this.x < 0 || this.y < 0 || this.x >= gc.gameBoardSize || this.y >= gc.gameBoardSize ) { // the object was pushed out of bounds and needs to die
			this.gameObject.tag = "dead";
			// later add the kill animation here
		} else 
			gc.store.Add(this.x * gc.gameBoardSize + this.y, this);

		return true;
	}

}
