using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour {

	public float destroyTime = 0.1f;

	// Use this for initialization
	void Start () {

		//自動消滅
		Destroy (this.gameObject, destroyTime);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
