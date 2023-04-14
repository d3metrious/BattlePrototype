using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    public GameObject TroopObj;
    public Vector2Int CellKey { get; set; }
    public Troop OccupiedTroop;
    public Vector2Int TargetCell;
    Material CellMaterial;

    // Start is called before the first frame update
    void Awake()
    {
        CellMaterial = GetComponent<Renderer>().material;
    }

    void OnMouseDown()
    {
        GameGrid.Instance.CellClicked(this);
    }

    void OnMouseOver()
    {
        SetMaterialHover(true);
    }
    void OnMouseExit()
    {
        SetMaterialHover(false);    
    }
    public void SetMaterialHover(bool isHover)
    {
        CellMaterial.SetInt("_Hover", isHover ? 1 : 0);
    }

    public void ChangeGridMaterial()
    {
        if(CellMaterial)CellMaterial.SetInt("_Alt", 1);
    }

    public void NewTroop(Troop troop)
    {
        ApplyDefaultTarget();
        if (troop != null)
        {
            var instance = ScriptableObject.Instantiate(troop);
            OccupiedTroop = instance;
            TroopObj.GetComponent<SpriteRenderer>().sprite = troop.Sprite;
        }
        else
        {
            OccupiedTroop = null;
            TroopObj.GetComponent<SpriteRenderer>().sprite=null;
        }
    }

    public void Battle()
    {
        if (!OccupiedTroop) return;

        var hasTarget = GameGrid.Instance.GridMap.TryGetValue(TargetCell, out GridCell target);
        if (hasTarget && OccupiedTroop)
        {
            var targetTroop = target.OccupiedTroop;
            if (targetTroop != null)
            {
                targetTroop.Health -= 
                    OccupiedTroop.Damage;
            }
        }
    }
    public void EndBattle()
    {
        if (!OccupiedTroop) return;
        Debug.Log(OccupiedTroop.TroopName + OccupiedTroop.Health);
        if (OccupiedTroop.Health <= 0) { NewTroop(null); }
    }

    void ApplyDefaultTarget()
    {
        TargetCell = new(-100, 100);
    }


}
