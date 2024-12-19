using System;
using System.Linq;

using MvvmLight;
using MvvmLight.CommandWpf;
using MvvmLight.Messaging;
using Minesweeper.Model;

namespace Minesweeper.ViewModel
{
    public sealed class HeroRankViewModel : ViewModelBase
    {
        private string archivefile = string.Empty;
        private bool hassetted = false;

        public HeroRankViewModel()
        {
            Messenger.Default.Register<ValueTuple<string, string, string>>(this, "NickNameToken", ReceiveNickName);
            Messenger.Default.Register<Tuple<GameLevel, int, int, int, History>>(this, "NewRecordToken", ReceiveNewHistory);

            PlayerArchive = new PlayerArchive();
        }

        private PlayerArchive playerarchive;
        public PlayerArchive PlayerArchive
        {
            get
            {
                return playerarchive;
            }
            set
            {
                playerarchive = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<GameLevel> resetdatacommand;
        public RelayCommand<GameLevel> ResetDataCommand
        {
            get
            {
                resetdatacommand ??= new RelayCommand<GameLevel>(ExecuteResetData);
                return resetdatacommand;
            }
            set
            {
                resetdatacommand = value;
            }
        }

        private void ExecuteResetData(GameLevel index)
        {
            if (Enum.IsDefined(typeof(GameLevel), index))
            {
                PlayerArchive.Records.Remove(index);

                Core.RecordParser.SaveRecord(PlayerArchive, AppDomain.CurrentDomain.BaseDirectory + "\\record\\" + archivefile + ".record");
            }
        }

        private void ReceiveNewHistory(Tuple<GameLevel, int, int, int, History> tuple)
        {
            if (!hassetted)
            {
                return;
            }

            try
            {
                if (PlayerArchive.Records.ContainsKey(tuple.Item1))
                {
                    Record record = PlayerArchive.Records[tuple.Item1];
                    record.Rounds++;
                    record.Won += tuple.Item2;

                    if (tuple.Item3 > record.WinningStreak)
                    {
                        record.WinningStreak = tuple.Item3;
                    }
                    if (tuple.Item4 > record.LosingStreak)
                    {
                        record.LosingStreak = tuple.Item4;
                    }

                    if (tuple.Item5 != null)
                    {
                        if (record.Histories.Count > 10)
                        {
                            if (record.Histories.Max(p => p.TimeCost) > tuple.Item5.TimeCost)
                            {
                                record.Histories.RemoveAt(record.Histories.Count - 1);
                            }
                        }

                        record.Histories.Add(tuple.Item5);

                        for (int i = 0; i < record.Histories.Count; i++)
                        {
                            record.Histories[i].Index = i + 1;
                        }
                    }
                }
                else
                {
                    Record record = new()
                    {
                        Rounds = 1,
                        Won = tuple.Item2
                    };
                    record.WinningStreak = record.Won;
                    record.LosingStreak = Math.Abs(record.Won - 1);
                    
                    if(tuple.Item5 != null)
                    {
                        tuple.Item5.Index = 1;
                        record.Histories.Add(tuple.Item5);
                    }

                    PlayerArchive.Records.Add(tuple.Item1, record);
                }

                Core.RecordParser.SaveRecord(PlayerArchive, AppDomain.CurrentDomain.BaseDirectory + "\\record\\" + archivefile + ".record");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ReceiveNickName(ValueTuple<string, string, string> data)
        {
            hassetted = true;
            PlayerArchive.UserID = data.Item1;
            PlayerArchive.PlayerName = data.Item2;
            archivefile = data.Item3;

            System.IO.Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\record\\");

            PlayerArchive archive = null;
            try
            {
                if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\record\\" + archivefile + ".record"))
                {
                    archive = Core.RecordParser.LoadRecord(AppDomain.CurrentDomain.BaseDirectory + "\\record\\" + archivefile + ".record");
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (archive != null && archive.UserID == data.Item1 && archive.PlayerName == data.Item2)
                {
                    PlayerArchive = archive;
                }
            }

            //try
            //{
            //    //recordParser.SaveRecord(new PlayerArchive(), AppDomain.CurrentDomain.BaseDirectory + "\\record\\" + archivefile + ".dat");
            //    //var asd = recordParser.LoadRecord(AppDomain.CurrentDomain.BaseDirectory + "\\record\\" + archivefile + ".dat");
            //}
            //catch (Exception ex)
            //{
            //    throw;
            //}

            //System.Reflection.Assembly assm = System.Reflection.Assembly.GetExecutingAssembly();
            //System.IO.Stream istr = assm.GetManifestResourceStream(assm.GetName().Name + "\\db\\record.accdb");

            //System.IO.Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\record\\");

            //using (System.IO.FileStream fileStream = new System.IO.FileStream(AppDomain.CurrentDomain.BaseDirectory + $"\\record\\{accessfile}.accdb", System.IO.FileMode.Create))
            //{
            //    var buffer = new byte[1024];
            //    int length = 0;
            //    using (System.IO.Stream stream = Application.GetResourceStream(new Uri("db\\record.accdb", UriKind.Relative)).Stream)
            //    {
            //        while ((length = stream.Read(buffer, 0, 1024)) > 0)
            //        {
            //            fileStream.Write(buffer, 0, length);
            //        }
            //    }
            //}
        }

        public override void Cleanup()
        {
            base.Cleanup();

            Messenger.Default.Unregister(this);
        }
    }
}
