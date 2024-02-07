using UnityEngine;

public class BillBoardLabelName:MonoBehaviour
{
    private Camera _toLookcam;
    private void Awake()
    {
        _toLookcam = Camera.main;
    }
    private void LateUpdate()
    {
        //transform.forward = _toLookcam.transform.forward;
    }
}
