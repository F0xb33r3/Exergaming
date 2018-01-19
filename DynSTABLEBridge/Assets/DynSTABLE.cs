using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynSTABLE : MonoBehaviour {

	public float a = 0.0f;
	public float b = 0.0f;

	public float c = 1.0f;

	void NewDFlowData(DFlowNetwork network) {
		a = network.getOutput(0);
		b = network.getOutput(1);

		Debug.Log (a + " " + b);

		network.setInput (0, c);
	}
		
	// Use this for initialization
	void Start () {
		DFlowNetwork.OnNewData += NewDFlowData;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
