using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ShipSystemManager : MonoBehaviour {

	public static ShipSystemManager Instance;

	public float minOperationalEnergyStorage = 200;

	public int numSystems { get; private set; }
	public int numConsumerSystems { get; private set; }
	public int numProducerSystems { get; private set; }

	public float LastPowerUsage { get; private set; }
	public float LastPowerProduction { get; private set; }
	public float LastTotalPower { get; private set; }

	public SortedDictionary<string, float> LastPowerUsages { get; private set; }
	public SortedDictionary<string, float> LastPowerProductions { get; private set; }

	private ShipSystem[] systems;

	private ShipResourceManager shipResources;

	// List of systems that are currently disabled because of a lack of power
	private List<ShipSystem> unavailablePowerSystems;
	private List<ShipSystem> availablePowerSystems;

	void Awake() {
		Instance = this;
	}

	void Start () {
		systems = GetComponents<ShipSystem>();
		numSystems = systems.Length;

		availablePowerSystems = systems.Where(s => s.PowerType == ShipSystem.SystemPowerType.Consumer).ToList();
		numConsumerSystems = availablePowerSystems.Count();
		numProducerSystems = numSystems - numConsumerSystems;

		shipResources = ShipResourceManager.Instance;
		unavailablePowerSystems = new List<ShipSystem>();
	}
	
	void LateUpdate () {
		LastPowerUsage = GetPowerUsage();
		LastPowerProduction = GetPowerProduction();
		LastTotalPower = LastPowerProduction - LastPowerUsage;

		if(shipResources.ChangeEnergy(LastTotalPower * TimeManager.Instance.GameDeltaTime) != (LastTotalPower * TimeManager.Instance.GameDeltaTime) && shipResources.StoredEnergy == 0) {
			StartDisablingSystems();
		} else if(unavailablePowerSystems.Count > 0) {
			StartEnablingSystems();
		}
	}

	public float GetPowerUsage() {
		LastPowerUsages = new SortedDictionary<string, float>();

		float total = 0;
		foreach(ShipSystem system in systems) {
			if(system.PowerType == ShipSystem.SystemPowerType.Consumer) {
				total += system.CurrentPower();
				if(system.CurrentPower() > 0) {
					LastPowerUsages[system.Name] = system.CurrentPower();
				}
			}
		}
		return total;
	}

	public float GetPowerProduction() {
		LastPowerProductions = new SortedDictionary<string, float>();

		float total = 0;
		foreach(ShipSystem system in systems) {
			if(system.PowerType == ShipSystem.SystemPowerType.Producer) {
				total += system.CurrentPower();
				if(system.CurrentPower() > 0) {
					LastPowerProductions[system.Name] = system.CurrentPower();
				}
 			}
		}
		return total;
	}

	private void StartDisablingSystems() {
		while(GetPowerProduction() - GetPowerUsage() < 0 && unavailablePowerSystems.Count < numConsumerSystems) {
			ShipSystem system = GetSystemWithLowestPriority(availablePowerSystems.ToArray());
			if(system != null) {
				system.SetPowerUnavailable();
				unavailablePowerSystems.Add(system);
				availablePowerSystems.Remove(system);
			}
		}
	}

	private ShipSystem GetSystemWithLowestPriority(ShipSystem[] systemList) {
		int lowestPriority = int.MaxValue;
		ShipSystem lowestPrioritySystem = null;
		foreach(ShipSystem system in systemList) {
			if(system.PowerType == ShipSystem.SystemPowerType.Consumer && system.PowerPriority < lowestPriority) {
				lowestPriority = system.PowerPriority;
				lowestPrioritySystem = system;
			}
		}
		return lowestPrioritySystem;
	}

	private void StartEnablingSystems() {
		while(shipResources.StoredEnergy >= minOperationalEnergyStorage && unavailablePowerSystems.Count > 0) {
			ShipSystem system = GetSystemWithHighestPriority(unavailablePowerSystems.ToArray());
			if(system != null) {
				system.SetPowerAvailable();
				unavailablePowerSystems.Remove(system);
				availablePowerSystems.Add(system);
			}
		}
	}

	private ShipSystem GetSystemWithHighestPriority(ShipSystem[] systemList) {
		int highestPriority = int.MinValue;
		ShipSystem highestPrioritySystem = null;
		foreach(ShipSystem system in systemList) {
			if(system.PowerType == ShipSystem.SystemPowerType.Consumer && system.PowerPriority > highestPriority) {
				highestPriority = system.PowerPriority;
				highestPrioritySystem = system;
			}
		}
		return highestPrioritySystem;
	}

}
