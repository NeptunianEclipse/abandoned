using UnityEngine;
using System.Collections;

public class WaterPlant : ShipSystem {

	public float MaxWaterProductionRate = 0.01f;
	public float ExtraPowerDrawPerWaterProductionRate = 400;

	public GameObject FixedModel;
	public GameObject BrokenModel;

	public Transform Water;
	public Transform Tank;

	private float currentWaterProductionRate = 0;

	private ShipResourceManager shipResources;

	protected override void Start() {
		base.Start();
		shipResources = ShipResourceManager.Instance;
		SystemFixed += OnFixed;
		SystemBreakDown += OnBroken;
		FixedModel.SetActive(true);
		BrokenModel.SetActive(false);
	}

	protected override void Update() {
		base.Update();

		if(Active) {
			if(shipResources.StoredWater < shipResources.MaxWater) {
				currentWaterProductionRate = Mathf.Min((shipResources.MaxWater - shipResources.StoredWater) * TimeManager.Instance.GameDeltaTime, MaxWaterProductionRate);
				if(currentWaterProductionRate == MaxWaterProductionRate) {
					shipResources.ChangeWater(currentWaterProductionRate * TimeManager.Instance.GameDeltaTime);
				} else {
					shipResources.ChangeWater(currentWaterProductionRate);
				}
			}
		}

		Water.localScale = new Vector3(Water.localScale.x, shipResources.StoredWater / shipResources.MaxWater, Water.localScale.z);
		Water.localPosition = new Vector3(Water.localPosition.x,  (-0.95f + Water.localScale.y) / 2, Water.localPosition.z);
	}

	public override float CurrentPower() {
		if(Active) {
			return BasePower + currentWaterProductionRate * ExtraPowerDrawPerWaterProductionRate;
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
}
