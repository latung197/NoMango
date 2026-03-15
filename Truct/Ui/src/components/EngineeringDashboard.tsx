import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Badge } from './ui/badge';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogTrigger } from './ui/dialog';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Textarea } from './ui/textarea';
import { Label } from './ui/label';
import { Tabs, TabsContent, TabsList, TabsTrigger } from './ui/tabs';
import { Avatar, AvatarFallback, AvatarImage } from './ui/avatar';
import { Progress } from './ui/progress';
import { Separator } from './ui/separator';
import { Calendar } from './ui/calendar';
import { toast } from 'sonner@2.0.3';
import {
  Search,
  Filter,
  Calendar as CalendarIcon,
  Clock,
  AlertTriangle,
  Wrench,
  CheckCircle,
  Play,
  Pause,
  ArrowUp,
  Settings,
  User,
  Timer,
  BarChart3,
  FileText,
  Download,
  Plus,
  Edit,
  Eye,
  TrendingUp,
  Activity,
  Zap,
  Package,
  Thermometer,
  Layers,
  Bolt,
  Droplets,
  Users,
  Target,
  RefreshCw,
  MapPin,
  Phone,
  Mail,
  X
} from 'lucide-react';

interface Machine {
  id: string;
  name: string;
  type: string;
  status: 'running' | 'idle' | 'down' | 'pm-due' | 'waiting-parts';
  currentJob?: {
    jobId: string;
    planId: string;
    product: string;
  };
  downtime?: {
    reason: string;
    reasonMM: string;
    startTime: string;
    duration: string;
  };
  assignedEngineer?: {
    id: string;
    name: string;
    role: 'plastic' | 'mold' | 'maintenance' | 'mold-change' | 'technician-head';
    avatar?: string;
    status: 'assigned' | 'in-progress' | 'resolved';
    contactInfo: {
      phone: string;
      email: string;
    };
  };
  timeline: {
    time: string;
    status: 'running' | 'idle' | 'down' | 'engineering';
  }[];
  lastPM: string;
  nextPM: string;
}

interface Task {
  id: string;
  machineId: string;
  machineName: string;
  title: string;
  titleMM: string;
  priority: 'P1' | 'P2' | 'P3';
  status: 'new' | 'assigned' | 'in-progress' | 'waiting-parts' | 'qa' | 'done';
  assignedTo?: {
    id: string;
    name: string;
    role: string;
    avatar?: string;
  };
  createdAt: string;
  slaCountdown: string;
  estimatedDuration: string;
  description: string;
}

const mockMachines: Machine[] = [
  {
    id: 'IM-001',
    name: 'IM-001',
    type: 'Injection Machine 1',
    status: 'down',
    currentJob: {
      jobId: 'JOB-2024-001',
      planId: 'PLN-2024-001',
      product: 'Bottle Cap Red'
    },
    downtime: {
      reason: 'Hydraulic Oil Leak',
      reasonMM: 'အိုင်ဒရောလစ်ဆီယို',
      startTime: '09:30',
      duration: '2h 15m'
    },
    assignedEngineer: {
      id: 'ENG-001',
      name: 'Ko Thant',
      role: 'maintenance',
      status: 'in-progress',
      contactInfo: {
        phone: '+95-9-123456789',
        email: 'ko.thant@factory.com'
      }
    },
    timeline: [
      { time: '00:00', status: 'running' },
      { time: '09:30', status: 'down' },
      { time: '11:00', status: 'engineering' },
    ],
    lastPM: '2024-01-15',
    nextPM: '2024-02-15'
  },
  {
    id: 'IM-002',
    name: 'IM-002',
    type: 'Injection Machine 2',
    status: 'running',
    currentJob: {
      jobId: 'JOB-2024-002',
      planId: 'PLN-2024-002',
      product: 'Container Blue'
    },
    timeline: [
      { time: '00:00', status: 'running' },
      { time: '08:00', status: 'idle' },
      { time: '08:30', status: 'running' },
    ],
    lastPM: '2024-01-20',
    nextPM: '2024-02-20'
  },
  {
    id: 'IM-003',
    name: 'IM-003',
    type: 'Injection Machine 3',
    status: 'down',
    downtime: {
      reason: 'Mold Jam',
      reasonMM: 'မှိုပိတ်ခြင်း',
      startTime: '13:15',
      duration: '45m'
    },
    timeline: [
      { time: '00:00', status: 'running' },
      { time: '13:15', status: 'down' },
    ],
    lastPM: '2024-01-12',
    nextPM: '2024-02-12'
  },
  {
    id: 'IM-004',
    name: 'IM-004',
    type: 'Injection Machine 4',
    status: 'pm-due',
    downtime: {
      reason: 'PM Scheduled for Tomorrow',
      reasonMM: 'မနက်ဖြန် PM သတ်မှတ်ထား',
      startTime: '',
      duration: ''
    },
    timeline: [
      { time: '00:00', status: 'running' },
    ],
    lastPM: '2023-12-15',
    nextPM: '2024-01-15'
  },
  {
    id: 'IM-005',
    name: 'IM-005',
    type: 'Injection Machine 5',
    status: 'waiting-parts',
    downtime: {
      reason: 'Waiting for Spare Parts',
      reasonMM: 'အပိုပစ္စည်းများစောင့်နေ',
      startTime: '10:00',
      duration: '4h 30m'
    },
    assignedEngineer: {
      id: 'ENG-002',
      name: 'Ma Aye',
      role: 'mold',
      status: 'assigned',
      contactInfo: {
        phone: '+95-9-987654321',
        email: 'ma.aye@factory.com'
      }
    },
    timeline: [
      { time: '00:00', status: 'running' },
      { time: '10:00', status: 'down' },
    ],
    lastPM: '2024-01-10',
    nextPM: '2024-02-10'
  }
];

