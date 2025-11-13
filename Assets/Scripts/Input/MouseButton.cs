/*
 * Unity6_MultiMouse Project
 * Copyright (c) 2025 Unity6_MultiMouse Contributors
 * License: MIT
 */

using UnityEngine;

namespace Unity6MultiMouse.Input
{
    /// <summary>
    /// マウスボタンの状態を表すクラス。
    /// 押下（IsPressed）、保持（IsHeld）、リリース（IsReleased）の3つのイベント状態を管理します。
    /// </summary>
    public class MouseButton
    {
        /// <summary>
        /// 今フレーム初めてボタンが押下されたか。
        /// 前フレーム未押下 → 今フレーム押下の遷移で true になり、次フレーム false に戻ります。
        /// </summary>
        public bool IsPressed { get; private set; }

        /// <summary>
        /// ボタンが現在押下中か。
        /// 押下中は毎フレーム true です。
        /// </summary>
        public bool IsHeld { get; private set; }

        /// <summary>
        /// 今フレーム初めてボタンがリリースされたか。
        /// 前フレーム押下 → 今フレーム未押下の遷移で true になり、次フレーム false に戻ります。
        /// </summary>
        public bool IsReleased { get; private set; }

        /// <summary>
        /// 前フレームのボタン押下状態。
        /// 状態遷移判定用に内部で使用します。
        /// </summary>
        private bool _previousState;

        /// <summary>
        /// 新しい MouseButton インスタンスを作成します。
        /// </summary>
        public MouseButton()
        {
            IsPressed = false;
            IsHeld = false;
            IsReleased = false;
            _previousState = false;
        }

        /// <summary>
        /// ボタン状態を更新します。
        /// 前フレーム状態と比較して、IsPressed/IsHeld/IsReleased を計算します。
        /// </summary>
        /// <param name="currentState">現在のボタン押下状態（true = 押下中、false = 未押下）</param>
        public void UpdateState(bool currentState)
        {
            // 状態遷移ロジック:
            // 前フレーム未押下 → 今フレーム押下: IsPressed = true, IsHeld = true
            // 前フレーム押下中 → 今フレーム押下中: IsPressed = false, IsHeld = true
            // 前フレーム押下中 → 今フレーム未押下: IsReleased = true, IsHeld = false
            // 前フレーム未押下 → 今フレーム未押下: IsPressed = false, IsHeld = false, IsReleased = false

            if (!_previousState && currentState)
            {
                // ボタン押下開始
                IsPressed = true;
                IsHeld = true;
                IsReleased = false;
            }
            else if (_previousState && currentState)
            {
                // ボタン押下中（継続）
                IsPressed = false;
                IsHeld = true;
                IsReleased = false;
            }
            else if (_previousState && !currentState)
            {
                // ボタンリリース
                IsPressed = false;
                IsHeld = false;
                IsReleased = true;
            }
            else
            {
                // ボタン未押下（継続）
                IsPressed = false;
                IsHeld = false;
                IsReleased = false;
            }

            _previousState = currentState;
        }

        /// <summary>
        /// ボタン状態をリセットします。
        /// フレーム末やシーン遷移時に呼び出し、前フレーム状態をクリアします。
        /// </summary>
        public void Reset()
        {
            IsPressed = false;
            IsHeld = false;
            IsReleased = false;
            _previousState = false;
        }
    }
}
