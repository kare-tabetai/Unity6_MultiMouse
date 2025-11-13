# Copilot インストラクション - Unity6_MultiMouse

このドキュメントは、GitHub Copilot を使用して本プロジェクト（Unity6_MultiMouse）を開発・保守する際の指針を示します。

---

## プロジェクト概要

**プロジェクト名**: Unity6_MultiMouse

**目的**: Windows API を C# の DllImport 経由で使用して、Unity 6 で複数マウス入力を処理するサンプルプロジェクト。初学者向けの実装例として、わかりやすさを最優先とします。

**バージョン**: Unity 6 | **言語**: C# | **プラットフォーム**: Windows

---

## 開発環境

| 項目 | 詳細 |
|------|------|
| **エンジン** | Unity 6 |
| **言語** | C# |
| **IDE/エディタ** | Visual Studio Code（初学者向けの軽量性を優先） |
| **プラットフォーム** | Windows（デスクトップ） |
| **API アクセス方式** | C# DllImport 経由（user32.dll など） |
| **仕様管理** | GitHub Spec Kit |

---

## GitHub Spec Kit を用いた仕様管理

本プロジェクトは **GitHub Spec Kit** を使用して、プロジェクトの仕様、計画、タスク、ガイダンスを体系的に管理します。

### Spec Kit の構成

```txt
.specify/
  ├── copilot-instruction.md       # このファイル（Copilot ガイダンス）
  ├── memory/
  │   ├── constitution.md          # プロジェクト憲法（原則とガバナンス）
  │   └── [その他の記憶ファイル]
  └── templates/
      ├── plan-template.md         # 計画テンプレート
      ├── spec-template.md         # 仕様テンプレート
      └── tasks-template.md        # タスクテンプレート
```

### 各ドキュメントの役割

| ドキュメント | 用途 | 対象者 |
|-----------|------|--------|
| **constitution.md** | プロジェクトの「なぜ」：原則、価値観、ガバナンス | 全員 |
| **copilot-instruction.md** | Copilot への「どうするか」：開発ガイダンス | GitHub Copilot |
| **計画ドキュメント** | 特定フェーズの実装計画、マイルストーン | 開発チーム |
| **仕様ドキュメント** | 機能の詳細要件、入出力、制約条件 | 開発チーム、Copilot |
| **タスクドキュメント** | 実装タスク、チェックリスト、完了条件 | 開発チーム |

### 仕様管理のワークフロー

1. **計画フェーズ**: `plan-template.md` を参考に、実装内容をまとめる
2. **仕様策定**: `spec-template.md` を参考に、詳細仕様を記述
3. **タスク化**: `tasks-template.md` を参考に、実装タスクに分割
4. **実装**: Copilot にこれらの仕様ドキュメントを提供して実装依頼
5. **レビュー**: 完成したコードが仕様を満たしているか、憲法に準拠しているか確認

### Copilot への指示例（Spec Kit 活用）

```markdown
以下の仕様ドキュメントに基づいて実装してください：

**仕様ドキュメント**: `.specify/docs/spec-multi-mouse-input.md`
**関連タスク**: `.specify/docs/tasks-multi-mouse-input.md`
**参照**: `.specify/memory/constitution.md`

仕様ドキュメントに記載されている要件をすべて満たすコードを生成してください。
```

---

## コーディング原則（Copilot が遵守すべき指針）

### 1. 初学者向けシンプル設計

- **複雑さの排除**: 正当化が明示的でかつドキュメント化されていない限り、複雑な実装パターンは使用しない
- **YAGNI の適用**: 必要でない機能やジェネリック化は実装しない
- **単一責務**: 1 つの関数は 1 つの責務のみを持つ
- **わかりやすさ優先**: ユーザーが実装の「なぜ」と「どうやって」を迅速に習得できることを最優先

**実装例**:

```csharp
// ✅ 良い例：シンプルで役割が明確
public static int GetMouseX()
{
    GetCursorPos(out var point);
    return point.X;
}

// ❌ 避けるべき例：過度な汎用化
public static T GetCursorProperty<T>(Func<Point, T> selector, object options = null)
{
    // 複雑なロジック...
}
```

