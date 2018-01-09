using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{

    WorldController gameController;
    Camera cam;

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

    }
        

}