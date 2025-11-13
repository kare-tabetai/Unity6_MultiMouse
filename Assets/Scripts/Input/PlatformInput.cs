/*
 * Unity6_MultiMouse Project
 * Copyright (c) 2025 Unity6_MultiMouse Contributors
 * License: MIT
 * 
 * このスクリプトはサンプルコードです。
 * Windows API への DllImport アクセスを提供します。
 * 自由に複製、修正、配布できます（ライセンス条件下で）。
 */

using System;
using System.Runtime.InteropServices;

namespace Unity6MultiMouse.Input
{
    /// <summary>
    /// Windows API (user32.dll) の DllImport 集約層。
    /// Raw Input API を使用した複数マウス入力の低レベルアクセスを提供します。
    /// </summary>
    public static class PlatformInput
    {
        // ============================================================================
        // Raw Input API Constants
        // ============================================================================

        /// <summary>
        /// Raw Input デバイスタイプ: マウス
        /// 参考: https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getrawinputdevicelist
        /// </summary>
        public const uint RIM_TYPEMOUSE = 0;

        /// <summary>
        /// Raw Input コマンド: 入力データ
        /// </summary>
        public const uint RID_INPUT = 0x10000003;

        /// <summary>
        /// Raw Input マウスボタンフラグ: 左ボタン押下
        /// </summary>
        public const ushort RI_MOUSE_LEFT_BUTTON_DOWN = 0x0001;

        /// <summary>
        /// Raw Input マウスボタンフラグ: 左ボタンリリース
        /// </summary>
        public const ushort RI_MOUSE_LEFT_BUTTON_UP = 0x0002;

        /// <summary>
        /// Raw Input マウスボタンフラグ: 右ボタン押下
        /// </summary>
        public const ushort RI_MOUSE_RIGHT_BUTTON_DOWN = 0x0004;

        /// <summary>
        /// Raw Input マウスボタンフラグ: 右ボタンリリース
        /// </summary>
        public const ushort RI_MOUSE_RIGHT_BUTTON_UP = 0x0008;

        /// <summary>
        /// Raw Input マウスボタンフラグ: 中央ボタン押下
        /// </summary>
        public const ushort RI_MOUSE_MIDDLE_BUTTON_DOWN = 0x0010;

        /// <summary>
        /// Raw Input マウスボタンフラグ: 中央ボタンリリース
        /// </summary>
        public const ushort RI_MOUSE_MIDDLE_BUTTON_UP = 0x0020;

        /// <summary>
        /// RegisterRawInputDevices フラグ: デバイス登録
        /// </summary>
        public const uint RIDEV_INPUTSINK = 0x00000100;

        // ============================================================================
        // Structures
        // ============================================================================

        /// <summary>
        /// Raw Input デバイスリスト用構造体
        /// 参考: https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-rawinputdevicelist
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUTDEVICELIST
        {
            /// <summary>
            /// デバイスハンドル（HANDLE）
            /// </summary>
            public IntPtr hDevice;

            /// <summary>
            /// デバイスタイプ（RIM_TYPEMOUSE など）
            /// </summary>
            public uint dwType;
        }

        /// <summary>
        /// Raw Input マウスデータ用構造体
        /// 参考: https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-rawmouse
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RAWMOUSE
        {
            /// <summary>
            /// マウスボタンフラグ
            /// RI_MOUSE_LEFT_BUTTON_DOWN など各フラグの OR 値
            /// </summary>
            public ushort usButtonFlags;

            /// <summary>
            /// X 軸移動（相対位置）
            /// </summary>
            public short lLastX;

            /// <summary>
            /// Y 軸移動（相対位置）
            /// </summary>
            public short lLastY;
        }

        /// <summary>
        /// Raw Input データ用構造体
        /// 参考: https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-rawinput
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUT
        {
            /// <summary>
            /// Raw Input ヘッダー
            /// </summary>
            public RAWINPUTHEADER header;

            /// <summary>
            /// Raw Input マウスデータ
            /// </summary>
            public RAWMOUSE mouse;
        }

        /// <summary>
        /// Raw Input ヘッダー構造体
        /// 参考: https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-rawinputheader
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUTHEADER
        {
            /// <summary>
            /// Raw Input データタイプ（RIM_TYPEMOUSE など）
            /// </summary>
            public uint dwType;

            /// <summary>
            /// Raw Input データサイズ
            /// </summary>
            public uint dwSize;

            /// <summary>
            /// デバイスハンドル（HANDLE）
            /// </summary>
            public IntPtr hDevice;

            /// <summary>
            /// WPARAM（ウィンドウプロシージャパラメータ）
            /// </summary>
            public IntPtr wParam;
        }

