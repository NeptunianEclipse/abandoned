using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LifeMonitorUI : MonoBehaviour {

	public static LifeMonitorUI Instance;

	public Text HealthText;
	public Text HungerText;
	public Text ThirstText;
	public Text EnergyText;
	public Text RadiationText;

	private PlayerResourceManager playerResources;

	void Awake() {
		Instance = this;
	}

	void Start() {
		playerResources = PlayerResourceManager.Instance;
	}

	void Update() {
		HealthText.text = playerResources.HealthState;
		HungerText.text = playerResources.HungerState;
		ThirstText.text = playerResources.ThirstState;
		EnergyText.text = playerResources.EnergyState;
		RadiationText.text = playerResources.RadiationState;
	}

}
