using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour
{
    public Image _lifeBar;
    [Networked, OnChangedRender(nameof(HealthChanged))]
    public float NetworkedHealth { get; set; } = 100;
    public bool IsDead => NetworkedHealth <= 0;
    public NetworkBehaviour LastDealer { get; set; }

    private void Awake()
    {
        _lifeBar.color = Color.Lerp(Color.red, Color.green, 1);

    }
    void HealthChanged()
    {
        Debug.Log($"Health changed to: {NetworkedHealth}");
        _lifeBar.fillAmount = NetworkedHealth / 100f;
        _lifeBar.color = Color.Lerp(Color.red, Color.green, _lifeBar.fillAmount);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void DealDamageRpc(NetworkBehaviour dealer, float damage)
    {
        // The code inside here will run on the client which owns this object (has state and input authority).
        Debug.Log("Received DealDamageRpc on StateAuthority, modifying Networked variable");
        if (!IsDead)
        {
            LastDealer = dealer;
            NetworkedHealth -= damage;
        }
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void ReviveRpc()
    {
        // The code inside here will run on the client which owns this object (has state and input authority).
        Debug.Log("Received Revive  on StateAuthority, modifying Networked variable");
        if (IsDead)
            NetworkedHealth = 100;
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void HealRPC(float heal)
    {
        // The code inside here will run on the client which owns this object (has state and input authority).
        Debug.Log("Received DealDamageRpc on StateAuthority, modifying Networked variable");
        if (!IsDead)
            NetworkedHealth += heal;
    }

    //[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    //public void PlayerDeath()
    //{
    //    GetComponent<Player>().SetPlayerDead();
    //}
}
