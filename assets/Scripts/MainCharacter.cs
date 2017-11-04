using UnityEngine;
using System.Collections;
[System.Serializable]
public class MainCharacter : MovingCharacter {

	public Animator anim;


	public override bool Move(Vector3 end){
		anim = GetComponent<Animator> ();
		Vector3 target = this.transform.position + end;
		boxCollider.enabled = false;
		RaycastHit2D hit = Physics2D.Linecast(transform.position, target, blockingLayer);
		boxCollider.enabled = true;
		if (hit.transform == null) {
			anim.SetBool ("isWalking", true);
			anim.SetFloat ("Input_x", end.x);
			anim.SetFloat ("Input_y", end.y);
			animX = end.x;
			animY = end.y;
			lastLocation = transform.position;
			StartCoroutine (SmoothMovement (target));
			return true;
		}
		anim.SetBool ("isWalking", false);
		return false;
	}
}