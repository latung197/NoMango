import React, { useState, useCallback, useRef } from 'react';
import {
  Factory,
  Play,
  Pause,
  Square,
  AlertTriangle,
  Wrench,
  Clock,
  User,
  Package,
  Target,
  TrendingUp,
  TrendingDown,
  Calendar,
  ChevronDown,
  Bell,
  Plus,
  RotateCcw,
  ArrowRight,
  Users,
  Award,
  AlertCircle,
  CheckCircle,
  XCircle,
  Settings,
  RefreshCw,
  Zap,
  Activity,
  BarChart3,
  PieChart,
  Timer,
  MapPin,
  Eye,
  Edit,
  ArrowRightLeft
} from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Progress } from './ui/progress';
import { Avatar, AvatarFallback, AvatarImage } from './ui/avatar';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Tabs, TabsContent, TabsList, TabsTrigger } from './ui/tabs';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogTrigger } from './ui/dialog';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { toast } from 'sonner@2.0.3';
import { motion, AnimatePresence } from 'motion/react';
import { DndProvider, useDrag, useDrop } from 'react-dnd';
import { HTML5Backend } from 'react-dnd-html5-backend';

// Mock data for factory operations
const factoryKPIs = {
  runningMachines: { count: 8, total: 12, percentage: 67 },
  idleMachines: { count: 2, total: 12, percentage: 17 },
  blockedMachines: { count: 2, total: 12, percentage: 16 },
  totalProduced: { count: 15420, target: 18000, percentage: 86 },
  defectsToday: { count: 85, total: 15420, percentage: 0.55, topReason: 'Mold Flash' }
};

const operators = [
  { id: 'OP001', name: 'Ko Aung', nameMyanmar: 'ကိုအောင်', photo: '/api/placeholder/40/40', shift: 'A', rating: 4.8 },
  { id: 'OP002', name: 'Ma Suu', nameMyanmar: 'မစု', photo: '/api/placeholder/40/40', shift: 'A', rating: 4.6 },
  { id: 'OP003', name: 'Ko Thant', nameMyanmar: 'ကိုသန့်', photo: '/api/placeholder/40/40', shift: 'A', rating: 4.9 },
  { id: 'OP004', name: 'Ma Phyu', nameMyanmar: 'မဖြူ', photo: '/api/placeholder/40/40', shift: 'A', rating: 4.5 },
  { id: 'OP005', name: 'Ko Zaw', nameMyanmar: 'ကိုဇော်', photo: '/api/placeholder/40/40', shift: 'A', rating: 4.7 },
];

const products = [
  { id: 'P001', name: 'Plastic Cup 250ml', image: '/api/placeholder/60/60', color: 'Clear' },
  { id: 'P002', name: 'Plastic Bottle 500ml', image: '/api/placeholder/60/60', color: 'Blue' },
  { id: 'P003', name: 'Container 1L', image: '/api/placeholder/60/60', color: 'White' },
  { id: 'P004', name: 'Lid 100ml', image: '/api/placeholder/60/60', color: 'Red' },
  { id: 'P005', name: 'Tray Large', image: '/api/placeholder/60/60', color: 'Black' }
];

const mockMachines = [
  {
    id: 'INJ-M001',
    name: 'Injection Machine 1',
    nameMyanmar: 'ထိုးစက် ၁',
    status: 'running',
    operatorId: 'OP001',
    productId: 'P001',
    jobId: 'JOB001',
    targetQty: 5000,
    completedQty: 3400,
    startTime: '09:00',
    estimatedFinish: '14:30',
    workOrderNo: 'WO-2025-001',
    efficiency: 92,
    cycleTime: 45,
    alerts: ['Material Low'],
    lastUpdate: new Date().toISOString(),
    moldCondition: 'Good',
    temperature: 185,
    pressure: 120
  },
  {
    id: 'INJ-M002',
    name: 'Injection Machine 2',
    nameMyanmar: 'ထိုးစက် ၂',
    status: 'idle',
    operatorId: 'OP002',
    productId: null,
    jobId: null,
    targetQty: 0,
    completedQty: 0,
    startTime: null,
    estimatedFinish: null,
    workOrderNo: null,
    efficiency: 0,
    cycleTime: 0,
    alerts: [],
    lastUpdate: new Date().toISOString(),
    moldCondition: 'Ready',
    temperature: 20,
    pressure: 0
  },
  {
    id: 'INJ-M003',
    name: 'Injection Machine 3',
    nameMyanmar: 'ထိုးစက် ၃',
    status: 'blocked',
    operatorId: 'OP003',
    productId: 'P002',
    jobId: 'JOB003',
    targetQty: 3000,
    completedQty: 1200,
    startTime: '08:00',
    estimatedFinish: '16:00',
    workOrderNo: 'WO-2025-003',
    efficiency: 65,
    cycleTime: 38,
    alerts: ['Mold Jam', 'Maintenance Required'],
    lastUpdate: new Date().toISOString(),
    moldCondition: 'Maintenance Required',
    temperature: 190,
    pressure: 0
  },
  {
    id: 'INJ-M004',
    name: 'Injection Machine 4',
    nameMyanmar: 'ထိုးစက် ၄',
    status: 'running',
    operatorId: 'OP004',
    productId: 'P003',
    jobId: 'JOB004',
    targetQty: 2000,
    completedQty: 1800,
    startTime: '07:00',
    estimatedFinish: '15:00',
    workOrderNo: 'WO-2025-004',
    efficiency: 98,
    cycleTime: 42,
    alerts: [],
    lastUpdate: new Date().toISOString(),
    moldCondition: 'Excellent',
    temperature: 180,
    pressure: 115
  },
  {
    id: 'INJ-M005',
    name: 'Injection Machine 5',
    nameMyanmar: 'ထိုးစက် ၅',
    status: 'maintenance',
    operatorId: null,
    productId: null,
    jobId: null,
    targetQty: 0,
    completedQty: 0,
    startTime: null,
    estimatedFinish: null,
    workOrderNo: null,
    efficiency: 0,
    cycleTime: 0,
    alerts: ['Scheduled Maintenance'],
    lastUpdate: new Date().toISOString(),
    moldCondition: 'Under Maintenance',
    temperature: 20,
    pressure: 0
  },
  {
    id: 'INJ-M006',
    name: 'Injection Machine 6',
    nameMyanmar: 'ထိုးစက် ၆',
    status: 'scheduled',
    operatorId: 'OP005',
    productId: 'P004',
    jobId: 'JOB006',
    targetQty: 8000,
    completedQty: 0,
    startTime: '15:00',
    estimatedFinish: '23:00',
    workOrderNo: 'WO-2025-006',
    efficiency: 0,
    cycleTime: 0,
    alerts: [],
    lastUpdate: new Date().toISOString(),
    moldCondition: 'Setup',
    temperature: 20,
    pressure: 0
  }
];

