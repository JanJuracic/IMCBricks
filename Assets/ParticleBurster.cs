using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleBurster : MonoBehaviour
{
    [SerializeField] private AudioSource boing;
    [SerializeField] private AudioSource reveal;

    public void PlayBoing()
    {
        boing.Play();
    }

    public void PlayReveal()
    {
        reveal.Play();
    }

    public void ReadyWebsite()
    {
        BillboardController billboard = GetComponentInChildren<BillboardController>();
        billboard.GetReadyToLinkToWebsite();
    }

}
