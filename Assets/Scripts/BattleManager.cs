using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class BattleManager : MonoBehaviour {

	//現在のゲームの状態を保存
	public static int battleStatus;

	//ゲーム状態
	public const int battleStart = 0;
	public const int battlePlay = 1;
	public const int battleEnd = 2;
	public const int titleState = 3;
	public const int resultState = 4;
	public const int licenseState = 5;
	public const int howPlayState = 6;

	//タイマーを用いた制御用
	private float timer;

	public Text start;
	public Text timeUp;
	public Text exhausted;

	private float battleTimer;
	private float battleTimerMax = 120f;

	public Text timerText;

	public Canvas battle;
	public Canvas title;
	public Canvas result;
	public Canvas license;

	//Canvas上限数のためゲームオブジェクトを使用　
	public GameObject howPlay;

	// Use this for initialization
	void Start () {

		//初期画面タイトル
		battleStatus = titleState;

		//画面遷移用タイマー
		timer = 0f;

		start.enabled = false;
		timeUp.enabled = false;
		exhausted.enabled = false;
		
	}
	
	// Update is called once per frame
	void Update () {

		//ステータスによって表示する項目を変更
		switch (battleStatus) {

		case titleState:

			//表示キャンバス
			result.enabled = false;
			battle.enabled = false;
			license.enabled = false;
			howPlay.SetActive (false);
			title.enabled = true;

			break;

		case battleStart:

			//表示文字
			exhausted.enabled = false;
			start.enabled = true;

			//バトルタイマーリセット
			battleTimer = battleTimerMax;

			//表示キャンバス
			battle.enabled = true;
			title.enabled = false;

			//シーン用タイマー管理
			timer += Time.deltaTime;

			if (timer > 1.5) {
				start.enabled = false;
				battleStatus = battlePlay;
				timer = 0;
			}

			break;

		case battlePlay:

			//バトル用タイマー設定
			battleTimer = Mathf.Clamp (battleTimer, 0, battleTimerMax);
			battleTimer -= Time.deltaTime;

			//timeup条件
			if (battleTimer <= 0) {
				battleStatus = battleEnd;
				timeUp.enabled = true;
			}

			break;

		case battleEnd:

			//バトル用タイマー誤差修正
			battleTimer = 0.00f;

			//シーン用タイマー管理
			timer += Time.deltaTime;

			if (timer >= 3f) {
				timeUp.enabled = false;
				exhausted.enabled = false;
				timer = 0f;
				battleStatus = resultState;
			}

			break;

		case resultState:

			//表示キャンバス
			//遷移用スクリプトはCharacter.cs
			battle.enabled = false;
			title.enabled = false;
			result.enabled = true;
			
			break;

		case licenseState:

			//表示キャンバス
			//遷移用スクリプトはCharacter.cs
			title.enabled = false;
			license.enabled = true;

			break;

		case howPlayState:

			//表示キャンバス
			//遷移用スクリプトはCharacter.cs
			title.enabled = false;
			howPlay.SetActive (true);

			break;

		}

		//バトル中のタイマー小数点第二位まで表示
		timerText.text = battleTimer.ToString ("N2");

		//シーン遷移用スクリプトはJoyconインスタンス取得済みのCharacter.csで　
	}
}
