/*
 * Unity6_MultiMouse Project
 * Copyright (c) 2025 Unity6_MultiMouse Contributors
 * License: MIT
 * 
 * テスト用スクリプト: ユーザーストーリー 2
 * マウスボタン検出の動作確認用です。
 */

using UnityEngine;
using Unity6MultiMouse.Input;

public class MultiMouseInputButtonTester : MonoBehaviour
{
    private void Update()
    {
        var mice = MultiMouseInput.GetAllMice();
        
        foreach (var mouse in mice)
        {
            // ボタン状態をログ出力
            string buttonStatus = $"マウス {mouse.DeviceId}: ";
            
            if (mouse.LeftButton.IsPressed)
                buttonStatus += "[L:Press] ";
            if (mouse.LeftButton.IsHeld)
                buttonStatus += "[L:Held] ";
            if (mouse.LeftButton.IsReleased)
                buttonStatus += "[L:Release] ";
            
            if (mouse.RightButton.IsPressed)
                buttonStatus += "[R:Press] ";
            if (mouse.RightButton.IsHeld)
                buttonStatus += "[R:Held] ";
            if (mouse.RightButton.IsReleased)
                buttonStatus += "[R:Release] ";
            
            if (mouse.MiddleButton.IsPressed)
                buttonStatus += "[M:Press] ";
            if (mouse.MiddleButton.IsHeld)
                buttonStatus += "[M:Held] ";
            if (mouse.MiddleButton.IsReleased)
                buttonStatus += "[M:Release] ";

            // ボタン押下イベントが発生した場合のみ出力
            if (mouse.LeftButton.IsPressed || mouse.RightButton.IsPressed || 
                mouse.MiddleButton.IsPressed || mouse.LeftButton.IsReleased || 
                mouse.RightButton.IsReleased || mouse.MiddleButton.IsReleased)
            {
                Debug.Log($"[US2-Test] {buttonStatus}");
            }
        }
    }
}
