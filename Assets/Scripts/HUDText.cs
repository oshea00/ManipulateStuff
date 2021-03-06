﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDText : Singleton<HUDText>
{
    public float UpdateInterval = 5.0f;
    public string axislabel = "NONE";
    private float m_LastInterval = 0;

    public enum FpsCounterAnchorPositions { TopLeft, BottomLeft, TopRight, BottomRight };

    public FpsCounterAnchorPositions AnchorPosition = FpsCounterAnchorPositions.TopRight;

    private string htmlColorTag;
    private const string hudLabel = "{0}</color>"+
        "\n<#00FF00>Obj World {1:0.0},{2:0.0},{3:0.0}" +
        "\n<#00FF00>Obj Local {4:0.0},{5:0.0},{6:0.0}" +
        "\n<#FF00FF>Head Pos {7:0.0},{8:0.0},{9:0.0}"+
        "\n<#FF00FF>Head Dir {10:0.0},{11:0.0},{12:0.0}";

    private TextMeshPro m_TextMeshPro;
    private Transform m_frameCounter_transform;
    private Camera m_camera;

    private FpsCounterAnchorPositions last_AnchorPosition;

    void Awake()
    {
        if (!enabled)
            return;

        m_camera = Camera.main;
        Application.targetFrameRate = -1;

        GameObject frameCounter = new GameObject("HUD Text");

        m_TextMeshPro = frameCounter.AddComponent<TextMeshPro>();
        m_TextMeshPro.font = Resources.Load("Fonts & Materials/LiberationSans SDF", typeof(TMP_FontAsset)) as TMP_FontAsset;
        m_TextMeshPro.fontSharedMaterial = Resources.Load("Fonts & Materials/LiberationSans SDF - Overlay", typeof(Material)) as Material;


        m_frameCounter_transform = frameCounter.transform;
        m_frameCounter_transform.SetParent(m_camera.transform);
        m_frameCounter_transform.localRotation = Quaternion.identity;

        m_TextMeshPro.enableWordWrapping = false;
        m_TextMeshPro.fontSize = 12;
        //m_TextMeshPro.FontColor = new Color32(255, 255, 255, 128);
        //m_TextMeshPro.edgeWidth = .15f;
        m_TextMeshPro.isOverlay = true;

        //m_TextMeshPro.FaceColor = new Color32(255, 128, 0, 0);
        //m_TextMeshPro.EdgeColor = new Color32(0, 255, 0, 255);
        //m_TextMeshPro.FontMaterial.renderQueue = 4000;

        //m_TextMeshPro.CreateSoftShadowClone(new Vector2(1f, -1f));

        Set_FrameCounter_Position(AnchorPosition);
        last_AnchorPosition = AnchorPosition;


    }

    void Start()
    {
        m_LastInterval = Time.realtimeSinceStartup;
    }

    void Update()
    {
        if (AnchorPosition != last_AnchorPosition)
            Set_FrameCounter_Position(AnchorPosition);

        last_AnchorPosition = AnchorPosition;
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow > m_LastInterval + UpdateInterval)
        {
            htmlColorTag = "<color=yellow>";
            if (GazeManager.Instance.Hit)
            {
                var p = GazeManager.Instance.Position;
                var localP = GazeManager.Instance.LocalPosition;
                var h = GazeManager.Instance.GazeOrigin;
                var rh = GazeManager.Instance.GazeAngles;
                m_TextMeshPro.SetText(htmlColorTag + string.Format(hudLabel, axislabel, 
                    p.z*39.37, -p.x*39.37, p.y*39.37, // converted to SA coords
                    localP.x/25.4, localP.y/25.4, localP.z/25.4,
                    h.x*39.37, h.y*39.37, h.z*39.37,
                    rh.x, rh.y, rh.z
                    ));

            }
            else
            {
                m_TextMeshPro.SetText(htmlColorTag + string.Format(hudLabel, axislabel, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
            }
            m_LastInterval = timeNow;
        }
    }


    void Set_FrameCounter_Position(FpsCounterAnchorPositions anchor_position)
    {
        //Debug.Log("Changing frame counter anchor position.");
        m_TextMeshPro.margin = new Vector4(1f, 1f, 1f, 1f);

        switch (anchor_position)
        {
            case FpsCounterAnchorPositions.TopLeft:
                m_TextMeshPro.alignment = TextAlignmentOptions.TopLeft;
                m_TextMeshPro.rectTransform.pivot = new Vector2(0, 1);
                m_frameCounter_transform.position = m_camera.ViewportToWorldPoint(new Vector3(0, 1, 100.0f));
                break;
            case FpsCounterAnchorPositions.BottomLeft:
                m_TextMeshPro.alignment = TextAlignmentOptions.BottomLeft;
                m_TextMeshPro.rectTransform.pivot = new Vector2(0, 0);
                m_frameCounter_transform.position = m_camera.ViewportToWorldPoint(new Vector3(0, 0, 100.0f));
                break;
            case FpsCounterAnchorPositions.TopRight:
                m_TextMeshPro.alignment = TextAlignmentOptions.TopRight;
                m_TextMeshPro.rectTransform.pivot = new Vector2(1, 1);
                m_frameCounter_transform.position = m_camera.ViewportToWorldPoint(new Vector3(1, 1, 100.0f));
                break;
            case FpsCounterAnchorPositions.BottomRight:
                m_TextMeshPro.alignment = TextAlignmentOptions.BottomRight;
                m_TextMeshPro.rectTransform.pivot = new Vector2(1, 0);
                m_frameCounter_transform.position = m_camera.ViewportToWorldPoint(new Vector3(1, 0, 100.0f));
                break;
        }
    }
}
