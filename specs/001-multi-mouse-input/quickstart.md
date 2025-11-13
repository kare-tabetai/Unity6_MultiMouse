# クイックスタート: マルチマウス入力マネージャー

**フェーズ**: 1 - 設計  
**生成日**: 2025-11-14  
**目的**: 5 分で複数マウス入力を動かす

---

## 前提条件

- **Unity 6** がインストール
- **Windows 10/11** システム
- **複数マウス**が物理的に接続（2 個以上推奨）

---

## セットアップ（3 分）

### ステップ 1: スクリプトをプロジェクトに追加

以下のファイルを `Assets/Scripts/Input/` に配置：

- `PlatformInput.cs`（Windows API DllImport）
- `Mouse.cs`（マウス状態構造体）
- `MouseButton.cs`（ボタン状態）
- `MultiMouseInput.cs`（マネージャー）

### ステップ 2: デモシーンをセットアップ

`Assets/Scenes/MultiMouseDemo.unity` をシーンに作成し、以下を配置：

1. **GameObject**: `InputManager`
2. **スクリプト**: `MultiMouseInputManager.cs` をアタッチ
3. **Canvas**: UI 表示用（テキスト＆グラフィック）

---

## 実装例（2 分）

### シンプルな使用方法

```csharp
using UnityEngine;
using Unity6MultiMouse.Input;

public class MouseTracker : MonoBehaviour
{
    private void Update()
    {
        // 全マウスを取得
        var mice = MultiMouseInput.GetAllMice();
        
        // 各マウスの状態を表示
        foreach (var mouse in mice)
        {
            Debug.Log($"マウス {mouse.DeviceId}: " +
                      $"位置=({mouse.PositionX}, {mouse.PositionY}) " +
                      $"移動=({mouse.DeltaX}, {mouse.DeltaY}) " +
                      $"左ボタン={mouse.LeftButton.IsPressed}");
        }
    }
}
```

---

## サンプルシーン動作（確認）

### 実行手順

1. Unity Editor でシーンを開く
2. **Play** ボタンを押下
3. 複数マウスを接続・移動・クリック
4. Hierarchy でマウスカーソルと状態が表示される

### 期待される動作

- **複数カーソル**: 各マウスが独立して移動
- **ボタン表示**: 各マウスのボタン押下が視覚化
- **デルタ表示**: フレーム内移動量が更新

---

## トラブルシューティング

### Q1: マウスが検出されない

**A**: Windows Raw Input は **Editor コンソールおよびビルド版**で動作します。

- `MultiMouseInput.GetMouseCount()` が 0 の場合、マウスが接続されていない可能性あり
- Windows デバイスマネージャーで確認

### Q2: ボタンが反応しない

**A**: Raw Input API は **フォーカスウィンドウ**でのみ入力を提供します。

- Editor ウィンドウがアクティブか確認
- ビルド版の場合、アプリケーションウィンドウがアクティブ

### Q3: 遅延が感じられる

**A**: ポーリング方式のため < 1 フレーム遅延は許容です。

- GPU 負荷が低い設定で確認（Edit > Project Settings > Quality）
- 60+ FPS 実行を確認

---

## API リファレンス

### MultiMouseInput（メインクラス）

#### GetAllMice()

```csharp
public static List<Mouse> GetAllMice()
```

**説明**: 接続マウス全て取得  
**戻り値**: マウスオブジェクトのリスト（コピー）  
**例**:

```csharp
var mice = MultiMouseInput.GetAllMice();
foreach (var mouse in mice)
{
    Debug.Log($"ID: {mouse.DeviceId}, X: {mouse.PositionX}");
}
```

---

#### GetMouse(int id)

```csharp
public static Mouse GetMouse(int id)
```

**説明**: デバイス ID で特定マウス取得  
**パラメータ**: `id` - デバイス序号（0, 1, 2...）  
**戻り値**: マウスオブジェクト（見つからない場合は null）  
**例**:

```csharp
var mouse = MultiMouseInput.GetMouse(0);
if (mouse != null)
{
    Debug.Log($"マウス 0 の位置: ({mouse.PositionX}, {mouse.PositionY})");
}
```

---

#### GetMouseCount()

```csharp
public static int GetMouseCount()
```

**説明**: 接続マウス数  
**戻り値**: マウスの個数  
**例**:

```csharp
int count = MultiMouseInput.GetMouseCount();
Debug.Log($"接続マウス数: {count}");
```

---

### Mouse（マウス状態）

| プロパティ | 型 | 説明 |
|-----------|-----|------|
| `DeviceId` | `int` | マウスの序号（0, 1, 2...） |
| `PositionX` | `int` | スクリーン X 座標 |
| `PositionY` | `int` | スクリーン Y 座標 |
| `DeltaX` | `int` | 前フレーム比較移動 X |
| `DeltaY` | `int` | 前フレーム比較移動 Y |
| `LeftButton` | `MouseButton` | 左ボタン状態 |
| `RightButton` | `MouseButton` | 右ボタン状態 |
| `MiddleButton` | `MouseButton` | 中央ボタン状態 |

---

### MouseButton（ボタン状態）

| プロパティ | 型 | 説明 |
|-----------|-----|------|
| `IsPressed` | `bool` | 今フレーム初めて押下されたか |
| `IsHeld` | `bool` | ボタンが押下中か |
| `IsReleased` | `bool` | 今フレーム初めてリリースされたか |

---

## 次のステップ

### デモシーンの拡張例

**複数マウスでカーソル描画**:

```csharp
public class MultiCursorDisplay : MonoBehaviour
{
    private Canvas _canvas;

    private void Start()
    {
        _canvas = FindObjectOfType<Canvas>();
    }

    private void Update()
    {
        var mice = MultiMouseInput.GetAllMice();
        
        foreach (var mouse in mice)
        {
            // 各マウス位置に UI 要素を配置
            var cursorGO = new GameObject($"Cursor_{mouse.DeviceId}");
            var image = cursorGO.AddComponent<Image>();
            var rectTransform = cursorGO.GetComponent<RectTransform>();
            
            rectTransform.SetParent(_canvas.transform);
            rectTransform.anchoredPosition = 
                new Vector2(mouse.PositionX, Screen.height - mouse.PositionY);
        }
    }
}
```

---

## ライセンス

このサンプルコードは MIT ライセンスの下で配布されています。  
自由に複製・修正・配布できます。

---

**バージョン**: 1.0.0 | **生成日**: 2025-11-14 | **ステータス**: 完成
