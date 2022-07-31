using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAfterGameStart : MonoBehaviour
{
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        this.gameObject.SetActive(false);
        Manager.StageManager.currentStageManager.onGameStart += () => this.gameObject.SetActive(true);
        Manager.StageManager.currentStageManager.onGameOver += () => this.gameObject.SetActive(false);
    }
}
