﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Dashboard
{
    public class SaveDashboardDataHandler : IRequestHandler<SaveDashboardData>
    {
        private readonly ITwitchBucketProvider _bucketProvider;

        public SaveDashboardDataHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucketProvider = bucketProvider;
        }

        public async Task<Unit> Handle(SaveDashboardData request, CancellationToken cancellationToken)
        {
            var bucket = await _bucketProvider.GetBucketAsync();
            var collection = bucket.DefaultCollection();

            await collection.UpsertAsync("homePageInfo", request.HomePageInfo);
            await collection.UpsertAsync("staticContentCommands", request.StaticCommandInfo);
            await collection.UpsertAsync("triviaMessages", request.TriviaMessages);
            return default;
        }
    }
}