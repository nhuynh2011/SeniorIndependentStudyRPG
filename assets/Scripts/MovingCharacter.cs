using UnityEngine;
using System.Collections;

public abstract class MovingCharacter : Character {

	public float moveTime = 0.1f;
	public LayerMask blockingLayer;
	public BoxCollider2D boxCollider;
	public Rigidbody2D rb2D;
	public float inverseMoveTime;
	public Vector2 lastLocation;

	public float animX;
	public float animY;

	public virtual void Start()
	{
		boxCollider = GetComponent<BoxCollider2D>();
		rb2D = GetComponent<Rigidbody2D>();
		inverseMoveTime = 1f/moveTime;
		lastLocation = transform.position;
	}
	public virtual bool Move(Vector3 end)
	{
		return false;
	}


	public IEnumerator SmoothMovement(Vector3 end)
	{
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
		while(sqrRemainingDistance > float.Epsilon)
		{
			Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
			rb2D.MovePosition(newPosition);
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			yield return null;
		}
	}
}