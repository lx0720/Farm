using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorUI : MonoBehaviour
{
    [SerializeField] private Sprite normalCursorSprite;
    [SerializeField] private Sprite buildCursorSprite;
    [SerializeField] private Sprite chooseCursorSprite;

    [SerializeField]private Image cursorImage;
    [SerializeField]private Image buildImage;

    private CursorType currentCursorType;
    private bool followMousePosition;

    private void Start()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        UpdateCursorStatus();
        UpdateCursorPosition();
    }

    private void UpdateCursorStatus()
    {
        currentCursorType = CursorManager.Instance.GetCursorType();
        switch(currentCursorType)
        {
            case CursorType.Normal:
                cursorImage.sprite = normalCursorSprite;
                followMousePosition = true;
                break;
            case CursorType.Build:
                cursorImage.sprite = buildCursorSprite;
                followMousePosition = true;
                break;
            case CursorType.Choose:
                cursorImage.sprite = chooseCursorSprite;
                followMousePosition = true;
                break;
             case CursorType.None:
                cursorImage.sprite = normalCursorSprite;
                break;
        }
        SetCursorVisible(currentCursorType == CursorType.None);
        followMousePosition = currentCursorType != CursorType.None;
        bool valid = CursorManager.Instance.CursorUseable();

        if (valid)
        {
            SetCursorValid();
        }
        else
        {
            SetCursorInvald();
        }
        cursorImage.SetNativeSize();
    }

    private void UpdateCursorPosition()
    {
        if(followMousePosition)
            cursorImage.rectTransform.position = Input.mousePosition;
    }

    public void SetCursorVisible(bool isVisible)
    {
        Cursor.visible = isVisible;
    }

    private void SetCursorValid()
    {
        cursorImage.color = new Color(1, 1, 1, 1);
        buildImage.color = new Color(1, 1, 1, 0.8f);
    }

    private void SetCursorInvald()
    {
        cursorImage.color = new Color(1, 0, 0, 0.5f);
        buildImage.color = new Color(1, 0, 0, 0.5f);
    }
}