### 2. Windows API は DllImport で実装

- **DllImport 標準**: 全ての Windows API アクセスは C# DllImport を使用
- **禁止事項**: C++ ラッパー、COM interop、第三者ライブラリの使用は不可
- **必須ドキュメント**: 全ての DllImport 宣言には Windows API ドキュメントソースへの参照注釈が必須

**実装例**:

```csharp
// ✅ 必須形式：パラメータ説明と参照元ドキュメント注釈を含む
/// <summary>
/// マウスカーソルの現在位置を取得します。
/// 参考: https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getcursorpos
/// </summary>
[DllImport("user32.dll", SetLastError = true)]
[return: MarshalAs(UnmanagedType.Bool)]
private static extern bool GetCursorPos(out POINT lpPoint);
```

### 3. 複数マウス対応は必須

- **複数デバイス対応**: システムは複数接続マウスからの入力を同時に処理すること（必須）
- **デバイス識別**: 各マウスは ID などで独立して追跡・識別可能であること（必須）
- **退行防止**: 既存の複数マウス機能を損なう変更は不可

### 4. 実行時デモンストレーション必須

- **動作するシーン**: 複数マウス入力を視覚的に実証する Unity シーンが必須
- **独立実行**: Unity Editor でシーンを開くだけで動作すること（複雑なセットアップ不可）

### 5. インターフェースシンプル性

- **最小限引数**: メインメソッドの引数は 0～1 個に限定
- **単一の使用方法**: 複数の使用方法を提供しない
- **分離された詳細制御**: 高度な制御が必要な場合は、基本インターフェースと分離

**実装例**:

```csharp
// ✅ 良い例：引数なしで複数マウスを取得
public static List<Mouse> GetAllMice()
{
    // シンプルな実装
    return mice;
}

// ❌ 避けるべき例：複数のオーバーロードや複雑なパラメータ
public static List<Mouse> GetMice(bool? includeDisabled = null, 
                                   MouseFilter filter = null,
                                   Action<Mouse> onFound = null)
{
    // 複雑すぎる
}
```

### 6. 明確なドキュメント及び著作権表示

- **インラインコメント**: 自明でない技術的選択には理由をコメント記載（必須）
- **XML ドキュメント**: 全てのパブリックメソッドに XML ドキュメントコメントを記載（必須）
- **README 要件**: セットアップ手順、API 参照、著作権表示を含めること
- **最初の 5 分**: README にはコピペ可能なサンプルコードを含め、初学者が最初の 5 分で動かせること

---

## 外部ライブラリの方針

### 使用を避けるべき

- 複数マウス処理の核心機能に関わる第三者ライブラリ
- Windows API ラッパー（直接 DllImport で対応）
- 過度な抽象化ライブラリ

### 使用可能な範囲

- Unity の標準機能（InputManager など）
- 最小限のユーティリティ（データ構造や汎用ヘルパー）
- テストやドキュメント生成の補助ツール

---

## C# コードスタイル

### 命名規則

| 種類 | 規則 | 例 |
|------|------|-----|
| **クラス名** | PascalCase | `MouseInput`, `CursorTracker` |
| **メソッド名** | PascalCase | `GetMousePosition()`, `InitializeInput()` |
| **変数名** | camelCase | `mouseCount`, `cursorPosition` |
| **定数名** | UPPER_CASE | `MAX_MICE_COUNT` |
| **プライベート変数** | `_camelCase` | `_mouseDevices` |

### コメント規則

```csharp
/// <summary>
/// マウス入力を初期化します。
/// 複数接続されたマウスからのデバイスハンドルを取得し、イベント購読を開始します。
/// </summary>
public void Initialize()
{
    // Windows API で登録デバイスを列挙
    // GetRawInputDeviceList を使用して、接続されたすべてのマウスを特定
    EnumerateMouseDevices();
}
```

