using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class Enemy2 : MonoBehaviour {

	public GameObject shot;

	public float shotInterval = 0f;
	public float shotIntervalMax = 3f;

	public float distance = 30f;

	private GameObject target;
	public GameObject explosion;

	public int armorPoint;
	public int armorPointMax = 300;
	private int damage;

	public Image markerImage;
	private Image marker;
	private GameObject compass;

	public GameObject enemyAP;
	private float lastAttackTime = 10.0f;
	public float displayTime = 7.0f;
	public Image gaugeImage;

	public GameObject apRecover;

	public AudioSource audio01;

	public int breakScore = 200;

	public bool addScore = true;

	// Use this for initialization
	void Start () {

		//ターゲット取得
		target = GameObject.Find ("PlayerTarget");

		//体力設定
		armorPoint = armorPointMax;

		//マーカーをレーダーに表示
		compass = GameObject.Find ("CompassMask");
		marker = Instantiate (markerImage, compass.transform.position, Quaternion.identity) as Image;
		marker.transform.SetParent (compass.transform, false);

		enemyAP.SetActive (false);

		audio01 = GetComponent<AudioSource> ();
		
	}
	
	// Update is called once per frame
	void Update () {

		if (BattleManager.battleStatus == BattleManager.battlePlay) {

			//プレイヤーの相対位置にマーカーを配置
			Vector3 position = transform.position - target.transform.position;
			marker.transform.localPosition = new Vector3 (position.x * 10, position.y * 10, 0);

			//体力ゲージ表示時間
			lastAttackTime += Time.deltaTime;

			if (lastAttackTime <= displayTime) {
				enemyAP.SetActive (true);
			} else {
				enemyAP.SetActive (false);
			}

			//UIカメラ方向へ
			enemyAP.transform.LookAt (Camera.main.transform.position);

			//ゲージ反映
			gaugeImage.transform.localScale = new Vector3 ((float)armorPoint / armorPointMax, 1, 1);

			//距離取得
			float distance = (target.transform.position - transform.position).sqrMagnitude;

			//一定距離内動作
			if (distance <= 500) {
			
				//方向転換
				Quaternion targetRotation = Quaternion.LookRotation (target.transform.position - transform.position);
				transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, Time.deltaTime * 8);

				//攻撃処理
				shotInterval += Time.deltaTime;

				//射撃間隔
				if (shotInterval > shotIntervalMax) {
					Instantiate (shot, transform.position, transform.rotation);
					shotInterval = 0;
				}
			}

			//消滅　アイテム出現
			if (armorPoint <= 0) {
				Instantiate (explosion, transform.position, transform.rotation);
				Instantiate (apRecover, transform.position, new Quaternion (0, 0, 0, 0));
				Destroy (gameObject);

				if (addScore == true) {
					Character.armorPoint += breakScore;
				}
			}

			//地面以下に出現時Destroy
			float terrainHeight = Terrain.activeTerrain.SampleHeight (transform.position);

			//Debug.Log (terrainHeight);
			//Debug.Log (transform.position.y);

			if (transform.position.y <= terrainHeight) {
				Destroy (gameObject);
			}

			//ダメージ設定
			damage = UnityEngine.Random.Range (150, 200);
				
		} else {

			Destroy (gameObject);
		}
	}

	//衝突判定
	private void OnCollisionEnter (Collision collider){
		//ダメージ判定
		if (collider.gameObject.tag == "Shot") {
			//damage = collider.gameObject.GetComponent<Shot> ().damage;
			armorPoint -= damage;
			Debug.Log (armorPoint);
			lastAttackTime = 0f;

		} else {
			//Debug.Log (armorPoint);
		}
	}

	//消滅と同時にマーカー消滅
	void OnDestroy(){
		Destroy (marker);
	}
}
