using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System;

public class DFlowNetwork : MonoBehaviour {

	//Remember to fix this befor compiling!
	public string ipAddress = "192.168.178.48";

	//Default port
	public int port = 3910;

	//Will store the assigned client-index
	private int clientIndex = 0;
	public int getClientIndex() {
		return clientIndex;
	}

	private int numberOfOutputs = 0;
	private float[] outputs = new float[256];
	public float[] getOutputs() {
		float[] result = new float[numberOfOutputs];

		for (int i = 0; i < numberOfOutputs; i++) {
			result [i] = outputs [i];
		}

		return result;
	}
	public float getOutput(int index) {
		return outputs[index];
	}

	private int numberOfInputs = 0;
	private float[] inputs  = new float[256];
	public void setInputs(float[] newInputs) {
		for (int i = 0; i < numberOfInputs; i++) {
			inputs [i] = newInputs [i];
		}
	}
	public void setInput(int index, float newValue) {
		inputs[index] = newValue;
	}
	public float[] getInputs() {
		float[] result = new float[numberOfInputs];

		for (int i = 0; i < numberOfInputs; i++) {
			result [i] = inputs [i];
		}

		return result;
	}
	public float getInput(int index) {
		return inputs [index];
	}

	//D-Flow Communication protocol in both directions
	public struct DFlowPackage {
		public enum PackageType {
			CLIENT_INIT = 0,
			SERVER_INIT = 1,
			UPDATE = 2
		};

		public PackageType packageType;
		public int numberInputs;
		public int numberOutputs;
		public int clientIndex;
		public string clientName;
		public float[] data;
	};

	public delegate void NewData (DFlowNetwork network);
	public static event NewData OnNewData;


	Thread NetworkingThread;
	bool runNetworkingThread = false;

	Socket initSocket;
	NetworkStream initStream;
	Socket dataSocket;
	NetworkStream dataStream;


	// Use this for initialization
	void Start () {
		NetworkingThread = new Thread (new ThreadStart (EstablishDFlowConnection));
		NetworkingThread.IsBackground = true;
		runNetworkingThread = true;
		NetworkingThread.Start ();
	}

	DFlowNetwork.DFlowPackage readPackage(NetworkStream stream) {
		
		DFlowNetwork.DFlowPackage newPackage;

		byte[] buffer = new byte[256 * sizeof(float)];

		//Package type
		stream.Read (buffer, 0, sizeof(Int32));
		newPackage.packageType = (DFlowNetwork.DFlowPackage.PackageType) System.BitConverter.ToInt32 (buffer, 0);

		//Number of inputs
		stream.Read (buffer, 0, sizeof(Int32));
		newPackage.numberInputs = System.BitConverter.ToInt32 (buffer, 0);

		//Number outputs
		stream.Read (buffer, 0, sizeof(Int32));
		newPackage.numberOutputs = System.BitConverter.ToInt32 (buffer, 0);

		//Index number
		stream.Read (buffer, 0, sizeof(Int32));
		newPackage.clientIndex = System.BitConverter.ToInt32 (buffer, 0);

		//Client name
		stream.Read (buffer, 0, 128 * sizeof(char));
		newPackage.clientName = Encoding.ASCII.GetString (buffer);

		newPackage.data = new float[256];

		for (int i = 0; i < 256; i++) {
			stream.Read (buffer, 0, sizeof(float));

			if (BitConverter.IsLittleEndian) {
				Array.Reverse (buffer, 0, sizeof(float));
			}

			newPackage.data [i] = System.BitConverter.ToSingle (buffer, 0);
		}
			
		return newPackage;
	}

	private void EstablishDFlowConnection() {

		//Socket to create a connection to initiate session with D-Flow
		initSocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

		Debug.Log ("Trying to establish a connection to D-Flow");

		try {
			initSocket.Connect (ipAddress, port);
		} catch (Exception e) {

			//TODO: Errorhandling
			Debug.Log ("Unable to connecto to D-Flow server. Message: " + e.Message);
			return;
		}

		initStream = new NetworkStream (initSocket);

		DFlowNetwork.DFlowPackage newPackage;
		newPackage.packageType = DFlowPackage.PackageType.UPDATE;
		newPackage.clientIndex = 0;
		newPackage.clientName = "";
		newPackage.numberInputs = 0;
		newPackage.numberOutputs = 0;
		newPackage.data = new float[256];

		while (runNetworkingThread && (clientIndex == 0)) {
			try {
				if (initStream.CanRead) {

					newPackage = readPackage(initStream);

				}
			} catch (Exception err) {
				Debug.Log (err.ToString ());
			}

			Debug.Log (newPackage.packageType);

			if (newPackage.packageType == DFlowPackage.PackageType.SERVER_INIT) {
				clientIndex = newPackage.clientIndex;
				numberOfInputs = newPackage.numberInputs;
				numberOfOutputs = newPackage.numberOutputs;
			}
		}

		Debug.Log ("Successfully connected to D-Flow server with ClientIndex " + clientIndex + ".");

		try {
			initStream.Close ();
			initSocket.Shutdown (SocketShutdown.Both);
			initSocket.Close ();
		} catch (Exception e) {
			Debug.Log (e.Message);
		}

		Debug.Log ("Will now open D-Flow stream.");

		initSocket.Close ();

		dataSocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

		try {
			dataSocket.Connect (ipAddress, port + clientIndex);
			Thread.Sleep(1000);
		} catch (SocketException e) {

			//TODO: Errorhandling
			Debug.Log ("Unable to connec to to D-Flow stream. Message: " + e.Message);
			return;
		}

		Debug.Log ("Successfully connected to D-Flow stream.");

		dataStream = new NetworkStream (dataSocket);

		while (runNetworkingThread) {
			try {
				if (dataStream.CanRead) {

					newPackage = readPackage(dataStream);

				}
			} catch (Exception err) {
				Debug.Log (err.ToString ());
			}

			if (newPackage.packageType == DFlowPackage.PackageType.UPDATE) {
				
				Array.Copy (newPackage.data, outputs, 256);

				Debug.Log (outputs [0] + " " + outputs [1]);

				OnNewData (this);

			} else {
				Debug.Log ("Unexpected communication package.");
			}

			if (dataStream.CanWrite) {
				byte[] buffer = new byte[16 + 256 + 256 * sizeof(float)];

				dataStream.Write (buffer, 0, 272);

				for (int i = 0; i < 256; i++) {
					byte[] valueToByte = BitConverter.GetBytes (inputs [i]);

					if (BitConverter.IsLittleEndian) {
						Array.Reverse (valueToByte, 0, sizeof(float));
					}

					dataStream.Write(valueToByte, 0, sizeof(float));
				}
			}

		}
	}
	
	// Update is called once per frame
	void Update () {
		//StartCoroutine (test());
	}

	void OnApplicationQuit() {
		Debug.Log ("Shutting down D-Flow connection");

		runNetworkingThread = false;

		NetworkingThread.Abort ();

		try {
			if (initSocket.Connected) {
				initStream.Close();
				initSocket.Shutdown(SocketShutdown.Both);
				initSocket.Close();
			}

			if (dataSocket.Connected) {
				dataStream.Close();
				dataSocket.Shutdown(SocketShutdown.Both);
				dataSocket.Close();
			}
		} catch (Exception e) {
			Debug.Log ("Shutdown of D-Flow connection failed. Message: " + e.Message);
		}
	}
}
