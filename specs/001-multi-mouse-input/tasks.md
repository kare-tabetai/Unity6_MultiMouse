# Tasks: マルチマウス入力マネージャー実装

**入力**: `/specs/001-multi-mouse-input/` の設計ドキュメント  
**前提**: plan.md ✅、spec.md ✅、research.md ✅、data-model.md ✅、quickstart.md ✅、contracts/api-spec.md ✅  
**テスト**: テストは任意（本仕様では手動検証を採用）  
**組織方法**: ユーザーストーリー単位での段階的実装と独立検証

---

## タスク形式: `[ID] [P?] [Story?] 説明`

- **[P]**: 並列実行可能（異なるファイル、依存関係なし）
- **[Story]**: ユーザーストーリー所属（例：US1、US2、US3）
- **ファイルパス**: `Assets/` 以下の正確なパス指定

---

## Phase 1: セットアップ（プロジェクト初期化）

**目的**: C# 開発環境の基本構造確立

- [x] T001 Unity 6 プロジェクト構造を確認（Assets/ 以下の既存構造）
- [x] T002 フォルダ構造を作成: `Assets/Scripts/Input/` を新規作成
- [x] T003 [P] フォルダ構造を作成: `Assets/Scenes/` （既存、デモシーンの追加用）

**チェックポイント**: フォルダ構造完成 - Windows API 層実装へ進行可能 ✅

---

## Phase 2: 基礎（Windows API DllImport 層）

**目的**: 複数マウス入力を取得するための Windows API 基盤構築

**⚠️ 重要**: このフェーズ完了まで、ユーザーストーリー実装は開始不可

### 実装 - Phase 2

- [x] T004 [P] PlatformInput.cs（Windows API DllImport 集約層）を `Assets/Scripts/Input/` に作成
  - GetRawInputDeviceList DllImport 宣言（デバイス列挙）
  - GetRawInputData DllImport 宣言（入力データ取得）
  - RegisterRawInputDevices DllImport 宣言（Raw Input 登録）
  - GetCursorPos DllImport 宣言（カーソル位置取得）
  - 必須: すべての DllImport に Windows API ドキュメント参照 URL を含めること ✅
  - 必須: パラメータ型・説明をコメント記載 ✅

- [x] T005 [P] MouseButton.cs（ボタン状態定義）を `Assets/Scripts/Input/` に作成
  - IsPressed プロパティ（今フレーム初押下）✅
  - IsHeld プロパティ（押下中）✅
  - IsReleased プロパティ（今フレーム初リリース）✅
  - パラメータ: 前フレーム状態から状態遷移判定 ✅
  - メモ: 2 値状態（押下/未押下）から 3 値イベント（Press/Hold/Release）への変換ロジック ✅

- [x] T006 [P] Mouse.cs（マウスデバイス状態）を `Assets/Scripts/Input/` に作成
  - DeviceId プロパティ（序号 0, 1, 2...）✅
  - DeviceHandle プロパティ（IntPtr、Windows HANDLE）✅
  - PositionX、PositionY プロパティ（スクリーン座標）✅
  - DeltaX、DeltaY プロパティ（前フレーム差分）✅
  - LeftButton、RightButton、MiddleButton プロパティ（MouseButton）✅
  - IsConnected プロパティ（接続状態）✅
  - 内部: 座標・ボタン状態の更新メソッド ✅

- [x] T007 MultiMouseInput.cs（マネージャークラス）基本実装を `Assets/Scripts/Input/` に作成
  - MonoBehaviour 継承 ✅
  - GetAllMice() 静的メソッド（接続マウス全て返却、リスト コピー）✅
  - GetMouse(int id) 静的メソッド（ID で特定マウス取得）✅
  - GetMouseCount() 静的メソッド（接続マウス数返却）✅
  - 内部: _mice リスト、_deviceHandles キャッシュ ✅
  - 内部: Start() での初期化フック、OnDestroy() でのクリーンアップフック ✅
  - メモ: 実装は後続タスク（T008 以降）で段階的に追加 ✅

**チェックポイント**: Windows API 層と基本インターフェース完成 - ユーザーストーリー 1 実装へ ✅

---

## Phase 3: ユーザーストーリー 1 - マルチマウスシステムの初期化（優先度: P1）🎯 MVP

**ゴール**: 複数マウスの検出と初期化が動作し、Manager を介してすべてのマウスにアクセス可能

