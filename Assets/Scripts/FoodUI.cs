using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FoodUI : MonoBehaviour {

	public Text FoodText;

	private ShipResourceManager shipResources;

	void Start () {
		shipResources = ShipResourceManager.Instance;
	}
	
	void Update () {
		FoodText.text = "Food: " + shipResources.StoredFood + " / " + shipResources.MaxFood;
	}
}
