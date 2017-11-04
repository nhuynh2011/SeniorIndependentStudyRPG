using UnityEngine;
using System.Collections;

[System.Serializable]
public class BoardOp : MonoBehaviour {

	public MainCharacter mC;
	public PartyMember[] PMs = new PartyMember[3];
	public Enemy[] Enemies;
	private Vector3 end = Vector3.zero;
	public float stepCount= 0;
	public BattleWarps bw;
	public string currentScene;


	private void Update()
	{
		StartCoroutine (MoveInput());
	}

	private IEnumerator MoveInput(){
		enabled = false;
		//end = mC.transform.position;
		if (Input.GetKey (KeyCode.LeftArrow) && Vector3.zero == end) {
				end = new Vector3(-.32f,0,0);
			}
		if (Input.GetKey (KeyCode.RightArrow) && Vector3.zero == end) {
			end = new Vector3(.32f,0,0);
			}
		if (Input.GetKey (KeyCode.UpArrow) && Vector3.zero == end) {
			end = new Vector3(0,.32f,0);
			}
		if (Input.GetKey (KeyCode.DownArrow) && Vector3.zero == end) {
			end = new Vector3(0,-.32f,0);
			}
		if (end != Vector3.zero) {
			Debug.Log ("Took input");
			if (mC.Move (end)) {
				PartyFollow (mC, 0);
				checkForBattle ();
				yield return new WaitForSeconds (0.10001f);
			}
		} else {
			mC.GetComponent<Animator> ().SetBool ("isWalking", false);
		}
		end = Vector3.zero;
		enabled = true;
	}

	public void setPMsSize(int a) {
		PMs = new PartyMember[a];
	}

	public void checkForBattle(){
		stepCount += .25f ;
		if (Random.value < stepCount / 100) {
			stepCount = 0;
			SaveLoad.recentState.lastScene = currentScene;
			bw.EnterBattle (mC);
			Debug.Log("Would have started a battle");
		}
	}

	public void PartyFollow(MovingCharacter c, int a)
	{
		if(a < PMs.Length && PMs[a] != null)
		{
			PMs[a].Move(c.lastLocation);
			PartyFollow(PMs[a], a + 1);
		}
		return;
	}

	public void MovePMsToCurrentPosition(){
		for (int i = 0; i < PMs.Length; i++) {
			PMs[i].transform.position = mC.transform.position;
		}
	}
		
}