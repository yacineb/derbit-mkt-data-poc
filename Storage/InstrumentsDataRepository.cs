using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using WebApi.Storage;

namespace deribit_mktdata.Storage
{
    public class InstrumentsDataRepository
    {
        private const string InstrumentsCollection = "instrument-prices";
        
        private readonly IMongoCollection<InstrumentData> _instruments;

        public InstrumentsDataRepository(DatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DbName);
            _instruments = database.GetCollection<InstrumentData>(InstrumentsCollection);
        }

        public async Task Add(InstrumentData data)
        {
            await _instruments.InsertOneAsync(data);
        }

        public async Task<ICollection<InstrumentData>> Retreive(string instrumentName, long @from, long @to)
        {
            var query = from instr in _instruments.AsQueryable()
                where instr.Name == instrumentName && instr.Timestamp <= @to && instr.Timestamp >= @from
                orderby instr.Timestamp
                select instr;
            
            return await query.ToListAsync();
        }
    }
}