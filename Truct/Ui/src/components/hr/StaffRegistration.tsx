import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '../ui/card';
import { Button } from '../ui/button';
import { Badge } from '../ui/badge';
import { Input } from '../ui/input';
import { Label } from '../ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../ui/select';
import { RadioGroup, RadioGroupItem } from '../ui/radio-group';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../ui/table';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogTrigger } from '../ui/dialog';
import { Avatar, AvatarFallback, AvatarImage } from '../ui/avatar';
import { Separator } from '../ui/separator';
import { toast } from 'sonner';
import { 
  UserPlus,
  Search,
  Edit,
  Trash2,
  Download,
  Upload,
  Scan,
  Camera,
  CheckCircle,
  XCircle,
  Filter
} from 'lucide-react';

// Mock data for staff
const mockStaffData = [
  {
    id: 'EMP-001',
    photo: null,
    nameEn: 'Ko Thant',
    nameMm: 'ကိုသန့်',
    employeeId: 'EMP-001',
    rfidId: 'RF001',
    department: 'Production',
    departmentMm: 'ထုတ်လုပ်မှု',
    role: 'Operator',
    roleMm: 'အော်ပရေတာ',
    workType: 'shift',
    workTypeMm: 'အလုပ်ပတ်',
    shiftSetup: 'Morning Shift',
    expectedHours: null,
    joinDate: '2023-01-15',
    status: 'active',
    statusMm: 'တက်ကြွ'
  },
  {
    id: 'EMP-002',
    photo: null,
    nameEn: 'Ma Hla',
    nameMm: 'မလှ',
    employeeId: 'EMP-002',
    rfidId: 'RF002',
    department: 'Production',
    departmentMm: 'ထုတ်လုပ်မှု',
    role: 'Operator',
    roleMm: 'အော်ပရေတာ',
    workType: 'shift',
    workTypeMm: 'အလုပ်ပတ်',
    shiftSetup: 'Evening Shift',
    expectedHours: null,
    joinDate: '2023-02-20',
    status: 'active',
    statusMm: 'တက်ကြွ'
  },
  {
    id: 'EMP-003',
    photo: null,
    nameEn: 'Ko Zaw',
    nameMm: 'ကိုဇော်',
    employeeId: 'EMP-003',
    rfidId: 'RF003',
    department: 'Engineering',
    departmentMm: 'အင်ဂျင်နီယာ',
    role: 'Engineer',
    roleMm: 'အင်ဂျင်နီယာ',
    workType: 'flexible',
    workTypeMm: 'လိုက်လျောညီထွေ',
    shiftSetup: null,
    expectedHours: 40,
    joinDate: '2022-11-10',
    status: 'active',
    statusMm: 'တက်ကြွ'
  },
  {
    id: 'EMP-004',
    photo: null,
    nameEn: 'Ma Su',
    nameMm: 'မစု',
    employeeId: 'EMP-004',
    rfidId: 'RF004',
    department: 'Quality Control',
    departmentMm: 'အရည်အသွေးထိန်းချုပ်မှု',
    role: 'QC Inspector',
    roleMm: 'QC စစ်ဆေးသူ',
    workType: 'flexible',
    workTypeMm: 'လိုက်လျောညီထွေ',
    shiftSetup: null,
    expectedHours: 35,
    joinDate: '2023-03-05',
    status: 'active',
    statusMm: 'တက်ကြွ'
  },
  {
    id: 'EMP-005',
    photo: null,
    nameEn: 'Ko Aung',
    nameMm: 'ကိုအောင်',
    employeeId: 'EMP-005',
    rfidId: 'RF005',
    department: 'Production',
    departmentMm: 'ထုတ်လုပ်မှု',
    role: 'Supervisor',
    roleMm: 'ကြီးကြပ်သူ',
    workType: 'shift',
    workTypeMm: 'အလုပ်ပတ်',
    shiftSetup: 'Morning Shift',
    expectedHours: null,
    joinDate: '2022-08-15',
    status: 'inactive',
    statusMm: 'မတက်ကြွ'
  }
];

