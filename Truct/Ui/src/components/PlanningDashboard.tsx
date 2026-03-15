import React, { useState } from 'react';
import { JobScheduleProvider } from './JobScheduleContext';

interface PlanningDashboardProps {
  currentPage?: string;
  onPageChange?: (page: string) => void;
  onMachineSelect?: (machineId: string) => void;
}

function PlanningDashboard({ currentPage, onPageChange, onMachineSelect }: PlanningDashboardProps) {
  const handleMachineSelect = (machineId: string) => {
    if (onMachineSelect) {
      onMachineSelect(machineId);
    }
  };

  return (
    <JobScheduleProvider>
      <PlanningMachineOverviewDashboard 
        onMachineSelect={handleMachineSelect}
      />
    </JobScheduleProvider>
  );
}

// Machine overview dashboard for planning - similar to production overview
import { Card, CardHeader, CardContent } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Progress } from './ui/progress';
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from './ui/dropdown-menu';
import { ImageWithFallback } from './figma/ImageWithFallback';
import { MoreVertical, Calendar, ExternalLink, Clock, User, Target, Activity, Package, Factory } from 'lucide-react';

interface Machine {
  id: string;
  name: string;
  nameMyanmar: string;
  planningStatus: 'fully-planned' | 'partially-planned' | 'unplanned' | 'overbooked';
  scheduledPlans?: {
    draft: number;
    final: number;
    approved: number;
    running: number;
    completed: number;
  };
  plannedCapacity: number;
  utilizationPercent: number;
  nextAvailableSlot?: string;
  plannedHours: number;
  totalPlans: number;
  location: string;
  lastUpdate: string;
}

interface PlanningMachineOverviewDashboardProps {
  onMachineSelect: (machineId: string) => void;
  onOpenInNewTab?: (machineId: string) => void;
}

const mockPlanningMachines: Machine[] = [
  {
    id: 'IM-001',
    name: 'Injection Machine 1',
    nameMyanmar: 'ထောက်ပံ့စက် ၁',
    planningStatus: 'fully-planned',
    scheduledPlans: {
      draft: 2,
      final: 3,
      approved: 2,
      running: 1,
      completed: 4
    },
    plannedCapacity: 1200,
    utilizationPercent: 85,
    nextAvailableSlot: '14:00',
    plannedHours: 20.4,
    totalPlans: 12,
    location: 'Production Floor A',
    lastUpdate: '2 minutes ago'
  },
  {
    id: 'IM-002',
    name: 'Injection Machine 2',
    nameMyanmar: 'ထောက်ပံ့စက် ၂',
    planningStatus: 'partially-planned',
    scheduledPlans: {
      draft: 1,
      final: 2,
      approved: 1,
      running: 0,
      completed: 2
    },
    plannedCapacity: 1500,
    utilizationPercent: 45,
    nextAvailableSlot: '09:00',
    plannedHours: 10.8,
    totalPlans: 6,
    location: 'Production Floor A',
    lastUpdate: '5 minutes ago'
  },
  {
    id: 'IM-003',
    name: 'Injection Machine 3',
    nameMyanmar: 'ထောက်ပံ့စက် ၃',
    planningStatus: 'overbooked',
    scheduledPlans: {
      draft: 3,
      final: 4,
      approved: 3,
      running: 2,
      completed: 3
    },
    plannedCapacity: 800,
    utilizationPercent: 110,
    nextAvailableSlot: 'Tomorrow 08:00',
    plannedHours: 26.4,
    totalPlans: 15,
    location: 'Production Floor B',
    lastUpdate: '1 minute ago'
  },
  {
    id: 'IM-004',
    name: 'Injection Machine 4',
    nameMyanmar: 'ထောက်ပံ့စက် ၄',
    planningStatus: 'unplanned',
    scheduledPlans: {
      draft: 0,
      final: 0,
      approved: 0,
      running: 0,
      completed: 1
    },
    plannedCapacity: 1000,
    utilizationPercent: 0,
    nextAvailableSlot: '08:00',
    plannedHours: 0,
    totalPlans: 1,
    location: 'Production Floor B',
    lastUpdate: '15 minutes ago'
  },
  {
    id: 'IM-005',
    name: 'Injection Machine 5',
    nameMyanmar: 'ထောက်ပံ့စက် ၅',
    planningStatus: 'partially-planned',
    scheduledPlans: {
      draft: 1,
      final: 1,
      approved: 2,
      running: 1,
      completed: 2
    },
    plannedCapacity: 1300,
    utilizationPercent: 65,
    plannedHours: 15.6,
    totalPlans: 7,
    location: 'Production Floor C',
    lastUpdate: '8 minutes ago'
  },
  {
    id: 'IM-006',
    name: 'Injection Machine 6',
    nameMyanmar: 'ထောက်ပံ့စက် ၆',
    planningStatus: 'fully-planned',
    scheduledPlans: {
      draft: 2,
      final: 2,
      approved: 3,
      running: 1,
      completed: 5
    },
    plannedCapacity: 2000,
    utilizationPercent: 90,
    nextAvailableSlot: '16:30',
    plannedHours: 21.6,
    totalPlans: 13,
    location: 'Production Floor C',
    lastUpdate: '3 minutes ago'
  }
];

