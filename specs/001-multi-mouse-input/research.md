# 研究ドキュメント: マルチマウス入力マネージャー

**フェーズ**: 0 - 研究と明確化  
**生成日**: 2025-11-14  
**ステータス**: 完成

---

## 不明点の解決

### NEEDS CLARIFICATION #1: 複数マウスを同時追跡する最小限の実装パターンは？

#### ポイント 1: 決定事項

**Raw Input API（GetRawInputDeviceList + GetRawInputData）を使用**し、フレーム単位でデバイスポーリングを実行するパターンを採用。

#### ポイント 2: 根拠

1. **標準 InputManager の制限**
   - Unity の InputManager は単一マウスのみをサポート
   - 複数マウスの同時独立追跡には Raw Input API（Windows OS レベル）が必須

2. **Raw Input API の採用理由**
   - GetRawInputDeviceList：接続デバイス列挙
   - GetRawInputData：フレーム内のイベント（ボタン、移動）取得
   - デバイス ID で各マウスを独立識別可能

3. **ポーリング方式の選択**
   - イベント駆動（WndProc 非フック）は初学者向けとして複雑
   - フレーム単位ポーリング（Update 内 GetRawInputData）は理解しやすく、Unity のゲームループ統合も容易

#### ポイント 3: 初学者向けシンプル化

```csharp
// 最小限パターン：毎フレーム GetRawInputDeviceList で接続デバイス列挙
// → GetRawInputData で入力データ取得 → キャッシュ更新
// 複数マウスの並列処理は Linq 不使用、単純 foreach で実装
```

---

### NEEDS CLARIFICATION #2: Raw Input API（GetRawInputData）の初学者向け説明方法は？

#### ポイント 1: 決定事項

**インラインコメント + PlatformInput.cs で Windows API の低レベル詳細を集約**し、MultiMouseInput.cs では「マウスデータ」の高レベル操作に集中。

#### ポイント 2: 実装パターン

**PlatformInput.cs（Windows API 層）**:

```csharp
/// <summary>
/// マウス入力デバイスの列挙と生データ取得を担当。
/// Windows API：https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getrawinputdevicelist
/// </summary>
public static class PlatformInput
{
    // DllImport 宣言：GetRawInputDeviceList、GetRawInputData など
    // パラメータの詳細説明注釈を全て含む
}
```

**MultiMouseInput.cs（管理層）**:

```csharp
/// <summary>
/// 複数マウスからの入力を取得・管理する。
/// PlatformInput から低レベル API を呼び出し、高レベルなマウスデータ構造を構築。
/// </summary>
public class MultiMouseInput : MonoBehaviour
{
    public static List<Mouse> GetAllMice() { /* ... */ }
}
```

#### ポイント 3: コメント戦略

- **PlatformInput**：Windows API リファレンス完全記載、パラメータ型・意味を詳述
- **MultiMouseInput**：「何をしているか」「なぜか」、API 呼び出しの目的を記載、実装の「なぜ」は省略（PlatformInput を参照）

---

### NEEDS CLARIFICATION #3: サンプルコードの著作権・ライセンス表示方法は？

#### ポイント 1: 決定事項

**各スクリプトファイル先頭に Copyright ブロックを記載**；README.md に全体ライセンスを明記。

#### ポイント 2: 実装パターン

```csharp
/*
 * Unity6_MultiMouse Project
 * Copyright (c) 2025 [プロジェクト所有者/組織]
 * License: [MIT, Apache 2.0, etc. - プロジェクト方針に従う]
 * 
 * このスクリプトはサンプルコードです。
 * 自由に複製、修正、配布できます（ライセンス条件下で）。
 */

using System;
using UnityEngine;

namespace Unity6MultiMouse.Input
{
    // ... コード本体
}
```

**README.md**:

```markdown
## ライセンス

このプロジェクトは MIT ライセンスの下で配布されています。  
詳細は [LICENSE](./LICENSE) ファイルを参照してください。

## 使用および配布

- このサンプルコードは自由に複製・修正・配布できます。
- 商用利用可能です。
- ライセンス通知の削除は禁止です。
```

#### ポイント 3: 著作権表示の根拠

