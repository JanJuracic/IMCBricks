using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BreakManager : MonoBehaviour
{

    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private List<BrickController> bricksToDestroy =new();

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void OnEnable()
    {
        BrickController.OnBrickClicked += HandleBrickClicked;
    }

    private void OnDisable()
    {
        BrickController.OnBrickClicked -= HandleBrickClicked;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            RemoveSelectedBricks();
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            DeselectBricks();
        }

        //FXs
        lineRenderer.positionCount = bricksToDestroy.Count;
        for (int i = 0; i < bricksToDestroy.Count; i++)
        {
            lineRenderer.SetPosition(i, bricksToDestroy[i].SpriteTransform.position);
        }

    }

    private void DeselectBricks()
    {
        foreach (BrickController brick in bricksToDestroy)
        {
            brick.HandleDeselected();
        }
        bricksToDestroy.Clear();
    }

    private void RemoveSelectedBricks()
    {
        if (bricksToDestroy.Count == 0) return;

        StartCoroutine(Co_SequentialRemoveBrick());

        lineRenderer.positionCount = 0;
    }

    public IEnumerator Co_SequentialRemoveBrick()
    {
        while (bricksToDestroy.Count > 0)
        {
            var brick = bricksToDestroy.Last();
            brick.RemoveBrick();
            bricksToDestroy.Remove(brick);

            yield return new WaitForSeconds(0.15f);
        }

        GridManager.instance.Fall();
    }

    private void HandleBrickClicked(BrickController brick)
    {
        if (bricksToDestroy.Contains(brick)) return;

        if (bricksToDestroy.Count == 0)
        {
            AddBrickToEnd();
        }

        foreach (GridSlot slot in bricksToDestroy.Last().GetAdjacentSlots())
        {
            if (slot.MyController == brick)
            {
                if (bricksToDestroy.Last().brickType == brick.brickType)
                {
                    AddBrickToFront();
                    return;
                }
            }
        }

        foreach (GridSlot slot in bricksToDestroy[0].GetAdjacentSlots())
        {
            if (slot.MyController == brick)
            {
                if (bricksToDestroy[0].brickType == brick.brickType)
                {
                    AddBrickToFront();
                    return;
                }
            }
        }

        void AddBrickToFront()
        {
            bricksToDestroy.Insert(0, brick);
            brick.HandleSelected();
        }

        void AddBrickToEnd()
        {
            bricksToDestroy.Add(brick);
            brick.HandleSelected();
        }
    }

}
