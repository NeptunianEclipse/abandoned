using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class ShipSystem : MonoBehaviour {

	public delegate void SystemEventHandler(ShipSystem system);
	public event SystemEventHandler SystemBreakDown;
	public event SystemEventHandler SystemFixed;
	public event SystemEventHandler SystemPowerUnavailable;
	public event SystemEventHandler SystemPowerAvailable;

	public enum SystemPowerType {
		Consumer,
		Producer
	}

	public string Name;
	public SystemPowerType PowerType;
	public float BaseBreakChance;
	public float MinBreakHealth;
	public float MaxBreakHealth;
	public float MaxHealth;
	public float BasePower;
	public int PowerPriority;

	public virtual bool Active {
		get {
			return TurnedOn && PowerAvailable && !Broken;
		}
	}

	public bool Interactable { get; protected set; }

	private float health;
	public float Health {
		get {
			return health;
		}
		protected set {
			health = Mathf.Clamp(value, 0, MaxHealth);
			if(!Broken && health < MaxHealth) {
				Broken = true;
				if(SystemBreakDown != null) {
					SystemBreakDown(this);
				}
				UIManager.Instance.PostEvent(this.Name + " broke down");
			} else if(Broken && health == MaxHealth) {
				Broken = false;
				UIManager.Instance.PostEvent(this.Name + " was fixed");
				if(SystemFixed != null) {
					SystemFixed(this);
				}
			}
		}
	}

	public bool Broken { get; protected set; }
	public float CurrentBreakChance { get; protected set; }
	public bool TurnedOn = true; // FIXME: PROBABLY SHOULDN'T BE EXTERNALLY SETTABLE
	public bool PowerAvailable { get; protected set; }

	protected virtual void Start() {
		Health = MaxHealth;
		PowerAvailable = true;
		Broken = false;
		Interactable = true;
	}

	protected virtual void Update() {
		if(Active) {
			if(Random.Range(0.0f, 1.0f) <= BaseBreakChance * TimeManager.Instance.GameDeltaTime) {
				Break();
			}	
		}
	}

	public virtual void Break() {
		Health = Random.Range(MinBreakHealth, MaxBreakHealth);
	}

	public virtual void Repair(float amount) {
		Health += amount;
	}

	public virtual void ToggleOnOff() {
		if(TurnedOn) {
			TurnOff();
		} else {
			TurnOn();
		}
	}

	public virtual void TurnOn() {
		TurnedOn = true;
	}

	public virtual void TurnOff() {
		TurnedOn = false;
	}

	public virtual void SetPowerUnavailable() {
		PowerAvailable = false;
		if(SystemPowerUnavailable != null) {
			SystemPowerUnavailable(this);
		}
	}

	public virtual void SetPowerAvailable() {
		PowerAvailable = true;
		if(SystemPowerAvailable != null) {
			SystemPowerAvailable(this);
		}
	}

	public virtual float CurrentPower() {
		if(Active) {
			return BasePower;	
		} else {
			return 0;
		}
	}

}
