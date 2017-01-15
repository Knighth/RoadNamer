﻿using ColossalFramework;
using UnityEngine;
using RoadNamer.Managers;
using ColossalFramework.UI;
using System;
using RoadNamer.Panels;

namespace RoadNamer.Tools
{
    public class RoadSelectTool : DefaultTool
    {
        public RoadNamePanel m_roadNamePanel = null;

        protected override void Awake()
        {
            Debug.Log("Road Namer: Tool awake");

            base.Awake();
        }

        protected override void OnToolGUI(Event e) //kh added new Event argment 
        {
            base.OnToolGUI(e); //added e arg
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void OnToolUpdate()
        {
            if (m_toolController != null && !m_toolController.IsInsideUI && Cursor.visible)
            {
                RaycastOutput raycastOutput;

                if (RaycastRoad(out raycastOutput))
                {
                    ushort netSegmentId = raycastOutput.m_netSegment;

                    if (netSegmentId != 0)
                    {
                        NetManager netManager = Singleton<NetManager>.instance;
                        NetSegment netSegment = netManager.m_segments.m_buffer[netSegmentId];

                        if (netSegment.m_flags.IsFlagSet(NetSegment.Flags.Created))
                        {
                            if (Event.current.type == EventType.MouseDown /*&& Event.current.button == (int)UIMouseButton.Left*/)
                            {
                                ShowToolInfo(false, null, new Vector3());

                                if (m_roadNamePanel != null)
                                {
                                    RandomNameManager.LoadRandomNames();
                                    m_roadNamePanel.initialRoadName = RoadNameManager.Instance().GetRoadName(netSegmentId);
                                    m_roadNamePanel.m_netSegmentId = netSegmentId;
                                    m_roadNamePanel.Show();

                                    OptionsManager.m_hasOpenedPanel = true;
                                    OptionsManager.SaveOptions();
                                }
                            }
                            else
                            {
                                ShowToolInfo(true, "Click to name this road segment", netSegment.m_bounds.center);
                            }
                        }
                    }
                }
            }
            else
            {
                ShowToolInfo(false, null, new Vector3());
            }
        }

        bool RaycastRoad(out RaycastOutput raycastOutput)
        {
            RaycastInput raycastInput = new RaycastInput(Camera.main.ScreenPointToRay(Input.mousePosition), Camera.main.farClipPlane);
            raycastInput.m_netService.m_service = ItemClass.Service.Road;
            raycastInput.m_netService.m_itemLayers = ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels | ItemClass.Layer.PublicTransport;
            raycastInput.m_ignoreSegmentFlags = NetSegment.Flags.None;
            raycastInput.m_ignoreNodeFlags = NetNode.Flags.None;
            raycastInput.m_ignoreBuildingFlags = Building.Flags.None;
            raycastInput.m_ignoreTerrain = true;

            return RayCast(raycastInput, out raycastOutput);
        }
    }
}
