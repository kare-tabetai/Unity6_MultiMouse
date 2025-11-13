# データモデル: マルチマウス入力マネージャー

**フェーズ**: 1 - 設計  
**生成日**: 2025-11-14  
**ステータス**: 完成

---

## エンティティ定義

### 1. Mouse（マウスデバイス）

マウス入力デバイスの状態を表すエンティティ。

#### フィールド

| フィールド | 型 | 説明 | 検証規則 |
|-----------|-----|------|--------|
| `DeviceHandle` | `IntPtr` | Windows API が割り当てたデバイスハンドル | 非 NULL（IntPtr.Zero 以外） |
| `DeviceId` | `int` | ユーザー向けデバイス序号（0, 1, 2...） | >= 0、ユニーク |
| `PositionX` | `int` | スクリーン上の X 座標（ピクセル） | >= 0 |
| `PositionY` | `int` | スクリーン上の Y 座標（ピクセル） | >= 0 |
| `DeltaX` | `int` | 前フレームからの X 位置変化量 | 任意の整数 |
| `DeltaY` | `int` | 前フレームからの Y 位置変化量 | 任意の整数 |
| `LeftButton` | `MouseButton` | 左ボタンの状態 | 非 NULL |
| `RightButton` | `MouseButton` | 右ボタンの状態 | 非 NULL |
| `MiddleButton` | `MouseButton` | 中央ボタンの状態 | 非 NULL |
| `IsConnected` | `bool` | デバイスが接続中か | - |
| `LastUpdateFrame` | `int` | 最後に更新されたフレーム番号 | >= 0 |

#### 関係

- **1 マウス : N ボタン** (LeftButton, RightButton, MiddleButton)
- **1 マウス : 1 MultiMouseInput マネージャー**

#### 状態遷移

```text
[切断] 
   ↓
[接続 + 初期化] → [入力受け取り] → [フレーム更新]
                       ↑              ↓
                       └──────────────┘
   ↓
[切断]
```

---

### 2. MouseButton（ボタン状態）

マウスボタン（左、右、中央）の状態を表すエンティティ。

#### フィールド

| フィールド | 型 | 説明 | 検証規則 |
|-----------|-----|------|--------|
| `IsPressed` | `bool` | 今フレーム初めて押下されたか | - |
| `IsHeld` | `bool` | ボタンが押下され続けているか | - |
| `IsReleased` | `bool` | 今フレーム初めてリリースされたか | - |
| `PreviousState` | `bool` | 前フレーム状態（内部用） | - |

#### ロジック

```text
前フレーム: 未押下 → 今フレーム: 押下   → IsPressed = true, IsHeld = true
前フレーム: 押下中 → 今フレーム: 押下中 → IsPressed = false, IsHeld = true
前フレーム: 押下中 → 今フレーム: 未押下 → IsReleased = true, IsHeld = false
前フレーム: 未押下 → 今フレーム: 未押下 → IsPressed = false, IsHeld = false, IsReleased = false
```

---

### 3. MultiMouseInput（マネージャー）

全マウスからの入力を管理・集約するシングルトンマネージャー。

#### フィールド（内部）

| フィールド | 型 | 説明 |
|-----------|-----|------|
| `_mice` | `List<Mouse>` | 接続マウスのリスト |
| `_deviceHandles` | `IntPtr[]` | Windows Raw Input デバイスハンドル配列 |
| `_lastMouseStates` | `Dictionary<IntPtr, MouseState>` | 前フレーム状態キャッシュ（デルタ計算用） |

#### パブリックメソッド

| メソッド | 戻り値 | パラメータ | 説明 |
|---------|--------|-----------|------|
| `GetAllMice()` | `List<Mouse>` | なし | 接続マウス全て取得（コピーリスト） |
| `GetMouse(int id)` | `Mouse \| null` | `id: int` | デバイス ID で特定マウス取得 |
| `GetMouseCount()` | `int` | なし | 接続マウス数 |

#### 内部メソッド

| メソッド | 説明 |
|---------|------|
| `InitializeRawInput()` | Windows Raw Input API 初期化 |
| `UpdateInputFrame()` | フレーム内入力データ取得・キャッシュ更新 |
| `RegisterRawInputDevices()` | Raw Input デバイス登録 |
| `UnregisterRawInputDevices()` | Raw Input デバイス登録解除 |
| `EnumerateConnectedMice()` | 接続マウスを GetRawInputDeviceList で列挙 |
| `ProcessRawInputData()` | GetRawInputData から生入力を処理 |
| `UpdateMouseButtonStates()` | ボタン状態（IsPressed/IsHeld/IsReleased）を計算 |
| `UpdateMousePositionDelta()` | デルタ座標を前フレーム座標と比較計算 |

---

## 検証規則

### Mouse エンティティ

1. **デバイス一意性**
   - 各マウスは `DeviceHandle` で一意識別
   - 同一 `DeviceId` は存在しない

2. **座標有効性**
   - `PositionX`, `PositionY` は画面サイズ内（0 以上）
   - GetCursorPos の結果をそのまま使用

3. **デルタ範囲**
   - `DeltaX`, `DeltaY` は前フレーム座標との差分
   - 1 フレーム遅延を許容

4. **接続状態**
   - `IsConnected = true` の場合、DeviceHandle は有効
   - `IsConnected = false` の場合、リスト削除予定

5. **フレーム更新頻度**
   - `LastUpdateFrame` は毎フレーム更新
   - 複数フレーム更新されないマウスは切断判定

### MouseButton エンティティ

1. **ボタン状態の相互排他性**
   - `IsPressed` と `IsReleased` は同フレーム内で同時発生しない
   - `IsPressed = true` なら `IsHeld = true`

2. **状態遷移の正当性**
   - 前フレーム `IsHeld = false` + 今フレーム `IsPressed = false` の場合、`IsHeld` も `false`

---

## 関係図

```text
┌─────────────────────────────┐
│   MultiMouseInput           │
│   (マネージャー)             │
├─────────────────────────────┤
│ - _mice: List<Mouse>        │
│ - _deviceHandles: IntPtr[]  │
│ - _lastMouseStates: Dict    │
├─────────────────────────────┤
│ + GetAllMice()              │
│ + GetMouse(id)              │
│ + GetMouseCount()           │
└─────────────────────────────┘
          ▲
          │ 1:N
          │
    ┌─────────────┐
    │   Mouse     │
    │  (デバイス)  │
    ├─────────────┤
    │ - DeviceId  │
    │ - Position  │
    │ - Delta     │
    ├─────────────┤
    │   Buttons   │
    │   (3個)     │
    └─────────────┘
          ▲
          │ 1:3
          │
┌──────────────────────┐
│   MouseButton        │
│   (ボタン状態)       │
├──────────────────────┤
│ - IsPressed: bool    │
│ - IsHeld: bool       │
│ - IsReleased: bool   │
└──────────────────────┘
```

---

## 次フェーズ

### Phase 1 後续

- `quickstart.md`：5 分で動く実装例
- `contracts/`：API 仕様書
- Copilot コンテキスト更新

---

**バージョン**: 1.0.0 | **生成日**: 2025-11-14 | **ステータス**: 完成