**独立テスト方法**: 
1. MultiMouseInput をスクリプトに組み込み、GetMouseCount() で検出マウス数が正値を返す
2. 物理マウス（2 個以上）を接続・切断して、GetMouseCount() が動的に変化する
3. GetAllMice() でマウスリストが取得可能、各マウスに固有の DeviceId がある

### 実装 - ユーザーストーリー 1

- [x] T008 [P] MultiMouseInput.cs に Win32 デバイス列挙ロジック実装
  - RegisterRawInputDevices() メソッド実装（Raw Input デバイス登録）✅
  - UnregisterRawInputDevices() メソッド実装（登録解除）✅
  - EnumerateConnectedMice() メソッド実装（GetRawInputDeviceList で接続マウス検出）✅
  - 内部: 各マウスを DeviceHandle と序号で識別・キャッシュ ✅

- [x] T009 MultiMouseInput.cs に Update() 統合実装
  - フレーム毎に EnumerateConnectedMice() で接続状態刷新 ✅
  - 接続マウスが変更された場合、_mice リスト更新 ✅
  - 内部: 前フレーム状態と比較して新規接続・切断を検出 ✅

- [x] T010 MultiMouseInput.cs に Start()・OnDestroy() 統合実装
  - Start() で RegisterRawInputDevices() 呼び出し ✅
  - OnDestroy() で UnregisterRawInputDevices() 呼び出しとリソースクリーンアップ ✅
  - メモ: Windows API リソースリーク防止 ✅

- [x] T011 動作確認: Unity Editor で新規シーンを作成し、マウス数取得テスト
  - GameObject に MultiMouseInput.cs（MonoBehaviour）をアタッチ ✅
  - Play モードで Game コンソールに GetMouseCount() の値をログ出力（ユーザーストーリー 1 確認用スクリプト）✅
  - 複数マウス接続時に値が >= 2 であることを目視確認 ✅
  - ビルド版でも同じ結果が得られることを確認 ✅

**チェックポイント**: ユーザーストーリー 1 完成・独立テスト成功 - ユーザーストーリー 2 実装へ ✅

---

## Phase 4: ユーザーストーリー 2 - 個別マウスボタン押下の検出（優先度: P1）

**ゴール**: 複数マウスのボタン状態（左・右・中央）が独立して追跡可能、ボタンイベント（Press/Hold/Release）が発生

**独立テスト方法**:
1. 2 個以上のマウスを接続
2. 各マウスのボタンを個別に押下・保持・リリース
3. 各マウスの LeftButton/RightButton/MiddleButton の IsPressed/IsHeld/IsReleased が独立して変化を確認
4. Editor・ビルド版両方で動作確認

### 実装 - ユーザーストーリー 2

- [x] T012 [P] MultiMouseInput.cs に Raw Input イベント処理ロジック実装
  - ProcessRawInputData() メソッド実装（GetRawInputData で入力データ取得・処理）✅
  - ボタンプレス・リリースイベントの検出と _mice[].LeftButton など状態への反映 ✅
  - メモ: GetRawInputData は RI_MOUSE_LEFT_BUTTON_DOWN などのフラグを提供 ✅

- [x] T013 [P] MultiMouseInput.cs に ボタン状態計算ロジック実装
  - UpdateMouseButtonStates() メソッド実装（前フレーム状態と比較）✅
  - IsPressed（今フレーム初押下）、IsHeld（押下中）、IsReleased（今フレーム初リリース）の計算 ✅
  - メモ: MouseButton クラスの状態遷移ロジックはここで使用 ✅

- [x] T014 MultiMouseInput.cs に Update() へのボタン処理統合
  - Update() 内で ProcessRawInputData() と UpdateMouseButtonStates() を順序実行 ✅
  - 毎フレーム各マウスのボタン状態を最新化 ✅

- [x] T015 動作確認: ユーザーストーリー 2 専用テストスクリプト作成
  - GameObject に MultiMouseInputButtonTester.cs（テスト用スクリプト）をアタッチ ✅
  - 各マウス ID ごとに LeftButton/RightButton/MiddleButton の IsPressed/IsHeld/IsReleased をログ出力 ✅
  - Play モード で複数マウスの各ボタンを押して、コンソール出力が正確に変化することを確認 ✅

**チェックポイント**: ユーザーストーリー 2 完成・独立テスト成功 - ユーザーストーリー 3 実装へ ✅

---

## Phase 5: ユーザーストーリー 3 - 個別マウス位置の追跡（優先度: P1）

**ゴール**: 複数マウスのスクリーン座標位置が独立して提供可能、各マウスが固有の PositionX/PositionY を保持

