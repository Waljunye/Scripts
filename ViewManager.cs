﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public enum View
{
    Hand,
    Default,
    Board,
    Scales,
    Choices,
}

public class ViewManager : MonoBehaviour
{
    public static ViewManager instance;

    public View CurrentView { get; set; }
    public bool ViewLocked { get; set; }

    [SerializeField]
    private Transform hand;

    [SerializeField]
    private Transform cameraRig;

    [SerializeField]
    private LerpAlpha fadeMask;

    private const float TRANSITION_SPEED = 0.25f;

    private readonly Vector3 DEFAULT_HAND_POS = new Vector3(1.15f, 3.88f, -5.43f);
    private readonly Vector3 DEFAULT_CAM_POS = new Vector3(0f, 7.65f, -6.86f);

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SwitchToView(View.Default, immediate: true);
    }

    public void SetBlack()
    {
        fadeMask.SetColor(Color.black);
        fadeMask.intendedAlpha = 1f;
    }

    public void FadeIn()
    {
        fadeMask.SetColor(Color.black);
        fadeMask.intendedAlpha = 0f;
    }

    public void FadeOut()
    {
        fadeMask.SetColor(new Color(0f,0f,0f,0f));
        fadeMask.intendedAlpha = 1f;
    }

    public void FadeInRed()
    {
        fadeMask.speed = 20f;
        fadeMask.SetColor(new Color(1f, 0f, 0f, 0f));
        fadeMask.intendedAlpha = 1f;
    }

    public void FadeOutRed()
    {
        fadeMask.SetColor(Color.red);
        fadeMask.intendedAlpha = 0f;
        fadeMask.speed = 2.5f;
    }

    public void SwitchToView(View view, bool immediate = false)
    {
        if (CurrentView != view)
        {
            float transitionSpeed = immediate ? 0f : TRANSITION_SPEED;
            switch (view)
            {
                case View.Default:
                    if (hand != null)
                    {
                        Tween.Position(hand, DEFAULT_HAND_POS, transitionSpeed, 0f, Tween.EaseInOut);
                    }

                    Tween.Position(cameraRig, DEFAULT_CAM_POS, transitionSpeed, 0f, Tween.EaseInOut);
                    Tween.Rotation(cameraRig, new Vector3(17.4f, 0f, 0f), transitionSpeed, 0f, Tween.EaseInOut);
                    Tween.FieldOfView(cameraRig.GetComponent<Camera>(), 60f, transitionSpeed, 0f, Tween.EaseInOut);
                    break;
                case View.Hand:
                    if (hand != null)
                        Tween.Position(hand, new Vector3(1.15f, 4.75f, -5f), transitionSpeed, 0f, Tween.EaseInOut);

                    Tween.Position(cameraRig, DEFAULT_CAM_POS, transitionSpeed, 0f, Tween.EaseInOut);
                    Tween.Rotation(cameraRig, new Vector3(25.75f, 0f, 0f), transitionSpeed, 0f, Tween.EaseInOut);
                    Tween.FieldOfView(cameraRig.GetComponent<Camera>(), 55f, transitionSpeed, 0f, Tween.EaseInOut);
                    break;
                case View.Board:
                    if (hand != null)
                        Tween.Position(hand, DEFAULT_HAND_POS, transitionSpeed, 0f, Tween.EaseInOut);

                    Tween.Position(cameraRig, new Vector3(0.96f, 9.79f, -0.92f), transitionSpeed, 0f, Tween.EaseInOut);
                    Tween.Rotation(cameraRig, new Vector3(79.9f, 0f, 0f), transitionSpeed, 0f, Tween.EaseInOut);
                    Tween.FieldOfView(cameraRig.GetComponent<Camera>(), 50f, transitionSpeed, 0f, Tween.EaseInOut);
                    break;
                case View.Scales:
                    if (hand != null)
                        Tween.Position(hand, DEFAULT_HAND_POS, transitionSpeed, 0f, Tween.EaseInOut);

                    Tween.Position(cameraRig, new Vector3(0.34f, 7.89f, -4.8f), transitionSpeed, 0f, Tween.EaseInOut);
                    Tween.Rotation(cameraRig, new Vector3(14.96f, -22.93f, -6.647f), transitionSpeed, 0f, Tween.EaseInOut);
                    Tween.FieldOfView(cameraRig.GetComponent<Camera>(), 60f, transitionSpeed, 0f, Tween.EaseInOut);
                    break;
                case View.Choices:
                    if (hand != null)
                        Tween.Position(hand, DEFAULT_HAND_POS + (Vector3.down * 100f), transitionSpeed, 0f, Tween.EaseInOut);

                    Tween.Position(cameraRig, new Vector3(1.02f, 7.94f, -4.84f), transitionSpeed, 0f, Tween.EaseInOut);
                    Tween.Rotation(cameraRig, new Vector3(56.44f, 5.17f, 3.709f), transitionSpeed, 0f, Tween.EaseInOut);
                    Tween.FieldOfView(cameraRig.GetComponent<Camera>(), 60f, transitionSpeed, 0f, Tween.EaseInOut);
                    break;
            }

            CurrentView = view;
        }
    }

    private void Update()
    {
        if (!ViewLocked)
        {
            if (Input.GetButtonDown("View Down"))
            {
                if (CurrentView != View.Hand)
                {
                    SwitchToView(CurrentView - 1);
                }
            }

            if (Input.GetButtonDown("View Up"))
            {
                if (CurrentView != View.Board)
                {
                    SwitchToView(CurrentView + 1);
                }
            }
        }
    }
}