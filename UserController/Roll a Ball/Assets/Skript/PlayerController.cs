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
	private float fCurrentPos;
	private float fPlanedPos;

	private float fStartPos;
	private bool bJumpUp;

	private enum Direction {None, LeaningLeft, LeaningRight, Jumping, JumpingOnLeftLeg, JumpingOnRightLeg, BalancingLeftLeg, BalancingRightLeg};
	int CurrentDirection;

    void Start () //run before randering a frame
    {
        rb = GetComponent<Rigidbody>();
		CurrentDirection = (int)Direction.None;
		if (speed > fSizeOfOnePath && speed > fJumpHigh) {
			//TODO Value Error
		}
    }

    void FixedUpdate () //run before perform phisical cacluations
	{	
		//setting of aim

		// Balance on one Leg
		if(Input.GetKey (KeyCode.RightArrow) && Input.GetKey (KeyCode.DownArrow) && CurrentDirection == (int)Direction.None){
			//TODO
			CurrentDirection = (int)Direction.LeaningRight;
		} else if(Input.GetKey (KeyCode.LeftArrow) && Input.GetKey (KeyCode.DownArrow) && CurrentDirection == (int)Direction.None){
			//TODO
			CurrentDirection = (int)Direction.LeaningLeft;
		} else
			
		// Jumping
		if(Input.GetKey (KeyCode.RightArrow) && Input.GetKey (KeyCode.UpArrow) && CurrentDirection == (int)Direction.None){
			//TODO
			CurrentDirection = (int)Direction.JumpingOnRightLeg;
		} else if(Input.GetKey (KeyCode.LeftArrow) && Input.GetKey (KeyCode.UpArrow) && CurrentDirection == (int)Direction.None){
			//TODO
			CurrentDirection = (int)Direction.JumpingOnLeftLeg;
		} else if (Input.GetKey (KeyCode.UpArrow) && CurrentDirection == (int)Direction.None) {// jump +y axis
			moveHorizontal = moveVertical = 0.0f;
			fCurrentPos =  rb.position.z;
			fPlanedPos = fCurrentPos + fJumpHigh;
			fStartPos = rb.position.z;
			bJumpUp = true;
			fStartTime = Time.fixedTime;
			CurrentDirection = (int)Direction.Jumping;
		} else
		
		// Leaning
		if (Input.GetKey (KeyCode.RightArrow) && CurrentDirection == (int)Direction.None) {// move right +z axis
			moveHigh = moveVertical = 0.0f;
			fCurrentPos =  rb.position.z;
			fPlanedPos = fCurrentPos + fSizeOfOnePath;
			fStartTime = Time.fixedTime;
			CurrentDirection = (int)Direction.LeaningRight;

		} else if (Input.GetKey (KeyCode.LeftArrow) && CurrentDirection == (int)Direction.None) { // move left -z axis (only horizontal)
			moveHigh = moveVertical = 0.0f;
			fCurrentPos =  rb.position.z;
			fPlanedPos = fCurrentPos - fSizeOfOnePath;
			fStartTime = Time.fixedTime;
			CurrentDirection = (int)Direction.LeaningLeft;
		} 
			

		//check Timeout
		if(fCurrentTime-fStartTime > fTimeOut && CurrentDirection != (int)Direction.None){
			//TODO error
			return;	
		}


		// Balancing on One Leg
		if (Input.GetKey (KeyCode.RightArrow) && Input.GetKey (KeyCode.DownArrow) && CurrentDirection == (int)Direction.BalancingRightLeg) {
			fCurrentTime = Time.fixedTime;
			//TODO Function
		} else if (Input.GetKey (KeyCode.LeftArrow) && Input.GetKey (KeyCode.DownArrow) && CurrentDirection == (int)Direction.BalancingLeftLeg) {
			fCurrentTime = Time.fixedTime;
			//TODO Function
		} else
			
		//Jumping 
		if (Input.GetKey (KeyCode.RightArrow) && Input.GetKey (KeyCode.UpArrow) && CurrentDirection == (int)Direction.JumpingOnRightLeg) {
			fCurrentTime = Time.fixedTime;
			//TODO Function
		} else if (Input.GetKey (KeyCode.LeftArrow) && Input.GetKey (KeyCode.UpArrow) && CurrentDirection == (int)Direction.JumpingOnLeftLeg) {
			fCurrentTime = Time.fixedTime;
			//TODO Function
		} else if (Input.GetKey (KeyCode.UpArrow) && CurrentDirection == (int)Direction.Jumping) {
			fCurrentTime = Time.fixedTime;
			Jump ();
		} else 		
				
		// Leaning
		if (Input.GetKey (KeyCode.RightArrow) && CurrentDirection == (int)Direction.LeaningRight) {
			fCurrentTime = Time.fixedTime;
			LeanRight ();
		} else if (Input.GetKey (KeyCode.LeftArrow) && CurrentDirection == (int)Direction.LeaningLeft) {
			fCurrentTime = Time.fixedTime;
			LeanLeft ();
		} else 
					
 		// prefent falling
		if (CurrentDirection != (int)Direction.None){
			//balanced lost -> do error or give time to get balance back TODO
		}
		return;
	}

	void CheckBalance(){//TODO
		//user does have to be in a stable positon before the next inpus is evaluated
		/*while(!Balance){
		 * wait...
		}*/
	}

	void LeanLeft(){ // move left -z axis (only horizontal)
		//core function
		fCurrentPos = rb.position.z;
		if (fCurrentPos > fPlanedPos) { //going left has to get smaller
			//get balance values for adaptive settings  TODO

			//update position
			moveHorizontal = Input.GetAxis("Horizontal");
			Vector3 movement = new Vector3 (moveHorizontal, moveHigh, moveVertical);
			rb.transform.position += movement*speed;
		} else {
			CurrentDirection = (int)Direction.None;
			CheckBalance();
		}
		return;
	}

	void LeanRight(){ // move right +z axis
		//core function
		fCurrentPos = rb.position.z; 

		if (fCurrentPos < fPlanedPos) { //going left has to get bigger
			//get balance values for adaptive settings afterwards TODO

			//update position
			moveHorizontal = Input.GetAxis("Horizontal");
			Vector3 movement = new Vector3 (moveHorizontal, moveHigh, moveVertical);
			rb.transform.position += movement*speed;
		} else {
			CurrentDirection = (int)Direction.None;
			CheckBalance();
		}
		return;
	}

	void Jump(){ // jump +y axis
		//core function
		fCurrentPos = rb.position.z; 

		//jump up
		if (fCurrentPos < fPlanedPos && bJumpUp) { 
			//get balance values for adaptive settings afterwards TODO

			//update position
			moveHigh = Input.GetAxis ("Vertical");
			Vector3 movement = new Vector3 (moveHorizontal, moveHigh, moveVertical);
			rb.transform.position += movement*speed;
		} else {
			bJumpUp = false;
		} 

		//fall down
		if (fCurrentPos > fStartPos && !bJumpUp) {
			//get balance values for adaptive settings afterwards TODO

			//update position
			moveHigh = Input.GetAxis("Vertical")*(-1);
			Vector3 movement = new Vector3 (moveHorizontal, moveHigh, moveVertical);
			rb.transform.position += movement*speed;
		} else {
			CurrentDirection = (int)Direction.None;
			CheckBalance();				
		}
	}  
}
