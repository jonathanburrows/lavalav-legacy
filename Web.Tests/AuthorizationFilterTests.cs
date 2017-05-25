using lvl.Repositories;
using lvl.Repositories.Querying;
using lvl.TestDomain;
using lvl.Web.Authorization;
using lvl.Web.Tests.Fixtures;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace lvl.Web.Tests
{
    [Collection(nameof(WebCollection))]
    public class AuthorizationFilterTests
    {
        private IRepository<AnomalyPhoto> PhotoRepository { get; }
        private IRepository<Moon> MoonRepository { get; }
        private IRepository<AstronautExamScore> ExamRepository { get; }
        private Impersonator Impersonator { get; }
        private IHttpContextAccessor HttpContextAccessor { get; }

        public AuthorizationFilterTests(WebServiceProviderFixture webServiceProviderFixture)
        {
            var serviceProvider = webServiceProviderFixture.ServiceProvider ?? throw new ArgumentNullException(nameof(webServiceProviderFixture));

            PhotoRepository = serviceProvider.GetRequiredService<IRepository<AnomalyPhoto>>();
            MoonRepository = serviceProvider.GetRequiredService<IRepository<Moon>>();
            ExamRepository = serviceProvider.GetRequiredService<IRepository<AstronautExamScore>>();
            Impersonator = serviceProvider.GetRequiredService<Impersonator>();
            HttpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        }

        [Fact]
        public async Task Get_all_will_return_all_entities_with_no_owned_by_attribute()
        {
            Impersonator.AsAdministrator();
            await MoonRepository.CreateAsync(new Moon());
            await MoonRepository.CreateAsync(new Moon());

            Impersonator.AsUser("random-user");
            var authorizedMoons = await MoonRepository.GetAsync();

            Assert.True(authorizedMoons.Count() >= 2);
        }

        [Fact]
        public async Task Get_all_will_return_all_entities_when_the_actions_have_read()
        {
            Impersonator.AsAdministrator();
            await PhotoRepository.CreateAsync(new AnomalyPhoto { Name = "dark side of the moon", TakenByUserId = "russia" });
            await PhotoRepository.CreateAsync(new AnomalyPhoto { Name = "venus passing the sun", TakenByUserId = "'MERICA!" });

            Impersonator.AsUser("random-user");
            var authorizedPhotos = await PhotoRepository.GetAsync();

            Assert.True(authorizedPhotos.Count() >= 2);
        }

        [Fact]
        public async Task Get_all_will_return_no_entities_when_user_is_not_signed_in()
        {
            Impersonator.AsAdministrator();
            var protectedExam = await ExamRepository.CreateAsync(new AstronautExamScore
            {
                ExamineeUserId = "Jean Luc Picard",
                Passed = true,
                Score = 100
            });

            HttpContextAccessor.HttpContext.User = null;
            var exams = await ExamRepository.GetAsync();

            Assert.DoesNotContain(protectedExam, exams);
        }

        [Fact]
        public async Task Get_all_will_return_no_entities_when_user_isnt_authenticated()
        {
            Impersonator.AsAdministrator();
            var protectedExam = await ExamRepository.CreateAsync(new AstronautExamScore
            {
                ExamineeUserId = "Jean Luc Picard",
                Passed = true,
                Score = 100
            });

            var fakeClaims = new ClaimsIdentity(new[]
            {
                new Claim("name", "Jean Luc Picard"),
                new Claim("sub", "Jean Luc Picard")
            });
            HttpContextAccessor.HttpContext.User = new ClaimsPrincipal(fakeClaims);
            var exams = await ExamRepository.GetAsync();

            Assert.DoesNotContain(protectedExam, exams);
        }

        [Fact]
        public async Task Get_all_will_return_all_entities_when_user_is_administrator()
        {
            Impersonator.AsAdministrator();
            var protectedExam = await ExamRepository.CreateAsync(new AstronautExamScore
            {
                ExamineeUserId = "Jean Luc Picard",
                Passed = true,
                Score = 100
            });

            var exams = await ExamRepository.GetAsync();

            Assert.Contains(protectedExam, exams);
        }

        [Fact]
        public async Task Get_all_will_return_entities_only_belonging_to_user()
        {
            Impersonator.AsAdministrator();
            var ownedExam = await ExamRepository.CreateAsync(new AstronautExamScore
            {
                ExamineeUserId = "Jean Luc Picard",
                Passed = true,
                Score = 100
            });
            var otherPersonsExam = await ExamRepository.CreateAsync(new AstronautExamScore
            {
                ExamineeUserId = "Captain Kirk",
                Passed = false,
                Score = 40
            });

            Impersonator.AsUser("Jean Luc Picard");
            var exams = await ExamRepository.GetAsync();

            Assert.DoesNotContain(otherPersonsExam, exams);
        }

        [Fact]
        public async Task Filtering_will_return_all_entities_with_no_owned_by_attribute()
        {
            Impersonator.AsAdministrator();
            await MoonRepository.CreateAsync(new Moon());
            await MoonRepository.CreateAsync(new Moon());

            Impersonator.AsUser("random-user");
            var query = new Query<Moon>().Where(m => m.Id > 0);
            var authorizedMoons = await MoonRepository.GetAsync(query);

            Assert.True(authorizedMoons.Count >= 2);
        }

        [Fact]
        public async Task Filtering_will_return_all_entities_when_the_actions_have_read()
        {
            Impersonator.AsAdministrator();
            await PhotoRepository.CreateAsync(new AnomalyPhoto { Name = "dark side of the moon", TakenByUserId = "russia" });
            await PhotoRepository.CreateAsync(new AnomalyPhoto { Name = "venus passing the sun", TakenByUserId = "'MERICA!" });

            Impersonator.AsUser("random-user");
            var query = new Query<AnomalyPhoto>().Where(e => e.Id > 0);
            var authorizedPhotos = await PhotoRepository.GetAsync(query);

            Assert.True(authorizedPhotos.Count >= 2);
        }

        [Fact]
        public async Task Filtering_will_return_no_entities_when_user_is_not_signed_in()
        {
            Impersonator.AsAdministrator();
            var protectedExam = await ExamRepository.CreateAsync(new AstronautExamScore
            {
                ExamineeUserId = "Jean Luc Picard",
                Passed = true,
                Score = 100
            });

            HttpContextAccessor.HttpContext.User = null;
            var query = new Query<AstronautExamScore>().Where(e => e.Id > 0);
            var exams = await ExamRepository.GetAsync(query);

            Assert.DoesNotContain(protectedExam, exams.Items);
        }

        [Fact]
        public async Task Filtering_will_return_no_entities_when_user_isnt_authenticated()
        {
            Impersonator.AsAdministrator();
            var protectedExam = await ExamRepository.CreateAsync(new AstronautExamScore
            {
                ExamineeUserId = "Jean Luc Picard",
                Passed = true,
                Score = 100
            });

            var fakeClaims = new ClaimsIdentity(new[]
            {
                new Claim("name", "Jean Luc Picard"),
                new Claim("sub", "Jean Luc Picard")
            });
            HttpContextAccessor.HttpContext.User = new ClaimsPrincipal(fakeClaims);
            var query = new Query<AstronautExamScore>().Where(e => e.Id > 0);
            var exams = await ExamRepository.GetAsync(query);

            Assert.DoesNotContain(protectedExam, exams.Items);
        }

        [Fact]
        public async Task Filtering_will_return_all_entities_when_user_is_administrator()
        {
            Impersonator.AsAdministrator();
            var protectedExam = await ExamRepository.CreateAsync(new AstronautExamScore
            {
                ExamineeUserId = "Jean Luc Picard",
                Passed = true,
                Score = 100
            });

            var query = new Query<AstronautExamScore>().Where(e => e.Id > 0);
            var exams = await ExamRepository.GetAsync(query);

            Assert.Contains(protectedExam, exams.Items);
        }

        [Fact]
        public async Task Filtering_will_return_entities_only_belonging_to_user()
        {
            Impersonator.AsAdministrator();
            var ownedExam = await ExamRepository.CreateAsync(new AstronautExamScore
            {
                ExamineeUserId = "Jean Luc Picard",
                Passed = true,
                Score = 100
            });
            var otherPersonsExam = await ExamRepository.CreateAsync(new AstronautExamScore
            {
                ExamineeUserId = "Captain Kirk",
                Passed = false,
                Score = 40
            });

            Impersonator.AsUser("Jean Luc Picard");
            var query = new Query<AstronautExamScore>().Where(e => e.Id > 0);
            var exams = await ExamRepository.GetAsync(query);

            Assert.DoesNotContain(otherPersonsExam, exams.Items);
        }

        [Fact]
        public async Task Getting_single_will_return_for_entity_with_no_owned_by_attribute()
        {
            Impersonator.AsAdministrator();
            var moon = await MoonRepository.CreateAsync(new Moon());

            Impersonator.AsUser("random-user");
            var matchingMoon = await MoonRepository.GetAsync(moon.Id);

            Assert.NotNull(matchingMoon);
        }

        [Fact]
        public async Task Getting_single_will_return_for_entity_that_allows_anonymous_read()
        {
            Impersonator.AsAdministrator();
            var photo = await PhotoRepository.CreateAsync(new AnomalyPhoto
            {
                Name = "dark side of the moon",
                TakenByUserId = "russia"
            });

            Impersonator.AsUser("random-user");
            var matchingPhoto = await PhotoRepository.GetAsync(photo.Id);

            Assert.NotNull(matchingPhoto);
        }

        [Fact]
        public async Task Getting_single_will_return_null_when_not_signed_in()
        {
            Impersonator.AsAdministrator();
            var exam = await ExamRepository.CreateAsync(new AstronautExamScore
            {
                ExamineeUserId = "Jean Luc Picard",
                Passed = true,
                Score = 100
            });

            // equivilant to logging out.
            HttpContextAccessor.HttpContext.User = null;
            var matchingExam = await ExamRepository.GetAsync(exam.Id);

            Assert.Null(matchingExam);
        }

        [Fact]
        public async Task Getting_single_will_return_null_when_not_authenticated()
        {
            Impersonator.AsAdministrator();
            var exam = await ExamRepository.CreateAsync(new AstronautExamScore
            {
                ExamineeUserId = "Jean Luc Picard",
                Passed = true,
                Score = 100
            });

            var fakeClaims = new ClaimsIdentity(new[]
            {
                new Claim("name", "Jean Luc Picard"),
                new Claim("sub", "Jean Luc Picard")
            });
            var matchingExam = await ExamRepository.GetAsync(exam.Id);

            Assert.NotNull(matchingExam);
        }

        [Fact]
        public async Task Getting_single_will_return_value_when_administrator()
        {
            Impersonator.AsAdministrator();
            var exam = await ExamRepository.CreateAsync(new AstronautExamScore
            {
                ExamineeUserId = "Jean Luc Picard",
                Passed = true,
                Score = 100
            });

            var matchingExam = ExamRepository.GetAsync(exam.Id);

            Assert.NotNull(matchingExam);
        }

        [Fact]
        public async Task Getting_single_will_return_null_when_belongs_to_another_user()
        {
            Impersonator.AsAdministrator();
            var exam = await ExamRepository.CreateAsync(new AstronautExamScore
            {
                ExamineeUserId = "Captain Kirk",
                Passed = false,
                Score = 40
            });

            Impersonator.AsUser("Jean Luc Picard");
            var matchingExam = await ExamRepository.GetAsync(exam.Id);

            Assert.Null(matchingExam);
        }
    }
}
