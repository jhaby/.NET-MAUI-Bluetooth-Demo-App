using SmartAsthmaAssistane.Models;
using SQLite;

namespace SmartAsthmaAssistane.Services;

public class DataService
{
    SQLiteAsyncConnection Database;

    async Task Init()
    {
        if (Database is not null)
            return;
        Database = new SQLiteAsyncConnection(Constants.DatabasePath,
       Constants.Flags);
        await Database.CreateTableAsync<Item>();
        await Database.CreateTableAsync<Humidity>();
    }

    public async Task DeleteItems()
    {
        await Init();
        await Database.DeleteAllAsync<Item>();
    }

    public async Task<List<Item>> GetItemsAsync()
    {
        await Init();
        return await Database.Table<Item>()
            .ToListAsync();
    }


    public async Task<int> SaveSensorAsync(Item item)
    {
        await Init();
        return await Database.InsertAsync(item);
    }
    public async Task<int> SaveHumidityAsync(Humidity item)
    {
        await Init();
        return await Database.InsertAsync(item);
    }
    public async Task<int> UpdateSensorValue(int id, Item value)
    {
        await Init();
        return await Database.UpdateAsync(value);
    }
    public async Task<int> UpdateHumidityValue(int id, Item value)
    {
        await Init();
        return await Database.UpdateAsync(value);
    }

}
