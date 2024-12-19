using System;
using System.ComponentModel;
using System.Linq;
using System.Management;

using MvvmLight;
using MvvmLight.CommandWpf;
using MvvmLight.Messaging;

namespace Minesweeper.ViewModel
{
    public sealed class NickNameViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly System.Text.RegularExpressions.Regex _regex;

        public NickNameViewModel()
        {
            _regex = new System.Text.RegularExpressions.Regex(@"^[A-Za-z]+$", System.Text.RegularExpressions.RegexOptions.Compiled);
            UserId = Core.RecordParser.ConfuseBytes(System.Text.Encoding.UTF8.GetBytes(GetCPUSerialNumber() + GetBIOSID()));
            NickName = "匿名";
            ArchiveName = "temp";
        }

        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                string result = string.Empty;
                if (columnName == nameof(NickName))
                {
                    if (string.IsNullOrEmpty(NickName) || NickName.Length > 8)
                    {
                        result = "昵称应不为空且字数不超过8个";
                    }
                }
                else if (columnName == nameof(ArchiveName))
                {
                    if (string.IsNullOrWhiteSpace(ArchiveName) || !_regex.IsMatch(ArchiveName))
                    {
                        result = "存档文件名应不为空且为纯字母";
                    }
                }

                return result;
            }
        }

        public string UserId
        {
            get;
        }

        private string nickname;
        public string NickName
        {
            get => nickname;
            set
            {
                nickname = value;
                RaisePropertyChanged();
                SetArchiveCommnad?.RaiseCanExecuteChanged();
            }
        }

        private string archivename;
        public string ArchiveName
        {
            get => archivename;
            set
            {
                archivename = value;
                RaisePropertyChanged();
                SetArchiveCommnad?.RaiseCanExecuteChanged();
            }
        }

        private RelayCommand setarchivecommnad;
        public RelayCommand SetArchiveCommnad
        {
            get
            {
                setarchivecommnad ??= new RelayCommand(ExecuteSetArchive, CanExecuteSetArchive);
                return setarchivecommnad;
            }
            set
            {
                setarchivecommnad = value;
            }
        }

        private void ExecuteSetArchive()
        {
            Messenger.Default.Send("NickName", "CloseWindowToken");

            Messenger.Default.Send(ValueTuple.Create(UserId, NickName, ArchiveName), "NickNameToken");
        }

        private bool CanExecuteSetArchive()
        {
            if (string.IsNullOrEmpty(NickName))
            {
                return false;
            }
            else if (NickName.Length > 8)
            {
                return false;
            }
            else if (string.IsNullOrWhiteSpace(ArchiveName) || !_regex.IsMatch(ArchiveName))
            {
                return false;
            }

            return true;
        }

        private static string GetCPUSerialNumber()
        {
            using (ManagementClass myCpu = new("win32_Processor"))
            {
                string cpu = string.Empty;
                ManagementObjectCollection myCpuConnection = myCpu.GetInstances();

                foreach (ManagementObject myObject in myCpuConnection.Cast<ManagementObject>())
                {
                    cpu = myObject.Properties["ProcessorId"].Value.ToString();
                    break;
                }
                return cpu;
            }
        }

        private static string GetBIOSID()
        {
            using (ManagementObjectSearcher searcher = new("Select * From Win32_BIOS"))
            {
                string sBIOSSerialNumber = null;
                foreach (ManagementObject mo in searcher.Get().Cast<ManagementObject>())
                {
                    sBIOSSerialNumber = mo.GetPropertyValue("SerialNumber").ToString().Trim();
                    break;
                }
                return sBIOSSerialNumber;
            }
        }
    }
}
