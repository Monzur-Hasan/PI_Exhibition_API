namespace Domain.Models.Access.DTO
{
    public class MenuStructureDto
    {
        public string MMId { get; set; }
        public string MenuName { get; set; }
        public List<SubmenuStructureDto> SubMenus { get; set; }
    }
}
