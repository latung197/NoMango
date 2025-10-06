using System.Collections.ObjectModel;

namespace ERP_System.Models
{
    public class MenuItem : BaseModel
    {
        private int _menuItemID;
        public int MenuItemID
        {
            get => _menuItemID;
            set => SetProperty(ref _menuItemID, value);
        }

        private int _parentID;
        public int ParentID
        {
            get => _parentID;
            set => SetProperty(ref _parentID, value);
        }

        private string _menuCode;
        public string MenuCode
        {
            get => _menuCode;
            set => SetProperty(ref _menuCode, value);
        }

        private string _menuName;
        public string MenuName
        {
            get => _menuName;
            set => SetProperty(ref _menuName, value);
        }

        private string _menuType;
        public string MenuType
        {
            get => _menuType;
            set => SetProperty(ref _menuType, value);
        }

        private string _assemblyName;
        public string AssemblyName
        {
            get => _assemblyName;
            set => SetProperty(ref _assemblyName, value);
        }

        private string _className;
        public string ClassName
        {
            get => _className;
            set => SetProperty(ref _className, value);
        }

        private string _icon;
        public string Icon
        {
            get => _icon;
            set => SetProperty(ref _icon, value);
        }

        private ObservableCollection<MenuItem> _children;
        public ObservableCollection<MenuItem> Children
        {
            get => _children ??= new ObservableCollection<MenuItem>();
            set => SetProperty(ref _children, value);
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        // Permissions
        public bool CanView { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanPrint { get; set; }
        public bool CanExport { get; set; }
        public bool CanApprove { get; set; }
    }
}