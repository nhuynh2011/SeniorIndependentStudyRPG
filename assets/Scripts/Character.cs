using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour{
	
	public string name; 
	public int health = 30; //Note to self, make private later on
	public int mana = 0;
	public int stamina = 0;
	public Move[] moveList;
	public double characterSpeedFactor = 1.5;
	public double experience;
	public int level;
	public string type;
	public string team;

	public bool checkMove(int a)
	{
		return true;
	}

	public string getTeam() {
		return team;
	}

	public void setType(string _type) {
		type = _type;
	}

	public string getType() {
		return type;
	}

	public Move[] getMoveList() {
		return moveList;
	}

	public void setMoveList(string preset) {
		if (preset == "archer") {
			moveList = new Move[5];
			moveList [0] = new Move ("Shoot", 10, 40);
			moveList [1] = new Move ("Quick Shot", 10, 1000);
			moveList [2] = new Move ("Flame Shot", 20, 30, "fire");
			moveList [3] = new Move ("Charged Shot", 50, 5);
			moveList [4] = new Move ("Arrow Barrage", 20, 40); //Eventually add a multi target attack
		} else if (preset == "mage") {
			moveList = new Move[6];
			moveList [0] = new Move ("Blast", 10, 40);
			moveList [1] = new Move ("Ice Beam", 40, 20, "ice");
			moveList [2] = new Move ("Fireball", 40, 20, "fire");
			moveList [3] = new Move ("Lightning Bolt", 50, 50, "lightning");
			moveList [4] = new Move ("Wind Cutter", 30, 20, "wind");
			moveList [5] = new Move ("Tsunami", 50, 5, "water");
		} else if (preset == "warrior") {
			moveList = new Move[4];
			moveList [0] = new Move ("Attack", 10, 40);
			moveList [1] = new Move ("Combo Slasher", 40, 20);
			moveList [2] = new Move ("Flame Slash", 20, 20, "fire");
			moveList [3] = new Move ("Lightning Blade", 25, 40, "lightning");
		} else if (preset == "monk") {
			moveList = new Move[3];
			moveList [0] = new Move ("Punch", 10, 40);
			moveList [1] = new Move ("Smashing Knuckle of the Monk", 50, 25);
			moveList [2] = new Move ("Winding Tornado Kick", 60, 5, "wind");
		}
	}

	public Move getMove(int a) {
		if (a < moveList.Length) {
			return moveList [a];
		} else
			return null;
	}

	public int moveListLength() {
		return moveList.Length;
	}

	public MoveMessenger makeMove(int a)
	{
		Debug.Log ("Move: " + a);
		MoveMessenger mm = new MoveMessenger();
		Debug.Log (mm);
		Debug.Log (moveList [a]);
		mm.recalculateDamage (moveList [a].getDamage ());
		mm.setSpeed (characterSpeedFactor * moveList [a].speed);
		mm.setAttackingPlayer (this);
		return mm;
	}

	public int receiveDamage(MoveMessenger m)
	{
		int damage = m.getRecalculatedDamage ();
		return health - damage;
	}

	public void setHealth(int h)
	{
		health = h;
	}

	public string getName() {
		return name;
	}

	public bool isAlive() {
		return (health > 0);
	}

	public int getHealth(){
		return health;
	}

	public double getExp() {
		return experience;
	}

	public void addExp (double exp) {
		print ("Adding " + exp + " to " + getName() + "'s current EXP which is " + experience);
		experience += exp;
	}

	public int getLevel() {
		print (getName() + "'s current level is " + level);
		return level;
	}

	public bool checkLevelUp() {
		if (experience > level * 100) {
			print (getName() + " leveled up.");
			return true;
		}
		return false;
	}

	public void levelUp() {
		level++;
		print (getName() + "'s new level is " + level);
	}
}
