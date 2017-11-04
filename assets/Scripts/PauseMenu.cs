using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

	public GameObject PauseUI;
	public GameOp go;

	private bool paused = false; 

	void Start()
	{
		PauseUI.SetActive (false);
	}

	void Update(){
		if (Input.GetButtonUp ("Pause")) {
			paused = !paused; 
		}

		if (paused) {
			PauseUI.SetActive (true);
			go.SaveStatePause ();
			Time.timeScale = 0;
		}

		if (!paused) {
			PauseUI.SetActive (false);
			Time.timeScale = 1;
		}
	}

	public void ResumeClick(){
		paused = false;
	}

	public void MainMenuClick(){
		go.SaveStatePause ();
		print (go.currentScene);
		print ("MainC current x: " + go.mainc._x);
		print ("MainC current y: " + go.mainc._y);
		SceneManager.LoadScene ("MainMenu");
	}
}
