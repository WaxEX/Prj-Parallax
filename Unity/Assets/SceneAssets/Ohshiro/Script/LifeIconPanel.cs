using UnityEngine;
using System.Collections;

public class LifeIconPanel : MonoBehaviour {

    public GameObject[] lifeIcons;

    public void UpdateLife(int life)
    {
        for (int i = 0; i<lifeIcons.Length; i++) {
            if(i+1 < life)
            {
                lifeIcons[i].SetActive(true);
            }
            else
            {
                lifeIcons[i].SetActive(false);
            }
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
