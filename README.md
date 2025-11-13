# Unity6_MultiMouse - マルチマウス入力マネージャー

複数の物理マウスからの独立した入力を Unity 6 で取得・管理するサンプルプロジェクトです。Windows API (`user32.dll`) の Raw Input API を C# DllImport 経由で使用します。

初学者向けのシンプルな設計で、わかりやすさを最優先としています。

---

## 🚀 クイックスタート（5 分で動く）

### 前提条件

- **Unity 6** がインストール
- **Windows 10/11** システム
- **複数マウス** が物理的に接続（2 個以上推奨）

### セットアップ

1. **このプロジェクトをダウンロード・開く**
   ```bash
   # リポジトリをクローン
   git clone https://github.com/kare-tabetai/Unity6_MultiMouse.git
   cd Unity6_MultiMouse
   
   # Unity 6 Editor で開く
   ```

2. **シーンを実行**
   - `Assets/Scenes/MultiMouseTest.unity` を開く
   - **Play** ボタンを押す
   - **Game ウィンドウ** に複数マウスの情報が表示される
   - **コンソール** にマウスデータがログ出力される

### 最小限の実装例

```csharp
using UnityEngine;
using Unity6MultiMouse.Input;

public class MouseTracker : MonoBehaviour
{
    private void Update()
    {
        // すべてのマウスを取得
        var mice = MultiMouseInput.GetAllMice();
        
        // 各マウスの状態を表示
        foreach (var mouse in mice)
        {
            // 位置取得
            Debug.Log($"マウス {mouse.DeviceId}: " +
                      $"位置=({mouse.PositionX}, {mouse.PositionY})");

            // ボタン検出
            if (mouse.LeftButton.IsPressed)
            {
                Debug.Log($"マウス {mouse.DeviceId} 左クリック!");
            }

            // 移動検出
            if (mouse.DeltaX != 0 || mouse.DeltaY != 0)
            {
                Debug.Log($"マウス {mouse.DeviceId} 移動: " +
                          $"({mouse.DeltaX}, {mouse.DeltaY})");
            }
        }
    }
}
```

---

## 📚 API リファレンス

### MultiMouseInput

#### GetAllMice()

接続されたすべてのマウスを取得します。

```csharp
public static List<Mouse> GetAllMice()
```

**戻り値**: マウスオブジェクトのリスト（見つからない場合は空リスト）

**例**:
```csharp
var mice = MultiMouseInput.GetAllMice();
foreach (var mouse in mice)
{
    Debug.Log($"マウス {mouse.DeviceId}: ({mouse.PositionX}, {mouse.PositionY})");
}
```

---

#### GetMouse(int deviceId)

デバイス ID でマウスを取得します。

```csharp
public static Mouse GetMouse(int deviceId)
```

**パラメータ**:
- `deviceId` (int): マウスの序号（0, 1, 2...）

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

接続マウス数を取得します。

```csharp
public static int GetMouseCount()
```

**戻り値**: マウスの個数

**例**:
```csharp
int count = MultiMouseInput.GetMouseCount();
Debug.Log($"接続マウス数: {count}");
```

---

### Mouse（マウスデバイス）

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
| `IsConnected` | bool | 接続状態 |

---

### MouseButton（ボタン状態）

| プロパティ | 型 | 説明 |
|-----------|-----|------|
| `IsPressed` | bool | 今フレーム初めて押下 |
| `IsHeld` | bool | 現在押下中 |
| `IsReleased` | bool | 今フレーム初めてリリース |

---

## 🎮 複数マウスでのゲーム実装例

### 複数プレイヤーのカーソル制御

```csharp
public class MultiplayerCursorController : MonoBehaviour
{
    private Sprite[] _cursorSprites;
    private GameObject[] _cursorGOs;

    private void Start()
    {
        int mouseCount = MultiMouseInput.GetMouseCount();
        _cursorGOs = new GameObject[mouseCount];

        for (int i = 0; i < mouseCount; i++)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("CursorPrefab"));
            _cursorGOs[i] = go;
        }
    }

    private void Update()
    {
        var mice = MultiMouseInput.GetAllMice();

        foreach (var mouse in mice)
        {
            if (mouse.DeviceId < _cursorGOs.Length)
            {
                // カーソル位置更新
                Vector3 cursorPos = new Vector3(mouse.PositionX, Screen.height - mouse.PositionY, 0);
                _cursorGOs[mouse.DeviceId].transform.position = cursorPos;

                // ボタンアクション
                if (mouse.LeftButton.IsPressed)
                {
                    OnPlayerClick(mouse.DeviceId);
                }
            }
        }
    }

    private void OnPlayerClick(int playerId)
    {
        Debug.Log($"プレイヤー {playerId} がクリック");
    }
}
```

