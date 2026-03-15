import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Badge } from './ui/badge';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Avatar, AvatarFallback, AvatarImage } from './ui/avatar';
import { Progress } from './ui/progress';
import { Separator } from './ui/separator';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from './ui/dialog';
import { Label } from './ui/label';
import { Textarea } from './ui/textarea';
import { ToggleGroup, ToggleGroupItem } from './ui/toggle-group';
import { AssignmentCalendarModal } from './AssignmentCalendarModal';
import { toast } from 'sonner';
import {
  Users,
  Factory,
  Search,
  Download,
  Plus,
  RotateCcw,
  RotateCw,
  User,
  UserPlus,
  UserX,
  RefreshCw,
  Monitor,
  Play,
  Pause,
  Power,
  AlertTriangle,
  Package,
  Calendar,
  Clock,
  CalendarDays,
  Sun,
  Moon,
  CalendarCheck
} from 'lucide-react';

// Mock data
const mockMachines = [
  {
    id: 'MCH-01',
    name: 'Injection Machine 1',
    status: 'running' as const,
    currentJob: {
      id: 'JOB-2024-001',
      product: 'Plastic Bottle 500ml',
      productImage: '/api/placeholder/80/80',
      targetQty: 10000,
      currentQty: 7500,
      progress: 75
    },
    operator: {
      id: 'OP-001',
      name: 'Ko Thant',
      nameLocal: 'ကိုသန့်',
      avatar: '/api/placeholder/40/40',
      department: 'Injection',
      shift: 'Day Shift'
    }
  },
  {
    id: 'MCH-02',
    name: 'Injection Machine 2',
    status: 'idle' as const,
    currentJob: {
      id: 'JOB-2024-002',
      product: 'Plastic Cup 250ml',
      productImage: '/api/placeholder/80/80',
      targetQty: 8000,
      currentQty: 8000,
      progress: 100
    },
    operator: {
      id: 'OP-002',
      name: 'Ma Hla',
      nameLocal: 'မလှ',
      avatar: '/api/placeholder/40/40',
      department: 'Injection',
      shift: 'Day Shift'
    }
  },
  {
    id: 'MCH-03',
    name: 'Injection Machine 3',
    status: 'down' as const,
    currentJob: null,
    operator: null
  },
  {
    id: 'MCH-04',
    name: 'Injection Machine 4',
    status: 'unassigned' as const,
    currentJob: {
      id: 'JOB-2024-004',
      product: 'Plastic Container 1L',
      productImage: '/api/placeholder/80/80',
      targetQty: 5000,
      currentQty: 0,
      progress: 0
    },
    operator: null
  },
  {
    id: 'MCH-05',
    name: 'Injection Machine 5',
    status: 'running' as const,
    currentJob: {
      id: 'JOB-2024-005',
      product: 'Plastic Tray 300x200mm',
      productImage: '/api/placeholder/80/80',
      targetQty: 3000,
      currentQty: 1800,
      progress: 60
    },
    operator: {
      id: 'OP-003',
      name: 'Ko Aung',
      nameLocal: 'ကိုအောင်',
      avatar: '/api/placeholder/40/40',
      department: 'Injection',
      shift: 'Day Shift'
    }
  },
  {
    id: 'MCH-06',
    name: 'Injection Machine 6',
    status: 'unassigned' as const,
    currentJob: null,
    operator: null
  }
];

