using UnityEngine;
using System.Collections;

public class KillsData : MonoBehaviour {
    private static KillsData instance;
    public int killCount;

    public static KillsData Instance
    {
        get
        {
            if (null == instance)
            {
                instance = (KillsData)FindObjectOfType(typeof(KillsData));
                if (null == instance)
                {
                    Debug.Log(" KillsData Instance Error ");
                }
            }
            return instance;
        }
    }

    void Awake()
    {
        GameObject[] obj = GameObject.FindGameObjectsWithTag("KillsData");
        if (1 < obj.Length)
        {
            // 既に存在しているなら削除
            Destroy(gameObject);
        }
        else {
            // シーン遷移では破棄させない
            DontDestroyOnLoad(gameObject);
        }
    }
}
