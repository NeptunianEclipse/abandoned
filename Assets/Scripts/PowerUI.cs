using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PowerUI : MonoBehaviour {

	public Text UsageText;
	public Text ProductionText;
	public Text TotalText;

	public Text ProducingListText;
	public Text ProducingListPowerText;

	public Text UsingListText;
	public Text UsingListPowerText;

	public Text EnergyStoredText;
	public Text EmptyTimeText;

	public Text EmptyText;
	public Text FullText;

	private ShipResourceManager shipResources;
	private ShipSystemManager systemManager;

	void Start() {
		shipResources = ShipResourceManager.Instance;
		systemManager = ShipSystemManager.Instance;
	}

	void Update() {
		UsageText.text = Mathf.Round(systemManager.LastPowerUsage).ToString();
		ProductionText.text = Mathf.Round(systemManager.LastPowerProduction).ToString();
		TotalText.text = Mathf.Round(systemManager.LastTotalPower).ToString();

		EnergyStoredText.text = Mathf.Round(shipResources.StoredEnergy).ToString();

		if(systemManager.LastTotalPower < 0) {
			EmptyTimeText.text = Mathf.Round(((shipResources.StoredEnergy / Mathf.Abs(systemManager.LastTotalPower)) / 60) * 10) / 10 + " hours";
			EmptyText.enabled = true;
			FullText.enabled = false;
		} else {
			EmptyTimeText.text = Mathf.Round(((shipResources.MaxEnergy / Mathf.Abs(systemManager.LastTotalPower)) / 60) * 10) / 10 + " hours";
			EmptyText.enabled = false;
			FullText.enabled = true;
		}

		SortedDictionary<string, float> productionList = systemManager.LastPowerProductions;
		string producingListText = "";
		string producingListPowerText = "";
		for(int i = 0; i < productionList.Count; i++) {
			producingListText += productionList.Keys.ElementAt(i) + "\n";
			producingListPowerText += productionList.Values.ElementAt(i) + "\n";
		}
		ProducingListText.text = producingListText;
		ProducingListPowerText.text = producingListPowerText;

		SortedDictionary<string, float> usageList = systemManager.LastPowerUsages;
		string usageListText = "";
		string usageListPowerText = "";
		for(int i = 0; i < usageList.Count; i++) {
			usageListText += usageList.Keys.ElementAt(i) + "\n";
			usageListPowerText += usageList.Values.ElementAt(i) + "\n";
		}
		UsingListText.text = usageListText;
		UsingListPowerText.text = usageListPowerText;
	}

}
