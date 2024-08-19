using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public Transform player { get; private set; }
    int playerMaxFloor;

    public bool gameIsOver { get; private set; }
    public bool gameHasStarted { get; private set; }
    public bool GameIsPlaying => !gameIsOver && gameHasStarted;
    public bool autoStart = true;

    public UnityEvent onGameStart = new UnityEvent();
    public UnityEvent onGameOver = new UnityEvent();
    public UnityEvent onGameWin = new UnityEvent();

    public static GameManager Instance;

    public int PlayerCurrentFloor
    {
        get
        {
            float altitude = player.position.y;
            altitude /= Tower.FLOOR_HEIGHT;
            return Mathf.FloorToInt(altitude);
        }
    }

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        Instance = this;
        if (autoStart)
            StartGame();
    }

    private void LateUpdate()
    {
        playerMaxFloor = Mathf.Max(PlayerCurrentFloor, playerMaxFloor);
    }

    public void StartGame()
    {
        if (gameHasStarted)
            return;
        gameHasStarted = true;
        onGameStart.Invoke();
    }

    public void GameOver()
    {
        if (gameIsOver)
            return;
        gameIsOver = true;
        onGameOver.Invoke();
    }

    public void ClearLevel()
    {
        if (gameIsOver)
            return;

        gameIsOver = true;
        onGameWin.Invoke();
    }


}
