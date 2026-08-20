// Navbar.tsx
import {  HomeIcon } from 'lucide-react';

interface NavbarProps {
  onToggleSidebar: () => void;
  isSidebarOpen: boolean;
}

const Navbar: React.FC<NavbarProps> = ({ onToggleSidebar, isSidebarOpen }) => {
  return (
    <nav className="fixed top-0 left-0 right-0 z-50 bg-white dark:bg-gray-900 border-b border-gray-200 dark:border-gray-700 h-16 shadow-sm">
      <div className="flex items-center justify-between h-full px-4">
        <div className="flex items-center gap-4">
          {/* Nút toggle sidebar */}
          <button
            onClick={onToggleSidebar}
            className="p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800 transition-colors focus:outline-none focus:ring-2 focus:ring-blue-500"
            aria-label={isSidebarOpen ? 'Close sidebar' : 'Open sidebar'}
          >
            {isSidebarOpen ? (
              <HomeIcon className="w-6 h-6 text-gray-700 dark:text-gray-300" />
            ) : (
              <HomeIcon className="w-6 h-6 text-gray-700 dark:text-gray-300" />
            )}
          </button>
        </div>
        
        {/* Các nút bên phải */}
        <div className="flex items-center gap-4">
          <button className="p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800 transition-colors">
            <span className="text-gray-700 dark:text-gray-300">🔔</span>
          </button>
          <button className="p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800 transition-colors">
            <span className="text-gray-700 dark:text-gray-300">👤</span>
          </button>
        </div>
      </div>
    </nav>
  );
};

export default Navbar;