
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WebManage.Models.Res;

namespace WebManage.Models
{
    public static class RedisHelper
    {
        public static void IniteRedis(IServiceCollection services, IConfiguration configuration) {
            RedisConfig item =services.Configure<RedisConfig>(configuration.GetSection("RedisConfig")).BuildServiceProvider().GetService<IOptions<RedisConfig>>().Value;

        }
    }
}
