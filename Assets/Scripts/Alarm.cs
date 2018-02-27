using UnityEngine;
using System.Collections;

public class Alarm : MonoBehaviour {

	public bool Activated { get; protected set; }
	public float flashSpeed = 10;

	private bool forward = true;

	private Renderer rend;

	void Start() {
		rend = GetComponent<Renderer>();
		Activated = false;
	}

	void Update() {
		if(Activated) {
			float emission = Mathf.PingPong(Time.time, 1);
			Color baseColour = Color.red;
			Color finalColour = baseColour * Mathf.LinearToGammaSpace(emission);
			rend.material.SetColor("_EmissionColor", finalColour);
		}
	}

	public void Activate() {
		Activated = true;
	}

	public void Deactivate() {
		Activated = false;
	}

}