---

## 📁 プロジェクト構成

```
Assets/
├── Scripts/
│   └── Input/
│       ├── PlatformInput.cs           # Windows API DllImport 層
│       ├── MouseButton.cs             # ボタン状態
│       ├── Mouse.cs                   # マウス状態
│       ├── MultiMouseInput.cs         # メインマネージャー
│       ├── MultiMouseInputInitTester.cs
│       ├── MultiMouseInputButtonTester.cs
│       ├── MultiMouseInputPositionTester.cs
│       ├── MultiMouseInputDeltaTester.cs
│       ├── MultiMouseDemoUI.cs
│       ├── MultiMouseCursorDisplay.cs
│       └── MultiMouseButtonDisplay.cs
├── Scenes/
│   └── MultiMouseTest.unity           # テストシーン
└── Resources/
    └── （UI 素材など必要に応じて）
```

---

## 🔧 トラブルシューティング

### Q: マウスが検出されない

**A**: 
- Windows Raw Input は **Editor およびビルド版で動作**します。
- `MultiMouseInput.GetMouseCount()` が 0 の場合、マウスが接続されていない可能性があります。
- Windows デバイスマネージャーで確認してください。

### Q: ボタン反応がない

**A**:
- Raw Input API は **フォーカスウィンドウでのみ入力**を提供します。
- Editor ウィンドウ（またはビルド版アプリケーション）がアクティブであることを確認してください。

### Q: 遅延が感じられる

**A**:
- ポーリング方式のため < 1 フレーム遅延は許容です。
- FPS が 60 以上であることを確認してください（Edit > Project Settings > Quality）。

### Q: ビルド版で動作しない

**A**:
- Windows Standalone ビルドで確認してください。
- Mac/Linux では Raw Input API (`user32.dll`) が利用不可です。

---

## 📖 詳細ドキュメント

- **仕様書**: `/specs/001-multi-mouse-input/spec.md`
- **実装計画**: `/specs/001-multi-mouse-input/plan.md`
- **研究ドキュメント**: `/specs/001-multi-mouse-input/research.md`
- **データモデル**: `/specs/001-multi-mouse-input/data-model.md`
- **クイックスタート**: `/specs/001-multi-mouse-input/quickstart.md`
- **API 仕様**: `/specs/001-multi-mouse-input/contracts/api-spec.md`
- **実装タスク**: `/specs/001-multi-mouse-input/tasks.md`

---

## ✅ チェックリスト

- [ ] 複数マウス（2 個以上）が接続されている
- [ ] Unity 6 Editor でプロジェクトを開いた
- [ ] `Assets/Scenes/MultiMouseTest.unity` で Play ボタン実行
- [ ] コンソール に「接続マウス数」が表示される
- [ ] 複数マウスを移動・クリックしてデータが更新される

---

## 🎯 実装原則（憲法）

このプロジェクトは以下の 6 つの原則に従っています：

1. **初学者向けシンプル設計**: 複雑さを排除、わかりやすさ優先
2. **Windows API via DllImport**: 外部ライブラリなし、C# DllImport のみ
3. **複数マウス対応必須**: システム設計のコア要件
4. **実行時デモンストレーション必須**: Unity シーンで動作実証
5. **明確なドキュメント及び著作権表示**: コメント・README 完備
6. **インターフェースシンプル性**: メインメソッド引数 0～1 個に限定

詳細は `/specify/memory/constitution.md` を参照してください。

---

## 📄 ライセンス

このプロジェクトは **MIT ライセンス** の下で配布されています。

```
MIT License

Copyright (c) 2025 Unity6_MultiMouse Contributors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
```

詳細は [LICENSE](./LICENSE) ファイルを参照してください。

---

## 🤝 サポート・フィードバック

問題が発生した場合や改善提案がある場合は、GitHub Issues でお知らせください。

---

**作成日**: 2025-11-14  
**ステータス**: 実装完了（Phase 3～7）  
**参照**: `.specify/` ドキュメント一式
