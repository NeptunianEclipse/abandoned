using UnityEngine;
using System.Collections;

public class ShipLight : MonoBehaviour {

	private ShipLights shipLights;
	private Light light;

	void Start() {
		shipLights = ShipLights.Instance;
		light = GetComponent<Light>();

		shipLights.LightsTurnedOn += TurnOn;
		shipLights.LightsTurnedOff += TurnOff;
	}

	public void TurnOn() {
		light.enabled = true;
	}

	public void TurnOff() {
		light.enabled = false;
	}

}
