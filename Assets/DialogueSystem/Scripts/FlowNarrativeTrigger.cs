﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowNarrativeTrigger : MonoBehaviour
{
    [Header("Flow Narrative To Be Triggered:")]
    [Tooltip("Drag in a flow narrative Scriptable Object.")]
    [SerializeField] sFlowNarrative thisFN;

    FlowNarrativeController fn_Con;
    CircleCollider2D circleCollider;
    GameObject target;

    private void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        fn_Con = Toolbox.GetInstance().GetDialogueSystemManager().GetFlowNarrativeController();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHandleInteract playerHandleInteract = collision.gameObject.GetComponent<PlayerHandleInteract>();

        if (playerHandleInteract != null)
        {
            target = playerHandleInteract.gameObject;
            fn_Con.GetNewFlowNarrative(thisFN, target);
            circleCollider.enabled = false;
        }
    }
}