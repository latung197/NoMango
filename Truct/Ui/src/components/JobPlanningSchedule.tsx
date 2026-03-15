import React, { useState, useEffect } from 'react';
import { Calendar, Clock, Search, Filter, Plus, Edit3, X, Save, Trash2, Play, Square, CheckCircle, AlertTriangle, Wrench, MoreHorizontal } from 'lucide-react';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Badge } from './ui/badge';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Drawer, DrawerContent, DrawerHeader, DrawerTitle, DrawerTrigger } from './ui/drawer';
import { Label } from './ui/label';
import { Tabs, TabsContent, TabsList, TabsTrigger } from './ui/tabs';
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from './ui/dropdown-menu';
import { Separator } from './ui/separator';
import { toast } from 'sonner';

// Mock data
const mockMachines = [
  { id: 'MCH-01', name: 'Machine 01', line: 'Line A' },
  { id: 'MCH-02', name: 'Machine 02', line: 'Line A' },
  { id: 'MCH-03', name: 'Machine 03', line: 'Line B' },
  { id: 'MCH-04', name: 'Machine 04', line: 'Line B' },
  { id: 'MCH-05', name: 'Extrusion 01', line: 'Extrusion' },
  { id: 'MCH-06', name: 'Cutting 01', line: 'Cutting' }
];

const mockApprovedJobs = [
  {
    id: 'JOB-001',
    productName: 'Plastic Container 500ml',
    qty: 1000,
    moldNo: 'MLD-A001',
    color: 'Blue',
    plannedDuration: 4,
    targetPerHour: 250,
    materialStatus: 'Ready'
  },
  {
    id: 'JOB-002',
    productName: 'Bottle Cap Standard',
    qty: 5000,
    moldNo: 'MLD-B003',
    color: 'White',
    plannedDuration: 6,
    targetPerHour: 833,
    materialStatus: 'Low'
  },
  {
    id: 'JOB-003',
    productName: 'Storage Box Large',
    qty: 200,
    moldNo: 'MLD-C007',
    color: 'Green',
    plannedDuration: 8,
    targetPerHour: 25,
    materialStatus: 'Not Reserved'
  }
];

const mockJobSlots = [
  {
    id: 1,
    machineId: 'MCH-01',
    jobId: 'JOB-001',
    job: mockApprovedJobs[0],
    startTime: 8,
    endTime: 12,
    status: 'Running',
    materialStatus: 'Ready',
    createdBy: 'Planner A',
    updatedBy: 'Planner A'
  },
  {
    id: 2,
    machineId: 'MCH-02',
    jobId: 'JOB-002',
    job: mockApprovedJobs[1],
    startTime: 14,
    endTime: 20,
    status: 'Scheduled',
    materialStatus: 'Low',
    createdBy: 'Planner B',
    updatedBy: 'Planner B'
  },
  {
    id: 3,
    machineId: 'MCH-01',
    startTime: 16,
    endTime: 18,
    status: 'Maintenance',
    materialStatus: null,
    createdBy: 'System',
    updatedBy: 'System'
  }
];

interface JobSlot {
  id?: number;
  machineId: string;
  jobId?: string;
  job?: any;
  startTime: number;
  endTime: number;
  status: 'Idle' | 'Scheduled' | 'Running' | 'Completed' | 'Maintenance';
  materialStatus?: 'Ready' | 'Low' | 'Not Reserved' | null;
  createdBy?: string;
  updatedBy?: string;
}

