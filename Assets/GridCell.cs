using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI HealthText;
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
    public void HighlightCellRange(bool isHover)
    {
        CellMaterial.SetInt("_Range", isHover ? 1 : 0);
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
            HealthText.text = OccupiedTroop.Health.ToString();

        }
        else
        {
            OccupiedTroop = null;
            TroopObj.GetComponent<SpriteRenderer>().sprite=null;
            HealthText.text = "";
        }
    }

    public void Battle()
    {
        if (!OccupiedTroop) return;

        var hasTarget = GameGrid.Instance.GridMap.TryGetValue(TargetCell, out GridCell target);
        if (hasTarget)
        {
            var targetTroop = target.OccupiedTroop;
            if (targetTroop)
            {
                targetTroop.Health -=
                    OccupiedTroop.Damage;
                Debug.Log(OccupiedTroop.TroopName + " attacks " + targetTroop.TroopName + " at cell " + TargetCell + ":: now has health: " + targetTroop.Health.ToString());
            }
        }
    }
    public void EndBattle()
    {
        if (!OccupiedTroop) return;
        HealthText.text = OccupiedTroop.Health.ToString();
        Debug.Log(OccupiedTroop.TroopName + OccupiedTroop.Health);
        if (OccupiedTroop.Health <= 0) {
            Debug.Log(OccupiedTroop.TroopName + " has perished at " + CellKey);
            NewTroop(null); 
        }
    }

    void ApplyDefaultTarget()
    {
        TargetCell = new(-100, 100);
    }


}
