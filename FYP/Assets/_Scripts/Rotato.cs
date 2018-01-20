using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotato : MonoBehaviour {

    public Vector3 rotationVector;

	// Use this for initialization
	void Start () {
        Vector3 initialY = new Vector3(0, 90, 0);
        transform.Rotate(initialY);
    }
	
	// Update is called once per frame
	void Update () {

        Debug.Log("Rotate");
        transform.Rotate(rotationVector * Time.deltaTime);
	}
}
