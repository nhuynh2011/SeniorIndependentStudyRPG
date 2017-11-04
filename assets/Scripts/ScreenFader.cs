using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour {

	Animator anim;
	bool isFading;

	void Start(){
		anim = GetComponent<Animator> ();
	}

	void AnimationComplete(){
		isFading = false;
	}

	public IEnumerator FadeToBlack(){
		isFading = true;
		anim.SetBool ("FadeOut", true);
		while (isFading)
			yield return null;
	}


}
