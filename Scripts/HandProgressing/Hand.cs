/// <summary>
/// This class uses the HandData to grab objects.
/// @author: Alexander Zimmermann
/// </summary>
using UnityEngine;
using System.Collections;
using KinectHandData;

public class Hand : MonoBehaviour {

    #region public member
    public enum HandDataType : int { SmoothedStateData, UnalteredData  }
	public HandDataType 		handDataType = HandDataType.UnalteredData;
	
	// Layers that we can grab.
	public LayerMask 			grabbableLayers = ~0;
    public bool isLeftHand = false;
    #endregion public member

    #region private member
    private HandDataInterface _hdi;
    protected Collider 			active_object_;
	protected Vector3 			hand_position_;
	protected Quaternion 		hand_rotation_;
	protected HandEventType 	current_event;
	protected Transform			oldActiveTransform;
    #endregion private member

    /// <summary>
    /// Initialization. Sets up the interface and the other variables
    /// </summary>
    void Start ()
	{
		//To add additional Interfaces, extend the cases and extend the HandDataType Enumeration (in this class)
		switch (handDataType)
		{
		case HandDataType.SmoothedStateData:
			_hdi = new SmoothedHandDataInterface ();
			break;
		case HandDataType.UnalteredData:
			_hdi = new UnalteredHandDataInterface ();
			break;
		default:
			_hdi = new UnalteredHandDataInterface ();
			break;
		}
		
		hand_position_ = Vector3.zero;
		current_event = HandEventType.Release;
		hand_rotation_ = Quaternion.identity;
	}

    /// <summary>
    /// Notify grabbable objects when they are ready to grab and sets the active_object_ to the grabbable object.
    /// </summary>
    protected void Hover ()
	{
		
		Collider hover = FindClosestGrabbableObject (hand_position_);
		
		if (hover != active_object_ && active_object_ != null) {
			GrabbableObject old_grabbable = active_object_.GetComponent<GrabbableObject> ();
			
			if (old_grabbable != null)
				old_grabbable.OnStopHover ();
		}
		
		if (hover != null) {
			GrabbableObject new_grabbable = hover.GetComponent<GrabbableObject> ();
			
			if (new_grabbable != null) {
				new_grabbable.OnStartHover ();
				if (new_grabbable.IsResetting ()) {
					
					return;
				}
			}
		}
		
		active_object_ = hover;
		
	}
	
    /// <summary>
    /// Called by Hover(). Returns the object closest to grab_position.
    /// </summary>
    /// <param name="grab_position"></param>
    /// <returns></returns>
	protected Collider FindClosestGrabbableObject (Vector3 grab_position)
	{
		Collider closest = null;
		Collider[] close_things =
			Physics.OverlapSphere (grab_position, 0.1f, grabbableLayers);
		
		for (int j = 0; j < close_things.Length; ++j) {
			
			//pinch position must be inside bounds of object's collider
			if (close_things [j].GetComponent<Rigidbody>() != null && close_things [j].GetComponent<Collider>().bounds.Contains (grab_position) &&
			    !close_things [j].transform.IsChildOf (transform) &&
			    close_things [j].tag != "NotGrabbable") {
				
				GrabbableObject grabbable = close_things [j].GetComponent<GrabbableObject> ();
				if (grabbable == null || !grabbable.IsGrabbed ()) {
					closest = close_things [j];
					
				}
			}
		}
		return closest;
	}
	
	/// <summary>
    /// Handles the release of the object. Called when hand event is set from grip to release.
    /// </summary>
	protected void OnRelease ()
	{
		if (active_object_ != null) {

			// Notify the grabbable object that is was released.
			GrabbableObject grabbable = active_object_.GetComponent<GrabbableObject> ();
			if (grabbable != null)
				grabbable.OnRelease ();

			if(oldActiveTransform != null){

				active_object_.gameObject.transform.parent = oldActiveTransform;
				oldActiveTransform = null;
			}
		}

		active_object_ = null;
		
		Hover ();
	}

    /// <summary>
    /// Handles the grabbing. Called when hand event is set from release to grip.
    /// </summary>
    protected void OnGrab ()
	{
		// Only grab something if there is something
		if (active_object_ == null)
			return;

		oldActiveTransform = active_object_.gameObject.transform.parent;

		active_object_.gameObject.transform.SetParent (transform, true);
		
		if (active_object_ != null) {
			// Notify grabbable object that it was grabbed.
			active_object_.GetComponent<GrabbableObject>().OnGrab();
			
		}
	}
	
    /// <summary>
    /// Handles the ongoing grabbing. Called when hand event stays on grip.
    /// </summary>
	protected void ContinueGrab ()
	{
		if (!active_object_.GetComponent<GrabbableObject> ().IsGrabbed ()) {
			OnGrab();
		}

		//ROTATE active_object_ BY hand_rotation_
		
	}
	
    /// <summary>
    /// Gets the position, orientation and event data and handles which method is called.
    /// </summary>
	void FixedUpdate ()
	{

		HandEventType new_event = _hdi.getHandEvent (isLeftHand);
		
		//hand_position_ is set to the position of the avatar-hands rather than the raw _hdi.getHandPosition return value.
		hand_position_ = gameObject.transform.position;
		
		hand_rotation_ = _hdi.getHandOrientation(isLeftHand);
		

		// State transitions from the last frame (current_event) to the next frame (new_event).
		if (current_event == HandEventType.Release) {
			if (new_event == HandEventType.Grip) {
                //From release to grip.
				OnGrab ();
				
			} else {
                //Hand remains open.
				Hover ();
			}
		} else {
			if (current_event == HandEventType.Grip){
				if(new_event == HandEventType.Release){
                    //From grip to release.
					OnRelease();
				}else {
					if(active_object_ != null){
                        //Hand remains closed if there is still an object.
						ContinueGrab();
					} else {
                        //Lost object in the hand.
						OnRelease();
					}
				}
			} else {
                //If the current event is neither Grip nor Release
				OnRelease();
			}
			
		}
		
		current_event = new_event;
		
	}
	
    /// <summary>
    /// Obligatory method.
    /// Once this whole project is a multiplayer-game, the grabbed object will be dropped if you get shot.
    /// </summary>
	void OnDestroy ()
	{
		OnRelease ();
	}
}