using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Video;

public class Character : MonoBehaviour {

	//各種変数の設定・代入
	public float speed;
	public float gravity = 20.0f;
	public float cameraRotateSpeed = 3.0f;
	private Vector3 moveDirection = Vector3.zero;

	public Animator animator;

	private Vector3 playerPosition;

	public GameObject shot;
	public GameObject muzzle;
	public GameObject camera;

	private float shotInterval = 0f;
	private float shotIntervalMax = 1.0f;

	public GameObject muzzleFlash;

	public int remainBullet;
	public int initialRemainBullet = 20;
	private int remainBulletMax = 50;

	public Image compassImage;

	public static int armorPoint;
	public int armorPointMax = 3000;

	public int damage;

	public Text armorPointText;

	public Image gaugeImageAP;

	public Text remainBulletText;

	public int staminaPoint;
	public int staminaPointMax = 500;

	public Image gaugeImageS;

	public AudioSource audio01;
	public AudioSource audio02;

	public Text reloadText;
	public Image reloadUI;

	private float appearX, appearZ;
	public GameObject enemy;
	public GameObject appearance;
	private Vector3 appearPosition;

	private float appearIntervalTime;
	private float appearInterval;

	public Image reticle;
	private Vector3 reticleReset;

	private Quaternion cameraReset;

	private bool changeWeapon;

	public GameObject enemy2;

	private float appearIntervalTime2;
	private float appearInterval2;
	public GameObject appearance2;

	private int shotDamage;

	public int layerMask = ~(1 << 9);

	public GameObject apRecover;

	private int maxEnemy = 10;

	public static int score;

	public Text exhausted;

	public Text scoreText;

	//遊び方画面用ステータス
	public VideoPlayer videoOne;
	public VideoPlayer videoTwo;
	public VideoPlayer videoThree;
	public VideoPlayer videoFour;
	public VideoPlayer videoFive;

	private int pushCount;

	//Joycon用変数配置
	private static readonly Joycon.Button[] button = Enum.GetValues( typeof( Joycon.Button ) ) as Joycon.Button[];
	private List<Joycon> joycons;
	private Joycon joyconL;
	private Joycon joyconR;
	private Joycon.Button? pressedButtonL;
	private Joycon.Button? pressedButtonR;

	// Use this for initialization
	void Start () {

		//Joyconインスタンス取得
		joycons = JoyconManager.Instance.j;

		//無いと動かない
		//要確認
		if (joycons == null || joycons.Count <= 0) {
			return;
		}

		//左右識別
		joyconL = joycons.Find( c =>  c.isLeft );
		joyconR = joycons.Find( c => !c.isLeft );

		//Unityちゃんanimator取得
		animator = GetComponent<Animator> ();

		//unityちゃんポジション取得
		playerPosition = GetComponent<Transform> ().position;

		//効果音音源取得
		AudioSource[] audioSources = gameObject.GetComponents<AudioSource> ();
		audio01 = audioSources [0];
		audio02 = audioSources [1];

		//照準初期値
		reticleReset = reticle.transform.position;

		//カメラ初期値
		cameraReset = Camera.main.transform.localRotation;

		//ゲーム開始時倒れた状態になるのを防ぐ体力設定
		armorPoint = armorPointMax;

		changeWeapon = true;

	}

	//移動スピード変更関数
	//移動スピード変更時アニメーション変化処理も必要であればここに入れる
	void SpeedChange () {
		speed = 1f;
	}

