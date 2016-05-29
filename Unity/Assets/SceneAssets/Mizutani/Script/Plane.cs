using UnityEngine;
using System.Collections;
using System;

using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

using Manager;

public class Plane : MonoBehaviour {

	private Texture2D myTex; 

	private DetectorManager dm = null;


	void Start () {
		dm = DetectorManager.Instance;

		//myTex = GetComponent<Renderer>().material.mainTexture;


		myTex = new Texture2D(dm.width, dm.height, TextureFormat.RGB24, false);
		GetComponent<Renderer>().material.mainTexture = myTex;

	}
	
	// Update is called once per frame
	void Update () {



		dm.drawTexture(ref myTex);

		myTex.Apply();





		//var myTex = dm.cameraImage;

		//myTex.LoadRawTextureData (dm.image);
		//myTex.Apply();

		//GetComponent<Renderer> ().material.mainTexture = myTex;
	}

	void OnGUI(){
		Vector3 pos = dm.facePos;
		GUI.Label(new UnityEngine.Rect(20, 20, 100, 100), pos.ToString());
	}
		
	void OnApplicationQuit()
	{
		if (dm != null){
			dm.Dispose();
			dm = null;
		}
	}
}
