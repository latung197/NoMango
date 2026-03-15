import React, { useState, useEffect } from 'react';
import {
  Search,
  QrCode,
  Package,
  Factory,
  Clock,
  User,
  Settings,
  CheckCircle,
  AlertTriangle,
  Circle,
  PlayCircle,
  MapPin,
  GitBranch,
  TreePine,
  Eye,
  Download,
  RefreshCw,
  Filter,
  Activity,
  Truck,
  AlertCircle,
  ChevronRight,
  ChevronDown,
  Archive,
  Recycle,
  Trash2,
  Calendar,
  Monitor,
  FileText,
  ShieldCheck,
  Wrench
} from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Badge } from './ui/badge';
import { Progress } from './ui/progress';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Label } from './ui/label';

// Mock data for job tracking
const mockJobData = {
  // Individual Job
  'JOB20250907-001-C1': {
    type: 'job',
    jobId: 'JOB20250907-001-C1',
    parentPlanId: 'PLAN20250907-001',
    product: '2011 - Plastic Bottle 500ml',
    productCode: '2011',
    targetQty: 10000,
    producedQty: 10000,
    goodQty: 9850,
    defectQty: 100,
    scrapQty: 50,
    machine: 'INJ-M001',
    operator: 'Operator A (OP001)',
    shift: 'Day Shift',
    status: 'completed',
    lastUpdate: '2025-09-07 16:30',
    qrCode: 'QR-JOB20250907-001-C1',
    isSplit: false
  },

  // Parent Plan
  'PLAN20250907-001': {
    type: 'plan',
    planId: 'PLAN20250907-001',
    assemblyProduct: '2011 Box Set',
    productCode: 'ASET001',
    targetQty: 10000,
    childJobs: [
      {
        jobId: 'JOB20250907-001-C1',
        component: 'Cover',
        product: '2011 - Plastic Bottle 500ml',
        targetQty: 10000,
        goodQty: 9850,
        defectQty: 100,
        scrapQty: 50,
        qcStatus: 'passed',
        status: 'completed'
      },
      {
        jobId: 'JOB20250907-001-C2',
        component: 'Container',
        product: '2021 - Plastic Cap Red',
        targetQty: 10000,
        goodQty: 9950,
        defectQty: 50,
        scrapQty: 0,
        qcStatus: 'passed',
        status: 'completed'
      },
      {
        jobId: 'JOB20250907-001-C3',
        component: 'Liner',
        product: '2031 - Assembly Instructions',
        targetQty: 10000,
        goodQty: 10000,
        defectQty: 0,
        scrapQty: 0,
        qcStatus: 'passed',
        status: 'completed'
      }
    ],
    overallStatus: 'released',
    lastUpdate: '2025-09-07 17:30',
    qrCode: 'QR-PLAN20250907-001',
    completionPercentage: 100,
    completedJobs: 3,
    totalJobs: 3
  },

  // Incomplete Parent Plan
  'PLAN20250908-002': {
    type: 'plan',
    planId: 'PLAN20250908-002',
    assemblyProduct: '2012 Container Set',
    productCode: 'ASET002',
    targetQty: 5000,
    childJobs: [
      {
        jobId: 'JOB20250908-002-C1',
        component: 'Container',
        product: '2012 - Large Container 1L',
        targetQty: 5000,
        goodQty: 4800,
        defectQty: 150,
        scrapQty: 50,
        qcStatus: 'passed',
        status: 'completed'
      },
      {
        jobId: 'JOB20250908-002-C2',
        component: 'Lid',
        product: '2022 - Container Lid Blue',
        targetQty: 5000,
        goodQty: 0,
        defectQty: 0,
        scrapQty: 0,
        qcStatus: 'waiting-qc',
        status: 'active'
      }
    ],
    overallStatus: 'active',
    lastUpdate: '2025-09-08 14:20',
    qrCode: null,
    completionPercentage: 50,
    completedJobs: 1,
    totalJobs: 2
  },

  // Split Job Parent
  'JOB20250903-001': {
    type: 'job',
    jobId: 'JOB20250903-001',
    parentPlanId: null,
    product: '2013 - Plastic Cup 250ml',
    productCode: '2013',
    targetQty: 15000,
    producedQty: 15000,
    goodQty: 14850,
    defectQty: 150,
    scrapQty: 0,
    machine: 'Multiple',
    operator: 'Multiple',
    shift: 'Multiple',
    status: 'completed',
    lastUpdate: '2025-09-03 21:15',
    qrCode: 'QR-JOB20250903-001',
    isSplit: true,
    splitJobs: [
      {
        jobId: 'JOB20250903-001A',
        machine: 'INJ-M004',
        operator: 'Operator E (OP005)',
        targetQty: 8000,
        producedQty: 8000,
        status: 'completed',
        progress: 100
      },
      {
        jobId: 'JOB20250903-001B',
        machine: 'INJ-M005',
        operator: 'Operator F (OP006)',
        targetQty: 7000,
        producedQty: 7000,
        status: 'completed',
        progress: 100
      }
    ]
  }
};

