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

RolesTable roleTable = new RolesTable(settings);
roleTable.Insert(new Role("User"));
roleTable.Insert(new Role("Admin"));
roleTable.Insert(new Role("Manager"));
await roleTable.CommitAsync();

var roles = await roleTable.QueryAsync("PartitionKey eq 'Roles'");

Console.ReadLine();