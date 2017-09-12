using CooperativeGasPriceBot.Models;
using Lime.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Takenet.MessagingHub.Client.Extensions.Bucket;

namespace CooperativeGasPriceBot.Services
{
    public class UserContextService : IUserContextService
    {
        public static string UserContextKey = nameof(UserContextKey);

        private readonly IBucketExtension _bucket;

        public UserContextService(
            IBucketExtension bucket
            )
        {
            _bucket = bucket;
        }

        public async Task<UserContext> GetContextAsync(Identity user, CancellationToken cancellationToken)
        {
            return await _bucket.GetAsync<UserContext>(GetBucketKey(user, UserContextKey), cancellationToken);
        }

        public async Task SetContextAsync(UserContext context, Identity user, CancellationToken cancellationToken)
        {
            await _bucket.SetAsync(GetBucketKey(user, UserContextKey), context, default(TimeSpan), cancellationToken);
        }

        private string GetBucketKey(Identity user, string key)
        {
            return $"{user.ToString()}_{key}";
        }

    }
}
