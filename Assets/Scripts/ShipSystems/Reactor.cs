using UnityEngine;
using System.Collections;

public class Reactor : ShipSystem {

	public float ActiveRadiationRate = 0.01f;

	private PlayerResourceManager playerResources;

	protected override void Start() {
		base.Start();

		playerResources = PlayerResourceManager.Instance;
	}

	protected override void Update() {
		base.Update();
		if(Active) {
			playerResources.ChangeRadiation(ActiveRadiationRate * TimeManager.Instance.GameDeltaTime);
		}
	}

}
