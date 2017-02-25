using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pieceController : MonoBehaviour {

	public int X;
	public int x {
		get {
			return X;
		}
		set {
			if( value < 0 )
				X = 0;
			else
				X = value;
		}
	}
	public int Y;
	public int y {
		get {
			return Y;
		}
		set {
			if( value < 0 )
				Y = 0;
			else
				Y = value;
		}
	}
	

}
