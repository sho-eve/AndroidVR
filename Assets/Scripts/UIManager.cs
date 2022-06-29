using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	//Joycon入力をInputManagerで管理できないため、カーソルを生成しタイトル画面操作

	private float iconSpeed = Screen.width * 0.3f;

	private RectTransform rect;
	private Vector2 offset;

	//Joycon用変数配置
	private static readonly Joycon.Button[] button = Enum.GetValues( typeof( Joycon.Button ) ) as Joycon.Button[];
	private List<Joycon> joycons;
	private Joycon joyconL;
	private Joycon joyconR;
	private Joycon.Button? pressedButtonR;

	public bool aPressed;

	void Start () {

		//Joyconインスタンス取得
		joycons = JoyconManager.Instance.j;

		//無いとエラー
		if (joycons == null || joycons.Count <= 0) {
			return;
		}

		//左右識別
		joyconL = joycons.Find (c => c.isLeft);
		joyconR = joycons.Find (c => !c.isLeft);

		rect = GetComponent<RectTransform>();

		//オフセット値をアイコンのサイズの半分で設定
		offset = new Vector2(rect.sizeDelta.x / 2f, rect.sizeDelta.y / 2f);
	}

	void Update () {

		//Joycon初期値
		pressedButtonR = null;

		//ボタン割り当て
		foreach (var button in button) {
			if (joyconR.GetButton (button)) {
				pressedButtonR = button;
			}
		}

		//スティック情報取得
		float stickLHorizontal = joyconL.GetStick () [0];
		float stickLVertical = joyconL.GetStick () [1];

		if (BattleManager.battleStatus == BattleManager.titleState) {
			//Aボタン入力判定
			if (joyconR.GetButtonDown (Joycon.Button.DPAD_RIGHT)) {
				aPressed = true;
			} else {
				aPressed = false;
			}
		} else {
			aPressed = false;
		}

		//Debug.Log (aPressed);

		//カーソル移動量計算
		Vector2 position = rect.anchoredPosition + new Vector2 (stickLHorizontal * iconSpeed, stickLVertical * iconSpeed) * Time.deltaTime;

		//カーソル範囲設定
		position.x = Mathf.Clamp(position.x, -Screen.width * 0.5f + offset.x, Screen.width * 0.5f - offset.x);
		position.y = Mathf.Clamp(position.y, -Screen.height * 0.5f + offset.y, Screen.height * 0.5f - offset.y);

		//カーソル位置更新
		rect.anchoredPosition = position;
	}
}
