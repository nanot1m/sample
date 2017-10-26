using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Logging;
using Vostok.Logging.Logs;
using Vostok.Sample.VotingService.Client;
using Vostok.Sample.VotingService.Client.Models;

namespace Tests
{
    public class VotingService_IntegrationTests
    {
        private ILog log;
        private VotingServiceClient votingServiceClient;

        [SetUp]
        public void SetUp()
        {
            log = new ConsoleLog();
            votingServiceClient = new VotingServiceClient(log, new Uri("http://localhost:33336"));
        }

        [Test]
        public async Task AddCandidatesToOneGroup_NoVotes_GetLeaderCandidates_ReturnsAllCandidates()
        {
            var groupId = Guid.NewGuid().ToString();
            await votingServiceClient.AddCandidateAsync(new Candidate("u1", groupId, "i1", "t1"));
            await votingServiceClient.AddCandidateAsync(new Candidate("u2", groupId, "i2", "t2"));
            var leaderCandidates = await votingServiceClient.GetLeaderCandidatesAsync(groupId);
            leaderCandidates.ShouldBeEquivalentTo(
                new[]
                {
                    new LeaderCandidate("u1", groupId, "i1", "t1", 0, 0, 0),
                    new LeaderCandidate("u2", groupId, "i2", "t2", 0, 0, 0)
                });
        }
    }
}