const plannedJobs = [
  {
    id: 'JOB007',
    productId: 'P005',
    targetQty: 1500,
    priority: 'high',
    duration: 6,
    scheduledStart: '16:00',
    machineId: null,
    status: 'planned'
  },
  {
    id: 'JOB008',
    productId: 'P001',
    targetQty: 4000,
    priority: 'medium',
    duration: 5,
    scheduledStart: '18:00',
    machineId: null,
    status: 'planned'
  }
];

const alerts = [
  {
    id: 1,
    type: 'critical',
    machine: 'INJ-M003',
    message: 'Machine blocked for 45 minutes',
    messageMyanmar: 'စက်ပိတ်နေပြီး ၄၅ မိနစ်ရှိပြီ',
    time: '14:15',
    priority: 'high'
  },
  {
    id: 2,
    type: 'warning',
    machine: 'INJ-M001',
    message: 'Material running low',
    messageMyanmar: 'ကုန်ကြမ်း နည်းနေပြီ',
    time: '14:30',
    priority: 'medium'
  },
  {
    id: 3,
    type: 'info',
    machine: 'INJ-M004',
    message: 'Target exceeded by 5%',
    messageMyanmar: 'ပစ်မှန်းထားတာထက် ၅% ပိုရရှိ',
    time: '14:45',
    priority: 'low'
  }
];

// Utility functions
const getStatusColor = (status: string) => {
  const colors = {
    running: 'bg-green-500',
    idle: 'bg-yellow-500',
    blocked: 'bg-red-500',
    scheduled: 'bg-blue-500',
    maintenance: 'bg-orange-500'
  };
  return colors[status as keyof typeof colors] || 'bg-gray-500';
};

const getStatusIcon = (status: string) => {
  const icons = {
    running: Play,
    idle: Pause,
    blocked: XCircle,
    scheduled: Clock,
    maintenance: Wrench
  };
  const IconComponent = icons[status as keyof typeof icons] || Square;
  return <IconComponent className="h-4 w-4" />;
};

const formatTime = (timeString: string | null) => {
  if (!timeString) return '--:--';
  return timeString;
};

const getProgressPercentage = (completed: number, target: number) => {
  if (target === 0) return 0;
  return Math.min(Math.round((completed / target) * 100), 100);
};

// Draggable Job Component
const DraggableJob = ({ job, onDrop }: { job: any; onDrop: (jobId: string, machineId: string, timeSlot: string) => void }) => {
  const [{ isDragging }, drag] = useDrag({
    type: 'job',
    item: { id: job.id, type: 'job' },
    collect: (monitor) => ({
      isDragging: monitor.isDragging(),
    }),
  });

  const product = products.find(p => p.id === job.productId);

  return (
    <motion.div
      ref={drag}
      className={`bg-purple-100 border-l-4 border-purple-500 p-3 rounded cursor-move min-w-[200px] ${
        isDragging ? 'opacity-50' : ''
      }`}
      whileHover={{ scale: 1.02 }}
      whileTap={{ scale: 0.98 }}
    >
      <div className="flex items-center gap-2 mb-1">
        <Badge className="bg-purple-500 text-white text-xs">
          {job.id}
        </Badge>
        <Badge variant="outline" className={`text-xs ${
          job.priority === 'high' ? 'border-red-500 text-red-600' :
          job.priority === 'medium' ? 'border-yellow-500 text-yellow-600' :
          'border-green-500 text-green-600'
        }`}>
          {job.priority}
        </Badge>
      </div>
      <div className="text-sm font-medium">{product?.name || 'Unknown Product'}</div>
      <div className="text-xs text-slate-600">
        Qty: {job.targetQty.toLocaleString()} | Duration: {job.duration}h
      </div>
    </motion.div>
  );
};

