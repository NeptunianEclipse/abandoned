using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipResourceManager : MonoBehaviour {

	public static ShipResourceManager Instance;

	public float StoredFood { get; private set; }
	public float StoredWater { get; private set; }
	public float StoredEnergy { get; private set; }
	public float StoredOxygen { get; private set; }
	public float OxygenLevel { get; private set; }
	public float Temperature {get; private set; }

	public float InitialFood = 2000;
	public float InitialWater = 500;
	public float InitialEnergy = 1000;
	public float InitialOxygen = 1000;
	public float InitialOxygenLevel = 20;
	public float InitialTemperature = 20;

	public float MaxFood = 3000;
	public float MaxWater = 1000;
	public float MaxEnergy = 1000;
	public float MaxOxygen = 1000;
	public float MaxOxygenLevel = 100;

	public float LowOxygenPercent = 0.1f;
	public float LowOxygenLevel = 20;
	public float LowFoodPercent = 0.1f;
	public float LowWaterPercent = 0.1f;
	public float LowEnergyPercent = 0.1f;

	public float BaseOxygenLevelRate = -0.01f;

	public Alarm OxygenAlarm;
	public Alarm AirOxygenAlarm;
	public Alarm FoodAlarm;
	public Alarm WaterAlarm;
	public Alarm PowerAlarm;

	// Changes the stored food by the given value, and returns the change that was actually able to be applied
	public float ChangeFood(float amount) {
		float newVal = StoredFood + amount;

		if(newVal < 0) {
			StoredFood = 0;
			return newVal - amount;
		} else if(newVal > MaxFood) {
			StoredFood = MaxFood;
			return MaxFood - newVal + amount;
		} else {
			StoredFood = newVal;
			return amount;
		}
	}

	// Changes the stored water by the given value, and returns the change that was actually able to be applied
	public float ChangeWater(float amount) {
		float newVal = StoredWater + amount;

		if(newVal < 0) {
			StoredWater = 0;
			return newVal - amount;
		} else if(newVal > MaxWater) {
			StoredWater = MaxWater;
			return MaxWater - newVal + amount;
		} else {
			StoredWater = newVal;
			return amount;
		}
	}

	// Changes the stored energy by the given value, and returns the change that was actually able to be applied
	public float ChangeEnergy(float amount) {
		float newVal = StoredEnergy + amount;

		if(newVal < 0) {
			StoredEnergy = 0;
			return newVal - amount;
		} else if(newVal > MaxEnergy) {
			StoredEnergy = MaxEnergy;
			return MaxEnergy - newVal + amount;
		} else {
			StoredEnergy = newVal;
			return amount;
		}

	}

	public bool ChangeOxygen(float amount) {
		float newVal = StoredOxygen + amount;
		if(newVal > 0 && newVal <= MaxOxygen) {
			StoredOxygen += amount;
			return true;
		}
		return false;
	}

	public bool ChangeOxygenLevel(float amount) {
		float newVal = OxygenLevel + amount;
		if(newVal > 0 && newVal <= MaxOxygenLevel) {
			OxygenLevel += amount;
			return true;
		}
		return false;
	}

	void Awake() {
		Instance = this;
	}

	void Update() {
		ChangeOxygenLevel(BaseOxygenLevelRate * TimeManager.Instance.GameDeltaTime);

		if(StoredOxygen < MaxOxygen * LowOxygenPercent && !OxygenAlarm.Activated) {
			UIManager.Instance.PostEvent("The oxygen tank is low");
			Debug.Log(OxygenAlarm.Activated);
			OxygenAlarm.Activate();
		} 
		if(!(StoredOxygen < MaxOxygen * LowOxygenPercent)){
			OxygenAlarm.Deactivate();
		}

		if(OxygenLevel < LowOxygenLevel && !AirOxygenAlarm.Activated) {
			UIManager.Instance.PostEvent("The air oxygen level is low! You're suffocating!");
			AirOxygenAlarm.Activate();
		}
		if(!(OxygenLevel < LowOxygenLevel)){
			AirOxygenAlarm.Deactivate();
		}

		if(StoredFood < MaxFood * LowFoodPercent && !FoodAlarm.Activated) {
			UIManager.Instance.PostEvent("The food stores are low");
			FoodAlarm.Activate();
		}
		if(!(StoredFood < MaxFood * LowFoodPercent)){
			FoodAlarm.Deactivate();
		}

		if(StoredWater < MaxWater * LowWaterPercent && !WaterAlarm.Activated) {
			UIManager.Instance.PostEvent("The water tank is low!");
			WaterAlarm.Activate();
		}
		if(!(StoredWater < MaxWater * LowWaterPercent)){
			WaterAlarm.Deactivate();
		}

		if(StoredEnergy < MaxEnergy * LowEnergyPercent && !PowerAlarm.Activated) {
			UIManager.Instance.PostEvent("The stored energy is low!");
			PowerAlarm.Activate();
		}
		if(!(StoredEnergy < MaxEnergy * LowEnergyPercent)){
			PowerAlarm.Deactivate();
		}
	}

	void Start() {
		StoredFood = InitialFood;
		StoredWater = InitialWater;
		StoredEnergy = InitialEnergy;
		StoredOxygen = InitialOxygen;
		OxygenLevel = InitialOxygenLevel;
		Temperature = InitialTemperature;
	}

}
