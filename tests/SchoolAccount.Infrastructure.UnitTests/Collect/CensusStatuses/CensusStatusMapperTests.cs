using SchoolAccount.Infrastructure.Clients.Collect.CensusStatuses;
using SchoolAccount.SharedKernel.Authentication;
using Shouldly;

namespace SchoolAccount.Infrastructure.UnitTests.Collect.CensusStatuses;

public class CensusStatusMapperTests
{
    [Fact]
    public void An_organisation_is_mapped_to_a_census_status()
    {
        // Arrange
        var organisation = new OrganisationResponse { Id = "123", Interesting = true };

        // Act
        var censusStatus = CensusStatusMapper.ToResponse(organisation);

        // Assert
        censusStatus.Id.ShouldBe("123");
        censusStatus.Interesting.ShouldBeTrue();
    }

    [Fact]
    public void Every_action_is_mapped_with_its_status()
    {
        // Arrange
        var octoberThird2026 = new DateTime(2026, 10, 03, 2, 0, 0, DateTimeKind.Utc);
        var octoberFirst2026 = new DateTime(2026, 10, 01, 2, 0, 0, DateTimeKind.Utc);
        var organisation = new OrganisationResponse
        {
            Id = "123",
            Actions =
            [
                Action("Autumn School Census", "Unavailable"),
                Action("Autumn School Census", "Not Started", 1, 2, octoberThird2026),
                Action("Spring School Census", "Complete", 3, 4, octoberFirst2026),
            ],
        };

        // Act
        var censusStatus = CensusStatusMapper.ToResponse(organisation);

        // Assert
        censusStatus.Actions.Count.ShouldBe(3);
        censusStatus.Actions[0].Name.ShouldBe("Autumn School Census");
        censusStatus.Actions[0].Status.Name.ShouldBe("Unavailable");
        censusStatus.Actions[0].Errors.ShouldBeNull();
        censusStatus.Actions[0].Queries.ShouldBeNull();
        censusStatus.Actions[0].UpdatedAt.ShouldBeNull();
        censusStatus.Actions[1].Name.ShouldBe("Autumn School Census");
        censusStatus.Actions[1].Status.Name.ShouldBe("Not Started");
        censusStatus.Actions[1].Errors.ShouldBe(1);
        censusStatus.Actions[1].Queries.ShouldBe(2);
        censusStatus.Actions[1].UpdatedAt.ShouldBe(octoberThird2026);
        censusStatus.Actions[2].Name.ShouldBe("Spring School Census");
        censusStatus.Actions[2].Status.Name.ShouldBe("Complete");
        censusStatus.Actions[2].Errors.ShouldBe(3);
        censusStatus.Actions[2].Queries.ShouldBe(4);
        censusStatus.Actions[2].UpdatedAt.ShouldBe(octoberFirst2026);
    }

    [Fact]
    public void An_organisation_without_actions_is_mapped_with_no_actions()
    {
        // Arrange
        var organisation = new OrganisationResponse { Id = "123" };

        // Act
        var censusStatus = CensusStatusMapper.ToResponse(organisation);

        // Assert
        censusStatus.Actions.ShouldBeEmpty();
    }

    [Fact]
    public void A_lookup_is_mapped_to_an_api_request()
    {
        // Arrange
        var organisations = new List<Organisation>
        {
            new() { Id = "org-id", Name = "Test School" },
        };

        // Act
        var request = CensusStatusMapper.ToApiRequest(
            "test-user-id",
            "test-user@example.com",
            organisations
        );

        // Assert
        request.Id.ShouldBe("test-user-id");
        request.Organisations.ShouldBe(organisations);
    }

    [Fact]
    public void The_email_address_is_mapped_onto_the_apis_email_field()
    {
        // Act
        var request = CensusStatusMapper.ToApiRequest("id", "test-user@example.com", []);

        // Assert
        request.Email.ShouldBe("test-user@example.com");
    }

    private static ActionApiResponse Action(
        string name,
        string status,
        int? errors = null,
        int? queries = null,
        DateTime? updatedAt = null
    ) =>
        new()
        {
            Name = name,
            Status = new StatusApiResponse { Name = status },
            Errors = errors,
            Queries = queries,
            UpdatedAt = updatedAt,
        };
}
