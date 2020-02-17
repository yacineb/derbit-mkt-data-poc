namespace deribit_mktdata.Storage
{
    public class DatabaseSettings
    {
        public string ConnectionString { get; set; } = "mongodb://localhost:27017";

        public string DbName { get; set; } = "deribit-data";
    }
}