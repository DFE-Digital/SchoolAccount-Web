using NetArchTest.Rules;
using Shouldly;

namespace SchoolAccount.ArchitectureTests.Layers;

public class LayerTests : BaseTest
{
    [Fact]
    public void The_domain_layer_does_not_depend_on_the_application_layer()
    {
        // Arrange & Act
        var result = Types
            .InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn("SchoolAccount.Application")
            .GetResult();

        // Assert
        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void The_domain_layer_does_not_depend_on_the_infrastructure_layer()
    {
        // Arrange & Act
        var result = Types
            .InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureAssembly.GetName().Name)
            .GetResult();

        // Assert
        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void The_domain_layer_does_not_depend_on_the_presentation_layer()
    {
        // Arrange & Act
        var result = Types
            .InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(PresentationAssembly.GetName().Name)
            .GetResult();

        // Assert
        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void The_application_layer_does_not_depend_on_the_infrastructure_layer()
    {
        // Arrange & Act
        var result = Types
            .InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureAssembly.GetName().Name)
            .GetResult();

        // Assert
        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void The_application_layer_does_not_depend_on_the_presentation_layer()
    {
        // Arrange & Act
        var result = Types
            .InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn(PresentationAssembly.GetName().Name)
            .GetResult();

        // Assert
        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void The_infrastructure_layer_does_not_depend_on_the_presentation_layer()
    {
        // Arrange & Act
        var result = Types
            .InAssembly(InfrastructureAssembly)
            .Should()
            .NotHaveDependencyOn(PresentationAssembly.GetName().Name)
            .GetResult();

        // Assert
        result.IsSuccessful.ShouldBeTrue();
    }
}
