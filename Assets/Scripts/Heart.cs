using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour {

	private float destroyTime = 0f;
	private float destroy = 30.0f;

	public GameObject flash;

	private GameObject player;

	public AudioSource audio01;

	// Use this for initialization
	void Start () {

		//回復処理用ターゲット取得
		player = GameObject.Find ("unitychan");

		//効果音情報取得
		audio01 = gameObject.GetComponent<AudioSource> ();
		
	}
	
	// Update is called once per frame
	void Update () {

		//ゲームプレイ時のみ
		if (BattleManager.battleStatus == BattleManager.battlePlay) {
			
			//回転処理
			transform.Rotate (new Vector3 (0, 5, 0));

			//時間経過で消滅
			destroyTime += Time.deltaTime;
			if (destroyTime >= destroy) {
				Destroy (gameObject);
			} else {
				return;
			}

		} else {
			Destroy (gameObject);
		}
		
	}

	//衝突処理
	private void OnCollisionEnter (Collision collider){
		if (collider.gameObject.tag == "Player") {
			apRecover ();
		}

		if (collider.gameObject.tag == "Shot") {
			apRecover ();
		}

	}

	//消滅時処理
	//OnDestroy処理内にInstantiateを使用できないため
	private void apRecover(){
		AudioSource.PlayClipAtPoint (audio01.clip, player.transform.position, 5.0f);
		Instantiate (flash, transform.position, transform.rotation);
		Instantiate (flash, player.transform.position, player.transform.rotation);
		Character.armorPoint += 500;
		Character.score += 100;
		Destroy (gameObject);
	}
}
