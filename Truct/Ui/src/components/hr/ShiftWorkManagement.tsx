import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '../ui/card';
import { Button } from '../ui/button';
import { Badge } from '../ui/badge';
import { Input } from '../ui/input';
import { Label } from '../ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../ui/table';
import { Alert, AlertDescription } from '../ui/alert';
import { Avatar, AvatarFallback, AvatarImage } from '../ui/avatar';
import { toast } from 'sonner@2.0.3';
import { 
  CalendarDays,
  Users,
  AlertTriangle,
  Clock,
  CheckCircle,
  XCircle,
  Download,
  Filter,
  ChevronLeft,
  ChevronRight
} from 'lucide-react';

// Mock data for staff assignments
const mockAssignments = [
  {
    staffId: 'EMP-001',
    name: 'Ko Thant',
    nameMm: 'ကိုသန့်',
    department: 'Production',
    workType: 'shift',
    currentShift: 'Morning Shift',
    photo: null,
    weeklySchedule: {
      'Monday': 'Morning Shift',
      'Tuesday': 'Morning Shift',
      'Wednesday': 'Morning Shift',
      'Thursday': 'Morning Shift',
      'Friday': 'Morning Shift',
      'Saturday': 'Morning Shift',
      'Sunday': 'Off'
    },
    hoursWorked: 48,
    targetHours: 48,
    conflicts: []
  },
  {
    staffId: 'EMP-002',
    name: 'Ma Hla',
    nameMm: 'မလှ',
    department: 'Production',
    workType: 'shift',
    currentShift: 'Evening Shift',
    photo: null,
    weeklySchedule: {
      'Monday': 'Evening Shift',
      'Tuesday': 'Evening Shift',
      'Wednesday': 'Evening Shift',
      'Thursday': 'Evening Shift',
      'Friday': 'Evening Shift',
      'Saturday': 'Evening Shift',
      'Sunday': 'Off'
    },
    hoursWorked: 54,
    targetHours: 48,
    conflicts: ['Overtime']
  },
  {
    staffId: 'EMP-003',
    name: 'Ko Zaw',
    nameMm: 'ကိုဇော်',
    department: 'Engineering',
    workType: 'flexible',
    currentShift: null,
    photo: null,
    weeklySchedule: {
      'Monday': 'Flexible',
      'Tuesday': 'Flexible',
      'Wednesday': 'Flexible',
      'Thursday': 'Flexible',
      'Friday': 'Flexible',
      'Saturday': 'Off',
      'Sunday': 'Off'
    },
    hoursWorked: 35,
    targetHours: 40,
    conflicts: ['Underworked']
  },
  {
    staffId: 'EMP-004',
    name: 'Ma Su',
    nameMm: 'မစု',
    department: 'Quality Control',
    workType: 'flexible',
    currentShift: null,
    photo: null,
    weeklySchedule: {
      'Monday': 'Flexible',
      'Tuesday': 'Flexible',
      'Wednesday': 'Flexible',
      'Thursday': 'Flexible',
      'Friday': 'Flexible',
      'Saturday': 'Off',
      'Sunday': 'Off'
    },
    hoursWorked: 42,
    targetHours: 35,
    conflicts: ['Overworked']
  },
  {
    staffId: 'EMP-005',
    name: 'Ko Aung',
    nameMm: 'ကိုအောင်',
    department: 'Production',
    workType: 'shift',
    currentShift: 'Morning Shift',
    photo: null,
    weeklySchedule: {
      'Monday': 'Morning Shift',
      'Tuesday': 'Morning Shift',
      'Wednesday': 'Double Booked',
      'Thursday': 'Morning Shift',
      'Friday': 'Morning Shift',
      'Saturday': 'Morning Shift',
      'Sunday': 'Off'
    },
    hoursWorked: 48,
    targetHours: 48,
    conflicts: ['Double Booked']
  }
];

const shifts = [
  { id: 'morning', name: 'Morning Shift', nameMm: 'နံနက်ပတ်', color: '#10b981' },
  { id: 'evening', name: 'Evening Shift', nameMm: 'ညနေပတ်', color: '#f59e0b' },
  { id: 'night', name: 'Night Shift', nameMm: 'ညပတ်', color: '#6366f1' },
  { id: 'flexible', name: 'Flexible', nameMm: 'လိုက်လျောညီထွေ', color: '#8b5cf6' },
  { id: 'off', name: 'Off', nameMm: 'ပိတ်ရက်', color: '#6b7280' }
];

