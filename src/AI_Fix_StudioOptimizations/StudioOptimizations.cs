﻿using System;
using System.Linq;
using BepInEx;
using Common;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IllusionFixes
{
    [BepInProcess(Constants.StudioProcessName)]
    [BepInPlugin(GUID, PluginName, Metadata.PluginsVersion)]
    public partial class StudioOptimizations : BaseUnityPlugin
    {
        private void Start()
        {
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            try
            {
                // Fix rotation gizmo center being disabled for some reason
                var rotateObj = Traverse.Create(Studio.GuideObjectManager.Instance).Field("objectOriginal").GetValue<GameObject>()
                    ?.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(y => y.name == "XYZ" && y.parent.name == "rotation");
                if (rotateObj != null)
                {
                    rotateObj.gameObject.SetActive(true);
                    SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }
    }
}
