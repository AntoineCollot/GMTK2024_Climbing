using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public Transform player { get; private set; }
    int playerLastFrameFloor;
    int playerMaxFloor;

    public bool gameIsOver { get; private set; }
    public bool gameHasStarted { get; private set; }
    public bool isInCutscene;
    public bool GameIsPlaying => !gameIsOver && gameHasStarted && !isInCutscene;
    public bool autoStart = true;

    public enum PlayMode { Infinite, Build }
    public PlayMode playMode;

    public class FloorEvent : UnityEvent<int, int> { }
    public FloorEvent onPlayerChangeFloor = new FloorEvent();

    public UnityEvent onGameStart = new UnityEvent();
    public UnityEvent onLoopToStart = new UnityEvent();
    public UnityEvent onGameOver = new UnityEvent();
    public UnityEvent onGameWin = new UnityEvent();

    public static GameManager Instance;

    public int PlayerCurrentFloor => Tower.GetFloorOfAltitude(player.position.y);

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        Instance = this;
        if (autoStart)
            StartGame();
    }

    private void Update()
    {
        int newPlayerFloor = PlayerCurrentFloor;
        playerMaxFloor = Mathf.Max(newPlayerFloor, playerMaxFloor);

        //Floor change
        if (newPlayerFloor != playerLastFrameFloor)
        {
            onPlayerChangeFloor.Invoke(playerLastFrameFloor, newPlayerFloor);
            playerLastFrameFloor = newPlayerFloor;
        }
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

    public void LoopToStart()
    {
        if (!gameIsOver)
            onLoopToStart.Invoke();
    }
}
