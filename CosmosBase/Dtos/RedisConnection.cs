namespace CosmosBase.Dtos
{
    public record RedisConnection
    {
        public RedisConnection(string host, int port, int database, string password, string instanceName)
        {
            Host = host;
            Port = port;
            Database = database;
            Password = password;
            InstanceName = instanceName;
        }

        public string Host { get; set; }
        public int Port { get; set; }
        public int Database { get; set; }
        public string Password { get; set; }
        public string InstanceName { get; set; }
    }
}
