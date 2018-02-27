using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonControl : InteractableObject {

	public Text OnText;
	public Text OffText;

	public bool DefaultState = true;
	public string OnOffSubject;

	private bool state;
	public bool State {
		get {
			return state;
		}
		set {
			state = value;
			if(state) {
				OnText.enabled = true;
				OffText.enabled = false;
				if(OnOffSubject != null) {
					DisplayText = "Turn " + OnOffSubject + " off";
				}
			} else {
				OnText.enabled = false;
				OffText.enabled = true;
				if(OnOffSubject != null) {
					DisplayText = "Turn " + OnOffSubject + " on";
				}
			}
		}
	}

	void Start() {
		State = DefaultState;
		if(OnOffSubject != null) {
			DisplayText = "Turn " + OnOffSubject + (State ? " off" : " on");
		}
	}

	public override void Click(Vector3 pos) {
		base.Click (pos);

		State = !State;
	}

}
