using System.IO;
using SQLite;
using Tizen.Applications;

namespace me.abuena.barcodewalletwatch
{
    public static class Constants
    {
        private const string DatabaseFilename = "BarcodeWallet.db3";

        public const SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLiteOpenFlags.SharedCache;

        public const double DefaultBarcodeWidth = 300;

        public static string DatabasePath =>
            Path.Combine(Application.Current.DirectoryInfo.Data, DatabaseFilename);
    }
}