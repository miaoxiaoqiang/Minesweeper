using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Core
{
    /// <summary>
    /// 方块池
    /// </summary>
    /// <remarks>
    /// 有就返回；没有则创建。避免重复创建资源
    /// </remarks>
    public sealed class CellPool
    {
        private static readonly IList<Cell> cellPool;

        static CellPool()
        {
            cellPool = new List<Cell>();
        }

        public static Cell GetCell(ushort index)
        {
            if (cellPool.Count != 0)
            {
                var temp = cellPool.Last();
                temp.Index = index;
                cellPool.RemoveAt(cellPool.Count - 1);
                return temp;
            }
            else
            {
                return new Cell(index);
            }
        }

        public static void ReturnCell(Cell cell)
        {
            if (cell != null)
            {
                cellPool.Add(cell);
            }
        }
    }
}
