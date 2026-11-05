using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SchoolAccount.Application;
using Shouldly;

namespace SchoolAccount.ApplicationTests;

public class DependencyInjectionTests
{
    [Theory]
    [MemberData(nameof(ApplicationValidators))]
    public void Every_validator_in_the_application_layer_is_registered(
        Type interfaceType,
        Type validatorType
    )
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplication();
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        // Assert
        scope.ServiceProvider.GetService(interfaceType).ShouldBeOfType(validatorType);
    }

    public static TheoryData<Type, Type> ApplicationValidators()
    {
        var data = new TheoryData<Type, Type>();

        var validators = AssemblyScanner.FindValidatorsInAssembly(
            typeof(DependencyInjection).Assembly,
            includeInternalTypes: true
        );

        foreach (var validator in validators)
        {
            data.Add(validator.InterfaceType, validator.ValidatorType);
        }

        return data;
    }
}
