import React, { useState } from 'react';
import {
  BarChart3,
  FileText,
  Users,
  Settings,
  Search,
  Filter,
  Download,
  Eye,
  Edit,
  Plus,
  Trash2,
  Save,
  Check,
  X,
  Upload,
  QrCode,
  Camera,
  User,
  Cog,
  Package,
  Factory,
  MapPin,
  Clock,
  Database,
  Wrench,
  Building2,
  Truck,
  Calendar,
  AlertTriangle,
  CheckCircle,
  XCircle,
  Wifi,
  WifiOff,
  Monitor,
  Smartphone,
  Printer,
  Radio,
  Zap,
  Info,
  ShoppingCart,
  Scale,
  Ruler,
  Calculator,
  ChevronRight,
  MoreHorizontal,
  Shield,
  UserCog,
  Settings2,
  Lock,
  Unlock,
  Key,
  UserCheck,
  UserPlus
} from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Badge } from './ui/badge';
import { Progress } from './ui/progress';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Label } from './ui/label';
import { Textarea } from './ui/textarea';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger, DialogDescription } from './ui/dialog';
import { Switch } from './ui/switch';
import { Tabs, TabsContent, TabsList, TabsTrigger } from './ui/tabs';
import { Checkbox } from './ui/checkbox';

interface MasterDataProps {
  currentPage: string;
  onPageChange: (page: string) => void;
}

