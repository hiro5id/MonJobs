using System.Threading.Tasks;
using MongoDB.Driver;

namespace MonJobs
{
    public class MongoJobAcknowledgmentService : IJobAcknowledgmentService
    {
        private readonly IMongoCollection<Job> _jobs;

        public MongoJobAcknowledgmentService(IMongoDatabase database)
        {
            _jobs = database.GetJobCollection();
        }

        public async Task<AcknowledgementResult> Ack(QueueId queue, JobId id, JobAcknowledgment acknowledgment)
        {
            var builder = Builders<Job>.Filter;

            var matchesQueue = builder.Eq(x => x.QueueId, queue);
            var matchesId = builder.Eq(x => x.Id, id);
            var hasNotBeenAcknowledged = builder.Eq(x => x.Acknowledgment, null);

            var filter = builder.And(matchesQueue, matchesId, hasNotBeenAcknowledged);
            var update = Builders<Job>.Update.Set(x => x.Acknowledgment, acknowledgment);

            var result = await _jobs.FindOneAndUpdateAsync(filter, update);
            
            return new AcknowledgementResult
            {
                Success = result != null
            };
        }
    }
}