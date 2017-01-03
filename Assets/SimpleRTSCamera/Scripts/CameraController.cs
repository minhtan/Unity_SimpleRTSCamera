using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour {
	public LayerMask groundLayer;
	public float panSpeed = 1f; //in unity unit
	public float rotateSpeed = 1f; //in degrees
	[Tooltip("mininam camera's vertical angle")][Range(1, 90)]
	public float minCamAngleX = 5f;
	[Tooltip("maximum camera's vertical angle")][Range(1, 90)]
	public float maxCamAngleX = 90f;

	public float zoomSpeed = 1f; //in unity unit
	[Tooltip("max zoom out height scale compare to init position")]
	public float maxCamHeightScale = 5f; //max zoom out height scale compare to init position
	[Tooltip("max zoom in height scale compare to init position")]
	public float minCamHeightScale = 0.2f; //max zoom in height scale compare to init position

	public bool ShowDebug = true;

	Transform camera;
	float maxCamHeight;
	float minCamHeight;
	IEnumerator xSmooth;
	IEnumerator ySmooth;

	void Awake () {
		if (GetComponent<Camera> () != null) {
			camera = transform;
		} else {
			camera = Camera.main.transform;
		}

		Messenger.AddListener<float> (Events.Input.PAN_CAM_X, PanCamX);
		Messenger.AddListener<float> (Events.Input.PAN_CAM_Y, PanCamY);
		Messenger.AddListener<float> (Events.Input.ROTATE_CAM_X, RotateCamX);
		Messenger.AddListener<float> (Events.Input.ROTATE_CAM_Y, RotateCamY);
		Messenger.AddListener<float> (Events.Input.ZOOM_CAM, ZoomCam);

		//vars set up
		maxCamHeight = camera.position.y * maxCamHeightScale;
		minCamHeight = camera.position.y * minCamHeightScale;
	}

	void OnDestroy(){
		Messenger.RemoveListener<float> (Events.Input.PAN_CAM_X, PanCamX);
		Messenger.RemoveListener<float> (Events.Input.PAN_CAM_Y, PanCamY);
		Messenger.RemoveListener<float> (Events.Input.ROTATE_CAM_X, RotateCamX);
		Messenger.RemoveListener<float> (Events.Input.ROTATE_CAM_Y, RotateCamY);
		Messenger.RemoveListener<float> (Events.Input.ZOOM_CAM, ZoomCam);
	}

	void Start(){
		CheckForInput ();
	}

	void CheckForInput(){
		if (EventSystem.current == null) {
			var go = new GameObject ();
			go.name = "EventSystem";
			go.AddComponent<EventSystem> ();
			go.AddComponent<StandaloneInputModule> ();
		}
		if (FindObjectOfType<InputManager> () == null) {
			var go = new GameObject ();
			go.name = "InputManager";
			go.AddComponent<InputManager> ();
		}
	}

	#region Pan

	bool PanCamCheck(Vector3 newPos){
		if (ShowDebug) {
			Debug.DrawRay (newPos, camera.forward * 100f, Color.red);
		}
		RaycastHit hit;
		if (Physics.Raycast (newPos, camera.forward, out hit, Mathf.Infinity, groundLayer)) {
			return true;
		} else {
			return false;
		}
	}

	bool MoveIfValid(Vector3 newPos){
		if (PanCamCheck (newPos)) {
			if (xSmooth != null) {
				StopCoroutine (xSmooth);
			}
			if (ySmooth != null) {
				StopCoroutine (ySmooth);
			}
			camera.position = newPos;
			return true;
		} else {
			return false;
		}
	}

	IEnumerator PanSmooth(Vector3 toward){
		while(PanCamCheck (Vector3.MoveTowards (camera.position, toward, panSpeed/20))){
			camera.position = Vector3.MoveTowards (camera.position, toward, panSpeed/20);
			yield return null;
		}
	}

	void PanCamX(float value){
		var a = camera.rotation.eulerAngles.y;
		Vector3 addPos;

		//move along x axis first
		addPos = new Vector3 (value * panSpeed * Mathf.Cos(a * Mathf.Deg2Rad), 0f, 0f);
		if (!MoveIfValid (camera.position + addPos)) {
			xSmooth = PanSmooth (camera.position + addPos) ;
			StartCoroutine (xSmooth);
		}

		//then move along z axis
		addPos = new Vector3 (0f, 0f, -value * panSpeed * Mathf.Sin (a * Mathf.Deg2Rad));
		if (!MoveIfValid (camera.position + addPos)) {
			ySmooth = PanSmooth (camera.position + addPos);
			StartCoroutine (ySmooth);
		}
	}

	void PanCamY(float value){
		var a = camera.rotation.eulerAngles.y;
		Vector3 addPos;

		//move along x axis first
		addPos = new Vector3 (value * panSpeed * Mathf.Sin (a * Mathf.Deg2Rad), 0f, 0f);
		if (!MoveIfValid (camera.position + addPos)) {
			xSmooth = PanSmooth (camera.position + addPos);
			StartCoroutine (xSmooth);
		}

		//then move along z axis
		addPos = new Vector3 (0f, 0f, value * panSpeed * Mathf.Cos (a * Mathf.Deg2Rad));
		if (!MoveIfValid (camera.position + addPos)) {
			ySmooth = PanSmooth (camera.position + addPos);
			StartCoroutine (ySmooth);
		}
	}

	#endregion

	#region Rotate

	void RotateCamX(float angle){
		RaycastHit hit;
		if (ShowDebug) {
			Debug.DrawRay (camera.position, camera.forward * 100f, Color.red);
		}
		if (Physics.Raycast (camera.position, camera.forward, out hit, Mathf.Infinity, groundLayer)) {
			camera.RotateAround (hit.point, Vector3.up, angle * rotateSpeed);
		}
	}

	void RotateCamY(float angle){
		RaycastHit hit;
		if (ShowDebug) {
			Debug.DrawRay (camera.position, camera.forward * 100f, Color.red);
		}
		if (Physics.Raycast (camera.position, camera.forward, out hit, Mathf.Infinity, groundLayer)) {
			camera.RotateAround (hit.point, camera.right, angle * rotateSpeed);

			Debug.Log (Vector3.Angle(camera.forward, Vector3.ProjectOnPlane(camera.forward, Vector3.up)) + " " + camera.localRotation.eulerAngles.x);

//			if (!Physics.Raycast (camera.position, camera.forward, out hit, Mathf.Infinity, groundLayer) 
//				|| camera.localRotation.eulerAngles.x < minCamAngleX 
//				|| camera.localRotation.eulerAngles.x > maxCamAngleX) {
//				camera.RotateAround (hit.point, camera.right, -angle * rotateSpeed);
//			}
		}
	}

	#endregion

	#region Zoom

	void ZoomCam(float amount){
		RaycastHit hit;
		if (ShowDebug) {
			Debug.DrawRay (camera.position, camera.forward * 100f, Color.red);
		}
		if (Physics.Raycast (camera.position, camera.forward, out hit, Mathf.Infinity, groundLayer)) {
			if (ZoomCamCheck(Vector3.MoveTowards (camera.position, hit.point, amount * zoomSpeed))) {
				camera.position = Vector3.MoveTowards (camera.position, hit.point, amount * zoomSpeed);
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
