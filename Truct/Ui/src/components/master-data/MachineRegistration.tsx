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
import { 
  Search,
  Filter,
  Download,
  Plus,
  Edit,
  Save,
  Factory,
  Cpu,
  Settings,
  FileSpreadsheet
} from 'lucide-react';

// Mock data for machines
const mockMachines = [
  {
    id: 1,
    machineCode: 'MCH-001',
    name: 'Injection Machine #1',
    type: 'Injection',
    department: 'Production Line A',
    efficiency: 85,
    maxCapacityPerDay: 5000,
    status: 'Active',
    createdBy: 'Admin',
    createdOn: '2024-01-15 10:30',
    updatedBy: 'Ko Zaw',
    updatedOn: '2024-03-10 14:20',
    version: 2
  },
  {
    id: 2,
    machineCode: 'MCH-002',
    name: 'Extrusion Line #1',
    type: 'Extrusion',
    department: 'Production Line B',
    efficiency: 90,
    maxCapacityPerDay: 3000,
    status: 'Active',
    createdBy: 'Admin',
    createdOn: '2024-02-01 09:15',
    updatedBy: 'Ma Thin',
    updatedOn: '2024-03-08 16:45',
    version: 1
  },
  {
    id: 3,
    machineCode: 'MCH-003',
    name: 'Cutting Machine #1',
    type: 'Cutting',
    department: 'Production Line A',
    efficiency: 88,
    maxCapacityPerDay: 12000,
    status: 'Maintenance',
    createdBy: 'Engineer',
    createdOn: '2024-01-20 11:00',
    updatedBy: 'Ko Aung',
    updatedOn: '2024-03-12 08:30',
    version: 3
  },
  {
    id: 4,
    machineCode: 'MCH-004',
    name: 'Packing Line #1',
    type: 'Packing',
    department: 'Packaging',
    efficiency: 92,
    maxCapacityPerDay: 8000,
    status: 'Active',
    createdBy: 'Planner',
    createdOn: '2024-02-15 13:20',
    updatedBy: 'Planner',
    updatedOn: '2024-02-15 13:20',
    version: 1
  }
];

const machineTypes = ['Injection', 'Extrusion', 'Cutting', 'Packing'];
const departments = ['Production Line A', 'Production Line B', 'Production Line C', 'Packaging', 'QC', 'Maintenance'];

