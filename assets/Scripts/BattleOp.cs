using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class BattleOp : MonoBehaviour {

	public int numChar, currentPageOfMovesOrTargets;
	public List<Character> aliveTeammateList, deadTeammateList, allTeammateList, aliveEnemyList, deadEnemyList, allEnemyList;
	public Character[] allEntityList;	
	public SortedList moveList = new SortedList(new MoveMessengerComparer());
	public bool hasSelectedMove = false, hasSelectedTarget = false, needToPrint = true, calledBattleCoroutine = false;
	public GameObject[] meButtons = new GameObject[4];
	GameObject lastKnownInput, battleLog;

	private bool playerTurn = false, pressedEnter = false, needToChange = false;
	private int currentChar, currentTarget, currentMove, selectedMove = -1, enemyMove, currentHighlight;
	private Character attackedPlayer = null;
	private string battleLogText = "";

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

		aliveTeammateList.TrimExcess ();
		//print (aliveTeammateList.Capacity);
		//print (aliveEnemyList.Capacity); weird? alive teammate list has one extra slot when created, and if i dont trim excess, then the battling wont work? enemy list is fine though?
		aliveEnemyList.TrimExcess();
		//print (aliveEnemyList.Capacity); check game op notes in initbattle
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
		}


		if (currentHighlight % 4 == 0 && Input.GetKeyDown (KeyCode.UpArrow)) {
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
			} 
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
				updateMovesForChar ();
				print ("Reselecting Move...");
			} else {
				if (currentChar == 0) {
					print ("This is the first character, can't go back to last character");
				} else {
					currentChar--;
					updateMovesForChar ();
					print ("Reselecting moves and target for " + aliveTeammateList [currentChar].getName ());
					moveList.RemoveAt (currentChar);
				}
			}
		}
	}

	private bool OnSelectMove(){
		/*************THIS IS WHERE I  WILL TOGGLE BUTTON TO SCROLL CODE, DISABLE INPUT BESIDES ENTER,
		disable me1,2,3,4, enable scroll text button ***********/
		if(aliveTeammateList[currentChar].checkMove(selectedMove)) { //Always return true, but in what case wouldn't they be able to perform the move?  not enough mana or something
			MoveMessenger messenger = aliveTeammateList[currentChar].makeMove(selectedMove);
			print(messenger.toString());
			messenger.setAttackedPlayer(attackedPlayer);  // the messenger stores the attacked player
			messenger.setIndexOfAttackedPlayer(currentTarget);
			messenger.setIndexOfMove (selectedMove);
			print("Attacked Player: " + messenger.getAttackedPlayer());
			FillMoveList(messenger);
			if(currentChar == (aliveTeammateList.Capacity - 1)){
				needToPrint = false;
				StartCoroutine (StartBattling());
			}
			else {
				currentChar++;
				print("CurrentChar after incrementation: " + currentChar);
				updateMovesForChar ();
				EventSystem.current.SetSelectedGameObject(meButtons[0]);
			}
			return true;
		}
		return false;
	}

	IEnumerator StartBattling() {
		aliveTeammateList.TrimExcess ();
		aliveEnemyList.TrimExcess ();
		for (int i = 0; i < 4; i++) {
			meButtons [i].SetActive (false);
		}
		battleLog.SetActive (true);
		playerTurn = false;
		yield return SelectAIMove ();
		//MakeMoves (); method is under this
		print (pressedEnter); //says its true because we hit enter for the final selection of target 
		pressedEnter = false;
		for (int i = 0; i < moveList.Count; i++) {
			print ("i is " + i);
			MoveMessenger m = (MoveMessenger)(moveList.GetKey (i));
			if (m.getAttackingPlayer ().getHealth () > 0) {
				if (m.getAttackedPlayer ().isAlive () != true) {
					updateBattleLog (m.getAttackingPlayer ().getName () + "'s target was dead before move.  Attacking next person in the enemy line");
					yield return StartCoroutine (chatEnter ());
					int newTarget = m.getIndexOfAttackedPlayer ();
					if (m.getAttackingPlayer ().getTeam () == "team") {
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
				yield return StartCoroutine (chatEnter ());
				m.getAttackedPlayer ().setHealth (health);
				if (health <= 0) {
					updateBattleLog (m.getAttackedPlayer ().getName () + " has died");
					if (m.getAttackedPlayer ().team == "team") {
						relocateToDeadTeammateList (m);
					} else { //no need to check if he's enemy, because he will be
						relocateToDeadEnemyList (m);
					}
					yield return StartCoroutine (chatEnter ());
			//moveList.RemoveAt (moveList.IndexOfValue (m.getAttackedPlayer ().getName ())); //the line that causes skipped turns
					if (isBattleOver ()) {
						yield return StartCoroutine (chatEnter ());
						updateBattleLog ("Switching back to movement scene...");
						yield return (new WaitForSeconds (1f));
						BattleOver ();
					}
				}
			} else {
				updateBattleLog (m.getAttackingPlayer ().getName () + "'s hp was " + m.getAttackingPlayer ().getHealth () + " so his turn is skipped.");
				yield return StartCoroutine (chatEnter ());
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

	private object SelectAIMove (){
		System.Random rnd = new System.Random();
		for (int i = 0; i < aliveEnemyList.Capacity; i++) {
			print("Called SelectAIMove()");
			print("Current Enemy: " + aliveEnemyList[i].getName());
			enemyMove = (rnd.Next(0, aliveEnemyList [i].getMoveList ().Length));
			MoveMessenger messenger = aliveEnemyList[i].makeMove (enemyMove);
			print(messenger.toString());
			int target = rnd.Next(0, aliveTeammateList.Capacity);
			print("Target is Teammate #: " + target);
			messenger.setAttackedPlayer(aliveTeammateList[target]);
			FillMoveList(messenger);
		}
		return null;
	}

	private void FillMoveList(MoveMessenger m){
		moveList.Add (m, aliveEnemyList [currentTarget].name);
		for(int i = 0; i < moveList.Count; i++) {
			MoveMessenger mm = (MoveMessenger) (moveList.GetKey (i));
			print((i+1) + " move(s) in list:" + " " + "Attacking Player: " + mm.getAttackingPlayer () + " " + "AttackedPlayer: " + mm.getAttackedPlayer () + " " + "Move Speed: " + mm.getSpeed () + " " + "Move Damage: " + mm.getRecalculatedDamage ());
		}
	}

	IEnumerator chatEnter () {
		while (!pressedEnter)
			yield return null;
		resetBattleLog ();
		print (pressedEnter);
		pressedEnter = false;
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

	private void relocateToDeadTeammateList (MoveMessenger m) {
		deadTeammateList.Add (m.getAttackedPlayer ());
		aliveTeammateList.Remove (m.getAttackedPlayer ());
		aliveTeammateList.TrimExcess ();
	}

	private void relocateToDeadEnemyList (MoveMessenger m) {
		deadEnemyList.Add (m.getAttackedPlayer ());
		aliveEnemyList.Remove (m.getAttackedPlayer ()); 
		aliveEnemyList.TrimExcess ();
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
			selectedMove = currentMove;
			currentTarget = 0;
			currentHighlight = 0;
			currentPageOfMovesOrTargets = 0;
			updateTargetsForChar();
			EventSystem.current.SetSelectedGameObject(meButtons[0]);
		}
		else if (hasSelectedTarget == false) {
			hasSelectedTarget = true;
			attackedPlayer = aliveEnemyList [currentTarget];
		}
		if (hasSelectedMove == true && hasSelectedTarget == true) {
			OnSelectMove();
			currentTarget = 0;
			currentMove = 0;
			currentHighlight = 0;
			currentPageOfMovesOrTargets = 0;
			selectedMove = -1;
			hasSelectedTarget = false;
			hasSelectedMove = false;
			attackedPlayer = null;
		}
	}

	public string printUpdates() {
		if (hasSelectedMove == false) {
			return ("Selecting moves for " + aliveTeammateList [currentChar].getName () + ".  \nCurrent move is " + aliveTeammateList [currentChar].moveList [currentMove].name + ". \nDamage = " + 
				aliveTeammateList [currentChar].moveList [currentMove].getDamage() + "\nSpeed = " + aliveTeammateList [currentChar].moveList [currentMove].getSpeed() + "\nType = " + 
				aliveTeammateList [currentChar].moveList [currentMove].GetType());
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
			updateBattleLog ("You lose, game over.");
			return true;
		}
		if (didTeamWin ()) {
			updateBattleLog ("You win the battle!");
			return true;
		}
		return false;
	}
		
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
		}
		if (didEnemyWin()) {
			gameOver();
		}
		Fade ();
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
		double givenXP = (calculateTotalEXP ()) / allTeammateList.Capacity;
		foreach (Character charac in allTeammateList) {
			charac.addExp(givenXP);
			while (charac.checkLevelUp()) {
				charac.levelUp ();
				updateBattleLog (charac.getName () + " leveled up! He is now level " + charac.getLevel () + "!");
			}
		}
		updateBattleLog ("You Win");
	}

	private double calculateTotalEXP() { //I don't like how I coded this, I will rewrite this later on.
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
