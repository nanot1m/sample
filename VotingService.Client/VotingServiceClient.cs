using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Vostok.Clusterclient;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Topology;
using Vostok.Clusterclient.Transport.Http;
using Vostok.Logging;
using Vostok.Sample.VotingService.Client.Models;

namespace Vostok.Sample.VotingService.Client
{
    public class VotingServiceClient
    {
        private readonly ClusterClient cluster;

        public VotingServiceClient(ILog log, Uri host)
        {
            cluster = new ClusterClient(
                log,
                config =>
                {
                    config.ClusterProvider = new FixedClusterProvider(host);
                    config.Transport = new VostokHttpTransport(log);
                });
        }

        public async Task AddCandidateAsync(Candidate candidate)
        {
            var request = Request.Put($"Candidate?{SerializeToQuery(candidate)}");
            var result = await cluster.SendAsync(request).ConfigureAwait(false);
            result.Response.EnsureSuccessStatusCode();
        }

        public async Task RemoveCandidateAsync(CandidateKey candidateKey)
        {
            var request = Request.Delete($"Candidate?{SerializeToQuery(candidateKey)}");
            var result = await cluster.SendAsync(request).ConfigureAwait(false);
            result.Response.EnsureSuccessStatusCode();
        }

        public async Task<Candidate[]> GetBallotAsync(UserKey userKey, int count = 2)
        {
            var request = Request.Get($"Ballot?{SerializeToQuery(userKey)}&count={count}");
            var result = await cluster.SendAsync(request).ConfigureAwait(false);
            result.Response.EnsureSuccessStatusCode();
            return Deserialize<Candidate[]>(result.Response.Content.ToString());
        }

        public async Task VoteAsync(Ballot ballot)
        {
            var request = Request.Post("Ballot").WithContent(Serialize(ballot));
            var result = await cluster.SendAsync(request).ConfigureAwait(false);
            result.Response.EnsureSuccessStatusCode();
        }

        public async Task<LeaderCandidate[]> GetLeaderCandidatesAsync(string groupId, int count = 10)
        {
            var request = Request.Get($"Leaderboard?groupId={groupId}&count={count}");
            var result = await cluster.SendAsync(request).ConfigureAwait(false);
            result.Response.EnsureSuccessStatusCode();
            return Deserialize<LeaderCandidate[]>(result.Response.Content.ToString());
        }

        private static string SerializeToQuery<T>(T value)
        {
            return string.Join(
                "&",
                typeof (T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.GetGetMethod() != null)
                    .Select(p => new {p.Name, Value = (string) p.GetValue(value)})
                    .Where(p => !string.IsNullOrEmpty(p.Value))
                    .Select(p => $"{p.Name}={Uri.EscapeDataString(p.Value)}"));
        }

        private static string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value);
        }

        private static T Deserialize<T>(string content)
        {
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}