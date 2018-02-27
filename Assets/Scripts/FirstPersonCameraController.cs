using UnityEngine;
using System.Collections;

public class FirstPersonCameraController : MonoBehaviour {

	public float SensitivityX = 15f;
	public float SensitivityY = 15f;

	public float MaxAngleY = 90;
	public float MinAngleY = 90;

	private bool canTurn = false;
	private Transform cameraTransform;
	private float rotationY;

	void Start() {
		PlayerController.Instance.OnPlayerStateChange += OnPlayerStateChange;

		cameraTransform = GetComponentInChildren<Camera>().transform;
		Cursor.lockState = CursorLockMode.Locked;

		rotationY = -cameraTransform.localEulerAngles.x;
	}

	void Update() {
		if(canTurn) {
			float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * SensitivityX;
			rotationY += Input.GetAxis("Mouse Y") * SensitivityY;
			rotationY = Mathf.Clamp(rotationY, MinAngleY, MaxAngleY);
			transform.localEulerAngles = new Vector3(0, rotationX, 0);
			cameraTransform.localEulerAngles = new Vector3(-rotationY, 0, 0);
		}
	}

	void OnPlayerStateChange(PlayerController.PlayerState state) {
		canTurn = PlayerController.Instance.PlayerCanControl;
	}

}
