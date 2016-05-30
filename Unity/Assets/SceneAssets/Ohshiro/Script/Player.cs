using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Player : MonoBehaviour {
    // Player Life
    const int CONST_PLAYER_DEFAULT_LIFE = 100;
    // PlayerのLife総数 初期化
    int playerTotalLifeNumber = CONST_PLAYER_DEFAULT_LIFE;
    // Player　生きてるか死んでるか
    private bool isPlayerLife = true;

    //// Lifeオブジェクトを管理しているパネルを参照
    //public GameObject lifePanel;
    //LifeIconPanel lifeIconPanelScript;

    // MainCameraオブジェクト
    GameObject mCamera;
    UnityStandardAssets.ImageEffects.NoiseAndScratches noiseScript;

    // ライフテキストのオブジェクト
    public Text LifeText;


    // Use this for initialization----------------------------------
    void Start () {
        ////プレイヤーのLifeを管理しているオブジェクトを探す。
        //lifePanel = GameObject.Find("LifeIconPanel");
        //lifeIconPanelScript = lifePanel.GetComponent<LifeIconPanel>();

        //MainCamera取得。WindowCameraコンポーネント取得。
        mCamera = GameObject.Find("Main Camera");
        noiseScript = mCamera.GetComponent<UnityStandardAssets.ImageEffects.NoiseAndScratches>();

        //プレイヤーライフ
        LifeText.text = "LIFE : " + CONST_PLAYER_DEFAULT_LIFE;


    }
    // Use this for initialization----------------------------------



    // Update is called once per frame------------------------------
    void Update () {

        //生命力がない場合は処理しない
        if (isPlayerLife == false) { return; }

        //Debug.Log("生命力:" + PlayerTotalLifeNumber());
        if (PlayerTotalLifeNumber() <= 0)
        {
            isPlayerLife = false;
            //Debug.Log("生命力なし");
            //ゲームオーバー画面へ遷移
            toGameOver();
        }

        //プレイヤーライフ
        LifeText.text = "LIFE : " + playerTotalLifeNumber;

    }
    // Update is called once per frame------------------------------



    // PlayerのLife総数管理------------------------------------------
    // 現在のPlayerのLife総数を取得する関数
    int PlayerTotalLifeNumber()
    {
        return playerTotalLifeNumber;
    }
    // PlayerのLifeの数値減少
    public void PlayerDecrease()
    {
        if (playerTotalLifeNumber > 0) {
            //ダメージと同時にノイズ
            StartCoroutine(NoiseCoroutine());
            //ライフの減少
            playerTotalLifeNumber--;
          //lifeIconPanelScript.UpdateLife(playerTotalLifeNumber);
        }
    }
    // PlayerのLife総数管理------------------------------------------

    //ノイズを走らせる
    IEnumerator NoiseCoroutine()
    {
        noiseScript.enabled = true;
        yield return new WaitForSeconds(0.25f); //0.5秒後にノイズOFF
        noiseScript.enabled = false;
    }



    // ゲームオーバー画面へ遷移
    public void toGameOver()
    {
        SceneManager.LoadScene("GameOver");
    }
}
