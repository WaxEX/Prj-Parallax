using UnityEngine;
using System.Collections;

public class PlayerLaser : MonoBehaviour {
    // プレイヤーから射出されるレーザ
    public GameObject laserObject;
    // 射程
    public float laserRange = 20;
    // 線の太さ
    public float laserEndWidth = 1;
    public float laserStartWidth = 0.1f;
    // 銃口
    public Transform muzzle;
    // LineRendererのコンポーネントの参照を入手
    LineRenderer lineRenderer;
    // MainCameraオブジェクト
    GameObject mCamera;
    WindowCamera windowCamera;

    // Use this for initialization----------------------------------------------------
    void Start () {
        //LineRendererのオブジェクトを作成し、lineRendererを取得
        GameObject l = Instantiate(laserObject, muzzle.position, Quaternion.identity) as GameObject;
        lineRenderer = l.GetComponent<LineRenderer>();
        //LinRendererを設定する。
        //始点と終点の2つの座標で線を引く
        lineRenderer.SetVertexCount(2);
        //LineRendererの太さ(始点と終点で変更できる)
        lineRenderer.SetWidth(laserStartWidth, laserEndWidth);
        ////LineRendererの色(始点は緑、終点は緑)
        //lineRenderer.SetColors(Color.blue, Color.blue);

        //MainCamera取得。WindowCameraコンポーネント取得。
        mCamera = GameObject.Find("Main Camera");
        windowCamera = mCamera.GetComponent<WindowCamera>();
    }
    // Use this for initialization----------------------------------------------------


    // Update is called once per frame------------------------------------------------
    void Update () {
        //光線射出
        PlayerLasarBeam();
    }
    // Update is called once per frame------------------------------------------------


    //光線描画-------------------------------------------------------------------------
    // virtualWindowの中心座標取得
    Vector3 VirtualWindowCenter()
    {
        float windowPosX = windowCamera.VirtualWindowCenterX();
        float windowPosY = windowCamera.VirtualWindowCenterY();
        return new Vector3(windowPosX, windowPosY, 0);
    }
    //LineRendererの位置を更新する処理
    void SetLaser(Vector3 s,Vector3 e)
    {
        //始点
        Vector3 start;
        //始点を設定
        start = s;
        lineRenderer.SetPosition(0, start);

        //終点
        Vector3 end;
        //終点を設定する（終点は仮想窓の中心点に向かって延ばす）
        end = e;
        //終点を設定する
        lineRenderer.SetPosition(1, end);
    }
    //光線描画-------------------------------------------------------------------------


    //光線射出-------------------------------------------------------------------------
    void PlayerLasarBeam()
    {
        //画面中央のスクリーン座標を取得
        Vector3 cameraCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        //Raycastで飛ばす光線を作成(Mainカメラの中央部分から飛ばす)
        Ray ray = Camera.main.ScreenPointToRay(cameraCenter);
        Debug.DrawRay(transform.position, ray.direction*laserRange);

        // 光線と物体の衝突判定
        RaycastHit rayHit;
        //if (Physics.Raycast(ray, out rayHit, laserRange))
        if (Physics.Raycast(transform.position, ray.direction * laserRange,out rayHit,laserRange))
        {
            // 敵に光線が衝突している場合　敵に死刑宣告
            if (rayHit.collider.gameObject.CompareTag("Enemy"))
            {
                //Debug.Log(rayHit.collider.gameObject.name);
                rayHit.collider.gameObject.SendMessage("EnemyDeleteByLaser");
            }
        }
        SetLaser(transform.position, ray.direction * laserRange);
    }
    //光線射出-------------------------------------------------------------------------




}
