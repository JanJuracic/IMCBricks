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
    [SerializeField] private List<BrickType> possiblebrickTypes;
    [SerializeField] public BrickType MyBrickType;

    [SerializeField] private SpriteRenderer faceRenderer;
    [SerializeField] private SpriteRenderer mortarRenderer;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private AudioSource audio;

    [SerializeField] private bool interactable = true;
    [SerializeField] private bool selected = false;
    
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        MyBrickType = possiblebrickTypes[UnityEngine.Random.Range(0, possiblebrickTypes.Count)];
        faceRenderer.sprite = MyBrickType.GetNextSprite();
        audio = GetComponent<AudioSource>();
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

    public Vector3 GetCenterPos()
    {
        return transform.position + new Vector3(0.5f, 0);
    }

    public Vector3 GetFaceTransformPos()
    {
        return faceRenderer.transform.position;
    }

    public void RemoveBrick()
    {
        selected = false;
        LeftSlot.RemoveFromGrid();
        RightSlot.RemoveFromGrid();
        GameManager.instance.RemoveBrickFromGameList(this);
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
        var originalLocalPos = faceRenderer.transform.localPosition;

        while (selected)
        {
            faceRenderer.transform.localPosition = Wobble(originalLocalPos);

            yield return null;
        }

        faceRenderer.transform.localPosition = originalLocalPos;

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
        //Disable mortar
        mortarRenderer.forceRenderingOff = true;
        mortarRenderer.enabled = false;

        //Particles
        particles.Play();

        //Sound
        audio.Play();

        //Draw order
        faceRenderer.sortingLayerName = "Foreground";
        faceRenderer.sortingOrder = UnityEngine.Random.Range(3, 10);

        //Physics 
        float startX = UnityEngine.Random.Range(-1f, 1f);
        float startY = UnityEngine.Random.Range(1.2f, 2f);
        Vector3 vel = new Vector3(startX, startY) * 0.1f;

        Vector3 gravity = Vector3.down * 0.005f;
        float rotation = UnityEngine.Random.Range(-2.8f, 2.8f);

        while (true)
        {
            vel += gravity;
            faceRenderer.transform.localPosition += vel;
            faceRenderer.transform.eulerAngles += new Vector3(0, 0, rotation);

            if (faceRenderer.transform.position.y < -20) break;

            yield return null;
        }

        Destroy(gameObject);
    }

    public void Fall()
    {
        Vector3 targetPosition = new Vector3(LeftSlot.currentCol, LeftSlot.currentRow) + GameManager.instance.transform.position;
        if (targetPosition == transform.position) return;

        StartCoroutine(Co_FallFx(targetPosition));
    }

    private IEnumerator Co_FallFx(Vector3 targetPos)
    {
        interactable = false;

        Transform faceTransform = faceRenderer.transform;
        Vector3 originalLocalPos = faceTransform.localPosition;
        Vector3 targetFacePos = new Vector3(faceTransform.position.x, targetPos.y, 0);

        //Turn off mortar
        mortarRenderer.enabled = false;

        //Physics 
        Vector3 gravity = Vector3.down * 0.005f;
        Vector3 vel = Vector3.zero;

        //Falling
        while (true)
        {
            vel += gravity;
            faceTransform.position += vel;

            if (faceTransform.position.y < targetFacePos.y)
            {
                transform.position = targetPos;
                faceTransform.localPosition = originalLocalPos;
                mortarRenderer.enabled = true;
                break;
            }
            yield return null;
        }

        //Shaking
        float wobbleTime = 0.1f;
        while (true)
        {
            faceTransform.localPosition += Wobble();
            wobbleTime += -Time.deltaTime;

            if (wobbleTime < 0f)
            {
                faceTransform.localPosition = originalLocalPos;
                break;
            }
            yield return null;
        }

        interactable = true;

        Vector3 Wobble()
        {
            float time = Time.time + UnityEngine.Random.Range(0f, 40f);
            float speed = 0.5f;
            float amount = 0.02f;

            Vector3 addedWobble = new Vector3(Mathf.Sin(time * speed) * amount, Mathf.Cos(time * speed) * amount);

            return addedWobble;
        }
    }

    private List<GridSlot> GetAdjacentSlots()
    {
        var leftAdjacentSlots = LeftSlot.GetAdjacentSlots();
        var rightAdjacentSlots = RightSlot.GetAdjacentSlots();

        leftAdjacentSlots.Remove(RightSlot);
        rightAdjacentSlots.Remove(LeftSlot);

        List<GridSlot> allAdjacent = leftAdjacentSlots.Union(rightAdjacentSlots).ToList();
        return allAdjacent;
    }

}