export function JobPlanningSchedule() {
  const [viewMode, setViewMode] = useState<'day' | 'week'>('day');
  const [currentDate, setCurrentDate] = useState(new Date());
  const [selectedMachineGroup, setSelectedMachineGroup] = useState('All');
  const [statusFilter, setStatusFilter] = useState<string[]>([]);
  const [materialFilter, setMaterialFilter] = useState('All');
  const [searchTerm, setSearchTerm] = useState('');
  const [jobSlots, setJobSlots] = useState<JobSlot[]>(mockJobSlots);
  
  // Drawer states
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [drawerMode, setDrawerMode] = useState<'create' | 'details'>('create');
  const [selectedSlot, setSelectedSlot] = useState<JobSlot | null>(null);
  const [selectedMachine, setSelectedMachine] = useState('');
  const [selectedStartTime, setSelectedStartTime] = useState(8);
  
  // Form states
  const [formData, setFormData] = useState({
    machineId: '',
    startTime: 8,
    endTime: 12,
    jobId: '',
    job: null as any
  });

  const hours = Array.from({ length: 24 }, (_, i) => i);
  
  const filteredMachines = mockMachines.filter(machine => {
    if (selectedMachineGroup === 'All') return true;
    if (selectedMachineGroup === 'Line A') return machine.line === 'Line A';
    if (selectedMachineGroup === 'Line B') return machine.line === 'Line B';
    if (selectedMachineGroup === 'Extrusion') return machine.line === 'Extrusion';
    if (selectedMachineGroup === 'Cutting') return machine.line === 'Cutting';
    return true;
  });

  const getSlotForMachineAndTime = (machineId: string, hour: number) => {
    return jobSlots.find(slot => 
      slot.machineId === machineId && 
      hour >= slot.startTime && 
      hour < slot.endTime
    );
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Idle': return 'bg-white border border-slate-200';
      case 'Scheduled': return 'bg-blue-100 border border-blue-300 text-blue-800';
      case 'Running': return 'bg-green-100 border border-green-300 text-green-800';
      case 'Completed': return 'bg-gray-100 border border-gray-300 text-gray-600';
      case 'Maintenance': return 'bg-orange-100 border border-orange-300 text-orange-800';
      default: return 'bg-white border border-slate-200';
    }
  };

  const getMaterialStatusBadge = (status: string | null) => {
    if (!status) return null;
    switch (status) {
      case 'Ready': return <Badge variant="secondary" className="bg-green-100 text-green-800 text-xs">✅ Ready</Badge>;
      case 'Low': return <Badge variant="secondary" className="bg-yellow-100 text-yellow-800 text-xs">⚠ Low</Badge>;
      case 'Not Reserved': return <Badge variant="secondary" className="bg-red-100 text-red-800 text-xs">❌ Not Reserved</Badge>;
      default: return null;
    }
  };

  const handleSlotClick = (machineId: string, hour: number) => {
    const existingSlot = getSlotForMachineAndTime(machineId, hour);
    
    if (existingSlot) {
      // Open details drawer
      setSelectedSlot(existingSlot);
      setDrawerMode('details');
      setDrawerOpen(true);
    } else {
      // Open create drawer
      setSelectedMachine(machineId);
      setSelectedStartTime(hour);
      setFormData({
        machineId,
        startTime: hour,
        endTime: hour + 4,
        jobId: '',
        job: null
      });
      setDrawerMode('create');
      setDrawerOpen(true);
    }
  };

  const handleJobSelect = (jobId: string) => {
    const selectedJob = mockApprovedJobs.find(job => job.id === jobId);
    if (selectedJob) {
      setFormData(prev => ({
        ...prev,
        jobId,
        job: selectedJob,
        endTime: prev.startTime + selectedJob.plannedDuration
      }));
    }
  };

  const handleSave = () => {
    if (!formData.jobId) {
      toast.error('Please select a job');
      return;
    }

    // Check for conflicts
    const hasConflict = jobSlots.some(slot => 
      slot.machineId === formData.machineId && 
      slot.id !== selectedSlot?.id &&
      ((formData.startTime >= slot.startTime && formData.startTime < slot.endTime) ||
       (formData.endTime > slot.startTime && formData.endTime <= slot.endTime) ||
       (formData.startTime <= slot.startTime && formData.endTime >= slot.endTime))
    );

    if (hasConflict) {
      toast.error('Time slot conflicts with existing schedule', {
        description: 'မြန်မာ: ဤအချိန်တွင် မစီစဉ်နိုင်ပါ။'
      });
      return;
    }

    const newSlot: JobSlot = {
      id: selectedSlot?.id || Date.now(),
      machineId: formData.machineId,
      jobId: formData.jobId,
      job: formData.job,
      startTime: formData.startTime,
      endTime: formData.endTime,
      status: 'Scheduled',
      materialStatus: formData.job?.materialStatus || 'Ready',
      createdBy: 'Current User',
      updatedBy: 'Current User'
    };

    if (selectedSlot) {
      // Update existing
      setJobSlots(prev => prev.map(slot => slot.id === selectedSlot.id ? newSlot : slot));
      toast.success('Job slot updated successfully');
    } else {
      // Create new
      setJobSlots(prev => [...prev, newSlot]);
      toast.success('Job slot created successfully');
    }

    setDrawerOpen(false);
    setSelectedSlot(null);
  };

  const handleDelete = () => {
    if (selectedSlot) {
      setJobSlots(prev => prev.filter(slot => slot.id !== selectedSlot.id));
      toast.success('Job slot deleted successfully');
      setDrawerOpen(false);
      setSelectedSlot(null);
    }
  };

  const handleStatusChange = (newStatus: string) => {
    if (selectedSlot) {
      const updatedSlot = { ...selectedSlot, status: newStatus as any };
      setJobSlots(prev => prev.map(slot => slot.id === selectedSlot.id ? updatedSlot : slot));
      setSelectedSlot(updatedSlot);
      toast.success(`Status changed to ${newStatus}`);
    }
  };

  return (
    <div className="p-6 bg-slate-50 min-h-screen">
      <div className="max-w-full mx-auto">
        {/* Header */}
        <div className="bg-white rounded-lg shadow-sm p-6 mb-6">
          <div className="flex items-center justify-between mb-4">
            <div>
              <h1 className="text-2xl font-semibold text-slate-900 flex items-center gap-2">
                <Calendar className="h-6 w-6 text-blue-600" />
                Job Planning Schedule | အလုပ်စီမံဇယား
              </h1>
              <p className="text-slate-600">Production planning and machine scheduling</p>
            </div>
            <div className="flex items-center gap-2">
              <Tabs value={viewMode} onValueChange={(value) => setViewMode(value as 'day' | 'week')}>
                <TabsList>
                  <TabsTrigger value="day">Day</TabsTrigger>
                  <TabsTrigger value="week">Week</TabsTrigger>
                </TabsList>
              </Tabs>
            </div>
          </div>

          {/* Filters */}
          <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
            <div>
              <Label>Machine Group | စက်နံပါတ်</Label>
              <Select value={selectedMachineGroup} onValueChange={setSelectedMachineGroup}>
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="All">All Machines</SelectItem>
                  <SelectItem value="Line A">Line A</SelectItem>
                  <SelectItem value="Line B">Line B</SelectItem>
                  <SelectItem value="Extrusion">Extrusion</SelectItem>
                  <SelectItem value="Cutting">Cutting</SelectItem>
                </SelectContent>
              </Select>
            </div>

            <div>
              <Label>Material Status | ပစ္စည်းအခြေအနေ</Label>
              <Select value={materialFilter} onValueChange={setMaterialFilter}>
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="All">All Status</SelectItem>
                  <SelectItem value="Ready">Ready | လက်ရှိရှိ</SelectItem>
                  <SelectItem value="Low">Low | နည်းပါး</SelectItem>
                  <SelectItem value="Not Reserved">Not Reserved | မသတ်မှတ်ရသေး</SelectItem>
                </SelectContent>
              </Select>
            </div>

            <div>
              <Label>Quick Search | ရှာဖွေမှု</Label>
              <div className="relative">
                <Search className="h-4 w-4 absolute left-3 top-3 text-slate-400" />
                <Input
                  placeholder="Job ID, Product, Mold..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="pl-10"
                />
              </div>
            </div>

            <div className="flex items-end gap-2">
              <Button variant="outline" size="icon">
                <Filter className="h-4 w-4" />
              </Button>
            </div>
          </div>
        </div>

        {/* Main Schedule Grid */}
        <div className="bg-white rounded-lg shadow-sm overflow-hidden">
          <div className="p-4 border-b border-slate-200">
            <h3 className="font-medium text-slate-900">Machine Schedule Grid</h3>
            <p className="text-sm text-slate-600">Click on empty slots to plan, click on existing jobs to view details</p>
          </div>

          <div className="overflow-x-auto">
            <div className="min-w-[1200px]">
              {/* Time Header */}
              <div className="flex border-b border-slate-200 bg-slate-50">
                <div className="w-48 p-2 font-medium text-sm text-slate-900 border-r border-slate-200 flex-shrink-0">
                  Machine | စက်
                </div>
                <div className="flex-1 grid grid-cols-24 gap-0">
                  {hours.map(hour => (
                    <div key={hour} className="p-2 text-center text-xs font-medium text-slate-600 border-r border-slate-200">
                      {hour.toString().padStart(2, '0')}:00
                    </div>
                  ))}
                </div>
              </div>

              {/* Machine Rows */}
              {filteredMachines.map(machine => (
                <div key={machine.id} className="flex border-b border-slate-200">
                  <div className="w-48 p-3 border-r border-slate-200 bg-slate-50 flex-shrink-0">
                    <div className="font-medium text-sm text-slate-900">{machine.name}</div>
                    <div className="text-xs text-slate-500">{machine.line}</div>
                  </div>
                  
                  <div className="flex-1 grid grid-cols-24 gap-0">
                    {hours.map(hour => {
                      const slot = getSlotForMachineAndTime(machine.id, hour);
                      const isSlotStart = slot && slot.startTime === hour;
                      const isSlotContinuation = slot && slot.startTime < hour;
                      
                      if (isSlotContinuation && !isSlotStart) {
                        return null; // Skip continuation cells
                      }
                      
                      const cellSpan = slot ? slot.endTime - slot.startTime : 1;
                      
                      return (
                        <div
                          key={hour}
                          className={`relative border-r border-slate-200 min-h-[60px] cursor-pointer hover:bg-slate-50 transition-colors ${
                            slot ? getStatusColor(slot.status) : 'bg-white'
                          }`}
                          style={{ gridColumn: `span ${cellSpan}` }}
                          onClick={() => handleSlotClick(machine.id, hour)}
                        >
                          {slot ? (
                            <div className="p-2 h-full">
                              <div className="text-xs font-medium truncate">
                                {slot.job ? slot.job.productName : (slot.status === 'Maintenance' ? 'Maintenance' : 'Unknown')}
                              </div>
                              {slot.job && (
                                <>
                                  <div className="text-xs text-slate-600 truncate">
                                    {slot.jobId} • {slot.job.qty} pcs
                                  </div>
                                  <div className="mt-1">
                                    {getMaterialStatusBadge(slot.materialStatus)}
                                  </div>
                                </>
                              )}
                              {slot.status === 'Maintenance' && (
                                <div className="flex items-center text-xs text-orange-600 mt-1">
                                  <Wrench className="h-3 w-3 mr-1" />
                                  Maintenance
                                </div>
                              )}
                            </div>
                          ) : (
                            <div className="p-2 h-full flex items-center justify-center text-xs text-slate-400">
                              <Plus className="h-3 w-3" />
                            </div>
                          )}
                        </div>
                      );
                    })}
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>

        {/* Legend */}
        <div className="mt-4 bg-white rounded-lg shadow-sm p-4">
          <h4 className="font-medium text-slate-900 mb-3">Legend | အဓိပ္ပာယ်ဖွင့်ဆိုချက်</h4>
          <div className="grid grid-cols-2 md:grid-cols-5 gap-4">
            <div className="flex items-center gap-2">
              <div className="w-4 h-4 bg-white border border-slate-200 rounded"></div>
              <span className="text-sm">Idle | အားလပ်</span>
            </div>
            <div className="flex items-center gap-2">
              <div className="w-4 h-4 bg-blue-100 border border-blue-300 rounded"></div>
              <span className="text-sm">Scheduled | စီစဉ်ပြီး</span>
            </div>
            <div className="flex items-center gap-2">
              <div className="w-4 h-4 bg-green-100 border border-green-300 rounded"></div>
              <span className="text-sm">Running | လုပ်ဆောင်နေသည်</span>
            </div>
            <div className="flex items-center gap-2">
              <div className="w-4 h-4 bg-gray-100 border border-gray-300 rounded"></div>
              <span className="text-sm">Completed | ပြီးဆုံး</span>
            </div>
            <div className="flex items-center gap-2">
              <div className="w-4 h-4 bg-orange-100 border border-orange-300 rounded"></div>
              <span className="text-sm">Maintenance | ပြုပြင်ထိန်းသိမ်းမှု</span>
            </div>
          </div>
        </div>

        {/* Job Slot Drawer */}
        <Drawer open={drawerOpen} onOpenChange={setDrawerOpen}>
          <DrawerContent className="max-w-md mx-auto">
            <DrawerHeader>
              <DrawerTitle>
                {drawerMode === 'create' ? 'Create Job Slot | အလုပ်အချိန်ဖန်တီး' : 'Job Slot Details | အလုပ်အချိန်အသေးစိတ်'}
              </DrawerTitle>
            </DrawerHeader>
            
            <div className="p-4 space-y-4">
              {drawerMode === 'create' ? (
                // Create Mode
                <>
                  <div>
                    <Label>Machine | စက်နံပါတ်</Label>
                    <Input value={formData.machineId} disabled className="bg-slate-50" />
                  </div>

                  <div className="grid grid-cols-2 gap-4">
                    <div>
                      <Label>Start Time | စတင်ချိန်</Label>
                      <Select value={formData.startTime.toString()} onValueChange={(value) => 
                        setFormData(prev => ({ ...prev, startTime: parseInt(value) }))
                      }>
                        <SelectTrigger>
                          <SelectValue />
                        </SelectTrigger>
                        <SelectContent>
                          {hours.map(hour => (
                            <SelectItem key={hour} value={hour.toString()}>
                              {hour.toString().padStart(2, '0')}:00
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                    </div>

                    <div>
                      <Label>End Time | ပြီးချိန်</Label>
                      <Select value={formData.endTime.toString()} onValueChange={(value) => 
                        setFormData(prev => ({ ...prev, endTime: parseInt(value) }))
                      }>
                        <SelectTrigger>
                          <SelectValue />
                        </SelectTrigger>
                        <SelectContent>
                          {hours.filter(h => h > formData.startTime).map(hour => (
                            <SelectItem key={hour} value={hour.toString()}>
                              {hour.toString().padStart(2, '0')}:00
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                    </div>
                  </div>

                  <div>
                    <Label>Job ID | အလုပ်အမှတ်</Label>
                    <Select value={formData.jobId} onValueChange={handleJobSelect}>
                      <SelectTrigger>
                        <SelectValue placeholder="Select approved job..." />
                      </SelectTrigger>
                      <SelectContent>
                        {mockApprovedJobs.map(job => (
                          <SelectItem key={job.id} value={job.id}>
                            <div>
                              <div className="font-medium">{job.id}</div>
                              <div className="text-xs text-slate-500">{job.productName}</div>
                            </div>
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>

                  {formData.job && (
                    <Card>
                      <CardHeader className="pb-2">
                        <CardTitle className="text-sm">Job Details</CardTitle>
                      </CardHeader>
                      <CardContent className="space-y-2">
                        <div className="grid grid-cols-2 gap-2 text-sm">
                          <div>Product: {formData.job.productName}</div>
                          <div>Qty: {formData.job.qty} pcs</div>
                          <div>Mold: {formData.job.moldNo}</div>
                          <div>Color: {formData.job.color}</div>
                          <div>Duration: {formData.job.plannedDuration}h</div>
                          <div>Target: {formData.job.targetPerHour}/h</div>
                        </div>
                        <div className="pt-2">
                          <Label>Material Status | ပစ္စည်းအခြေအနေ</Label>
                          <div className="mt-1">
                            {getMaterialStatusBadge(formData.job.materialStatus)}
                          </div>
                        </div>
                      </CardContent>
                    </Card>
                  )}

                  <div className="flex gap-2 pt-4">
                    <Button onClick={handleSave} className="flex-1">
                      <Save className="h-4 w-4 mr-2" />
                      Save | သိမ်းဆည်း
                    </Button>
                    <Button variant="outline" onClick={() => setDrawerOpen(false)}>
                      Cancel | ပယ်ဖျက်
                    </Button>
                  </div>
                </>
              ) : (
                // Details Mode
                selectedSlot && (
                  <>
                    <div className="space-y-3">
                      <div className="flex items-center justify-between">
                        <h4 className="font-medium">Job Slot Information</h4>
                        <Badge variant={selectedSlot.status === 'Running' ? 'default' : 'secondary'}>
                          {selectedSlot.status}
                        </Badge>
                      </div>

                      <div className="grid grid-cols-2 gap-4 text-sm">
                        <div>
                          <Label>Machine | စက်</Label>
                          <div className="mt-1">{selectedSlot.machineId}</div>
                        </div>
                        <div>
                          <Label>Time | အချိน်</Label>
                          <div className="mt-1">
                            {selectedSlot.startTime.toString().padStart(2, '0')}:00 - {selectedSlot.endTime.toString().padStart(2, '0')}:00
                          </div>
                        </div>
                      </div>

                      {selectedSlot.job && (
                        <Card>
                          <CardHeader className="pb-2">
                            <CardTitle className="text-sm flex items-center justify-between">
                              Job Details
                              {getMaterialStatusBadge(selectedSlot.materialStatus)}
                            </CardTitle>
                          </CardHeader>
                          <CardContent className="space-y-2">
                            <div className="grid grid-cols-2 gap-2 text-sm">
                              <div><strong>Job ID:</strong> {selectedSlot.jobId}</div>
                              <div><strong>Product:</strong> {selectedSlot.job.productName}</div>
                              <div><strong>Qty:</strong> {selectedSlot.job.qty} pcs</div>
                              <div><strong>Mold:</strong> {selectedSlot.job.moldNo}</div>
                              <div><strong>Color:</strong> {selectedSlot.job.color}</div>
                              <div><strong>Target:</strong> {selectedSlot.job.targetPerHour}/h</div>
                            </div>
                          </CardContent>
                        </Card>
                      )}

                      <Separator />

                      <div className="text-xs text-slate-500">
                        <div>Created by: {selectedSlot.createdBy}</div>
                        <div>Updated by: {selectedSlot.updatedBy}</div>
                      </div>
                    </div>

                    <div className="flex gap-2 pt-4">
                      {selectedSlot.status === 'Scheduled' && (
                        <>
                          <DropdownMenu>
                            <DropdownMenuTrigger asChild>
                              <Button variant="outline" size="sm">
                                <Play className="h-4 w-4 mr-2" />
                                Actions
                              </Button>
                            </DropdownMenuTrigger>
                            <DropdownMenuContent>
                              <DropdownMenuItem onClick={() => handleStatusChange('Running')}>
                                <Play className="h-4 w-4 mr-2" />
                                Start Job
                              </DropdownMenuItem>
                              <DropdownMenuItem onClick={() => handleStatusChange('Completed')}>
                                <CheckCircle className="h-4 w-4 mr-2" />
                                Mark Complete
                              </DropdownMenuItem>
                              <DropdownMenuItem onClick={handleDelete} className="text-red-600">
                                <Trash2 className="h-4 w-4 mr-2" />
                                Cancel Schedule
                              </DropdownMenuItem>
                            </DropdownMenuContent>
                          </DropdownMenu>
                        </>
                      )}
                      
                      {selectedSlot.status === 'Running' && (
                        <Button variant="outline" size="sm" onClick={() => handleStatusChange('Completed')}>
                          <Square className="h-4 w-4 mr-2" />
                          Complete Job
                        </Button>
                      )}

                      <Button variant="outline" onClick={() => setDrawerOpen(false)} className="ml-auto">
                        Close | ပိတ်
                      </Button>
                    </div>
                  </>
                )
              )}
            </div>
          </DrawerContent>
        </Drawer>
      </div>
    </div>
  );
}

export default JobPlanningSchedule;