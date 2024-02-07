using Fusion;
using TMPro;
using UnityEngine;
using static InputHandler;
using Fusion.Addons.SimpleKCC;
public class Player : NetworkBehaviour
{
    //private Rigidbody _rb;
    private  SimpleKCC _simpleKCC;
    //private CharacterController _cc;
    [SerializeField] private TMP_Text _nameLabel;
    [SerializeField] private Ball _prefabBall;
    [SerializeField] private PhysxBall _prefabPhysxBall;
    [Networked] public bool spawnedProjectile { get; set; }
    [Networked] private TickTimer delay { get; set; }
    private Vector3 _forward = Vector3.forward;
    private ChangeDetector _changeDetector;
    public Material material;
    private TMP_Text _messages;
    private ThirdPersonCameraController _thirdPersonCameraController;
    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        _simpleKCC = GetComponent<SimpleKCC>();
        _simpleKCC.SetGravity(Physics.gravity.y * 4.0f);

        if (Object.HasInputAuthority)
        {
            _thirdPersonCameraController = FindObjectOfType<ThirdPersonCameraController>();
            _thirdPersonCameraController.SetTarget(transform);
        }
    }
    private void Awake()
    {
        //_rb = GetComponent<Rigidbody>();
        _forward = transform.forward;
        material = GetComponentInChildren<MeshRenderer>().material;
        _nameLabel.text = PlayerInputDataManager.Instance.GetNickName();
    }

  
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    private void RPC_SendMessage(string message, RpcInfo info = default)
    {
        RPC_RelayMessage(message, info.Source);
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    private void RPC_RelayMessage(string message, PlayerRef source)
    {
        if (_messages == null)
            _messages = FindObjectOfType<TMP_Text>();

        if (source == Runner.LocalPlayer)
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
        if (_thirdPersonCameraController == null) return;

        if (GetInput(out NetworkInputData data))
        {

            Vector3 viewDirection = new Vector3(_thirdPersonCameraController.Camera.transform.position.x, _thirdPersonCameraController.Camera.transform.position.y, _thirdPersonCameraController.Camera.transform.position.z) - transform.position;
            //camera forward and right vectors:
            Vector3 forward = _thirdPersonCameraController.Camera.transform.forward;
            Vector3 right = _thirdPersonCameraController.Camera.transform.right;
            //project forward and right vectors on the horizontal plane (y = 0)
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            data.direction.Normalize();
            //this is the direction in the world space we want to move:
            var desiredMoveDirection = forward * data.direction.z + right * data.direction.x;

            Debug.Log(desiredMoveDirection);


            var displacement = (5 * data.direction );
            _simpleKCC.Move(displacement);
            _simpleKCC.SetLookRotation(Quaternion.LookRotation(_forward)) ;
            //_rb.MovePosition(newPos);
            if (desiredMoveDirection.sqrMagnitude > 0)
                _forward = desiredMoveDirection;


            Debug.DrawRay(transform.position, forward, Color.blue);
            Debug.DrawRay(transform.position, right, Color.red);


            if (Object.HasStateAuthority && delay.ExpiredOrNotRunning(Runner))
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
