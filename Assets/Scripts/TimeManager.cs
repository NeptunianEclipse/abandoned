using UnityEngine;
using System.Collections;

public class TimeManager : MonoBehaviour {

	public static TimeManager Instance { get; private set; }

	// The number of game minutes that pass every real life second
	public float GameMinutesPerSecond = 1;

	// The number of game minutes that pass every real life second when time is accelerated
	public float FastGameMinutesPerSecond = 10;

	// The current game time in minutes
	public float CurrentTime { get; private set; }
	public bool Paused = false;
	public bool Fast = false;

	[Range(0.0f, 200.0f)]
	public float TimeScale = 1f;

	public float StartingTime;

	public Transform Sun;

	// The current day number
	public int CurrentDay {
		get {
			return Mathf.FloorToInt(CurrentTime / 1440) + 1;
		}
	}

	// The current time of day in minutes (the number of minutes since the current day began)
	public float CurrentDayTime {
		get {
			return CurrentTime % 1440;
		}
	}

	// The current time and day formatted as a string
	public string CurrentDayTimeFormatted {
		get {
			return "Day " + CurrentDay + ", " + Mathf.FloorToInt(CurrentDayTime / 60).ToString("00") + ":" + Mathf.FloorToInt(CurrentDayTime % 60).ToString("00");
		}
	}

	public float GameDeltaTime { get; private set; }

	public void Accelerate() {
		Fast = true;
	}

	public void Decelerate() {
		Fast = false;
	}

	void Awake() {
		Instance = this;
	}

	void Start() {
		CurrentTime = StartingTime;
	}

	void Update() {
		CurrentTime += GameDeltaTime;
		Sun.eulerAngles = new Vector3(0, 90 + (CurrentTime / 1440) * 360, 0);
	}

	void LateUpdate() {
		if(!Paused) {
			if(Fast) {
				GameDeltaTime = Time.deltaTime * FastGameMinutesPerSecond * TimeScale;
			} else {
				GameDeltaTime = Time.deltaTime * GameMinutesPerSecond * TimeScale;
			}
		} else {
			GameDeltaTime = 0;
		}
	}

}
