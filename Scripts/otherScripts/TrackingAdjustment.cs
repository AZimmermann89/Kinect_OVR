/// <summary>
/// This script maps the camera object of the oculus rift to the parameter gameObject "head".
/// @author: Alexander Zimmermann
/// </summary>
using UnityEngine;
using System.Collections;

namespace KinectHandData
{
	public class TrackingAdjustment : MonoBehaviour
	{
        
        #region public member
        public Vector3 headPositionOffset = Vector3.zero;
		public GameObject head = null;
        #endregion public member

        #region private member
        private GameObject cameraObject;
		private bool rotationSet;
		private HandDataInterface _hdi;
		private int adjustCounter;
		private int deadjustCounter;
		private Vector3 cameraPositionoffset = Vector3.zero;
        #endregion private member
        
        /// <summary>
        /// initialization
        /// </summary>
        void Start ()
		{
			_hdi = new SmoothedHandDataInterface ();
			
			rotationSet = false;
			cameraObject = GameObject.Find ("CameraMain");
			adjustCounter = 0;
			deadjustCounter = 0;
            
		}
       
      
        /// <summary>
        /// If both hand events are 'point' for 60 frames, map the oculus camera to the head
        /// </summary>
        void Update ()
		{
			
			//Setting position of the OVR to the head + the Offset
			gameObject.transform.position = head.transform.position + headPositionOffset;
			
			if (!rotationSet) {
				if (_hdi.getHandEvent (true) == HandEventType.Point && _hdi.getHandEvent (false) == HandEventType.Point) {
					adjustCounter++;
				} else {
					deadjustCounter++;
				}
				if (deadjustCounter == 200) {
					deadjustCounter = 0;
					adjustCounter = 0;
				}
				
				if (adjustCounter == 60) {
					
					
					gameObject.transform.rotation = Quaternion.identity;
					cameraPositionoffset = UnityEngine.VR.InputTracking.GetLocalPosition(UnityEngine.VR.VRNode.CenterEye);
					UnityEngine.VR.InputTracking.Recenter();
                    overlayTextChanger.g_oTC.setStateClear();
                    
					rotationSet = true;
					
				}
			}
		}
    }
}
