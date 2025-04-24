namespace api.tests;

using System.Threading.Tasks;
using api.Contracts.Responses;
using api.Endpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using NUnit.Framework.Internal;


public class EmployeesApiTest
{

    [SetUp]
    public void SetupClient()
    {

    }

    [Test]
    [TestCaseSource(nameof(TestCasesGetEmployees))]
    public async Task TestGetEmployees(string query, string[] expected)
    {
        var result = await EmployeesEndpoints.HandleGetEmployeesAsync(query);
        Assert.That(result, Is.TypeOf<Ok<IEnumerable<EmployeeResponse>>>());

        //assert data
        // Assert.That(result.Value, Is.Not.Null);
        // Assert.That(expected, Is.EqualTo(result.Value.Select(d => d.Name).ToArray()));

    }

    public static IEnumerable<TestCaseData> TestCasesGetEmployees()
    {
        var testCases = new List<TestCaseData>(){
            new TestCaseData("", new string[]{"Rose", "Almond"}),
            new TestCaseData("Rose", new string[]{"Rose"}),
            new TestCaseData("Sam", new string[]{})
        };
        foreach (var tc in testCases)
        {
            yield return tc;
        }
    }

    // [Test]
    // public async Task TestGetEmployees()
    // {
    //     var result = await EmployeesEndpoints.HandleGetEmployeeAsync();
    //     Assert.That(result, Is.TypeOf<Ok<string>>());
    // }

}
