using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;

namespace me.abuena.barcodewalletwatch
{
    public class BarcodeDatabase
    {
        private SQLiteAsyncConnection _database;

        private async Task Init()
        {
            if (_database is null)
            {
                _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
                var result = await _database.CreateTableAsync<Barcode>();
            }
        }

        public async Task<List<Barcode>> GetBarcodesAsync()
        {
            await Init();
            return await _database.Table<Barcode>().ToListAsync();
        }

        public async Task<Barcode> GetBarcodeAsync(int id)
        {
            await Init();
            return await _database.Table<Barcode>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveBarcodeAsync(Barcode item)
        {
            await Init();
            if (item.ID != 0)
                return await _database.UpdateAsync(item);
            return await _database.InsertAsync(item);
        }

        public async Task<int> DeleteBarcodeAsync(Barcode item)
        {
            await Init();
            return await _database.DeleteAsync(item);
        }

        public async Task<int> DeleteAllBarcodesAsync()
        {
            return await _database.DeleteAllAsync<Barcode>();
        }
    }
}