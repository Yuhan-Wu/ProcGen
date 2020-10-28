using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TilePowerUp : MonoBehaviour
{
    public GameObject OptionPanel;
    public List<GameObject> Buttons = new List<GameObject>();
    public bool Used = false;

    private void Start()
    {
        OptionPanel = GameObject.Find("PowerUp");
        if (OptionPanel)
        {
            OptionPanel.SetActive(false);
            foreach (RectTransform btn in OptionPanel.GetComponent<RectTransform>())
            {
                Buttons.Add(btn.gameObject);
                btn.gameObject.SetActive(false);
            }
        }
    }

    public void ShowOptions()
    {
        // TODO disable actions
        OptionPanel.SetActive(true);
        // TODO bad coding style
        int random_remove = UnityEngine.Random.Range(0, Buttons.Count);
        int initial_y = 100;
        int real_index = 0;
        for(int i = 0; i < Buttons.Count; i++)
        {
            if (i != random_remove)
            {
                Buttons[real_index].SetActive(true);
                Buttons[real_index].GetComponent<RectTransform>().localPosition = new Vector3(0, initial_y - 50 * real_index, 0);
                real_index++;
            }
        }
        OptionPanel.GetComponent<PowerUpPanelController>().DisableActions();
    }
}
