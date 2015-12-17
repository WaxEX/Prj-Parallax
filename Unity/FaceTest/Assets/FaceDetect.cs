//unity側
using UnityEngine;
using System.Collections;
using System;
//openCV.openCVSharp
using OpenCvSharp.CPlusPlus;
using OpenCvSharp;


public class FaceDetect : MonoBehaviour
{

	//スクリーンサイズ
	public int Width = 640;
	public int Height = 480;
	//カメラからの情報を2Dデータとして保管する変数
	private Texture2D texture;
	//顏を追っかけるやつ
	public GameObject obj;
	
	//_Cameraは、2次元から3次元に座標変換するときに必要
	private Camera _Camera;

	//openCVの機能：
	//ビデオファイルやカメラからキャプチャを行う
	VideoCapture video;
	public int VideoIndex = 0;
	//顏検出のためのクラス
	CascadeClassifier cascade;

	// Use this for initialization
	void Start()
	{
		// ビデオの設定
		//カメラからのストリーミングを開始
		video = new VideoCapture(VideoIndex);
		//ビデオストリーム中のフレームの幅と高さ
		video.Set(CaptureProperty.FrameWidth, Width);
		video.Set(CaptureProperty.FrameHeight, Height);
		//print(string.Format("{0},{1}", Width, Height));

		// 顔検出器の作成 (.xmlファイルで、他にも顏のパーツや動物の検出器もあるみたい
		cascade = new CascadeClassifier(Application.dataPath + @"/haarcascade_frontalface_alt.xml");
		
		// テクスチャの作成(何処のマテリアルに画像を出力するか
		texture = new Texture2D(Width, Height, TextureFormat.RGB24, false);
		GameObject.Find("Plane").GetComponent<Renderer>().material.mainTexture = texture;
		
		// 変換用のカメラの作成
		_Camera = GameObject.Find("Main Camera").GetComponent<Camera>();
		//print(string.Format("({0},{1})({2},{3})", Screen.width, Screen.height, _Camera.pixelWidth, _Camera.pixelHeight));

		// 顏の上にオブジェクト…
		obj = GameObject.Find("moveObj");
	}
	
	// Update is called once per frame
	void Update()
	{
		//usingでインスタンス化されたオブジェクトはusing{}を抜けると解放される
		using (Mat image = new Mat())
		{
			// ストリーミングしている画像を読み込む
			video.Read(image);
			
			// 顔を検出する
			var faces = cascade.DetectMultiScale(image);
			//　検出出来たら処理
			if (faces.Length > 0)
			{
				var face = faces[0];

                // 顔の矩形を描画する

               image.Rectangle(face, new Scalar(255, 0, 0), 2);

				// 中心の座標を計算する
				var x = face.TopLeft.X + (face.Size.Width / 2);
				var y = face.TopLeft.Y + (face.Size.Height / 2);
				//print(string.Format("({0},{1})",x,y));

				// オブジェクトを移動する
				if (obj != null)
				{
					obj.transform.localPosition = Vector2ToVector3(new Vector2(x, y));
				}
			}
			
			// OpenCVのデータがBGRなのでRGBに変える
			// Bitmap形式に変えてテクスチャに流し込む
			using (var cvtImage = image.CvtColor(ColorConversion.BgrToRgb))
			{
				texture.LoadRawTextureData(cvtImage.ImEncode(".bmp"));
				//描画：変更点のあったピクセルの更新
				texture.Apply();
			}
		}
	}
	
	void OnApplicationQuit()
	{
		if (video != null)
		{
			video.Dispose();
			video = null;
		}
	}
	
	/// <summary>
	/// OpenCVの2次元座標をUnityの3次元座標に変換する
	/// </summary>
	/// <param name="vector2"></param>
	/// <returns></returns>
	private Vector3 Vector2ToVector3(Vector2 vector2)
	{
		//変換用のカメラがない時は例外エラーを出す
		if (_Camera == null)
		{
			throw new Exception("");
		}

		// スクリーンサイズで調整(WebCamera->Unity)
		vector2.x = vector2.x * Screen.width / Width;
		vector2.y = vector2.y * Screen.height / Height;
		
		// Unityのワールド座標系(3次元)に変換
		var vector3 = _Camera.ScreenToWorldPoint(vector2);

		
		// 座標の調整
		// Y座標は逆、Z座標は0にする
		vector3.y *= -1;
		vector3.z = 0;
		
		return vector3;
	}
}