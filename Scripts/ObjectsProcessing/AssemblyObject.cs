/// <summary>
/// Inherits GrabbableObject, adds assembly-functions to the object
/// NOTE: Needs a rework.
/// @author: Alexander Zimmermann
/// </summary> 
using System;
using UnityEngine;

public class AssemblyObject : GrabbableObject
{
	private bool isInPosition;
	private bool oldIsInPosition;
	private bool isFrozen;

	private GameObject assemblyScriptObject;

	public void Start ()
	{
		base.Start ();

		if (gameObject.transform.parent != null) {
			baseObject = gameObject.transform.parent.gameObject;
		}

		oldIsInPosition = false;

		if (transform.localPosition == Vector3.zero){
			isInPosition = true;
		} else {
			isInPosition = false;
		}

	}

    public void Update () 
	{
		base.Update ();
		
		if (transform.localPosition == Vector3.zero){
			isInPosition = true;

			if(isInPosition != oldIsInPosition){

				GameObject.Find ("AssemblyScript").GetComponent<AssemblyScript> ().notify ();
				oldIsInPosition = isInPosition;
			}
		} else {
			isInPosition = false;

			if(isInPosition != oldIsInPosition){
				GameObject.Find ("AssemblyScript").GetComponent<AssemblyScript> ().notify ();
				oldIsInPosition = isInPosition;
			}
		}

	}

    public void OnTriggerStay	(Collider collisionObject) {
		
		if (collisionObject.gameObject.name == ("Clone of " + gameObject.name) 
		    && !gameObject.GetComponent<AssemblyObject>().IsGrabbed()) {
			gameObject.GetComponent<AssemblyObject>().ResetPosition();
		}
	}

	public void freezeObject () 
	{
		Collider goCollider = gameObject.GetComponent<Collider> ();
		goCollider.enabled = false;

		isFrozen = true;
		
		gameObject.GetComponent<Renderer> ().material.color = Color.gray;
		
	}
	
	public void unfreezeObject ()
	{
		Collider goCollider = gameObject.GetComponent<Collider> ();
		goCollider.enabled = true;

		isFrozen = false;

		gameObject.GetComponent<Renderer> ().material.color = Color.green;

	}
	

	//returns the isInPosition bool
	public bool getIsInPosition ()
	{
		return isInPosition;
	}
	
	//registers this object to the assemblyScript
	public void registerToAssemblyScript ()
	{
			assemblyScriptObject = GameObject.Find ("AssemblyScript");
			assemblyScriptObject.GetComponent<AssemblyScript> ().registerChild (gameObject);
	}
}
