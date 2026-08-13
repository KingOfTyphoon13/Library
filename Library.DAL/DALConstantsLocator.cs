namespace Library.DAL;

internal static class DALConstantsLocator
{
    public const string InitialCatalog = "master";

    public static class ScriptsFiles
    {
        public const string InitializeDb = _folder + ".CreateLibraryDb.sql";

        public const string SeedDb = _folder + ".LibrarySeed.sql";

        private const string _folder = "Library.DAL.Scripts";
    }
}
