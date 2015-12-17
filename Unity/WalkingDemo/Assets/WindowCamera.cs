﻿using UnityEngine;
using System;

using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

// カメラにアタッチして使用
[RequireComponent(typeof(Camera))]
//[ExecuteInEditMode]
public class WindowCamera : MonoBehaviour
{
    [SerializeField, TooltipAttribute("仮想窓の高さ（横幅はaspectに従う）")]
    private float WINDOW_HEIGHT = 20.0F;

    [SerializeField, TooltipAttribute("仮想窓のデフォルト位置 >0")]
    public Vector3 WINDOW_INIT_POS = new Vector3(0, 0, 50.0F);

    [SerializeField, TooltipAttribute("長さの単位比率（外部入力との単位の差分を吸収）")]
    public float UNIT_RATIO = 0.1f;

    // 自身のカメラコンポーネントの参照を保持しておく為のモノ
    private Camera thisCamera;

    // 仮想窓　（座標は"カメラを原点とする"ローカル座標系）
    private VirtualWindow window = new VirtualWindow();

    // 頭の座標（"頭を原点として"画面の中心へのベクトル）
    private Vector3 headPos;

    // 顔検出器
    private CameraDetector cameraDetector = null;

    private float lastUpdateTime = -1000.0f;

    void Start()
    {
        // 自分自身のカメラコンポーネントを入れておく
        thisCamera = GetComponent<Camera>();

        // 顔座標の初期化
        headPos = WINDOW_INIT_POS;

        // 仮想窓の初期化
        window.Itit(WINDOW_HEIGHT, WINDOW_HEIGHT * thisCamera.aspect, headPos);

        // の初期化
        cameraDetector = new CameraDetector(headPos / UNIT_RATIO);
    }


    void Update()
    {
        // 実行中にアス比が変わるかもなので、更新する
        window.width = WINDOW_HEIGHT * thisCamera.aspect;

        try
        {
            // 顔座標の取得を試みる→取れないときは例外を投げる
            Vector3 newPos = cameraDetector.getFacePos() * UNIT_RATIO;
            Vector3 diff = newPos - headPos;

            // 誤検知をスルーするための小細工
            float nowTime = Time.realtimeSinceStartup;
            if (diff.magnitude > 10.0f && (nowTime - this.lastUpdateTime) < 0.5f)
            {
                return;
            }
            this.lastUpdateTime = nowTime;

            // 取れたから更新
            headPos = newPos;

            // カメラが移動
            transform.Translate(diff);

            // 仮想窓は動かない→カメラからは逆向きに移動して見える
            window.Translate(-1 * diff);

            //仮想窓での描画範囲を計算
            Matrix4x4 m = ProjectionToVirtialWindow(window, thisCamera.nearClipPlane, thisCamera.farClipPlane);
            thisCamera.projectionMatrix = m;
        }
        catch{
            // 顔が検出できない→スルー
        }
    }

    void OnApplicationQuit()
    {
        if (cameraDetector != null)
        {
            cameraDetector.Dispose();
            cameraDetector = null;
        }
    }

    // 仮想窓(window)とクリップ面の情報(near, far)から、仮想窓から映る範囲を対象にした射影変換行列を計算する
    private Matrix4x4 ProjectionToVirtialWindow(VirtualWindow window, float near, float far)
    {
        // 「(カメラの)ローカル座標」と「カメラ座標」ではz軸が±逆になるのでwindow.zに-が付いている
        return ProjectionToOffCenter(window.left, window.right, window.bottom, window.top, -window.z, near, far);
    }

    // カメラ座標にて、Zの位置にある矩形(Left, Right, Bottom, Top)を範囲とする、クリップ領域(Near, Far)を対象とした射影変換行列を計算する
    private Matrix4x4 ProjectionToOffCenter(float L, float R, float B, float T, float Z, float N, float F)
    {
        Matrix4x4 m = new Matrix4x4();
        m[0, 0] = -2.0F * Z / (R - L); m[0, 1] = 0; m[0, 2] = (R + L) / (R - L); m[0, 3] = 0;
        m[1, 0] = 0; m[1, 1] = -2.0F * Z / (T - B); m[1, 2] = (T + B) / (T - B); m[1, 3] = 0;
        m[2, 0] = 0; m[2, 1] = 0; m[2, 2] = -(F + N) / (F - N); m[2, 3] = -(2.0F * F * N) / (F - N);
        m[3, 0] = 0; m[3, 1] = 0; m[3, 2] = -1.0F; m[3, 3] = 0;
        return m;
    }

