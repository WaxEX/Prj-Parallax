﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class Enemy : MonoBehaviour
{
    private Renderer _renderer;

    // 敵の全体数を管理しているオブジェクト
    private GameObject enemyManager;
    EnemyManager eManager;

    // プレイヤーのライフ管理しているオブジェクト
    private GameObject playerObj;
    Player playerScript;

    //防御壁オブジェクト
    private GameObject wallObj;

    //変数宣言
    Vector3 position;
    float speed;
    float rotSpeed;

    //点滅フラグ(壁との距離が近づくとtrueになる)
    bool isBlinker = false;
    //点滅フラグをオンの距離
    float onBlinkerDistance = 40.0f;
    //スピードを変える距離
    float onBlinkerDistanceMiddle = 25.0f;
    float onBlinkerDistanceEnd = 15.0f;
    //点滅のスピード(3段階)
    float blinkerSpeedFirst = 1.0f;
    float blinkerSpeedMiddle = 0.25f;
    float blinkerSpeedEnd = 0.01f;

    //死んだときに使うパーティクル
    public GameObject flare;
    public GameObject spark;

    // Use this for initialization
    void Start()
    {
        //自分自身の描画処理を取得
        _renderer = GetComponent<Renderer>();

        //敵の全体数を管理しているオブジェクトを探す。
        enemyManager = GameObject.Find("EnemyManager");
        //敵の全体数を管理しているオブジェクトからスクリプトをゲットする
        eManager = enemyManager.GetComponent<EnemyManager>();

        //プレイヤーのLifeを管理しているオブジェクトを探す。
        playerObj = GameObject.Find("Player");
        playerScript = playerObj.GetComponent<Player>();

        //防御壁を探す。
        wallObj = GameObject.Find("InvisibilityWall");
        
        speed = Random.Range(0.1f,0.3f);
        rotSpeed = Random.Range(0.1f, 0.6f);
    }

    // Update is called once per frame
    void Update()
    {

        //enemy進行方向　マイナスＺ軸方向。等速。
        transform.position += new Vector3(0, 0, -speed);
        transform.Rotate(rotSpeed, rotSpeed, 0, 0);

        // 敵が点滅していない状態
        if (isBlinker == false)
        {
            //敵と壁との距離が20以下になると点滅開始
            if (DistanceBetweenTheWall() < onBlinkerDistance)
            {
                StartCoroutine(BlinkerCoroutine());
                isBlinker = true;
            }
        }


    }


    // 壁との距離
    float DistanceBetweenTheWall() {
        //エネミーの中心点
        Vector3 enemyPos = transform.position;
        //壁の中心点
        Vector3 wallPos = wallObj.transform.position;

        //エネミーから直線状に当たる壁の位置
        Vector3 distanceWallPos;
        distanceWallPos.x = wallPos.x + enemyPos.x;
        distanceWallPos.y = wallPos.y + enemyPos.y;
        distanceWallPos.z = wallPos.z;
        //エネミーと壁との距離
        float diff = Vector3.Distance(enemyPos, distanceWallPos);
        return diff;
    }

    // Enemy delete
    void Death()
    {
        eManager.enemyDecrease();
        Destroy(gameObject);
    }
    // 壁にぶつかった時の死
    public void EnemyDelete()
    {
        playerScript.PlayerDecrease();
        this.Death();
    }
    // 光線に当たった時の死
    public void EnemyDeleteByLaser()
    {
        //エフェクト生成
//        Instantiate(flare, transform.position, Quaternion.identity);
        Instantiate(spark, transform.position, Quaternion.identity);

        eManager.KillCount();
        this.Death();
    }


    //点滅処理
    IEnumerator BlinkerCoroutine()
    {
        //変更前のマテリアルのコピーを保存
        var originalMaterial = new Material(_renderer.material);
        float blinkerSpeed = blinkerSpeedFirst;
        _renderer.material.EnableKeyword("_EMISSION"); //キーワードの有効化を忘れずに

        for (;;)
        {
            //壁にすごく近い
            if (DistanceBetweenTheWall() < onBlinkerDistanceEnd)
            {
                blinkerSpeed = blinkerSpeedEnd;
                originalMaterial.color = new Color(1, 0.05f, 0.05f, 1);//赤色
                _renderer.material.SetColor("_EmissionColor", new Color(1, 0, 0, 1)); //赤色点滅
            }
            //壁にまぁまぁ近い
            if (DistanceBetweenTheWall() < onBlinkerDistanceMiddle)
            {
                blinkerSpeed = blinkerSpeedMiddle;
                originalMaterial.color = new Color(1, 0.92f, 0.016f, 1);//黄色
            }
            //壁にほどほどに近い
            if (DistanceBetweenTheWall() < onBlinkerDistance)
            {
                originalMaterial.color = new Color(1, 0.92f, 0.016f, 1);//緑色
            }
            yield return new WaitForSeconds(blinkerSpeed);//任意の秒間明るく発色
            _renderer.material = originalMaterial; //元に戻す
            yield return new WaitForSeconds(blinkerSpeed);//任意の秒間元の色に発色
        }
    }

}