using GovUK.Dfe.AcademiesApi.Client.Contracts;
using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;
using Shouldly;
using static SchoolAccount.Infrastructure.Clients.Academies.GetAcademies.GetAcademiesMapper;

namespace SchoolAccount.Infrastructure.UnitTests.Academies.GetAcademies;

public class GetAcademiesMapperTests
{
    [Fact]
    public void A_trust_from_the_academies_api_is_mapped()
    {
        // Arrange
        var trust = new TrustDto
        {
            Ukprn = "10012345",
            Name = "Test Trust",
            GroupUid = "TR00123",
            Type = new NameAndCodeDto { Name = "Multi-academy trust", Code = "MAT" },
        };

        // Act
        var result = ToTrustResponse(trust, []);

        // Assert
        result.Ukprn.ShouldBe("10012345");
        result.Name.ShouldBe("Test Trust");
        result.GroupUid.ShouldBe("TR00123");
        result.Type.ShouldNotBeNull();
        result.Type.Name.ShouldBe("Multi-academy trust");
        result.Type.Code.ShouldBe("MAT");
    }

    [Fact]
    public void A_trust_without_a_type_is_mapped_with_a_null_type()
    {
        // Arrange
        var trust = new TrustDto
        {
            Ukprn = "10012345",
            Name = "Test Trust",
            Type = null,
        };

        // Act
        var result = ToTrustResponse(trust, []);

        // Assert
        result.Type.ShouldBeNull();
    }

    [Fact]
    public void A_trust_without_a_name_throws()
    {
        // Arrange
        var trust = new TrustDto { Ukprn = "10012345", Name = null };

        // Act & Assert
        Should.Throw<ArgumentException>(() => ToTrustResponse(trust, []));
    }

    [Fact]
    public void A_trust_without_a_ukprn_throws()
    {
        // Arrange
        var trust = new TrustDto { Ukprn = null, Name = "Test Trust" };

        // Act & Assert
        Should.Throw<ArgumentException>(() => ToTrustResponse(trust, []));
    }

    [Fact]
    public void Every_establishment_is_mapped_onto_the_trust()
    {
        // Arrange
        var trust = new TrustDto { Ukprn = "10012345", Name = "Test Trust" };
        var establishments = new List<GetAcademyEstablishmentResponse>
        {
            ToEstablishmentResponse(
                new EstablishmentDto { Ukprn = "10011111", Name = "First School" }
            ),
            ToEstablishmentResponse(
                new EstablishmentDto { Ukprn = "10022222", Name = "Second School" }
            ),
        };

        // Act
        var result = ToTrustResponse(trust, establishments);

        // Assert
        result.Establishments.ShouldNotBeNull();
        result.Establishments.Count.ShouldBe(2);
        result.Establishments[0].EstablishmentName.ShouldBe("First School");
        result.Establishments[1].EstablishmentName.ShouldBe("Second School");
    }

    [Fact]
    public void A_trust_without_establishments_is_mapped_with_no_establishments()
    {
        // Arrange
        var trust = new TrustDto { Ukprn = "10012345", Name = "Test Trust" };

        // Act
        var result = ToTrustResponse(trust, []);

        // Assert
        result.Establishments.ShouldBeEmpty();
    }

    [Fact]
    public void An_establishment_is_mapped_to_an_establishment_response()
    {
        // Arrange
        var establishment = new EstablishmentDto
        {
            Ukprn = "10011111",
            Name = "Test School",
            Urn = "100001",
            EstablishmentNumber = "1234",
            LocalAuthorityCode = "201",
            LocalAuthorityName = "Camden",
        };

        // Act
        var result = ToEstablishmentResponse(establishment);

        // Assert
        result.Ukprn.ShouldBe("10011111");
        result.EstablishmentName.ShouldBe("Test School");
        result.Urn.ShouldBe("100001");
        result.EstablishmentNumber.ShouldBe("1234");
        result.LocalAuthorityCode.ShouldBe("201");
        result.LocalAuthorityName.ShouldBe("Camden");
    }

