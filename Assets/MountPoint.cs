using UnityEngine;

public class MountPoint : MonoBehaviour {

	[SerializeField]
	private GameObject target;
	void Start () {
		target.transform.parent = transform;
	}
}
