using UnityEngine;
using System.Collections;

public class SimpleMove : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float z = Input.GetAxis("Vertical");
		transform.Translate(0, 0, z);
	}
}
