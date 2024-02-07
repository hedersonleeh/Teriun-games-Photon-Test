using Fusion;
using UnityEngine;
using Cinemachine;
public class ThirdPersonCameraController : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private CinemachineFreeLook _freelookCamera;
    private Transform _target;
    public Camera Camera => _camera;
    public void SetTarget(Transform target)
    {
        _target = target;
        _freelookCamera.Follow = target;
        _freelookCamera.LookAt = target;
    }


}
