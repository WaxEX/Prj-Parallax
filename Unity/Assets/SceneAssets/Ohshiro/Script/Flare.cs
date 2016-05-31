using UnityEngine;
using System.Collections;

public class Flare : MonoBehaviour {

	// Use this for initialization
	void Start () {
        // エフェクトをコルーチンで消す
        StartCoroutine("AutoEraseEffect");
	}
	
	// Update is called once per frame
	void Update () {
	}

    IEnumerator AutoEraseEffect()
    {
        yield return new WaitForSeconds(3.0f);
        Destroy(gameObject);
    }
}
