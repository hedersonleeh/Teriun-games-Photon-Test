using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.SceneManagement;
using Fusion.Addons.Physics;
using TMPro;
public class MainLobby : MonoBehaviour
{
    private NetworkRunner _runner;

    [SerializeField] private TMP_InputField _nickName = null;
    [SerializeField] private TMP_Text _placeholder = null;

    private void LateUpdate()
    {
        if(string.IsNullOrEmpty(_nickName.text))
        {
            _placeholder.text = "Enter your nickname";
        }
    }
    public void SetNickName()
    {
        PlayerInputDataManager.Instance.SetNickName(_nickName.text);
      

    }
  
    


}
