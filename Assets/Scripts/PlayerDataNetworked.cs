using Fusion;
using TMPro;
using UnityEngine;


public class PlayerDataNetworked : NetworkBehaviour
{
    [HideInInspector, Networked, OnChangedRender(nameof(OnNickNameChange))] public NetworkString<_16> NickName { get; private set; }

    [SerializeField] private TMP_Text _nameLabel;

    [HideInInspector, Networked, OnChangedRender(nameof(OnStatisticKillChange))] public int KillCount { get; private set; } = -100;
    [HideInInspector, Networked, OnChangedRender(nameof(OnScoreChange))] public int Score { get; private set; } = -100;
    [HideInInspector, Networked, OnChangedRender(nameof(OnStatisticDeathChange))] public int DeathCount { get; private set; } = -100;
    [Networked, OnChangedRender(nameof(OnChangeColor))] public Color playerColor { get; set; }

    private PlayerDataDisplay _display;
    public override void Spawned()
    {
        _display = FindObjectOfType<PlayerDataDisplay>();
        if (Object.HasStateAuthority)
        {
            var nickname = PlayerInputDataManager.Instance.GetNickName();
            RPC_SendNickName(nickname);
            RPC_SendStatistics(0, 0, 0);
            playerColor = PlayerColor.GetPlayerColor(Object.InputAuthority.PlayerId);

        }
        OnChangeColor();
        OnNickNameChange();
        OnStatisticChange();

    }

    [Rpc(RpcSources.All, targets: RpcTargets.StateAuthority)]
    private void RPC_SendNickName(string nickname)
    {
        if (string.IsNullOrEmpty(nickname)) return;

        NickName = nickname;
    }

    [Rpc(RpcSources.All, targets: RpcTargets.StateAuthority)]
    private void RPC_SendStatistics(int killCount, int deathCount, int score)
    {
        KillCount = killCount;
        DeathCount = deathCount;
        Score = score;
    }


    private void OnNickNameChange()
    {
        _nameLabel.text = NickName.ToString();
    }
    private void OnStatisticChange()
    {
        _display.UpdateInfo(Object.StateAuthority, this);

    }
    private void OnChangeColor()
    {
        GetComponent<PlayerColor>()._renderers.ForEach(r => r.material.color = playerColor);
    }
    public void OnStatisticDeathChange()
    {
        OnStatisticChange();
    }
    public void OnScoreChange()
    {
        OnStatisticChange();
    }

    public void OnStatisticKillChange()
    {
        OnStatisticChange();
    }


    internal void AddCointCount(int amount)
    {
        RPC_SendStatistics(KillCount, DeathCount, Score + amount);
    }
    internal void AddKillCount()
    {
        RPC_SendStatistics(KillCount + 1, DeathCount, Score);
    }

    internal void AddDeathCount()
    {
        RPC_SendStatistics(KillCount, DeathCount + 1, Score);
    }
}
