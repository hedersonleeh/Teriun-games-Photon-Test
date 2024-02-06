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
    [SerializeField] private NetworkRunner _runnerPrefab;
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
    public void HostButton()
    {
        PlayerInputDataManager.Instance.SetNickName(_nickName.text);
        StartGame(GameMode.AutoHostOrClient);

    }
    public void ClientButton()
    {
        PlayerInputDataManager.Instance.SetNickName(_nickName.text);
        StartGame(GameMode.Client);
    }
    private async void StartGame(GameMode mode)
    {
        _runner= Instantiate(_runnerPrefab);
        gameObject.AddComponent<RunnerSimulatePhysics3D>();

        _runner.ProvideInput = true;

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex + 1);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }
        _runner.ProvideInput = true;

        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "Only Room",
        });
        await _runner.LoadScene(scene);
    }


}
