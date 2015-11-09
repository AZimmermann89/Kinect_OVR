/// <summary>
/// An implementation of HandData which delivers unaltered data.
/// @author: Alexander Zimmermann
/// </summary>//------------------------------------------------------------------------------
using System;
using UnityEngine;

namespace KinectHandData{
	public class UnalteredHandDataInterface : HandDataInterface
	{
		private HandData _hd;

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
			return _hd.getHandOrientation(isLeftHand);
			

		}

        /// <summary>
        /// returns the state of the hand
        /// </summary>
        /// <param name="isLeftHand"></param>
        /// <returns></returns>
        public HandEventType getHandEvent(bool isLeftHand){
			if (!HandData.IsHandDataInitialized()) _hd = HandData.Instance;
			return _hd.getHandEvent(isLeftHand);

		}

        /// <summary>
        /// constructor
        /// </summary>
        public UnalteredHandDataInterface ()
		{
			_hd = HandData.Instance;
		}
	}
}

