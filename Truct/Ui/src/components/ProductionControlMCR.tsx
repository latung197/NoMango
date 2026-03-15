import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Textarea } from './ui/textarea';
import { Badge } from './ui/badge';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from './ui/dialog';
import { Checkbox } from './ui/checkbox';
import { toast } from "sonner";
import { 
  Search, 
  Filter, 
  Settings, 
  Eye, 
  Edit, 
  Check, 
  X, 
  Calendar,
  Clock,
  User,
  AlertTriangle,
  FileText,
  Plus,
  ChevronRight,
  Save,
  XCircle,
  Bell,
  ArrowUpDown,
  RefreshCw,
  Download,
  Upload
} from 'lucide-react';

interface MCRData {
  id: string;
  jobId: string;
  machine: string;
  currentMold: string;
  requiredMold: string;
  type: 'Planned' | 'Emergency' | 'Maintenance';
  estimatedTime: number; // in minutes
  status: 'Pending' | 'Approved' | 'Rejected' | 'In Progress' | 'Completed' | 'Retry Required';
  plannedStart: string;
  actualStart?: string;
  actualEnd?: string;
  technician?: string;
  priority: 'Low' | 'Medium' | 'High' | 'Critical';
  createdBy: string;
  createdAt: string;
  notes?: string;
  previousJob?: {
    id: string;
    product: string;
    mold: string;
    material: string;
    color: string;
  };
  nextJob?: {
    id: string;
    product: string;
    mold: string;
    material: string;
    color: string;
  };
}

const mockMCRData: MCRData[] = [
  {
    id: 'MCR-20250903-001',
    jobId: 'JOB-20250903-001',
    machine: 'INJ-M001',
    currentMold: 'M-001-500ML-BTL',
    requiredMold: 'M-002-1L-CONT',
    type: 'Planned',
    estimatedTime: 45,
    status: 'Pending',
    plannedStart: '2025-09-03 14:00',
    technician: 'Ko Thant',
    priority: 'Medium',
    createdBy: 'Production Supervisor',
    createdAt: '2025-09-03 09:30',
    notes: 'Standard changeover between bottle and container production',
    previousJob: {
      id: 'JOB-20250903-001',
      product: 'Plastic Bottle 500ml',
      mold: 'M-001-500ML-BTL',
      material: 'PET Clear',
      color: 'Clear'
    },
    nextJob: {
      id: 'JOB-20250903-002',
      product: 'Plastic Container 1L',
      mold: 'M-002-1L-CONT',
      material: 'PP White',
      color: 'White'
    }
  },
  {
    id: 'MCR-20250903-002',
    jobId: 'JOB-20250903-003',
    machine: 'INJ-M002',
    currentMold: 'M-003-250ML-CUP',
    requiredMold: 'M-004-TRAY-LG',
    type: 'Emergency',
    estimatedTime: 60,
    status: 'Approved',
    plannedStart: '2025-09-03 16:30',
    technician: 'Ma Phyu',
    priority: 'High',
    createdBy: 'Production Manager',
    createdAt: '2025-09-03 11:15',
    notes: 'Urgent changeover due to customer demand',
    previousJob: {
      id: 'JOB-20250903-003',
      product: 'Plastic Cup 250ml',
      mold: 'M-003-250ML-CUP',
      material: 'PP Blue',
      color: 'Blue'
    },
    nextJob: {
      id: 'JOB-20250903-004',
      product: 'Large Tray',
      mold: 'M-004-TRAY-LG',
      material: 'ABS Black',
      color: 'Black'
    }
  },
  {
    id: 'MCR-20250903-003',
    jobId: 'JOB-20250903-005',
    machine: 'INJ-M003',
    currentMold: 'M-005-BOTTLE-1L',
    requiredMold: 'M-006-CONTAINER-2L',
    type: 'Maintenance',
    estimatedTime: 90,
    status: 'In Progress',
    plannedStart: '2025-09-03 10:00',
    actualStart: '2025-09-03 10:15',
    technician: 'Ko Aung',
    priority: 'Critical',
    createdBy: 'Maintenance Supervisor',
    createdAt: '2025-09-02 16:45',
    notes: 'Mold maintenance required during changeover'
  }
];

