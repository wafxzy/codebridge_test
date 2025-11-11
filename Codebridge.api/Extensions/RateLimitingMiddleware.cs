using System.Collections.Concurrent;

namespace Codebridge.API.Extensions
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;
        private readonly ConcurrentDictionary<string, TokenBucket> _clients = new();
        private readonly int _requestsPerSecond;
        private readonly TimeSpan _timeWindow;

        public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger, IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _requestsPerSecond = configuration.GetValue<int>("RateLimit:RequestsPerSecond", 10);
            _timeWindow = TimeSpan.FromSeconds(1);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string clientId = GetClientIdentifier(context);
            TokenBucket bucket = _clients.GetOrAdd(clientId, _ => new TokenBucket(_requestsPerSecond, _timeWindow));

            if (!bucket.TryConsume())
            {
                _logger.LogWarning("Rate limit exceeded for client: {ClientId}", clientId);
                context.Response.StatusCode = 429;
                await context.Response.WriteAsync("Too Many Requests");
                return;
            }

            await _next(context);
        }

        private string GetClientIdentifier(HttpContext context)
        {
            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }

    public class TokenBucket
    {
        private readonly int _capacity;
        private readonly TimeSpan _refillInterval;
        private readonly object _lock = new();
        private int _tokens;
        private DateTime _lastRefill;

        public TokenBucket(int capacity, TimeSpan refillInterval)
        {
            _capacity = capacity;
            _refillInterval = refillInterval;
            _tokens = capacity;
            _lastRefill = DateTime.UtcNow;
        }

        public bool TryConsume()
        {
            lock (_lock)
            {
                Refill();

                if (_tokens > 0)
                {
                    _tokens--;
                    return true;
                }

                return false;
            }
        }

        private void Refill()
        {
            DateTime now = DateTime.UtcNow;
            TimeSpan timeSinceLastRefill = now - _lastRefill;

            if (timeSinceLastRefill >= _refillInterval)
            {
                _tokens = _capacity;
                _lastRefill = now;
            }
        }
    }
}
