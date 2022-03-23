using AzureTables;
using AzureTables.Program;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
	.AddUserSecrets<Program>()
	.Build();

AzureStorageSettings settings = new()
{
    ConnectionString = config["AzureStorageSettings:ConnectionString"]
};

UserTable userTable = new UserTable(settings);

User user = new(
    Guid.NewGuid(),
    "Test",
    "User"
);

userTable.Insert(user);
await userTable.CommitAsync();

User? user1 = await userTable.QueryAsync(user.Id);
userTable.Delete(user1);
await userTable.CommitAsync();

Console.ReadLine();