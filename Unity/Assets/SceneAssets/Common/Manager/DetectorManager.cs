using System;
using System.Threading;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using UnityEngine;

namespace Manager
{

	public sealed class DetectorManager : IDisposable
	{
		// for DEBUG カメラ映像を取りたい
		private byte[] _image = new byte[WIDTH*HEIGHT*3];
		public  byte[] image{get{return _image;}}
		public int width {get{return WIDTH;}}
		public int height{get{return HEIGHT;}}

		//スクリーンサイズ
		private const int WIDTH  = 640;
		private const int HEIGHT = 480;

		// 定義ファイルの場所
		private const string CASCADE_FILEPATH = @"/Resources/Others/haarcascade_frontalface_alt.xml";

		// シングルトン実装
		private static DetectorManager instance = new DetectorManager();
		public  static DetectorManager Instance{get { return instance; }}

		// スレッド管理
		private Thread thread = null;
		private bool isRun = false;

		// ビデオファイルやカメラからキャプチャを行う
		const int VIDEO_INDEX = 0;
		VideoCapture video;	

		//顏検出クラス
		CascadeClassifier cascade;

		// 外部公開用のメンバ変数
		private int _status = 0;
		public Vector3 _facePos;
		public Vector3 facePos {get{return _facePos;}}


		private DetectorManager() {
			// 初期座標をセット
			this._facePos    = new Vector3(0,0,0);

			//カメラからのストリーミングを開始
			video = new VideoCapture(VIDEO_INDEX);
			video.Set(CaptureProperty.FrameWidth, WIDTH);
			video.Set(CaptureProperty.FrameHeight, HEIGHT);

			// 顔検出器の作成
			cascade = new CascadeClassifier( Application.dataPath + CASCADE_FILEPATH);
			
			// スレッド開始
			this.thread = new Thread(new ThreadStart(updatePosition));
			this.thread.Start();
			this.isRun = true;
		}
			
		// 別スレッドで動かす処理実態（顔座標を取る
		private void updatePosition(){
			while (this.isRun) {

				// カメラ見つかんない
				if (!video.IsOpened ()) {this._status = 2; continue;}

				using (Mat image = new Mat()) {
					// ストリーミングしている画像を読み込む
					video.Read (image);

					// 顔を検出する
					var faces = cascade.DetectMultiScale (image);

				
					if (faces.Length <= 0) {
						// 検出出来なかった
						this._status = 1;
					} else {
						this._status = 0;

						var face = faces[0];

						// 中心の座標を計算する(一旦Zは考慮しない)
						var x = face.TopLeft.X + (face.Size.Width / 2) - WIDTH / 2;
						var y = face.TopLeft.Y + (face.Size.Height / 2) - HEIGHT / 2;

						this._status = 0;
						this._facePos = new Vector3 (-x, -y, this._facePos.z);    // 座標系変換のため、-が付いてる

						// if DEBUG 顔の矩形を描画する
						image.Rectangle(face, new Scalar(255, 0, 0), 2);
					}

					// if DEBUG
					using (var cvtImage = image.CvtColor (ColorConversion.BgrToRgb)) {
						this._image = cvtImage.ImEncode (".bmp");
					}
				}
			}
		}
			
		public void Dispose(){
			Debug.Log("[Disposed] Thread is stop.");
			if (this.isRun) {
				// スレッド止める
				this.isRun = false;
				this.thread.Join ();
			}
		}
	}	


}