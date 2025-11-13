/*
 * Unity6_MultiMouse Project
 * Copyright (c) 2025 Unity6_MultiMouse Contributors
 * License: MIT
 */

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Unity6MultiMouse.Input
{
    /// <summary>
    /// 複数マウスからの入力を管理・集約するシングルトンマネージャー。
    /// Windows API の Raw Input API を使用して、複数接続マウスを独立して追跡します。
    /// MonoBehaviour として動作し、Update() でフレーム毎に入力状態を更新します。
    /// </summary>
    public class MultiMouseInput : MonoBehaviour
    {
        /// <summary>
        /// MultiMouseInput のシングルトンインスタンス。
        /// 静的メソッド（GetAllMice など）から参照されます。
        /// </summary>
        private static MultiMouseInput _instance;

        /// <summary>
        /// 接続マウスのリスト。DeviceId 順でソートされています。
        /// </summary>
        private List<Mouse> _mice = new List<Mouse>();

        /// <summary>
        /// Windows Raw Input デバイスハンドル配列。
        /// GetRawInputDeviceList で取得したマウスハンドルを保持します。
        /// </summary>
        private IntPtr[] _deviceHandles = new IntPtr[0];

        /// <summary>
        /// Raw Input マウスイベントが保留中かをフラグで管理。
        /// （Windows メッセージキューで Raw Input イベントが待機中）
        /// </summary>
        private bool _hasRawInputPending = false;

        // ============================================================================
        // MonoBehaviour Lifecycle
        // ============================================================================

        private void Start()
        {
            // シングルトンパターン: 複数インスタンスを防止
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            // Raw Input 初期化
            InitializeRawInput();
        }

        private void Update()
        {
            // 毎フレーム Raw Input デバイスを列挙し、接続状態を刷新
            EnumerateConnectedMice();

            // Raw Input イベントを処理（GetRawInputData）
            ProcessRawInputData();
        }

        private void OnDestroy()
        {
            // Raw Input デバイス登録解除、リソースクリーンアップ
            UnregisterRawInputDevices();
        }

        // ============================================================================
        // Public Static API
        // ============================================================================

        /// <summary>
        /// 接続されたすべてのマウスを取得します。
        /// </summary>
        /// <returns>マウスオブジェクトのリスト（コピー）。見つからない場合は空リスト。</returns>
        public static List<Mouse> GetAllMice()
        {
            if (_instance == null)
            {
                Debug.LogWarning("MultiMouseInput インスタンスが見つかりません。シーンに MultiMouseInput MonoBehaviour を配置してください。");
                return new List<Mouse>();
            }

            return new List<Mouse>(_instance._mice);
        }

        /// <summary>
        /// デバイス ID でマウスを取得します。
        /// </summary>
        /// <param name="deviceId">マウスの序号（0, 1, 2...）</param>
        /// <returns>マウスオブジェクト。見つからない場合は null。</returns>
        public static Mouse GetMouse(int deviceId)
        {
            if (_instance == null)
            {
                Debug.LogWarning("MultiMouseInput インスタンスが見つかりません。");
                return null;
            }

            foreach (var mouse in _instance._mice)
            {
                if (mouse.DeviceId == deviceId)
                {
                    return mouse;
                }
            }

            return null;
        }

        /// <summary>
        /// 接続マウス数を取得します。
        /// </summary>
        /// <returns>接続中のマウス個数。マウスがない場合は 0。</returns>
        public static int GetMouseCount()
        {
            if (_instance == null)
            {
                return 0;
            }

            return _instance._mice.Count;
        }

        // ============================================================================
        // Private Methods - Raw Input Initialization
        // ============================================================================

        /// <summary>
        /// Raw Input API を初期化します。
        /// RegisterRawInputDevices を呼び出し、マウス入力受け取りを登録します。
        /// </summary>
        private void InitializeRawInput()
        {
            try
            {
                RegisterRawInputDevices();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Raw Input 初期化エラー: {ex.Message}");
            }
        }

        /// <summary>
        /// Raw Input デバイスを Windows に登録します。
        /// Raw Input イベントが WndProc（Window Message）を通じて配信されるようにします。
        /// </summary>
        private void RegisterRawInputDevices()
        {
            // マウスの Raw Input を登録
            PlatformInput.RAWINPUTDEVICE[] devices = new PlatformInput.RAWINPUTDEVICE[1];

            devices[0].usUsagePage = 0x01;  // ジェネリックデスクトップページ
            devices[0].usUsage = 0x02;      // マウス
            devices[0].dwFlags = PlatformInput.RIDEV_INPUTSINK;  // バックグラウンドで入力受け取り
            devices[0].hwndTarget = IntPtr.Zero;  // このウィンドウに入力を配信

            bool success = PlatformInput.RegisterRawInputDevices(
                devices,
                (uint)devices.Length,
                (uint)Marshal.SizeOf(typeof(PlatformInput.RAWINPUTDEVICE))
            );

            if (!success)
            {
                Debug.LogWarning("RegisterRawInputDevices 失敗。Raw Input が機能しない可能性があります。");
            }
        }

        /// <summary>
        /// Raw Input デバイス登録を解除します。
        /// アプリケーション終了時に呼び出し、Windows リソースをクリーンアップします。
        /// </summary>
        private void UnregisterRawInputDevices()
        {
            try
            {
                PlatformInput.RAWINPUTDEVICE[] devices = new PlatformInput.RAWINPUTDEVICE[1];
                devices[0].usUsagePage = 0x01;
                devices[0].usUsage = 0x02;
                devices[0].dwFlags = 0;  // 登録解除フラグ

                PlatformInput.RegisterRawInputDevices(
                    devices,
                    (uint)devices.Length,
                    (uint)Marshal.SizeOf(typeof(PlatformInput.RAWINPUTDEVICE))
                );
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Raw Input 登録解除エラー: {ex.Message}");
            }
        }

        // ============================================================================
        // Private Methods - Device Enumeration
        // ============================================================================

        /// <summary>
        /// Windows に接続されているマウスを列挙し、_mice リストを更新します。
        /// 毎フレーム呼び出され、新規接続・切断を検出します。
        /// </summary>
        private void EnumerateConnectedMice()
        {
            try
            {
                // GetRawInputDeviceList でマウスデバイス列挙
                uint deviceCount = 0;
                uint result = PlatformInput.GetRawInputDeviceList(null, ref deviceCount, (uint)Marshal.SizeOf(typeof(PlatformInput.RAWINPUTDEVICELIST)));

                if (result == 0 || deviceCount == 0)
                {
                    // マウスが接続されていない
                    _deviceHandles = new IntPtr[0];
                    _mice.Clear();
                    return;
                }

                // デバイスリスト取得
                PlatformInput.RAWINPUTDEVICELIST[] deviceList = new PlatformInput.RAWINPUTDEVICELIST[deviceCount];
                result = PlatformInput.GetRawInputDeviceList(deviceList, ref deviceCount, (uint)Marshal.SizeOf(typeof(PlatformInput.RAWINPUTDEVICELIST)));

                // マウスデバイスのみをフィルタリング
                List<IntPtr> mouseHandles = new List<IntPtr>();
                for (int i = 0; i < deviceCount; i++)
                {
                    if (deviceList[i].dwType == PlatformInput.RIM_TYPEMOUSE)
                    {
                        mouseHandles.Add(deviceList[i].hDevice);
                    }
                }

                // デバイスハンドル配列を更新
                _deviceHandles = mouseHandles.ToArray();

                // _mice リストを更新
                UpdateMouseList();
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"デバイス列挙エラー: {ex.Message}");
            }
        }

        /// <summary>
        /// 検出されたマウスハンドルに基づいて _mice リストを更新します。
        /// 新規接続マウスを追加、切断マウスを削除します。
        /// </summary>
        private void UpdateMouseList()
        {
            // 既存マウスの接続状態をリセット
            foreach (var mouse in _mice)
            {
                mouse.SetConnected(false);
            }

            // 現在接続されているマウスを追加・更新
            for (int i = 0; i < _deviceHandles.Length; i++)
            {
                IntPtr handle = _deviceHandles[i];

                // 既存マウスをチェック
                bool found = false;
                foreach (var mouse in _mice)
                {
                    if (mouse.DeviceHandle == handle)
                    {
                        mouse.SetConnected(true);
                        found = true;
                        break;
                    }
                }

                // 新規接続マウスを追加
                if (!found)
                {
                    Mouse newMouse = new Mouse(i, handle);
                    _mice.Add(newMouse);
                }
            }

            // 切断マウスをリストから削除
            _mice.RemoveAll(m => !m.IsConnected);
        }

        // ============================================================================
        // Private Methods - Raw Input Data Processing
        // ============================================================================

        /// <summary>
        /// Raw Input イベントデータを処理します。
        /// GetRawInputData で各マウスのボタン・位置データを取得し、_mice リストを更新します。
        /// </summary>
        private void ProcessRawInputData()
        {
            try
            {
                foreach (var mouse in _mice)
                {
                    // 各マウスの Raw Input データを取得
                    uint dataSize = (uint)Marshal.SizeOf(typeof(PlatformInput.RAWINPUT));
                    PlatformInput.RAWINPUT rawInput;

                    uint result = PlatformInput.GetRawInputData(
                        mouse.DeviceHandle,
                        PlatformInput.RID_INPUT,
                        out rawInput,
                        ref dataSize,
                        (uint)Marshal.SizeOf(typeof(PlatformInput.RAWINPUTHEADER))
                    );

                    if (result > 0 && rawInput.header.dwType == PlatformInput.RIM_TYPEMOUSE)
                    {
                        // マウスボタン状態を更新
                        UpdateMouseButtonStates(mouse, rawInput.mouse.usButtonFlags);

                        // マウス位置を更新（GetCursorPos で絶対座標）
                        if (PlatformInput.GetCursorPos(out PlatformInput.POINT point))
                        {
                            mouse.SetPosition(point.X, point.Y);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Raw Input データ処理エラー: {ex.Message}");
            }
        }

        /// <summary>
        /// Raw Input マウスボタンフラグを解析し、Mouse のボタン状態を更新します。
        /// </summary>
        /// <param name="mouse">更新対象のマウスオブジェクト</param>
        /// <param name="buttonFlags">Raw Input ボタンフラグ（RI_MOUSE_LEFT_BUTTON_DOWN など）</param>
        private void UpdateMouseButtonStates(Mouse mouse, ushort buttonFlags)
        {
            // 左ボタン
            bool leftPressed = (buttonFlags & PlatformInput.RI_MOUSE_LEFT_BUTTON_DOWN) != 0;
            bool leftReleased = (buttonFlags & PlatformInput.RI_MOUSE_LEFT_BUTTON_UP) != 0;
            if (leftPressed || leftReleased)
            {
                mouse.UpdateLeftButton(leftPressed);
            }

            // 右ボタン
            bool rightPressed = (buttonFlags & PlatformInput.RI_MOUSE_RIGHT_BUTTON_DOWN) != 0;
            bool rightReleased = (buttonFlags & PlatformInput.RI_MOUSE_RIGHT_BUTTON_UP) != 0;
            if (rightPressed || rightReleased)
            {
                mouse.UpdateRightButton(rightPressed);
            }

            // 中央ボタン
            bool middlePressed = (buttonFlags & PlatformInput.RI_MOUSE_MIDDLE_BUTTON_DOWN) != 0;
            bool middleReleased = (buttonFlags & PlatformInput.RI_MOUSE_MIDDLE_BUTTON_UP) != 0;
            if (middlePressed || middleReleased)
            {
                mouse.UpdateMiddleButton(middlePressed);
            }
        }
    }
}
