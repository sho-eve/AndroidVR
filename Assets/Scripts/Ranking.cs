using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranking : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//ランキング画面へ遷移
	//マウス選択可
	public void OnClick () {
		BattleManager.battleStatus = BattleManager.resultState;
	}

	//joyconスティックによるスティック判定
	void OnTriggerStay2D (Collider2D collider) {
		if (collider.gameObject.GetComponent<UIManager> ().aPressed == true) {
			OnClick ();
		} else {
			return;
		}
	}
}
