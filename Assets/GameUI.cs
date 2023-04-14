using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI CurrentGameStateText;
    // Start is called before the first frame update

    void Awake()
    {
        if(!CurrentGameStateText)CurrentGameStateText = gameObject.GetComponent<TextMeshProUGUI>();

    }
    void Start()
    {
        ActionToBind();
        
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
               
                text = (GameGrid.Instance.selectedCell) 
                    ? "target: " + GameGrid.Instance.selectedCell.TargetCell.ToString() 
                    : "selet target";

                break;
            case GameGrid.InteractState.RemoveUnit:
                text = "Removing troops from grid";

                break; 
            default:
                break;
        }
        CurrentGameStateText.SetText(text);
    }

    void Destroy()
    {
        //Remove Listener
    }

    void ActionToBind() => GameGrid.Instance.AddNewGameStateListener(UpdateStateUI);

}
