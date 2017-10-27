using System;
using System.Linq;
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

        [Test]
        public async Task AddCandidatesToOneGroup_RemoveOneCandidate_GetLeaderCandidates_ReturnsCandidatesButRemoved()
        {
            var groupId = Guid.NewGuid().ToString();
            var candidate1 = new Candidate("u1", groupId, "i1", "t1");
            var candidate2 = new Candidate("u2", groupId, "i2", "t2");
            await votingServiceClient.AddCandidateAsync(candidate1);
            await votingServiceClient.AddCandidateAsync(candidate2);
            await votingServiceClient.RemoveCandidateAsync(new CandidateKey(candidate2));
            var leaderCandidates = await votingServiceClient.GetLeaderCandidatesAsync(groupId);
            leaderCandidates.ShouldBeEquivalentTo(
                new[]
                {
                    new LeaderCandidate("u1", groupId, "i1", "t1", 0, 0, 0)
                });
        }

        [Test]
        public async Task AddCandidatesToOneGroup_Votes_GetLeaderCandidates_ReturnsAllCandidatesInValidOrder()
        {
            var groupId = Guid.NewGuid().ToString();
            var candidate1 = new Candidate("u1", groupId, "i1", "t1");
            var candidate2 = new Candidate("u2", groupId, "i2", "t2");
            await votingServiceClient.AddCandidateAsync(candidate1);
            await votingServiceClient.AddCandidateAsync(candidate2);
            await votingServiceClient.VoteAsync(new BallotCandidate(candidate1, true), new BallotCandidate(candidate2, false));
            var leaderCandidates = await votingServiceClient.GetLeaderCandidatesAsync(groupId);
            leaderCandidates.ShouldBeEquivalentTo(
                new[]
                {
                    new LeaderCandidate("u1", groupId, "i1", "t1", 1, 1, 1),
                    new LeaderCandidate("u2", groupId, "i2", "t2", 1, 0, 0)
                });
        }

        [Test]
        public async Task AddCandidatesToOneGroup_GetBallot_ReturnsCandidatesAlwaysNotBelongingToAskingUser()
        {
            var groupId = Guid.NewGuid().ToString();
            var candidate1 = new Candidate("u1", groupId, "i1", "t1");
            var candidate2 = new Candidate("u2", groupId, "i2", "t2");
            var candidate3 = new Candidate("u3", groupId, "i3", "t3");
            await votingServiceClient.AddCandidateAsync(candidate1);
            await votingServiceClient.AddCandidateAsync(candidate2);
            await votingServiceClient.AddCandidateAsync(candidate3);

            var userKey = new UserKey(candidate1);
            for (var i = 0; i < 10; i++)
            {
                var candidates = await votingServiceClient.GetBallotAsync(userKey);
                candidates.ShouldBeEquivalentTo(new[] {candidate2, candidate3});
            }
        }
    }
}