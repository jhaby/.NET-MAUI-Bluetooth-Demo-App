
namespace SmartAsthmaAssistane.Models;

public static class Constants
{
    public const string BaseUrl = "https://ruling-hen-gladly.ngrok-free.app/";
    public const string DatabaseFilename = "TodoSQLite.db3";
    public const SQLite.SQLiteOpenFlags Flags =
    // open the database in read/write mode
    SQLite.SQLiteOpenFlags.ReadWrite |
    // create the database if it doesn't exist
    SQLite.SQLiteOpenFlags.Create |
    // enable multi-threaded database access
    SQLite.SQLiteOpenFlags.SharedCache;
    public static string DatabasePath =>
    Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

    public const string TempValue = "tempValue";
    public const string HumidityValue = "humidityValue";
    public const string DeviceName = " HC-05 ";
    public const string firebaseUser = "firebaseUser";
}
