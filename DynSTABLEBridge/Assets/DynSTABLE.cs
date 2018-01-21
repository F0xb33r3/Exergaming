using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynSTABLE : MonoBehaviour {

	public int samplePeriod = 60;

	public struct CenterOfPressure {
		public float x;
		public float z;
		public float force;
		public System.DateTime time;
	};

	public DynSTABLE.CenterOfPressure cop = new DynSTABLE.CenterOfPressure {
		x = 0.0f,
		z = 0.0f,
		force = 0.0f,
		time = System.DateTime.Now
	};

	private DynSTABLE.CenterOfPressure lastCop = new DynSTABLE.CenterOfPressure {
		x = 0.0f,
		z = 0.0f,
		force = 0.0f,
		time = System.DateTime.Now
	};

	public DynSTABLE.CenterOfPressure[] copHistory = new DynSTABLE.CenterOfPressure[100];

	void NewDFlowData(DFlowNetwork network) {
		DynSTABLE.CenterOfPressure newCop = new DynSTABLE.CenterOfPressure {
			x = network.getOutput (0),
			z = network.getOutput (1),
			force = network.getOutput (2),
			time = System.DateTime.Now
		};

		int timeToLastCop = lastCop.time.Subtract (copHistory [0].time).Milliseconds;
		int timeToNewCop = newCop.time.Subtract (copHistory [0].time).Milliseconds;

		if (timeToNewCop > samplePeriod) {
			float transition = (samplePeriod - timeToLastCop) / (float)samplePeriod;

			DynSTABLE.CenterOfPressure linearCop = new DynSTABLE.CenterOfPressure {
				x = lastCop.x + (newCop.x - lastCop.x) * transition,
				z = lastCop.z + (newCop.z - lastCop.z) * transition,
				force = lastCop.force + (newCop.force - lastCop.force) * transition,
				time = System.DateTime.Now
			};

			//System.Array.Copy (copHistory, 0, copHistory, 1);
			copHistory [0] = linearCop;
		}

		cop = newCop;
	}
		
	// Use this for initialization
	void Start () {
		DFlowNetwork.OnNewData += NewDFlowData;

		int a = 50;
		int b = 70;

		float c = (float)a / b;
		Debug.Log (c);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
