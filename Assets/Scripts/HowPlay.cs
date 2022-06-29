using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowPlay : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

	//遊び方画面へ遷移
	//Playスクリプトに同じくマウス選択可
	public void OnClick(){
		BattleManager.battleStatus = BattleManager.howPlayState;
	}

	//joyconスティックによるカーソル判定
	void OnTriggerStay2D (Collider2D collider) {
		if (collider.gameObject.GetComponent<UIManager> ().aPressed == true) {
			OnClick ();
		} else {
			return;
		}
	}
		
}
