using System;
using System.Xml.Serialization;

using MvvmLight;

namespace Minesweeper.Model
{
    [XmlRoot(ElementName = "archive")]
    [Serializable]
    public sealed class PlayerArchive : ViewModelBase
    {
        public PlayerArchive()
        {
            Records = new ObservableDictionary<GameLevel, Record>(new ObservableDictionary<GameLevel, Record>.LevelComparer());
        }

        private string userid;
        [XmlAttribute(AttributeName = "pin")]
        public string UserID
        {
            get
            {
                return userid;
            }
            set
            {
                userid = value;
                RaisePropertyChanged();
            }
        }

        private string playername;
        [XmlAttribute(AttributeName = "player")]
        public string PlayerName
        {
            get
            {
                return playername;
            }
            set
            {
                playername = value;
                RaisePropertyChanged();
            }
        }

        [XmlElement(ElementName = "records")]
        public ObservableDictionary<GameLevel, Record> Records
        {
            get;
            set;
        }
    }

    [XmlRoot(ElementName = "game")]
    [Serializable]
    public class Record : ViewModelBase
    {
        public Record()
        {
            Histories = new SortableObservableCollection<History>()
            {
                SortingSelector = i => i.TimeCost
            };
        }

        private int rounds;
        [XmlElement(ElementName = "rounds")]
        public int Rounds
        {
            get
            {
                return rounds;
            }
            set
            {
                rounds = value;
                RaisePropertyChanged();
            }
        }

        private int won;
        [XmlElement(ElementName = "won")]
        public int Won
        {
            get
            {
                return won;
            }
            set
            {
                won = value;
                RaisePropertyChanged();
            }
        }

        private int winningstreak;
        [XmlElement(ElementName = "winningstreak")]
        public int WinningStreak
        {
            get
            {
                return winningstreak;
            }
            set
            {
                winningstreak = value;
                RaisePropertyChanged();
            }
        }

        private int losingstreak;
        [XmlElement(ElementName = "losingstreak")]
        public int LosingStreak
        {
            get
            {
                return losingstreak;
            }
            set
            {
                losingstreak = value;
                RaisePropertyChanged();
            }
        }

        [XmlArray(ElementName = "histories")]
        [XmlArrayItem(ElementName = "history")]
        public SortableObservableCollection<History> Histories
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 游戏记录
    /// </summary>
    [XmlRoot(ElementName = "history")]
    [Serializable]
    public sealed class History : ViewModelBase
    {
        private int index;
        [XmlElement(ElementName = "index")]
        public int Index
        {
            get
            {
                return index;
            }
            set
            {
                index = value;
                RaisePropertyChanged();
            }
        }

        [XmlElement(ElementName = "timecost")]
        public double TimeCost
        {
            get;
            set;
        }

        [XmlElement(ElementName = "datetime")]
        public DateTime Date
        {
            get;
            set;
        }

        [XmlElement(ElementName = "minescount")]
        public int MinesCount
        {
            get;
            set;
        }

        [XmlElement(ElementName = "rows")]
        public int Rows
        {
            get;
            set;
        }

        [XmlElement(ElementName = "cols")]
        public int Cols
        {
            get;
            set;
        }

        [XmlIgnore]
        public string Info
        {
            get
            {
                return $"雷区：{Rows.ToString()}行{Cols.ToString()}列\r\n雷数：{MinesCount.ToString()}";
            }
        }
    }
}
