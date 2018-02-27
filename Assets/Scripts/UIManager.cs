using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour {

	public static UIManager Instance;

	public Text TooltipText;
	public Image LeftClickIcon;

	public Text TooltipTextSub;

	public Text HealthText;
	public Text HungerText;
	public Text ThirstText;
	public Text EnergyText;

	public Text TimeText;

	public Text FoodText;
	public Text WaterText;
	public Text StoredOxygenText;
	public Text OxygenLevelText;
	public Text TemperatureText;
	public Text StoredEnergyText;
	public Text PowerUsageText;
	public Text PowerProductionText;
	public Text NetPowerText;

	public Image DarkOverlay;
	public Image RepairBar;
	public Image RepairBarContainer;

	public GameObject GameWonScreen;
	public GameObject GameLostScreen;

	public GameObject Controls;

	public GameObject EventBox;
	public Text EventText;

	public Text OyxgenLevelTextOnMachine;

	public float EventDuration = 5;

	private bool darkOverlayEnabled = false;
	public bool DarkOverlayEnabled {
		get {
			return darkOverlayEnabled;
		}
		set {
			darkOverlayEnabled = value;
			DarkOverlay.gameObject.SetActive(darkOverlayEnabled);
		}
	}

	private Transform cameraTransform;
	private PlayerResourceManager playerResources;
	private ShipResourceManager shipResources;
	private ShipSystemManager systemManager;

	private string currentEventText;
	private float eventTimer;

	public void ShowControls() {
		Cursor.lockState = CursorLockMode.None;
		Controls.SetActive(true);
	}

	public void CloseControls() {
		Cursor.lockState = CursorLockMode.Locked;
		Controls.SetActive(false);
	}

	void Awake() {
		Instance = this;
	}

	void Start() {
		darkOverlayEnabled = false;



		cameraTransform = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Camera>().transform;
		playerResources = PlayerResourceManager.Instance;
		shipResources = ShipResourceManager.Instance;
		systemManager = ShipSystemManager.Instance;

		EventBox.SetActive(false);
	}

	void FixedUpdate() {
		// Update player stats
		HealthText.text = "Health: " + playerResources.HealthState + " (" + playerResources.Health + ")";
		HungerText.text = "Hunger: " + playerResources.HungerState + " (" + playerResources.Hunger + ")";
		ThirstText.text = "Thirst: " + playerResources.ThirstState + " (" + playerResources.Thirst + ")";
		EnergyText.text = "Energy: " + playerResources.EnergyState + " (" + playerResources.Energy + ")";

		// Update time
		TimeText.text = TimeManager.Instance.CurrentDayTimeFormatted;

		// Update ship stats
		FoodText.text = "Food: " + shipResources.StoredFood;
		WaterText.text = "Water: " + shipResources.StoredWater;
		StoredOxygenText.text = "Stored oxygen: " + shipResources.StoredOxygen;
		OxygenLevelText.text = "Oxygen level: " + shipResources.OxygenLevel + "%";
		OyxgenLevelTextOnMachine.text = "Oxygen in air:\n" + shipResources.OxygenLevel + "%";
		TemperatureText.text = "Temperature: " + shipResources.Temperature;
		StoredEnergyText.text = "Stored energy: " + shipResources.StoredEnergy;
		PowerUsageText.text = "Power usage: " + systemManager.GetPowerUsage();
		PowerProductionText.text = "Power production: " + systemManager.GetPowerProduction();
		NetPowerText.text = "Net power: " + (systemManager.GetPowerProduction() - systemManager.GetPowerUsage());

		if(eventTimer > 0) {
			eventTimer -= Time.deltaTime;
		}
		if(eventTimer <= 0) {
			EventBox.SetActive(false);
		}
	}

	public void UpdateTooltipText(string text, bool leftClickIcon) {
		if(text != null) {
			TooltipText.text = text;
			TooltipText.enabled = true;
			LeftClickIcon.enabled = leftClickIcon;
		} else {
			TooltipText.enabled = false;
			LeftClickIcon.enabled = false;
		}
	}

	public void UpdateTooltipTextSub(string text) {
		if(text != null) {
			TooltipTextSub.text = text;
			TooltipTextSub.enabled = true;
		} else {
			TooltipTextSub.enabled = false;
		}
	}

	public void PostEvent(string name) {
		currentEventText = name;
		EventText.text = currentEventText;
		eventTimer = EventDuration;
		EventBox.SetActive(true);
	}

	public void ShowRepairBar() {
		RepairBarContainer.gameObject.SetActive(true);
	}

	public void HideRepairBar() {
		RepairBarContainer.gameObject.SetActive(false);
	}

	public void UpdateRepairBar(float percent) {
		RepairBar.rectTransform.localScale = new Vector3(percent, 1, 1);
	}

	public void ShowGameWonScreen() {
		Cursor.lockState = CursorLockMode.None;
		GameWonScreen.SetActive(true);
	}

	public void ShowGameLostScreen() {
		Cursor.lockState = CursorLockMode.None;
		GameLostScreen.SetActive(true);
	}

}
