/*
 * Unity6_MultiMouse Project
 * Copyright (c) 2025 Unity6_MultiMouse Contributors
 * License: MIT
 */

using UnityEngine;
using UnityEngine.UI;
using Unity6MultiMouse.Input;

namespace Unity6MultiMouse.Demo
{
    /// <summary>
    /// マルチマウスデモシーンの UI 更新スクリプト。
    /// 各マウスの情報（ID、位置、ボタン状態）を画面に表示します。
    /// </summary>
    public class MultiMouseDemoUI : MonoBehaviour
    {
        [SerializeField]
        private Text _infoText;

        private void Update()
        {
            if (_infoText == null)
            {
                _infoText = FindObjectOfType<Text>();
                if (_infoText == null)
                    return;
            }

            // UI テキスト更新
            var mice = MultiMouseInput.GetAllMice();
            string infoString = $"接続マウス数: {mice.Count}\n\n";

            foreach (var mouse in mice)
            {
                infoString += $"=== マウス {mouse.DeviceId} ===\n";
                infoString += $"位置: ({mouse.PositionX}, {mouse.PositionY})\n";
                infoString += $"デルタ: ({mouse.DeltaX}, {mouse.DeltaY})\n";
                
                string buttonInfo = "ボタン: ";
                if (mouse.LeftButton.IsPressed) buttonInfo += "[L:↓] ";
                else if (mouse.LeftButton.IsHeld) buttonInfo += "[L:●] ";
                else buttonInfo += "[L:○] ";
                
                if (mouse.RightButton.IsPressed) buttonInfo += "[R:↓] ";
                else if (mouse.RightButton.IsHeld) buttonInfo += "[R:●] ";
                else buttonInfo += "[R:○] ";
                
                if (mouse.MiddleButton.IsPressed) buttonInfo += "[M:↓] ";
                else if (mouse.MiddleButton.IsHeld) buttonInfo += "[M:●] ";
                else buttonInfo += "[M:○] ";
                
                infoString += buttonInfo + "\n\n";
            }

            _infoText.text = infoString;
        }
    }
}