        /// <summary>
        /// Raw Input デバイス登録用構造体
        /// 参考: https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-rawinputdevice
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUTDEVICE
        {
            /// <summary>
            /// デバイスタイプ（RIM_TYPEMOUSE など）
            /// </summary>
            public ushort usUsagePage;

            /// <summary>
            /// デバイス使用（0x02 = マウス）
            /// </summary>
            public ushort usUsage;

            /// <summary>
            /// Raw Input フラグ（RIDEV_INPUTSINK など）
            /// </summary>
            public uint dwFlags;

            /// <summary>
            /// ターゲットウィンドウハンドル
            /// </summary>
            public IntPtr hwndTarget;
        }

        /// <summary>
        /// スクリーン座標用構造体
        /// 参考: https://docs.microsoft.com/en-us/windows/win32/api/windef/ns-windef-point
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            /// <summary>
            /// X 座標
            /// </summary>
            public int X;

            /// <summary>
            /// Y 座標
            /// </summary>
            public int Y;
        }

        // ============================================================================
        // DllImport Declarations
        // ============================================================================

        /// <summary>
        /// 接続された Raw Input デバイスのリストを取得します。
        /// </summary>
        /// <param name="pRawInputDeviceList">デバイスリスト配列へのポインタ</param>
        /// <param name="puiNumDevices">デバイス数へのポインタ</param>
        /// <param name="cbSize">RAWINPUTDEVICELIST 構造体のサイズ</param>
        /// <returns>取得されたデバイス数</returns>
        /// <remarks>
        /// 参考: https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getrawinputdevicelist
        /// </remarks>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetRawInputDeviceList(
            [In, Out] RAWINPUTDEVICELIST[] pRawInputDeviceList,
            [In, Out] ref uint puiNumDevices,
            uint cbSize);

        /// <summary>
        /// Raw Input デバイスから生入力データを取得します。
        /// </summary>
        /// <param name="hRawInput">Raw Input ハンドル</param>
        /// <param name="uiCommand">コマンド（RID_INPUT など）</param>
        /// <param name="pData">Raw Input データバッファ</param>
        /// <param name="pcbSize">バッファサイズへのポインタ</param>
        /// <returns>取得したデータサイズ</returns>
        /// <remarks>
        /// 参考: https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getrawinputdata
        /// </remarks>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetRawInputData(
            IntPtr hRawInput,
            uint uiCommand,
            [Out] out RAWINPUT pData,
            [In, Out] ref uint pcbSize,
            uint cbSizeHeader);

        /// <summary>
        /// Raw Input デバイスを Windows に登録します。
        /// </summary>
        /// <param name="pRawInputDevices">登録デバイス配列へのポインタ</param>
        /// <param name="uiNumDevices">登録デバイス数</param>
        /// <param name="cbSize">RAWINPUTDEVICE 構造体のサイズ</param>
        /// <returns>登録成功時は TRUE</returns>
        /// <remarks>
        /// 参考: https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-registerrawinputdevices
        /// </remarks>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RegisterRawInputDevices(
            [In] RAWINPUTDEVICE[] pRawInputDevices,
            uint uiNumDevices,
            uint cbSize);

        /// <summary>
        /// 現在のマウスカーソル位置（スクリーン座標）を取得します。
        /// </summary>
        /// <param name="lpPoint">座標を受け取る POINT 構造体へのポインタ</param>
        /// <returns>成功時は TRUE</returns>
        /// <remarks>
        /// 参考: https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getcursorpos
        /// 注意: これは OS レベルのグローバルマウスカーソル位置です。
        /// Raw Input の相対位置と組み合わせて使用します。
        /// </remarks>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(out POINT lpPoint);

        /// <summary>
        /// Raw Input デバイスの情報を取得します。
        /// </summary>
        /// <param name="hDevice">デバイスハンドル</param>
        /// <param name="uiCommand">コマンド（RIDI_PREPARSEDDATA など）</param>
        /// <param name="pData">情報バッファ</param>
        /// <param name="pcbSize">バッファサイズへのポインタ</param>
        /// <returns>取得したデータサイズ</returns>
        /// <remarks>
        /// 参考: https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getrawinputdeviceinfo
        /// </remarks>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetRawInputDeviceInfo(
            IntPtr hDevice,
            uint uiCommand,
            IntPtr pData,
            ref uint pcbSize);
    }
}
