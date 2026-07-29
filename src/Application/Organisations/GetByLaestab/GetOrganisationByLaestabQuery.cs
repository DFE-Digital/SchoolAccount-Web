using Application.Abstractions.Messaging;

namespace Application.Organisations.GetByLaestab;

public sealed record GetOrganisationByLaestabQuery(string laestab) : IQuery<OrganisationResponse>;
