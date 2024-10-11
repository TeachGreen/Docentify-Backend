using Docentify.Application.Common.Queries;

namespace Docentify.Application.Institutions.Queries;

public class GetInstitutionUsersQuery : PagedQuery
{
    public int InstitutionId { get; private set; }
    
    public void SetInstitutionId(int institutionId)
    {
        InstitutionId = institutionId;
    }
}