const mockTasks: Task[] = [
  {
    id: 'TSK-001',
    machineId: 'IM-001',
    machineName: 'IM-001',
    title: 'Hydraulic System Repair',
    titleMM: 'အိုင်ဒရောလစ်စနစ်ပြုပြင်',
    priority: 'P1',
    status: 'in-progress',
    assignedTo: {
      id: 'ENG-001',
      name: 'Ko Thant',
      role: 'Maintenance Engineer',
      avatar: ''
    },
    createdAt: '2024-01-25 09:30',
    slaCountdown: '2h 15m',
    estimatedDuration: '4h',
    description: 'Hydraulic oil leak detected, need to replace seals and check pump'
  },
  {
    id: 'TSK-002',
    machineId: 'IM-005',
    machineName: 'IM-005',
    title: 'Mold Core Replacement',
    titleMM: 'မှိုအူတုံးအစားထိုး',
    priority: 'P2',
    status: 'waiting-parts',
    assignedTo: {
      id: 'ENG-002',
      name: 'Ma Aye',
      role: 'Mold Technician',
      avatar: ''
    },
    createdAt: '2024-01-25 10:00',
    slaCountdown: '1h 30m',
    estimatedDuration: '6h',
    description: 'Mold core damaged, waiting for replacement part delivery'
  }
];