    [Fact]
    public void An_establishment_without_a_ukprn_throws()
    {
        // Arrange
        var establishment = new EstablishmentDto { Ukprn = null, Name = "Test Trust" };

        // Act & Assert
        Should.Throw<ArgumentException>(() => ToEstablishmentResponse(establishment));
    }

    [Fact]
    public void An_establishment_without_a_name_throws()
    {
        // Arrange
        var establishment = new EstablishmentDto { Ukprn = "Test Trust", Name = null };

        // Act & Assert
        Should.Throw<ArgumentException>(() => ToEstablishmentResponse(establishment));
    }

    [Fact]
    public void An_establishment_with_null_fields_is_mapped_with_nulls()
    {
        // Arrange
        var establishment = new EstablishmentDto
        {
            Ukprn = "10011111",
            Name = "Test School",
            Urn = null,
            EstablishmentNumber = null,
            LocalAuthorityCode = null,
            LocalAuthorityName = null,
        };

        // Act
        var result = ToEstablishmentResponse(establishment);

        // Assert
        result.Ukprn.ShouldBe("10011111");
        result.EstablishmentName.ShouldBe("Test School");
        result.Urn.ShouldBeNull();
        result.EstablishmentNumber.ShouldBeNull();
        result.LocalAuthorityCode.ShouldBeNull();
        result.LocalAuthorityName.ShouldBeNull();
    }

    [Fact]
    public void The_establishment_name_and_code_types_are_mapped()
    {
        // Arrange
        var establishment = new EstablishmentDto
        {
            Ukprn = "10011111",
            Name = "Test School",
            EstablishmentType = new NameAndCodeDto { Name = "Academy", Code = "AC" },
            EstablishmentGroupType = new NameAndCodeDto
            {
                Name = "Academy sponsor led",
                Code = "ASL",
            },
            PhaseOfEducation = new NameAndCodeDto { Name = "Primary", Code = "PR" },
        };

        // Act
        var result = ToEstablishmentResponse(establishment);

        // Assert
        result.EstablishmentType!.Name.ShouldBe("Academy");
        result.EstablishmentType.Code.ShouldBe("AC");
        result.EstablishmentGroupType!.Name.ShouldBe("Academy sponsor led");
        result.EstablishmentGroupType.Code.ShouldBe("ASL");
        result.PhaseOfEducation!.Name.ShouldBe("Primary");
        result.PhaseOfEducation.Code.ShouldBe("PR");
    }

    [Fact]
    public void An_establishment_without_name_and_code_types_is_mapped_with_nulls()
    {
        // Arrange
        var establishment = new EstablishmentDto
        {
            Ukprn = "10011111",
            Name = "Test School",
            EstablishmentType = null,
            EstablishmentGroupType = null,
            PhaseOfEducation = null,
        };

        // Act
        var result = ToEstablishmentResponse(establishment);

        // Assert
        result.EstablishmentType.ShouldBeNull();
        result.EstablishmentGroupType.ShouldBeNull();
        result.PhaseOfEducation.ShouldBeNull();
    }

    [Fact]
    public void A_name_and_code_with_null_values_returns_null()
    {
        // Arrange
        var trust = new TrustDto
        {
            Ukprn = "10012345",
            Name = "Test Trust",
            Type = new NameAndCodeDto { Name = null, Code = null },
        };

        // Act
        var result = ToTrustResponse(trust, []);

        // Assert
        result.Type.ShouldBeNull();
    }

    [Fact]
    public void A_null_name_for_a_name_and_code_returns_null()
    {
        // Arrange
        var trust = new TrustDto
        {
            Ukprn = "10012345",
            Name = "Test Trust",
            Type = new NameAndCodeDto { Name = null, Code = "MAT" },
        };

        // Act
        var result = ToTrustResponse(trust, []);

        // Assert
        result.Type.ShouldBeNull();
    }

    [Fact]
    public void A_null_code_for_a_name_and_code_returns_null()
    {
        // Arrange
        var trust = new TrustDto
        {
            Ukprn = "10012345",
            Name = "Test Trust",
            Type = new NameAndCodeDto { Name = "Primary School", Code = null },
        };

        // Act
        var result = ToTrustResponse(trust, []);

        // Assert
        result.Type.ShouldBeNull();
    }
}
