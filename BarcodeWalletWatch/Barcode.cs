using SQLite;

namespace me.abuena.barcodewalletwatch
{
    public class Barcode
    {
        [PrimaryKey] [AutoIncrement] public int ID { get; set; }

        public string Name { get; set; }
        public string Image { get; set; }
    }
}