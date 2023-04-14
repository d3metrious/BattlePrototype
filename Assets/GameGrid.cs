using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class CellData
{
    [SerializeField] public Vector2Int Location;
    [SerializeField] public Troop TroopType;
}
public class GameGrid : MonoBehaviour
{
    public static GameGrid Instance;
    [SerializeField] Vector2Int GridSize = new(10, 10);
    [SerializeField] int CellSize = 128;
    [SerializeField] GameObject CellPrefab = null;
    [SerializeField] CellData[] DefaultLoadout;

    public Dictionary<Vector2Int, GridCell> GridMap = new Dictionary<Vector2Int, GridCell>();

    public enum InteractState { Default, AddUnit, SelectedUnit, RemoveUnit, MoveUnit}
    public InteractState CurrentInteractState = InteractState.Default;
    public Troop CurrentTroopType;
    public GridCell SelectedCell;
    public GridCell CurrentCell;

   


    UnityEvent NewGameState = new UnityEvent();
    UnityEvent MouseClick = new UnityEvent();

    public void AddNewGameStateListener(UnityAction call) { NewGameState.AddListener(call); }
    public void AddNewClickListener(UnityAction call) { MouseClick.AddListener(call); }

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
    public void MarkAsMoving()
    {
        UpdateInteractState(InteractState.MoveUnit);
    }
    public void MarkAsSelecting()
    {
        UpdateInteractState(InteractState.Default);
    }
    public void CellClicked(GridCell cell)
    {
        UnHighlightAllCells();

        CurrentCell = cell; 
        MouseClick.Invoke();

        switch (CurrentInteractState) {
            case InteractState.AddUnit:
                cell.NewTroop(CurrentTroopType);
                // Reduce troop inventory amount
                UpdateInteractState(InteractState.Default);
                CurrentTroopType = null;
                break;

            case InteractState.MoveUnit:
                MoveUnit(cell);
                break;
                
            case InteractState.RemoveUnit:
                cell.NewTroop(null);
                //Add troop back to inventory
                break;

            case InteractState.Default:
                UpdateInteractState(InteractState.SelectedUnit);
                CurrentTroopType = cell.OccupiedTroop;
                SelectedCell = cell;
                HighlightCellsInRange();
                break;

            case InteractState.SelectedUnit:
                HandleTargetSelect(cell);
                UpdateInteractState(InteractState.Default);

                break;
        }



    }
    void HighlightCellsInRange() {
        if (!CurrentTroopType) return;

        int range = CurrentTroopType.Range;
        for (int i = -range; i < range+1; i++)
        {
            for (int j = -range; j < range+1; j++)
            {
                Vector2Int highlightedCellKey = new(SelectedCell.CellKey.x + i, SelectedCell.CellKey.y + j);
                var foundCell = GridMap.TryGetValue(highlightedCellKey, out GridCell cell);
                if (foundCell)
                    cell.HighlightCellRange(true);
            }
        }
    }
    void UnHighlightAllCells()
    {
        foreach (var item in GridMap.Values)
        {
            item.HighlightCellRange(false);
        }
    }
    bool IsValidTarget(Vector2Int newTarget) 
    {
        if (newTarget == SelectedCell.CellKey) return false;
        return ((newTarget-SelectedCell.CellKey).magnitude)<=CurrentTroopType.Range+1; 
    }
    void HandleTargetSelect(GridCell cell)
    {
        if (!CurrentTroopType) return;
        if (IsValidTarget(cell.CellKey))
        {
            SelectedCell.TargetCell = cell.CellKey;
        }
    }

    void MoveUnit(GridCell Cell)
    {
        if(Cell == null) return;
        if (SelectedCell && SelectedCell!=Cell && !Cell.OccupiedTroop)
        {
            Cell.NewTroop(SelectedCell.OccupiedTroop);
            Cell.OccupiedTroop.Health = SelectedCell.OccupiedTroop.Health;
            SelectedCell.NewTroop(null);
            SelectedCell = null;
        }
        else
        {
            if (Cell.OccupiedTroop != null)
                SelectedCell = Cell;
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

    public void AddDefaultLoadout()
    {
        foreach (var item in GridMap.Values)
        {
            item.NewTroop(null);
        }
        foreach (CellData item in DefaultLoadout)
        {
            GridMap.TryGetValue(new Vector2Int(item.Location.x, item.Location.y), out GridCell cell);
            if (cell) cell.NewTroop(item.TroopType);
        }
        
    }

    void Destroy() { if (Instance != null) Destroy(Instance); }

}
