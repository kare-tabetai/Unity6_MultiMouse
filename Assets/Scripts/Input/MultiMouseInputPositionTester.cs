/*
 * Unity6_MultiMouse Project
 * Copyright (c) 2025 Unity6_MultiMouse Contributors
 * License: MIT
 * 
 * テスト用スクリプト: ユーザーストーリー 3
 * マウス位置追跡の動作確認用です。
 */

using UnityEngine;
using Unity6MultiMouse.Input;

public class MultiMouseInputPositionTester : MonoBehaviour
{
    private int _frameCounter = 0;
    
    private void Update()
    {
        // フレームスキップ（毎フレーム出力は冗長なため、10 フレームごと）
        _frameCounter++;
        if (_frameCounter < 10)
            return;
        
        _frameCounter = 0;
        
        var mice = MultiMouseInput.GetAllMice();
        
        foreach (var mouse in mice)
        {
            Debug.Log($"[US3-Test] マウス {mouse.DeviceId}: " +
                      $"位置=({mouse.PositionX}, {mouse.PositionY}), " +
                      $"デルタ=({mouse.DeltaX}, {mouse.DeltaY})");
        }
    }
}
