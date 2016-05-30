using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class KillsDataRead : MonoBehaviour {
    // 撃破された数値のテキスト表示
    public Text killsText;

    // Use this for initialization
    void Start () {
        killsText.text = "kills : " + KillsData.Instance.killCount;
    }

    // Update is called once per frame
    void Update () {
	
	}
}
