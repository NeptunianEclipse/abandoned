using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Beacon : ShipSystem {

	public static Beacon Instance;

	public int PartsRequired = 5;

	public float MinimumTimeToRescue = 150;
	public float MaximumTimeToRescue = 150;

	public float BroadcastRate = 1;

	public float PowerDrawWhenBroadcasting = 20;

	public Transform BeaconContainer;
	public SystemInteractableObject InteractObject;

	public GameObject UnfinishedModel;
	public Text PartsText;
	public GameObject FinishedModel;

	public delegate void BeaconEventHandler();
	public event BeaconEventHandler OnBeaconFixed;
	public event BeaconEventHandler OnBeaconBroadcast;
	public event BeaconEventHandler OnBeaconBroadcastStop;

	public int PartsCollected { get; protected set; }
	public bool AllPartsCollected {
		get {
			return PartsCollected == PartsRequired;
		}
	}

	public override bool Active {
		get {
			if(AllPartsCollected) {
				return base.Active;
			} else {
				return false;
			}
		}
	}

	public float BroadcastProgress { get; protected set; }
	public  bool Broadcasting { get; protected set; }

	private RobotManager robotManager;

	private float timeToRescue;

	private bool wasBroadcasting = false;

	protected void Awake() {
		Instance = this;
	}

	protected override void Start() {
		base.Start();

		robotManager = RobotManager.Instance;
		robotManager.OnRobotPartReturn += CollectPart;
		SystemFixed += BeaconFixed;

		Interactable = false;
		PartsCollected = 0;
		BroadcastProgress = 0;
		Broadcasting = false;

		UnfinishedModel.SetActive(true);
		FinishedModel.SetActive(false);
		PartsText.text = "Beacon\nparts:\n" + PartsCollected + " / " + PartsRequired;

		InteractObject.DisplayText = PartsCollected + "/" + PartsRequired + " beacon parts collected";
		InteractObject.ShowMouseIcon = false;
	}

	protected override void Update() {
		base.Update();

		if(Broadcasting) {
			BroadcastProgress += TimeManager.Instance.GameDeltaTime;

			if(BroadcastProgress > timeToRescue) {
				Rescue();
			}
		}
	}

	public void CollectPart() {
		PartsCollected++;

		if(AllPartsCollected) {
			Health = 0;
			Interactable = true;

			InteractObject.DisplayText = "Broadcast distress signal";
			InteractObject.ShowMouseIcon = true;

			UnfinishedModel.SetActive(false);
			FinishedModel.SetActive(true);
		} else {
			InteractObject.DisplayText = PartsCollected + "/" + PartsRequired + " beacon parts collected";
			InteractObject.ShowMouseIcon = false;
			PartsText.text = "Beacon\nparts:\n" + PartsCollected + " / " + PartsRequired;
		}
	}

	public override float CurrentPower() {
		if(Active && Broadcasting) {
			return PowerDrawWhenBroadcasting + BasePower;
		}
		return base.CurrentPower();
	}

	public void ToggleBroadcasting() {
		if(AllPartsCollected) {
			if(Broadcasting) {
				StopBroadcasting();
			} else {
				StartBroadcasting();
			}
		}
	}

	protected void BeaconFixed(ShipSystem beacon) {
		if(OnBeaconFixed != null) {
			OnBeaconFixed();
		}
	}

	protected void StartBroadcasting() {
		Broadcasting = true;
		if(!wasBroadcasting) {
			timeToRescue = Random.Range(MinimumTimeToRescue, MaximumTimeToRescue);
		}
		wasBroadcasting = true;

		InteractObject.DisplayText = "Stop broadcasting";
		InteractObject.ShowMouseIcon = true;

		if(OnBeaconBroadcast != null) {
			OnBeaconBroadcast();
		}
	}

	protected void StopBroadcasting() {
		Broadcasting = false;
		InteractObject.DisplayText = "Broadcast distress signal";
		BroadcastProgress = 0;
		InteractObject.ShowMouseIcon = true;

		if(OnBeaconBroadcastStop != null) {
			OnBeaconBroadcastStop();
		}
	}

	protected void Rescue() {
		GameController.Instance.GameWon();
	}

}