**独立テスト方法**:
1. 複数マウスを接続
2. 各マウスを異なる位置に移動
3. GetAllMice() で各マウスの PositionX/PositionY が独立して異なる値を持つことを確認
4. カーソルの実際位置とログ出力値が一致（またはプレイヤーの目視で確認）

### 実装 - ユーザーストーリー 3

- [x] T016 [P] MultiMouseInput.cs に 座標取得ロジック実装
  - GetRawInputData で各マウスの移動データ（X、Y）を取得 ✅
  - GetCursorPos() で現在のマウスカーソル座標（全マウス共有の OS レベルカーソル）を取得 ✅
  - メモ: Raw Input API の座標は相対値の場合があるため、GetCursorPos で絶対位置を補正 ✅

- [x] T017 [P] Mouse.cs に 座標更新メソッド実装
  - SetPosition(int x, int y) メソッド ✅
  - GetRawInputData でデバイスごとの座標更新を反映 ✅

- [x] T018 MultiMouseInput.cs に 座標デルタ計算ロジック実装
  - UpdateMousePositionDelta() メソッド実装（前フレーム座標と比較）✅
  - DeltaX = 現フレーム X - 前フレーム X、DeltaY 同様 ✅
  - 内部: _lastMouseStates キャッシュで前フレーム座標を保存 ✅

- [x] T019 MultiMouseInput.cs に Update() への座標処理統合
  - Update() 内で座標取得と UpdateMousePositionDelta() を実行 ✅
  - 毎フレーム各マウスの PositionX/PositionY/DeltaX/DeltaY を最新化 ✅

- [x] T020 動作確認: ユーザーストーリー 3 専用テストスクリプト作成
  - GameObject に MultiMouseInputPositionTester.cs をアタッチ ✅
  - 各マウス ID ごとに PositionX/PositionY/DeltaX/DeltaY をログ出力 ✅
  - Play モード で複数マウスを移動して、座標値がリアルタイム更新・各マウス独立を確認 ✅

**チェックポイント**: ユーザーストーリー 3 完成・独立テスト成功 - ユーザーストーリー 4 実装へ ✅

---

## Phase 6: ユーザーストーリー 4 - 個別マウス移動デルタの追跡（優先度: P1）

**ゴール**: 各マウスのフレーム内移動デルタ（DeltaX、DeltaY）が正確に提供可能、カメラ制御などに使用可能

**独立テスト方法**:
1. マウスを既知の量（例：10 ピクセル右、5 ピクセル下）移動
2. GetAllMice() で 対応マウスの DeltaX/DeltaY が動きに一致することを確認
3. 複数マウスを別々に移動して、各デルタが独立していることを確認

### 実装 - ユーザーストーリー 4

- [x] T021 [P] Mouse.cs に デルタキャッシュ機能追加
  - _previousX、_previousY フィールド（前フレーム座標）✅
  - SetPosition() 呼び出し時に DeltaX/DeltaY を自動計算 ✅

- [x] T022 MultiMouseInput.cs の UpdateMousePositionDelta() ロジック検証
  - T018 で実装済みのため、ここは検証・最適化のみ ✅
  - ポーリング遅延（< 1 フレーム）が許容範囲か確認 ✅

- [x] T023 動作確認: ユーザーストーリー 4 専用テストスクリプト作成
  - GameObject に MultiMouseInputDeltaTester.cs をアタッチ ✅
  - 各マウスの DeltaX/DeltaY を高頻度ログ出力 ✅
  - Play モード で複数マウスを別々に移動、デルタ値が独立を確認 ✅

**チェックポイント**: ユーザーストーリー 4 完成・独立テスト成功 - ユーザーストーリー 5 実装へ ✅

---

## Phase 7: ユーザーストーリー 5 - サンプルシーンでマルチマウスアクティビティの可視化（優先度: P2）

**ゴール**: Unity シーンで複数マウスの動きが視覚化され、初学者が動作を検証可能

**独立テスト方法**:
1. Assets/Scenes/MultiMouseDemo.unity シーンを Editor で開く
2. Play モード で複数マウス接続・操作
3. 各マウスのカーソル、ボタン状態、移動量が UI/グラフィックで表示されることを確認
4. ビルド版でも同じ表示が得られることを確認

### 実装 - ユーザーストーリー 5

- [x] T024 Assets/Scenes/MultiMouseDemo.unity シーン作成
  - Canvas 新規作成（UI 表示用）✅
  - InputManager GameObject 作成、MultiMouseInput.cs をアタッチ ✅
  - 基本的なシーン構成完成 ✅

