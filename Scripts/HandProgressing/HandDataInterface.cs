/// <summary>
/// HandData Interface
/// @author: Alexander Zimmermann
/// </summary>
using System;
using UnityEngine;

namespace KinectHandData{
	public interface HandDataInterface
	{
		Vector3 getHandPosition(bool isLeftHand);
		Quaternion getHandOrientation(bool isLeftHand);
		HandEventType getHandEvent(bool isLeftHand);
	}
}


