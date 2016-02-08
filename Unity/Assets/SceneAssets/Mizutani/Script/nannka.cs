using UnityEngine;
using System.Collections;

public class nannka : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(0, 0, Random.Range(-1000, 1000));
	
	}
}
