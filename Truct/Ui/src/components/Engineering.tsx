import React, { useState, useEffect } from 'react';
import { 
  Activity, AlertTriangle, Calendar, CheckCircle, Clock, Cog, Download, 
  Eye, Filter, FormInput, Package, Plus, Search, Settings, User, Wrench,
  Thermometer, Layers, Bolt, Zap, Timer, ArrowUp, Edit, Play, Save, Send,
  CheckSquare, XCircle, TrendingUp, FileText, Info, History, CalendarDays,
  List, MoreHorizontal, Home, Scan, Camera, Mic, MapPin, RefreshCw,
  Phone, MessageSquare, Bell, ChevronLeft, ChevronRight, Menu, X
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
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogTrigger } from './ui/dialog';
import { Switch } from './ui/switch';
import { Tabs, TabsContent, TabsList, TabsTrigger } from './ui/tabs';
import { Avatar, AvatarFallback, AvatarImage } from './ui/avatar';
import { Separator } from './ui/separator';
import { ScrollArea } from './ui/scroll-area';
import { EngineeringMobile } from './EngineeringMobile';

interface EngineeringProps {
  currentPage: string;
  onPageChange: (page: string) => void;
}

// Role types for engineering staff
type EngineerRole = 'plastic-technician' | 'mold-technician' | 'maintenance-engineer' | 'technician-head' | 'operator';

// Device type detection
type DeviceType = 'desktop' | 'mobile' | 'kiosk';

// Mock user role - in real app this would come from authentication
const CURRENT_USER_ROLE: EngineerRole = 'plastic-technician';
const CURRENT_DEVICE: DeviceType = 'desktop';

// Status configurations with bilingual support
const STATUS_CONFIG = {
  'running': { 
    color: 'bg-green-100 text-green-800 border-green-200', 
    label: '🟢 Running | လုပ်ငန်းလည်ပတ်နေ', 
    icon: '🟢' 
  },
  'idle': { 
    color: 'bg-yellow-100 text-yellow-800 border-yellow-200', 
    label: '🟡 Idle | ရပ်နား', 
    icon: '🟡' 
  },
  'down': { 
    color: 'bg-red-100 text-red-800 border-red-200', 
    label: '🔴 Down | ပျက်စီး', 
    icon: '🔴' 
  },
  'in-progress': { 
    color: 'bg-blue-100 text-blue-800 border-blue-200', 
    label: '🟡 In Progress | လုပ်ဆောင်နေ', 
    icon: '🟡' 
  },
  'pending': { 
    color: 'bg-gray-100 text-gray-800 border-gray-200', 
    label: 'Pending | စောင့်ဆိုင်း', 
    icon: null 
  },
  'completed': { 
    color: 'bg-green-100 text-green-800 border-green-200', 
    label: 'Completed | ပြီးစီး', 
    icon: null 
  },
  'escalated': { 
    color: 'bg-orange-100 text-orange-800 border-orange-200', 
    label: 'Escalated | တက်လှမ်းထား', 
    icon: null 
  },
  'overdue': { 
    color: 'bg-red-100 text-red-800 border-red-200', 
    label: 'Overdue | သက်တမ်းကျော်လွန်', 
    icon: null 
  }
};

// Priority configurations
const PRIORITY_CONFIG = {
  'P1': { color: 'bg-red-100 text-red-800 border-red-200', label: 'P1 Critical | P1 အရေးကြီး' },
  'P2': { color: 'bg-yellow-100 text-yellow-800 border-yellow-200', label: 'P2 High | P2 မြင့်' },
  'P3': { color: 'bg-blue-100 text-blue-800 border-blue-200', label: 'P3 Medium | P3 အလယ်အလတ်' },
  'P4': { color: 'bg-green-100 text-green-800 border-green-200', label: 'P4 Low | P4 နိမ့်' }
};

