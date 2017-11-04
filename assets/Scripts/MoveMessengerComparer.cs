using UnityEngine;
using System.Collections;

public class MoveMessengerComparer : IComparer {

	int IComparer.Compare(object mm1, object mm2){
		int value = ((MoveMessenger) mm2).getSpeed ().CompareTo (((MoveMessenger) mm1).getSpeed ());
		if(value == 0){
			return -1;
		}
		return value;
	}
}