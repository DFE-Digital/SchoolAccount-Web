using GovUK.Dfe.AcademiesApi.Client.Contracts;
using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;
using Shouldly;
using static SchoolAccount.Infrastructure.Clients.Academies.GetAcademies.GetAcademiesMapper;

namespace SchoolAccount.Infrastructure.UnitTests.Clients.Academies.GetAcademies;

public class GetAcademiesMapperTests
{
    [Fact]
    public void A_trust_from_the_academies_api_is_mapped()
    {
        // Arrange
        var trustDto = new TrustDto
        {
            Ukprn = "10012345",
            Name = "Test Trust",
            GroupUid = "TR00123",
            Type = new NameAndCodeDto { Name = "Multi-academy trust", Code = "MAT" },
        };

        // Act
        var result = ToTrustResponse(trustDto, []);

        // Assert
        result.ShouldSatisfyAllConditions(
            trust => trust.Ukprn.ShouldBe("10012345"),
            trust => trust.Name.ShouldBe("Test Trust"),
            trust => trust.GroupUid.ShouldBe("TR00123"),
            trust => trust.Type.ShouldNotBeNull(),
            trust => trust.Type!.Name.ShouldBe("Multi-academy trust"),
            trust => trust.Type!.Code.ShouldBe("MAT")
        );
    }

    [Fact]
    public void A_trust_without_a_type_is_mapped_with_a_null_type()
    {
        // Arrange
        var trustDto = new TrustDto
        {
            Ukprn = "10012345",
            Name = "Test Trust",
            Type = null,
        };

        // Act
        var result = ToTrustResponse(trustDto, []);

        // Assert
        result.Type.ShouldBeNull();
    }

    [Fact]
    public void A_trust_without_a_name_throws()
    {
        // Arrange
        var trustDto = new TrustDto { Ukprn = "10012345", Name = null };

        // Act & Assert
        Should.Throw<ArgumentException>(() => ToTrustResponse(trustDto, []));
    }

    [Fact]
    public void A_trust_without_a_ukprn_throws()
    {
        // Arrange
        var trustDto = new TrustDto { Ukprn = null, Name = "Test Trust" };

        // Act & Assert
        Should.Throw<ArgumentException>(() => ToTrustResponse(trustDto, []));
    }

    [Fact]
    public void Every_establishment_is_mapped_onto_the_trust()
    {
        // Arrange
        var trustDto = new TrustDto { Ukprn = "10012345", Name = "Test Trust" };
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
        var result = ToTrustResponse(trustDto, establishments);

        // Assert
        result.Establishments.ShouldNotBeNull();
        result.ShouldSatisfyAllConditions(
            trust => trust.Establishments.Count.ShouldBe(2),
            trust => trust.Establishments[0].EstablishmentName.ShouldBe("First School"),
            trust => trust.Establishments[1].EstablishmentName.ShouldBe("Second School")
        );
    }

    [Fact]
    public void A_trust_without_establishments_is_mapped_with_no_establishments()
    {
        // Arrange
        var trustDto = new TrustDto { Ukprn = "10012345", Name = "Test Trust" };

        // Act
        var result = ToTrustResponse(trustDto, []);

        // Assert
        result.Establishments.ShouldBeEmpty();
    }

    [Fact]
    public void An_establishment_is_mapped_to_an_establishment_response()
    {
        // Arrange
        var establishmentDto = new EstablishmentDto
        {
            Ukprn = "10011111",
            Name = "Test School",
            Urn = "100001",
            EstablishmentNumber = "1234",
            LocalAuthorityCode = "201",
            LocalAuthorityName = "Camden",
        };

        // Act
        var result = ToEstablishmentResponse(establishmentDto);

        // Assert
        result.ShouldSatisfyAllConditions(
            establishment => establishment.Ukprn.ShouldBe("10011111"),
            establishment => establishment.EstablishmentName.ShouldBe("Test School"),
            establishment => establishment.Urn.ShouldBe("100001"),
            establishment => establishment.EstablishmentNumber.ShouldBe("1234"),
            establishment => establishment.LocalAuthorityCode.ShouldBe("201"),
            establishment => establishment.LocalAuthorityName.ShouldBe("Camden")
        );
    }

