import React, { useState, useEffect } from 'react';
import { 
  Home, CheckSquare, Scan, History, User, Bell, Camera, Mic, 
  ArrowLeft, Play, Pause, Wrench, ArrowUp, Timer, Eye, Plus,
  Thermometer, Layers, Bolt, Cog, Package, Phone, MessageSquare,
  Activity, AlertTriangle, Clock, CheckCircle, Info, Edit,
  RefreshCw, Settings, Download, Upload, MapPin, Calendar
} from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Badge } from './ui/badge';
import { Progress } from './ui/progress';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Label } from './ui/label';
import { Textarea } from './ui/textarea';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from './ui/dialog';
import { Separator } from './ui/separator';
import { ScrollArea } from './ui/scroll-area';
import { Avatar, AvatarFallback, AvatarImage } from './ui/avatar';
import { Tabs, TabsContent, TabsList, TabsTrigger } from './ui/tabs';

interface EngineeringMobileProps {
  userRole: 'plastic-technician' | 'mold-technician' | 'maintenance-engineer' | 'technician-head';
  isOffline?: boolean;
  onNavigate?: (page: string) => void;
}

export function EngineeringMobile({ userRole, isOffline = false, onNavigate }: EngineeringMobileProps) {
  const [activeView, setActiveView] = useState('home');
  const [selectedTicket, setSelectedTicket] = useState<any>(null);
  const [showCamera, setShowCamera] = useState(false);
  const [lastActiveTicket, setLastActiveTicket] = useState<string | null>('TKT-20250907-001');

  // Mock tickets filtered by role
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
      status: 'escalated',
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
      status: 'pending',
      assignedTo: 'mold-technician',
      assignedToName: 'Sarah Johnson',
      reportedBy: 'QC Officer 1',
      reportedTime: '2025-09-07 10:00',
      downtime: '4h 15m',
      slaRemaining: '45m',
      description: 'Mold not aligning properly, causing dimensional issues'
    }
  ];

  // Filter tickets by role
  const getMyTickets = () => {
    return mockTickets.filter(ticket => ticket.assignedTo === userRole);
  };

  // Status and priority configurations
  const StatusBadge = ({ status }: { status: string }) => {
    const configs = {
      'running': 'bg-green-100 text-green-800 border-green-200',
      'in-progress': 'bg-blue-100 text-blue-800 border-blue-200',
      'pending': 'bg-gray-100 text-gray-800 border-gray-200',
      'completed': 'bg-green-100 text-green-800 border-green-200',
      'escalated': 'bg-orange-100 text-orange-800 border-orange-200'
    };
    const config = configs[status as keyof typeof configs] || configs.pending;
    return <Badge className={`${config} border text-xs`}>{status}</Badge>;
  };

  const PriorityBadge = ({ priority }: { priority: string }) => {
    const configs = {
      'P1': 'bg-red-100 text-red-800 border-red-200',
      'P2': 'bg-yellow-100 text-yellow-800 border-yellow-200',
      'P3': 'bg-blue-100 text-blue-800 border-blue-200',
      'P4': 'bg-green-100 text-green-800 border-green-200'
    };
    const config = configs[priority as keyof typeof configs] || configs.P4;
    return <Badge className={`${config} border text-xs`}>{priority}</Badge>;
  };

  const SLATimer = ({ remaining, isOverdue = false }: { remaining: string; isOverdue?: boolean }) => (
    <div className={`flex items-center gap-1 ${isOverdue ? 'text-red-600' : 'text-orange-600'}`}>
      <Timer className="h-3 w-3" />
      <span className="text-xs font-medium">{remaining}</span>
    </div>
  );

  // Role-specific configurations
  const getRoleConfig = () => {
    const configs = {
      'plastic-technician': {
        name: 'Plastic Technician',
        nameMM: 'ပလတ်စတစ်နည်းပညာသည်',
        icon: Thermometer,
        color: 'blue',
        actions: ['Start', 'Fix & Close', 'Escalate', 'Add Reading']
      },
      'mold-technician': {
        name: 'Mold Change Technician',
        nameMM: 'မှိုပြောင်းလဲခြင်းနည်းပညာသည်',
        icon: Layers,
        color: 'green',
        actions: ['Setup Mold', 'Complete', 'Return']
      },
      'maintenance-engineer': {
        name: 'Maintenance Engineer',
        nameMM: 'ပြုပြင်ထိန်းသိမ်းရေးအင်ဂျင်နီယာ',
        icon: Bolt,
        color: 'orange',
        actions: ['Diagnose', 'Repair', 'Complete', 'Order Parts']
      },
      'technician-head': {
        name: 'Technician Head',
        nameMM: 'နည်းပညာသည်အကြီးအကဲ',
        icon: CheckCircle,
        color: 'purple',
        actions: ['Verify', 'Approve', 'Restart', 'Send Back']
      }
    };
    return configs[userRole];
  };

  // Offline Banner
  const OfflineBanner = () => (
    isOffline && (
      <div className="bg-yellow-50 border-l-4 border-yellow-400 p-2 mb-3">
        <div className="flex items-center">
          <AlertTriangle className="h-4 w-4 text-yellow-400 mr-2" />
          <p className="text-xs text-yellow-800">
            Offline Mode | မရေရာမမ်မ် - Changes will sync when online
          </p>
        </div>
      </div>
    )
  );

  // Header component
  const MobileHeader = ({ title, subtitle, onBack }: { title: string; subtitle?: string; onBack?: () => void }) => (
    <div className="bg-white border-b p-4 sticky top-0 z-10">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          {onBack && (
            <Button variant="ghost" size="sm" onClick={onBack}>
              <ArrowLeft className="h-4 w-4" />
            </Button>
          )}
          <div>
            <h1 className="font-semibold text-base">{title}</h1>
            {subtitle && <p className="text-xs text-muted-foreground">{subtitle}</p>}
          </div>
        </div>
        <div className="flex items-center gap-2">
          {isOffline && <div className="w-2 h-2 bg-yellow-400 rounded-full" />}
          <Button variant="ghost" size="sm">
            <Bell className="h-4 w-4" />
          </Button>
        </div>
      </div>
    </div>
  );

  // Bottom Navigation
  const BottomNav = () => (
    <div className="fixed bottom-0 left-0 right-0 bg-white border-t border-gray-200 px-2 py-1 safe-area-bottom">
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
            onClick={() => setActiveView(item.id)}
            className={`flex flex-col items-center py-2 px-3 min-w-16 ${
              activeView === item.id ? 'text-blue-600' : 'text-gray-400'
            }`}
          >
            <item.icon className="h-5 w-5" />
            <span className="text-xs mt-1">{item.label}</span>
          </button>
        ))}
      </div>
    </div>
  );

  // Home view for all roles
  const HomeView = () => {
    const roleConfig = getRoleConfig();
    const myTickets = getMyTickets();
    const p1Tickets = myTickets.filter(t => t.priority === 'P1');
    
    return (
      <ScrollArea className="h-full">
        <div className="p-4 space-y-4 pb-20">
          <OfflineBanner />
          
          {/* Role Status Card */}
          <Card className={`border-l-4 border-l-${roleConfig.color}-500`}>
            <CardContent className="p-4">
              <div className="flex items-center gap-3 mb-3">
                <roleConfig.icon className={`h-6 w-6 text-${roleConfig.color}-600`} />
                <div>
                  <div className="font-semibold">{roleConfig.name}</div>
                  <div className="text-sm text-muted-foreground">{roleConfig.nameMM}</div>
                </div>
              </div>
              <div className="grid grid-cols-3 gap-4 text-center">
                <div>
                  <div className="text-lg font-bold text-red-600">{p1Tickets.length}</div>
                  <div className="text-xs text-muted-foreground">P1 Critical</div>
                </div>
                <div>
                  <div className="text-lg font-bold text-blue-600">{myTickets.length}</div>
                  <div className="text-xs text-muted-foreground">Total Active</div>
                </div>
                <div>
                  <div className="text-lg font-bold text-green-600">2.5h</div>
                  <div className="text-xs text-muted-foreground">Avg. Time</div>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Resume Last Action */}
          {lastActiveTicket && (
            <Button 
              size="lg" 
              className="w-full bg-green-600 hover:bg-green-700 h-12"
              onClick={() => {
                const ticket = mockTickets.find(t => t.id === lastActiveTicket);
                if (ticket) {
                  setSelectedTicket(ticket);
                  setActiveView('ticket-detail');
                }
              }}
            >
              <Play className="h-5 w-5 mr-2" />
              Resume Last Ticket | နောက်ဆုံးတိကစ်ဆက်လုပ်
            </Button>
          )}

          {/* P1 Critical Tickets */}
          {p1Tickets.length > 0 && (
            <div className="space-y-3">
              <h3 className="font-medium text-red-600">🚨 P1 Critical Tickets</h3>
              {p1Tickets.map((ticket) => (
                <Card key={ticket.id} className="border-l-4 border-l-red-500">
                  <CardContent className="p-3">
                    <div className="flex justify-between items-start mb-2">
                      <div>
                        <div className="font-medium text-sm">{ticket.id}</div>
                        <div className="text-xs text-muted-foreground">{ticket.machineName}</div>
                      </div>
                      <PriorityBadge priority={ticket.priority} />
                    </div>
                    <div className="text-xs mb-2">{ticket.symptom}</div>
                    <div className="flex justify-between items-center">
                      <SLATimer remaining={ticket.slaRemaining} />
                      <Button 
                        size="sm" 
                        className="h-7 text-xs"
                        onClick={() => {
                          setSelectedTicket(ticket);
                          setActiveView('ticket-detail');
                        }}
                      >
                        Start
                      </Button>
                    </div>
                  </CardContent>
                </Card>
              ))}
            </div>
          )}

          {/* Recent Tickets */}
          <div className="space-y-3">
            <h3 className="font-medium">My Recent Tickets | ကျွန်ုပ်၏လတ်တလောတိကစ်များ</h3>
            {myTickets.slice(0, 3).map((ticket) => (
              <Card key={ticket.id} className="border-l-4 border-l-blue-500">
                <CardContent className="p-3">
                  <div className="flex justify-between items-start mb-2">
                    <div>
                      <div className="font-medium text-sm">{ticket.id}</div>
                      <div className="text-xs text-muted-foreground">{ticket.machineName}</div>
                    </div>
                    <div className="flex gap-1">
                      <PriorityBadge priority={ticket.priority} />
                      <StatusBadge status={ticket.status} />
                    </div>
                  </div>
                  <div className="text-xs mb-2">{ticket.symptom}</div>
                  <div className="flex justify-between items-center">
                    <SLATimer remaining={ticket.slaRemaining} />
                    <Button 
                      size="sm" 
                      variant="outline" 
                      className="h-7 text-xs"
                      onClick={() => {
                        setSelectedTicket(ticket);
                        setActiveView('ticket-detail');
                      }}
                    >
                      <Eye className="h-3 w-3 mr-1" />
                      View
                    </Button>
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>
        </div>
      </ScrollArea>
    );
  };

  // Tasks view with role-specific actions
  const TasksView = () => {
    const myTickets = getMyTickets();
    
    return (
      <ScrollArea className="h-full">
        <div className="p-4 space-y-4 pb-20">
          <OfflineBanner />
          
          <div className="flex justify-between items-center">
            <h2 className="font-semibold">My Tasks | ကျွန်ုပ်၏အလုပ်များ</h2>
            <Button size="sm" variant="outline">
              <RefreshCw className="h-4 w-4 mr-1" />
              Refresh
            </Button>
          </div>

          {myTickets.map((ticket) => (
            <Card key={ticket.id}>
              <CardContent className="p-4">
                <div className="flex justify-between items-start mb-3">
                  <div>
                    <div className="font-medium">{ticket.id}</div>
                    <div className="text-sm text-muted-foreground">{ticket.machineName}</div>
                  </div>
                  <div className="flex gap-2">
                    <PriorityBadge priority={ticket.priority} />
                    <StatusBadge status={ticket.status} />
                  </div>
                </div>
                
                <div className="space-y-2 mb-3">
                  <div className="text-sm"><strong>Symptom:</strong> {ticket.symptom}</div>
                  <div className="flex justify-between">
                    <SLATimer remaining={ticket.slaRemaining} />
                    <span className="text-sm text-red-600">Downtime: {ticket.downtime}</span>
                  </div>
                </div>
                
                <div className="flex gap-2">
                  <Button 
                    size="sm" 
                    className="flex-1"
                    onClick={() => {
                      setSelectedTicket(ticket);
                      setActiveView('ticket-detail');
                    }}
                  >
                    <Wrench className="h-4 w-4 mr-1" />
                    Work On
                  </Button>
                  <Button size="sm" variant="outline" className="flex-1">
                    <ArrowUp className="h-4 w-4 mr-1" />
                    Escalate
                  </Button>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      </ScrollArea>
    );
  };

  // Ticket Detail View with role-specific tabs
  const TicketDetailView = () => {
    if (!selectedTicket) return null;

    return (
      <div className="h-full flex flex-col">
        <MobileHeader 
          title={selectedTicket.id}
          subtitle={selectedTicket.machineName}
          onBack={() => setActiveView('home')}
        />
        
        <ScrollArea className="flex-1">
          <div className="p-4 space-y-4 pb-20">
            {/* Ticket Summary */}
            <Card>
              <CardContent className="p-4">
                <div className="flex justify-between items-start mb-3">
                  <div>
                    <div className="font-semibold">{selectedTicket.machineName}</div>
                    <div className="text-sm text-muted-foreground">{selectedTicket.symptom}</div>
                  </div>
                  <div className="flex gap-2">
                    <PriorityBadge priority={selectedTicket.priority} />
                    <StatusBadge status={selectedTicket.status} />
                  </div>
                </div>
                
                <div className="grid grid-cols-2 gap-4 text-sm">
                  <div>
                    <div className="text-muted-foreground">Downtime</div>
                    <div className="font-medium text-red-600">{selectedTicket.downtime}</div>
                  </div>
                  <div>
                    <div className="text-muted-foreground">SLA Remaining</div>
                    <SLATimer remaining={selectedTicket.slaRemaining} />
                  </div>
                </div>
              </CardContent>
            </Card>

            {/* Role-specific content */}
            {userRole === 'plastic-technician' && <PlasticTechnicianContent ticket={selectedTicket} />}
            {userRole === 'mold-technician' && <MoldTechnicianContent ticket={selectedTicket} />}
            {userRole === 'maintenance-engineer' && <MaintenanceEngineerContent ticket={selectedTicket} />}
            {userRole === 'technician-head' && <TechnicianHeadContent ticket={selectedTicket} />}
          </div>
        </ScrollArea>
      </div>
    );
  };

  // Role-specific content components
  const PlasticTechnicianContent = ({ ticket }: { ticket: any }) => (
    <div className="space-y-4">
      {/* Readings Tab */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2 text-base">
            <Thermometer className="h-4 w-4" />
            Add Readings | တိုင်းတာမှုများထည့်သွင်း
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid grid-cols-2 gap-3">
            <div>
              <Label className="text-sm">Barrel Z1 (°C)</Label>
              <Input type="number" defaultValue={ticket.readings?.barrelZ1} className="text-sm" />
            </div>
            <div>
              <Label className="text-sm">Barrel Z2 (°C)</Label>
              <Input type="number" defaultValue={ticket.readings?.barrelZ2} className="text-sm" />
            </div>
            <div>
              <Label className="text-sm">Nozzle (°C)</Label>
              <Input type="number" defaultValue={ticket.readings?.nozzle} className="text-sm" />
            </div>
            <div>
              <Label className="text-sm">Mold (°C)</Label>
              <Input type="number" defaultValue={ticket.readings?.mold} className="text-sm" />
            </div>
          </div>
          <Button size="sm" className="w-full">
            <Plus className="h-4 w-4 mr-1" />
            Save Readings
          </Button>
        </CardContent>
      </Card>

      {/* Actions */}
      <Card>
        <CardHeader>
          <CardTitle className="text-base">Actions | လုပ်ဆောင်ချက်များ</CardTitle>
        </CardHeader>
        <CardContent className="space-y-3">
          <Textarea placeholder="Diagnosis and actions taken..." rows={3} className="text-sm" />
          <div className="grid grid-cols-2 gap-2">
            <Button className="bg-green-600 hover:bg-green-700">
              <CheckCircle className="h-4 w-4 mr-1" />
              Fix & Close
            </Button>
            <Button variant="outline" className="text-orange-600 border-orange-600">
              <ArrowUp className="h-4 w-4 mr-1" />
              Escalate
            </Button>
          </div>
        </CardContent>
      </Card>

      {/* Camera */}
      <Card>
        <CardContent className="p-4">
          <Button 
            variant="outline" 
            className="w-full"
            onClick={() => setShowCamera(true)}
          >
            <Camera className="h-4 w-4 mr-2" />
            Take Photo | ဓာတ်ပုံရိုက်
          </Button>
        </CardContent>
      </Card>
    </div>
  );

  const MoldTechnicianContent = ({ ticket }: { ticket: any }) => (
    <div className="space-y-4">
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2 text-base">
            <Layers className="h-4 w-4" />
            Mold Setup | မှိုပုံစံထည့်သွင်းမှု
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid grid-cols-2 gap-3">
            <div>
              <Label className="text-sm">Clamp Position (mm)</Label>
              <Input type="number" placeholder="0" className="text-sm" />
            </div>
            <div>
              <Label className="text-sm">Core Position (mm)</Label>
              <Input type="number" placeholder="0" className="text-sm" />
            </div>
            <div>
              <Label className="text-sm">Mold Temp (°C)</Label>
              <Input type="number" placeholder="180" className="text-sm" />
            </div>
            <div>
              <Label className="text-sm">Cycle Time (sec)</Label>
              <Input type="number" placeholder="45" className="text-sm" />
            </div>
          </div>
          <div className="grid grid-cols-2 gap-2">
            <Button className="bg-green-600 hover:bg-green-700">
              <CheckCircle className="h-4 w-4 mr-1" />
              Complete Setup
            </Button>
            <Button variant="outline">
              <ArrowUp className="h-4 w-4 mr-1" />
              Return to Head
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  );

  const MaintenanceEngineerContent = ({ ticket }: { ticket: any }) => (
    <div className="space-y-4">
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2 text-base">
            <Bolt className="h-4 w-4" />
            Maintenance Work | ပြုပြင်ထိန်းသိမ်းမှု
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div>
            <Label className="text-sm">Diagnosis</Label>
            <Textarea placeholder="Problem diagnosis..." rows={2} className="text-sm" />
          </div>
          <div>
            <Label className="text-sm">Action Taken</Label>
            <Textarea placeholder="Repair actions..." rows={2} className="text-sm" />
          </div>
          <div>
            <Label className="text-sm">Spare Parts Used</Label>
            <Select>
              <SelectTrigger className="text-sm">
                <SelectValue placeholder="Select parts" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="sp001">SP-001 Temperature Sensor</SelectItem>
                <SelectItem value="sp002">SP-002 Motor Belt</SelectItem>
              </SelectContent>
            </Select>
          </div>
          <Button className="w-full bg-green-600 hover:bg-green-700">
            <CheckCircle className="h-4 w-4 mr-1" />
            Complete Repair
          </Button>
        </CardContent>
      </Card>
    </div>
  );

  const TechnicianHeadContent = ({ ticket }: { ticket: any }) => (
    <div className="space-y-4">
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2 text-base">
            <CheckCircle className="h-4 w-4" />
            Verify & Approve | စိစစ်ပြီးအတည်ပြု
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="bg-blue-50 p-3 rounded-lg">
            <div className="text-sm font-medium text-blue-800 mb-2">Set vs Actual Comparison</div>
            <div className="grid grid-cols-3 gap-2 text-xs">
              <div className="text-center">
                <div className="text-muted-foreground">Parameter</div>
                <div>Temp Z1</div>
              </div>
              <div className="text-center">
                <div className="text-muted-foreground">Set</div>
                <div>240°C</div>
              </div>
              <div className="text-center">
                <div className="text-muted-foreground">Actual</div>
                <div className="text-green-600">242°C</div>
              </div>
            </div>
          </div>
          <div>
            <Label className="text-sm">Verification Notes</Label>
            <Textarea placeholder="Add verification comments..." rows={2} className="text-sm" />
          </div>
          <div className="grid grid-cols-2 gap-2">
            <Button className="bg-green-600 hover:bg-green-700">
              <CheckCircle className="h-4 w-4 mr-1" />
              Approve Restart
            </Button>
            <Button variant="outline" className="text-red-600 border-red-600">
              <ArrowLeft className="h-4 w-4 mr-1" />
              Send Back
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  );

  // Scan view with QR code scanner
  const ScanView = () => (
    <div className="h-full flex flex-col items-center justify-center p-4 pb-20">
      <div className="text-center space-y-4">
        <div className="w-48 h-48 border-2 border-dashed border-gray-300 rounded-lg flex items-center justify-center">
          <div className="text-center">
            <Scan className="h-12 w-12 text-gray-400 mx-auto mb-2" />
            <p className="text-sm text-gray-500">Position QR code here</p>
            <p className="text-xs text-gray-400">QR ကုဒ်ကိုဒီနေရာမှာထားပါ</p>
          </div>
        </div>
        <div className="space-y-2">
          <Button className="w-full">
            <Camera className="h-4 w-4 mr-2" />
            Scan Machine QR
          </Button>
          <Button variant="outline" className="w-full">
            <Package className="h-4 w-4 mr-2" />
            Scan Part QR
          </Button>
        </div>
      </div>
    </div>
  );

  // History view
  const HistoryView = () => (
    <ScrollArea className="h-full">
      <div className="p-4 space-y-4 pb-20">
        <h2 className="font-semibold">Completed Tickets | ပြီးစီးသောတိကစ်များ</h2>
        
        {mockTickets.slice(0, 5).map((ticket) => (
          <Card key={ticket.id}>
            <CardContent className="p-3">
              <div className="flex justify-between items-start">
                <div>
                  <div className="font-medium text-sm">{ticket.id}</div>
                  <div className="text-xs text-muted-foreground">{ticket.machineName}</div>
                  <div className="text-xs">{ticket.symptom}</div>
                </div>
                <div className="text-right">
                  <StatusBadge status="completed" />
                  <div className="text-xs text-muted-foreground mt-1">
                    {ticket.reportedTime}
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>
    </ScrollArea>
  );

  // Profile view
  const ProfileView = () => {
    const roleConfig = getRoleConfig();
    
    return (
      <ScrollArea className="h-full">
        <div className="p-4 space-y-4 pb-20">
          <Card>
            <CardContent className="p-4 text-center">
              <Avatar className="w-16 h-16 mx-auto mb-3">
                <AvatarFallback>{roleConfig.name.charAt(0)}</AvatarFallback>
              </Avatar>
              <div className="font-semibold">{roleConfig.name}</div>
              <div className="text-sm text-muted-foreground">{roleConfig.nameMM}</div>
              <div className="text-xs text-blue-600 mt-1">ENG-001</div>
            </CardContent>
          </Card>
          
          <Card>
            <CardHeader>
              <CardTitle className="text-base">Today's Stats</CardTitle>
            </CardHeader>
            <CardContent className="grid grid-cols-2 gap-4">
              <div className="text-center">
                <div className="text-2xl font-bold text-green-600">3</div>
                <div className="text-xs text-muted-foreground">Completed</div>
              </div>
              <div className="text-center">
                <div className="text-2xl font-bold text-blue-600">2.5h</div>
                <div className="text-xs text-muted-foreground">Avg Time</div>
              </div>
            </CardContent>
          </Card>

          <div className="space-y-2">
            <Button variant="outline" className="w-full">
              <Settings className="h-4 w-4 mr-2" />
              Settings | ဆက်တင်များ
            </Button>
            <Button variant="outline" className="w-full">
              <Download className="h-4 w-4 mr-2" />
              Sync Data | ဒေတာစင့်ခ်လုပ်
            </Button>
          </div>
        </div>
      </ScrollArea>
    );
  };

  // Main render
  const renderCurrentView = () => {
    switch (activeView) {
      case 'home':
        return <HomeView />;
      case 'tasks':
        return <TasksView />;
      case 'ticket-detail':
        return <TicketDetailView />;
      case 'scan':
        return <ScanView />;
      case 'history':
        return <HistoryView />;
      case 'profile':
        return <ProfileView />;
      default:
        return <HomeView />;
    }
  };

  return (
    <div className="h-screen bg-slate-50 flex flex-col">
      {activeView !== 'ticket-detail' && (
        <MobileHeader 
          title="Engineering" 
          subtitle={getRoleConfig().name}
        />
      )}
      
      <div className="flex-1 overflow-hidden">
        {renderCurrentView()}
      </div>
      
      <BottomNav />
    </div>
  );
}