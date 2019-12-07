using System.Threading.Tasks;
using SecureNotepad.Models;
using SQLite;

namespace SecureNotepad
{
    public class Database
    {
        readonly SQLiteAsyncConnection _database;

        public Database(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Note>().Wait();
        }

        public Task<int> SaveNoteAsync(Note note)
        {
            return _database.UpdateAsync(note);
        }

        public Task<Note> GetNote()
        {
            return _database.Table<Note>().FirstOrDefaultAsync();
        }

        async public Task<Note> CreateNote()
        {
            await _database.InsertAsync(new Note());
            return await GetNote();
        }

        public void Clear()
        {
            _database.DeleteAllAsync<Note>();
        }
    }
}