// Stage definitions for timeline
const stageDefinitions = [
  { id: 'plan-created', name: 'Plan Created', nameMyanmar: 'အစီအစဉ်ဖန်တီး', icon: PlayCircle },
  { id: 'authorized', name: 'Authorized', nameMyanmar: 'အတည်ပြု', icon: CheckCircle },
  { id: 'material-reserved', name: 'Material Reserved', nameMyanmar: 'ပစ္စည်းသိမ်း', icon: Archive },
  { id: 'material-issued', name: 'Material Issued', nameMyanmar: 'ပစ္စည်းထုတ်', icon: Package },
  { id: 'in-production', name: 'In Production', nameMyanmar: 'ထုတ်လုပ်နေ', icon: Factory },
  { id: 'qc-inspection', name: 'QC Inspection', nameMyanmar: 'QC စစ်ဆေး', icon: Eye },
  { id: 'fg-receiving', name: 'FG Receiving', nameMyanmar: 'FG လက်ခံ', icon: Truck },
  { id: 'fg-release', name: 'FG-WH Release', nameMyanmar: 'FG-WH ထုတ်', icon: MapPin }
];

// Mock stage data
const mockStageData = {
  'JOB20250907-001-C1': [
    { stageId: 'plan-created', status: 'completed', timestamp: '2025-09-07 08:00', duration: '5 min' },
    { stageId: 'authorized', status: 'completed', timestamp: '2025-09-07 08:05', duration: '10 min' },
    { stageId: 'material-reserved', status: 'completed', timestamp: '2025-09-07 08:15', duration: '15 min' },
    { stageId: 'material-issued', status: 'completed', timestamp: '2025-09-07 08:30', duration: '30 min' },
    { stageId: 'in-production', status: 'completed', timestamp: '2025-09-07 09:00', duration: '5h 30min' },
    { stageId: 'qc-inspection', status: 'completed', timestamp: '2025-09-07 14:30', duration: '30 min' },
    { stageId: 'fg-receiving', status: 'completed', timestamp: '2025-09-07 15:00', duration: '30 min' },
    { stageId: 'fg-release', status: 'completed', timestamp: '2025-09-07 15:30', duration: 'Completed' }
  ],
  'PLAN20250907-001': [
    { stageId: 'plan-created', status: 'completed', timestamp: '2025-09-07 07:30', duration: '10 min' },
    { stageId: 'authorized', status: 'completed', timestamp: '2025-09-07 07:40', duration: '20 min' },
    { stageId: 'material-reserved', status: 'completed', timestamp: '2025-09-07 08:00', duration: '30 min' },
    { stageId: 'material-issued', status: 'completed', timestamp: '2025-09-07 08:30', duration: '1h' },
    { stageId: 'in-production', status: 'completed', timestamp: '2025-09-07 09:30', duration: '7h' },
    { stageId: 'qc-inspection', status: 'completed', timestamp: '2025-09-07 16:30', duration: '30 min' },
    { stageId: 'fg-receiving', status: 'completed', timestamp: '2025-09-07 17:00', duration: '30 min' },
    { stageId: 'fg-release', status: 'completed', timestamp: '2025-09-07 17:30', duration: 'Completed' }
  ]
};

// Mock defect and scrap data
const mockDefectScrapData = {
  'JOB20250907-001-C1': {
    defects: [
      {
        defectId: 'DEF-20250907-001',
        jobId: 'JOB20250907-001-C1',
        qty: 80,
        reason: 'Surface scratches',
        actionTaken: 'Sent to rework'
      },
      {
        defectId: 'DEF-20250907-002',
        jobId: 'JOB20250907-001-C1',
        qty: 20,
        reason: 'Dimension variance',
        actionTaken: 'Reprocessing'
      }
    ],
    scrap: [
      {
        scrapId: 'SCR-20250907-001',
        jobId: 'JOB20250907-001-C1',
        qty: 50,
        reason: 'Material contamination',
        date: '2025-09-07'
      }
    ]
  },
  'PLAN20250907-001': {
    defects: [
      {
        defectId: 'DEF-20250907-001',
        jobId: 'JOB20250907-001-C1',
        qty: 100,
        reason: 'Surface scratches',
        actionTaken: 'Sent to rework'
      },
      {
        defectId: 'DEF-20250907-003',
        jobId: 'JOB20250907-001-C2',
        qty: 50,
        reason: 'Color mismatch',
        actionTaken: 'Reprocessing'
      }
    ],
    scrap: [
      {
        scrapId: 'SCR-20250907-001',
        jobId: 'JOB20250907-001-C1',
        qty: 50,
        reason: 'Material contamination',
        date: '2025-09-07'
      }
    ]
  }
};

