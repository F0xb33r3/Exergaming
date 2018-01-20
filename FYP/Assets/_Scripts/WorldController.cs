using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldController : MonoBehaviour {

    public GameObject[] platformLayout = new GameObject[6];
    Camera cam;

    private int[] trackers = new int[3];
    private int currentTracker = 0;
    private int nextTracker = 1;
    private int farTracker = 0;

    bool earthquake = true;

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

    public Text scoreText;
    public Text timerText;
    public Text gameOverText;

    //Obstacles
    public GameObject[] blockade = new GameObject[3];
 
    public double score = 0;

    public double timeLeft = 30;
    public int difficulty;

    public bool gameOver;


    void Start()
    {
        //Initialise camera obj
        GameObject camOb = GameObject.FindWithTag("MainCamera");
        if (camOb != null)
        {
            cam = camOb.GetComponent<Camera>();
        }

        //initialise the first platforms
        currentSection = Instantiate(platformLayout[currentTracker], new Vector3(0, 0, 0.0f), transform.rotation) as GameObject;
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

        gameOver = false;
        timerText.text = "Time: " + timeLeft;

        StartCoroutine(Earthquake());
    }

    void Update()
    {
        for (int i = 0; i < TRACK_SIZE; i++)
        {
            //ScrollingWorld(trackPiece[i]);
            trackPiece[i].transform.position -= scrollSpeed;
        }
        UpdateTrack();
        UpdateScore();
        timer();
        //move the track
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
                        case 4:
                            StepSpawn(i);
                            break;
                        case 5:
                            DifJumpSpawn(i);
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
                //trackPiece[i + 1].transform.position = new Vector3(0.0f, 0.0f, 0.0f);
                //trackPiece[i + 2].transform.position = new Vector3(0.0f, 0.0f, 30.0f);
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
    void StepSpawn(int val)
    {
        if (difficulty == 1)
        {
            
        }
        else
        {
            
        }

    }
    void DifJumpSpawn(int val)
    {
        if (difficulty == 1)
        {
            int side = rand.Next(1,3);
            //spawns the platforms on the same side
            if (side == 1)
            {
                trackPiece[val].transform.Find("Pad2").Translate(0, 1.0f, 0);
                trackPiece[val].transform.Find("Pad4").Translate(0, 1.0f, 0);
            }
            else
            {
                trackPiece[val].transform.Find("Pad3").Translate(0, 1.0f, 0);
                trackPiece[val].transform.Find("Pad5").Translate(0, 1.0f, 0);
            }
        }
        else
        {
            int front = rand.Next(1, 3);
            int back = rand.Next(1, 3);
            //Spawns random pads
            if (front == 1)
            {
                trackPiece[val].transform.Find("Pad2").Translate(0, 1.0f, 0);
            }
            else
            {
                trackPiece[val].transform.Find("Pad3").Translate(0, 1.0f, 0);
            }
            if (back == 1)
            {
                trackPiece[val].transform.Find("Pad4").Translate(0, 1.0f, 0);
            }
            else
            {
                trackPiece[val].transform.Find("Pad5").Translate(0, 1.0f, 0);
            }
        }

    }
    public void UpdateSpeed(float v)
    {
        speed += v;
        if (speed < minSpeed)
        {
            speed = minSpeed;
        }
        else if(speed > maxSpeed)
        {
            speed = maxSpeed;
        }
        scrollSpeed = new Vector3(0.0f, 0.0f, speed);
    }
    void ScrollingWorld(GameObject g)
    {
        g.transform.position -= scrollSpeed;
    }

    public void SetScoreText()
    {
        scoreText.text = "Score: " + score.ToString("F");
    }
    private void SetTimerText()
    {
        timerText.text = "Time: " + timeLeft.ToString("F");
    }

    void timer()
    {
        timeLeft -= Time.deltaTime;
        SetTimerText();
        if (timeLeft < 0)
        {
            //Game over
            gameOver = true;
            timeLeft = 0.00;
            gameOverText.text = "Time is up!";
            SetTimerText();
        }
    }
    void UpdateScore()
    {
        if (!gameOver)
        {
            score += speed * Time.deltaTime;
            SetScoreText();
        } 
    }

    //Flashes 
    IEnumerator Earthquake()
    {
        while(earthquake)
        {
            yield return new WaitForSeconds(rand.Next(5, 10));
            Debug.Log("Shake");
            //Earthquake
            cam.GetComponent<CameraShake>().SetTimer(1.0f);
        }    
    }
}
