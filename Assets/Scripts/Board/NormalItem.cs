using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalItem : Item
{
    public enum eNormalType
    {
        TYPE_ONE,
        TYPE_TWO,
        TYPE_THREE,
        TYPE_FOUR,
        TYPE_FIVE,
        TYPE_SIX,
        TYPE_SEVEN,
        TYPE_EIGHT
    }

    public eNormalType ItemType;
    public int ImageIndex = 0; // 0, 1, or 2 representing imageUrl1, imageUrl2, imageUrl3 from the server set

    public void SetType(eNormalType type)
    {
        ItemType = type;
    }

    public override void SetView()
    {
        base.SetView();

        if (View != null)
        {
            SpriteRenderer sp = View.GetComponent<SpriteRenderer>();
            if (sp != null)
            {
                Sprite dynamicSprite = DynamicSpriteManager.Instance.GetSprite(ItemType, ImageIndex);
                if (dynamicSprite != null)
                {
                    sp.sprite = dynamicSprite;
                }
            }
        }
    }

    public void RefreshSprite()
    {
        if (View != null)
        {
            SpriteRenderer sp = View.GetComponent<SpriteRenderer>();
            if (sp != null)
            {
                Sprite dynamicSprite = DynamicSpriteManager.Instance.GetSprite(ItemType, ImageIndex);
                if (dynamicSprite != null)
                {
                    sp.sprite = dynamicSprite;
                }
            }
        }
    }

    protected override string GetPrefabName()
    {
        string prefabname = string.Empty;
        switch (ItemType)
        {
            case eNormalType.TYPE_ONE:
                prefabname = Constants.PREFAB_NORMAL_TYPE_ONE;
                break;
            case eNormalType.TYPE_TWO:
                prefabname = Constants.PREFAB_NORMAL_TYPE_TWO;
                break;
            case eNormalType.TYPE_THREE:
                prefabname = Constants.PREFAB_NORMAL_TYPE_THREE;
                break;
            case eNormalType.TYPE_FOUR:
                prefabname = Constants.PREFAB_NORMAL_TYPE_FOUR;
                break;
            case eNormalType.TYPE_FIVE:
                prefabname = Constants.PREFAB_NORMAL_TYPE_FIVE;
                break;
            case eNormalType.TYPE_SIX:
                prefabname = Constants.PREFAB_NORMAL_TYPE_SIX;
                break;
            case eNormalType.TYPE_SEVEN:
                prefabname = Constants.PREFAB_NORMAL_TYPE_SEVEN;
                break;
            case eNormalType.TYPE_EIGHT:
                prefabname = Constants.PREFAB_NORMAL_TYPE_EIGHT;
                break;
        }

        return prefabname;
    }

    internal override bool IsSameType(Item other)
    {
        NormalItem it = other as NormalItem;

        return it != null && it.ItemType == this.ItemType;
    }
}