const mockOperators = [
  {
    id: 'OP-001',
    name: 'Ko Thant',
    nameLocal: 'ကိုသန့်',
    avatar: '/api/placeholder/40/40',
    status: 'busy' as const,
    department: 'Injection',
    shift: 'Day Shift',
    currentMachine: 'MCH-01',
    skillLevel: 'Expert',
    experience: '5 years'
  },
  {
    id: 'OP-002',
    name: 'Ma Hla',
    nameLocal: 'မလှ',
    avatar: '/api/placeholder/40/40',
    status: 'busy' as const,
    department: 'Injection',
    shift: 'Day Shift',
    currentMachine: 'MCH-02',
    skillLevel: 'Advanced',
    experience: '3 years'
  },
  {
    id: 'OP-003',
    name: 'Ko Aung',
    nameLocal: 'ကိုအောင်',
    avatar: '/api/placeholder/40/40',
    status: 'busy' as const,
    department: 'Injection',
    shift: 'Day Shift',
    currentMachine: 'MCH-05',
    skillLevel: 'Intermediate',
    experience: '2 years'
  },
  {
    id: 'OP-004',
    name: 'Ma Su',
    nameLocal: 'မစု',
    avatar: '/api/placeholder/40/40',
    status: 'free' as const,
    department: 'Injection',
    shift: 'Day Shift',
    currentMachine: null,
    skillLevel: 'Advanced',
    experience: '4 years'
  },
  {
    id: 'OP-005',
    name: 'Ko Zaw',
    nameLocal: 'ကိုဇော်',
    avatar: '/api/placeholder/40/40',
    status: 'free' as const,
    department: 'Injection',
    shift: 'Day Shift',
    currentMachine: null,
    skillLevel: 'Expert',
    experience: '6 years'
  },
  {
    id: 'OP-006',
    name: 'Ma Aye',
    nameLocal: 'မအေး',
    avatar: '/api/placeholder/40/40',
    status: 'absent' as const,
    department: 'Injection',
    shift: 'Day Shift',
    currentMachine: null,
    skillLevel: 'Intermediate',
    experience: '1.5 years'
  },
  {
    id: 'OP-007',
    name: 'Ko Min',
    nameLocal: 'ကိုမင်း',
    avatar: '/api/placeholder/40/40',
    status: 'free' as const,
    department: 'Extrusion',
    shift: 'Day Shift',
    currentMachine: null,
    skillLevel: 'Advanced',
    experience: '3.5 years'
  }
];

interface Machine {
  id: string;
  name: string;
  status: 'running' | 'idle' | 'down' | 'unassigned';
  currentJob?: {
    id: string;
    product: string;
    productImage: string;
    targetQty: number;
    currentQty: number;
    progress: number;
  } | null;
  operator?: {
    id: string;
    name: string;
    nameLocal: string;
    avatar: string;
    department: string;
    shift: string;
  } | null;
}

interface Operator {
  id: string;
  name: string;
  nameLocal: string;
  avatar: string;
  status: 'free' | 'busy' | 'absent';
  department: string;
  shift: string;
  currentMachine: string | null;
  skillLevel: string;
  experience: string;
}

