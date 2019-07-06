﻿using BepInEx.Logging;
using Harmony;
using System;
using System.Collections;
using UnityEngine;
using Logger = BepInEx.Logger;

namespace Common
{
    internal class CC
    {
        internal static bool InsideStudio => Application.productName == "CharaStudio";
        internal static bool InsideKoikatsuParty => Application.productName == "Koikatsu Party";
        internal static void Log(string text) => Logger.Log(LogLevel.Info, text);
        internal static void Log(LogLevel level, string text) => Logger.Log(level, text);
        internal static void Log(object text) => Logger.Log(LogLevel.Info, text?.ToString());
        internal static void Log(LogLevel level, object text) => Logger.Log(level, text?.ToString());
    }

    internal static class Utils
    {
        /// <summary>
        /// Create a coroutine that calls each of the action delegates on consecutive frames
        /// (yield return null is returned after each one of the actions)
        /// </summary>
        public static IEnumerator CreateCoroutine(params Action[] actions)
        {
            foreach (var action in actions)
            {
                action();
                yield return null;
            }
        }

        /// <summary>
        /// Create a coroutine that calls each of the supplied coroutines in order
        /// </summary>
        public static IEnumerator ComposeCoroutine(params IEnumerator[] coroutine)
        {
            return coroutine.GetEnumerator();
        }
    }

    internal static partial class AccessoriesApi
    {
        private static UnityEngine.Object _moreAccessoriesInstance;
        private static Type _moreAccessoriesType;

        private static Func<ChaControl, int, ChaAccessoryComponent> _getChaAccessoryCmp;

        private static bool MoreAccessoriesInstalled => _moreAccessoriesType != null;
        private static bool init = false;

        internal static ChaAccessoryComponent GetAccessory(this ChaControl character, int accessoryIndex)
        {
            if (!init)
                Init();

            return _getChaAccessoryCmp(character, accessoryIndex);
        }

        private static void Init()
        {
            DetectMoreAccessories();

            if (MoreAccessoriesInstalled)
            {
                var getAccCmpM = AccessTools.Method(_moreAccessoriesType, "GetChaAccessoryComponent");
                _getChaAccessoryCmp = (control, componentIndex) => (ChaAccessoryComponent)getAccCmpM.Invoke(_moreAccessoriesInstance, new object[] { control, componentIndex });
            }
            else
            {
                _getChaAccessoryCmp = (control, i) => control.cusAcsCmp[i];
            }
            init = true;
        }

        private static void DetectMoreAccessories()
        {
            try
            {
                _moreAccessoriesType = Type.GetType("MoreAccessoriesKOI.MoreAccessories, MoreAccessories, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");

                if (_moreAccessoriesType != null)
                    _moreAccessoriesInstance = UnityEngine.Object.FindObjectOfType(_moreAccessoriesType);
            }
            catch
            {
                _moreAccessoriesType = null;
            }
        }
    }

}
