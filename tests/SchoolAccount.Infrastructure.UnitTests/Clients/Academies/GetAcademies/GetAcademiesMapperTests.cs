using GovUK.Dfe.AcademiesApi.Client.Contracts;
using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;
using Shouldly;
using static SchoolAccount.Infrastructure.Clients.Academies.GetAcademies.GetAcademiesMapper;
using static SchoolAccount.Infrastructure.UnitTests.Common.Builders.Clients.Academies.GetAcademies.EstablishmentDtoBuilder;
using static SchoolAccount.Infrastructure.UnitTests.Common.Builders.Clients.Academies.GetAcademies.TrustDtoBuilder;

namespace SchoolAccount.Infrastructure.UnitTests.Clients.Academies.GetAcademies;

public class GetAcademiesMapperTests
{
    [Fact]
    public void A_trust_from_the_academies_api_is_mapped()
    {
        // Arrange
        var trustDto = ATrustDto()
            .WithUkprn("10012345")
            .WithName("Test Trust")
            .WithGroupUid("TR00123")
            .WithType(new NameAndCodeDto { Name = "Multi-academy trust", Code = "MAT" })
            .Build();

        // Act
        var trustResponse = ToTrustResponse(trustDto, []);

        // Assert
        var trustType = trustResponse.Type.ShouldNotBeNull();

        trustResponse.ShouldSatisfyAllConditions(
            trust => trust.Ukprn.ShouldBe("10012345"),
            trust => trust.Name.ShouldBe("Test Trust"),
            trust => trust.GroupUid.ShouldBe("TR00123")
        );

        trustType.ShouldSatisfyAllConditions(
            type => type.Name.ShouldBe("Multi-academy trust"),
            type => type.Code.ShouldBe("MAT")
        );
    }

    [Fact]
    public void A_trust_without_a_type_is_mapped_with_a_null_type()
    {
        // Arrange
        var trustDto = ATrustDto().WithType(null).Build();

        // Act
        var trustResponse = ToTrustResponse(trustDto, []);

        // Assert
        trustResponse.Type.ShouldBeNull();
    }

    [Fact]
    public void A_trust_without_a_name_throws()
    {
        // Arrange
        var trustDto = ATrustDto().WithName(null).Build();

        // Act & Assert
        Should.Throw<ArgumentException>(() => ToTrustResponse(trustDto, []));
    }

    [Fact]
    public void A_trust_without_a_ukprn_throws()
    {
        // Arrange
        var trustDto = ATrustDto().WithUkprn(null).Build();

        // Act & Assert
        Should.Throw<ArgumentException>(() => ToTrustResponse(trustDto, []));
    }

    [Fact]
    public void Every_establishment_is_mapped_onto_the_trust()
    {
        // Arrange
        var trustDto = ATrustDto().Build();
        var establishments = new List<GetAcademyEstablishmentResponse>
        {
            ToEstablishmentResponse(
                AnEstablishmentDto().WithUkprn("10011111").WithName("First School").Build()
            ),
            ToEstablishmentResponse(
                AnEstablishmentDto().WithUkprn("10022222").WithName("Second School").Build()
            ),
        };

        // Act
        var trustResponse = ToTrustResponse(trustDto, establishments);

        // Assert
        trustResponse.Establishments.Count.ShouldBe(2);
        trustResponse.Establishments.ShouldSatisfyAllConditions(
            establishment => establishment[0].EstablishmentName.ShouldBe("First School"),
            establishment => establishment[0].Ukprn.ShouldBe("10011111"),
            establishment => establishment[1].EstablishmentName.ShouldBe("Second School"),
            establishment => establishment[1].Ukprn.ShouldBe("10022222")
        );
    }

    [Fact]
    public void A_trust_without_establishments_is_mapped_with_no_establishments()
    {
        // Arrange
        var trustDto = ATrustDto().Build();

        // Act
        var trustResponse = ToTrustResponse(trustDto, []);

        // Assert
        trustResponse.Establishments.ShouldBeEmpty();
    }