    [Fact]
    public void An_establishment_without_a_ukprn_throws()
    {
        // Arrange
        var establishmentDto = new EstablishmentDto { Ukprn = null, Name = "Test Trust" };

        // Act & Assert
        Should.Throw<ArgumentException>(() => ToEstablishmentResponse(establishmentDto));
    }

    [Fact]
    public void An_establishment_without_a_name_throws()
    {
        // Arrange
        var establishmentDto = new EstablishmentDto { Ukprn = "Test Trust", Name = null };

        // Act & Assert
        Should.Throw<ArgumentException>(() => ToEstablishmentResponse(establishmentDto));
    }

    [Fact]
    public void An_establishment_with_null_fields_is_mapped_with_nulls()
    {
        // Arrange
        var establishmentDto = new EstablishmentDto
        {
            Ukprn = "10011111",
            Name = "Test School",
            Urn = null,
            EstablishmentNumber = null,
            LocalAuthorityCode = null,
            LocalAuthorityName = null,
        };

        // Act
        var result = ToEstablishmentResponse(establishmentDto);

        // Assert
        result.ShouldSatisfyAllConditions(
            establishment => establishment.Ukprn.ShouldBe("10011111"),
            establishment => establishment.EstablishmentName.ShouldBe("Test School"),
            establishment => establishment.Urn.ShouldBeNull(),
            establishment => establishment.EstablishmentNumber.ShouldBeNull(),
            establishment => establishment.LocalAuthorityCode.ShouldBeNull(),
            establishment => establishment.LocalAuthorityName.ShouldBeNull()
        );
    }

    [Fact]
    public void The_establishment_name_and_code_types_are_mapped()
    {
        // Arrange
        var establishmentDto = new EstablishmentDto
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
        var result = ToEstablishmentResponse(establishmentDto);

        // Assert
        result.ShouldSatisfyAllConditions(
            establishment => establishment.EstablishmentType!.Name.ShouldBe("Academy"),
            establishment => establishment.EstablishmentType!.Code.ShouldBe("AC"),
            establishment =>
                establishment.EstablishmentGroupType!.Name.ShouldBe("Academy sponsor led"),
            establishment => establishment.EstablishmentGroupType!.Code.ShouldBe("ASL"),
            establishment => establishment.PhaseOfEducation!.Name.ShouldBe("Primary"),
            establishment => establishment.PhaseOfEducation!.Code.ShouldBe("PR")
        );
    }

    [Fact]
    public void An_establishment_without_name_and_code_types_is_mapped_with_nulls()
    {
        // Arrange
        var establishmentDto = new EstablishmentDto
        {
            Ukprn = "10011111",
            Name = "Test School",
            EstablishmentType = null,
            EstablishmentGroupType = null,
            PhaseOfEducation = null,
        };

        // Act
        var result = ToEstablishmentResponse(establishmentDto);

        // Assert
        result.EstablishmentType.ShouldBeNull();
        result.EstablishmentGroupType.ShouldBeNull();
        result.PhaseOfEducation.ShouldBeNull();
    }

    [Fact]
    public void A_name_and_code_with_null_values_returns_null()
    {
        // Arrange
        var trustDto = new TrustDto
        {
            Ukprn = "10012345",
            Name = "Test Trust",
            Type = new NameAndCodeDto { Name = null, Code = null },
        };

        // Act
        var result = ToTrustResponse(trustDto, []);

        // Assert
        result.Type.ShouldBeNull();
    }

    [Fact]
    public void A_null_name_for_a_name_and_code_returns_null()
    {
        // Arrange
        var trustDto = new TrustDto
        {
            Ukprn = "10012345",
            Name = "Test Trust",
            Type = new NameAndCodeDto { Name = null, Code = "MAT" },
        };

        // Act
        var result = ToTrustResponse(trustDto, []);

        // Assert
        result.Type.ShouldBeNull();
    }

    [Fact]
    public void A_null_code_for_a_name_and_code_returns_null()
    {
        // Arrange
        var trustDto = new TrustDto
        {
            Ukprn = "10012345",
            Name = "Test Trust",
            Type = new NameAndCodeDto { Name = "Primary School", Code = null },
        };

        // Act
        var result = ToTrustResponse(trustDto, []);

        // Assert
        result.Type.ShouldBeNull();
    }
}
