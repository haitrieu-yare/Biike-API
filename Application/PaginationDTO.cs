namespace Application
{
    public class PaginationDto
    {
        public PaginationDto(int page, int limit, int count, int lastPage, int totalRecord)
        {
            Page = page;
            Limit = limit;
            Count = count;
            LastPage = lastPage;
            TotalRecord = totalRecord;
        }

        public int Page { get; }
        public int Limit { get; }
        public int Count { get; }
        public int LastPage { get; }
        public int TotalRecord { get; }
    }
}