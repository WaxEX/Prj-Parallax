using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    // 敵の全体数を管理しているオブジェクト
    private GameObject enemyManager;
    EnemyManager eManager;

    // プレイヤーのライフ管理しているオブジェクト
    private GameObject playerObj;
    Player playerScript;
    
    //変数宣言
    Vector3 position;
    float speed;

    // Use this for initialization
    void Start()
    {
        //変数初期化
        speed = 0.1f;

        //敵の全体数を管理しているオブジェクトを探す。
        enemyManager = GameObject.Find("EnemyManager");
        //敵の全体数を管理しているオブジェクトからスクリプトをゲットする
        eManager = enemyManager.GetComponent<EnemyManager>();

        //プレイヤーのLifeを管理しているオブジェクトを探す。
        playerObj = GameObject.Find("Player");
        playerScript = playerObj.GetComponent<Player>();

    }

    // Update is called once per frame
    void Update()
    {
        //enemy進行方向　マイナスＺ軸方向。等速。
        transform.position += new Vector3(0, 0, -speed);
    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    Destroy(gameObject);
    //}

    // Enemy delete
    // 壁にぶつかった時の死
    public void EnemyDelete()
    {
        playerScript.PlayerDecrease();
        this.Death();
    }
    // 光線に当たった時の死
    public void EnemyDeleteByLaser()
    {
        eManager.KillCount();
        this.Death();
    }

    void Death() {
        eManager.enemyDecrease();
        Destroy(gameObject);
    }
}
