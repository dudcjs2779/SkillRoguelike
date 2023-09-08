using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class AutoScroll
{
    static public void MoveViewport(RectTransform itemRect, RectTransform viewportRect, RectTransform contentRect)
    {
        float viewportHeight = viewportRect.rect.size.y;
        float contentTop = contentRect.anchoredPosition.y;
        float contentBottom = contentTop + viewportHeight;

        float itemY = itemRect.anchoredPosition.y;
        float itemTop = itemRect.anchoredPosition.y + itemRect.rect.size.y / 2;
        float itemBottom = itemRect.anchoredPosition.y - itemRect.rect.size.y / 2;

        if (contentBottom + itemBottom < 0)
        {
            contentRect.anchoredPosition = new Vector2(
                contentRect.anchoredPosition.x,
                contentRect.anchoredPosition.y - (contentBottom + itemBottom));
        }
        else if (contentTop + itemTop > 0)
        {
            contentRect.anchoredPosition = new Vector2(
                contentRect.anchoredPosition.x,
                contentRect.anchoredPosition.y - (contentTop + itemTop));
        }
    }
}
