namespace TradeUnionBureauSystem.Model
{
    public partial class Members
    {
        public string FullName => $"{LastName} {FirstName} {MiddleName}";
    }
}
