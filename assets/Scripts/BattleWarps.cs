using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BattleWarps : MonoBehaviour{

	public void EnterBattle(Character other){
		SaveLoad.recentState.entrenceX = other.transform.position.x;
		SaveLoad.recentState.entrenceY = other.transform.position.y;
		StartCoroutine (Fade (other));
	}
		
	public IEnumerator Fade(Character other){
		yield return new WaitForSeconds (.1f);
		SaveLoad.SaveState ();
		Debug.Log ("Saved");
		GameObject go = GameObject.FindGameObjectWithTag ("Fader");
		ScreenFader sf = go.GetComponent<ScreenFader> ();
		sf.FadeToBlack ();
		SceneManager.LoadScene ("Battle test");
	}

}
