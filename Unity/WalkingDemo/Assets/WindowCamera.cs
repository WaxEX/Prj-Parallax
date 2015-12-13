using UnityEngine;
using System.Collections;
//openCV.openCVSharp
//using OpenCvSharp.CPlusPlus;
using OpenCvSharp;



// カメラにアタッチして使用
[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class WindowCamera : MonoBehaviour {
	
	[SerializeField, TooltipAttribute("仮想窓の高さ（横幅はaspectに従う）")]
	private float WINDOW_HEIGHT = 20.0F;

	[SerializeField, TooltipAttribute("仮想窓のデフォルト位置 >0")]
	public Vector3 WINDOW_INIT_POS = new Vector3(0, 0, 50.0F);

	[SerializeField, TooltipAttribute("長さの単位比率（外部入力との単位の差分を吸収）")]
	public float UNIT_RATIO = 1.0F;

	// 自身のカメラコンポーネントの参照を保持しておく為のモノ
	private Camera thisCamera;

	// 仮想窓　（座標は"カメラを原点とする"ローカル座標系）
	private VirtualWindow window = new VirtualWindow();

	// 頭の座標（"頭を原点として"画面の中心へのベクトル）
	private Vector3 headPos;

    //@ビデオファイルやカメラからキャプチャを行う
    OpenCvSharp.CPlusPlus.VideoCapture video;
    public int VideoIndex = 0;
    //顏検出のためのクラス
    OpenCvSharp.CPlusPlus.CascadeClassifier cascade;
    GameObject faceObj;


    void Start () {
		// 自分自身のカメラコンポーネントを入れておく
		thisCamera = GetComponent<Camera>();

		headPos = WINDOW_INIT_POS/UNIT_RATIO;


		window.Itit(WINDOW_HEIGHT, WINDOW_HEIGHT*thisCamera.aspect, headPos);

        //@カメラからのストリーミングを開始
        video = new OpenCvSharp.CPlusPlus.VideoCapture(VideoIndex);
        //@ビデオストリーム中のフレームの幅と高さ
        video.Set(CaptureProperty.FrameWidth, WINDOW_HEIGHT * thisCamera.aspect);
        video.Set(CaptureProperty.FrameHeight, WINDOW_HEIGHT);
        //@顔検出器の作成 (.xmlファイルで、他にも顏のパーツや動物の検出器もあるみたい
        cascade = new OpenCvSharp.CPlusPlus.CascadeClassifier(Application.dataPath + @"/haarcascade_frontalface_alt.xml");
        faceObj = GameObject.Find("Face_obj");


    }


    void Update () {

		window.width = WINDOW_HEIGHT*thisCamera.aspect;


		// ToDO顔の座標を取るように改良
		Vector3 newPos = getHeadPosition() * UNIT_RATIO;
		Vector3 diff = newPos - headPos;
		headPos = newPos;

		// カメラが移動
		transform.Translate(diff);

		// 仮想窓は動かない→カメラからは逆向きに移動して見える
		window.Translate(-1*diff);


		//仮想窓での描画範囲を計算
		Matrix4x4 m = ProjectionToVirtialWindow(window, thisCamera.nearClipPlane, thisCamera.farClipPlane);
		thisCamera.projectionMatrix = m;
	}
	

	// 仮想窓(window)とクリップ面の情報(near, far)から、仮想窓から映る範囲を対象にした射影変換行列を計算する
	private Matrix4x4 ProjectionToVirtialWindow(VirtualWindow window, float near, float far){
		// 「(カメラの)ローカル座標」と「カメラ座標」ではz軸が±逆になるのでwindow.zに-が付いている
		return ProjectionToOffCenter(window.left, window.right, window.bottom, window.top, -window.z, near, far);
	}

	// カメラ座標にて、Zの位置にある矩形(Left, Right, Bottom, Top)を範囲とする、クリップ領域(Near, Far)を対象とした射影変換行列を計算する
	private Matrix4x4 ProjectionToOffCenter(float L, float R, float B, float T, float Z, float N, float F) {
		Matrix4x4 m = new Matrix4x4();
		m[0, 0] =-2.0F*Z/(R-L);	m[0, 1] = 0;			m[0, 2] = (R+L)/(R-L);	m[0, 3] = 0;
		m[1, 0] = 0;			m[1, 1] =-2.0F*Z/(T-B);	m[1, 2] = (T+B)/(T-B);	m[1, 3] = 0;
		m[2, 0] = 0;			m[2, 1] = 0;			m[2, 2] =-(F+N)/(F-N);	m[2, 3] = -(2.0F*F*N)/(F-N);
		m[3, 0] = 0;			m[3, 1] = 0;			m[3, 2] =-1.0F;			m[3, 3] = 0;
		return m;
	}

	// カメラの描画範囲となる仮想窓
	private class VirtualWindow{
		private Rect  _rect = new Rect();
		private float _z = 0;

		//座標は読取専用
		public float left   { get {return this._rect.xMin;} }
		public float right  { get {return this._rect.xMax;} }
		public float bottom { get {return this._rect.yMin;} }
		public float top    { get {return this._rect.yMax;} }
		public float z      { get {return this._z;} }

		// 設定用（widthは後で変わる可能性があるので、外から更新できる様にpublic）
		public  float   width  { set {this._rect.width  = value;} }
		private float   height { set {this._rect.height = value;} }
		private Vector3 center { set {this._rect.center = value; this._z = value.z;} }

		// 初期設定
		public void Itit(float height, float width, Vector3 center){
			this.height = height;
			this.width  = width;
			this.center = center;	// Rect.centerのセッティングは最後にしないとRectの座標がズレる様子
		}

		// diffだけ移動させる
		public void Translate(Vector3 diff){
			this._rect.center += (Vector2)diff;
			this._z += diff.z;
		}
	}


	// ===================================
	// 中身は仮。外部からの入力を持ってくる予定
	// ===================================
	private Vector3 getHeadPosition(){
        /*
		float x = 2.0F * Input.GetAxis("Mouse X");
		float y = 2.0F * Input.GetAxis("Mouse Y");
		float z = 20.0F * Input.GetAxis("Mouse ScrollWheel");
		
		return headPos + new Vector3 (x, y, z);
        */

        using (OpenCvSharp.CPlusPlus.Mat image = new OpenCvSharp.CPlusPlus.Mat())
        {
            // ストリーミングしている画像を読み込む
            video.Read(image);

            // 顔を検出する
            var faces = cascade.DetectMultiScale(image);
            //　検出出来たら処理
            if (faces.Length > 0)
            {
                var face = faces[0];
                // 中心の座標を計算する
                var x = face.TopLeft.X + (face.Size.Width / 2);
                var y = face.TopLeft.Y + (face.Size.Height / 2);

                faceObj.transform.localPosition = Vector2ToVector3(new Vector2(x,y));
                float tes = faceObj.transform.position.x;
                float tes2 = faceObj.transform.position.y;
                print(string.Format("({0},{1})",tes,tes2));

                return new Vector3(tes,tes2,0.0F);

            }
        }

       return new Vector3();
//       return headPos + face_pos;
	}



    private Vector3 Vector2ToVector3(Vector2 vector2)
    {
        //変換用のカメラがない時は例外エラーを出す
        if (thisCamera == null)
        {
            throw new System.Exception("");
        }

        // スクリーンサイズで調整(WebCamera->Unity)
        vector2.x = vector2.x * Screen.width / WINDOW_HEIGHT * thisCamera.aspect;
        vector2.y = vector2.y * Screen.height / WINDOW_HEIGHT;


        var vector3 = thisCamera.ScreenToWorldPoint(vector2);
        //var vector3 = new Vector3(vector2.x,vector2.y,0.0F);
       // print(string.Format("{0}", vector3));


        return vector3;
 //       return new Vector3();
    }




#if UNITY_EDITOR
        // 開発用　シーンビューで視野を描画する
        void OnDrawGizmos(){
		// 仮想窓の4隅
		Vector3 to1 = new Vector3(window.left, window.bottom, window.z);
		Vector3 to2 = new Vector3(window.left, window.top, window.z);
		Vector3 to3 = new Vector3(window.right, window.top, window.z);
		Vector3 to4 = new Vector3(window.right, window.bottom, window.z);
		
		// ワールド座標系へ
		Vector3 to1_w = transform.TransformPoint(to1);
		Vector3 to2_w = transform.TransformPoint(to2);
		Vector3 to3_w = transform.TransformPoint(to3);
		Vector3 to4_w = transform.TransformPoint(to4);
		
		// 仮想窓を描画
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(to1_w, to2_w);
		Gizmos.DrawLine(to2_w, to3_w);
		Gizmos.DrawLine(to3_w, to4_w);
		Gizmos.DrawLine(to4_w, to1_w);
		
		// 視野錐を描画
		Gizmos.color /=1.5F;
		Gizmos.DrawRay(transform.position, transform.TransformDirection(to1)*5);
		Gizmos.DrawRay(transform.position, transform.TransformDirection(to2)*5);
		Gizmos.DrawRay(transform.position, transform.TransformDirection(to3)*5);
		Gizmos.DrawRay(transform.position, transform.TransformDirection(to4)*5);
	}
#endif

}