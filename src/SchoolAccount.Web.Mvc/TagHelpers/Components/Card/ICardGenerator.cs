namespace SchoolAccount.Web.Mvc.TagHelpers.Components.Card;

public interface ICardGenerator : IComponentGenerator
{
    CardComponent Generate(CardOptions options);
}
