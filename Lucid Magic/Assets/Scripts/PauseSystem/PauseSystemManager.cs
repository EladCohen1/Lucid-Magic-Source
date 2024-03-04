using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseSystemManager : MonoBehaviour
{
    public static PauseSystemManager Instance;
    public GameObject PauseMenuCanvas;
    public bool IsPaused;
    public bool CanPause;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        IsPaused = false;
        CanPause = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (CanPause && !IsPaused)
        {
            // check for pause
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Pause();
            }
        }
        else if (IsPaused)
        {
            // check for unPause
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UnPause();
            }
        }
    }

    // Utils
    public void Pause()
    {
        PauseMenuCanvas.SetActive(true);
        IsPaused = true;
        AbilitySystemManagerScript.Instance.ResetSelection();
        AttackSystemManagerScript.Instance.ResetSelection();
    }
    public void UnPause()
    {
        PauseMenuCanvas.SetActive(false);
        IsPaused = false;
    }
    public void RestartGame()
    {
        GameManager.Instance.InitiatePreGame();
        UnPause();
    }
}
