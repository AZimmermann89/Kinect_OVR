/// <summary>
/// Assembly script.
/// NOTE: Needs a rework.
/// @author: Alexander Zimmermann
/// </summary>
using System;
using System.Collections;
using UnityEngine;

public class AssemblyScript : MonoBehaviour
{

	public GameObject baseAssemblyObject;

    #region private member
    private ArrayList assemblyObjects;
    private GameObject cloneObject;
    private int scoreStore;
    private bool recentlyFrozen;
    private bool recentlyCloned;

    private float colorAlpha;
	private float colorAlphaIteration;
    #endregion private member

    /// <summary>
    /// Initialization
    /// </summary>
    void Start ()
	{
		scoreStore = 0;
		assemblyObjects = new ArrayList ();
		baseAssemblyObject.BroadcastMessage ("registerToAssemblyScript");
		recentlyFrozen = false;
		colorAlpha = 0.0f;
		colorAlphaIteration = 0.02f;
	}
	

	void Update ()
	{
		GameObject go = getActiveObject ();

		if (!recentlyFrozen) {
			refreezeAllObjects ();

			if (go != null) {
				if(!recentlyCloned){
					cloneObject = destroyClone ();
					cloneObject = cloneActiveObject (go);
				}
			}
		}

		if (go != null)
			pulseCloneMaterial ();

		scoreStore = scoreGame ();
	}

	//gets called by the childs to register themselves
	public void registerChild (GameObject go)
	{
		assemblyObjects.Add (go);
	}

	//scores the game
	private int scoreGame ()
	{
		int nScore = 0;

		foreach (GameObject go in assemblyObjects) {
			if (go.GetComponent<AssemblyObject> ().getIsInPosition () == true)
				nScore++;
		}

		return nScore;
	}

	//returns the active Object or null if every object is in position
	private GameObject getActiveObject ()
	{

		for (int i = 0; i <= (assemblyObjects.Count-1); i++) {
			if (!isObjectInPosition (assemblyObjects [i])) {
				return ((GameObject)assemblyObjects [i]);
			}
		}
		
		cloneObject = destroyClone ();
		return null;
	}

	private void refreezeAllObjects ()
	{
		for (int i = 0; i <= (assemblyObjects.Count-1); i++) {
			if (isObjectInPosition (assemblyObjects [i])) {
				freezeObject (assemblyObjects [i]);
			} else {
				unfreezeObject (assemblyObjects [i]);
			}
		}
		recentlyFrozen = true;
	}

	//calls the freezeObject from the component
	private void freezeObject (object o)
	{
		((GameObject)o).GetComponent<AssemblyObject> ().freezeObject ();
	}

	//calls the unfreezeObject from the component
	private void unfreezeObject (object o)
	{
		((GameObject)o).GetComponent<AssemblyObject> ().unfreezeObject ();
	}

	//calls the getIsInPosition from the component
	private bool isObjectInPosition (object o)
	{
		return ((GameObject)o).GetComponent<AssemblyObject> ().getIsInPosition ();
	}

	//creates a clone of the activeObject at the destinated location
	private GameObject cloneActiveObject (object o)
	{
		GameObject clone;
		clone = (GameObject)Instantiate ((GameObject)o, baseAssemblyObject.transform.position, Quaternion.identity);
		clone.GetComponent<MeshRenderer> ().material = Resources.Load ("Materials/GhostMaterial", typeof(Material)) as Material;

		clone.transform.parent = baseAssemblyObject.transform;
		clone.transform.localScale = ((GameObject)o).transform.localScale;
		clone.transform.localRotation = Quaternion.identity;
		clone.transform.localPosition = Vector3.zero;

		clone.name = ("Clone of "+((GameObject)o).name);
		
		clone.GetComponent<AssemblyObject> ().enabled = false;
		clone.GetComponent<GrabbableObject> ().enabled = false;
		clone.GetComponent<Collider> ().isTrigger = true;
		clone.GetComponent<BoxCollider> ().size *= 1.5f;

		recentlyCloned = true;
		return clone;
	}

	//destroy the clonedObject and returns null
	private GameObject destroyClone ()
	{
		Destroy (cloneObject);
		return null;
	}

	//lets the clone Material pulse
	private void pulseCloneMaterial ()
	{
		cloneObject.GetComponent<MeshRenderer> ().material.color = new Vector4 (1, 1, 0, colorAlpha);
		if (colorAlpha <= 0)
			colorAlphaIteration = 0.02f;
		if (colorAlpha >= 1)
			colorAlphaIteration = -0.02f;
		colorAlpha += colorAlphaIteration;
	}

	//gets called from assemblyObject if position has changed
	public void notify(){

		recentlyFrozen = false;
		recentlyCloned = false;
	}

	public String getCloneName(){
		return cloneObject.name;
	}
}


