namespace Domain.Models.Access.DTO
{
    public class SubmenuStructureDto
    {
        public string SubmenuId { get; set; }
        public string SubmenuName { get; set; }
        public bool IsActAsParent { get; set; }
        public string ParentSubmenuId { get; set; }
        public string MenuSequence { get; set; }
        public string MMId { get; set; }
        public List<SubmenuStructureDto> ChildSubMenus { get; set; }
    }
}
