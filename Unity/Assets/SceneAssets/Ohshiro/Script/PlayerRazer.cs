using UnityEngine;using System.Collections;public class PlayerRazer : MonoBehaviour {    // プレイヤーから射出されるレーザ    //public GameObject laser;	// Use this for initialization	void Start () {		}		// Update is called once per frame	void Update () {        // マウスカーソル位置からカメラ中心点へ向かう光線(Rayを飛ばす)        Vector3 pos = new Vector3(Screen.width / 2, Screen.height / 2, 0);        Ray ray = Camera.main.ScreenPointToRay(pos);        // 光線と物体の衝突判定        RaycastHit rayHit;        if (Physics.Raycast(ray, out rayHit, Mathf.Infinity))        {
//            Debug.Log(rayHit.collider.gameObject.name);
            // 敵に光線が衝突している場合　敵に死刑宣告
            if (rayHit.collider.gameObject.CompareTag("Enemy"))
            {
                rayHit.collider.gameObject.SendMessage("EnemyDelete");
            }
        }    }}