    // カメラの描画範囲となる仮想窓
    private class VirtualWindow
    {
        private UnityEngine.Rect _rect = new UnityEngine.Rect();
        private float _z = 0;

        //座標は読取専用
        public float left { get { return this._rect.xMin; } }
        public float right { get { return this._rect.xMax; } }
        public float bottom { get { return this._rect.yMin; } }
        public float top { get { return this._rect.yMax; } }
        public float z { get { return this._z; } }

        // 設定用（widthは後で変わる可能性があるので、外から更新できる様にpublic）
        public float width { set { this._rect.width = value; } }
        private float height { set { this._rect.height = value; } }
        private Vector3 center { set { this._rect.center = value; this._z = value.z; } }

        // 初期設定
        public void Itit(float height, float width, Vector3 center)
        {
            this.height = height;
            this.width = width;
            this.center = center;   // Rect.centerのセッティングは最後にしないとRectの座標がズレる様子
        }

        // diffだけ移動させる
        public void Translate(Vector3 diff)
        {
            this._rect.center += (Vector2)diff;
            this._z += diff.z;
        }
    }

    // 顔座標検出器
    private class CameraDetector : IDisposable
    {
        //スクリーンサイズ
        public int Width = 640;
        public int Height = 480;

        private Vector3 defaultPos;

        //openCVの機能：
        //ビデオファイルやカメラからキャプチャを行う
        VideoCapture video;
        public int VideoIndex = 0;
        //顏検出のためのクラス
        CascadeClassifier cascade;

        //カメラからの情報を2Dデータとして保管する変数
        private Texture2D texture;

        public CameraDetector(Vector3 defaultPos)
        {

            // 初期座標をセット
            this.defaultPos = defaultPos;

            //カメラからのストリーミングを開始
            video = new VideoCapture(VideoIndex);
            //ビデオストリーム中のフレームの幅と高さ
            video.Set(CaptureProperty.FrameWidth, Width);
            video.Set(CaptureProperty.FrameHeight, Height);

            // 顔検出器の作成 (.xmlファイルで、他にも顏のパーツや動物の検出器もあるみたい
            cascade = new CascadeClassifier(Application.dataPath + @"/haarcascade_frontalface_alt.xml");


            // テクスチャの作成(何処のマテリアルに画像を出力するか
            texture = new Texture2D(Width, Height, TextureFormat.RGB24, false);
            GameObject.Find("Plane").GetComponent<Renderer>().material.mainTexture = texture;
        }


        // 顔座標を取る（コレ、取れなかったら例外出すけど、それで良いのか…）
        public Vector3 getFacePos(){

            if (!video.IsOpened()) { throw new Exception(@"Camera Not found"); }

            using (Mat image = new Mat())
            {
                // ストリーミングしている画像を読み込む
                video.Read(image);

                // 顔を検出する
                var faces = cascade.DetectMultiScale(image);

                // 検出出来なかった → 描画だけして、例外投げておしまい
                if (faces.Length <= 0){
                    using (var cvtImage = image.CvtColor(ColorConversion.BgrToRgb))
                    {
                        texture.LoadRawTextureData(cvtImage.ImEncode(".bmp"));
                        texture.Apply();
                    }
                    throw new Exception(@"Face Not found");
                }

                var face = faces[0];

                // 顔の矩形を描画する
                image.Rectangle(face, new Scalar(255, 0, 0), 2);

                using (var cvtImage = image.CvtColor(ColorConversion.BgrToRgb))
                {
                    texture.LoadRawTextureData(cvtImage.ImEncode(".bmp"));
                    texture.Apply();
                }

                // 中心の座標を計算する(一旦Zは考慮しない)
                var x = face.TopLeft.X + (face.Size.Width / 2) - Width/2;
                var y = face.TopLeft.Y + (face.Size.Height / 2) - Height/2;

                return new Vector3(-x, -y, defaultPos.z);    // 座標系変換のため、-が付いてる
            }
        }

        public void Dispose(){
            if (video != null){
                video.Dispose();
                video = null;
            }
        }
    }


#if UNITY_EDITOR
    // 開発用　シーンビューで視野を描画する
    void OnDrawGizmos()
    {
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
        Gizmos.color /= 1.5F;
        Gizmos.DrawRay(transform.position, transform.TransformDirection(to1) * 5);
        Gizmos.DrawRay(transform.position, transform.TransformDirection(to2) * 5);
        Gizmos.DrawRay(transform.position, transform.TransformDirection(to3) * 5);
        Gizmos.DrawRay(transform.position, transform.TransformDirection(to4) * 5);
    }
#endif
}