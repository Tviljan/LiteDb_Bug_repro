using System;
using System.IO;
using System.Linq;
using LiteDB;

namespace litedb_bug_repro
{
    /// <summary>
    /// A wrapper class for objects to serialize
    /// </summary>
    public class MongoDbDataWrapper<T>
    {

        /// <summary>
        /// Gets and sets the data object
        /// </summary>
        public T d { get; set; }


        /// <summary>
        /// Gets and sets the id of the data object (This is a primary key in MongoDB)
        /// </summary>
        public string Id { get; set; }
    }

    class Program
    {
        const string Database = "Database.db";
        const string DatabaseCopy1 = "DatabaseCopy1.db";
        const string DatabaseCopy2 = "DatabaseCopy2.db";
        const string CollectionName = "DataObjects";
        const string MagicId = "1lZ5vhb.15Db3Y";

        static void Main(string[] _)
        {
            File.Copy(Database, DatabaseCopy1, true);
            File.Copy(Database, DatabaseCopy2, true);

            using var db1 = new LiteDatabase(DatabaseCopy1);
            using var db2 = new LiteDatabase(DatabaseCopy2);

            Console.WriteLine("Finding without rebuild");
            FindMagicId(db1);

            Console.WriteLine("Finding with rebuild");
            db2.Rebuild();
            FindMagicId(db2);

            File.Delete(DatabaseCopy1);
            File.Delete(DatabaseCopy2);
        }

        static void FindMagicId(LiteDatabase db)
        {
            var collection = db.GetCollection<MongoDbDataWrapper<string>>(CollectionName);
            var allDocuments = collection.FindAll();

            var foundByFindById = collection.FindById(MagicId);
            var foundByLinq = allDocuments.FirstOrDefault(d => d.Id == MagicId);

            Console.WriteLine($"Found {foundByLinq?.d ?? "null"} using FindAll + Linq");
            Console.WriteLine($"Found {foundByFindById?.d ?? "null"} using FindById");
        }
    }
}
