import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '../ui/card';
import { Button } from '../ui/button';
import { Badge } from '../ui/badge';
import { Input } from '../ui/input';
import { Label } from '../ui/label';
import { Textarea } from '../ui/textarea';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../ui/table';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogTrigger } from '../ui/dialog';
import { Separator } from '../ui/separator';
import { toast } from 'sonner@2.0.3';
import { 
  Building2,
  Plus,
  Edit,
  Trash2,
  Download,
  Users,
  Calendar,
  FileText
} from 'lucide-react';

// Mock data for departments
const mockDepartments = [
  {
    id: 1,
    nameEn: 'Production',
    nameMm: 'ထုတ်လုပ်မှု',
    code: 'PROD',
    description: 'Manufacturing and production operations',
    descriptionMm: 'ကုန်ထုတ်လုပ်မှုနှင့် ထုတ်လုပ်မှုလုပ်ငန်းများ',
    staffCount: 85,
    defaultShifts: ['Morning Shift', 'Evening Shift', 'Night Shift'],
    manager: 'Ko Zaw Min',
    managerMm: 'ကိုဇော်မင်း',
    budget: 250000,
    status: 'active'
  },
  {
    id: 2,
    nameEn: 'Engineering',
    nameMm: 'အင်ဂျင်နီယာ',
    code: 'ENG',
    description: 'Technical engineering and maintenance',
    descriptionMm: 'နည်းပညာအင်ဂျင်နီယာနှင့် ပြုပြင်ထိန်းသိမ်းမှု',
    staffCount: 12,
    defaultShifts: ['Flexible Hours'],
    manager: 'Ma Khin Thida',
    managerMm: 'မခင်သီတာ',
    budget: 180000,
    status: 'active'
  },
  {
    id: 3,
    nameEn: 'Quality Control',
    nameMm: 'အရည်အသွေးထိန်းချုပ်မှု',
    code: 'QC',
    description: 'Quality assurance and inspection',
    descriptionMm: 'အရည်အသွေးအာမခံနှင့် စစ်ဆေးမှု',
    staffCount: 8,
    defaultShifts: ['Morning Shift', 'Evening Shift'],
    manager: 'Ko Thant Zin',
    managerMm: 'ကိုသန့်ဇင်',
    budget: 120000,
    status: 'active'
  },
  {
    id: 4,
    nameEn: 'Administration',
    nameMm: 'စီမံခန့်ခွဲမှု',
    code: 'ADMIN',
    description: 'Administrative and support functions',
    descriptionMm: 'စီမံခန့်ခွဲမှုနှင့် ပံ့ပိုးမှုလုပ်ငန်းများ',
    staffCount: 6,
    defaultShifts: ['Office Hours'],
    manager: 'Ma Ei Thwe',
    managerMm: 'မအိသွေး',
    budget: 90000,
    status: 'active'
  },
  {
    id: 5,
    nameEn: 'Finance',
    nameMm: 'ဘဏ္ဍာရေး',
    code: 'FIN',
    description: 'Financial management and accounting',
    descriptionMm: 'ဘဏ္ဍာရေးစီမံခန့်ခွဲမှုနှင့် စာရင်းကိုင်',
    staffCount: 4,
    defaultShifts: ['Office Hours'],
    manager: 'Ko Myint Swe',
    managerMm: 'ကိုမြင့်စွေ',
    budget: 150000,
    status: 'active'
  },
  {
    id: 6,
    nameEn: 'Maintenance',
    nameMm: 'ပြုပြင်ထိန်းသိမ်းမှု',
    code: 'MAINT',
    description: 'Equipment maintenance and repair',
    descriptionMm: 'စက်ပစ္စည်းပြုပြင်ထိန်းသိမ်းမှုနှင့် ပြင်ဆင်မှု',
    staffCount: 9,
    defaultShifts: ['Morning Shift', 'Evening Shift', 'On-call'],
    manager: 'Ko Aung Kyaw',
    managerMm: 'ကိုအောင်ကျော်',
    budget: 140000,
    status: 'active'
  }
];

