using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebsiteGoer : MonoBehaviour
{
    private BoxCollider2D collider;
    [SerializeField] private bool ready = false;


    private void Start()
    {
        collider = GetComponent<BoxCollider2D>();
    }

    public void GetReadyToLinkToWebsite()
    {
        ready = true;
    }

    private void OnMouseDown()
    {
        if (ready) Application.OpenURL("https://www.imc-agencija.hr/");
    }

}
