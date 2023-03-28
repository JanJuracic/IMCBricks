using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private Animator sceneAnimator;
    [SerializeField] private BillboardController billboardController;

    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private GridSlot[,] slotGrid;


    [SerializeField] private int startingNumOfBricks;
    [SerializeField, Range(0f, 1f)] private float percentForCompletion;
    [SerializeField] private List<BrickController> bricksInGame;



    private void Awake()
    {
        slotGrid = new GridSlot[height, width];
        instance = this;
    }

    private void Start()
    {
        //Center my tranform
        Vector3 centeredTransform = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0);
        transform.localPosition = centeredTransform;

        //Find all slots in scene
        GridSlot[] foundSlots = FindObjectsOfType<GridSlot>();

        //Place all valid slots in grid.
        foreach (GridSlot slot in foundSlots)
        {
            Vector3 slotPos = slot.transform.position;
            slotPos = new Vector3(Mathf.RoundToInt(slotPos.x), Mathf.RoundToInt(slotPos.y), 0);
            slot.transform.position = slotPos;

            //Check if slotPos is on grid;
            Vector3 centeredSlotPos = slotPos - transform.position;
            if (0 <= centeredSlotPos.y && centeredSlotPos.y < slotGrid.GetLength(0))
            {
                if (0 <= centeredSlotPos.x && centeredSlotPos.x < slotGrid.GetLength(1))
                {
                    slotGrid[(int)centeredSlotPos.y, (int)centeredSlotPos.x] = slot;
                    slot.Setup(this, (int)centeredSlotPos.y, (int)centeredSlotPos.x);
                }
            }
        }

        //Find all bricks, and keep them in a list to check against completion
        bricksInGame = FindObjectsOfType<BrickController>().ToList();
        startingNumOfBricks = bricksInGame.Count;
    }

    public void RemoveBrickFromGameList(BrickController brick)
    {
        if (bricksInGame.Contains(brick))
        {
            bricksInGame.Remove(brick);
        }
    }

    public void Fall()
    {
        for (int row = 0; row < slotGrid.GetLength(0); row++)
        {
            List<BrickController> bricksInRow = new();
            for (int col = 0; col < slotGrid.GetLength(1); col++)
            {
                var slot = slotGrid[row, col];

                if (slot == null) continue;
                if (slot.MyController == null) continue;

                if (bricksInRow.Contains(slot.MyController))
                {
                    continue;
                }
                else
                {
                    bricksInRow.Add(slot.MyController);
                }
            }

            //Move the bricks with slots on the current row
            foreach (var brick in bricksInRow)
            {
                var slots = brick.GetSlots();
                int newRow = 0;

                foreach (GridSlot slot in slots)
                {
                    int possibleRow = slot.currentRow;

                    for (int rowToCheck = slot.currentRow; rowToCheck >= 0; rowToCheck--)
                    {
                        var slotToCheck = slotGrid[rowToCheck, slot.currentCol];
                        if (slotToCheck != null && slotToCheck != slot)
                        {
                            break;
                        }
                        possibleRow = rowToCheck;
                    }

                    if (possibleRow > newRow)
                    {
                        newRow = possibleRow;
                    }
                }

                foreach (GridSlot slot in slots)
                {
                    MoveSlot(slot, newRow, slot.currentCol);
                }

                brick.Fall();
            }
        }

        //Check if game complete
        if ((float)bricksInGame.Count / (float)startingNumOfBricks <= percentForCompletion)
        {
            FinishLevel();
        }

    }

    private void FinishLevel()
    {
        StartCoroutine(Co_FinishBricks());
    }

    private IEnumerator Co_FinishBricks()
    {
        //Bricks
        var bricksToFinish = new List<BrickController>(bricksInGame);

        bricksToFinish = bricksToFinish
            .OrderBy(b => b.transform.position.y)
            .Reverse()
            .ToList();
        
        while (bricksToFinish.Count > 0)
        {
            bricksToFinish[0].RemoveBrick();
            bricksToFinish.RemoveAt(0);

            yield return new WaitForSeconds(0.03f);
        }

        //Bring up billboard
        sceneAnimator.Play("BillboardRiseUp");
        billboardController.Activate();
    }

    private void MoveSlot(GridSlot slot, int newRow, int newCol)
    {
        if (slotGrid[newRow, newCol] != null && slotGrid[newRow, newCol] != slot)
        {
            Debug.LogWarning($"MoveSlot Failed, slot already in use: {newRow}, {newCol}");
            return;
        }

        slotGrid[slot.currentRow, slot.currentCol] = null;
        slot.UpdateGridPos(newRow, newCol);
        slotGrid[slot.currentRow, slot.currentCol] = slot;
    }

    public void EndLevel()
    {
        sceneAnimator.Play("DropGreenery");
    }


    public void RemoveFromGrid(GridSlot slotToRemove)
    {
        slotGrid[slotToRemove.currentRow, slotToRemove.currentCol] = null;
    }

    public List<GridSlot> GetAdjacentGridSlots(GridSlot startSlot)
    {
        List<GridSlot> adjacentSlots = new();

        int startRow = 0;
        int startCol = 0;

        for (int row = 0; row < slotGrid.GetLength(0); row++)
        {
            for (int col = 0; col < slotGrid.GetLength(1); col++)
            {
                if (slotGrid[row, col] == startSlot)
                {
                    startRow = row;
                    startCol = col;
                }
            }
        }

        for (int rowToCheck = startRow - 1; rowToCheck <= startRow + 1; rowToCheck++)
        {
            for (int colToCheck = startCol -1; colToCheck <= startCol + 1; colToCheck++)
            {
                try
                {
                    var slot = slotGrid[rowToCheck, colToCheck];
                    if (slot != null && slot != startSlot)
                    {
                        adjacentSlots.Add(slot);
                    }
                }
                catch
                {
                    continue;
                }
            }
        }

        return adjacentSlots;
    }

    private void OnDrawGizmosSelected()
    {
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                if (slotGrid != null)
                {
                    if (slotGrid[row, col] != null)
                    {
                        Gizmos.color = Color.red;
                    }
                    else
                    {
                        Gizmos.color = Color.white;
                    }
                }
                Gizmos.DrawCube(new Vector2(col, row) + (Vector2)transform.position, Vector2.one * 0.5f);
            }
        }
    }
}