    [Fact]
    public void An_establishment_is_mapped_to_an_establishment_response()
    {
        // Arrange
        var establishmentDto = AnEstablishmentDto()
            .WithUkprn("10011111")
            .WithName("Test School")
            .WithUrn("100001")
            .WithEstablishmentNumber("1234")
            .WithLocalAuthorityCode("201")
            .WithLocalAuthorityName("Camden")
            .Build();

        // Act
        var establishmentResponse = ToEstablishmentResponse(establishmentDto);

        // Assert
        establishmentResponse.ShouldSatisfyAllConditions(
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
        var establishmentDto = AnEstablishmentDto().WithUkprn(null).Build();

        // Act & Assert
        Should.Throw<ArgumentException>(() => ToEstablishmentResponse(establishmentDto));
    }

    [Fact]
    public void An_establishment_without_a_name_throws()
    {
        // Arrange
        var establishmentDto = AnEstablishmentDto().WithName(null).Build();

        // Act & Assert
        Should.Throw<ArgumentException>(() => ToEstablishmentResponse(establishmentDto));
    }

    [Fact]
    public void An_establishment_with_null_fields_is_mapped_with_nulls()
    {
        // Arrange
        var establishmentDto = AnEstablishmentDto()
            .WithUrn(null)
            .WithEstablishmentNumber(null)
            .WithLocalAuthorityCode(null)
            .WithLocalAuthorityName(null)
            .Build();

        // Act
        var establishmentResponse = ToEstablishmentResponse(establishmentDto);

        // Assert
        establishmentResponse.ShouldSatisfyAllConditions(
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
        var establishmentDto = AnEstablishmentDto()
            .WithEstablishmentType(new NameAndCodeDto { Name = "Academy", Code = "AC" })
            .WithEstablishmentGroupType(
                new NameAndCodeDto { Name = "Academy sponsor led", Code = "ASL" }
            )
            .WithPhaseOfEducation(new NameAndCodeDto { Name = "Primary", Code = "PR" })
            .Build();

        // Act
        var establishmentResponse = ToEstablishmentResponse(establishmentDto);

        // Assert
        var establishmentType = establishmentResponse.EstablishmentType.ShouldNotBeNull();
        var establishmentGroupType = establishmentResponse.EstablishmentGroupType.ShouldNotBeNull();
        var phaseOfEducation = establishmentResponse.PhaseOfEducation.ShouldNotBeNull();

        establishmentType.ShouldSatisfyAllConditions(
            type => type.Name.ShouldBe("Academy"),
            type => type.Code.ShouldBe("AC")
        );

        establishmentGroupType.ShouldSatisfyAllConditions(
            groupType => groupType.Name.ShouldBe("Academy sponsor led"),
            groupType => groupType.Code.ShouldBe("ASL")
        );

        phaseOfEducation.ShouldSatisfyAllConditions(
            phase => phase.Name.ShouldBe("Primary"),
            phase => phase.Code.ShouldBe("PR")
        );
    }

    [Fact]
    public void An_establishment_without_name_and_code_types_is_mapped_with_nulls()
    {
        // Arrange
        var establishmentDto = AnEstablishmentDto()
            .WithEstablishmentType(null)
            .WithEstablishmentGroupType(null)
            .WithPhaseOfEducation(null)
            .Build();

        // Act
        var establishmentResponse = ToEstablishmentResponse(establishmentDto);

        // Assert
        establishmentResponse.EstablishmentType.ShouldBeNull();
        establishmentResponse.EstablishmentGroupType.ShouldBeNull();
        establishmentResponse.PhaseOfEducation.ShouldBeNull();
    }

    [Fact]
    public void A_name_and_code_with_null_values_returns_null()
    {
        // Arrange
        var trustDto = ATrustDto()
            .WithType(new NameAndCodeDto { Name = null, Code = null })
            .Build();

        // Act
        var trustResponse = ToTrustResponse(trustDto, []);

        // Assert
        trustResponse.Type.ShouldBeNull();
    }

    [Fact]
    public void A_null_name_for_a_name_and_code_returns_null()
    {
        // Arrange
        var trustDto = ATrustDto()
            .WithType(new NameAndCodeDto { Name = null, Code = "MAT" })
            .Build();

        // Act
        var trustResponse = ToTrustResponse(trustDto, []);

        // Assert
        trustResponse.Type.ShouldBeNull();
    }

    [Fact]
    public void A_null_code_for_a_name_and_code_returns_null()
    {
        // Arrange
        var trustDto = ATrustDto()
            .WithType(new NameAndCodeDto { Name = "Multi-academy trust", Code = null })
            .Build();

        // Act
        var trustResponse = ToTrustResponse(trustDto, []);

        // Assert
        trustResponse.Type.ShouldBeNull();
    }
}
