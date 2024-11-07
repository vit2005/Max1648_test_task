using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PauseUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI pauseText;
    private bool _isPaused = false;

    public void Pause()
    {
        _isPaused = !_isPaused;
        pauseText.text = _isPaused ? "Unpause" : "Pause";
        Time.timeScale = _isPaused ? 0.0f : 1.0f;
    }
}
