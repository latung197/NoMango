import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '../ui/card';
import { Button } from '../ui/button';
import { Badge } from '../ui/badge';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription } from '../ui/dialog';
import { Label } from '../ui/label';
import { Input } from '../ui/input';
import { Textarea } from '../ui/textarea';
import { 
  Shield, 
  Users, 
  Settings, 
  Lock, 
  UserCog,
  Settings2,
  Eye,
  Plus,
  Save,
  Check,
  X,
  Edit,
  Trash2,
  Download,
  Upload,
  Zap,
  Factory,
  Calendar,
  Package,
  Wrench,
  BarChart3,
  Cog
} from 'lucide-react';

// Role definitions with badges and colors
const roleDefinitions = [
  { 
    id: 'admin', 
    name: 'Administrator', 
    nameMM: 'စီမံခန့်ခွဲသူ', 
    badge: '👑', 
    color: 'bg-red-100 text-red-800',
    description: 'Full system access'
  },
  { 
    id: 'planner', 
    name: 'Production Planner', 
    nameMM: 'ထုတ်လုပ်မှုစီမံသူ', 
    badge: '📋', 
    color: 'bg-blue-100 text-blue-800',
    description: 'Planning operations'
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
      { id: 'job-monitoring', name: 'Job Monitoring', nameMM: 'အလုပ်စောင့်ကြည့်မှု' }
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
  { id: 'delete', name: 'Delete', nameMM: 'ဖျက်ရန်', icon: Trash2 }
];

export function RoleMatrixManagement() {
  const [languageToggle, setLanguageToggle] = useState<'en' | 'mm'>('en');
  const [selectedRole, setSelectedRole] = useState<string>('admin');
  const [showCreateRoleDialog, setShowCreateRoleDialog] = useState(false);
  const [rolePermissions, setRolePermissions] = useState<Record<string, Record<string, string>>>({});

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
            } else {
              defaultPermissions[role.id][key] = level.id === 'view' ? 'limited' : 'denied';
            }
          });
        });
      });
    });

    setRolePermissions(defaultPermissions);
  }, []);

  const getPermissionIcon = (permission: string) => {
    switch (permission) {
      case 'allowed': return '✅';
      case 'denied': return '❌';
      case 'limited': return '⚠️';
      default: return '❌';
    }
  };

  const getPermissionColor = (permission: string) => {
    switch (permission) {
      case 'allowed': return 'bg-green-100 text-green-800';
      case 'denied': return 'bg-gray-100 text-gray-800';
      case 'limited': return 'bg-yellow-100 text-yellow-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  const togglePermission = (moduleId: string, featureId: string, levelId: string) => {
    const key = `${moduleId}_${featureId}_${levelId}`;
    const currentPermission = rolePermissions[selectedRole]?.[key] || 'denied';
    
    let newPermission: string;
    switch (currentPermission) {
      case 'denied': newPermission = 'limited'; break;
      case 'limited': newPermission = 'allowed'; break;
      case 'allowed': newPermission = 'denied'; break;
      default: newPermission = 'denied';
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

  return (
    <div className="p-6 space-y-6">
      {/* Top Toolbar */}
      <div className="flex justify-between items-center">
        <div>
          <h2 className="text-2xl font-semibold text-slate-900">
            {languageToggle === 'en' ? 'Role Matrix Management' : 'အခန်းကဏ္ဍဇယားစီမံခန့်ခွဲမှု'}
          </h2>
          <p className="text-slate-600 mt-1">
            {languageToggle === 'en' 
              ? 'Configure role-based permissions for system modules and features' 
              : 'စနစ်မော်ဂျူးများနှင့် လုပ်ဆောင်ချက်များအတွက် အခန်းကဏ္ဍအခြေခံခွင့်ပြုချက်များကို စီမံဆောင်ရွက်ပါ'}
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

      {/* Summary Stats */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <Card className="border-0 shadow-sm">
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-blue-100 rounded-lg">
                <Users className="w-4 h-4 text-blue-600" />
              </div>
              <div>
                <div className="text-2xl font-bold text-blue-700">{roleDefinitions.length}</div>
                <div className="text-sm text-slate-600">Total Roles</div>
                <div className="text-xs text-slate-500">စုစုပေါင်းအခန်းကဏ္ဍများ</div>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="border-0 shadow-sm">
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-green-100 rounded-lg">
                <Lock className="w-4 h-4 text-green-600" />
              </div>
              <div>
                <div className="text-2xl font-bold text-green-700">
                  {permissionModules.reduce((sum, m) => sum + m.features.length * permissionLevels.length, 0)}
                </div>
                <div className="text-sm text-slate-600">Permissions</div>
                <div className="text-xs text-slate-500">ခွင့်ပြုချက်များ</div>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="border-0 shadow-sm">
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-orange-100 rounded-lg">
                <Settings className="w-4 h-4 text-orange-600" />
              </div>
              <div>
                <div className="text-2xl font-bold text-orange-700">12</div>
                <div className="text-sm text-slate-600">Active Users</div>
                <div className="text-xs text-slate-500">တက်ကြွအသုံးပြုသူများ</div>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="border-0 shadow-sm">
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-purple-100 rounded-lg">
                <Shield className="w-4 h-4 text-purple-600" />
              </div>
              <div>
                <div className="text-2xl font-bold text-purple-700">3</div>
                <div className="text-sm text-slate-600">Admin Roles</div>
                <div className="text-xs text-slate-500">စီမံခန့်ခွဲသူများ</div>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Role Selection Cards */}
      <Card className="border-0 shadow-sm">
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
                  <div className="text-3xl mb-2">{role.badge}</div>
                  <div className="font-semibold">{languageToggle === 'en' ? role.name : role.nameMM}</div>
                  <div className="text-xs text-slate-500 mt-1">{role.description}</div>
                  <div className="mt-3">
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
          <Card className="border-0 shadow-sm">
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
                    {permissionModules.map((module) => (
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
                        {module.features.map((feature) => (
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
          <Card className="border-0 shadow-sm">
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
                    <span className="text-3xl">{roleDefinitions.find(r => r.id === selectedRole)?.badge}</span>
                    <div>
                      <div className="font-semibold">
                        {languageToggle === 'en' 
                          ? roleDefinitions.find(r => r.id === selectedRole)?.name
                          : roleDefinitions.find(r => r.id === selectedRole)?.nameMM}
                      </div>
                      <div className="text-xs text-slate-500">
                        {roleDefinitions.find(r => r.id === selectedRole)?.description}
                      </div>
                    </div>
                  </div>
                  
                  <div className="text-sm bg-slate-50 p-3 rounded">
                    <div className="font-medium mb-2">
                      {languageToggle === 'en' ? 'Role Summary:' : 'အခန်းကဏ္ဍအကျဉ်းချုပ်:'}
                    </div>
                    <div className="text-xs text-slate-600">
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
          <Card className="border-0 shadow-sm">
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
                : 'သင့်စက်ရုံစနစ်အတွက် အထူးခွင့်ပြုချက်များပါသော စိတ်ကြိုက်အခန်းကဏ္ဍအသစ်တစ်ခုကို သတ်မှတ်ပါ။'}
            </DialogDescription>
          </DialogHeader>
          <div className="space-y-4">
            <div>
              <Label htmlFor="roleName">
                {languageToggle === 'en' ? 'Role Name (English)' : 'အခန်းကဏ္ဍအမည် (အင်္ဂလိပ်)'}
              </Label>
              <Input id="roleName" placeholder="Enter role name" />
            </div>
            <div>
              <Label htmlFor="roleNameMM">
                {languageToggle === 'en' ? 'Role Name (Myanmar)' : 'အခန်းကဏ္ဍအမည် (မြန်မာ)'}
              </Label>
              <Input id="roleNameMM" placeholder="အခန်းကဏ္ဍအမည် ရိုက်ထည့်ပါ" />
            </div>
            <div>
              <Label htmlFor="roleDescription">
                {languageToggle === 'en' ? 'Description' : 'ဖော်ပြချက်'}
              </Label>
              <Textarea id="roleDescription" placeholder="Describe the role responsibilities" />
            </div>
            <div className="flex justify-end gap-3">
              <Button variant="outline" onClick={() => setShowCreateRoleDialog(false)}>
                {languageToggle === 'en' ? 'Cancel' : 'ပယ်ဖျက်ပါ'}
              </Button>
              <Button>
                <Save className="h-4 w-4 mr-2" />
                {languageToggle === 'en' ? 'Create Role' : 'အခန်းကဏ္ဍဖန်တီးပါ'}
              </Button>
            </div>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );
}