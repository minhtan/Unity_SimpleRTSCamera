using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	#region vars

	Transform camera;
	float maxPanStep = 1f; //in unity unit
	float maxRotateStep = 8f; //in degrees
	float camZoomStep = 1.5f; //in unity unit
	float maxCamHeightScale = 2.5f; //max zoom out height scale compare to init position
	float minCamHeightScale = 0.5f; //max zoom in height scale compare to init position
	float maxCamHeight;
	float minCamHeight;

	IEnumerator smoothX;
	IEnumerator smoothZ;
	#endregion

	public void Start ()
	{
		Messenger.AddListener<float> (Events.Input.PAN_CAM_X, PanCamX);
		Messenger.AddListener<float> (Events.Input.PAN_CAM_Y, PanCamY);
		Messenger.AddListener<float> (Events.Input.ROTATE_CAM, RotateCam);
		Messenger.AddListener<float> (Events.Input.ZOOM_CAM, ZoomCam);

		//vars set up
		camera = Camera.main.transform;
		maxCamHeight = camera.position.y * maxCamHeightScale;
		minCamHeight = camera.position.y * minCamHeightScale;
	}

	public void OnDestroy ()
	{
		Messenger.RemoveListener<float> (Events.Input.PAN_CAM_X, PanCamX);
		Messenger.RemoveListener<float> (Events.Input.PAN_CAM_Y, PanCamY);
		Messenger.RemoveListener<float> (Events.Input.ROTATE_CAM, RotateCam);
		Messenger.RemoveListener<float> (Events.Input.ZOOM_CAM, ZoomCam);
	}

	#region Pan

	bool PanCamCheck(Vector3 newPos){
		RaycastHit hit;
		if (Physics.Raycast (newPos, camera.forward, out hit)) {
			return true;
		} else {
			return false;
		}
	}

	bool MoveIfValid(Vector3 newPos){
		if (PanCamCheck (newPos)) {
			if (smoothX != null) {
				StopCoroutine (smoothX);
				smoothX = null;
			}
			if (smoothZ != null){
				StopCoroutine (smoothZ);
				smoothZ = null;
			}
			camera.position = newPos;
			return true;
		} else {
			return false;
		}
	}

	IEnumerator PanSmooth(Vector3 toward){
		while(PanCamCheck (Vector3.MoveTowards (camera.position, toward, 0.05f))){
			camera.position = Vector3.MoveTowards (camera.position, toward, 0.05f);
			yield return null;
		}
	}

	void PanCamX(float value){
		var a = camera.rotation.eulerAngles.y;
		if (Mathf.Abs(value) > maxPanStep) {
			value = value > 0 ? maxPanStep : -maxPanStep;
		}
		Vector3 addPos;

		//move along x axis first
		addPos = new Vector3 (value * Mathf.Cos(a * Mathf.Deg2Rad), 0f, 0f);
		if (!MoveIfValid (camera.position + addPos)) {
			if(smoothX == null){
				smoothX = PanSmooth (camera.position + addPos);
				StartCoroutine (smoothX);
			}
		}

		//then move along z axis
		addPos = new Vector3 (0f, 0f, -value * Mathf.Sin (a * Mathf.Deg2Rad));
		if (!MoveIfValid (camera.position + addPos)) {
			if(smoothZ == null){
				smoothZ = PanSmooth (camera.position + addPos);
				StartCoroutine (smoothX);
			}
		}
	}

	void PanCamY(float value){
		var a = camera.rotation.eulerAngles.y;
		if (Mathf.Abs(value) > maxPanStep) {
			value = value > 0 ? maxPanStep : -maxPanStep;
		}
		Vector3 addPos;

		//move along x axis first
		addPos = new Vector3 (value * Mathf.Sin (a * Mathf.Deg2Rad), 0f, 0f);
		if (!MoveIfValid (camera.position + addPos)) {
			if(smoothX == null){
				smoothX = PanSmooth (camera.position + addPos);
				StartCoroutine (smoothX);
			}
		}

		//then move along z axis
		addPos = new Vector3 (0f, 0f, value * Mathf.Cos (a * Mathf.Deg2Rad));
		if (!MoveIfValid (camera.position + addPos)) {
			if(smoothZ == null){
				smoothZ = PanSmooth (camera.position + addPos);
				StartCoroutine (smoothX);
			}
		}
	}

	#endregion

	#region Rotate

	void RotateCam(float angle){
		RaycastHit hit;
		if (Physics.Raycast (camera.position, camera.forward, out hit)) {
			if (Mathf.Abs(angle) > maxRotateStep) {
				angle = angle > 0 ? maxRotateStep : -maxRotateStep;
			}
			camera.RotateAround (hit.point, Vector3.up, angle);
		}
	}

	#endregion

	#region Zoom

	void ZoomCam(float amount){
		RaycastHit hit;
		if (Physics.Raycast (camera.position, camera.forward, out hit)) {
			amount = amount > 0 ? camZoomStep : -camZoomStep;
			if (ZoomCamCheck(Vector3.MoveTowards (camera.position, hit.point, amount))) {
				camera.position = Vector3.MoveTowards (camera.position, hit.point, amount);
			}
		}
	}

	bool ZoomCamCheck(Vector3 newPos){
		if (newPos.y >= minCamHeight && newPos.y <= maxCamHeight) {
			return true;
		} else {
			return false;
		}
	}

	#endregion
}
