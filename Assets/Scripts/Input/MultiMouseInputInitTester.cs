/*
 * Unity6_MultiMouse Project
 * Copyright (c) 2025 Unity6_MultiMouse Contributors
 * License: MIT
 * 
 * テスト用スクリプト: ユーザーストーリー 1
 * マウス検出・初期化の動作確認用です。
 */

using UnityEngine;
using Unity6MultiMouse.Input;

public class MultiMouseInputInitTester : MonoBehaviour
{
    private void Update()
    {
        // 毎フレーム接続マウス数を取得・表示
        int mouseCount = MultiMouseInput.GetMouseCount();
        
        Debug.Log($"[US1-Test] 接続マウス数: {mouseCount}");

        // すべての接続マウスを列挙
        var mice = MultiMouseInput.GetAllMice();
        foreach (var mouse in mice)
        {
            Debug.Log($"  マウス {mouse.DeviceId}: DeviceHandle={mouse.DeviceHandle}, 接続={mouse.IsConnected}");
        }
    }
}
