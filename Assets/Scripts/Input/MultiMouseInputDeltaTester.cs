/*
 * Unity6_MultiMouse Project
 * Copyright (c) 2025 Unity6_MultiMouse Contributors
 * License: MIT
 * 
 * テスト用スクリプト: ユーザーストーリー 4
 * マウス移動デルタ追跡の動作確認用です。
 */

using UnityEngine;
using Unity6MultiMouse.Input;

public class MultiMouseInputDeltaTester : MonoBehaviour
{
    private void Update()
    {
        var mice = MultiMouseInput.GetAllMice();
        
        foreach (var mouse in mice)
        {
            // デルタが 0 でない場合のみログ出力
            if (mouse.DeltaX != 0 || mouse.DeltaY != 0)
            {
                Debug.Log($"[US4-Test] マウス {mouse.DeviceId}: " +
                          $"デルタ=({mouse.DeltaX}, {mouse.DeltaY})");
            }
        }
    }
}
