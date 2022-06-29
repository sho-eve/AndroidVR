using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Play : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//バトル画面へ遷移
	//マウス選択も可能できるよう関数を使用
	public void OnClick () {
		BattleManager.battleStatus = BattleManager.battleStart;
	}

	//joyconスティックによるカーソル判定
	void OnTriggerStay2D (Collider2D collider){
		if (collider.gameObject.GetComponent<UIManager> ().aPressed == true) {
			OnClick ();
		} else {
			return;
		}
	}
		
}
