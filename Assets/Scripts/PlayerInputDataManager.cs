using UnityEngine;
public class PlayerInputDataManager : PersistentSingleton<PlayerInputDataManager>
{
    private string _nickName;
    private int _score;
    public void SetNickName(string name)
    {
        _nickName = name;
    }
    public string GetNickName()
    {
        if (string.IsNullOrWhiteSpace(_nickName))
        {
            _nickName = GetRandomNickName();
        }

        return _nickName;
    }

    private string GetRandomNickName()
    {
        var rngPlayerNumber = Random.Range(0, 9999);
        return $"Player {rngPlayerNumber.ToString("0000")}";
    }
}
