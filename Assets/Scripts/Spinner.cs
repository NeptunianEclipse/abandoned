using UnityEngine;
using System.Collections;

public class Spinner : MonoBehaviour {

	public bool Spin = true;
	public Vector3 SpinDelta;

	void Update() {
		if(Spin) {
			transform.Rotate(SpinDelta);
		}
	}
}
