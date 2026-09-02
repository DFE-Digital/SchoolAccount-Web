using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SchoolAccount.Application.Abstractions.Clients;
using SchoolAccount.Application.Collect.CensusStatuses;
using SchoolAccount.Application.Features.GetCensusJourney;
using SchoolAccount.SharedKernel;
using SchoolAccount.SharedKernel.Authentication;
using static System.Net.Mime.MediaTypeNames.Application;

namespace SchoolAccount.Infrastructure.Collect.CensusStatuses;

public sealed class CollectApiClient(HttpClient httpClient, ILogger<CollectApiClient> logger)
    : ICollectApiClient
{
    private const string _statusEndpoint = "/status";

    public async Task<List<GetCensusStatusesResponse>> GetCensusStatuses(
        string id,
        string emailAddress,
        IReadOnlyList<Organisation> organisations,
        CancellationToken cancellationToken
    )
    {
        var request = CensusStatusMapper.ToApiRequest(id, emailAddress, organisations);
        using var response = await httpClient.PostAsJsonAsync(
            _statusEndpoint,
            request,
            cancellationToken
        );

        if (!response.IsSuccessStatusCode)
        {
            await LogProblem(response, _statusEndpoint);
            response.EnsureSuccessStatusCode();
        }

        var content = await response.Content.ReadFromJsonAsync<GetCensusStatusesApiResponse>(
            cancellationToken
        );

        if (content is null)
        {
            logger.LogError("Response from {RequestUri} was empty", _statusEndpoint);

            throw new Exception("Response from Collect API was empty");
        }

        return content.Details.Select(CensusStatusMapper.ToResponse).ToList();
    }

    public async Task<Result<GetCensusJourneyResponse>> GetCensusJourneyContent(
        string id,
        string emailAddress,
        IReadOnlyList<Organisation> organisations,
        CancellationToken cancellationToken
    )
    {
        return await Task.FromResult(
            new GetCensusJourneyResponse
            {
                StepByStep =
                [
                    new StepByStep
                    {
                        Title = "Check the census dates",
                        Body = """
                        <p>There are two important dates to know.</p>
                        <p>‘Census date’ is the date that your census return describes. Your return should reflect your school as it was on this day.</p>
                        <p>‘Return date’ is the deadline for submitting your return. If your school is part of a trust or maintained by a local authority (LA), they may set an earlier deadline. They will confirm this with you.</p>
                        """,
                    },
                    new StepByStep
                    {
                        Title = "Prepare the data in your MIS",
                        Body = """
                        <p>Data for the census is prepared and checked in your school’s MIS, not in COLLECT. COLLECT is only used to submit the return and fix any errors.</p>
                        <p>There is <a href="https://www.gov.uk/guidance/complete-the-school-census/data-items-2026-to-2027">guidance on data items</a> covering everything the census collects this term. Your MIS supplier’s guidance will tell you where to record this data.</p>
                        <p>Start preparing the data well before census day. You may need information from several people in your school, including your:</p>
                        <ul>
                         <li>headteacher</li>
                         <li>special educational needs coordinator</li>
                         <li>staff responsible for catering and free school meals</li>
                        </ul>
                        """,
                    },
                    new StepByStep
                    {
                        Status = new StepByStepStatus { Label = "Available 3 September 2026" },
                        Title = "Test your data before you submit it",
                        Body = """
                        <p>COLLECT runs a “familiarisation period”, usually beginning 4 weeks before the census opens. During this time you can upload your census return file in advance, see any errors or queries it raises. Your school’s census contact will be emailed when this is available.</p>
                        <p>Export the census return file from your MIS, upload it to COLLECT, and see what issues it identifies. Fix any problems in your MIS and generate a new return. You can do this as many times as you need until your return is error-free.</p>
                        <p><a href="https://schools-account-prototype-44fad52ea0e1.herokuapp.com/v7/compliance/census-details#autumn-school-census-step-5">See step 5 for guidance on resolving errors and queries</a>.</p>
                        """,
                    },
                    new StepByStep
                    {
                        Title = "Generate your return and get headteacher sign-off",
                        Body = """
                        <p>When you’re confident that your census data is free of errors, generate the final return from your MIS.</p>
                        <p>Your headteacher must check and authorise the return before you submit it. Make sure they have seen it, because submitting the return confirms that they have authorised it.</p>
                        """,
                    },
                    new StepByStep
                    {
                        Status = new StepByStepStatus { Label = "Available 1 October 2026" },
                        Title = "Submit your return",
                        Body = """
                        <p>Upload your return to COLLECT. You will be told whether there are any errors or queries.</p>
                        <p>'Errors' must be fixed before you can submit. Correct them in your MIS, export a new return and upload it again.</p>
                        <p>'Queries' flag data that looks unusual. You must either correct them in your MIS, or explain why the data is correct by adding an explanation note in COLLECT.</p>
                        <p>Notes for queries must give a detailed reason. Notes like ‘confirmed’ or ‘data is correct’ won’t be accepted for most queries, and will delay DfE approving your return.</p>
                        <p>See guidance on:</p>
                        <ul>
                         <li><a rel="noreferrer noopener" target="_blank" href="https://www.gov.uk/guidance/complete-the-school-census/check-your-data#viewing-your-errors-and-queries">viewing your errors and queries (opens in new tab)</a></li>
                         <li><a rel="noreferrer noopener" target="_blank" href="https://www.gov.uk/guidance/complete-the-school-census/check-your-data#adding-explanation-notes-for-queries">adding explanation notes for queries (opens in new tab)</a></li>
                         <li><a href="https://assets.publishing.service.gov.uk/media/5cd5822840f0b6604efa74e2/School_Census_Notepad_entries_2025_26_v.1.1.xlsx">COLLECT queries and the explanatory notes DfE accepts (Excel sheet, 49KB)</a></li>
                        </ul>
                        <p>Once you have addressed any errors and queries, you can submit your return.</p>
                        """,
                    },
                    new StepByStep
                    {
                        Title = "Wait for DfE to check and authorise your return",
                        Body = """
                        <p>Once you have submitted your return, DfE will check and authorise it. This is the final confirmation that your return is complete.</p>
                        <p>You will be notified by email when your return is authorised. Until then, you do not need to do anything unless we contact you about a problem.</p>
                        <p>If a local authority or multi-academy trust approves your return on your behalf, they will check and approve it before sending it to DfE.</p>
                        """,
                    },
                ],
                CallToAction = new CallToAction
                {
                    Url = new Uri(
                        $"https://www.gov.uk/guidance/complete-the-school-census/generate-and-submit-your-return"
                    ),
                    ButtonText = "Go to Autumn Census 2026",
                },
            }
        );
    }

    private async Task LogProblem(HttpResponseMessage response, string requestUri)
    {
        var statusCode = response.StatusCode;
        var isBadRequest = statusCode == HttpStatusCode.BadRequest;
        var isJson = response.Content.Headers.ContentType?.MediaType is ProblemJson or Json;

        if (!isBadRequest || !isJson)
        {
            return;
        }

        var problemDetails = await ReadProblemDetails(response);

        if (problemDetails is null)
        {
            return;
        }

        if (problemDetails.Errors.Count > 0)
        {
            logger.LogError(
                "Request to {RequestUri} failed validation with {ValidationErrorCount} errors {@ValidationErrors}",
                requestUri,
                problemDetails.Errors.Count,
                problemDetails.Errors
            );
        }
    }

    private static async Task<HttpValidationProblemDetails?> ReadProblemDetails(
        HttpResponseMessage response
    )
    {
        try
        {
            return await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
