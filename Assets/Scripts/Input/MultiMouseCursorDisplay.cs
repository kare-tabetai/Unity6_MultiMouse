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
    /// 複数マウスのカーソルを視覚化するスクリプト。
    /// 各マウスに対して異なる色のカーソル UI を生成・表示します。
    /// </summary>
    public class MultiMouseCursorDisplay : MonoBehaviour
    {
        [SerializeField]
        private Canvas _canvas;

        [SerializeField]
        private Color[] _cursorColors = new Color[]
        {
            Color.red,
            Color.green,
            Color.blue,
            Color.yellow,
            Color.cyan,
            Color.magenta,
        };

        private Image[] _cursorImages;

        private void Start()
        {
            if (_canvas == null)
            {
                _canvas = FindObjectOfType<Canvas>();
            }

            // 最大 6 個のマウス用カーソルを事前作成
            _cursorImages = new Image[6];
            for (int i = 0; i < 6; i++)
            {
                GameObject cursorGO = new GameObject($"Cursor_{i}");
                cursorGO.transform.SetParent(_canvas.transform, false);

                Image image = cursorGO.AddComponent<Image>();
                image.color = _cursorColors[i % _cursorColors.Length];

                RectTransform rectTransform = cursorGO.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(20, 20);

                _cursorImages[i] = image;
                cursorGO.SetActive(false);
            }
        }

        private void Update()
        {
            var mice = MultiMouseInput.GetAllMice();

            for (int i = 0; i < _cursorImages.Length; i++)
            {
                if (i < mice.Count)
                {
                    var mouse = mice[i];
                    _cursorImages[i].gameObject.SetActive(true);

                    RectTransform rectTransform = _cursorImages[i].GetComponent<RectTransform>();
                    // Canvas のサイズに応じて座標を正規化
                    Vector2 anchoredPos = new Vector2(
                        mouse.PositionX - Screen.width / 2f,
                        -(mouse.PositionY - Screen.height / 2f)
                    );
                    rectTransform.anchoredPosition = anchoredPos;
                }
                else
                {
                    _cursorImages[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
