﻿using UnityEngine;using System.Collections;public class InvisibilityWall : MonoBehaviour {	// Use this for initialization	void Start () {		}		// Update is called once per frame	void Update () {		}    void OnTriggerEnter(Collider col) {        // 敵死刑宣告        if (col.gameObject.CompareTag("Enemy")) {            col.gameObject.SendMessage("EnemyDelete");        }    }}