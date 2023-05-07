using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Samsung.Sap;
using SQLitePCL;
using Tizen.Wearable.CircularUI.Forms;
using TizenHotReloader;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DataReceivedEventArgs = Samsung.Sap.DataReceivedEventArgs;

namespace me.abuena.barcodewalletwatch
{
    public class BarcodeTransfer
    {
        public string Name { get; set; }
        public string Image { get; set; }
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class App : Application
    {
        private Agent _agent;
        private Connection _connection;
        private readonly BarcodeDatabase _database = new BarcodeDatabase();
        private readonly MainPage _mainPage;
        private Peer _peer;

        public App()
        {
            InitializeComponent();

            InitializeSQLite();

            _mainPage = new MainPage(this, _database);
            MainPage = new NavigationPage(_mainPage);
#if DEBUG
            HotReloader.Open(this);
#endif
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        private void InitializeSQLite()
        {
            // Need to initialize SQLite
            raw.SetProvider(new SQLite3Provider_sqlite3());
            raw.FreezeProvider();
        }

        public async Task<bool> Sync()
        {
            try
            {
                if (_connection == null)
                {
                    _agent = await Agent.GetAgent("/me/abuena/barcodewallet/barcodes");
                    _agent.PeerStatusChanged += PeerStatusChanged;
                    var peers = await _agent.FindPeers();
                    _connection = peers.FirstOrDefault()?.Connection;
                    if (_connection != null)
                    {
                        _connection.DataReceived -= Connection_DataReceived;
                        _connection.DataReceived += Connection_DataReceived;
                        _connection.StatusChanged -= Connection_StatusChanged;
                        _connection.StatusChanged += Connection_StatusChanged;
                        await _connection.Open();
                    }
                    else
                    {
                        Toast.DisplayText("Barcode Wallet not running on phone");
                        return false;
                    }
                }

                _connection.Send(_agent.Channels.First().Value, Encoding.UTF8.GetBytes("SYNC"));
                return true;
            }
            catch (Exception ex)
            {
                // Toast.DisplayText(ex.Message);
                Toast.DisplayText("Sync error");
                return false;
            }
        }

        private void PeerStatusChanged(object sender, PeerStatusEventArgs e)
        {
            if (e.Peer == _peer) ShowMessage($"Peer Available: {e.Available}, Status: {e.Peer.Status}");
        }

        private async void Connection_DataReceived(object sender, DataReceivedEventArgs e)
        {
            await _database.DeleteAllBarcodesAsync();
            var message = Encoding.UTF8.GetString(e.Data);
            var transfers = JsonSerializer.Deserialize<List<BarcodeTransfer>>(message);
            foreach (var barcode in transfers.Select(transfer => new Barcode
                     {
                         Name = transfer.Name,
                         Image = transfer.Image
                     }))
            {
                await _database.SaveBarcodeAsync(barcode);
            }

            _mainPage.UpdateList();
            ShowMessage(Encoding.UTF8.GetString(e.Data));
        }

        private void Connection_StatusChanged(object sender, ConnectionStatusEventArgs e)
        {
            ShowMessage(e.Reason.ToString());

            if (e.Reason != ConnectionStatus.ConnectionClosed &&
                e.Reason != ConnectionStatus.ConnectionLost) return;
            _connection.DataReceived -= Connection_DataReceived;
            _connection.StatusChanged -= Connection_StatusChanged;
            _connection.Close();
            _connection = null;
            _peer = null;
            _agent = null;
        }

        private static void ShowMessage(string message, string debugLog = null)
        {
            Debug.WriteLine("[DEBUG] " + message);
        }
    }
}