// Mock event history
const mockEventHistory = {
  'JOB20250907-001-C1': [
    {
      id: 1,
      timestamp: '2025-09-07 17:30',
      icon: Truck,
      description: 'FG Warehouse received 9850 pcs (QR FG-20250907-001)',
      descriptionMyanmar: 'FG ဂိုဒေါင်သို့ 9850 ခု လက်ခံ'
    },
    {
      id: 2,
      timestamp: '2025-09-07 15:00',
      icon: Eye,
      description: 'QC Inspection passed (Good 9850, Defect 100, Scrap 50)',
      descriptionMyanmar: 'QC စစ်ဆေးမှုအောင်မြင်'
    },
    {
      id: 3,
      timestamp: '2025-09-07 14:30',
      icon: Factory,
      description: 'Production job completed - 10000 units produced on INJ-M001',
      descriptionMyanmar: 'ထုတ်လုပ်မှုပြီးမြောက်'
    },
    {
      id: 4,
      timestamp: '2025-09-07 12:00',
      icon: Factory,
      description: '50% production milestone reached - 5000 units completed',
      descriptionMyanmar: '50% ထုတ်လုပ်မှုပြီး'
    },
    {
      id: 5,
      timestamp: '2025-09-07 09:00',
      icon: PlayCircle,
      description: 'Production started on machine INJ-M001 by Operator A',
      descriptionMyanmar: 'ထုတ်လုပ်မှုစတင်'
    },
    {
      id: 6,
      timestamp: '2025-09-02 09:00',
      icon: Package,
      description: '40kg PET Resin issued by Warehouse',
      descriptionMyanmar: 'ပစ္စည်းထုတ်ပေးခြင်း'
    }
  ],
  'PLAN20250907-001': [
    {
      id: 1,
      timestamp: '2025-09-07 17:30',
      icon: Truck,
      description: 'Assembly Plan released to FG Warehouse (QR-PLAN20250907-001)',
      descriptionMyanmar: 'Assembly Plan FG ဂိုဒေါင်သို့ ထုတ်ပေး'
    },
    {
      id: 2,
      timestamp: '2025-09-07 16:45',
      icon: CheckCircle,
      description: 'All child jobs completed - Assembly ready for release',
      descriptionMyanmar: 'child job များအားလုံးပြီး - Assembly ထုတ်ရန်အဆင်သင့်'
    },
    {
      id: 3,
      timestamp: '2025-09-07 15:30',
      icon: Eye,
      description: 'Final QC inspection completed for all components',
      descriptionMyanmar: 'အစိတ်အပိုင်းများအတွက် QC စစ်ဆေးမှုပြီး'
    },
    {
      id: 4,
      timestamp: '2025-09-07 08:00',
      icon: PlayCircle,
      description: 'Assembly Plan production started - 3 child jobs initiated',
      descriptionMyanmar: 'Assembly Plan ထုတ်လုပ်မှုစတင်'
    }
  ]
};

