namespace Application
{
    public static class Utils
    {
        public static int CalculateLastPage(int totalRecord, int limit)
        {
            int remain = totalRecord % limit;
            int lastPage = (totalRecord - remain) / limit;

            if (remain > 0) lastPage++;

            return lastPage;
        }
    }
}