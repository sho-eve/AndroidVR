using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

	public Text first;
	public Text second;
	public Text third;

	private bool scoreUpdate;

	private Color initialColor;

	// Use this for initialization
	void Start () {

		initialColor = first.color;
		
	}
	
	// Update is called once per frame
	void Update () {

		//バトル終了時とタイトルのどちらからシーン遷移したか区別
		if (BattleManager.battleStatus == BattleManager.battleEnd) {
			scoreUpdate = true;
		}

		//バトル修了後遷移した場合はハイスコア更新、更新したスコアは赤文字化
		//更新されなかった時はシーン遷移元区別変数だけ変更
		if (BattleManager.battleStatus == BattleManager.resultState && scoreUpdate == true) {
			
			if (Character.score >= PlayerPrefs.GetInt ("1st")) {

				PlayerPrefs.SetInt ("3rd", PlayerPrefs.GetInt ("2nd"));
				PlayerPrefs.SetInt ("2nd", PlayerPrefs.GetInt ("1st"));
				PlayerPrefs.SetInt ("1st", Character.score);
				PlayerPrefs.Save ();
				first.color = Color.red;
				scoreUpdate = false;

			} else if (Character.score >= PlayerPrefs.GetInt ("2nd")) {

				PlayerPrefs.SetInt ("3rd", PlayerPrefs.GetInt ("2nd"));
				PlayerPrefs.SetInt ("2nd", Character.score);
				PlayerPrefs.Save ();
				second.color = Color.red;
				scoreUpdate = false;

			} else if (Character.score >= PlayerPrefs.GetInt ("3rd")) {
				
				PlayerPrefs.SetInt ("3rd", Character.score);
				PlayerPrefs.Save ();
				third.color = Color.red;
				scoreUpdate = false;

			} else {
				
				scoreUpdate = false;

			}
		}

		//赤文字時以外は最初の文字カラーを適用
		//タイトル時に設定
		if (BattleManager.battleStatus == BattleManager.titleState) {

			first.color = initialColor;
			second.color = initialColor;
			third.color = initialColor;

		}

		//PlayerPrefsをString化
		first.text = PlayerPrefs.GetInt ("1st").ToString ();
		second.text = PlayerPrefs.GetInt ("2nd").ToString ();
		third.text = PlayerPrefs.GetInt ("3rd").ToString ();

		//タイトル画面遷移処理はCharacter.cs
		//スコア初期化処理もCharacter.cs
		//joyconインスタンス取得済みのスクリプトで行う
	}
}
