using Fusion;
using System.Collections;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    bool _spawned = false;
    public enum GameState
    {
        NULL,
        STARTING,
        PLAYING,
        FINISH,
    }
    public static GameState CurrentGameState { get; private set; } = GameState.NULL;

    private void Awake()
    {
        SetNextState(GameState.STARTING);


    }

    public override void Spawned()
    {
        print("Waiting for spawning");

        SetNextState(GameState.PLAYING);
    }
    public override void FixedUpdateNetwork()
    {
       
    }
    private void SetNextState(GameState nextState)
    {
        var oldState = CurrentGameState;
        switch (nextState)
        {

            case GameState.STARTING:
                break;
            case GameState.PLAYING:

                if (Object.HasStateAuthority)//only host
                {

                    //FindObjectOfType<PlayerSpawner>().StartSpawning(this);
                }
                break;
            case GameState.FINISH:
                break;
        }
    }
    private void OnDestroy()
    {
        CurrentGameState = GameState.NULL;
    }
}