export function OperatorDashboard() {
  const [machines, setMachines] = useState<Machine[]>(mockMachines);
  const [operators, setOperators] = useState<Operator[]>(mockOperators);
  const [filterDepartment, setFilterDepartment] = useState('all');
  const [filterShift, setFilterShift] = useState('all');
  const [searchTerm, setSearchTerm] = useState('');
  const [draggedOperator, setDraggedOperator] = useState<Operator | null>(null);
  const [hoveredMachine, setHoveredMachine] = useState<string | null>(null);
  const [assignmentHistory, setAssignmentHistory] = useState<any[]>([]);
  
  // New state for enhanced assignment features
  const [assignmentMode, setAssignmentMode] = useState<'scheduled' | 'immediate'>('scheduled');
  const [showAssignModal, setShowAssignModal] = useState(false);
  const [showCalendarModal, setShowCalendarModal] = useState(false);
  const [selectedMachine, setSelectedMachine] = useState<Machine | null>(null);
  const [selectedOperatorId, setSelectedOperatorId] = useState('');
  const [assignmentNote, setAssignmentNote] = useState('');
  const [calendarView, setCalendarView] = useState<'week' | 'month'>('week');
  const [scheduledAssignments, setScheduledAssignments] = useState<any[]>([]);
  const [selectedDate, setSelectedDate] = useState<Date>(new Date());
  const [selectedShift, setSelectedShift] = useState<'day' | 'night'>('day');
  const [selectedDuration, setSelectedDuration] = useState<'week' | 'month' | 'custom'>('week');

  // Calculate stats
  const availableOperators = operators.filter(op => op.status === 'free').length;
  const busyOperators = operators.filter(op => op.status === 'busy').length;
  const absentOperators = operators.filter(op => op.status === 'absent').length;
  const todayReassignments = assignmentHistory.filter(h => 
    h.timestamp.startsWith(new Date().toISOString().split('T')[0])
  ).length;

  const getStatusConfig = (status: string) => {
    switch (status) {
      case 'running':
        return { color: 'bg-green-100 text-green-800 border-green-200', icon: '🟢', text: 'Running', textLocal: 'လုပ်ဆောင်နေ' };
      case 'idle':
        return { color: 'bg-yellow-100 text-yellow-800 border-yellow-200', icon: '🟡', text: 'Idle', textLocal: 'စောင့်နေ' };
      case 'down':
        return { color: 'bg-red-100 text-red-800 border-red-200', icon: '🔴', text: 'Down', textLocal: 'ပျက်စီး' };
      case 'unassigned':
        return { color: 'bg-gray-100 text-gray-800 border-gray-200', icon: '⚪', text: 'Unassigned', textLocal: 'မခန့်အပ်ရသေး' };
      default:
        return { color: 'bg-gray-100 text-gray-800 border-gray-200', icon: '⚪', text: 'Unknown', textLocal: 'မသိ' };
    }
  };

  const getOperatorStatusConfig = (status: string) => {
    switch (status) {
      case 'free':
        return { color: 'bg-green-100 text-green-800 border-green-200', icon: '🟢', text: 'Free', textLocal: 'လွတ်' };
      case 'busy':
        return { color: 'bg-yellow-100 text-yellow-800 border-yellow-200', icon: '🟡', text: 'Busy', textLocal: 'အလုပ်ရှုပ်' };
      case 'absent':
        return { color: 'bg-red-100 text-red-800 border-red-200', icon: '🔴', text: 'Absent', textLocal: 'မရှိ' };
      default:
        return { color: 'bg-gray-100 text-gray-800 border-gray-200', icon: '⚪', text: 'Unknown', textLocal: 'မသိ' };
    }
  };

  const filteredOperators = operators.filter(operator => {
    const matchesSearch = operator.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         operator.nameLocal.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         operator.id.toLowerCase().includes(searchTerm.toLowerCase());
    
    const matchesDepartment = filterDepartment === 'all' || operator.department === filterDepartment;
    const matchesShift = filterShift === 'all' || operator.shift === filterShift;
    
    return matchesSearch && matchesDepartment && matchesShift;
  });

  const handleDragStart = (operator: Operator) => {
    setDraggedOperator(operator);
  };

  const handleDragEnd = () => {
    setDraggedOperator(null);
    setHoveredMachine(null);
  };

  const handleDragOver = (e: React.DragEvent, machineId: string) => {
    e.preventDefault();
    setHoveredMachine(machineId);
  };

  const handleDragLeave = () => {
    setHoveredMachine(null);
  };

  const handleDrop = (e: React.DragEvent, machineId: string) => {
    e.preventDefault();
    if (!draggedOperator) return;

    const machine = machines.find(m => m.id === machineId);
    if (!machine) return;

    // Check if operator is already assigned to another machine
    if (draggedOperator.status === 'busy' && draggedOperator.currentMachine) {
      // This would create a swap - release previous machine
      setMachines(prev => prev.map(m => 
        m.id === draggedOperator.currentMachine 
          ? { ...m, operator: null }
          : m.id === machineId
          ? { ...m, operator: {
              id: draggedOperator.id,
              name: draggedOperator.name,
              nameLocal: draggedOperator.nameLocal,
              avatar: draggedOperator.avatar,
              department: draggedOperator.department,
              shift: draggedOperator.shift
            }}
          : m
      ));
    } else {
      // Simple assignment
      setMachines(prev => prev.map(m => 
        m.id === machineId
          ? { ...m, operator: {
              id: draggedOperator.id,
              name: draggedOperator.name,
              nameLocal: draggedOperator.nameLocal,
              avatar: draggedOperator.avatar,
              department: draggedOperator.department,
              shift: draggedOperator.shift
            }}
          : m
      ));
    }

    // Update operator status
    setOperators(prev => prev.map(op => 
      op.id === draggedOperator.id
        ? { ...op, status: 'busy' as const, currentMachine: machineId }
        : op
    ));

    // Add to assignment history
    const newAssignment = {
      id: Date.now(),
      operatorId: draggedOperator.id,
      operatorName: draggedOperator.name,
      machineId: machineId,
      machineName: machine.name,
      timestamp: new Date().toISOString(),
      action: 'assign'
    };
    setAssignmentHistory(prev => [newAssignment, ...prev]);

    toast.success(`✅ ${draggedOperator.name} assigned to ${machine.name} | ${draggedOperator.nameLocal} ကို ${machine.name} တွင်ခန့်အပ်ပြီး`);

    setDraggedOperator(null);
    setHoveredMachine(null);
  };

  const handleUnassignOperator = (machineId: string) => {
    const machine = machines.find(m => m.id === machineId);
    if (!machine?.operator) return;

    // Remove operator from machine
    setMachines(prev => prev.map(m => 
      m.id === machineId ? { ...m, operator: null } : m
    ));

    // Update operator status to free
    setOperators(prev => prev.map(op => 
      op.id === machine.operator!.id
        ? { ...op, status: 'free' as const, currentMachine: null }
        : op
    ));

    // Add to assignment history
    const newAssignment = {
      id: Date.now(),
      operatorId: machine.operator.id,
      operatorName: machine.operator.name,
      machineId: machineId,
      machineName: machine.name,
      timestamp: new Date().toISOString(),
      action: 'unassign'
    };
    setAssignmentHistory(prev => [newAssignment, ...prev]);

    toast.success(`✅ ${machine.operator.name} unassigned from ${machine.name} | ${machine.operator.nameLocal} ကို ${machine.name} မှ ဖယ်ရှားပြီး`);
  };

  const isRecommendedOperator = (operator: Operator, machineId: string) => {
    if (!draggedOperator || draggedOperator.id !== operator.id) return false;
    
    const machine = machines.find(m => m.id === machineId);
    if (!machine) return false;

    // Simple recommendation logic - expert operators for running machines
    if (machine.status === 'running' && operator.skillLevel === 'Expert') return true;
    if (machine.status === 'down' && operator.skillLevel === 'Expert') return true;
    if (machine.status === 'unassigned' && operator.status === 'free') return true;
    
    return false;
  };

  // Handle immediate assignment
  const handleImmediateAssign = () => {
    if (!selectedMachine || !selectedOperatorId) return;
    
    const operator = operators.find(op => op.id === selectedOperatorId);
    if (!operator) return;

    // Update machines
    setMachines(prev => prev.map(m => 
      m.id === selectedMachine.id
        ? { ...m, operator: {
            id: operator.id,
            name: operator.name,
            nameLocal: operator.nameLocal,
            avatar: operator.avatar,
            department: operator.department,
            shift: operator.shift
          }}
        : m
    ));

    // Update operators
    setOperators(prev => prev.map(op => 
      op.id === operator.id
        ? { ...op, status: 'busy' as const, currentMachine: selectedMachine.id }
        : op
    ));

    toast.success(`✅ ${operator.name} assigned to ${selectedMachine.name} immediately`);
    
    setShowAssignModal(false);
    setSelectedMachine(null);
    setSelectedOperatorId('');
    setAssignmentNote('');
  };

  // Generate week dates for calendar
  const generateWeekDates = () => {
    const dates = [];
    const today = new Date();
    const currentDay = today.getDay();
    const diff = currentDay === 0 ? -6 : 1 - currentDay; // Start from Monday
    
    for (let i = 0; i < 7; i++) {
      const date = new Date(today);
      date.setDate(today.getDate() + diff + i);
      dates.push(date);
    }
    return dates;
  };

  // Generate month dates for calendar
  const generateMonthDates = () => {
    const dates = [];
    const today = new Date();
    const firstDay = new Date(today.getFullYear(), today.getMonth(), 1);
    const lastDay = new Date(today.getFullYear(), today.getMonth() + 1, 0);
    
    for (let i = 1; i <= lastDay.getDate(); i++) {
      dates.push(new Date(today.getFullYear(), today.getMonth(), i));
    }
    return dates;
  };

  // Get machine card background color based on status
  const getMachineCardBackground = (machine: Machine) => {
    if (machine.operator && machine.status === 'running') {
      return 'bg-gradient-to-br from-green-50 to-green-100';
    } else if (machine.operator && machine.status === 'idle') {
      return 'bg-gradient-to-br from-yellow-50 to-yellow-100';
    } else if (!machine.operator) {
      return 'bg-gradient-to-br from-red-50 to-red-100';
    }
    return 'bg-white';
  };

  return (
    <div className="min-h-screen bg-slate-50">
      {/* Top Bar */}
      <div className="bg-white border-b border-slate-200 px-6 py-4">
        <div className="flex justify-between items-start mb-4">
          <div>
            <h1 className="text-2xl font-semibold text-slate-900">
              Operator Dashboard | အော်ပရေတာဒက်ရှ်ဘုတ်
            </h1>
            <p className="text-slate-600 mt-1">
              Real-time operator assignment management
            </p>
            <p className="text-sm text-slate-500">
              အော်ပရေတာများ တိုက်ရိုက်ခန့်အပ်မှု စီမံခန့်ခွဲမှု
            </p>
          </div>

          <div className="flex items-center gap-3">
            <Button variant="outline" className="gap-2">
              <Download className="h-4 w-4" />
              Export Log
            </Button>
          </div>
        </div>

        {/* Quick Stats Row */}
        <div className="grid grid-cols-4 gap-4 mb-4">
          <Card className="p-3">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-green-100 rounded-lg">
                <Users className="h-4 w-4 text-green-600" />
              </div>
              <div>
                <div className="text-lg font-semibold text-green-700">{availableOperators}</div>
                <div className="text-sm text-slate-600">Available</div>
                <div className="text-xs text-slate-500">ရရှိနိုင်သော</div>
              </div>
            </div>
          </Card>

          <Card className="p-3">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-yellow-100 rounded-lg">
                <User className="h-4 w-4 text-yellow-600" />
              </div>
              <div>
                <div className="text-lg font-semibold text-yellow-700">{busyOperators}</div>
                <div className="text-sm text-slate-600">Busy</div>
                <div className="text-xs text-slate-500">အလုပ်ရှုပ်နေသော</div>
              </div>
            </div>
          </Card>

          <Card className="p-3">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-red-100 rounded-lg">
                <UserX className="h-4 w-4 text-red-600" />
              </div>
              <div>
                <div className="text-lg font-semibold text-red-700">{absentOperators}</div>
                <div className="text-sm text-slate-600">Absent</div>
                <div className="text-xs text-slate-500">မရှိသော</div>
              </div>
            </div>
          </Card>

          <Card className="p-3">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-blue-100 rounded-lg">
                <RefreshCw className="h-4 w-4 text-blue-600" />
              </div>
              <div>
                <div className="text-lg font-semibold text-blue-700">{todayReassignments}</div>
                <div className="text-sm text-slate-600">Reassignments Today</div>
                <div className="text-xs text-slate-500">ယနေ့ပြောင်းလဲမှုများ</div>
              </div>
            </div>
          </Card>
        </div>

        {/* Filters */}
        <div className="flex items-center justify-between gap-4">
          <div className="flex items-center gap-4">
            <Select value={filterDepartment} onValueChange={setFilterDepartment}>
              <SelectTrigger className="w-48">
                <SelectValue placeholder="Filter by Department" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">All Departments</SelectItem>
                <SelectItem value="Injection">Injection</SelectItem>
                <SelectItem value="Extrusion">Extrusion</SelectItem>
                <SelectItem value="Assembly">Assembly</SelectItem>
              </SelectContent>
            </Select>

            <Select value={filterShift} onValueChange={setFilterShift}>
              <SelectTrigger className="w-48">
                <SelectValue placeholder="Filter by Shift" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">All Shifts</SelectItem>
                <SelectItem value="Day Shift">Day Shift</SelectItem>
                <SelectItem value="Night Shift">Night Shift</SelectItem>
              </SelectContent>
            </Select>
          </div>

          {/* Assignment Mode Toggle */}
          <div className="flex items-center gap-3">
            <Button
              variant="outline"
              className="gap-2"
              onClick={() => setShowCalendarModal(true)}
            >
              <CalendarDays className="h-4 w-4" />
              Assignment Calendar
            </Button>
            
            <div className="flex items-center gap-2 bg-slate-100 rounded-lg p-1">
              <Button
                variant={assignmentMode === 'immediate' ? 'default' : 'ghost'}
                size="sm"
                onClick={() => setAssignmentMode('immediate')}
                className="gap-2"
              >
                <Clock className="h-3.5 w-3.5" />
                Immediate
              </Button>
              <Button
                variant={assignmentMode === 'scheduled' ? 'default' : 'ghost'}
                size="sm"
                onClick={() => setAssignmentMode('scheduled')}
                className="gap-2"
              >
                <Calendar className="h-3.5 w-3.5" />
                Scheduled
              </Button>
            </div>
          </div>
        </div>
      </div>

      <div className="flex h-[calc(100vh-200px)]">
        {/* Machine Grid View (Main Section) */}
        <div className="flex-1 p-6 overflow-y-auto">
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {machines.map((machine) => {
              const statusConfig = getStatusConfig(machine.status);
              const isHovered = hoveredMachine === machine.id;
              const isRecommended = draggedOperator && isRecommendedOperator(draggedOperator, machine.id);
              
              return (
                <Card 
                  key={machine.id} 
                  className={`
                    border-2 transition-all duration-200 cursor-pointer
                    ${isHovered ? 'border-blue-400 shadow-lg scale-105' : statusConfig.color}
                    ${isRecommended ? 'border-green-400 shadow-green-200 shadow-lg' : ''}
                  `}
                  onDragOver={(e) => handleDragOver(e, machine.id)}
                  onDragLeave={handleDragLeave}
                  onDrop={(e) => handleDrop(e, machine.id)}
                >
                  {/* Card Header */}
                  <CardHeader className="pb-3">
                    <div className="flex justify-between items-start">
                      <div className="flex items-center gap-3">
                        <Monitor className="h-5 w-5 text-slate-500" />
                        <div>
                          <CardTitle className="text-base font-semibold">
                            {machine.id}
                          </CardTitle>
                          <p className="text-sm text-slate-600">{machine.name}</p>
                        </div>
                      </div>
                      
                      <Badge className={statusConfig.color}>
                        <span className="mr-1">{statusConfig.icon}</span>
                        <div className="text-center">
                          <div className="text-xs">{statusConfig.text}</div>
                          <div className="text-xs">{statusConfig.textLocal}</div>
                        </div>
                      </Badge>
                    </div>
                  </CardHeader>

                  {/* Card Body */}
                  <CardContent className="space-y-4">
                    {/* Product Info */}
                    {machine.currentJob ? (
                      <div className="flex items-center gap-3 p-3 bg-slate-50 rounded-lg">
                        <div className="w-12 h-12 bg-slate-200 rounded-lg flex items-center justify-center">
                          <Package className="h-6 w-6 text-slate-500" />
                        </div>
                        <div className="flex-1">
                          <p className="font-medium text-sm">{machine.currentJob.product}</p>
                          <p className="text-xs text-slate-500">Job ID: {machine.currentJob.id}</p>
                          
                          {/* Progress Bar */}
                          <div className="mt-2">
                            <div className="flex justify-between text-xs text-slate-600 mb-1">
                              <span>{machine.currentJob.currentQty.toLocaleString()}</span>
                              <span>{machine.currentJob.targetQty.toLocaleString()}</span>
                            </div>
                            <Progress value={machine.currentJob.progress} className="h-2" />
                          </div>
                        </div>
                      </div>
                    ) : (
                      <div className="p-3 bg-slate-50 rounded-lg text-center">
                        <Package className="h-8 w-8 text-slate-400 mx-auto mb-2" />
                        <p className="text-sm text-slate-500">No Job Assigned</p>
                        <p className="text-xs text-slate-400">အလုပ်မခန့်အပ်ရသေး</p>
                      </div>
                    )}

                    {/* Current Operator */}
                    <div>
                      <p className="text-xs text-slate-500 mb-2">Current Operator | လက်ရှိအော်ပရေတာ</p>
                      {machine.operator ? (
                        <div className="flex items-center justify-between p-3 bg-blue-50 rounded-lg border border-blue-200">
                          <div className="flex items-center gap-3">
                            <Avatar className="h-8 w-8">
                              <AvatarImage src={machine.operator.avatar} />
                              <AvatarFallback className="text-xs">
                                {machine.operator.name.split(' ').map(n => n[0]).join('')}
                              </AvatarFallback>
                            </Avatar>
                            <div>
                              <p className="font-medium text-sm">{machine.operator.name}</p>
                              <p className="text-xs text-slate-600">{machine.operator.nameLocal} ({machine.operator.id})</p>
                              <p className="text-xs text-slate-500">{machine.operator.department} • {machine.operator.shift}</p>
                            </div>
                          </div>
                          <Button
                            variant="outline"
                            size="sm"
                            onClick={() => handleUnassignOperator(machine.id)}
                            className="text-red-600 hover:text-red-700"
                          >
                            <UserX className="h-3 w-3" />
                          </Button>
                        </div>
                      ) : (
                        <div className="p-4 bg-gray-50 rounded-lg border-2 border-dashed border-gray-300 text-center">
                          <UserPlus className="h-8 w-8 text-gray-400 mx-auto mb-2" />
                          <p className="text-sm font-medium text-gray-600">Assign Operator</p>
                          <p className="text-xs text-gray-500">အော်ပရေတာခန့်အပ်ရန်</p>
                          <p className="text-xs text-gray-400 mt-1">Drag operator here</p>
                        </div>
                      )}
                    </div>
                  </CardContent>
                </Card>
              );
            })}
          </div>
        </div>

        {/* Operator List (Right Sidebar) */}
        <div className="w-80 bg-white border-l border-slate-200 p-4 overflow-y-auto">
          <div className="sticky top-0 bg-white pb-4 mb-4 border-b border-slate-200">
            <h3 className="font-semibold text-lg mb-3">
              Operator List | အော်ပရေတာစာရင်း
            </h3>
            
            {/* Search Bar */}
            <div className="relative mb-3">
              <Search className="absolute left-3 top-2.5 h-4 w-4 text-slate-400" />
              <Input
                placeholder="Search operators..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="pl-10"
              />
            </div>
          </div>

          {/* Operator Cards */}
          <div className="space-y-3">
            {filteredOperators.map((operator) => {
              const statusConfig = getOperatorStatusConfig(operator.status);
              
              return (
                <Card 
                  key={operator.id}
                  className={`
                    cursor-move transition-all duration-200 hover:shadow-md
                    ${operator.status === 'absent' ? 'opacity-50 cursor-not-allowed' : ''}
                  `}
                  draggable={operator.status !== 'absent'}
                  onDragStart={() => handleDragStart(operator)}
                  onDragEnd={handleDragEnd}
                >
                  <CardContent className="p-3">
                    <div className="flex items-center gap-3">
                      <Avatar className="h-10 w-10">
                        <AvatarImage src={operator.avatar} />
                        <AvatarFallback className="text-xs">
                          {operator.name.split(' ').map(n => n[0]).join('')}
                        </AvatarFallback>
                      </Avatar>
                      
                      <div className="flex-1">
                        <div className="flex items-center gap-2 mb-1">
                          <p className="font-medium text-sm">{operator.name}</p>
                          <Badge className={`${statusConfig.color} text-xs`}>
                            {statusConfig.icon}
                          </Badge>
                        </div>
                        <p className="text-xs text-slate-600">{operator.nameLocal} ({operator.id})</p>
                        <p className="text-xs text-slate-500">{operator.department} • {operator.shift}</p>
                        
                        {operator.currentMachine && (
                          <p className="text-xs text-blue-600 mt-1">
                            📍 {operator.currentMachine}
                          </p>
                        )}
                        
                        <div className="flex items-center gap-2 mt-1">
                          <span className="text-xs text-slate-500">{operator.skillLevel}</span>
                          <span className="text-xs text-slate-400">•</span>
                          <span className="text-xs text-slate-500">{operator.experience}</span>
                        </div>
                      </div>
                    </div>
                  </CardContent>
                </Card>
              );
            })}
          </div>

          {filteredOperators.length === 0 && (
            <div className="text-center py-8">
              <Users className="h-12 w-12 text-slate-400 mx-auto mb-3" />
              <p className="text-slate-600">No operators found</p>
              <p className="text-sm text-slate-500">အော်ပရေတာများမတွေ့ရပါ</p>
            </div>
          )}
        </div>
      </div>

      {/* Undo/Redo Buttons (Bottom) */}
      {assignmentHistory.length > 0 && (
        <div className="fixed bottom-6 left-1/2 transform -translate-x-1/2">
          <div className="flex items-center gap-2 bg-white rounded-lg shadow-lg border border-slate-200 p-2">
            <Button variant="outline" size="sm" className="gap-2">
              <RotateCcw className="h-4 w-4" />
              Undo
            </Button>
            <Button variant="outline" size="sm" className="gap-2">
              <RotateCw className="h-4 w-4" />
              Redo
            </Button>
            <Separator orientation="vertical" className="h-6" />
            <span className="text-sm text-slate-600">
              {assignmentHistory.length} changes today
            </span>
          </div>
        </div>
      )}

      {/* Immediate Assignment Modal */}
      <Dialog open={showAssignModal} onOpenChange={setShowAssignModal}>
        <DialogContent className="max-w-md" aria-describedby={undefined}>
          <DialogHeader>
            <DialogTitle>Assign Operator Now | အော်ပရေတာခန့်အပ်မည်</DialogTitle>
          </DialogHeader>
          
          <div className="space-y-4 py-4">
            <div>
              <Label htmlFor="operator">Select Operator | အော်ပရေတာရွေးချယ်ပါ</Label>
              <Select value={selectedOperatorId} onValueChange={setSelectedOperatorId}>
                <SelectTrigger id="operator" className="mt-1.5">
                  <SelectValue placeholder="Choose operator..." />
                </SelectTrigger>
                <SelectContent>
                  {operators.filter(op => op.status !== 'absent').map(op => (
                    <SelectItem key={op.id} value={op.id}>
                      <div className="flex items-center gap-2">
                        <Avatar className="h-6 w-6">
                          <AvatarImage src={op.avatar} />
                          <AvatarFallback className="text-xs">{op.name[0]}</AvatarFallback>
                        </Avatar>
                        <span>{op.name} - {op.nameLocal} ({op.skillLevel})</span>
                      </div>
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div>
              <Label>Effective Time | အချိန်</Label>
              <div className="flex items-center gap-2 mt-1.5 p-3 bg-blue-50 rounded-lg border border-blue-200">
                <Clock className="h-4 w-4 text-blue-600" />
                <span className="text-sm">Immediately (Now)</span>
              </div>
            </div>

            <div>
              <Label htmlFor="note">Note (Optional) | မှတ်ချက်</Label>
              <Textarea 
                id="note"
                placeholder="Add any notes..."
                value={assignmentNote}
                onChange={(e) => setAssignmentNote(e.target.value)}
                className="mt-1.5"
                rows={3}
              />
            </div>
          </div>

          <DialogFooter>
            <Button variant="outline" onClick={() => setShowAssignModal(false)}>
              Cancel
            </Button>
            <Button 
              onClick={handleImmediateAssign}
              disabled={!selectedOperatorId}
              className="bg-blue-600 hover:bg-blue-700"
            >
              <UserPlus className="h-4 w-4 mr-2" />
              Confirm Assignment
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Assignment Calendar Modal */}
      <AssignmentCalendarModal
        open={showCalendarModal}
        onOpenChange={setShowCalendarModal}
        machines={machines}
        operators={operators}
        scheduledAssignments={scheduledAssignments}
        onAssignmentSave={setScheduledAssignments}
      />
    </div>
  );
}