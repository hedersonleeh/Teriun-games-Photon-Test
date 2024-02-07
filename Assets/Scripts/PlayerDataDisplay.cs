using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDataDisplay : SimulationBehaviour, IPlayerLeft
{
    [SerializeField] GameObject _uiInfoPrefab;
    private RectTransform _header;
    private Dictionary<PlayerRef, GameObject> _playerRegister = new Dictionary<PlayerRef, GameObject>();

    public void UpdateInfo(PlayerRef player, PlayerDataNetworked networked)
    {
        if (!_playerRegister.ContainsKey(player)) AddPlayerInfo(player);

        if (_playerRegister.TryGetValue(player, out var obj))
        {
            obj.transform.Find("ColorLabel/LabelName(TMP)").GetComponent<TMPro.TMP_Text>().text = networked.NickName.ToString();
            obj.transform.Find("KILL Count").GetComponent<TMPro.TMP_Text>().text = "Kill count: " +networked.KillCount;
            obj.transform.Find("DeathCount").GetComponent<TMPro.TMP_Text>().text = "Death count: " + networked.DeathCount;
            obj.transform.Find("CoinCount").GetComponent<TMPro.TMP_Text>().text = "Coint count: " + networked.Score;
            obj.transform.Find("ColorLabel").GetComponent<Image>().color = networked.playerColor;
        }
    }
    public void AddPlayerInfo(PlayerRef player)
    {
        if (!_playerRegister.ContainsKey(player))
        {

            if (_header == null)
            {
                _header = GameObject.Find("Canvas/MainUI/Header").GetComponent<RectTransform>();

            }
            _playerRegister.Add(player, Instantiate(_uiInfoPrefab, _header));

            //StartCoroutine(InitilizeLabelCoroutine(player));
            ////var playerObj = Runner.GetPlayerObject(player);
            ////var data = playerObj.GetComponent<PlayerDataNetworked>();
            //UpdateInfo(player, R.GetComponent<PlayerColor>().playerColor, data.NickName.ToString(), data.KillCount, data.DeathCount);
        }
    }


    public void RemovePlayerFromUI(PlayerRef player)
    {
        if (_playerRegister.TryGetValue(player, out var obj))
        {
            Destroy(obj);
        };
        _playerRegister.Remove(player);

    }

    public void PlayerJoined(PlayerRef player)
    {
        if (_header == null)
        {
            _header = GameObject.Find("Canvas/MainUI/Header").GetComponent<RectTransform>();

        }
        AddPlayerInfo(player);
    }

    public void PlayerLeft(PlayerRef player)
    {
        RemovePlayerFromUI(player);
    }


}
