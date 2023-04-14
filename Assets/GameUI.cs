using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI CurrentGameStateText;
    [SerializeField] TextMeshProUGUI CurrentSelectText;

    // Start is called before the first frame update

    void Awake()
    {
        if (!CurrentGameStateText) CurrentGameStateText = gameObject.GetComponent<TextMeshProUGUI>();
        if (!CurrentGameStateText) CurrentGameStateText = gameObject.GetComponentInChildren<TextMeshProUGUI>();

    }
    void Start()
    {
        ActionToBind();
        GameGrid.Instance.AddNewGameStateListener(UpdateSelectionInfo);
    }


    // Update is called once per frame
    void UpdateStateUI()
    {
        string text="";
        switch (GameGrid.Instance.CurrentInteractState) 
        {
            case GameGrid.InteractState.Default:
                break;
            case GameGrid.InteractState.AddUnit:
                text = "Adding troop: " + GameGrid.Instance.CurrentTroopType.TroopName;
                break;
            case GameGrid.InteractState.SelectedUnit:
               
                text = (GameGrid.Instance.SelectedCell) 
                    ? "target: " + GameGrid.Instance.SelectedCell.TargetCell.ToString() 
                    : "select target";

                break;
            case GameGrid.InteractState.RemoveUnit:
                text = "Removing troops from grid";

                break; 
            default:
                break;
        }
        CurrentGameStateText.SetText(text);
        UpdateSelectionInfo();
    }
    void UpdateSelectionInfo()
    {
        var gameGrid = GameGrid.Instance;
        if (CurrentSelectText && gameGrid.CurrentCell)
        {
            if (!gameGrid.CurrentCell.OccupiedTroop) CurrentSelectText.text = "";
            else
            {
                CurrentSelectText.text =
                    gameGrid.CurrentCell.OccupiedTroop.TroopName.ToString() +
                    ":\n Health=" + gameGrid.CurrentCell.OccupiedTroop.Health.ToString() +
                     "\n Damage=" + gameGrid.CurrentCell.OccupiedTroop.Health.ToString() +
                     "\n Target Cell=" + gameGrid.CurrentCell.TargetCell.ToString();
            }
        }
    }

    void Destroy()
    {
        //Remove Listener
    }

    void ActionToBind() => GameGrid.Instance.AddNewGameStateListener(UpdateStateUI);

}
