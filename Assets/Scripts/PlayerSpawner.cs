using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;

//Only Host can use this script
public class PlayerSpawner : SimulationBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] private GameObject _playerPrefab;
    private PlayerDataDisplay _displayInfo;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    private void SpawnPlayer(PlayerRef player)
    {
        // Create a unique position for the player
        Vector3 spawnPosition = new Vector3((player.PlayerId % Runner.Config.Simulation.PlayerCount) * 3, 0, 0);
        var networkPlayerObject = Runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
        // Set Player Object to facilitate access across systems.
        //Runner.SetPlayerObject(player, networkPlayerObject);
        _displayInfo = FindObjectOfType<PlayerDataDisplay>();
        _displayInfo.AddPlayerInfo(player);

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
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            Runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
            _displayInfo.RemovePlayerFromUI(player);

        }
        // Reset Player Object
        Runner.SetPlayerObject(player, null);

    }
}
