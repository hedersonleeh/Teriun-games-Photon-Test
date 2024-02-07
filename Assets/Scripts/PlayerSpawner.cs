using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;

//Only Host can use this script
public class PlayerSpawner : SimulationBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] private GameObject _playerPrefab;

    private void SpawnPlayer(PlayerRef player)
    {
        // Create a unique position for the player

        Vector3 spawnPosition = new Vector3();
        switch (player.PlayerId % 4)
        {
            case 0:
                spawnPosition = new Vector3(20, 0, 0);
                break;
            case 1:
                spawnPosition = new Vector3(0, 0, 20);
                break;
            case 2:
                spawnPosition = new Vector3(-20, 0, 0);
                break;
            case 3:
                spawnPosition = new Vector3(0, 0, -20);
                break;
        }
        var networkPlayerObject = Runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
        // Set Player Object to facilitate access across systems.
        //Runner.SetPlayerObject(player, networkPlayerObject);

    }

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            SpawnPlayer(player);

        }
    }

    public void PlayerLeft(PlayerRef player)
    {

        // Reset Player Object
        Runner.SetPlayerObject(player, null);

    }
}
