import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '../ui/card';
import { Button } from '../ui/button';
import { Badge } from '../ui/badge';
import { Input } from '../ui/input';
import { Label } from '../ui/label';
import { RadioGroup, RadioGroupItem } from '../ui/radio-group';
import { Checkbox } from '../ui/checkbox';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../ui/table';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogTrigger } from '../ui/dialog';
import { Separator } from '../ui/separator';
import { toast } from 'sonner@2.0.3';
import { 
  Clock,
  Plus,
  Edit,
  Trash2,
  Download,
  Calendar,
  Users,
  Settings
} from 'lucide-react';

// Mock data for shift and flexible setups
const mockSetups = [
  {
    id: 1,
    type: 'shift',
    name: 'Morning Shift',
    nameMm: 'နံနက်ပတ်',
    timeFrom: '08:00',
    timeTo: '17:00',
    gracePeriod: 15,
    activeDays: ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'],
    colorTag: '#10b981',
    expectedHours: null,
    allowedTimeRange: null,
    staffCount: 45,
    status: 'active'
  },
  {
    id: 2,
    type: 'shift',
    name: 'Evening Shift',
    nameMm: 'ညနေပတ်',
    timeFrom: '17:00',
    timeTo: '02:00',
    gracePeriod: 10,
    activeDays: ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'],
    colorTag: '#f59e0b',
    expectedHours: null,
    allowedTimeRange: null,
    staffCount: 32,
    status: 'active'
  },
  {
    id: 3,
    type: 'shift',
    name: 'Night Shift',
    nameMm: 'ညပတ်',
    timeFrom: '02:00',
    timeTo: '08:00',
    gracePeriod: 10,
    activeDays: ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'],
    colorTag: '#6366f1',
    expectedHours: null,
    allowedTimeRange: null,
    staffCount: 0,
    status: 'inactive'
  },
  {
    id: 4,
    type: 'flexible',
    name: 'Engineering Flexible',
    nameMm: 'အင်ဂျင်နီယာ လိုက်လျောညီထွေ',
    timeFrom: null,
    timeTo: null,
    gracePeriod: null,
    activeDays: ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday'],
    colorTag: '#8b5cf6',
    expectedHours: 40,
    allowedTimeRange: '08:00-20:00',
    staffCount: 12,
    status: 'active'
  },
  {
    id: 5,
    type: 'flexible',
    name: 'Part-time Support',
    nameMm: 'အချိန်ပိုင်းပံ့ပိုးမှု',
    timeFrom: null,
    timeTo: null,
    gracePeriod: null,
    activeDays: ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday'],
    colorTag: '#ec4899',
    expectedHours: 20,
    allowedTimeRange: '10:00-18:00',
    staffCount: 6,
    status: 'active'
  }
];

const daysOfWeek = [
  { id: 'Monday', name: 'Monday', nameMm: 'တနင်္လာ' },
  { id: 'Tuesday', name: 'Tuesday', nameMm: 'အင်္ဂါ' },
  { id: 'Wednesday', name: 'Wednesday', nameMm: 'ဗုဒ္ဓဟူး' },
  { id: 'Thursday', name: 'Thursday', nameMm: 'ကြာသပတေး' },
  { id: 'Friday', name: 'Friday', nameMm: 'သောကြာ' },
  { id: 'Saturday', name: 'Saturday', nameMm: 'စနေ' },
  { id: 'Sunday', name: 'Sunday', nameMm: 'တနင်္ဂနွေ' }
];

const colorOptions = [
  { value: '#10b981', name: 'Green', nameMm: 'အစိမ်း' },
  { value: '#f59e0b', name: 'Orange', nameMm: 'လိမ္မော်' },
  { value: '#6366f1', name: 'Indigo', nameMm: 'နီလာ' },
  { value: '#8b5cf6', name: 'Purple', nameMm: 'ခရမ်း' },
  { value: '#ec4899', name: 'Pink', nameMm: 'ပန်းရောင်' },
  { value: '#ef4444', name: 'Red', nameMm: 'အနီ' },
  { value: '#3b82f6', name: 'Blue', nameMm: 'အပြာ' },
  { value: '#06b6d4', name: 'Cyan', nameMm: 'စိမ်းပြာ' }
];

