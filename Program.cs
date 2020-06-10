using System;
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
        static void Main(string[] args)
        {
            const string dbFileName = "TypeDataDbRootNode.db";
            const string collectionName = "DataObjects";
            const string magicId = "1lZ5vhb.15Db3Y";

            using var db = new LiteDatabase(dbFileName);
            var collection = db.GetCollection<MongoDbDataWrapper<string>>(collectionName);
            var allDocuments = collection.FindAll().ToArray();
            var document = collection.FindById(magicId);

            Console.WriteLine(
                $"Found {allDocuments.FirstOrDefault(d => d.Id == magicId)?.Id ?? "null"} " +
                "using FindAll");

            Console.WriteLine($"Found {document?.d ?? "null"} using FindById");
        }
    }
}
