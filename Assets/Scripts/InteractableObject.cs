using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class InteractableObject : MonoBehaviour {

	public string DisplayText;
	public bool ShowMouseIcon = true;

	[System.Serializable]
	public class Vector3Event : UnityEvent<Vector3> {}
	public Vector3Event OnClick;

	public virtual void Click(Vector3 pos) {
		OnClick.Invoke(pos);
	}

}
