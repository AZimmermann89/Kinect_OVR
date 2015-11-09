/// <summary>
/// Singleton-HandData that provides methods to get the position, orientation and state of the hands.
/// The data is unaltered.
/// If you wan't to change the data from the sensor, write an adapter. Don't change this class.
/// Use an HandDataInterface to get the data.
/// @author: Alexander Zimmermann
/// </summary>
using UnityEngine;
using System.Collections;

namespace KinectHandData{
	public class HandData : MonoBehaviour {

		#region singleton
		private bool handDataInitialized = false; 
		private static HandData instance = null;

		public static HandData Instance
		{
			get
			{
				return instance;
			}
		}

		public static bool IsHandDataInitialized()
		{
			return instance != null ? instance.handDataInitialized : false;
		}

		public bool IsInitialized()
		{
			return handDataInitialized;
		}
		#endregion singleton

		#region private member
		private KinectManager _km;
		
		private long primaryUserID;
		
		//left side
		private Vector3 leftHandPos;
		private Quaternion leftHandOri;
		private HandEventType lastLeftHandEvent;
		private HandEventType leftHandEvent;
		private KinectInterop.HandState leftHandState;
		
		//right side
		private Vector3 rightHandPos;		
		private Quaternion rightHandOri;
		private HandEventType lastRightHandEvent;
		private HandEventType rightHandEvent;
		private KinectInterop.HandState rightHandState;
		#endregion private member

		#region start
        /// <summary>
        /// initialization
        /// </summary>
		void Start () {
			// set the singleton instance
			instance = this;

			_km = KinectManager.Instance;
			primaryUserID = 0;

			//left init
			leftHandPos = Vector3.zero;
			leftHandOri = Quaternion.identity;
			lastLeftHandEvent = HandEventType.Release;
			leftHandEvent = HandEventType.None;
			leftHandState = KinectInterop.HandState.Unknown;
			
			//right init
			rightHandPos = Vector3.zero;
			rightHandOri = Quaternion.identity;
			lastRightHandEvent = HandEventType.Release;
			rightHandEvent = HandEventType.None;
			rightHandState = KinectInterop.HandState.Unknown;
		}
		#endregion start
		
		#region update
		void Update () {
			if (_km == null) {
				_km = KinectManager.Instance;
			}
			
			if (_km && _km.IsInitialized ()) {
					
				primaryUserID = _km.GetUserIdByIndex(0);

				this.pullPosition (primaryUserID);
				this.pullOrientation (primaryUserID);
				this.pullState (primaryUserID);

			}

		}
        #endregion update

        #region pull data
        /// <summary>
        /// Gets the data from the KinectManager
        /// </summary>
        private void pullPosition(long userID){

			if (userID != 0) {
				
				if(_km.GetJointTrackingState(userID, (int)KinectInterop.JointType.HandLeft) != KinectInterop.TrackingState.NotTracked)
				{
					leftHandPos = _km.GetJointPosition(userID, (int)KinectInterop.JointType.HandLeft);
				}
				
				if(_km.GetJointTrackingState(userID, (int)KinectInterop.JointType.HandRight) != KinectInterop.TrackingState.NotTracked)
				{
					rightHandPos = _km.GetJointPosition(userID, (int)KinectInterop.JointType.HandRight);

				}
			}
		}

		private void pullOrientation(long userID){
			if (userID != 0) {
				
				if(_km.GetJointTrackingState(userID, (int)KinectInterop.JointType.HandLeft) != KinectInterop.TrackingState.NotTracked)
				{
					leftHandOri = _km.GetJointOrientation(userID, (int)KinectInterop.JointType.HandLeft, false);
				}
				
				if(_km.GetJointTrackingState(userID, (int)KinectInterop.JointType.HandRight) != KinectInterop.TrackingState.NotTracked)
				{
					rightHandOri = _km.GetJointOrientation(userID, (int)KinectInterop.JointType.HandRight, false);
				}
			}
		}
		
		private void pullState(long userID){
			if(userID != 0)
			{
				HandEventType handEvent = HandEventType.None;
				
				// get the left hand state
				leftHandState = _km.GetLeftHandState(userID);
				// get the right hand state
				rightHandState = _km.GetRightHandState(userID);


				// process left hand
				handEvent = HandStateToEvent(leftHandState, lastLeftHandEvent);
				leftHandEvent = handEvent;
				

				if(handEvent != HandEventType.None)
				{
					lastLeftHandEvent = handEvent;
				}
				
				
				
				// process right hand
				handEvent = HandStateToEvent(rightHandState, lastRightHandEvent);
				rightHandEvent = handEvent;
				if(handEvent != HandEventType.None)
				{
					lastRightHandEvent = handEvent;
				}	

				
			}
			else
			{
				leftHandState = KinectInterop.HandState.NotTracked;
				rightHandState = KinectInterop.HandState.NotTracked;

				leftHandEvent = HandEventType.None;
				rightHandEvent = HandEventType.None;
				
				lastLeftHandEvent = HandEventType.Release;
				lastRightHandEvent = HandEventType.Release;

			}
		}
		#endregion pull data
		
		#region convert hand event
        /// <summary>
        /// Converts the HandState to an event
        /// </summary>
        /// <param name="handState"></param>
        /// <param name="lastEventType"></param>
        /// <returns></returns>
		private HandEventType HandStateToEvent(KinectInterop.HandState handState, HandEventType lastEventType)
		{
			switch(handState)
			{
			case KinectInterop.HandState.Open:
				return HandEventType.Release;
				
			case KinectInterop.HandState.Lasso:
				return HandEventType.Point;

			case KinectInterop.HandState.Closed:
				return HandEventType.Grip;
				
			case KinectInterop.HandState.Unknown:
				return lastEventType;
			}
			
			return HandEventType.None;
		}
		#endregion convert hand event
		
		#region getter
		internal Vector3 getHandPosition(bool isLeftHand){
			if (isLeftHand)
				return leftHandPos;
			else
				return rightHandPos;
			
		}
		
		internal Quaternion getHandOrientation(bool isLeftHand){
			if (isLeftHand)
				return leftHandOri;
			else
				return rightHandOri;
		}
		
		internal HandEventType getHandEvent(bool isLeftHand){
			if (isLeftHand)
				return leftHandEvent;
			else
				return rightHandEvent;
		}
		#endregion getter

	}
}