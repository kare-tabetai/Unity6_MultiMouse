# 実装計画: マルチマウス入力マネージャー

**ブランチ**: `001-multi-mouse-input` | **日付**: 2025-11-14 | **仕様**: `/specs/001-multi-mouse-input/spec.md`

**入力**: `/specs/001-multi-mouse-input/spec.md` からの機能仕様

## 概要

Windows API（user32.dll）経由で複数接続マウスからの入力を検出・追跡するマネージャークラスを実装する。各マウスはデバイス ID で一意に識別され、ボタン状態、スクリーン座標、フレーム内移動デルタを独立して提供する。Unity 6 Editor とビルド版両対応のサンプルシーンで動作実証。

---

## 技術コンテキスト

**言語/バージョン**: C# | Unity 6 | Windows 固有

**主要依存**:

- Unity 6 標準モジュール（InputManager、MonoBehaviour）
- Windows API（user32.dll：GetRawInputDeviceList、GetRawInputData など）
- 外部ライブラリなし

**ストレージ**: N/A

**テスト方式**:

- Unity Editor プレイモード（手動検証）
- ビルド済み Windows アプリケーション実行
- サンプルシーン視覚確認

**ターゲットプラットフォーム**: Windows デスクトップ（user32.dll 対応）

**プロジェクト種別**: Unity モノプロジェクト（Assets 以下に統一）

**パフォーマンス目標**:

- 入力遅延 < 1 フレーム（60 FPS）
- 4 個以上のマウス同時追跡

**制約**:

- 複数マウス対応必須
- 初学者向けシンプル設計（正当化なき複雑さ禁止）
- Windows API は DllImport のみ（C++ ラッパー禁止）
- API サーフェス最小化（メインメソッド引数 0～1 個）

**スケール**: 4～10 個同時接続マウス

---

## 憲法チェック

*ゲート: Phase 0 研究前に必須パス。Phase 1 設計後に再評価。*

### 原則 I: 初学者向けシンプル設計

**要件**: 正当化なき複雑さ禁止、YAGNI の厳密適用、単一責務、シンプルなインターフェース

**事前評価**: ⚠️ NEEDS CLARIFICATION

- 複数マウスを同時追跡する最小限の実装パターンは？
- Raw Input API（GetRawInputData）の初学者向け説明方法は？

### 原則 II: Windows API via DllImport（非交渉）

**要件**: 全 Windows API アクセスは DllImport、C++ ラッパー禁止、宣言に参照注釈必須

**事前評価**: ✅ 明確

- user32.dll の GetRawInputDeviceList、GetRawInputData、GetCursorPos を使用予定
- 全 DllImport に Windows API ドキュメント参照を含める

### 原則 III: 複数マウス対応必須

**要件**: システムは複数接続マウスを同時追跡、各マウス独立識別必須

**事前評価**: ✅ 明確

- 要件 FR-002 で「デバイス ID で一意識別」
- 要件 FR-001 で「全接続マウス検出」

### 原則 IV: 実行時デモンストレーション必須

**要件**: Unity シーン必須、Editor で追加セットアップなし実行可能

**事前評価**: ✅ 仕様に明記

- ユーザーストーリー 5「サンプルシーン」
- 要件 FR-010「追加ビルド手順なし」

### 原則 V: 明確なドキュメント及び著作権表示

**要件**: DllImport に Windows API 参照必須、README に 5 分で動く例、著作権表示

**事前評価**: ⚠️ NEEDS CLARIFICATION

- サンプルコードの著作権・ライセンス表示方法は？

### 原則 VI: インターフェースシンプル性

**要件**: メインメソッド引数 0～1 個、単一使用方法、詳細制御は分離

**事前評価**: ✅ 明確

- 「GetAllMice()」（引数なし）で全マウス取得がベース
- 詳細クエリは各マウスオブジェクトのメソッドで分離

---

## Phase 1 設計後の憲法チェック再評価

*ゲート: Phase 1 設計完了後。全原則を再確認。*

### 原則 I - 初学者向けシンプル設計

**事後評価**: ✅ **準拠確認**

- `research.md` で Raw Input ポーリングパターン確定（初学者向け設計）
- `data-model.md` で単純な3つのエンティティ（MultiMouseInput, Mouse, MouseButton）に限定
- 不必要な汎用化なし、複雑さは全て正当化

### 原則 II - Windows API via DllImport（非交渉）

**事後評価**: ✅ **準拠確認**

- `research.md` で user32.dll API（GetRawInputDeviceList, GetRawInputData など）を明記
- `quickstart.md` で `PlatformInput.cs` 集約を示唆
- C++ ラッパーなし、DllImport のみ

### 原則 III - 複数マウス対応必須

