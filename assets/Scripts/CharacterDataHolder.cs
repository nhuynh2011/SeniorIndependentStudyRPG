using UnityEngine;
using UnityEditor;
using System.Collections;

[System.Serializable]
public class CharacterDataHolder {
	private int _health;
	public Move[] _moveList;
	public double _characterSpeedFactor;
	public double _experience;
	public int _level;
	public float _x;
	public float _y;
	public string _name;
	public float _animX;
	public float _animY;
	public string _team;

	public CharacterDataHolder(Character c){
		_health = c.getHealth();
		_moveList = c.moveList;
		_characterSpeedFactor = c.characterSpeedFactor;
		_experience = c.experience;
		_level = c.level;
		_x = c.transform.position.x;
		_y = c.transform.position.y;
		_name = c.name;
		_team = c.getTeam ();
	}

	public CharacterDataHolder(MovingCharacter c){
		_health = c.getHealth();
		_moveList = c.moveList;
		_characterSpeedFactor = c.characterSpeedFactor;
		_experience = c.experience;
		_level = c.level;
		_x = c.transform.position.x;
		_y = c.transform.position.y;
		_name = c.name;
		_team = c.getTeam ();
		_animX = c.animX;
		_animY = c.animY;
	}

	public MainCharacter createMain(string desiredClass){
		GameObject go = GameObject.Instantiate (Resources.Load("MainCharacter")) as GameObject;
		MainCharacter mc = go.GetComponent<MainCharacter> ();
		Animator anim = go.GetComponent<Animator> ();
		anim.SetBool ("isWalking", true);
		anim.SetFloat ("Input_x", _animX);
		anim.SetFloat ("Input_y", _animY);
		anim.SetBool ("isWalking", true);
		mc.setHealth (_health);
		mc.setMoveList(desiredClass);
		mc.characterSpeedFactor = _characterSpeedFactor;
		mc.experience = _experience;
		mc.level = _level;
		mc.transform.position = new Vector3(_x,_y,0);
		mc.name = _name;
		mc.team = _team;
		go.name = mc.name;
		return mc;
	}

	public PartyMember createParty(string desiredClass){
		GameObject gameO = GameObject.Instantiate (Resources.Load("PartyMember")) as GameObject;
		PartyMember mc = gameO.GetComponent<PartyMember> ();
		mc.setHealth (_health);
		mc.setMoveList(desiredClass);
		mc.characterSpeedFactor = _characterSpeedFactor;
		mc.experience = _experience;
		mc.level = _level;
		mc.transform.position = new Vector3(_x,_y,0);
		mc.name = _name;
		mc.team = _team;
		gameO.name = mc.name;
		return mc;
	}

	public Enemy createEnemy(){
		GameObject gameO = GameObject.Instantiate (Resources.Load ("Enemy")) as GameObject;
		Enemy mc = gameO.GetComponent<Enemy> ();
		mc.setHealth (_health);
		mc.moveList = _moveList;
		mc.characterSpeedFactor = _characterSpeedFactor;
		mc.experience = _experience;
		mc.level = _level;
		mc.transform.position = new Vector3(_x,_y,0);
		mc.name = _name;
		mc.team = _team;
		gameO.name = mc.name;
		return mc;
	}
}
