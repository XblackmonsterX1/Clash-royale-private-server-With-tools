using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvLogic
{
    public class ChestOrder : Data
    {
        public ChestOrder(Row row, DataTable datatable) : base(row, datatable)
        {
            LoadData(this, GetType(), row, 52);
        }

        public string Name { get; set; }
        public string Chest { get; set; }
    }
}