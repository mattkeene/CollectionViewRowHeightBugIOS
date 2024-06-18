using MvvmHelpers;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AlternateProtocols.Models
{
    public class ProtocolGroup : ObservableRangeCollection<Protocol>
    {
        public string GroupName { get; private set; }
        
        private string groupDropdownText;
        public string GroupDropdownText
        {
            get => groupDropdownText;
            //set => SetProperty(ref groupDropdownText, value);
            set
            {
                if (groupDropdownText == value) return;
                groupDropdownText = value;
                OnPropertyChanged(new(nameof(GroupDropdownText)));
            }
        }

        private bool isCollapsed;
        public bool IsCollapsed 
        { 
            get => isCollapsed;
            set
            {
                //SetProperty(ref isCollapsed, value);
                if (isCollapsed == value) return;
                isCollapsed = value;
                OnPropertyChanged(new(nameof(IsCollapsed)));
                GroupDropdownText = DropdownText;
            }
        }

        private string DropdownText => isCollapsed ? "◀" : "▼";

        public ProtocolGroup(string name, bool isCollapsed, List<Protocol> protocols) : base(protocols)
        {
            GroupName = name;
            IsCollapsed = isCollapsed;
        }

        public override string ToString()
        {
            string text = $"Group: {GroupName}, IsCollapsed: {IsCollapsed}, Protocols: \n";
            foreach (var protocol in this)
            {
                text += protocol.ToString();
            }
            return text;
        }

        //protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action? onChanged = null)
        //{
        //    if (EqualityComparer<T>.Default.Equals(backingStore, value))
        //        return false;

        //    backingStore = value;
        //    onChanged?.Invoke();
        //    OnPropertyChanged(propertyName);
        //    return true;
        //}

        //public new event PropertyChangedEventHandler? PropertyChanged;
        //protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
       
    }
}
