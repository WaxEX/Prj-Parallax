using UnityEngine;
using System.Collections;

public class SceneController : MonoBehaviour {

	public GameObject Enemy;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if ((int)Time.time % 5 == 0) {
			Instantiate(this.Enemy, new Vector3(Random.Range(-10,10), Random.Range(-10,10), Random.Range(-10,10)), Quaternion.identity);
		}
	}
}
