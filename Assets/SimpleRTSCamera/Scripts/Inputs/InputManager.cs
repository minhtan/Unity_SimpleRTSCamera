using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {
	public InputType inputType;

	IInput input;

	#region Unity Input
	public UnityMouse mousePan = UnityMouse.Left;
	public UnityMouse mouseRotate = UnityMouse.Middle;
	public string mouseX = "Mouse X";
	public string mouseY = "Mouse Y";

	public string panXAxis = "Horizontal";
	public string panYAxis = "Vertical";

	public string rotateXAxis = "";
	public string rotateYAxis = "";

	public string zoomAxis = "Mouse ScrollWheel";
	#endregion

	void Awake(){
		switch (inputType) {
		case InputType.Unity:
			input = new UnityInput ((int)mousePan, (int)mouseRotate, mouseX, mouseY, panXAxis, panYAxis, rotateXAxis, rotateYAxis, zoomAxis);	
			break;
		default:
			input = new UnityInput ((int)mousePan, (int)mouseRotate, mouseX, mouseY, panXAxis, panYAxis, rotateXAxis, rotateYAxis, zoomAxis);	
			break;
		}
	}

	void Start (){
//		StartCoroutine (input.StartRecordingClick (CheckClick));
	}

	void Update () {
		CheckPanInputs ();
		CheckRotateInputs ();
		CheckZoomInputs ();
	}

	void CheckPanInputs(){
		var x = input.GetXMove();

		if (x != 0) {
			Messenger.Broadcast<float> (Events.Input.PAN_CAM_X, x);
		}

		var y = input.GetYMove();

		if (y != 0) {
			Messenger.Broadcast<float> (Events.Input.PAN_CAM_Y, y);
		}
	}

	void CheckRotateInputs(){
		var x = input.GetXRotation ();

		if (x != 0) {
			Messenger.Broadcast<float> (Events.Input.ROTATE_CAM_X, x);
		}

		var y = input.GetYRotation ();

		if (y != 0) {
			Messenger.Broadcast<float> (Events.Input.ROTATE_CAM_Y, y);
		}
	}

	void CheckZoomInputs(){
		var amount = input.GetZoomAmount ();

		if (amount != 0) {
			Messenger.Broadcast<float> (Events.Input.ZOOM_CAM, amount);
		}
	}

	void CheckClick(Ray ray){
		RaycastHit hitInfo;
		if (Physics.Raycast (ray, out hitInfo)) {
			Messenger.Broadcast<GameObject> (Events.Input.CLICK_HIT_SOMETHING, hitInfo.collider.gameObject);
			Messenger.Broadcast<Vector3> (Events.Input.CLICK_HIT_POS, hitInfo.point);
		}else{
			Messenger.Broadcast (Events.Input.CLICK_HIT_NOTHING);
		}
	}
}