export function PlanningMachineOverviewDashboard({ onMachineSelect, onOpenInNewTab }: PlanningMachineOverviewDashboardProps) {
  const [viewMode, setViewMode] = useState<'grid' | 'list'>('grid');
  const [filterStatus, setFilterStatus] = useState<string>('all');

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'fully-planned':
        return 'bg-green-500';
      case 'partially-planned':
        return 'bg-yellow-500';
      case 'unplanned':
        return 'bg-red-500';
      case 'overbooked':
        return 'bg-orange-500';
      default:
        return 'bg-gray-500';
    }
  };

  const getStatusIcon = (status: string) => {
    switch (status) {
      case 'fully-planned':
        return <Calendar className="h-4 w-4" />;
      case 'partially-planned':
        return <Clock className="h-4 w-4" />;
      case 'unplanned':
        return <Target className="h-4 w-4" />;
      case 'overbooked':
        return <Activity className="h-4 w-4" />;
      default:
        return <Package className="h-4 w-4" />;
    }
  };

  const getStatusLabel = (status: string) => {
    switch (status) {
      case 'fully-planned':
        return 'FULLY PLANNED';
      case 'partially-planned':
        return 'PARTIAL PLAN';
      case 'unplanned':
        return 'NO PLANS';
      case 'overbooked':
        return 'OVERBOOKED';
      default:
        return status.toUpperCase();
    }
  };

  const filteredMachines = mockPlanningMachines.filter(machine => 
    filterStatus === 'all' || machine.planningStatus === filterStatus
  );

  const handleMachineClick = (machineId: string) => {
    onMachineSelect(machineId);
  };

  const handleOpenInNewTab = (machineId: string, e: React.MouseEvent) => {
    e.stopPropagation();
    if (onOpenInNewTab) {
      onOpenInNewTab(machineId);
    }
  };

  return (
    <div className="p-6 bg-slate-50 min-h-screen space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-semibold text-slate-900">Planning Dashboard</h1>
          <p className="text-slate-600 mt-1">စက်ပစ္စည်းများ အစီအစဥ်ခြုံငုံကြည့်ရှုရန်</p>
        </div>
        
        {/* Controls */}
        <div className="flex items-center gap-4">
          {/* Status Filter */}
          <div className="flex items-center gap-2">
            <Button
              variant={filterStatus === 'all' ? 'default' : 'outline'}
              size="sm"
              onClick={() => setFilterStatus('all')}
            >
              All ({mockPlanningMachines.length})
            </Button>
            <Button
              variant={filterStatus === 'fully-planned' ? 'default' : 'outline'}
              size="sm"
              onClick={() => setFilterStatus('fully-planned')}
              className="text-green-700"
            >
              🟢 Fully Planned ({mockPlanningMachines.filter(m => m.planningStatus === 'fully-planned').length})
            </Button>
            <Button
              variant={filterStatus === 'partially-planned' ? 'default' : 'outline'}
              size="sm"
              onClick={() => setFilterStatus('partially-planned')}
              className="text-yellow-700"
            >
              🟡 Partial ({mockPlanningMachines.filter(m => m.planningStatus === 'partially-planned').length})
            </Button>
            <Button
              variant={filterStatus === 'unplanned' ? 'default' : 'outline'}
              size="sm"
              onClick={() => setFilterStatus('unplanned')}
              className="text-red-700"
            >
              🔴 Unplanned ({mockPlanningMachines.filter(m => m.planningStatus === 'unplanned').length})
            </Button>
            <Button
              variant={filterStatus === 'overbooked' ? 'default' : 'outline'}
              size="sm"
              onClick={() => setFilterStatus('overbooked')}
              className="text-orange-700"
            >
              🟠 Overbooked ({mockPlanningMachines.filter(m => m.planningStatus === 'overbooked').length})
            </Button>
          </div>

          {/* View Toggle */}
          <div className="flex items-center border rounded-lg">
            <Button
              variant={viewMode === 'grid' ? 'default' : 'ghost'}
              size="sm"
              onClick={() => setViewMode('grid')}
              className="rounded-r-none"
            >
              Grid
            </Button>
            <Button
              variant={viewMode === 'list' ? 'default' : 'ghost'}
              size="sm"
              onClick={() => setViewMode('list')}
              className="rounded-l-none"
            >
              List
            </Button>
          </div>
        </div>
      </div>

      {/* Machine Cards */}
      <div className={`grid gap-6 ${
        viewMode === 'grid' 
          ? 'grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4' 
          : 'grid-cols-1'
      }`}>
        {filteredMachines.map((machine) => (
          <Card 
            key={machine.id} 
            className={`cursor-pointer transition-all duration-300 hover:shadow-lg hover:scale-[1.02] border-2 ${
              machine.planningStatus === 'fully-planned' ? 'border-green-200 bg-green-50' :
              machine.planningStatus === 'partially-planned' ? 'border-yellow-200 bg-yellow-50' :
              machine.planningStatus === 'unplanned' ? 'border-red-200 bg-red-50' :
              'border-orange-200 bg-orange-50'
            }`}
            onClick={() => handleMachineClick(machine.id)}
          >
            <CardHeader className="pb-3">
              <div className="flex items-start justify-between">
                <div className="space-y-1">
                  <div className="flex items-center gap-2">
                    <div className={`w-3 h-3 rounded-full ${getStatusColor(machine.planningStatus)} ${
                      machine.planningStatus === 'overbooked' ? 'animate-pulse' : ''
                    }`}></div>
                    <h3 className="font-bold text-lg text-slate-900">{machine.name}</h3>
                  </div>
                  <p className="text-sm text-slate-600">{machine.nameMyanmar}</p>
                  <Badge className={`text-xs ${
                    machine.planningStatus === 'fully-planned' ? 'bg-green-500 text-white' :
                    machine.planningStatus === 'partially-planned' ? 'bg-yellow-500 text-white' :
                    machine.planningStatus === 'unplanned' ? 'bg-red-500 text-white' :
                    'bg-orange-500 text-white'
                  }`}>
                    {getStatusIcon(machine.planningStatus)}
                    <span className="ml-1">{getStatusLabel(machine.planningStatus)}</span>
                  </Badge>
                </div>

                {/* Actions Menu */}
                <DropdownMenu>
                  <DropdownMenuTrigger asChild>
                    <Button variant="ghost" size="sm" className="h-8 w-8 p-0">
                      <MoreVertical className="h-4 w-4" />
                    </Button>
                  </DropdownMenuTrigger>
                  <DropdownMenuContent align="end">
                    <DropdownMenuItem onClick={() => handleMachineClick(machine.id)}>
                      <Calendar className="h-4 w-4 mr-2" />
                      View Planning Calendar
                    </DropdownMenuItem>
                    <DropdownMenuItem onClick={(e) => handleOpenInNewTab(machine.id, e)}>
                      <ExternalLink className="h-4 w-4 mr-2" />
                      Open in New Tab
                    </DropdownMenuItem>
                  </DropdownMenuContent>
                </DropdownMenu>
              </div>
            </CardHeader>

            <CardContent className="space-y-4">
              {/* Planning Overview */}
              <div className="space-y-3">
                <div className="flex items-center gap-3">
                  <Factory className="w-8 h-8 text-blue-600" />
                  <div className="flex-1">
                    <h4 className="font-semibold text-slate-900">
                      {machine.totalPlans} Total Plans
                    </h4>
                    <div className="flex items-center gap-2 text-sm text-slate-600">
                      <Clock className="h-3 w-3" />
                      <span>{machine.plannedHours}h scheduled</span>
                      {machine.nextAvailableSlot && (
                        <span>• Next: {machine.nextAvailableSlot}</span>
                      )}
                    </div>
                  </div>
                </div>

                {/* Utilization Progress */}
                <div className="space-y-2">
                  <div className="flex justify-between text-sm">
                    <span className="text-slate-600">Utilization:</span>
                    <span className={`font-semibold ${
                      machine.utilizationPercent > 100 ? 'text-orange-600' :
                      machine.utilizationPercent > 80 ? 'text-green-600' :
                      machine.utilizationPercent > 50 ? 'text-yellow-600' :
                      'text-red-600'
                    }`}>
                      {machine.utilizationPercent}%
                    </span>
                  </div>
                  <Progress 
                    value={Math.min(machine.utilizationPercent, 100)} 
                    className="h-3"
                  />
                  {machine.utilizationPercent > 100 && (
                    <div className="text-right">
                      <span className="text-xs font-bold text-orange-600">
                        Overbooked by {machine.utilizationPercent - 100}%
                      </span>
                    </div>
                  )}
                </div>
              </div>

              {/* Plan Status Breakdown */}
              {machine.scheduledPlans && (
                <div className="space-y-3">
                  <div className="text-sm font-medium text-slate-700">Plan Status Breakdown:</div>
                  <div className="grid grid-cols-2 gap-2 text-xs">
                    <div className="flex items-center gap-1">
                      <div className="w-2 h-2 rounded-full bg-blue-500"></div>
                      <span>Draft: {machine.scheduledPlans.draft}</span>
                    </div>
                    <div className="flex items-center gap-1">
                      <div className="w-2 h-2 rounded-full bg-yellow-500"></div>
                      <span>Final: {machine.scheduledPlans.final}</span>
                    </div>
                    <div className="flex items-center gap-1">
                      <div className="w-2 h-2 rounded-full bg-orange-500"></div>
                      <span>Approved: {machine.scheduledPlans.approved}</span>
                    </div>
                    <div className="flex items-center gap-1">
                      <div className="w-2 h-2 rounded-full bg-green-500"></div>
                      <span>Running: {machine.scheduledPlans.running}</span>
                    </div>
                  </div>
                </div>
              )}

              {/* Machine Stats */}
              <div className="grid grid-cols-3 gap-3 pt-2 border-t">
                <div className="text-center">
                  <div className="font-bold text-blue-600">{machine.plannedCapacity}</div>
                  <div className="text-xs text-slate-600">pcs/hr</div>
                </div>
                <div className="text-center">
                  <div className="font-bold text-purple-600">{machine.totalPlans}</div>
                  <div className="text-xs text-slate-600">Plans</div>
                </div>
                <div className="text-center">
                  <div className="font-bold text-green-600">{machine.plannedHours}h</div>
                  <div className="text-xs text-slate-600">Scheduled</div>
                </div>
              </div>

              {/* Last Update */}
              <div className="text-xs text-slate-500 text-center pt-2 border-t">
                Last updated: {machine.lastUpdate}
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      {/* Empty State */}
      {filteredMachines.length === 0 && (
        <div className="text-center py-12">
          <div className="text-slate-400 mb-4">
            <Factory className="h-16 w-16 mx-auto" />
          </div>
          <h3 className="text-lg font-semibold text-slate-900 mb-2">
            No machines found
          </h3>
          <p className="text-slate-600">
            No machines match the selected planning status filter.
          </p>
        </div>
      )}
    </div>
  );
}

export default PlanningDashboard;