using Fusion;
using TMPro;
using UnityEngine;
using static InputHandler;
public class Player : NetworkBehaviour
{
    public enum PlayerState
    {
        WAITING,
        PLAYING,
        DEAD
    }
    //private Rigidbody _rb;
    public Material material;
    [SerializeField] private Ball _prefabBall;
    [SerializeField] private PhysxBall _prefabPhysxBall;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _shootPosition;
    [SerializeField] private Transform _lookAtCamera;
    [SerializeField] private Transform _orientation;
    private PlayerState _state;
    private CharacterController _cc;
    private PlayerHealth _playerHealth;
    private TMP_Text _messages;
    private ThirdPersonCameraController _thirdPersonCameraController;
    private Vector3 _forward = Vector3.forward;
    [Networked] public bool spawnedProjectile { get; set; }
    [Networked] private TickTimer delay { get; set; }
    [Networked] private TickTimer deadTimer { get; set; }
    [Networked] private TickTimer revivingTimer { get; set; }
    public Camera Camera => _thirdPersonCameraController.Camera;

    public override void Spawned()
    {
        _cc = GetComponent<CharacterController>();
        _playerHealth = GetComponent<PlayerHealth>();
        if (Object.HasInputAuthority)
        {
            _state = PlayerState.PLAYING;
            _thirdPersonCameraController = FindObjectOfType<ThirdPersonCameraController>();
            _thirdPersonCameraController.SetTarget(transform,_lookAtCamera);
        }
    }
    private void Awake()
    {
        //_rb = GetComponent<Rigidbody>();
        _forward = transform.forward;
        material = GetComponentInChildren<MeshRenderer>().material;
    }


   
    Vector3 _inputDirection = Vector3.zero;
    bool mouseButton0, mouseButton1;
    private void Update()
    {
        if (_thirdPersonCameraController == null) return;
        if (HasStateAuthority == false) return;

        _animator.SetBool("Dead", _state == PlayerState.DEAD);

        switch (_state)
        {
            case PlayerState.WAITING:
                if (revivingTimer.ExpiredOrNotRunning(Runner))
                {
                    _state = PlayerState.PLAYING;
                }
                break;
            case PlayerState.PLAYING:
                UpdatePlayingState();
                CheckIfDead();
                break;
            case PlayerState.DEAD:
                if (deadTimer.ExpiredOrNotRunning(Runner))
                {
                    _state = PlayerState.WAITING;
                    revivingTimer = TickTimer.CreateFromSeconds(Runner, 1f);
                    _playerHealth.ReviveRpc();
                }
                break;
        }

    }

    private void UpdatePlayingState()
    {


        _inputDirection.x = Input.GetAxisRaw("Horizontal");
        _inputDirection.z = Input.GetAxisRaw("Vertical");
        mouseButton0 = Input.GetMouseButton(0);
        mouseButton1 = Input.GetMouseButton(1);
        _inputDirection.Normalize();
        _animator.SetFloat("Horizontal", _inputDirection.x);
        _animator.SetFloat("Vertical", _inputDirection.z);
        _animator.SetBool("Aiming", mouseButton1);
    }

    public override void FixedUpdateNetwork()
    {
        if (_thirdPersonCameraController == null) return;
        if (HasStateAuthority == false) return;
        //camera forward and right vectors:
        Vector3 viewDir = _lookAtCamera.position - new Vector3( _thirdPersonCameraController.Camera.transform.position.x,
                                                                _lookAtCamera.position.y,
                                                                _thirdPersonCameraController.Camera.transform.position.z);
        _orientation.forward = viewDir.normalized;
        Vector3 forward = _thirdPersonCameraController.Camera.transform.forward;
        Vector3 right = _thirdPersonCameraController.Camera.transform.right;
        //project forward and right vectors on the horizontal plane (y = 0)
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        //this is the direction in the world space we want to move:
        var desiredMoveDirection = forward * _inputDirection.z + right * _inputDirection.x;


        var displacement = (5 * desiredMoveDirection * Runner.DeltaTime);
        _cc.Move(displacement);
        if (desiredMoveDirection.sqrMagnitude > 0)
            _forward = desiredMoveDirection;

        Debug.DrawRay(transform.position, forward, Color.blue, 1f);
        Debug.DrawRay(transform.position, right, Color.red, 1f);
        Debug.DrawRay(transform.position, desiredMoveDirection, Color.magenta, 1f);
        if (_state != PlayerState.DEAD)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(viewDir.normalized), 45 * Runner.DeltaTime);

        if (delay.ExpiredOrNotRunning(Runner))
        {

            if (mouseButton0)
            {
                var origin = _shootPosition.position;
                GetComponent<RaycastAttack>().ShootRayAttack(origin, transform.forward);
                _animator.SetTrigger("Shoot");
                delay = TickTimer.CreateFromSeconds(Runner, 0.25f);
                Runner.Spawn(_prefabBall,
                    origin, Quaternion.LookRotation(transform.forward), Object.InputAuthority,
                    (runner, o) =>
                    {
                        o.GetComponent<Ball>().Init(GetComponent<PlayerDataNetworked>().playerColor);
                    });
                spawnedProjectile = !spawnedProjectile;

            }

        }

    }

    private void CheckIfDead()
    {
        if (_playerHealth.IsDead)
        {
            _state = PlayerState.DEAD;
            deadTimer = TickTimer.CreateFromSeconds(Runner, 3f);
            GetComponent<PlayerDataNetworked>().AddDeathCount();
            _playerHealth.LastDealer.GetComponent<PlayerDataNetworked>().AddKillCount();
        }
    }
}


