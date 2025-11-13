# API 仕様: MultiMouseInput

**フォーマット**: C# インターフェース定義  
**バージョン**: 1.0.0  
**生成日**: 2025-11-14

---

## 名前空間

```csharp
namespace Unity6MultiMouse.Input
```

---

## 型定義

### MultiMouseInput

```csharp
public class MultiMouseInput : MonoBehaviour
{
    /// <summary>
    /// 接続されたすべてのマウスを取得します。
    /// </summary>
    public static List<Mouse> GetAllMice();
    
    /// <summary>
    /// デバイス ID でマウスを取得します。
    /// </summary>
    public static Mouse GetMouse(int deviceId);
    
    /// <summary>
    /// 接続マウス数を取得します。
    /// </summary>
    public static int GetMouseCount();
}
```

---

### Mouse

```csharp
public class Mouse
{
    // デバイス識別
    public int DeviceId { get; }
    
    // 位置
    public int PositionX { get; }
    public int PositionY { get; }
    
    // 移動デルタ
    public int DeltaX { get; }
    public int DeltaY { get; }
    
    // ボタン状態
    public MouseButton LeftButton { get; }
    public MouseButton RightButton { get; }
    public MouseButton MiddleButton { get; }
}
```

---

### MouseButton

```csharp
public class MouseButton
{
    public bool IsPressed { get; }   // 今フレーム初押下
    public bool IsHeld { get; }      // 押下中
    public bool IsReleased { get; }  // 今フレーム初リリース
}
```

---

## メソッド仕様

### GetAllMice()

接続されたすべてのマウスをリストで取得します。

**戻り値**: `List<Mouse>` - マウスリスト（見つからない場合は空リスト）

**例外**: なし

**用法**:

```csharp
var mice = MultiMouseInput.GetAllMice();
foreach (var mouse in mice)
{
    Debug.Log($"マウス {mouse.DeviceId}: ({mouse.PositionX}, {mouse.PositionY})");
}
```

---

### GetMouse(int deviceId)

デバイス ID で特定マウスを取得します。

**パラメータ**:

- `deviceId` (int): マウスの序号（0 から始まる）

**戻り値**: `Mouse | null` - マウスオブジェクト、見つからない場合は null

**用法**:

```csharp
var mouse = MultiMouseInput.GetMouse(0);
if (mouse != null && mouse.LeftButton.IsPressed)
{
    Debug.Log("マウス 0 左クリック");
}
```

---

### GetMouseCount()

接続マウス数を取得します。

**戻り値**: `int` - マウスの個数（0 以上）

**用法**:

```csharp
int count = MultiMouseInput.GetMouseCount();
Debug.Log($"接続マウス数: {count}");
```

---

## プロパティ仕様

### Mouse プロパティ

| プロパティ | 型 | 説明 |
|-----------|-----|------|
| `DeviceId` | int | マウスの序号（0, 1, 2...） |
| `PositionX` | int | スクリーン X 座標（ピクセル） |
| `PositionY` | int | スクリーン Y 座標（ピクセル） |
| `DeltaX` | int | 前フレーム比較 X 変化 |
| `DeltaY` | int | 前フレーム比較 Y 変化 |
| `LeftButton` | MouseButton | 左ボタン状態 |
| `RightButton` | MouseButton | 右ボタン状態 |
| `MiddleButton` | MouseButton | 中央ボタン状態 |

### MouseButton プロパティ

| プロパティ | 型 | 説明 |
|-----------|-----|------|
| `IsPressed` | bool | 今フレーム初めて押下 |
| `IsHeld` | bool | 現在押下中 |
| `IsReleased` | bool | 今フレーム初めてリリース |

---

## 実装ガイドライン

### 複数マウス入力の処理

```csharp
private void Update()
{
    var mice = MultiMouseInput.GetAllMice();
    
    foreach (var mouse in mice)
    {
        // ボタン検出
        if (mouse.LeftButton.IsPressed)
        {
            OnLeftClick(mouse.DeviceId);
        }
        
        // 移動検出
        if (mouse.DeltaX != 0 || mouse.DeltaY != 0)
        {
            OnMouseMove(mouse.DeviceId, mouse.DeltaX, mouse.DeltaY);
        }
    }
}
```

---

## 保証事項

- **フレーム更新**: 全プロパティは毎フレーム更新
- **スクリーン座標**: 左上 (0, 0) が原点
- **スレッドセーフ**: Main Thread のみで呼び出し
- **メモリ**: GetAllMice() は コピーリストを返す

---

**バージョン**: 1.0.0 | **生成日**: 2025-11-14
