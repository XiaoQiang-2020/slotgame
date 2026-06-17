using UnityEngine;

public class GameManagerEx : Utils.Singleton<GameManagerEx>
{
    public void Init()
    {
        Debug.Log("GameManager Init");
        // Initialize subsystems here (audio, scene, network, etc.)
    }
}