using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LSL;
using Assets.LSL4Unity.Scripts;
using System.Linq;

public class core2 : MonoBehaviour
{
    public GameObject cubeYellowPrefab;
    public GameObject cubeBluePrefab;
    public Transform[] spawnPoints;
    public Transform[] spawnPointsBlack;
    public TextMeshPro gameTimeText;
    public float gameTime;
    public GameObject cubeBlackPrefab;
    private LSLMarkerStream Spawn_marker;
    private liblsl.StreamOutlet outlet;
    private liblsl.StreamInfo spawnRateInfo;
    private liblsl.StreamOutlet spawnRateOutlet;
    private float lastSpawnTime;
    private liblsl.StreamInfo scoreInfo;
    private liblsl.StreamOutlet scoreOutlet;
    float spawnRate = 2.5f;

    //float increaseSpawnRate = 0.15f;
    //float increaseSpawnRateInterval = 30f;

    private int cubesDestroyedSession;
    int score;
    private CountDown countdownTimer;
    private PointerController pointerController;
    private PointerControllerBlue pointerControllerBlue;

    void Start()
    {
        Spawn_marker = FindObjectOfType<LSLMarkerStream>();
        var info = new liblsl.StreamInfo("CubeSpawnData", "Markers", 4, 0, liblsl.channel_format_t.cf_float32, "SpawnID");
        outlet = new liblsl.StreamOutlet(info);
        // Create a new StreamInfo object for the spawn rate
        spawnRateInfo = new liblsl.StreamInfo("SpawnRate", "SpawnRate", 1, 0, liblsl.channel_format_t.cf_float32, "SpawnRateID");
        // Create a new StreamOutlet object for the spawn rate
        spawnRateOutlet = new liblsl.StreamOutlet(spawnRateInfo);
        // Create a new StreamInfo object for the score
        scoreInfo = new liblsl.StreamInfo("CubeDestroyScore", "Markers", 1, 0, liblsl.channel_format_t.cf_float32, "ScoreID");
        // Create a new StreamOutlet object for the score
        scoreOutlet = new liblsl.StreamOutlet(scoreInfo);

        InvokeRepeating("SpawnYellow", 1f, spawnRate);
        InvokeRepeating("SpawnBlue", 2f, spawnRate);
        InvokeRepeating("SpawnBlack", 5f, 7f);
        //InvokeRepeating("IncreaseSpawnRate", increaseSpawnRateInterval, increaseSpawnRateInterval);
        
        score = 0;
        Invoke("EndExperiment", 45f);  // Call EndExperiment initially


    }

    private void Update()
    {
        gameTime -= Time.deltaTime;
        if (gameTime < 1)
        {
            gameTime = 0;
        }
        gameTimeText.text = gameTime.ToString();
        // Stream spawn rate
        if (outlet != null && Time.time > lastSpawnTime + 1f / spawnRate)
        {
            float[] spawnRateSample = { spawnRate };
            spawnRateOutlet.push_sample(spawnRateSample);
            lastSpawnTime = Time.time;
        }

        // Stream score
        if (scoreOutlet != null)
        {
            float[] scoreSample = { score };
            scoreOutlet.push_sample(scoreSample);
        }
        // Reset countdown timer at the end of each session
        if (gameTime <= 0)
        {
            countdownTimer.ResetTimer();
        }

    }
    public void ResetTimer()
    {
         gameTime = 45f;
         gameTimeText.text = gameTime.ToString();
    }

    void IncreaseSpawnRate()
    {
        if (cubesDestroyedSession < 10)
        {
            spawnRate *= 0.85f; // Increase spawn rate by 15%
        }
        else if (cubesDestroyedSession > 20)
        {
            spawnRate *= 1.15f; // Decrease spawn rate by 15%
        }

        cubesDestroyedSession = 0;

        //CancelInvoke("SpawnYellow");
        //CancelInvoke("SpawnBlue");
        //InvokeRepeating("SpawnYellow", spawnRate, spawnRate);
       // InvokeRepeating("SpawnBlue", spawnRate, spawnRate);
    }

    void EndSession()
    {
        // Delete all cubes
        GameObject[] remainingCubesYellow = GameObject.FindGameObjectsWithTag("yellow");
        GameObject[] remainingCubesBlue = GameObject.FindGameObjectsWithTag("blue");
        GameObject[] remainingCubesBlack = GameObject.FindGameObjectsWithTag("black");

        List<GameObject> remainingCubesList = new List<GameObject>();
        remainingCubesList.AddRange(remainingCubesYellow);
        remainingCubesList.AddRange(remainingCubesBlue);
        remainingCubesList.AddRange(remainingCubesBlack);

        foreach (GameObject cube in remainingCubesList)
        {
            Destroy(cube);
            cubesDestroyedSession++;
        }
        ResetScore();
        pointerController.ResetScore();
        pointerControllerBlue.ResetScore();
        ResetTimer();

        
        // Adjust spawn rate based on cubesDestroyedSession
        if (cubesDestroyedSession < 10)
        {
            spawnRate *= 0.85f; // Increase spawn rate by 15%
        }
        else if (cubesDestroyedSession > 20)
        {
            spawnRate *= 1.15f; // Decrease spawn rate by 15%
        }

        cubesDestroyedSession = 0;

        // Restart the session
        Invoke("IncreaseSpawnRate", 0f);
        //InvokeRepeating("SpawnYellow", 1f, spawnRate);
        //InvokeRepeating("SpawnBlue", 2f, spawnRate);
        //InvokeRepeating("SpawnBlack", 5f, 7f);
        Invoke("EndSession", 45f); // Start another 45-second session
    }


    void ResetScore()
    {
        score = 0;
    }

    public void IncrementCubesDestroyed()
    {
        cubesDestroyedSession++;
    }

    public void SpawnYellow()
    {
        Spawn_marker.Write("yellow spawned");
        GameObject cubeYellow = Instantiate(cubeYellowPrefab) as GameObject;
        cubeYellow.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
        float[] sample = { cubeYellow.transform.position.x, cubeYellow.transform.position.y, cubeYellow.transform.position.z, 0 };
        if (outlet != null)
            outlet.push_sample(sample);
    }

    public void SpawnBlue()
    {
        GameObject cubeBlue = Instantiate(cubeBluePrefab) as GameObject;
        Spawn_marker.Write("blue spawned");
        cubeBlue.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
        float[] sample = { cubeBlue.transform.position.x, cubeBlue.transform.position.y, cubeBlue.transform.position.z, 1 };
        if (outlet != null)
            outlet.push_sample(sample);
    }

    public void SpawnBlack()
    {
        GameObject cubeBlack = Instantiate(cubeBlackPrefab) as GameObject;
        cubeBlack.transform.position = spawnPointsBlack[Random.Range(0, spawnPointsBlack.Length)].transform.position;
        Spawn_marker.Write("black spawned");
        float[] sample = { cubeBlack.transform.position.x, cubeBlack.transform.position.y, cubeBlack.transform.position.z, 2 };
        if (outlet != null)
            outlet.push_sample(sample);
    }
    public void IncrementScore()
    {
        score++;
    }

}