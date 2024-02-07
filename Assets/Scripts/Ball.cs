using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class Ball : NetworkBehaviour
{
    [Networked] private TickTimer life { get; set; }
    [SerializeField] private Renderer _renderer;

    public void Init(Color playerColor)
    {
        life = TickTimer.CreateFromSeconds(Runner, 1.0f);
        _renderer.material.color = playerColor;
    }

    public override void FixedUpdateNetwork()
    {

        if (life.Expired(Runner))
            Runner.Despawn(Object);
        else
            transform.position += 250 * transform.forward * Runner.DeltaTime;


    }
}