const daysOfWeek = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
const daysMm = ['တနင်္လာ', 'အင်္ဂါ', 'ဗုဒ္ဓဟူး', 'ကြာသပတေး', 'သောကြာ', 'စနေ', 'တနင်္ဂနွေ'];

export function ShiftWorkManagement() {
  const [assignments, setAssignments] = useState(mockAssignments);
  const [selectedWeek, setSelectedWeek] = useState(new Date());
  const [viewMode, setViewMode] = useState<'calendar' | 'table'>('calendar');
  const [filterDepartment, setFilterDepartment] = useState('all');
  const [filterWorkType, setFilterWorkType] = useState('all');

  // Calculate week start (Monday)
  const getWeekStart = (date: Date) => {
    const d = new Date(date);
    const day = d.getDay();
    const diff = d.getDate() - day + (day === 0 ? -6 : 1);
    return new Date(d.setDate(diff));
  };

  const weekStart = getWeekStart(selectedWeek);
  const weekDates = Array.from({ length: 7 }, (_, i) => {
    const date = new Date(weekStart);
    date.setDate(weekStart.getDate() + i);
    return date;
  });

  const navigateWeek = (direction: 'prev' | 'next') => {
    const newDate = new Date(selectedWeek);
    newDate.setDate(selectedWeek.getDate() + (direction === 'next' ? 7 : -7));
    setSelectedWeek(newDate);
  };

  const handleShiftChange = (staffId: string, day: string, newShift: string) => {
    setAssignments(prev => prev.map(assignment => {
      if (assignment.staffId === staffId) {
        const updatedSchedule = { ...assignment.weeklySchedule, [day]: newShift };
        
        // Check for conflicts
        const conflicts: string[] = [];
        
        // Check for double booking (same person assigned to multiple shifts on same day)
        if (newShift !== 'Off' && newShift !== 'Flexible') {
          const sameShiftCount = Object.values(updatedSchedule).filter(shift => shift === newShift).length;
          if (sameShiftCount > 6) {
            conflicts.push('Exceeding weekly limit');
          }
        }
        
        // Calculate hours
        let hoursWorked = 0;
        Object.values(updatedSchedule).forEach(shift => {
          if (shift === 'Morning Shift' || shift === 'Evening Shift' || shift === 'Night Shift') {
            hoursWorked += 8;
          } else if (shift === 'Flexible') {
            hoursWorked += assignment.targetHours / 5; // Assuming 5 flexible days
          }
        });
        
        // Check for overwork/underwork
        if (assignment.workType === 'flexible') {
          if (hoursWorked > assignment.targetHours * 1.1) {
            conflicts.push('Overworked');
          } else if (hoursWorked < assignment.targetHours * 0.9) {
            conflicts.push('Underworked');
          }
        } else {
          if (hoursWorked > 48) {
            conflicts.push('Overtime');
          }
        }
        
        return {
          ...assignment,
          weeklySchedule: updatedSchedule,
          hoursWorked: Math.round(hoursWorked),
          conflicts
        };
      }
      return assignment;
    }));
    
    toast.success(`Shift updated for ${assignments.find(a => a.staffId === staffId)?.name}`);
  };

  // Filter assignments
  const filteredAssignments = assignments.filter(assignment => {
    const matchesDept = filterDepartment === 'all' || assignment.department === filterDepartment;
    const matchesType = filterWorkType === 'all' || assignment.workType === filterWorkType;
    return matchesDept && matchesType;
  });

  const conflictCount = assignments.filter(a => a.conflicts.length > 0).length;
  const totalAssigned = assignments.length;
  const shiftStaff = assignments.filter(a => a.workType === 'shift').length;
  const flexibleStaff = assignments.filter(a => a.workType === 'flexible').length;

  return (
    <div className="p-6 bg-slate-50 min-h-screen">
      <div className="max-w-7xl mx-auto space-y-6">
        {/* Header */}
        <div className="flex items-center justify-between mb-6">
          <div className="flex items-center gap-3">
            <div className="p-2 bg-orange-100 rounded-lg">
              <CalendarDays className="h-6 w-6 text-orange-600" />
            </div>
            <div>
              <h1 className="text-3xl font-semibold text-slate-900">
                Shift & Work Management | အလုပ်ချိန်စီမံခန့်ခွဲမှု
              </h1>
              <p className="text-slate-600 mt-1">
                Weekly/monthly shift assignment and planning
              </p>
              <p className="text-sm text-slate-500">
                အပတ်စဉ်/လစဉ် အလုပ်ပတ်ခန့်အပ်မှုနှင့် အစီအစဉ်ချမှတ်ခြင်း
              </p>
            </div>
          </div>
          
          <div className="flex items-center gap-3">
            <Button variant="outline" className="gap-2">
              <Download className="h-4 w-4" />
              Export Schedule
            </Button>
            <Button variant="outline" className="gap-2">
              <Filter className="h-4 w-4" />
              Bulk Assign
            </Button>
          </div>
        </div>

        {/* Summary Stats */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
          <Card className="border-0 shadow-sm">
            <CardContent className="p-4">
              <div className="flex items-center gap-3">
                <div className="p-2 bg-blue-100 rounded-lg">
                  <Users className="w-4 h-4 text-blue-600" />
                </div>
                <div>
                  <div className="text-2xl font-bold text-blue-700">{totalAssigned}</div>
                  <div className="text-sm text-slate-600">Total Assigned</div>
                  <div className="text-xs text-slate-500">စုစုပေါင်းခန့်အပ်ထားသော</div>
                </div>
              </div>
            </CardContent>
          </Card>

          <Card className="border-0 shadow-sm">
            <CardContent className="p-4">
              <div className="flex items-center gap-3">
                <div className="p-2 bg-green-100 rounded-lg">
                  <Clock className="w-4 h-4 text-green-600" />
                </div>
                <div>
                  <div className="text-2xl font-bold text-green-700">{shiftStaff}</div>
                  <div className="text-sm text-slate-600">Shift Staff</div>
                  <div className="text-xs text-slate-500">အလုပ်ပတ်ဝန်ထမ်း</div>
                </div>
              </div>
            </CardContent>
          </Card>

          <Card className="border-0 shadow-sm">
            <CardContent className="p-4">
              <div className="flex items-center gap-3">
                <div className="p-2 bg-purple-100 rounded-lg">
                  <CheckCircle className="w-4 h-4 text-purple-600" />
                </div>
                <div>
                  <div className="text-2xl font-bold text-purple-700">{flexibleStaff}</div>
                  <div className="text-sm text-slate-600">Flexible Staff</div>
                  <div className="text-xs text-slate-500">လိုက်လျောညီထွေဝန်ထမ်း</div>
                </div>
              </div>
            </CardContent>
          </Card>

          <Card className="border-0 shadow-sm">
            <CardContent className="p-4">
              <div className="flex items-center gap-3">
                <div className="p-2 bg-red-100 rounded-lg">
                  <AlertTriangle className="w-4 h-4 text-red-600" />
                </div>
                <div>
                  <div className="text-2xl font-bold text-red-700">{conflictCount}</div>
                  <div className="text-sm text-slate-600">Conflicts</div>
                  <div className="text-xs text-slate-500">ပဋိပက္ခများ</div>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Week Navigation and View Toggle */}
        <Card className="border-0 shadow-sm">
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-4">
                <div className="flex items-center gap-2">
                  <Button variant="outline" size="sm" onClick={() => navigateWeek('prev')}>
                    <ChevronLeft className="h-4 w-4" />
                  </Button>
                  <div className="text-lg font-medium">
                    {weekStart.toLocaleDateString('en-US', { 
                      month: 'long', 
                      day: 'numeric' 
                    })} - {weekDates[6].toLocaleDateString('en-US', { 
                      month: 'long', 
                      day: 'numeric',
                      year: 'numeric'
                    })}
                  </div>
                  <Button variant="outline" size="sm" onClick={() => navigateWeek('next')}>
                    <ChevronRight className="h-4 w-4" />
                  </Button>
                </div>
                
                <Button 
                  variant="outline" 
                  size="sm" 
                  onClick={() => setSelectedWeek(new Date())}
                >
                  This Week | ဤအပတ်
                </Button>
              </div>
              
              <div className="flex items-center gap-3">
                <div className="flex items-center gap-2">
                  <Label className="text-sm">Department:</Label>
                  <Select value={filterDepartment} onValueChange={setFilterDepartment}>
                    <SelectTrigger className="w-[150px]">
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="all">All Departments</SelectItem>
                      <SelectItem value="Production">Production</SelectItem>
                      <SelectItem value="Engineering">Engineering</SelectItem>
                      <SelectItem value="Quality Control">Quality Control</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
                
                <div className="flex items-center gap-2">
                  <Label className="text-sm">Type:</Label>
                  <Select value={filterWorkType} onValueChange={setFilterWorkType}>
                    <SelectTrigger className="w-[120px]">
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="all">All Types</SelectItem>
                      <SelectItem value="shift">Shift</SelectItem>
                      <SelectItem value="flexible">Flexible</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Conflict Alerts */}
        {conflictCount > 0 && (
          <Alert className="border-red-200 bg-red-50">
            <AlertTriangle className="h-4 w-4 text-red-600" />
            <AlertDescription className="text-red-800">
              <strong>{conflictCount} scheduling conflicts detected:</strong> 
              {assignments.filter(a => a.conflicts.length > 0).map(staff => (
                <span key={staff.staffId} className="ml-2">
                  {staff.name} ({staff.conflicts.join(', ')})
                </span>
              ))}
            </AlertDescription>
          </Alert>
        )}

        {/* Weekly Calendar View */}
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <CalendarDays className="h-5 w-5 text-orange-600" />
              <div>
                <div>Weekly Schedule Calendar | အပတ်စဉ်အစီအစဉ်ပြက္ခဒိန်</div>
                <div className="text-sm text-slate-500">Drag and drop to assign shifts</div>
              </div>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <table className="w-full border-collapse">
                <thead>
                  <tr className="border-b border-slate-200">
                    <th className="text-left p-3 w-48">Staff | ဝန်ထမ်း</th>
                    {daysOfWeek.map((day, index) => (
                      <th key={day} className="text-center p-3 min-w-[120px]">
                        <div>{day}</div>
                        <div className="text-sm text-slate-500">{daysMm[index]}</div>
                        <div className="text-xs text-slate-400">
                          {weekDates[index].toLocaleDateString('en-US', { month: 'short', day: 'numeric' })}
                        </div>
                      </th>
                    ))}
                    <th className="text-center p-3">Hours | ချိန်</th>
                    <th className="text-center p-3">Status | အခြေအနေ</th>
                  </tr>
                </thead>
                <tbody>
                  {filteredAssignments.map((staff) => (
                    <tr key={staff.staffId} className={`border-b border-slate-100 ${staff.conflicts.length > 0 ? 'bg-red-50' : 'hover:bg-slate-50'}`}>
                      <td className="p-3">
                        <div className="flex items-center gap-3">
                          <Avatar className="h-8 w-8">
                            <AvatarImage src={staff.photo} />
                            <AvatarFallback className="text-xs">
                              {staff.name.split(' ').map(n => n[0]).join('')}
                            </AvatarFallback>
                          </Avatar>
                          <div>
                            <div className="font-medium">{staff.name}</div>
                            <div className="text-sm text-slate-500">
                              {staff.nameMm} ({staff.staffId})
                            </div>
                            <div className="text-xs text-slate-400">{staff.department}</div>
                          </div>
                        </div>
                      </td>
                      
                      {daysOfWeek.map((day) => {
                        const currentShift = staff.weeklySchedule[day];
                        const shift = shifts.find(s => s.name === currentShift);
                        
                        return (
                          <td key={day} className="p-2">
                            <Select 
                              value={currentShift} 
                              onValueChange={(value) => handleShiftChange(staff.staffId, day, value)}
                            >
                              <SelectTrigger className="w-full h-8 text-xs">
                                <SelectValue />
                              </SelectTrigger>
                              <SelectContent>
                                {shifts.map(s => (
                                  <SelectItem key={s.id} value={s.name}>
                                    <div className="flex items-center gap-2">
                                      <div 
                                        className="w-3 h-3 rounded-full" 
                                        style={{ backgroundColor: s.color }}
                                      ></div>
                                      {s.name}
                                    </div>
                                  </SelectItem>
                                ))}
                              </SelectContent>
                            </Select>
                          </td>
                        );
                      })}
                      
                      <td className="p-3 text-center">
                        <div className="font-medium">
                          {staff.hoursWorked}/{staff.targetHours}
                        </div>
                        <div className="text-xs text-slate-500">
                          {((staff.hoursWorked / staff.targetHours) * 100).toFixed(0)}%
                        </div>
                      </td>
                      
                      <td className="p-3 text-center">
                        {staff.conflicts.length > 0 ? (
                          <div className="space-y-1">
                            {staff.conflicts.map((conflict, index) => (
                              <Badge key={index} className="bg-red-100 text-red-800 text-xs">
                                {conflict}
                              </Badge>
                            ))}
                          </div>
                        ) : (
                          <Badge className="bg-green-100 text-green-800">
                            <CheckCircle className="h-3 w-3 mr-1" />
                            OK
                          </Badge>
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </CardContent>
        </Card>

        {/* Flexible Staff Management */}
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <CheckCircle className="h-5 w-5 text-purple-600" />
              <div>
                <div>Flexible Staff Hours Tracking | လိုက်လျောညီထွေဝန်ထမ်းချိန်များခြေရာခံမှု</div>
                <div className="text-sm text-slate-500">Monitor flexible staff work hours and compliance</div>
              </div>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Staff | ဝန်ထမ်း</TableHead>
                    <TableHead>Department | ဌာန</TableHead>
                    <TableHead>Expected Hours | မျှော်လင့်ချိန်</TableHead>
                    <TableHead>Hours Completed | ပြီးစီးချိန်</TableHead>
                    <TableHead>Balance | လက်ကျန်</TableHead>
                    <TableHead>Compliance | လိုက်နာမှု</TableHead>
                    <TableHead>Status | အခြေအနေ</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {assignments.filter(staff => staff.workType === 'flexible').map((staff) => {
                    const balance = staff.hoursWorked - staff.targetHours;
                    const compliance = (staff.hoursWorked / staff.targetHours) * 100;
                    
                    return (
                      <TableRow key={staff.staffId}>
                        <TableCell>
                          <div className="flex items-center gap-3">
                            <Avatar className="h-8 w-8">
                              <AvatarImage src={staff.photo} />
                              <AvatarFallback className="text-xs">
                                {staff.name.split(' ').map(n => n[0]).join('')}
                              </AvatarFallback>
                            </Avatar>
                            <div>
                              <div className="font-medium">{staff.name}</div>
                              <div className="text-sm text-slate-500">{staff.nameMm}</div>
                            </div>
                          </div>
                        </TableCell>
                        <TableCell>{staff.department}</TableCell>
                        <TableCell>
                          <div className="font-medium">{staff.targetHours} hrs/week</div>
                        </TableCell>
                        <TableCell>
                          <div className="font-medium">{staff.hoursWorked} hrs</div>
                        </TableCell>
                        <TableCell>
                          <div className={`font-medium ${balance > 0 ? 'text-orange-600' : balance < 0 ? 'text-red-600' : 'text-green-600'}`}>
                            {balance > 0 ? '+' : ''}{balance} hrs
                          </div>
                        </TableCell>
                        <TableCell>
                          <div className="flex items-center gap-2">
                            <div className="w-16 bg-slate-200 rounded-full h-2">
                              <div 
                                className={`h-2 rounded-full ${
                                  compliance >= 90 ? 'bg-green-500' : 
                                  compliance >= 70 ? 'bg-yellow-500' : 'bg-red-500'
                                }`}
                                style={{ width: `${Math.min(100, compliance)}%` }}
                              ></div>
                            </div>
                            <span className="text-sm font-medium">{compliance.toFixed(0)}%</span>
                          </div>
                        </TableCell>
                        <TableCell>
                          {staff.conflicts.includes('Overworked') ? (
                            <Badge className="bg-orange-100 text-orange-800">
                              <XCircle className="h-3 w-3 mr-1" />
                              Overworked | အလုပ်များ
                            </Badge>
                          ) : staff.conflicts.includes('Underworked') ? (
                            <Badge className="bg-red-100 text-red-800">
                              <XCircle className="h-3 w-3 mr-1" />
                              Underworked | အလုပ်နည်း
                            </Badge>
                          ) : (
                            <Badge className="bg-green-100 text-green-800">
                              <CheckCircle className="h-3 w-3 mr-1" />
                              On Track | လမ်းကြောင်းမှန်
                            </Badge>
                          )}
                        </TableCell>
                      </TableRow>
                    );
                  })}
                </TableBody>
              </Table>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}