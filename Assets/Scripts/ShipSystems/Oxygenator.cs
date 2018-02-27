using UnityEngine;
using System.Collections;

public class Oxygenator : ShipSystem {

	public float TargetOxygenLevel = 22;
	public float MaxOxygenRate = 0.1f;
	public float ExtraPowerDrawPerOxygenRate = 20;

	public float MaxOxygenProductionRate = 0.01f;
	public float ExtraPowerDrawPerOxygenProductionRate = 400;

	public GameObject FixedModel;
	public GameObject BrokenModel;

	public Transform Oxygen;

	private float currentOxygenRate = 0;
	private float currentOxygenProductionRate = 0;

	private ShipResourceManager shipResources;

	protected override void Start() {
		base.Start();
		shipResources = ShipResourceManager.Instance;
		SystemFixed += OnFixed;
		SystemBreakDown += OnBroken;
	}

	protected override void Update() {
		base.Update();

		if(shipResources.OxygenLevel < TargetOxygenLevel) {
			currentOxygenRate = MaxOxygenRate;
			if(shipResources.ChangeOxygen(-currentOxygenRate * TimeManager.Instance.GameDeltaTime)) {
				shipResources.ChangeOxygenLevel(currentOxygenRate * TimeManager.Instance.GameDeltaTime);
			} else {
				currentOxygenRate = 0;
			}
		}

		if(Active) {
			if(shipResources.StoredOxygen < shipResources.MaxOxygen) {
				currentOxygenProductionRate = Mathf.Min((shipResources.MaxOxygen - shipResources.StoredOxygen) * TimeManager.Instance.GameDeltaTime, MaxOxygenProductionRate);
				if(currentOxygenProductionRate == MaxOxygenProductionRate) {
					shipResources.ChangeOxygen(currentOxygenProductionRate * TimeManager.Instance.GameDeltaTime);
				} else {
					shipResources.ChangeOxygen(currentOxygenProductionRate);
				}
			}
		}

		Oxygen.localScale = new Vector3(Oxygen.localScale.x, shipResources.StoredOxygen / shipResources.MaxOxygen, Oxygen.localScale.z);
		Oxygen.localPosition = new Vector3(Oxygen.localPosition.x,  (-0.95f + Oxygen.localScale.y) / 2, Oxygen.localPosition.z);
	}

	public override float CurrentPower() {
		if(Active) {
			return BasePower + currentOxygenRate * ExtraPowerDrawPerOxygenRate + currentOxygenProductionRate * ExtraPowerDrawPerOxygenProductionRate;	
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
