using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.EventSystems;

public enum UnityMouse{
	None = -1,
	Left = 0,
	Right = 1,
	Middle = 2
}

public class UnityInput : IInput {
	int mousePan = (int)UnityMouse.Left;
	int mouseRotate = (int)UnityMouse.Middle;
	int mouseClick = (int)UnityMouse.Left;
	string mouseX = "Mouse X";
	string mouseY = "Mouse Y";

	string panXAxis = "Horizontal";
	string panYAxis = "Vertical";

	string rotateXAxis = "";
	string rotateYAxis = "";

	string zoomAxis = "Mouse ScrollWheel";

	public UnityInput (int mousePan, int mouseRotate, string mouseX, string mouseY, string panXAxis, string panYAxis, string rotateXAxis, string rotateYAxis, string zoomAxis)
	{
		this.mousePan = mousePan;
		this.mouseRotate = mouseRotate;
		this.mouseX = mouseX;
		this.mouseY = mouseY;
		this.panXAxis = panXAxis;
		this.panYAxis = panYAxis;
		this.rotateXAxis = rotateXAxis;
		this.rotateYAxis = rotateYAxis;
		this.zoomAxis = zoomAxis;
	}
	
	//This is default unity inputs (Edit -> Project Settings -> Inputs)
	#region IInput implementation
	public float GetXMove ()
	{
		if (EventSystem.current.IsPointerOverGameObject()) {
			return 0f;
		}

		if (!string.IsNullOrEmpty(panXAxis) && Input.GetAxis (panXAxis) != 0) {
			return Input.GetAxis (panXAxis);
		}
			
		if (mousePan >= 0 && Input.GetMouseButton (mousePan)) {
			return -Input.GetAxis (mouseX);
		}
			
		return 0f;
	}

	public float GetYMove ()
	{
		if (EventSystem.current.IsPointerOverGameObject()) {
			return 0f;
		}

		if (!string.IsNullOrEmpty(panYAxis) &&  Input.GetAxis (panYAxis) != 0) {
			return Input.GetAxis (panYAxis);
		}

		if (mousePan >= 0 && Input.GetMouseButton (mousePan)) {
			return -Input.GetAxis (mouseY);
		}

		return 0f;
	}

	public float GetXRotation()
	{
		if (EventSystem.current.IsPointerOverGameObject()) {
			return 0f;
		}

		if (!string.IsNullOrEmpty(rotateXAxis) && Input.GetAxis (rotateXAxis) != 0) {
			return Input.GetAxis (rotateXAxis);
		}

		if (mouseRotate >= 0 && Input.GetMouseButton (mouseRotate)) {
			return Input.GetAxis (mouseX);
		}

		return 0f;
	}

	public float GetYRotation()
	{
		if (EventSystem.current.IsPointerOverGameObject()) {
			return 0f;
		}

		if (!string.IsNullOrEmpty(rotateYAxis) && Input.GetAxis (rotateYAxis) != 0) {
			return Input.GetAxis (rotateYAxis);
		}

		if (mouseRotate >= 0 && Input.GetMouseButton (mouseRotate)) {
			return Input.GetAxis (mouseY);
		}

		return 0f;
	}

	public float GetZoomAmount (){
		if (EventSystem.current.IsPointerOverGameObject()) {
			return 0f;
		}

		if (!string.IsNullOrEmpty (zoomAxis)) {
			return Input.GetAxis (zoomAxis);
		}

		return 0f;
	}


	public IEnumerator StartRecordingClick (System.Action<Ray> callbackRay){
		float timer = Mathf.Infinity;
		while (true) {
			if (Input.GetMouseButtonDown(mouseClick)) {
				timer = 0f;
			}

			if (Input.GetMouseButtonUp(mouseClick)) {
				if (timer < 0.5f && !EventSystem.current.IsPointerOverGameObject()) {
					callbackRay (Camera.main.ScreenPointToRay(Input.mousePosition));
				}
				timer = Mathf.Infinity;
			}

			timer += Time.deltaTime;
			yield return null;
		}
	}

	#endregion
}
