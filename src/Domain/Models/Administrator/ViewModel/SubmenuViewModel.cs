using Domain.Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Administrator.ViewModel
{
    /// <summary>
    ///  Do not remove any property from this SubmenuViewModel class without knowing about well.
    ///  Because it is inherited by AppSubmenus class
    /// </summary>
    public class SubmenuViewModel : BaseModel
    {
        public long SubmenuId { get; set; }
        [StringLength(100)]
        public string SubmenuName { get; set; }
        [StringLength(100)]
        public string ControllerName { get; set; }
        [StringLength(100)]
        public string ActionName { get; set; }
        [StringLength(100)]
        public string Path { get; set; }
        [StringLength(100)]
        public string Component { get; set; }
        [StringLength(100)]
        public string IconClass { get; set; }
        [StringLength(100)]
        public string IconColor { get; set; }
        public bool IsViewable { get; set; }
        public bool IsActAsParent { get; set; }
        public bool HasTab { get; set; }
        public bool IsActive { get; set; }
        public long? ParentSubmenuId { get; set; }
        public long MMId { get; set; }
        [StringLength(100)]
        public string MenuName { get; set; }
        public long ModuleId { get; set; }
        [StringLength(100)]
        public string ModuleName { get; set; }
        public int? MenuSequence { get; set; } = 0;
        public long? ApplicationId { get; set; }
        public int? SequenceNo { get; set; } = 0;
        [StringLength(100)]
        public string ApplicationName { get; set; }
        public long? BranchId { get; set; }
    }
    public class AppSubmenus : SubmenuViewModel
    {
        public string MainmenuIconClass { get; set; }
        public string MainmenuIconColor { get; set; }
        public string ParentSubmenuName { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Detail { get; set; }
        public bool Delete { get; set; }
        public bool Approval { get; set; }
        public bool Report { get; set; }
        public bool Upload { get; set; }
        public long TabId { get; set; }
        public string TabName { get; set; }
        public string TabIconClass { get; set; }
        public string TabIconColor { get; set; }
        public bool TabAdd { get; set; }
        public bool TabEdit { get; set; }
        public bool TabDetail { get; set; }
        public bool TabDelete { get; set; }
        public bool TabApproval { get; set; }
        public bool TabReport { get; set; }
        public bool TabUpload { get; set; }
        public int SequenceNo { get; set; }
    }
    public class AppUserMenu
    {
        public long MainmenuId { get; set; }
        public string MainmenuName { get; set; }
        public string IconClass { get; set; }
        public string IconColor { get; set; }
        public int? SequenceNo { get; set; } = 0;
        public List<AppUserSubmenu> AppUserSubmenus { get; set; }
    }

    public class UsermenuWithComponent
    {
        public UsermenuWithComponent()
        {
            AppUserMenus = new List<AppUserMenu>();
            Components = new List<Component>();
        }
        public IEnumerable<AppUserMenu> AppUserMenus { get; set; }
        public List<Component> Components { get; set; }
    }

    public class Component
    {
        public long SubmenuId { get; set; }
        public string Submenu { get; set; }
        public string Name { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Detail { get; set; }
        public bool Delete { get; set; }
        public bool Approval { get; set; }
        public bool Report { get; set; }
        public bool Upload { get; set; }
        public bool HasTab { get; set; }
        public string Tabs { get; set; }
        public List<TabsOfComponent> TabsOfComponents { get; set; }
    }

    public class TabsOfComponent
    {
        public long TabId { get; set; }
        public string TabName { get; set; }
        public bool TabAdd { get; set; }
        public bool TabEdit { get; set; }
        public bool TabDetail { get; set; }
        public bool TabDelete { get; set; }
        public bool TabApproval { get; set; }
        public bool TabReport { get; set; }
        public bool TabUpload { get; set; }
    }

    public class AppUserSubmenu
    {
        public long SubmenuId { get; set; }
        public string SubmenuName { get; set; }
        public string Path { get; set; }
        public string IconClass { get; set; }
        public string IconColor { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Detail { get; set; }
        public bool Delete { get; set; }
        public bool Approval { get; set; }
        public bool Report { get; set; }
        public bool Upload { get; set; }
        public bool HasTab { get; set; }
        public bool IsActAsParent { get; set; }
        public string Component { get; set; }
        public List<AppUserSubSubmenu> AppUserSubSubmenus { get; set; }
        public List<AppUserPageTab> AppUserPageTabs { get; set; }
    }
    public class AppUserSubSubmenu
    {
        public long SubSubmenuId { get; set; }
        public string SubSubmenuName { get; set; }
        public string Path { get; set; }
        public string IconClass { get; set; }
        public string IconColor { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Detail { get; set; }
        public bool Delete { get; set; }
        public bool Approval { get; set; }
        public bool Report { get; set; }
        public bool Upload { get; set; }
        public bool HasTab { get; set; }
        public string Component { get; set; }
        public List<AppUserPageTab> AppUserPageTabs { get; set; }
    }
    public class AppUserPageTab
    {
        public long TabId { get; set; }
        public string TabName { get; set; }
        public string TabIconClass { get; set; }
        public string TabIconColor { get; set; }
        public string TabComponent { get; set; }
        public bool TabAdd { get; set; }
        public bool TabEdit { get; set; }
        public bool TabDetail { get; set; }
        public bool TabDelete { get; set; }
        public bool TabApproval { get; set; }
        public bool TabReport { get; set; }
        public bool TabUpload { get; set; }
    }
    public class AppMainMenuForPermission
    {
        public long MainmenuId { get; set; }
        public string MainmenuName { get; set; }
        public long ModuleId { get; set; }
        public List<AppSubmenuForPermission> AppSubmenuForPermissions { get; set; }
    }
    public class AppSubmenuForPermission
    {
        public long SubmenuId { get; set; }
        public string SubmenuName { get; set; }
        public long SubSubmenuId { get; set; }
        public string SubSubmenuName { get; set; }
        public long TabId { get; set; }
        public string TabName { get; set; }
        public bool IsSubmenuPermission { get; set; }
        public bool IsPageTabPermission { get; set; }
        public bool HasParentSubmenu { get; set; }
        public bool IsAll { get; set; }
        public bool IsAdd { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDetail { get; set; }
        public bool IsDelete { get; set; }
        public bool IsApproval { get; set; }
        public bool IsReport { get; set; }
        public bool IsUpload { get; set; }
    }
}
