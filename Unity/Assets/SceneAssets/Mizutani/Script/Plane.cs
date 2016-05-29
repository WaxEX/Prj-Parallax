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

		myTex = new Texture2D(dm.width, dm.height, TextureFormat.RGB24, false);
		GetComponent<Renderer>().material.mainTexture = myTex;
	}
	
	// Update is called once per frame
	void Update () {
		myTex = dm.cameraImage;

		//myTex.LoadRawTextureData (dm.image);
		myTex.Apply();
	}
		
	void OnApplicationQuit()
	{
		if (dm != null){
			dm.Dispose();
			dm = null;
		}
	}
}
