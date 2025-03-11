namespace CosmosBase
{
    public class SerendipBaseEntity<TId> :BaseEntity
    {
        public TId ObjId { get; set; }
        public string? Sistem_TransactionId { get; set; }
        public string? Sistem_InsertLogin { get; set; }
        public DateTime? Sistem_InsertTime { get; set; }
        public string? Sistem_InsertTerminal { get; set; }
        public string? Sistem_UpdateLogin { get; set; }
        public DateTime? Sistem_UpdateTime { get; set; }
        public string? Sistem_UpdateTerminal { get; set; }

        public DateTime? DeletedDate { get; set; }

        public SerendipBaseEntity()
        {
            ObjId = default!;
            Sistem_InsertTime = default!;
            Sistem_UpdateTime = default!;
            Sistem_TransactionId =$"{Environment.CurrentDirectory}-{Environment.Version}" ;
            Sistem_InsertLogin = "*";
            Sistem_InsertTerminal = "*";
        }

        public SerendipBaseEntity(TId objId)
        {
            ObjId = objId;
        }

        public void SetCreatedAt(DateTime createdAt)
        {
            Sistem_InsertTime = createdAt;
        }

        public void SetUpdatedAt(DateTime updatedAt)
        {
            Sistem_UpdateTime = updatedAt;
        }

    }
}