// Drop Zone for Timeline
const TimelineDropZone = ({ machineId, timeSlot, onDrop, children }: {
  machineId: string;
  timeSlot: string;
  onDrop: (jobId: string, machineId: string, timeSlot: string) => void;
  children: React.ReactNode;
}) => {
  const [{ isOver }, drop] = useDrop({
    accept: 'job',
    drop: (item: { id: string; type: string }) => {
      onDrop(item.id, machineId, timeSlot);
    },
    collect: (monitor) => ({
      isOver: monitor.isOver(),
    }),
  });

  return (
    <div
      ref={drop}
      className={`min-h-[60px] border-2 border-dashed transition-all ${
        isOver ? 'border-blue-500 bg-blue-50' : 'border-transparent'
      }`}
    >
      {children}
    </div>
  );
};

export function FactoryDashboard() {
  const [currentShift, setCurrentShift] = useState('Day Shift A');
  const [selectedDate, setSelectedDate] = useState(new Date().toISOString().split('T')[0]);
  const [activeTab, setActiveTab] = useState('monitoring');
  const [selectedMachine, setSelectedMachine] = useState<string | null>(null);
  const [newJobDialog, setNewJobDialog] = useState(false);
  const [timelineView, setTimelineView] = useState('today');

  // Handle job drag and drop
  const handleJobDrop = useCallback((jobId: string, machineId: string, timeSlot: string) => {
    toast.success(`Job ${jobId} scheduled on ${machineId} at ${timeSlot}`);
    // In real implementation, update the job scheduling data
  }, []);

  // Auto-refresh simulation
  const [lastUpdate, setLastUpdate] = useState(new Date());
  React.useEffect(() => {
    const interval = setInterval(() => {
      setLastUpdate(new Date());
    }, 30000); // Refresh every 30 seconds
    return () => clearInterval(interval);
  }, []);

  return (
    <DndProvider backend={HTML5Backend}>
      <div className="min-h-screen bg-slate-50">
        {/* Top Navigation Bar */}
        <div className="bg-white border-b border-slate-200 px-6 py-4">
          <div className="flex items-center justify-between">
            {/* Logo */}
            <div className="flex items-center gap-3">
              <div className="w-10 h-10 bg-blue-600 rounded-lg flex items-center justify-center">
                <Factory className="h-6 w-6 text-white" />
              </div>
              <div>
                <h1 className="text-xl font-bold text-slate-900">Smart Factory Dashboard</h1>
                <p className="text-sm text-slate-600">စမတ်စက်ရုံ ထိန်းချုပ်ရေး</p>
              </div>
            </div>

            {/* Date & Shift Selector */}
            <div className="flex items-center gap-4">
              <div className="flex items-center gap-2">
                <Calendar className="h-5 w-5 text-slate-600" />
                <Input
                  type="date"
                  value={selectedDate}
                  onChange={(e) => setSelectedDate(e.target.value)}
                  className="w-40"
                />
              </div>
              <Select value={currentShift} onValueChange={setCurrentShift}>
                <SelectTrigger className="w-48">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="Day Shift A">Day Shift A (08:00-16:00)</SelectItem>
                  <SelectItem value="Day Shift B">Day Shift B (16:00-24:00)</SelectItem>
                  <SelectItem value="Night Shift">Night Shift (00:00-08:00)</SelectItem>
                </SelectContent>
              </Select>
            </div>

            {/* User Profile */}
            <div className="flex items-center gap-3">
              <div className="text-right">
                <div className="text-sm font-medium">Production Manager</div>
                <div className="text-xs text-slate-600">ထုတ်လုပ်ရေးမန်နေဂျာ</div>
              </div>
              <Avatar>
                <AvatarImage src="/api/placeholder/40/40" />
                <AvatarFallback>PM</AvatarFallback>
              </Avatar>
              <div className="flex items-center gap-1">
                <div className={`w-2 h-2 rounded-full bg-green-500`}></div>
                <span className="text-xs text-slate-600">Online</span>
              </div>
            </div>
          </div>
        </div>

        {/* Main Content */}
        <div className="p-6">
          <Tabs value={activeTab} onValueChange={setActiveTab} className="space-y-6">
            <TabsList className="grid w-full grid-cols-4 lg:w-auto lg:grid-cols-4">
              <TabsTrigger value="monitoring" className="text-base font-medium">
                📊 Live Monitoring
              </TabsTrigger>
              <TabsTrigger value="planning" className="text-base font-medium">
                📅 Production Planning
              </TabsTrigger>
              <TabsTrigger value="analytics" className="text-base font-medium">
                📈 Analytics
              </TabsTrigger>
              <TabsTrigger value="reports" className="text-base font-medium">
                📋 Reports
              </TabsTrigger>
            </TabsList>

            <TabsContent value="monitoring" className="space-y-6">
              {/* Factory KPI Summary */}
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-6 mb-8">
                {/* Running Machines */}
                <motion.div
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: 0.1 }}
                >
                  <Card className="bg-gradient-to-br from-green-500 to-green-600 text-white">
                    <CardContent className="p-6">
                      <div className="flex items-center justify-between">
                        <div>
                          <p className="text-green-100 text-sm font-medium">🟢 Running Machines</p>
                          <p className="text-xs text-green-100">လည်ပတ်နေသောစက်များ</p>
                        </div>
                        <Play className="h-8 w-8 text-green-100" />
                      </div>
                      <div className="mt-4">
                        <div className="text-3xl font-bold">
                          {factoryKPIs.runningMachines.count}
                        </div>
                        <div className="text-sm text-green-100">
                          of {factoryKPIs.runningMachines.total} machines ({factoryKPIs.runningMachines.percentage}%)
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                </motion.div>

                {/* Idle Machines */}
                <motion.div
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: 0.2 }}
                >
                  <Card className="bg-gradient-to-br from-yellow-500 to-yellow-600 text-white">
                    <CardContent className="p-6">
                      <div className="flex items-center justify-between">
                        <div>
                          <p className="text-yellow-100 text-sm font-medium">🟡 Idle Machines</p>
                          <p className="text-xs text-yellow-100">အပတ်စဉ်စက်များ</p>
                        </div>
                        <Pause className="h-8 w-8 text-yellow-100" />
                      </div>
                      <div className="mt-4">
                        <div className="text-3xl font-bold">
                          {factoryKPIs.idleMachines.count}
                        </div>
                        <div className="text-sm text-yellow-100">
                          of {factoryKPIs.idleMachines.total} machines ({factoryKPIs.idleMachines.percentage}%)
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                </motion.div>

                {/* Blocked/Down Machines */}
                <motion.div
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: 0.3 }}
                >
                  <Card className="bg-gradient-to-br from-red-500 to-red-600 text-white">
                    <CardContent className="p-6">
                      <div className="flex items-center justify-between">
                        <div>
                          <p className="text-red-100 text-sm font-medium">🔴 Blocked/Down</p>
                          <p className="text-xs text-red-100">ပိတ်နေသောစက်များ</p>
                        </div>
                        <AlertCircle className="h-8 w-8 text-red-100" />
                      </div>
                      <div className="mt-4">
                        <div className="text-3xl font-bold">
                          {factoryKPIs.blockedMachines.count}
                        </div>
                        <div className="text-sm text-red-100">
                          of {factoryKPIs.blockedMachines.total} machines ({factoryKPIs.blockedMachines.percentage}%)
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                </motion.div>

                {/* Total Production */}
                <motion.div
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: 0.4 }}
                >
                  <Card className="bg-gradient-to-br from-blue-500 to-blue-600 text-white">
                    <CardContent className="p-6">
                      <div className="flex items-center justify-between">
                        <div>
                          <p className="text-blue-100 text-sm font-medium">📦 Units Produced Today</p>
                          <p className="text-xs text-blue-100">ယနေ့ထုတ်လုပ်မှု</p>
                        </div>
                        <Package className="h-8 w-8 text-blue-100" />
                      </div>
                      <div className="mt-4">
                        <div className="text-3xl font-bold">
                          {factoryKPIs.totalProduced.count.toLocaleString()}
                        </div>
                        <div className="text-sm text-blue-100">
                          Target: {factoryKPIs.totalProduced.target.toLocaleString()} ({factoryKPIs.totalProduced.percentage}%)
                        </div>
                        <Progress 
                          value={factoryKPIs.totalProduced.percentage} 
                          className="mt-2 h-2 bg-blue-400"
                        />
                      </div>
                    </CardContent>
                  </Card>
                </motion.div>

                {/* Defects */}
                <motion.div
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: 0.5 }}
                >
                  <Card className="bg-gradient-to-br from-orange-500 to-orange-600 text-white">
                    <CardContent className="p-6">
                      <div className="flex items-center justify-between">
                        <div>
                          <p className="text-orange-100 text-sm font-medium">❌ Defects Today</p>
                          <p className="text-xs text-orange-100">ယနေ့ချွတ်ယွင်းချက်များ</p>
                        </div>
                        <XCircle className="h-8 w-8 text-orange-100" />
                      </div>
                      <div className="mt-4">
                        <div className="text-3xl font-bold">
                          {factoryKPIs.defectsToday.count}
                        </div>
                        <div className="text-sm text-orange-100">
                          {factoryKPIs.defectsToday.percentage}% defect rate
                        </div>
                        <div className="text-xs text-orange-100 mt-1">
                          Top: {factoryKPIs.defectsToday.topReason}
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                </motion.div>
              </div>

              <div className="grid grid-cols-1 lg:grid-cols-4 gap-6">
                {/* Machine Tiles Panel - 3/4 width */}
                <div className="lg:col-span-3">
                  <Card>
                    <CardHeader>
                      <CardTitle className="flex items-center justify-between">
                        <div className="flex items-center gap-2">
                          <Factory className="h-6 w-6 text-blue-600" />
                          <span>Live Machine Status</span>
                          <span className="text-sm font-normal text-slate-600">တိုက်ရိုက်စက်အခြေအနေ</span>
                        </div>
                        <div className="flex items-center gap-2 text-sm text-slate-600">
                          <RefreshCw className="h-4 w-4" />
                          <span>Last updated: {lastUpdate.toLocaleTimeString()}</span>
                        </div>
                      </CardTitle>
                    </CardHeader>
                    <CardContent>
                      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                        {mockMachines.map((machine, index) => {
                          const operator = operators.find(op => op.id === machine.operatorId);
                          const product = products.find(p => p.id === machine.productId);
                          const progressPercentage = getProgressPercentage(machine.completedQty, machine.targetQty);

                          return (
                            <motion.div
                              key={machine.id}
                              initial={{ opacity: 0, scale: 0.9 }}
                              animate={{ opacity: 1, scale: 1 }}
                              transition={{ delay: index * 0.1 }}
                              whileHover={{ scale: 1.02 }}
                              className="cursor-pointer"
                              onClick={() => setSelectedMachine(machine.id)}
                            >
                              <Card className={`border-l-4 ${
                                machine.status === 'running' ? 'border-l-green-500 bg-green-50' :
                                machine.status === 'idle' ? 'border-l-yellow-500 bg-yellow-50' :
                                machine.status === 'blocked' ? 'border-l-red-500 bg-red-50' :
                                machine.status === 'scheduled' ? 'border-l-blue-500 bg-blue-50' :
                                'border-l-orange-500 bg-orange-50'
                              }`}>
                                <CardContent className="p-4">
                                  {/* Machine Header */}
                                  <div className="flex items-center justify-between mb-3">
                                    <div>
                                      <h3 className="font-bold text-base">{machine.name}</h3>
                                      <p className="text-xs text-slate-600">{machine.nameMyanmar}</p>
                                    </div>
                                    <div className="flex items-center gap-2">
                                      <div className={`w-3 h-3 rounded-full ${getStatusColor(machine.status)} ${
                                        machine.status === 'running' ? 'animate-pulse' : ''
                                      }`}></div>
                                      {getStatusIcon(machine.status)}
                                    </div>
                                  </div>

                                  {/* Status Badge */}
                                  <div className="mb-3">
                                    <Badge className={`${
                                      machine.status === 'running' ? 'bg-green-100 text-green-800' :
                                      machine.status === 'idle' ? 'bg-yellow-100 text-yellow-800' :
                                      machine.status === 'blocked' ? 'bg-red-100 text-red-800' :
                                      machine.status === 'scheduled' ? 'bg-blue-100 text-blue-800' :
                                      'bg-orange-100 text-orange-800'
                                    } text-sm font-medium`}>
                                      {machine.status.toUpperCase()} {machine.status === 'running' && `- ${progressPercentage}%`}
                                    </Badge>
                                  </div>

                                  {/* Operator Info */}
                                  {operator && (
                                    <div className="flex items-center gap-2 mb-3">
                                      <Avatar className="h-8 w-8">
                                        <AvatarImage src={operator.photo} />
                                        <AvatarFallback>{operator.id}</AvatarFallback>
                                      </Avatar>
                                      <div>
                                        <p className="text-sm font-medium">{operator.name}</p>
                                        <p className="text-xs text-slate-600">{operator.nameMyanmar} • Shift {operator.shift}</p>
                                      </div>
                                    </div>
                                  )}

                                  {/* Product & Job Info */}
                                  {product && machine.jobId && (
                                    <div className="mb-3">
                                      <div className="flex items-center gap-2 mb-2">
                                        <img 
                                          src={product.image} 
                                          alt={product.name}
                                          className="w-8 h-8 rounded border"
                                        />
                                        <div className="flex-1">
                                          <p className="text-sm font-medium">{product.name}</p>
                                          <p className="text-xs text-slate-600">Job: {machine.jobId}</p>
                                        </div>
                                      </div>
                                      
                                      {machine.targetQty > 0 && (
                                        <div className="space-y-1">
                                          <div className="flex justify-between text-sm">
                                            <span>Progress:</span>
                                            <span className="font-medium">
                                              {machine.completedQty.toLocaleString()} / {machine.targetQty.toLocaleString()}
                                            </span>
                                          </div>
                                          <Progress value={progressPercentage} className="h-2" />
                                        </div>
                                      )}
                                    </div>
                                  )}

                                  {/* Alerts */}
                                  {machine.alerts.length > 0 && (
                                    <div className="mb-3">
                                      {machine.alerts.map((alert, alertIndex) => (
                                        <div 
                                          key={alertIndex}
                                          className="flex items-center gap-1 text-xs bg-yellow-100 text-yellow-800 px-2 py-1 rounded mb-1"
                                        >
                                          <AlertTriangle className="h-3 w-3" />
                                          <span>{alert}</span>
                                        </div>
                                      ))}
                                    </div>
                                  )}

                                  {/* Footer Info */}
                                  <div className="text-xs text-slate-600 space-y-1 border-t pt-2">
                                    {machine.startTime && (
                                      <div className="flex justify-between">
                                        <span>Started:</span>
                                        <span>{formatTime(machine.startTime)}</span>
                                      </div>
                                    )}
                                    {machine.estimatedFinish && (
                                      <div className="flex justify-between">
                                        <span>ETA:</span>
                                        <span>{formatTime(machine.estimatedFinish)}</span>
                                      </div>
                                    )}
                                    {machine.workOrderNo && (
                                      <div className="flex justify-between">
                                        <span>WO:</span>
                                        <span className="font-mono">{machine.workOrderNo}</span>
                                      </div>
                                    )}
                                    {machine.efficiency > 0 && (
                                      <div className="flex justify-between">
                                        <span>Efficiency:</span>
                                        <span className={`font-medium ${
                                          machine.efficiency >= 90 ? 'text-green-600' :
                                          machine.efficiency >= 75 ? 'text-yellow-600' :
                                          'text-red-600'
                                        }`}>
                                          {machine.efficiency}%
                                        </span>
                                      </div>
                                    )}
                                  </div>
                                </CardContent>
                              </Card>
                            </motion.div>
                          );
                        })}
                      </div>
                    </CardContent>
                  </Card>
                </div>

                {/* Right Sidebar - 1/4 width */}
                <div className="space-y-6">
                  {/* Live Alerts */}
                  <Card>
                    <CardHeader>
                      <CardTitle className="flex items-center gap-2">
                        <Bell className="h-5 w-5 text-red-600" />
                        <span>🔔 Live Alerts</span>
                      </CardTitle>
                    </CardHeader>
                    <CardContent className="space-y-3">
                      <AnimatePresence>
                        {alerts.map((alert) => (
                          <motion.div
                            key={alert.id}
                            initial={{ opacity: 0, x: 20 }}
                            animate={{ opacity: 1, x: 0 }}
                            exit={{ opacity: 0, x: -20 }}
                            className={`p-3 rounded-lg border-l-4 ${
                              alert.type === 'critical' ? 'border-l-red-500 bg-red-50' :
                              alert.type === 'warning' ? 'border-l-yellow-500 bg-yellow-50' :
                              'border-l-blue-500 bg-blue-50'
                            }`}
                          >
                            <div className="flex items-start gap-2">
                              <div className={`w-2 h-2 rounded-full mt-2 ${
                                alert.type === 'critical' ? 'bg-red-500' :
                                alert.type === 'warning' ? 'bg-yellow-500' :
                                'bg-blue-500'
                              }`}></div>
                              <div className="flex-1">
                                <p className="text-sm font-medium">{alert.machine}</p>
                                <p className="text-sm text-slate-700">{alert.message}</p>
                                <p className="text-xs text-slate-600">{alert.messageMyanmar}</p>
                                <p className="text-xs text-slate-500 mt-1">{alert.time}</p>
                              </div>
                            </div>
                          </motion.div>
                        ))}
                      </AnimatePresence>
                    </CardContent>
                  </Card>

                  {/* Quick Actions */}
                  <Card>
                    <CardHeader>
                      <CardTitle className="flex items-center gap-2">
                        <Zap className="h-5 w-5 text-blue-600" />
                        <span>⚡ Quick Actions</span>
                      </CardTitle>
                    </CardHeader>
                    <CardContent className="space-y-3">
                      <Button 
                        className="w-full bg-blue-600 hover:bg-blue-700 text-white"
                        onClick={() => setNewJobDialog(true)}
                      >
                        <Plus className="h-4 w-4 mr-2" />
                        ➕ Add New Job
                      </Button>
                      
                      <Button 
                        variant="outline" 
                        className="w-full border-purple-600 text-purple-600 hover:bg-purple-50"
                        onClick={() => setActiveTab('planning')}
                      >
                        <ArrowRightLeft className="h-4 w-4 mr-2" />
                        🔄 Reschedule Job
                      </Button>
                      
                      <Button 
                        variant="outline" 
                        className="w-full border-green-600 text-green-600 hover:bg-green-50"
                      >
                        <Package className="h-4 w-4 mr-2" />
                        📦 Material Request
                      </Button>
                      
                      <Button 
                        variant="outline" 
                        className="w-full border-orange-600 text-orange-600 hover:bg-orange-50"
                      >
                        <Wrench className="h-4 w-4 mr-2" />
                        🔧 Maintenance Alert
                      </Button>
                    </CardContent>
                  </Card>

                  {/* Top Operators Today */}
                  <Card>
                    <CardHeader>
                      <CardTitle className="flex items-center gap-2">
                        <Award className="h-5 w-5 text-yellow-600" />
                        <span>🏆 Top Operators</span>
                      </CardTitle>
                    </CardHeader>
                    <CardContent className="space-y-3">
                      {operators.slice(0, 3).map((operator, index) => (
                        <div key={operator.id} className="flex items-center gap-3">
                          <div className="text-lg">
                            {index === 0 ? '🥇' : index === 1 ? '🥈' : '🥉'}
                          </div>
                          <Avatar className="h-8 w-8">
                            <AvatarImage src={operator.photo} />
                            <AvatarFallback>{operator.id}</AvatarFallback>
                          </Avatar>
                          <div className="flex-1">
                            <p className="text-sm font-medium">{operator.name}</p>
                            <p className="text-xs text-slate-600">{operator.nameMyanmar}</p>
                          </div>
                          <div className="text-right">
                            <div className="text-sm font-medium">{operator.rating}</div>
                            <div className="text-xs text-slate-600">Rating</div>
                          </div>
                        </div>
                      ))}
                    </CardContent>
                  </Card>
                </div>
              </div>
            </TabsContent>

            <TabsContent value="planning" className="space-y-6">
              {/* Planning Timeline */}
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center justify-between">
                    <div className="flex items-center gap-2">
                      <Calendar className="h-6 w-6 text-purple-600" />
                      <span>📅 Production Planning Timeline</span>
                      <span className="text-sm font-normal text-slate-600">ထုတ်လုပ်မှုအစီအစဉ်</span>
                    </div>
                    <div className="flex items-center gap-2">
                      <Select value={timelineView} onValueChange={setTimelineView}>
                        <SelectTrigger className="w-32">
                          <SelectValue />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="today">Today</SelectItem>
                          <SelectItem value="tomorrow">Tomorrow</SelectItem>
                          <SelectItem value="week">This Week</SelectItem>
                        </SelectContent>
                      </Select>
                    </div>
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  {/* Timeline Header */}
                  <div className="grid grid-cols-25 gap-1 mb-4 text-xs text-slate-600">
                    <div className="col-span-3 font-medium">Machine</div>
                    {Array.from({ length: 24 }, (_, i) => (
                      <div key={i} className="text-center border-l border-slate-200 px-1">
                        {String(i).padStart(2, '0')}:00
                      </div>
                    ))}
                  </div>

                  {/* Timeline Rows */}
                  <div className="space-y-2">
                    {mockMachines.map((machine) => (
                      <div key={machine.id} className="grid grid-cols-25 gap-1 items-center">
                        <div className="col-span-3 p-2 bg-slate-50 rounded text-sm font-medium">
                          <div>{machine.name}</div>
                          <div className="text-xs text-slate-600">{machine.nameMyanmar}</div>
                        </div>
                        
                        {Array.from({ length: 24 }, (_, hour) => {
                          const timeSlot = `${String(hour).padStart(2, '0')}:00`;
                          const isCurrentTime = hour === new Date().getHours();
                          
                          return (
                            <TimelineDropZone
                              key={hour}
                              machineId={machine.id}
                              timeSlot={timeSlot}
                              onDrop={handleJobDrop}
                            >
                              <div 
                                className={`h-12 border border-slate-200 rounded ${
                                  isCurrentTime ? 'bg-blue-100 border-blue-300' : 'bg-white'
                                }`}
                              >
                                {/* Show running job */}
                                {machine.status === 'running' && machine.jobId && 
                                 hour >= parseInt(machine.startTime?.split(':')[0] || '0') && 
                                 hour < parseInt(machine.estimatedFinish?.split(':')[0] || '24') && (
                                  <div className="h-full bg-green-400 rounded px-1 text-xs text-white flex items-center justify-center font-medium">
                                    {machine.jobId}
                                  </div>
                                )}

                                {/* Show scheduled job */}
                                {machine.status === 'scheduled' && machine.jobId && 
                                 hour >= parseInt(machine.startTime?.split(':')[0] || '0') && 
                                 hour < parseInt(machine.estimatedFinish?.split(':')[0] || '24') && (
                                  <div className="h-full bg-blue-400 rounded px-1 text-xs text-white flex items-center justify-center font-medium">
                                    {machine.jobId}
                                  </div>
                                )}

                                {/* Show maintenance */}
                                {machine.status === 'maintenance' && hour >= 9 && hour < 11 && (
                                  <div className="h-full bg-orange-400 rounded px-1 text-xs text-white flex items-center justify-center font-medium">
                                    MAINT
                                  </div>
                                )}
                              </div>
                            </TimelineDropZone>
                          );
                        })}
                      </div>
                    ))}
                  </div>

                  {/* Planned Jobs (Draggable) */}
                  <div className="mt-6 border-t pt-4">
                    <h3 className="text-lg font-medium mb-3">📋 Planned Jobs (Drag to Schedule)</h3>
                    <div className="flex gap-3 flex-wrap">
                      {plannedJobs.map((job) => (
                        <DraggableJob key={job.id} job={job} onDrop={handleJobDrop} />
                      ))}
                    </div>
                  </div>

                  {/* Legend */}
                  <div className="mt-6 flex items-center gap-4 text-sm">
                    <div className="flex items-center gap-2">
                      <div className="w-4 h-4 bg-green-400 rounded"></div>
                      <span>Running</span>
                    </div>
                    <div className="flex items-center gap-2">
                      <div className="w-4 h-4 bg-blue-400 rounded"></div>
                      <span>Scheduled</span>
                    </div>
                    <div className="flex items-center gap-2">
                      <div className="w-4 h-4 bg-orange-400 rounded"></div>
                      <span>Maintenance</span>
                    </div>
                    <div className="flex items-center gap-2">
                      <div className="w-4 h-4 bg-purple-400 rounded"></div>
                      <span>Planned</span>
                    </div>
                  </div>
                </CardContent>
              </Card>
            </TabsContent>

            <TabsContent value="analytics" className="space-y-6">
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                <Card>
                  <CardHeader>
                    <CardTitle className="flex items-center gap-2">
                      <BarChart3 className="h-5 w-5 text-blue-600" />
                      <span>📊 Machine Efficiency</span>
                    </CardTitle>
                  </CardHeader>
                  <CardContent>
                    <div className="space-y-3">
                      {mockMachines.filter(m => m.efficiency > 0).map((machine) => (
                        <div key={machine.id} className="space-y-1">
                          <div className="flex justify-between text-sm">
                            <span>{machine.name}</span>
                            <span className="font-medium">{machine.efficiency}%</span>
                          </div>
                          <Progress value={machine.efficiency} className="h-2" />
                        </div>
                      ))}
                    </div>
                  </CardContent>
                </Card>

                <Card>
                  <CardHeader>
                    <CardTitle className="flex items-center gap-2">
                      <PieChart className="h-5 w-5 text-green-600" />
                      <span>🥧 Downtime Reasons</span>
                    </CardTitle>
                  </CardHeader>
                  <CardContent>
                    <div className="space-y-3">
                      <div className="flex justify-between items-center">
                        <span className="text-sm">Mold Jam</span>
                        <div className="flex items-center gap-2">
                          <div className="w-16 h-2 bg-red-200 rounded">
                            <div className="w-12 h-2 bg-red-500 rounded"></div>
                          </div>
                          <span className="text-sm font-medium">35%</span>
                        </div>
                      </div>
                      <div className="flex justify-between items-center">
                        <span className="text-sm">Material Shortage</span>
                        <div className="flex items-center gap-2">
                          <div className="w-16 h-2 bg-yellow-200 rounded">
                            <div className="w-10 h-2 bg-yellow-500 rounded"></div>
                          </div>
                          <span className="text-sm font-medium">25%</span>
                        </div>
                      </div>
                      <div className="flex justify-between items-center">
                        <span className="text-sm">Maintenance</span>
                        <div className="flex items-center gap-2">
                          <div className="w-16 h-2 bg-orange-200 rounded">
                            <div className="w-8 h-2 bg-orange-500 rounded"></div>
                          </div>
                          <span className="text-sm font-medium">20%</span>
                        </div>
                      </div>
                      <div className="flex justify-between items-center">
                        <span className="text-sm">Quality Issues</span>
                        <div className="flex items-center gap-2">
                          <div className="w-16 h-2 bg-purple-200 rounded">
                            <div className="w-6 h-2 bg-purple-500 rounded"></div>
                          </div>
                          <span className="text-sm font-medium">15%</span>
                        </div>
                      </div>
                      <div className="flex justify-between items-center">
                        <span className="text-sm">Others</span>
                        <div className="flex items-center gap-2">
                          <div className="w-16 h-2 bg-slate-200 rounded">
                            <div className="w-2 h-2 bg-slate-500 rounded"></div>
                          </div>
                          <span className="text-sm font-medium">5%</span>
                        </div>
                      </div>
                    </div>
                  </CardContent>
                </Card>

                <Card>
                  <CardHeader>
                    <CardTitle className="flex items-center gap-2">
                      <Package className="h-5 w-5 text-purple-600" />
                      <span>📦 Material Stock</span>
                    </CardTitle>
                  </CardHeader>
                  <CardContent>
                    <div className="space-y-3">
                      <div className="p-3 bg-green-50 rounded-lg">
                        <div className="flex justify-between items-center">
                          <span className="text-sm font-medium">PET Resin Clear</span>
                          <Badge className="bg-green-100 text-green-800">Good</Badge>
                        </div>
                        <div className="text-xs text-slate-600 mt-1">Stock: 2,450 kg</div>
                      </div>
                      <div className="p-3 bg-yellow-50 rounded-lg">
                        <div className="flex justify-between items-center">
                          <span className="text-sm font-medium">PP Blue Compound</span>
                          <Badge className="bg-yellow-100 text-yellow-800">Low</Badge>
                        </div>
                        <div className="text-xs text-slate-600 mt-1">Stock: 125 kg</div>
                      </div>
                      <div className="p-3 bg-red-50 rounded-lg">
                        <div className="flex justify-between items-center">
                          <span className="text-sm font-medium">ABS Black</span>
                          <Badge className="bg-red-100 text-red-800">Critical</Badge>
                        </div>
                        <div className="text-xs text-slate-600 mt-1">Stock: 45 kg</div>
                      </div>
                    </div>
                  </CardContent>
                </Card>
              </div>
            </TabsContent>

            <TabsContent value="reports" className="space-y-6">
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <BarChart3 className="h-6 w-6 text-blue-600" />
                    <span>📊 Production Reports</span>
                    <span className="text-sm font-normal text-slate-600">ထုတ်လုပ်မှုအစီရင်ခံစာများ</span>
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="text-center py-12">
                    <div className="text-slate-400 mb-4">
                      <BarChart3 className="h-16 w-16 mx-auto" />
                    </div>
                    <h3 className="text-lg font-medium text-slate-600 mb-2">
                      Detailed Reports Coming Soon
                    </h3>
                    <p className="text-slate-500">
                      အသေးစိတ်အစီရင်ခံစာများ မကြာမီရရှိမည်
                    </p>
                  </div>
                </CardContent>
              </Card>
            </TabsContent>
          </Tabs>
        </div>

        {/* New Job Dialog */}
        <Dialog open={newJobDialog} onOpenChange={setNewJobDialog}>
          <DialogContent className="max-w-2xl">
            <DialogHeader>
              <DialogTitle className="flex items-center gap-2">
                <Plus className="h-5 w-5 text-blue-600" />
                Add New Production Job
              </DialogTitle>
              <DialogDescription>
                Create a new production job and assign it to a machine
                စက်သစ်တစ်ခုတွင် ထုတ်လုပ်မှုအလုပ်သစ်ဖန်တီးပါ
              </DialogDescription>
            </DialogHeader>
            <div className="space-y-4">
              <div>
                <Label>Product</Label>
                <Select>
                  <SelectTrigger>
                    <SelectValue placeholder="Select product" />
                  </SelectTrigger>
                  <SelectContent>
                    {products.map((product) => (
                      <SelectItem key={product.id} value={product.id}>
                        {product.name} - {product.color}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              <div>
                <Label>Target Quantity</Label>
                <Input type="number" placeholder="Enter quantity" />
              </div>
              <div>
                <Label>Machine</Label>
                <Select>
                  <SelectTrigger>
                    <SelectValue placeholder="Select machine" />
                  </SelectTrigger>
                  <SelectContent>
                    {mockMachines.filter(m => m.status === 'idle').map((machine) => (
                      <SelectItem key={machine.id} value={machine.id}>
                        {machine.name} - {machine.nameMyanmar}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              <div className="flex justify-end gap-2 pt-4">
                <Button variant="outline" onClick={() => setNewJobDialog(false)}>
                  Cancel
                </Button>
                <Button 
                  className="bg-blue-600 hover:bg-blue-700"
                  onClick={() => {
                    toast.success('New job created successfully!');
                    setNewJobDialog(false);
                  }}
                >
                  Create Job
                </Button>
              </div>
            </div>
          </DialogContent>
        </Dialog>
      </div>
    </DndProvider>
  );
}