export function EngineeringDashboard() {
  const [selectedView, setSelectedView] = useState<'table' | 'kanban' | 'calendar' | 'reports'>('table');
  const [filterStatus, setFilterStatus] = useState('all');
  const [filterRole, setFilterRole] = useState('all');
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedDate, setSelectedDate] = useState(new Date());
  const [selectedShift, setSelectedShift] = useState('day');
  const [showBreakdownModal, setShowBreakdownModal] = useState(false);
  const [selectedMachine, setSelectedMachine] = useState<Machine | null>(null);

  // KPI calculations
  const totalMachines = mockMachines.length;
  const machinesDown = mockMachines.filter(m => m.status === 'down' || m.status === 'waiting-parts').length;
  const avgMTTR = '2.5h';
  const mtbf = '45h';
  const overduePM = mockMachines.filter(m => m.status === 'pm-due').length;
  const openTasks = mockTasks.filter(t => t.status !== 'done').length;

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'running': return 'bg-green-100 text-green-800 border-green-200';
      case 'idle': return 'bg-yellow-100 text-yellow-800 border-yellow-200';
      case 'down': return 'bg-red-100 text-red-800 border-red-200';
      case 'pm-due': return 'bg-purple-100 text-purple-800 border-purple-200';
      case 'waiting-parts': return 'bg-orange-100 text-orange-800 border-orange-200';
      default: return 'bg-gray-100 text-gray-800 border-gray-200';
    }
  };

  const getStatusText = (status: string) => {
    switch (status) {
      case 'running': return { en: '🟢 Running', mm: 'လုပ်ဆောင်နေ' };
      case 'idle': return { en: '🟡 Idle', mm: 'ခဏရပ်' };
      case 'down': return { en: '🔴 Down', mm: 'ပျက်စီး' };
      case 'pm-due': return { en: '🟣 PM Due', mm: 'ပြုပြင်ထိန်းသိမ်းရန်' };
      case 'waiting-parts': return { en: '🟠 Waiting Parts', mm: 'အပိုပစ္စည်းစောင့်' };
      default: return { en: 'Unknown', mm: 'မသိ' };
    }
  };

  const getRoleIcon = (role: string) => {
    switch (role) {
      case 'plastic': return <Thermometer className="h-3 w-3" />;
      case 'mold': return <Layers className="h-3 w-3" />;
      case 'maintenance': return <Wrench className="h-3 w-3" />;
      case 'mold-change': return <RefreshCw className="h-3 w-3" />;
      case 'technician-head': return <Users className="h-3 w-3" />;
      default: return <User className="h-3 w-3" />;
    }
  };

  const getRoleText = (role: string) => {
    switch (role) {
      case 'plastic': return { en: 'Plastic Technician', mm: 'ပလတ်စတစ်နည်းပညာရှင်' };
      case 'mold': return { en: 'Mold Technician', mm: 'မောလ်နည်းပညာရှင်' };
      case 'maintenance': return { en: 'Maintenance Engineer', mm: 'ပြုပြင်ထိန်းသိမ်းအင်ဂျင်နီယာ' };
      case 'mold-change': return { en: 'Mold Change', mm: 'မောလ်အပြောင်းအလဲအလုပ်' };
      case 'technician-head': return { en: 'Technician Head', mm: 'နည်းပညာဦးစီး' };
      default: return { en: 'Unknown', mm: 'မသိ' };
    }
  };

  const getEngineerStatusColor = (status: string) => {
    switch (status) {
      case 'assigned': return 'border-orange-400';
      case 'in-progress': return 'border-blue-400';
      case 'resolved': return 'border-green-400';
      default: return 'border-gray-300';
    }
  };

  const getPriorityColor = (priority: string) => {
    switch (priority) {
      case 'P1': return 'bg-red-100 text-red-800 border-red-300';
      case 'P2': return 'bg-orange-100 text-orange-800 border-orange-300';
      case 'P3': return 'bg-green-100 text-green-800 border-green-300';
      default: return 'bg-gray-100 text-gray-800 border-gray-300';
    }
  };

  const filteredMachines = mockMachines.filter(machine => {
    if (filterStatus !== 'all' && machine.status !== filterStatus) return false;
    if (filterRole !== 'all' && (!machine.assignedEngineer || machine.assignedEngineer.role !== filterRole)) return false;
    if (searchTerm && !machine.id.toLowerCase().includes(searchTerm.toLowerCase()) && 
        !machine.currentJob?.jobId.toLowerCase().includes(searchTerm.toLowerCase()) &&
        !machine.currentJob?.planId.toLowerCase().includes(searchTerm.toLowerCase())) return false;
    return true;
  });

  const renderBreakdownModal = () => (
    <Dialog open={showBreakdownModal} onOpenChange={setShowBreakdownModal}>
      <DialogContent className="max-w-2xl">
        <DialogHeader>
          <DialogTitle>Machine Breakdown Report | စက်ပျက်စီးမှုအစီရင်ခံစာ</DialogTitle>
          <DialogDescription>
            Report a new machine breakdown and assign an engineer
          </DialogDescription>
        </DialogHeader>
        
        <div className="space-y-6">
          <div className="grid grid-cols-2 gap-4">
            <div>
              <Label>Machine ID | စက်နံပါတ်</Label>
              <Select>
                <SelectTrigger>
                  <SelectValue placeholder="Select machine" />
                </SelectTrigger>
                <SelectContent>
                  {mockMachines.map(machine => (
                    <SelectItem key={machine.id} value={machine.id}>
                      {machine.id} - {machine.type}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            
            <div>
              <Label>Priority | ဦးစားပေး</Label>
              <Select>
                <SelectTrigger>
                  <SelectValue placeholder="Select priority" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="P1">P1 - Critical</SelectItem>
                  <SelectItem value="P2">P2 - High</SelectItem>
                  <SelectItem value="P3">P3 - Medium</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>
          
          <div>
            <Label>Reason Code | အကြောင်းရင်းကုဒ်</Label>
            <Select>
              <SelectTrigger>
                <SelectValue placeholder="Select reason" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="mechanical">Mechanical Issue | စက်ပိုင်းဆိုင်ရာပြဿနာ</SelectItem>
                <SelectItem value="electrical">Electrical Issue | လျှပ်စစ်ပြဿနာ</SelectItem>
                <SelectItem value="hydraulic">Hydraulic Issue | အိုင်ဒရောလစ်ပြဿနာ</SelectItem>
                <SelectItem value="mold">Mold Issue | မှိုပြဿနာ</SelectItem>
                <SelectItem value="other">Other | အခြား</SelectItem>
              </SelectContent>
            </Select>
          </div>
          
          <div>
            <Label>Assign Engineer | အင်ဂျင်နီယာခန့်အပ်</Label>
            <Select>
              <SelectTrigger>
                <SelectValue placeholder="Select engineer" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="plastic">🔧 Plastic Technician | ပလတ်စတစ်နည်းပညာရှင်</SelectItem>
                <SelectItem value="mold">🛠 Mold Technician | မောလ်နည်းပညာရှင်</SelectItem>
                <SelectItem value="maintenance">⚙️ Maintenance Engineer | ပြုပြင်ထိန်းသိမ်းအင်ဂျင်နီယာ</SelectItem>
                <SelectItem value="mold-change">🧩 Mold Change | မောလ်အပြောင်းအလဲအလုပ်</SelectItem>
                <SelectItem value="head">👨‍💼 Technician Head | နည်းပညာဦးစီး</SelectItem>
              </SelectContent>
            </Select>
          </div>
          
          <div>
            <Label>Description | ဖော်ပြချက်</Label>
            <Textarea 
              placeholder="Describe the issue in detail..." 
              rows={4}
            />
          </div>
          
          <div className="flex justify-end gap-3">
            <Button variant="outline" onClick={() => setShowBreakdownModal(false)}>
              Cancel | ပယ်ဖျက်
            </Button>
            <Button onClick={() => {
              setShowBreakdownModal(false);
              toast.success('Breakdown report saved and engineer assigned | ပျက်စီးမှုအစီရင်ခံစာသိမ်းပြီး အင်ဂျင်နီယာခန့်အပ်ပြီး');
            }}>
              Save | သိမ်း
            </Button>
            <Button className="bg-blue-600 hover:bg-blue-700" onClick={() => {
              setShowBreakdownModal(false);
              toast.success('Report saved and repair started | အစီရင်ခံစာသိမ်းပြီး ပြုပြင်မှုစတင်ပြီး');
            }}>
              Save & Start Repair | သိမ်း & ပြုပြင်စတင်
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );

  return (
    <div className="min-h-screen bg-slate-50">
      {/* Header */}
      <div className="bg-white border-b border-slate-200 px-6 py-4">
        <div className="flex justify-between items-center">
          <div>
            <h1 className="text-2xl font-semibold text-slate-900">
              Engineering Dashboard | အင်ဂျင်နီယာအင်တာဖေ့စ်
            </h1>
          </div>
          
          {/* Top Filters as Tabs */}
          <div className="flex items-center gap-3">
            <div className="flex items-center gap-2">
              <Button
                variant={filterStatus === 'all' ? 'default' : 'outline'}
                size="sm"
                onClick={() => setFilterStatus('all')}
              >
                All
              </Button>
              <Button
                variant={filterStatus === 'running' ? 'default' : 'outline'}
                size="sm"
                onClick={() => setFilterStatus('running')}
              >
                Running
              </Button>
              <Button
                variant={filterStatus === 'down' ? 'default' : 'outline'}
                size="sm"
                onClick={() => setFilterStatus('down')}
              >
                Down
              </Button>
              <Button
                variant={filterStatus === 'pm-due' ? 'default' : 'outline'}
                size="sm"
                onClick={() => setFilterStatus('pm-due')}
              >
                PM Due
              </Button>
              <Button
                variant={filterStatus === 'waiting-parts' ? 'default' : 'outline'}
                size="sm"
                onClick={() => setFilterStatus('waiting-parts')}
              >
                Waiting Parts
              </Button>
            </div>
            
            <Separator orientation="vertical" className="h-6" />
            
            <Select value={filterRole} onValueChange={setFilterRole}>
              <SelectTrigger className="w-48">
                <SelectValue placeholder="Filter by Engineer Role" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">All Roles</SelectItem>
                <SelectItem value="plastic">Plastic</SelectItem>
                <SelectItem value="mold">Mold</SelectItem>
                <SelectItem value="maintenance">Maintenance</SelectItem>
                <SelectItem value="mold-change">Mold Change</SelectItem>
                <SelectItem value="technician-head">Technician Head</SelectItem>
              </SelectContent>
            </Select>
            
            <div className="relative">
              <Search className="absolute left-3 top-2.5 h-4 w-4 text-slate-400" />
              <Input
                placeholder="Machine ID / Plan ID / Job ID"
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="pl-10 w-64"
              />
            </div>
          </div>
        </div>
      </div>

      <div className="p-6">
        {/* Action Buttons */}
        <div className="flex justify-between items-center mb-6">
          <h3 className="text-lg font-semibold">Engineering Dashboard – Machine List | အင်ဂျင်နီယာဒက်ረှ်ဘုတ် – စက်စာရင်း</h3>
          <div className="flex gap-2">
            <Button variant="outline" size="sm">
              <Download className="h-4 w-4 mr-2" />
              Export | တင်ယူ
            </Button>
            <Button onClick={() => setShowBreakdownModal(true)}>
              <Plus className="h-4 w-4 mr-2" />
              Report Breakdown | ပျက်စီးမှုအစီရင်ခံ
            </Button>
          </div>
        </div>

        {/* Machine Cards Grid */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
          {filteredMachines.map((machine) => {
            const borderColor = 
              machine.status === 'running' ? 'border-green-400' :
              machine.status === 'down' ? 'border-red-400' :
              machine.status === 'pm-due' ? 'border-purple-400' :
              machine.status === 'waiting-parts' ? 'border-orange-400' :
              'border-yellow-400';

            return (
              <Card key={machine.id} className={`border-2 ${borderColor} hover:shadow-lg transition-all duration-200`}>
                {/* Card Header */}
                <CardHeader className="pb-3">
                  <div className="flex justify-between items-start">
                    <div className="flex-1">
                      <CardTitle className="text-base font-semibold">
                        {machine.type}
                      </CardTitle>
                      <p className="text-sm text-slate-600 font-medium">({machine.name})</p>
                    </div>
                    
                    <div className="flex flex-col items-end gap-2">
                      <Badge className={`${getStatusColor(machine.status)} border text-xs`}>
                        {getStatusText(machine.status).en}
                      </Badge>
                      {machine.assignedEngineer && (
                        <div className="h-6 w-6 bg-blue-100 rounded-full flex items-center justify-center">
                          {getRoleIcon(machine.assignedEngineer.role)}
                        </div>
                      )}
                    </div>
                  </div>
                </CardHeader>

                {/* Card Body */}
                <CardContent className="space-y-3">
                  {/* Current Job */}
                  <div>
                    <p className="text-xs text-slate-500 mb-1">Current Job | လက်ရှိအလုပ်</p>
                    {machine.currentJob ? (
                      <div>
                        <p className="text-sm font-medium">{machine.currentJob.jobId}</p>
                        <p className="text-xs text-slate-600">{machine.currentJob.product}</p>
                      </div>
                    ) : (
                      <p className="text-sm text-slate-500">No Job | အလုပ်မရှိ</p>
                    )}
                  </div>

                  {/* Downtime/Issue (only if Down) */}
                  {machine.downtime && machine.status !== 'pm-due' && (
                    <div className="bg-red-50 p-3 rounded-lg border border-red-200">
                      <p className="text-xs text-red-600 font-medium mb-1">Issue | ပြဿနာ</p>
                      <p className="text-sm font-medium text-red-800">{machine.downtime.reason}</p>
                      <p className="text-xs text-red-600">{machine.downtime.reasonMM}</p>
                      {machine.downtime.duration && (
                        <div className="flex items-center gap-2 mt-2">
                          <Timer className="h-3 w-3 text-red-500" />
                          <span className="text-xs text-red-600 font-medium">
                            {machine.downtime.duration}
                          </span>
                        </div>
                      )}
                    </div>
                  )}

                  {/* PM Due status */}
                  {machine.status === 'pm-due' && (
                    <div className="bg-purple-50 p-3 rounded-lg border border-purple-200">
                      <p className="text-xs text-purple-600 font-medium mb-1">Maintenance | ပြုပြင်ထိန်းသိမ်း</p>
                      <p className="text-sm font-medium text-purple-800">{machine.downtime?.reason || 'PM Scheduled'}</p>
                      <p className="text-xs text-purple-600">{machine.downtime?.reasonMM || 'PM သတ်မှတ်ထား'}</p>
                    </div>
                  )}

                  {/* Down For Who */}
                  <div>
                    <p className="text-xs text-slate-500 mb-2">Down for Who | တာဝန်ခံအင်ဂျင်နီယာ</p>
                    {machine.assignedEngineer ? (
                      <div className="flex items-center gap-3 p-2 bg-slate-50 rounded-lg">
                        <div className="relative">
                          <Avatar className={`h-8 w-8 border-2 ${getEngineerStatusColor(machine.assignedEngineer.status)}`}>
                            <AvatarFallback className="text-xs">
                              {machine.assignedEngineer.name.split(' ').map(n => n[0]).join('')}
                            </AvatarFallback>
                          </Avatar>
                          <div className="absolute -top-1 -right-1 h-4 w-4 bg-white rounded-full flex items-center justify-center border">
                            {getRoleIcon(machine.assignedEngineer.role)}
                          </div>
                        </div>
                        
                        <div className="flex-1">
                          <p className="text-sm font-medium">{getRoleText(machine.assignedEngineer.role).en}</p>
                          <p className="text-xs text-slate-600">{machine.assignedEngineer.name}</p>
                        </div>
                      </div>
                    ) : (
                      <div className="p-2 bg-red-50 rounded-lg border border-red-200">
                        <p className="text-sm font-medium text-red-600">❌ Unassigned</p>
                        <p className="text-xs text-red-500">မသတ်မှတ်ရသေး</p>
                      </div>
                    )}
                  </div>
                </CardContent>

                {/* Card Footer */}
                <div className="px-6 pb-6">
                  <Separator className="mb-3" />
                  
                  {/* Quick Metrics */}
                  <div className="grid grid-cols-3 gap-2 mb-3 text-xs">
                    <div>
                      <p className="text-slate-500">🕒 Downtime</p>
                      <p className="font-medium">
                        {machine.downtime?.duration || '0m'}
                      </p>
                    </div>
                    <div>
                      <p className="text-slate-500">🔧 Open Tasks</p>
                      <p className="font-medium">
                        {mockTasks.filter(t => t.machineId === machine.id && t.status !== 'done').length}
                      </p>
                    </div>
                    <div>
                      <p className="text-slate-500">📅 Next PM</p>
                      <p className="font-medium text-xs">
                        {new Date(machine.nextPM).toLocaleDateString('en-GB', { day: '2-digit', month: '2-digit' })}
                      </p>
                    </div>
                  </div>

                  {/* View Details Button */}
                  <Button 
                    variant="outline" 
                    size="sm" 
                    className="w-full"
                    onClick={() => {
                      setSelectedMachine(machine);
                      toast.info(`Opening details for ${machine.name} | ${machine.name} အသေးစိတ်ကိုဖွင့်နေသည်`);
                    }}
                  >
                    <Eye className="h-4 w-4 mr-2" />
                    View Details | အသေးစိတ်ကြည့်ရှု
                  </Button>
                </div>
              </Card>
            );
          })}
        </div>

        {/* No machines found */}
        {filteredMachines.length === 0 && (
          <Card className="p-8 text-center">
            <div className="space-y-2">
              <AlertTriangle className="h-12 w-12 text-slate-400 mx-auto" />
              <h3 className="text-lg font-semibold text-slate-600">No Machines Found</h3>
              <p className="text-slate-500">စက်များမတွေ့ရပါ</p>
              <p className="text-sm text-slate-400">Try adjusting your filters to see more results.</p>
            </div>
          </Card>
        )}
      </div>

      {renderBreakdownModal()}
    </div>
  );
}