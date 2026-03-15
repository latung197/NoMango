import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '../ui/card';
import { Button } from '../ui/button';
import { Input } from '../ui/input';
import { Label } from '../ui/label';
import { Badge } from '../ui/badge';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../ui/table';
import { Sheet, SheetContent, SheetDescription, SheetHeader, SheetTitle } from '../ui/sheet';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../ui/select';
import { Switch } from '../ui/switch';
import { Avatar, AvatarFallback, AvatarImage } from '../ui/avatar';
import { 
  Search,
  Filter,
  Download,
  Plus,
  Edit,
  Save,
  UserPlus,
  Users,
  Upload,
  UserCheck,
  Shield,
  FileSpreadsheet
} from 'lucide-react';

// Mock data for users
const mockUsers = [
  {
    id: 1,
    empId: 'OP-101',
    nameEn: 'Ko Aung Kyaw',
    nameMM: 'ကိုအောင်ကျော်',
    rfidCode: 'RFID-001',
    role: 'Operator',
    department: 'Production Line A',
    photo: null,
    username: 'aung.kyaw',
    status: 'Active',
    createdBy: 'Admin',
    createdOn: '2024-01-15 09:30',
    updatedBy: 'HR Manager',
    updatedOn: '2024-03-10 14:20',
    version: 2
  },
  {
    id: 2,
    empId: 'SV-201',
    nameEn: 'Ma Thin Thin Aye',
    nameMM: 'မသင်းသင်းအေး',
    rfidCode: 'RFID-002',
    role: 'Supervisor',
    department: 'Production Line B',
    photo: null,
    username: 'thin.aye',
    status: 'Active',
    createdBy: 'Admin',
    createdOn: '2024-02-01 10:15',
    updatedBy: 'HR Manager',
    updatedOn: '2024-02-01 10:15',
    version: 1
  },
  {
    id: 3,
    empId: 'QC-301',
    nameEn: 'Ko Zaw Min',
    nameMM: 'ကိုဇော်မင်း',
    rfidCode: 'RFID-003',
    role: 'QC Inspector',
    department: 'Quality Control',
    photo: null,
    username: 'zaw.min',
    status: 'Active',
    createdBy: 'Admin',
    createdOn: '2024-01-20 11:45',
    updatedBy: 'QC Manager',
    updatedOn: '2024-03-05 16:30',
    version: 1
  },
  {
    id: 4,
    empId: 'EN-401',
    nameEn: 'Ma Moe Moe',
    nameMM: 'မမိုးမိုး',
    rfidCode: 'RFID-004',
    role: 'Engineer',
    department: 'Engineering',
    photo: null,
    username: 'moe.moe',
    status: 'Inactive',
    createdBy: 'Admin',
    createdOn: '2024-02-10 14:00',
    updatedBy: 'Admin',
    updatedOn: '2024-03-12 09:15',
    version: 2
  }
];

const roles = ['Admin', 'Planner', 'Operator', 'Supervisor', 'QC Inspector', 'Engineer', 'Maintenance', 'HR Manager'];
const departments = ['Production Line A', 'Production Line B', 'Production Line C', 'Quality Control', 'Engineering', 'Maintenance', 'HR', 'Planning', 'Warehouse'];

