using Fusion;
using TMPro;
using UnityEngine;


public class PlayerDataNetworked : NetworkBehaviour
{

    [HideInInspector, Networked, OnChangedRender(nameof(OnNickNameChange))] public NetworkString<_16> NickName { get; private set; }

    [SerializeField] private TMP_Text _nameLabel;

    [HideInInspector, Networked] public int Score { get; private set; }
    [HideInInspector, Networked] public int KillCount { get; private set; }
    [HideInInspector, Networked] public int DeathCount { get; private set; }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            var nickname = PlayerInputDataManager.Instance.GetNickName();
            RPC_SendNickName(nickname);
            Score = 0;
        }
        OnNickNameChange();


    }

    [Rpc(RpcSources.All, targets: RpcTargets.StateAuthority)]
    private void RPC_SendNickName(string nickname)
    {
        if (string.IsNullOrEmpty(nickname)) return;

        NickName = nickname;
    }
    private void OnNickNameChange()
    {
        _nameLabel.text = NickName.ToString();
    }
}
