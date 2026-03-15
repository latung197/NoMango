import React, { useState } from 'react';
import {
  BarChart3,
  FileText,
  Users,
  Calendar,
  Clock,
  Award,
  UserCheck,
  Search,
  Filter,
  Download,
  Eye,
  Edit,
  Plus,
  Minus,
  CheckCircle,
  XCircle,
  User,
  Settings,
  Trash2,
  Send,
  Save,
  Check,
  X,
  Camera,
  Printer,
  History,
  TrendingUp,
  TrendingDown,
  Activity,
  MapPin,
  Info,
  ExternalLink,
  Shuffle,
  UserPlus,
  RefreshCw,
  Star,
  Medal,
  Trophy,
  Target,
  Zap,
  AlertCircle,
  ChevronLeft,
  ChevronRight,
  MoreHorizontal
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

interface HRProps {
  currentPage: string;
  onPageChange: (page: string) => void;
}

export function HR({ currentPage, onPageChange }: HRProps) {
  const [selectedOperator, setSelectedOperator] = useState<string | null>(null);
  const [calendarView, setCalendarView] = useState<'week' | 'month'>('week');
  const [draggedOperator, setDraggedOperator] = useState<string | null>(null);
  const [showQRDialog, setShowQRDialog] = useState(false);
  const [showPermissionsDialog, setShowPermissionsDialog] = useState(false);

  // Mock data for operators with comprehensive info
  const mockOperators = [
    {
      id: 'OP001',
      name: 'Aung Kyaw',
      nameMM: 'အောင်ကျော်',
      photo: '/api/placeholder/48/48',
      employeeId: 'EMP001',
      rfidId: 'RFID001',
      department: 'Production',
      departmentMM: 'ထုတ်လုပ်မှုပိုင်း',
      assignedMachine: 'MCH-01',
      currentShift: 'Morning',
      currentShiftMM: 'မနက်ပိုင်း',
      task: 'Plastic Bottle Production',
      taskMM: 'ပလတ်စတစ်ပုလင်းထုတ်လုပ်မှု',
      status: 'working',
      outputToday: 1250,
      targetToday: 1200,
      efficiency: 104,
      experience: '3 years',
      skills: ['Injection Molding', 'Quality Control'],
      clockIn: '06:00',
      clockOut: null,
      totalHours: 8.5,
      overtime: 0.5
    },
    {
      id: 'OP002',
      name: 'Ma Thandar',
      nameMM: 'မသန္တာ',
      photo: '/api/placeholder/48/48',
      employeeId: 'EMP002',
      rfidId: 'RFID002',
      department: 'Quality Control',
      departmentMM: 'အရည်အသွေးထိန်းချုပ်မှု',
      assignedMachine: 'QC-STN-01',
      currentShift: 'Morning',
      currentShiftMM: 'မနက်ပိုင်း',
      task: 'Final Inspection',
      taskMM: 'နောက်ဆုံးစစ်ဆေးမှု',
      status: 'idle',
      outputToday: 890,
      targetToday: 1000,
      efficiency: 89,
      experience: '2 years',
      skills: ['Visual Inspection', 'Measurement'],
      clockIn: '06:00',
      clockOut: null,
      totalHours: 8.0,
      overtime: 0
    },
    {
      id: 'OP003',
      name: 'Ko Zaw Min',
      nameMM: 'ကိုဇော်မင်း',
      photo: '/api/placeholder/48/48',
      employeeId: 'EMP003',
      rfidId: 'RFID003',
      department: 'Production',
      departmentMM: 'ထုတ်လုပ်မှုပိုင်း',
      assignedMachine: 'MCH-02',
      currentShift: 'Evening',
      currentShiftMM: 'ညနေပိုင်း',
      task: 'Container Molding',
      taskMM: 'ကွန်တေနာပုံသွင်းမှု',
      status: 'absent',
      outputToday: 0,
      targetToday: 1100,
      efficiency: 0,
      experience: '5 years',
      skills: ['Advanced Molding', 'Machine Setup'],
      clockIn: null,
      clockOut: null,
      totalHours: 0,
      overtime: 0
    },
    {
      id: 'OP004',
      name: 'Ma Khin Soe',
      nameMM: 'မခင်စိုး',
      photo: '/api/placeholder/48/48',
      employeeId: 'EMP004',
      rfidId: 'RFID004',
      department: 'Packaging',
      departmentMM: 'ပုံးထည့်မှု',
      assignedMachine: 'PKG-01',
      currentShift: 'Night',
      currentShiftMM: 'ညပိုင်း',
      task: 'Product Packaging',
      taskMM: 'ထုတ်ကုန်ပုံးထည့်မှု',
      status: 'working',
      outputToday: 2100,
      targetToday: 2000,
      efficiency: 105,
      experience: '4 years',
      skills: ['Packaging', 'Labeling'],
      clockIn: '22:00',
      clockOut: null,
      totalHours: 8.0,
      overtime: 0
    }
  ];

  const mockShifts = [
    {
      id: 'shift-morning',
      name: 'Morning Shift',
      nameMM: 'မနက်ပိုင်းအလုပ်ပတ်',
      time: '06:00 - 14:00',
      machines: ['MCH-01', 'MCH-02', 'MCH-03', 'QC-STN-01', 'PKG-01'],
      operators: ['OP001', 'OP002'],
      date: '2025-09-07'
    },
    {
      id: 'shift-evening',
      name: 'Evening Shift',
      nameMM: 'ညနေပိုင်းအလုပ်ပတ်',
      time: '14:00 - 22:00',
      machines: ['MCH-01', 'MCH-02', 'PKG-01'],
      operators: ['OP003'],
      date: '2025-09-07'
    },
    {
      id: 'shift-night',
      name: 'Night Shift',
      nameMM: 'ညပိုင်းအလုပ်ပတ်',
      time: '22:00 - 06:00',
      machines: ['MCH-01', 'PKG-01'],
      operators: ['OP004'],
      date: '2025-09-07'
    }
  ];

  const mockAttendanceData = [
    {
      operatorId: 'OP001',
      name: 'Aung Kyaw',
      nameMM: 'အောင်ကျော်',
      date: '2025-09-07',
      clockIn: '06:00',
      clockOut: '14:30',
      totalHours: 8.5,
      overtime: 0.5,
      status: 'on-time',
      rfidScanIn: '05:58',
      rfidScanOut: '14:32'
    },
    {
      operatorId: 'OP002',
      name: 'Ma Thandar',
      nameMM: 'မသန္တာ',
      date: '2025-09-07',
      clockIn: '06:15',
      clockOut: '14:00',
      totalHours: 7.75,
      overtime: 0,
      status: 'late',
      rfidScanIn: '06:17',
      rfidScanOut: '14:01'
    },
    {
      operatorId: 'OP003',
      name: 'Ko Zaw Min',
      nameMM: 'ကိုဇော်မင်း',
      date: '2025-09-07',
      clockIn: null,
      clockOut: null,
      totalHours: 0,
      overtime: 0,
      status: 'absent',
      rfidScanIn: null,
      rfidScanOut: null,
      absenceReason: 'Sick Leave'
    }
  ];

  const mockProductivityData = [
    { operator: 'Aung Kyaw', daily: 1250, weekly: 8750, target: 1200, defects: 5, downtime: 15 },
    { operator: 'Ma Khin Soe', daily: 2100, weekly: 14700, target: 2000, defects: 3, downtime: 8 },
    { operator: 'Ma Thandar', daily: 890, weekly: 6230, target: 1000, defects: 2, downtime: 25 },
    { operator: 'Ko Zaw Min', daily: 0, weekly: 5500, target: 1100, defects: 8, downtime: 0 }
  ];

  const mockRolePermissions = [
    { role: 'Operator', operator: '✅ Log Production', technician: '❌', supervisor: '❌', hr: '❌', qrPrint: '❌' },
    { role: 'Technician', operator: '✅ Log Production', technician: '✅ Setup Machines', supervisor: '❌', hr: '❌', qrPrint: '✅' },
    { role: 'Supervisor', operator: '✅ Log Production', technician: '✅ Setup Machines', supervisor: '✅ Assign Tasks', hr: '❌', qrPrint: '✅' },
    { role: 'HR', operator: '❌', technician: '❌', supervisor: '❌', hr: '✅ Manage Staff', qrPrint: '❌' }
  ];

  const getStatusBadge = (status: string) => {
    const statusConfig: any = {
      'working': { color: 'bg-green-100 text-green-800', label: '🟢 Working | အလုပ်လုပ်နေ', icon: '🟢' },
      'idle': { color: 'bg-yellow-100 text-yellow-800', label: '🟡 Idle | ပစ်လစ်', icon: '🟡' },
      'absent': { color: 'bg-red-100 text-red-800', label: '🔴 Absent | မရှိ', icon: '🔴' },
      'on-time': { color: 'bg-green-100 text-green-800', label: '🟢 On-time | အချိန်မှန်', icon: '🟢' },
      'late': { color: 'bg-yellow-100 text-yellow-800', label: '🟡 Late | နောက်ကျ', icon: '🟡' },
      'break': { color: 'bg-blue-100 text-blue-800', label: 'Break | အနားယူချိန်', icon: null }
    };
    const config = statusConfig[status] || { color: 'bg-slate-100 text-slate-800', label: status, icon: null };
    return <Badge className={config.color}>{config.label}</Badge>;
  };

  const getEfficiencyColor = (efficiency: number) => {
    if (efficiency >= 100) return 'text-green-600';
    if (efficiency >= 90) return 'text-yellow-600';
    return 'text-red-600';
  };

  const renderOperatorDashboard = () => (
    <div className="space-y-6">
      {/* Quick Filters */}
      <div className="flex gap-4">
        <div className="flex-1">
          <div className="relative">
            <Search className="absolute left-3 top-3 h-4 w-4 text-muted-foreground" />
            <Input placeholder="Search operators..." className="pl-10" />
          </div>
        </div>
        <Select defaultValue="all">
          <SelectTrigger className="w-48">
            <SelectValue placeholder="Filter by department" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">All Departments</SelectItem>
            <SelectItem value="production">Production</SelectItem>
            <SelectItem value="quality">Quality Control</SelectItem>
            <SelectItem value="packaging">Packaging</SelectItem>
          </SelectContent>
        </Select>
        <Select defaultValue="all">
          <SelectTrigger className="w-48">
            <SelectValue placeholder="Filter by shift" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">All Shifts</SelectItem>
            <SelectItem value="morning">Morning</SelectItem>
            <SelectItem value="evening">Evening</SelectItem>
            <SelectItem value="night">Night</SelectItem>
          </SelectContent>
        </Select>
        <Select defaultValue="all">
          <SelectTrigger className="w-48">
            <SelectValue placeholder="Filter by machine" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">All Machines</SelectItem>
            <SelectItem value="mch-01">MCH-01</SelectItem>
            <SelectItem value="mch-02">MCH-02</SelectItem>
            <SelectItem value="qc-stn-01">QC-STN-01</SelectItem>
          </SelectContent>
        </Select>
      </div>

      {/* Operator Status Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
        {mockOperators.map((operator) => (
          <Card key={operator.id} className="hover:shadow-md transition-shadow cursor-pointer" 
                onClick={() => setSelectedOperator(operator.id)}>
            <CardHeader className="flex flex-row items-center space-y-0 pb-4">
              <div className="flex items-center gap-3">
                <div className="relative">
                  <div className="w-12 h-12 bg-slate-200 rounded-full flex items-center justify-center">
                    <User className="h-6 w-6 text-slate-500" />
                  </div>
                  <div className={`absolute -bottom-1 -right-1 w-4 h-4 rounded-full border-2 border-white ${
                    operator.status === 'working' ? 'bg-green-500' : 
                    operator.status === 'idle' ? 'bg-yellow-500' : 'bg-red-500'
                  }`}></div>
                </div>
                <div className="flex-1">
                  <div className="font-medium text-sm">{operator.name}</div>
                  <div className="text-xs text-muted-foreground">{operator.nameMM}</div>
                </div>
              </div>
            </CardHeader>
            <CardContent className="space-y-3">
              <div className="grid grid-cols-2 gap-2 text-xs">
                <div>
                  <span className="text-muted-foreground">ID:</span> {operator.employeeId}
                </div>
                <div>
                  <span className="text-muted-foreground">RFID:</span> {operator.rfidId}
                </div>
              </div>
              
              <div className="text-sm">
                <div className="flex items-center gap-1 mb-1">
                  <MapPin className="h-3 w-3 text-muted-foreground" />
                  <span className="font-medium">{operator.assignedMachine}</span>
                </div>
                <div className="text-xs text-muted-foreground">{operator.task}</div>
                <div className="text-xs text-muted-foreground">{operator.taskMM}</div>
              </div>

              <div className="flex items-center justify-between">
                <div className="text-xs">
                  <span className="text-muted-foreground">Shift:</span> {operator.currentShift}
                </div>
                {getStatusBadge(operator.status)}
              </div>

              {operator.status === 'working' && (
                <div className="space-y-2">
                  <div className="flex justify-between items-center text-xs">
                    <span>Output Today:</span>
                    <span className={`font-medium ${getEfficiencyColor(operator.efficiency)}`}>
                      {operator.outputToday}/{operator.targetToday} ({operator.efficiency}%)
                    </span>
                  </div>
                  <Progress value={operator.efficiency} className="h-2" />
                </div>
              )}
            </CardContent>
          </Card>
        ))}
      </div>

      {/* Operators Table View */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Users className="h-5 w-5" />
            Operator Status Table | အလုပ်သမားအခြေအနေဇယား
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-hidden rounded-lg border">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Operator</TableHead>
                  <TableHead>Machine</TableHead>
                  <TableHead>Shift</TableHead>
                  <TableHead>Task/Product</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Output Today</TableHead>
                  <TableHead>Efficiency</TableHead>
                  <TableHead>Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {mockOperators.map((operator) => (
                  <TableRow key={operator.id}>
                    <TableCell>
                      <div className="flex items-center gap-3">
                        <div className="relative">
                          <div className="w-8 h-8 bg-slate-200 rounded-full flex items-center justify-center">
                            <User className="h-4 w-4 text-slate-500" />
                          </div>
                          <div className={`absolute -bottom-0.5 -right-0.5 w-3 h-3 rounded-full border border-white ${
                            operator.status === 'working' ? 'bg-green-500' : 
                            operator.status === 'idle' ? 'bg-yellow-500' : 'bg-red-500'
                          }`}></div>
                        </div>
                        <div>
                          <div className="font-medium text-sm">{operator.name}</div>
                          <div className="text-xs text-muted-foreground">{operator.employeeId}</div>
                        </div>
                      </div>
                    </TableCell>
                    <TableCell className="font-medium">{operator.assignedMachine}</TableCell>
                    <TableCell>
                      <div>
                        <div className="text-sm">{operator.currentShift}</div>
                        <div className="text-xs text-muted-foreground">{operator.currentShiftMM}</div>
                      </div>
                    </TableCell>
                    <TableCell>
                      <div>
                        <div className="text-sm">{operator.task}</div>
                        <div className="text-xs text-muted-foreground max-w-32 truncate">{operator.taskMM}</div>
                      </div>
                    </TableCell>
                    <TableCell>{getStatusBadge(operator.status)}</TableCell>
                    <TableCell>
                      {operator.status === 'working' ? (
                        <div className="text-sm">
                          <div>{operator.outputToday}/{operator.targetToday}</div>
                          <div className="text-xs text-muted-foreground">pcs</div>
                        </div>
                      ) : (
                        <span className="text-muted-foreground">-</span>
                      )}
                    </TableCell>
                    <TableCell>
                      {operator.status === 'working' ? (
                        <div className="flex items-center gap-2">
                          <Progress value={operator.efficiency} className="w-16 h-2" />
                          <span className={`text-sm font-medium ${getEfficiencyColor(operator.efficiency)}`}>
                            {operator.efficiency}%
                          </span>
                        </div>
                      ) : (
                        <span className="text-muted-foreground">-</span>
                      )}
                    </TableCell>
                    <TableCell>
                      <div className="flex gap-2">
                        <Button size="sm" variant="outline" onClick={() => setSelectedOperator(operator.id)}>
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

  const renderShiftAssignment = () => (
    <div className="space-y-6">
      {/* Calendar Controls */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-4">
          <h2 className="text-lg font-semibold">Shift & Assignment Management | အလုပ်ပတ်နှင့်တာဝန်ချထားမှုစီမံခန့်ခွဲမှု</h2>
          <div className="flex items-center gap-2">
            <Button 
              variant={calendarView === 'week' ? 'default' : 'outline'} 
              size="sm"
              onClick={() => setCalendarView('week')}
            >
              Week
            </Button>
            <Button 
              variant={calendarView === 'month' ? 'default' : 'outline'} 
              size="sm"
              onClick={() => setCalendarView('month')}
            >
              Month
            </Button>
          </div>
        </div>
        <div className="flex gap-2">
          <Button variant="outline">
            <ChevronLeft className="h-4 w-4 mr-2" />
            Previous
          </Button>
          <Button variant="outline">
            <ChevronRight className="h-4 w-4 mr-2" />
            Next
          </Button>
          <Button>
            <Plus className="h-4 w-4 mr-2" />
            Add Assignment
          </Button>
        </div>
      </div>

      {/* Calendar View */}
      <Card>
        <CardHeader>
          <CardTitle>Weekly Schedule - September 7-13, 2025</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-8 gap-2">
            {/* Header */}
            <div className="font-medium text-sm p-2">Shift / Machine</div>
            {['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'].map(day => (
              <div key={day} className="font-medium text-sm p-2 text-center">{day}</div>
            ))}
            
            {/* Morning Shift */}
            <div className="p-2 bg-yellow-50 rounded">
              <div className="font-medium text-sm">Morning</div>
              <div className="text-xs text-muted-foreground">06:00-14:00</div>
            </div>
            {Array.from({ length: 7 }, (_, i) => (
              <div key={i} className="p-2 border border-dashed border-slate-300 rounded min-h-20">
                {i === 2 && ( // Wednesday
                  <div className="space-y-1">
                    <div className="text-xs bg-blue-100 text-blue-800 px-2 py-1 rounded">
                      MCH-01: Aung Kyaw
                    </div>
                    <div className="text-xs bg-green-100 text-green-800 px-2 py-1 rounded">
                      QC-STN-01: Ma Thandar
                    </div>
                  </div>
                )}
              </div>
            ))}
            
            {/* Evening Shift */}
            <div className="p-2 bg-orange-50 rounded">
              <div className="font-medium text-sm">Evening</div>
              <div className="text-xs text-muted-foreground">14:00-22:00</div>
            </div>
            {Array.from({ length: 7 }, (_, i) => (
              <div key={i} className="p-2 border border-dashed border-slate-300 rounded min-h-20">
                {i === 2 && ( // Wednesday
                  <div className="text-xs bg-purple-100 text-purple-800 px-2 py-1 rounded">
                    MCH-02: Ko Zaw Min
                  </div>
                )}
              </div>
            ))}
            
            {/* Night Shift */}
            <div className="p-2 bg-slate-100 rounded">
              <div className="font-medium text-sm">Night</div>
              <div className="text-xs text-muted-foreground">22:00-06:00</div>
            </div>
            {Array.from({ length: 7 }, (_, i) => (
              <div key={i} className="p-2 border border-dashed border-slate-300 rounded min-h-20">
                {i === 2 && ( // Wednesday
                  <div className="text-xs bg-indigo-100 text-indigo-800 px-2 py-1 rounded">
                    PKG-01: Ma Khin Soe
                  </div>
                )}
              </div>
            ))}
          </div>
        </CardContent>
      </Card>

      {/* Shift Planner Table */}
      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <CardTitle className="flex items-center gap-2">
            <Calendar className="h-5 w-5" />
            Shift Planner | အလုပ်ပတ်စီမံချက်
          </CardTitle>
          <div className="flex gap-2">
            <Button variant="outline">
              <Shuffle className="h-4 w-4 mr-2" />
              Reassign
            </Button>
            <Button variant="outline">
              <Trash2 className="h-4 w-4 mr-2" />
              Remove
            </Button>
          </div>
        </CardHeader>
        <CardContent>
          <div className="overflow-hidden rounded-lg border">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Machine</TableHead>
                  <TableHead>Operator</TableHead>
                  <TableHead>Department</TableHead>
                  <TableHead>Shift Time</TableHead>
                  <TableHead>Assigned Product</TableHead>
                  <TableHead>Task Notes</TableHead>
                  <TableHead>Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                <TableRow>
                  <TableCell className="font-medium">MCH-01</TableCell>
                  <TableCell>
                    <div className="flex items-center gap-2">
                      <div className="w-6 h-6 bg-slate-200 rounded-full flex items-center justify-center">
                        <User className="h-3 w-3" />
                      </div>
                      <div>
                        <div className="text-sm">Aung Kyaw</div>
                        <div className="text-xs text-muted-foreground">OP001</div>
                      </div>
                    </div>
                  </TableCell>
                  <TableCell>Production</TableCell>
                  <TableCell>
                    <Badge variant="secondary">Morning (06:00-14:00)</Badge>
                  </TableCell>
                  <TableCell>Plastic Bottle 500ml</TableCell>
                  <TableCell>Standard production run</TableCell>
                  <TableCell>
                    <div className="flex gap-2">
                      <Button size="sm" variant="outline">
                        <Edit className="h-4 w-4" />
                      </Button>
                      <Button size="sm" variant="outline">
                        <Shuffle className="h-4 w-4" />
                      </Button>
                    </div>
                  </TableCell>
                </TableRow>
                <TableRow>
                  <TableCell className="font-medium">QC-STN-01</TableCell>
                  <TableCell>
                    <div className="flex items-center gap-2">
                      <div className="w-6 h-6 bg-slate-200 rounded-full flex items-center justify-center">
                        <User className="h-3 w-3" />
                      </div>
                      <div>
                        <div className="text-sm">Ma Thandar</div>
                        <div className="text-xs text-muted-foreground">OP002</div>
                      </div>
                    </div>
                  </TableCell>
                  <TableCell>Quality Control</TableCell>
                  <TableCell>
                    <Badge variant="secondary">Morning (06:00-14:00)</Badge>
                  </TableCell>
                  <TableCell>Final Inspection</TableCell>
                  <TableCell>Visual & dimensional checks</TableCell>
                  <TableCell>
                    <div className="flex gap-2">
                      <Button size="sm" variant="outline">
                        <Edit className="h-4 w-4" />
                      </Button>
                      <Button size="sm" variant="outline">
                        <Shuffle className="h-4 w-4" />
                      </Button>
                    </div>
                  </TableCell>
                </TableRow>
                <TableRow>
                  <TableCell className="font-medium">PKG-01</TableCell>
                  <TableCell>
                    <div className="flex items-center gap-2">
                      <div className="w-6 h-6 bg-slate-200 rounded-full flex items-center justify-center">
                        <User className="h-3 w-3" />
                      </div>
                      <div>
                        <div className="text-sm">Ma Khin Soe</div>
                        <div className="text-xs text-muted-foreground">OP004</div>
                      </div>
                    </div>
                  </TableCell>
                  <TableCell>Packaging</TableCell>
                  <TableCell>
                    <Badge variant="outline">Night (22:00-06:00)</Badge>
                  </TableCell>
                  <TableCell>Product Packaging</TableCell>
                  <TableCell>Box packing & labeling</TableCell>
                  <TableCell>
                    <div className="flex gap-2">
                      <Button size="sm" variant="outline">
                        <Edit className="h-4 w-4" />
                      </Button>
                      <Button size="sm" variant="outline">
                        <Shuffle className="h-4 w-4" />
                      </Button>
                    </div>
                  </TableCell>
                </TableRow>
              </TableBody>
            </Table>
          </div>
        </CardContent>
      </Card>
    </div>
  );

  const renderAttendanceLogs = () => (
    <div className="space-y-6">
      {/* RFID Attendance Summary */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Present Today</CardTitle>
            <CheckCircle className="h-4 w-4 text-green-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-green-600">3/4</div>
            <p className="text-xs text-muted-foreground">ဒီနေ့ရှိနေသူများ</p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">On-Time</CardTitle>
            <Clock className="h-4 w-4 text-green-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-green-600">2</div>
            <p className="text-xs text-muted-foreground">အချိန်မှန်ရောက်ရှိသူများ</p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Late Arrival</CardTitle>
            <AlertCircle className="h-4 w-4 text-yellow-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-yellow-600">1</div>
            <p className="text-xs text-muted-foreground">နောက်ကျရောက်ရှိသူများ</p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Absent</CardTitle>
            <XCircle className="h-4 w-4 text-red-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-red-600">1</div>
            <p className="text-xs text-muted-foreground">မရှိသူများ</p>
          </CardContent>
        </Card>
      </div>

      {/* RFID-based Attendance Table */}
      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <CardTitle className="flex items-center gap-2">
            <Clock className="h-5 w-5" />
            RFID-based Attendance | RFID အခြေခံတက်ရောက်မှု
          </CardTitle>
          <div className="flex gap-2">
            <Button variant="outline">
              <Download className="h-4 w-4 mr-2" />
              Export PDF
            </Button>
            <Button variant="outline">
              <Download className="h-4 w-4 mr-2" />
              Export Excel
            </Button>
          </div>
        </CardHeader>
        <CardContent>
          <div className="overflow-hidden rounded-lg border">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Operator</TableHead>
                  <TableHead>Date</TableHead>
                  <TableHead>Clock In</TableHead>
                  <TableHead>Clock Out</TableHead>
                  <TableHead>Total Hours</TableHead>
                  <TableHead>Overtime</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>RFID Scan</TableHead>
                  <TableHead>Notes</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {mockAttendanceData.map((attendance) => (
                  <TableRow key={`${attendance.operatorId}-${attendance.date}`}>
                    <TableCell>
                      <div className="flex items-center gap-2">
                        <div className="w-8 h-8 bg-slate-200 rounded-full flex items-center justify-center">
                          <User className="h-4 w-4 text-slate-500" />
                        </div>
                        <div>
                          <div className="font-medium text-sm">{attendance.name}</div>
                          <div className="text-xs text-muted-foreground">{attendance.nameMM}</div>
                        </div>
                      </div>
                    </TableCell>
                    <TableCell>{attendance.date}</TableCell>
                    <TableCell className={attendance.status === 'late' ? 'text-yellow-600 font-medium' : ''}>
                      {attendance.clockIn || '-'}
                    </TableCell>
                    <TableCell>{attendance.clockOut || '-'}</TableCell>
                    <TableCell>
                      <div className="flex items-center gap-1">
                        <Clock className="h-3 w-3 text-muted-foreground" />
                        {attendance.totalHours}h
                      </div>
                    </TableCell>
                    <TableCell>
                      {attendance.overtime > 0 ? (
                        <Badge variant="secondary" className="bg-blue-100 text-blue-800">
                          +{attendance.overtime}h
                        </Badge>
                      ) : (
                        '-'
                      )}
                    </TableCell>
                    <TableCell>{getStatusBadge(attendance.status)}</TableCell>
                    <TableCell>
                      {attendance.rfidScanIn && attendance.rfidScanOut ? (
                        <div className="text-xs">
                          <div>In: {attendance.rfidScanIn}</div>
                          <div>Out: {attendance.rfidScanOut}</div>
                        </div>
                      ) : (
                        <span className="text-muted-foreground">-</span>
                      )}
                    </TableCell>
                    <TableCell>
                      {attendance.absenceReason ? (
                        <Badge variant="outline" className="text-red-600 border-red-200">
                          {attendance.absenceReason}
                        </Badge>
                      ) : (
                        '-'
                      )}
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        </CardContent>
      </Card>

      {/* Weekly Hours Summary */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <BarChart3 className="h-5 w-5" />
            Weekly Hours Summary | အပတ်စဉ်အလုပ်ချိန်အနှစ်ချုပ်
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            {mockOperators.map((operator) => (
              <div key={operator.id} className="flex items-center justify-between p-3 bg-slate-50 rounded-lg">
                <div className="flex items-center gap-3">
                  <div className="w-8 h-8 bg-slate-200 rounded-full flex items-center justify-center">
                    <User className="h-4 w-4 text-slate-500" />
                  </div>
                  <div>
                    <div className="font-medium text-sm">{operator.name}</div>
                    <div className="text-xs text-muted-foreground">{operator.department}</div>
                  </div>
                </div>
                <div className="flex items-center gap-6">
                  <div className="text-center">
                    <div className="text-sm font-medium">40h</div>
                    <div className="text-xs text-muted-foreground">Regular</div>
                  </div>
                  <div className="text-center">
                    <div className="text-sm font-medium text-blue-600">2h</div>
                    <div className="text-xs text-muted-foreground">Overtime</div>
                  </div>
                  <div className="text-center">
                    <div className="text-sm font-medium">42h</div>
                    <div className="text-xs text-muted-foreground">Total</div>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </CardContent>
      </Card>
    </div>
  );

  const renderProductivityReports = () => (
    <div className="space-y-6">
      {/* Top Performers */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Trophy className="h-5 w-5" />
            Top Performers Leaderboard | ထိပ်တန်းစွမ်းဆောင်ရည်မြင့်သူများ
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            {mockProductivityData
              .sort((a, b) => (b.daily / b.target) - (a.daily / a.target))
              .slice(0, 3)
              .map((operator, index) => (
                <div key={operator.operator} className="flex items-center justify-between p-4 bg-gradient-to-r from-slate-50 to-white rounded-lg border">
                  <div className="flex items-center gap-4">
                    <div className="flex items-center justify-center w-10 h-10 rounded-full bg-gradient-to-br from-yellow-400 to-orange-500">
                      {index === 0 && <span className="text-lg">🥇</span>}
                      {index === 1 && <span className="text-lg">🥈</span>}
                      {index === 2 && <span className="text-lg">🥉</span>}
                    </div>
                    <div>
                      <div className="font-medium">{operator.operator}</div>
                      <div className="text-sm text-muted-foreground">
                        Efficiency: {Math.round((operator.daily / operator.target) * 100)}%
                      </div>
                    </div>
                  </div>
                  <div className="text-right">
                    <div className="font-semibold text-lg text-green-600">{operator.daily}</div>
                    <div className="text-sm text-muted-foreground">/ {operator.target} target</div>
                  </div>
                </div>
              ))}
          </div>
        </CardContent>
      </Card>

      {/* Productivity Charts */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <BarChart3 className="h-5 w-5" />
              Daily Output Comparison | နေ့စဉ်ထုတ်လုပ်မှုနှိုင်းယှဉ်ခြင်း
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {mockProductivityData.map((operator) => {
                const percentage = Math.round((operator.daily / operator.target) * 100);
                return (
                  <div key={operator.operator} className="space-y-2">
                    <div className="flex justify-between items-center">
                      <span className="text-sm font-medium">{operator.operator}</span>
                      <span className={`text-sm font-medium ${getEfficiencyColor(percentage)}`}>
                        {operator.daily}/{operator.target} ({percentage}%)
                      </span>
                    </div>
                    <Progress value={percentage} className="h-3" />
                  </div>
                );
              })}
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Target className="h-5 w-5" />
              Quality Metrics | အရည်အသွေးတိုင်းတာမှု
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {mockProductivityData.map((operator) => (
                <div key={operator.operator} className="flex items-center justify-between p-3 bg-slate-50 rounded-lg">
                  <div>
                    <div className="font-medium text-sm">{operator.operator}</div>
                    <div className="text-xs text-muted-foreground">Quality Performance</div>
                  </div>
                  <div className="flex items-center gap-4 text-sm">
                    <div className="text-center">
                      <div className="font-medium text-red-600">{operator.defects}</div>
                      <div className="text-xs text-muted-foreground">Defects</div>
                    </div>
                    <div className="text-center">
                      <div className="font-medium text-orange-600">{operator.downtime}min</div>
                      <div className="text-xs text-muted-foreground">Downtime</div>
                    </div>
                    <div className="text-center">
                      <div className="font-medium text-green-600">
                        {Math.round(((operator.daily - operator.defects) / operator.daily) * 100)}%
                      </div>
                      <div className="text-xs text-muted-foreground">Quality Rate</div>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Detailed Productivity Table */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Activity className="h-5 w-5" />
            Detailed Productivity Report | အသေးစိတ်ထုတ်လုပ်နိုင်မှုအစီရင်ခံစာ
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-hidden rounded-lg border">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Operator</TableHead>
                  <TableHead>Daily Output</TableHead>
                  <TableHead>Weekly Output</TableHead>
                  <TableHead>Target %</TableHead>
                  <TableHead>Defects Linked</TableHead>
                  <TableHead>Downtime Events</TableHead>
                  <TableHead>Quality Rate</TableHead>
                  <TableHead>Performance</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {mockProductivityData.map((operator) => {
                  const dailyPercentage = Math.round((operator.daily / operator.target) * 100);
                  const qualityRate = Math.round(((operator.daily - operator.defects) / operator.daily) * 100);
                  
                  return (
                    <TableRow key={operator.operator}>
                      <TableCell className="font-medium">{operator.operator}</TableCell>
                      <TableCell>
                        <div className="text-sm">
                          <div>{operator.daily} pcs</div>
                          <div className="text-xs text-muted-foreground">Target: {operator.target}</div>
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="text-sm">
                          <div>{operator.weekly} pcs</div>
                          <div className="text-xs text-muted-foreground">Weekly total</div>
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-2">
                          <Progress value={dailyPercentage} className="w-16 h-2" />
                          <span className={`text-sm font-medium ${getEfficiencyColor(dailyPercentage)}`}>
                            {dailyPercentage}%
                          </span>
                        </div>
                      </TableCell>
                      <TableCell>
                        <Badge variant={operator.defects > 5 ? 'destructive' : operator.defects > 2 ? 'secondary' : 'default'}>
                          {operator.defects} defects
                        </Badge>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-1">
                          <Clock className="h-3 w-3 text-muted-foreground" />
                          {operator.downtime}min
                        </div>
                      </TableCell>
                      <TableCell>
                        <span className={`font-medium ${qualityRate >= 95 ? 'text-green-600' : qualityRate >= 90 ? 'text-yellow-600' : 'text-red-600'}`}>
                          {qualityRate}%
                        </span>
                      </TableCell>
                      <TableCell>
                        {dailyPercentage >= 100 && qualityRate >= 95 ? (
                          <Badge className="bg-green-100 text-green-800">Excellent</Badge>
                        ) : dailyPercentage >= 90 && qualityRate >= 90 ? (
                          <Badge className="bg-blue-100 text-blue-800">Good</Badge>
                        ) : (
                          <Badge className="bg-yellow-100 text-yellow-800">Needs Improvement</Badge>
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
  );

  const renderRoleAccessControl = () => (
    <div className="space-y-6">
      {/* Role Permissions Matrix */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <UserCheck className="h-5 w-5" />
            Role & Access Control Matrix | အခန်းကဏ္ဍနှင့်ဝင်ရောက်ခွင့်ထိန်းချုပ်မှုဇယား
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-hidden rounded-lg border">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead className="w-32">Role</TableHead>
                  <TableHead>Log Production</TableHead>
                  <TableHead>Setup Machines</TableHead>
                  <TableHead>Assign Tasks</TableHead>
                  <TableHead>Manage Staff</TableHead>
                  <TableHead>QR Print</TableHead>
                  <TableHead>View Reports</TableHead>
                  <TableHead>Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {mockRolePermissions.map((role) => (
                  <TableRow key={role.role}>
                    <TableCell className="font-medium">
                      <div className="flex items-center gap-2">
                        <UserCheck className="h-4 w-4" />
                        {role.role}
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center">
                        {role.operator.includes('✅') ? (
                          <Badge className="bg-green-100 text-green-800">Allowed</Badge>
                        ) : (
                          <Badge variant="outline" className="text-red-600 border-red-200">Denied</Badge>
                        )}
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center">
                        {role.technician.includes('✅') ? (
                          <Badge className="bg-green-100 text-green-800">Allowed</Badge>
                        ) : (
                          <Badge variant="outline" className="text-red-600 border-red-200">Denied</Badge>
                        )}
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center">
                        {role.supervisor.includes('✅') ? (
                          <Badge className="bg-green-100 text-green-800">Allowed</Badge>
                        ) : (
                          <Badge variant="outline" className="text-red-600 border-red-200">Denied</Badge>
                        )}
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center">
                        {role.hr.includes('✅') ? (
                          <Badge className="bg-green-100 text-green-800">Allowed</Badge>
                        ) : (
                          <Badge variant="outline" className="text-red-600 border-red-200">Denied</Badge>
                        )}
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center">
                        {role.qrPrint.includes('✅') ? (
                          <Badge className="bg-green-100 text-green-800">Allowed</Badge>
                        ) : (
                          <Badge variant="outline" className="text-red-600 border-red-200">Denied</Badge>
                        )}
                      </div>
                    </TableCell>
                    <TableCell>
                      <Badge className="bg-green-100 text-green-800">Allowed</Badge>
                    </TableCell>
                    <TableCell>
                      <div className="flex gap-2">
                        <Button size="sm" variant="outline" onClick={() => setShowPermissionsDialog(true)}>
                          <Edit className="h-4 w-4" />
                        </Button>
                        <Switch defaultChecked />
                      </div>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        </CardContent>
      </Card>

      {/* Operator Profile Cards */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Users className="h-5 w-5" />
            Operator Profiles with RFID | RFID ဖြင့်အလုပ်သမားပရိုဖိုင်များ
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {mockOperators.map((operator) => (
              <Card key={operator.id} className="hover:shadow-md transition-shadow">
                <CardHeader className="text-center pb-4">
                  <div className="relative mx-auto">
                    <div className="w-16 h-16 bg-slate-200 rounded-full flex items-center justify-center mx-auto">
                      <User className="h-8 w-8 text-slate-500" />
                    </div>
                    <div className={`absolute -bottom-2 -right-2 w-6 h-6 rounded-full border-2 border-white flex items-center justify-center text-xs ${
                      operator.status === 'working' ? 'bg-green-500' : 
                      operator.status === 'idle' ? 'bg-yellow-500' : 'bg-red-500'
                    }`}>
                      {operator.status === 'working' ? '✓' : operator.status === 'idle' ? '⏸' : '✕'}
                    </div>
                  </div>
                  <div>
                    <h3 className="font-semibold">{operator.name}</h3>
                    <p className="text-sm text-muted-foreground">{operator.nameMM}</p>
                  </div>
                </CardHeader>
                <CardContent className="space-y-3">
                  <div className="grid grid-cols-2 gap-2 text-sm">
                    <div>
                      <span className="text-muted-foreground">Employee ID:</span>
                      <div className="font-medium">{operator.employeeId}</div>
                    </div>
                    <div>
                      <span className="text-muted-foreground">RFID ID:</span>
                      <div className="font-medium flex items-center gap-1">
                        <div className="w-2 h-2 bg-blue-500 rounded-full"></div>
                        {operator.rfidId}
                      </div>
                    </div>
                  </div>
                  
                  <div className="text-sm">
                    <span className="text-muted-foreground">Department:</span>
                    <div className="font-medium">{operator.department}</div>
                    <div className="text-xs text-muted-foreground">{operator.departmentMM}</div>
                  </div>
                  
                  <div className="text-sm">
                    <span className="text-muted-foreground">Experience:</span>
                    <div className="font-medium">{operator.experience}</div>
                  </div>
                  
                  <div className="text-sm">
                    <span className="text-muted-foreground">Skills:</span>
                    <div className="flex flex-wrap gap-1 mt-1">
                      {operator.skills.map((skill, index) => (
                        <Badge key={index} variant="secondary" className="text-xs">
                          {skill}
                        </Badge>
                      ))}
                    </div>
                  </div>
                  
                  <div className="flex gap-2 pt-2">
                    <Button size="sm" variant="outline" className="flex-1">
                      <Edit className="h-3 w-3 mr-1" />
                      Edit
                    </Button>
                    <Button size="sm" variant="outline" className="flex-1">
                      <Eye className="h-3 w-3 mr-1" />
                      View
                    </Button>
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>
        </CardContent>
      </Card>

      {/* Permission Toggle Interface */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Settings className="h-5 w-5" />
            Permission Management | ခွင့်ပြုချက်စီမံခန့်ခွဲမှု
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div className="space-y-4">
                <h4 className="font-medium">Production Permissions</h4>
                <div className="space-y-3">
                  <div className="flex items-center justify-between">
                    <div>
                      <Label htmlFor="log-production">Log Production Data</Label>
                      <div className="text-sm text-muted-foreground">Allow logging production output</div>
                    </div>
                    <Switch id="log-production" defaultChecked />
                  </div>
                  <div className="flex items-center justify-between">
                    <div>
                      <Label htmlFor="qr-print">Print QR Codes</Label>
                      <div className="text-sm text-muted-foreground">Allow printing product QR codes</div>
                    </div>
                    <Switch id="qr-print" />
                  </div>
                  <div className="flex items-center justify-between">
                    <div>
                      <Label htmlFor="machine-setup">Machine Setup</Label>
                      <div className="text-sm text-muted-foreground">Allow machine configuration changes</div>
                    </div>
                    <Switch id="machine-setup" />
                  </div>
                </div>
              </div>
              
              <div className="space-y-4">
                <h4 className="font-medium">Administrative Permissions</h4>
                <div className="space-y-3">
                  <div className="flex items-center justify-between">
                    <div>
                      <Label htmlFor="assign-tasks">Assign Tasks</Label>
                      <div className="text-sm text-muted-foreground">Allow task assignment to operators</div>
                    </div>
                    <Switch id="assign-tasks" />
                  </div>
                  <div className="flex items-center justify-between">
                    <div>
                      <Label htmlFor="manage-staff">Manage Staff</Label>
                      <div className="text-sm text-muted-foreground">Allow staff management operations</div>
                    </div>
                    <Switch id="manage-staff" />
                  </div>
                  <div className="flex items-center justify-between">
                    <div>
                      <Label htmlFor="view-reports">View Reports</Label>
                      <div className="text-sm text-muted-foreground">Allow access to system reports</div>
                    </div>
                    <Switch id="view-reports" defaultChecked />
                  </div>
                </div>
              </div>
            </div>
            
            <div className="flex gap-4 pt-4">
              <Button>
                <Save className="h-4 w-4 mr-2" />
                Save Changes
              </Button>
              <Button variant="outline">
                <RefreshCw className="h-4 w-4 mr-2" />
                Reset to Default
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );

  const getPageTitle = () => {
    switch (currentPage) {
      case 'operator-dashboard':
        return {
          title: 'Operator Dashboard | အလুပ်သမားဒက်ရှ်ဘုတ်',
          subtitle: 'Real-time operator status monitoring with live productivity tracking and machine assignments.',
          subtitleMM: 'တိုက်ရိုက်ထုတ်လုပ်နိုင်မှုခြေရာခံမှုနှင့် စက်တာဝန်ချထားမှုများဖြင့် အလုပ်သမားအခြေအနေကို အချိန်နှင့်တပြေးညီ စောင့်ကြည့်မှု။'
        };
      case 'shift-assignment':
        return {
          title: 'Shift & Assignment Management | အလုပ်ပတ်နှင့်တာဝန်ချထားမှုစီမံခန့်ခွဲမှု',
          subtitle: 'Drag-and-drop shift planning with calendar view and operator-machine assignments.',
          subtitleMM: 'ဆွဲ-လွှတ် အလုပ်ပတ်စီမံချက်နှင့် ပြက္ခဒိန်မြင်ကွင်း အလုပ်သမား-စက်တာဝန်ချထားမှုများ။'
        };
      case 'attendance-logs':
        return {
          title: 'Attendance & Working Hour Logs | တက်ရောက်မှုနှင့်အလုပ်ချိန်မှတ်တမ်းများ',
          subtitle: 'RFID-based attendance tracking with automated clock in/out and overtime calculations.',
          subtitleMM: 'RFID အခြေခံတက်ရောက်မှုခြေရာခံမှု၊ အလိုအလျောက် ဝင်/ထွက် နှင့် ညလုပ်ချိန်တွက်ချက်မှုများ။'
        };
      case 'productivity-reports':
        return {
          title: 'Operator Productivity Reports | အလုပ်သမားထုတ်လုပ်နိုင်မှုအစီရင်ခံစာများ',
          subtitle: 'Comprehensive productivity analysis with leaderboards, quality metrics, and performance tracking.',
          subtitleMM: 'ထိပ်တန်းစာရင်းများ၊ အရည်အသွေးတိုင်းတာမှုများနှင့် စွမ်းဆောင်ရည်ခြေရာခံမှုများဖြင့် ပြည့်စုံသော ထုတ်လုပ်နိုင်မှုခွဲခြမ်းစိတ်ဖြာမှု။'
        };
      case 'role-access-control':
        return {
          title: 'Role & Access Control | အခန်းကဏ္ဍနှင့်ဝင်ရောက်ခွင့်ထိန်းချုပ်မှု',
          subtitle: 'Role-based permission management with RFID profiles and access control matrix.',
          subtitleMM: 'RFID ပရိုဖိုင်များနှင့် ဝင်ရောက်ခွင့်ထိန်းချုပ်မှုဇယားဖြင့် အခန်းကဏ္ဍအခြေခံ ခွင့်ပြုချက်စီမံခန့်ခွဲမှု။'
        };
      default:
        return {
          title: 'HR & Operator Management | လူ့စွမ်းအားနှင့်အော်ပရေတာစီမံခန့်ခွဲမှု',
          subtitle: 'Comprehensive workforce management system for smart plastic factory operations.',
          subtitleMM: 'စမတ်ပလတ်စတစ်စက်ရုံလုပ်ငန်းများအတွက် ပြည့်စုံသော လုပ်သားစီမံခန့်ခွဲမှုစနစ်။'
        };
    }
  };

  const renderCurrentPage = () => {
    switch (currentPage) {
      case 'operator-dashboard':
        return renderOperatorDashboard();
      case 'shift-assignment':
        return renderShiftAssignment();
      case 'attendance-logs':
        return renderAttendanceLogs();
      case 'productivity-reports':
        return renderProductivityReports();
      case 'role-access-control':
        return renderRoleAccessControl();
      default:
        return renderOperatorDashboard();
    }
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
          <p className="text-sm text-slate-500 mt-1">
            {pageInfo.subtitleMM}
          </p>
        </div>

        {/* Current Page Content */}
        {renderCurrentPage()}

        {/* QR Scanner Dialog */}
        <Dialog open={showQRDialog} onOpenChange={setShowQRDialog}>
          <DialogContent className="max-w-md">
            <DialogHeader>
              <DialogTitle className="flex items-center gap-2">
                <Camera className="h-5 w-5" />
                QR Code Scanner
              </DialogTitle>
              <DialogDescription>
                Position QR code within the frame to scan for operator identification and tracking.
              </DialogDescription>
            </DialogHeader>
            <div className="text-center p-8">
              <div className="w-48 h-48 bg-slate-100 rounded-lg mx-auto mb-4 flex items-center justify-center">
                <Camera className="h-16 w-16 text-slate-400" />
              </div>
              <p className="text-sm text-muted-foreground mb-4">
                Position QR code within the frame to scan
              </p>
              <Button onClick={() => setShowQRDialog(false)}>
                <X className="h-4 w-4 mr-2" />
                Close Scanner
              </Button>
            </div>
          </DialogContent>
        </Dialog>

        {/* Permissions Edit Dialog */}
        <Dialog open={showPermissionsDialog} onOpenChange={setShowPermissionsDialog}>
          <DialogContent className="max-w-2xl">
            <DialogHeader>
              <DialogTitle>Edit User Permissions</DialogTitle>
              <DialogDescription>
                Configure role-based permissions for HR and operator management operations.
              </DialogDescription>
            </DialogHeader>
            <div className="space-y-4">
              <p className="text-sm text-muted-foreground">
                Configure role-based permissions for HR operations.
              </p>
              {/* Permission editing interface would go here */}
              <div className="flex justify-end gap-2">
                <Button variant="outline" onClick={() => setShowPermissionsDialog(false)}>
                  Cancel
                </Button>
                <Button onClick={() => setShowPermissionsDialog(false)}>
                  Save Changes
                </Button>
              </div>
            </div>
          </DialogContent>
        </Dialog>
      </div>
    </div>
  );
}