export function ShiftFlexibleSetup() {
  const [setups, setSetups] = useState(mockSetups);
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [editingSetup, setEditingSetup] = useState<any>(null);

  // Form state
  const [formData, setFormData] = useState({
    type: 'shift',
    name: '',
    nameMm: '',
    timeFrom: '',
    timeTo: '',
    gracePeriod: 15,
    activeDays: [] as string[],
    colorTag: '#10b981',
    expectedHours: '',
    allowedTimeRange: '',
    status: 'active'
  });

  const resetForm = () => {
    setFormData({
      type: 'shift',
      name: '',
      nameMm: '',
      timeFrom: '',
      timeTo: '',
      gracePeriod: 15,
      activeDays: [],
      colorTag: '#10b981',
      expectedHours: '',
      allowedTimeRange: '',
      status: 'active'
    });
    setEditingSetup(null);
  };

  const handleOpenForm = (setup = null) => {
    if (setup) {
      setEditingSetup(setup);
      setFormData({
        type: setup.type,
        name: setup.name,
        nameMm: setup.nameMm,
        timeFrom: setup.timeFrom || '',
        timeTo: setup.timeTo || '',
        gracePeriod: setup.gracePeriod || 15,
        activeDays: setup.activeDays || [],
        colorTag: setup.colorTag,
        expectedHours: setup.expectedHours?.toString() || '',
        allowedTimeRange: setup.allowedTimeRange || '',
        status: setup.status
      });
    } else {
      resetForm();
    }
    setIsFormOpen(true);
  };

  const handleDayToggle = (day: string, checked: boolean) => {
    if (checked) {
      setFormData(prev => ({ ...prev, activeDays: [...prev.activeDays, day] }));
    } else {
      setFormData(prev => ({ ...prev, activeDays: prev.activeDays.filter(d => d !== day) }));
    }
  };

  const handleSubmit = () => {
    // Validate required fields
    if (!formData.name) {
      toast.error('Please enter setup name');
      return;
    }

    if (formData.type === 'shift') {
      if (!formData.timeFrom || !formData.timeTo) {
        toast.error('Please enter shift start and end time');
        return;
      }
    } else {
      if (!formData.expectedHours) {
        toast.error('Please enter expected hours per week');
        return;
      }
    }

    if (formData.activeDays.length === 0) {
      toast.error('Please select at least one active day');
      return;
    }

    const newSetup = {
      id: editingSetup ? editingSetup.id : Date.now(),
      type: formData.type,
      name: formData.name,
      nameMm: formData.nameMm,
      timeFrom: formData.type === 'shift' ? formData.timeFrom : null,
      timeTo: formData.type === 'shift' ? formData.timeTo : null,
      gracePeriod: formData.type === 'shift' ? formData.gracePeriod : null,
      activeDays: formData.activeDays,
      colorTag: formData.colorTag,
      expectedHours: formData.type === 'flexible' ? parseInt(formData.expectedHours) : null,
      allowedTimeRange: formData.type === 'flexible' ? formData.allowedTimeRange : null,
      status: formData.status,
      staffCount: editingSetup ? editingSetup.staffCount : 0
    };

    if (editingSetup) {
      setSetups(prev => prev.map(setup => setup.id === editingSetup.id ? newSetup : setup));
      toast.success('Setup updated successfully');
    } else {
      setSetups(prev => [...prev, newSetup]);
      toast.success('New setup created successfully');
    }

    setIsFormOpen(false);
    resetForm();
  };

  const handleDelete = (setupId: number) => {
    const setup = setups.find(s => s.id === setupId);
    if (setup && setup.staffCount > 0) {
      toast.error('Cannot delete setup with assigned staff. Please reassign staff first.');
      return;
    }
    
    setSetups(prev => prev.filter(setup => setup.id !== setupId));
    toast.success('Setup deleted successfully');
  };

  const shiftSetups = setups.filter(s => s.type === 'shift');
  const flexibleSetups = setups.filter(s => s.type === 'flexible');
  const totalStaff = setups.reduce((sum, setup) => sum + setup.staffCount, 0);

  return (
    <div className="p-6 bg-slate-50 min-h-screen">
      <div className="max-w-7xl mx-auto space-y-6">
        {/* Header */}
        <div className="flex items-center justify-between mb-6">
          <div className="flex items-center gap-3">
            <div className="p-2 bg-purple-100 rounded-lg">
              <Clock className="h-6 w-6 text-purple-600" />
            </div>
            <div>
              <h1 className="text-3xl font-semibold text-slate-900">
                Shift & Flexible Setup | Shift နှင့် အချိန်ပိုင်းအလုပ် စာရင်းသွင်း
              </h1>
              <p className="text-slate-600 mt-1">
                Configure shift schedules and flexible work arrangements
              </p>
              <p className="text-sm text-slate-500">
                အလုပ်ပတ်အစီအစဉ်နှင့် လိုက်လျောညီထွေ အလုပ်အစီအစဉ်များ ပြင်ဆင်ခြင်း
              </p>
            </div>
          </div>
          
          <div className="flex items-center gap-3">
            <Button variant="outline" className="gap-2">
              <Download className="h-4 w-4" />
              Export Configuration
            </Button>
            <Button onClick={() => handleOpenForm()} className="gap-2 bg-purple-600 hover:bg-purple-700">
              <Plus className="h-4 w-4" />
              Add Setup | စနစ်အသစ်ထည့်ရန်
            </Button>
          </div>
        </div>

        {/* Summary Stats */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
          <Card className="border-0 shadow-sm">
            <CardContent className="p-4">
              <div className="flex items-center gap-3">
                <div className="p-2 bg-blue-100 rounded-lg">
                  <Clock className="w-4 h-4 text-blue-600" />
                </div>
                <div>
                  <div className="text-2xl font-bold text-blue-700">{shiftSetups.length}</div>
                  <div className="text-sm text-slate-600">Shift Types</div>
                  <div className="text-xs text-slate-500">အလုပ်ပတ်အမျိုးအစားများ</div>
                </div>
              </div>
            </CardContent>
          </Card>

          <Card className="border-0 shadow-sm">
            <CardContent className="p-4">
              <div className="flex items-center gap-3">
                <div className="p-2 bg-purple-100 rounded-lg">
                  <Settings className="w-4 h-4 text-purple-600" />
                </div>
                <div>
                  <div className="text-2xl font-bold text-purple-700">{flexibleSetups.length}</div>
                  <div className="text-sm text-slate-600">Flexible Types</div>
                  <div className="text-xs text-slate-500">လိုက်လျောညီထွေအမျိုးအစားများ</div>
                </div>
              </div>
            </CardContent>
          </Card>

          <Card className="border-0 shadow-sm">
            <CardContent className="p-4">
              <div className="flex items-center gap-3">
                <div className="p-2 bg-green-100 rounded-lg">
                  <Users className="w-4 h-4 text-green-600" />
                </div>
                <div>
                  <div className="text-2xl font-bold text-green-700">{totalStaff}</div>
                  <div className="text-sm text-slate-600">Assigned Staff</div>
                  <div className="text-xs text-slate-500">ခန့်အပ်ထားသောဝန်ထမ်း</div>
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
                  <div className="text-2xl font-bold text-orange-700">{setups.filter(s => s.status === 'active').length}</div>
                  <div className="text-sm text-slate-600">Active Setups</div>
                  <div className="text-xs text-slate-500">တက်ကြွသောစနစ်များ</div>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Shift-based Setups */}
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Clock className="h-5 w-5 text-blue-600" />
              <div>
                <div>Shift-based Work Schedules | အလုပ်ပတ်အခြေခံအစီအစဉ်များ</div>
                <div className="text-sm text-slate-500">Fixed time schedules with specific start and end times</div>
              </div>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Shift Name | ပတ်အမည်</TableHead>
                    <TableHead>Time Range | အချိန်ကွာဟချက်</TableHead>
                    <TableHead>Grace Period | ခွင့်ပြုချိန်</TableHead>
                    <TableHead>Active Days | တက်ကြွရက်များ</TableHead>
                    <TableHead>Assigned Staff | ခန့်အပ်ထားသောဝန်ထမ်း</TableHead>
                    <TableHead>Status | အခြေအနေ</TableHead>
                    <TableHead>Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {shiftSetups.map((setup) => (
                    <TableRow key={setup.id}>
                      <TableCell>
                        <div className="flex items-center gap-3">
                          <div
                            className="w-4 h-4 rounded-full"
                            style={{ backgroundColor: setup.colorTag }}
                          ></div>
                          <div>
                            <div className="font-medium">{setup.name}</div>
                            <div className="text-sm text-slate-500">{setup.nameMm}</div>
                          </div>
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="font-mono text-sm">
                          {setup.timeFrom} - {setup.timeTo}
                        </div>
                        <div className="text-xs text-slate-500">
                          {parseInt(setup.timeTo.split(':')[0]) > parseInt(setup.timeFrom.split(':')[0]) ? 
                            `${parseInt(setup.timeTo.split(':')[0]) - parseInt(setup.timeFrom.split(':')[0])} hours` :
                            `${24 - parseInt(setup.timeFrom.split(':')[0]) + parseInt(setup.timeTo.split(':')[0])} hours`
                          }
                        </div>
                      </TableCell>
                      <TableCell>
                        <Badge variant="outline">
                          {setup.gracePeriod} minutes
                        </Badge>
                      </TableCell>
                      <TableCell>
                        <div className="text-sm">
                          {setup.activeDays.length === 7 ? 'All Days' :
                           setup.activeDays.length === 6 ? 'Mon-Sat' :
                           setup.activeDays.length === 5 ? 'Mon-Fri' :
                           `${setup.activeDays.length} days`}
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-2">
                          <Users className="h-4 w-4 text-slate-400" />
                          <span className="font-medium">{setup.staffCount}</span>
                        </div>
                      </TableCell>
                      <TableCell>
                        <Badge className={setup.status === 'active' ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}>
                          {setup.status === 'active' ? 'Active | တက်ကြွ' : 'Inactive | မတက်ကြွ'}
                        </Badge>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-2">
                          <Button
                            variant="outline"
                            size="sm"
                            onClick={() => handleOpenForm(setup)}
                            className="gap-1"
                          >
                            <Edit className="h-3 w-3" />
                            Edit
                          </Button>
                          <Button
                            variant="outline"
                            size="sm"
                            onClick={() => handleDelete(setup.id)}
                            disabled={setup.staffCount > 0}
                            className="gap-1 text-red-600 hover:text-red-700 disabled:opacity-50"
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
              
              {shiftSetups.length === 0 && (
                <div className="text-center py-8 text-slate-500">
                  <Clock className="h-12 w-12 mx-auto mb-4 text-slate-300" />
                  <p>No shift setups configured</p>
                  <p className="text-sm">အလုပ်ပတ်စနစ်များ မပြင်ဆင်ထားပါ</p>
                </div>
              )}
            </div>
          </CardContent>
        </Card>

        {/* Flexible Work Setups */}
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Settings className="h-5 w-5 text-purple-600" />
              <div>
                <div>Flexible Work Arrangements | လိုက်လျောညီထွေအလုပ်အစီအစဉ်များ</div>
                <div className="text-sm text-slate-500">Hour-based schedules with flexible timing</div>
              </div>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Setup Name | စနစ်အမည်</TableHead>
                    <TableHead>Expected Hours | မျှော်လင့်ချိန်များ</TableHead>
                    <TableHead>Allowed Time Range | ခွင့်ပြုအချိန်ကွာဟချက်</TableHead>
                    <TableHead>Work Days | အလုပ်ရက်များ</TableHead>
                    <TableHead>Assigned Staff | ခန့်အပ်ထားသောဝန်ထမ်း</TableHead>
                    <TableHead>Status | အခြေအနေ</TableHead>
                    <TableHead>Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {flexibleSetups.map((setup) => (
                    <TableRow key={setup.id}>
                      <TableCell>
                        <div className="flex items-center gap-3">
                          <div
                            className="w-4 h-4 rounded-full"
                            style={{ backgroundColor: setup.colorTag }}
                          ></div>
                          <div>
                            <div className="font-medium">{setup.name}</div>
                            <div className="text-sm text-slate-500">{setup.nameMm}</div>
                          </div>
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="font-medium text-purple-700">
                          {setup.expectedHours} hrs/week
                        </div>
                        <div className="text-xs text-slate-500">
                          ~{Math.round(setup.expectedHours / setup.activeDays.length)} hrs/day
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="font-mono text-sm">
                          {setup.allowedTimeRange || 'No restriction'}
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="text-sm">
                          {setup.activeDays.length === 7 ? 'All Days' :
                           setup.activeDays.length === 6 ? 'Mon-Sat' :
                           setup.activeDays.length === 5 ? 'Mon-Fri' :
                           `${setup.activeDays.length} days`}
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-2">
                          <Users className="h-4 w-4 text-slate-400" />
                          <span className="font-medium">{setup.staffCount}</span>
                        </div>
                      </TableCell>
                      <TableCell>
                        <Badge className={setup.status === 'active' ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}>
                          {setup.status === 'active' ? 'Active | တက်ကြွ' : 'Inactive | မတက်ကြွ'}
                        </Badge>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-2">
                          <Button
                            variant="outline"
                            size="sm"
                            onClick={() => handleOpenForm(setup)}
                            className="gap-1"
                          >
                            <Edit className="h-3 w-3" />
                            Edit
                          </Button>
                          <Button
                            variant="outline"
                            size="sm"
                            onClick={() => handleDelete(setup.id)}
                            disabled={setup.staffCount > 0}
                            className="gap-1 text-red-600 hover:text-red-700 disabled:opacity-50"
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
              
              {flexibleSetups.length === 0 && (
                <div className="text-center py-8 text-slate-500">
                  <Settings className="h-12 w-12 mx-auto mb-4 text-slate-300" />
                  <p>No flexible setups configured</p>
                  <p className="text-sm">လိုက်လျောညီထွေစနစ်များ မပြင်ဆင်ထားပါ</p>
                </div>
              )}
            </div>
          </CardContent>
        </Card>

        {/* Setup Form Dialog */}
        <Dialog open={isFormOpen} onOpenChange={setIsFormOpen}>
          <DialogContent className="max-w-3xl max-h-[90vh] overflow-y-auto">
            <DialogHeader>
              <DialogTitle className="flex items-center gap-2">
                <Clock className="h-5 w-5 text-purple-600" />
                {editingSetup ? 'Edit Work Setup' : 'Create New Work Setup'}
              </DialogTitle>
              <DialogDescription>
                {editingSetup ? 'Update work schedule configuration' : 'Configure a new shift or flexible work arrangement'}
              </DialogDescription>
            </DialogHeader>
            
            <div className="space-y-6 py-4">
              {/* Work Type Selection */}
              <div>
                <Label>Work Type | အလုပ်အမျိုးအစား *</Label>
                <RadioGroup
                  value={formData.type}
                  onValueChange={(value) => setFormData(prev => ({ ...prev, type: value }))}
                  className="mt-2"
                >
                  <div className="flex items-center space-x-2">
                    <RadioGroupItem value="shift" id="shift-type" />
                    <Label htmlFor="shift-type">Shift-based | အလုပ်ပတ်အခြေခံ</Label>
                  </div>
                  <div className="flex items-center space-x-2">
                    <RadioGroupItem value="flexible" id="flexible-type" />
                    <Label htmlFor="flexible-type">Flexible | လိုက်လျောညီထွေ</Label>
                  </div>
                </RadioGroup>
              </div>

              {/* Basic Information */}
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label>Setup Name (English) | စနစ်အမည် (အင်္ဂလိပ်) *</Label>
                  <Input
                    value={formData.name}
                    onChange={(e) => setFormData(prev => ({ ...prev, name: e.target.value }))}
                    placeholder="Morning Shift / Engineering Flexible"
                    className="mt-1"
                  />
                </div>
                
                <div>
                  <Label>Setup Name (Myanmar) | စနစ်အမည် (မြန်မာ)</Label>
                  <Input
                    value={formData.nameMm}
                    onChange={(e) => setFormData(prev => ({ ...prev, nameMm: e.target.value }))}
                    placeholder="နံနက်ပတ် / အင်ဂျင်နီယာ လိုက်လျောညီထွေ"
                    className="mt-1"
                  />
                </div>
              </div>

              {/* Shift-specific fields */}
              {formData.type === 'shift' && (
                <>
                  <div className="grid grid-cols-3 gap-4">
                    <div>
                      <Label>Start Time | စတင်ချိန် *</Label>
                      <Input
                        type="time"
                        value={formData.timeFrom}
                        onChange={(e) => setFormData(prev => ({ ...prev, timeFrom: e.target.value }))}
                        className="mt-1"
                      />
                    </div>
                    
                    <div>
                      <Label>End Time | ပြီးဆုံးချိန် *</Label>
                      <Input
                        type="time"
                        value={formData.timeTo}
                        onChange={(e) => setFormData(prev => ({ ...prev, timeTo: e.target.value }))}
                        className="mt-1"
                      />
                    </div>
                    
                    <div>
                      <Label>Grace Period (minutes) | ခွင့်ပြုချိန် (မိနစ်)</Label>
                      <Input
                        type="number"
                        value={formData.gracePeriod}
                        onChange={(e) => setFormData(prev => ({ ...prev, gracePeriod: parseInt(e.target.value) || 0 }))}
                        className="mt-1"
                        min="0"
                        max="60"
                      />
                    </div>
                  </div>
                </>
              )}

              {/* Flexible-specific fields */}
              {formData.type === 'flexible' && (
                <>
                  <div className="grid grid-cols-2 gap-4">
                    <div>
                      <Label>Expected Hours per Week | အপတ်စဉ်မျှော်လင့်ချိန် *</Label>
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
                    
                    <div>
                      <Label>Allowed Time Range | ခွင့်ပြုအချိน်ကွာဟချက် (Optional)</Label>
                      <Input
                        value={formData.allowedTimeRange}
                        onChange={(e) => setFormData(prev => ({ ...prev, allowedTimeRange: e.target.value }))}
                        placeholder="08:00-20:00"
                        className="mt-1"
                      />
                      <div className="text-xs text-slate-500 mt-1">
                        Format: HH:MM-HH:MM (optional constraint)
                      </div>
                    </div>
                  </div>
                </>
              )}

              {/* Color Tag */}
              <div>
                <Label>Color Tag | အရောင်တဂ်</Label>
                <div className="mt-2 grid grid-cols-8 gap-2">
                  {colorOptions.map((color) => (
                    <button
                      key={color.value}
                      type="button"
                      onClick={() => setFormData(prev => ({ ...prev, colorTag: color.value }))}
                      className={`w-8 h-8 rounded-full border-2 ${
                        formData.colorTag === color.value ? 'border-slate-400' : 'border-slate-200'
                      }`}
                      style={{ backgroundColor: color.value }}
                      title={`${color.name} | ${color.nameMm}`}
                    />
                  ))}
                </div>
              </div>

              {/* Active Days */}
              <div>
                <Label>Active Days | တက်ကြွရက်များ *</Label>
                <div className="mt-2 grid grid-cols-4 gap-3">
                  {daysOfWeek.map((day) => (
                    <div key={day.id} className="flex items-center space-x-2">
                      <Checkbox
                        id={day.id}
                        checked={formData.activeDays.includes(day.id)}
                        onCheckedChange={(checked) => handleDayToggle(day.id, checked as boolean)}
                      />
                      <Label htmlFor={day.id} className="text-sm">
                        {day.name}
                      </Label>
                    </div>
                  ))}
                </div>
              </div>

              {/* Status */}
              <div>
                <Label>Status | အခြေအနေ</Label>
                <RadioGroup
                  value={formData.status}
                  onValueChange={(value) => setFormData(prev => ({ ...prev, status: value }))}
                  className="mt-2"
                >
                  <div className="flex items-center space-x-2">
                    <RadioGroupItem value="active" id="active-status" />
                    <Label htmlFor="active-status">Active | တက်ကြွ</Label>
                  </div>
                  <div className="flex items-center space-x-2">
                    <RadioGroupItem value="inactive" id="inactive-status" />
                    <Label htmlFor="inactive-status">Inactive | မတက်ကြွ</Label>
                  </div>
                </RadioGroup>
              </div>

              <Separator />

              {/* Action Buttons */}
              <div className="flex justify-end gap-3">
                <Button variant="outline" onClick={() => setIsFormOpen(false)}>
                  Cancel | ပယ်ဖျက်
                </Button>
                <Button onClick={handleSubmit} className="bg-purple-600 hover:bg-purple-700">
                  {editingSetup ? 'Update Setup' : 'Create Setup'} | 
                  {editingSetup ? ' စနစ်အပ်ဒေ့လုပ်' : ' စနစ်ဖန်တီး'}
                </Button>
              </div>
            </div>
          </DialogContent>
        </Dialog>
      </div>
    </div>
  );
}