
namespace DIn.Domain;

public class DInRequest
{
    public decimal BalanceValue { get; set; }
    public string DealerHeadOfficeCode { get; set; }
    public DfsPk DfsPk { get; set; }
}

public class DfsPk
{
    public string CodeDFSPlan { get; set; }
    public string DealerBranchCode { get; set; }
    public string PeriodMonth { get; set; }
    public string PeriodYear { get; set; }
    public string SectionCode { get; set; }
}

public class DInResponse
{
    public List<string> DealerBranchCodes { get; set; }
    public string DealerHeadOfficeCode { get; set; }
    public string PeriodMonth { get; set; }
    public string PeriodYear { get; set; }
    public string Protocol { get; set; }
    public decimal TotalValue { get; set; }
}
