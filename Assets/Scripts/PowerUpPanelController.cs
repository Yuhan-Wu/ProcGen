using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PowerUpPanelController : MonoBehaviour
{
    public PlayerController Player;
    private bool InitializationDone = false;

    public void Update()
    {
        if (!InitializationDone)
        {
            InitializationDone = true;
            this.gameObject.SetActive(false);
        }
    }

    public void Close()
    {
        GetComponentInParent<ButtonController>().ActivateActions();
        this.gameObject.SetActive(false);
        Player.DisableAction = false;
    }

    public void DisableActions()
    {
        GetComponentInParent<ButtonController>().DeactiveAll();
        Player.DisableAction = true;
    }
}
