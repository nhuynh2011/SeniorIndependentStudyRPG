using UnityEngine;
using System.Collections;

[System.Serializable]
public class Move {

	public string name;
	public int speed = 10; //Eventually alter speed based on name
	public int damage;
	public string type;

	public Move(string n, int d, int s){
		name = n;
		damage = d;
		speed = s;
		type = "normal";
	}

	public Move (string n, int d, int s, string moveType) {
		name = n;
		damage = d;
		speed = s;
		type = moveType;
	}

	public int getDamage()
	{
		return damage;
	}

	public int getSpeed() {
		return speed;
	}

	public string toString(){
		return name;
	}
}
