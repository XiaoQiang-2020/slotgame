using UnityEngine;
using Utils;

public class GameEntry : MonoSingleton<GameEntry>
{
    void Start()
    {
        GameManagerEx.Instance.Init();
    }
}