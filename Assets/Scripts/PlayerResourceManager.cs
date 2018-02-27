using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerResourceManager : MonoBehaviour {

	public static PlayerResourceManager Instance { get; private set; }

	public float InitialHealth = 100;
	public float InitialHunger = 100;
	public float InitialThirst = 100;
	public float InitialRadiation = 0;
	public float InitialEnergy = 100;

	public float HealthCap = 100;
	public float HungerCap = 100;
	public float ThirstCap = 100;
	public float RadiationCap = 100;
	public float EnergyCap = 100;

	public float BaseHungerRate = -0.005f;
	public float BaseThirstRate = -0.05f;
	public float BaseRadiationRate = -0.01f;
	public float BaseEnergyRate = -0.025f;

	public float SleepingEnergyRate = 0.1f;

	// The hunger at which the player will start to take damage
	public float HungerDamageThreshold = 40;
	public float MaxHungerDamageRate = -1;
	public float MaxHungerDamageThreshold = 20;

	// The thirst at which the player will start to take damage
	public float ThirstDamageThreshold = 40;
	public float MaxThirstDamageRate = -1;
	public float MaxThirstDamageThreshold = 20;

	// The radiation at which the player will start to take damage
	public float RadiationDamageThreshold = 30;
	public float MaxRadiationDamageRate = -5;
	public float MaxRadiationDamageThreshold = 100;

	// The energy at which the player will start to take damage
	public float EnergyDamageThreshold = 20;
	public float MaxEnergyDamageRate = -1;
	public float MaxEnergyDamageThreshold = 0;

	// The oxygen level at which the player will start to take damage
	public float LowOxygenDamageThreshold = 18;
	public float MaxLowOxygenDamageRate = -10;
	public float MaxLowOxygenDamageThreshold = 5;

	public float Health { get; private set; }
	public float Hunger { get; private set; }
	public float Thirst { get; private set; }
	public float Radiation { get; private set; }
	public float Energy { get; private set; }

	// The string health states for each health value
	private SortedDictionary<float, string> healthStates = new SortedDictionary<float, string> {
		{80, "Good"},
		{60, "Okay"},
		{40, "Poor"},
		{20, "Bad"},
		{0, "Dying"}
	};

	// The string hunger states for each hunger value
	private SortedDictionary<float, string> hungerStates = new SortedDictionary<float, string> {
		{80, "Full"},
		{60, "Slightly hungry"},
		{40, "Hungry"},
		{20, "Starving"},
		{0, "Empty"}
	};

	// The string thirst states for each thirst value
	private SortedDictionary<float, string> thirstStates = new SortedDictionary<float, string> {
		{80, "Hydrated"},
		{60, "Parched"},
		{40, "Thirsty"},
		{20, "Dehydrated"},
		{0, "Dried out"}
	};

	// The string energy states for each energy value
	private SortedDictionary<float, string> energyStates = new SortedDictionary<float, string> {
		{80, "Wide awake"},
		{60, "Tired"},
		{40, "Exhausted"},
		{20, "Shattered"},
		{0, "Barely awake"}
	};

	// The string energy states for each radiation value
	private SortedDictionary<float, string> radiationStates = new SortedDictionary<float, string> {
		{80, "Deadly"},
		{60, "Exteme"},
		{40, "High"},
		{20, "Medium"},
		{0, "Low"}
	};

	public string HealthState {
		get {
			for(var i = 0; i < healthStates.Count; i++) {
				if(healthStates.Keys.ElementAt(i) > Health) {
					return healthStates.Values.ElementAt(i - 1);
				}
			}
			return healthStates.Values.ElementAt(healthStates.Count - 1);
		}
	}

	public string HungerState {
		get {
			for(var i = 0; i < hungerStates.Count; i++) {
				if(hungerStates.Keys.ElementAt(i) > Hunger) {
					return hungerStates.Values.ElementAt(i - 1);
				}
			}
			return hungerStates.Values.ElementAt(hungerStates.Count - 1);
		}
	}

	public string ThirstState {
		get {
			for(var i = 0; i < thirstStates.Count; i++) {
				if(thirstStates.Keys.ElementAt(i) > Thirst) {
					return thirstStates.Values.ElementAt(i - 1);
				}
			}
			return thirstStates.Values.ElementAt(thirstStates.Count - 1);
		}
	}

	public string EnergyState {
		get {
			for(var i = 0; i < energyStates.Count; i++) {
				if(energyStates.Keys.ElementAt(i) > Energy) {
					return energyStates.Values.ElementAt(i - 1);
				}
			}
			return energyStates.Values.ElementAt(energyStates.Count - 1);
		}
	}

	public string RadiationState {
		get {
			for(var i = 0; i < radiationStates.Count; i++) {
				if(radiationStates.Keys.ElementAt(i) > Radiation) {
					return radiationStates.Values.ElementAt(i - 1);
				}
			}
			return radiationStates.Values.ElementAt(radiationStates.Count + 1);
		}
	}

	private TimeManager timeManager;
	private ShipResourceManager shipResources;
	private PlayerController playerController;
	private GameController gameController;

	void Awake() {
		Instance = this;
	}

	void Start() {
		timeManager = TimeManager.Instance;
		shipResources = ShipResourceManager.Instance;
		playerController = PlayerController.Instance;
		gameController = GameController.Instance;

		Health = InitialHealth;
		Hunger = InitialHunger;
		Thirst = InitialThirst;
		Radiation = InitialRadiation;
		Energy = InitialEnergy;
	}

	void Update() {
		// Take damage from low player stats
		if(Hunger <= HungerDamageThreshold) {
			float diff = Mathf.Max(Hunger - MaxHungerDamageThreshold, 0);
			float damageRate = Mathf.Lerp(0, MaxHungerDamageRate, 1 / diff);
			Health += damageRate * timeManager.GameDeltaTime;
		}

		if(Thirst <= ThirstDamageThreshold) {
			float diff = Mathf.Max(Thirst - MaxThirstDamageThreshold, 0);
			float damageRate = Mathf.Lerp(0, MaxThirstDamageRate, 1 / diff);
			Health += damageRate * timeManager.GameDeltaTime;
		}

		if(Radiation >= RadiationDamageThreshold) {
			float diff = Mathf.Max(MaxRadiationDamageThreshold - Radiation, 0);
			float damageRate = Mathf.Lerp(0, MaxRadiationDamageRate, 1 / diff);
			Health += damageRate * timeManager.GameDeltaTime;
		}
			
		if(Energy <= EnergyDamageThreshold) {
			float diff = Mathf.Max(MaxEnergyDamageThreshold - Energy, 0);
			float damageRate = Mathf.Lerp(0, MaxEnergyDamageRate, 1 / diff);
			Health += damageRate * timeManager.GameDeltaTime;
		}

		Hunger += BaseHungerRate * timeManager.GameDeltaTime;
		Thirst += BaseThirstRate * timeManager.GameDeltaTime;
		Radiation += BaseRadiationRate * timeManager.GameDeltaTime;

		if(playerController.State == PlayerController.PlayerState.Sleeping) {
			Energy += SleepingEnergyRate * timeManager.GameDeltaTime;
		} else {
			Energy += BaseEnergyRate * timeManager.GameDeltaTime;
		}

		// Take damage from environmental factors
		if(shipResources.OxygenLevel <= LowOxygenDamageThreshold) {
			float diff = Mathf.Max(MaxLowOxygenDamageThreshold - shipResources.OxygenLevel, 0);
			float damageRate = Mathf.Lerp(0, MaxLowOxygenDamageRate, 1 / diff);
			Health += damageRate * timeManager.GameDeltaTime;
		}

		Health = Mathf.Clamp(Health, 0, HealthCap);
		Hunger = Mathf.Clamp(Hunger, 0, HungerCap);
		Thirst = Mathf.Clamp(Thirst, 0, ThirstCap);
		Radiation = Mathf.Clamp(Radiation, 0, RadiationCap);
		Energy = Mathf.Clamp(Energy, 0, EnergyCap);

		if(Health <= 0) {
			gameController.GameLost();
		}

	}

	public float ChangeHunger(float amount) {
		float newVal = Hunger + amount;

		if(newVal < 0) {
			Hunger = 0;
			return newVal - amount;
		} else if(newVal > HungerCap) {
			Hunger = HungerCap;
			return HungerCap - newVal + amount;
		} else {
			Hunger = newVal;
			return amount;
		}
	}

	public float ChangeThirst(float amount) {
		float newVal = Thirst + amount;

		if(newVal < 0) {
			Thirst = 0;
			return newVal - amount;
		} else if(newVal > ThirstCap) {
			Thirst = ThirstCap;
			return ThirstCap - newVal + amount;
		} else {
			Thirst = newVal;
			return amount;
		}
	}

	public float ChangeRadiation(float amount) {
		float newVal = Radiation + amount;

		if(newVal < 0) {
			Radiation = 0;
			return newVal - amount;
		} else if(newVal > RadiationCap) {
			Radiation = RadiationCap;
			return RadiationCap - newVal + amount;
		} else {
			Radiation = newVal;
			return amount;
		}
	}

}