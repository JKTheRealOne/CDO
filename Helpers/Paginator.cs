namespace CDO.Helpers
{
    public class Paginator
    {
        public int TotalItems { get;private set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }

        public Paginator()
        {
        }
        public Paginator(int totalitems,int page, int pagesize = 10)
        {
            int totalPages = (int)Math.Ceiling((decimal)totalitems / (decimal)pagesize);
            int currentpage = page;

            int startPage = currentpage - 5;
            int endPage = currentpage + 4;

            if (startPage <= 0)
            {
                endPage = endPage - (startPage - 1);
                startPage = 1;
            }
            if (endPage > totalPages)
            {
                endPage = totalPages;
                if (endPage > 10)
                {
                    startPage = endPage - 9;
                }
            }

            TotalItems = totalitems;
            CurrentPage = currentpage;
            PageSize = pagesize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
        }
    }
}