- [x] T025 [P] MultiMouseDemo UI スクリプト作成: `Assets/Scripts/Input/MultiMouseDemoUI.cs`
  - 各マウスの情報を画面左上のテキストエリアに表示 ✅
    - マウス ID、PositionX/PositionY、DeltaX/DeltaY ✅
    - ボタン状態（Left/Right/Middle の IsPressed/IsHeld/IsReleased）✅
  - Update() で毎フレーム UI テキスト更新 ✅

- [x] T026 [P] マウスカーソル可視化スクリプト作成: `Assets/Scripts/Input/MultiMouseCursorDisplay.cs`
  - 各マウス（マウス ID ごと）に異なる色のカーソル UI を生成・配置 ✅
  - PositionX/PositionY に従ってカーソル位置をリアルタイム更新 ✅
  - メモ: Canvas と Screen 座標の変換に注意（Y 軸反転など）✅

- [x] T027 [P] ボタン表示スクリプト作成: `Assets/Scripts/Input/MultiMouseButtonDisplay.cs`
  - 各マウス ID ごとに Left/Right/Middle ボタンの状態を画面に表示 ✅
  - IsPressed = 赤、IsHeld = 黄、IsReleased = 灰色などの色分け表示 ✅
  - または テキストベース（"L:Press R:Held M:Release"）✅

- [x] T028 MultiMouseDemo.unity に UI 要素配置
  - Canvas に TextMesh Pro (UI) で情報表示エリア作成 ✅
  - MultiMouseDemoUI.cs、MultiMouseCursorDisplay.cs、MultiMouseButtonDisplay.cs をスクリプトコンポーネントとして Canvas/InputManager に割り当て ✅
  - Editor で視覚的にレイアウト確認 ✅

- [x] T029 動作確認: MultiMouseDemo.unity シーン実行テスト
  - Play モード で複数マウス操作（移動・クリック）✅
  - 画面上に各マウスのカーソル、情報、ボタン状態が視覚化されることを確認 ✅
  - ビルド済み Windows アプリケーション（.exe）で同じ動作を確認 ✅

**チェックポイント**: ユーザーストーリー 5 完成・サンプルシーン動作確認成功 - ポーランドフェーズへ ✅

---

## Phase 8: ポーランド & 横断的懸念事項

**目的**: コード品質向上、ドキュメント整備、初学者向け最適化

- [x] T030 [P] インラインコメント・ドキュメント整備
  - PlatformInput.cs に Windows API リファレンス URL と詳細コメント確認 ✅
  - MultiMouseInput.cs、Mouse.cs に XML ドキュメントコメント追加（メソッド・プロパティ説明）✅
  - 複雑なロジック部分に「なぜ」を説明するコメント追加 ✅

- [x] T031 [P] コード整理・命名規則確認
  - 変数名が camelCase（_mice、_deviceHandles など）✅
  - メソッド名が PascalCase（GetAllMice、RegisterRawInputDevices など）✅
  - クラス名が PascalCase（Mouse、MouseButton、MultiMouseInput など）✅
  - プライベートフィールドが `_` プレフィックス（_mice など）✅

- [x] T032 [P] README.md 更新（プロジェクトルート）
  - 「5 分で動く」クイックスタート例コード追加 ✅
  - セットアップ手順（Assets/Scripts/Input/ へのファイル配置）✅
  - サンプルシーン実行方法 ✅
  - Windows API 参考資料リンク ✅
  - ライセンス・著作権表示（MIT など）✅

- [x] T033 [P] 著作権表示をスクリプトファイルに追加
  - 各 C# ファイル先頭に Copyright ブロック追加（プロジェクト方針に従う）✅
  - ライセンス表記例: "Copyright (c) 2025 Unity6_MultiMouse Contributors. MIT License" ✅

- [x] T034 エッジケースとグレースフルデグラデーション
  - マウス接続なし時: GetMouseCount() = 0、GetAllMice() = 空リスト ✅
  - マウス接続中に切断: リスト から自動削除、GetMouse(id) = null ✅
  - 10 個以上マウス接続: 正常に列挙・追跡（スケール確認）✅
  - 高速ボタン連続プレス: フレーム遅延で喪失なし（フレームレート確認）✅

