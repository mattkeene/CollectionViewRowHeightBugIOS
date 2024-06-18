using AlternateProtocols.Models;
using AlternateProtocols.Services;
using Microsoft.Maui.Controls.Handlers.Items;
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
                Debug.WriteLine("InitProtocols() finished execution. Status of FullProtocols:");
                foreach (var protocolGroup in FullProtocols)
                {
                    Debug.WriteLine(protocolGroup.ToString());
                }
            }

        }

        private void CollapseProtocols(bool isNowCollapsed, ProtocolGroup groupToCollapse, ProtocolGroup originalGroup)
        {
            //protocolListView.ItemsSource = null;
            if (isNowCollapsed)
            {
                groupToCollapse.IsCollapsed = true;
                //while (groupToCollapse.Count > 0) groupToCollapse.RemoveAt(0); // This is needed for Android to work around a known issue
                groupToCollapse.Clear(); // Thsi is needed for iOS
            }
            else
            {
                Debug.WriteLine($"Adding to list: \n {originalGroup.ToString()}");
                groupToCollapse.IsCollapsed = false;
                groupToCollapse.AddRange(originalGroup.ToList<Protocol>());
            }
            //protocolListView.ItemsSource = AllProtocols;
            Debug.WriteLine($"CollapseProtocols() finished execution on {groupToCollapse.GroupName}; isNowCollapsed: {isNowCollapsed}; MainThread: {MainThread.IsMainThread}");
        }

        private void ProtocolCategory_Tapped(object sender, EventArgs e)
        {
            Debug.WriteLine($"ProtocolCategory_Tapped() called. MainThread: {MainThread.IsMainThread}");
            //var stopwatch = Stopwatch.StartNew();
            if (sender is Grid grid && grid.BindingContext is ProtocolGroup protocolGroup)
            {
                var originalGroup = FullProtocols?.FirstOrDefault(g => g.GroupName == protocolGroup.GroupName);
                if (originalGroup != null)
                {
                    bool isNowCollapsed = !originalGroup.IsCollapsed;
                    originalGroup.IsCollapsed = isNowCollapsed;
                    Debug.WriteLine($"Tapped on {protocolGroup.GroupName}. isNowCollapsed: {isNowCollapsed}");
                    CollapseProtocols(isNowCollapsed, protocolGroup, originalGroup);
                    ForceUIUpadteAsync();
                }
                
                //Debug.WriteLine($"ProtocolCategory_Tapped({protocolGroup.GroupName}) finished execution in {stopwatch.ElapsedMilliseconds}ms.");
            }
            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                //ForceUIUpadteAsync();
            }
        }

        private async void ForceUIUpadteAsync()
        {
            await Task.Delay(3000);
            Debug.WriteLine("ForceUIUpadteAsync() called.");

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Debug.WriteLine($"ForceUIUpadteAsync() called on MainThread.");
                protocolListView.InvalidateMeasureNonVirtual(Microsoft.Maui.Controls.Internals.InvalidationTrigger.SizeRequestChanged);
            });
        }
    }
}
