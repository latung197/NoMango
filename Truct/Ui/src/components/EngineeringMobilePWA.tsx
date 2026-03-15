import React, { useState, useEffect } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Badge } from './ui/badge';
import { Progress } from './ui/progress';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Textarea } from './ui/textarea';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogTrigger } from './ui/dialog';
import { Alert, AlertDescription } from './ui/alert';
import { Checkbox } from './ui/checkbox';
import { Label } from './ui/label';
import { 
  Home, 
  ClipboardList, 
  Scan, 
  History, 
  User, 
  Wrench, 
  Layers, 
  Bolt, 
  Thermometer,
  Clock,
  AlertTriangle,
  CheckCircle,
  Camera,
  ArrowUp,
  PlayCircle,
  RotateCcw,
  Send,
  Bell,
  Wifi,
  WifiOff,
  Plus,
  Minus,
  Settings,
  LogOut,
  Filter,
  Search,
  Timer,
  Users,
  ChevronRight,
  ChevronDown,
  X,
  Check
} from 'lucide-react';

// Types (same as main module)
type EngineerRole = 'plastic-technician' | 'mold-technician' | 'maintenance-engineer' | 'technician-head';
type TicketStatus = 'new' | 'in-progress' | 'escalated' | 'completed' | 'verified' | 'closed';
type TicketPriority = 'P1' | 'P2' | 'P3';
type MobileView = 'home' | 'tasks' | 'scan' | 'history' | 'profile';

interface Ticket {
  id: string;
  machineId: string;
  machineName: string;
  title: string;
  description: string;
  status: TicketStatus;
  priority: TicketPriority;
  assignedTo: EngineerRole;
  createdAt: string;
  slaDeadline: string;
  downtimeMinutes: number;
  category: 'mechanical' | 'electrical' | 'mold' | 'process';
}

interface PMTask {
  id: string;
  machineId: string;
  machineName: string;
  taskName: string;
  frequency: string;
  nextDue: string;
  isOverdue: boolean;
  checklist: PMChecklistItem[];
}

interface PMChecklistItem {
  id: string;
  description: string;
  completed: boolean;
  notes?: string;
}

interface Notification {
  id: string;
  type: 'machine-down' | 'escalated' | 'approval-needed';
  title: string;
  message: string;
  timestamp: string;
  read: boolean;
}

