using Fusion;
using UnityEngine;

public class PlayerDataNetworked : NetworkBehaviour
{
    [HideInInspector,Networked] public NetworkString<_16> NickName { get; private set; }
    [HideInInspector, Networked] public int Score { get; private set; }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            var nickname = PlayerInputDataManager.Instance.GetNickName();
            RPC_SendNickName(nickname);
        }
        if (Object.HasStateAuthority)
        {
            Score = 0;
        }


    }

    [Rpc(RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RPC_SendNickName(string nickname)
    {
        if (string.IsNullOrEmpty(nickname)) return;

        NickName = nickname;
    }
}
