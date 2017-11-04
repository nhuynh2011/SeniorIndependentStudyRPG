using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Warps : MonoBehaviour {

	public float x;
	public float y;
	public string targetScene;

	void OnTriggerEnter2D(Collider2D other)
	{
		SaveLoad.recentState.entrenceX = x;
		SaveLoad.recentState.entrenceY = y;
		MainCharacter mc = other.GetComponent<MainCharacter> ();
		SaveLoad.recentState.mainc = new CharacterDataHolder (mc);
		StartCoroutine (Wait(other));
	}

	public IEnumerator Wait(Collider2D other){
		yield return new WaitForSeconds (0.1f);
		SaveLoad.SaveState ();
		Debug.Log ("Saved");
		GameObject go = GameObject.FindGameObjectWithTag ("Fader");
		ScreenFader sf = go.GetComponent<ScreenFader> ();
		sf.FadeToBlack ();
		SceneManager.LoadScene (targetScene);
	}
}
