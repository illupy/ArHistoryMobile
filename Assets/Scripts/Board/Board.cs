using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board
{
    public enum eMatchDirection
    {
        NONE,
        HORIZONTAL,
        VERTICAL,
        ALL
    }

    private int boardSizeX;

    private int boardSizeY;

    private Cell[,] m_cells;

    private Transform m_root;

    private int m_matchMin;

    public Board(Transform transform, GameSettings gameSettings)
    {
        m_root = transform;

        m_matchMin = gameSettings.MatchesMin;

        this.boardSizeX = gameSettings.BoardSizeX;
        this.boardSizeY = gameSettings.BoardSizeY;

        m_cells = new Cell[boardSizeX, boardSizeY];

        CreateBoard();
    }

    private void CreateBoard()
    {
        float spacing = 1.25f;
        float boardOffsetY = 0.5f; // Dịch chuyển bàn cờ lên trên 0.5f đơn vị để nhường chỗ cho khay chứa ở dưới đáy
        Vector3 origin = new Vector3(-boardSizeX * 0.5f + 0.5f, -boardSizeY * 0.5f + 0.5f, 0f) * spacing;
        origin.y += boardOffsetY;

        GameObject prefabBG = Resources.Load<GameObject>(Constants.PREFAB_CELL_BACKGROUND);
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                GameObject go = GameObject.Instantiate(prefabBG);
                go.transform.position = origin + new Vector3(x * spacing, y * spacing, 0f);
                go.transform.localScale = new Vector3(spacing, spacing, 1f);
                go.transform.SetParent(m_root);

                // Đảm bảo ô nền luôn vẽ ở lớp dưới cùng (phía sau các icon)
                SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sortingOrder = -5;
                }

                Cell cell = go.GetComponent<Cell>();
                cell.Setup(x, y);

                m_cells[x, y] = cell;
            }
        }

        //set neighbours
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                if (y + 1 < boardSizeY) m_cells[x, y].NeighbourUp = m_cells[x, y + 1];
                if (x + 1 < boardSizeX) m_cells[x, y].NeighbourRight = m_cells[x + 1, y];
                if (y > 0) m_cells[x, y].NeighbourBottom = m_cells[x, y - 1];
                if (x > 0) m_cells[x, y].NeighbourLeft = m_cells[x - 1, y];
            }
        }

    }

    internal void Fill()
    {
        int totalCells = boardSizeX * boardSizeY;
        int setsOfThree = totalCells / 3;
        
        List<NormalItem.eNormalType> generatedTypes = new List<NormalItem.eNormalType>();

        var allTypes = System.Enum.GetValues(typeof(NormalItem.eNormalType));
        int totalTypes = allTypes.Length;

        // Đảm bảo mỗi loại cá xuất hiện ít nhất 1 bộ 3
        foreach (NormalItem.eNormalType type in allTypes)
        {
            generatedTypes.Add(type);
            generatedTypes.Add(type);
            generatedTypes.Add(type);
        }

        // Random các slot còn lại
        int remainingSets = setsOfThree - totalTypes;
        for (int i = 0; i < remainingSets; i++)
        {
            NormalItem.eNormalType randomType = Utils.GetRandomNormalType();
            generatedTypes.Add(randomType);
            generatedTypes.Add(randomType);
            generatedTypes.Add(randomType);
        }

        // Fill nốt các ô dư (nếu có) để tránh lỗi mảng
        while (generatedTypes.Count < totalCells)
        {
            generatedTypes.Add(Utils.GetRandomNormalType());
        }

        // Xáo trộn vị trí cá

        for (int i = 0; i < generatedTypes.Count; i++)
        {
            NormalItem.eNormalType temp = generatedTypes[i];
            int randomIndex = UnityEngine.Random.Range(i, generatedTypes.Count);
            generatedTypes[i] = generatedTypes[randomIndex];
            generatedTypes[randomIndex] = temp;
        }

        // Gắn cá lên bàn chơi
        int index = 0;
        Dictionary<NormalItem.eNormalType, int> typeCounts = new Dictionary<NormalItem.eNormalType, int>();
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                Cell cell = m_cells[x, y];
                NormalItem item = new NormalItem();

                NormalItem.eNormalType type = generatedTypes[index];
                item.SetType(type);

                if (!typeCounts.ContainsKey(type))
                {
                    typeCounts[type] = 0;
                }
                item.ImageIndex = typeCounts[type] % 3;
                typeCounts[type]++;

                item.SetView();
                item.SetViewRoot(m_root);

                cell.Assign(item);
                cell.ApplyItemPosition(false);
                
                index++;
            }
        }
    }

    internal void Shuffle()
    {
        List<Item> list = new List<Item>();
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                list.Add(m_cells[x, y].Item);
                m_cells[x, y].Free();
            }
        }

        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                int rnd = UnityEngine.Random.Range(0, list.Count);
                m_cells[x, y].Assign(list[rnd]);
                m_cells[x, y].ApplyItemMoveToPosition();

                list.RemoveAt(rnd);
            }
        }
    }

    public bool IsBoardEmpty()
    {
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                if (!m_cells[x, y].IsEmpty) return false;
            }
        }
        return true;
    }

    public List<Cell> GetAllActiveCells()
    {
        List<Cell> activeCells = new List<Cell>();
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                if (!m_cells[x, y].IsEmpty)
                {
                    activeCells.Add(m_cells[x, y]);
                }
            }
        }
        return activeCells;
    }

    public void RefreshAllItemSprites()
    {
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                Cell cell = m_cells[x, y];
                if (cell != null && cell.Item is NormalItem normalItem)
                {
                    normalItem.RefreshSprite();
                }
            }
        }
    }

    public void Clear()
    {
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                Cell cell = m_cells[x, y];
                cell.Clear();

                GameObject.Destroy(cell.gameObject);
                m_cells[x, y] = null;
            }
        }
    }
}
