using UnityEngine;
using System.Collections;

public class SceneController : MonoBehaviour {

	public GameOp go;

	void Awake(){
		go.Create ();
	}

	public void Update(){
		UnityEngine.SceneManagement.SceneManager.LoadScene ("movement test 1");
	}
}
