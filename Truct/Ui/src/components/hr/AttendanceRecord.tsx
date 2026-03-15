import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '../ui/card';
import { Button } from '../ui/button';
import { Badge } from '../ui/badge';
import { Input } from '../ui/input';
import { Label } from '../ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../ui/table';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogTrigger } from '../ui/dialog';
import { Textarea } from '../ui/textarea';
import { Avatar, AvatarFallback, AvatarImage } from '../ui/avatar';
import { Separator } from '../ui/separator';
import { toast } from 'sonner@2.0.3';
import { 
  ClipboardCheck,
  Search,
  Download,
  Filter,
  Clock,
  Edit,
  CheckCircle,
  AlertTriangle,
  XCircle,
  Users,
  Calendar,
  FileText
} from 'lucide-react';

// Mock attendance data
const mockAttendanceData = [
  {
    id: 1,
    staffId: 'EMP-001',
    name: 'Ko Thant',
    nameMm: 'ကိုသန့်',
    department: 'Production',
    departmentMm: 'ထုတ်လုပ်မှု',
    workType: 'shift',
    shiftSetup: 'Morning Shift (08:00-17:00)',
    clockIn: '07:58',
    clockOut: '17:05',
    hoursWorked: 9.12,
    expectedHours: 8,
    lateMinutes: 0,
    overtimeMinutes: 5,
    status: 'on-time',
    statusMm: 'အချိန်မှန်',
    date: '2025-09-07',
    photo: null,
    remarks: null
  },
  {
    id: 2,
    staffId: 'EMP-002',
    name: 'Ma Hla',
    nameMm: 'မလှ',
    department: 'Production',
    departmentMm: 'ထုတ်လုပ်မှု',
    workType: 'shift',
    shiftSetup: 'Evening Shift (17:00-02:00)',
    clockIn: '17:12',
    clockOut: '02:03',
    hoursWorked: 8.85,
    expectedHours: 9,
    lateMinutes: 12,
    overtimeMinutes: 3,
    status: 'late',
    statusMm: 'နောက်ကျ',
    date: '2025-09-07',
    photo: null,
    remarks: 'Traffic jam on main road'
  },
  {
    id: 3,
    staffId: 'EMP-003',
    name: 'Ko Zaw',
    nameMm: 'ကိုဇော်',
    department: 'Engineering',
    departmentMm: 'အင်ဂျင်နီယာ',
    workType: 'flexible',
    shiftSetup: 'Flexible (40 hrs/week)',
    clockIn: '09:30',
    clockOut: '17:45',
    hoursWorked: 8.25,
    expectedHours: 8,
    lateMinutes: 0,
    overtimeMinutes: 15,
    status: 'on-time',
    statusMm: 'အချိန်မှန်',
    date: '2025-09-07',
    photo: null,
    remarks: null
  },
  {
    id: 4,
    staffId: 'EMP-004',
    name: 'Ma Su',
    nameMm: 'မစု',
    department: 'Quality Control',
    departmentMm: 'အရည်အသွေးထိန်းချုပ်မှု',
    workType: 'flexible',
    shiftSetup: 'Flexible (35 hrs/week)',
    clockIn: '10:15',
    clockOut: '16:30',
    hoursWorked: 6.25,
    expectedHours: 7,
    lateMinutes: 0,
    overtimeMinutes: 0,
    status: 'underworked',
    statusMm: 'အလုပ်နည်း',
    date: '2025-09-07',
    photo: null,
    remarks: 'Early finish due to low workload'
  },
  {
    id: 5,
    staffId: 'EMP-005',
    name: 'Ko Aung',
    nameMm: 'ကိုအောင်',
    department: 'Production',
    departmentMm: 'ထုတ်လုပ်မှု',
    workType: 'shift',
    shiftSetup: 'Morning Shift (08:00-17:00)',
    clockIn: null,
    clockOut: null,
    hoursWorked: 0,
    expectedHours: 8,
    lateMinutes: 0,
    overtimeMinutes: 0,
    status: 'absent',
    statusMm: 'မရှိ',
    date: '2025-09-07',
    photo: null,
    remarks: 'Sick leave'
  },
  {
    id: 6,
    staffId: 'EMP-006',
    name: 'Ma Aye',
    nameMm: 'မအေး',
    department: 'Administration',
    departmentMm: 'စီမံခန့်ခွဲမှု',
    workType: 'flexible',
    shiftSetup: 'Flexible (40 hrs/week)',
    clockIn: '08:45',
    clockOut: '18:30',
    hoursWorked: 9.75,
    expectedHours: 8,
    lateMinutes: 0,
    overtimeMinutes: 105,
    status: 'overtime',
    statusMm: 'အပိုချိန်',
    date: '2025-09-07',
    photo: null,
    remarks: 'End of month reporting'
  }
];

