using UnityEngine;
using Utils;

public class GameEntry : MonoSingleton<GameEntry>
{
    void Start()
    {
        GameManager.Instance.Init();
    }
}