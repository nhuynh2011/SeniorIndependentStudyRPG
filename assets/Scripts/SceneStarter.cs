using UnityEngine;
using System.Collections;

public class SceneStarter : MonoBehaviour {

	public GameOp go;

	void Awake(){
		go.Init ();
	}
}
