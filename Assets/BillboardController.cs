using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardController : MonoBehaviour
{

    
    [SerializeField] private bool interactable = true;


    private BoxCollider2D boxCollider;

    //private void Awake()
    //{
    //    boardRenderer.enabled = false;
    //    greeneryRenderer.enabled = false;
    //}

    public void Activate()
    {
        interactable = true;
    }

    private void OnMouseDown()
    {
        if (interactable)
        {
            GameManager.instance.EndLevel();
        }
    }

    //[ContextMenu("Rise")]
    //public void Rise()
    //{
    //    StartCoroutine(Co_RiseFx());
    //}


    //[ContextMenu("Drop Board")]
    //public void DropBillBoard()
    //{
    //    StartCoroutine(Co_DropSprite(boardRenderer));
    //}


    //public IEnumerator Co_DropSprite(SpriteRenderer targetSprite)
    //{
    //    //Physics 
    //    float startX = UnityEngine.Random.Range(-0.1f, 0.1f);
    //    float startY = 0f;
    //    Vector3 vel = new Vector3(startX, startY) * 0.1f;

    //    Vector3 gravity = Vector3.down * 0.0015f;
    //    float rotation = UnityEngine.Random.Range(-0.5f, 0.5f);

    //    while (true)
    //    {
    //        vel += gravity;
    //        targetSprite.transform.localPosition += vel;
    //        targetSprite.transform.eulerAngles += new Vector3(0, 0, rotation);

    //        if (targetSprite.transform.position.y < -20) break;

    //        yield return null;
    //    }

    //    targetSprite.enabled = false;
    //}

    //private IEnumerator Co_RiseFx()
    //{
    //    interactable = false;

    //    //Get current position to be final position
    //    Vector3 targetPos = boardRenderer.transform.position;
    //    Vector3 startPos = boardRenderer.transform.position + (Vector3.down * 15f);

    //    boardRenderer.transform.position = startPos;
    //    greeneryRenderer.transform.position = startPos;

    //    boardRenderer.enabled = true;
    //    greeneryRenderer.enabled = true;

    //    //Physics 
    //    Vector3 risingForce = Vector3.up * 0.006f;
    //    Vector3 vel = Vector3.zero;

    //    //Rising
    //    while (true)
    //    {
    //        vel += risingForce;
    //        boardRenderer.transform.position += vel;
    //        greeneryRenderer.transform.position += vel;

    //        if (boardRenderer.transform.position.y > targetPos.y)
    //        {
    //            boardRenderer.transform.position = targetPos;
    //            greeneryRenderer.transform.position = targetPos;
    //            break;
    //        }
    //        yield return null;
    //    }

    //    //Shaking
    //    float wobbleTime = 0.1f;
    //    while (true)
    //    {
    //        boardRenderer.transform.position += Wobble();
    //        greeneryRenderer.transform.position += Wobble();
    //        wobbleTime += -Time.deltaTime;

    //        if (wobbleTime < 0f)
    //        {
    //            boardRenderer.transform.position = targetPos;
    //            greeneryRenderer.transform.position = targetPos;
    //            break;
    //        }
    //        yield return null;
    //    }

    //    interactable = true;

    //    Vector3 Wobble()
    //    {
    //        float time = Time.time + UnityEngine.Random.Range(0f, 40f);
    //        float speed = 0.35f;
    //        float amount = 0.02f;

    //        Vector3 addedWobble = new Vector3(Mathf.Sin(time * speed) * amount, Mathf.Cos(time * speed) * amount);

    //        return addedWobble;
    //    }
    //}

}
