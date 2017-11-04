using UnityEngine;
using System.Collections;
[System.Serializable]
public class PartyMember : MovingCharacter {

	public override bool Move(Vector3 end){

		lastLocation = transform.position;
		StartCoroutine(SmoothMovement(end));
		return true;
	}

}