// Departments and Roles data
const departments = [
  { id: 'production', name: 'Production', nameMm: 'ထုတ်လုပ်မှု' },
  { id: 'engineering', name: 'Engineering', nameMm: 'အင်ဂျင်နီယာ' },
  { id: 'quality-control', name: 'Quality Control', nameMm: 'အရည်အသွေးထိန်းချုပ်မှု' },
  { id: 'administration', name: 'Administration', nameMm: 'စီမံခန့်ခွဲမှု' },
  { id: 'finance', name: 'Finance', nameMm: 'ဘဏ္ဍာရေး' },
  { id: 'maintenance', name: 'Maintenance', nameMm: 'ပြုပြင်ထိန်းသိမ်းမှု' }
];

const roles = [
  { id: 'operator', name: 'Operator', nameMm: 'အော်ပရေတာ' },
  { id: 'supervisor', name: 'Supervisor', nameMm: 'ကြီးကြပ်သူ' },
  { id: 'technician', name: 'Technician', nameMm: 'နည်းပညာသမား' },
  { id: 'engineer', name: 'Engineer', nameMm: 'အင်ဂျင်နီယာ' },
  { id: 'qc-inspector', name: 'QC Inspector', nameMm: 'QC စစ်ဆေးသူ' },
  { id: 'admin', name: 'Admin', nameMm: 'စီမံခန့်ခွဲသူ' },
  { id: 'finance-staff', name: 'Finance Staff', nameMm: 'ဘဏ္ဍာရေးဝန်ထမ်း' }
];

const shifts = [
  { id: 'morning', name: 'Morning Shift (08:00-17:00)', nameMm: 'နံနက်ပတ် (၀၈:၀၀-၁၇:၀၀)' },
  { id: 'evening', name: 'Evening Shift (17:00-02:00)', nameMm: 'ညနေပတ် (၁၇:၀၀-၀၂:၀၀)' },
  { id: 'night', name: 'Night Shift (02:00-08:00)', nameMm: 'ညပတ် (၀၂:၀၀-၀၈:၀၀)' }
];

