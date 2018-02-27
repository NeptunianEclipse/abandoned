using UnityEngine;
using System.Collections;

public class ShipLights : ShipSystem {

	public static ShipLights Instance;

	public delegate void LightsEventHandler();
	public event LightsEventHandler LightsTurnedOn;
	public event LightsEventHandler LightsTurnedOff;

	private bool lightsSwitchedOn = true;
	private bool lightsAreOn;

	void Awake() {
		Instance = this;
	}

	protected override void Start() {
		base.Start();

		StartCoroutine(LateStart());
	}

	protected override void Update() {
		base.Update();

		if(PowerAvailable) {
			if(lightsSwitchedOn && !lightsAreOn) {
				TurnOn();
			}
		} else {
			if(lightsAreOn) {
				TurnOff();
			}
		}
	}

	IEnumerator LateStart() {
		yield return new WaitForSeconds(0.01f);
		TurnOn();
	}

	public void ToggleLights() {
		if(lightsSwitchedOn) {
			SwitchLightsOff();
		} else {
			SwitchLightsOn();
		}
	}

	public void SwitchLightsOn() {
		lightsSwitchedOn = true;
		if(Active) {
			TurnOn();
		}
	}

	public void SwitchLightsOff() {
		lightsSwitchedOn = false;
		TurnOff();
	}

	void TurnOn() {
		lightsAreOn = true;
		if(LightsTurnedOn != null) {
			LightsTurnedOn();
		}
	}

	void TurnOff() {
		lightsAreOn = false;
		if(LightsTurnedOff != null) {
			LightsTurnedOff();
		}
	}

}
