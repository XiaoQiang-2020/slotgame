using UnityEngine;

public class GameManager : Utils.Singleton<GameManager>
{
    public void Init()
    {
        Debug.Log("GameManager Init");
        // Initialize subsystems here (audio, scene, network, etc.)
    }
}