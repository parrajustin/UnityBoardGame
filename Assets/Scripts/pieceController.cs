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
		private int Z;
		public int z {
			get {
				return Z;
			}
			set {
				Z = value;
			}
		}
	#endregion
	
	/** Move this piece based on the dir, 0 = Positive Y, 1 = Postive X, ...etc */
	/// <summary>
	/// Move this piece a certain direction
	/// </summary>
	/// <param name="dir">the direction to move the piece, 0 = Positive Z, 1 = Positive X, 2 = Negative Z, 3 = Negative X</param>
	/// <returns>boolean if it was able to move</returns>
	public bool move(int dir = 0) {
		// TODO: Some bug causes this I need to fix it
		if( this.gameObject.tag == "dead" )
			return true;

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
			int.TryParse(this.gameObject.tag.Split('m')[1], out teamTemp);
			gc.reduceLives(teamTemp);
			this.gameObject.tag = "dead";
			this.gameObject.GetComponentInChildren<MeshRenderer>().material = gc.getGrey;
		} else 
			gc.store.Add(new Vector2(this.x, this.z).ToString(), this);

		return true;
	}

}