export function Engineering({ currentPage, onPageChange }: EngineeringProps) {
  const [currentRole, setCurrentRole] = useState<EngineerRole>(CURRENT_USER_ROLE);
  const [deviceType, setDeviceType] = useState<DeviceType>(CURRENT_DEVICE);
  const [isOffline, setIsOffline] = useState(false);
  const [activeTicket, setActiveTicket] = useState<string | null>(null);
  const [mobileView, setMobileView] = useState('home');
  const [selectedTicket, setSelectedTicket] = useState<any>(null);
  const [isDarkMode, setIsDarkMode] = useState(false);

  // Detect device type and online status
  useEffect(() => {
    const checkDevice = () => {
      const width = window.innerWidth;
      if (width >= 1024) {
        setDeviceType('desktop');
      } else if (width >= 640) {
        setDeviceType('kiosk');
      } else {
        setDeviceType('mobile');
      }
    };

    const handleOnline = () => setIsOffline(false);
    const handleOffline = () => setIsOffline(true);

    checkDevice();
    window.addEventListener('resize', checkDevice);
    window.addEventListener('online', handleOnline);
    window.addEventListener('offline', handleOffline);

    return () => {
      window.removeEventListener('resize', checkDevice);
      window.removeEventListener('online', handleOnline);
      window.removeEventListener('offline', handleOffline);
    };
  }, []);

  // Mock data for tickets, machines, etc.
  const mockTickets = [
    {
      id: 'TKT-20250907-001',
      machine: 'INJ-001',
      machineName: 'Injection Machine #1',
      symptom: 'Temperature sensor malfunction',
      priority: 'P1',
      status: 'in-progress',
      assignedTo: 'plastic-technician',
      assignedToName: 'John Doe',
      reportedBy: 'Operator 1',
      reportedTime: '2025-09-07 14:00',
      downtime: '2h 45m',
      slaRemaining: '1h 15m',
      description: 'Temperature readings showing inconsistent values, affecting injection quality',
      readings: {
        barrelZ1: 240,
        barrelZ2: 250,
        barrelZ3: 255,
        nozzle: 260,
        mold: 180,
        injectionPressure: 120,
        holdingPressure: 95,
        screwSpeed: 150
      }
    },
    {
      id: 'TKT-20250907-002',
      machine: 'EXT-001',
      machineName: 'Extruder Machine #1',
      symptom: 'Motor vibration excessive',
      priority: 'P2',
      status: 'pending',
      assignedTo: 'maintenance-engineer',
      assignedToName: 'Mike Smith',
      reportedBy: 'Supervisor 2',
      reportedTime: '2025-09-07 15:30',
      downtime: '30m',
      slaRemaining: '3h 30m',
      description: 'Unusual vibration detected during operation, may need bearing inspection'
    },
    {
      id: 'TKT-20250907-003',
      machine: 'MOL-001',
      machineName: 'Mold Station #1',
      symptom: 'Mold alignment issue',
      priority: 'P3',
      status: 'escalated',
      assignedTo: 'mold-technician',
      assignedToName: 'Sarah Johnson',
      reportedBy: 'QC Officer 1',
      reportedTime: '2025-09-07 10:00',
      downtime: '4h 15m',
      slaRemaining: '45m',
      description: 'Mold not aligning properly, causing dimensional issues'
    }
  ];

  const mockMachines = [
    { id: 'INJ-001', name: 'Injection Machine #1', status: 'running', uptime: '24h 30m', efficiency: 95 },
    { id: 'INJ-002', name: 'Injection Machine #2', status: 'idle', uptime: '2h 15m', efficiency: 0 },
    { id: 'INJ-003', name: 'Injection Machine #3', status: 'down', uptime: '0h 0m', efficiency: 0 },
    { id: 'EXT-001', name: 'Extruder Machine #1', status: 'running', uptime: '18h 45m', efficiency: 88 },
    { id: 'MOL-001', name: 'Mold Station #1', status: 'down', uptime: '0h 0m', efficiency: 0 }
  ];

  // Status and priority badge components
  const StatusBadge = ({ status }: { status: string }) => {
    const config = STATUS_CONFIG[status as keyof typeof STATUS_CONFIG] || STATUS_CONFIG.pending;
    return (
      <Badge className={`${config.color} border`}>
        {config.icon && <span className="mr-1">{config.icon}</span>}
        {config.label}
      </Badge>
    );
  };

  const PriorityBadge = ({ priority }: { priority: string }) => {
    const config = PRIORITY_CONFIG[priority as keyof typeof PRIORITY_CONFIG] || PRIORITY_CONFIG.P4;
    return <Badge className={`${config.color} border`}>{config.label}</Badge>;
  };

  // SLA Timer Component
  const SLATimer = ({ remaining, isOverdue = false }: { remaining: string; isOverdue?: boolean }) => (
    <div className={`flex items-center gap-1 ${isOverdue ? 'text-red-600' : 'text-orange-600'}`}>
      <Timer className="h-4 w-4" />
      <span className="text-sm font-medium">{remaining}</span>
    </div>
  );

  // Offline Banner
  const OfflineBanner = () => (
    isOffline && (
      <div className="bg-yellow-50 border-l-4 border-yellow-400 p-3 mb-4">
        <div className="flex items-center">
          <AlertTriangle className="h-5 w-5 text-yellow-400 mr-2" />
          <p className="text-sm text-yellow-800">
            You're currently offline. Changes will sync when connection is restored. | 
            သင်လောလောဆယ် offline ဖြစ်နေသည်။ ချိတ်ဆက်မှုပြန်လည်ရရှိသောအခါ အပြောင်းအလဲများ sync ဖြစ်မည်။
          </p>
        </div>
      </div>
    )
  );

  // Mobile Bottom Navigation
  const MobileBottomNav = () => (
    <div className="fixed bottom-0 left-0 right-0 bg-white border-t border-gray-200 px-4 py-2">
      <div className="flex justify-around">
        {[
          { id: 'home', icon: Home, label: 'Home' },
          { id: 'tasks', icon: CheckSquare, label: 'Tasks' },
          { id: 'scan', icon: Scan, label: 'Scan' },
          { id: 'history', icon: History, label: 'History' },
          { id: 'profile', icon: User, label: 'Profile' }
        ].map((item) => (
          <button
            key={item.id}
            onClick={() => setMobileView(item.id)}
            className={`flex flex-col items-center py-2 px-3 min-w-16 ${
              mobileView === item.id ? 'text-blue-600' : 'text-gray-400'
            }`}
          >
            <item.icon className="h-6 w-6" />
            <span className="text-xs mt-1">{item.label}</span>
          </button>
        ))}
      </div>
    </div>
  );

  // Operator Kiosk View (Large Red Button)
  const OperatorKioskView = () => (
    <div className="min-h-screen bg-slate-50 p-8">
      <div className="max-w-2xl mx-auto text-center">
        <h1 className="text-4xl font-bold text-slate-900 mb-4">
          Machine Status Kiosk | စက်အခြေအနေကိုင်က်
        </h1>
        <p className="text-lg text-slate-600 mb-12">
          Report machine issues immediately | စက်ပြဿနာများကို ချက်ခြင်းအစီရင်ခံပါ
        </p>
        
        <div className="grid grid-cols-2 gap-8 mb-12">
          <Card className="p-8">
            <CardHeader>
              <CardTitle className="flex items-center justify-center gap-2 text-xl">
                <Activity className="h-6 w-6 text-green-600" />
                Machines Running
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="text-4xl font-bold text-green-600">2/5</div>
              <p className="text-slate-600">လုပ်ငန်းလည်ပတ်နေသောစက်များ</p>
            </CardContent>
          </Card>
          
          <Card className="p-8">
            <CardHeader>
              <CardTitle className="flex items-center justify-center gap-2 text-xl">
                <AlertTriangle className="h-6 w-6 text-red-600" />
                Active Issues
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="text-4xl font-bold text-red-600">3</div>
              <p className="text-slate-600">လက်ရှိပြဿနာများ</p>
            </CardContent>
          </Card>
        </div>

        <MachineDownDialog />
      </div>
    </div>
  );

  // Machine Down Dialog Component
  const MachineDownDialog = () => {
    const [isOpen, setIsOpen] = useState(false);
    const [selectedMachine, setSelectedMachine] = useState('');
    const [symptom, setSymptom] = useState('');
    const [jobId, setJobId] = useState('JOB20250907-001-C1');

    const handleSubmit = () => {
      // Create ticket and notify plastic engineer
      setIsOpen(false);
      alert('Machine Down reported — Plastic Engineer notified | စက်ရပ်နေကြောင်းအစီရင်ခံပြီး — ပလတ်စတစ်အင်ဂျင်နီယာကို အကြောင်းကြားပြီး');
    };

    return (
      <Dialog open={isOpen} onOpenChange={setIsOpen}>
        <DialogTrigger asChild>
          <Button size="lg" className="bg-red-600 hover:bg-red-700 text-white px-16 py-8 text-2xl h-auto">
            <AlertTriangle className="h-8 w-8 mr-4" />
            🔴 Machine Down | စက်ရပ် (အရေးပေါ်)
          </Button>
        </DialogTrigger>
        <DialogContent className="max-w-lg">
          <DialogHeader>
            <DialogTitle className="text-xl">Report Machine Down | စက်ရပ်နေကြောင်းအစီရင်ခံ</DialogTitle>
            <DialogDescription>
              Fill in the basic information. A Plastic Engineer will be automatically notified.
            </DialogDescription>
          </DialogHeader>
          <div className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label>Machine ID (Auto-filled)</Label>
                <Select value={selectedMachine} onValueChange={setSelectedMachine}>
                  <SelectTrigger>
                    <SelectValue placeholder="Select machine" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="INJ-001">INJ-001</SelectItem>
                    <SelectItem value="INJ-002">INJ-002</SelectItem>
                    <SelectItem value="EXT-001">EXT-001</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              <div>
                <Label>Job ID (Auto-filled)</Label>
                <Input value={jobId} onChange={(e) => setJobId(e.target.value)} />
              </div>
            </div>
            
            <div>
              <Label>Symptom (Optional)</Label>
              <Textarea
                placeholder="Describe what you observed..."
                value={symptom}
                onChange={(e) => setSymptom(e.target.value)}
                rows={3}
              />
            </div>
            
            <div className="flex justify-end gap-2">
              <Button variant="outline" onClick={() => setIsOpen(false)}>
                Cancel | ပယ်ဖျက်
              </Button>
              <Button onClick={handleSubmit} className="bg-red-600 hover:bg-red-700">
                Report Machine Down | စက်ရပ်အစီရင်ခံ
              </Button>
            </div>
          </div>
        </DialogContent>
      </Dialog>
    );
  };

  // Desktop Engineering Dashboard
  const DesktopDashboard = () => (
    <div className="space-y-6">
      {/* KPI Cards */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Active Machines</CardTitle>
            <Activity className="h-4 w-4 text-green-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-green-600">2/5</div>
            <p className="text-xs text-muted-foreground">လှုပ်ရှားနေသောစက်များ</p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Open Tickets</CardTitle>
            <AlertTriangle className="h-4 w-4 text-red-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-red-600">3</div>
            <p className="text-xs text-muted-foreground">ဖွင့်ထားသောတိကစ်များ</p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">P1 Critical</CardTitle>
            <Timer className="h-4 w-4 text-red-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-red-600">1</div>
            <p className="text-xs text-muted-foreground">အရေးကြီးပြဿနာများ</p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">MTTR Today</CardTitle>
            <Clock className="h-4 w-4 text-blue-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-blue-600">2.5h</div>
            <p className="text-xs text-muted-foreground">ဒီနေ့ပျမ်းမျှပြုပြင်ချိန်</p>
          </CardContent>
        </Card>
      </div>

      {/* Machine Status Table */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Cog className="h-5 w-5" />
            Machine Status | စက်အခြေအနေ
          </CardTitle>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Machine ID</TableHead>
                <TableHead>Name</TableHead>
                <TableHead>Status</TableHead>
                <TableHead>Uptime</TableHead>
                <TableHead>Efficiency</TableHead>
                <TableHead>Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {mockMachines.map((machine) => (
                <TableRow key={machine.id}>
                  <TableCell className="font-medium">{machine.id}</TableCell>
                  <TableCell>{machine.name}</TableCell>
                  <TableCell><StatusBadge status={machine.status} /></TableCell>
                  <TableCell>{machine.uptime}</TableCell>
                  <TableCell>
                    <div className="flex items-center gap-2">
                      <Progress value={machine.efficiency} className="w-16" />
                      <span className="text-sm">{machine.efficiency}%</span>
                    </div>
                  </TableCell>
                  <TableCell>
                    <Button size="sm" variant="outline">
                      <Eye className="h-4 w-4 mr-1" />
                      Details
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </CardContent>
      </Card>

      {/* Active Tickets */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <AlertTriangle className="h-5 w-5" />
            Active Tickets | လက်ရှိတိကစ်များ
          </CardTitle>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Ticket ID</TableHead>
                <TableHead>Machine</TableHead>
                <TableHead>Priority</TableHead>
                <TableHead>Status</TableHead>
                <TableHead>Assigned To</TableHead>
                <TableHead>SLA</TableHead>
                <TableHead>Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {mockTickets.map((ticket) => (
                <TableRow key={ticket.id}>
                  <TableCell className="font-medium">{ticket.id}</TableCell>
                  <TableCell>{ticket.machineName}</TableCell>
                  <TableCell><PriorityBadge priority={ticket.priority} /></TableCell>
                  <TableCell><StatusBadge status={ticket.status} /></TableCell>
                  <TableCell>{ticket.assignedToName}</TableCell>
                  <TableCell><SLATimer remaining={ticket.slaRemaining} /></TableCell>
                  <TableCell>
                    <Button 
                      size="sm" 
                      variant="outline"
                      onClick={() => setSelectedTicket(ticket)}
                    >
                      <Eye className="h-4 w-4 mr-1" />
                      View
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </CardContent>
      </Card>
    </div>
  );

  // Engineering Form Component
  const EngineeringFormView = () => (
    <div className="max-w-6xl mx-auto space-y-6">
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <FormInput className="h-5 w-5" />
            Engineering Form - Process Setup | အင်ဂျင်နီယာဖောင် - လုပ်ငန်းစဉ်သတ်မှတ်ခြင်း
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-8">
          {/* Job Information */}
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div className="space-y-2">
              <Label htmlFor="job-id">Job ID</Label>
              <Input id="job-id" placeholder="JOB20250907-001-C1" />
            </div>
            <div className="space-y-2">
              <Label htmlFor="product">Product</Label>
              <Input id="product" placeholder="2011 - Plastic Bottle 500ml" />
            </div>
            <div className="space-y-2">
              <Label htmlFor="machine">Machine</Label>
              <Select>
                <SelectTrigger>
                  <SelectValue placeholder="Select machine" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="inj-001">INJ-001 - Injection Machine #1</SelectItem>
                  <SelectItem value="inj-002">INJ-002 - Injection Machine #2</SelectItem>
                  <SelectItem value="ext-001">EXT-001 - Extruder Machine #1</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>

          {/* Process Setup Sections */}
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
            
            {/* Mold Section */}
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Layers className="h-5 w-5" />
                  Mold Settings | မှိုပုံစံချိန်ညှိမှု
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                <div className="grid grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <Label htmlFor="clamp-pos-1">Clamp Position 1</Label>
                    <div className="flex gap-2">
                      <Input id="clamp-pos-1" type="number" placeholder="0" />
                      <span className="flex items-center text-sm text-muted-foreground">mm</span>
                    </div>
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="clamp-pos-2">Clamp Position 2</Label>
                    <div className="flex gap-2">
                      <Input id="clamp-pos-2" type="number" placeholder="50" />
                      <span className="flex items-center text-sm text-muted-foreground">mm</span>
                    </div>
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="core-fwd">Core Forward</Label>
                    <div className="flex gap-2">
                      <Input id="core-fwd" type="number" placeholder="25" />
                      <span className="flex items-center text-sm text-muted-foreground">mm</span>
                    </div>
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="core-back">Core Back</Label>
                    <div className="flex gap-2">
                      <Input id="core-back" type="number" placeholder="0" />
                      <span className="flex items-center text-sm text-muted-foreground">mm</span>
                    </div>
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="mold-temp">Mold Temperature</Label>
                    <div className="flex gap-2">
                      <Input id="mold-temp" type="number" placeholder="180" />
                      <span className="flex items-center text-sm text-muted-foreground">°C</span>
                    </div>
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="cycle-time">Cycle Time</Label>
                    <div className="flex gap-2">
                      <Input id="cycle-time" type="number" placeholder="45" />
                      <span className="flex items-center text-sm text-muted-foreground">sec</span>
                    </div>
                  </div>
                </div>
              </CardContent>
            </Card>

            {/* Injection Section */}
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Zap className="h-5 w-5" />
                  Injection Settings | ထိုးသွင်းမှုချိန်ညှိမှု
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                <div className="grid grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <Label htmlFor="injection-pressure">Injection Pressure</Label>
                    <div className="flex gap-2">
                      <Input id="injection-pressure" type="number" placeholder="120" />
                      <span className="flex items-center text-sm text-muted-foreground">MPa</span>
                    </div>
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="injection-speed">Injection Speed</Label>
                    <div className="flex gap-2">
                      <Input id="injection-speed" type="number" placeholder="85" />
                      <span className="flex items-center text-sm text-muted-foreground">%</span>
                    </div>
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="holding-pressure">Holding Pressure</Label>
                    <div className="flex gap-2">
                      <Input id="holding-pressure" type="number" placeholder="95" />
                      <span className="flex items-center text-sm text-muted-foreground">MPa</span>
                    </div>
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="holding-time">Holding Time</Label>
                    <div className="flex gap-2">
                      <Input id="holding-time" type="number" placeholder="15" />
                      <span className="flex items-center text-sm text-muted-foreground">sec</span>
                    </div>
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="injection-timer">Injection Timer</Label>
                    <div className="flex gap-2">
                      <Input id="injection-timer" type="number" placeholder="3.5" />
                      <span className="flex items-center text-sm text-muted-foreground">sec</span>
                    </div>
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="back-pressure">Back Pressure</Label>
                    <div className="flex gap-2">
                      <Input id="back-pressure" type="number" placeholder="5" />
                      <span className="flex items-center text-sm text-muted-foreground">MPa</span>
                    </div>
                  </div>
                </div>
              </CardContent>
            </Card>

            {/* Screw Section */}
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Cog className="h-5 w-5" />
                  Screw Settings | ဝက်အူသံုးချိန်ညှိမှု
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                <div className="grid grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <Label htmlFor="screw-speed">Screw Speed</Label>
                    <div className="flex gap-2">
                      <Input id="screw-speed" type="number" placeholder="150" />
                      <span className="flex items-center text-sm text-muted-foreground">RPM</span>
                    </div>
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="cushion">Cushion</Label>
                    <div className="flex gap-2">
                      <Input id="cushion" type="number" placeholder="5" />
                      <span className="flex items-center text-sm text-muted-foreground">mm</span>
                    </div>
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="back-position">Back Position</Label>
                    <div className="flex gap-2">
                      <Input id="back-position" type="number" placeholder="25" />
                      <span className="flex items-center text-sm text-muted-foreground">mm</span>
                    </div>
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="screw-timer">Screw Timer</Label>
                    <div className="flex gap-2">
                      <Input id="screw-timer" type="number" placeholder="8" />
                      <span className="flex items-center text-sm text-muted-foreground">sec</span>
                    </div>
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="decompression">Decompression</Label>
                    <div className="flex gap-2">
                      <Input id="decompression" type="number" placeholder="2" />
                      <span className="flex items-center text-sm text-muted-foreground">mm</span>
                    </div>
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="dosing-position">Dosing Position</Label>
                    <div className="flex gap-2">
                      <Input id="dosing-position" type="number" placeholder="25" />
                      <span className="flex items-center text-sm text-muted-foreground">mm</span>
                    </div>
                  </div>
                </div>
              </CardContent>
            </Card>

            {/* Temperature Section */}
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Thermometer className="h-5 w-5" />
                  Temperature Settings | အပူချိန်ချိန်ညှိမှု
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                <div className="grid grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <Label htmlFor="barrel-z1">Barrel Zone 1</Label>
                    <div className="flex gap-2">
                      <Input id="barrel-z1" type="number" placeholder="240" />
                      <span className="flex items-center text-sm text-muted-foreground">°C</span>
                    </div>
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="barrel-z2">Barrel Zone 2</Label>
                    <div className="flex gap-2">
                      <Input id="barrel-z2" type="number" placeholder="250" />
                      <span className="flex items-center text-sm text-muted-foreground">°C</span>
                    </div>
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="barrel-z3">Barrel Zone 3</Label>
                    <div className="flex gap-2">
                      <Input id="barrel-z3" type="number" placeholder="255" />
                      <span className="flex items-center text-sm text-muted-foreground">°C</span>
                    </div>
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="barrel-z4">Barrel Zone 4</Label>
                    <div className="flex gap-2">
                      <Input id="barrel-z4" type="number" placeholder="260" />
                      <span className="flex items-center text-sm text-muted-foreground">°C</span>
                    </div>
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="nozzle-temp">Nozzle Temperature</Label>
                    <div className="flex gap-2">
                      <Input id="nozzle-temp" type="number" placeholder="260" />
                      <span className="flex items-center text-sm text-muted-foreground">°C</span>
                    </div>
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="mold-temp-set">Mold Temperature</Label>
                    <div className="flex gap-2">
                      <Input id="mold-temp-set" type="number" placeholder="180" />
                      <span className="flex items-center text-sm text-muted-foreground">°C</span>
                    </div>
                  </div>
                </div>
              </CardContent>
            </Card>
          </div>

          {/* Workflow Status & Actions */}
          <Card>
            <CardHeader>
              <CardTitle>Workflow Status | လုပ်ငန်းစဉ်အခြေအနေ</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-6">
                <div className="flex items-center gap-3">
                  <div className="w-8 h-8 bg-blue-100 text-blue-800 rounded-full flex items-center justify-center">1</div>
                  <div>
                    <div className="font-medium">Fill</div>
                    <div className="text-sm text-muted-foreground">Plastic Technician</div>
                  </div>
                </div>
                <div className="flex items-center gap-3">
                  <div className="w-8 h-8 bg-slate-100 text-slate-800 rounded-full flex items-center justify-center">2</div>
                  <div>
                    <div className="font-medium">Submit</div>
                    <div className="text-sm text-muted-foreground">Submit for Verification</div>
                  </div>
                </div>
                <div className="flex items-center gap-3">
                  <div className="w-8 h-8 bg-slate-100 text-slate-800 rounded-full flex items-center justify-center">3</div>
                  <div>
                    <div className="font-medium">Verify</div>
                    <div className="text-sm text-muted-foreground">Technician Head</div>
                  </div>
                </div>
                <div className="flex items-center gap-3">
                  <div className="w-8 h-8 bg-slate-100 text-slate-800 rounded-full flex items-center justify-center">4</div>
                  <div>
                    <div className="font-medium">Approve</div>
                    <div className="text-sm text-muted-foreground">Final Approval</div>
                  </div>
                </div>
              </div>

              <div className="flex gap-4">
                <Button variant="outline">
                  <Save className="h-4 w-4 mr-2" />
                  Save Draft | မူကြမ်းသိမ်း
                </Button>
                <Button>
                  <Send className="h-4 w-4 mr-2" />
                  Submit for Verification | စိစစ်ရန်တင်သွင်း
                </Button>
                <Button variant="outline" disabled>
                  <CheckSquare className="h-4 w-4 mr-2" />
                  Verify | စိစစ်
                </Button>
                <Button variant="outline" disabled>
                  <CheckCircle className="h-4 w-4 mr-2" />
                  Approve | အတည်ပြု
                </Button>
              </div>
            </CardContent>
          </Card>
        </CardContent>
      </Card>
    </div>
  );

  // Breakdown Handling Component
  const BreakdownHandlingView = () => {
    const [selectedTicket, setSelectedTicket] = useState<any>(null);
    
    return (
      <div className="space-y-6">
        {/* Breakdown Overview Cards */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Active Breakdowns</CardTitle>
              <AlertTriangle className="h-4 w-4 text-red-600" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-red-600">3</div>
              <p className="text-xs text-muted-foreground">လှုပ်ရှားနေသောပျက်စီးမှုများ</p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">P1 Critical</CardTitle>
              <Timer className="h-4 w-4 text-red-600" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-red-600">1</div>
              <p className="text-xs text-muted-foreground">အရေးကြီးပြဿနာများ</p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Avg. MTTR</CardTitle>
              <Clock className="h-4 w-4 text-blue-600" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-blue-600">2.5h</div>
              <p className="text-xs text-muted-foreground">ပျမ်းမျှပြုပြင်ချိန်</p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Escalated</CardTitle>
              <ArrowUp className="h-4 w-4 text-orange-600" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-orange-600">1</div>
              <p className="text-xs text-muted-foreground">တက်လှမ်းသောတိကစ်များ</p>
            </CardContent>
          </Card>
        </div>

        {/* Breakdown Tickets Table */}
        <Card>
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle className="flex items-center gap-2">
              <AlertTriangle className="h-5 w-5" />
              Breakdown Tickets | ပျက်စီးမှုလက်မှတ်များ
            </CardTitle>
            <Button>
              <Plus className="h-4 w-4 mr-2" />
              Create New Ticket
            </Button>
          </CardHeader>
          <CardContent>
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Ticket ID</TableHead>
                  <TableHead>Machine</TableHead>
                  <TableHead>Priority</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Assigned To</TableHead>
                  <TableHead>SLA</TableHead>
                  <TableHead>Downtime</TableHead>
                  <TableHead>Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {mockTickets.map((ticket) => (
                  <TableRow 
                    key={ticket.id} 
                    className="cursor-pointer hover:bg-slate-50"
                    onClick={() => setSelectedTicket(ticket)}
                  >
                    <TableCell className="font-medium">{ticket.id}</TableCell>
                    <TableCell>{ticket.machineName}</TableCell>
                    <TableCell><PriorityBadge priority={ticket.priority} /></TableCell>
                    <TableCell><StatusBadge status={ticket.status} /></TableCell>
                    <TableCell>{ticket.assignedToName}</TableCell>
                    <TableCell><SLATimer remaining={ticket.slaRemaining} /></TableCell>
                    <TableCell className="flex items-center gap-1">
                      <Timer className="h-4 w-4 text-red-600" />
                      {ticket.downtime}
                    </TableCell>
                    <TableCell>
                      <div className="flex gap-2">
                        <Button size="sm" variant="outline">
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
          </CardContent>
        </Card>

        {/* Ticket Detail Panel */}
        {selectedTicket && (
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Info className="h-5 w-5" />
                Ticket Details: {selectedTicket.id} | တိကစ်အသေးစိတ်
              </CardTitle>
            </CardHeader>
            <CardContent>
              <Tabs defaultValue="overview" className="space-y-4">
                <TabsList className="grid w-full grid-cols-6">
                  <TabsTrigger value="overview">Overview</TabsTrigger>
                  <TabsTrigger value="readings">Readings</TabsTrigger>
                  <TabsTrigger value="actions">Actions</TabsTrigger>
                  <TabsTrigger value="parts">Parts</TabsTrigger>
                  <TabsTrigger value="photos">Photos</TabsTrigger>
                  <TabsTrigger value="history">History</TabsTrigger>
                </TabsList>

                <TabsContent value="overview" className="space-y-4">
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                    <div className="space-y-4">
                      <div>
                        <Label className="text-sm text-muted-foreground">Machine</Label>
                        <div className="font-medium">{selectedTicket.machineName}</div>
                      </div>
                      <div>
                        <Label className="text-sm text-muted-foreground">Symptom</Label>
                        <div className="font-medium">{selectedTicket.symptom}</div>
                      </div>
                      <div>
                        <Label className="text-sm text-muted-foreground">Description</Label>
                        <div>{selectedTicket.description}</div>
                      </div>
                    </div>
                    <div className="space-y-4">
                      <div>
                        <Label className="text-sm text-muted-foreground">Priority</Label>
                        <div><PriorityBadge priority={selectedTicket.priority} /></div>
                      </div>
                      <div>
                        <Label className="text-sm text-muted-foreground">Current Downtime</Label>
                        <div className="flex items-center gap-2">
                          <Timer className="h-4 w-4 text-red-600" />
                          <span className="font-medium text-red-600">{selectedTicket.downtime}</span>
                        </div>
                      </div>
                      <div>
                        <Label className="text-sm text-muted-foreground">SLA Status</Label>
                        <SLATimer remaining={selectedTicket.slaRemaining} />
                      </div>
                    </div>
                  </div>
                </TabsContent>

                <TabsContent value="actions" className="space-y-4">
                  <div>
                    <Label htmlFor="diagnosis">Diagnosis & Actions Taken</Label>
                    <Textarea id="diagnosis" placeholder="Enter diagnosis and actions..." rows={4} />
                  </div>
                  <div className="flex gap-4">
                    <Button className="bg-green-600 hover:bg-green-700">
                      <Wrench className="h-4 w-4 mr-2" />
                      Mark as Fixed
                    </Button>
                    <Button variant="outline" className="text-orange-600 border-orange-600">
                      <ArrowUp className="h-4 w-4 mr-2" />
                      Escalate Issue
                    </Button>
                  </div>
                </TabsContent>
              </Tabs>
            </CardContent>
          </Card>
        )}
      </div>
    );
  };

  // Preventive Maintenance Component
  const PreventiveMaintenanceView = () => {
    const mockPMTasks = [
      {
        id: 'PM-001',
        machine: 'INJ-001',
        machineName: 'Injection Machine #1',
        taskName: 'Monthly Oil Change',
        dueDate: '2025-09-15',
        status: 'due',
        assignedTo: 'Maintenance Team',
        estimatedTime: '2h'
      },
      {
        id: 'PM-002',
        machine: 'EXT-001', 
        machineName: 'Extruder Machine #1',
        taskName: 'Quarterly Belt Inspection',
        dueDate: '2025-09-10',
        status: 'overdue',
        assignedTo: 'Tech Team A',
        estimatedTime: '1.5h'
      }
    ];

    return (
      <div className="space-y-6">
        {/* PM Overview Cards */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Due Today</CardTitle>
              <Calendar className="h-4 w-4 text-orange-600" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-orange-600">1</div>
              <p className="text-xs text-muted-foreground">ဒီနေ့သက်တမ်းကျ</p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Overdue</CardTitle>
              <AlertTriangle className="h-4 w-4 text-red-600" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-red-600">1</div>
              <p className="text-xs text-muted-foreground">သက်တမ်းကျော်လွန်</p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">This Week</CardTitle>
              <CalendarDays className="h-4 w-4 text-blue-600" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-blue-600">3</div>
              <p className="text-xs text-muted-foreground">ဒီအပတ်အတွင်း</p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Compliance</CardTitle>
              <CheckCircle className="h-4 w-4 text-green-600" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-green-600">92%</div>
              <p className="text-xs text-muted-foreground">လိုက်နာမှုနှုန်း</p>
            </CardContent>
          </Card>
        </div>

        {/* PM Tasks Table */}
        <Card>
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle className="flex items-center gap-2">
              <Cog className="h-5 w-5" />
              Preventive Maintenance Tasks | ကြိုတင်ပြုပြင်ထိန်းသိမ်းမှုအလုပ်များ
            </CardTitle>
            <Button>
              <Plus className="h-4 w-4 mr-2" />
              Schedule PM
            </Button>
          </CardHeader>
          <CardContent>
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Task ID</TableHead>
                  <TableHead>Machine</TableHead>
                  <TableHead>Task Name</TableHead>
                  <TableHead>Due Date</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {mockPMTasks.map((task) => (
                  <TableRow key={task.id}>
                    <TableCell className="font-medium">{task.id}</TableCell>
                    <TableCell>{task.machineName}</TableCell>
                    <TableCell>{task.taskName}</TableCell>
                    <TableCell className={task.status === 'overdue' ? 'text-red-600 font-medium' : ''}>
                      {task.dueDate}
                    </TableCell>
                    <TableCell><StatusBadge status={task.status} /></TableCell>
                    <TableCell>
                      <Button size="sm" className="bg-green-600 hover:bg-green-700">
                        <Play className="h-4 w-4 mr-1" />
                        Start
                      </Button>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </CardContent>
        </Card>
      </div>
    );
  };

  // Spare Parts Component
  const SparePartsView = () => {
    const mockSpareParts = [
      { partCode: 'SP-001', description: 'Temperature Sensor', stock: 5, minLevel: 3, usedThisMonth: 2, category: 'Sensors' },
      { partCode: 'SP-002', description: 'Injection Nozzle', stock: 2, minLevel: 5, usedThisMonth: 3, category: 'Injection' },
      { partCode: 'SP-003', description: 'Motor Belt', stock: 8, minLevel: 4, usedThisMonth: 1, category: 'Mechanical' }
    ];

    return (
      <div className="space-y-6">
        {/* Spare Parts Overview */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Total Parts</CardTitle>
              <Package className="h-4 w-4 text-blue-600" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">28</div>
              <p className="text-xs text-muted-foreground">စုစုပေါင်းအရန်အစိတ်အပိုင်း</p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Low Stock</CardTitle>
              <AlertTriangle className="h-4 w-4 text-yellow-600" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-yellow-600">2</div>
              <p className="text-xs text-muted-foreground">စတော့ခ်နည်းနေသည်</p>
            </CardContent>
          </Card>
        </div>

        {/* Spare Parts Table */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Package className="h-5 w-5" />
              Spare Parts Inventory | အရန်အစိတ်အပိုင်းများစာရင်း
            </CardTitle>
          </CardHeader>
          <CardContent>
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Part Code</TableHead>
                  <TableHead>Description</TableHead>
                  <TableHead>Stock</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {mockSpareParts.map((part) => {
                  const isLowStock = part.stock <= part.minLevel;
                  return (
                    <TableRow key={part.partCode}>
                      <TableCell className="font-medium">{part.partCode}</TableCell>
                      <TableCell>{part.description}</TableCell>
                      <TableCell className={isLowStock ? 'text-yellow-600 font-medium' : ''}>
                        {part.stock}
                      </TableCell>
                      <TableCell>
                        {isLowStock ? (
                          <Badge className="bg-yellow-100 text-yellow-800">Low Stock</Badge>
                        ) : (
                          <Badge className="bg-green-100 text-green-800">In Stock</Badge>
                        )}
                      </TableCell>
                      <TableCell>
                        <Button size="sm" variant="outline">
                          <Edit className="h-4 w-4" />
                        </Button>
                      </TableCell>
                    </TableRow>
                  );
                })}
              </TableBody>
            </Table>
          </CardContent>
        </Card>
      </div>
    );
  };

  // Engineering Reports Component
  const EngineeringReportsView = () => (
    <div className="space-y-6">
      {/* Report Summary Cards */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm">MTTR</CardTitle>
            <Clock className="h-4 w-4 text-blue-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">2.5h</div>
            <div className="text-sm text-green-600">-15% from last month</div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm">MTBF</CardTitle>
            <Activity className="h-4 w-4 text-green-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">168h</div>
            <div className="text-sm text-green-600">+8% from last month</div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm">PM Compliance</CardTitle>
            <CheckCircle className="h-4 w-4 text-green-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">92%</div>
            <div className="text-sm text-green-600">+5% from last month</div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm">Machine Uptime</CardTitle>
            <TrendingUp className="h-4 w-4 text-green-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">95.2%</div>
            <div className="text-sm text-green-600">+2.3% from last month</div>
          </CardContent>
        </Card>
      </div>

      {/* Report Details */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <FileText className="h-5 w-5" />
            Engineering Reports | အင်ဂျင်နီယာအစီရင်ခံစာများ
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
            <Button variant="outline">
              <Download className="h-4 w-4 mr-2" />
              Engineering Form History
            </Button>
            <Button variant="outline">
              <Download className="h-4 w-4 mr-2" />
              Breakdown Log (MTTR/MTBF)
            </Button>
            <Button variant="outline">
              <Download className="h-4 w-4 mr-2" />
              PM Compliance Report
            </Button>
          </div>
          <p className="text-sm text-muted-foreground">
            Select date range and machine filters to generate detailed reports.
          </p>
        </CardContent>
      </Card>
    </div>
  );

  // Main render logic based on device and role
  const renderContent = () => {
    // Operator Kiosk View
    if (currentRole === 'operator' || deviceType === 'kiosk') {
      return <OperatorKioskView />;
    }

    // Mobile Views
    if (deviceType === 'mobile') {
      return (
        <EngineeringMobile 
          userRole={currentRole} 
          isOffline={isOffline}
          onNavigate={onPageChange}
        />
      );
    }

    // Desktop Views - Content based on current page
    const renderPageContent = () => {
      switch (currentPage) {
        case 'engineering-dashboard':
          return <DesktopDashboard />;
        case 'engineering-form':
          return <EngineeringFormView />;
        case 'breakdown-handling':
          return <BreakdownHandlingView />;
        case 'preventive-maintenance':
          return <PreventiveMaintenanceView />;
        case 'spare-parts-management':
          return <SparePartsView />;
        case 'engineering-reports':
          return <EngineeringReportsView />;
        default:
          return <DesktopDashboard />;
      }
    };

    const getPageTitle = () => {
      switch (currentPage) {
        case 'engineering-dashboard':
          return { 
            en: 'Engineering Dashboard', 
            mm: 'အင်ဂျင်နီယာဒက်ရှ်ဘုတ်',
            desc: 'Real-time machine monitoring, breakdown tracking, and maintenance oversight.',
            descMm: 'အချိန်နှင့်တပြေးညီစက်များစောင့်ကြည့်မှု၊ ပျက်စီးမှုခြေရာခံမှုနှင့် ပြုပြင်ထိန်းသိမ်းမှုကြီးကြပ်မှု။'
          };
        case 'engineering-form':
          return { 
            en: 'Process Setup Form', 
            mm: 'လုပ်ငန်းစဉ်သတ်မှတ်ခြင်းဖောင်',
            desc: 'Configure machine parameters and process settings for production jobs.',
            descMm: 'ထုတ်လုပ်မှုအလုပ်များအတွက် စက်ကိန်းရှင်များနှင့် လုပ်ငန်းစဉ်ချိန်ညှိမှုများကို ပြင်ဆင်ချိန်ညှိခြင်း။'
          };
        case 'breakdown-handling':
          return { 
            en: 'Breakdown Handling', 
            mm: 'ပျက်စီးမှုကိုင်တွယ်ခြင်း',
            desc: 'Manage machine breakdowns, assign tickets, and track repair progress.',
            descMm: 'စက်ပျက်စီးမှုများကို စီမံခန့်ခွဲရန်၊ တိကစ်များခွဲဝေရန်နှင့် ပြုပြင်မှုတိုးတက်မှုကို ခြေရာခံရန်။'
          };
        case 'preventive-maintenance':
          return { 
            en: 'Preventive Maintenance', 
            mm: 'ကြိုတင်ပြုပြင်ထိန်းသိမ်းမှု',
            desc: 'Schedule and track preventive maintenance activities for all machines.',
            descMm: 'စက်အားလုံးအတွက် ကြိုတင်ပြုပြင်ထိန်းသိမ်းမှုလုပ်ငန်းများကို အစီအစဉ်ချရန်နှင့် ခြေရာခံရန်။'
          };
        case 'spare-parts-management':
          return { 
            en: 'Spare Parts Management', 
            mm: 'အရန်အစိတ်အပိုင်းများစီမံခန့်ခွဲမှု',
            desc: 'Monitor spare parts inventory, track usage, and manage procurement.',
            descMm: 'အရန်အစိတ်အပိုင်းများ စာရင်းကို စောင့်ကြည့်ရန်၊ အသုံးပြုမှုကို ခြေရာခံရန်နှင့် ဝယ်ယူမှုကို စီမံခန့်ခွဲရန်။'
          };
        case 'engineering-reports':
          return { 
            en: 'Engineering Reports', 
            mm: 'အင်ဂျင်နီယာအစီရင်ခံစာများ',
            desc: 'Generate and export various engineering and maintenance reports.',
            descMm: 'အမျိုးမျိုးသော အင်ဂျင်နီယာနှင့် ပြုပြင်ထိန်းသိမ်းမှုအစီရင်ခံစာများကို ထုတ်လုပ်ရန်နှင့် ပို့ထုတ်ရန်။'
          };
        default:
          return { 
            en: 'Engineering Dashboard', 
            mm: 'အင်ဂျင်နီယာဒက်ရှ်ဘုတ်',
            desc: 'Real-time machine monitoring, breakdown tracking, and maintenance oversight.',
            descMm: 'အချိန်နှင့်တပြေးညီစက်များစောင့်ကြည့်မှု၊ ပျက်စီးမှုခြေရာခံမှုနှင့် ပြုပြင်ထိန်းသိမ်းမှုကြီးကြပ်မှု။'
          };
      }
    };

    const pageTitle = getPageTitle();

    return (
      <div className="p-6 bg-slate-50 min-h-screen">
        <div className="max-w-7xl mx-auto">
          <OfflineBanner />
          
          {/* Page Header */}
          <Card className="mb-6 border-0 shadow-sm bg-gradient-to-r from-blue-50 to-indigo-50">
            <CardContent className="p-6">
              <div className="flex items-center gap-4 mb-4">
                <div className="p-3 bg-blue-100 rounded-lg">
                  <Wrench className="h-8 w-8 text-blue-600" />
                </div>
                <div className="flex-1">
                  <h1 className="text-3xl font-semibold text-slate-900 mb-1">
                    {pageTitle.en}
                  </h1>
                  <h2 className="text-xl text-slate-700 mb-2">
                    {pageTitle.mm}
                  </h2>
                  <p className="text-slate-600">
                    {pageTitle.desc}
                  </p>
                  <p className="text-sm text-slate-500 mt-1">
                    {pageTitle.descMm}
                  </p>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Page Content */}
          {renderPageContent()}
        </div>
      </div>
    );
  };

  return renderContent();
}