	// Update is called once per frame
	void Update () {

		//体力表示
		armorPointText.text = armorPoint + " / " + armorPointMax;

		//残弾表示
		remainBulletText.text = "残弾：" + remainBullet + " / " + remainBulletMax;

		//残存体力による色彩調整
		float percentageArmorpoint = (float)armorPoint / armorPointMax;

		if (percentageArmorpoint > 0.5f) {
			armorPointText.color = Color.white;
			gaugeImageAP.color = Color.white;
		} else if (percentageArmorpoint > 0.2f) {
			armorPointText.color = Color.yellow;
			gaugeImageAP.color = Color.yellow;
		} else {
			armorPointText.color = Color.red;
			gaugeImageAP.color = Color.red;
		}

		//アニメーション取得
		AnimatorStateInfo animationState = animator.GetCurrentAnimatorStateInfo (0);
			
		//ゲージ調整
		gaugeImageAP.transform.localScale = new Vector3 (percentageArmorpoint, 1, 1);

		//スタミナゲージ
		gaugeImageS.transform.localScale = new Vector3 ((float)staminaPoint / staminaPointMax, 1, 1);

		//リロードゲージ
		reloadUI.transform.localScale = new Vector3 ((float)shotInterval / shotIntervalMax, 1, 1);

		//スコアString化
		scoreText.text = score.ToString();

		//バトル状態でキャラクター操作を有効
		if (BattleManager.battleStatus == BattleManager.battlePlay) {

			//前ゲーム時のアニメージョンリセット
			animator.SetBool ("Reset", false);

			//Joycon初期値配置
			pressedButtonL = null;
			pressedButtonR = null;

			//無いと動かない、フリーズ？
			//要確認
			if (joycons == null || joycons.Count <= 0) {
				return;
			}

			//スティック情報取得
			float stickLHorizontal = joyconL.GetStick () [0];
			float stickLVertical = joyconL.GetStick () [1];
			float stickRHorizontal = joyconR.GetStick () [0];
			float stickRVertical = joyconR.GetStick () [1];

			//joyconR傾き取得
			Vector3 eulerOrientation = joyconR.GetVector ().eulerAngles;
			Quaternion orientation = joyconR.GetVector ();
			orientation.x = joyconR.GetVector ().z;
			orientation.z = joyconR.GetVector ().x;

			//ボタン割り当て
			foreach (var button in button) {
				
				if (joyconL.GetButton (button)) {
					
					pressedButtonL = button;

				}

				if (joyconR.GetButton (button)) {
					
					pressedButtonR = button;

				}
			}

			/*
			//Joycon振動テスト
			//使わない時はコメント化
			if (Input.GetKeyDown (KeyCode.L)) {
				joyconL.SetRumble (160, 320, 0.6f, 200);
			}
			if (Input.GetKeyDown (KeyCode.R)) {
				joyconR.SetRumble (160, 320, 0.6f, 200);
			}
			*/
			
			//unityちゃんcontrollerを取得
			CharacterController controller = GetComponent<CharacterController> ();

			//JoyConL ZLボタン入力時スピード変更(後進時を除く（スタミナ要素あり
			if (stickLVertical >= 0f) {
				if (joyconL.GetButton (Joycon.Button.SHOULDER_2) && staminaPoint > 1) {
					speed = 3.0f;
					staminaPoint -= 1;
				} else {
					SpeedChange ();
				}
			} else {
				SpeedChange ();
			}

			//スタミナ回復
			if (!joyconL.GetButton (Joycon.Button.SHOULDER_2)) {
				staminaPoint += 2;
				staminaPoint = Mathf.Clamp (staminaPoint, 0, staminaPointMax);
			}

			//プレイヤーの移動を地面設置時に限定
			if (controller.isGrounded) {

				//JoyConLスティック
				moveDirection = new Vector3 (stickLHorizontal, 0, stickLVertical);
				moveDirection = transform.TransformDirection (moveDirection);
				moveDirection *= speed;

				/*
				//JoyConR Bボタンでジャンプ
				//必要性なしと判断、使う目処がたったら使う
				if (joyconR.GetButtonDown (Joycon.Button.DPAD_DOWN)) {
					StartCoroutine (Jump ());	//コルーチン使用
				}
				*/
				
			}

			//入力情報を反映
			moveDirection.y -= gravity * Time.deltaTime;
			controller.Move (moveDirection * Time.deltaTime);

			/*
			//プレイヤーを回転させる JoyconRジャイロ
			//ジャイロリセット方法が見つかり次第利用
			var playerAngles = transform.localEulerAngles;
			playerAngles.y = eulerOrientation.y;
			transform.localEulerAngles = playerAngles;
			*/

			/*
			//カメラを回転させる　JoyconRジャイロ
			//ジャイロリセット方法が見つかり次第利用
			var angles = CameraParent.transform.localEulerAngles;
			angles.x = eulerOrientation.x;
			CameraParent.transform.localEulerAngles = angles;
			*/

			/*
			//カメラ初期値を用いたジャイロリセット
			//使えない　要修正
			if (joyconL.GetButton (Joycon.Button.SHOULDER_1)) {
				scope.SetActive (true);
				activeCamera.SetActive (false);
			} else {
				scope.SetActive (false);
				activeCamera.SetActive (true);
			}
			*/

			//プレイヤーを回転させる　JoyconRスティック
			transform.Rotate (0, stickRHorizontal * cameraRotateSpeed, 0);

			//カメラを回転させる JoyconRスティック
			GameObject CameraParent = Camera.main.transform.parent.gameObject;
			CameraParent.transform.Rotate (-stickRVertical, 0, 0);

			//アニメーターモーション切り替え
			if (stickLHorizontal > 0) {
				animator.SetInteger ("Horizontal", 1);

			} else if (stickLHorizontal < 0) {
				animator.SetInteger ("Horizontal", -1);

			} else {
				animator.SetInteger ("Horizontal", 0);
			}

			if (stickLVertical > 0) {
				animator.SetInteger ("Vertical", 1);

			} else if (stickLVertical < 0) {
				animator.SetInteger ("Vertical", -1);

			} else {
				animator.SetInteger ("Vertical", 0);

			}
			
			//ジャンプモーション
			//ジャンプ利用しないときはコメント
			//animator.SetBool ("Jump",joyconR.GetButtonDown (Joycon.Button.DPAD_DOWN));

			//ダッシュモーション
			animator.SetBool ("Dush", joyconL.GetButton (Joycon.Button.SHOULDER_2));

			//銃撃モーション
			animator.SetBool ("Gun", joyconR.GetButtonDown (Joycon.Button.SHOULDER_1));

			//弾発射間隔
			shotInterval += Time.deltaTime;
			shotInterval = Mathf.Clamp (shotInterval, 0, shotIntervalMax);

			//ShotIntervalUI
			if (shotInterval < shotIntervalMax) {
				reloadText.text = "Reload";
				reloadText.color = Color.red;
			} else {
				reloadText.text = "Ready";
				reloadText.color = Color.blue;
			}

			//方角の回転
			compassImage.transform.rotation = Quaternion.Euler (compassImage.transform.rotation.x, compassImage.transform.rotation.y, transform.eulerAngles.y);

			//敵出現位置
			//ワールド座標になってしまう為却下
			//広大な座標を用いた毎ゲームランダムに発生するイベントとかも面白いかも
			//一応残しておく
			/*
			appearX = UnityEngine.Random.Range (10, 100);
			appearZ = UnityEngine.Random.Range (10, 100);
			Vector3 randomRange = new Vector3 (appearX, 100, appearZ);
			transform.localPosition = randomRange;
			*/

			//出現間隔
			//歩行型と飛行船型エネミー
			appearIntervalTime += Time.deltaTime;
			appearIntervalTime2 += Time.deltaTime;

			//最大数以下の時出現
			if (GameObject.FindGameObjectsWithTag ("Enemy").Length <= maxEnemy) {

				//敵の出現
				appearPosition = appearance.transform.position;
				appearPosition.y += 10;

				if (appearIntervalTime >= appearInterval) {
					Instantiate (enemy, appearPosition, transform.rotation);
					appearIntervalTime = 0f;
					if (appearInterval > 2.0f) {
						appearInterval -= UnityEngine.Random.Range (0.05f, 0.2f);
					}
				}

				//Debug.Log (appearIntervalTime);

				if (appearIntervalTime2 > appearInterval2) {
					Instantiate (enemy2, appearance2.transform.position, appearance.transform.rotation);
					appearIntervalTime2 = 0;
					appearInterval2 = 10f;
				}
			}


			//Jab JoyconL加速度を使用
			Vector3 accel = joyconL.GetAccel ();

			//平方根計算高負荷の為平方値を使用
			//ダメージ処理はenemy用スクリプトに記述
			if (accel.sqrMagnitude >= 25) {
				animator.SetBool ("Jab", true);
				audio02.Play ();

				//collisionを用いると遅延の為却下
				//使えそうになれば使う
				//jab.SetActive (true);
				//Debug.Log (accel);

			} else {
				animator.SetBool ("Jab", false);
				//jab.SetActive (false);
			}

			//装備武器銃時
			//その他の武器ギミックを追加できるときに編集できるように設定
			//剣装備機能つけたい	//アニメーター未だ見つからず。。。
			if (changeWeapon == true) {

				//照準設定
				//スプラトゥーンに見られるような着弾予想地点に照準が移動する
				Ray ray = new Ray (muzzle.transform.position, Camera.main.transform.parent.forward);
				RaycastHit hit;
				int distance = 20;
				Debug.DrawRay (ray.origin, ray.direction * distance, Color.red);

				if (Physics.Raycast (ray, out hit, 30.0f, layerMask)) {

					//ワールド座標をCanvasスクリーン座標に変換
					reticle.transform.position = RectTransformUtility.WorldToScreenPoint (Camera.main, hit.point);
					string hitTag = hit.collider.tag;

					//照準色変更
					if (hitTag == "Enemy") {
						reticle.color = Color.red;
					} else if (hitTag == "Heart") {
						reticle.color = Color.green;
					} else {
						reticle.color = Color.blue;
					}

				} else { 

					//空向いた時・Ray絵画距離内に衝突物がない時、初期値に設定
					reticle.transform.position = reticleReset;
					reticle.color = Color.blue;

				}

				//弾発射処理
				//処理変更が容易にできるよう段階を分けて条件分岐	//必要なし？
				if (joyconR.GetButtonDown (Joycon.Button.SHOULDER_1)) {
			
					if (shotInterval >= shotIntervalMax) {
				
						if (remainBullet >= 1) {
					
							Instantiate (shot, muzzle.transform.position, camera.transform.rotation);
							shotInterval = 0f;
							Instantiate (muzzleFlash, muzzle.transform.position, transform.rotation);
							remainBullet -= 1;
							joyconR.SetRumble (160, 320, 0.6f, 50);
							audio01.PlayOneShot (audio01.clip);
							//Debug.Log (remainBullet);
						}
					}
				}
			}

			//衝突判定用ダメージ判定
			damage = UnityEngine.Random.Range (200, 300);

			//体力上限
			armorPoint = Mathf.Clamp (armorPoint, 0, armorPointMax);

			//毎フレーム呼び出しで動作するのでこのまま
			//要確認
			animator.SetBool ("Damage", false);

			//体力０以下で強制終了
			if (armorPoint <= 0) {
				animator.SetBool ("Exhausted", true);
				exhausted.enabled = true;
				BattleManager.battleStatus = BattleManager.battleEnd;
			}

			//Debug.Log (score);

		} else {

			//バトル終了時のアニメーション設定
			animator.SetBool ("Damage", false);

			//アニメーターのループを避ける
			if (armorPoint > 0) {
				animator.SetBool ("Reset", true);
				animator.SetBool ("Jab", false);
			} else {
				animator.SetBool ("Exhausted", true);
				if (animationState.fullPathHash == Animator.StringToHash ("Base Layer.Exhausted")) {
					animator.SetBool ("Exhausted", false);
				}
			}

		}

		//バトル開始時各種初期値設定
		if (BattleManager.battleStatus == BattleManager.battleStart) {
			animator.SetBool ("Reset", true);
			//体力設定
			armorPoint = armorPointMax;

			//スタミナ設定
			staminaPoint = staminaPointMax;

			//初期残弾設定
			remainBullet = initialRemainBullet;

			//ゲーム難易度初期値設定
			shotInterval = 0f;

			appearIntervalTime = 7f;
			appearInterval = 10.0f;

			appearIntervalTime2 = 0f;
			appearInterval2 = 40f;

			//スコアリセット
			score = 0;
		}

		//Joycon用インスタンスを取得している永続スクリプトでリザルト・ライセンス・遊び方画面操作
		//スコアランキング画面
		if (BattleManager.battleStatus == BattleManager.resultState) {

			//Bボタンでタイトル遷移
			if (joyconR.GetButtonDown (Joycon.Button.DPAD_DOWN)) {
				BattleManager.battleStatus = BattleManager.titleState;
			}

			//LRボタン同時押しでスコアランキングリセット
			if (joyconR.GetButton (Joycon.Button.STICK) && joyconL.GetButton (Joycon.Button.STICK)) {
				PlayerPrefs.DeleteAll();
			}
		}

		//ライセンス画面
		if (BattleManager.battleStatus == BattleManager.licenseState) {

			//Bボタンでタイトル遷移
			if (joyconR.GetButtonDown (Joycon.Button.DPAD_DOWN)) {
				BattleManager.battleStatus = BattleManager.titleState;
			}
		}

		//タイトル画面時遊び方画面でのカウント数リセット
		if (BattleManager.battleStatus == BattleManager.titleState) {
			pushCount = 0;
		}

		//遊び方画面
		if (BattleManager.battleStatus == BattleManager.howPlayState) {

			//Rボタンでプラスカウント
			if (joyconR.GetButtonDown (Joycon.Button.SHOULDER_1)) {
				pushCount++;
			}

			//Lボタンでマイナスカウント
			if (joyconL.GetButtonDown (Joycon.Button.SHOULDER_1)) {
				if (pushCount > 0) {
					pushCount--;
				}
			}

			//遊び方画面LRボタンで画面遷移
			//カウント数により遷移
			switch (pushCount) {

			case 0:

				videoOne.enabled = true;
				videoTwo.enabled = false;
				videoThree.enabled = false;
				videoFour.enabled = false;
				videoFive.enabled = false;

				break;

			case 1:

				videoOne.enabled = false;
				videoTwo.enabled = true;
				videoThree.enabled = false;

				break;

			case 2:

				videoTwo.enabled = false;
				videoThree.enabled = true;
				videoFour.enabled = false;

				break;

			case 3:

				videoThree.enabled = false;
				videoFour.enabled = true;
				videoFive.enabled = false;

				break;

			case 4:

				videoFour.enabled = false;
				videoFive.enabled = true;

				break;

			case 5:

				BattleManager.battleStatus = BattleManager.titleState;
				//タイトル画面でカウントリセット

				break;

			}

		}

	}


	//衝突判定
	private void OnCollisionEnter (Collision collider) {

		//弾回復
		if (collider.gameObject.tag == "gunrecover") {
			int rbRandom = UnityEngine.Random.Range (3, 5);
			remainBullet += rbRandom;
		}

		//敵の弾衝突でダメージ
		if (collider.gameObject.tag == "ShotEnemy") {
			shotDamage = collider.gameObject.GetComponent<ShotEnemy> ().damage;
			armorPoint -= shotDamage;
			animator.SetBool ("Damage", true);
		}

		/*
		//子オブジェクトとのコライダー判定高負荷のため却下
		//敵と衝突でダメージ
		if (collider.gameObject.tag == "EnemyAttack") {
			armorPoint -= damage;
			Debug.Log (armorPoint);

		}
		*/
			
	}

	//エラー回避用
	private void Hit(){
		//使用したアセットにより呼び出される>無いとエラー
		//パンチアニメーション時呼び出される
		//処理に遅延が生じるため使用却下
	}

}