export function MachineRegistration() {
  const [searchTerm, setSearchTerm] = useState('');
  const [filterType, setFilterType] = useState('');
  const [filterStatus, setFilterStatus] = useState('');
  const [showDrawer, setShowDrawer] = useState(false);
  const [editingItem, setEditingItem] = useState<any>(null);
  const [formData, setFormData] = useState({
    machineCode: '',
    name: '',
    type: '',
    department: '',
    efficiency: '85',
    maxCapacityPerDay: '',
    status: true
  });

  const handleCreate = () => {
    setEditingItem(null);
    setFormData({
      machineCode: '',
      name: '',
      type: '',
      department: '',
      efficiency: '85',
      maxCapacityPerDay: '',
      status: true
    });
    setShowDrawer(true);
  };

  const handleEdit = (item: any) => {
    setEditingItem(item);
    setFormData({
      machineCode: item.machineCode,
      name: item.name,
      type: item.type,
      department: item.department,
      efficiency: item.efficiency.toString(),
      maxCapacityPerDay: item.maxCapacityPerDay.toString(),
      status: item.status === 'Active'
    });
    setShowDrawer(true);
  };

  const handleSave = () => {
    console.log('Saving machine:', formData);
    setShowDrawer(false);
  };

  const handleSaveAndNew = () => {
    console.log('Saving machine:', formData);
    setFormData({
      machineCode: '',
      name: '',
      type: '',
      department: '',
      efficiency: '85',
      maxCapacityPerDay: '',
      status: true
    });
  };

  const filteredMachines = mockMachines.filter(machine =>
    machine.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    machine.machineCode.toLowerCase().includes(searchTerm.toLowerCase()) ||
    machine.department.toLowerCase().includes(searchTerm.toLowerCase())
  ).filter(machine =>
    filterType === '' || filterType === 'all' || machine.type === filterType
  ).filter(machine =>
    filterStatus === '' || filterStatus === 'all' || machine.status === filterStatus
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
          Machine Registration | စက်မှတ်ပုံတင်မှု
        </h2>
        <p className="text-slate-600 mt-1">
          Register and manage production machines with capacity and efficiency settings
        </p>
        <p className="text-sm text-slate-500">
          စွမ်းရည်နှင့်ထိရောက်မှုဆက်တင်များဖြင့် ထုတ်လုပ်မှုစက်များကို မှတ်ပုံတင်ပြီး စီမံခန့်ခွဲခြင်း
        </p>
      </div>

      {/* Toolbar */}
      <div className="flex justify-between items-center">
        <div className="flex items-center gap-4">
          {/* Search */}
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-slate-400 h-4 w-4" />
            <Input
              placeholder="Search by machine code, name, or department..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="pl-10 w-80"
            />
          </div>

          {/* Filters */}
          <Select value={filterType} onValueChange={setFilterType}>
            <SelectTrigger className="w-32">
              <SelectValue placeholder="Type" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">All Types</SelectItem>
              {machineTypes.map((type) => (
                <SelectItem key={type} value={type}>{type}</SelectItem>
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
              <SelectItem value="Maintenance">Maintenance</SelectItem>
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
            Add Machine | စက်ထည့်သွင်းရန်
          </Button>
        </div>
      </div>

      {/* Main Table */}
      <Card className="border-0 shadow-sm">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Factory className="h-5 w-5 text-blue-600" />
            Machines | စက်များ
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Machine Code | စက်ကုဒ်</TableHead>
                  <TableHead>Name | အမည်</TableHead>
                  <TableHead>Type | အမျိုးအစား</TableHead>
                  <TableHead>Department | ဌာန</TableHead>
                  <TableHead>Efficiency % | ထိရောက်မှု</TableHead>
                  <TableHead>Max Capacity/day | တစ်နေ့အများဆုံး</TableHead>
                  <TableHead>Status | အခြေအနေ</TableHead>
                  <TableHead>Actions | လုပ်ဆောင်ချက်များ</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {filteredMachines.map((machine) => (
                  <TableRow key={machine.id}>
                    <TableCell>
                      <Badge variant="outline" className="font-mono">
                        {machine.machineCode}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-2">
                        <Cpu className="h-4 w-4 text-blue-500" />
                        <span className="font-medium">{machine.name}</span>
                      </div>
                    </TableCell>
                    <TableCell>
                      <Badge className={
                        machine.type === 'Injection' ? 'bg-blue-100 text-blue-800' :
                        machine.type === 'Extrusion' ? 'bg-green-100 text-green-800' :
                        machine.type === 'Cutting' ? 'bg-orange-100 text-orange-800' :
                        'bg-purple-100 text-purple-800'
                      }>
                        {machine.type}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <span className="text-slate-600">{machine.department}</span>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-1">
                        <span className="font-semibold text-green-700">{machine.efficiency}</span>
                        <span className="text-xs text-slate-500">%</span>
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-1">
                        <span className="font-medium">{machine.maxCapacityPerDay.toLocaleString()}</span>
                        <span className="text-xs text-slate-500">pcs</span>
                      </div>
                    </TableCell>
                    <TableCell>
                      <Badge className={
                        machine.status === 'Active' ? 'bg-green-100 text-green-800' :
                        machine.status === 'Maintenance' ? 'bg-yellow-100 text-yellow-800' :
                        'bg-red-100 text-red-800'
                      }>
                        {machine.status}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => handleEdit(machine)}
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
              Showing {filteredMachines.length} of {mockMachines.length} machines
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
        <SheetContent className="w-[500px] sm:max-w-[500px] overflow-y-auto">
          <SheetHeader>
            <SheetTitle className="flex items-center gap-2">
              <Factory className="h-5 w-5 text-blue-600" />
              {editingItem ? 'Edit Machine' : 'Create New Machine'}
            </SheetTitle>
            <SheetDescription>
              {editingItem ? 'Update machine information and settings' : 'Add new production machine to the system'}
            </SheetDescription>
          </SheetHeader>

          <div className="space-y-6 py-6">
            {/* Basic Information */}
            <div className="space-y-4">
              <h4 className="font-medium text-slate-900">Basic Information | အခြေခံအချက်အလက်</h4>
              
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label htmlFor="machineCode">Machine Code * | စက်ကုဒ် *</Label>
                  <Input
                    id="machineCode"
                    value={formData.machineCode}
                    onChange={(e) => setFormData(prev => ({ ...prev, machineCode: e.target.value }))}
                    placeholder="MCH-001"
                    className="mt-1"
                  />
                </div>

                <div>
                  <Label htmlFor="name">Name * | အမည် *</Label>
                  <Input
                    id="name"
                    value={formData.name}
                    onChange={(e) => setFormData(prev => ({ ...prev, name: e.target.value }))}
                    placeholder="Injection Machine #1"
                    className="mt-1"
                  />
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label htmlFor="type">Type * | အမျိုးအစား *</Label>
                  <Select value={formData.type} onValueChange={(value) => setFormData(prev => ({ ...prev, type: value }))}>
                    <SelectTrigger className="mt-1">
                      <SelectValue placeholder="Select type..." />
                    </SelectTrigger>
                    <SelectContent>
                      {machineTypes.map((type) => (
                        <SelectItem key={type} value={type}>{type}</SelectItem>
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
            </div>

            {/* Performance Settings */}
            <div className="space-y-4">
              <h4 className="font-medium text-slate-900">Performance Settings | စွမ်းဆောင်ရည်ဆက်တင်များ</h4>
              
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label htmlFor="efficiency">Efficiency % | ထိရောက်မှု %</Label>
                  <Input
                    id="efficiency"
                    type="number"
                    min="1"
                    max="100"
                    value={formData.efficiency}
                    onChange={(e) => setFormData(prev => ({ ...prev, efficiency: e.target.value }))}
                    placeholder="85"
                    className="mt-1"
                  />
                  <div className="text-xs text-slate-500 mt-1">
                    Default planning efficiency (1-100%)
                  </div>
                </div>

                <div>
                  <Label htmlFor="maxCapacityPerDay">Max Capacity/day | တစ်နေ့အများဆုံး</Label>
                  <Input
                    id="maxCapacityPerDay"
                    type="number"
                    min="0"
                    value={formData.maxCapacityPerDay}
                    onChange={(e) => setFormData(prev => ({ ...prev, maxCapacityPerDay: e.target.value }))}
                    placeholder="5000"
                    className="mt-1"
                  />
                  <div className="text-xs text-slate-500 mt-1">
                    Maximum pieces per day
                  </div>
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
                <h4 className="font-medium text-slate-900 mb-2">Audit Trail | စာရင်းစစ်မှတ്တမ်း</h4>
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