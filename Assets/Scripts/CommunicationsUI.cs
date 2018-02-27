using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class CommunicationsUI : MonoBehaviour {

	public static CommunicationsUI Instance;

	public Text OfflineText;
	public Text OnlineText;

	public GameObject MainComms;
	public GameObject BackupComms;

	public GameObject CommsArea;
	public Text CommsText;

	private SortedDictionary<float, string> rescueCommunications = new SortedDictionary<float, string> {
		{20, "Signal found..."},
		{25, "[Rescue] Distress signal detected up in sector A4-FF-3B"},
		{30, "[Rescue] Honing in on signal, estimated time: 2 hours"},
		{100, "[Rescue] Signal isolated to sub-sector BA-28-81 of sector A4-FF-3B"},
		{105, "[Rescue] Honing in on signal, estimated time: 30 minutes"},
		{130, "[Rescue] Signal location identified to within 100 m"},
		{132, "[Rescue] Dispatching rescue vessel"}
	};
	private string lostSignalString = "Signal lost";
	private bool signalLost = false;

	private Beacon beacon;

	void Awake() {
		Instance = this;
	}

	void Start() {
		beacon = Beacon.Instance;
		beacon.OnBeaconFixed += BeaconFixed;
		beacon.OnBeaconBroadcast += BroadcastStart;
		beacon.OnBeaconBroadcastStop += BroadcastStop;

		OfflineText.enabled = true;
		OnlineText.enabled = false;
		CommsArea.SetActive(false);
	}

	void Update() {
		if(!beacon.Broken) {
			string commString = GetCommsString(beacon.BroadcastProgress);
			if(signalLost) {
				commString += lostSignalString;
			}
			CommsText.text = commString;
		}
	}

	protected string GetCommsString(float broadcastProgress) {
		string commString = "";
		for(int i = 0; i < rescueCommunications.Count; i++) {
			if(broadcastProgress > rescueCommunications.Keys.ElementAt(i)) {
				commString += rescueCommunications.Values.ElementAt(i) + "\n";
			}
		}
		return commString;
	}

	protected void BeaconFixed() {
		OfflineText.enabled = false;
		OnlineText.enabled = true;
	}

	protected void BroadcastStart() {
		MainComms.SetActive(false);
		BackupComms.SetActive(false);
		CommsArea.SetActive(true);
		signalLost = false;
	}

	protected void BroadcastStop() {
		signalLost = true;
	}

}
