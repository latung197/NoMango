using System.Data;
using System.Collections.Generic;
using ERP_System.Models;

namespace ERP_System.Repositories
{
    public class MenuRepository : BaseRepository
    {
        public List<MenuItem> GetUserMenu(int userID)
        {
            var menuItems = new List<MenuItem>();
            var query = @"
                SELECT * FROM get_user_menu(@user_id)";

            var parameters = new Dictionary<string, object>
            {
                { "@user_id", userID }
            };

            var dataTable = ExecuteQuery(query, parameters);

            // Build menu tree
            var allItems = new Dictionary<int, MenuItem>();

            foreach (DataRow row in dataTable.Rows)
            {
                var menuItem = new MenuItem
                {
                    MenuItemID = row.Field<int>("menu_item_id"),
                    ParentID = row.IsDBNull("parent_id") ? 0 : row.Field<int>("parent_id"),
                    MenuCode = row.Field<string>("menu_code"),
                    MenuName = row.Field<string>("menu_name"),
                    MenuType = row.Field<string>("menu_type"),
                    AssemblyName = row.IsDBNull("assembly_name") ? "" : row.Field<string>("assembly_name"),
                    ClassName = row.IsDBNull("class_name") ? "" : row.Field<string>("class_name"),
                    Icon = row.IsDBNull("icon") ? "" : row.Field<string>("icon"),
                    SortOrder = row.Field<int>("sort_order"),
                    CanView = row.Field<bool>("can_view"),
                    CanAdd = row.Field<bool>("can_add"),
                    CanEdit = row.Field<bool>("can_edit"),
                    CanDelete = row.Field<bool>("can_delete"),
                    CanPrint = row.Field<bool>("can_print"),
                    CanExport = row.Field<bool>("can_export"),
                    CanApprove = row.Field<bool>("can_approve")
                };

                allItems[menuItem.MenuItemID] = menuItem;
            }

            // Build hierarchy
            foreach (var menuItem in allItems.Values)
            {
                if (menuItem.ParentID == 0)
                {
                    menuItems.Add(menuItem);
                }
                else if (allItems.ContainsKey(menuItem.ParentID))
                {
                    allItems[menuItem.ParentID].Children.Add(menuItem);
                }
            }

            // Sort menus
            menuItems = menuItems.OrderBy(m => m.SortOrder).ToList();
            foreach (var menu in menuItems)
            {
                menu.Children = new System.Collections.ObjectModel.ObservableCollection<MenuItem>(
                    menu.Children.OrderBy(c => c.SortOrder).ToList());
            }

            return menuItems;
        }
    }
}