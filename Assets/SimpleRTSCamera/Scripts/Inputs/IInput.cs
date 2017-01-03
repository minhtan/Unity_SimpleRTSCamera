using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public enum InputType{
	Unity
}

public interface IInput {
	float GetXMove ();
	float GetYMove ();
	float GetXRotation ();
	float GetYRotation ();
	float GetZoomAmount ();

	IEnumerator StartRecordingClick (System.Action<Ray> callbackRay);
}
