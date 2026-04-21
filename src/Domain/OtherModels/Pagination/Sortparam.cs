using System.ComponentModel.DataAnnotations;

namespace Domain.OtherModels.Pagination
{
    public class Sortparam : Pageparam
    {
        public Sortparam()
        {
            SortingCol = "";
            SortType = "";
        }
        [StringLength(100)]
        public string? SortingCol { get; set; }
        [StringLength(100)]
        public string? SortType { get; set; }
    }
}
