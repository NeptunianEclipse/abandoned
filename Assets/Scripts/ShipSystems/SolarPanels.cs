using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SolarPanels : ShipSystem {

	public float LightStartTime = 350;
	public float LightEndTime = 750;

	public Text CurrentPowerText;
	public Text MaxPowerText;
	public Text HoursUntilText;

	public Text HoursUntilSunlight;
	public Text SunlightHoursLeft;

	public bool lightAvailable { get; protected set; }

	protected override void Update() {
		base.Update();

		float currentTime = TimeManager.Instance.CurrentDayTime;
		lightAvailable = currentTime >= LightStartTime && currentTime <= LightEndTime;

		CurrentPowerText.text = Mathf.Round(CurrentPower()).ToString();
		MaxPowerText.text = Mathf.Round(BasePower).ToString();
		if(lightAvailable) {
			HoursUntilSunlight.enabled = false;
			SunlightHoursLeft.enabled = true;

			HoursUntilText.text = Mathf.Round(((LightEndTime - TimeManager.Instance.CurrentDayTime) / 60) * 10) / 10 + " hours";
		} else {
			HoursUntilSunlight.enabled = true;
			SunlightHoursLeft.enabled = false;

			if(TimeManager.Instance.CurrentDayTime > LightEndTime) {
				HoursUntilText.text = Mathf.Round(((LightStartTime + 1440 - TimeManager.Instance.CurrentDayTime) / 60) * 10) / 10 + " hours";
			} else {
				HoursUntilText.text = Mathf.Round(((LightStartTime - TimeManager.Instance.CurrentDayTime) / 60) * 10) / 10 + " hours";
			}
		}
	}

	public override float CurrentPower() {
		if(lightAvailable) {
			return base.CurrentPower();
		} else {
			return 0;
		}
	}

}
