using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveLoad {

	public static GameOp recentState;

	public static void SaveState(){
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/recentState.gd");
		bf.Serialize(file, recentState);
		file.Close ();
	}

	public static void LoadState(){
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Open (Application.persistentDataPath + "/recentState.gd", FileMode.Open);
		SaveLoad.recentState = (GameOp)bf.Deserialize (file);
		file.Close ();
	}

}