export function JobTracker() {
  const [searchTerm, setSearchTerm] = useState('');
  const [searchResults, setSearchResults] = useState<any>(null);
  const [expandedSplits, setExpandedSplits] = useState<Set<string>>(new Set());
  const [expandedAssembly, setExpandedAssembly] = useState<Set<string>>(new Set());
  const [isLiveMode, setIsLiveMode] = useState(false);

  // Auto-refresh for live mode
  useEffect(() => {
    let interval: NodeJS.Timeout;
    if (isLiveMode) {
      interval = setInterval(() => {
        // Mock refresh - in real app would fetch latest data
        console.log('Refreshing data...');
      }, 5000);
    }
    return () => clearInterval(interval);
  }, [isLiveMode]);

  const handleSearch = () => {
    if (!searchTerm.trim()) return;
    
    // Mock search logic
    const result = mockJobData[searchTerm.toUpperCase() as keyof typeof mockJobData];
    if (result) {
      setSearchResults(result);
    } else {
      setSearchResults(null);
    }
  };

  const handleKeyPress = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter') {
      handleSearch();
    }
  };

  const getStatusIcon = (status: string) => {
    switch (status) {
      case 'completed':
        return <CheckCircle className="h-4 w-4 text-green-600" />;
      case 'current':
        return <PlayCircle className="h-4 w-4 text-blue-600" />;
      case 'pending':
        return <Circle className="h-4 w-4 text-slate-400" />;
      default:
        return <Circle className="h-4 w-4 text-slate-400" />;
    }
  };

  const getStatusBadge = (status: string) => {
    const statusConfig: any = {
      'active': { color: 'bg-blue-100 text-blue-800', label: 'Active | တက်ကြွ' },
      'completed': { color: 'bg-green-100 text-green-800', label: 'Completed | ပြီး' },
      'waiting-qc': { color: 'bg-yellow-100 text-yellow-800', label: 'Waiting QC | QC စောင့်' },
      'released': { color: 'bg-purple-100 text-purple-800', label: 'Released | ထုတ်ပေး' }
    };
    const config = statusConfig[status] || { color: 'bg-slate-100 text-slate-800', label: status };
    return <Badge className={config.color}>{config.label}</Badge>;
  };

  const getQCStatusBadge = (status: string) => {
    const statusConfig: any = {
      'passed': { color: 'bg-green-100 text-green-800', label: 'Passed | အောင်' },
      'failed': { color: 'bg-red-100 text-red-800', label: 'Failed | ကျ' },
      'waiting-qc': { color: 'bg-yellow-100 text-yellow-800', label: 'Waiting QC | QC စောင့်' }
    };
    const config = statusConfig[status] || { color: 'bg-slate-100 text-slate-800', label: status };
    return <Badge className={config.color}>{config.label}</Badge>;
  };

  const toggleSplitExpanded = (jobId: string) => {
    const newExpanded = new Set(expandedSplits);
    if (newExpanded.has(jobId)) {
      newExpanded.delete(jobId);
    } else {
      newExpanded.add(jobId);
    }
    setExpandedSplits(newExpanded);
  };

  const toggleAssemblyExpanded = (planId: string) => {
    const newExpanded = new Set(expandedAssembly);
    if (newExpanded.has(planId)) {
      newExpanded.delete(planId);
    } else {
      newExpanded.add(planId);
    }
    setExpandedAssembly(newExpanded);
  };

  const renderStageTimeline = () => {
    if (!searchResults) return null;

    const stages = mockStageData[searchResults.jobId || searchResults.planId as keyof typeof mockStageData] || [];

    return (
      <Card className="mb-6">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Activity className="h-5 w-5" />
            Stage Timeline | အဆင့်အချိန်ကာလ
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="relative overflow-x-auto">
            <div className="flex items-center justify-between min-w-full pb-4" style={{ minWidth: '800px' }}>
              {stageDefinitions.map((stageDef, index) => {
                const stageData = stages.find(s => s.stageId === stageDef.id);
                const IconComponent = stageDef.icon;
                const isCompleted = stageData?.status === 'completed';
                const isCurrent = stageData?.status === 'current';
                
                return (
                  <div key={stageDef.id} className="flex flex-col items-center flex-1 relative">
                    {/* Connecting Line */}
                    {index < stageDefinitions.length - 1 && (
                      <div 
                        className={`absolute top-6 left-1/2 h-0.5 ${
                          isCompleted ? 'bg-green-300' : 'bg-slate-200'
                        }`}
                        style={{ 
                          width: 'calc(100% + 2rem)',
                          transform: 'translateX(50%)'
                        }}
                      />
                    )}
                    
                    {/* Stage Circle */}
                    <div className={`w-12 h-12 rounded-full flex items-center justify-center mb-2 relative z-10 ${
                      isCompleted ? 'bg-green-100 text-green-600 border-2 border-green-300' : 
                      isCurrent ? 'bg-blue-100 text-blue-600 border-2 border-blue-300' : 
                      'bg-slate-100 text-slate-400 border-2 border-slate-200'
                    }`}>
                      {isCompleted ? (
                        <CheckCircle className="h-6 w-6" />
                      ) : isCurrent ? (
                        <PlayCircle className="h-6 w-6" />
                      ) : (
                        <Circle className="h-6 w-6" />
                      )}
                    </div>
                    
                    {/* Stage Info */}
                    <div className="text-center">
                      <p className="text-xs font-medium">{stageDef.name}</p>
                      <p className="text-xs text-slate-500">{stageDef.nameMyanmar}</p>
                      {stageData && (
                        <div className="mt-1">
                          <p className="text-xs text-slate-600">{stageData.timestamp}</p>
                          <p className="text-xs text-slate-500">{stageData.duration}</p>
                        </div>
                      )}
                    </div>
                  </div>
                );
              })}
            </div>
          </div>
        </CardContent>
      </Card>
    );
  };

  return (
    <div className="p-6 bg-slate-50 min-h-screen">
      <div className="max-w-7xl mx-auto">
        {/* Header */}
        <div className="mb-6">
          <h1 className="text-2xl font-semibold text-slate-900 mb-2">
            Job Tracker | အလုပ်ခြေရာခံမှု
          </h1>
          <p className="text-slate-600">
            Real-time tracking of production jobs and assembly plans with comprehensive status monitoring.
          </p>
          <p className="text-sm text-slate-500 mt-1">
            ထုတ်လုပ်မှုအလုပ်များနှင့် assembly plan များ၏ အချိန်နှင့်တပြေးညီ ခြေရာခံမှုနှင့် အခြေအနေစောင့်ကြည့်မှု။
          </p>
        </div>

        {/* Search Section */}
        <Card className="mb-6">
          <CardContent className="pt-6">
            <div className="flex gap-4 items-end">
              <div className="flex-1">
                <Label htmlFor="search">Enter Plan ID or Job ID / Scan QR</Label>
                <p className="text-sm text-slate-500 mb-2">Plan ID သို့မဟုတ် Job ID ရိုက်ထည့်ပါ / QR စကင်န်ပါ</p>
                <div className="relative">
                  <Search className="absolute left-3 top-3 h-4 w-4 text-slate-400" />
                  <Input
                    id="search"
                    placeholder="e.g., PLAN20250907-001 or JOB20250907-001-C1"
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    onKeyPress={handleKeyPress}
                    className="pl-10"
                  />
                </div>
              </div>
              <Button onClick={handleSearch} className="bg-slate-900 hover:bg-slate-800">
                <Search className="h-4 w-4 mr-2" />
                Track
              </Button>
              <Button variant="outline">
                <QrCode className="h-4 w-4 mr-2" />
                Scan QR
              </Button>
            </div>
          </CardContent>
        </Card>

        {searchResults ? (
          <>
            {/* Job Summary Card */}
            <Card className="mb-6">
              <CardHeader>
                <CardTitle className="flex items-center justify-between">
                  <div className="flex items-center gap-2">
                    {searchResults.type === 'plan' ? <TreePine className="h-5 w-5" /> : <Package className="h-5 w-5" />}
                    {searchResults.type === 'plan' ? 'Plan Summary | Plan အနှစ်ချုပ်' : 'Job Summary | အလုပ်အနှစ်ချုပ်'}
                  </div>
                  <div className="flex items-center gap-2">
                    {isLiveMode && (
                      <Badge className="bg-red-100 text-red-800 animate-pulse">
                        <Monitor className="h-3 w-3 mr-1" />
                        LIVE
                      </Badge>
                    )}
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() => setIsLiveMode(!isLiveMode)}
                    >
                      <RefreshCw className={`h-4 w-4 mr-2 ${isLiveMode ? 'animate-spin' : ''}`} />
                      {isLiveMode ? 'Stop Live' : 'Auto-refresh'}
                    </Button>
                  </div>
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mb-4">
                  <div>
                    <Label>ID</Label>
                    <p className="font-medium">{searchResults.jobId || searchResults.planId}</p>
                    {searchResults.parentPlanId && (
                      <p className="text-sm text-slate-500">Parent: {searchResults.parentPlanId}</p>
                    )}
                  </div>
                  <div>
                    <Label>Product Name & Code</Label>
                    <p className="font-medium">{searchResults.product || searchResults.assemblyProduct}</p>
                    <p className="text-sm text-slate-500">{searchResults.productCode}</p>
                  </div>
                  <div>
                    <Label>Target Qty vs Produced Qty</Label>
                    <p className="font-medium">
                      <span className="text-blue-600">{searchResults.targetQty?.toLocaleString()}</span>
                      {searchResults.producedQty && (
                        <>
                          {' / '}
                          <span className="text-green-600">{searchResults.producedQty.toLocaleString()}</span>
                        </>
                      )}
                    </p>
                  </div>
                  <div>
                    <Label>Current Status</Label>
                    <div className="mt-1">
                      {getStatusBadge(searchResults.status || searchResults.overallStatus)}
                    </div>
                  </div>
                </div>

                {searchResults.type !== 'plan' && (
                  <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mb-4">
                    <div>
                      <Label>Good Qty / Defect Qty / Scrap Qty</Label>
                      <div className="flex gap-2 mt-1">
                        <span className="text-green-600 font-medium">{searchResults.goodQty?.toLocaleString()}</span>
                        <span className="text-slate-400">/</span>
                        <span className="text-yellow-600 font-medium">{searchResults.defectQty?.toLocaleString()}</span>
                        <span className="text-slate-400">/</span>
                        <span className="text-red-600 font-medium">{searchResults.scrapQty?.toLocaleString()}</span>
                      </div>
                    </div>
                    <div>
                      <Label>Machine</Label>
                      <p className="font-medium flex items-center gap-2">
                        <Settings className="h-4 w-4" />
                        {searchResults.machine}
                      </p>
                    </div>
                    <div>
                      <Label>Operator</Label>
                      <p className="font-medium flex items-center gap-2">
                        <User className="h-4 w-4" />
                        {searchResults.operator}
                      </p>
                    </div>
                    <div>
                      <Label>Shift</Label>
                      <p className="font-medium flex items-center gap-2">
                        <Clock className="h-4 w-4" />
                        {searchResults.shift}
                      </p>
                    </div>
                  </div>
                )}

                <div className="flex items-center justify-between pt-4 border-t">
                  <div className="flex items-center gap-2">
                    <Calendar className="h-4 w-4 text-slate-400" />
                    <span className="text-sm text-slate-600">Last Update: {searchResults.lastUpdate}</span>
                  </div>
                  {searchResults.qrCode && (
                    <div className="flex items-center gap-2">
                      <QrCode className="h-4 w-4 text-slate-400" />
                      <span className="text-sm text-slate-600">QR: {searchResults.qrCode}</span>
                    </div>
                  )}
                </div>
              </CardContent>
            </Card>

            {/* Stage Timeline */}
            {renderStageTimeline()}

            {/* Split Tracking Section */}
            {searchResults.isSplit && searchResults.splitJobs && (
              <Card className="mb-6">
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <GitBranch className="h-5 w-5" />
                    Split Tracking | ခွဲထားသောအလုပ်ခြေရာခံ
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="mb-4 p-3 bg-blue-50 rounded-lg">
                    <p className="text-sm text-blue-800">
                      Parent Job: <span className="font-medium">{searchResults.jobId}</span> was split into {searchResults.splitJobs.length} jobs
                    </p>
                  </div>
                  
                  <Button
                    variant="ghost"
                    onClick={() => toggleSplitExpanded(searchResults.jobId)}
                    className="mb-4"
                  >
                    {expandedSplits.has(searchResults.jobId) ? (
                      <ChevronDown className="h-4 w-4 mr-2" />
                    ) : (
                      <ChevronRight className="h-4 w-4 mr-2" />
                    )}
                    Tree view of split jobs
                  </Button>

                  {expandedSplits.has(searchResults.jobId) && (
                    <div className="space-y-4 ml-6">
                      {searchResults.splitJobs.map((splitJob: any, index: number) => (
                        <div key={splitJob.jobId} className="border-l-2 border-blue-200 pl-4">
                          <div className="bg-white border rounded-lg p-4">
                            <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
                              <div>
                                <Label>Split Job ID</Label>
                                <p className="font-medium">{splitJob.jobId}</p>
                              </div>
                              <div>
                                <Label>Machine, Qty, Status</Label>
                                <p className="text-sm">{splitJob.machine}</p>
                                <p className="text-sm">{splitJob.targetQty?.toLocaleString()} pcs</p>
                                {getStatusBadge(splitJob.status)}
                              </div>
                              <div>
                                <Label>Progress</Label>
                                <div className="flex items-center gap-2">
                                  <Progress value={splitJob.progress} className="flex-1" />
                                  <span className="text-sm font-medium">{splitJob.progress}%</span>
                                </div>
                              </div>
                              <div>
                                <Label>Operator</Label>
                                <p className="text-sm">{splitJob.operator}</p>
                              </div>
                            </div>
                          </div>
                        </div>
                      ))}
                    </div>
                  )}
                </CardContent>
              </Card>
            )}

            {/* Assembly Tracking Section */}
            {searchResults.type === 'plan' && (
              <Card className="mb-6">
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <TreePine className="h-5 w-5" />
                    Assembly Tracking | Assembly ခြေရာခံမှု
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="mb-4">
                    <div className="flex items-center justify-between mb-4">
                      <div>
                        <p className="font-medium">Parent Product: {searchResults.assemblyProduct}</p>
                        <div className="flex items-center gap-4 mt-2">
                          <div className="flex items-center gap-2">
                            <Progress value={(searchResults.completedJobs / searchResults.totalJobs) * 100} className="w-32" />
                            <span className="text-sm font-medium text-green-600">
                              {searchResults.completedJobs}/{searchResults.totalJobs} Complete
                            </span>
                          </div>
                        </div>
                      </div>
                      <Button
                        variant="ghost"
                        onClick={() => toggleAssemblyExpanded(searchResults.planId)}
                      >
                        {expandedAssembly.has(searchResults.planId) ? (
                          <ChevronDown className="h-4 w-4 mr-2" />
                        ) : (
                          <ChevronRight className="h-4 w-4 mr-2" />
                        )}
                        View Child Jobs
                      </Button>
                    </div>

                    {searchResults.completionPercentage < 100 && (
                      <div className="p-3 bg-red-50 border border-red-200 rounded-lg mb-4">
                        <div className="flex items-center gap-2">
                          <AlertTriangle className="h-5 w-5 text-red-600" />
                          <p className="text-red-800 font-medium">
                            ⚠ Parent Plan incomplete – cannot release to FG Warehouse
                          </p>
                        </div>
                      </div>
                    )}
                  </div>

                  {expandedAssembly.has(searchResults.planId) && (
                    <div className="overflow-hidden rounded-lg border">
                      <Table>
                        <TableHeader>
                          <TableRow>
                            <TableHead>Job ID</TableHead>
                            <TableHead>Component</TableHead>
                            <TableHead>Target Qty</TableHead>
                            <TableHead>Good Qty</TableHead>
                            <TableHead>Defect Qty</TableHead>
                            <TableHead>Scrap Qty</TableHead>
                            <TableHead>QC Status</TableHead>
                          </TableRow>
                        </TableHeader>
                        <TableBody>
                          {searchResults.childJobs.map((childJob: any) => (
                            <TableRow key={childJob.jobId}>
                              <TableCell className="font-medium">{childJob.jobId}</TableCell>
                              <TableCell>{childJob.component}</TableCell>
                              <TableCell>{childJob.targetQty?.toLocaleString()}</TableCell>
                              <TableCell className="text-green-600 font-medium">
                                {childJob.goodQty?.toLocaleString()}
                              </TableCell>
                              <TableCell className="text-yellow-600 font-medium">
                                {childJob.defectQty?.toLocaleString()}
                              </TableCell>
                              <TableCell className="text-red-600 font-medium">
                                {childJob.scrapQty?.toLocaleString()}
                              </TableCell>
                              <TableCell>{getQCStatusBadge(childJob.qcStatus)}</TableCell>
                            </TableRow>
                          ))}
                        </TableBody>
                      </Table>
                    </div>
                  )}
                </CardContent>
              </Card>
            )}

            {/* Defect & Scrap Section */}
            <Card className="mb-6">
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <AlertTriangle className="h-5 w-5" />
                  Defect & Scrap | ချွတ်ယွင်းမှုနှင့် အပိုင်းအစများ
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                  {/* Defects Table */}
                  <div>
                    <h4 className="font-medium mb-3 text-blue-800">Defects | ချွတ်ယွင်းမှုများ</h4>
                    <div className="border rounded-lg overflow-hidden">
                      <Table>
                        <TableHeader>
                          <TableRow>
                            <TableHead>Defect ID</TableHead>
                            <TableHead>Job ID</TableHead>
                            <TableHead>Qty</TableHead>
                            <TableHead>Reason</TableHead>
                            <TableHead>Action Taken</TableHead>
                          </TableRow>
                        </TableHeader>
                        <TableBody>
                          {mockDefectScrapData[searchResults.jobId || searchResults.planId as keyof typeof mockDefectScrapData]?.defects?.map((defect: any) => (
                            <TableRow key={defect.defectId} className="bg-blue-50">
                              <TableCell className="font-medium text-blue-800">{defect.defectId}</TableCell>
                              <TableCell>{defect.jobId}</TableCell>
                              <TableCell className="font-medium">{defect.qty}</TableCell>
                              <TableCell>{defect.reason}</TableCell>
                              <TableCell>{defect.actionTaken}</TableCell>
                            </TableRow>
                          )) || (
                            <TableRow>
                              <TableCell colSpan={5} className="text-center text-slate-500">
                                No defects recorded
                              </TableCell>
                            </TableRow>
                          )}
                        </TableBody>
                      </Table>
                    </div>
                  </div>

                  {/* Scrap Table */}
                  <div>
                    <h4 className="font-medium mb-3 text-red-800">Scrap | အပိုင်းအစများ</h4>
                    <div className="border rounded-lg overflow-hidden">
                      <Table>
                        <TableHeader>
                          <TableRow>
                            <TableHead>Scrap ID</TableHead>
                            <TableHead>Job ID</TableHead>
                            <TableHead>Qty</TableHead>
                            <TableHead>Reason</TableHead>
                            <TableHead>Date</TableHead>
                          </TableRow>
                        </TableHeader>
                        <TableBody>
                          {mockDefectScrapData[searchResults.jobId || searchResults.planId as keyof typeof mockDefectScrapData]?.scrap?.map((scrap: any) => (
                            <TableRow key={scrap.scrapId} className="bg-red-50">
                              <TableCell className="font-medium text-red-800">{scrap.scrapId}</TableCell>
                              <TableCell>{scrap.jobId}</TableCell>
                              <TableCell className="font-medium">{scrap.qty}</TableCell>
                              <TableCell>{scrap.reason}</TableCell>
                              <TableCell>{scrap.date}</TableCell>
                            </TableRow>
                          )) || (
                            <TableRow>
                              <TableCell colSpan={5} className="text-center text-slate-500">
                                No scrap recorded
                              </TableCell>
                            </TableRow>
                          )}
                        </TableBody>
                      </Table>
                    </div>
                  </div>
                </div>
              </CardContent>
            </Card>

            {/* Event History Feed */}
            <Card className="mb-6">
              <CardHeader>
                <CardTitle className="flex items-center justify-between">
                  <div className="flex items-center gap-2">
                    <Activity className="h-5 w-5" />
                    Event History | ဖြစ်ရပ်မှတ်တမ်း
                  </div>
                  <Button variant="outline" size="sm">
                    <Download className="h-4 w-4 mr-2" />
                    Export
                  </Button>
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="space-y-4">
                  {mockEventHistory[searchResults.jobId || searchResults.planId as keyof typeof mockEventHistory]?.map((event: any) => {
                    const IconComponent = event.icon;
                    return (
                      <div key={event.id} className="flex items-start gap-4 p-4 border-l-4 border-blue-200 bg-slate-50 rounded-r-lg">
                        <div className="flex-shrink-0 mt-1">
                          <IconComponent className="h-4 w-4 text-blue-600" />
                        </div>
                        <div className="flex-1">
                          <div className="flex items-center justify-between mb-1">
                            <span className="text-sm font-medium text-slate-900">{event.timestamp}</span>
                          </div>
                          <p className="text-sm text-slate-700 mb-1">{event.description}</p>
                          <p className="text-xs text-slate-500">{event.descriptionMyanmar}</p>
                        </div>
                      </div>
                    );
                  }) || (
                    <div className="text-center py-8 text-slate-500">
                      No event history available
                    </div>
                  )}
                </div>
              </CardContent>
            </Card>

            {/* Export Actions */}
            <Card>
              <CardContent className="pt-6">
                <div className="flex gap-2 flex-wrap">
                  <Button variant="outline">
                    <Download className="h-4 w-4 mr-2" />
                    Export PDF
                  </Button>
                  <Button variant="outline">
                    <Download className="h-4 w-4 mr-2" />
                    Export Excel
                  </Button>
                </div>
              </CardContent>
            </Card>
          </>
        ) : (
          <Card>
            <CardContent className="pt-6">
              <div className="text-center py-12">
                <Search className="h-12 w-12 text-slate-400 mx-auto mb-4" />
                <h3 className="text-lg font-medium text-slate-900 mb-2">
                  Enter a Plan ID or Job ID to start tracking
                </h3>
                <p className="text-slate-600 mb-4">
                  Try searching for: PLAN20250907-001, JOB20250907-001-C1, PLAN20250908-002, or JOB20250903-001
                </p>
                <p className="text-sm text-slate-500">
                  ခြေရာခံစတင်ရန် Plan ID သို့မဟုတ် Job ID ရိုက်ထည့်ပါ
                </p>
              </div>
            </CardContent>
          </Card>
        )}
      </div>
    </div>
  );
}