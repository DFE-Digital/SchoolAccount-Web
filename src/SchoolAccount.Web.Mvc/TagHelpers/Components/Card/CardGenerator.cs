namespace SchoolAccount.Web.Mvc.TagHelpers.Components.Card;

public sealed class CardGenerator : ICardGenerator
{
    public CardComponent Generate(CardOptions options) => new(options);
}
