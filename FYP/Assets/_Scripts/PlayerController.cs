using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    private Rigidbody rb;
    
    private enum Side { Left, Middle, Right};
    int CurrentSide;

    private bool inTheAir = false; 

    CollisionManager collisionManager;

    private float movementX = 3.5f;
    private float movementY = 1.4f;
    private float correction = 0.3f;
    private float resetValue = 3.0f;  

    void Start () //run before randering a frame
    {
        rb = GetComponent<Rigidbody>();
        collisionManager = GetComponent<CollisionManager>();
        CurrentSide = (int)Side.Middle; 
    }

  
    void Update() //run before perform phisical cacluations
	{
        //setting of aim


        CheckSides();

        if (Input.GetKeyDown(KeyCode.RightArrow) && !(CurrentSide == (int)Side.Right)){
            rb.AddForce( Vector3.right * movementX, ForceMode.Impulse);

        } else if (Input.GetKeyDown(KeyCode.LeftArrow) && !(CurrentSide == (int)Side.Left))
        {
            rb.AddForce(Vector3.left * movementX, ForceMode.Impulse);
        }
     
        if (Input.GetKey(KeyCode.UpArrow) && (collisionManager.bJump == true || collisionManager.bJump2 == true) && !inTheAir)
        {
            rb.AddForce(Vector3.up * movementY, ForceMode.VelocityChange);
        }

        CheckSides();
        ResetPosition();

    }

	void CheckBalance(){//TODO
		//user does have to be in a stable positon before the next input is evaluated
		/*while(!Balance){
		 * wait...
		}*/
	} 

    void CheckSides()
    {
        if (rb.position.x >= (movementX))
        {
            CurrentSide = (int)Side.Right;
        }
        else if (rb.position.x < movementX - correction || rb.position.x > -movementX + correction)
        {
            CurrentSide = (int)Side.Middle;
        }
        else if (rb.position.x <= (-movementX))
        {
            CurrentSide = (int)Side.Left;
        }

        if (rb.position.y >= 1.5f)
        {
            inTheAir = true;
        }
        else
        {
            inTheAir = false;
        }

    }

    void ResetPosition()
    {

        if (rb.position.x > resetValue + correction)
        {
            rb.position = new Vector3(movementX, rb.position.y, 0);
        }

        if (rb.position.x < -resetValue - correction)
        {
            rb.position = new Vector3(-movementX, rb.position.y, 0);
        }
    }

    
    public void ResetPlayer()
    {
        gameObject.transform.position = new Vector3(0, 1, 0);
    }
}


