using UnityEngine;
using System.Collections;

public class MovementController : MonoBehaviour {

	public float Speed = 6.0f;
	public float Gravity = 20.0f;
	public bool AirControl = false;

	public bool canMove = true;

	private CharacterController charController;

	private Vector3 moveDirection = Vector3.zero;
	private bool grounded = false;
	private RaycastHit hit;
	private float fallStartLevel;
	private bool falling;
	private float rayDistance;
	private Vector3 contactPoint;
	private bool playerControl = false;

	void Start() {
		PlayerController.Instance.OnPlayerStateChange += OnPlayerStateChange;
		charController = GetComponent<CharacterController>();
		rayDistance = charController.height * 0.5f + charController.radius;
	}

	void FixedUpdate() {
		float inputX = 0;
		float inputY = 0;
		if(canMove) {
			inputX = Input.GetAxis("Horizontal");
			inputY = Input.GetAxis("Vertical");
		}
			
		if(grounded) {
			moveDirection = new Vector3(inputX, 0, inputY);
			moveDirection = transform.TransformDirection(moveDirection) * Speed;
			playerControl = true;
		} else {
			if(!falling) {
				falling = true;
			}

			if(AirControl && playerControl) {
				moveDirection.x = inputX * Speed;
				moveDirection.y = inputY * Speed;
			}
		}

		moveDirection.y -= Gravity * Time.fixedDeltaTime;
		CollisionFlags flags = charController.Move(moveDirection * Time.deltaTime);
		grounded = (flags & CollisionFlags.Below) != 0;
	}

	void OnPlayerStateChange(PlayerController.PlayerState state) {
		canMove = PlayerController.Instance.PlayerCanControl;
	}
}
