using UnityEngine;
using System.Collections;

public class PlayerRazer : MonoBehaviour {
    // プレイヤーから射出されるレーザ
    public GameObject laserObject;
    // 射程
    public float laserRange = 20;
    // 線の太さ
    public float laserWidth = 1;
    // 銃口
    public Transform muzzle;
    // LineRendererのコンポーネントの参照を入手
    LineRenderer lineRenderer;

    // Use this for initialization
    void Start () {
        //LineRendererのオブジェクトを作成し、lineRendererを取得
        GameObject l = Instantiate(laserObject, muzzle.position, Quaternion.identity) as GameObject;
        lineRenderer = l.GetComponent<LineRenderer>();

        //LinRendererを設定する。
        //始点と終点の2つの座標で線を引く
        lineRenderer.SetVertexCount(2);
        //LineRendererの太さ(始点と終点で変更できる)
        lineRenderer.SetWidth(laserWidth, laserWidth);
        //LineRendererの色(始点は緑、終点は緑)
        lineRenderer.SetColors(Color.blue, Color.blue);
    }
	
	// Update is called once per frame
	void Update () {
        SetLaser();

        // カメラ中心点へ向かう光線(Rayを飛ばす)
        Vector3 pos = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = Camera.main.ScreenPointToRay(pos);

        // 光線と物体の衝突判定
        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit, Mathf.Infinity))
        {
            Debug.Log(rayHit.collider.gameObject.name);
            // 敵に光線が衝突している場合　敵に死刑宣告
            if (rayHit.collider.gameObject.CompareTag("Enemy"))
            {
                //rayHit.collider.gameObject.SendMessage("EnemyDelete");
                Debug.Log(rayHit.collider.gameObject.name);
            }
        }
    }

    //LineRendererの位置を更新する処理
    void SetLaser()
    {
        //始点
        Vector3 start;
        //始点を設定
        start = muzzle.position;
        lineRenderer.SetPosition(0, start);

        //終点
        Vector3 end;
        //終点を設定する（終点は始点からmuzzuleの前方向にrange分伸ばした先に設定される）
        end = start + (muzzle.forward * laserRange);

        //終点を設定する
        lineRenderer.SetPosition(1, end);
    }


}
