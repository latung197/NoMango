import React, { useState } from 'react';
import { Card, CardHeader, CardContent } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Progress } from './ui/progress';
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from './ui/dropdown-menu';
import { ImageWithFallback } from './figma/ImageWithFallback';
import { MoreVertical, Calendar, ExternalLink, Play, Pause, AlertTriangle, User, Target, Clock, Activity } from 'lucide-react';

interface Machine {
  id: string;
  name: string;
  nameMyanmar: string;
  status: 'running' | 'idle' | 'down' | 'maintenance';
  currentProduct?: {
    id: string;
    name: string;
    image: string;
    targetQuantity: number;
    actualQuantity: number;
    startTime: string;
    estimatedEndTime: string;
  };
  operator?: {
    name: string;
    nameMyanmar: string;
    shift: string;
  };
  efficiency: number;
  capacity: number;
  cycleTime: number;
  location: string;
  lastUpdate: string;
}

interface MachineOverviewDashboardProps {
  onMachineSelect: (machineId: string) => void;
  onOpenInNewTab?: (machineId: string) => void;
}

const mockMachines: Machine[] = [
  {
    id: 'IM-001',
    name: 'Injection Machine 1',
    nameMyanmar: 'ထောက်ပံ့စက် ၁',
    status: 'running',
    currentProduct: {
      id: 'PRD-001',
      name: 'Plastic Cup 16oz Clear',
      image: 'https://images.unsplash.com/photo-1558618047-3c8c76ca7d13?w=100&h=100&fit=crop',
      targetQuantity: 5000,
      actualQuantity: 3200,
      startTime: '08:00',
      estimatedEndTime: '16:30'
    },
    operator: {
      name: 'Thant Zin',
      nameMyanmar: 'သန့်ဇင်',
      shift: 'Day Shift'
    },
    efficiency: 87,
    capacity: 1200,
    cycleTime: 12,
    location: 'Production Floor A',
    lastUpdate: '2 minutes ago'
  },
  {
    id: 'IM-002',
    name: 'Injection Machine 2',
    nameMyanmar: 'ထောက်ပံ့စက် ၂',
    status: 'idle',
    operator: {
      name: 'Mg Aung',
      nameMyanmar: 'မောင်အောင်',
      shift: 'Day Shift'
    },
    efficiency: 92,
    capacity: 1500,
    cycleTime: 10,
    location: 'Production Floor A',
    lastUpdate: '5 minutes ago'
  },
  {
    id: 'IM-003',
    name: 'Injection Machine 3',
    nameMyanmar: 'ထောက်ပံ့စက် ၃',
    status: 'running',
    currentProduct: {
      id: 'PRD-002',
      name: 'Food Container 1L White',
      image: 'https://images.unsplash.com/photo-1584464491033-06628f3a6b7b?w=100&h=100&fit=crop',
      targetQuantity: 2500,
      actualQuantity: 1800,
      startTime: '10:15',
      estimatedEndTime: '18:00'
    },
    operator: {
      name: 'Win Htut',
      nameMyanmar: 'ဝင်းထွဋ်',
      shift: 'Day Shift'
    },
    efficiency: 94,
    capacity: 800,
    cycleTime: 15,
    location: 'Production Floor B',
    lastUpdate: '1 minute ago'
  },
  {
    id: 'IM-004',
    name: 'Injection Machine 4',
    nameMyanmar: 'ထောက်ပံ့စက် ၄',
    status: 'down',
    operator: {
      name: 'Ko Zaw',
      nameMyanmar: 'ကိုဇေါ်',
      shift: 'Day Shift'
    },
    efficiency: 45,
    capacity: 1000,
    cycleTime: 18,
    location: 'Production Floor B',
    lastUpdate: '15 minutes ago'
  },
  {
    id: 'IM-005',
    name: 'Injection Machine 5',
    nameMyanmar: 'ထောက်ပံ့စက် ၅',
    status: 'maintenance',
    efficiency: 0,
    capacity: 1300,
    cycleTime: 14,
    location: 'Production Floor C',
    lastUpdate: '1 hour ago'
  },
  {
    id: 'IM-006',
    name: 'Injection Machine 6',
    nameMyanmar: 'ထောက်ပံ့စက် ၆',
    status: 'running',
    currentProduct: {
      id: 'PRD-003',
      name: 'Bottle Cap 28mm Blue',
      image: 'https://images.unsplash.com/photo-1582719471384-894fbb16e074?w=100&h=100&fit=crop',
      targetQuantity: 10000,
      actualQuantity: 7500,
      startTime: '06:00',
      estimatedEndTime: '14:30'
    },
    operator: {
      name: 'Ma Aye',
      nameMyanmar: 'မအေး',
      shift: 'Day Shift'
    },
    efficiency: 89,
    capacity: 2000,
    cycleTime: 8,
    location: 'Production Floor C',
    lastUpdate: '3 minutes ago'
  }
];

