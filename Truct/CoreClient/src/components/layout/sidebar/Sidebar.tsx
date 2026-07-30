// Sidebar.tsx
import { NavLink } from 'react-router-dom';
import { 
  HomeIcon, 
  UserIcon, 
  CogIcon,
  ChartBarIcon,
  InboxIcon,
  FolderIcon,
  CalendarIcon
} from 'lucide-react';

interface MenuItem {
  path: string;
  label: string;
  icon: React.ForwardRefExoticComponent<React.ComponentProps<'svg'> & {
    title?: string;
    titleId?: string;
  }>;
}

interface SidebarProps {
  isOpen: boolean;
}

const menuItems: MenuItem[] = [
  { path: '/dashboard', label: 'Dashboard', icon: HomeIcon },
  { path: '/users', label: 'Users', icon: UserIcon },
  { path: '/inbox', label: 'Inbox', icon: InboxIcon },
  { path: '/projects', label: 'Projects', icon: FolderIcon },
  { path: '/calendar', label: 'Calendar', icon: CalendarIcon },
  { path: '/reports', label: 'Reports', icon: ChartBarIcon },
  { path: '/documents', label: 'Documents', icon: CalendarIcon },
  { path: '/settings', label: 'Settings', icon: CogIcon },
];

const Sidebar: React.FC<SidebarProps> = ({ isOpen }) => {
  return (
    <nav className="h-full py-4 overflow-y-auto">
      <ul className="space-y-1">
        {menuItems.map((item) => (
          <li key={item.path} className="relative group">
            <NavLink
              to={item.path}
              className={({ isActive }) =>
                `flex items-center mx-2 px-3 py-3 rounded-lg transition-all duration-200 ${
                  isActive
                    ? 'bg-blue-500 text-white shadow-md'
                    : 'text-gray-700 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-gray-700'
                } ${!isOpen && 'justify-center'}`
              }
              title={!isOpen ? item.label : undefined}
            >
              <item.icon className={`w-6 h-6 flex-shrink-0 ${!isOpen && 'w-7 h-7'}`} />
              {isOpen && (
                <span className="ml-3 text-sm font-medium whitespace-nowrap">
                  {item.label}
                </span>
              )}
            </NavLink>
            
            {/* Tooltip khi sidebar đóng */}
            {!isOpen && (
              <div className="absolute left-full top-1/2 -translate-y-1/2 ml-2 px-2 py-1 bg-gray-900 text-white text-sm rounded-md opacity-0 group-hover:opacity-100 pointer-events-none transition-opacity whitespace-nowrap z-50 shadow-lg">
                {item.label}
              </div>
            )}
          </li>
        ))}
      </ul>
    </nav>
  );
};

export default Sidebar;