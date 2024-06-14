using AlternateProtocols.Models;
using AlternateProtocols.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace AlternateProtocols
{
    public partial class MainPage : ContentPage
    {
        private readonly IProtocolsService protocolsService;
        public ObservableCollection<ProtocolGroup> AllProtocols { get; set; } = new ObservableCollection<ProtocolGroup>();
        private List<ProtocolGroup>? FullProtocols { get; set; }
        public MainPage(IProtocolsService _protocolsService)
        {
            this.protocolsService = _protocolsService;
            InitializeComponent();



            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Debug.WriteLine("OnAppearing() called in ProtocolListView.");
            Debug.WriteLine("Calling InitProtocols()");
            await InitProtocols();
            Debug.WriteLine("Exiting OnAppearing() call");
        }

        private async Task InitProtocols()
        {
            if (FullProtocols == null)
            {
                await protocolsService.GetProtocolsFromConfigAsync();
                FullProtocols = protocolsService.GetProtocolGroupList();
                foreach (var protocolGroup in FullProtocols)
                {
                    if (protocolGroup.IsCollapsed) AllProtocols.Add(new ProtocolGroup(protocolGroup.GroupName, true, new List<Protocol>()));
                    else
                    {
                        ProtocolGroup newGroup = new ProtocolGroup(protocolGroup.GroupName, false, new List<Protocol>());
                        newGroup.AddRange(protocolGroup.ToList<Protocol>());
                        AllProtocols.Add(newGroup);
                    }
                }
            }

        }

        private static void CollapseProtocols(bool isNowCollapsed, ProtocolGroup groupToCollapse, ProtocolGroup originalGroup)
        {
            if (isNowCollapsed)
            {
                groupToCollapse.IsCollapsed = true;
                groupToCollapse.Clear();
            }
            else
            {
                groupToCollapse.IsCollapsed = false;
                groupToCollapse.AddRange(originalGroup.ToList<Protocol>());
            }
        }

        private void ProtocolCategory_Tapped(object sender, EventArgs e)
        {
            Debug.WriteLine("ProtocolCategory_Tapped() called.");
            if (sender is Grid grid && grid.BindingContext is ProtocolGroup protocolGroup)
            {
                var originalGroup = FullProtocols.FirstOrDefault(g => g.GroupName == protocolGroup.GroupName);
                if (originalGroup != null)
                {
                    bool isNowCollapsed = !originalGroup.IsCollapsed;
                    originalGroup.IsCollapsed = isNowCollapsed;

                    CollapseProtocols(isNowCollapsed, protocolGroup, originalGroup);
                }
            }
        }

    }

}
