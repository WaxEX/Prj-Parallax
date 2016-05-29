using UnityEngine;
using System.Collections;
using System.Threading;

using Manager;



public class araburuTester : MonoBehaviour {


	private DetectorManager dm = null;


	void Start () {
		dm = DetectorManager.Instance;
	}
	
	// Update is called once per frame
	void Update () {
		
		transform.position = dm.facePos * 0.01f;

		//Debug.Log(dm.facePos);
	}


	void OnApplicationQuit()
	{
		if (dm != null){
			dm.Dispose();
			dm = null;
		}
	}
}