### 構造化例

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity6MultiMouse.Input
{
    /// <summary>
    /// 複数マウスからの入力を管理するメインクラスです。
    /// </summary>
    public class MultiMouseInput : MonoBehaviour
    {
        private List<Mouse> _mice;

        private void Start()
        {
            Initialize();
        }

        /// <summary>
        /// システムに接続されたすべてのマウスを取得します。
        /// </summary>
        public List<Mouse> GetAllMice()
        {
            return _mice;
        }
    }
}
```

---

## ドキュメント言語

- **必須言語**: 日本語
- **対象読者**: 初学者
- **形式**: Markdown（GitHub 表示対応）

---

## Git ワークフロー

- **ブランチ名**: `feature/[説明]` または `[チケット]-[説明]`
- **コミットメッセージ**: わかりやすく、何を変更したかを明確に
- **プルリクエスト**: 以下を含める
  1. 変更の説明
  2. テスト方法（該当する場合）
  3. 複数マウス機能の実証確認

---

## ファイル構造の方針

```
Assets/
  ├── Scripts/              # C# スクリプト
  │   ├── Input/           # 入力処理関連
  │   ├── UI/              # UI 関連
  │   └── Utilities/       # ユーティリティ
  ├── Scenes/              # サンプルシーン
  └── Resources/           # リソース（必要に応じて）

.specify/
  ├── copilot-instruction.md    # このファイル
  ├── memory/
  │   └── constitution.md       # プロジェクト憲法
  └── templates/                # テンプレート類
```txt

---

## Copilot への指示テンプレート

### 実装時の指示例

```markdown
以下の仕様で C# メソッドを実装してください：

要件:
- 複数マウスから座標を取得する
- Windows API DllImport を使用
- 初学者が理解できるシンプル実装
- インラインコメントで Windows API 参照を含める

仕様書: [詳細記載]
```

### コードレビュー指示例

```markdown
このコードが以下の基準を満たしているか確認してください：

1. Windows API は DllImport で実装されているか？
2. 実装がシンプルで初学者向けか？
3. コメントに Windows API ドキュメント参照があるか？
4. 複数マウス機能を損なっていないか？
```

---

## Copilot が使用すべき主要なツール

### ファイル操作

- コード生成時は必ず実装する（提案のみでなく）
- ファイル編集は正確な行指定で実行
- テンプレートは `.specify/templates` から参照

### 確認・検証

- 実装後は当該ファイルを読み込んで動作確認
- シンプル性を損なう変更がないか確認
- 複数マウス機能の退行がないか確認

### ドキュメント作成

- README は日本語で、最初の 5 分で動かせる内容
- インラインコメントは日本語でコマンド、パラメータ説明は必須
- サンプルコードはコピペで動作する形式

### Spec Kit との連携

- **計画・仕様ドキュメントの参照**: 実装時は常に `.specify` ディレクトリ配下のドキュメントを確認
- **憲法の遵守確認**: すべての変更が `constitution.md` の 6 つの原則に準拠しているか検証
- **ドキュメント更新**: 実装完了後、関連する Spec Kit ドキュメントを最新状態に保つ
- **テンプレートの活用**: 新規ドキュメント作成時は `.specify/templates/` 配下のテンプレートを使用

---

## よくある質問への対応方針

### Q: 外部ライブラリを使いたい

**A**: 複数マウス処理の核心機能なら避けるべき。代わりに Windows API DllImport で直接実装し、コメントで参考ドキュメントを示してください。

### Q: より汎用的な設計にしたい

**A**: 初学者向けなので、不必要な抽象化は避けてください。「わかりやすさ」を優先し、複雑さが必要な場合は明示的に正当化してください。

### Q: テストコードはどうする？

**A**: 初期段階では Unity Editor でのシーン実行による手動検証で十分。自動テストが必要になったら、憲法を更新してください。

### Q: ドキュメントはどこに？

**A**: README.md（セットアップと使用方法）とインラインコメント（実装の詳細）を組み合わせてください。

---

## バージョン履歴

| バージョン | 日付 | 変更内容 |
|-----------|------|--------|
| 1.1.0 | 2025-11-13 | GitHub Spec Kit を用いた仕様管理セクションを追加 |
| 1.0.0 | 2025-11-13 | 初版作成 |

---

**最終更新**: 2025-11-13

**責任者**: GitHub Copilot

**参照**: `.specify/memory/constitution.md`
