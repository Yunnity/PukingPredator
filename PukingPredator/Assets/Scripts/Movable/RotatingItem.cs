using System;
using UnityEngine;


public class RotatingItem : MovableItem
{
    [SerializeField]
    private float rotationSpeed = 100f;

    [SerializeField]
    private Vector3 rotationAxis = Vector3.up;

    private Player player;

    protected override void Start()
    {
        base.Start();

        foreach (var interactable in GetComponentsInChildren<Interactable>())
        {
            interactable.Highlighted += HighlightAll;
            interactable.Unhighlighted += UnhighlightAll;
        }
    }

    private void UnhighlightAll()
    {
        foreach (var interactable in GetComponentsInChildren<Interactable>())
        {
            interactable.outline.enabled = false;
        }
    }

    private void HighlightAll()
    {
        foreach (var interactable in GetComponentsInChildren<Interactable>())
        {
            interactable.outline.enabled = true;
        }
    }

    private void Update()
    {
        transform.Rotate(rotationAxis.normalized * rotationSpeed * Time.deltaTime);
    }
}