using Fusion;
using UnityEngine;

public class PlayerColor : NetworkBehaviour
{
    [Networked] public Color playerColor { get; set; }
    public MeshRenderer render;
}
