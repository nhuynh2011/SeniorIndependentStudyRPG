using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public Transform target;
	Camera mycam;

	void Start () {
		mycam = GetComponent<Camera> ();
	}
	
	// Update is called once per frame
	void Update () {
		mycam.orthographicSize = (Screen.height / 100f) / 3f;
		if(target){
			//transform.position = Vector3.Lerp(transform.position, target.position, 0.1f) + new Vector3(0, 0, -10);
			transform.position = target.position + new Vector3(0, 0, -10);
		}
	}
}
