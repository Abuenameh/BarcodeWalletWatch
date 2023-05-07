using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace me.abuena.barcodewalletwatch
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        private readonly App _app;
        private readonly BarcodeDatabase _database;

        public MainPage(App mainApp, BarcodeDatabase barcodeDatabase)
        {
            InitializeComponent();
            _app = mainApp;
            _database = barcodeDatabase;
            BindingContext = this;
            UpdateList();
        }

        public ObservableCollection<Barcode> Barcodes { get; set; } = new ObservableCollection<Barcode>();

        public async void UpdateList()
        {
            var barcodes = await _database.GetBarcodesAsync();
            Barcodes.Clear();
            foreach (var barcode in barcodes)
                Barcodes.Add(barcode);
            Barcodes.Add(new Barcode
            {
                Name = "Sync",
                Image = "Sync"
            });
        }

        private async void BarcodeList_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (!(e.Item is Barcode barcode)) return;
            if (barcode.Name.Equals("Sync") && barcode.Image.Equals("Sync"))
                await _app.Sync();
            else
                await Navigation.PushAsync(new BarcodePage(barcode));
        }
    }
}