export function MasterData({ currentPage, onPageChange }: MasterDataProps) {
  const [selectedItem, setSelectedItem] = useState<string | null>(null);
  const [showAddDialog, setShowAddDialog] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [languageToggle, setLanguageToggle] = useState<'en' | 'mm'>('en');
  const [selectedRole, setSelectedRole] = useState<string>('planner');
  const [showCreateRoleDialog, setShowCreateRoleDialog] = useState(false);
  const [rolePermissions, setRolePermissions] = useState<Record<string, Record<string, string>>>({});

  // Master data categories with counts and status
  const masterDataCategories = [
    {
      id: 'machines',
      title: 'Machines',
      titleMM: 'စက်များ',
      icon: Factory,
      total: 25,
      active: 23,
      pending: 1,
      inactive: 1,
      description: 'Production machines and equipment'
    },
    {
      id: 'users-roles',
      title: 'Users & Roles',
      titleMM: 'အသုံးပြုသူများနှင့်အခန်းကဏ္ဍများ',
      icon: Users,
      total: 89,
      active: 82,
      pending: 4,
      inactive: 3,
      description: 'User accounts and role assignments'
    },
    {
      id: 'products',
      title: 'Products',
      titleMM: 'ထုတ်ကုန်များ',
      icon: Package,
      total: 156,
      active: 148,
      pending: 6,
      inactive: 2,
      description: 'Product catalog and specifications'
    },
    {
      id: 'raw-materials',
      title: 'Raw Materials',
      titleMM: 'ကုန်ကြမ်းများ',
      icon: ShoppingCart,
      total: 342,
      active: 298,
      pending: 12,
      inactive: 32,
      description: 'Raw material inventory'
    },
    {
      id: 'molds-tooling',
      title: 'Molds & Tooling',
      titleMM: 'ပုံစံများနှင့်ကိရိယာများ',
      icon: Wrench,
      total: 78,
      active: 65,
      pending: 8,
      inactive: 5,
      description: 'Molds and tooling equipment'
    },
    {
      id: 'warehouses',
      title: 'Warehouses',
      titleMM: 'ကုန်လှောင်ရုံများ',
      icon: Building2,
      total: 12,
      active: 11,
      pending: 1,
      inactive: 0,
      description: 'Warehouse locations and zones'
    },
    {
      id: 'shifts-schedules',
      title: 'Shifts & Schedules',
      titleMM: 'အလုပ်ပတ်နှင့်အစီအစဉ်များ',
      icon: Clock,
      total: 18,
      active: 15,
      pending: 2,
      inactive: 1,
      description: 'Work shifts and schedules'
    },
    {
      id: 'reason-codes',
      title: 'Reason Codes',
      titleMM: 'အကြောင်းပြချက်ကုဒ်များ',
      icon: AlertTriangle,
      total: 45,
      active: 42,
      pending: 2,
      inactive: 1,
      description: 'System reason codes'
    },
    {
      id: 'devices',
      title: 'Devices',
      titleMM: 'ကိရိယာများ',
      icon: Monitor,
      total: 67,
      active: 58,
      pending: 5,
      inactive: 4,
      description: 'RFID, QR, and system devices'
    },
    {
      id: 'customers-hq',
      title: 'Customers & HQ',
      titleMM: 'ဖောက်သည်များနှင့်ရုံးချုပ်',
      icon: Truck,
      total: 23,
      active: 21,
      pending: 1,
      inactive: 1,
      description: 'Customer and HQ information'
    },
    {
      id: 'units-measure',
      title: 'Units of Measure',
      titleMM: 'တိုင်းတာမှုယူနစ်များ',
      icon: Scale,
      total: 35,
      active: 33,
      pending: 1,
      inactive: 1,
      description: 'Measurement units and conversions'
    }
  ];

  // Mock data for different categories
  const mockMachines = [
    {
      id: 'MCH-001',
      name: 'Injection Molding Machine 1',
      nameMM: 'ပန်းရိုးမုန့်ထည့်သွင်းစက် ၁',
      type: 'Injection',
      zone: 'Production Zone A',
      capacity: 1200,
      status: 'active',
      linkedMolds: ['MLD-001', 'MLD-002'],
      lastMaintenance: '2025-01-15'
    },
    {
      id: 'MCH-002',
      name: 'Extrusion Line 1',
      nameMM: 'ဆွဲထုတ်မှုလိုင်း ၁',
      type: 'Extrusion',
      zone: 'Production Zone B',
      capacity: 800,
      status: 'maintenance',
      linkedMolds: ['MLD-003'],
      lastMaintenance: '2025-01-10'
    },
    {
      id: 'MCH-003',
      name: 'Cutting Machine 1',
      nameMM: 'ဖြတ်တောက်စက် ၁',
      type: 'Cutting',
      zone: 'Finishing Zone',
      capacity: 2000,
      status: 'active',
      linkedMolds: [],
      lastMaintenance: '2025-01-20'
    }
  ];

  const mockUsers = [
    {
      id: 'USR-001',
      name: 'John Doe',
      nameMM: 'ဂျွန်ဒို',
      department: 'Production',
      role: 'Operator',
      rfidCode: 'RFID-001',
      qrCode: 'QR-001',
      status: 'active',
      email: 'john.doe@factory.com',
      phone: '+95-123-456789'
    },
    {
      id: 'USR-002',
      name: 'Ma Thandar',
      nameMM: 'မသန္တာ',
      department: 'Quality Control',
      role: 'QC Inspector',
      rfidCode: 'RFID-002',
      qrCode: 'QR-002',
      status: 'active',
      email: 'thandar@factory.com',
      phone: '+95-987-654321'
    },
    {
      id: 'USR-003',
      name: 'Ko Zaw Min',
      nameMM: 'ကိုဇော်မင်း',
      department: 'Engineering',
      role: 'Technician',
      rfidCode: 'RFID-003',
      qrCode: 'QR-003',
      status: 'pending',
      email: 'zawmin@factory.com',
      phone: '+95-555-123456'
    }
  ];

  const mockProducts = [
    {
      id: 'PRD-001',
      name: 'Plastic Bottle 500ml',
      nameMM: 'ပလတ်စတစ်ပုလင်း ၅၀၀မီလီ',
      type: 'Container',
      photo: '/api/placeholder/100/100',
      uom: 'Pieces',
      weight: 15.5,
      color: 'Clear',
      packaging: 'Carton of 48',
      status: 'active'
    },
    {
      id: 'PRD-002',
      name: 'Food Container Large',
      nameMM: 'အစားအသောက်ပုံးကြီး',
      type: 'Container',
      photo: '/api/placeholder/100/100',
      uom: 'Pieces',
      weight: 32.8,
      color: 'White',
      packaging: 'Carton of 24',
      status: 'active'
    }
  ];

  const mockRawMaterials = [
    {
      id: 'RM-001',
      name: 'PET Pellets',
      nameMM: 'PET ပုဆိုးများ',
      type: 'Pellet',
      uom: 'KG',
      location: 'RM-WH-A',
      vendor: 'Polymer Solutions Ltd',
      expiry: '2025-12-31',
      status: 'active'
    },
    {
      id: 'RM-002',
      name: 'Colorant Blue',
      nameMM: 'အပြာရောင်ဆေး',
      type: 'Colorant',
      uom: 'KG',
      location: 'RM-WH-B',
      vendor: 'Color Tech Co',
      expiry: '2025-08-15',
      status: 'active'
    }
  ];

  const mockDevices = [
    {
      id: 'DEV-001',
      name: 'RFID Terminal 1',
      nameMM: 'RFID တာမီနယ် ၁',
      type: 'RFID Terminal',
      assignedMachine: 'MCH-001',
      status: 'online',
      lastSeen: '2025-01-25 14:30',
      ipAddress: '192.168.1.101'
    },
    {
      id: 'DEV-002',
      name: 'QR Printer Zone A',
      nameMM: 'QR ပရင်တာ ဇုန် A',
      type: 'QR Printer',
      assignedMachine: 'MCH-002',
      status: 'offline',
      lastSeen: '2025-01-25 12:15',
      ipAddress: '192.168.1.102'
    },
    {
      id: 'DEV-003',
      name: 'Tablet Station 1',
      nameMM: 'တက်ဘလက်စခန်း ၁',
      type: 'Tablet',
      assignedMachine: 'MCH-003',
      status: 'online',
      lastSeen: '2025-01-25 14:35',
      ipAddress: '192.168.1.103'
    },
    {
      id: 'role-matrix',
      title: 'Role Matrix',
      titleMM: 'အခန်းကဏ္ဍဇယား',
      icon: Shield,
      total: 8,
      active: 6,
      pending: 1,
      inactive: 1,
      description: 'Role permissions matrix'
    }
  ];

  // Role definitions with badges and colors
  const roleDefinitions = [
    { 
      id: 'admin', 
      name: 'Admin', 
      nameMM: 'စီမံအုပ်ချုပ်သူ', 
      badge: '🔐', 
      color: 'bg-red-100 text-red-800',
      description: 'Full system access'
    },
    { 
      id: 'planner', 
      name: 'Planner', 
      nameMM: 'အစီအစဉ်ချမှတ်သူ', 
      badge: '🗂', 
      color: 'bg-blue-100 text-blue-800',
      description: 'Planning and scheduling'
    },
    { 
      id: 'operator', 
      name: 'Operator', 
      nameMM: 'အော်ပရေတာ', 
      badge: '👷', 
      color: 'bg-green-100 text-green-800',
      description: 'Production operations'
    },
    { 
      id: 'supervisor', 
      name: 'Supervisor', 
      nameMM: 'ကြီးကြပ်ရေးမှူး', 
      badge: '👨‍💼', 
      color: 'bg-purple-100 text-purple-800',
      description: 'Team supervision'
    },
    { 
      id: 'maintenance', 
      name: 'Maintenance', 
      nameMM: 'ပြုပြင်ထိန်းသိမ်းမှု', 
      badge: '🛠', 
      color: 'bg-orange-100 text-orange-800',
      description: 'Equipment maintenance'
    },
    { 
      id: 'inventory', 
      name: 'Inventory', 
      nameMM: 'စတော့ခ်', 
      badge: '📦', 
      color: 'bg-teal-100 text-teal-800',
      description: 'Stock management'
    },
    { 
      id: 'qc', 
      name: 'QC Inspector', 
      nameMM: 'အရည်အသွေးစစ်ဆေးသူ', 
      badge: '🔍', 
      color: 'bg-indigo-100 text-indigo-800',
      description: 'Quality control'
    },
    { 
      id: 'custom', 
      name: 'Custom Role', 
      nameMM: 'စိတ်ကြိုက်အခန်းကဏ္ဍ', 
      badge: '⚙️', 
      color: 'bg-slate-100 text-slate-800',
      description: 'User-defined role'
    }
  ];

  // Permission modules and features
  const permissionModules = [
    {
      id: 'production',
      name: 'Production',
      nameMM: 'ထုတ်လုပ်မှု',
      icon: Factory,
      features: [
        { id: 'operator-kiosk', name: 'Operator Kiosk', nameMM: 'အော်ပရေတာကီရာ့စ်' },
        { id: 'supervisor-screen', name: 'Supervisor Screen', nameMM: 'ကြီးကြပ်ရေးမှူးမျက်နှာပြင်' },
        { id: 'live-monitoring', name: 'Live Monitoring', nameMM: 'တိုက်ရိုက်စောင့်ကြည့်မှု' },
        { id: 'job-monitoring', name: 'Job Monitoring', nameMM: 'အလုပ်စောင့်ကြည��်မှု' }
      ]
    },
    {
      id: 'planning',
      name: 'Planning',
      nameMM: 'အစီအစဉ်ချမှတ်မှု',
      icon: Calendar,
      features: [
        { id: 'production-planning', name: 'Production Planning', nameMM: 'ထုတ်လုပ်မှုအစီအစဉ်' },
        { id: 'product-master', name: 'Product Master', nameMM: 'ထုတ်ကုန်စာရင်း' },
        { id: 'demand-authorization', name: 'Demand Authorization', nameMM: 'လိုအပ်ချက်အတည်ပြုခြင်း' }
      ]
    },
    {
      id: 'inventory',
      name: 'Inventory',
      nameMM: 'စတော့ခ်',
      icon: Package,
      features: [
        { id: 'warehouse-management', name: 'Warehouse Management', nameMM: 'ကုန်လှောင်ရုံစီမံခန့်ခွဲမှု' },
        { id: 'stock-transactions', name: 'Stock Transactions', nameMM: 'စတော့ခ်ငွေကြေးလွှဲပြောင်းမှု' },
        { id: 'stock-adjustment', name: 'Stock Adjustment', nameMM: 'စတော့ခ်ချိန်ညှိမှု' },
        { id: 'hq-dispatch', name: 'HQ Dispatch', nameMM: 'ရုံးချုပ်စေလွှတ်မှု' }
      ]
    },
    {
      id: 'engineering',
      name: 'Engineering',
      nameMM: 'အင်ဂျင်နီယာ',
      icon: Wrench,
      features: [
        { id: 'breakdown-handling', name: 'Breakdown Handling', nameMM: 'ပျက်စီးမှုကိုင်တွယ်ခြင်း' },
        { id: 'preventive-maintenance', name: 'Preventive Maintenance', nameMM: 'ကြိုတင်ပြုပြင်ထိန်းသိမ်းမှု' },
        { id: 'spare-parts-management', name: 'Spare Parts Management', nameMM: 'အရန်အစိတ်အပိုင်းများစီမံခန့်ခွဲမှု' }
      ]
    },
    {
      id: 'qc',
      name: 'Quality Control',
      nameMM: 'အရည်အသွေးထိန်းချုပ်မှု',
      icon: Shield,
      features: [
        { id: 'qc-dashboard', name: 'QC Dashboard', nameMM: 'အရည်အသွေးဒက်ရှ်ဘုတ်' },
        { id: 'qc-entry-form', name: 'QC Entry Form', nameMM: 'အရည်အသွေးဖြည့်သွင်းဖောင်' },
        { id: 'defect-handling', name: 'Defect Handling', nameMM: 'ချွတ်ယွင်းမှုကိုင်တွယ်မှု' }
      ]
    },
    {
      id: 'hr',
      name: 'HR & Operator',
      nameMM: 'လူ့စွမ်းအားနှင့်အော်ပရေတာ',
      icon: Users,
      features: [
        { id: 'shift-assignment', name: 'Shift Assignment', nameMM: 'အလုပ်ပတ်သတ်မှတ်မှု' },
        { id: 'attendance-logs', name: 'Attendance Logs', nameMM: 'တက်ရောက်မှုမှတ်တမ်းများ' },
        { id: 'productivity-reports', name: 'Productivity Reports', nameMM: 'ကုန်ထုတ်စွမ်းအားအစီရင်ခံစာများ' }
      ]
    },
    {
      id: 'maintenance',
      name: 'Maintenance',
      nameMM: 'ပြုပြင်ထိန်းသိမ်းမှု',
      icon: Cog,
      features: [
        { id: 'machine-downtime', name: 'Machine Downtime', nameMM: 'စက်ရပ်နားချိန်' },
        { id: 'maintenance-schedule', name: 'Maintenance Schedule', nameMM: 'ပြုပြင်ထိန်းသိမ်းမှုအစီအစဉ်' }
      ]
    },
    {
      id: 'reports',
      name: 'Reports & Dashboard',
      nameMM: 'အစီရင်ခံစာများနှင့်ဒက်ရှ်ဘုတ်',
      icon: BarChart3,
      features: [
        { id: 'executive-dashboard', name: 'Executive Dashboard', nameMM: 'အမှုဆောင်ဒက်ရှ်ဘုတ်' },
        { id: 'advanced-reporting', name: 'Advanced Reporting', nameMM: 'အဆင့်မြင့်အစီရင်ခံစာများ' },
        { id: 'data-export', name: 'Data Export', nameMM: 'ဒေတာထုတ်ယူမှု' }
      ]
    }
  ];

  // Permission levels
  const permissionLevels = [
    { id: 'view', name: 'View', nameMM: 'ကြည့်ရန်', icon: Eye },
    { id: 'create', name: 'Create', nameMM: 'ဖန်တီးရန်', icon: Plus },
    { id: 'edit', name: 'Edit', nameMM: 'တည်းဖြတ်ရန်', icon: Edit },
    { id: 'approve', name: 'Approve', nameMM: 'အတည်ပြုရန်', icon: Check },
    { id: 'export', name: 'Export', nameMM: 'ထုတ်ယူရန်', icon: Download },
    { id: 'delete', name: 'Delete', nameMM: 'ဖျက်ရ��်', icon: Trash2 }
  ];

  // Initialize default permissions for roles
  React.useEffect(() => {
    const defaultPermissions: Record<string, Record<string, string>> = {};
    
    roleDefinitions.forEach(role => {
      defaultPermissions[role.id] = {};
      permissionModules.forEach(module => {
        module.features.forEach(feature => {
          permissionLevels.forEach(level => {
            const key = `${module.id}_${feature.id}_${level.id}`;
            
            if (role.id === 'admin') {
              defaultPermissions[role.id][key] = 'allowed';
            } else if (role.id === 'planner') {
              if (['planning', 'inventory'].includes(module.id) && ['view', 'create', 'edit'].includes(level.id)) {
                defaultPermissions[role.id][key] = 'allowed';
              } else if (level.id === 'view') {
                defaultPermissions[role.id][key] = 'allowed';
              } else {
                defaultPermissions[role.id][key] = 'denied';
              }
            } else if (role.id === 'operator') {
              if (module.id === 'production' && ['view', 'create', 'edit'].includes(level.id)) {
                defaultPermissions[role.id][key] = 'allowed';
              } else if (level.id === 'view') {
                defaultPermissions[role.id][key] = 'limited';
              } else {
                defaultPermissions[role.id][key] = 'denied';
              }
            } else if (role.id === 'supervisor') {
              if (['production', 'hr'].includes(module.id)) {
                defaultPermissions[role.id][key] = level.id === 'delete' ? 'limited' : 'allowed';
              } else if (level.id === 'view') {
                defaultPermissions[role.id][key] = 'allowed';
              } else {
                defaultPermissions[role.id][key] = 'limited';
              }
            } else if (role.id === 'maintenance') {
              if (module.id === 'maintenance') {
                defaultPermissions[role.id][key] = 'allowed';
              } else if (['engineering', 'production'].includes(module.id) && ['view', 'edit'].includes(level.id)) {
                defaultPermissions[role.id][key] = 'allowed';
              } else {
                defaultPermissions[role.id][key] = level.id === 'view' ? 'limited' : 'denied';
              }
            } else if (role.id === 'inventory') {
              if (module.id === 'inventory') {
                defaultPermissions[role.id][key] = 'allowed';
              } else if (level.id === 'view') {
                defaultPermissions[role.id][key] = 'allowed';
              } else {
                defaultPermissions[role.id][key] = 'denied';
              }
            } else if (role.id === 'qc') {
              if (module.id === 'qc') {
                defaultPermissions[role.id][key] = 'allowed';
              } else if (module.id === 'production' && ['view', 'edit'].includes(level.id)) {
                defaultPermissions[role.id][key] = 'allowed';
              } else {
                defaultPermissions[role.id][key] = level.id === 'view' ? 'limited' : 'denied';
              }
            } else {
              defaultPermissions[role.id][key] = 'denied';
            }
          });
        });
      });
    });

    setRolePermissions(defaultPermissions);
  }, []);

  const getStatusBadge = (status: string) => {
    const statusConfig: any = {
      'active': { color: 'bg-green-100 text-green-800', label: '🟢 Active', labelMM: '🟢 လှုပ်ရှားနေ', icon: CheckCircle },
      'pending': { color: 'bg-yellow-100 text-yellow-800', label: '🟡 Pending', labelMM: '🟡 စောင့်ဆိုင်းနေ', icon: Clock },
      'inactive': { color: 'bg-red-100 text-red-800', label: '🔴 Inactive', labelMM: '🔴 လှုပ်ရှားမှုမရှိ', icon: XCircle },
      'maintenance': { color: 'bg-orange-100 text-orange-800', label: '🔧 Maintenance', labelMM: '🔧 ပြုပြင်ထိန်းသိမ်းမှု', icon: Wrench },
      'online': { color: 'bg-green-100 text-green-800', label: '🟢 Online', labelMM: '🟢 အွန်လိုင်း', icon: Wifi },
      'offline': { color: 'bg-red-100 text-red-800', label: '🔴 Offline', labelMM: '🔴 အော့ဖ်လိုင်း', icon: WifiOff }
    };
    const config = statusConfig[status] || { color: 'bg-slate-100 text-slate-800', label: status, labelMM: status, icon: Info };
    const label = languageToggle === 'en' ? config.label : config.labelMM;
    return <Badge className={config.color}>{label}</Badge>;
  };

  const getPermissionIcon = (permission: string) => {
    switch (permission) {
      case 'allowed':
        return '✅';
      case 'denied':
        return '❌';
      case 'limited':
        return '⚠️';
      default:
        return '❌';
    }
  };

  const getPermissionColor = (permission: string) => {
    switch (permission) {
      case 'allowed':
        return 'bg-green-100 text-green-800';
      case 'denied':
        return 'bg-gray-100 text-gray-800';
      case 'limited':
        return 'bg-yellow-100 text-yellow-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  const togglePermission = (moduleId: string, featureId: string, levelId: string) => {
    const key = `${moduleId}_${featureId}_${levelId}`;
    const currentPermission = rolePermissions[selectedRole]?.[key] || 'denied';
    
    let newPermission: string;
    switch (currentPermission) {
      case 'denied':
        newPermission = 'limited';
        break;
      case 'limited':
        newPermission = 'allowed';
        break;
      case 'allowed':
        newPermission = 'denied';
        break;
      default:
        newPermission = 'denied';
    }

    setRolePermissions(prev => ({
      ...prev,
      [selectedRole]: {
        ...prev[selectedRole],
        [key]: newPermission
      }
    }));
  };

  const getRoleSummary = (roleId: string) => {
    const permissions = rolePermissions[roleId] || {};
    const role = roleDefinitions.find(r => r.id === roleId);
    
    const allowedModules: string[] = [];
    const limitedModules: string[] = [];
    const deniedModules: string[] = [];
    
    permissionModules.forEach(module => {
      let hasAllowed = false;
      let hasLimited = false;
      let hasDenied = false;
      
      module.features.forEach(feature => {
        permissionLevels.forEach(level => {
          const key = `${module.id}_${feature.id}_${level.id}`;
          const permission = permissions[key];
          
          if (permission === 'allowed') hasAllowed = true;
          else if (permission === 'limited') hasLimited = true;
          else hasDenied = true;
        });
      });
      
      if (hasAllowed && !hasLimited && !hasDenied) {
        allowedModules.push(module.name);
      } else if (hasLimited || (hasAllowed && hasDenied)) {
        limitedModules.push(module.name);
      } else {
        deniedModules.push(module.name);
      }
    });
    
    let summary = `${role?.name} → `;
    if (allowedModules.length > 0) {
      summary += `Full Access: ${allowedModules.join(', ')}`;
    }
    if (limitedModules.length > 0) {
      if (allowedModules.length > 0) summary += '; ';
      summary += `Limited Access: ${limitedModules.join(', ')}`;
    }
    if (deniedModules.length > 0 && deniedModules.length < permissionModules.length) {
      if (allowedModules.length > 0 || limitedModules.length > 0) summary += '; ';
      summary += `No Access: ${deniedModules.join(', ')}`;
    }
    
    return summary;
  };

  const renderRoleMatrix = () => (
    <div className="space-y-6">
      {/* Top Toolbar */}
      <div className="flex justify-between items-center">
        <div>
          <h2 className="text-xl font-semibold">
            {languageToggle === 'en' ? 'Role Matrix Management' : 'အခန်းကဏ္ဍဇယားစီမံခန့်ခွဲမှု'}
          </h2>
          <p className="text-muted-foreground">
            {languageToggle === 'en' 
              ? 'Configure role-based permissions for system modules and features' 
              : 'စနစ်မော်ဂျူးများနှင့် လုပ်ဆောင်ချက်များအတွက် အခန်းကဏ္ဍအခ���ေခံခွင့်ပြုချက်များကို စီမံဆောင်ရွက်ပါ'}
          </p>
        </div>
        <div className="flex gap-3">
          <Button variant="outline" onClick={() => setLanguageToggle(languageToggle === 'en' ? 'mm' : 'en')}>
            {languageToggle === 'en' ? 'မြန်မာ' : 'English'}
          </Button>
          <Button onClick={() => setShowCreateRoleDialog(true)}>
            <Plus className="h-4 w-4 mr-2" />
            {languageToggle === 'en' ? 'Create New Role' : 'အခန်းကဏ္ဍအသစ်ဖန်တီးပါ'}
          </Button>
          <Button>
            <Save className="h-4 w-4 mr-2" />
            {languageToggle === 'en' ? 'Save Role Matrix' : 'အခန်းကဏ္ဍဇယားသိမ်းဆည်းပါ'}
          </Button>
        </div>
      </div>

      {/* Role Selection */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <UserCog className="h-5 w-5" />
            {languageToggle === 'en' ? 'Select Role to Configure' : 'စီမံဆောင်ရွက်ရန် အခန်းကဏ္ဍရွေးချယ်ပါ'}
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
            {roleDefinitions.map((role) => (
              <Card 
                key={role.id}
                className={`cursor-pointer transition-all ${selectedRole === role.id ? 'ring-2 ring-blue-500 bg-blue-50' : 'hover:shadow-md'}`}
                onClick={() => setSelectedRole(role.id)}
              >
                <CardContent className="p-4 text-center">
                  <div className="text-2xl mb-2">{role.badge}</div>
                  <div className="font-medium">{languageToggle === 'en' ? role.name : role.nameMM}</div>
                  <div className="text-xs text-muted-foreground">{role.description}</div>
                  <div className="mt-2">
                    <Badge className={role.color}>
                      {role.badge} {role.name}
                    </Badge>
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>
        </CardContent>
      </Card>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Permission Matrix */}
        <div className="lg:col-span-2">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Settings2 className="h-5 w-5" />
                {languageToggle === 'en' ? 'Permission Matrix' : 'ခွင့်ပြုချက်ဇယား'}
                {selectedRole && (
                  <Badge className={roleDefinitions.find(r => r.id === selectedRole)?.color}>
                    {roleDefinitions.find(r => r.id === selectedRole)?.badge} {roleDefinitions.find(r => r.id === selectedRole)?.name}
                  </Badge>
                )}
              </CardTitle>
            </CardHeader>
            <CardContent className="p-0">
              <div className="overflow-x-auto">
                <table className="w-full">
                  <thead className="bg-slate-50">
                    <tr>
                      <th className="text-left p-3 border-b font-medium sticky left-0 bg-slate-50 min-w-[200px]">
                        {languageToggle === 'en' ? 'Module | Feature' : 'မော်ဂျူး | လုပ်ဆောင်ချက်'}
                      </th>
                      {permissionLevels.map((level) => (
                        <th key={level.id} className="text-center p-3 border-b font-medium min-w-[80px]">
                          <div className="flex flex-col items-center gap-1">
                            <level.icon className="h-4 w-4" />
                            <span className="text-xs">
                              {languageToggle === 'en' ? level.name : level.nameMM}
                            </span>
                          </div>
                        </th>
                      ))}
                    </tr>
                  </thead>
                  <tbody>
                    {permissionModules.map((module, moduleIndex) => (
                      <React.Fragment key={module.id}>
                        {/* Module Header */}
                        <tr className="bg-slate-25">
                          <td className="p-3 border-b font-medium sticky left-0 bg-slate-25">
                            <div className="flex items-center gap-2">
                              <module.icon className="h-4 w-4" />
                              <span>{languageToggle === 'en' ? module.name : module.nameMM}</span>
                            </div>
                          </td>
                          {permissionLevels.map((level) => (
                            <td key={`${module.id}-header-${level.id}`} className="p-3 border-b bg-slate-25"></td>
                          ))}
                        </tr>
                        
                        {/* Module Features */}
                        {module.features.map((feature, featureIndex) => (
                          <tr key={feature.id} className="hover:bg-slate-50">
                            <td className="p-3 border-b sticky left-0 bg-white">
                              <div className="pl-6 text-sm">
                                {languageToggle === 'en' ? feature.name : feature.nameMM}
                              </div>
                            </td>
                            {permissionLevels.map((level) => {
                              const key = `${module.id}_${feature.id}_${level.id}`;
                              const permission = rolePermissions[selectedRole]?.[key] || 'denied';
                              
                              return (
                                <td key={level.id} className="p-3 border-b text-center">
                                  <button
                                    className={`w-8 h-8 rounded-full flex items-center justify-center transition-all hover:scale-110 ${getPermissionColor(permission)}`}
                                    onClick={() => togglePermission(module.id, feature.id, level.id)}
                                    title={`${permission} - Click to toggle`}
                                  >
                                    <span className="text-sm">{getPermissionIcon(permission)}</span>
                                  </button>
                                </td>
                              );
                            })}
                          </tr>
                        ))}
                      </React.Fragment>
                    ))}
                  </tbody>
                </table>
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Preview Summary Panel */}
        <div className="space-y-6">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Eye className="h-5 w-5" />
                {languageToggle === 'en' ? 'Permission Summary' : 'ခွင့်ပြုချက်အကျဉ်းချုပ်'}
              </CardTitle>
            </CardHeader>
            <CardContent>
              {selectedRole && (
                <div className="space-y-4">
                  <div className="flex items-center gap-2">
                    <span className="text-2xl">{roleDefinitions.find(r => r.id === selectedRole)?.badge}</span>
                    <div>
                      <div className="font-medium">
                        {languageToggle === 'en' 
                          ? roleDefinitions.find(r => r.id === selectedRole)?.name
                          : roleDefinitions.find(r => r.id === selectedRole)?.nameMM}
                      </div>
                      <div className="text-xs text-muted-foreground">
                        {roleDefinitions.find(r => r.id === selectedRole)?.description}
                      </div>
                    </div>
                  </div>
                  
                  <div className="text-sm bg-slate-50 p-3 rounded">
                    <div className="font-medium mb-2">
                      {languageToggle === 'en' ? 'Role Summary:' : 'အခန်းကဏ္ဍအကျဉ်းချုပ်:'}
                    </div>
                    <div className="text-xs text-muted-foreground">
                      {getRoleSummary(selectedRole)}
                    </div>
                  </div>
                  
                  {/* Permission Legend */}
                  <div className="space-y-2">
                    <div className="text-sm font-medium">
                      {languageToggle === 'en' ? 'Permission Legend:' : 'ခွင့်ပြုချက်သင်္ကေတ:'}
                    </div>
                    <div className="space-y-1 text-xs">
                      <div className="flex items-center gap-2">
                        <span className="w-6 h-6 rounded-full bg-green-100 text-green-800 flex items-center justify-center">✅</span>
                        <span>{languageToggle === 'en' ? 'Allowed - Full Access' : 'ခွင့်ပြု - အပြည့်အဝဝင်ရောက်ခွင့်'}</span>
                      </div>
                      <div className="flex items-center gap-2">
                        <span className="w-6 h-6 rounded-full bg-yellow-100 text-yellow-800 flex items-center justify-center">⚠️</span>
                        <span>{languageToggle === 'en' ? 'Limited - Conditional Access' : 'ကန့်သတ် - အခြေအနေအလိုက်ဝင်ရောက်ခွင့်'}</span>
                      </div>
                      <div className="flex items-center gap-2">
                        <span className="w-6 h-6 rounded-full bg-gray-100 text-gray-800 flex items-center justify-center">❌</span>
                        <span>{languageToggle === 'en' ? 'Denied - No Access' : 'ငြင်းပယ် - ဝင်ရောက်ခွင့်မရှိ'}</span>
                      </div>
                    </div>
                  </div>
                </div>
              )}
            </CardContent>
          </Card>

          {/* Quick Actions */}
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Zap className="h-5 w-5" />
                {languageToggle === 'en' ? 'Quick Actions' : 'မြန်ဆန်သောလုပ်ဆောင်ချက်များ'}
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="space-y-2">
                <Button variant="outline" className="w-full justify-start" size="sm">
                  <Check className="h-4 w-4 mr-2" />
                  {languageToggle === 'en' ? 'Grant All Permissions' : 'ခွင့်ပြုချက်အားလုံးပေးပါ'}
                </Button>
                <Button variant="outline" className="w-full justify-start" size="sm">
                  <X className="h-4 w-4 mr-2" />
                  {languageToggle === 'en' ? 'Revoke All Permissions' : 'ခွင့်ပြုချက်အားလုံးရုပ်သိမ်းပါ'}
                </Button>
                <Button variant="outline" className="w-full justify-start" size="sm">
                  <Eye className="h-4 w-4 mr-2" />
                  {languageToggle === 'en' ? 'View Only Mode' : 'ကြည့်ရှုရုံမျှ စနစ်'}
                </Button>
                <Button variant="outline" className="w-full justify-start" size="sm">
                  <Upload className="h-4 w-4 mr-2" />
                  {languageToggle === 'en' ? 'Import from Excel' : 'Excel မှတင်သွင်းပါ'}
                </Button>
                <Button variant="outline" className="w-full justify-start" size="sm">
                  <Download className="h-4 w-4 mr-2" />
                  {languageToggle === 'en' ? 'Export to Excel' : 'Excel သို့ထုတ်ယူပါ'}
                </Button>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>

      {/* Create New Role Dialog */}
      <Dialog open={showCreateRoleDialog} onOpenChange={setShowCreateRoleDialog}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>
              {languageToggle === 'en' ? 'Create New Role' : 'အခန်းကဏ္ဍအသစ်ဖန်တီးပါ'}
            </DialogTitle>
            <DialogDescription>
              {languageToggle === 'en' 
                ? 'Define a new custom role with specific permissions for your factory system.'
                : 'သင့်စက်ရုံစနစ်အတွက် အထူးခွင့်ပြုချက်မ���ားပါသော စိတ်ကြိုက်အခန်းကဏ္ဍအသစ်တစ်ခုကို သတ်မှတ်ပါ။'}
            </DialogDescription>
          </DialogHeader>
          <div className="space-y-4">
            <div>
              <Label htmlFor="roleName">
                {languageToggle === 'en' ? 'Role Name (English)' : 'အခန်းကဏ္ဍအမည် (အင်္ဂလိပ်)'}
              </Label>
              <Input id="roleName" placeholder="Enter role name..." />
            </div>
            <div>
              <Label htmlFor="roleNameMM">
                {languageToggle === 'en' ? 'Role Name (Myanmar)' : 'အခန်းကဏ္ဍအမည် (မြန်မာ)'}
              </Label>
              <Input id="roleNameMM" placeholder="အခန်းကဏ္ဍအမည်ရိုက်ထည့်ပါ..." />
            </div>
            <div>
              <Label htmlFor="roleDescription">
                {languageToggle === 'en' ? 'Description' : 'ဖော်ပြချက်'}
              </Label>
              <Input id="roleDescription" placeholder="Role description..." />
            </div>
            <div>
              <Label htmlFor="roleBadge">
                {languageToggle === 'en' ? 'Badge Emoji' : 'အမှတ်လက္ခဏာ Emoji'}
              </Label>
              <Input id="roleBadge" placeholder="🔧" />
            </div>
            <div className="flex justify-end gap-2">
              <Button variant="outline" onClick={() => setShowCreateRoleDialog(false)}>
                {languageToggle === 'en' ? 'Cancel' : 'ပယ်ဖျက်ပါ'}
              </Button>
              <Button onClick={() => setShowCreateRoleDialog(false)}>
                <UserPlus className="h-4 w-4 mr-2" />
                {languageToggle === 'en' ? 'Create Role' : 'အခန်းကဏ္ဍဖန်တီးပါ'}
              </Button>
            </div>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );

  const renderDashboard = () => (
    <div className="space-y-6">
      {/* Dashboard Header */}
      <div className="flex justify-between items-center">
        <div>
          <h2 className="text-xl font-semibold">
            {languageToggle === 'en' ? 'Master Data Dashboard' : 'အခြေခံဒေတာဒက်ရှ်ဘုတ်'}
          </h2>
          <p className="text-muted-foreground">
            {languageToggle === 'en' 
              ? 'Manage all factory master data and registrations' 
              : 'စက်ရုံအခြေခံဒေတာများနှင့် မှတ်ပုံတင်မှုများကို စီမံခန့်ခွဲပါ'}
          </p>
        </div>
        <div className="flex gap-3">
          <Button variant="outline" onClick={() => setLanguageToggle(languageToggle === 'en' ? 'mm' : 'en')}>
            {languageToggle === 'en' ? 'မြန်မာ' : 'English'}
          </Button>
          <Button>
            <Plus className="h-4 w-4 mr-2" />
            {languageToggle === 'en' ? 'Add New' : 'အသစ်ထည့်ပါ'}
          </Button>
        </div>
      </div>

      {/* Search Bar */}
      <div className="flex gap-4">
        <div className="flex-1 relative">
          <Search className="absolute left-3 top-3 h-4 w-4 text-muted-foreground" />
          <Input 
            placeholder={languageToggle === 'en' ? "Search by code, name, or type..." : "ကုဒ်၊ အမည် သို့မဟုတ် အမျိုးအစားဖြင့် ရှာဖွေပါ..."}
            className="pl-10" 
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
        </div>
        <Button variant="outline">
          <Filter className="h-4 w-4 mr-2" />
          {languageToggle === 'en' ? 'Filter' : 'စစ်ထုတ်ပါ'}
        </Button>
        <Button variant="outline">
          <Download className="h-4 w-4 mr-2" />
          {languageToggle === 'en' ? 'Export' : 'ထုတ်ယူပါ'}
        </Button>
      </div>

      {/* Master Data Category Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
        {masterDataCategories.map((category) => {
          const Icon = category.icon;
          return (
            <Card 
              key={category.id} 
              className="hover:shadow-md transition-shadow cursor-pointer"
              onClick={() => onPageChange(category.id)}
            >
              <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle className="text-sm font-medium">
                  {languageToggle === 'en' ? category.title : category.titleMM}
                </CardTitle>
                <Icon className="h-4 w-4 text-muted-foreground" />
              </CardHeader>
              <CardContent>
                <div className="text-2xl font-bold mb-2">{category.total}</div>
                <p className="text-xs text-muted-foreground mb-3">
                  {languageToggle === 'en' ? category.description : category.titleMM}
                </p>
                <div className="flex gap-2 text-xs">
                  <span className="flex items-center gap-1">
                    <div className="w-2 h-2 bg-green-500 rounded-full"></div>
                    {category.active}
                  </span>
                  <span className="flex items-center gap-1">
                    <div className="w-2 h-2 bg-yellow-500 rounded-full"></div>
                    {category.pending}
                  </span>
                  <span className="flex items-center gap-1">
                    <div className="w-2 h-2 bg-red-500 rounded-full"></div>
                    {category.inactive}
                  </span>
                </div>
              </CardContent>
            </Card>
          );
        })}
      </div>

      {/* Quick Stats */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">
              {languageToggle === 'en' ? 'Total Records' : 'စုစုပေါင်းမှတ်တမ်းများ'}
            </CardTitle>
            <Database className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">890</div>
            <p className="text-xs text-muted-foreground">
              {languageToggle === 'en' ? 'Across all categories' : 'အမျိုးအစားအားလုံးတွင်'}
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">
              {languageToggle === 'en' ? 'Active Items' : 'လှုပ်ရှားနေသောအရာများ'}
            </CardTitle>
            <CheckCircle className="h-4 w-4 text-green-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-green-600">781</div>
            <p className="text-xs text-muted-foreground">87.8% active rate</p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">
              {languageToggle === 'en' ? 'Pending Approval' : 'အတည်ပြုခြင်းစောင့်ဆိုင်းနေ'}
            </CardTitle>
            <Clock className="h-4 w-4 text-yellow-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-yellow-600">43</div>
            <p className="text-xs text-muted-foreground">
              {languageToggle === 'en' ? 'Requires review' : 'ပြန်လည်သုံးသပ်ရန်လိုအပ်'}
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">
              {languageToggle === 'en' ? 'System Health' : 'စနစ်ကျန်းမာရေး'}
            </CardTitle>
            <Zap className="h-4 w-4 text-blue-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-blue-600">98%</div>
            <p className="text-xs text-muted-foreground">
              {languageToggle === 'en' ? 'Data integrity' : 'ဒေတာတည်ငြိမ်မှု'}
            </p>
          </CardContent>
        </Card>
      </div>
    </div>
  );

  const renderMachines = () => (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <h2 className="text-xl font-semibold">
            {languageToggle === 'en' ? 'Machine Registration' : 'စက်မှတ်ပုံတင်မှု'}
          </h2>
          <p className="text-muted-foreground">
            {languageToggle === 'en' ? 'Manage production machines and equipment' : 'ထုတ်လုပ်မှုစက်များနှင့် ကိရိယာများကို စီမံခန့်ခွဲပါ'}
          </p>
        </div>
        <Button onClick={() => setShowAddDialog(true)}>
          <Plus className="h-4 w-4 mr-2" />
          {languageToggle === 'en' ? 'Add Machine' : 'စက်ထည့်ပါ'}
        </Button>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Factory className="h-5 w-5" />
            {languageToggle === 'en' ? 'Machines List' : 'စက်များစာရင်း'}
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-hidden rounded-lg border">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>{languageToggle === 'en' ? 'Machine ID' : 'စက်အမှတ်'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Machine Name' : 'စက်အမည်'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Type' : 'အမျိုးအစား'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Zone' : 'ဇုန်'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Capacity' : 'စွမ်းရည်'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Status' : 'အခြေအနေ'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Linked Molds' : 'ချိတ်ဆက်ထားသောပုံစံများ'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Actions' : 'လုပ်ဆောင်ချက်များ'}</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {mockMachines.map((machine) => (
                  <TableRow key={machine.id}>
                    <TableCell className="font-medium">{machine.id}</TableCell>
                    <TableCell>
                      <div>
                        <div className="font-medium">{machine.name}</div>
                        <div className="text-xs text-muted-foreground">{machine.nameMM}</div>
                      </div>
                    </TableCell>
                    <TableCell>
                      <Badge variant="outline">{machine.type}</Badge>
                    </TableCell>
                    <TableCell>{machine.zone}</TableCell>
                    <TableCell>
                      <div className="flex items-center gap-2">
                        <span>{machine.capacity}</span>
                        <span className="text-xs text-muted-foreground">pcs/day</span>
                      </div>
                    </TableCell>
                    <TableCell>{getStatusBadge(machine.status)}</TableCell>
                    <TableCell>
                      <div className="flex flex-wrap gap-1">
                        {machine.linkedMolds.map((mold) => (
                          <Badge key={mold} variant="secondary" className="text-xs">
                            {mold}
                          </Badge>
                        ))}
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex gap-2">
                        <Button size="sm" variant="outline">
                          <Eye className="h-4 w-4" />
                        </Button>
                        <Button size="sm" variant="outline">
                          <Edit className="h-4 w-4" />
                        </Button>
                      </div>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        </CardContent>
      </Card>
    </div>
  );

  const renderUsers = () => (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <h2 className="text-xl font-semibold">
            {languageToggle === 'en' ? 'User & Role Registration' : 'အသုံးပြုသူနှင့်အခန်းကဏ္ဍမှတ်ပုံတင်မှု'}
          </h2>
          <p className="text-muted-foreground">
            {languageToggle === 'en' ? 'Manage user accounts and role assignments' : 'အသုံးပြုသူအကောင့်များနှင့် အခန်းကဏ္ဍချမှတ်မှုများကို စီမံခန့်ခွဲပါ'}
          </p>
        </div>
        <Button onClick={() => setShowAddDialog(true)}>
          <Plus className="h-4 w-4 mr-2" />
          {languageToggle === 'en' ? 'Add User' : 'အသုံးပြုသူထည့်ပါ'}
        </Button>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Users className="h-5 w-5" />
            {languageToggle === 'en' ? 'Users List' : 'အသုံးပြုသူများစာရင်း'}
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-hidden rounded-lg border">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>{languageToggle === 'en' ? 'User ID' : 'အသုံးပြုသူအမှတ်'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Name' : 'အမည်'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Department' : 'ဌာန'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Role' : 'အခန်းကဏ္ဍ'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'RFID/QR Code' : 'RFID/QR ကုဒ်'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Status' : 'အခြေအနေ'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Actions' : 'လုပ်ဆောင်ချက်များ'}</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {mockUsers.map((user) => (
                  <TableRow key={user.id}>
                    <TableCell className="font-medium">{user.id}</TableCell>
                    <TableCell>
                      <div className="flex items-center gap-3">
                        <div className="w-8 h-8 bg-slate-200 rounded-full flex items-center justify-center">
                          <User className="h-4 w-4 text-slate-500" />
                        </div>
                        <div>
                          <div className="font-medium">{user.name}</div>
                          <div className="text-xs text-muted-foreground">{user.nameMM}</div>
                        </div>
                      </div>
                    </TableCell>
                    <TableCell>{user.department}</TableCell>
                    <TableCell>
                      <Badge variant="outline">{user.role}</Badge>
                    </TableCell>
                    <TableCell>
                      <div className="flex gap-2">
                        <div className="flex items-center gap-1">
                          <Radio className="h-3 w-3 text-blue-500" />
                          <span className="text-xs">{user.rfidCode}</span>
                        </div>
                        <div className="flex items-center gap-1">
                          <QrCode className="h-3 w-3 text-purple-500" />
                          <span className="text-xs">{user.qrCode}</span>
                        </div>
                      </div>
                    </TableCell>
                    <TableCell>{getStatusBadge(user.status)}</TableCell>
                    <TableCell>
                      <div className="flex gap-2">
                        <Button size="sm" variant="outline">
                          <Eye className="h-4 w-4" />
                        </Button>
                        <Button size="sm" variant="outline">
                          <Edit className="h-4 w-4" />
                        </Button>
                        <Switch defaultChecked={user.status === 'active'} />
                      </div>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        </CardContent>
      </Card>
    </div>
  );

  const renderProducts = () => (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <h2 className="text-xl font-semibold">
            {languageToggle === 'en' ? 'Product Registration' : 'ထုတ်ကုန်မှတ်ပုံတင်မှု'}
          </h2>
          <p className="text-muted-foreground">
            {languageToggle === 'en' ? 'Manage product catalog and specifications' : 'ထုတ်ကုန်ကတ်တလောက်နှင့် အသေးစိတ်ဖော်ပြချက်များကို စီမံခန့်ခွဲပ���'}
          </p>
        </div>
        <Button onClick={() => setShowAddDialog(true)}>
          <Plus className="h-4 w-4 mr-2" />
          {languageToggle === 'en' ? 'Add Product' : 'ထုတ်ကုန်ထည့်ပါ'}
        </Button>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Package className="h-5 w-5" />
            {languageToggle === 'en' ? 'Products List' : 'ထုတ်ကုန်များစာရင်း'}
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-hidden rounded-lg border">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>{languageToggle === 'en' ? 'Product Code' : 'ထုတ်ကုန်ကုဒ်'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Name' : 'အမည်'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Photo' : 'ဓာတ်ပုံ'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Type' : 'အမျိုးအစား'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'UOM' : 'တိုင်းတာယူနစ်'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Weight/Unit' : 'တစ်ယူနစ်အလေးချိန���'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Default Color' : 'မူလရောင်'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Packaging' : 'ထုပ်ပိုးမှု'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Actions' : 'လုပ်ဆောင်ချက်များ'}</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {mockProducts.map((product) => (
                  <TableRow key={product.id}>
                    <TableCell className="font-medium">{product.id}</TableCell>
                    <TableCell>
                      <div>
                        <div className="font-medium">{product.name}</div>
                        <div className="text-xs text-muted-foreground">{product.nameMM}</div>
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="w-12 h-12 bg-slate-200 rounded-lg flex items-center justify-center">
                        <Package className="h-6 w-6 text-slate-500" />
                      </div>
                    </TableCell>
                    <TableCell>
                      <Badge variant="outline">{product.type}</Badge>
                    </TableCell>
                    <TableCell>{product.uom}</TableCell>
                    <TableCell>{product.weight}g</TableCell>
                    <TableCell>
                      <div className="flex items-center gap-2">
                        <div className="w-4 h-4 bg-slate-200 rounded-full border"></div>
                        {product.color}
                      </div>
                    </TableCell>
                    <TableCell>{product.packaging}</TableCell>
                    <TableCell>
                      <div className="flex gap-2">
                        <Button size="sm" variant="outline">
                          <Eye className="h-4 w-4" />
                        </Button>
                        <Button size="sm" variant="outline">
                          <Edit className="h-4 w-4" />
                        </Button>
                        <Button size="sm" variant="outline">
                          <Camera className="h-4 w-4" />
                        </Button>
                      </div>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        </CardContent>
      </Card>
    </div>
  );

  const renderRawMaterials = () => (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <h2 className="text-xl font-semibold">
            {languageToggle === 'en' ? 'Raw Material Registration' : 'ကုန်ကြမ်းမှတ်ပုံတင်မှု'}
          </h2>
          <p className="text-muted-foreground">
            {languageToggle === 'en' ? 'Manage raw material inventory and specifications' : 'ကုန်ကြမ်းစာရင်းနှင့် အသေးစိတ်ဖော်ပြချက်များကို စီမံခန့်ခွဲပါ'}
          </p>
        </div>
        <Button onClick={() => setShowAddDialog(true)}>
          <Plus className="h-4 w-4 mr-2" />
          {languageToggle === 'en' ? 'Add Raw Material' : 'ကုန်ကြမ်းထည့်ပါ'}
        </Button>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <ShoppingCart className="h-5 w-5" />
            {languageToggle === 'en' ? 'Raw Materials List' : 'ကုန်ကြမ်းများစာရင်း'}
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-hidden rounded-lg border">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>{languageToggle === 'en' ? 'Material Code' : 'ပစ္စည်းကုဒ်'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Name' : 'အမည်'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Type' : 'အမျိုးအစား'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'UOM' : 'တိုင်းတာယူနစ်'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Location' : 'တည်နေရာ'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Vendor' : 'ရောင်းချသူ'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Expiry' : 'သက်တမ်းကုန်ဆုံးရက်'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Status' : 'အခြေအနေ'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Actions' : 'လုပ်ဆောင်ချက်များ'}</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {mockRawMaterials.map((material) => (
                  <TableRow key={material.id}>
                    <TableCell className="font-medium">{material.id}</TableCell>
                    <TableCell>
                      <div>
                        <div className="font-medium">{material.name}</div>
                        <div className="text-xs text-muted-foreground">{material.nameMM}</div>
                      </div>
                    </TableCell>
                    <TableCell>
                      <Badge variant="outline">{material.type}</Badge>
                    </TableCell>
                    <TableCell>{material.uom}</TableCell>
                    <TableCell>
                      <div className="flex items-center gap-1">
                        <MapPin className="h-3 w-3 text-muted-foreground" />
                        {material.location}
                      </div>
                    </TableCell>
                    <TableCell>{material.vendor}</TableCell>
                    <TableCell>
                      <div className="text-sm">
                        {material.expiry}
                        <div className="text-xs text-orange-600">6 months left</div>
                      </div>
                    </TableCell>
                    <TableCell>{getStatusBadge(material.status)}</TableCell>
                    <TableCell>
                      <div className="flex gap-2">
                        <Button size="sm" variant="outline">
                          <Eye className="h-4 w-4" />
                        </Button>
                        <Button size="sm" variant="outline">
                          <Edit className="h-4 w-4" />
                        </Button>
                      </div>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        </CardContent>
      </Card>
    </div>
  );

  const renderDevices = () => (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <h2 className="text-xl font-semibold">
            {languageToggle === 'en' ? 'Device Registration' : 'ကိရိယာမှတ်ပုံတင်မှု'}
          </h2>
          <p className="text-muted-foreground">
            {languageToggle === 'en' ? 'Manage RFID terminals, QR printers, and system devices' : 'RFID တာမီနယ်များ၊ QR ပရင်တာများနှင့် စနစ်ကိရိယာများကို စီမံခန့်ခွဲပါ'}
          </p>
        </div>
        <Button onClick={() => setShowAddDialog(true)}>
          <Plus className="h-4 w-4 mr-2" />
          {languageToggle === 'en' ? 'Add Device' : 'ကိရိယာထည့်ပါ'}
        </Button>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Monitor className="h-5 w-5" />
            {languageToggle === 'en' ? 'Devices List' : 'ကိရိယာများစာရင်း'}
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-hidden rounded-lg border">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>{languageToggle === 'en' ? 'Device ID' : 'ကိရိယာအမှတ်'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Device Name' : 'ကိရိယာအမည်'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Type' : 'အမျိုးအစား'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Assigned Machine' : 'သတ်မှတ်ထားသောစက်'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Status' : 'အခြေအနေ'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Last Seen' : 'နောက်ဆုံးတွေ့ရှိမှု'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'IP Address' : 'IP လိပ်စာ'}</TableHead>
                  <TableHead>{languageToggle === 'en' ? 'Actions' : 'လုပ်ဆောင်ချက်များ'}</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {mockDevices.map((device) => (
                  <TableRow key={device.id}>
                    <TableCell className="font-medium">{device.id}</TableCell>
                    <TableCell>
                      <div>
                        <div className="font-medium">{device.name}</div>
                        <div className="text-xs text-muted-foreground">{device.nameMM}</div>
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-2">
                        {device.type === 'RFID Terminal' && <Radio className="h-4 w-4 text-blue-500" />}
                        {device.type === 'QR Printer' && <Printer className="h-4 w-4 text-purple-500" />}
                        {device.type === 'Tablet' && <Smartphone className="h-4 w-4 text-green-500" />}
                        <Badge variant="outline">{device.type}</Badge>
                      </div>
                    </TableCell>
                    <TableCell>{device.assignedMachine}</TableCell>
                    <TableCell>{getStatusBadge(device.status)}</TableCell>
                    <TableCell>
                      <div className="text-sm">
                        {device.lastSeen}
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="font-mono text-sm">{device.ipAddress}</div>
                    </TableCell>
                    <TableCell>
                      <div className="flex gap-2">
                        <Button size="sm" variant="outline">
                          <Eye className="h-4 w-4" />
                        </Button>
                        <Button size="sm" variant="outline">
                          <Edit className="h-4 w-4" />
                        </Button>
                        <Button size="sm" variant="outline">
                          <Zap className="h-4 w-4" />
                        </Button>
                      </div>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        </CardContent>
      </Card>
    </div>
  );

  const renderGenericPage = () => {
    const category = masterDataCategories.find(cat => cat.id === currentPage);
    const Icon = category?.icon || Database;
    
    return (
      <div className="space-y-6">
        <div className="flex justify-between items-center">
          <div>
            <h2 className="text-xl font-semibold">
              {languageToggle === 'en' ? category?.title : category?.titleMM}
            </h2>
            <p className="text-muted-foreground">
              {languageToggle === 'en' ? category?.description : category?.titleMM}
            </p>
          </div>
          <Button onClick={() => setShowAddDialog(true)}>
            <Plus className="h-4 w-4 mr-2" />
            {languageToggle === 'en' ? 'Add New' : 'အသစ်ထည့်ပါ'}
          </Button>
        </div>

        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Icon className="h-5 w-5" />
              {languageToggle === 'en' ? `${category?.title} List` : `${category?.titleMM}စာရင်း`}
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-center py-12">
              <Icon className="h-16 w-16 text-muted-foreground mx-auto mb-4" />
              <h3 className="text-lg font-medium mb-2">
                {languageToggle === 'en' ? 'Coming Soon' : 'မကြာမီ'}
              </h3>
              <p className="text-muted-foreground">
                {languageToggle === 'en' 
                  ? 'This module is under development' 
                  : 'ဤမော်ဂျူးကို ဖွံ့ဖြိုးတိုးတက်နေဆဲဖြစ်သည်'}
              </p>
            </div>
          </CardContent>
        </Card>
      </div>
    );
  };

  const renderCurrentPage = () => {
    switch (currentPage) {
      case 'machines':
        return renderMachines();
      case 'users-roles':
        return renderUsers();
      case 'products':
        return renderProducts();
      case 'raw-materials':
        return renderRawMaterials();
      case 'devices':
        return renderDevices();
      case 'role-matrix':
        return renderRoleMatrix();
      // For other pages, show similar structure with appropriate data
      case 'molds-tooling':
      case 'warehouses':
      case 'shifts-schedules':
      case 'reason-codes':
      case 'customers-hq':
      case 'units-measure':
        return renderGenericPage();
      default:
        return renderDashboard();
    }
  };

  const getPageTitle = () => {
    const category = masterDataCategories.find(cat => cat.id === currentPage);
    if (category) {
      return {
        title: languageToggle === 'en' ? category.title : category.titleMM,
        subtitle: languageToggle === 'en' ? category.description : category.titleMM
      };
    }
    return {
      title: languageToggle === 'en' ? 'Master Data & Registration' : 'အခြေခံဒေတာနှင့်မှတ်ပုံတင်မှု',
      subtitle: languageToggle === 'en' ? 'Manage all factory master data and registrations' : 'စက်ရုံအခြေခံဒေတာများနှင့် မှတ်ပုံတင်မှုများကို စီမံခန့်ခွဲပါ'
    };
  };

  const pageInfo = getPageTitle();

  return (
    <div className="p-6 bg-slate-50 min-h-screen">
      <div className="max-w-7xl mx-auto">
        {/* Header */}
        <div className="mb-6">
          <h1 className="text-2xl font-semibold text-slate-900 mb-2">
            {pageInfo.title}
          </h1>
          <p className="text-slate-600">
            {pageInfo.subtitle}
          </p>
        </div>

        {/* Current Page Content */}
        {renderCurrentPage()}

        {/* Add New Dialog */}
        <Dialog open={showAddDialog} onOpenChange={setShowAddDialog}>
          <DialogContent className="max-w-2xl">
            <DialogHeader>
              <DialogTitle>
                {languageToggle === 'en' ? 'Add New Record' : 'မှတ်တမ်းအသစ်ထည့်ပါ'}
              </DialogTitle>
              <DialogDescription>
                {languageToggle === 'en' 
                  ? 'Fill in the details to create a new master data record.'
                  : 'အခြေခံဒေတာမှတ်တမ်းအသစ်ဖန်တီးရန် အသေးစိတ်အချက်အလက်များကို ဖြည့်စွက်ပါ။'}
              </DialogDescription>
            </DialogHeader>
            <div className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label htmlFor="code">
                    {languageToggle === 'en' ? 'Code' : 'ကုဒ်'}
                  </Label>
                  <Input id="code" placeholder="Enter code..." />
                </div>
                <div>
                  <Label htmlFor="name">
                    {languageToggle === 'en' ? 'Name (English)' : 'အမည် (အင်္ဂလိပ်)'}
                  </Label>
                  <Input id="name" placeholder="Enter name..." />
                </div>
              </div>
              <div>
                <Label htmlFor="nameMM">
                  {languageToggle === 'en' ? 'Name (Myanmar)' : 'အမည် (မြန်မာ)'}
                </Label>
                <Input id="nameMM" placeholder="မြန်မာအမည်ရိုက်ထည့်ပါ..." />
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label htmlFor="type">
                    {languageToggle === 'en' ? 'Type' : 'အမျိုးအစား'}
                  </Label>
                  <Select>
                    <SelectTrigger>
                      <SelectValue placeholder="Select type..." />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="type1">Type 1</SelectItem>
                      <SelectItem value="type2">Type 2</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
                <div>
                  <Label htmlFor="status">
                    {languageToggle === 'en' ? 'Status' : 'အခြေအနေ'}
                  </Label>
                  <Select defaultValue="active">
                    <SelectTrigger>
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="active">🟢 Active</SelectItem>
                      <SelectItem value="pending">🟡 Pending</SelectItem>
                      <SelectItem value="inactive">🔴 Inactive</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
              </div>
              <div className="flex justify-end gap-2">
                <Button variant="outline" onClick={() => setShowAddDialog(false)}>
                  {languageToggle === 'en' ? 'Cancel' : 'ပယ်ဖျက်ပါ'}
                </Button>
                <Button onClick={() => setShowAddDialog(false)}>
                  <Save className="h-4 w-4 mr-2" />
                  {languageToggle === 'en' ? 'Save' : 'သိမ်းဆည်းပါ'}
                </Button>
              </div>
            </div>
          </DialogContent>
        </Dialog>
      </div>
    </div>
  );
}