export function AttendanceRecord() {
  const [attendanceData, setAttendanceData] = useState(mockAttendanceData);
  const [selectedDate, setSelectedDate] = useState('2025-09-07');
  const [searchTerm, setSearchTerm] = useState('');
  const [filterDepartment, setFilterDepartment] = useState('all');
  const [filterStatus, setFilterStatus] = useState('all');
  const [filterWorkType, setFilterWorkType] = useState('all');
  const [editingRecord, setEditingRecord] = useState<any>(null);
  const [isEditOpen, setIsEditOpen] = useState(false);

  // Form state for manual corrections
  const [editForm, setEditForm] = useState({
    clockIn: '',
    clockOut: '',
    remarks: '',
    correctionReason: ''
  });

  const getStatusConfig = (status: string) => {
    switch (status) {
      case 'on-time':
        return { 
          color: 'bg-green-100 text-green-800', 
          icon: '🟢', 
          text: 'On Time', 
          textLocal: 'အချိန်မှန်' 
        };
      case 'late':
        return { 
          color: 'bg-yellow-100 text-yellow-800', 
          icon: '🟡', 
          text: 'Late', 
          textLocal: 'နောက်ကျ' 
        };
      case 'absent':
        return { 
          color: 'bg-red-100 text-red-800', 
          icon: '🔴', 
          text: 'Absent', 
          textLocal: 'မရှိ' 
        };
      case 'overtime':
        return { 
          color: 'bg-blue-100 text-blue-800', 
          icon: '🔵', 
          text: 'Overtime', 
          textLocal: 'အပိုချိန်' 
        };
      case 'underworked':
        return { 
          color: 'bg-orange-100 text-orange-800', 
          icon: '🟠', 
          text: 'Underworked', 
          textLocal: 'အလုပ်နည်း' 
        };
      default:
        return { 
          color: 'bg-gray-100 text-gray-800', 
          icon: '⚪', 
          text: 'Unknown', 
          textLocal: 'မသိ' 
        };
    }
  };

  const handleEditRecord = (record: any) => {
    setEditingRecord(record);
    setEditForm({
      clockIn: record.clockIn || '',
      clockOut: record.clockOut || '',
      remarks: record.remarks || '',
      correctionReason: ''
    });
    setIsEditOpen(true);
  };

  const handleSaveCorrection = () => {
    if (!editForm.correctionReason.trim()) {
      toast.error('Please provide a reason for the correction');
      return;
    }

    // Calculate new hours and status
    let newHoursWorked = 0;
    let newStatus = editingRecord.status;
    let lateMinutes = 0;
    let overtimeMinutes = 0;

    if (editForm.clockIn && editForm.clockOut) {
      const clockIn = new Date(`${selectedDate} ${editForm.clockIn}`);
      const clockOut = new Date(`${selectedDate} ${editForm.clockOut}`);
      
      // Handle overnight shifts
      if (clockOut < clockIn) {
        clockOut.setDate(clockOut.getDate() + 1);
      }
      
      newHoursWorked = (clockOut.getTime() - clockIn.getTime()) / (1000 * 60 * 60);
      
      // Calculate late minutes for shift workers
      if (editingRecord.workType === 'shift') {
        const expectedStart = new Date(`${selectedDate} ${editingRecord.shiftSetup.match(/\((\d{2}:\d{2})/)[1]}`);
        if (clockIn > expectedStart) {
          lateMinutes = Math.floor((clockIn.getTime() - expectedStart.getTime()) / (1000 * 60));
        }
      }
      
      // Calculate overtime
      if (newHoursWorked > editingRecord.expectedHours) {
        overtimeMinutes = Math.floor((newHoursWorked - editingRecord.expectedHours) * 60);
      }
      
      // Determine new status
      if (lateMinutes > 0) {
        newStatus = 'late';
      } else if (overtimeMinutes > 0) {
        newStatus = 'overtime';
      } else if (newHoursWorked < editingRecord.expectedHours * 0.9) {
        newStatus = 'underworked';
      } else {
        newStatus = 'on-time';
      }
    } else if (!editForm.clockIn && !editForm.clockOut) {
      newStatus = 'absent';
    }

    // Update attendance record
    setAttendanceData(prev => prev.map(record => 
      record.id === editingRecord.id ? {
        ...record,
        clockIn: editForm.clockIn || null,
        clockOut: editForm.clockOut || null,
        hoursWorked: newHoursWorked,
        lateMinutes,
        overtimeMinutes,
        status: newStatus,
        statusMm: getStatusConfig(newStatus).textLocal,
        remarks: editForm.remarks
      } : record
    ));

    toast.success('Attendance record updated successfully. Supervisor approval required.');
    setIsEditOpen(false);
    setEditingRecord(null);
  };

  // Filter attendance data
  const filteredData = attendanceData.filter(record => {
    const matchesSearch = 
      record.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
      record.nameMm.includes(searchTerm) ||
      record.staffId.toLowerCase().includes(searchTerm.toLowerCase());
    
    const matchesDepartment = filterDepartment === 'all' || record.department === filterDepartment;
    const matchesStatus = filterStatus === 'all' || record.status === filterStatus;
    const matchesWorkType = filterWorkType === 'all' || record.workType === filterWorkType;
    
    return matchesSearch && matchesDepartment && matchesStatus && matchesWorkType;
  });

  // Calculate summary stats
  const totalStaff = filteredData.length;
  const presentStaff = filteredData.filter(r => r.status !== 'absent').length;
  const lateStaff = filteredData.filter(r => r.status === 'late').length;
  const absentStaff = filteredData.filter(r => r.status === 'absent').length;
  const averageHours = filteredData.reduce((sum, r) => sum + r.hoursWorked, 0) / totalStaff;

  return (
    <div className="p-6 bg-slate-50 min-h-screen">
      <div className="max-w-7xl mx-auto space-y-6">
        {/* Header */}
        <div className="flex items-center justify-between mb-6">
          <div className="flex items-center gap-3">
            <div className="p-2 bg-indigo-100 rounded-lg">
              <ClipboardCheck className="h-6 w-6 text-indigo-600" />
            </div>
            <div>
              <h1 className="text-3xl font-semibold text-slate-900">
                Attendance Record | တက်ရောက်မှုမှတ်တမ်း
              </h1>
              <p className="text-slate-600 mt-1">
                Track daily attendance and working hours with RFID integration
              </p>
              <p className="text-sm text-slate-500">
                RFID ပေါင်းစပ်မှုနှင့်အတူ နေ့စဉ်တက်ရောက်မှုနှင့် အလုပ်ချိန်များကို ခြေရာခံခြင်း
              </p>
            </div>
          </div>
          
          <div className="flex items-center gap-3">
            <Button variant="outline" className="gap-2">
              <Download className="h-4 w-4" />
              Export PDF
            </Button>
            <Button variant="outline" className="gap-2">
              <FileText className="h-4 w-4" />
              Export Excel
            </Button>
          </div>
        </div>

        {/* Summary Stats */}
        <div className="grid grid-cols-1 md:grid-cols-5 gap-4 mb-6">
          <Card className="border-0 shadow-sm">
            <CardContent className="p-4">
              <div className="flex items-center gap-3">
                <div className="p-2 bg-green-100 rounded-lg">
                  <CheckCircle className="w-4 h-4 text-green-600" />
                </div>
                <div>
                  <div className="text-2xl font-bold text-green-700">{presentStaff}</div>
                  <div className="text-sm text-slate-600">Present</div>
                  <div className="text-xs text-slate-500">တက်ရောက်နေသော</div>
                </div>
              </div>
            </CardContent>
          </Card>

          <Card className="border-0 shadow-sm">
            <CardContent className="p-4">
              <div className="flex items-center gap-3">
                <div className="p-2 bg-yellow-100 rounded-lg">
                  <AlertTriangle className="w-4 h-4 text-yellow-600" />
                </div>
                <div>
                  <div className="text-2xl font-bold text-yellow-700">{lateStaff}</div>
                  <div className="text-sm text-slate-600">Late</div>
                  <div className="text-xs text-slate-500">နောက်ကျသော</div>
                </div>
              </div>
            </CardContent>
          </Card>

          <Card className="border-0 shadow-sm">
            <CardContent className="p-4">
              <div className="flex items-center gap-3">
                <div className="p-2 bg-red-100 rounded-lg">
                  <XCircle className="w-4 h-4 text-red-600" />
                </div>
                <div>
                  <div className="text-2xl font-bold text-red-700">{absentStaff}</div>
                  <div className="text-sm text-slate-600">Absent</div>
                  <div className="text-xs text-slate-500">မရှိသော</div>
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
                <div className="p-2 bg-purple-100 rounded-lg">
                  <Clock className="w-4 h-4 text-purple-600" />
                </div>
                <div>
                  <div className="text-2xl font-bold text-purple-700">{averageHours.toFixed(1)}</div>
                  <div className="text-sm text-slate-600">Avg Hours</div>
                  <div className="text-xs text-slate-500">ပျမ်းမျှချိန်</div>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Filters */}
        <Card className="border-0 shadow-sm">
          <CardContent className="p-4">
            <div className="grid grid-cols-1 md:grid-cols-6 gap-4">
              <div>
                <Label className="text-sm">Date | ရက်စွဲ</Label>
                <Input
                  type="date"
                  value={selectedDate}
                  onChange={(e) => setSelectedDate(e.target.value)}
                  className="mt-1"
                />
              </div>
              
              <div>
                <Label className="text-sm">Search | ရှာဖွေရန်</Label>
                <div className="relative mt-1">
                  <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-slate-400" />
                  <Input
                    placeholder="Name, ID..."
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
                    <SelectItem value="Production">Production</SelectItem>
                    <SelectItem value="Engineering">Engineering</SelectItem>
                    <SelectItem value="Quality Control">Quality Control</SelectItem>
                    <SelectItem value="Administration">Administration</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              
              <div>
                <Label className="text-sm">Work Type | အလုပ်အမျိုးအစား</Label>
                <Select value={filterWorkType} onValueChange={setFilterWorkType}>
                  <SelectTrigger className="mt-1">
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">All Types</SelectItem>
                    <SelectItem value="shift">Shift</SelectItem>
                    <SelectItem value="flexible">Flexible</SelectItem>
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
                    <SelectItem value="on-time">On Time</SelectItem>
                    <SelectItem value="late">Late</SelectItem>
                    <SelectItem value="absent">Absent</SelectItem>
                    <SelectItem value="overtime">Overtime</SelectItem>
                    <SelectItem value="underworked">Underworked</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              
              <div className="flex items-end">
                <Button variant="outline" className="gap-2 w-full">
                  <Filter className="h-4 w-4" />
                  Clear
                </Button>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Attendance Table */}
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <ClipboardCheck className="h-5 w-5 text-indigo-600" />
              <div>
                <div>Daily Attendance Record ({filteredData.length})</div>
                <div className="text-sm text-slate-500">နေ့စဉ်တက်ရောက်မှုမှတ်တမ်း ({filteredData.length})</div>
              </div>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Name | အမည်</TableHead>
                    <TableHead>Employee ID</TableHead>
                    <TableHead>Department | ဌာန</TableHead>
                    <TableHead>Work Type | အလုပ်အမျိုးအစား</TableHead>
                    <TableHead>Shift/Flexible Setup</TableHead>
                    <TableHead>Clock-In | ဝင်ချိန်</TableHead>
                    <TableHead>Clock-Out | ထွက်ချိန်</TableHead>
                    <TableHead>Hours | ချိန်</TableHead>
                    <TableHead>Late/OT | နောက်ကျ/အပိုချိန်</TableHead>
                    <TableHead>Status | အခြေအနေ</TableHead>
                    <TableHead>Remark | မှတ်ချက်</TableHead>
                    <TableHead>Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {filteredData.map((record) => {
                    const statusConfig = getStatusConfig(record.status);
                    
                    return (
                      <TableRow key={record.id}>
                        <TableCell>
                          <div className="flex items-center gap-3">
                            <Avatar className="h-8 w-8">
                              <AvatarImage src={record.photo} />
                              <AvatarFallback className="text-xs">
                                {record.name.split(' ').map(n => n[0]).join('')}
                              </AvatarFallback>
                            </Avatar>
                            <div>
                              <div className="font-medium">{record.name}</div>
                              <div className="text-sm text-slate-500">{record.nameMm}</div>
                            </div>
                          </div>
                        </TableCell>
                        <TableCell>
                          <div className="font-mono text-sm">{record.staffId}</div>
                        </TableCell>
                        <TableCell>
                          <div>
                            <div className="font-medium">{record.department}</div>
                            <div className="text-sm text-slate-500">{record.departmentMm}</div>
                          </div>
                        </TableCell>
                        <TableCell>
                          <Badge className={record.workType === 'shift' ? 'bg-blue-100 text-blue-800' : 'bg-purple-100 text-purple-800'}>
                            {record.workType === 'shift' ? 'Shift' : 'Flexible'}
                          </Badge>
                        </TableCell>
                        <TableCell>
                          <div className="text-sm">{record.shiftSetup}</div>
                        </TableCell>
                        <TableCell>
                          <div className="font-mono text-sm">
                            {record.clockIn || <span className="text-slate-400">--:--</span>}
                          </div>
                        </TableCell>
                        <TableCell>
                          <div className="font-mono text-sm">
                            {record.clockOut || <span className="text-slate-400">--:--</span>}
                          </div>
                        </TableCell>
                        <TableCell>
                          <div>
                            <div className="font-medium">{record.hoursWorked.toFixed(2)}h</div>
                            <div className="text-xs text-slate-500">
                              Expected: {record.expectedHours}h
                            </div>
                          </div>
                        </TableCell>
                        <TableCell>
                          <div>
                            {record.lateMinutes > 0 && (
                              <div className="text-xs text-red-600">Late: {record.lateMinutes}m</div>
                            )}
                            {record.overtimeMinutes > 0 && (
                              <div className="text-xs text-blue-600">OT: {record.overtimeMinutes}m</div>
                            )}
                            {record.lateMinutes === 0 && record.overtimeMinutes === 0 && (
                              <span className="text-slate-400">--</span>
                            )}
                          </div>
                        </TableCell>
                        <TableCell>
                          <Badge className={statusConfig.color}>
                            <span className="mr-1">{statusConfig.icon}</span>
                            <div className="text-center">
                              <div>{statusConfig.text}</div>
                              <div className="text-xs">{statusConfig.textLocal}</div>
                            </div>
                          </Badge>
                        </TableCell>
                        <TableCell>
                          <div className="text-sm max-w-[150px] truncate">
                            {record.remarks || <span className="text-slate-400">No remarks</span>}
                          </div>
                        </TableCell>
                        <TableCell>
                          <Button
                            variant="outline"
                            size="sm"
                            onClick={() => handleEditRecord(record)}
                            className="gap-1"
                          >
                            <Edit className="h-3 w-3" />
                            Correct
                          </Button>
                        </TableCell>
                      </TableRow>
                    );
                  })}
                </TableBody>
              </Table>
              
              {filteredData.length === 0 && (
                <div className="text-center py-8 text-slate-500">
                  <ClipboardCheck className="h-12 w-12 mx-auto mb-4 text-slate-300" />
                  <p>No attendance records found</p>
                  <p className="text-sm">တက်ရောက်မှုမှတ်တမ်းများမတွေ့ရှိပါ</p>
                </div>
              )}
            </div>
          </CardContent>
        </Card>

        {/* Manual Correction Dialog */}
        <Dialog open={isEditOpen} onOpenChange={setIsEditOpen}>
          <DialogContent className="max-w-2xl">
            <DialogHeader>
              <DialogTitle className="flex items-center gap-2">
                <Edit className="h-5 w-5 text-indigo-600" />
                Manual Attendance Correction
              </DialogTitle>
              <DialogDescription>
                Request manual correction for attendance record. Supervisor approval required.
              </DialogDescription>
            </DialogHeader>
            
            {editingRecord && (
              <div className="space-y-6 py-4">
                {/* Staff Information */}
                <div className="p-4 bg-slate-50 rounded-lg">
                  <h4 className="font-medium mb-2">Staff Information | ဝန်ထမ်းအချက်အလက်</h4>
                  <div className="grid grid-cols-2 gap-4 text-sm">
                    <div>
                      <span className="text-slate-600">Name:</span>
                      <span className="ml-2 font-medium">{editingRecord.name} ({editingRecord.nameMm})</span>
                    </div>
                    <div>
                      <span className="text-slate-600">Employee ID:</span>
                      <span className="ml-2 font-medium">{editingRecord.staffId}</span>
                    </div>
                    <div>
                      <span className="text-slate-600">Department:</span>
                      <span className="ml-2 font-medium">{editingRecord.department}</span>
                    </div>
                    <div>
                      <span className="text-slate-600">Work Setup:</span>
                      <span className="ml-2 font-medium">{editingRecord.shiftSetup}</span>
                    </div>
                  </div>
                </div>

                {/* Current vs New Times */}
                <div className="grid grid-cols-2 gap-6">
                  <div>
                    <h4 className="font-medium mb-3">Current Times | လက်ရှိအချိန်များ</h4>
                    <div className="space-y-2 text-sm">
                      <div className="flex justify-between">
                        <span className="text-slate-600">Clock-In:</span>
                        <span className="font-mono">{editingRecord.clockIn || 'Not recorded'}</span>
                      </div>
                      <div className="flex justify-between">
                        <span className="text-slate-600">Clock-Out:</span>
                        <span className="font-mono">{editingRecord.clockOut || 'Not recorded'}</span>
                      </div>
                      <div className="flex justify-between">
                        <span className="text-slate-600">Hours:</span>
                        <span className="font-medium">{editingRecord.hoursWorked.toFixed(2)}h</span>
                      </div>
                    </div>
                  </div>

                  <div>
                    <h4 className="font-medium mb-3">Corrected Times | ပြင်ဆင်သောအချိန်များ</h4>
                    <div className="space-y-3">
                      <div>
                        <Label className="text-sm">Clock-In Time | ဝင်ချိန်</Label>
                        <Input
                          type="time"
                          value={editForm.clockIn}
                          onChange={(e) => setEditForm(prev => ({ ...prev, clockIn: e.target.value }))}
                          className="mt-1"
                        />
                      </div>
                      
                      <div>
                        <Label className="text-sm">Clock-Out Time | ထွက်ချိန်</Label>
                        <Input
                          type="time"
                          value={editForm.clockOut}
                          onChange={(e) => setEditForm(prev => ({ ...prev, clockOut: e.target.value }))}
                          className="mt-1"
                        />
                      </div>
                    </div>
                  </div>
                </div>

                <div>
                  <Label className="text-sm">Correction Reason | ပြင်ဆင်ရသည့်အကြောင်းရင်း *</Label>
                  <Select value={editForm.correctionReason} onValueChange={(value) => setEditForm(prev => ({ ...prev, correctionReason: value }))}>
                    <SelectTrigger className="mt-1">
                      <SelectValue placeholder="Select reason..." />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="forgot-to-clock">Forgot to clock in/out | ချိန်မှတ်ဖို့မေ့သွားခြင်း</SelectItem>
                      <SelectItem value="system-error">System error | စနစ်အမှားအယွင်း</SelectItem>
                      <SelectItem value="rfid-malfunction">RFID card malfunction | RFID ကဒ်ပျက်ခြင်း</SelectItem>
                      <SelectItem value="emergency-leave">Emergency leave | အရေးပေါ်အလုပ်ပိတ်</SelectItem>
                      <SelectItem value="meeting-overtime">Extended meeting | အစည်းအဝေးကြာခြင်း</SelectItem>
                      <SelectItem value="other">Other reason | အခြားအကြောင်းရင်း</SelectItem>
                    </SelectContent>
                  </Select>
                </div>

                <div>
                  <Label className="text-sm">Additional Remarks | နောက်ထပ်မှတ်ချက်များ</Label>
                  <Textarea
                    value={editForm.remarks}
                    onChange={(e) => setEditForm(prev => ({ ...prev, remarks: e.target.value }))}
                    placeholder="Additional notes about the attendance correction..."
                    className="mt-1"
                    rows={3}
                  />
                </div>

                <Separator />

                {/* Action Buttons */}
                <div className="flex justify-end gap-3">
                  <Button variant="outline" onClick={() => setIsEditOpen(false)}>
                    Cancel | ပယ်ဖျက်
                  </Button>
                  <Button onClick={handleSaveCorrection} className="bg-indigo-600 hover:bg-indigo-700">
                    Submit Correction | ပြင်ဆင်မှုတင်သွင်း
                  </Button>
                </div>
              </div>
            )}
          </DialogContent>
        </Dialog>
      </div>
    </div>
  );
}