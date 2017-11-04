using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameOp {
	public CharacterDataHolder mainc;
	public CharacterDataHolder[] ActivePMs, NonActivePMs, enemiesCDH;
	public float entrenceX, entrenceY, inputX, inputY;
	public bool isBattle;
	public string lastScene, currentScene;
	public static string newMainName = "", newParty1Name = "", newParty2Name = "", newParty3Name = ""; 

	public void Awake(){
		GameObject camera = GameObject.Find ("Main Camera");
		PauseMenu pm = camera.GetComponent<PauseMenu> ();
		pm.go = this;
	}

	public void Create(){
		ActivePMs = new CharacterDataHolder[3];
		// Instantiating our Main Character
		GameObject go = GameObject.Instantiate (Resources.Load("MainCharacter")) as GameObject;
		MainCharacter mainC = go.GetComponent<MainCharacter> ();
		mainc = new CharacterDataHolder (mainC);
		mainc._name = newMainName;
		// Instantiating first teammate
		GameObject gameO = GameObject.Instantiate (Resources.Load("PartyMember")) as GameObject;
		PartyMember party0 = gameO.GetComponent<PartyMember> ();
		party0.name = newParty1Name;
		CharacterDataHolder pmHolder0 = new CharacterDataHolder (party0);
		ActivePMs [0] = pmHolder0;
		// Instantiating second teammate
		GameObject gameO1 = GameObject.Instantiate (Resources.Load("PartyMember")) as GameObject;
		PartyMember party1 = gameO1.GetComponent<PartyMember> ();
		party1.name = newParty2Name;
		CharacterDataHolder pmHolder1 = new CharacterDataHolder (party1);
		ActivePMs [1] = pmHolder1;	
		// Instantiating third teammate
		GameObject gameO2 = GameObject.Instantiate (Resources.Load("PartyMember")) as GameObject;
		PartyMember party2 = gameO2.GetComponent<PartyMember> ();
		party2.name = newParty3Name;
		CharacterDataHolder pmHolder2 = new CharacterDataHolder (party2);
		ActivePMs [2] = pmHolder2;
		// Set initial conditions of map
		entrenceX = 0.47f;
		entrenceY = -1.44f;
		isBattle = false;
		SaveLoad.recentState = this;
		SaveLoad.SaveState ();
	}


	public void Init(){
		SaveLoad.LoadState ();
		if (SaveLoad.recentState == null) {
			if (isBattle) {
				InitBattleScene();
			} else {
				InitMovementScene ();
			}
		}
		mainc = SaveLoad.recentState.mainc;
		ActivePMs = SaveLoad.recentState.ActivePMs;
		NonActivePMs = SaveLoad.recentState.NonActivePMs;
		if (isBattle == true) {
			enemiesCDH = SaveLoad.recentState.enemiesCDH;
			InitBattleScene ();
		} 
		else{
			mainc._x = SaveLoad.recentState.entrenceX;
			mainc._y = SaveLoad.recentState.entrenceY;
			InitMovementScene ();
		}
	}

	public void InitMovementScene()
	{
		GameObject boardOp = GameObject.Find ("BoardOp");
		BoardOp bo = boardOp.GetComponent<BoardOp> ();
		bo.setPMsSize (ActivePMs.Length);
		currentScene = bo.currentScene;
		GameObject camera = GameObject.Find ("Main Camera");
		CameraFollow cf = camera.GetComponent<CameraFollow> ();
		bo.mC = mainc.createMain ("archer");
		cf.target = bo.mC.transform;
		for (int i = 0; i < ActivePMs.Length; i++) {
			string classselect = "";
			if (i % 3 == 0) {
				classselect = "mage";
			} else if (i % 3 == 1) {
				classselect = "warrior";
			} else if (i % 3 == 2) {
				classselect = "monk";
			}
			bo.PMs [i] = ActivePMs [i].createParty (classselect);
		}
		bo.MovePMsToCurrentPosition();
		enemiesCDH = new CharacterDataHolder[bo.Enemies.Length];
		for (int i = 0; i < enemiesCDH.Length; i++) {
			enemiesCDH [i] = new CharacterDataHolder(bo.Enemies[i]);
		}
		GameObject battleW = GameObject.Find ("BattleWarp");
		BattleWarps bwarp = battleW.GetComponent<BattleWarps> ();
		bo.bw = bwarp;
		PauseMenu pm = camera.GetComponent<PauseMenu> ();
		pm.go = this;
		SaveLoad.recentState = this;
	}

	public void InitBattleScene(){
		GameObject battleOp = GameObject.Find ("BattleOp");
		BattleOp bo = battleOp.GetComponent<BattleOp> ();

		bo.allTeammateList = new List<Character>();
		bo.aliveTeammateList = new List<Character> ();
		bo.allEnemyList = new List<Character> ();
		bo.aliveEnemyList = new List<Character> ();

		bo.allEntityList = new Character[enemiesCDH.Length + ActivePMs.Length + 1];
		MainCharacter mc = mainc.createMain ("archer");
		mc.GetComponent<SpriteRenderer> ().enabled = false;
		bo.allTeammateList.Add (mc);
		bo.aliveTeammateList.Add (mc);
		GameObject tmHPmc = GameObject.Instantiate (Resources.Load("Teammate HP 0")) as GameObject;
		tmHPmc.name = (mc.getName () + "'s HP");
		tmHPmc.transform.SetParent (GameObject.Find ("Canvas").transform);
		tmHPmc.transform.localPosition = new Vector3 (-300, 125, 0);
		tmHPmc.transform.localScale = new Vector3(1,1,1);
		for (int i = 1; i < (1 + ActivePMs.Length); i++) {
			string classselect = "";
			if (i-1 % 3 == 0) {
				classselect = "mage";
			} else if (i-1 % 3 == 1) {
				classselect = "warrior";
			} else if (i-1 % 3 == 2) {
				classselect = "monk";
			}
			PartyMember pm = ActivePMs [i-1].createParty (classselect);
			pm.GetComponent<SpriteRenderer> ().enabled = false;
			bo.allTeammateList.Add (pm);
			bo.aliveTeammateList.Add (pm);
			GameObject tmHPpm = GameObject.Instantiate (Resources.Load("Teammate HP 0")) as GameObject;
			tmHPpm.name = (pm.getName () + "'s HP");
			tmHPpm.transform.SetParent (GameObject.Find ("Canvas").transform);
			ChangeText ct = tmHPpm.GetComponent<ChangeText> ();
			ct.textNum = i;
			tmHPpm.transform.localPosition = new Vector3(-300, (125 - (25 * i)), 0);
			tmHPpm.transform.localScale = new Vector3 (1, 1, 1);
		}
		for (int i = 0; i < enemiesCDH.Length ; i++) { 
			Enemy enemyX = enemiesCDH [i].createEnemy ();
			bo.allEnemyList.Add(enemyX);
			bo.aliveEnemyList.Add(enemyX);
			GameObject eHP = GameObject.Instantiate (Resources.Load("Enemy HP 0")) as GameObject;
			eHP.name = (bo.allEnemyList [i].getName () + "'s HP");
			eHP.transform.SetParent (GameObject.Find ("Canvas").transform);
			ChangeText ct = eHP.GetComponent<ChangeText> ();
			ct.textNum = i;
			eHP.transform.localPosition = new Vector3(300,125 - (25 * i),0);
			eHP.transform.localScale = new Vector3 (1, 1,1);
		}
		bo.allEnemyList.TrimExcess();
		bo.allTeammateList.TrimExcess ();
		for (int i = 0; i < bo.allTeammateList.Capacity; i++) {
			bo.allEntityList [i] = bo.allTeammateList [i];
		}
		for (int i = bo.allTeammateList.Capacity; i < bo.allTeammateList.Capacity + bo.allEnemyList.Capacity; i++) {
			bo.allEntityList [i] = bo.allEnemyList [i - bo.allTeammateList.Capacity];
		}

	}

	public void SaveStatePause()
	{
		GameObject boardOp = GameObject.Find ("BoardOp");
		BoardOp bo = boardOp.GetComponent<BoardOp> ();
		mainc = new CharacterDataHolder (bo.mC);
		for (int i = 0; i < ActivePMs.Length; i++) {
			ActivePMs [i] = new CharacterDataHolder (bo.PMs [i]);
		}
		this.entrenceX = mainc._x;
		this.entrenceY = mainc._y;
		SaveLoad.recentState = this;
		SaveLoad.SaveState ();
	}

}