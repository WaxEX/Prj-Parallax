using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    // Player Life
    const int CONST_PLAYER_DEFAULT_LIFE = 5;
    // PlayerのLife総数 初期化
    int playerTotalLifeNumber = CONST_PLAYER_DEFAULT_LIFE;
    // Player　生きてるか死んでるか
    private bool isPlayerLife = true;

    // PlayerのLife総数管理------------------------------------------
    // 現在のPlayerのLife総数を取得する関数
    int PlayerTotalLifeNumber() {
        return playerTotalLifeNumber;
    }
    // PlayerのLifeの数値減少
    public void PlayerDecrease() {
        if (playerTotalLifeNumber > 0) { playerTotalLifeNumber--; }
    }
    // PlayerのLife総数管理------------------------------------------



    // Use this for initialization----------------------------------
    void Start () {
	
	}
    // Use this for initialization----------------------------------



    // Update is called once per frame------------------------------
    void Update () {
        // 生命力がない場合は処理しない
        if (isPlayerLife == false) { return; }

        Debug.Log("生命力:" + PlayerTotalLifeNumber());
        if (PlayerTotalLifeNumber() <= 0)
        {
            isPlayerLife = false;
            Debug.Log("生命力なし");
        }

    }
    // Update is called once per frame------------------------------


}