export function DepartmentRegistration() {
  const [departments, setDepartments] = useState(mockDepartments);
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [editingDepartment, setEditingDepartment] = useState<any>(null);

  // Form state
  const [formData, setFormData] = useState({
    nameEn: '',
    nameMm: '',
    code: '',
    description: '',
    descriptionMm: '',
    manager: '',
    managerMm: '',
    budget: '',
    status: 'active'
  });

  const resetForm = () => {
    setFormData({
      nameEn: '',
      nameMm: '',
      code: '',
      description: '',
      descriptionMm: '',
      manager: '',
      managerMm: '',
      budget: '',
      status: 'active'
    });
    setEditingDepartment(null);
  };

  const handleOpenForm = (department = null) => {
    if (department) {
      setEditingDepartment(department);
      setFormData({
        nameEn: department.nameEn,
        nameMm: department.nameMm,
        code: department.code,
        description: department.description,
        descriptionMm: department.descriptionMm,
        manager: department.manager,
        managerMm: department.managerMm,
        budget: department.budget.toString(),
        status: department.status
      });
    } else {
      resetForm();
    }
    setIsFormOpen(true);
  };

  const handleSubmit = () => {
    // Validate required fields
    if (!formData.nameEn || !formData.code) {
      toast.error('Please fill in all required fields');
      return;
    }

    // Check for duplicate code (excluding current department if editing)
    const existingDept = departments.find(dept => 
      dept.code === formData.code && 
      (!editingDepartment || dept.id !== editingDepartment.id)
    );
    
    if (existingDept) {
      toast.error('Department code already exists');
      return;
    }

    const newDepartment = {
      id: editingDepartment ? editingDepartment.id : Date.now(),
      nameEn: formData.nameEn,
      nameMm: formData.nameMm,
      code: formData.code.toUpperCase(),
      description: formData.description,
      descriptionMm: formData.descriptionMm,
      manager: formData.manager,
      managerMm: formData.managerMm,
      budget: parseInt(formData.budget) || 0,
      status: formData.status,
      staffCount: editingDepartment ? editingDepartment.staffCount : 0,
      defaultShifts: editingDepartment ? editingDepartment.defaultShifts : ['Office Hours']
    };

    if (editingDepartment) {
      setDepartments(prev => prev.map(dept => dept.id === editingDepartment.id ? newDepartment : dept));
      toast.success('Department updated successfully');
    } else {
      setDepartments(prev => [...prev, newDepartment]);
      toast.success('New department created successfully');
    }

    setIsFormOpen(false);
    resetForm();
  };

  const handleDelete = (deptId: number) => {
    const department = departments.find(d => d.id === deptId);
    if (department && department.staffCount > 0) {
      toast.error('Cannot delete department with existing staff. Please reassign staff first.');
      return;
    }
    
    setDepartments(prev => prev.filter(dept => dept.id !== deptId));
    toast.success('Department deleted successfully');
  };

  const totalStaff = departments.reduce((sum, dept) => sum + dept.staffCount, 0);

  return (
    <div className="p-6 bg-slate-50 min-h-screen">
      <div className="max-w-7xl mx-auto space-y-6">
        {/* Header */}
        <div className="flex items-center justify-between mb-6">
          <div className="flex items-center gap-3">
            <div className="p-2 bg-green-100 rounded-lg">
              <Building2 className="h-6 w-6 text-green-600" />
            </div>
            <div>
              <h1 className="text-3xl font-semibold text-slate-900">
                Department Registration | ဌာနစာရင်းသွင်း
              </h1>
              <p className="text-slate-600 mt-1">
                Manage departments and organizational structure
              </p>
              <p className="text-sm text-slate-500">
                ဌာနများနှင့် အဖွဲ့အစည်းဖွဲ့စည်းပုံကို စီမံခန့်ခွဲခြင်း
              </p>
            </div>
          </div>
          
          <div className="flex items-center gap-3">
            <Button variant="outline" className="gap-2">
              <Download className="h-4 w-4" />
              Export PDF
            </Button>
            <Button onClick={() => handleOpenForm()} className="gap-2 bg-green-600 hover:bg-green-700">
              <Plus className="h-4 w-4" />
              Add Department | ဌာနအသစ်ထည့်ရန်
            </Button>
          </div>
        </div>

        {/* Summary Stats */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
          <Card className="border-0 shadow-sm">
            <CardContent className="p-4">
              <div className="flex items-center gap-3">
                <div className="p-2 bg-green-100 rounded-lg">
                  <Building2 className="w-4 h-4 text-green-600" />
                </div>
                <div>
                  <div className="text-2xl font-bold text-green-700">{departments.length}</div>
                  <div className="text-sm text-slate-600">Total Departments</div>
                  <div className="text-xs text-slate-500">စုစုပေါင်းဌာနများ</div>
                </div>
              </div>
            </CardContent>
          </Card>

          <Card className="border-0 shadow-sm">
            <CardContent className="p-4">
              <div className="flex items-center gap-3">
                <div className="p-2 bg-blue-100 rounded-lg">
                  <Users className="w-4 h-4 text-blue-600" />
                </div>
                <div>
                  <div className="text-2xl font-bold text-blue-700">{totalStaff}</div>
                  <div className="text-sm text-slate-600">Total Staff</div>
                  <div className="text-xs text-slate-500">စုစုပေါင်းဝန်ထမ်း</div>
                </div>
              </div>
            </CardContent>
          </Card>



          <Card className="border-0 shadow-sm">
            <CardContent className="p-4">
              <div className="flex items-center gap-3">
                <div className="p-2 bg-orange-100 rounded-lg">
                  <Calendar className="w-4 h-4 text-orange-600" />
                </div>
                <div>
                  <div className="text-2xl font-bold text-orange-700">{Math.round(totalStaff / departments.length)}</div>
                  <div className="text-sm text-slate-600">Avg Staff/Dept</div>
                  <div className="text-xs text-slate-500">တစ်ဌာနလျှင် ပျမ်းမျှဝန်ထမ်း</div>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Department Table */}
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Building2 className="h-5 w-5 text-green-600" />
              <div>
                <div>Department Directory | ဌာနလမ်းညွှန်</div>
                <div className="text-sm text-slate-500">Organizational structure and management</div>
              </div>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Department | ဌာန</TableHead>
                    <TableHead>Code | ကုဒ်</TableHead>
                    <TableHead>Manager | မန်နေဂျာ</TableHead>
                    <TableHead>No. of Staff | ဝန်ထမ်းအရေအတွက်</TableHead>
                    <TableHead>Default Shifts | ပုံမှန်အလုပ်ပတ်များ</TableHead>
                    <TableHead>Budget | ဘတ်ဂျက်</TableHead>
                    <TableHead>Status | အခြေအနေ</TableHead>
                    <TableHead>Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {departments.map((department) => (
                    <TableRow key={department.id}>
                      <TableCell>
                        <div>
                          <div className="font-medium">{department.nameEn}</div>
                          <div className="text-sm text-slate-500">{department.nameMm}</div>
                          <div className="text-xs text-slate-400 mt-1">{department.description}</div>
                        </div>
                      </TableCell>
                      <TableCell>
                        <Badge variant="outline" className="font-mono">
                          {department.code}
                        </Badge>
                      </TableCell>
                      <TableCell>
                        <div>
                          <div className="font-medium">{department.manager}</div>
                          <div className="text-sm text-slate-500">{department.managerMm}</div>
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-2">
                          <Users className="h-4 w-4 text-slate-400" />
                          <span className="font-medium text-lg">{department.staffCount}</span>
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="space-y-1">
                          {department.defaultShifts.map((shift, index) => (
                            <Badge key={index} variant="outline" className="text-xs mr-1">
                              {shift}
                            </Badge>
                          ))}
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="font-medium text-green-700">
                          ${department.budget.toLocaleString()}
                        </div>
                      </TableCell>
                      <TableCell>
                        <Badge className={department.status === 'active' ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}>
                          {department.status === 'active' ? 'Active | တက်ကြွ' : 'Inactive | မတက်ကြွ'}
                        </Badge>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-2">
                          <Button
                            variant="outline"
                            size="sm"
                            onClick={() => handleOpenForm(department)}
                            className="gap-1"
                          >
                            <Edit className="h-3 w-3" />
                            Edit
                          </Button>
                          <Button
                            variant="outline"
                            size="sm"
                            onClick={() => handleDelete(department.id)}
                            disabled={department.staffCount > 0}
                            className="gap-1 text-red-600 hover:text-red-700 disabled:opacity-50 disabled:cursor-not-allowed"
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
            </div>
          </CardContent>
        </Card>

        {/* Department Form Dialog */}
        <Dialog open={isFormOpen} onOpenChange={setIsFormOpen}>
          <DialogContent className="max-w-2xl">
            <DialogHeader>
              <DialogTitle className="flex items-center gap-2">
                <Building2 className="h-5 w-5 text-green-600" />
                {editingDepartment ? 'Edit Department' : 'Create New Department'}
              </DialogTitle>
              <DialogDescription>
                {editingDepartment ? 'Update department information and settings' : 'Add a new department to the organizational structure'}
              </DialogDescription>
            </DialogHeader>
            
            <div className="space-y-6 py-4">
              {/* Basic Information */}
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label>Department Name (English) | ဌာနအမည် (အင်္ဂလိပ်) *</Label>
                  <Input
                    value={formData.nameEn}
                    onChange={(e) => setFormData(prev => ({ ...prev, nameEn: e.target.value }))}
                    placeholder="Enter department name"
                    className="mt-1"
                  />
                </div>
                
                <div>
                  <Label>Department Name (Myanmar) | ဌာနအမည် (မြန်မာ)</Label>
                  <Input
                    value={formData.nameMm}
                    onChange={(e) => setFormData(prev => ({ ...prev, nameMm: e.target.value }))}
                    placeholder="ဌာနအမည် ရိုက်ထည့်ပါ"
                    className="mt-1"
                  />
                </div>
              </div>

              <div>
                <Label>Department Code | ဌာနကုဒ် *</Label>
                <Input
                  value={formData.code}
                  onChange={(e) => setFormData(prev => ({ ...prev, code: e.target.value.toUpperCase() }))}
                  placeholder="PROD, ENG, QC, etc."
                  className="mt-1 font-mono"
                  maxLength={10}
                />
                <div className="text-xs text-slate-500 mt-1">
                  Short code for identification (will be converted to uppercase)
                </div>
              </div>

              <div className="grid grid-cols-1 gap-4">
                <div>
                  <Label>Description (English) | ဖော်ပြချက် (အင်္ဂလိပ်)</Label>
                  <Textarea
                    value={formData.description}
                    onChange={(e) => setFormData(prev => ({ ...prev, description: e.target.value }))}
                    placeholder="Brief description of department functions"
                    className="mt-1"
                    rows={2}
                  />
                </div>
                
                <div>
                  <Label>Description (Myanmar) | ဖော်ပြချက် (မြန်မာ)</Label>
                  <Textarea
                    value={formData.descriptionMm}
                    onChange={(e) => setFormData(prev => ({ ...prev, descriptionMm: e.target.value }))}
                    placeholder="ဌာနလုပ်ငန်းများ၏ အကျဉ်းချုပ်ဖော်ပြချက်"
                    className="mt-1"
                    rows={2}
                  />
                </div>
              </div>

              {/* Management Information */}
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label>Department Manager | ဌာနမန်နေဂျာ</Label>
                  <Input
                    value={formData.manager}
                    onChange={(e) => setFormData(prev => ({ ...prev, manager: e.target.value }))}
                    placeholder="Manager name"
                    className="mt-1"
                  />
                </div>
                
                <div>
                  <Label>Manager Name (Myanmar) | မန်နေဂျာအမည် (မြန်မာ)</Label>
                  <Input
                    value={formData.managerMm}
                    onChange={(e) => setFormData(prev => ({ ...prev, managerMm: e.target.value }))}
                    placeholder="မန်နေဂျာအမည်"
                    className="mt-1"
                  />
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label>Annual Budget (USD) | နှစ်ပတ်လုံးဘတ်ဂျက်</Label>
                  <Input
                    type="number"
                    value={formData.budget}
                    onChange={(e) => setFormData(prev => ({ ...prev, budget: e.target.value }))}
                    placeholder="150000"
                    className="mt-1"
                    min="0"
                  />
                </div>
                
                <div>
                  <Label>Status | အခြေအနေ</Label>
                  <select
                    value={formData.status}
                    onChange={(e) => setFormData(prev => ({ ...prev, status: e.target.value }))}
                    className="mt-1 w-full px-3 py-2 border border-slate-300 rounded-md focus:outline-none focus:ring-2 focus:ring-green-500"
                  >
                    <option value="active">Active | တက်ကြွ</option>
                    <option value="inactive">Inactive | မတက်ကြွ</option>
                  </select>
                </div>
              </div>

              <Separator />

              {/* Action Buttons */}
              <div className="flex justify-end gap-3">
                <Button variant="outline" onClick={() => setIsFormOpen(false)}>
                  Cancel | ပယ်ဖျက်
                </Button>
                <Button onClick={handleSubmit} className="bg-green-600 hover:bg-green-700">
                  {editingDepartment ? 'Update Department' : 'Create Department'} | 
                  {editingDepartment ? ' ဌာနအပ်ဒေ့လုပ်' : ' ဌာနဖန်တီး'}
                </Button>
              </div>
            </div>
          </DialogContent>
        </Dialog>
      </div>
    </div>
  );
}