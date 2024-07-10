namespace Manager.Api.Features;

public interface IWriteDatabase
{
    List<Account> Accounts { get; set; }
}