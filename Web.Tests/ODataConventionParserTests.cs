using FluentNHibernate.Data;
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
    [Collection(nameof(WebCollection))]
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
        public async Task It_will_return_all_entities_when_predicate_is_always_true()
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
        public async Task It_will_return_no_entities_when_predicate_is_always_false()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon());
            var query = CompileQuery<Moon>("1 ne 1");

            var filteredResult = await repository.GetAsync(query);

            Assert.Equal(0, filteredResult.Items.Count());
        }

        [Fact]
        public async Task It_will_return_matching_entities_that_have_matching_properties_when_filtering_using_eq_operator()
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
        public async Task It_will_return_unmatched_entities_that_have_unmatched_properties_when_filtering_using_ne_property()
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
        public async Task It_will_not_match_entity_with_null_property_when_filtering_against_property_with_value()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Moon { Name = null });
            var query = CompileQuery<Moon>("Name ne 'Luna'");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_return_entities_with_larger_property_when_filtering_with_gt_operator()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Moon { Radius = 3 });
            var query = CompileQuery<Moon>("Radius gt 2");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_not_return_entities_when_equal_properties_when_filtering_with_gt_operator()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Moon { Radius = 3 });
            var query = CompileQuery<Moon>("Radius gt 3");

            var filteredResult = await repository.GetAsync(query);

            Assert.Empty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_not_return_entities_with_smaller_properties_when_filtering_with_gt_operator()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Moon { Radius = 3 });
            var query = CompileQuery<Moon>("Radius gt 4");

            var filteredResult = await repository.GetAsync(query);

            Assert.Empty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_return_entities_with_larger_properties_when_filtering_with_ge_operator()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Moon { Radius = 3 });
            var query = CompileQuery<Moon>("Radius ge 2");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_return_entities_with_equal_properties_when_filtering_with_ge_operator()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Moon { Radius = 3 });
            var query = CompileQuery<Moon>("Radius ge 3");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_not_return_entities_with_smaller_properties_when_filtering_with_ge_operator()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Moon { Radius = 3 });
            var query = CompileQuery<Moon>("Radius ge 4");

            var filteredResult = await repository.GetAsync(query);

            Assert.Empty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_return_entities_with_smaller_properties_when_filtering_with_lt_operator()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Moon { Radius = 3 });
            var query = CompileQuery<Moon>("Radius lt 4");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_not_return_entities_with_equal_properties_when_filtering_with_lt_operator()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Moon { Radius = 3 });
            var query = CompileQuery<Moon>("Radius lt 3");

            var filteredResult = await repository.GetAsync(query);

            Assert.Empty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_not_return_entities_with_larger_properties_when_filtering_with_lt_operator()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Moon { Radius = 3 });
            var query = CompileQuery<Moon>("Radius lt 2");

            var filteredResult = await repository.GetAsync(query);

            Assert.Empty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_return_entities_with_smaller_properties_when_filtering_with_le_operator()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Moon { Radius = 3 });
            var query = CompileQuery<Moon>("Radius le 4");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_return_entities_with_equal_properties_when_filtering_with_le_operator()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Moon { Radius = 3 });
            var query = CompileQuery<Moon>("Radius le 3");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_not_return_entities_with_larger_properties_when_filtering_with_le_operator()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Moon { Radius = 3 });
            var query = CompileQuery<Moon>("Radius le 2");

            var filteredResult = await repository.GetAsync(query);

            Assert.Empty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_not_return_entities_when_left_is_false_and_right_is_true()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon());
            var query = CompileQuery<Moon>("false and true");

            var filteredResult = await repository.GetAsync(query);

            Assert.Empty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_return_entities_when_both_left_and_right_are_true()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon());
            var query = CompileQuery<Moon>("true and true");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_return_entities_when_both_left_and_right_are_false()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon());
            var query = CompileQuery<Moon>("false and false");

            var filteredResult = await repository.GetAsync(query);

            Assert.Empty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_return_entities_when_true_or_true()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon());
            var query = CompileQuery<Moon>("true or true");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_return_entities_when_true_or_false()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon());
            var query = CompileQuery<Moon>("true or false");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_return_entities_when_false_or_true()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon());
            var query = CompileQuery<Moon>("false or true");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_not_return_entities_when_false_or_false()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon());
            var query = CompileQuery<Moon>("false or false");

            var filteredResult = await repository.GetAsync(query);

            Assert.Empty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_return_entities_when_false_and_true_second_expression_in_brackets_is_false_or_true()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon());
            var query = CompileQuery<Moon>("false or (false and true)");

            var filteredResult = await repository.GetAsync(query);

            Assert.Empty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_return_entities_when_false_and_true_second_expression_in_brackets_is_true_or_false()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon());
            var query = CompileQuery<Moon>("false or (true and true)");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_return_entities_when_left_is_true_and_right_expression_in_brackets_is_true_or_false()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon());
            var query = CompileQuery<Moon>("true and (true or false)");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_return_entities_when_left_is_false_and_right_expression_in_brackets_is_true_or_false()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await repository.CreateAsync(new Moon());
            var query = CompileQuery<Moon>("false and (true or false)");

            var filteredResult = await repository.GetAsync(query);

            Assert.Empty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_return_matching_entities_when_add_operation_used_matches_difference()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            var moon = await repository.CreateAsync(new Moon());
            var query = CompileQuery<Moon>($"{moon.Id - 1} add 1 eq Id");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_return_matching_entities_when_sub_operation_used_matches_difference()
        {
            var repository = Services.GetRequiredService<IRepository<Moon>>();
            await ClearRepositoryAsync(repository);
            var moon = await repository.CreateAsync(new Moon());
            var query = CompileQuery<Moon>($"{moon.Id + 1} sub 1 eq Id");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_apply_multiplication_when_mul_operator_is_used()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { AstronomicalUnits = 6 });
            var query = CompileQuery<Planet>("AstronomicalUnits mul 2 eq 12");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_add_operation_before_multipling_when_brackets_are_used()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            var planet = await repository.CreateAsync(new Planet());
            var query = CompileQuery<Planet>($"(Id add 3) mul 4 eq {(planet.Id + 3) * 4}");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_divide_properties_before_comparing_when_the_div_operator_is_used()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { AstronomicalUnits = 6 });
            var query = CompileQuery<Planet>("AstronomicalUnits div 2 eq 3");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_divide_by_the_product_when_brackets_are_used()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { AstronomicalUnits = 24 });
            var query = CompileQuery<Planet>("AstronomicalUnits div (2 mul 3) eq 4");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_return_zero_when_moding_by_divisible_number()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { AstronomicalUnits = 24 });
            var query = CompileQuery<Planet>("AstronomicalUnits mod 6 eq 0");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_return_remainder_when_modding_by_non_divisible_number()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { AstronomicalUnits = 24 });
            var query = CompileQuery<Planet>("AstronomicalUnits mod 5 eq 4");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_not_return_entities_that_match_when_using_the_not_operator()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await repository.CreateAsync(new Planet());
            var query = CompileQuery<Planet>("not true");

            var filteredResult = await repository.GetAsync(query);

            Assert.Empty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_return_matching_entities_when_using_a_double_not_operator()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await repository.CreateAsync(new Planet());
            var query = CompileQuery<Planet>("not not true");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_not_return_entities_whos_boolean_properties_are_true_when_using_the_not_operator()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { SupportsLife = true });
            var query = CompileQuery<Planet>("not SupportsLife");

            var filteredResult = await repository.GetAsync(query);

            Assert.Empty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_return_entities_whos_boolean_properties_are_false_when_using_the_not_operator()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { SupportsLife = false });
            var query = CompileQuery<Planet>("not SupportsLife");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_return_entities_whos_value_is_negative_when_compared_against_a_negative_value()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { AstronomicalUnits = -2 });
            var query = CompileQuery<Planet>("AstronomicalUnits eq -2");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_flip_the_sign_of_the_property_when_applying_the_negative_operator()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { AstronomicalUnits = -2 });
            var query = CompileQuery<Planet>($"2 eq -{nameof(Planet.AstronomicalUnits)}");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_flip_the_sign_of_a_property_before_arithmetic_operations_begin()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { AstronomicalUnits = -2 });
            var query = CompileQuery<Planet>("(4 add -AstronomicalUnits) eq 6");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_allow_a_value_to_have_the_positive_sign_be_applied()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { AstronomicalUnits = -2 });
            var query = CompileQuery<Planet>("+AstronomicalUnits eq -2");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_return_entities_with_matching_values_when_filtering_with_the_substring_method()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { Name = "Terra" });
            var query = CompileQuery<Planet>("substringof('Terra', Name) eq true");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_return_entities_if_they_are_substrins_when_filtering_with_the_substring_method()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { Name = "Terra" });
            var query = CompileQuery<Planet>("substringof('Terran', Name) eq true");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_not_return_entities_if_their_value_isnt_a_substring_when_filtering_with_the_substring_method()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { Name = "Terra" });
            var query = CompileQuery<Planet>("substringof('Earth', Name) eq false");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_return_entities_who_end_with_a_value_when_filtering_using_the_endswith_method()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { Name = "rth" });
            var query = CompileQuery<Planet>("endswith('Earth', Name) eq true");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_return_entities_whos_value_match_when_filtering_with_the_endswith_method()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { Name = "Earth" });
            var query = CompileQuery<Planet>("endswith('Earth', Name) eq true");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_not_return_entities_whos_value_doesnt_end_when_filtering_when_endswith_method()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { Name = "Earth" });
            var query = CompileQuery<Planet>("endswith('Ear', Name) eq false");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_not_return_entities_whos_value_is_different_when_filtering_with_endswith_method()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { Name = "Earth" });
            var query = CompileQuery<Planet>("endswith('Terra', Name) eq false");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_return_entities_with_values_starting_with_argument_when_filtering_with_startswith_method()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { Name = "Terra" });
            var query = CompileQuery<Planet>("startswith('Terran', Name) eq true");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_return_entities_with_values_equal_the_argument_when_filtering_with_startswith_method()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            var planet = await repository.CreateAsync(new Planet { Name = "Terra" });
            var query = CompileQuery<Planet>($"startswith('{planet.Name}', {nameof(Planet.Name)}) eq true");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_not_return_entities_whos_values_are_supersets_of_the_argument_when_filtering_with_startswith_method()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { Name = "Terran" });
            var query = CompileQuery<Planet>($"startswith('Terra', {nameof(Planet.Name)}) eq false");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_return_entities_whos_properties_are_of_equal_length_when_filtering_using_length_method()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            var planet = await repository.CreateAsync(new Planet { Name = "Terran" });
            var query = CompileQuery<Planet>($"length({nameof(Planet.Name)}) eq {planet.Name.Length}");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_not_return_entities_whos_properties_are_of_different_length_when_filtering_using_length_method()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            var planet = await repository.CreateAsync(new Planet { Name = "Terran" });
            var query = CompileQuery<Planet>($"length({nameof(Planet.Name)}) eq {planet.Name.Length - 1}");

            var filteredResult = await repository.GetAsync(query);

            Assert.Empty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_replace_values_before_comparing_when_filtering_with_replace_method()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            var planet = await repository.CreateAsync(new Planet { Name = "Terran" });
            var query = CompileQuery<Planet>($"replace({nameof(Planet.Name)}, '{planet.Name}', 'Earth') eq 'Earth'");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_skip_characters_equal_to_argument_when_using_the_substring_method()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            var planet = await repository.CreateAsync(new Planet { Name = "Terran" });
            var query = CompileQuery<Planet>($"substring({nameof(Planet.Name)}, 1) eq '{planet.Name.Substring(1)}'");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_return_a_string_of_length_equal_to_third_argument_when_using_the_substring_method()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            var planet = await repository.CreateAsync(new Planet { Name = "Terran" });
            var query = CompileQuery<Planet>($"substring({nameof(Planet.Name)}, 1, 2) eq '{planet.Name.Substring(1, 2)}'");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_convert_a_property_value_to_lowercase_when_filtering_using_the_tolower_method()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { Name = "Terran" });
            var query = CompileQuery<Planet>($"tolower({nameof(Planet.Name)}) eq 'terran'");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_convet_a_property_to_uppercase_when_filtering_using_the_toupper_method()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { Name = "Terran" });
            var query = CompileQuery<Planet>($"toupper({nameof(Planet.Name)}) eq 'TERRAN'");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_remove_whitespace_from_the_ends_of_a_property_when_filtering_using_the_trim_method()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { Name = " Terran " });
            var query = CompileQuery<Planet>($"trim({nameof(Planet.Name)}) eq 'Terran'");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_allow_two_string_values_to_be_combined_when_using_the_concat_method()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { Name = "Terran" });
            var query = CompileQuery<Planet>($"concat('Terra','n') eq {nameof(Planet.Name)}");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_call_functions_in_the_correct_order_when_they_are_nested()
        {
            var repository = Services.GetRequiredService<IRepository<Planet>>();
            await ClearRepositoryAsync(repository);
            await repository.CreateAsync(new Planet { Name = "abcde" });
            var query = CompileQuery<Planet>($"concat('a', concat(concat('b', concat('c', 'd')), 'e')) eq {nameof(Planet.Name)}");

            var filteredResult = await repository.GetAsync(query);

            Assert.NotEmpty(filteredResult.Items);
        }

        [Fact]
        public async Task It_will_return_a_dates_day_value_when_the_day_method_is_used()
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
        public async Task It_will_return_a_dates_hour_value_when_the_hour_method_is_used()
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
        public async Task It_will_return_a_dates_minute_value_when_the_minute_method_is_used()
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
        public async Task It_will_return_a_dates_second_value_when_the_second_method_is_used()
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
        public async Task It_will_return_a_dates_year_value_when_the_year_method_is_used()
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
