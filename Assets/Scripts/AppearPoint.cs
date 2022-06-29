using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearPoint : MonoBehaviour {

	float y;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		//1フレームごとに位置更新
		y = UnityEngine.Random.Range (0, 360);
		transform.Rotate (0, y, 0);
		
	}
}
