using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{

    WorldController gameController;
    Camera cam;

    public bool bLean = false;
    public bool bOneLeg = false;
    public bool bStep = false;
    public bool bJump = false;
    public bool bJump2 = false;
    public bool bBase = false; 
        
    void Start()
    {
        //access the world controller for updating speed + score
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<WorldController>();
        }
        else if (gameController == null)
        {
            Debug.Log("cannot find 'GameController' script");
        }
        GameObject camOb = GameObject.FindWithTag("MainCamera");
        if (camOb != null)
        {
            cam = camOb.GetComponent<Camera>();
        }
        else if (camOb == null)
        {
            Debug.Log("cannot find 'GameController' script");
        }
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit");
        //Player vs Obstacle
        if (other.gameObject.tag == "Obstacle")
        {
            Debug.Log("explode?");
            Destroy(other.gameObject);
            gameController.UpdateSpeed(-0.025f);
            cam.GetComponent<CameraShake>().SetTimer(1.0f);
            //palpitation TBD

        }
        else if (other.gameObject.tag == "Pit")
        {
            gameObject.GetComponent<PlayerController>().ResetPlayer();
            Debug.Log("Fallen");

            //remove this later lolz
          //  bBase = true; 
        }

        if (other.gameObject.tag == "Lean")
        {
            bLean = true; 
        }
        else if (other.gameObject.tag == "OneLeg")
        {
            bOneLeg = true;
        }
        else if (other.gameObject.tag == "Step")
        {
            bStep = true; 
        }
        else if (other.gameObject.tag == "Jump")
        {
            bJump = true; 
        }
        else if (other.gameObject.tag == "Jump2")
        {
            bJump2 = true; 
        }
        else if (other.gameObject.tag == "Base")
        {
            bBase = true; 
        }

    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Lean")
        {
            Debug.Log("Lean");
        }
        else if (other.gameObject.tag == "OneLeg")
        {
            Debug.Log("OneLeg");
        }
        else if (other.gameObject.tag == "Step")
        {
            Debug.Log("Step");
        }
        else if (other.gameObject.tag == "Jump")
        {
            Debug.Log("Jump");
        }
        else if (other.gameObject.tag == "Jump2")
        {
            Debug.Log("Jump2");
        }
        else if (other.gameObject.tag == "Base")
        {
            Debug.Log("Base");
        }

    }
    //Leaving each platform
    void OnTriggerExit(Collider other)
    {
        Debug.Log("Exit");
        if (other.gameObject.tag == "Lean")
        {
            Debug.Log("Stop Lean");
            bLean = false; 
        }
        else if (other.gameObject.tag == "OneLeg")
        {
            Debug.Log(" Stop OneLeg");
            bOneLeg = false;
        }
        else if (other.gameObject.tag == "Step")
        {
            Debug.Log(" Stop Step");
            bStep = false; 
        }
        else if (other.gameObject.tag == "Jump")
        {
            Debug.Log(" Stop Jump");
            bJump = false; 
        }
        else if (other.gameObject.tag == "Jump2")
        {
            Debug.Log("Stop Jump2");
            bJump2 = false; 
        }
        else if (other.gameObject.tag == "Base")
        {
            Debug.Log("Stop Base");
            bBase = false;
        }
    }

}