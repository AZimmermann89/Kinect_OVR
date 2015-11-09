/// <summary>
/// Quits the application on 'esc'-press.
/// @author: Alexander Zimmermann
/// </summary>
using UnityEngine;
using System.Collections;

public class QuitOnEsc : MonoBehaviour {

	void Start () {
	
	}
	
	void Update () {
		if (Input.GetKey("escape"))
			Application.Quit();
	
	}
}
