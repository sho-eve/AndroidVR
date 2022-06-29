using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRecover : MonoBehaviour {

	private float destroyTime = 0f;
	private float destroy = 30.0f;

	public GameObject flash;

	public AudioSource audio01;

	// Use this for initialization
	void Start () {

		audio01 = gameObject.GetComponent<AudioSource> ();

	}
	
	// Update is called once per frame
	void Update () {

		if (BattleManager.battleStatus == BattleManager.battlePlay) {
			//回転処理
			transform.Rotate (new Vector3 (0, 5, 0));

			//時間経過で消滅
			destroyTime += Time.deltaTime;
			if (destroyTime >= destroy) {
				Destroy (gameObject);
			}
		} else {
			Destroy (gameObject);
		}
	}

	//衝突で消滅
	private void OnCollisionEnter (Collision collider){
		if (collider.gameObject.tag == "Player") {
			Instantiate (flash, transform.position, transform.rotation);
			AudioSource.PlayClipAtPoint (audio01.clip, transform.position);
			Character.score += 50;
			Destroy (gameObject);
		}
	}
}