**事後評価**: ✅ **準拠確認**

- `data-model.md` で各マウスを `DeviceId` で一意識別
- `contracts/api-spec.md` で複数マウス独立追跡 API を仕様化
- ボタン・位置・デルタすべて マウス単位で独立

### 原則 IV - 実行時デモンストレーション必須

**事後評価**: ✅ **準拠確認**

- `plan.md` で `Assets/Scenes/MultiMouseDemo.unity` シーンを明記
- `quickstart.md` で「Editor から Play で動作確認」を記載
- セットアップ不要（スクリプト配置＋シーン開くだけ）

### 原則 V - 明確なドキュメント及び著作権表示

**事後評価**: ✅ **準拠確認**

- `research.md` で Windows API 参照（完全 URL 記載）を記述戦略に組込
- `quickstart.md` で「5 分で動く例」コード提示
- `contracts/api-spec.md` で API 完全仕様化
- 著作権表示方法を `research.md` で定義（ファイル先頭＋README）

### 原則 VI - インターフェースシンプル性

**事後評価**: ✅ **準拠確認**

- `contracts/api-spec.md` で メインメソッド確定：
  - `GetAllMice()` - 引数なし
  - `GetMouse(int id)` - 引数1個
  - `GetMouseCount()` - 引数なし
- 単一の使用パターン（シンプル＆直感的）
- 詳細制御はプロパティ（PositionX, IsPressed など）で分離

**結論**: 全 6 原則を満たす。Phase 2（実装準備）へ進行可能。

---

## プロジェクト構造

### ドキュメント（本機能）

```text
specs/001-multi-mouse-input/
├── spec.md              # 仕様書（既存）
├── plan.md              # 本ファイル（この実装計画）
├── research.md          # Phase 0 出力（研究結果）
├── data-model.md        # Phase 1 出力（データモデル）
├── quickstart.md        # Phase 1 出力（クイックスタート）
├── contracts/           # Phase 1 出力（API 契約）
└── checklists/          # チェックリスト
```

### ソースコード（リポジトリルート）

```text
Assets/
├── Scripts/
│   └── Input/                          # 入力処理関連
│       ├── MultiMouseInput.cs          # メインマネージャークラス
│       ├── Mouse.cs                    # マウス状態を表す構造体/クラス
│       ├── MouseButton.cs              # ボタン状態定義
│       └── PlatformInput.cs            # Windows API DllImport 集約
├── Scenes/
│   ├── SampleScene.unity               # 既存サンプルシーン（未変更）
│   └── MultiMouseDemo.unity            # 新規マルチマウスデモシーン
└── Resources/                          # リソース（必要に応じて）
    └── UI/                             # デモ UI 素材
```

**構造の決定根拠**:

- Unity 標準的な Assets 以下に統一（初学者向け）
- 入力処理は `Scripts/Input/` に集約（責務明確化）
- Windows API（DllImport）は `PlatformInput.cs` に集約（保守性）
- デモシーンは独立（本体を汚さない）

---

## 複雑性追跡

### 憲法違反の検証

| 違反 | 正当化 | 代替案が不十分な理由 |
|------|--------|-------------------|
| Raw Input API 使用（低レベル） | 複数マウスを同時且つ独立追跡するには Raw Input が必須 | 標準 InputManager は単一マウス前提 |
| DllImport 宣言 3～4 個 | GetRawInputDeviceList、GetRawInputData、GetCursorPos で機能完成 | 各 API は独立した責務を持つ |
| UI 表示ロジック（デモシーン） | 初学者が動作を視覚確認でき、コピペ例になるため | UI なしでは動作確認が困難 |

### シンプル設計の維持戦略

1. **最初は 4 個ボタン対応（左・右・中央・ホイール）に限定、追加ボタンは後続フェーズ**
2. **デバイスフィルタリング（例：キーボード除外）は基本実装に含まない**
3. **振動・力フィードバックは非対応（スコープ外）**

---

## 次のステップ

### Phase 0: 研究と明確化

- NEEDS CLARIFICATION の解決
- Raw Input API ベストプラクティス調査
- 著作権・ライセンス表示方法確定

### Phase 1: 設計とコントラクト

- `data-model.md` 生成（エンティティ定義）
- `quickstart.md` 生成（5 分で動く例）
- `contracts/` 生成（API 仕様）
- Copilot コンテキスト更新

### Phase 2: 実装準備

- `tasks.md` 生成（実装タスク）
- コードレビュー基準確定
- テストシーン設計確定

---

**バージョン**: 1.0.0 | **作成日**: 2025-11-14 | **ステータス**: ドラフト

**参照**: `.specify/memory/constitution.md` v1.1.0
