using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BreakManager : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private AudioSource ping;
    [SerializeField] private List<BrickController> bricksToDestroy =new();

    [SerializeField] private Vector3 startPos;
    [SerializeField] private Vector3 endPos;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        ping = GetComponent<AudioSource>();
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
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            RemoveSelectedBricks();
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            DeselectBricks();
        }

        if (Input.GetMouseButtonDown(1))
        {
            DeselectLastBrick();
        }

        //FXs
        lineRenderer.positionCount = bricksToDestroy.Count;
        for (int i = 0; i < bricksToDestroy.Count; i++)
        {
            lineRenderer.SetPosition(i, bricksToDestroy[i].GetFaceTransformPos());
        }

    }

    private void DeselectLastBrick()
    {
        if (bricksToDestroy.Count > 0)
        {
            bricksToDestroy.Last().HandleDeselected();
            bricksToDestroy.Remove(bricksToDestroy.Last());
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

    private IEnumerator Co_SequentialRemoveBrick()
    {
        float delay = 0.20f;

        while (bricksToDestroy.Count > 0)
        {
            var brick = bricksToDestroy[0];
            brick.RemoveBrick();
            bricksToDestroy.Remove(brick);

            yield return new WaitForSeconds(delay);

            delay = Mathf.Lerp(delay, 0.07f, 0.15f);
        }

        GameManager.instance.Fall();
    }

    private void HandleBrickClicked(BrickController brick)
    {
        if (bricksToDestroy.Contains(brick)) return;

        if (bricksToDestroy.Count == 0)
        {
            AddBrickToEnd();
            return;
        }

        var lastBrick = bricksToDestroy.Last();
        endPos = brick.GetCenterPos();
        startPos = lastBrick.GetCenterPos();

        RaycastHit2D[] hits = Physics2D.RaycastAll(startPos, (endPos - startPos), (endPos - startPos).magnitude);
        List<BrickController> hitBricks = new();
        for (int i = 0; i < hits.Length; i++)
        {
            var hitBrick = hits[i].collider.gameObject.GetComponent<BrickController>();

            if (hitBrick != null)
            {
                hitBricks.Add(hitBrick);
            }
        }

        //Remove bricks that are last brick
        hitBricks = hitBricks
            .Where(b => b != lastBrick)
            .ToList();

        //Check if the nearest hit brick is the correct type
        if (hitBricks[0] == brick && hitBricks[0].MyBrickType == lastBrick.MyBrickType)
        {
            AddBrickToEnd();
        }

        void AddBrickToEnd()
        {
            if (bricksToDestroy.Contains(brick) == false)
            {
                bricksToDestroy.Add(brick);
                brick.HandleSelected();
            }
            ping.Play();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(startPos, endPos - startPos);
    }

}
