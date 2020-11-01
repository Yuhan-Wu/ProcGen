using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public List<Button> ActionButtons = new List<Button>();
    public Button CancelButton;
    public GameController Context;

    public void ActivateActions()
    {
        Unit Player = Context.Player;
        foreach(Button button in ActionButtons)
        {
            if(Player.GetStat("Mana").CurrentValue >= Player.GetAction(button.gameObject.name).Cost)
                button.interactable = true;
        }

        CancelButton.interactable = false;
    }

    public void DeactivateActions()
    {
        foreach (Button button in ActionButtons)
        {
            button.interactable = false;
        }

        CancelButton.interactable = true;
    }

    public void DeactiveAll()
    {
        foreach (Button button in ActionButtons)
        {
            button.interactable = false;
        }

        CancelButton.interactable = false;
    }
}
