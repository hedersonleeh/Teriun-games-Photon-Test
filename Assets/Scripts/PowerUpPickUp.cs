﻿using Fusion;
using UnityEngine;

public class PowerUpPickUp : NetworkBehaviour
{
    public enum PowerUpType
    {
        HEAL,
        MONEY
    }
    [SerializeField] private GameObject _render;
    [SerializeField] private int healAmount;
    [SerializeField] private float _pickUpRadius;
    [SerializeField] private LayerMask _whatIsPlayer;
    [SerializeField] private PowerUpType _typpeOfPickUp;
    [Networked] bool IsActive { get; set; }
    Collider[] _colbuffer;
    TickTimer _respawnDelay;
    public override void Spawned()
    {
        IsActive = true;
        _colbuffer = new Collider[5];
    }
    public override void FixedUpdateNetwork()
    {

        if (IsActive == false)
        {
            if (_respawnDelay.ExpiredOrNotRunning(Runner))
            {
                Spawned();
            }
            return;
        };
        var colisions = Runner.GetPhysicsScene().OverlapSphere(transform.position, _pickUpRadius, _colbuffer, _whatIsPlayer, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < colisions; i++)
        {
            switch (_typpeOfPickUp)
            {
                case PowerUpType.HEAL:
                    if (_colbuffer[i].TryGetComponent<PlayerHealth>(out var playerHealth))
                    {
                        PickUpPowerUp(playerHealth);
                    }
                    break;
                case PowerUpType.MONEY:
                    if (_colbuffer[i].TryGetComponent<PlayerDataNetworked>(out var netWorked))
                    {
                        PickUpPowerUp(netWorked);
                    }
                    break;

            }

        }
    }
    public override void Render()
    {
        _render.gameObject.SetActive(IsActive);
    }
    private void PickUpPowerUp(PlayerHealth playerHealth)
    {
        playerHealth.HealRPC(healAmount);
        IsActive = false;
        _respawnDelay = TickTimer.CreateFromSeconds(Runner, 5.0f);
    }
    private void PickUpPowerUp(PlayerDataNetworked playerData)
    {
        playerData.AddCointCount(healAmount);
        IsActive = false;
        _respawnDelay = TickTimer.CreateFromSeconds(Runner, 10.0f);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _pickUpRadius);
    }
}
