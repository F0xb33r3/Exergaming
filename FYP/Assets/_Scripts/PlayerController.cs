using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    private Rigidbody rb;

	private enum Direction {None, LeaningLeft, LeaningRight, Jumping, JumpingOnLeftLeg, JumpingOnRightLeg, BalancingLeftLeg, BalancingRightLeg, StepRight, StepUp, StepDown, Stepleft};
    int CurrentDirection;

    CollisionManager collisionManager;

    private float movementX = 2.0f;
    private float movementY = 0.5f; 

    void Start () //run before randering a frame
    {
        rb = GetComponent<Rigidbody>();
		CurrentDirection = (int)Direction.None;
        collisionManager = GetComponent<CollisionManager>(); 
    }

    void FixedUpdate () //run before perform phisical cacluations
	{
        //setting of aim

        // One Leg
        if (Input.GetKeyDown(KeyCode.RightArrow) && collisionManager.bOneLeg == true && !(CurrentDirection == (int)Direction.BalancingRightLeg))
        {
            CurrentDirection = (int)Direction.BalancingRightLeg;
            rb.MovePosition(transform.position += new Vector3(movementX, 0, 0));
            Debug.Log(CurrentDirection);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && collisionManager.bOneLeg == true && !(CurrentDirection == (int)Direction.BalancingLeftLeg))
        {
            CurrentDirection = (int)Direction.BalancingLeftLeg;
            rb.MovePosition(transform.position += new Vector3(-movementX, 0, 0));
            Debug.Log(CurrentDirection);
        }
        else

      // Jumping
      if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.UpArrow) && collisionManager.bJump == true && !(CurrentDirection == (int)Direction.JumpingOnRightLeg))
        {
            CurrentDirection = (int)Direction.JumpingOnRightLeg;
            rb.MovePosition(transform.position += new Vector3(movementX, movementY, 0));
            Debug.Log(CurrentDirection);
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.UpArrow) && collisionManager.bJump == true && !(CurrentDirection == (int)Direction.JumpingOnLeftLeg))
        {
            CurrentDirection = (int)Direction.JumpingOnLeftLeg;
            rb.MovePosition(transform.position += new Vector3(-movementX, movementY, 0));
            Debug.Log(CurrentDirection);
        }
        else if (Input.GetKey(KeyCode.UpArrow) && collisionManager.bJump == true)
        {
            rb.MovePosition(transform.position += new Vector3(0, movementY, 0));
            Debug.Log(CurrentDirection);
        }
        else

        // Jumping 2
      if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.UpArrow) && collisionManager.bJump2 == true && !(CurrentDirection == (int)Direction.JumpingOnRightLeg))
        {
            CurrentDirection = (int)Direction.JumpingOnRightLeg;
            rb.MovePosition(transform.position += new Vector3(movementX, movementY, 0));
            Debug.Log(CurrentDirection);
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.UpArrow) && collisionManager.bJump2 == true && !(CurrentDirection == (int)Direction.JumpingOnLeftLeg))
        {
            CurrentDirection = (int)Direction.JumpingOnLeftLeg;
            rb.MovePosition(transform.position += new Vector3(-movementX, movementY, 0));
            Debug.Log(CurrentDirection);
        }
        else if (Input.GetKey(KeyCode.UpArrow) && collisionManager.bJump2 == true )
        {
            rb.MovePosition(transform.position += new Vector3(0, movementY, 0));
            Debug.Log(CurrentDirection);
        }
        else

      // Leaning
      if (Input.GetKeyDown(KeyCode.RightArrow) && collisionManager.bLean == true && (!(CurrentDirection == (int)Direction.LeaningRight)))
        {
            CurrentDirection = (int)Direction.LeaningRight;
            rb.MovePosition(transform.position += new Vector3(movementX, 0, 0));
            Debug.Log(CurrentDirection);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && collisionManager.bLean == true && (!(CurrentDirection == (int)Direction.LeaningLeft)))
        { 
            CurrentDirection = (int)Direction.LeaningLeft;
            rb.MovePosition(transform.position += new Vector3(-movementX, 0, 0));
            Debug.Log(CurrentDirection);

        }
        else 
        
        //Stepping
        if (Input.GetKey(KeyCode.LeftArrow) && collisionManager.bStep == true && (!(CurrentDirection == (int)Direction.LeaningLeft)))
        {
            Debug.Log(CurrentDirection);
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && collisionManager.bStep == true && (!(CurrentDirection == (int)Direction.LeaningLeft)))
        {
            Debug.Log(CurrentDirection);
        }
        else
        {
            CurrentDirection = (int)Direction.None;
            Debug.Log(CurrentDirection);
        }
        
   
	}

	void CheckBalance(){//TODO
		//user does have to be in a stable positon before the next input is evaluated
		/*while(!Balance){
		 * wait...
		}*/
	} 
		
    public void ResetPlayer()
    {
        gameObject.transform.position = new Vector3(0, 1, 0);
    }
}


