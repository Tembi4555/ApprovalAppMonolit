namespace ApprovalAppMonolit.Contracts
{
    public record TicketsRequest
    (
        string? Title,

        string? Description,

        long IdAuthor
    );
}
