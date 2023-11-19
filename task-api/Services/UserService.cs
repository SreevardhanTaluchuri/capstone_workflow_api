using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Runtime.CompilerServices;
using task_api.Interfaces;
using task_api.Models;

namespace task_api.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> loginCollection;
        public UserService(
        IOptions<DatabaseSettings> bookStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                bookStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                bookStoreDatabaseSettings.Value.DatabaseName);

            this.loginCollection = mongoDatabase.GetCollection<User>(
                bookStoreDatabaseSettings.Value.UsersCollectionName);

        }

        public async Task<ActionResult<List<User>>> GetAsync() =>
        await this.loginCollection.Find(_ => true).ToListAsync();

        public async Task<ActionResult<User>> GetAsync(string email) =>
            await this.loginCollection.Find(x => x.Email.ToLower() == email.ToLower()).FirstOrDefaultAsync();


        public async Task<ActionResult<User>> GetUsingIdAsync(string id) =>
            await this.loginCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<ActionResult<string>> CreateAsync(User user)
        {
            await this.loginCollection.InsertOneAsync(user);
            return user.Id;
        }

        public async Task UpdateAsync(string id, User user) =>
            await this.loginCollection.ReplaceOneAsync(x => x.Id == id, user);

        public async Task RemoveAsync(string id) =>
            await this.loginCollection.DeleteOneAsync(x => x.Id == id);
    }
}
