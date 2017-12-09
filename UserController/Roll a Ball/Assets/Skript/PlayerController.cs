using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float speed;
	public float fJumpHigh;
	public float fSizeOfOnePath;
	public float fTimeOut;

	private float fCurrentTime;
	private float fStartTime;

    private Rigidbody rb;
	private float moveHorizontal, moveHigh, moveVertical;

    void Start () //run before randering a frame
    {
        rb = GetComponent<Rigidbody>();
		if (speed > fSizeOfOnePath && speed > fJumpHigh) {
			//TODO Value Error
		}
    }

    void FixedUpdate () //run before perform phisical cacluations
	{		
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			Jump ();
			return;
		} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
			Duck();
			return;
		} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
			GoRight ();
			return;
		} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			GoLeft();
			return;
		} else {/*
			Vector3 movement = new Vector3 (speed, 0.0f, 0.0f);
			rb.AddForce (movement);*/
		}
	}

	void CheckBalance(){//TODO
		//user does have to be in a stable positon before the next inpus is evaluated
		/*while(!Balance){
		 * wait...
		}*/
	}

	void GoLeft(){ // move left -z axis (only horizontal)
		moveHigh = moveVertical = 0.0f;
		float fCurrentPos =  rb.position.z;
		float fPlanedPos = fCurrentPos - fSizeOfOnePath;

		fStartTime = Time.fixedTime;

		while (Input.GetKeyDown (KeyCode.LeftArrow)) {
			//check Timeout
			fCurrentTime = Time.fixedTime;
			if(fCurrentTime-fStartTime > fTimeOut){
				//TODO error
				return;	
			}

			//core function
			fCurrentPos = rb.position.z;
			if (fCurrentPos > fPlanedPos) { //going left has to get smaller
				//get balance values for adaptive settings  TODO

				//update position
				moveHorizontal = Input.GetAxis("Horizontal");
				Vector3 movement = new Vector3 (moveHorizontal, moveHigh, moveVertical);
				rb.AddForce (movement*speed);
			} else {
				CheckBalance();
				return;
			}
		}
		if(fCurrentPos > fSizeOfOnePath){
			//balanced lost -> do error or give time to get balance back TODO
		}
	}

	void GoRight(){ // move right +z axis
		moveHigh = moveVertical = 0.0f;
		float fCurrentPos =  rb.position.z;
		float fPlanedPos = fCurrentPos + fSizeOfOnePath;

		fStartTime = Time.fixedTime;

		while (Input.GetKeyDown (KeyCode.RightArrow)) {
			//check Timeout
			fCurrentTime = Time.fixedTime;
			if(fCurrentTime-fStartTime > fTimeOut){
				//TODO error
				return;	
			}

			//core function
			fCurrentPos = rb.position.z; 
			if (fCurrentPos < fPlanedPos) { //going left has to get bigger
				//get balance values for adaptive settings afterwards TODO

				//update position
				moveHorizontal = Input.GetAxis("Horizontal");
				Vector3 movement = new Vector3 (moveHorizontal, moveHigh, moveVertical);
				rb.AddForce (movement*speed);
			} else {
				CheckBalance();
				return;
			}
		}
		if(fCurrentPos < fSizeOfOnePath){
			//balanced lost -> do error or give time to get balance back TODO
		}		
	}

	void Jump(){ // jump +y axis
		moveHorizontal = moveVertical = 0.0f;

		float fCurrentPos =  rb.position.z;
		float fPlanedPos = fCurrentPos + fJumpHigh;
		float fStartPos = rb.position.z;
		bool bJumpUp = true;

		fStartTime = Time.fixedTime;

		while (Input.GetKeyDown (KeyCode.UpArrow)) {
			//check Timeout
			fCurrentTime = Time.fixedTime;
			if(fCurrentTime-fStartTime > fTimeOut){
				//TODO error
				return;	
			}

			//core function
			fCurrentPos = rb.position.z; 

			//jump up
			if (fCurrentPos < fPlanedPos && bJumpUp) { 
				//get balance values for adaptive settings afterwards TODO

				//update position
				moveHigh = Input.GetAxis ("Vertical");
				Vector3 movement = new Vector3 (moveHorizontal, moveHigh, moveVertical);
				rb.AddForce (movement * speed);
			} else {
				bJumpUp = false;
			} 

			//fall down
			if (fCurrentPos > fStartPos && !bJumpUp) {
				//get balance values for adaptive settings afterwards TODO

				//update position
				moveHigh = Input.GetAxis("Vertical")*(-1);
				Vector3 movement = new Vector3 (moveHorizontal, moveHigh, moveVertical);
				rb.AddForce (movement*speed);
			} else {
				CheckBalance();
				return;
			}
		}
		if(fCurrentPos < fSizeOfOnePath){
			//balanced lost -> do error or give time to get balance back TODO
		}
	}

	void Duck () { // lean back do not anything solely change animation??
		//TODO
	}   
}
