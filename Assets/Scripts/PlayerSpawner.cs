using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;

//Only Host can use this script
public class PlayerSpawner : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    private bool _gameHasStated = false;
    public void StartSpawning(GameManager manager)
    {
        foreach (var player in Runner.ActivePlayers)
        {
            SpawnPlayer(player);
        }
        _gameHasStated = true;
    }

    private void SpawnPlayer(PlayerRef player)
    {
        // Create a unique position for the player
        Vector3 spawnPosition = new Vector3((player.RawEncoded % Runner.Config.Simulation.PlayerCount) * 3, 1, 0);
        NetworkObject networkPlayerObject = Runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
        // Set Player Object to facilitate access across systems.
        Runner.SetPlayerObject(player, networkPlayerObject);

        // Keep track of the player avatars for easy access
        _spawnedCharacters.Add(player, networkPlayerObject);
    }

    public void PlayerJoined(PlayerRef player)
    {
        if (Runner.IsServer && _gameHasStated)
        {
            if (!_spawnedCharacters.ContainsKey(player))
                SpawnPlayer(player);
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            Runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);

        }
        // Reset Player Object
        Runner.SetPlayerObject(player, null);

    }
}
