using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public enum Menu {
		MainMenu,
		NewGame,
		Continue
	}

	public Menu currentMenu;


	//change it so they need to put in names and none of them can be identical or blank
	void OnGUI () {

		GUILayout.BeginArea(new Rect(0,0,Screen.width, Screen.height));
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.BeginVertical();
		GUILayout.FlexibleSpace();

		if(currentMenu == Menu.MainMenu) {
			
			SaveLoad.LoadState ();
			GUILayout.Box("Role-Playing Game");
			GUILayout.Space(10);

			if(GUILayout.Button("New Game")){
				currentMenu = Menu.NewGame;
			}

			if(GUILayout.Button("Continue")) {
				if (GameOp.newMainName == "") {
				} else {
					currentMenu = Menu.Continue;
				}
			}

			if(GUILayout.Button("Quit")) {
				Application.Quit();
			}
		}

		else if (currentMenu == Menu.NewGame) {

			GUILayout.Box("Name Your Characters");
			GUILayout.Space(10);

			GUILayout.Label("Your Name");
			GameOp.newMainName = GUILayout.TextField(GameOp.newMainName, 20);
			GUILayout.Label("1st Party Member Name");
			GameOp.newParty1Name = GUILayout.TextField(GameOp.newParty1Name, 20);
			GUILayout.Label("2nd Party Member Name");
			GameOp.newParty2Name = GUILayout.TextField(GameOp.newParty2Name, 20);
			GUILayout.Label("3rd Party Member Name");
			GameOp.newParty3Name = GUILayout.TextField(GameOp.newParty3Name, 20);

			if(GUILayout.Button("Start")) {
				if (GameOp.newMainName.Equals("") || GameOp.newParty1Name.Equals("") || GameOp.newParty2Name.Equals("") || GameOp.newParty3Name.Equals("")) {
					Debug.LogWarning("Give your party names!");
				} else if (GameOp.newParty1Name.Equals (GameOp.newParty2Name) || GameOp.newParty2Name.Equals (GameOp.newParty3Name) || GameOp.newParty1Name.Equals (GameOp.newParty3Name)) {
					Debug.LogWarning("Teammate Names Cannot Match!");
				} else {
					SaveLoad.SaveState ();
					//Move on to game...
					SceneManager.LoadScene ("Start");
				}
			}

			GUILayout.Space(10);
			if(GUILayout.Button("Cancel")) {
				currentMenu = Menu.MainMenu;
			}

		}

		else if (currentMenu == Menu.Continue) {
			SaveLoad.LoadState();
			print (SaveLoad.recentState.currentScene);
			SceneManager.LoadScene (SaveLoad.recentState.currentScene);
			
		}

		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();

	}
}
