namespace MYMAUIAPP.Components.Data;

public static class SqlitePathProvider
{
    public static string GetDbPath(string fileName = "journalapp.db")
    {
        var baseDir = FileSystem.AppDataDirectory;
        Directory.CreateDirectory(baseDir);
        return Path.Combine(baseDir, fileName);
    }
}
