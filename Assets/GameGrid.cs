using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;

public class GameGrid : MonoBehaviour
{
    public static GameGrid Instance;
    [SerializeField] Vector2Int GridSize = new (10,10);
    [SerializeField] int CellSize = 128;
    [SerializeField] GameObject CellPrefab = null;

    public Dictionary<Vector2Int, GridCell> GridMap = new Dictionary<Vector2Int, GridCell>();

    public enum InteractState { Default, AddUnit, SelectedUnit, RemoveUnit}
    public InteractState CurrentInteractState = InteractState.Default;
    public Troop CurrentTroopType;
    public GridCell selectedCell;

    UnityEvent NewGameState = new UnityEvent();

    public void AddNewGameStateListener(UnityAction call )
    {
        NewGameState.AddListener( call );

    }

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        CreateGrid();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    void CreateGrid()
    {
        bool odd = true;
        for (int i = -(GridSize.x/2); i < GridSize.x/2; i++)
        {
            odd = !odd;

            for (int j = -(GridSize.y/2); j < GridSize.y/2; j++)
            {
                odd = !odd;

                var newCell = Instantiate(CellPrefab, new Vector3(i*CellSize, 0, j*CellSize), Quaternion.identity);
                GridCell gridCell = newCell.GetComponent<GridCell>();

                if (gridCell)
                {

                    if (odd)
                    {
                        newCell.GetComponent<GridCell>().ChangeGridMaterial();
                    }

                    Vector2Int key = new Vector2Int(i, j);
                    gridCell.CellKey = key;
                    GridMap.Add(key, gridCell);
                }

            }
        }
    }
    public void UpdateInteractState(InteractState interactState)
    {
        CurrentInteractState = interactState;
        NewGameState.Invoke();
    }
    public void AddNewTroop(Troop troop)
    {
        CurrentTroopType = troop;
        UpdateInteractState(InteractState.AddUnit);
    }
    public void MarkAsRemoving() 
    {
        CurrentTroopType = null;
        UpdateInteractState(InteractState.RemoveUnit);
    }
    public void CellClicked(GridCell cell)
    {  
        switch (CurrentInteractState) {
            case InteractState.AddUnit:
                cell.NewTroop(CurrentTroopType);
                // Reduce troop inventory amount
                UpdateInteractState(InteractState.Default);
                CurrentTroopType = null;
                break;
                
            case InteractState.RemoveUnit:
                cell.NewTroop(null);
                //Add troop back to inventory
                break;
            case InteractState.Default:
                UpdateInteractState(InteractState.SelectedUnit);
                CurrentTroopType = cell.OccupiedTroop;
                selectedCell = cell;
                HighlightCellsInRange();
                break;
            case InteractState.SelectedUnit:
                HandleTargetSelect(cell);
                UpdateInteractState(InteractState.Default);

                break;
        }

    }
    void HighlightCellsInRange() { }
    bool IsValidTarget(Vector2Int newTarget) 
    {
        if (newTarget == selectedCell.CellKey) return false;
        return ((newTarget-selectedCell.CellKey).magnitude)<=CurrentTroopType.Range; 
    }
    void HandleTargetSelect(GridCell cell)
    {
        if (!CurrentTroopType) return;
        if (IsValidTarget(cell.CellKey))
        {
            selectedCell.TargetCell = cell.CellKey;
        }
    }

    public void BattleTick()
    {
        
        foreach (var item in GridMap.Values)
        {
            item.Battle();
        }
        foreach (var item in GridMap.Values)
        {
            item.EndBattle();
        }
    }


    void Destroy() { if (Instance != null) Destroy(Instance); }

}