export function UserRegistration() {
  const [searchTerm, setSearchTerm] = useState('');
  const [filterRole, setFilterRole] = useState('');
  const [filterStatus, setFilterStatus] = useState('');
  const [showDrawer, setShowDrawer] = useState(false);
  const [editingItem, setEditingItem] = useState<any>(null);
  const [formData, setFormData] = useState({
    empId: '',
    nameEn: '',
    nameMM: '',
    rfidCode: '',
    role: '',
    department: '',
    photo: null as File | null,
    username: '',
    status: true
  });

  const handleCreate = () => {
    setEditingItem(null);
    setFormData({
      empId: '',
      nameEn: '',
      nameMM: '',
      rfidCode: '',
      role: '',
      department: '',
      photo: null,
      username: '',
      status: true
    });
    setShowDrawer(true);
  };

  const handleEdit = (item: any) => {
    setEditingItem(item);
    setFormData({
      empId: item.empId,
      nameEn: item.nameEn,
      nameMM: item.nameMM || '',
      rfidCode: item.rfidCode,
      role: item.role,
      department: item.department,
      photo: null,
      username: item.username,
      status: item.status === 'Active'
    });
    setShowDrawer(true);
  };

  const handleSave = () => {
    console.log('Saving user:', formData);
    setShowDrawer(false);
  };

  const handleSaveAndNew = () => {
    console.log('Saving user:', formData);
    setFormData({
      empId: '',
      nameEn: '',
      nameMM: '',
      rfidCode: '',
      role: '',
      department: '',
      photo: null,
      username: '',
      status: true
    });
  };

  const handlePhotoUpload = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      setFormData(prev => ({ ...prev, photo: file }));
    }
  };

  const filteredUsers = mockUsers.filter(user =>
    user.nameEn.toLowerCase().includes(searchTerm.toLowerCase()) ||
    user.empId.toLowerCase().includes(searchTerm.toLowerCase()) ||
    user.username.toLowerCase().includes(searchTerm.toLowerCase()) ||
    user.department.toLowerCase().includes(searchTerm.toLowerCase())
  ).filter(user =>
    filterRole === '' || filterRole === 'all' || user.role === filterRole
  ).filter(user =>
    filterStatus === '' || filterStatus === 'all' || user.status === filterStatus
  );

  const exportToCSV = () => {
    console.log('Exporting to CSV...');
  };

  const exportToXLSX = () => {
    console.log('Exporting to XLSX...');
  };

  return (
    <div className="p-6 space-y-6">
      {/* Header */}
      <div>
        <h2 className="text-2xl font-semibold text-slate-900">
          User Registration | အသုံးပြုသူမှတ်ပုံတင်မှု
        </h2>
        <p className="text-slate-600 mt-1">
          Register and manage system users with roles and RFID access
        </p>
        <p className="text-sm text-slate-500">
          အခန်းကဏ္ဍများနှင့် RFID ဝင်ရောက်ခွင့်ဖြင့် စနစ်အသုံးပြုသူများကို မှတ်ပုံတင်ပြီး စီမံခန့်ခွဲခြင်း
        </p>
      </div>

      {/* Toolbar */}
      <div className="flex justify-between items-center">
        <div className="flex items-center gap-4">
          {/* Search */}
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-slate-400 h-4 w-4" />
            <Input
              placeholder="Search by name, EmpID, username, or department..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="pl-10 w-80"
            />
          </div>

          {/* Filters */}
          <Select value={filterRole} onValueChange={setFilterRole}>
            <SelectTrigger className="w-32">
              <SelectValue placeholder="Role" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">All Roles</SelectItem>
              {roles.map((role) => (
                <SelectItem key={role} value={role}>{role}</SelectItem>
              ))}
            </SelectContent>
          </Select>

          <Select value={filterStatus} onValueChange={setFilterStatus}>
            <SelectTrigger className="w-32">
              <SelectValue placeholder="Status" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">All Status</SelectItem>
              <SelectItem value="Active">Active</SelectItem>
              <SelectItem value="Inactive">Inactive</SelectItem>
            </SelectContent>
          </Select>

          <Button variant="outline" className="gap-2">
            <Filter className="h-4 w-4" />
            More Filters
          </Button>
        </div>

        <div className="flex items-center gap-3">
          <Button variant="outline" onClick={exportToCSV} className="gap-2">
            <Download className="h-4 w-4" />
            CSV
          </Button>
          <Button variant="outline" onClick={exportToXLSX} className="gap-2">
            <FileSpreadsheet className="h-4 w-4" />
            XLSX
          </Button>
          <Button onClick={handleCreate} className="gap-2">
            <Plus className="h-4 w-4" />
            Add User | အသုံးပြုသူထည့်သွင်းရန်
          </Button>
        </div>
      </div>

      {/* Main Table */}
      <Card className="border-0 shadow-sm">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Users className="h-5 w-5 text-green-600" />
            Users | အသုံးပြုသူများ
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>EmpID | ဝန်ထမ်းအမှတ်</TableHead>
                  <TableHead>Name | အမည်</TableHead>
                  <TableHead>RFID Code | RFID ကုဒ်</TableHead>
                  <TableHead>Role | အခန်းကဏ္ဍ</TableHead>
                  <TableHead>Department | ဌာန</TableHead>
                  <TableHead>Username | အသုံးပြုသူအမည်</TableHead>
                  <TableHead>Status | အခြေအနေ</TableHead>
                  <TableHead>Actions | လုပ်ဆောင်ချက်များ</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {filteredUsers.map((user) => (
                  <TableRow key={user.id}>
                    <TableCell>
                      <Badge variant="outline" className="font-mono">
                        {user.empId}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-3">
                        <Avatar className="h-8 w-8">
                          <AvatarImage src={user.photo || undefined} />
                          <AvatarFallback className="bg-blue-100 text-blue-700">
                            {user.nameEn.split(' ').map(n => n[0]).join('').slice(0, 2)}
                          </AvatarFallback>
                        </Avatar>
                        <div>
                          <div className="font-medium">{user.nameEn}</div>
                          {user.nameMM && (
                            <div className="text-sm text-slate-500">{user.nameMM}</div>
                          )}
                        </div>
                      </div>
                    </TableCell>
                    <TableCell>
                      <Badge className="bg-purple-100 text-purple-800 font-mono">
                        {user.rfidCode}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <Badge className={
                        user.role === 'Admin' ? 'bg-red-100 text-red-800' :
                        user.role === 'Supervisor' ? 'bg-blue-100 text-blue-800' :
                        user.role === 'Engineer' ? 'bg-green-100 text-green-800' :
                        user.role === 'QC Inspector' ? 'bg-orange-100 text-orange-800' :
                        'bg-slate-100 text-slate-800'
                      }>
                        <Shield className="h-3 w-3 mr-1" />
                        {user.role}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <span className="text-slate-600">{user.department}</span>
                    </TableCell>
                    <TableCell>
                      <code className="bg-slate-100 px-2 py-1 rounded text-sm">
                        {user.username}
                      </code>
                    </TableCell>
                    <TableCell>
                      <Badge className={
                        user.status === 'Active' 
                          ? 'bg-green-100 text-green-800' 
                          : 'bg-red-100 text-red-800'
                      }>
                        {user.status === 'Active' ? (
                          <UserCheck className="h-3 w-3 mr-1" />
                        ) : (
                          <UserCheck className="h-3 w-3 mr-1" />
                        )}
                        {user.status}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => handleEdit(user)}
                        className="gap-1"
                      >
                        <Edit className="h-3 w-3" />
                        Edit
                      </Button>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>

          {/* Pagination */}
          <div className="flex items-center justify-between space-x-2 py-4">
            <div className="text-sm text-slate-600">
              Showing {filteredUsers.length} of {mockUsers.length} users
            </div>
            <div className="flex items-center gap-2">
              <Button variant="outline" size="sm" disabled>Previous</Button>
              <Button variant="outline" size="sm" className="bg-blue-50">1</Button>
              <Button variant="outline" size="sm" disabled>Next</Button>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Right-side Drawer */}
      <Sheet open={showDrawer} onOpenChange={setShowDrawer}>
        <SheetContent className="w-[600px] sm:max-w-[600px] overflow-y-auto">
          <SheetHeader>
            <SheetTitle className="flex items-center gap-2">
              <UserPlus className="h-5 w-5 text-green-600" />
              {editingItem ? 'Edit User' : 'Create New User'}
            </SheetTitle>
            <SheetDescription>
              {editingItem ? 'Update user information and access rights' : 'Add new system user with appropriate role and access'}
            </SheetDescription>
          </SheetHeader>

          <div className="space-y-6 py-6">
            {/* Personal Information */}
            <div className="space-y-4">
              <h4 className="font-medium text-slate-900">Personal Information | ကိုယ်ရေးကိုယ်တာအချက်အလက်</h4>
              
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label htmlFor="empId">EmpID * | ဝန်ထမ်းအမှတ် *</Label>
                  <Input
                    id="empId"
                    value={formData.empId}
                    onChange={(e) => setFormData(prev => ({ ...prev, empId: e.target.value }))}
                    placeholder="OP-101"
                    className="mt-1"
                  />
                </div>

                <div>
                  <Label htmlFor="rfidCode">RFID Code | RFID ကုဒ်</Label>
                  <Input
                    id="rfidCode"
                    value={formData.rfidCode}
                    onChange={(e) => setFormData(prev => ({ ...prev, rfidCode: e.target.value }))}
                    placeholder="RFID-001"
                    className="mt-1"
                  />
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label htmlFor="nameEn">Name (EN) * | အမည် (အင်္ဂလိပ်) *</Label>
                  <Input
                    id="nameEn"
                    value={formData.nameEn}
                    onChange={(e) => setFormData(prev => ({ ...prev, nameEn: e.target.value }))}
                    placeholder="Ko Aung Kyaw"
                    className="mt-1"
                  />
                </div>

                <div>
                  <Label htmlFor="nameMM">Name (MM) | အမည် (မြန်မာ)</Label>
                  <Input
                    id="nameMM"
                    value={formData.nameMM}
                    onChange={(e) => setFormData(prev => ({ ...prev, nameMM: e.target.value }))}
                    placeholder="ကိုအောင်ကျော်"
                    className="mt-1"
                  />
                </div>
              </div>

              {/* Photo Upload */}
              <div>
                <Label htmlFor="photo">Photo | ဓာတ်ပုံ</Label>
                <div className="mt-1 flex items-center gap-4">
                  <div className="flex-1">
                    <Input
                      id="photo"
                      type="file"
                      accept="image/*"
                      onChange={handlePhotoUpload}
                      className="cursor-pointer"
                    />
                    <div className="text-xs text-slate-500 mt-1">
                      Upload JPG, PNG (max 2MB)
                    </div>
                  </div>
                  <Button variant="outline" size="sm" className="gap-1">
                    <Upload className="h-3 w-3" />
                    Browse
                  </Button>
                </div>
              </div>
            </div>

            {/* System Access */}
            <div className="space-y-4">
              <h4 className="font-medium text-slate-900">System Access | စနစ်ဝင်ရောက်ခွင့်</h4>
              
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label htmlFor="role">Role * | အခန်းကဏ္ဍ *</Label>
                  <Select value={formData.role} onValueChange={(value) => setFormData(prev => ({ ...prev, role: value }))}>
                    <SelectTrigger className="mt-1">
                      <SelectValue placeholder="Select role..." />
                    </SelectTrigger>
                    <SelectContent>
                      {roles.map((role) => (
                        <SelectItem key={role} value={role}>
                          <div className="flex items-center gap-2">
                            <Shield className="h-3 w-3" />
                            {role}
                          </div>
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>

                <div>
                  <Label htmlFor="department">Department | ဌာန</Label>
                  <Select value={formData.department} onValueChange={(value) => setFormData(prev => ({ ...prev, department: value }))}>
                    <SelectTrigger className="mt-1">
                      <SelectValue placeholder="Select department..." />
                    </SelectTrigger>
                    <SelectContent>
                      {departments.map((dept) => (
                        <SelectItem key={dept} value={dept}>{dept}</SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
              </div>

              <div>
                <Label htmlFor="username">Username/Login | အသုံးပြုသူအမည်</Label>
                <Input
                  id="username"
                  value={formData.username}
                  onChange={(e) => setFormData(prev => ({ ...prev, username: e.target.value }))}
                  placeholder="aung.kyaw"
                  className="mt-1"
                />
                <div className="text-xs text-slate-500 mt-1">
                  Used for system login
                </div>
              </div>
            </div>

            {/* Status */}
            <div className="space-y-4">
              <div className="flex items-center justify-between">
                <Label htmlFor="status">Status | အခြေအနေ</Label>
                <div className="flex items-center gap-2">
                  <span className={formData.status ? 'text-green-600' : 'text-red-600'}>
                    {formData.status ? 'Active' : 'Inactive'}
                  </span>
                  <Switch
                    id="status"
                    checked={formData.status}
                    onCheckedChange={(checked) => setFormData(prev => ({ ...prev, status: checked }))}
                  />
                </div>
              </div>
            </div>

            {/* Audit Trail */}
            {editingItem && (
              <div className="p-4 bg-slate-50 border rounded-lg">
                <h4 className="font-medium text-slate-900 mb-2">Audit Trail | စာရင်းစစ်မှတ်တမ်း</h4>
                <div className="text-sm text-slate-600 space-y-1">
                  <div>Created by: <strong>{editingItem.createdBy}</strong> on {editingItem.createdOn}</div>
                  <div>Updated by: <strong>{editingItem.updatedBy}</strong> on {editingItem.updatedOn}</div>
                  <div>Version: <strong>{editingItem.version}</strong></div>
                </div>
              </div>
            )}

            {/* Action Buttons */}
            <div className="flex justify-end gap-3 pt-4 border-t">
              <Button variant="outline" onClick={() => setShowDrawer(false)}>
                Cancel | ပယ်ဖျက်ပါ
              </Button>
              <Button variant="outline" onClick={handleSaveAndNew}>
                <Save className="h-4 w-4 mr-2" />
                Save & New | သိမ်းဆည်းပြီးအသစ်
              </Button>
              <Button onClick={handleSave}>
                <Save className="h-4 w-4 mr-2" />
                Save | သိမ်းဆည်းပါ
              </Button>
            </div>
          </div>
        </SheetContent>
      </Sheet>
    </div>
  );
}