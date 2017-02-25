using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public GameObject piece;
	private GameObject other;

	// Use this for initialization
	void Start () {
	}

	private void setUpBoard() {

	}
	
	// Update is called once per frame
	void Update () {
		if( Input.GetKeyDown(KeyCode.W) ) {
			piece.transform.Translate(new Vector3(0.0f, 0.0f, 1.25f));
		} else if( Input.GetKeyDown(KeyCode.S) ) {
			piece.transform.Translate(-new Vector3(0.0f, 0.0f, 1.25f));
		} else if( Input.GetKeyDown(KeyCode.A) ) {
			piece.transform.Translate(-new Vector3(1.25f, 0.0f, 0.0f));
		} else if( Input.GetKeyDown(KeyCode.D) ) {
			piece.transform.Translate(new Vector3(1.25f, 0.0f, 0.0f));
		}
	}
}
