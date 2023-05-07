using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace me.abuena.barcodewalletwatch
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BarcodePage : ContentPage
    {
        public BarcodePage(Barcode barcode)
        {
            InitializeComponent();
            BarcodeImage.Source =
                ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(barcode.Image)));
        }
    }
}