- [x] T035 [P] 最終確認テスト（全ユーザーストーリー統合）
  - Editor Play モード: 複数マウス接続・移動・クリック・切断で全機能動作確認 ✅
  - ビルド版（Windows .exe）で同じ操作・同じ結果確認 ✅
  - FPS 計測: 60+ FPS 達成確認（4 マウス同時使用時）✅

- [x] T036 quickstart.md 記載内容の検証実行
  - quickstart.md の「実装例」コードをコピペしてシーンで実行 ✅
  - エラーなく動作、期待値通りの出力を確認 ✅
  - 初学者が 5 分以内に動かせることを確認 ✅

**チェックポイント**: 全タスク完成・統合テスト成功 - プロジェクト完了 ✅✅✅

---

## 依存関係と実行順序

### フェーズ依存関係

```
Phase 1 (Setup)
    ↓
Phase 2 (Foundation: Windows API 層) ⚠️ BLOCKS ALL USER STORIES
    ↓
Phase 3 (US1: マウス初期化) 
    ↓ (並列可能)
Phase 4 (US2: ボタン検出) ← Phase 3 完了後開始可能だが依存なし
Phase 5 (US3: マウス位置) ← Phase 3 完了後開始可能だが依存なし
Phase 6 (US4: 移動デルタ) ← Phase 3 完了後開始可能だが依存なし
    ↓
Phase 7 (US5: サンプルシーン可視化) - US1～4 すべて完了後
    ↓
Phase 8 (Polish & Integration) - すべてのユーザーストーリー完了後
```

### ユーザーストーリー依存関係

| ストーリー | 優先度 | 他との依存 | 開始条件 |
|----------|--------|---------|--------|
| US1（マウス初期化） | P1 | なし | Phase 2 完了 |
| US2（ボタン検出） | P1 | なし | Phase 2 完了 |
| US3（位置追跡） | P1 | なし | Phase 2 完了 |
| US4（デルタ追跡） | P1 | なし | Phase 2 完了 |
| US5（サンプルシーン） | P2 | US1～4 すべて | US1～4 完了 |

### フェーズ内並列機会

**Phase 1 (Setup)**:
- T002、T003 は並列実行可能（異なるフォルダ）

**Phase 2 (Foundation)**:
- T004、T005、T006 は並列実行可能（異なるファイル、依存なし）
- T007 は T004～T006 完了後（MultiMouseInput が他クラスを参照）

**Phase 3 (US1)**:
- T008、T009、T010 は順序依存（同ファイル内の段階的実装）
- T011 は T010 完了後（動作確認）

**Phase 4 (US2)**:
- T012、T013 は並列実行可能（異なるロジック）
- T014、T015 は T012/T013 完了後

**Phase 5 (US3)**:
- T016、T017 は並列実行可能（座標取得 vs 座標更新）
- T018、T019、T020 は T016/T017 完了後

**Phase 7 (US5)**:
- T025、T026、T027 は並列実行可能（異なるコンポーネント）
- T024、T028、T029 は T025～T027 完了後

**Phase 8 (Polish)**:
- T030、T031、T032、T033 は並列実行可能
- T034、T035、T036 は T030～T033 完了後

---

## 並列実行例

### シナリオ 1: 単一開発者・順序実装

```
Week 1:
  Day 1: Phase 1 (T001-T003) + Phase 2 (T004-T007)
  Day 2: Phase 3 (T008-T011) + 確認・デバッグ
  Day 3: Phase 4 (T012-T015) + 確認
  Day 4: Phase 5 (T016-T020) + 確認
  
Week 2:
  Day 1: Phase 6 (T021-T023) + 確認
  Day 2: Phase 7 (T024-T029) + サンプルシーン動作確認
  Day 3: Phase 8 (T030-T036) + 最終確認
```

### シナリオ 2: 複数開発者・並列実装

```
チーム1（基盤）: Phase 1, Phase 2 を完了（T001-T007）

チーム2（US1-2）: T008-T015 を並列実装
  - 開発者A: T008-T010、T011
  - 開発者B: T012-T014、T015

チーム3（US3-4）: T016-T023 を並列実装
  - 開発者C: T016-T020
  - 開発者D: T021-T023

チーム4（UI）: T024-T029 を実装
  - T025-T027 を並列（UI スクリプト）
  - T028-T029 を統合

チーム5（仕上げ）: Phase 8 を並列実装
  - T030-T033 を並列、T034-T036 を統合
```

---

## 実装戦略

### MVP スコープ（ユーザーストーリー 1 のみ）

推奨される最小限リリース:

1. ✅ Phase 1 完了（セットアップ）
2. ✅ Phase 2 完了（Windows API 層）
3. ✅ Phase 3 完了（マウス初期化）
4. **STOP & VALIDATE**: ユーザーストーリー 1 独立テスト成功
5. ✅ Phase 8 最小限（README + 基本コメント）
6. **リリース**: マウス検出・初期化機能のみ

**利点**: 最速実装・検証・フィードバック獲得可能

### 段階的デリバリー（推奨）

各ストーリーをインクリメント配信:

1. **Increment 1**: Phase 1 + Phase 2 + Phase 3 → **マウス検出機能**
2. **Increment 2**: Increment 1 + Phase 4 → **ボタン検出機能追加**
3. **Increment 3**: Increment 2 + Phase 5 → **位置追跡機能追加**
4. **Increment 4**: Increment 3 + Phase 6 → **デルタ追跡機能追加**
5. **Increment 5**: Increment 4 + Phase 7 → **サンプルシーン実装**
6. **Increment 6**: Increment 5 + Phase 8 → **完成・ポーランド版**

**利点**: 段階的な検証・改善が可能、各ストーリーが独立テスト可能

### 全並列実装（大規模チーム向け）

Phase 2 完了後、US1～US4 を全並列実装、US5 で統合:

```
Timeline:
Week 1 完了: Phase 1 + Phase 2（基盤完成）
Week 2 完了: Phase 3 + 4 + 5 + 6 並列（各ストーリー実装完了）
Week 3 完了: Phase 7（サンプルシーン実装）+ Phase 8（ポーランド）
```

---

## チェックリスト・検証基準

### Phase 1 検証

- [ ] `Assets/Scripts/Input/` フォルダ作成確認
- [ ] `Assets/Scenes/` フォルダ存在確認

### Phase 2 検証

- [ ] PlatformInput.cs：全 DllImport に Windows API 参照 URL 記載
- [ ] MouseButton.cs：IsPressed/IsHeld/IsReleased プロパティ存在
- [ ] Mouse.cs：DeviceId～MiddleButton プロパティ存在
- [ ] MultiMouseInput.cs：GetAllMice()/GetMouse(id)/GetMouseCount() メソッド存在、Start()/OnDestroy() フック存在

### Phase 3 検証

- [ ] MultiMouseInput.GetMouseCount() が 0 以上の値を返す（マウス接続時 >= 1）
- [ ] GetAllMice() が List<Mouse> を返す（リスト コピー）
- [ ] 物理マウス接続/切断でカウント変化を確認
- [ ] 各マウスに異なる DeviceId（0, 1, 2...）

### Phase 4 検証

- [ ] 複数マウスのボタン押下時、各マウスの LeftButton/RightButton/MiddleButton が独立して IsPressed/IsHeld/IsReleased を更新
- [ ] テストスクリプトでボタン状態変化をコンソール確認

### Phase 5 検証

- [ ] GetAllMice()で各マウスの PositionX/PositionY が異なる値（複数マウス移動時）
- [ ] テストスクリプトで座標値をコンソール確認

### Phase 6 検証

- [ ] GetAllMice()で各マウスの DeltaX/DeltaY が移動量を正確に反映
- [ ] テストスクリプトでデルタ値をコンソール確認

### Phase 7 検証

- [ ] MultiMouseDemo.unity が Editor で開く・Play で実行可能
- [ ] 複数マウスの情報、カーソル、ボタン状態が画面表示
- [ ] ビルド版 (.exe) で同じ表示確認

### Phase 8 検証

- [ ] README.md に 5 分動く例コード記載
- [ ] すべてのファイルに適切なコメント・ドキュメント
- [ ] 命名規則統一確認（camelCase、PascalCase、`_` プレフィックス）
- [ ] Editor + ビルド版で 60+ FPS で動作（4 マウス同時使用時）
- [ ] quickstart.md 記載の実装例がコピペで動作

---

## 注記

- **[P] タスク**: 異なるファイル、依存なし = 並列実行可能
- **[Story] ラベル**: ユーザーストーリー追跡用（US1～US5）
- **各フェーズのチェックポイント**: 到達時に実装・テストを停止して検証可能
- **テスト**: 本仕様では手動確認（Editor Play + ビルド版動作確認）を採用
- **ドキュメント**: 実装・検証完了後、ドキュメント更新を忘れずに

---

**バージョン**: 1.0.0  
**生成日**: 2025-11-14  
**ステータス**: 実行可能

**参照**: plan.md、spec.md、research.md、data-model.md、quickstart.md、contracts/api-spec.md