const availableMolds = [
  { id: 'M-001-500ML-BTL', name: '500ml Bottle Mold', status: 'Available', location: 'Rack A-01' },
  { id: 'M-002-1L-CONT', name: '1L Container Mold', status: 'Available', location: 'Rack A-02' },
  { id: 'M-003-250ML-CUP', name: '250ml Cup Mold', status: 'In Use', location: 'INJ-M002' },
  { id: 'M-004-TRAY-LG', name: 'Large Tray Mold', status: 'Maintenance', location: 'Workshop' },
  { id: 'M-005-BOTTLE-1L', name: '1L Bottle Mold', status: 'In Use', location: 'INJ-M003' },
  { id: 'M-006-CONTAINER-2L', name: '2L Container Mold', status: 'Available', location: 'Rack B-01' }
];

const technicians = [
  { id: 'T001', name: 'Ko Thant', department: 'Mold Tech', experience: 'Senior' },
  { id: 'T002', name: 'Ma Phyu', department: 'Mold Tech', experience: 'Senior' },
  { id: 'T003', name: 'Ko Aung', department: 'Mold Tech', experience: 'Expert' },
  { id: 'T004', name: 'Ma Soe', department: 'Mold Tech', experience: 'Junior' }
];

const requiredEquipment = [
  'Crane Operator',
  'Mold Cart',
  'Alignment Tools',
  'Safety Equipment',
  'Cleaning Supplies',
  'Torque Wrench Set',
  'Temperature Gun'
];

