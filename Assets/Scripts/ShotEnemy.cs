using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotEnemy : MonoBehaviour {

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

		//距離による威力減衰 最小１
		damage --;
		if (damage <= 1) {
			damage = 1;
		}

		//弾前進
		transform.position += transform.forward * Time.deltaTime * shotSpeed;
		
	}

	//地面またはとぶつかると消滅
	private void OnCollisionEnter (Collision collider) {
		if (collider.gameObject.name == "Terrain") {
			Destroy (gameObject);
			Instantiate (explosion, transform.position, transform.rotation);
		}

		if (collider.gameObject.tag == "Player") {
			Destroy (gameObject);
			Instantiate (explosion, transform.position, transform.rotation);
		}
	}
		 
}
