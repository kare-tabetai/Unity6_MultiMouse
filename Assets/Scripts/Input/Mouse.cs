/*
 * Unity6_MultiMouse Project
 * Copyright (c) 2025 Unity6_MultiMouse Contributors
 * License: MIT
 */

using System;
using UnityEngine;

namespace Unity6MultiMouse.Input
{
    /// <summary>
    /// マウスデバイスの状態を表すクラス。
    /// 1 つの物理マウスに相当し、デバイス ID、位置、ボタン状態を保持します。
    /// </summary>
    public class Mouse
    {
        /// <summary>
        /// マウスの序号（0, 1, 2...）。ユーザー向けの識別子です。
        /// </summary>
        public int DeviceId { get; private set; }

        /// <summary>
        /// Windows API が割り当てたデバイスハンドル（IntPtr）。
        /// システム内で一意にマウスを識別します。
        /// </summary>
        public IntPtr DeviceHandle { get; private set; }

        /// <summary>
        /// スクリーン上の X 座標（ピクセル）。
        /// 左上が原点です。
        /// </summary>
        public int PositionX { get; private set; }

        /// <summary>
        /// スクリーン上の Y 座標（ピクセル）。
        /// 左上が原点です。
        /// </summary>
        public int PositionY { get; private set; }

        /// <summary>
        /// 前フレームからの X 軸移動量（ピクセル）。
        /// 正の値 = 右移動、負の値 = 左移動
        /// </summary>
        public int DeltaX { get; private set; }

        /// <summary>
        /// 前フレームからの Y 軸移動量（ピクセル）。
        /// 正の値 = 下移動、負の値 = 上移動
        /// </summary>
        public int DeltaY { get; private set; }

        /// <summary>
        /// 左ボタンの状態。
        /// IsPressed, IsHeld, IsReleased を提供します。
        /// </summary>
        public MouseButton LeftButton { get; private set; }

        /// <summary>
        /// 右ボタンの状態。
        /// </summary>
        public MouseButton RightButton { get; private set; }

        /// <summary>
        /// 中央（ホイール）ボタンの状態。
        /// </summary>
        public MouseButton MiddleButton { get; private set; }

        /// <summary>
        /// デバイスが接続中か。
        /// true = 接続、false = 切断予定
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// 最後に更新されたフレーム番号。
        /// 複数フレーム更新されないマウスを検出するのに使用します。
        /// </summary>
        public int LastUpdateFrame { get; private set; }

        /// <summary>
        /// 前フレームの X 座標（デルタ計算用）。
        /// </summary>
        private int _previousPositionX;

        /// <summary>
        /// 前フレームの Y 座標（デルタ計算用）。
        /// </summary>
        private int _previousPositionY;

        /// <summary>
        /// 新しい Mouse インスタンスを作成します。
        /// </summary>
        /// <param name="deviceId">マウスの序号（0, 1, 2...）</param>
        /// <param name="deviceHandle">Windows API デバイスハンドル</param>
        public Mouse(int deviceId, IntPtr deviceHandle)
        {
            DeviceId = deviceId;
            DeviceHandle = deviceHandle;
            PositionX = 0;
            PositionY = 0;
            DeltaX = 0;
            DeltaY = 0;
            IsConnected = true;
            LastUpdateFrame = Time.frameCount;

            _previousPositionX = 0;
            _previousPositionY = 0;

            // 3 つのボタンを初期化
            LeftButton = new MouseButton();
            RightButton = new MouseButton();
            MiddleButton = new MouseButton();
        }

        /// <summary>
        /// マウスの位置を更新します。
        /// 同時にデルタ（前フレーム比較）を計算します。
        /// </summary>
        /// <param name="x">新しい X 座標</param>
        /// <param name="y">新しい Y 座標</param>
        public void SetPosition(int x, int y)
        {
            _previousPositionX = PositionX;
            _previousPositionY = PositionY;

            PositionX = x;
            PositionY = y;

            // デルタ計算
            DeltaX = PositionX - _previousPositionX;
            DeltaY = PositionY - _previousPositionY;

            UpdateFrame();
        }

        /// <summary>
        /// 左ボタンの状態を更新します。
        /// </summary>
        /// <param name="isPressed">ボタンが押下中か</param>
        public void UpdateLeftButton(bool isPressed)
        {
            LeftButton.UpdateState(isPressed);
            UpdateFrame();
        }

        /// <summary>
        /// 右ボタンの状態を更新します。
        /// </summary>
        /// <param name="isPressed">ボタンが押下中か</param>
        public void UpdateRightButton(bool isPressed)
        {
            RightButton.UpdateState(isPressed);
            UpdateFrame();
        }

        /// <summary>
        /// 中央ボタンの状態を更新します。
        /// </summary>
        /// <param name="isPressed">ボタンが押下中か</param>
        public void UpdateMiddleButton(bool isPressed)
        {
            MiddleButton.UpdateState(isPressed);
            UpdateFrame();
        }

        /// <summary>
        /// 接続状態を更新します。
        /// </summary>
        /// <param name="isConnected">接続中か</param>
        public void SetConnected(bool isConnected)
        {
            IsConnected = isConnected;
            if (isConnected)
            {
                UpdateFrame();
            }
        }

        /// <summary>
        /// 最後の更新フレーム番号を現在のフレーム番号で更新します。
        /// 内部用メソッドです。
        /// </summary>
        private void UpdateFrame()
        {
            LastUpdateFrame = Time.frameCount;
        }
    }
}
