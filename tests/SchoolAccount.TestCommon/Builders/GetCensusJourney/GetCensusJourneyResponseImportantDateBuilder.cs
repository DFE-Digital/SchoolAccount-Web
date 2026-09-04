using SchoolAccount.Application.Features.GetCensusJourney;

namespace SchoolAccount.TestCommon.Builders.GetCensusJourney;

public class GetCensusJourneyResponseImportantDateBuilder
{
    private DateOnly _date = DateOnly.FromDateTime(DateTime.Now);
    private string _label = "Return due";

    public static GetCensusJourneyResponseImportantDateBuilder AnImportantDate()
    {
        return new GetCensusJourneyResponseImportantDateBuilder();
    }

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

    public ImportantDate Build()
    {
        return new ImportantDate { Label = _label, Date = _date };
    }
}
