using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour {

	public static GameController Instance;

	private UIManager uiManager;

	void Awake() {
		Instance = this;
	}

	void Start() {
		uiManager = UIManager.Instance;
		uiManager.ShowControls();
	}

	void Update() {
		if(Input.GetMouseButtonDown(0)) {
			GetComponent<AudioSource>().Play();
		}

		if(Input.GetKeyDown(KeyCode.Escape)) {
			GameLost();
		}
	}

	public void ReloadNewGame() {
		SceneManager.LoadScene(0);
	}

	public void Exit() {
		Application.Quit();
	}

	public void GameWon() {
		TimeManager.Instance.Paused = true;
		PlayerController.Instance.State = PlayerController.PlayerState.AnimationLocked;
		uiManager.ShowGameWonScreen();
	}

	public void GameLost() {
		TimeManager.Instance.Paused = true;
		PlayerController.Instance.State = PlayerController.PlayerState.AnimationLocked;
		uiManager.ShowGameLostScreen();
	}

}
