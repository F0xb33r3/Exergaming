using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceVisualization : MonoBehaviour {

	public DynSTABLE platform;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 scale = transform.localScale;
		scale.y = 6 * platform.cop.force;

		Vector3 position = transform.localPosition;
		position.y = 3 * platform.cop.force;

		position.x = 10 * platform.cop.x;
		position.z = -10 * platform.cop.z;

		transform.localScale = scale;
		transform.position = position;
	}
}
