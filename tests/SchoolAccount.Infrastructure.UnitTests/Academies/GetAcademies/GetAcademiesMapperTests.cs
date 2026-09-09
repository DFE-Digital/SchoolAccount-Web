using System.Collections.ObjectModel;
using GovUK.Dfe.AcademiesApi.Client.Contracts;
using SchoolAccount.Infrastructure.Clients.Academies.GetAcademies;
using Shouldly;

namespace SchoolAccount.Infrastructure.UnitTests.Academies.GetAcademies;

public class GetAcademiesMapperTests
{
    [Fact]
    public void A_TrustDto_is_mapped_to_a_GetAcademyTrustResponse()
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
        var response = GetAcademiesMapper.ToTrustResponse(trust, []);

        // Assert
        response.Ukprn.ShouldBe("10012345");
        response.Name.ShouldBe("Test Trust");
        response.GroupUid.ShouldBe("TR00123");
        response.Type.ShouldNotBeNull();
        response.Type.Name.ShouldBe("Multi-academy trust");
        response.Type.Code.ShouldBe("MAT");
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
        var response = GetAcademiesMapper.ToTrustResponse(trust, []);

        // Assert
        response.Type.ShouldBeNull();
    }

    [Fact]
    public void A_trust_without_a_name_throws()
    {
        // Arrange
        var trust = new TrustDto { Ukprn = "10012345", Name = null };

        // Act
        var mapperResponse = () => GetAcademiesMapper.ToTrustResponse(trust, []);

        // Assert
        Should.Throw<InvalidOperationException>(mapperResponse);
    }

    [Fact]
    public void A_trust_without_a_ukprn_throws()
    {
        // Arrange
        var trust = new TrustDto { Ukprn = null, Name = "Test Trust" };

        // Act
        var mapperResponse = () => GetAcademiesMapper.ToTrustResponse(trust, []);

        // Assert
        Should.Throw<InvalidOperationException>(mapperResponse);
    }

    [Fact]
    public void Every_establishment_is_mapped_onto_the_trust()
    {
        // Arrange
        var trust = new TrustDto { Ukprn = "10012345", Name = "Test Trust" };
        var establishments = new ObservableCollection<EstablishmentDto>
        {
            new() { Ukprn = "10011111", Name = "First School" },
            new() { Ukprn = "10022222", Name = "Second School" },
        };

        // Act
        var response = GetAcademiesMapper.ToTrustResponse(trust, establishments);

        // Assert
        response.Establishments.ShouldNotBeNull();
        response.Establishments.Count.ShouldBe(2);
        response.Establishments[0].EstablishmentName.ShouldBe("First School");
        response.Establishments[1].EstablishmentName.ShouldBe("Second School");
    }

    [Fact]
    public void A_trust_without_establishments_is_mapped_with_no_establishments()
    {
        // Arrange
        var trust = new TrustDto { Ukprn = "10012345", Name = "Test Trust" };

        // Act
        var response = GetAcademiesMapper.ToTrustResponse(trust, []);

        // Assert
        response.Establishments.ShouldBeNull();
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
        var response = GetAcademiesMapper.ToEstablishmentResponse(establishment);

        // Assert
        response.Ukprn.ShouldBe("10011111");
        response.EstablishmentName.ShouldBe("Test School");
        response.Urn.ShouldBe("100001");
        response.EstablishmentNumber.ShouldBe("1234");
        response.LocalAuthorityCode.ShouldBe("201");
        response.LocalAuthorityName.ShouldBe("Camden");
    }

    [Fact]
    public void An_establishment_without_a_ukprn_throws()
    {
        // Arrange
        var establishment = new EstablishmentDto { Ukprn = null, Name = "Test Trust" };

        // Act
        var mapperResponse = () => GetAcademiesMapper.ToEstablishmentResponse(establishment);

        // Assert
        Should.Throw<InvalidOperationException>(mapperResponse);
    }

    [Fact]
    public void An_establishment_without_a_name_throws()
    {
        // Arrange
        var establishment = new EstablishmentDto { Ukprn = "Test Trust", Name = null };

        // Act
        var mapperResponse = () => GetAcademiesMapper.ToEstablishmentResponse(establishment);

        // Assert
        Should.Throw<InvalidOperationException>(mapperResponse);
    }

    [Fact]
    public void An_establishment_with_null_fields_is_mapped_to_empty_strings()
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
        var response = GetAcademiesMapper.ToEstablishmentResponse(establishment);

        // Assert
        response.Ukprn.ShouldBe("10011111");
        response.EstablishmentName.ShouldBe("Test School");
        response.Urn.ShouldBe(string.Empty);
        response.EstablishmentNumber.ShouldBe(string.Empty);
        response.LocalAuthorityCode.ShouldBe(string.Empty);
        response.LocalAuthorityName.ShouldBe(string.Empty);
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
        var response = GetAcademiesMapper.ToEstablishmentResponse(establishment);

        // Assert
        response.EstablishmentType!.Name.ShouldBe("Academy");
        response.EstablishmentType.Code.ShouldBe("AC");
        response.EstablishmentGroupType!.Name.ShouldBe("Academy sponsor led");
        response.EstablishmentGroupType.Code.ShouldBe("ASL");
        response.PhaseOfEducation!.Name.ShouldBe("Primary");
        response.PhaseOfEducation.Code.ShouldBe("PR");
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
        var response = GetAcademiesMapper.ToEstablishmentResponse(establishment);

        // Assert
        response.EstablishmentType.ShouldBeNull();
        response.EstablishmentGroupType.ShouldBeNull();
        response.PhaseOfEducation.ShouldBeNull();
    }

    [Fact]
    public void A_name_and_code_with_null_values_is_mapped_to_empty_strings()
    {
        // Arrange
        var trust = new TrustDto
        {
            Ukprn = "10012345",
            Name = "Test Trust",
            Type = new NameAndCodeDto { Name = null, Code = null },
        };

        // Act
        var response = GetAcademiesMapper.ToTrustResponse(trust, []);

        // Assert
        response.Type!.Name.ShouldBe(string.Empty);
        response.Type.Code.ShouldBe(string.Empty);
    }
}
