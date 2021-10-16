namespace Application
{
    public static class ApplicationUtils
    {
        public static int CalculateLastPage(int totalRecord, int limit)
        {
            var remain = totalRecord % limit;
            var lastPage = (totalRecord - remain) / limit;

            if (remain > 0) lastPage++;

            return lastPage;
        }
    }
}