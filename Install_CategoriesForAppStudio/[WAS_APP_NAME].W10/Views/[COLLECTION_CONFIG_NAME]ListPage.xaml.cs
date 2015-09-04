using Windows.UI.Xaml.Navigation;
using [WAS_APP_NAME].Sections;
using [WAS_APP_NAME].ViewModels;
using Windows.ApplicationModel.DataTransfer;
using AppStudio.DataProviders.Json;

namespace [WAS_APP_NAME].Views
{
    public sealed partial class [COLLECTION_CONFIG_NAME]ListPage : PageBase
    {
        private DataTransferManager _dataTransferManager;
        public [COLLECTION_CONFIG_NAME]ListPage()
        {
            this.ViewModel = new ListViewModelWithCategories<JsonDataConfig, [COLLECTION_SCHEMA_NAME]Schema>(new [COLLECTION_CONFIG_NAME]Config());
            this.InitializeComponent();
        }

        public ListViewModelWithCategories<JsonDataConfig, [COLLECTION_SCHEMA_NAME]Schema> ViewModel { get; set; }

        protected async override void LoadState(object navParameter)
        {
            await this.ViewModel.LoadDataAsync(navParameter as ItemViewModel);
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _dataTransferManager = DataTransferManager.GetForCurrentView();
            _dataTransferManager.DataRequested += OnDataRequested;

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _dataTransferManager.DataRequested -= OnDataRequested;

            base.OnNavigatedFrom(e);
        }

        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            ViewModel.ShareContent(args.Request);
        }
    }
}
