using System;
using System.Collections.Generic;
using UnityEngine;


namespace WATP.Map
{
    public enum CellBlockType
    {
        NotBlock,
        Block,
        None
    }

    public enum CellKind
    {
        Cell,           //기본 셀
        FarmLand,       //농장 타일
        Water,          //물 타일
        Portal,         //포탈 타일
        None
    }

    /// <summary>
    /// cell 내부에 이미지 정보, 옵션정보를 가지며
    /// 해당 정보를 가지고 tilemap(cell)을 구성한다.
    /// </summary>
    public class Cell : IComparable
    {
        public string ImagePrefix { get; protected set; }
        public string ImageName { get; protected set; }
        public int ImageIndex { get; protected set; }                                //어떤 이미지에 위치하는지 나타낸다(세이브, 로드시 사용)

        //a* 길찾기 알고리즘에 필요한 property
        public Cell Parent { get; set; }
        public int HCost { get; protected set; }
        public int GCost { get; protected set; }
        public int FCost { get; protected set; }
        //

        public Vector2 Position { get; protected set; }
        public Rect Rect { get; protected set; }


        public CellBlockType BlockType = CellBlockType.NotBlock;

        public readonly List<Cell> NeighborCells = new();
        public CellKind CellKind { get; protected set; } = CellKind.Cell;

        public bool Block { get => BlockType != CellBlockType.NotBlock; }
        public bool ObjectBlock { get; set; } = false;

        public Cell(Vector2 position, float gridSize, char type, bool block)
        {
            Position = position + new Vector2(gridSize * 0.5f, gridSize * 0.5f);
            if (type == default)
            {
                CellKind = CellKind.Cell;
                BlockType = CellBlockType.NotBlock;
            }
            else
            {
                switch (type)
                {
                    case 'C':
                        CellKind = CellKind.Cell;
                        BlockType = CellBlockType.NotBlock;
                        break;
                    case 'F':
                        CellKind = (CellKind.FarmLand);
                        BlockType = CellBlockType.NotBlock;
                        break;
                    case 'W':
                        CellKind = (CellKind.Water);
                        BlockType = CellBlockType.Block;
                        break;
                    case 'P':
                        CellKind = (CellKind.Portal);
                        BlockType = CellBlockType.NotBlock;
                        break;
                }
            }

            if(block && BlockType == CellBlockType.NotBlock)
                BlockType = CellBlockType.Block;

            GCost = 0;
            HCost = 0;
            FCost = 0;

            Parent = null;
        }

        public void Clear()
        {
            NeighborCells.Clear();
        }

        public virtual Vector2 GetPosition()
        {
            return new(Position.x, Position.y);
        }

        public CellKind GetCellType()
        {
            return CellKind;
        }

        public bool CompareCellType(CellKind type)
        {
            return CellKind == type;
        }

        public void AddObject(CellBlockType type)
        {
            BlockType = type;
        }

        public void RemoveObject(CellBlockType type)
        {
            BlockType = CellBlockType.NotBlock;
        }

        public bool AddNeighbor(Cell cell, string pos = "")
        {
            if (cell == null || NeighborCells.Contains(cell)) return false;

            NeighborCells.Add(cell);
            return true;

        }

        public void RemoveNeighbor(Cell cell)
        {
            if (cell != null && NeighborCells.Contains(cell))
                NeighborCells.Remove(cell);
        }

        public void ClearNeighbors()
        {
            NeighborCells.Clear();
        }

        public void ResetCost()
        {
            FCost = 0;
            GCost = 0;
            HCost = 0;
            Parent = null;
        }

        public void CalculateHeuristic(Cell value)
        {
            if (value == null) return;
            int x = Mathf.Abs(Mathf.FloorToInt(value.Position.x - Position.x));
            int y = Mathf.Abs(Mathf.FloorToInt(value.Position.y - Position.y));

            HCost = ((x + y) * 20);
            FCost = GCost + HCost;
        }

        public void SetGCost(int addCost)
        {
            GCost = addCost;
        }

        public void TileImageSetting(string prefix, string imageName, int index)
        {
            ImagePrefix = prefix;
            ImageName = imageName;
            ImageIndex = index;
        }

        public void SetBlock(bool value)
        {
            if (value)
                BlockType = CellBlockType.Block;
            else
                BlockType = CellBlockType.NotBlock;
        }

        public void SetAttribute(char type)
        {
            switch (type)
            {
                case 'C':
                    CellKind = CellKind.Cell;
                    BlockType = CellBlockType.NotBlock;
                    break;
                case 'B':
                    CellKind = CellKind.Cell;
                    BlockType = CellBlockType.Block;
                    break;
                case 'F':
                    CellKind = (CellKind.FarmLand);
                    BlockType = CellBlockType.NotBlock;
                    break;
                case 'W':
                    CellKind = (CellKind.Water);
                    BlockType = CellBlockType.Block;
                    break;
                case 'P':
                    CellKind = (CellKind.Portal);
                    BlockType = CellBlockType.NotBlock;
                    break;
            }
        }

        public char GetKindChar()
        {
            switch (CellKind)
            {
                case CellKind.Cell:
                    return 'C';
                case CellKind.FarmLand:
                    return 'F';
                case CellKind.Water:
                    return 'W';
                case CellKind.Portal:
                    return 'P';
                default:
                    return 'C';
            }
        }

        public int CompareTo(object obj)
        {
            var cell = obj as Cell;

            if (cell == null)
                Debug.LogError("Cell Compare Error - Compare 변수가 Cell 이 아닙니다.");

            return FCost.CompareTo(cell.FCost);
        }
    }
}