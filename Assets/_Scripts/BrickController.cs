using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BrickController : MonoBehaviour
{
    public static event Action<BrickController> OnBrickClicked;

    [SerializeField] private GridSlot LeftSlot;
    [SerializeField] private GridSlot RightSlot;
    [SerializeField] public BrickType brickType;    
    [SerializeField] private List<Sprite> possibleSprites;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private bool interactable = true;
    [SerializeField] private bool selected = false;
    
    private BoxCollider2D boxCollider;

    [field: SerializeField] public Transform SpriteTransform { get; private set; }

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        SpriteTransform = spriteRenderer.transform;
    }

    private void Start()
    {
        brickType = (BrickType)UnityEngine.Random.Range(0, 2);
        spriteRenderer.sprite = possibleSprites[(int)brickType];
    }

    private void OnMouseDown()
    {
        if (interactable) OnBrickClicked?.Invoke(this);
    }

    private void OnMouseEnter()
    {
        if (Input.GetMouseButton(0))
        {
            if (interactable) OnBrickClicked?.Invoke(this);
        }
    }

    public List<GridSlot> GetSlots()
    {
        List<GridSlot> mySlots = new();

        mySlots.Add(LeftSlot);
        mySlots.Add(RightSlot);

        return mySlots;
    }

    public void RemoveBrick()
    {
        selected = false;
        LeftSlot.RemoveFromGrid();
        RightSlot.RemoveFromGrid();
        StartCoroutine(Co_RemoveBrickFX());
    }

    public void HandleSelected()
    {
        if (selected) return;

        selected = true;
        StartCoroutine(Co_SelectedFx());
    }

    public void HandleDeselected()
    {
        if (!selected) return;

        selected = false;
    }


    public IEnumerator Co_SelectedFx()
    {
        var originalLocalPos = SpriteTransform.localPosition;

        while (selected)
        {
            SpriteTransform.localPosition = Wobble(originalLocalPos);

            yield return null;
        }

        SpriteTransform.localPosition = originalLocalPos;

        Vector3 Wobble(Vector3 pos)
        {
            float time = Time.time + UnityEngine.Random.Range(0f, 40f);
            float speed = 1f;
            float amount = 0.1f;

            Vector3 addedWobble = new Vector3(Mathf.Sin(time * speed) * amount, Mathf.Cos(time * speed) * amount);

            return addedWobble + originalLocalPos;
        }
    }

    public IEnumerator Co_RemoveBrickFX()
    {
        Vector3 gravity = Vector3.down * 0.005f;

        float startX = UnityEngine.Random.Range(-1f, 1f);
        float startY = UnityEngine.Random.Range(1.2f, 2f);
        Vector3 vel = new Vector3(startX, startY) * 0.1f;

        float rotation = UnityEngine.Random.Range(-2.8f, 2.8f);

        while (true)
        {
            vel += gravity;
            SpriteTransform.localPosition += vel;
            SpriteTransform.eulerAngles += new Vector3(0, 0, rotation);

            if (SpriteTransform.position.y < -20) break;

            yield return null;
        }

        Destroy(gameObject);
    }

    public void FallFx()
    {
        StartCoroutine(Co_FallFx());
    }

    private IEnumerator Co_FallFx()
    {
        interactable = false;

        Vector3 targetPosition = new Vector3(LeftSlot.currentCol, LeftSlot.currentRow) + GridManager.instance.transform.position;

        while (true)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, 0.05f);

            if ((transform.position - targetPosition).magnitude < 0.05f)
            {
                transform.position = targetPosition;
                break;
            }

            yield return null;
        }

        interactable = true;
    }

    public List<GridSlot> GetAdjacentSlots()
    {
        var leftAdjacentSlots = LeftSlot.GetAdjacentSlots();
        var rightAdjacentSlots = RightSlot.GetAdjacentSlots();

        leftAdjacentSlots.Remove(RightSlot);
        rightAdjacentSlots.Remove(LeftSlot);

        List<GridSlot> allAdjacent = leftAdjacentSlots.Union(rightAdjacentSlots).ToList();
        return allAdjacent;
    }
}

public enum BrickType
{
    Red,
    Yellow
}
