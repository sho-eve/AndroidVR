using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour {

	//各種変数設定
	public float shotSpeed = 15.0f;
	public float destroytime = 2.0f;

	public GameObject explosion;

	public int damage = 200;

	// Use this for initialization
	void Start () {

		//一定時間後自動的に消滅
		Destroy (gameObject,destroytime);
		
	}
	
	// Update is called once per frame
	void Update () {

		if (BattleManager.battleStatus == BattleManager.battlePlay) {

			//距離による威力減衰 最小１
			damage--;
			if (damage <= 1) {
				damage = 1;
			}

			//弾前進
			transform.position += transform.forward * Time.deltaTime * shotSpeed;

		} else {
			Destroy (gameObject);
		}
	}

	//地面またはとぶつかると消滅
	private void OnCollisionEnter (Collision collider) {
		if (collider.gameObject.name == "Terrain") {
			DestroyCarryOut ();
		}

		if (collider.gameObject.tag == "Enemy") {
			DestroyCarryOut ();
		}

		if (collider.gameObject.tag == "Heart") {
			DestroyCarryOut ();
		}
	}

	//OnDestroy内でのInstantiateはエラー原因となるため
	private void DestroyCarryOut () {
		Instantiate (explosion, transform.position, transform.rotation);
		Destroy (gameObject);
	}
		 
}
