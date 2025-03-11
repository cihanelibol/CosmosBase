namespace CosmosBase
{
    public  class SerendipEntity : SerendipBaseEntity<long>
    {
        public bool? Aktif { get; set; } = true;
        public byte[]? Sistem_Timestamp { get; set; } = null!;

    }
}
