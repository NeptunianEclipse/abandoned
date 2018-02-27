using UnityEngine;
using System.Collections;

public class WaterDispenser : ShipSystem {

	public float MaxWaterLevel = 20;
	public float RefillLevel = 5;

	public float CupAnimationFillTime = 0.5f;
	public float CupAnimationDrinkMinDistance = 0.1f;
	public float CupAnimationCupSpeed = 10;
	public float CupAnimationDrinkTime = 2;

	public Transform WaterCube;
	public Transform CupRestingPositionMarker;
	public Transform CupDrinkingPositionMarker;
	public Transform Cup;
	public GameObject WaterStream;
	public Transform CupWater;

	public GameObject FixedModel;
	public GameObject BrokenModel;

	private float waterLevel;
	public float WaterLevel {
		get {
			return waterLevel;
		}
		protected set {
			waterLevel = value;
			if(WaterLevel < RefillLevel) {
				Refill();
			}
			UpdateGraphics();
		} 
	}
	
	private ShipResourceManager shipResources;
	private PlayerController playerController;
	private UIManager uiManager;

	private bool canDrink = true;

	protected override void Start() {
		base.Start();

		SystemFixed += OnFixed;
		SystemBreakDown += OnBroken;

		shipResources = ShipResourceManager.Instance;
		playerController = PlayerController.Instance;
		uiManager = UIManager.Instance;
		WaterLevel = MaxWaterLevel;

		FixedModel.SetActive(true);
		BrokenModel.SetActive(false);
		WaterStream.SetActive(false);
		CupWater.transform.localScale = Vector3.zero;
	}

	protected override void Update() {
		base.Update();

		if(WaterLevel < RefillLevel) {
			Refill();
		}
	}

	// Reduces the dispensers water level by the given amount, and returns the amount that could actually be taken (i.e. if there is less than amount
	// left in the dispenser then it will return a value lower than amount representing the amount of water that was left)
	public float DispenseWater(float amount) {
		if(canDrink && WaterLevel > 0) {
			StartCoroutine(AnimateCup());

			float afterLevel = WaterLevel - amount;
			if(afterLevel < 0) {
				WaterLevel = 0;
				return afterLevel + amount;
			} else {
				WaterLevel = afterLevel;
				return amount;	
			}
		} else {
			return 0;
		}
	}

	void OnBroken(ShipSystem system) {
		BrokenModel.SetActive(true);
		FixedModel.SetActive(false);
	}

	void OnFixed(ShipSystem system) {
		BrokenModel.SetActive(false);
		FixedModel.SetActive(true);
	}

	private void Refill() {
		float refillAmount = MaxWaterLevel - WaterLevel;
		float inputWater = shipResources.ChangeWater(-refillAmount);
		waterLevel -= inputWater;
		waterLevel = Mathf.Clamp(WaterLevel, 0, MaxWaterLevel);
	}

	private IEnumerator AnimateCup() {
		canDrink = false;

		playerController.State = PlayerController.PlayerState.AnimationLocked;
		uiManager.UpdateTooltipText(null, false);
		Vector3 restPos = CupRestingPositionMarker.position;

		// Animate cup filling
		CupWater.localScale = Vector3.zero;
		WaterStream.SetActive(true);
		for(float t = 0; t < CupAnimationFillTime; t += Time.deltaTime) {
			CupWater.localScale = new Vector3(1, t / CupAnimationFillTime, 1);
			yield return null;
		}
		WaterStream.SetActive(false);

		// Animate cup moving towards player
		while(Vector3.Distance(Cup.position, CupDrinkingPositionMarker.position) > CupAnimationDrinkMinDistance) {
			Cup.position = Vector3.MoveTowards(Cup.position, CupDrinkingPositionMarker.position, CupAnimationCupSpeed * Time.deltaTime);
			yield return null;
		}

		// Animate cup drinking
		for(float t = 0; t < CupAnimationDrinkTime; t += Time.deltaTime) {
			CupWater.localScale = new Vector3(1, 1 - (t / CupAnimationDrinkTime), 1);
			yield return null;
		}


		// Animate cup moving back to rest position
		while(Vector3.Distance(Cup.position, CupRestingPositionMarker.position) > CupAnimationDrinkMinDistance) {
			Cup.position = Vector3.MoveTowards(Cup.position, CupRestingPositionMarker.position, CupAnimationCupSpeed * Time.deltaTime);
			yield return null;
		}
		Cup.position = CupRestingPositionMarker.position;
		playerController.State = PlayerController.PlayerState.FreeMoving;

		canDrink = true;
	}

	private void UpdateGraphics() {
		WaterCube.localScale = new Vector3(WaterCube.localScale.x, WaterLevel / MaxWaterLevel, WaterCube.localScale.z);
		WaterCube.localPosition = new Vector3(WaterCube.localPosition.x, (-0.95f + WaterCube.localScale.y) / 2, WaterCube.localPosition.z);
	}

}
