/// <summary>
/// Script that makes an object grabbable with the avatar.
/// Code partially from Nathan Beattie
///  http://blog.leapmotion.com/cad-experiment-disassemble-spherical-robot-vr/
/// https://github.com/Zaeran/CAD-Demo
/// @author: Alexander Zimmermann, Nathan Beattie
/// </summary>
using UnityEngine;
using System.Collections;

public class GrabbableObject : MonoBehaviour
{
    #region private member
    protected GameObject baseObject;
    protected bool grabbed_ = false;
    protected bool hovered_ = false;

    private bool resetting_ = false;
    #endregion private member

    /// <summary>
    /// Returns whether the object is hovered by a hand.
    /// </summary>
    /// <returns></returns>
    public bool IsHovered()
    {
        return hovered_;
    }

    /// <summary>
    /// Returns whether the object is grabbed by a hand.
    /// </summary>
    /// <returns></returns>
    public bool IsGrabbed()
    {
        return grabbed_;
    }

    /// <summary>
    /// Returns whether the object is resetting.
    /// </summary>
    /// <returns></returns>
    public bool IsResetting()
    {
        return resetting_;
    }

    /// <summary>
    /// Sets the object state to "hovered" and changes the material.
    /// </summary>
    public virtual void OnStartHover()
    {
		gameObject.GetComponent<MeshRenderer>().material.shader = Shader.Find("Legacy Shaders/Self-Illumin/Specular");
        hovered_ = true;
    }

    /// <summary>
    /// Sets the object state to "not-hovered" and changes the material.
    /// </summary>
    public virtual void OnStopHover()
    {
		gameObject.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard");
        hovered_ = false;
    }

    /// <summary>
    /// Sets the object state to "grabbed".
    /// </summary>
    public virtual void OnGrab()
    {
        grabbed_ = true;
        hovered_ = false;

        
    }

    /// <summary>
    /// Sets the object state to "hovered".
    /// </summary>
    public virtual void OnRelease()
    {
		grabbed_ = false;
		hovered_ = false;
        
		OnStopHover ();
    }

    /// <summary>
    /// Increases the box-collider of the object and sets the parent object to the base object, if it has one.
    /// (If not, its the root object of the object-tree.)
    /// </summary>
	public void Start(){
		if (gameObject.transform.parent != null) {
			baseObject = gameObject.transform.parent.gameObject;
		}
		gameObject.GetComponent<BoxCollider> ().size *= 1.3f;
	}

    /// <summary>
    /// Resets the position of the object, if the state of the object is "resetting'.
    /// </summary>
	public void Update()
    {
		
        if (resetting_)
        {
            //move to reset position
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0,0,0), 0.1f);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, 0.1f);
            if (Vector3.Distance(transform.localPosition, new Vector3(0, 0, 0)) < 0.001f)
            {
                resetting_ = false;
            }
        }


    }

    /// <summary>
    /// Sets the object state to "resetting".
    /// </summary>
    public void ResetPosition()
    {
        resetting_ = true;
    }
}
