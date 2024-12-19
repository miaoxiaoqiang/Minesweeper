using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MvvmLight;
using Minesweeper.Model;

namespace Minesweeper.Core
{
    /// <summary>
    /// 表示雷区的方块
    /// </summary>
    public sealed class Cell : ViewModelBase
    {
        /// <summary>
        /// 初始化 <see cref="Cell"/> 类的新实例
        /// </summary>
        /// <param name="index"></param>
        public Cell(ushort index)
        {
            Index = index;
            CellImage = CellImage.NotOpened;
        }

        /// <summary>
        /// 方块在雷区的索引
        /// </summary>
        public ushort Index
        {
            get;
            set;
        }

        private CellImage cellimage;
        /// <summary>
        /// 方块状态图像
        /// </summary>
        public CellImage CellImage
        {
            get => cellimage;
            set
            {
                if (cellimage != value)
                {
                    cellimage = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// 指示该方块是否是雷
        /// </summary>
        /// <remarks>
        /// 此属性 <see cref="IsMine"/> 和 属性 <see cref="CellImage"/> 会共同作用于方格显示什么图像
        /// </remarks>
        public bool IsMine
        {
            get;
            set;
        }

        /// <summary>
        /// 指示该方块是否已打开
        /// </summary>
        /// <remarks>
        /// 此属性 <see cref="IsOpened"/> 和 属性 <see cref="CellImage"/> 会共同作用于方格显示什么图像
        /// </remarks>
        public bool IsOpened
        {
            get;
            set;
        }

        /// <summary>
        /// 指示该方块是否是空的。空的代表既不是雷也不是数字
        /// </summary>
        /// <remarks>
        /// 此属性 <see cref="IsBlank"/> 和 属性 <see cref="CellImage"/> 会共同作用于方格显示什么图像
        /// </remarks>
        public bool IsBlank
        {
            get;
            set;
        }

        /// <summary>
        /// 方格数字，代表该方块周围的雷数
        /// </summary>
        /// <remarks>
        /// 此属性 <see cref="AroundMineNum"/> 和 属性 <see cref="CellImage"/> 会共同作用于方格显示什么图像
        /// </remarks>
        public byte AroundMineNum
        {
            get;
            set;
        }

        /// <summary>
        /// 将方格属性重置为初始状态
        /// </summary>
        public void Init()
        {
            IsBlank = true;
            IsOpened = false;
            IsMine = false;
            AroundMineNum = 0;

            if (CellImage != CellImage.NotOpened)
            {
                CellImage = CellImage.NotOpened;
            }
        }
    }
}
