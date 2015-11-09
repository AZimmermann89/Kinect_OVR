/// <summary>
/// An implementation of HandData which delivers smoothed data.
/// @author: Alexander Zimmermann
/// </summary>
using System;
using UnityEngine;

namespace KinectHandData{
	public class SmoothedHandDataInterface : HandDataInterface
	{
        #region private member
        private HandData _hd;
        private HandEventType current_event;
        private HandEventType[] handEventStorage;
        private Quaternion hand_rotation_;
        private int storageSize = 4;
		private int storageIterator = 0;
		private float rotationFiltering = 0.5f;
        #endregion private member

        /// <summary>
        /// returns the position of the hand
        /// </summary>
        /// <param name="isLeftHand"></param>
        /// <returns></returns>
        public Vector3 getHandPosition(bool isLeftHand){
			if (!HandData.IsHandDataInitialized()) _hd = HandData.Instance;
			return _hd.getHandPosition(isLeftHand);
		}

        /// <summary>
        /// returns the orientation of the hand
        /// </summary>
        /// <param name="isLeftHand"></param>
        /// <returns></returns>
        public Quaternion getHandOrientation(bool isLeftHand){
			if (!HandData.IsHandDataInitialized()) _hd = HandData.Instance;

			hand_rotation_ = Quaternion.Slerp (hand_rotation_, _hd.getHandOrientation (isLeftHand), (1.0f - rotationFiltering));

			return hand_rotation_;
        }

        /// <summary>
        /// returns the state of the hand
        /// </summary>
        /// <param name="isLeftHand"></param>
        /// <returns></returns>
		public HandEventType getHandEvent(bool isLeftHand){
			if (!HandData.IsHandDataInitialized()) _hd = HandData.Instance;

			HandEventType new_event = _hd.getHandEvent (isLeftHand);
			int current_event_counter = 0;

			handEventStorage [storageIterator % storageSize] = new_event;

			for (int i = 0; i < storageSize; i++) {
				if(handEventStorage[i] == new_event) current_event_counter++;
			}

			if (current_event_counter == storageSize) { 
				current_event = new_event;
			}

			storageIterator++;
			return current_event;
		}

        /// <summary>
        /// constructor of the interface
        /// </summary>
        public SmoothedHandDataInterface ()
		{
			_hd = HandData.Instance;
			hand_rotation_ = Quaternion.identity;
			current_event = HandEventType.None;

			handEventStorage = new HandEventType[storageSize];

			for (int i = 0; i < storageSize; i++) {
				handEventStorage[i] = HandEventType.None;
			}
		}
	}
}