export function ProductionControlMCR() {
  const [selectedMCR, setSelectedMCR] = useState<MCRData | null>(null);
  const [showDetailModal, setShowDetailModal] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState('all');
  const [machineFilter, setMachineFilter] = useState('all');
  const [dateFilter, setDateFilter] = useState('today');

  // Edit form state
  const [editForm, setEditForm] = useState({
    moldToInstall: '',
    toolLocation: '',
    requiredEquipment: [] as string[],
    assignedTechnicians: [] as string[],
    plannedStart: '',
    plannedEnd: '',
    estimatedTime: 0,
    notes: ''
  });

  // Technician checklist state
  const [checklist, setChecklist] = useState({
    clampInspection: false,
    coolingConnection: false,
    hotRunnerSetup: false,
    sensorCalibration: false,
    firstShotSample: false
  });

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Pending': return 'bg-blue-100 text-blue-800 border-blue-200';
      case 'Approved': return 'bg-green-100 text-green-800 border-green-200';
      case 'Rejected': return 'bg-red-100 text-red-800 border-red-200';
      case 'In Progress': return 'bg-orange-100 text-orange-800 border-orange-200';
      case 'Completed': return 'bg-emerald-100 text-emerald-800 border-emerald-200';
      case 'Retry Required': return 'bg-yellow-100 text-yellow-800 border-yellow-200';
      default: return 'bg-gray-100 text-gray-800 border-gray-200';
    }
  };

  const getPriorityColor = (priority: string) => {
    switch (priority) {
      case 'Critical': return 'bg-red-100 text-red-800';
      case 'High': return 'bg-orange-100 text-orange-800';
      case 'Medium': return 'bg-yellow-100 text-yellow-800';
      case 'Low': return 'bg-green-100 text-green-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  const getTypeColor = (type: string) => {
    switch (type) {
      case 'Planned': return 'bg-blue-100 text-blue-800';
      case 'Emergency': return 'bg-red-100 text-red-800';
      case 'Maintenance': return 'bg-purple-100 text-purple-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  const filteredMCRs = mockMCRData.filter(mcr => {
    const matchesSearch = mcr.id.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         mcr.jobId.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         mcr.machine.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesStatus = statusFilter === 'all' || mcr.status === statusFilter;
    const matchesMachine = machineFilter === 'all' || mcr.machine === machineFilter;
    
    return matchesSearch && matchesStatus && matchesMachine;
  });

  const handleViewDetails = (mcr: MCRData) => {
    setSelectedMCR(mcr);
    setEditForm({
      moldToInstall: mcr.requiredMold,
      toolLocation: availableMolds.find(m => m.id === mcr.requiredMold)?.location || '',
      requiredEquipment: [],
      assignedTechnicians: mcr.technician ? [mcr.technician] : [],
      plannedStart: mcr.plannedStart,
      plannedEnd: '',
      estimatedTime: mcr.estimatedTime,
      notes: mcr.notes || ''
    });
    setShowDetailModal(true);
  };

  const handleApproveMCR = (mcrId: string) => {
    toast.success(
      'MCR approved successfully. Technicians have been notified. | MCR အတည်ပြုပြီးပါပြီ။ နည်းပညာသမားများကို အကြောင်းကြားပြီးပါပြီ။',
      {
        description: `MCR ${mcrId} is now available for technician assignment`,
        action: {
          label: 'View Details',
          onClick: () => console.log('View MCR details')
        }
      }
    );
    // Update MCR status logic here
    // Send notification to assigned technicians
  };

  const handleRejectMCR = (mcrId: string) => {
    const reason = prompt('Please provide a reason for rejection | ငြင်းပယ်ရသည့်အကြောင်းရင်းကို ဖော်ပြပါ:');
    if (reason) {
      toast.error(
        `MCR rejected: ${reason} | MCR ကို ငြင်းပယ်ခဲ့သည်: ${reason}`,
        {
          description: `MCR ${mcrId} has been rejected and requester notified`,
        }
      );
      // Update MCR status logic here
      // Send notification to requester
    }
  };

  const handleSaveDraft = () => {
    // Validate required fields
    if (!editForm.moldToInstall) {
      toast.error('Please select a mold to install | တပ်ဆင်မည့်မိုကို ရွေးချယ်ပါ');
      return;
    }
    if (!editForm.plannedStart) {
      toast.error('Please set planned start time | စီစဉ်ထားသည့်အချိန်ကို သတ်မှတ်ပါ');
      return;
    }
    if (editForm.assignedTechnicians.length === 0) {
      toast.error('Please assign at least one technician | နည်းပညာသမားအနည်းဆုံးတစ်ယောက်ကို တာဝန်ပေးအပ်ပါ');
      return;
    }

    toast.success('Draft saved successfully | မူကြမ်းအောင်မြင်စွာသိမ်းပြီး', {
      description: 'All changes have been saved. You can continue editing later.',
    });
    setShowDetailModal(false);
  };

  const handleEquipmentChange = (equipment: string, checked: boolean) => {
    setEditForm(prev => ({
      ...prev,
      requiredEquipment: checked 
        ? [...prev.requiredEquipment, equipment]
        : prev.requiredEquipment.filter(e => e !== equipment)
    }));
  };

  const handleTechnicianChange = (technician: string, checked: boolean) => {
    setEditForm(prev => ({
      ...prev,
      assignedTechnicians: checked 
        ? [...prev.assignedTechnicians, technician]
        : prev.assignedTechnicians.filter(t => t !== technician)
    }));
  };

  const urgentMCRs = mockMCRData.filter(mcr => 
    mcr.priority === 'Critical' || mcr.priority === 'High'
  );

  return (
    <div className="p-6 bg-slate-50 min-h-screen">
      <div className="max-w-7xl mx-auto space-y-6">
        {/* Urgent MCR Alert Banner */}
        {urgentMCRs.length > 0 && (
          <div className="bg-red-50 border border-red-200 rounded-lg p-4">
            <div className="flex items-center gap-3">
              <Bell className="h-5 w-5 text-red-600" />
              <div className="flex-1">
                <h3 className="font-medium text-red-800">
                  {urgentMCRs.length} Urgent MCR{urgentMCRs.length > 1 ? 's' : ''} Require Attention | 
                  အရေးကြီး MCR {urgentMCRs.length} ခု အာရုံစိုက်ရန်လိုအပ်
                </h3>
                <p className="text-sm text-red-600 mt-1">
                  {urgentMCRs.map(mcr => mcr.id).join(', ')} - Please review and approve immediately
                </p>
              </div>
              <Button 
                size="sm" 
                className="bg-red-600 hover:bg-red-700 text-white"
                onClick={() => setStatusFilter('Pending')}
              >
                Review Now | ယခုပင်ပြန်စစ်
              </Button>
            </div>
          </div>
        )}
        {/* Header */}
        <div className="bg-white rounded-lg shadow-sm p-6">
          <div className="flex items-center justify-between mb-6">
            <div>
              <h1 className="text-2xl font-semibold text-slate-900 mb-2">
                Mold Change Request | မို ပြောင်းလဲမှု
              </h1>
              <p className="text-slate-600">
                Manage mold changeover requests and workflows | မို ပြောင်းလဲတောင်းဆိုမှုများနှင့် လုပ်ငန်းစဉ်များကို စီမံခန့်ခွဲပါ
              </p>
            </div>
            <div className="flex gap-2">
              <Button variant="outline" className="text-slate-600">
                <RefreshCw className="h-4 w-4 mr-2" />
                Refresh | ပြန်လည်ပြင်ဆင်
              </Button>
              <Button variant="outline" className="text-slate-600">
                <Download className="h-4 w-4 mr-2" />
                Export | ထုတ်ယူ
              </Button>
              <Button className="bg-blue-600 hover:bg-blue-700 text-white">
                <Plus className="h-4 w-4 mr-2" />
                New MCR | MCR အသစ်
              </Button>
            </div>
          </div>

          {/* Filters */}
          <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
            <div>
              <Label>Search Job ID / MCR ID</Label>
              <div className="relative">
                <Search className="absolute left-3 top-1/2 transform -y-1/2 h-4 w-4 text-slate-400" />
                <Input
                  placeholder="Search..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="pl-10"
                />
              </div>
            </div>
            <div>
              <Label>Machine | စက်</Label>
              <Select value={machineFilter} onValueChange={setMachineFilter}>
                <SelectTrigger>
                  <SelectValue placeholder="All Machines" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All Machines | စက်အားလုံး</SelectItem>
                  <SelectItem value="INJ-M001">INJ-M001</SelectItem>
                  <SelectItem value="INJ-M002">INJ-M002</SelectItem>
                  <SelectItem value="INJ-M003">INJ-M003</SelectItem>
                  <SelectItem value="INJ-M004">INJ-M004</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div>
              <Label>Status | အခြေအနေ</Label>
              <Select value={statusFilter} onValueChange={setStatusFilter}>
                <SelectTrigger>
                  <SelectValue placeholder="All Status" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All Status | အားလုံး</SelectItem>
                  <SelectItem value="Pending">Pending | စောင့်ဆိုင်း</SelectItem>
                  <SelectItem value="Approved">Approved | အတည်ပြု</SelectItem>
                  <SelectItem value="Rejected">Rejected | ငြင်းပယ်</SelectItem>
                  <SelectItem value="In Progress">In Progress | လုပ်ဆောင်</SelectItem>
                  <SelectItem value="Completed">Completed | ပြီးစီး</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div>
              <Label>Date Range | ရက်စွဲအပိုင်းအခြား</Label>
              <Select value={dateFilter} onValueChange={setDateFilter}>
                <SelectTrigger>
                  <SelectValue placeholder="Select date range" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="today">Today | ယနေ့</SelectItem>
                  <SelectItem value="week">This Week | ဤအပတ်</SelectItem>
                  <SelectItem value="month">This Month | ဤလ</SelectItem>
                  <SelectItem value="custom">Custom Range | စိတ်ကြိုက်</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>
        </div>

        {/* Status Summary Cards */}
        <div className="grid grid-cols-2 md:grid-cols-6 gap-4">
          <Card className="p-4">
            <div className="text-center">
              <div className="text-2xl font-bold text-blue-600">
                {mockMCRData.filter(mcr => mcr.status === 'Pending').length}
              </div>
              <div className="text-sm text-slate-600">Pending | စောင့်ဆိုင်း</div>
            </div>
          </Card>
          <Card className="p-4">
            <div className="text-center">
              <div className="text-2xl font-bold text-green-600">
                {mockMCRData.filter(mcr => mcr.status === 'Approved').length}
              </div>
              <div className="text-sm text-slate-600">Approved | အတည်ပြု</div>
            </div>
          </Card>
          <Card className="p-4">
            <div className="text-center">
              <div className="text-2xl font-bold text-orange-600">
                {mockMCRData.filter(mcr => mcr.status === 'In Progress').length}
              </div>
              <div className="text-sm text-slate-600">In Progress | လုပ်ဆောင်</div>
            </div>
          </Card>
          <Card className="p-4">
            <div className="text-center">
              <div className="text-2xl font-bold text-emerald-600">
                {mockMCRData.filter(mcr => mcr.status === 'Completed').length}
              </div>
              <div className="text-sm text-slate-600">Completed | ပြီးစီး</div>
            </div>
          </Card>
          <Card className="p-4">
            <div className="text-center">
              <div className="text-2xl font-bold text-red-600">
                {mockMCRData.filter(mcr => mcr.status === 'Rejected').length}
              </div>
              <div className="text-sm text-slate-600">Rejected | ငြင်းပယ်</div>
            </div>
          </Card>
          <Card className="p-4">
            <div className="text-center">
              <div className="text-2xl font-bold text-yellow-600">
                {mockMCRData.filter(mcr => mcr.status === 'Retry Required').length}
              </div>
              <div className="text-sm text-slate-600">Retry | ပြန်လုပ်</div>
            </div>
          </Card>
        </div>

        {/* Data Table */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Settings className="h-5 w-5" />
              Mold Change Requests | မို ပြောင်းလဲတောင်းဆိုမှုများ
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead className="cursor-pointer hover:bg-slate-50">
                      <div className="flex items-center gap-1">
                        MCR ID
                        <ArrowUpDown className="h-3 w-3" />
                      </div>
                    </TableHead>
                    <TableHead className="cursor-pointer hover:bg-slate-50">
                      <div className="flex items-center gap-1">
                        Job ID
                        <ArrowUpDown className="h-3 w-3" />
                      </div>
                    </TableHead>
                    <TableHead>Machine | စက်</TableHead>
                    <TableHead>Current Mold | လက်ရှိမို</TableHead>
                    <TableHead>Required Mold | လိုအပ်သောမို</TableHead>
                    <TableHead>Type | အမျိုးအစား</TableHead>
                    <TableHead className="cursor-pointer hover:bg-slate-50">
                      <div className="flex items-center gap-1">
                        Est. Time | ခန့်မှန်းအချိန်
                        <ArrowUpDown className="h-3 w-3" />
                      </div>
                    </TableHead>
                    <TableHead>Status | အခြေအနေ</TableHead>
                    <TableHead className="cursor-pointer hover:bg-slate-50">
                      <div className="flex items-center gap-1">
                        Planned Start | စီစဉ်ထားသည့်အစ
                        <ArrowUpDown className="h-3 w-3" />
                      </div>
                    </TableHead>
                    <TableHead>Technician | နည်းပညာသမား</TableHead>
                    <TableHead className="text-center">Actions | လုပ်ဆောင်ချက်များ</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {filteredMCRs.map((mcr) => (
                    <TableRow 
                      key={mcr.id} 
                      className="cursor-pointer hover:bg-slate-50"
                      onClick={() => handleViewDetails(mcr)}
                    >
                      <TableCell className="font-medium">
                        <div className="flex items-center gap-2">
                          {mcr.priority === 'Critical' && <AlertTriangle className="h-4 w-4 text-red-500" />}
                          {mcr.priority === 'High' && <AlertTriangle className="h-4 w-4 text-orange-500" />}
                          {mcr.id}
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-1">
                          {mcr.jobId}
                          <Badge variant="outline" className={getPriorityColor(mcr.priority)}>
                            {mcr.priority}
                          </Badge>
                        </div>
                      </TableCell>
                      <TableCell className="font-medium">{mcr.machine}</TableCell>
                      <TableCell className="max-w-32 truncate">{mcr.currentMold}</TableCell>
                      <TableCell className="max-w-32 truncate">{mcr.requiredMold}</TableCell>
                      <TableCell>
                        <Badge className={getTypeColor(mcr.type)}>
                          {mcr.type}
                        </Badge>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-1">
                          <Clock className="h-3 w-3 text-slate-400" />
                          {mcr.estimatedTime} min
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-2">
                          <Badge className={getStatusColor(mcr.status)}>
                            {mcr.status}
                          </Badge>
                          {mcr.status === 'In Progress' && (
                            <div className="w-2 h-2 bg-orange-500 rounded-full animate-pulse"></div>
                          )}
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="text-sm">
                          {mcr.plannedStart}
                          {mcr.actualStart && (
                            <div className="text-xs text-green-600">
                              Started: {mcr.actualStart}
                            </div>
                          )}
                        </div>
                      </TableCell>
                      <TableCell>{mcr.technician || '-'}</TableCell>
                      <TableCell>
                        <div className="flex gap-1">
                          <Button
                            size="sm"
                            variant="outline"
                            onClick={(e) => {
                              e.stopPropagation();
                              handleViewDetails(mcr);
                            }}
                          >
                            <Eye className="h-3 w-3" />
                          </Button>
                          {mcr.status === 'Pending' && (
                            <>
                              <Button
                                size="sm"
                                variant="outline"
                                className="text-green-600 border-green-600 hover:bg-green-50"
                                onClick={(e) => {
                                  e.stopPropagation();
                                  handleApproveMCR(mcr.id);
                                }}
                              >
                                <Check className="h-3 w-3" />
                              </Button>
                              <Button
                                size="sm"
                                variant="outline"
                                className="text-red-600 border-red-600 hover:bg-red-50"
                                onClick={(e) => {
                                  e.stopPropagation();
                                  handleRejectMCR(mcr.id);
                                }}
                              >
                                <X className="h-3 w-3" />
                              </Button>
                            </>
                          )}
                        </div>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          </CardContent>
        </Card>

        {/* Detail Modal */}
        <Dialog open={showDetailModal} onOpenChange={setShowDetailModal}>
          <DialogContent className="max-w-5xl max-h-[90vh] overflow-y-auto">
            <DialogHeader>
              <DialogTitle className="flex items-center gap-2">
                <Settings className="h-5 w-5" />
                MCR Details: {selectedMCR?.id} | MCR အသေးစိတ်
              </DialogTitle>
              <DialogDescription>
                Mold change request details and configuration | မို ပြောင်းလဲတောင်းဆိုမှု အသေးစိတ်နှင့် ကိန်ဂူရေးရှင်း
              </DialogDescription>
            </DialogHeader>

            {selectedMCR && (
              <div className="space-y-6">
                {/* Job Context */}
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <Card>
                    <CardHeader>
                      <CardTitle className="text-lg">Previous Job | အရင်အလုပ်</CardTitle>
                    </CardHeader>
                    <CardContent className="space-y-3">
                      <div>
                        <Label className="text-sm text-slate-600">Product | ထုတ်ကုန်</Label>
                        <p className="font-medium">{selectedMCR.previousJob?.product}</p>
                      </div>
                      <div>
                        <Label className="text-sm text-slate-600">Mold | မို</Label>
                        <p className="font-medium">{selectedMCR.previousJob?.mold}</p>
                      </div>
                      <div className="grid grid-cols-2 gap-4">
                        <div>
                          <Label className="text-sm text-slate-600">Material | ပစ္စည်း</Label>
                          <p className="font-medium">{selectedMCR.previousJob?.material}</p>
                        </div>
                        <div>
                          <Label className="text-sm text-slate-600">Color | အရောင်</Label>
                          <p className="font-medium">{selectedMCR.previousJob?.color}</p>
                        </div>
                      </div>
                    </CardContent>
                  </Card>

                  <Card>
                    <CardHeader>
                      <CardTitle className="text-lg">Next Job | နောက်အလုပ်</CardTitle>
                    </CardHeader>
                    <CardContent className="space-y-3">
                      <div>
                        <Label className="text-sm text-slate-600">Product | ထုတ်ကုန်</Label>
                        <p className="font-medium">{selectedMCR.nextJob?.product}</p>
                      </div>
                      <div>
                        <Label className="text-sm text-slate-600">Mold | မို</Label>
                        <p className="font-medium">{selectedMCR.nextJob?.mold}</p>
                      </div>
                      <div className="grid grid-cols-2 gap-4">
                        <div>
                          <Label className="text-sm text-slate-600">Material | ပစ္စည်း</Label>
                          <p className="font-medium">{selectedMCR.nextJob?.material}</p>
                        </div>
                        <div>
                          <Label className="text-sm text-slate-600">Color | အရောင်</Label>
                          <p className="font-medium">{selectedMCR.nextJob?.color}</p>
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                </div>

                {/* Changeover Details */}
                <Card>
                  <CardHeader>
                    <CardTitle className="text-lg">Changeover Configuration | ပြောင်းလဲမှု ကိန်ဂူရေးရှင်း</CardTitle>
                  </CardHeader>
                  <CardContent className="space-y-4">
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                      <div>
                        <Label>Mold to Install | တပ်ဆင်မည့်မို *</Label>
                        <Select value={editForm.moldToInstall} onValueChange={(value) => setEditForm(prev => ({...prev, moldToInstall: value}))}>
                          <SelectTrigger>
                            <SelectValue placeholder="Select mold" />
                          </SelectTrigger>
                          <SelectContent>
                            {availableMolds.map((mold) => (
                              <SelectItem key={mold.id} value={mold.id}>
                                {mold.name} - {mold.status} ({mold.location})
                              </SelectItem>
                            ))}
                          </SelectContent>
                        </Select>
                      </div>
                      <div>
                        <Label>Tool Location | ကိရိယာတည်နေရာ</Label>
                        <Input
                          value={editForm.toolLocation}
                          onChange={(e) => setEditForm(prev => ({...prev, toolLocation: e.target.value}))}
                          placeholder="e.g., Rack A-01"
                        />
                      </div>
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                      <div>
                        <Label>Estimated Time (minutes) | ခန့်မှန်းအချိန် (မိနစ်)</Label>
                        <Input
                          type="number"
                          value={editForm.estimatedTime}
                          onChange={(e) => setEditForm(prev => ({...prev, estimatedTime: parseInt(e.target.value) || 0}))}
                          placeholder="45"
                        />
                      </div>
                      <div>
                        <Label>Changeover Type | ပြောင်းလဲမှုအမျိုးအစား</Label>
                        <div className="p-3 bg-slate-50 rounded-lg">
                          <Badge className={getTypeColor(selectedMCR.type)}>
                            {selectedMCR.type} Changeover
                          </Badge>
                        </div>
                      </div>
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                      <div>
                        <Label>Planned Start | စီစဉ်ထားသည့်အစ</Label>
                        <Input
                          type="datetime-local"
                          value={editForm.plannedStart}
                          onChange={(e) => setEditForm(prev => ({...prev, plannedStart: e.target.value}))}
                        />
                      </div>
                      <div>
                        <Label>Planned End | စီစဉ်ထားသည့်အဆုံး</Label>
                        <Input
                          type="datetime-local"
                          value={editForm.plannedEnd}
                          onChange={(e) => setEditForm(prev => ({...prev, plannedEnd: e.target.value}))}
                        />
                      </div>
                    </div>
                  </CardContent>
                </Card>

                {/* Required Equipment */}
                <Card>
                  <CardHeader>
                    <CardTitle className="text-lg">Required Equipment | လိုအပ်သောကိရိယာများ</CardTitle>
                  </CardHeader>
                  <CardContent>
                    <div className="grid grid-cols-2 md:grid-cols-3 gap-3">
                      {requiredEquipment.map((equipment) => (
                        <div key={equipment} className="flex items-center space-x-2">
                          <Checkbox
                            id={equipment}
                            checked={editForm.requiredEquipment.includes(equipment)}
                            onCheckedChange={(checked) => handleEquipmentChange(equipment, checked as boolean)}
                          />
                          <Label htmlFor={equipment} className="text-sm">
                            {equipment}
                          </Label>
                        </div>
                      ))}
                    </div>
                  </CardContent>
                </Card>

                {/* Assigned Technicians */}
                <Card>
                  <CardHeader>
                    <CardTitle className="text-lg">Assigned Technicians | တာဝန်ပေးအပ်ထားသောနည်းပညာသမားများ</CardTitle>
                  </CardHeader>
                  <CardContent>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                      {technicians.map((tech) => (
                        <div key={tech.id} className="flex items-center justify-between p-3 border rounded-lg">
                          <div className="flex items-center space-x-3">
                            <Checkbox
                              id={tech.id}
                              checked={editForm.assignedTechnicians.includes(tech.name)}
                              onCheckedChange={(checked) => handleTechnicianChange(tech.name, checked as boolean)}
                            />
                            <div>
                              <Label htmlFor={tech.id} className="font-medium">
                                {tech.name}
                              </Label>
                              <p className="text-sm text-slate-600">
                                {tech.department} - {tech.experience}
                              </p>
                            </div>
                          </div>
                          <Badge variant="outline">{tech.experience}</Badge>
                        </div>
                      ))}
                    </div>
                  </CardContent>
                </Card>

                {/* Technician Checklist */}
                <Card>
                  <CardHeader>
                    <CardTitle className="text-lg">Technician Checklist | နည်းပညာသမားစစ်ဆေးစာရင်း</CardTitle>
                  </CardHeader>
                  <CardContent>
                    <div className="space-y-3">
                      {Object.entries({
                        clampInspection: 'Clamp Mechanism Inspection | ကလမ်ယန္တရားစစ်ဆေးခြင်း',
                        coolingConnection: 'Cooling Line Connection | အေးခဲလိုင်းချိတ်ဆက်ခြင်း',
                        hotRunnerSetup: 'Hot Runner Setup | Hot Runner ထူထောင်ခြင်း',
                        sensorCalibration: 'Sensor Calibration | အာရုံခံကိရိယာညှိခြင်း',
                        firstShotSample: 'First Shot Sample Check | ပထမအကြိမ်နမူနာစစ်ဆေးခြင်း'
                      }).map(([key, label]) => (
                        <div key={key} className="flex items-center space-x-3">
                          <Checkbox
                            id={key}
                            checked={checklist[key as keyof typeof checklist]}
                            onCheckedChange={(checked) => 
                              setChecklist(prev => ({...prev, [key]: checked as boolean}))
                            }
                          />
                          <Label htmlFor={key} className="flex-1">
                            {label}
                          </Label>
                        </div>
                      ))}
                    </div>
                  </CardContent>
                </Card>

                {/* Notes */}
                <Card>
                  <CardHeader>
                    <CardTitle className="text-lg">Notes & Comments | မှတ်ချက်များ</CardTitle>
                  </CardHeader>
                  <CardContent>
                    <Textarea
                      value={editForm.notes}
                      onChange={(e) => setEditForm(prev => ({...prev, notes: e.target.value}))}
                      placeholder="Add any additional notes or special instructions..."
                      rows={4}
                    />
                  </CardContent>
                </Card>

                {/* Footer Actions */}
                <div className="flex justify-end gap-3 pt-4 border-t">
                  <Button
                    variant="outline"
                    onClick={() => setShowDetailModal(false)}
                  >
                    <XCircle className="h-4 w-4 mr-2" />
                    Cancel | ပယ်ဖျက်
                  </Button>
                  <Button
                    variant="outline"
                    onClick={handleSaveDraft}
                  >
                    <Save className="h-4 w-4 mr-2" />
                    Save Draft | မူကြမ်းသိမ်း
                  </Button>
                  {selectedMCR.status === 'Pending' && (
                    <>
                      <Button
                        variant="outline"
                        className="text-red-600 border-red-600 hover:bg-red-50"
                        onClick={() => handleRejectMCR(selectedMCR.id)}
                      >
                        <X className="h-4 w-4 mr-2" />
                        Reject | ငြင်းပယ်
                      </Button>
                      <Button
                        className="bg-green-600 hover:bg-green-700 text-white"
                        onClick={() => handleApproveMCR(selectedMCR.id)}
                      >
                        <Check className="h-4 w-4 mr-2" />
                        Approve | အတည်ပြု
                      </Button>
                    </>
                  )}
                </div>
              </div>
            )}
          </DialogContent>
        </Dialog>
      </div>
    </div>
  );
}