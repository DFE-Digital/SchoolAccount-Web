using SchoolAccount.Application.Features.GetCensusJourney;

namespace SchoolAccount.TestCommon.Builders.GetCensusJourney;

public class GetCensusJourneyResponseImportantDateBuilder
{
    private string _label = "Return due";
    private DateOnly _date = DateOnly.FromDateTime(DateTime.Now);

    public static GetCensusJourneyResponseImportantDateBuilder AnImportantDate() => new();

    public GetCensusJourneyResponseImportantDateBuilder WithLabel(string label)
    {
        _label = label;
        return this;
    }

    public GetCensusJourneyResponseImportantDateBuilder WithDate(int year, int month, int day)
    {
        _date = new DateOnly(year, month, day);
        return this;
    }

    public GetCensusJourneyResponseImportantDate Build()
    {
        return new GetCensusJourneyResponseImportantDate { Label = _label, Date = _date };
    }
}
