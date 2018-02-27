using UnityEngine;
using System.Collections;

public class FoodDispenser : ShipSystem {

	public float MaxFoodLevel = 20;
	public float RefillLevel = 5;

	public float DoorOpenAngle;
	public float DoorOpenSpeed;
	public float PlateMoveSpeed;
	public float FoodMoveSpeed;

	public Transform Door;
	public Transform Plate;
	public Transform FoodBlob;

	public Transform PlateRestingMarker;
	public Transform PlateOutMarker;
	public Transform EatingMarker;
	public Transform FoodRestingMarker;

	public GameObject FixedModel;
	public GameObject BrokenModel;

	private float foodLevel;
	public float FoodLevel {
		get {
			return foodLevel;
		}
		protected set {
			foodLevel = value;
			if(foodLevel < RefillLevel) {
				Refill();
			}
		}
	}

	private bool canEat = true;

	private ShipResourceManager shipResources;
	private PlayerController playerController;
	private UIManager uiManager;

	protected override void Start() {
		base.Start();

		SystemFixed += OnFixed;
		SystemBreakDown += OnBroken;

		shipResources = ShipResourceManager.Instance;
		playerController = PlayerController.Instance;
		uiManager = UIManager.Instance;

		FixedModel.SetActive(true);
		BrokenModel.SetActive(false);

		foodLevel = MaxFoodLevel;
	}

	// Reduces the dispensers water level by the given amount, and returns the amount that could actually be taken (i.e. if there is less than amount
	// left in the dispenser then it will return a value lower than amount representing the amount of water that was left)
	public float DispenseFood(float amount) {
		if(canEat && FoodLevel > 0) {
			StartCoroutine(AnimateFood());

			float afterLevel = FoodLevel - amount;
			if(afterLevel < 0) {
				return 0;
			} else {
				FoodLevel = afterLevel;
				return amount;	
			}
		} else {
			return 0;
		}
	}

	private IEnumerator AnimateFood() {
		canEat = false;

		playerController.State = PlayerController.PlayerState.AnimationLocked;
		uiManager.UpdateTooltipText(null, false);

		FoodBlob.position = FoodRestingMarker.position;
		FoodBlob.gameObject.SetActive(true);

		// Animate door opening
		for(float t = 0; t < DoorOpenAngle; t += DoorOpenSpeed * Time.deltaTime) {
			Door.transform.eulerAngles = new Vector3(t, 0, 0);
			yield return null;
		}

		// Animate plate moving out
		while(Vector3.Distance(Plate.position, PlateOutMarker.position) > 0.1f) {
			Plate.position = Vector3.MoveTowards(Plate.position, PlateOutMarker.position, PlateMoveSpeed * Time.deltaTime);
			yield return null;
		}

		// Animate food moving towards player
		while(Vector3.Distance(FoodBlob.position, EatingMarker.position) > 0.1f) {
			FoodBlob.position = Vector3.MoveTowards(FoodBlob.position, EatingMarker.position, FoodMoveSpeed * Time.deltaTime);
			yield return null;
		}
		FoodBlob.gameObject.SetActive(false);

		// Animate plate moving in
		while(Vector3.Distance(Plate.position, PlateRestingMarker.position) > 0.1f) {
			Plate.position = Vector3.MoveTowards(Plate.position, PlateRestingMarker.position, PlateMoveSpeed * Time.deltaTime);
			yield return null;
		}

		// Animate door closing
		for(float t = DoorOpenAngle; t > 0; t -= DoorOpenSpeed * Time.deltaTime) {
			Door.transform.eulerAngles = new Vector3(t, 0, 0);
			yield return null;
		}

		playerController.State = PlayerController.PlayerState.FreeMoving;

		canEat = true;
	}

	private void Refill() {
		float refillAmount = MaxFoodLevel - FoodLevel;
		float inputFood = shipResources.ChangeFood(-refillAmount);
		foodLevel -= inputFood;
		foodLevel = Mathf.Clamp(FoodLevel, 0, FoodLevel);
	}

	void OnBroken(ShipSystem system) {
		BrokenModel.SetActive(true);
		FixedModel.SetActive(false);
	}

	void OnFixed(ShipSystem system) {
		BrokenModel.SetActive(false);
		FixedModel.SetActive(true);
	}

}
