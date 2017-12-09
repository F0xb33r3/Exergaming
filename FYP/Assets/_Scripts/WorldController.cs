using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {

    public GameObject[] platformLayout = new GameObject[6];

    private int[] trackers = new int[3];
    private int currentTracker = 0;
    private int nextTracker = 1;
    private int farTracker = 0;

    //Destroys track off screen
    public float killPoint;

    //Sections of track
    private int TRACK_SIZE = 3;
    private GameObject[] trackPiece = new GameObject[3];
    private GameObject currentSection;
    private GameObject nextSection;
    private GameObject farSection;

    //Scrolling speed of the platforms
    public float speed;
    public float minSpeed;
    public float maxSpeed;

    private Vector3 scrollSpeed;

    //Where the next section will spawn
    public float spawnPoint;
    public float spawnPointFar;

    //Random number gen
    System.Random rand = new System.Random();
    public int minRand;
    public int maxRand;

    //Obstacles
    public GameObject[] blockade = new GameObject[3];
 
    private int score = 2;
    public int difficulty;

    void Start()
    {

        //initialise the first platforms
        currentSection = Instantiate(platformLayout[currentTracker], new Vector3(0, 0, 2.0f), transform.rotation) as GameObject;
        nextSection = Instantiate(platformLayout[nextTracker], new Vector3(0, 0, spawnPoint), transform.rotation) as GameObject;
        farSection = Instantiate(platformLayout[farTracker], new Vector3(0, 0, spawnPointFar), transform.rotation) as GameObject;

        //Set up the track array
        trackPiece[0] = currentSection;
        trackPiece[1] = nextSection;
        trackPiece[2] = farSection;

        //Initialises the first set of obstacles
        ObstacleSpawn(nextTracker);

        //Set up the trackers array
        trackers[0] = currentTracker;
        trackers[1] = nextTracker;
        trackers[2] = farTracker;

        scrollSpeed = new Vector3(0.0f, 0.0f, speed);
    }

    void Update()
    {
        //Sets speed of track
        DifficultySetting();

        //move the track
        for (int i = 0; i < TRACK_SIZE; i++)
        {
            ScrollingWorld(trackPiece[i]);
        }

        UpdateTrack();
    }


    void UpdateTrack()
    {
        //Creates a looping track
        for (int i = 0; i < TRACK_SIZE; i++)
        {
            if (trackPiece[i].transform.position.z <= killPoint)
            {
                Destroy(trackPiece[i]);
                //If the track piece is a base piece the next one will be a random exercise
                if (trackers[i] == 0)
                {
                    trackers[i] = rand.Next(minRand, maxRand + 1);
                    trackPiece[i] = Instantiate(platformLayout[trackers[i]], new Vector3(0, 0, spawnPointFar), transform.rotation) as GameObject;
                    //Set up the track pieces
                    switch (trackers[i])
                    {
                        case 1:
                            ObstacleSpawn(i);
                            break;
                        case 2:
                            OneLegSpawn(i);
                            break;
                        case 3:
                            JumpSpawn(i);
                            break;
                        default:
                            break;
                    }
                }
                //If the track piece is an exercise, the next one will be a base piece
                else if(trackers[i] > 0)
                {
                    trackers[i] = 0;
                    trackPiece[i] = Instantiate(platformLayout[trackers[i]], new Vector3(0, 0, spawnPointFar), transform.rotation) as GameObject;
                }
            }
        }

    }
    void ObstacleSpawn(int val)
    {
        switch (difficulty)
        {
            case 1:
                trackPiece[val].transform.Find("ObstacleMid").Translate(rand.Next(-4, 5), 2, 0);
                break;
            case 2:
                trackPiece[val].transform.Find("ObstacleFront").Translate(rand.Next(-4, 5), 2, 0);
                trackPiece[val].transform.Find("ObstacleBack").Translate(rand.Next(-4, 5), 2, 0);
                break;
            case 3:
                trackPiece[val].transform.Find("ObstacleFront").Translate(rand.Next(-4, 5), 2, 0);
                trackPiece[val].transform.Find("ObstacleMid").Translate(rand.Next(-4, 5), 2, 0);
                trackPiece[val].transform.Find("ObstacleBack").Translate(rand.Next(-4, 5), 2, 0);
                break;
            default:
                break;
        }
        trackPiece[val].transform.Find("ObstacleFront").localScale = trackPiece[val].transform.Find("ObstacleFront").localScale * difficulty * 0.75f;
        trackPiece[val].transform.Find("ObstacleMid").localScale = trackPiece[val].transform.Find("ObstacleMid").localScale * difficulty * 0.75f;
        trackPiece[val].transform.Find("ObstacleBack").localScale = trackPiece[val].transform.Find("ObstacleBack").localScale * difficulty * 0.75f;
    }
    void OneLegSpawn(int val)
    {
        //Randomly pick a side
        int side = rand.Next(1, 3);
        if (side == 1)
        {
            //Right
            trackPiece[val].transform.Find("OLMid").Translate(-3, 0, 0);
        }
        else if (side == 2)
        {
            //Left
            trackPiece[val].transform.Find("OLMid").Translate(3, 0, 0);
        }

    }
    void JumpSpawn(int val)
    {
        if (difficulty == 1)
        {
            trackPiece[val].transform.Find("WallMid").Translate(0, 3.5f, 0);
        }
        else
        {
            trackPiece[val].transform.Find("WallFront").Translate(0, 3.5f, 0);
            trackPiece[val].transform.Find("WallBack").Translate(0, 3.5f, 0);
        }
         
    }
    void DifficultySetting()
    {
    
    }
    void ScrollingWorld(GameObject g)
    {
        g.transform.position -= scrollSpeed;
    }
}