- **ファイル単位表示**：初学者がコピペ時に著作権情報を保持しやすい
- **README で統合表示**：プロジェクト全体の方針を一元管理

---

## ベストプラクティス（研究結果）

### Raw Input API 設計のベストプラクティス

#### 1. デバイス ID の永続管理

**決定**: 各マウスを HANDLE（IntPtr）で識別し、フレーム間で一貫性を保つ。

```csharp
public class Mouse
{
    public IntPtr DeviceHandle { get; private set; }  // Windows HANDLE
    public int DeviceId { get; private set; }         // User-friendly ID (0, 1, 2...)
    // ... その他プロパティ
}
```

**根拠**:

- Windows は HANDLE で各デバイスを一意識別
- ユーザー向けは 0, 1, 2... の序号で シンプル化

#### 2. 入力イベント vs ポーリング

**決定**: フレーム単位ポーリング（毎フレーム GetRawInputData 呼び出し）

**根拠**:

- **イベント駆動（WndProc）**: 高パフォーマンスだが、Unity ゲームループ統合に複雑な処理が必要
- **ポーリング**: 少し遅延があるが（< 1ms）、Update/LateUpdate に簡潔に統合可能で初学者向け

#### 3. ボタン状態の追跡

**決定**: 前フレーム状態をキャッシュし、現フレーム状態と比較（IsPressed, IsReleased, IsHeld の判定）

```csharp
public class MouseButton
{
    public bool IsPressed { get; }   // 今フレーム初めて押下
    public bool IsHeld { get; }      // 保持中
    public bool IsReleased { get; }  // 今フレーム初めてリリース
}
```

**根拠**:

- Rawデータは「押下」「リリース」のエッジのみ。前状態比較で押下・保持・リリースを判定
- ゲーム開発の標準パターン（Unity InputManager に同等の概念）

#### 4. 移動デルタの計算

**決定**: 前フレーム座標をキャッシュ、現フレーム座標と差分（Delta X、Delta Y）

```csharp
public class Mouse
{
    public int PositionX { get; private set; }
    public int PositionY { get; private set; }
    public int DeltaX { get; private set; }  // 前フレーム比較
    public int DeltaY { get; private set; }
}
```

**根拠**:

- Raw Input API はスクリーン絶対座標のみ提供（デルタなし）
- デルタはゲーム開発で必須（カメラ回転、エイム制御）
- キャッシュで 1 フレーム遅延、ポーリング方式では許容

#### 5. 初期化と終了処理

**決定**: MonoBehaviour の Start/OnDestroy で RAWINPUT 登録解除を実施

```csharp
public class MultiMouseInput : MonoBehaviour
{
    private void Start()
    {
        RegisterRawInput();  // Windows API 登録
    }

    private void OnDestroy()
    {
        UnregisterRawInput();  // 終了時クリーンアップ
    }
}
```

**根拠**:

- Raw Input は Windows OS レベルリソース、メモリリークを防止
- MonoBehaviour ライフサイクルとの統合

---

## 選択肢の検討と却下理由

### 検討した代替案

| 案 | 採択 | 却下理由 |
|-----|------|--------|
| **DirectInput API** | ❌ | レガシー API、Modern Raw Input より複雑、初学者向けでない |
| **イベント駆動（WndProc）** | ❌ | Window 非表示対応が複雑、Unity ゲームループとの統合困難 |
| **第三者ライブラリ（InputSystem など）** | ❌ | 単一マウス前提、複数マウスサポートなし、憲法違反 |
| **HID API（hid.dll）** | ❌ | Raw Input より低レベル、初学者向けでない |

---

## 参考ドキュメント

### Windows API リファレンス

- [GetRawInputDeviceList](https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getrawinputdevicelist)
- [GetRawInputData](https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getrawinputdata)
- [RegisterRawInputDevices](https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-registerrawinputdevices)
- [GetCursorPos](https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getcursorpos)

### Unity ゲームループ

- [MonoBehaviour.Update](https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html)
- [MonoBehaviour.OnDestroy](https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnDestroy.html)

---

**バージョン**: 1.0.0 | **生成日**: 2025-11-14 | **ステータス**: 完成

**次フェーズ**: Phase 1 設計（data-model.md、quickstart.md、contracts/）
