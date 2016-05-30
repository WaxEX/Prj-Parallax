using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    // enemyの初期総数。
    const int CONST_ENEMY_DEFAULT_NUM = 10;
    // enemyを生成するべき境界値の設定
    const int CONST_ENEMY_LOWER_NUM = 5;
    // 境界値になったら生成すべきenemyの個体数設定
    const int CONST_ENEMY_POP_NUM = 10;

    // enemy生成時のZ軸方向の範囲
    // Unityのワールド座標Z+方向の範囲
    const float FRONT_Z = 50.0f;
    //// Unityのワールド座標Z-方向の範囲
    //const float BACK_Z = 30.0f;

    // エネミー総数　初期化
    int enemyTotalNumber = CONST_ENEMY_DEFAULT_NUM;

    // enemyを動的生成用のPrefab
    public GameObject enemyPrefab;
    // enemyの親になるオブジェクト
    public GameObject EnemyParent;
    // 壁オブジェクト（enemyを生成する範囲の基となる）
    public GameObject Wall;

    // enemyがプレイヤーに撃破された数
    private static int deathTotal;
    // 撃破された数値のテキスト表示
    public Text killsText;    

    // Use this for initialization-----------------------------------------
    void Start()
    {
        KillsData.Instance.killCount = 0;
        //enemyがプレイヤーに撃破された数　初期化
        deathTotal = 0;
        this.InstantiateEnemy(CONST_ENEMY_DEFAULT_NUM);
        //this.InstantiateEnemy(1);

        killsText.text = "kills : 0";

    }
    // Use this for initialization-----------------------------------------


    // Update is called once per frame-------------------------------------
    void Update()
    {
        // 敵の総数が一定未満になったら一定量生成する
        if (enemyTotalNumber < CONST_ENEMY_LOWER_NUM)
        {
            this.InstantiateEnemy(CONST_ENEMY_POP_NUM);
            enemyIncrease();
        }

        //現在殺されている数
        int kills = this.KillTotalCount();
        killsText.text = "kills : " + kills;
        KillsData.Instance.killCount = kills;
    }
    // Update is called once per frame-------------------------------------


    // enemyのPosition決定処理----------------------------------------------
    Vector3 GetInstantiatePosition(GameObject obj)
    {
        //指定オブジェクトの中心座標とサイズ取得
        Vector3 objPos = GetPosByObj(obj);
        Vector3 objSize = GetSizeByObj(obj);
        //enemyをランダムに生成する範囲の計算。(zのenemy出現範囲は定数で設定しているため計算なし)
        float leftPosX = objPos.x - (objSize.x / 2);
        float rightPosX = objPos.x + (objSize.x / 2);
        float topPosY = objPos.y + (objSize.y / 2);
        float downPosY = objPos.y - (objSize.y / 2);
        //各座標をランダムに取得
        float x = Random.Range(leftPosX, rightPosX);
        float y = Random.Range(topPosY, downPosY+2.0f);
        //float z = Random.Range(BACK_Z, FRONT_Z);

        //生成位置を返す。
        //return new Vector3();
        return new Vector3(x, y, 0);
        //return new Vector3(x, y, z);
    }
    // enemyのPosition決定処理----------------------------------------------


    // enemyを生産----------------------------------------------------------
    void InstantiateEnemy(int num)
    {
        for (int i = 0; i < num; i++)
        {
            //生成する位置のvector3 positionをランダムになるように設定する。
            Vector3 enemyInstansPos = GetInstantiatePosition(Wall);
            enemyInstansPos.z = FRONT_Z+(i*10);
            GameObject enemy = (GameObject)Instantiate(enemyPrefab, enemyInstansPos, Quaternion.identity);
            //生成したenemyオブジェクトの親を設定
            enemy.transform.parent = EnemyParent.transform;
        }
    }
    // enemyを生産----------------------------------------------------------


    // enemyの総数管理------------------------------------------------------
    // 現在のenemyの総数を取得する関数
    int EnemyTotalNumber()
    {
        return enemyTotalNumber;
    }
    // enemyTotalNumberの数値減少
    public void enemyDecrease()
    {
        if (enemyTotalNumber > 0) { enemyTotalNumber--;}
    }
    // enemyTotalNumberの数値増加
    public void enemyIncrease()
    {
        enemyTotalNumber += CONST_ENEMY_POP_NUM;
    }
    // enemyの総数管理------------------------------------------------------

    // enemyがプレイヤーに撃破された数----------------------------------------
    // 撃破された総数
    public int KillTotalCount() {
        //Debug.Log("殺されてる数" + deathTotal);
        return deathTotal;
    }
    // 撃破された時にカウントする
    public int KillCount() {
        return deathTotal++;
    }
    // enemyがプレイヤーに撃破された数----------------------------------------



    // オブジェクトの情報取得------------------------------------------------
    // 指定オブジェクトの中心座標取得
    Vector3 GetPosByObj(GameObject Obj)
    {
        return Obj.transform.position;
    }
    // 指定オブジェクトのサイズ取得
    Vector3 GetSizeByObj(GameObject Obj)
    {
        return Obj.transform.localScale;
    }
    // オブジェクトの情報取得------------------------------------------------


}
