using lvl.Ontology;
using lvl.Repositories;
using lvl.Repositories.Querying;
using lvl.TestDomain;
using lvl.Web.OData;
using lvl.Web.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace lvl.Web.Tests
{
    [Collection(WebCollection.Name)]
    public class ODataConventionParserTests
    {
        private ODataConventionTokenizer Tokenizer { get; }
        private ODataConventionParser Parser { get; }
        private IServiceProvider Services { get; }

        public ODataConventionParserTests(WebServiceProviderFixture webServiceProviderFixture)
        {
            Services = webServiceProviderFixture.ServiceProvider ?? throw new ArgumentNullException(nameof(webServiceProviderFixture));

            Parser = Services.GetRequiredService<ODataConventionParser>();
            Tokenizer = Services.GetRequiredService<ODataConventionTokenizer>();
        }

        [Fact]
        public async Task OneEqualsOne_ReturnsEntireSet()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon());
            await repository.CreateAsync(new Moon());
            await repository.CreateAsync(new Moon());
            var total = (await repository.GetAsync()).Count();
            var query = CompileQuery<Moon>("1 eq 1");

            var filteredResult = await repository.GetAsync(query);

            Assert.Equal(total, filteredResult.Items.Count());
        }

        [Fact]
        public async Task OneEqualsTwo_ReturnsEmptySet()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon());
            var query = CompileQuery<Moon>("1 ne 1");

            var filteredResult = await repository.GetAsync(query);

            Assert.Equal(0, filteredResult.Items.Count());
        }

        [Fact]
        public async Task FilteringOnProperty_ReturnsMatchingProperties()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Moon { Name = "Luna" });
            await repository.CreateAsync(new Moon { Name = "Old Moon" });
            var query = CompileQuery<Moon>("Name eq 'Luna'");

            var filteredResult = await repository.GetAsync(query);

            Assert.Equal(1, filteredResult.Items.Count());
        }

        [Fact]
        public async Task FilteringNotEqualsOnProperty_ReturnsUnmatchedProperties()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Moon { Name = "Luna" });
            await repository.CreateAsync(new Moon { Name = "Old Moon" });
            var query = CompileQuery<Moon>("Name ne 'Luna'");

            var filteredResult = await repository.GetAsync(query);

            Assert.Equal(1, filteredResult.Items.Count());
        }

        [Fact]
        public async Task FilteringNotEqualsOnProperty_AndSetContainsNull_NullIsReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Moon { Name = null });
            var query = CompileQuery<Moon>("Name ne 'Luna'");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task GreaterThanOnPropety_WhenGreaterThan_ReturnsRecord()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Moon { Radius = 3 });
            var query = CompileQuery<Moon>("Radius gt 2");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task GreaterThanProperty_WhenEqual_DoesNotReturnRecord()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Moon { Radius = 3 });
            var query = CompileQuery<Moon>("Radius gt 3");

            var filteredResult = await repository.GetAsync(query);

            Assert.Empty(filteredResult.Items);
        }

        [Fact]
        public async Task GreaterThanProperty_WhenLessThan_DoesNotReturnRecord()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Moon { Radius = 3 });
            var query = CompileQuery<Moon>("Radius gt 4");

            var filteredResult = await repository.GetAsync(query);

            Assert.Empty(filteredResult.Items);
        }

        [Fact]
        public async Task GreaterThanEqual_WhenPropertyGreaterThan_ReturnIsRecord()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Moon { Radius = 3 });
            var query = CompileQuery<Moon>("Radius ge 2");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task GreaterThanEqual_WhenPropertyEqual_RecordIsReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Moon { Radius = 3 });
            var query = CompileQuery<Moon>("Radius ge 3");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task GreaterThanEqual_WhenPropertyLessThan_RecordIsNotReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Moon { Radius = 3 });
            var query = CompileQuery<Moon>("Radius ge 4");

            var filteredResult = await repository.GetAsync(query);

            Assert.Empty(filteredResult.Items);
        }

        [Fact]
        public async Task LessThan_WhenPropertyLessThan_RecordIsReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Moon { Radius = 3 });
            var query = CompileQuery<Moon>("Radius lt 4");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task LessThan_WhenPropertyEquals_NoRecordsAreReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Moon { Radius = 3 });
            var query = CompileQuery<Moon>("Radius lt 3");

            var filteredResult = await repository.GetAsync(query);

            Assert.Empty(filteredResult.Items);
        }

        [Fact]
        public async Task LessThan_WhenPropertyGreaterThan_NoRecordsAreReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Moon { Radius = 3 });
            var query = CompileQuery<Moon>("Radius lt 2");

            var filteredResult = await repository.GetAsync(query);

            Assert.Empty(filteredResult.Items);
        }

        [Fact]
        public async Task LessThanEquals_WhenPropertyLessThan_RecordsAreReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Moon { Radius = 3 });
            var query = CompileQuery<Moon>("Radius le 4");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task LessThanEquals_WhenPropertyEquals_RecordsAreReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Moon { Radius = 3 });
            var query = CompileQuery<Moon>("Radius le 3");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task LessThanEquals_WhenPropertyGreaterThan_NoRecordsAreReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Moon { Radius = 3 });
            var query = CompileQuery<Moon>("Radius le 2");

            var filteredResult = await repository.GetAsync(query);

            Assert.Empty(filteredResult.Items);
        }

        [Fact]
        public async Task And_WhenLeftIsFalse_NoRecordsAreReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon());
            var query = CompileQuery<Moon>("false and true");

            var filteredResult = await repository.GetAsync(query);

            Assert.Empty(filteredResult.Items);
        }

        [Fact]
        public async Task And_WhenBothTrue_RecordsAreReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon());
            var query = CompileQuery<Moon>("true and true");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task And_WhenBothFalse_NoRecordsAreReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon());
            var query = CompileQuery<Moon>("false and false");

            var filteredResult = await repository.GetAsync(query);

            Assert.Empty(filteredResult.Items);
        }

        [Fact]
        public async Task Or_WhenBothTrue_RecordsAreReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon());
            var query = CompileQuery<Moon>("true or true");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task Or_WhenLeftTrueRightFalse_RecordsAreReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon());
            var query = CompileQuery<Moon>("true or false");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task Or_WhenLeftFalseRightTrue_RecordsAreReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon());
            var query = CompileQuery<Moon>("false or true");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task Or_WhenBothFalse_NoRecordsAreReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon());
            var query = CompileQuery<Moon>("false or false");

            var filteredResult = await repository.GetAsync(query);

            Assert.Empty(filteredResult.Items);
        }

        [Fact]
        public async Task Or_WithFalseBrackets_NoRecordsAreReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon());
            var query = CompileQuery<Moon>("false or (false and true)");

            var filteredResult = await repository.GetAsync(query);

            Assert.Empty(filteredResult.Items);
        }

        [Fact]
        public async Task Or_WithFalseAndTrueBracket_RecordsAreReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon());
            var query = CompileQuery<Moon>("false or (true and true)");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task And_WithTrueBrackets_RecordsAreReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon());
            var query = CompileQuery<Moon>("true and (true or false)");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task And_WithTrueAndFalseBracket_NoRecordsAreReturned()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon());
            var query = CompileQuery<Moon>("false and (true or false)");

            var filteredResult = await repository.GetAsync(query);

            Assert.Empty(filteredResult.Items);
        }

        [Fact]
        public async Task Adding_ToPropertyToEqualValue_ReturnsValues()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            var moon = await repository.CreateAsync(new Moon());
            var query = CompileQuery<Moon>($"{moon.Id - 1} add 1 eq Id");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task Subtracting_FromPropertyToEqualValue_ReturnsValues()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            var moon = await repository.CreateAsync(new Moon());
            var query = CompileQuery<Moon>($"{moon.Id + 1} sub 1 eq Id");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task Multiplying_PropertyToEqualValue_ReturnsValues()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { AstronomicalUnits = 6 });
            var query = CompileQuery<Planet>("AstronomicalUnits mul 2 eq 12");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task Multiplying_ValuesWithinBrackets_AppliesOrderOfOperationsCorrectly()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            var planet = await repository.CreateAsync(new Planet());
            var query = CompileQuery<Planet>($"(Id add 3) mul 4 eq {(planet.Id + 3) * 4}");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task Dividing_PropertyToEqualValue_ReturnsValues()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { AstronomicalUnits = 6 });
            var query = CompileQuery<Planet>("AstronomicalUnits div 2 eq 3");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task Dividing_ValuesWithinBrackets_AppliesOrderOfOperationsCorrectly()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { AstronomicalUnits = 24 });
            var query = CompileQuery<Planet>("AstronomicalUnits div (2 mul 3) eq 4");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task Moding_ByDivisibleNumber_ReturnsValues()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { AstronomicalUnits = 24 });
            var query = CompileQuery<Planet>("AstronomicalUnits mod 6 eq 0");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task Moding_ByNonDivisibleNumberMatchingRemainder_ReturnsValues()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { AstronomicalUnits = 24 });
            var query = CompileQuery<Planet>("AstronomicalUnits mod 5 eq 4");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task Not_True_ReturnsNoValues()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await repository.CreateAsync(new Planet());
            var query = CompileQuery<Planet>("not true");

            var filteredResult = await repository.GetAsync(query);

            Assert.Empty(filteredResult.Items);
        }

        [Fact]
        public async Task Not_DoubleNegative_ReturnsValues()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await repository.CreateAsync(new Planet());
            var query = CompileQuery<Planet>("not not true");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task Not_WhenComparingEquvilantProperties_ReturnsNoValues()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { SupportsLife = true });
            var query = CompileQuery<Planet>("not SupportsLife");

            var filteredResult = await repository.GetAsync(query);

            Assert.Empty(filteredResult.Items);
        }

        [Fact]
        public async Task Not_WhenComparingCompliment_ReturnsValues()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { SupportsLife = false });
            var query = CompileQuery<Planet>("not SupportsLife");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task NegativeSign_WhenComparingEquivilantNegativeValue_ReturnsValues()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { AstronomicalUnits = -2 });
            var query = CompileQuery<Planet>("AstronomicalUnits eq -2");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task NegativeSign_WhenComparingEquivilantNegativeProperty_ReturnsValues()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { AstronomicalUnits = -2 });
            var query = CompileQuery<Planet>($"2 eq -{nameof(Planet.AstronomicalUnits)}");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task NegativeSign_WhenAddingToProperty_MatchesThatValue()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { AstronomicalUnits = -2 });
            var query = CompileQuery<Planet>("(4 add -AstronomicalUnits) eq 6");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task PositiveSign_WhenAddingToProperty_MatchesThatValue()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { AstronomicalUnits = -2 });
            var query = CompileQuery<Planet>("+AstronomicalUnits eq -2");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task SubstringOf_WhenStringMatches_ReturnsTrue()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { Name = "Terra" });
            var query = CompileQuery<Planet>("substringof('Terra', Name) eq true");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task SubstringOf_WhenStringIsContained_ReturnsTrue()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { Name = "Terra" });
            var query = CompileQuery<Planet>("substringof('Terran', Name) eq true");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task SubstringOf_WhenStringIsNotContained_ReturnsFalse()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { Name = "Terra" });
            var query = CompileQuery<Planet>("substringof('Earth', Name) eq false");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task EndsWith_WhenEndsWith_ReturnsTrue()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { Name = "rth" });
            var query = CompileQuery<Planet>("endswith('Earth', Name) eq true");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task EndsWith_WhenMatches_ReturnsTrue()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { Name = "Earth" });
            var query = CompileQuery<Planet>("endswith('Earth', Name) eq true");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task EndsWith_WhenStartsWith_ReturnsFalse()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { Name = "Earth" });
            var query = CompileQuery<Planet>("endswith('Ear', Name) eq false");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task EndsWith_WhenNoMatch_ReturnsTrue()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { Name = "Earth" });
            var query = CompileQuery<Planet>("endswith('Terra', Name) eq false");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task StartsWith_WhenContained_ReturnsTrue()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { Name = "Terra" });
            var query = CompileQuery<Planet>("startswith('Terran', Name) eq true");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task StartsWith_WhenEquals_ReturnsTrue()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            var planet = await repository.CreateAsync(new Planet { Name = "Terra" });
            var query = CompileQuery<Planet>($"startswith('{planet.Name}', {nameof(Planet.Name)}) eq true");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task StartsWith_WhenNotContains_ReturnsFalse()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { Name = "Terran" });
            var query = CompileQuery<Planet>($"startswith('Terra', {nameof(Planet.Name)}) eq false");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task Length_WhenMatching_ReturnsTrue()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            var planet = await repository.CreateAsync(new Planet { Name = "Terran" });
            var query = CompileQuery<Planet>($"length({nameof(Planet.Name)}) eq {planet.Name.Length}");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task Length_WhenNotMatching_ReturnsFalse()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            var planet = await repository.CreateAsync(new Planet { Name = "Terran" });
            var query = CompileQuery<Planet>($"length({nameof(Planet.Name)}) eq {planet.Name.Length - 1}");

            var filteredResult = await repository.GetAsync(query);

            Assert.Empty(filteredResult.Items);
        }

        [Fact]
        public async Task Replace_ReplacesInstance()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            var planet = await repository.CreateAsync(new Planet { Name = "Terran" });
            var query = CompileQuery<Planet>($"replace({nameof(Planet.Name)}, '{planet.Name}', 'Earth') eq 'Earth'");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task Substring_SkipsThoseCharacters()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            var planet = await repository.CreateAsync(new Planet { Name = "Terran" });
            var query = CompileQuery<Planet>($"substring({nameof(Planet.Name)}, 1) eq '{planet.Name.Substring(1)}'");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task Substring_WithLength_ReturnsThoseCharacters()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            var planet = await repository.CreateAsync(new Planet { Name = "Terran" });
            var query = CompileQuery<Planet>($"substring({nameof(Planet.Name)}, 1, 2) eq '{planet.Name.Substring(1, 2)}'");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task ToLower_ReturnsLoweredCharacters()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { Name = "Terran" });
            var query = CompileQuery<Planet>($"tolower({nameof(Planet.Name)}) eq 'terran'");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task ToUpper_ReturnsCapitilizedCharacters()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { Name = "Terran" });
            var query = CompileQuery<Planet>($"toupper({nameof(Planet.Name)}) eq 'TERRAN'");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task Trimming_RemovesWhitespace()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { Name = " Terran " });
            var query = CompileQuery<Planet>($"trim({nameof(Planet.Name)}) eq 'Terran'");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task Concat_CombinesTwoStrings()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { Name = "Terran" });
            var query = CompileQuery<Planet>($"concat('Terra','n') eq {nameof(Planet.Name)}");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task NestedFunctionCalls_AreParsedInCorrectOrder()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { Name = "abcde" });
            var query = CompileQuery<Planet>($"concat('a', concat(concat('b', concat('c', 'd')), 'e')) eq {nameof(Planet.Name)}");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task Day_ReturnsDay()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            var dateOfDiscovery = new DateTime(2000, 1, 2, 3, 4, 5);
            await repository.CreateAsync(new Planet { DiscoveredOn = dateOfDiscovery });

            var query = CompileQuery<Planet>($"day({nameof(Planet.DiscoveredOn)}) eq {dateOfDiscovery.Day}");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task Hour_ReturnsHour()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            var dateOfDiscovery = new DateTime(2000, 1, 2, 3, 4, 5);
            await repository.CreateAsync(new Planet { DiscoveredOn = dateOfDiscovery });
            var query = CompileQuery<Planet>($"hour({nameof(Planet.DiscoveredOn)}) eq {dateOfDiscovery.Hour}");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task Minute_ReturnsMinute()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            var dateOfDiscovery = new DateTime(2000, 1, 2, 3, 4, 5);
            await repository.CreateAsync(new Planet { DiscoveredOn = dateOfDiscovery });
            var query = CompileQuery<Planet>($"minute({nameof(Planet.DiscoveredOn)}) eq {dateOfDiscovery.Minute}");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task Second_ReturnsSecond()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            var dateOfDiscovery = new DateTime(2000, 1, 2, 3, 4, 5);
            await repository.CreateAsync(new Planet { DiscoveredOn = dateOfDiscovery });
            var query = CompileQuery<Planet>($"second({nameof(Planet.DiscoveredOn)}) eq {dateOfDiscovery.Second}");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task Year_ReturnsYear()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            var dateOfDiscovery = new DateTime(2000, 1, 2, 3, 4, 5);
            await repository.CreateAsync(new Planet { DiscoveredOn = dateOfDiscovery });
            var query = CompileQuery<Planet>($"year({nameof(Planet.DiscoveredOn)}) eq {dateOfDiscovery.Year}");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        private async Task ClearRepositoryAsync<TEntity>(IRepository<TEntity> repository) where TEntity : Entity, IAggregateRoot
        {
            foreach (var clearing in await repository.GetAsync())
            {
                await repository.DeleteAsync(clearing);
            }
        }

        private IQuery<T, T> CompileQuery<T>(string odataQuery)
        {
            var tokens = Tokenizer.Tokenize(odataQuery);
            var expression = Parser.Parse(tokens);
            var csFilter = expression.CsString();
            return new Query<T>().Where(csFilter);
        }
    }
}
