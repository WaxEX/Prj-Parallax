using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class CvRapper_iOS {

	[DllImport("__Internal")]
	private static extern bool _awakeCamera (string str, int index);
	[DllImport("__Internal")]
	private static extern void _releaseCamera();
	[DllImport("__Internal")]
	private static extern void _update();
	[DllImport("__Internal")]
	private static extern void _setTextureInfo(bool isDraw, int width, int height);
	[DllImport ("__Internal")]
	private static extern void _getTexture(IntPtr data);
	[DllImport("__Internal")]
	private static extern int getX_();
	[DllImport("__Internal")]
	private static extern int getY_();
	[DllImport("__Internal")]
	private static extern int getZ_();

	private static bool _isOpen = false;
	public static bool isOpen {get{return _isOpen;}}

	public static void init(string str, int index){
		_isOpen = _awakeCamera(str, index);
	}
	public static void delete(){
		_releaseCamera();
		_isOpen = false;
	}

	public static void update() {
		_update();
	}


	public static Vector3 getPos(){
		return new Vector3 (getX_(), getY_(), getZ_());

	}

	public static void setTextureInfo(bool isDraw, int width, int height) {
		_setTextureInfo(isDraw, width, height);
	}

	public static void getTexture(IntPtr texPtr) {
		_getTexture(texPtr);
	}

}
