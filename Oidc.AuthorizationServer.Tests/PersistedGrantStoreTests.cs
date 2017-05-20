using IdentityServer4.Models;
using IdentityServer4.Stores;
using lvl.Oidc.AuthorizationServer.Models;
using lvl.Oidc.AuthorizationServer.Tests.Fixtures;
using lvl.Repositories;
using lvl.Repositories.Querying;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace lvl.Oidc.AuthorizationServer.Tests
{
    [Collection(nameof(OidcAuthorizationServerCollection))]
    public class PersistedGrantStoreTests
    {
        private IRepository<PersistedGrantEntity> PersistedGrantRepository { get; }
        private IPersistedGrantStore PersistedGrantStore { get; }

        public PersistedGrantStoreTests(ServiceProviderFixture serviceProviderFixture)
        {
            var serviceProvider = serviceProviderFixture.ServiceProvider;
            PersistedGrantRepository = serviceProvider.GetRequiredService<IRepository<PersistedGrantEntity>>();
            PersistedGrantStore = serviceProvider.GetRequiredService<IPersistedGrantStore>();
        }

        [Fact]
        public async Task Get_all_will_throw_argument_null_exception_if_subject_is_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => PersistedGrantStore.GetAllAsync(null));
        }

        [Fact]
        public async Task Get_all_will_return_empty_if_no_matching_grants()
        {
            await PersistedGrantRepository.CreateAsync(new PersistedGrantEntity
            {
                ClientId = "get-all-not-matched",
                SubjectId = "get-all-not-matched",
                Key = "get-all-not-matched",
                Type = "grant"
            });

            var matchingGrants = await PersistedGrantStore.GetAllAsync("different-subject");

            Assert.Empty(matchingGrants);
        }

        [Fact]
        public async Task Get_all_will_return_matching_grants()
        {
            await PersistedGrantRepository.CreateAsync(new PersistedGrantEntity
            {
                ClientId = "get-all-matching",
                SubjectId = "get-all-matching",
                Key = "get-all-matching",
                Type = "grant"
            });

            var matchingGrants = await PersistedGrantStore.GetAllAsync("get-all-matching");

            Assert.Equal(matchingGrants.Count(), 1);
        }

        [Fact]
        public async Task Get_will_throw_argument_null_exception_if_key_is_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => PersistedGrantStore.GetAsync(null));
        }

        [Fact]
        public async Task Get_will_return_null_if_no_grant_is_found_with_given_key()
        {
            var matchingGrant = await PersistedGrantStore.GetAsync("non-existant");

            Assert.Null(matchingGrant);
        }

        [Fact]
        public async Task Get_will_return_grant_with_matching_key()
        {
            await PersistedGrantRepository.CreateAsync(new PersistedGrantEntity
            {
                ClientId = "get-matching",
                SubjectId = "get-matching",
                Key = "get-matching",
                Type = "grant"
            });

            var matchinGrant = await PersistedGrantStore.GetAsync("get-matching");

            Assert.NotNull(matchinGrant);
        }

        [Fact]
        public async Task Remove_all_no_type_will_throw_argument_null_exception_if_subject_id_is_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => PersistedGrantStore.RemoveAllAsync(null, ""));
        }

        [Fact]
        public async Task Remove_all_no_type_will_throw_argument_null_exception_if_client_id_is_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => PersistedGrantStore.RemoveAllAsync("", null));
        }

        [Fact]
        public async Task Remove_all_no_type_will_not_remove_any_with_matching_subject_but_mistmatched_client()
        {
            var matchingSubjectMismatchedClient = await PersistedGrantRepository.CreateAsync(new PersistedGrantEntity
            {
                ClientId = "matching-subject-mistmatched-client",
                SubjectId = "matching-subject-mistmatched-client",
                Key = "matching-subject-mistmatched-client",
                Type = "grant"
            });

            await PersistedGrantStore.RemoveAllAsync("matching-subject-mistmatched-client", "non-existant");

            var remainingGrants = await PersistedGrantRepository.GetAsync();
            Assert.Contains(matchingSubjectMismatchedClient, remainingGrants);
        }

        [Fact]
        public async Task Remove_all_no_type_will_not_remove_any_with_matching_client_but_mismatched_subject()
        {
            var matchingClientMistmatchingSubject = await PersistedGrantRepository.CreateAsync(new PersistedGrantEntity
            {
                ClientId = "matching-client-mistmatched-subject",
                SubjectId = "matching-client-mistmatched-subject",
                Key = "matching-client-mistmatched-subject",
                Type = "grant"
            });

            await PersistedGrantStore.RemoveAllAsync("non-existant", "matching-client-mistmatched-subject");

            var remainingGrants = await PersistedGrantRepository.GetAsync();
            Assert.Contains(matchingClientMistmatchingSubject, remainingGrants);
        }

        [Fact]
        public async Task Remove_all_will_remove_one_with_matching_client_and_subject()
        {
            var matchingClientAndSubject = await PersistedGrantRepository.CreateAsync(new PersistedGrantEntity
            {
                ClientId = "one-matching-client-and-subject",
                SubjectId = "one-matching-client-and-subject",
                Key = "one-matching-client-and-subject",
                Type = "grant"
            });

            await PersistedGrantStore.RemoveAllAsync("one-matching-client-and-subject", "one-matching-client-and-subject");

            var remainingGrants = await PersistedGrantRepository.GetAsync();
            Assert.DoesNotContain(matchingClientAndSubject, remainingGrants);
        }

        [Fact]
        public async Task Remove_all_will_remove_two_with_matching_client_and_subject()
        {
            await PersistedGrantRepository.CreateAsync(new PersistedGrantEntity
            {
                ClientId = "two-matching-client-and-subject",
                SubjectId = "two-matching-client-and-subject",
                Key = "two-matching-client-and-subject1",
                Type = "grant"
            });
            await PersistedGrantRepository.CreateAsync(new PersistedGrantEntity
            {
                ClientId = "two-matching-client-and-subject",
                SubjectId = "two-matching-client-and-subject",
                Key = "two-matching-client-and-subject2",
                Type = "grant"
            });

            await PersistedGrantStore.RemoveAllAsync("two-matching-client-and-subject", "two-matching-client-and-subject");

            var remainingGrants = await PersistedGrantRepository.GetAsync();
            Assert.False(remainingGrants.Any(g => g.SubjectId == "two-matching-client-and-subject" && g.ClientId == "two-matching-client-and-subject"));
        }

        [Fact]
        public async Task Remove_all_will_throw_argument_null_exception_when_subject_id_is_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => PersistedGrantStore.RemoveAllAsync(null, "", ""));
        }

        [Fact]
        public async Task Remove_all_will_throw_argument_null_exception_when_client_id_is_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => PersistedGrantStore.RemoveAllAsync("", null, ""));
        }

        [Fact]
        public async Task Remove_all_will_throw_argument_null_exception_when_type_id_is_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => PersistedGrantStore.RemoveAllAsync("", "", null));
        }

        [Fact]
        public async Task Remove_all_will_not_remove_grant_if_subject_doesnt_match()
        {
            var key = "remove-all-unmatched-subject";
            var grantWithUnmatchedSubject = await PersistedGrantRepository.CreateAsync(new PersistedGrantEntity
            {
                ClientId = key,
                SubjectId = key,
                Key = key,
                Type = key
            });

            await PersistedGrantStore.RemoveAllAsync("unmatched", key, key);

            var remainingGrants = await PersistedGrantRepository.GetAsync();
            Assert.Contains(grantWithUnmatchedSubject, remainingGrants);
        }

        [Fact]
        public async Task Remove_all_will_not_remove_grant_if_client_doesnt_match()
        {
            var key = "remove-all-unmatched-client";
            var grantWithUnmatchedSubject = await PersistedGrantRepository.CreateAsync(new PersistedGrantEntity
            {
                ClientId = key,
                SubjectId = key,
                Key = key,
                Type = key
            });

            await PersistedGrantStore.RemoveAllAsync(key, "client", key);

            var remainingGrants = await PersistedGrantRepository.GetAsync();
            Assert.Contains(grantWithUnmatchedSubject, remainingGrants);
        }

        [Fact]
        public async Task Remove_all_will_not_remove_grant_if_type_doesnt_match()
        {
            var key = "remove-all-unmatched-type";
            var grantWithUnmatchedSubject = await PersistedGrantRepository.CreateAsync(new PersistedGrantEntity
            {
                ClientId = key,
                SubjectId = key,
                Key = key,
                Type = key
            });

            await PersistedGrantStore.RemoveAllAsync(key, key, "unmatched");

            var remainingGrants = await PersistedGrantRepository.GetAsync();
            Assert.Contains(grantWithUnmatchedSubject, remainingGrants);
        }

        [Fact]
        public async Task Remove_all_will_remove_one_matching_grant()
        {
            var key = "remove-all-one-matching";
            var matchingClientAndSubject = await PersistedGrantRepository.CreateAsync(new PersistedGrantEntity
            {
                ClientId = key,
                SubjectId = key,
                Key = key,
                Type = key
            });

            await PersistedGrantStore.RemoveAllAsync(key, key, key);

            var remainingGrants = await PersistedGrantRepository.GetAsync();
            Assert.DoesNotContain(matchingClientAndSubject, remainingGrants);
        }

        [Fact]
        public async Task Remove_all_will_remove_two_matching_grants()
        {
            var key = "remove-all-two-matching";
            await PersistedGrantRepository.CreateAsync(new PersistedGrantEntity
            {
                ClientId = key,
                SubjectId = key,
                Key = $"{key}1",
                Type = "key"
            });
            await PersistedGrantRepository.CreateAsync(new PersistedGrantEntity
            {
                ClientId = key,
                SubjectId = key,
                Key = $"{key}2",
                Type = key
            });

            await PersistedGrantStore.RemoveAllAsync(key, key);

            var remainingGrants = await PersistedGrantRepository.GetAsync();
            Assert.False(remainingGrants.Any(g => g.SubjectId == key && g.ClientId == key && g.Type == key));
        }

        [Fact]
        public async Task Remove_will_throw_argument_null_exception_when_key_is_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => PersistedGrantStore.RemoveAsync(null));
        }

        [Fact]
        public async Task Remove_will_not_remove_grant_with_mistmatched_key()
        {
            var grantWithDifferentKey = await PersistedGrantRepository.CreateAsync(new PersistedGrantEntity
            {
                ClientId = "remove-different-key",
                SubjectId = "remove-different-key",
                Key = "remove-different-key",
                Type = "grant"
            });

            await PersistedGrantStore.RemoveAsync("non-existant");

            var remainingGrants = await PersistedGrantRepository.GetAsync();
            Assert.Contains(grantWithDifferentKey, remainingGrants);
        }

        [Fact]
        public async Task Remove_will_remove_grant_with_matching_key()
        {
            var grantWithMatchingKey = await PersistedGrantRepository.CreateAsync(new PersistedGrantEntity
            {
                ClientId = "remove-matching-key",
                SubjectId = "remove-matching-key",
                Key = "remove-matching-key",
                Type = "grant"
            });

            await PersistedGrantStore.RemoveAsync("remove-matching-key");

            var remainingGrants = await PersistedGrantRepository.GetAsync();
            Assert.DoesNotContain(grantWithMatchingKey, remainingGrants);
        }

        [Fact]
        public async Task Store_will_throw_argument_null_exception_when_grant_is_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => PersistedGrantStore.StoreAsync(null));
        }

        [Fact]
        public async Task Store_will_persist_the_given_grant()
        {
            await PersistedGrantStore.StoreAsync(new PersistedGrant
            {
                ClientId = "storing",
                SubjectId = "storing",
                Key = "storing",
                Type = "grant"
            });

            var storedGrantQuery = new Query<PersistedGrantEntity>().Where(g => g.Key == "storing");
            var storedGrants = await PersistedGrantRepository.GetAsync(storedGrantQuery);
            Assert.Equal(storedGrants.Count, 1);
        }
    }
}