export function EngineeringMobilePWA() {
  const [currentRole, setCurrentRole] = useState<EngineerRole>('plastic-technician');
  const [currentView, setCurrentView] = useState<MobileView>('home');
  const [isOnline, setIsOnline] = useState(true);
  const [selectedTicket, setSelectedTicket] = useState<Ticket | null>(null);
  const [showQRScanner, setShowQRScanner] = useState(false);
  const [notifications, setNotifications] = useState<Notification[]>([
    {
      id: 'N001',
      type: 'machine-down',
      title: 'Machine Down Alert',
      message: 'INJ-002 reported down - Temperature issue',
      timestamp: '2024-01-15 09:15',
      read: false
    },
    {
      id: 'N002', 
      type: 'escalated',
      title: 'Ticket Escalated',
      message: 'T001 escalated to Maintenance Engineer',
      timestamp: '2024-01-15 08:45',
      read: false
    }
  ]);

  // Mock data
  const [tickets] = useState<Ticket[]>([
    {
      id: 'T001',
      machineId: 'INJ-001',
      machineName: 'Injection Machine 1',
      title: 'Temperature Fluctuation',
      description: 'Machine temperature varying beyond acceptable limits',
      status: 'new',
      priority: 'P1',
      assignedTo: 'plastic-technician',
      createdAt: '2024-01-15 08:30',
      slaDeadline: '2024-01-15 10:30',
      downtimeMinutes: 45,
      category: 'process'
    },
    {
      id: 'T002',
      machineId: 'INJ-002',
      machineName: 'Injection Machine 2', 
      title: 'Mold Alignment Issue',
      description: 'Mold not closing properly, causing defects',
      status: 'escalated',
      priority: 'P2',
      assignedTo: 'mold-technician',
      createdAt: '2024-01-15 07:15',
      slaDeadline: '2024-01-15 11:15',
      downtimeMinutes: 120,
      category: 'mold'
    }
  ]);

  const [pmTasks] = useState<PMTask[]>([
    {
      id: 'PM001',
      machineId: 'INJ-001',
      machineName: 'Injection Machine 1',
      taskName: 'Monthly Lubrication Check',
      frequency: 'Monthly',
      nextDue: '2024-01-15',
      isOverdue: true,
      checklist: [
        { id: 'C1', description: 'Check hydraulic oil level', completed: false },
        { id: 'C2', description: 'Lubricate guide pins', completed: false },
        { id: 'C3', description: 'Check belt tension', completed: false }
      ]
    }
  ]);

  // Utility functions
  const getRoleTickets = () => {
    switch (currentRole) {
      case 'plastic-technician':
        return tickets.filter(t => t.assignedTo === 'plastic-technician' && t.status !== 'escalated');
      case 'mold-technician':
        return tickets.filter(t => t.assignedTo === 'mold-technician' || t.category === 'mold');
      case 'maintenance-engineer':
        return tickets.filter(t => t.assignedTo === 'maintenance-engineer' || ['mechanical', 'electrical'].includes(t.category));
      case 'technician-head':
        return tickets.filter(t => t.status === 'completed');
      default:
        return [];
    }
  };

  const getRoleIcon = (role: EngineerRole) => {
    switch (role) {
      case 'plastic-technician': return <Thermometer className="h-6 w-6" />;
      case 'mold-technician': return <Layers className="h-6 w-6" />;
      case 'maintenance-engineer': return <Wrench className="h-6 w-6" />;
      case 'technician-head': return <Users className="h-6 w-6" />;
    }
  };

  const getRoleTitle = (role: EngineerRole) => {
    switch (role) {
      case 'plastic-technician': return 'Plastic Technician';
      case 'mold-technician': return 'Mold Change Tech';
      case 'maintenance-engineer': return 'Maintenance Eng';
      case 'technician-head': return 'Tech Head';
    }
  };

  const getPriorityColor = (priority: TicketPriority) => {
    switch (priority) {
      case 'P1': return 'bg-red-500';
      case 'P2': return 'bg-orange-500'; 
      case 'P3': return 'bg-blue-500';
    }
  };

  const getStatusColor = (status: TicketStatus) => {
    switch (status) {
      case 'new': return 'bg-blue-500';
      case 'in-progress': return 'bg-yellow-500';
      case 'escalated': return 'bg-orange-500';
      case 'completed': return 'bg-purple-500';
      case 'verified': return 'bg-green-500';
      case 'closed': return 'bg-gray-500';
    }
  };

  const timeUntilSLA = (deadline: string) => {
    const now = new Date();
    const sla = new Date(deadline);
    const diff = sla.getTime() - now.getTime();
    const minutes = Math.floor(diff / 60000);
    
    if (minutes <= 0) return { text: 'OVERDUE', color: 'text-red-600', urgent: true };
    if (minutes <= 30) return { text: `${minutes}m`, color: 'text-orange-600', urgent: true };
    return { text: `${minutes}m`, color: 'text-green-600', urgent: false };
  };

  // Home Screen - Role specific
  const renderHomeScreen = () => {
    const roleTickets = getRoleTickets();
    const urgentTickets = roleTickets.filter(t => t.priority === 'P1');
    const overdueTickets = roleTickets.filter(t => {
      const sla = timeUntilSLA(t.slaDeadline);
      return sla.urgent;
    });

    return (
      <div className="space-y-4 pb-20">
        {/* Role Header */}
        <Card className="bg-gradient-to-r from-blue-600 to-blue-700 text-white">
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <div className="p-3 bg-white/20 rounded-full">
                {getRoleIcon(currentRole)}
              </div>
              <div>
                <h2 className="font-semibold">{getRoleTitle(currentRole)}</h2>
                <p className="text-sm opacity-90">
                  {currentRole === 'plastic-technician' && 'ပလတ်စတစ်အင်ဂျင်နီယာ'}
                  {currentRole === 'mold-technician' && 'မှိုပြောင်းအင်ဂျင်နီယာ'}  
                  {currentRole === 'maintenance-engineer' && 'ပြင်ဆင်ရေးအင်ဂျင်နီယာ'}
                  {currentRole === 'technician-head' && 'အင်ဂျင်နီယာလုပ်ငန်းမှူး'}
                </p>
              </div>
              <div className="ml-auto flex items-center gap-2">
                {isOnline ? <Wifi className="h-5 w-5" /> : <WifiOff className="h-5 w-5" />}
                <Bell className="h-5 w-5" />
                {notifications.filter(n => !n.read).length > 0 && (
                  <div className="w-2 h-2 bg-red-400 rounded-full"></div>
                )}
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Quick Stats */}
        <div className="grid grid-cols-3 gap-3">
          <Card>
            <CardContent className="p-4 text-center">
              <div className="text-2xl font-bold text-red-600">{urgentTickets.length}</div>
              <div className="text-sm text-slate-600">P1 Tickets</div>
              <div className="text-xs text-slate-500">အရေးပေါ်</div>
            </CardContent>
          </Card>
          
          <Card>
            <CardContent className="p-4 text-center">
              <div className="text-2xl font-bold text-orange-600">{overdueTickets.length}</div>
              <div className="text-sm text-slate-600">Overdue</div>
              <div className="text-xs text-slate-500">နောက်ကျ</div>
            </CardContent>
          </Card>
          
          <Card>
            <CardContent className="p-4 text-center">
              <div className="text-2xl font-bold text-blue-600">{roleTickets.length}</div>
              <div className="text-sm text-slate-600">Total</div>
              <div className="text-xs text-slate-500">စုစုပေါင်း</div>
            </CardContent>
          </Card>
        </div>

        {/* My Priority Tickets */}
        <Card>
          <CardHeader className="pb-3">
            <CardTitle className="text-lg">My Priority Tickets</CardTitle>
            <p className="text-sm text-slate-600">ကျွန်ုပ်၏ဦးစားပေးတာဝန်များ</p>
          </CardHeader>
          <CardContent className="space-y-3">
            {roleTickets.slice(0, 3).map((ticket) => {
              const sla = timeUntilSLA(ticket.slaDeadline);
              return (
                <div 
                  key={ticket.id}
                  className="bg-slate-50 rounded-lg p-4 active:bg-slate-100 cursor-pointer"
                  onClick={() => setSelectedTicket(ticket)}
                >
                  <div className="flex items-start gap-3">
                    <div className={`w-1 h-12 ${getPriorityColor(ticket.priority)} rounded-full`}></div>
                    <div className="flex-1">
                      <div className="flex items-center gap-2 mb-1">
                        <Badge className={`${getStatusColor(ticket.status)} text-white text-xs`}>
                          {ticket.status}
                        </Badge>
                        <span className={`text-sm font-mono ${sla.color}`}>{sla.text}</span>
                      </div>
                      <h4 className="font-medium text-base mb-1">{ticket.title}</h4>
                      <p className="text-sm text-slate-600 mb-2">{ticket.machineName}</p>
                      <div className="text-xs text-slate-500">
                        Down: {ticket.downtimeMinutes}min • {ticket.priority}
                      </div>
                    </div>
                    <ChevronRight className="h-5 w-5 text-slate-400" />
                  </div>
                </div>
              );
            })}
            
            <Button 
              variant="outline" 
              className="w-full h-12 text-base"
              onClick={() => setCurrentView('tasks')}
            >
              View All Tasks / အားလုံးကြည့်ရန်
            </Button>
          </CardContent>
        </Card>

        {/* Quick Actions */}
        <Card>
          <CardHeader className="pb-3">
            <CardTitle className="text-lg">Quick Actions</CardTitle>
            <p className="text-sm text-slate-600">လျင်မြန်လုပ်ဆောင်ချက်များ</p>
          </CardHeader>
          <CardContent className="grid grid-cols-2 gap-3">
            <Button 
              className="h-16 flex flex-col gap-1 bg-blue-600 hover:bg-blue-700"
              onClick={() => setShowQRScanner(true)}
            >
              <Scan className="h-6 w-6" />
              <span className="text-sm">QR Scan</span>
            </Button>
            
            <Button 
              className="h-16 flex flex-col gap-1 bg-green-600 hover:bg-green-700"
              onClick={() => setCurrentView('tasks')}
            >
              <ClipboardList className="h-6 w-6" />
              <span className="text-sm">My Tasks</span>
            </Button>
            
            <Button 
              className="h-16 flex flex-col gap-1 bg-purple-600 hover:bg-purple-700"
            >
              <Camera className="h-6 w-6" />
              <span className="text-sm">Photo</span>
            </Button>
            
            <Button 
              className="h-16 flex flex-col gap-1 bg-orange-600 hover:bg-orange-700"
              onClick={() => setCurrentView('history')}
            >
              <History className="h-6 w-6" />
              <span className="text-sm">History</span>
            </Button>
          </CardContent>
        </Card>
      </div>
    );
  };

  // Tasks Screen
  const renderTasksScreen = () => {
    const roleTickets = getRoleTickets();
    
    return (
      <div className="space-y-4 pb-20">
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <ClipboardList className="h-5 w-5" />
              My Tasks / ကျွန်ုပ်၏တာဝန်များ
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-3">
            {roleTickets.map((ticket) => {
              const sla = timeUntilSLA(ticket.slaDeadline);
              return (
                <div 
                  key={ticket.id}
                  className="border rounded-lg p-4 active:bg-slate-50 cursor-pointer"
                  onClick={() => setSelectedTicket(ticket)}
                >
                  <div className="flex items-start gap-3">
                    <div className={`w-1 h-16 ${getPriorityColor(ticket.priority)} rounded-full`}></div>
                    <div className="flex-1">
                      <div className="flex items-center gap-2 mb-1">
                        <Badge className={`${getStatusColor(ticket.status)} text-white text-xs`}>
                          {ticket.status}
                        </Badge>
                        <Badge variant="outline" className="text-xs">{ticket.priority}</Badge>
                        <span className={`text-sm font-mono ml-auto ${sla.color}`}>{sla.text}</span>
                      </div>
                      <h4 className="font-medium text-base mb-1">{ticket.title}</h4>
                      <p className="text-sm text-slate-600 mb-1">{ticket.description}</p>
                      <div className="text-xs text-slate-500">
                        {ticket.machineName} • Down: {ticket.downtimeMinutes}min
                      </div>
                    </div>
                  </div>
                </div>
              );
            })}
          </CardContent>
        </Card>

        {/* PM Tasks for Maintenance Engineer */}
        {currentRole === 'maintenance-engineer' && (
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Clock className="h-5 w-5" />
                Preventive Maintenance
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-3">
              {pmTasks.map((task) => (
                <div key={task.id} className={`border rounded-lg p-4 ${task.isOverdue ? 'border-red-300 bg-red-50' : ''}`}>
                  <div className="flex items-start justify-between mb-2">
                    <div>
                      <h4 className="font-medium">{task.taskName}</h4>
                      <p className="text-sm text-slate-600">{task.machineName}</p>
                      <div className="text-xs text-slate-500 mt-1">Due: {task.nextDue}</div>
                    </div>
                    {task.isOverdue && (
                      <Badge className="bg-red-100 text-red-800 text-xs">Overdue</Badge>
                    )}
                  </div>
                  
                  <div className="space-y-2">
                    {task.checklist.slice(0, 2).map((item) => (
                      <div key={item.id} className="flex items-center gap-2">
                        <Checkbox 
                          checked={item.completed}
                          onChange={() => {
                            // Update checklist item
                          }}
                        />
                        <span className={`text-sm ${item.completed ? 'line-through text-slate-500' : ''}`}>
                          {item.description}
                        </span>
                      </div>
                    ))}
                    {task.checklist.length > 2 && (
                      <div className="text-xs text-slate-500">+{task.checklist.length - 2} more items</div>
                    )}
                  </div>
                  
                  <Button className="w-full mt-3 bg-blue-600 hover:bg-blue-700">
                    <PlayCircle className="h-4 w-4 mr-2" />
                    Start PM Task
                  </Button>
                </div>
              ))}
            </CardContent>
          </Card>
        )}
      </div>
    );
  };

  // QR Scanner Screen
  const renderQRScanner = () => (
    <Dialog open={showQRScanner} onOpenChange={setShowQRScanner}>
      <DialogContent className="max-w-sm mx-auto">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <Scan className="h-5 w-5" />
            QR Code Scanner
          </DialogTitle>
          <DialogDescription>
            Scan machine QR code or spare part barcode
          </DialogDescription>
        </DialogHeader>
        
        <div className="bg-slate-100 rounded-lg p-8 text-center">
          <div className="w-48 h-48 mx-auto border-2 border-dashed border-blue-300 rounded-lg flex items-center justify-center">
            <div className="text-center">
              <Scan className="h-12 w-12 mx-auto mb-2 text-blue-500" />
              <p className="text-sm text-slate-600">Position QR code in frame</p>
              <p className="text-xs text-slate-500 mt-1">QR ကုဒ်ကို ဘောင်အတွင်းထားပါ</p>
            </div>
          </div>
        </div>
        
        <div className="flex gap-3">
          <Button 
            variant="outline" 
            className="flex-1"
            onClick={() => setShowQRScanner(false)}
          >
            Cancel
          </Button>
          <Button className="flex-1 bg-blue-600 hover:bg-blue-700">
            Manual Entry
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  );

  // Ticket Detail Modal for Mobile
  const renderMobileTicketDetail = () => {
    if (!selectedTicket) return null;

    const canEscalate = currentRole === 'plastic-technician';
    const canApprove = currentRole === 'technician-head';
    const canComplete = ['mold-technician', 'maintenance-engineer'].includes(currentRole);

    return (
      <Dialog open={!!selectedTicket} onOpenChange={() => setSelectedTicket(null)}>
        <DialogContent className="max-w-sm mx-auto max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle className="text-lg">
              {selectedTicket.title}
            </DialogTitle>
            <DialogDescription>
              {selectedTicket.machineName} • {selectedTicket.id}
            </DialogDescription>
          </DialogHeader>

          <div className="space-y-4">
            {/* Status and Priority */}
            <div className="flex gap-2">
              <Badge className={`${getStatusColor(selectedTicket.status)} text-white`}>
                {selectedTicket.status}
              </Badge>
              <Badge className={`${getPriorityColor(selectedTicket.priority)} text-white`}>
                {selectedTicket.priority}
              </Badge>
            </div>

            {/* Description */}
            <div>
              <Label className="text-sm font-medium">Description</Label>
              <p className="text-sm text-slate-600 mt-1">{selectedTicket.description}</p>
            </div>

            {/* Machine Readings */}
            {currentRole === 'plastic-technician' && (
              <div className="space-y-3">
                <Label className="text-sm font-medium">Machine Readings / စက်အခြေအနေ</Label>
                <div className="grid grid-cols-2 gap-3">
                  <div>
                    <Label className="text-xs">Temp (°C)</Label>
                    <Input type="number" placeholder="180" className="text-center text-lg h-12" />
                  </div>
                  <div>
                    <Label className="text-xs">Pressure (bar)</Label>
                    <Input type="number" placeholder="120" className="text-center text-lg h-12" />
                  </div>
                  <div>
                    <Label className="text-xs">Speed (RPM)</Label>
                    <Input type="number" placeholder="1500" className="text-center text-lg h-12" />
                  </div>
                  <div>
                    <Label className="text-xs">Cushion (mm)</Label>
                    <Input type="number" placeholder="5.2" className="text-center text-lg h-12" />
                  </div>
                </div>
              </div>
            )}

            {/* Mold Specific Readings */}
            {currentRole === 'mold-technician' && (
              <div className="space-y-3">
                <Label className="text-sm font-medium">Mold Setup / မှိုတပ်ဆင်မှု</Label>
                <div className="space-y-2">
                  <div>
                    <Label className="text-xs">Clamp Position (mm)</Label>
                    <Input type="number" placeholder="245" className="text-center text-lg h-12" />
                  </div>
                  <div>
                    <Label className="text-xs">Core Position (mm)</Label>
                    <Input type="number" placeholder="15.5" className="text-center text-lg h-12" />
                  </div>
                  <div>
                    <Label className="text-xs">Mold Temp (°C)</Label>
                    <Input type="number" placeholder="65" className="text-center text-lg h-12" />
                  </div>
                </div>
              </div>
            )}

            {/* Action Notes */}
            <div>
              <Label className="text-sm font-medium">Action Notes / လုပ်ဆောင်ချက်မှတ်ချက်</Label>
              <Textarea 
                rows={3}
                placeholder="Describe actions taken..."
                className="mt-1"
              />
            </div>

            {/* Photo Upload */}
            <div>
              <Label className="text-sm font-medium">Upload Photos / ဓာတ်ပုံတင်ရန်</Label>
              <Button 
                variant="outline" 
                className="w-full h-12 mt-1"
              >
                <Camera className="h-5 w-5 mr-2" />
                Take Photo
              </Button>
            </div>

            {/* Action Buttons */}
            <div className="space-y-2 pt-4 border-t">
              {canEscalate && selectedTicket.status === 'new' && (
                <>
                  <Button className="w-full h-12 bg-green-600 hover:bg-green-700">
                    <CheckCircle className="h-5 w-5 mr-2" />
                    Fix & Close / ပြီးပြင်စွာပိတ်ရန်
                  </Button>
                  <div className="grid grid-cols-2 gap-2">
                    <Button variant="outline" className="h-12">
                      <ArrowUp className="h-4 w-4 mr-1" />
                      To Mold
                    </Button>
                    <Button variant="outline" className="h-12">
                      <ArrowUp className="h-4 w-4 mr-1" />
                      To Maint
                    </Button>
                  </div>
                </>
              )}

              {canComplete && selectedTicket.status === 'escalated' && (
                <>
                  <Button className="w-full h-12 bg-purple-600 hover:bg-purple-700">
                    <CheckCircle className="h-5 w-5 mr-2" />
                    Complete Task / တာဝန်ပြီးစီးပြီ
                  </Button>
                  <Button variant="outline" className="w-full h-12">
                    <Send className="h-5 w-5 mr-2" />
                    Return to Head / လုပ်ငန်းမှူးထံပြန်ပို့
                  </Button>
                </>
              )}

              {canApprove && selectedTicket.status === 'completed' && (
                <>
                  <Button className="w-full h-12 bg-green-600 hover:bg-green-700">
                    <PlayCircle className="h-5 w-5 mr-2" />
                    Approve Restart / ပြန်လည်စတင်အတည်ပြု
                  </Button>
                  <Button variant="outline" className="w-full h-12 text-orange-600">
                    <RotateCcw className="h-5 w-5 mr-2" />
                    Send Back / ပြန်ပို့ရန်
                  </Button>
                </>
              )}
            </div>
          </div>
        </DialogContent>
      </Dialog>
    );
  };

  // History Screen
  const renderHistoryScreen = () => (
    <div className="space-y-4 pb-20">
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <History className="h-5 w-5" />
            Task History / တာဝန်မှတ်တမ်း
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-3">
          <div className="border rounded-lg p-4">
            <div className="flex items-center gap-2 mb-2">
              <div className="w-2 h-2 bg-green-500 rounded-full"></div>
              <span className="text-sm font-medium">Completed</span>
              <span className="text-xs text-slate-500 ml-auto">2 hours ago</span>
            </div>
            <h4 className="font-medium">Temperature Control Fixed</h4>
            <p className="text-sm text-slate-600">INJ-003 • Replaced temperature sensor</p>
          </div>
          
          <div className="border rounded-lg p-4">
            <div className="flex items-center gap-2 mb-2">
              <div className="w-2 h-2 bg-blue-500 rounded-full"></div>
              <span className="text-sm font-medium">Escalated</span>
              <span className="text-xs text-slate-500 ml-auto">4 hours ago</span>
            </div>
            <h4 className="font-medium">Hydraulic Leak</h4>
            <p className="text-sm text-slate-600">INJ-001 • Sent to Maintenance Engineer</p>
          </div>
        </CardContent>
      </Card>
    </div>
  );

  // Profile Screen
  const renderProfileScreen = () => (
    <div className="space-y-4 pb-20">
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <User className="h-5 w-5" />
            Profile / ကိုယ်ရေးအချက်အလက်
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="text-center">
            <div className="w-16 h-16 bg-blue-100 rounded-full mx-auto mb-3 flex items-center justify-center">
              {getRoleIcon(currentRole)}
            </div>
            <h3 className="font-medium">{getRoleTitle(currentRole)}</h3>
            <p className="text-sm text-slate-600">John Doe • EMP001</p>
          </div>
          
          <div className="space-y-3 pt-4 border-t">
            <div className="flex items-center justify-between">
              <span>Online Status</span>
              <div className="flex items-center gap-2">
                {isOnline ? <Wifi className="h-4 w-4 text-green-600" /> : <WifiOff className="h-4 w-4 text-red-600" />}
                <span className="text-sm">{isOnline ? 'Online' : 'Offline'}</span>
              </div>
            </div>
            
            <div className="flex items-center justify-between">
              <span>Notifications</span>
              <div className="flex items-center gap-2">
                <Bell className="h-4 w-4" />
                <span className="text-sm">{notifications.filter(n => !n.read).length} unread</span>
              </div>
            </div>
          </div>

          <div className="space-y-2 pt-4 border-t">
            <Label className="text-sm font-medium">Change Role (Demo)</Label>
            <div className="grid grid-cols-2 gap-2">
              {(['plastic-technician', 'mold-technician', 'maintenance-engineer', 'technician-head'] as EngineerRole[]).map((role) => (
                <Button
                  key={role}
                  variant={currentRole === role ? "default" : "outline"}
                  className="h-16 flex flex-col gap-1 text-xs"
                  onClick={() => setCurrentRole(role)}
                >
                  {getRoleIcon(role)}
                  <span>{getRoleTitle(role)}</span>
                </Button>
              ))}
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );

  // Bottom Navigation
  const renderBottomNav = () => (
    <div className="fixed bottom-0 left-0 right-0 bg-white border-t border-slate-200 p-2">
      <div className="grid grid-cols-5 gap-1">
        {[
          { id: 'home', icon: Home, label: 'Home', labelMy: 'မူလ' },
          { id: 'tasks', icon: ClipboardList, label: 'Tasks', labelMy: 'တာဝန်' },
          { id: 'scan', icon: Scan, label: 'Scan', labelMy: 'စကင်' },
          { id: 'history', icon: History, label: 'History', labelMy: 'မှတ်တမ်း' },
          { id: 'profile', icon: User, label: 'Profile', labelMy: 'ကိုယ်ရေး' }
        ].map((item) => {
          const Icon = item.icon;
          const isActive = currentView === item.id;
          return (
            <Button
              key={item.id}
              variant="ghost"
              className={`h-16 flex flex-col gap-1 p-2 ${isActive ? 'text-blue-600 bg-blue-50' : 'text-slate-600'}`}
              onClick={() => {
                if (item.id === 'scan') {
                  setShowQRScanner(true);
                } else {
                  setCurrentView(item.id as MobileView);
                }
              }}
            >
              <Icon className={`h-6 w-6 ${isActive ? 'text-blue-600' : 'text-slate-600'}`} />
              <span className="text-xs">{item.label}</span>
              <span className="text-xs opacity-70">{item.labelMy}</span>
            </Button>
          );
        })}
      </div>
    </div>
  );

  const renderCurrentView = () => {
    switch (currentView) {
      case 'home': return renderHomeScreen();
      case 'tasks': return renderTasksScreen();
      case 'history': return renderHistoryScreen();
      case 'profile': return renderProfileScreen();
      default: return renderHomeScreen();
    }
  };

  return (
    <div className="min-h-screen bg-slate-50">
      {/* Status Bar */}
      <div className="bg-blue-600 text-white p-2 flex items-center justify-between text-sm">
        <div className="flex items-center gap-2">
          <span>Engineering PWA</span>
          {!isOnline && (
            <Badge className="bg-red-500 text-white text-xs">Offline</Badge>
          )}
        </div>
        <div className="flex items-center gap-2">
          <span>{new Date().toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' })}</span>
          <div className="flex items-center">
            <div className="w-1 h-1 bg-white rounded-full mx-0.5"></div>
            <div className="w-1 h-1 bg-white rounded-full mx-0.5"></div>
            <div className="w-1 h-1 bg-white/50 rounded-full mx-0.5"></div>
          </div>
        </div>
      </div>

      {/* Main Content */}
      <div className="p-4">
        {renderCurrentView()}
      </div>

      {/* Bottom Navigation */}
      {renderBottomNav()}

      {/* QR Scanner Modal */}
      {renderQRScanner()}

      {/* Ticket Detail Modal */}
      {renderMobileTicketDetail()}
    </div>
  );
}