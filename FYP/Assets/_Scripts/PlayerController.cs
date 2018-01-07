using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

	private float fAproximatly;

	private enum Direction {None, LeaningLeft, LeaningRight, Jumping, JumpingOnLeftLeg, JumpingOnRightLeg, BalancingLeftLeg, BalancingRightLeg};
	int CurrentDirection;

    void Start () //run before randering a frame
    {
        rb = GetComponent<Rigidbody>();
		CurrentDirection = (int)Direction.None;
		if (speed > fSizeOfOnePath && speed > fJumpHigh) {
			//TODO Value Error
		}
		fAproximatly = Mathf.Pow(10,-5); 
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
			fCurrentPos =  rb.position.y;
			fPlanedPos = fCurrentPos + fJumpHigh;
			fStartPos = rb.position.y;
			bJumpUp = true;
			fStartTime = Time.realtimeSinceStartup;
			CurrentDirection = (int)Direction.Jumping;
		} else
		
		// Leaning
		if (Input.GetKey (KeyCode.RightArrow) && CurrentDirection == (int)Direction.None) {// move right +z axis
			moveHigh = moveVertical = 0.0f;
			fCurrentPos =  rb.position.x;
			fPlanedPos = fCurrentPos + fSizeOfOnePath;
			fStartTime = Time.realtimeSinceStartup;
			CurrentDirection = (int)Direction.LeaningRight;

		} else if (Input.GetKey (KeyCode.LeftArrow) && CurrentDirection == (int)Direction.None) { // move left -z axis (only horizontal)
			moveHigh = moveVertical = 0.0f;
			fCurrentPos =  rb.position.x;
			fPlanedPos = fCurrentPos - fSizeOfOnePath;
			fStartTime = Time.realtimeSinceStartup;
			CurrentDirection = (int)Direction.LeaningLeft;
		} 
					
		//check Timeout
		fCurrentTime = Time.realtimeSinceStartup;
		if(fCurrentTime-fStartTime > fTimeOut && CurrentDirection != (int)Direction.None){
			//TODO error
			return;	
		}
			
		// Balancing on One Leg
		if (Input.GetKey (KeyCode.RightArrow) && Input.GetKey (KeyCode.DownArrow) && CurrentDirection == (int)Direction.BalancingRightLeg) {
			fStartTime = Time.realtimeSinceStartup;
			//TODO Function
		} else if (Input.GetKey (KeyCode.LeftArrow) && Input.GetKey (KeyCode.DownArrow) && CurrentDirection == (int)Direction.BalancingLeftLeg) {
			fStartTime = Time.realtimeSinceStartup;
			//TODO Function
		} else
			
		//Jumping 
		if (Input.GetKey (KeyCode.RightArrow) && Input.GetKey (KeyCode.UpArrow) && CurrentDirection == (int)Direction.JumpingOnRightLeg) {
			fStartTime = Time.realtimeSinceStartup;
			//TODO Function
		} else if (Input.GetKey (KeyCode.LeftArrow) && Input.GetKey (KeyCode.UpArrow) && CurrentDirection == (int)Direction.JumpingOnLeftLeg) {
			fStartTime = Time.realtimeSinceStartup;
			//TODO Function
		} else if (Input.GetKey (KeyCode.UpArrow) && CurrentDirection == (int)Direction.Jumping) {
			fStartTime = Time.realtimeSinceStartup;
			Jump ();
		} else 		
				
		// Leaning
		if (Input.GetKey (KeyCode.RightArrow) && CurrentDirection == (int)Direction.LeaningRight) {
			fStartTime = Time.realtimeSinceStartup;
			LeanRight ();
		} else if (Input.GetKey (KeyCode.LeftArrow) && CurrentDirection == (int)Direction.LeaningLeft) {
			fStartTime = Time.realtimeSinceStartup;
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
		fCurrentPos = rb.position.x;
		if (fCurrentPos > fPlanedPos) { //going left has to get smaller
			//get balance values for adaptive settings  TODO

			//update position
			moveHorizontal = Input.GetAxis("Horizontal");
			Vector3 movement = new Vector3 (moveHorizontal, moveHigh, moveVertical);
			movement *= speed;

			if (Val1SmallerOrAlmostEqual(fCurrentPos + movement.x, fPlanedPos)) {				//do not overstep
				movement.Set(fPlanedPos, rb.position.y, rb.position.z);
				rb.transform.position = movement;					
			} else {
				rb.transform.position += movement;
			}
		} else {
			//TODO?? display somethink that this task is done???
			CurrentDirection = (int)Direction.None;
			CheckBalance();
		}
		return;
	}

	void LeanRight(){ // move right +z axis
		//core function
		fCurrentPos = rb.position.x; 

		if (fCurrentPos < fPlanedPos) { //going left has to get bigger
			//get balance values for adaptive settings afterwards TODO

			//update position
			moveHorizontal = Input.GetAxis("Horizontal");
			Vector3 movement = new Vector3 (moveHorizontal, moveHigh, moveVertical);
			movement *= speed;

			if (Val1BigerOrAlmostEqual(fCurrentPos + movement.x, fPlanedPos)) {			//do not overstep
				movement.Set(fPlanedPos, rb.position.y, rb.position.z);
				rb.transform.position = movement;
			} else {
				rb.transform.position += movement;
			}
		} else {
			//TODO?? display something that this task is done???
			CurrentDirection = (int)Direction.None;
			CheckBalance();
		}
		return;
	}

	void Jump(){ // jump +y axis
		//core function
		fCurrentPos = rb.position.y; 

		//jump up
		if (fCurrentPos < fPlanedPos && bJumpUp) { 
			//get balance values for adaptive settings afterwards TODO

			//update position
			moveHigh = Input.GetAxis ("Vertical");
			Vector3 movement = new Vector3 (moveHorizontal, moveHigh, moveVertical);
			movement *= speed;

			if (Val1BigerOrAlmostEqual(fCurrentPos + movement.y, fPlanedPos)) {			//do not overstep
				movement.Set(rb.position.x, fPlanedPos, rb.position.z);
				rb.transform.position = movement;
			} else {
				rb.transform.position += movement;
			}
		} else if(bJumpUp){
			bJumpUp = false;
		} 

		//fall down
		if (fCurrentPos > fStartPos && !bJumpUp) {
			//get balance values for adaptive settings afterwards TODO

			//update position
			moveHigh = Input.GetAxis("Vertical")*(-1);
			Vector3 movement = new Vector3 (moveHorizontal, moveHigh, moveVertical);
			movement *= speed;

			if (Val1SmallerOrAlmostEqual(fCurrentPos + movement.y, fStartPos)) {		//do not overstep
				movement.Set(rb.position.x, fStartPos, rb.position.z);
				rb.transform.position = movement;
			} else {
				rb.transform.position += movement;
			}
		} else if(!bJumpUp) {
			//TODO?? display somethink that this task is done???

			CurrentDirection = (int)Direction.None;
			CheckBalance();				
		}
	}  
		
	bool AlmostEqual(float Val1, float Val2){
		if (Mathf.Abs (Val1 - Val2) <= fAproximatly) return true;
		return false;
	}

	bool Val1BigerOrAlmostEqual(float Val1, float Val2){
		if(Val1 > Val2 || AlmostEqual(Val1,Val2)) return true;
		return false;
	}

	bool Val1SmallerOrAlmostEqual(float Val1, float Val2){
		if(Val1 < Val2 || AlmostEqual(Val1,Val2)) return true;
		return false;
	}
    public void ResetPlayer()
    {
        gameObject.transform.position = new Vector3(0, 1, 0);
    }
}