export function StaffRegistration() {
  const [staffData, setStaffData] = useState(mockStaffData);
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [editingStaff, setEditingStaff] = useState<any>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [filterDepartment, setFilterDepartment] = useState('all');
  const [filterStatus, setFilterStatus] = useState('all');
  const [rfidScanning, setRfidScanning] = useState(false);

  // Form state
  const [formData, setFormData] = useState({
    photo: null,
    nameEn: '',
    nameMm: '',
    employeeId: '',
    rfidId: '',
    department: '',
    role: '',
    workType: 'shift',
    shiftSetup: '',
    expectedHours: '',
    joinDate: '',
    status: 'active'
  });

  const resetForm = () => {
    setFormData({
      photo: null,
      nameEn: '',
      nameMm: '',
      employeeId: '',
      rfidId: '',
      department: '',
      role: '',
      workType: 'shift',
      shiftSetup: '',
      expectedHours: '',
      joinDate: '',
      status: 'active'
    });
    setEditingStaff(null);
  };

  const handleOpenForm = (staff = null) => {
    if (staff) {
      setEditingStaff(staff);
      setFormData({
        photo: staff.photo,
        nameEn: staff.nameEn,
        nameMm: staff.nameMm,
        employeeId: staff.employeeId,
        rfidId: staff.rfidId,
        department: staff.department.toLowerCase().replace(/\s+/g, '-'),
        role: staff.role.toLowerCase().replace(/\s+/g, '-'),
        workType: staff.workType,
        shiftSetup: staff.shiftSetup || '',
        expectedHours: staff.expectedHours?.toString() || '',
        joinDate: staff.joinDate,
        status: staff.status
      });
    } else {
      resetForm();
    }
    setIsFormOpen(true);
  };

  const handleSubmit = () => {
    // Validate required fields
    if (!formData.nameEn || !formData.employeeId || !formData.rfidId || !formData.department || !formData.role) {
      toast.error('Please fill in all required fields');
      return;
    }

    // Validate work type specific fields
    if (formData.workType === 'shift' && !formData.shiftSetup) {
      toast.error('Please select a shift for shift-based work type');
      return;
    }

    if (formData.workType === 'flexible' && !formData.expectedHours) {
      toast.error('Please enter expected hours for flexible work type');
      return;
    }

    // Get department and role names
    const dept = departments.find(d => d.id === formData.department);
    const roleData = roles.find(r => r.id === formData.role);
    const shift = shifts.find(s => s.id === formData.shiftSetup);

    const newStaff = {
      id: editingStaff ? editingStaff.id : `EMP-${(staffData.length + 1).toString().padStart(3, '0')}`,
      photo: formData.photo,
      nameEn: formData.nameEn,
      nameMm: formData.nameMm,
      employeeId: formData.employeeId,
      rfidId: formData.rfidId,
      department: dept?.name || '',
      departmentMm: dept?.nameMm || '',
      role: roleData?.name || '',
      roleMm: roleData?.nameMm || '',
      workType: formData.workType,
      workTypeMm: formData.workType === 'shift' ? 'အလုပ်ပတ်' : 'လိုက်လျောညီထွေ',
      shiftSetup: formData.workType === 'shift' ? (shift?.name || '') : null,
      expectedHours: formData.workType === 'flexible' ? parseInt(formData.expectedHours) : null,
      joinDate: formData.joinDate,
      status: formData.status,
      statusMm: formData.status === 'active' ? 'တက်ကြွ' : 'မတက်ကြွ'
    };

    if (editingStaff) {
      setStaffData(prev => prev.map(staff => staff.id === editingStaff.id ? newStaff : staff));
      toast.success('Staff information updated successfully');
    } else {
      setStaffData(prev => [...prev, newStaff]);
      toast.success('New staff registered successfully');
    }

    setIsFormOpen(false);
    resetForm();
  };

  const handleDelete = (staffId: string) => {
    setStaffData(prev => prev.filter(staff => staff.id !== staffId));
    toast.success('Staff deleted successfully');
  };

  const handleRfidScan = () => {
    setRfidScanning(true);
    // Simulate RFID scan
    setTimeout(() => {
      const newRfidId = `RF${Math.floor(Math.random() * 1000).toString().padStart(3, '0')}`;
      setFormData(prev => ({ ...prev, rfidId: newRfidId }));
      setRfidScanning(false);
      toast.success(`RFID scanned successfully: ${newRfidId}`);
    }, 2000);
  };

  // Filter staff data
  const filteredStaff = staffData.filter(staff => {
    const matchesSearch = 
      staff.nameEn.toLowerCase().includes(searchTerm.toLowerCase()) ||
      staff.nameMm.includes(searchTerm) ||
      staff.employeeId.toLowerCase().includes(searchTerm.toLowerCase()) ||
      staff.rfidId.toLowerCase().includes(searchTerm.toLowerCase());
    
    const matchesDepartment = filterDepartment === 'all' || staff.department === filterDepartment;
    const matchesStatus = filterStatus === 'all' || staff.status === filterStatus;
    
    return matchesSearch && matchesDepartment && matchesStatus;
  });

  return (
    <div className="p-6 bg-slate-50 min-h-screen">
      <div className="max-w-7xl mx-auto space-y-6">
        {/* Header */}
        <div className="flex items-center justify-between mb-6">
          <div className="flex items-center gap-3">
            <div className="p-2 bg-blue-100 rounded-lg">
              <UserPlus className="h-6 w-6 text-blue-600" />
            </div>
            <div>
              <h1 className="text-3xl font-semibold text-slate-900">
                Staff Registration | ဝန်ထမ်းမှတ်ပုံတင်
              </h1>
              <p className="text-slate-600 mt-1">
                Register and manage employee information with RFID integration
              </p>
              <p className="text-sm text-slate-500">
                RFID ပေါင်းစပ်မှုနှင့်အတူ ဝန်ထမ်းအချက်အလက်များ မှတ်ပုံတင်ပြီး စီမံခန့်ခွဲခြင်း
              </p>
            </div>
          </div>
          
          <div className="flex items-center gap-3">
            <Button variant="outline" className="gap-2">
              <Download className="h-4 w-4" />
              Export Excel
            </Button>
            <Button variant="outline" className="gap-2">
              <Upload className="h-4 w-4" />
              Import
            </Button>
            <Button onClick={() => handleOpenForm()} className="gap-2 bg-blue-600 hover:bg-blue-700">
              <UserPlus className="h-4 w-4" />
              Add New Staff | ဝန်ထမ်းအသစ်ထည့်ရန်
            </Button>
          </div>
        </div>

        {/* Search and Filters */}
        <Card className="border-0 shadow-sm">
          <CardContent className="p-4">
            <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
              <div>
                <Label className="text-sm">Search | ရှာဖွေရန်</Label>
                <div className="relative mt-1">
                  <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-slate-400" />
                  <Input
                    placeholder="Name, ID, RFID..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    className="pl-10"
                  />
                </div>
              </div>
              
              <div>
                <Label className="text-sm">Department | ဌာန</Label>
                <Select value={filterDepartment} onValueChange={setFilterDepartment}>
                  <SelectTrigger className="mt-1">
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">All Departments</SelectItem>
                    {departments.map(dept => (
                      <SelectItem key={dept.id} value={dept.name}>
                        {dept.name} | {dept.nameMm}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              
              <div>
                <Label className="text-sm">Status | အခြေအနေ</Label>
                <Select value={filterStatus} onValueChange={setFilterStatus}>
                  <SelectTrigger className="mt-1">
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">All Status</SelectItem>
                    <SelectItem value="active">Active | တက်က���ွ</SelectItem>
                    <SelectItem value="inactive">Inactive | မတက်ကြွ</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              
              <div className="flex items-end">
                <Button variant="outline" className="gap-2 w-full">
                  <Filter className="h-4 w-4" />
                  Clear Filters
                </Button>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Staff Table */}
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center justify-between">
              <div className="flex items-center gap-2">
                <UserPlus className="h-5 w-5 text-blue-600" />
                <div>
                  <div>Staff Directory ({filteredStaff.length})</div>
                  <div className="text-sm text-slate-500">ဝန်ထမ်းလမ်းညွှန် ({filteredStaff.length})</div>
                </div>
              </div>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Photo | ဓာတ်ပုံ</TableHead>
                    <TableHead>Name | အမည်</TableHead>
                    <TableHead>Employee ID</TableHead>
                    <TableHead>RFID ID</TableHead>
                    <TableHead>Department | ဌာန</TableHead>
                    <TableHead>Role | ရာထူး</TableHead>
                    <TableHead>Work Type | အလုပ်အမျိုးအစား</TableHead>
                    <TableHead>Shift/Flexible Setup</TableHead>
                    <TableHead>Status | အခြေအနေ</TableHead>
                    <TableHead>Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {filteredStaff.map((staff) => (
                    <TableRow key={staff.id}>
                      <TableCell>
                        <Avatar className="h-10 w-10">
                          <AvatarImage src={staff.photo} />
                          <AvatarFallback className="text-sm">
                            {staff.nameEn.split(' ').map(n => n[0]).join('')}
                          </AvatarFallback>
                        </Avatar>
                      </TableCell>
                      <TableCell>
                        <div>
                          <div className="font-medium">{staff.nameEn}</div>
                          <div className="text-sm text-slate-500">{staff.nameMm}</div>
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="font-mono text-sm">{staff.employeeId}</div>
                      </TableCell>
                      <TableCell>
                        <div className="font-mono text-sm">{staff.rfidId}</div>
                      </TableCell>
                      <TableCell>
                        <div>
                          <div className="font-medium">{staff.department}</div>
                          <div className="text-sm text-slate-500">{staff.departmentMm}</div>
                        </div>
                      </TableCell>
                      <TableCell>
                        <div>
                          <div className="font-medium">{staff.role}</div>
                          <div className="text-sm text-slate-500">{staff.roleMm}</div>
                        </div>
                      </TableCell>
                      <TableCell>
                        <Badge className={staff.workType === 'shift' ? 'bg-blue-100 text-blue-800' : 'bg-purple-100 text-purple-800'}>
                          {staff.workType === 'shift' ? 'Shift-based' : 'Flexible'}
                        </Badge>
                        <div className="text-xs text-slate-500 mt-1">{staff.workTypeMm}</div>
                      </TableCell>
                      <TableCell>
                        {staff.workType === 'shift' ? (
                          <div className="text-sm">{staff.shiftSetup}</div>
                        ) : (
                          <div className="text-sm">{staff.expectedHours} hrs/week</div>
                        )}
                      </TableCell>
                      <TableCell>
                        <Badge className={staff.status === 'active' ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}>
                          {staff.status === 'active' ? (
                            <>
                              <CheckCircle className="h-3 w-3 mr-1" />
                              Active | တက်ကြွ
                            </>
                          ) : (
                            <>
                              <XCircle className="h-3 w-3 mr-1" />
                              Inactive | မတက်ကြွ
                            </>
                          )}
                        </Badge>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-2">
                          <Button
                            variant="outline"
                            size="sm"
                            onClick={() => handleOpenForm(staff)}
                            className="gap-1"
                          >
                            <Edit className="h-3 w-3" />
                            Edit
                          </Button>
                          <Button
                            variant="outline"
                            size="sm"
                            onClick={() => handleDelete(staff.id)}
                            className="gap-1 text-red-600 hover:text-red-700"
                          >
                            <Trash2 className="h-3 w-3" />
                            Delete
                          </Button>
                        </div>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
              
              {filteredStaff.length === 0 && (
                <div className="text-center py-8 text-slate-500">
                  <UserPlus className="h-12 w-12 mx-auto mb-4 text-slate-300" />
                  <p>No staff found</p>
                  <p className="text-sm">ဝန်ထမ်းများမတွေ့ရှိပါ</p>
                </div>
              )}
            </div>
          </CardContent>
        </Card>

        {/* Registration Form Dialog */}
        <Dialog open={isFormOpen} onOpenChange={setIsFormOpen}>
          <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
            <DialogHeader>
              <DialogTitle className="flex items-center gap-2">
                <UserPlus className="h-5 w-5 text-blue-600" />
                {editingStaff ? 'Edit Staff Information' : 'Register New Staff'}
              </DialogTitle>
              <DialogDescription>
                {editingStaff ? 'Update staff information and work setup' : 'Add new employee with RFID integration and work type configuration'}
              </DialogDescription>
            </DialogHeader>
            
            <div className="space-y-6 py-4">
              {/* Photo Upload */}
              <div>
                <Label>Employee Photo | ဝန်ထမ်းဓာတ်ပုံ</Label>
                <div className="mt-2 flex items-center gap-4">
                  <Avatar className="h-16 w-16">
                    <AvatarImage src={formData.photo} />
                    <AvatarFallback>
                      {formData.nameEn ? formData.nameEn.split(' ').map(n => n[0]).join('') : 'N/A'}
                    </AvatarFallback>
                  </Avatar>
                  <Button variant="outline" className="gap-2">
                    <Camera className="h-4 w-4" />
                    Upload Photo
                  </Button>
                </div>
              </div>

              {/* Basic Information */}
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label>Name (English) | အမည် (အင်္ဂလိပ်) *</Label>
                  <Input
                    value={formData.nameEn}
                    onChange={(e) => setFormData(prev => ({ ...prev, nameEn: e.target.value }))}
                    placeholder="Enter English name"
                    className="mt-1"
                  />
                </div>
                
                <div>
                  <Label>Name (Myanmar) | အမည် (မြန်မာ)</Label>
                  <Input
                    value={formData.nameMm}
                    onChange={(e) => setFormData(prev => ({ ...prev, nameMm: e.target.value }))}
                    placeholder="မြန်မာအမည် ရိုက်ထည့်ပါ"
                    className="mt-1"
                  />
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label>Employee ID | ဝန်ထမ်းအိုင်ဒီ *</Label>
                  <Input
                    value={formData.employeeId}
                    onChange={(e) => setFormData(prev => ({ ...prev, employeeId: e.target.value }))}
                    placeholder="EMP-001"
                    className="mt-1"
                  />
                </div>
                
                <div>
                  <Label>RFID ID | RFID အိုင်ဒီ *</Label>
                  <div className="flex gap-2 mt-1">
                    <Input
                      value={formData.rfidId}
                      onChange={(e) => setFormData(prev => ({ ...prev, rfidId: e.target.value }))}
                      placeholder="RF001"
                      className="flex-1"
                    />
                    <Button
                      variant="outline"
                      onClick={handleRfidScan}
                      disabled={rfidScanning}
                      className="gap-2"
                    >
                      <Scan className={`h-4 w-4 ${rfidScanning ? 'animate-pulse' : ''}`} />
                      {rfidScanning ? 'Scanning...' : 'Scan'}
                    </Button>
                  </div>
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label>Department | ဌာန *</Label>
                  <Select value={formData.department} onValueChange={(value) => setFormData(prev => ({ ...prev, department: value }))}>
                    <SelectTrigger className="mt-1">
                      <SelectValue placeholder="Select department" />
                    </SelectTrigger>
                    <SelectContent>
                      {departments.map(dept => (
                        <SelectItem key={dept.id} value={dept.id}>
                          {dept.name} | {dept.nameMm}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
                
                <div>
                  <Label>Role | ရာထူး *</Label>
                  <Select value={formData.role} onValueChange={(value) => setFormData(prev => ({ ...prev, role: value }))}>
                    <SelectTrigger className="mt-1">
                      <SelectValue placeholder="Select role" />
                    </SelectTrigger>
                    <SelectContent>
                      {roles.map(role => (
                        <SelectItem key={role.id} value={role.id}>
                          {role.name} | {role.nameMm}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
              </div>

              {/* Work Type Selection */}
              <div>
                <Label>Work Type | အလုပ်အမျိုးအစား *</Label>
                <RadioGroup
                  value={formData.workType}
                  onValueChange={(value) => setFormData(prev => ({ ...prev, workType: value, shiftSetup: '', expectedHours: '' }))}
                  className="mt-2"
                >
                  <div className="flex items-center space-x-2">
                    <RadioGroupItem value="shift" id="shift" />
                    <Label htmlFor="shift">Shift-based | အလုပ်ပတ်အခြေခံ</Label>
                  </div>
                  <div className="flex items-center space-x-2">
                    <RadioGroupItem value="flexible" id="flexible" />
                    <Label htmlFor="flexible">Flexible | လိုက်လျောညီထွေ</Label>
                  </div>
                </RadioGroup>
              </div>

              {/* Conditional Work Setup */}
              {formData.workType === 'shift' && (
                <div>
                  <Label>Shift Assignment | အလုပ်ပတ်ခန့်အပ်မှု *</Label>
                  <Select value={formData.shiftSetup} onValueChange={(value) => setFormData(prev => ({ ...prev, shiftSetup: value }))}>
                    <SelectTrigger className="mt-1">
                      <SelectValue placeholder="Select shift" />
                    </SelectTrigger>
                    <SelectContent>
                      {shifts.map(shift => (
                        <SelectItem key={shift.id} value={shift.id}>
                          {shift.name}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
              )}

              {formData.workType === 'flexible' && (
                <div>
                  <Label>Expected Hours per Week | အပတ်စဉ်မျှော်လင့်ချိန်များ *</Label>
                  <Input
                    type="number"
                    value={formData.expectedHours}
                    onChange={(e) => setFormData(prev => ({ ...prev, expectedHours: e.target.value }))}
                    placeholder="40"
                    className="mt-1"
                    min="1"
                    max="60"
                  />
                </div>
              )}

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label>Join Date | ဝင်ရောက်သည့်ရက်စွဲ</Label>
                  <Input
                    type="date"
                    value={formData.joinDate}
                    onChange={(e) => setFormData(prev => ({ ...prev, joinDate: e.target.value }))}
                    className="mt-1"
                  />
                </div>
                
                <div>
                  <Label>Status | အခြေအနေ</Label>
                  <Select value={formData.status} onValueChange={(value) => setFormData(prev => ({ ...prev, status: value }))}>
                    <SelectTrigger className="mt-1">
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="active">Active | တက်ကြွ</SelectItem>
                      <SelectItem value="inactive">Inactive | မတက်ကြွ</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
              </div>

              <Separator />

              {/* Action Buttons */}
              <div className="flex justify-end gap-3">
                <Button variant="outline" onClick={() => setIsFormOpen(false)}>
                  Cancel | ပယ်ဖျက်
                </Button>
                <Button onClick={handleSubmit} className="bg-blue-600 hover:bg-blue-700">
                  {editingStaff ? 'Update Staff' : 'Register Staff'} | 
                  {editingStaff ? ' ဝန်ထမ်းအပ်ဒေ့လုပ်' : ' ဝန်ထမ်းမှတ်ပုံတင်'}
                </Button>
              </div>
            </div>
          </DialogContent>
        </Dialog>
      </div>
    </div>
  );
}