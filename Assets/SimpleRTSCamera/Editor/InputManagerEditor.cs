using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(InputManager))]
public class InputManagerEditor : Editor {
	public override void OnInspectorGUI()
	{
		InputManager myTarget = (InputManager)target;

		myTarget.inputType = (InputType)EditorGUILayout.EnumPopup("Input type", myTarget.inputType);

		switch (myTarget.inputType) {
		case InputType.Unity:
			DrawUnityInput (myTarget);
			break;
		default:
			break;
		}
	}

	void DrawUnityInput(InputManager myTarget){
		myTarget.mousePan = (UnityMouse)EditorGUILayout.EnumPopup ("Mouse to pan", myTarget.mousePan);
		myTarget.mouseRotate = (UnityMouse)EditorGUILayout.EnumPopup ("Mouse to rotate", myTarget.mouseRotate);
		myTarget.mouseX = EditorGUILayout.TextField ("MouseX axis name", myTarget.mouseX);
		myTarget.mouseY = EditorGUILayout.TextField ("MouseY axis name", myTarget.mouseY);
		myTarget.panXAxis = EditorGUILayout.TextField ("Horizontal pan axis name", myTarget.panXAxis);
		myTarget.panYAxis = EditorGUILayout.TextField ("Vertical pan axis name", myTarget.panYAxis);
		myTarget.rotateXAxis = EditorGUILayout.TextField ("Horizontal rotate axis name", myTarget.rotateXAxis);
		myTarget.rotateYAxis = EditorGUILayout.TextField ("Vertical rotate axis name", myTarget.rotateYAxis);
		myTarget.zoomAxis = EditorGUILayout.TextField ("Zoom axis name", myTarget.zoomAxis);
	}
}
