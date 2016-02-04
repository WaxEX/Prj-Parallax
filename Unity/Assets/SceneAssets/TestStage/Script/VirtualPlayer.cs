using UnityEngine;
using System.Collections;

public class VirtualPlayer : MonoBehaviour {
	public GameObject Prefab;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButton ("Jump")) {

			// TODO ちゃんとカメラの法線方向に打つようにする
			GameObject Shot = (GameObject)Instantiate(this.Prefab, this.transform.position, Quaternion.identity);

			Rigidbody rigid = Shot.GetComponent<Rigidbody> ();
			rigid.AddForce (transform.forward * 100.0f);
		}
	}
}
