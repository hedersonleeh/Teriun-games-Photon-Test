using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataDisplay : NetworkBehaviour
{
    [SerializeField] GameObject _uiInfoPrefab;
    [SerializeField] RectTransform _header;
    private Dictionary<PlayerRef, GameObject> _playerRegister = new Dictionary<PlayerRef, GameObject>();
    public override void Spawned()
    {
        if (HasStateAuthority == false) return;
    }
    public void UpdateInfo(int killcount, int deathCount)
    {

    }
    public void AddPlayerInfo(PlayerRef player)
    {
        _playerRegister.Add(player, Instantiate(_uiInfoPrefab, _header));
    }
    public void RemovePlayerFromUI(PlayerRef player)
    {
        if (_playerRegister.TryGetValue(player, out var obj))
        {
            Destroy(obj);
        };
        _playerRegister.Remove(player);

    }
}
