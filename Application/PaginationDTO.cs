namespace Application
{
    public class PaginationDto
    {
        public PaginationDto()
        {
        }

        public PaginationDto(int page, int limit, int count, int lastPage, int totalRecord)
        {
            Page = page;
            Limit = limit;
            Count = count;
            LastPage = lastPage;
            TotalRecord = totalRecord;
        }

        public int Page { get; set; }
        public int Limit { get; set; }
        public int Count { get; set; }
        public int LastPage { get; set; }
        public int TotalRecord { get; set; }
    }
}