using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Navigation;

using [WAS_APP_NAME];
using [WAS_APP_NAME].Sections;
using [WAS_APP_NAME].ViewModels;

namespace [WAS_APP_NAME].Views

{
    public sealed partial class [COLLECTION_CONFIG_NAME]ListPage : PageBase
    {
        private DataTransferManager _dataTransferManager;
        public ListViewModel<[COLLECTION_SCHEMA_NAME]Schema> ViewModel { get; set; }

        public [COLLECTION_CONFIG_NAME]ListPage()
        {
            this.ViewModel = new ListViewModel<[COLLECTION_SCHEMA_NAME]Schema>(new [COLLECTION_CONFIG_NAME]Config());
            this.InitializeComponent();            
        }

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
            bool supportsHtml = true;
#if WINDOWS_PHONE_APP
            supportsHtml = false;
#endif
            ViewModel.ShareContent(args.Request, supportsHtml);
        }
    }
}