export function MachineOverviewDashboard({ onMachineSelect, onOpenInNewTab }: MachineOverviewDashboardProps) {
  const [viewMode, setViewMode] = useState<'grid' | 'list'>('grid');
  const [filterStatus, setFilterStatus] = useState<string>('all');

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'running':
        return 'bg-green-500';
      case 'idle':
        return 'bg-yellow-500';
      case 'down':
        return 'bg-red-500';
      case 'maintenance':
        return 'bg-orange-500';
      default:
        return 'bg-gray-500';
    }
  };

  const getStatusIcon = (status: string) => {
    switch (status) {
      case 'running':
        return <Play className="h-4 w-4" />;
      case 'idle':
        return <Pause className="h-4 w-4" />;
      case 'down':
        return <AlertTriangle className="h-4 w-4" />;
      case 'maintenance':
        return <Activity className="h-4 w-4" />;
      default:
        return <Pause className="h-4 w-4" />;
    }
  };

  const getProgressPercentage = (machine: Machine) => {
    if (!machine.currentProduct) return 0;
    return Math.round((machine.currentProduct.actualQuantity / machine.currentProduct.targetQuantity) * 100);
  };

  const filteredMachines = mockMachines.filter(machine => 
    filterStatus === 'all' || machine.status === filterStatus
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
          <h1 className="text-2xl font-semibold text-slate-900">Machine Overview Dashboard</h1>
          <p className="text-slate-600 mt-1">စက်ပစ္စည်းများ အခြေအနေ ခြုံငုံကြည့်ရှုရန်</p>
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
              All ({mockMachines.length})
            </Button>
            <Button
              variant={filterStatus === 'running' ? 'default' : 'outline'}
              size="sm"
              onClick={() => setFilterStatus('running')}
              className="text-green-700"
            >
              🟢 Running ({mockMachines.filter(m => m.status === 'running').length})
            </Button>
            <Button
              variant={filterStatus === 'idle' ? 'default' : 'outline'}
              size="sm"
              onClick={() => setFilterStatus('idle')}
              className="text-yellow-700"
            >
              🟡 Idle ({mockMachines.filter(m => m.status === 'idle').length})
            </Button>
            <Button
              variant={filterStatus === 'down' ? 'default' : 'outline'}
              size="sm"
              onClick={() => setFilterStatus('down')}
              className="text-red-700"
            >
              🔴 Down ({mockMachines.filter(m => m.status === 'down').length})
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
              machine.status === 'running' ? 'border-green-200 bg-green-50' :
              machine.status === 'idle' ? 'border-yellow-200 bg-yellow-50' :
              machine.status === 'down' ? 'border-red-200 bg-red-50' :
              'border-orange-200 bg-orange-50'
            }`}
            onClick={() => handleMachineClick(machine.id)}
          >
            <CardHeader className="pb-3">
              <div className="flex items-start justify-between">
                <div className="space-y-1">
                  <div className="flex items-center gap-2">
                    <div className={`w-3 h-3 rounded-full ${getStatusColor(machine.status)} ${
                      machine.status === 'running' ? 'animate-pulse' : ''
                    }`}></div>
                    <h3 className="font-bold text-lg text-slate-900">{machine.name}</h3>
                  </div>
                  <p className="text-sm text-slate-600">{machine.nameMyanmar}</p>
                  <Badge className={`text-xs ${
                    machine.status === 'running' ? 'bg-green-500 text-white' :
                    machine.status === 'idle' ? 'bg-yellow-500 text-white' :
                    machine.status === 'down' ? 'bg-red-500 text-white' :
                    'bg-orange-500 text-white'
                  }`}>
                    {getStatusIcon(machine.status)}
                    <span className="ml-1">{machine.status.toUpperCase()}</span>
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
                      View Calendar
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
              {/* Current Product */}
              {machine.currentProduct ? (
                <div className="space-y-3">
                  <div className="flex items-center gap-3">
                    <ImageWithFallback
                      src={machine.currentProduct.image}
                      alt={machine.currentProduct.name}
                      className="w-12 h-12 rounded-lg object-cover border-2 border-white shadow-sm"
                    />
                    <div className="flex-1 min-w-0">
                      <h4 className="font-semibold text-slate-900 truncate">
                        {machine.currentProduct.name}
                      </h4>
                      <div className="flex items-center gap-2 text-sm text-slate-600">
                        <Clock className="h-3 w-3" />
                        <span>{machine.currentProduct.startTime} - {machine.currentProduct.estimatedEndTime}</span>
                      </div>
                    </div>
                  </div>

                  {/* Progress Bar */}
                  <div className="space-y-2">
                    <div className="flex justify-between text-sm">
                      <span className="text-slate-600">Progress:</span>
                      <span className="font-semibold text-blue-600">
                        {machine.currentProduct.actualQuantity.toLocaleString()} / {machine.currentProduct.targetQuantity.toLocaleString()}
                      </span>
                    </div>
                    <Progress 
                      value={getProgressPercentage(machine)} 
                      className="h-3"
                    />
                    <div className="text-right">
                      <span className="text-xs font-bold text-green-600">
                        {getProgressPercentage(machine)}% Complete
                      </span>
                    </div>
                  </div>
                </div>
              ) : (
                <div className="text-center py-6 text-slate-500">
                  <Target className="h-8 w-8 mx-auto mb-2 opacity-50" />
                  <p className="text-sm">No Active Production</p>
                  <p className="text-xs text-slate-400">စက်မတည်ပြေးနေပါ</p>
                </div>
              )}

              {/* Operator Info */}
              {machine.operator ? (
                <div className="flex items-center gap-2 p-3 bg-blue-50 rounded-lg">
                  <User className="h-4 w-4 text-blue-600" />
                  <div className="flex-1">
                    <p className="font-medium text-blue-900">{machine.operator.name}</p>
                    <p className="text-xs text-blue-700">{machine.operator.nameMyanmar} • {machine.operator.shift}</p>
                  </div>
                </div>
              ) : (
                <div className="flex items-center gap-2 p-3 bg-gray-50 rounded-lg">
                  <User className="h-4 w-4 text-gray-400" />
                  <p className="text-sm text-gray-600">No Operator Assigned</p>
                </div>
              )}

              {/* Machine Stats */}
              <div className="grid grid-cols-3 gap-3 pt-2 border-t">
                <div className="text-center">
                  <div className="font-bold text-blue-600">{machine.capacity}</div>
                  <div className="text-xs text-slate-600">pcs/hr</div>
                </div>
                <div className="text-center">
                  <div className="font-bold text-green-600">{machine.efficiency}%</div>
                  <div className="text-xs text-slate-600">Efficiency</div>
                </div>
                <div className="text-center">
                  <div className="font-bold text-purple-600">{machine.cycleTime}s</div>
                  <div className="text-xs text-slate-600">Cycle</div>
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
            <Activity className="h-16 w-16 mx-auto" />
          </div>
          <h3 className="text-lg font-semibold text-slate-900 mb-2">
            No machines found
          </h3>
          <p className="text-slate-600">
            No machines match the selected status filter.
          </p>
        </div>
      )}
    </div>
  );
}

export default MachineOverviewDashboard;