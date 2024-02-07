﻿using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColor : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnChangeColor))] public Color playerColor { get; set; }

    public List<Renderer> _renderers;
    private void OnChangeColor()
    {

        _renderers.ForEach(r => r.material.color = playerColor);
    }
    public override void Spawned()
    {

        if (HasStateAuthority)
            playerColor = GetPlayerColor(Object.InputAuthority.PlayerId);

        OnChangeColor();
        

    }
    public static Color GetPlayerColor(int id)
    {
        switch (id % 8)
        {
            case 0: return Color.black;
            case 1: return Color.red;
            case 2: return Color.blue;
            case 3: return Color.green;
            case 4: return Color.yellow;
            case 5: return Color.cyan;
            case 6: return Color.grey;
            case 7: return Color.magenta;
            case 8: return Color.white;
        }
        return Color.black;
    }

}
