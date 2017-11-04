using UnityEngine;
using System.Collections;

public class MoveMessenger {

	private Move damageType;
	private int recalculatedDamage;
	private double speed;
	private Character attackedPlayer;
	private Character attackingPlayer;
	private int indexOfAttackedPlayer;
	private int indexOfMove;

	public void setAttackedPlayer(Character c){
		attackedPlayer = c;
	}

	public Character getAttackedPlayer(){
		return attackedPlayer;
	}

	public void setIndexOfAttackedPlayer(int a) {
		indexOfAttackedPlayer = a;
	}

	public int getIndexOfAttackedPlayer() {
		return indexOfAttackedPlayer;
	}

	public void recalculateDamage(int damage){ //Change this later on
		recalculatedDamage = damage;
	}

	public void setIndexOfMove(int a) {
		indexOfMove = a;
	}

	public int getIndexOfMove() {
		return indexOfMove;
	}

	public int getRecalculatedDamage(){
		return recalculatedDamage;
	}

	public void setSpeed(double s){
		speed = s;
	}

	public double getSpeed(){
		return speed;
	}

	public string toString(){
		return("Recalculated Damage: " + getRecalculatedDamage () + "Speed: " + getSpeed ());
	}

	public Character getAttackingPlayer(){
		return attackingPlayer;
	}

	public void setAttackingPlayer(Character a){
		attackingPlayer = a;
	}
}
