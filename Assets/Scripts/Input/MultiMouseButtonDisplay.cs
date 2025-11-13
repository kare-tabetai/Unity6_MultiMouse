/*
 * Unity6_MultiMouse Project
 * Copyright (c) 2025 Unity6_MultiMouse Contributors
 * License: MIT
 */

using UnityEngine;
using UnityEngine.UI;
using Unity6MultiMouse.Input;

namespace Unity6MultiMouse.Demo
{
    /// <summary>
    /// マウスボタン状態を視覚化するスクリプト。
    /// 各マウスのボタン状態を色分け表示します。
    /// </summary>
    public class MultiMouseButtonDisplay : MonoBehaviour
    {
        [SerializeField]
        private Canvas _canvas;

        [SerializeField]
        private Color _pressColor = Color.red;      // 押下時

        [SerializeField]
        private Color _heldColor = Color.yellow;    // 保持時

        [SerializeField]
        private Color _releasedColor = Color.gray;  // リリース時

        [SerializeField]
        private Color _unpressedColor = Color.white; // 未押下時

        private Image[][] _buttonImages;  // [마우스 ID][버튼 타입]

        private void Start()
        {
            if (_canvas == null)
            {
                _canvas = FindObjectOfType<Canvas>();
            }

            // 最大 6 マウス × 3 ボタンの UI を事前作成
            _buttonImages = new Image[6][];
            for (int i = 0; i < 6; i++)
            {
                _buttonImages[i] = new Image[3]; // Left, Right, Middle

                // 各マウスのボタンコンテナを作成
                GameObject containerGO = new GameObject($"ButtonDisplay_{i}");
                containerGO.transform.SetParent(_canvas.transform, false);
                
                RectTransform containerRect = containerGO.AddComponent<RectTransform>();
                containerRect.anchoredPosition = new Vector2(-150 + i * 50, 100);
                containerRect.sizeDelta = new Vector2(40, 120);

                // Left ボタン
                GameObject leftGO = new GameObject("Left");
                leftGO.transform.SetParent(containerGO.transform, false);
                Image leftImage = leftGO.AddComponent<Image>();
                leftImage.color = _unpressedColor;
                RectTransform leftRect = leftGO.GetComponent<RectTransform>();
                leftRect.sizeDelta = new Vector2(30, 30);
                leftRect.anchoredPosition = new Vector2(0, 40);
                _buttonImages[i][0] = leftImage;

                // Right ボタン
                GameObject rightGO = new GameObject("Right");
                rightGO.transform.SetParent(containerGO.transform, false);
                Image rightImage = rightGO.AddComponent<Image>();
                rightImage.color = _unpressedColor;
                RectTransform rightRect = rightGO.GetComponent<RectTransform>();
                rightRect.sizeDelta = new Vector2(30, 30);
                rightRect.anchoredPosition = new Vector2(0, 0);
                _buttonImages[i][1] = rightImage;

                // Middle ボタン
                GameObject middleGO = new GameObject("Middle");
                middleGO.transform.SetParent(containerGO.transform, false);
                Image middleImage = middleGO.AddComponent<Image>();
                middleImage.color = _unpressedColor;
                RectTransform middleRect = middleGO.GetComponent<RectTransform>();
                middleRect.sizeDelta = new Vector2(30, 30);
                middleRect.anchoredPosition = new Vector2(0, -40);
                _buttonImages[i][2] = middleImage;

                containerGO.SetActive(false);
            }
        }

        private void Update()
        {
            var mice = MultiMouseInput.GetAllMice();

            for (int i = 0; i < _buttonImages.Length; i++)
            {
                GameObject containerGO = _buttonImages[i][0].transform.parent.gameObject;

                if (i < mice.Count)
                {
                    var mouse = mice[i];
                    containerGO.SetActive(true);

                    // Left ボタン
                    _buttonImages[i][0].color = GetButtonColor(mouse.LeftButton);

                    // Right ボタン
                    _buttonImages[i][1].color = GetButtonColor(mouse.RightButton);

                    // Middle ボタン
                    _buttonImages[i][2].color = GetButtonColor(mouse.MiddleButton);
                }
                else
                {
                    containerGO.SetActive(false);
                }
            }
        }

        private Color GetButtonColor(MouseButton button)
        {
            if (button.IsPressed)
                return _pressColor;
            if (button.IsHeld)
                return _heldColor;
            if (button.IsReleased)
                return _releasedColor;
            return _unpressedColor;
        }
    }
}
