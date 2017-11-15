using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class _BattleOp : MonoBehaviour {
	
	public int numChar;
	public List<Character> aliveTeammateList = new List<Character>();
	public List<Character> deadTeammateList = new List<Character>();
	public List<Character> aliveEnemyList = new List<Character>();
	public List<Character> deadEnemyList = new List<Character>();
	public List<Character> allTeammateList = new List<Character>();
	public List<Character> allEnemyList = new List<Character>();
	public Character[] allEntityList;
	private bool playerTurn = false;
	private int currentChar = 0;
	private int currentTarget = 0;
	private int currentMove = 0;
	private int selectedMove = -1;
	private Character attackedPlayer = null;
	public SortedList moveList = new SortedList(new MoveMessengerComparer());
	public int enemyMove;
	public bool hasSelectedMove = false;
	public bool hasSelectedTarget = false;
	GameObject lastKnownInput;
	public GameObject[] meButtons = new GameObject[4];
	GameObject battleLog;
	private string battleLogText = "";
	public int currentPageOfMovesOrTargets = 0;
	private bool pressedEnter = false;
	private bool needToChange = false;
	private int currentHighlight = 0;
	public bool needToPrint = true;
	public bool calledBattleCoroutine = false;

	void Start () {
		lastKnownInput = EventSystem.current.firstSelectedGameObject;
		meButtons [0] = GameObject.Find ("ME 1");
		meButtons [1] = GameObject.Find ("ME 2");
		meButtons [2] = GameObject.Find ("ME 3");
		meButtons [3] = GameObject.Find ("ME 4");
		battleLog = GameObject.Find ("BattleLog");
		battleLog.SetActive (false);
		updateMovesForChar ();

		playerTurn = true;
	}

	void Update () {

		if (EventSystem.current.currentSelectedGameObject == null)
		{
			Debug.Log("Unexpected mouse input... moving cursor back to last known input");
			EventSystem.current.SetSelectedGameObject(lastKnownInput);
		}

		if (!playerTurn) {
			if (Input.GetKeyDown (KeyCode.Return)) pressedEnter = true;
			return;
		}

		if (currentHighlight % 4 == 3 && Input.GetKeyDown(KeyCode.DownArrow)) {
			if (needToChange) {
				if (hasSelectedMove == false) {
					Debug.Log ("Went past move " + currentMove + ", checking if there are more moves available.");
					if (currentMove + 1 < aliveTeammateList [currentChar].getMoveList ().Length) {
						currentPageOfMovesOrTargets++;
						currentMove = 0 + (currentPageOfMovesOrTargets * 4);
						updateMovesForChar ();
						EventSystem.current.SetSelectedGameObject (meButtons [0]);
					}
				}
				else if (hasSelectedTarget == false) {
					Debug.Log ("Went past target " + currentTarget + ", checking if there are more targets available.");
					if (currentTarget + 1 < aliveEnemyList.Capacity) {
						currentPageOfMovesOrTargets++;
						currentTarget = 0 + (currentPageOfMovesOrTargets * 4);
						updateTargetsForChar ();
						EventSystem.current.SetSelectedGameObject (meButtons [0]);
					}
				}
			}
			else needToChange = true;
		}


		if (currentHighlight % 4 == 0 && Input.GetKeyDown(KeyCode.UpArrow)) {
			if (needToChange) {
				if (hasSelectedMove == false) {
					Debug.Log ("Went up past move " + currentMove + ", checking if there are more moves available.");
					if (currentMove > 0) {
						currentPageOfMovesOrTargets--;
						currentMove = 3 + (4 * currentPageOfMovesOrTargets);
						updateMovesForChar ();
						EventSystem.current.SetSelectedGameObject (meButtons [3]);
					}
				} else if (hasSelectedTarget == false) {
					Debug.Log ("Went up past target " + currentTarget + ", checking if there are more targets available.");
					if (currentTarget > 0) {
						currentPageOfMovesOrTargets--;
						currentTarget = 3 + (4 * currentPageOfMovesOrTargets);
						updateTargetsForChar ();
						EventSystem.current.SetSelectedGameObject (meButtons [3]);
					}
				}
			} else needToChange = true;
		}

		if (currentHighlight % 4 == 3 || currentHighlight % 4 == 0) {
			needToChange = true;
		} else
			needToChange = false;

		if (needToPrint) {
			printUpdates ();
		}

		if (Input.GetKeyDown (KeyCode.Backspace)) {
			if (hasSelectedMove == true) {
				selectedMove = -1;
				hasSelectedMove = false;
				currentPageOfMovesOrTargets = 0;
				print ("Reselecting Move...");
			} else {
				if (currentChar == 0) {
					print ("This is the first character, can't go back to last character");
				} else {
					currentChar--;
					print ("Reselecting moves and target for " + aliveTeammateList [currentChar].getName ());
					moveList.RemoveAt (currentChar);
				}
			}
		}
	}

	IEnumerator StartBattling() {
		for (int i = 0; i < 4; i++) {
			meButtons [i].SetActive (false);
		}
		battleLog.SetActive (true);
		playerTurn = false;
		currentChar++;
		print (new Time());
		yield return SelectAIMove ();
		print (new Time());
		//MakeMoves (); method is under this
		print (pressedEnter);
		pressedEnter = false;
		for (int i = 0; i < moveList.Count && moveList[i] != null; i++) {
			print (i);
			MoveMessenger m = (MoveMessenger)(moveList.GetKey (i));
			if (m.getAttackedPlayer ().isAlive () != true) {
				updateBattleLog (m.getAttackingPlayer().getName() + "'s target was dead before move.  Attacking next person in the enemy line");
				yield return chatEnter ();
				int newTarget = m.getIndexOfAttackedPlayer ();
				if (m.getAttackingPlayer ().getTeam() == "team") {
					if (newTarget > aliveEnemyList.Capacity - 1)
						newTarget = 0;
					m.setAttackedPlayer (aliveEnemyList [newTarget]);
				} else {
					if (newTarget > aliveTeammateList.Capacity - 1)
						newTarget = 0;
					m.setAttackedPlayer (aliveTeammateList [newTarget]);
				}
				m.setIndexOfAttackedPlayer (newTarget);
				m.recalculateDamage (m.getAttackingPlayer ().moveList [m.getIndexOfMove ()].getDamage ());
			} 
			updateBattleLog (m.getAttackingPlayer ().getName () + " is attacking " + m.getAttackedPlayer ().getName () + " with damage: " + m.getRecalculatedDamage ());
			updateBattleLog (m.getAttackedPlayer ().getName () + "'s health before the move is: " + m.getAttackedPlayer ().getHealth ());
			int health = m.getAttackedPlayer ().receiveDamage ((MoveMessenger)moveList.GetKey (i));
			updateBattleLog (m.getAttackedPlayer ().getName () + "'s health after the move is: " + health);
			yield return chatEnter ();
			m.getAttackedPlayer ().setHealth (health);
			if (health <= 0) {
				updateBattleLog (m.getAttackedPlayer ().getName () + " has died");
				if (m.getAttackedPlayer ().team == "team") {
					aliveTeammateList.Remove (m.getAttackedPlayer ());
				} else //no need to check if he's enemy, because he will be
					aliveEnemyList.Remove (m.getAttackedPlayer ()); 
				yield return chatEnter ();
				moveList [moveList.IndexOfValue (m.getAttackedPlayer ().getName ())] = null;
				//moveList.RemoveAt (moveList.IndexOfValue (m.getAttackedPlayer ().getName ())); //the line that causes skipped turns
				if (isBattleOver ()) {
					updateBattleLog ("Battle's over.");
					BattleOver ();
				}
			}
		}
		moveList.Clear();
		print("MoveList cleared");
		currentChar = 0; 
		print("CurrentChar has been reset");
		currentPageOfMovesOrTargets = 0;
		battleLog.SetActive (false);
		playerTurn = true;
		updateMovesForChar ();
		EventSystem.current.SetSelectedGameObject(meButtons[0]);
		needToPrint = true;
	}

	//need to revamp it
	private object SelectAIMove (){
		System.Random rnd = new System.Random();
		for (int i = 0; i < aliveEnemyList.Capacity; i++) {
			print("Called SelectAIMove()");
			print("Current Enemy: " + aliveEnemyList[i].getName());
			MoveMessenger messenger = aliveEnemyList[i].makeMove (enemyMove);
			print(messenger.toString());
			int target = rnd.Next(0, aliveTeammateList.Capacity);
			print("Target is Teammate #: " + target);
			messenger.setAttackedPlayer(aliveTeammateList[target]);
			FillMoveList(messenger);
			currentChar++;
		}
		return null;
	}

	private void FillMoveList(MoveMessenger m){
		moveList.Add (m, allEntityList [currentChar].name);
		for(int i = 0; i < moveList.Count; i++) {
			MoveMessenger mm = (MoveMessenger) (moveList.GetKey (i));
			print((i+1) + " move(s) in list:" + " " + "Attacking Player: " + mm.getAttackingPlayer () + " " + "AttackedPlayer: " + mm.getAttackedPlayer () + " " + "Move Speed: " + mm.getSpeed () + " " + "Move Damage: " + mm.getRecalculatedDamage ());
		}
	}

	private object chatEnter () {
		while (didPressEnter ()) {
			
		}
		resetBattleLog ();
		print (pressedEnter);
		pressedEnter = false;
		return null;
	}

	/**
	 * 
	 * Functions to make reading code easier
	 * 
	 **/
	private void updateMovesForChar() {
		meButtons [0].SetActive (false);
		meButtons [1].SetActive (false);
		meButtons [2].SetActive (false);
		meButtons [3].SetActive (false);
		if (aliveTeammateList [currentChar].getMoveList ().Length < 5) {
			for (int a = 0; a < aliveTeammateList[currentChar].getMoveList ().Length; a++) {
				meButtons [a].SetActive (true);
			}
		} else if (aliveTeammateList [currentChar].getMoveList ().Length > 4) {
			for (int a = 0; a < 4 && a + (currentPageOfMovesOrTargets * 4) < aliveTeammateList [currentChar].getMoveList ().Length; a++) {
				meButtons [a].SetActive (true);
			}
		}
	}

	private void updateTargetsForChar() {
		meButtons [0].SetActive (false);
		meButtons [1].SetActive (false);
		meButtons [2].SetActive (false);
		meButtons [3].SetActive (false);
		if (aliveEnemyList.Capacity < 5) {
			for (int a = 0; a < aliveEnemyList.Capacity; a++) {
				meButtons [a].SetActive (true);
			}
		} else if (aliveEnemyList.Capacity > 4) {
			for (int a = 0; a < 4 && a + (currentPageOfMovesOrTargets * 4) < aliveEnemyList.Capacity; a++) {
				meButtons [a].SetActive (true);
			}
		}
	}

	private void callBattleCoroutine() {
		calledBattleCoroutine = true;
		StartCoroutine (StartBattling());
		calledBattleCoroutine = false;
	}

	/// <summary>
	/// This section covers the events of the battling. 
	/// </summary>

	public void eventHighlightME (int a) {
		lastKnownInput = EventSystem.current.currentSelectedGameObject;
		currentHighlight = a + (currentPageOfMovesOrTargets * 4);
		if (hasSelectedMove == false) {
			currentMove = a + (currentPageOfMovesOrTargets*4);
		}
		if (hasSelectedMove == true) {
			currentTarget = a + (currentPageOfMovesOrTargets*4);
		}
	}

	public void eventSelectME (int a) {
		if (hasSelectedMove == false) {
			hasSelectedMove = true;
			selectedMove = a + (currentPageOfMovesOrTargets*4);
			currentTarget = 0;
			currentPageOfMovesOrTargets = 0;
			updateTargetsForChar();
		}
		else if (hasSelectedTarget == false) {
			hasSelectedTarget = true;
			attackedPlayer = aliveEnemyList [a + (currentPageOfMovesOrTargets * 4)];
		}
		if (hasSelectedMove == true && hasSelectedTarget == true) {
			//OnSelectMove();
			currentTarget = 0;
			currentMove = 0;
			currentPageOfMovesOrTargets = 0;
			selectedMove = -1;
			hasSelectedTarget = false;
			hasSelectedMove = false;
			attackedPlayer = null;
			updateMovesForChar ();
		}
	}

	public string printUpdates() {
		if (hasSelectedMove == false) {
			return ("Selecting moves for " + aliveTeammateList [currentChar].getName () + ".  Current move is " + aliveTeammateList [currentChar].moveList [currentMove].name);
		}
		if (hasSelectedMove && hasSelectedTarget == false) {
			return ("Selecting target for " + aliveTeammateList [currentChar].getName () + ".  Current target is " + aliveEnemyList [currentTarget].getName ());
		}
		return null;
	}
		
	private bool didPressEnter() {
		return !pressedEnter;
	}

	/// <summary>>
	/// End Battle stuff
	/// </summary>


	private bool isBattleOver(){
		if (didEnemyWin ()) {
			updateBattleLog ("You lose.");
			return true;
		}
		if (didTeamWin ()) {
			updateBattleLog ("You win.");
			return true;
		}
		return false;
	}
	//Need to make sure that when a target dies and is moved to dead enemy list, if anyone has a
	//number bigger than capacity, it has to be moved down to capacity - 1.
	private bool didEnemyWin() {
		bool friendlyTeamDead = true; // Set default to everyone's dead
		if (aliveTeammateList.Capacity > 0) {
			friendlyTeamDead = false;
		}
		return friendlyTeamDead;
	}

	private bool didTeamWin() {
		bool enemyTeamDead = true; // Set default to everyone's dead
		if (aliveEnemyList.Capacity > 0) {
			enemyTeamDead = false;
		}
		return enemyTeamDead;
	}

	private void BattleOver(){
		if (didTeamWin()) {
			distributeEXP();
			Fade();
		}
		if (didEnemyWin()) {
			gameOver();
			Fade();
		}
	}

	public void Fade() {
		GameObject go = GameObject.FindGameObjectWithTag ("Fader");
		ScreenFader sf = go.GetComponent<ScreenFader> ();
		sf.FadeToBlack ();
		SceneManager.LoadScene (SaveLoad.recentState.lastScene);
	}

	private void gameOver() {
		updateBattleLog ("Game Over");
		UnityEngine.UI.Button[] allButtons = (FindObjectsOfType<UnityEngine.UI.Button>() as UnityEngine.UI.Button[]);
		/******************removes any further input by player, will eventually change to restart battle or go back to main menu**********************/
		for (int i = 0; i < allButtons.Length; i++)
		{
			Destroy(allButtons[i]);
		}

	}

	private void distributeEXP() {
		double givenXP = calculateEXP ();
		foreach (Character charac in allTeammateList) {
			charac.addExp(givenXP);
			while (charac.checkLevelUp()) {
				charac.levelUp ();
				updateBattleLog (charac.getName () + " leveled up! He is now level " + charac.getLevel () + "!");
			}
		}
		updateBattleLog ("You Win");
	}

	private double calculateEXP() { //I don't like how I coded this, I will rewrite this later on.
		double currentEXP = 0;
		foreach (Character charac in allEnemyList) {
			currentEXP += (charac.getExp());
		}
		if (currentEXP == 0)
			return -1;
		return currentEXP;
	}

	public string printToBattleLog() {
		return battleLogText;
	}


	public void resetBattleLog() {
		battleLogText = "";
	}

	public void updateBattleLog (string newText) {
		battleLogText += (newText + "\n");
	}

	public int getCurrentChar() {
		return currentChar;
	}

	public int getCurrentTarget() {
		return currentTarget;
	}
}
