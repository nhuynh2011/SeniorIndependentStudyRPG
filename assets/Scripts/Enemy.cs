using UnityEngine;
using System.Collections;
[System.Serializable]
public class Enemy : Character {


	// Use this for initialization
	void Start () {
		experience = level * 10; //change value later based on gameplay
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
