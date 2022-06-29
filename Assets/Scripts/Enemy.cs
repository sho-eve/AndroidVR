using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class Enemy : MonoBehaviour {

	public float gravity = 20.0f;
	public GameObject target;
	private GameObject targetParent;
	private Vector3 targetPos;
	private Vector3 moveDirection = Vector3.zero;

	public float speed = 1.8f;

	public GameObject explosion;

	public int armorPoint;
	public int armorPointMax = 1000;
	public int damage;

	public Image markerImage;
	private Image marker;
	private GameObject compass;

	public GameObject enemyAP;
	private float lastAttackTime = 10.0f;
	public float displayTime = 7.0f;
	public Image gaugeImage;

	public GameObject gunRecover;

	private float jabInterval = 0f;

	private float difference;
	private float accel;
	private int jabDamage;

	public Animator animator;

	public bool attackNow = false;

	private float attackTime;
	private int attackDamage;
	private float onlyAttackTime;

	public AudioSource audio01;
	public AudioSource audio02;
	public AudioSource audio03;

	public int breakScore = 300;

	private float appearTimer;

	private bool addScore = true;

	//Joycon用変数配置
	private static readonly Joycon.Button[] button = Enum.GetValues( typeof( Joycon.Button ) ) as Joycon.Button[];
	private List<Joycon> joycons;
	private Joycon joyconL;
	private Joycon.Button? pressedButtonL;

	// Use this for initialization
	void Start () {

		//プレイヤーオブジェクト取得
		target = GameObject.Find ("PlayerTarget");
		targetParent = target.transform.root.gameObject;
		//Debug.Log (targetParent.name);
		
		//体力設定
		armorPoint = armorPointMax;

		//マーカーをレーダーに表示
		compass = GameObject.Find ("CompassMask");
		marker = Instantiate (markerImage, compass.transform.position, Quaternion.identity) as Image;
		marker.transform.SetParent (compass.transform, false);

		enemyAP.SetActive (false);

		//効果音情報
		AudioSource[] audioSources = gameObject.GetComponents<AudioSource>();
		audio01 = audioSources [0];
		audio02 = audioSources [1];
		audio03 = audioSources [2];

		//Joyconインスタンス取得
		joycons = JoyconManager.Instance.j;

		//無いと動かない
		//要確認
		if (joycons == null || joycons.Count <= 0) {
			return;
		}

		//左右識別
		joyconL = joycons.Find( c =>  c.isLeft );
	
	}
	
	// Update is called once per frame
	void Update () {

		//バトル時のみに限定
		//それ以外の時破壊
		if (BattleManager.battleStatus == BattleManager.battlePlay) {

			//体力値制限
			armorPoint = Mathf.Clamp (armorPoint, 0, armorPointMax);

			//Joycon初期値配置
			pressedButtonL = null;

			//無いと以下略
			if (joycons == null || joycons.Count <= 0) {
				return;
			}

			//UnityChanポジション取得
			targetPos = target.transform.position;
			targetPos.y = transform.position.y;

			//EnemyController取得
			CharacterController controller = GetComponent<CharacterController> ();

			//前進制御
			moveDirection = new Vector3 (0, 0, 1);
			moveDirection = transform.TransformDirection (moveDirection);
			moveDirection *= speed;

			//重力制御
			moveDirection.y -= gravity;
			controller.Move (moveDirection * Time.deltaTime);

			//スムーズにターゲットを向く
			Quaternion targetRotation = Quaternion.LookRotation (targetPos - transform.position);
			transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, Time.deltaTime * 10);

			//動きがカクカクする
			//transform.LookAt(targetPos);

			//プレイヤーの相対位置にマーカーを配置
			Vector3 position = transform.position - target.transform.position;
			marker.transform.localPosition = new Vector3 (position.x * 10, position.y * 10, 0);

			/*
			//範囲外の例外設定
			if (Vector3.Distance (target.transform.position, transform.position) <= 15) {
				marker.enabled = true;
			} else {
				marker.enabled = false;
			}
			*/

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

			//地下に潜り込んだ時破壊
			if (transform.position.y < 0) {
				Destroy (gameObject);
			}			

			//armorPoint0以下で消滅
			if (armorPoint <= 0) {
				//アイテム出現位置補正
				Vector3 appearRecover = transform.position;
				appearRecover.y += 1f;

				//時間制御で出現
				appearTimer += Time.deltaTime;

				if (appearTimer >= 1.4f) {
					Instantiate (explosion, transform.position, transform.rotation);
					Instantiate (gunRecover, appearRecover, transform.rotation);
					AudioSource.PlayClipAtPoint (audio03.clip, transform.position);
					appearTimer = 0;
				}

				Destroy (gameObject, 1.5f);
				animator.SetBool ("Die", true);

				if (addScore == true) {
					Character.score += breakScore;
					addScore = false;
				}


			}

			//パンチ用数値設定
			jabInterval += Time.deltaTime;
			jabDamage = UnityEngine.Random.Range (450, 550);

			//パンチ処理を距離で判定
			//平方根計算高負荷により平方値を使用
			difference = (targetPos - transform.position).sqrMagnitude;
			accel = joyconL.GetAccel ().sqrMagnitude;
			if (accel >= 25 && difference <= 2.25 && jabInterval >= 0.5) {
				StartCoroutine (Jab ());
			}

			//距離によるアニメーション設定
			if (difference <= 2.25) {
				animator.SetBool ("Attack", true);
			}

			attackDamage = UnityEngine.Random.Range (200, 300);
				
			//アニメーターで攻撃処理
			//コライダー判定によるダメージ判定高負荷により
			//時間経過でダメージ判定を行う
			AnimatorStateInfo animationState = animator.GetCurrentAnimatorStateInfo (0);

			//本意では無いがアニメーションに合わせていい感じに二回動作
			//いじらない・要確認
			if (animationState.fullPathHash == Animator.StringToHash ("Base Layer.Attack")) {
			
				//Debug.Log ("正常に実行");
				speed = 0f;
				animator.SetBool ("Attack", false);
				//HR.SetActive (true);
				//HL.SetActive (true);
				attackTime += Time.deltaTime;
				onlyAttackTime += Time.deltaTime;

				if (difference <= 2.25 && attackTime >= 0.4f && onlyAttackTime <= 0.5f) {
					Character.armorPoint -= attackDamage;
					targetParent.GetComponent<Character> ().animator.SetBool ("Damage", true);
					audio02.PlayOneShot (audio02.clip);
					attackTime = 0;
				}

			} else if (animationState.fullPathHash == Animator.StringToHash ("Base Layer.Die")) {

				animator.SetBool ("Die", false);
				speed = 0f;

			} else {
			
				//Debug.Log ("引数なし");
				speed = 1.8f;
				//HR.SetActive (false);
				//HL.SetActive (false);
				onlyAttackTime = 0;

			}

		} else { 
			Destroy (gameObject);
		}
	}

	//弾衝突後処理
	private void OnCollisionEnter (Collision collider){
		if (collider.gameObject.tag == "Shot") {

			/*
			//消滅処理
			//クリーンに生成されない為却下
			//Destroy後Instantiateはエラー
			//備忘録・消さない
			Destroy (gameObject);
			Instantiate (explosion, transform.position, transform.rotation);
			*/

			//ダメージランダム設定の時は有効化
			//damage = Random.Range (50, 150);

			damage = collider.gameObject.GetComponent<Shot> ().damage;

			//ダメージ処理
			armorPoint -= damage;
			//Debug.Log (armorPoint);

			lastAttackTime = 0f;

			//動的処理負担軽減のため却下
			/*
			//Jabによるダメージ
			if (collider.gameObject.tag == "Jab") {
				int jabDamage = UnityEngine.Random.Range (450, 550);
				armorPoint -= jabDamage;

				lastAttackTime = 0f;
			}
			*/

		}

	}

	/*
	private void OnCollisionStay (Collision collider){
		//Jabによるダメージ
		if (collider.gameObject.tag == "Jab") {
			int jabDamage = UnityEngine.Random.Range (450, 550);
			armorPoint -= jabDamage;

			lastAttackTime = 0f;
		}
	
	}
	*/

	//消滅時処理
	void OnDestroy(){
		Destroy (marker);
	}

	//パンチ用コルーチン
	//距離を測定をするため敵側スクリプトで処理
	IEnumerator Jab (){
		//Debug.Log (armorPoint);
		lastAttackTime = 0f;
		jabInterval = 0f;
		yield return new WaitForSeconds (0.3f);
		armorPoint -= jabDamage;
		audio01.Play ();
	}
}