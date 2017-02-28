using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teamController : MonoBehaviour {

	private int finishedPieces = 0;
	private int totalPieces = 0;
	private GameController value_game = null;
	private int value_teamNumber = -1;

	public int team {
		get { return value_teamNumber; }
		set { value_teamNumber = value; }
	}

	public GameController game {
		get { return value_game; }
		set { value_game = value; }
	}

	// Use this for initialization
	void Start () {
		game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
	}

	public void turn() {
		finishedPieces = 0;
		int dir = -1; // which direction should a team go
		bool b;

		// pieceControllers of the pieces to be moved
		pieceController[] pieces = null;

		foreach(GameObject obj in GameObject.FindGameObjectsWithTag("team" + team.ToString())) {
			pieceController pc = obj.GetComponent<pieceController>();
			if( dir == -1 ) {
				int count = 0;
				int[] temp = new int[]{-1, -1, -1, -1};

				if( pc.x != 0 ) {
					temp[count] = 3;
					count++;
				}
				if( pc.x != game.gameBoardSize - 1 ) {
					temp[count] = 1;
					count++;
				}
				if( pc.z != 0 ) {
					temp[count] = 2;
					count++;
				}
				if( pc.z != game.gameBoardSize - 1 ) {
					temp[count] = 0;
					count++;
				}

				dir =  temp[Random.Range(0, count)];

				b = pc.move(dir, ref pieces);
			} else 
				b = pc.move(dir, ref pieces);
		}

		if( pieces != null ) {
			totalPieces = pieces.Length;

			foreach(pieceController piece in pieces) {
				iTween.MoveTo(piece.gameObject, iTween.Hash(
					"position", piece.getMoveTo(),
					"time", 1.0,
					"oncompletetarget", this.gameObject,
					"oncomplete", "endTurn",
					"oncompleteparams", piece
				));
			}
		}
	}

	/// <summary>
	/// Finish this teams turn
	/// </summary>
	public void endTurn(pieceController piece) {
		finishedPieces++;
		piece.finishMove();

		if( finishedPieces == totalPieces )
			game.finishedTurn(team);
	}
}
