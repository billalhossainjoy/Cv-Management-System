using CVMS.Application.Positions;

namespace CVMS.Web.Models.Home;

public class HomeViewModel
{
    public int PositionCount { get; set; }

    public int CandidateCount { get; set; }

    public int RecruiterCount { get; set; }

    public int AttributeCount { get; set; }

    public IReadOnlyList<PositionListItem> LatestPositions { get; set; }
        = [];
}