using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSlot : MonoBehaviour
{
    [SerializeField] private GridManager myGridManager;

    [field: SerializeField] public int currentRow { get; private set; }
    [field: SerializeField] public int currentCol { get; private set; }

    [field: SerializeField] public BrickController MyController { get; private set; }

    public void Setup(GridManager grid, int row, int col)
    {
        myGridManager = grid;
        UpdateGridPos(row, col);
        MyController = GetComponentInParent<BrickController>();
    }

    public void UpdateGridPos(int row, int col)
    {
        currentRow = row;
        currentCol = col;
    }

    public void RemoveFromGrid()
    {
        myGridManager.RemoveFromGrid(this);
    }

    public List<GridSlot> GetAdjacentSlots()
    {
        if (myGridManager == null)
        {
            return new List<GridSlot>();
        }
        return myGridManager.GetAdjacentGridSlots(this);
    }

}
