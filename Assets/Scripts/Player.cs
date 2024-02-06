using Fusion;
using TMPro;
using UnityEngine;
using static InputHandler;

public class Player : NetworkBehaviour
{
    private NetworkCharacterController _cc;
    [SerializeField] private TMP_Text _nameLabel;
    [SerializeField] private Ball _prefabBall;
    [SerializeField] private PhysxBall _prefabPhysxBall;
    [Networked] public bool spawnedProjectile { get; set; }
    [Networked] private TickTimer delay { get; set; }
    private Vector3 _forward = Vector3.forward;
    private ChangeDetector _changeDetector;
    public Material material;
    private TMP_Text _messages;
    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }
    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
        _forward = transform.forward;
        material = GetComponentInChildren<MeshRenderer>().material;
        _nameLabel.text = PlayerInputDataManager.Instance.GetNickName();
    }
    private void Update()
    {
        //if(Object.HasInputAuthority && Input.GetKeyDown(KeyCode.R))
        //{
        //    RPC_SendMessage("Hey Mate!");
        //}
    }
    [Rpc(RpcSources.InputAuthority,RpcTargets.StateAuthority,HostMode =RpcHostMode.SourceIsHostPlayer)]
    private void RPC_SendMessage(string message,RpcInfo info = default)
    {
        RPC_RelayMessage(message, info.Source);
    }
    [Rpc(RpcSources.StateAuthority,RpcTargets.All,HostMode =RpcHostMode.SourceIsServer)]
    private void RPC_RelayMessage(string message, PlayerRef source)
    {
        if (_messages == null)
            _messages = FindObjectOfType<TMP_Text>();

        if(source == Runner.LocalPlayer)
        {
            message = $"You said: {message}\n";
        }
        else
        {
            message = $"Some other player said: {message}\n";
        }
        _messages.text += message;
    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(spawnedProjectile):
                    material.color = Color.white;
                    break;
            }
        }
        material.color = Color.Lerp(material.color, Color.blue, Time.smoothDeltaTime);

    }
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            _cc.Move(5 * data.direction * Runner.DeltaTime);

            if (data.direction.sqrMagnitude > 0)
                _forward = data.direction;
            if (HasStateAuthority && delay.ExpiredOrNotRunning(Runner))
            {

                if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
                {
                    delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                    Runner.Spawn(_prefabBall,
                        transform.position + _forward, Quaternion.LookRotation(_forward), Object.InputAuthority,
                        (runner, o) =>
                        {
                            o.GetComponent<Ball>().Init();
                        });
                    spawnedProjectile = !spawnedProjectile;

                }
                else if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON1))
                {
                    delay = TickTimer.CreateFromSeconds(Runner, 0.1f);

                    Runner.Spawn(_prefabPhysxBall,
                       transform.position + _forward,
                       Quaternion.LookRotation(_forward),
                       Object.InputAuthority,
                       (runner, o) =>
                       {
                           o.GetComponent<PhysxBall>().Init(10 * _forward);
                       });
                    spawnedProjectile = !spawnedProjectile;

                }

            }

        }
    }
}
