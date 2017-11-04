using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChangeText : MonoBehaviour {

	public BattleOp changeTextBattleOp;
	public string textType;
	public int textNum;
	Text yourButtonText;

	// Use this for initialization
	void Start () {
		yourButtonText = transform.FindChild ("Text").GetComponent<Text> ();
		GameObject battleOp = GameObject.Find ("BattleOp");
		BattleOp bo = battleOp.GetComponent<BattleOp> ();
		changeTextBattleOp = bo;
	}

	// Update is called once per frame
	void Update () {
		if (textType == "move")
			updateTextForMoves ();
		if (textType == "updatebox")
			updateTextForUpdateBox ();
		if (textType == "battleLog")
			updateTextForBattleLog ();
		else if (textType == "teamHP" || textType == "enemyHP")
			updateTextForHP ();
	}

	void updateTextForMoves() {
		if (changeTextBattleOp.currentPageOfMovesOrTargets > 0) {
			int newIndexOfMove = textNum + (changeTextBattleOp.currentPageOfMovesOrTargets * 4);
			if (newIndexOfMove < changeTextBattleOp.aliveTeammateList[changeTextBattleOp.getCurrentChar()].getMoveList().Length) {
				changeTextBattleOp.meButtons [textNum].SetActive (true);
				yourButtonText.text = (changeTextBattleOp.aliveTeammateList[changeTextBattleOp.getCurrentChar()].getMoveList()[newIndexOfMove].name);
			}
			else {
				changeTextBattleOp.meButtons [textNum].SetActive (false);
			}
		}
		if (changeTextBattleOp.hasSelectedMove == false)
			yourButtonText.text = (changeTextBattleOp.aliveTeammateList[changeTextBattleOp.getCurrentChar()].getMove(textNum+4*changeTextBattleOp.currentPageOfMovesOrTargets).name);
		else
			yourButtonText.text = (changeTextBattleOp.aliveEnemyList[textNum+(4*changeTextBattleOp.currentPageOfMovesOrTargets)].getName());
	}

	void updateTextForUpdateBox() {
		if (changeTextBattleOp.needToPrint && changeTextBattleOp.printUpdates () != null) { 
			yourButtonText.text = changeTextBattleOp.printUpdates ();
		}
	}

	void updateTextForHP() {
		if (textType == "teamHP") {
			yourButtonText.text = (changeTextBattleOp.allTeammateList[textNum].getName() + "'s HP : " + changeTextBattleOp.allTeammateList [textNum].getHealth ());
		} else {
			yourButtonText.text = (changeTextBattleOp.allEnemyList[textNum].getName() + "'s HP : " + changeTextBattleOp.allEnemyList [textNum].getHealth ());
		}
	}

	void updateTextForBattleLog() {
		yourButtonText.text = changeTextBattleOp.printToBattleLog ();
	}
}
