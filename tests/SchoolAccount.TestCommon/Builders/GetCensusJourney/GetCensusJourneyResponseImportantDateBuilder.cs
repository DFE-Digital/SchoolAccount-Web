using SchoolAccount.Application.Features.GetCensusJourney;

namespace SchoolAccount.TestCommon.Builders.GetCensusJourney;

public class GetCensusJourneyResponseImportantDateBuilder
{
    private string _label = "Return due";
    private DateOnly _date = DateOnly.FromDateTime(DateTime.Now);

    public static GetCensusJourneyResponseImportantDateBuilder Create() => new();

    public GetCensusJourneyResponseImportantDateBuilder WithLabel(string label)
    {
        _label = label;
        return this;
    }

    public GetCensusJourneyResponseImportantDateBuilder WithDate(DateOnly date)
    {
        _date = date;
        return this;
    }

    public GetCensusJourneyResponseImportantDate Build()
    {
        return new GetCensusJourneyResponseImportantDate { Label = _label, Date = _date };
    }
}
