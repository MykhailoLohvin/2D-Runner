using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateGame : MonoBehaviour
{

    Vector3 startPositionForGround = new Vector3(-8.5f, -3.5f, 0);
    private float stepForGround = 1.0f;

    Vector3 startPositionForBackGround = new Vector3(0.0f, 0.0f, 0.0f);
    private float stepForBackGround = 19.2f;

    Transform creatorPosition;

    Random rnd;
    int previousGroundLevel = 0;

    int distanceBetweenTraps = 4;

    [SerializeField]
    Camera mainCamera;

    void Awake()
    {
        GenerateGameScene();

        creatorPosition = mainCamera.GetComponentsInChildren<Transform>()[2];
    }

    private void Update()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        if (creatorPosition.position.x > startPositionForBackGround.x)
        {
            Instantiate(Resources.Load("Background"), startPositionForBackGround, new Quaternion(0f, 0f, 0f, 0f));
            startPositionForBackGround.x += stepForBackGround;
        }
        if (creatorPosition.position.x > startPositionForGround.x)
        {
            Vector3 groundPosition = GenerateGroundLevel();

            for (int i = 0; i < 4; i++)
            {
                Instantiate(Resources.Load("Ground"), groundPosition, new Quaternion(0f, 0f, 0f, 0f));

                distanceBetweenTraps--;

                if (distanceBetweenTraps <= 0)
                {
                    if (SetSawTrap() && groundPosition.y != 2.5f)
                    {
                        Instantiate(Resources.Load("Saw"), new Vector3(groundPosition.x, groundPosition.y + 0.5f, 0.0f), new Quaternion(0f, 0f, 0f, 0f));

                        distanceBetweenTraps = 4;
                    }
                }

                startPositionForGround.x += stepForGround;
                groundPosition.x += stepForGround;
            }
        }
    }

    Vector3 GenerateGroundLevel()
    {
        Vector3 result = startPositionForGround;

        float level = Random.Range(0.0f, 1.1f);

        if (level <= 0.5f)
        {
            previousGroundLevel = 0;
            return result;
        }
        else
            if (level <= 0.75f)
        {
            previousGroundLevel = 1;
            return new Vector3(startPositionForGround.x, startPositionForGround.y + 3.0f, 0.0f);
        }
        else
            if (previousGroundLevel == 1 || previousGroundLevel == 2)
        {
            previousGroundLevel = 2;
            return new Vector3(startPositionForGround.x, startPositionForGround.y + 6.0f, 0.0f);
        }
        else
            return GenerateGroundLevel();
    }

    bool SetSawTrap()
    {
        return Random.Range(0.0f, 1.0f) > 0.9f;
    }

    private void GenerateGameScene()
    {
        for (int i = 0; i < 18; i++)
        {
            Instantiate(Resources.Load("Ground"), startPositionForGround, new Quaternion(0f, 0f, 0f, 0f));
            startPositionForGround.x += 1f;
        }

        Instantiate(Resources.Load("Character"), new Vector3(0.0f, -2.0f, 0.0f), new Quaternion(0f, 0f, 0f, 0f));

        Instantiate(Resources.Load("Monster"), new Vector3(-5.0f, -2.0f, 0.0f), new Quaternion(0f, 0f, 0f, 0f));

        Instantiate(Resources.Load("Background"), startPositionForBackGround, new Quaternion(0f, 0f, 0f, 0f));
        startPositionForBackGround.x += stepForBackGround;
        Instantiate(Resources.Load("Background"), startPositionForBackGround, new Quaternion(0f, 0f, 0f, 0f));
        startPositionForBackGround.x += stepForBackGround;

        mainCamera.GetComponent<CameraMove>().StartMove();
    }
}
