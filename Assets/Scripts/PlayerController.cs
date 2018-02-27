using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public static PlayerController Instance;

	public delegate void PlayerStateAction(PlayerState state);
	public event PlayerStateAction OnPlayerStateChange;

	public enum PlayerState {
		FreeMoving,
		AnimationLocked,
		Repairing,
		Sleeping,
		Sitting
	}

	private PlayerState state;
	public PlayerState State {
		get {
			return state;
		}
		set {
			state = value;
			if(OnPlayerStateChange != null) {
				OnPlayerStateChange(state);
			}
		}
	}

	public float WaterPerDrink = 1;
	public float ThirstSatisfactionPerWater = 1;
	public float FoodPerEat = 5;
	public float HungerSatisfactionPerFood = 1;
	public float InteractRange = 5;
	public float RepairRate = 1;

	public AudioSource glug;
	public AudioSource nom;

	public bool PlayerCanControl {
		get {
			return State == PlayerState.FreeMoving;
		}
	}

	private FirstPersonCameraController cameraController;
	private MovementController movementController;
	private ShipSystemManager systemManager;
	private WaterDispenser waterDispenser;
	private FoodDispenser foodDispenser;
	private PlayerResourceManager playerResources;
	private ShipSystem currentRepairingSystem;
	private Transform cameraTransform;
	private UIManager uiManager; // FIXME: BAD COUPLING

	private InteractableObject hoveringObj;
	private ShipSystem hoveringBrokenSystem;

	void Awake() {
		Instance = this;
	}

	void Start() {
		OnPlayerStateChange += OnPlayerStateChanged;
		state = PlayerState.Sitting;

		cameraController = GetComponent<FirstPersonCameraController>();
		movementController = GetComponent<MovementController>();
		cameraTransform = GetComponentInChildren<Camera>().transform;
		uiManager = UIManager.Instance;
		systemManager = ShipSystemManager.Instance;
		waterDispenser = systemManager.GetComponent<WaterDispenser>();
		foodDispenser = systemManager.GetComponent<FoodDispenser>();
		playerResources = PlayerResourceManager.Instance;

		uiManager.UpdateTooltipTextSub(null);
		uiManager.UpdateTooltipText("Begin", true);

		StartCoroutine(LateStart());
	}

	IEnumerator LateStart() {
		yield return new WaitForSeconds(0.001f);
		State = PlayerState.Sitting;
	}

	void Update() {
		if(State == PlayerState.Repairing) {
			if(currentRepairingSystem.Health < currentRepairingSystem.MaxHealth) {
				currentRepairingSystem.Repair(RepairRate * TimeManager.Instance.GameDeltaTime);
				uiManager.UpdateRepairBar(currentRepairingSystem.Health / currentRepairingSystem.MaxHealth);
			} else {
				StopRepairing();
			}

			if(Input.GetMouseButtonDown(0)) {
				StopRepairing();
			}

		} else if(State == PlayerState.Sleeping) {
			if(playerResources.Energy == playerResources.EnergyCap) {
				StopSleeping();
			}

			if(Input.GetMouseButtonDown(0)) {
				StopSleeping();
			}

		} else if(State == PlayerState.Sitting) { 
			if(Input.GetMouseButtonDown(0)) {
				State = PlayerState.FreeMoving;
			}

		} else if(State == PlayerState.FreeMoving) {
			Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
			RaycastHit hit;

			hoveringObj = null;
			hoveringBrokenSystem = null;

			if(Physics.Raycast(cameraTransform.position, forward, out hit, InteractRange)) {	
				hoveringObj = hit.transform.GetComponent<InteractableObject>();

				if(hoveringObj) {
					if(hoveringObj is SystemInteractableObject) {
						SystemInteractableObject obj = (hoveringObj as SystemInteractableObject);
//						if(obj.System.Interactable) {
							if(obj.System.Broken) {
								uiManager.UpdateTooltipText("Repair " + obj.System.Name, true);
								hoveringBrokenSystem = obj.System;
							} else {
								uiManager.UpdateTooltipText(hoveringObj.DisplayText, hoveringObj.ShowMouseIcon);
							}

						uiManager.UpdateTooltipTextSub(obj.systemDescription);
//						} else {
//							uiManager.UpdateTooltipText(null, false);
//						}
					} else {
						uiManager.UpdateTooltipText(hoveringObj.DisplayText, hoveringObj.ShowMouseIcon);
						uiManager.UpdateTooltipTextSub(null);
					}
				} else {
					uiManager.UpdateTooltipText(null, false);
					uiManager.UpdateTooltipTextSub(null);
				}
			} else {
				uiManager.UpdateTooltipText(null, false);
				uiManager.UpdateTooltipTextSub(null);
			}

			if(Input.GetMouseButtonDown(0)) {
				if(hoveringObj != null) {
					if(hoveringBrokenSystem != null) {
						StartRepairing(hoveringBrokenSystem);
					} else {
						hoveringObj.Click(hit.point);
					}
				}
			}
		}

		if(State != PlayerState.AnimationLocked) {
			if(Input.GetMouseButton(1)) {
				TimeManager.Instance.Fast = true;
			} else {
				TimeManager.Instance.Fast = false;
			}
		}
	}

	public void DrinkWaterFromDispenser() {
		float amount = Mathf.Min(WaterPerDrink * ThirstSatisfactionPerWater, playerResources.ThirstCap - playerResources.Thirst);
		float amountAvailable = waterDispenser.DispenseWater(amount / ThirstSatisfactionPerWater);
		float amt = playerResources.ChangeThirst(amountAvailable * ThirstSatisfactionPerWater);
		if(amt > 0) {
			glug.Play(40000);
		}

	}

	public void EatFoodFromDispenser() {
		float amount = FoodPerEat * HungerSatisfactionPerFood;
		float amountAvailable = foodDispenser.DispenseFood(amount);
		float amt = playerResources.ChangeHunger(amountAvailable);
		if(amt > 0) {
			nom.Play(40000);
		}
	}

	public void Sleep() {
		State = PlayerState.Sleeping;
		uiManager.UpdateTooltipText("Stop sleeping", true);
	}

	public void StopSleeping() {
		State = PlayerState.FreeMoving;
		uiManager.UpdateTooltipText(null, true);
	}

	public void StartRepairing(ShipSystem system) {
		currentRepairingSystem = system;
		State = PlayerState.Repairing;
		uiManager.UpdateTooltipText("Stop repairing", true);
		uiManager.ShowRepairBar();
	}

	public void StopRepairing() {
		State = PlayerState.FreeMoving;
		uiManager.UpdateTooltipText(null, false);
		uiManager.HideRepairBar();
	}

	private void OnPlayerStateChanged(PlayerState state) {
		if(state == PlayerState.Sleeping) {
			uiManager.DarkOverlayEnabled = true;
			TimeManager.Instance.Accelerate();
		} else {
			uiManager.DarkOverlayEnabled = false;
			TimeManager.Instance.Decelerate();
		}
	}
}
