import React, { useState, Fragment } from 'react';
import {
  ClipboardList,
  Package,
  CheckCircle,
  Truck,
  AlertTriangle,
  Search,
  Filter,
  Download,
  Eye,
  Edit,
  Trash2,
  Play,
  Pause,
  Square,
  Calendar,
  User,
  Settings,
  ArrowRight,
  QrCode,
  Recycle,
  X,
  Save,
  RotateCcw,
  Clock,
  Factory,
  Award,
  AlertCircle,
  CheckCircle2,
  TrendingUp,
  GitBranch,
  TreePine,
  ChevronRight,
  ArrowLeft,
  Plus,
  Monitor,
  Split,
  MapPin,
  Printer,
  ShoppingCart,
  Layers,
  ChevronDown,
  ChevronRight,
  GitBranch,
  TreePine,
  Scale,
  RotateCw,
  Undo,
  ScanLine,
  ArrowLeft,
  FileText,
  Minus,
  History,
  BarChart3,
  ArrowRightLeft
} from 'lucide-react';
import { RejectManagement } from './RejectManagement';
import { FinishedGoodsReceiving } from './FinishedGoodsReceiving';
import { FinishedGoodsTransfer } from './FinishedGoodsTransfer';
import { MoldChangeRequests } from './MoldChangeRequests';
import { ProductionControlMCR } from './ProductionControlMCR';
import { CutGlueResidueManagement } from './CutGlueResidueManagement';

import { JobTracker } from './JobTracker';
import { WasteManagement } from './WasteManagement';
import { SplitJobManagement } from './SplitJobManagement';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Badge } from './ui/badge';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogTrigger } from './ui/dialog';
import { Tabs, TabsContent, TabsList, TabsTrigger } from './ui/tabs';
import { Switch } from './ui/switch';
import { Checkbox } from './ui/checkbox';
import { Textarea } from './ui/textarea';
import { Progress } from './ui/progress';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from './ui/tooltip';
import { toast } from 'sonner';

interface ProductionControlProps {
  currentPage: string;
  onPageChange: (page: string) => void;
}

// Warehouse color mapping for consistent UI
const getWarehouseColor = (warehouseCode: string) => {
  const colorMap: Record<string, string> = {
    'RM-WH1': 'bg-blue-100 text-blue-800 border-blue-200',
    'RM-WH2': 'bg-green-100 text-green-800 border-green-200',
    'RM-WH3': 'bg-purple-100 text-purple-800 border-purple-200',
    'RM-WH4': 'bg-orange-100 text-orange-800 border-orange-200',
    'RM-WH5': 'bg-pink-100 text-pink-800 border-pink-200'
  };
  return colorMap[warehouseCode] || 'bg-slate-100 text-slate-800 border-slate-200';
};

// Validate warehouse breakdown totals
const validateWarehouseTotal = (warehouseBreakdown: any[], expectedTotal: number) => {
  const actualTotal = warehouseBreakdown.reduce((sum, wh) => sum + wh.qty, 0);
  return Math.abs(actualTotal - expectedTotal) < 0.01; // Allow for minor floating point errors
};

// Mock data for Production Control
const mockProductionDemands = [
  {
    planId: 'PL20250903-001',
    product: '2011 - Plastic Bottle 500ml',
    quantity: 1000,
    plannerName: 'John Doe',
    status: 'submitted',
    submittedDate: '2025-09-03',
    priority: 'high'
  },
  {
    planId: 'PL20250903-002',
    product: '2012 - Plastic Container 1L',
    quantity: 500,
    plannerName: 'Jane Smith',
    status: 'draft',
    submittedDate: '2025-09-03',
    priority: 'medium'
  },
  {
    planId: 'PL20250902-001',
    product: '2013 - Plastic Cup 250ml',
    quantity: 2000,
    plannerName: 'Mike Johnson',
    status: 'approved',
    submittedDate: '2025-09-02',
    priority: 'low',
    jobId: 'JOB20250903-001'
  },
  {
    planId: 'PL20250902-002',
    product: '2011 - Plastic Bottle 500ml',
    quantity: 1500,
    plannerName: 'Sarah Wilson',
    status: 'rejected',
    submittedDate: '2025-09-02',
    priority: 'high',
    rejectionReason: 'Insufficient raw material inventory'
  }
];

const mockActiveJobs = [
  {
    jobId: 'JOB20250903-001',
    product: '2013 - Plastic Cup 250ml',
    targetQty: 2000,
    producedQty: 1200,
    operator: 'Operator A (OP001)',
    machine: 'INJ-M001',
    status: 'running',
    startTime: '2025-09-03 08:00',
    estimatedEnd: '2025-09-03 18:00'
  },
  {
    jobId: 'JOB20250903-002',
    product: '2011 - Plastic Bottle 500ml',
    targetQty: 1500,
    producedQty: 750,
    operator: 'Operator B (OP002)',
    machine: 'INJ-M002',
    status: 'paused',
    startTime: '2025-09-03 09:00',
    estimatedEnd: '2025-09-03 19:00'
  },
  {
    jobId: 'JOB20250903-003',
    product: '2012 - Plastic Container 1L',
    targetQty: 500,
    producedQty: 500,
    operator: 'Operator C (OP003)',
    machine: 'INJ-M003',
    status: 'completed',
    startTime: '2025-09-03 07:00',
    estimatedEnd: '2025-09-03 15:00',
    actualEnd: '2025-09-03 14:30'
  }
];

// Mock data for active jobs with reserved materials for Job-to-Job Transfer
const mockActiveJobsWithMaterials = [
  {
    jobId: 'JOB20250903-001',
    product: '2013 - Plastic Cup 250ml',
    machine: 'INJ-M001',
    reservedMaterials: [
      { materialCode: 'PET001', materialName: 'PET Resin Clear', color: 'Clear', qty: 40.0, unit: 'kg' },
      { materialCode: 'PP002', materialName: 'PP Blue Compound', color: 'Blue', qty: 15.5, unit: 'kg' }
    ]
  },
  {
    jobId: 'JOB20250903-002',
    product: '2011 - Plastic Bottle 500ml',
    machine: 'INJ-M002',
    reservedMaterials: [
      { materialCode: 'PP002', materialName: 'PP Blue Compound', color: 'Blue', qty: 25.5, unit: 'kg' }
    ]
  },
  {
    jobId: 'JOB20250903-004',
    product: '2011 - Plastic Bottle 500ml',
    machine: 'INJ-M004',
    reservedMaterials: [
      { materialCode: 'PET001', materialName: 'PET Resin Clear', color: 'Clear', qty: 32.0, unit: 'kg' }
    ]
  },
  {
    jobId: 'JOB20250903-005',
    product: '2013 - Plastic Cup 250ml',
    machine: 'INJ-M005',
    reservedMaterials: []
  }
];

// Mock data for Job-to-Job Transfer requests  
const mockJobTransferRequests = [
  {
    id: 'JT-REQ-001',
    refNo: 'JT-20250908-001',
    fromJobId: 'JOB20250903-001',
    fromMachine: 'INJ-M001',
    toJobId: 'JOB20250903-005',
    toMachine: 'INJ-M005',
    materialCode: 'PET001',
    materialName: 'PET Resin Clear',
    color: 'Clear',
    transferQty: 15.0,
    unit: 'kg',
    reason: 'Over Reserved',
    status: 'Pending Acknowledgement (Warehouse)',
    requestedBy: 'Production Manager 1',
    requestedDate: '2025-09-08 10:30',
    processedDate: null,
    processedBy: null
  },
  {
    id: 'JT-REQ-002',
    refNo: 'JT-20250908-002',
    fromJobId: 'JOB20250903-002',
    fromMachine: 'INJ-M002',
    toJobId: 'JOB20250903-004',
    toMachine: 'INJ-M004',
    materialCode: 'PP002',
    materialName: 'PP Blue Compound',
    color: 'Blue',
    transferQty: 10.0,
    unit: 'kg',
    reason: 'Urgent Order',
    status: 'Acknowledged',
    requestedBy: 'Production Manager 2',
    requestedDate: '2025-09-08 14:15',
    processedDate: '2025-09-08 14:45',
    processedBy: 'Warehouse Staff 1'
  }
];

// Mock data for Job Monitoring Dashboard
const mockJobMonitoringData = [
  {
    jobId: 'JOB20250908-001',
    planId: 'PL20250903-001',
    product: '2011 - Plastic Bottle 500ml',
    productCode: 'PB500',
    targetQty: 2000,
    producedQty: 1580,
    defectQty: 35,
    goodQty: 1545,
    machine: 'INJ-M001',
    operator: 'Operator A (OP001)',
    shift: 'Day Shift (08:00-16:00)',
    status: 'Running',
    priority: 'High',
    startTime: '2025-09-08 08:00',
    estimatedEnd: '2025-09-08 17:30',
    actualEnd: null,
    cycleTime: 45,
    currentCycleTime: 47,
    efficiency: 95.7,
    oeeScore: 91.2,
    downtimeMinutes: 22,
    lastActivity: '2025-09-08 14:45',
    qcStatus: 'Pass',
    materialStatus: 'Available',
    moldCondition: 'Good',
    alertCount: 1,
    progressPercentage: 79,
    rawMaterials: [
      { code: 'PET001', name: 'PET Resin Clear', required: 80, consumed: 63.2, remaining: 16.8, unit: 'kg' },
      { code: 'ADD001', name: 'Additive Blue', required: 2, consumed: 1.58, remaining: 0.42, unit: 'kg' }
    ],
    recentEvents: [
      { time: '14:45', event: 'Quality check passed', type: 'info' },
      { time: '14:30', event: 'Cycle time spike detected', type: 'warning' },
      { time: '14:15', event: 'Material top-up completed', type: 'success' }
    ]
  },
  {
    jobId: 'JOB20250908-002',
    planId: 'PL20250903-002',
    product: '2012 - Plastic Container 1L',
    productCode: 'PC1000',
    targetQty: 1200,
    producedQty: 1200,
    defectQty: 18,
    goodQty: 1182,
    machine: 'INJ-M002',
    operator: 'Operator B (OP002)',
    shift: 'Day Shift (08:00-16:00)',
    status: 'Completed',
    priority: 'Medium',
    startTime: '2025-09-08 07:30',
    estimatedEnd: '2025-09-08 15:00',
    actualEnd: '2025-09-08 14:45',
    cycleTime: 38,
    currentCycleTime: 38,
    efficiency: 98.5,
    oeeScore: 95.8,
    downtimeMinutes: 8,
    lastActivity: '2025-09-08 14:45',
    qcStatus: 'Pass',
    materialStatus: 'Depleted',
    moldCondition: 'Good',
    alertCount: 0,
    progressPercentage: 100,
    rawMaterials: [
      { code: 'PP002', name: 'PP Blue Compound', required: 60, consumed: 60, remaining: 0, unit: 'kg' }
    ],
    recentEvents: [
      { time: '14:45', event: 'Job completed successfully', type: 'success' },
      { time: '14:30', event: 'Final quality inspection passed', type: 'info' },
      { time: '14:00', event: 'Approaching target quantity', type: 'info' }
    ]
  },
  {
    jobId: 'JOB20250908-003',
    planId: 'PL20250902-003',
    product: '2013 - Plastic Cup 250ml',
    productCode: 'PC250',
    targetQty: 3000,
    producedQty: 950,
    defectQty: 28,
    goodQty: 922,
    machine: 'INJ-M003',
    operator: 'Operator C (OP003)',
    shift: 'Day Shift (08:00-16:00)',
    status: 'Paused',
    priority: 'Low',
    startTime: '2025-09-08 09:00',
    estimatedEnd: '2025-09-08 18:30',
    actualEnd: null,
    cycleTime: 30,
    currentCycleTime: 0,
    efficiency: 88.2,
    oeeScore: 82.5,
    downtimeMinutes: 45,
    lastActivity: '2025-09-08 13:15',
    qcStatus: 'Pending',
    materialStatus: 'Low',
    moldCondition: 'Maintenance Required',
    alertCount: 3,
    progressPercentage: 32,
    rawMaterials: [
      { code: 'PS001', name: 'PS Clear', required: 45, consumed: 14.25, remaining: 30.75, unit: 'kg' },
      { code: 'ADD002', name: 'UV Stabilizer', required: 1.5, consumed: 0.48, remaining: 1.02, unit: 'kg' }
    ],
    recentEvents: [
      { time: '13:15', event: 'Job paused - Mold maintenance required', type: 'error' },
      { time: '13:00', event: 'High defect rate detected', type: 'warning' },
      { time: '12:45', event: 'Material level low warning', type: 'warning' }
    ]
  },
  {
    jobId: 'JOB20250908-004',
    planId: 'PL20250902-004',
    product: '2014 - Plastic Lid 100ml',
    productCode: 'PL100',
    targetQty: 5000,
    producedQty: 3200,
    defectQty: 45,
    goodQty: 3155,
    machine: 'INJ-M004',
    operator: 'Operator D (OP004)',
    shift: 'Day Shift (08:00-16:00)',
    status: 'Running',
    priority: 'High',
    startTime: '2025-09-08 06:00',
    estimatedEnd: '2025-09-08 16:00',
    actualEnd: null,
    cycleTime: 15,
    currentCycleTime: 16,
    efficiency: 93.8,
    oeeScore: 89.4,
    downtimeMinutes: 18,
    lastActivity: '2025-09-08 14:50',
    qcStatus: 'Pass',
    materialStatus: 'Available',
    moldCondition: 'Good',
    alertCount: 0,
    progressPercentage: 64,
    rawMaterials: [
      { code: 'PP003', name: 'PP White', required: 25, consumed: 16, remaining: 9, unit: 'kg' }
    ],
    recentEvents: [
      { time: '14:50', event: 'Production rhythm stable', type: 'info' },
      { time: '14:30', event: 'Operator break completed', type: 'info' },
      { time: '14:00', event: 'Mid-day quality check passed', type: 'success' }
    ]
  },
  {
    jobId: 'JOB20250908-005',
    planId: 'PL20250903-005',
    product: '2015 - Plastic Tray Large',
    productCode: 'PT-L',
    targetQty: 800,
    producedQty: 0,
    defectQty: 0,
    goodQty: 0,
    machine: 'INJ-M005',
    operator: 'Operator E (OP005)',
    shift: 'Day Shift (08:00-16:00)',
    status: 'Setup',
    priority: 'Medium',
    startTime: '2025-09-08 15:00',
    estimatedEnd: '2025-09-09 01:00',
    actualEnd: null,
    cycleTime: 120,
    currentCycleTime: 0,
    efficiency: 0,
    oeeScore: 0,
    downtimeMinutes: 0,
    lastActivity: '2025-09-08 14:55',
    qcStatus: 'Not Started',
    materialStatus: 'Loading',
    moldCondition: 'Installing',
    alertCount: 0,
    progressPercentage: 0,
    rawMaterials: [
      { code: 'ABS001', name: 'ABS Black', required: 120, consumed: 0, remaining: 120, unit: 'kg' },
      { code: 'ADD003', name: 'Flame Retardant', required: 3, consumed: 0, remaining: 3, unit: 'kg' }
    ],
    recentEvents: [
      { time: '14:55', event: 'Mold installation in progress', type: 'info' },
      { time: '14:45', event: 'Material preparation started', type: 'info' },
      { time: '14:30', event: 'Job setup initiated', type: 'info' }
    ]
  }
];

// Return and Transfer Reason Options
const returnReasonOptions = [
  'End of Job',
  'Over Issued',
  'Material Change',
  'Machine Issue',
  'Quality Issue',
  'Production Stop'
];

const transferReasonOptions = [
  'Over Reserved',
  'Urgent Order',
  'Machine Breakdown',
  'Priority Change',
  'Resource Reallocation'
];

// Mock data for Return Leftover Requests
const mockReturnLeftoverRequests = [
  {
    id: 'RL-REQ-001',
    refNo: 'RL-20250908-001',
    jobId: 'JOB20250903-001',
    machine: 'INJ-M001',
    product: '2013 - Plastic Cup 250ml',
    materialCode: 'PET001',
    materialName: 'PET Resin Clear',
    color: 'Clear',
    issuedQty: 40.0,
    returnedQty: 8.5,
    unit: 'kg',
    reason: 'End of Job',
    status: 'Pending',
    requestedBy: 'Production Manager',
    requestedDate: '2025-09-08 16:30',
    processedDate: null,
    processedBy: null
  },
  {
    id: 'RL-REQ-002',
    refNo: 'RL-20250908-002',
    jobId: 'JOB20250903-002',
    machine: 'INJ-M002',
    product: '2011 - Plastic Bottle 500ml',
    materialCode: 'PP002',
    materialName: 'PP Blue Compound',
    color: 'Blue',
    issuedQty: 25.5,
    returnedQty: 3.0,
    unit: 'kg',
    reason: 'Over Issued',
    status: 'Approved',
    requestedBy: 'Production Manager',
    requestedDate: '2025-09-08 14:45',
    processedDate: '2025-09-08 15:20',
    processedBy: 'Warehouse Manager'
  },
  {
    id: 'RL-REQ-003',
    refNo: 'RL-20250908-003',
    jobId: 'JOB20250903-003',
    machine: 'INJ-M003',
    product: '2012 - Plastic Container 1L',
    materialCode: 'PP002',
    materialName: 'PP Blue Compound',
    color: 'Blue',
    issuedQty: 18.0,
    returnedQty: 2.5,
    unit: 'kg',
    reason: 'Material Change',
    status: 'Rejected',
    requestedBy: 'Production Manager',
    requestedDate: '2025-09-08 13:15',
    processedDate: '2025-09-08 14:00',
    processedBy: 'Warehouse Manager',
    rejectionReason: 'Material already contaminated'
  },
  {
    id: 'RL-REQ-004',
    refNo: 'RL-20250908-004',
    jobId: 'JOB20250903-004',
    machine: 'INJ-M004',
    product: '2011 - Plastic Bottle 500ml',
    materialCode: 'PET001',
    materialName: 'PET Resin Clear',
    color: 'Clear',
    issuedQty: 32.0,
    returnedQty: 0,
    unit: 'kg',
    reason: 'Machine Issue',
    status: 'Pending',
    requestedBy: 'Production Manager',
    requestedDate: '2025-09-08 17:10',
    processedDate: null,
    processedBy: null
  }
];

// Mock data for Raw Material Requests - simplified statuses
const mockIssueRequests = [
  {
    id: 'RMR-REQ-001',
    refNo: 'RMR-20250908-001',
    jobId: 'JOB20250903-001',
    machine: 'INJ-M001',
    product: '2013 - Plastic Cup 250ml',
    materialCode: 'PET001',
    materialName: 'PET Resin Clear',
    color: 'Clear',
    requiredQty: 40.0,
    reservedQty: 40.0,
    issueQty: 40.0,
    issuedQty: 40.0,
    warehouseBreakdown: [
      { warehouseCode: 'RM-WH1', warehouseName: 'Raw Material Warehouse A', qty: 20.0 },
      { warehouseCode: 'RM-WH2', warehouseName: 'Raw Material Warehouse B', qty: 20.0 }
    ],
    unit: 'kg',
    notes: 'Initial material requirement for production start',
    status: 'Issued',
    requestedBy: 'Production Control',
    requestedDate: '2025-09-08 08:15',
    processedDate: '2025-09-08 09:00',
    processedBy: 'Warehouse Staff 1'
  },
  {
    id: 'RMR-REQ-002',
    refNo: 'RMR-20250908-002',
    jobId: 'JOB20250903-002',
    machine: 'INJ-M002',
    product: '2011 - Plastic Bottle 500ml',
    materialCode: 'PP002',
    materialName: 'PP Blue Compound',
    color: 'Blue',
    requiredQty: 25.5,
    reservedQty: 25.5,
    issueQty: 20.0,
    issuedQty: 20.0,
    warehouseBreakdown: [
      { warehouseCode: 'RM-WH2', warehouseName: 'Raw Material Warehouse B', qty: 10.0 },
      { warehouseCode: 'RM-WH3', warehouseName: 'Raw Material Warehouse C', qty: 10.0 }
    ],
    unit: 'kg',
    notes: '',
    status: 'Issued',
    requestedBy: 'Production Control',
    requestedDate: '2025-09-08 09:30',
    processedDate: '2025-09-08 10:15',
    processedBy: 'Warehouse Staff 1'
  },
  {
    id: 'RMR-REQ-003',
    refNo: 'RMR-20250908-003',
    jobId: 'JOB20250903-004',
    machine: 'INJ-M004',
    product: '2011 - Plastic Bottle 500ml',
    materialCode: 'PET001',
    materialName: 'PET Resin Clear',
    color: 'Clear',
    requiredQty: 32.0,
    reservedQty: 32.0,
    issueQty: 15.0,
    issuedQty: 15.0,
    warehouseBreakdown: [
      { warehouseCode: 'RM-WH1', warehouseName: 'Raw Material Warehouse A', qty: 15.0 }
    ],
    unit: 'kg',
    notes: 'Urgent production order',
    status: 'Completed',
    requestedBy: 'Production Control',
    requestedDate: '2025-09-08 11:45',
    processedDate: '2025-09-08 12:30',
    processedBy: 'Warehouse Staff 2'
  },
  {
    id: 'RMR-REQ-004',
    refNo: 'RMR-20250908-004',
    jobId: 'JOB20250903-003',
    machine: 'INJ-M003',
    product: '2012 - Plastic Container 1L',
    materialCode: 'PP002',
    materialName: 'PP Blue Compound',
    color: 'Blue',
    requiredQty: 18.0,
    reservedQty: 18.0,
    issueQty: 18.0,
    issuedQty: 18.0,
    warehouseBreakdown: [
      { warehouseCode: 'RM-WH3', warehouseName: 'Raw Material Warehouse C', qty: 18.0 }
    ],
    unit: 'kg',
    notes: 'Quality checked material needed',
    status: 'Rejected',
    requestedBy: 'Production Control',
    requestedDate: '2025-09-08 13:20',
    processedDate: '2025-09-08 14:00',
    processedBy: 'Warehouse Supervisor'
  }
];

export function ProductionControl({ currentPage, onPageChange }: ProductionControlProps) {
  // If reject-management is selected, render the dedicated RejectManagement component
  if (currentPage === 'reject-management') {
    return <RejectManagement />;
  }

  // If finished-goods-transfer is selected, render the dedicated FinishedGoodsTransfer component
  if (currentPage === 'finished-goods-transfer') {
    return <FinishedGoodsTransfer />;
  }

  // If job-tracker is selected, render the dedicated JobTracker component
  if (currentPage === 'job-tracker') {
    return <JobTracker />;
  }

  // If waste-management is selected, render the dedicated WasteManagement component
  if (currentPage === 'waste-management') {
    return <WasteManagement />;
  }

  // If split-job-management is selected, render the dedicated SplitJobManagement component
  if (currentPage === 'split-job-management') {
    return <SplitJobManagement />;
  }

  // Issue Requests state
  const [issueRequests, setIssueRequests] = useState(mockIssueRequests);

  // Demand Authorization state
  const [expandedPlans, setExpandedPlans] = useState<string[]>([]);
  const [selectedPlan, setSelectedPlan] = useState<any>(null);
  const [showPlanDetail, setShowPlanDetail] = useState(false);
  const [planToReject, setPlanToReject] = useState<any>(null);
  const [showRejectDialog, setShowRejectDialog] = useState(false);
  const [rejectReason, setRejectReason] = useState('');

  // Job-to-Job Transfer state
  const [jobTransferDialog, setJobTransferDialog] = useState(false);
  const [jobTransferFormData, setJobTransferFormData] = useState({
    fromJobId: '',
    toJobId: '',
    materialCode: '',
    transferQty: '',
    reason: ''
  });

  // Get available materials for selected "From Job"
  const getAvailableMaterials = (jobId: string) => {
    const job = mockActiveJobsWithMaterials.find(j => j.jobId === jobId);
    return job?.reservedMaterials || [];
  };

  // Get available "To Jobs" (excluding the selected "From Job")
  const getAvailableToJobs = (fromJobId: string) => {
    return mockActiveJobsWithMaterials.filter(j => j.jobId !== fromJobId);
  };

  // Handle Job-to-Job Transfer submission
  const handleJobTransferSubmit = () => {
    const { fromJobId, toJobId, materialCode, transferQty, reason } = jobTransferFormData;
    
    if (!fromJobId || !toJobId || !materialCode || !transferQty || !reason) {
      toast.error('Please fill in all required fields');
      return;
    }

    // Find the material details
    const fromJob = mockActiveJobsWithMaterials.find(j => j.jobId === fromJobId);
    const material = fromJob?.reservedMaterials.find(m => m.materialCode === materialCode);
    
    if (!material) {
      toast.error('Material not found');
      return;
    }

    if (parseFloat(transferQty) > material.qty) {
      toast.error(`Transfer quantity cannot exceed reserved quantity (${material.qty} ${material.unit})`);
      return;
    }

    // Generate new request
    const newRequest = {
      id: `JT-REQ-${Date.now()}`,
      refNo: `JT-${new Date().toISOString().slice(0, 10).replace(/-/g, '')}-${Math.floor(Math.random() * 1000).toString().padStart(3, '0')}`,
      fromJobId,
      fromMachine: fromJob?.machine || '',
      toJobId,
      toMachine: mockActiveJobsWithMaterials.find(j => j.jobId === toJobId)?.machine || '',
      materialCode,
      materialName: material.materialName,
      color: material.color,
      transferQty: parseFloat(transferQty),
      unit: material.unit,
      reason,
      status: 'Pending Acknowledgement (Warehouse)',
      requestedBy: 'Production Manager',
      requestedDate: new Date().toISOString().slice(0, 16).replace('T', ' '),
      processedDate: null,
      processedBy: null
    };

    // In a real app, this would be sent to the backend
    mockJobTransferRequests.push(newRequest);

    toast.success('Job-to-Job Transfer Request Submitted');
    setJobTransferDialog(false);
    setJobTransferFormData({
      fromJobId: '',
      toJobId: '',
      materialCode: '',
      transferQty: '',
      reason: ''
    });
  };

  // Additional state for Issue Requests
  const [issueRequestDialog, setIssueRequestDialog] = useState(false);
  const [issueRequestFormData, setIssueRequestFormData] = useState({
    jobId: '',
    materialCode: '',
    issueQty: '',
    notes: ''
  });

  // Get job details for auto-fill
  const getJobDetails = (jobId: string) => {
    return mockActiveJobsWithMaterials.find(job => job.jobId === jobId);
  };

  // Get reserved materials for selected job
  const getReservedMaterials = (jobId: string) => {
    const job = mockActiveJobsWithMaterials.find(j => j.jobId === jobId);
    return job?.reservedMaterials || [];
  };

  // Handle Raw Material Request submission
  const handleIssueRequestSubmit = () => {
    const { jobId, materialCode, issueQty, notes } = issueRequestFormData;
    
    if (!jobId || !materialCode || !issueQty) {
      toast.error('Please fill in all required fields');
      return;
    }

    const job = getJobDetails(jobId);
    const material = getReservedMaterials(jobId).find(m => m.materialCode === materialCode);
    
    if (!job || !material) {
      toast.error('Job or material not found');
      return;
    }

    if (parseFloat(issueQty) > material.qty) {
      toast.error(`Issue quantity cannot exceed reserved quantity (${material.qty} ${material.unit})`);
      return;
    }

    // Generate new raw material request
    const newRequest = {
      id: `RMR-REQ-${Date.now()}`,
      refNo: `RMR-${new Date().toISOString().slice(0, 10).replace(/-/g, '')}-${Math.floor(Math.random() * 1000).toString().padStart(3, '0')}`,
      jobId,
      machine: job.machine,
      product: job.product,
      materialCode,
      materialName: material.materialName,
      color: material.color,
      requiredQty: material.qty,
      reservedQty: material.qty,
      issueQty: parseFloat(issueQty),
      issuedQty: 0,
      unit: material.unit,
      notes: notes || '',
      status: 'Pending',
      requestedBy: 'Production Control',
      requestedDate: new Date().toISOString().slice(0, 16).replace('T', ' '),
      processedDate: null,
      processedBy: null
    };

    // Add to the state
    setIssueRequests([...issueRequests, newRequest]);

    toast.success('✅ Raw Material Request Created Successfully! Request is ready to be submitted to Inventory → Raw Material Warehouse.');
    setIssueRequestDialog(false);
    setIssueRequestFormData({
      jobId: '',
      materialCode: '',
      issueQty: '',
      notes: ''
    });
  };

  // Handle Return Leftover submission
  const handleReturnLeftoverSubmit = () => {
    const { jobId, materialCode, returnedQty, reason } = returnLeftoverFormData;
    
    if (!jobId || !materialCode || !returnedQty || !reason) {
      toast.error('Please fill in all required fields');
      return;
    }

    const job = getJobDetails(jobId);
    const material = getReservedMaterials(jobId).find(m => m.materialCode === materialCode);
    
    if (!job || !material) {
      toast.error('Job or material not found');
      return;
    }

    if (parseFloat(returnedQty) <= 0) {
      toast.error('Return quantity must be greater than 0');
      return;
    }

    // Generate new return leftover request
    const newRequest = {
      id: `RL-REQ-${Date.now()}`,
      refNo: `RL-${new Date().toISOString().slice(0, 10).replace(/-/g, '')}-${Math.floor(Math.random() * 1000).toString().padStart(3, '0')}`,
      jobId,
      machine: job.machine,
      product: job.product,
      materialCode,
      materialName: material.materialName,
      color: material.color,
      issuedQty: material.qty,
      returnedQty: parseFloat(returnedQty),
      unit: material.unit,
      reason,
      status: 'Pending',
      requestedBy: 'Production Manager',
      requestedDate: new Date().toISOString().slice(0, 16).replace('T', ' '),
      processedDate: null,
      processedBy: null
    };

    // In a real app, this would be sent to the backend
    mockReturnLeftoverRequests.push(newRequest);

    toast.success('Return Leftover Request Submitted Successfully! Request sent to warehouse for approval.');
    setReturnLeftoverDialog(false);
    setReturnLeftoverFormData({
      jobId: '',
      materialCode: '',
      returnedQty: '',
      reason: ''
    });
  };

  // Handle Request Material action
  const handleRequestMaterial = (requestId: string) => {
    // Find the request to update
    const requestIndex = issueRequests.findIndex(req => req.id === requestId);
    if (requestIndex === -1) {
      toast.error('Request not found');
      return;
    }

    const request = issueRequests[requestIndex];
    
    if (request.issueQty <= 0) {
      toast.error('Please enter a valid Issue Qty before submitting request');
      return;
    }

    if (request.issueQty > request.reservedQty) {
      toast.error(`Issue quantity cannot exceed reserved quantity (${request.reservedQty} ${request.unit})`);
      return;
    }

    // Update status to indicate it's been sent to warehouse
    const updatedRequests = [...issueRequests];
    updatedRequests[requestIndex] = {
      ...request,
      status: 'Submitted to Warehouse',
      submittedDate: new Date().toISOString().slice(0, 16).replace('T', ' ')
    };
    setIssueRequests(updatedRequests);

    // In a real app, this would:
    // 1. Update the request in the database
    // 2. Create an entry in the Raw Material Warehouse Stock Out Requests
    // 3. Send notification to warehouse team

    toast.success('✅ Material request submitted to Warehouse.');
  };

  // Handle Demand Authorization actions
  const handleApproveDemand = (planId: string, jobId?: string) => {
    const demandIndex = mockProductionDemands.findIndex(demand => demand.planId === planId);
    if (demandIndex === -1) {
      toast.error('Production plan not found');
      return;
    }

    // Update status to approved and assign job ID
    mockProductionDemands[demandIndex] = {
      ...mockProductionDemands[demandIndex],
      status: 'approved',
      jobId: jobId || `JOB${Date.now()}`
    };

    toast.success(`✅ Production Plan ${planId} has been approved and Job ${mockProductionDemands[demandIndex].jobId} created.`);
  };

  const handleRejectDemand = (planId: string, reason: string) => {
    const demandIndex = mockProductionDemands.findIndex(demand => demand.planId === planId);
    if (demandIndex === -1) {
      toast.error('Production plan not found');
      return;
    }

    // Update status to rejected with reason
    mockProductionDemands[demandIndex] = {
      ...mockProductionDemands[demandIndex],
      status: 'rejected',
      rejectionReason: reason
    };

    toast.success(`❌ Production Plan ${planId} has been rejected.`);
  };

  // Handler functions for the new demand authorization section
  const handleApprovePlan = (plan: any) => {
    const jobId = `JOB${new Date().toISOString().slice(0, 10).replace(/-/g, '')}-${Math.floor(Math.random() * 1000).toString().padStart(3, '0')}`;
    
    // In a real app, this would update the database
    // For now, we'll just show a success message
    toast.success(`✅ Plan ${plan.planId} approved and Job ${jobId} created!`);
  };

  const handleRejectPlan = (plan: any, reason: string) => {
    // In a real app, this would update the database
    // For now, we'll just show a success message
    toast.success(`❌ Plan ${plan.planId} rejected. Reason: ${reason}`);
    setShowRejectDialog(false);
    setPlanToReject(null);
  };

  // Demand Authorization state
  const [demandDialog, setDemandDialog] = useState(false);
  const [selectedDemand, setSelectedDemand] = useState<any>(null);
  const [rejectionReason, setRejectionReason] = useState('');

  // Return Leftover state
  const [returnLeftoverDialog, setReturnLeftoverDialog] = useState(false);
  const [returnLeftoverFormData, setReturnLeftoverFormData] = useState({
    jobId: '',
    materialCode: '',
    returnedQty: '',
    reason: ''
  });



  // Render Material Control section - simplified for PC role
  if (currentPage === 'material-control') {
    return (
      <div className="space-y-6">
        {/* Top Action Buttons */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Package className="h-5 w-5 text-green-600" />
              Raw Material Control
              <span className="text-sm font-normal text-slate-600">ကုန်ကြမ်း ထိန်းချုပ်မှု</span>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="flex gap-3">
              <Button 
                className="bg-blue-600 hover:bg-blue-700 text-white"
                onClick={() => setIssueRequestDialog(true)}
                title="➕ New Raw Material Request = Create a new material request."
              >
                <Plus className="h-4 w-4 mr-2" />
                ➕ New Raw Material Request
              </Button>
              <Button 
                variant="outline"
                className="border-purple-600 text-purple-600 hover:bg-purple-50"
                onClick={() => setJobTransferDialog(true)}
              >
                <ArrowRightLeft className="h-4 w-4 mr-2" />
                🔁 Job-to-Job Transfer
              </Button>
              <Button 
                variant="outline"
                className="border-orange-600 text-orange-600 hover:bg-orange-50"
                onClick={() => setReturnLeftoverDialog(true)}
                title="🔄 Return Leftover = Return unused materials back to warehouse"
              >
                <RotateCcw className="h-4 w-4 mr-2" />
                🔄 Return Leftover
              </Button>
            </div>
          </CardContent>
        </Card>

        {/* Raw Material Requests Table */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <ArrowRight className="h-5 w-5 text-blue-600" />
              Raw Material Requests
              <span className="text-sm font-normal text-slate-600">ကုန်ကြမ်းတောင်းခံမှုများ</span>
              <Badge className="bg-blue-100 text-blue-800 ml-auto">
                {issueRequests.length} requests
              </Badge>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <TooltipProvider>
                <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Ref No.</TableHead>
                    <TableHead>Job ID</TableHead>
                    <TableHead>Product</TableHead>
                    <TableHead>Material + Color</TableHead>
                    <TableHead>Required Qty</TableHead>
                    <TableHead>Reserved Qty</TableHead>
                    <TableHead>Issued Qty (Total)</TableHead>
                    <TableHead className="min-w-[250px]">
                      Issued Qty by Warehouse
                      <div className="text-xs font-normal text-slate-500 mt-1">
                        ဂိုဒေါင်အလိုက် ထုတ်ပေးသည့်အရေအတွက်
                      </div>
                    </TableHead>
                    <TableHead>Status</TableHead>
                    <TableHead>Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {issueRequests.map((request) => (
                    <TableRow key={request.id}>
                      <TableCell className="font-mono text-sm">{request.refNo}</TableCell>
                      <TableCell className="font-medium">{request.jobId}</TableCell>
                      <TableCell className="max-w-[200px]">
                        <div className="truncate" title={request.product}>
                          {request.product}
                        </div>
                      </TableCell>
                      <TableCell>
                        <div>
                          <div className="font-medium">{request.materialCode}</div>
                          <div className="text-sm text-slate-600">{request.materialName} - {request.color}</div>
                        </div>
                      </TableCell>
                      <TableCell>{request.requiredQty} {request.unit}</TableCell>
                      <TableCell>{request.reservedQty} {request.unit}</TableCell>
                      <TableCell>
                        {request.status === 'Pending' ? (
                          <Input
                            type="number"
                            step="0.1"
                            value={request.issueQty}
                            onChange={(e) => {
                              const updatedRequests = issueRequests.map(req => 
                                req.id === request.id 
                                  ? { ...req, issueQty: parseFloat(e.target.value) || 0 }
                                  : req
                              );
                              setIssueRequests(updatedRequests);
                            }}
                            className="w-20 text-center border-blue-200 focus:border-blue-400"
                            max={request.reservedQty}
                          />
                        ) : (
                          <div className="flex flex-col">
                            <span className="font-medium text-blue-600">
                              {request.issuedQty} {request.unit}
                            </span>
                            {request.issuedQty !== request.issueQty && (
                              <span className="text-xs text-slate-500">
                                (Req: {request.issueQty} {request.unit})
                              </span>
                            )}
                          </div>
                        )}
                      </TableCell>
                      <TableCell className="max-w-[250px]">
                        {request.status === 'Pending' ? (
                          <div className="text-center text-slate-400 py-4">
                            <Clock className="h-4 w-4 mx-auto mb-1" />
                            <div className="text-xs">Waiting for issue</div>
                          </div>
                        ) : request.warehouseBreakdown && request.warehouseBreakdown.length > 0 ? (
                            <Tooltip>
                              <TooltipTrigger asChild>
                                <div className="space-y-1 cursor-pointer">
                                  {/* Warehouse breakdown tags */}
                                  <div className="flex flex-wrap gap-1">
                                    {request.warehouseBreakdown.map((warehouse, index) => (
                                      <Badge 
                                        key={index}
                                        variant="outline" 
                                        className={`text-xs px-2 py-1 border hover:shadow-md transition-shadow ${getWarehouseColor(warehouse.warehouseCode)}`}
                                      >
                                        <div className="flex items-center gap-1">
                                          <MapPin className="h-3 w-3" />
                                          <span className="font-mono">{warehouse.warehouseCode}</span>
                                          <span className="font-semibold">{warehouse.qty} {request.unit}</span>
                                        </div>
                                      </Badge>
                                    ))}
                                  </div>
                                  
                                  {/* Total validation */}
                                  {!validateWarehouseTotal(request.warehouseBreakdown, request.issuedQty) && (
                                    <div className="flex items-center gap-1 text-red-600 text-xs mt-2 p-1 bg-red-50 rounded border border-red-200">
                                      <AlertTriangle className="h-3 w-3" />
                                      <span>⚠️ Total mismatch!</span>
                                    </div>
                                  )}
                                  
                                  {/* Summary line */}
                                  <div className="text-xs text-slate-600 mt-1 border-t pt-1">
                                    📦 Total from {request.warehouseBreakdown.length} warehouse{request.warehouseBreakdown.length > 1 ? 's' : ''}
                                  </div>
                                </div>
                              </TooltipTrigger>
                              <TooltipContent side="left" className="max-w-sm">
                                <div className="space-y-2">
                                  <div className="font-semibold text-sm border-b pb-1">
                                    Warehouse Breakdown Details
                                  </div>
                                  {request.warehouseBreakdown.map((warehouse, index) => (
                                    <div key={index} className="flex items-center justify-between gap-4 text-sm">
                                      <div className="flex items-center gap-2">
                                        <div className={`w-3 h-3 rounded border ${getWarehouseColor(warehouse.warehouseCode).split(' ')[0]} ${getWarehouseColor(warehouse.warehouseCode).split(' ')[2]}`}></div>
                                        <span className="font-mono text-xs">{warehouse.warehouseCode}</span>
                                      </div>
                                      <div className="text-right">
                                        <div className="font-semibold">{warehouse.qty} {request.unit}</div>
                                        <div className="text-xs text-slate-500">{warehouse.warehouseName}</div>
                                      </div>
                                    </div>
                                  ))}
                                  <div className="border-t pt-2 mt-2">
                                    <div className="flex justify-between items-center font-semibold">
                                      <span>Total Issued:</span>
                                      <span className="text-blue-600">{request.issuedQty} {request.unit}</span>
                                    </div>
                                    {request.issuedQty !== request.issueQty && (
                                      <div className="flex justify-between items-center text-xs text-slate-600">
                                        <span>Originally Requested:</span>
                                        <span>{request.issueQty} {request.unit}</span>
                                      </div>
                                    )}
                                  </div>
                                  {!validateWarehouseTotal(request.warehouseBreakdown, request.issuedQty) && (
                                    <div className="text-red-600 text-xs bg-red-50 p-2 rounded border">
                                      ⚠️ Warning: Warehouse breakdown total ({request.warehouseBreakdown.reduce((sum, wh) => sum + wh.qty, 0)} {request.unit}) does not match issued quantity ({request.issuedQty} {request.unit})
                                    </div>
                                  )}
                                </div>
                              </TooltipContent>
                            </Tooltip>
                        ) : (
                          <div className="text-center text-slate-400 py-2">
                            <Package className="h-4 w-4 mx-auto mb-1" />
                            <div className="text-xs">No breakdown available</div>
                          </div>
                        )}
                      </TableCell>
                      <TableCell>
                        <Badge 
                          variant={
                            request.status === 'Issued' ? 'default' :
                            request.status === 'Completed' ? 'secondary' :
                            request.status === 'Pending' ? 'secondary' :
                            'destructive'
                          }
                          className={
                            request.status === 'Issued' ? 'bg-green-100 text-green-800' :
                            request.status === 'Completed' ? 'bg-slate-100 text-slate-800' :
                            request.status === 'Pending' ? 'bg-yellow-100 text-yellow-800' :
                            'bg-red-100 text-red-800'
                          }
                        >
                          {request.status === 'Issued' ? '🟢' : 
                           request.status === 'Completed' ? '⚪' :
                           request.status === 'Pending' ? '🟡' : '🔴'} 
                          {request.status}
                        </Badge>
                      </TableCell>
                      <TableCell>
                        <div className="flex gap-1">
                          <Button 
                            size="sm" 
                            variant="ghost"
                            title="View request details"
                          >
                            <Eye className="h-4 w-4" />
                          </Button>
                          
                          {/* Only show Submit Request for Pending status */}
                          {request.status === 'Pending' && (
                            <Button 
                              size="sm" 
                              variant="outline" 
                              className="text-blue-600 border-blue-600 hover:bg-blue-50"
                              title="Submit this request to Inventory → Raw Material Warehouse"
                              onClick={() => {
                                const updatedRequests = issueRequests.map(req => 
                                  req.id === request.id 
                                    ? { ...req, status: 'Issued' }
                                    : req
                                );
                                setIssueRequests(updatedRequests);
                                toast.success('📤 Material request submitted to Warehouse');
                              }}
                            >
                              <ArrowRight className="h-4 w-4 mr-1" />
                              📤 Submit Request
                            </Button>
                          )}
                        </div>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
              </TooltipProvider>
            </div>
          </CardContent>
        </Card>

        {/* Job-to-Job Transfer Requests Table */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <ArrowRightLeft className="h-5 w-5 text-purple-600" />
              Job-to-Job Transfer Requests
              <span className="text-sm font-normal text-slate-600">အလုပ်မှအလုပ်သို့ လွှဲပြောင်းတောင်းခံမှုများ</span>
              <Badge className="bg-purple-100 text-purple-800 ml-auto">
                {mockJobTransferRequests.length} requests
              </Badge>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Ref No.</TableHead>
                    <TableHead>From Job (Machine)</TableHead>
                    <TableHead>To Job (Machine)</TableHead>
                    <TableHead>Material + Color</TableHead>
                    <TableHead>Qty</TableHead>
                    <TableHead>Reason</TableHead>
                    <TableHead>Status</TableHead>
                    <TableHead>Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {mockJobTransferRequests.map((request) => (
                    <TableRow key={request.id}>
                      <TableCell className="font-mono text-sm">{request.refNo}</TableCell>
                      <TableCell>
                        <div>
                          <div className="font-medium">{request.fromJobId}</div>
                          <div className="text-sm text-slate-600">({request.fromMachine})</div>
                        </div>
                      </TableCell>
                      <TableCell>
                        <div>
                          <div className="font-medium">{request.toJobId}</div>
                          <div className="text-sm text-slate-600">({request.toMachine})</div>
                        </div>
                      </TableCell>
                      <TableCell>
                        <div>
                          <div className="font-medium">{request.materialCode}</div>
                          <div className="text-sm text-slate-600">{request.materialName} - {request.color}</div>
                        </div>
                      </TableCell>
                      <TableCell className="font-medium text-purple-600">
                        {request.transferQty} {request.unit}
                      </TableCell>
                      <TableCell>
                        <Badge variant="outline">{request.reason}</Badge>
                      </TableCell>
                      <TableCell>
                        <Badge 
                          variant={
                            request.status === 'Acknowledged' ? 'default' :
                            request.status === 'Pending Acknowledgement (Warehouse)' ? 'secondary' :
                            'destructive'
                          }
                          className={
                            request.status === 'Acknowledged' ? 'bg-green-100 text-green-800' :
                            request.status === 'Pending Acknowledgement (Warehouse)' ? 'bg-yellow-100 text-yellow-800' :
                            'bg-red-100 text-red-800'
                          }
                        >
                          {request.status === 'Acknowledged' ? '🟢' : 
                           request.status === 'Pending Acknowledgement (Warehouse)' ? '🟡' : '🔴'} 
                          {request.status === 'Acknowledged' ? 'Acknowledged' : 
                           request.status === 'Pending Acknowledgement (Warehouse)' ? 'Pending' : 'Rejected'}
                        </Badge>
                      </TableCell>
                      <TableCell>
                        <Button 
                          size="sm" 
                          variant="ghost"
                          title="👁 View Details = View transfer request details"
                        >
                          <Eye className="h-4 w-4" />
                          👁 View Details
                        </Button>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          </CardContent>
        </Card>

        {/* Return Leftover Requests Table */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <RotateCcw className="h-5 w-5 text-orange-600" />
              Return Leftover Requests (By Job ID)
              <span className="text-sm font-normal text-slate-600">ကျန်ကုန်ကြမ်း ပြန်လည်အပ်နှံတောင်းခံမှုများ</span>
              <Badge className="bg-orange-100 text-orange-800 ml-auto">
                {mockReturnLeftoverRequests.length} requests
              </Badge>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Ref No.</TableHead>
                    <TableHead>Job ID</TableHead>
                    <TableHead>Machine No.</TableHead>
                    <TableHead>Material + Color</TableHead>
                    <TableHead>Issued Qty</TableHead>
                    <TableHead>Returned Qty</TableHead>
                    <TableHead>Reason</TableHead>
                    <TableHead>Status</TableHead>
                    <TableHead>Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {mockReturnLeftoverRequests.map((request) => (
                    <TableRow key={request.id}>
                      <TableCell className="font-mono text-sm">{request.refNo}</TableCell>
                      <TableCell>
                        <div className="font-medium text-blue-600">{request.jobId}</div>
                        <div className="text-sm text-slate-600 max-w-[150px] truncate" title={request.product}>
                          {request.product}
                        </div>
                      </TableCell>
                      <TableCell className="font-medium">{request.machine}</TableCell>
                      <TableCell>
                        <div>
                          <div className="font-medium">{request.materialCode}</div>
                          <div className="text-sm text-slate-600">{request.materialName} - {request.color}</div>
                        </div>
                      </TableCell>
                      <TableCell className="font-medium text-slate-700">
                        {request.issuedQty} {request.unit}
                      </TableCell>
                      <TableCell>
                        {request.status === 'Pending' ? (
                          <Input
                            type="number"
                            step="0.1"
                            value={request.returnedQty}
                            onChange={(e) => {
                              const updatedRequests = mockReturnLeftoverRequests.map(req => 
                                req.id === request.id 
                                  ? { ...req, returnedQty: parseFloat(e.target.value) || 0 }
                                  : req
                              );
                              // In real app, this would update the state
                            }}
                            className="w-20 text-center border-orange-200 focus:border-orange-400"
                            max={request.issuedQty}
                          />
                        ) : (
                          <span className="font-medium text-orange-600">
                            {request.returnedQty} {request.unit}
                          </span>
                        )}
                      </TableCell>
                      <TableCell>
                        <Badge variant="outline" className="text-xs">
                          {request.reason}
                        </Badge>
                      </TableCell>
                      <TableCell>
                        <Badge 
                          variant={
                            request.status === 'Approved' ? 'default' :
                            request.status === 'Pending' ? 'secondary' :
                            'destructive'
                          }
                          className={
                            request.status === 'Approved' ? 'bg-green-100 text-green-800' :
                            request.status === 'Pending' ? 'bg-yellow-100 text-yellow-800' :
                            'bg-red-100 text-red-800'
                          }
                        >
                          {request.status === 'Approved' ? '🟢' : 
                           request.status === 'Pending' ? '🟡' : '🔴'} 
                          {request.status}
                        </Badge>
                      </TableCell>
                      <TableCell>
                        <div className="flex gap-1">
                          <Button 
                            size="sm" 
                            variant="ghost"
                            title="👁 View = View return request details"
                          >
                            <Eye className="h-4 w-4" />
                            👁 View
                          </Button>
                          {request.status === 'Pending' && (
                            <Button 
                              size="sm" 
                              variant="outline" 
                              className="text-orange-600 border-orange-600 hover:bg-orange-50"
                              title="Submit this return request to Warehouse for approval."
                              onClick={() => {
                                if (request.returnedQty <= 0) {
                                  toast.error('Please enter a valid Return Qty before submitting request');
                                  return;
                                }
                                // In real app, this would update the request status and send to warehouse
                                toast.success('🔄 Return Leftover request submitted to Warehouse for approval.');
                              }}
                            >
                              <RotateCcw className="h-4 w-4 mr-1" />
                              🔄 Request Return
                            </Button>
                          )}
                          {request.status === 'Rejected' && request.rejectionReason && (
                            <Button 
                              size="sm" 
                              variant="ghost"
                              className="text-red-600"
                              title={`Rejection Reason: ${request.rejectionReason}`}
                            >
                              <AlertCircle className="h-4 w-4" />
                              Reason
                            </Button>
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

        {/* Raw Material Request Modal */}
        <Dialog open={issueRequestDialog} onOpenChange={setIssueRequestDialog}>
          <DialogContent className="max-w-2xl">
            <DialogHeader>
              <DialogTitle className="flex items-center gap-2">
                <Plus className="h-5 w-5 text-blue-600" />
                New Raw Material Request
              </DialogTitle>
              <DialogDescription>
                Create a new raw material request for production job - will be sent directly to Inventory → Raw Material Warehouse
              </DialogDescription>
            </DialogHeader>
            <div className="space-y-4">
              <div>
                <Label>Job ID *</Label>
                <Select
                  value={issueRequestFormData.jobId}
                  onValueChange={(value) => {
                    setIssueRequestFormData(prev => ({
                      ...prev,
                      jobId: value,
                      materialCode: '', // Reset material when job changes
                      issueQty: ''
                    }));
                  }}
                >
                  <SelectTrigger>
                    <SelectValue placeholder="Select active job" />
                  </SelectTrigger>
                  <SelectContent>
                    {mockActiveJobsWithMaterials.map((job) => (
                      <SelectItem key={job.jobId} value={job.jobId}>
                        {job.jobId} - {job.product}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>

              {issueRequestFormData.jobId && (
                <div className="grid grid-cols-2 gap-4 p-4 bg-slate-50 rounded-lg">
                  <div>
                    <Label className="text-sm font-medium text-slate-600">Machine No. (auto)</Label>
                    <div className="font-mono text-sm">{getJobDetails(issueRequestFormData.jobId)?.machine}</div>
                  </div>
                  <div>
                    <Label className="text-sm font-medium text-slate-600">Product (auto)</Label>
                    <div className="text-sm">{getJobDetails(issueRequestFormData.jobId)?.product}</div>
                  </div>
                </div>
              )}

              <div>
                <Label>Material Code + Color *</Label>
                <Select
                  value={issueRequestFormData.materialCode}
                  onValueChange={(value) => {
                    setIssueRequestFormData(prev => ({ ...prev, materialCode: value, issueQty: '' }));
                  }}
                  disabled={!issueRequestFormData.jobId}
                >
                  <SelectTrigger>
                    <SelectValue placeholder="Select material" />
                  </SelectTrigger>
                  <SelectContent>
                    {getReservedMaterials(issueRequestFormData.jobId).map((material) => (
                      <SelectItem key={material.materialCode} value={material.materialCode}>
                        {material.materialCode} - {material.materialName} ({material.color})
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>

              {issueRequestFormData.materialCode && (
                <div className="grid grid-cols-2 gap-4 p-4 border border-slate-200 rounded-lg">
                  <div>
                    <Label className="text-sm font-medium text-slate-600">Required Qty (auto from plan)</Label>
                    <div className="font-medium">
                      {getReservedMaterials(issueRequestFormData.jobId)
                        .find(m => m.materialCode === issueRequestFormData.materialCode)?.qty || 0} kg
                    </div>
                  </div>
                  <div>
                    <Label className="text-sm font-medium text-slate-600">Reserved Qty</Label>
                    <div className="font-medium text-blue-600">
                      {getReservedMaterials(issueRequestFormData.jobId)
                        .find(m => m.materialCode === issueRequestFormData.materialCode)?.qty || 0} kg
                    </div>
                  </div>
                </div>
              )}

              <div>
                <Label>Issue Qty (editable by PC)</Label>
                <Input
                  type="number"
                  step="0.1"
                  placeholder="Enter quantity to issue"
                  value={issueRequestFormData.issueQty}
                  onChange={(e) => {
                    setIssueRequestFormData(prev => ({ ...prev, issueQty: e.target.value }));
                  }}
                  disabled={!issueRequestFormData.materialCode}
                />
                {issueRequestFormData.materialCode && (
                  <p className="text-xs text-slate-600 mt-1">
                    Max: {getReservedMaterials(issueRequestFormData.jobId)
                      .find(m => m.materialCode === issueRequestFormData.materialCode)?.qty || 0} kg
                  </p>
                )}
              </div>

              <div>
                <Label>Notes (optional text)</Label>
                <Textarea
                  placeholder="Additional notes or instructions"
                  value={issueRequestFormData.notes}
                  onChange={(e) => {
                    setIssueRequestFormData(prev => ({ ...prev, notes: e.target.value }));
                  }}
                />
              </div>

              <div className="flex justify-end gap-2 pt-4">
                <Button 
                  variant="outline" 
                  onClick={() => setIssueRequestDialog(false)}
                >
                  Cancel
                </Button>
                <Button 
                  className="bg-blue-600 hover:bg-blue-700"
                  onClick={handleIssueRequestSubmit}
                >
                  <ArrowRight className="h-4 w-4 mr-2" />
                  Submit Request
                </Button>
              </div>
            </div>
          </DialogContent>
        </Dialog>

        {/* Job-to-Job Transfer Modal */}
        <Dialog open={jobTransferDialog} onOpenChange={setJobTransferDialog}>
          <DialogContent className="max-w-2xl">
            <DialogHeader>
              <DialogTitle className="flex items-center gap-2">
                <ArrowRightLeft className="h-5 w-5 text-purple-600" />
                Job-to-Job Transfer Modal
              </DialogTitle>
              <DialogDescription>
                Create a special transfer request for raw materials between jobs
              </DialogDescription>
            </DialogHeader>
            <div className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label>From Job ID</Label>
                  <Select
                    value={jobTransferFormData.fromJobId}
                    onValueChange={(value) => {
                      setJobTransferFormData(prev => ({
                        ...prev,
                        fromJobId: value,
                        materialCode: '', // Reset material when job changes
                        transferQty: ''
                      }));
                    }}
                  >
                    <SelectTrigger>
                      <SelectValue placeholder="Select source job" />
                    </SelectTrigger>
                    <SelectContent>
                      {mockActiveJobsWithMaterials
                        .filter(job => job.reservedMaterials.length > 0)
                        .map((job) => (
                        <SelectItem key={job.jobId} value={job.jobId}>
                          {job.jobId} - {job.machine}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  {jobTransferFormData.fromJobId && (
                    <p className="text-xs text-slate-600 mt-1">
                      Machine: {getJobDetails(jobTransferFormData.fromJobId)?.machine}
                    </p>
                  )}
                </div>

                <div>
                  <Label>To Job ID</Label>
                  <Select
                    value={jobTransferFormData.toJobId}
                    onValueChange={(value) => {
                      setJobTransferFormData(prev => ({ ...prev, toJobId: value }));
                    }}
                    disabled={!jobTransferFormData.fromJobId}
                  >
                    <SelectTrigger>
                      <SelectValue placeholder="Select destination job" />
                    </SelectTrigger>
                    <SelectContent>
                      {getAvailableToJobs(jobTransferFormData.fromJobId).map((job) => (
                        <SelectItem key={job.jobId} value={job.jobId}>
                          {job.jobId} - {job.machine}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  {jobTransferFormData.toJobId && (
                    <p className="text-xs text-slate-600 mt-1">
                      Machine: {getJobDetails(jobTransferFormData.toJobId)?.machine}
                    </p>
                  )}
                </div>
              </div>

              <div>
                <Label>Material + Color</Label>
                <Select
                  value={jobTransferFormData.materialCode}
                  onValueChange={(value) => {
                    setJobTransferFormData(prev => ({ ...prev, materialCode: value, transferQty: '' }));
                  }}
                  disabled={!jobTransferFormData.fromJobId}
                >
                  <SelectTrigger>
                    <SelectValue placeholder="Select material to transfer" />
                  </SelectTrigger>
                  <SelectContent>
                    {getAvailableMaterials(jobTransferFormData.fromJobId).map((material) => (
                      <SelectItem key={material.materialCode} value={material.materialCode}>
                        {material.materialCode} - {material.materialName} ({material.color}) - Available: {material.qty} {material.unit}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label>Transfer Qty</Label>
                  <Input
                    type="number"
                    step="0.1"
                    placeholder="Enter quantity"
                    value={jobTransferFormData.transferQty}
                    onChange={(e) => {
                      setJobTransferFormData(prev => ({ ...prev, transferQty: e.target.value }));
                    }}
                    disabled={!jobTransferFormData.materialCode}
                  />
                  {jobTransferFormData.materialCode && (
                    <p className="text-xs text-slate-600 mt-1">
                      Max: {getAvailableMaterials(jobTransferFormData.fromJobId)
                        .find(m => m.materialCode === jobTransferFormData.materialCode)?.qty || 0} kg
                    </p>
                  )}
                </div>

                <div>
                  <Label>Reason</Label>
                  <Select
                    value={jobTransferFormData.reason}
                    onValueChange={(value) => {
                      setJobTransferFormData(prev => ({ ...prev, reason: value }));
                    }}
                  >
                    <SelectTrigger>
                      <SelectValue placeholder="Select reason" />
                    </SelectTrigger>
                    <SelectContent>
                      {transferReasonOptions.map((reason) => (
                        <SelectItem key={reason} value={reason}>
                          {reason}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
              </div>

              <div className="flex justify-end gap-2 pt-4">
                <Button 
                  variant="outline" 
                  onClick={() => setJobTransferDialog(false)}
                >
                  Cancel
                </Button>
                <Button 
                  className="bg-purple-600 hover:bg-purple-700"
                  onClick={handleJobTransferSubmit}
                >
                  Submit Transfer Request
                </Button>
              </div>
            </div>
          </DialogContent>
        </Dialog>

        {/* Return Leftover Modal */}
        <Dialog open={returnLeftoverDialog} onOpenChange={setReturnLeftoverDialog}>
          <DialogContent className="max-w-2xl">
            <DialogHeader>
              <DialogTitle className="flex items-center gap-2">
                <RotateCcw className="h-5 w-5 text-orange-600" />
                Return Leftover (By Job ID)
              </DialogTitle>
              <DialogDescription>
                Select Job and enter leftover qty to return back to Raw Material Warehouse
              </DialogDescription>
            </DialogHeader>
            <div className="space-y-4">
              <div>
                <Label>Job ID</Label>
                <Select
                  value={returnLeftoverFormData.jobId}
                  onValueChange={(value) => {
                    setReturnLeftoverFormData(prev => ({
                      ...prev,
                      jobId: value,
                      materialCode: '', // Reset material when job changes
                      returnedQty: ''
                    }));
                  }}
                >
                  <SelectTrigger>
                    <SelectValue placeholder="Select job with issued materials" />
                  </SelectTrigger>
                  <SelectContent>
                    {mockActiveJobsWithMaterials
                      .filter(job => job.reservedMaterials.length > 0)
                      .map((job) => (
                      <SelectItem key={job.jobId} value={job.jobId}>
                        {job.jobId} - {job.product}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>

              {returnLeftoverFormData.jobId && (
                <div className="grid grid-cols-2 gap-4 p-4 bg-slate-50 rounded-lg">
                  <div>
                    <Label className="text-sm font-medium text-slate-600">Machine No.</Label>
                    <div className="font-mono text-sm">{getJobDetails(returnLeftoverFormData.jobId)?.machine}</div>
                  </div>
                  <div>
                    <Label className="text-sm font-medium text-slate-600">Product</Label>
                    <div className="text-sm">{getJobDetails(returnLeftoverFormData.jobId)?.product}</div>
                  </div>
                </div>
              )}

              <div>
                <Label>Material + Color</Label>
                <Select
                  value={returnLeftoverFormData.materialCode}
                  onValueChange={(value) => {
                    setReturnLeftoverFormData(prev => ({ ...prev, materialCode: value, returnedQty: '' }));
                  }}
                  disabled={!returnLeftoverFormData.jobId}
                >
                  <SelectTrigger>
                    <SelectValue placeholder="Select material to return" />
                  </SelectTrigger>
                  <SelectContent>
                    {getReservedMaterials(returnLeftoverFormData.jobId).map((material) => (
                      <SelectItem key={material.materialCode} value={material.materialCode}>
                        {material.materialCode} - {material.materialName} ({material.color})
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>

              {returnLeftoverFormData.materialCode && (
                <div className="grid grid-cols-1 gap-4 p-4 border border-slate-200 rounded-lg">
                  <div>
                    <Label className="text-sm font-medium text-slate-600">Issued Qty (Auto-fill)</Label>
                    <div className="font-medium text-blue-600">
                      {getReservedMaterials(returnLeftoverFormData.jobId)
                        .find(m => m.materialCode === returnLeftoverFormData.materialCode)?.qty || 0} kg
                    </div>
                  </div>
                </div>
              )}

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label>Returned Qty</Label>
                  <Input
                    type="number"
                    step="0.1"
                    placeholder="Enter leftover quantity"
                    value={returnLeftoverFormData.returnedQty}
                    onChange={(e) => {
                      setReturnLeftoverFormData(prev => ({ ...prev, returnedQty: e.target.value }));
                    }}
                    disabled={!returnLeftoverFormData.materialCode}
                  />
                  {returnLeftoverFormData.materialCode && (
                    <p className="text-xs text-slate-600 mt-1">
                      Max: {getReservedMaterials(returnLeftoverFormData.jobId)
                        .find(m => m.materialCode === returnLeftoverFormData.materialCode)?.qty || 0} kg
                    </p>
                  )}
                </div>

                <div>
                  <Label>Reason</Label>
                  <Select
                    value={returnLeftoverFormData.reason}
                    onValueChange={(value) => {
                      setReturnLeftoverFormData(prev => ({ ...prev, reason: value }));
                    }}
                  >
                    <SelectTrigger>
                      <SelectValue placeholder="Select reason" />
                    </SelectTrigger>
                    <SelectContent>
                      {returnReasonOptions.map((reason) => (
                        <SelectItem key={reason} value={reason}>
                          {reason}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
              </div>

              <div className="flex justify-end gap-2 pt-4">
                <Button 
                  variant="outline" 
                  onClick={() => setReturnLeftoverDialog(false)}
                >
                  Cancel
                </Button>
                <Button 
                  className="bg-orange-600 hover:bg-orange-700"
                  onClick={handleReturnLeftoverSubmit}
                >
                  <RotateCcw className="h-4 w-4 mr-2" />
                  Submit Request
                </Button>
              </div>
            </div>
          </DialogContent>
        </Dialog>
      </div>
    );
  }

  // Render Job Monitoring section
  if (currentPage === 'job-monitoring') {
    const runningJobs = mockJobMonitoringData.filter(job => job.status === 'Running').length;
    const completedJobs = mockJobMonitoringData.filter(job => job.status === 'Completed').length;
    const pausedJobs = mockJobMonitoringData.filter(job => job.status === 'Paused').length;
    const setupJobs = mockJobMonitoringData.filter(job => job.status === 'Setup').length;
    const totalAlerts = mockJobMonitoringData.reduce((sum, job) => sum + job.alertCount, 0);
    const avgOEE = (mockJobMonitoringData.reduce((sum, job) => sum + job.oeeScore, 0) / mockJobMonitoringData.length).toFixed(1);

    return (
      <div className="space-y-6">
        {/* Page Header */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Monitor className="h-5 w-5 text-blue-600" />
              Job Monitoring Dashboard
              <span className="text-sm font-normal text-slate-600">အလုပ်စောင့်ကြည့်ခြင်း ဒေါ်ရှ်ဘုတ်</span>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="flex gap-3 mb-6">
              <div className="flex items-center gap-2 bg-blue-50 px-3 py-2 rounded-lg">
                <Monitor className="h-4 w-4 text-blue-600" />
                <span className="text-sm">📊 Real-time job status monitoring and performance tracking</span>
              </div>
            </div>

            {/* KPI Summary Cards */}
            <div className="grid grid-cols-6 gap-4 mb-6">
              <Card>
                <CardContent className="p-4">
                  <div className="text-2xl font-bold text-green-600">{runningJobs}</div>
                  <div className="text-sm text-slate-600">🟢 Running Jobs</div>
                </CardContent>
              </Card>
              <Card>
                <CardContent className="p-4">
                  <div className="text-2xl font-bold text-blue-600">{completedJobs}</div>
                  <div className="text-sm text-slate-600">🔵 Completed</div>
                </CardContent>
              </Card>
              <Card>
                <CardContent className="p-4">
                  <div className="text-2xl font-bold text-yellow-600">{pausedJobs}</div>
                  <div className="text-sm text-slate-600">🟡 Paused</div>
                </CardContent>
              </Card>
              <Card>
                <CardContent className="p-4">
                  <div className="text-2xl font-bold text-purple-600">{setupJobs}</div>
                  <div className="text-sm text-slate-600">🟣 Setup</div>
                </CardContent>
              </Card>
              <Card>
                <CardContent className="p-4">
                  <div className="text-2xl font-bold text-red-600">{totalAlerts}</div>
                  <div className="text-sm text-slate-600">🔴 Active Alerts</div>
                </CardContent>
              </Card>
              <Card>
                <CardContent className="p-4">
                  <div className="text-2xl font-bold text-indigo-600">{avgOEE}%</div>
                  <div className="text-sm text-slate-600">📈 Avg OEE</div>
                </CardContent>
              </Card>
            </div>
          </CardContent>
        </Card>

        {/* Job Monitoring Table */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Factory className="h-5 w-5 text-slate-700" />
              Active Jobs List & Status
              <Badge className="bg-slate-100 text-slate-800 ml-auto">
                {mockJobMonitoringData.length} jobs
              </Badge>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Job ID</TableHead>
                    <TableHead>Product</TableHead>
                    <TableHead>Machine</TableHead>
                    <TableHead>Progress</TableHead>
                    <TableHead>Status</TableHead>
                    <TableHead>OEE</TableHead>
                    <TableHead>Operator</TableHead>
                    <TableHead>Alerts</TableHead>
                    <TableHead>Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {mockJobMonitoringData.map((job) => (
                    <TableRow key={job.jobId}>
                      <TableCell>
                        <div className="font-medium text-blue-600">{job.jobId}</div>
                        <div className="text-sm text-slate-600">Plan: {job.planId}</div>
                      </TableCell>
                      <TableCell>
                        <div className="font-medium">{job.productCode}</div>
                        <div className="text-sm text-slate-600 max-w-[150px] truncate" title={job.product}>
                          {job.product}
                        </div>
                      </TableCell>
                      <TableCell className="font-mono text-sm">{job.machine}</TableCell>
                      <TableCell>
                        <div className="space-y-1">
                          <div className="flex justify-between text-sm">
                            <span>{job.producedQty}/{job.targetQty}</span>
                            <span>{job.progressPercentage}%</span>
                          </div>
                          <Progress value={job.progressPercentage} className="h-2" />
                          <div className="text-xs text-slate-600">
                            Good: {job.goodQty} | Defect: {job.defectQty}
                          </div>
                        </div>
                      </TableCell>
                      <TableCell>
                        <Badge 
                          className={
                            job.status === 'Running' ? 'bg-green-100 text-green-800' :
                            job.status === 'Completed' ? 'bg-blue-100 text-blue-800' :
                            job.status === 'Paused' ? 'bg-yellow-100 text-yellow-800' :
                            job.status === 'Setup' ? 'bg-purple-100 text-purple-800' :
                            'bg-slate-100 text-slate-800'
                          }
                        >
                          {job.status === 'Running' ? '🟢' : 
                           job.status === 'Completed' ? '🔵' : 
                           job.status === 'Paused' ? '🟡' : 
                           job.status === 'Setup' ? '🟣' : '⚪'} 
                          {job.status}
                        </Badge>
                        <div className="text-xs text-slate-600 mt-1">
                          Eff: {job.efficiency}%
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="font-medium text-indigo-600">{job.oeeScore}%</div>
                        <div className="text-xs text-slate-600">
                          DT: {job.downtimeMinutes}min
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="text-sm">{job.operator.split(' ')[0]} {job.operator.split(' ')[1]}</div>
                        <div className="text-xs text-slate-600">{job.shift.split(' ')[0]} {job.shift.split(' ')[1]}</div>
                      </TableCell>
                      <TableCell>
                        {job.alertCount > 0 ? (
                          <Badge variant="outline" className="bg-red-50 text-red-700 border-red-200">
                            🚨 {job.alertCount} Alert{job.alertCount > 1 ? 's' : ''}
                          </Badge>
                        ) : (
                          <Badge variant="outline" className="bg-green-50 text-green-700 border-green-200">
                            ✅ Normal
                          </Badge>
                        )}
                      </TableCell>
                      <TableCell>
                        <div className="flex gap-1">
                          <Button 
                            size="sm" 
                            variant="ghost"
                            title="👁 View = View detailed job information"
                          >
                            <Eye className="h-4 w-4" />
                            👁 View
                          </Button>
                          {job.status === 'Running' && (
                            <Button 
                              size="sm" 
                              variant="outline" 
                              className="text-yellow-600 border-yellow-600 hover:bg-yellow-50"
                              title="⏸ Pause job"
                            >
                              <Pause className="h-4 w-4" />
                            </Button>
                          )}
                          {job.status === 'Paused' && (
                            <Button 
                              size="sm" 
                              variant="outline" 
                              className="text-green-600 border-green-600 hover:bg-green-50"
                              title="▶ Resume job"
                            >
                              <Play className="h-4 w-4" />
                            </Button>
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

        {/* Detailed Job Cards Grid */}
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          {mockJobMonitoringData.slice(0, 4).map((job) => (
            <Card key={job.jobId} className="relative">
              {job.alertCount > 0 && (
                <div className="absolute top-3 right-3">
                  <Badge variant="outline" className="bg-red-50 text-red-700 border-red-200">
                    🚨 {job.alertCount}
                  </Badge>
                </div>
              )}
              <CardHeader className="pb-3">
                <CardTitle className="flex items-center gap-2 text-lg">
                  <div className="flex items-center gap-2">
                    <div className={`w-3 h-3 rounded-full ${
                      job.status === 'Running' ? 'bg-green-500' :
                      job.status === 'Completed' ? 'bg-blue-500' :
                      job.status === 'Paused' ? 'bg-yellow-500' :
                      job.status === 'Setup' ? 'bg-purple-500' : 'bg-slate-500'
                    }`} />
                    {job.jobId}
                  </div>
                  <Badge variant="outline" className={`text-xs ${
                    job.priority === 'High' ? 'border-red-200 text-red-700 bg-red-50' :
                    job.priority === 'Medium' ? 'border-yellow-200 text-yellow-700 bg-yellow-50' :
                    'border-green-200 text-green-700 bg-green-50'
                  }`}>
                    {job.priority}
                  </Badge>
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                {/* Job Basic Info */}
                <div className="grid grid-cols-2 gap-4 text-sm">
                  <div>
                    <Label className="text-slate-600">Product</Label>
                    <div className="font-medium">{job.productCode}</div>
                  </div>
                  <div>
                    <Label className="text-slate-600">Machine</Label>
                    <div className="font-mono">{job.machine}</div>
                  </div>
                  <div>
                    <Label className="text-slate-600">Operator</Label>
                    <div>{job.operator.split(' ')[0]} {job.operator.split(' ')[1]}</div>
                  </div>
                  <div>
                    <Label className="text-slate-600">Status</Label>
                    <div className={`font-medium ${
                      job.status === 'Running' ? 'text-green-600' :
                      job.status === 'Completed' ? 'text-blue-600' :
                      job.status === 'Paused' ? 'text-yellow-600' :
                      job.status === 'Setup' ? 'text-purple-600' : 'text-slate-600'
                    }`}>
                      {job.status}
                    </div>
                  </div>
                </div>

                {/* Progress Bar */}
                <div>
                  <div className="flex justify-between text-sm mb-2">
                    <Label className="text-slate-600">Progress</Label>
                    <span className="font-medium">{job.producedQty}/{job.targetQty} ({job.progressPercentage}%)</span>
                  </div>
                  <Progress value={job.progressPercentage} className="h-3" />
                  <div className="text-xs text-slate-600 mt-1 flex justify-between">
                    <span>Good: {job.goodQty}</span>
                    <span>Defect: {job.defectQty}</span>
                  </div>
                </div>

                {/* Performance Metrics */}
                <div className="grid grid-cols-3 gap-4 text-sm">
                  <div className="text-center p-2 bg-indigo-50 rounded-lg">
                    <div className="font-bold text-indigo-600">{job.oeeScore}%</div>
                    <div className="text-indigo-700 text-xs">OEE Score</div>
                  </div>
                  <div className="text-center p-2 bg-green-50 rounded-lg">
                    <div className="font-bold text-green-600">{job.efficiency}%</div>
                    <div className="text-green-700 text-xs">Efficiency</div>
                  </div>
                  <div className="text-center p-2 bg-yellow-50 rounded-lg">
                    <div className="font-bold text-yellow-600">{job.downtimeMinutes}m</div>
                    <div className="text-yellow-700 text-xs">Downtime</div>
                  </div>
                </div>

                {/* Material Status */}
                <div>
                  <Label className="text-slate-600">Raw Materials</Label>
                  <div className="space-y-1 mt-1">
                    {job.rawMaterials.map((material, index) => (
                      <div key={index} className="flex justify-between items-center text-xs bg-slate-50 p-2 rounded">
                        <span className="font-medium">{material.code}</span>
                        <span className="text-slate-600">
                          {material.remaining}/{material.required} {material.unit}
                        </span>
                        <div className={`px-2 py-1 rounded text-xs ${
                          material.remaining / material.required > 0.3 ? 'bg-green-100 text-green-700' :
                          material.remaining / material.required > 0.1 ? 'bg-yellow-100 text-yellow-700' :
                          'bg-red-100 text-red-700'
                        }`}>
                          {((material.remaining / material.required) * 100).toFixed(0)}%
                        </div>
                      </div>
                    ))}
                  </div>
                </div>

                {/* Recent Events */}
                <div>
                  <Label className="text-slate-600">Recent Activity</Label>
                  <div className="space-y-1 mt-1">
                    {job.recentEvents.slice(0, 2).map((event, index) => (
                      <div key={index} className="flex items-center gap-2 text-xs">
                        <span className="text-slate-500 font-mono">{event.time}</span>
                        <div className={`w-2 h-2 rounded-full ${
                          event.type === 'success' ? 'bg-green-500' :
                          event.type === 'warning' ? 'bg-yellow-500' :
                          event.type === 'error' ? 'bg-red-500' : 'bg-blue-500'
                        }`} />
                        <span className="flex-1">{event.event}</span>
                      </div>
                    ))}
                  </div>
                </div>

                {/* Action Buttons */}
                <div className="flex gap-2 pt-2">
                  <Button size="sm" variant="outline" className="flex-1">
                    <Eye className="h-4 w-4 mr-1" />
                    Details
                  </Button>
                  {job.status === 'Running' && (
                    <Button size="sm" variant="outline" className="text-yellow-600 border-yellow-600">
                      <Pause className="h-4 w-4" />
                    </Button>
                  )}
                  {job.status === 'Paused' && (
                    <Button size="sm" variant="outline" className="text-green-600 border-green-600">
                      <Play className="h-4 w-4" />
                    </Button>
                  )}
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      </div>
    );
  }

  // Render Demand Authorization section
  if (currentPage === 'demand-authorization') {
    // Mock data for Production Plans - with Myanmar names and parent-child relationships
    const mockProductionPlans = [
      {
        planId: 'PL20250903-001',
        planType: 'Parent',
        productCode: '2011',
        productName: 'Plastic Bottle 500ml',
        qty: 1000,
        unit: 'pcs',
        planner: 'ကိုသန့် (Ko Thant)',
        submittedDate: '2025-09-03',
        priority: 'High',
        status: 'Pending',
        rawMaterials: [
          { materialCode: 'PET001', materialName: 'PET Resin Clear', requiredQty: 25.0, unit: 'kg', availableStock: 150.0, shortage: 0 },
          { materialCode: 'ADD001', materialName: 'Additive Blue', requiredQty: 1.0, unit: 'kg', availableStock: 3.0, shortage: 0 },
          { materialCode: 'CAP001', materialName: 'Bottle Cap PP White', requiredQty: 1000, unit: 'pcs', availableStock: 6000, shortage: 0 },
          { materialCode: 'LAB001', materialName: 'Product Label', requiredQty: 1000, unit: 'pcs', availableStock: 4800, shortage: 0 }
        ],
        children: ['PL20250903-001-C1', 'PL20250903-001-C2'],
        hasShortage: false,
        jobId: null
      },
      {
        planId: 'PL20250903-001-C1',
        planType: 'Child',
        parentPlan: 'PL20250903-001',
        productCode: '2011',
        productName: 'Plastic Bottle 500ml - Batch 1',
        qty: 500,
        unit: 'pcs',
        planner: 'ကိုသန့် (Ko Thant)',
        submittedDate: '2025-09-03',
        priority: 'High',
        status: 'Pending',
        rawMaterials: [
          { materialCode: 'PET001', materialName: 'PET Resin Clear', requiredQty: 12.5, unit: 'kg', availableStock: 150.0, shortage: 0 },
          { materialCode: 'ADD001', materialName: 'Additive Blue', requiredQty: 0.5, unit: 'kg', availableStock: 3.0, shortage: 0 }
        ],
        children: [],
        hasShortage: false,
        jobId: null
      },
      {
        planId: 'PL20250903-001-C2',
        planType: 'Child',
        parentPlan: 'PL20250903-001',
        productCode: '2011',
        productName: 'Plastic Bottle 500ml - Batch 2',
        qty: 500,
        unit: 'pcs',
        planner: 'ကိုသန့် (Ko Thant)',
        submittedDate: '2025-09-03',
        priority: 'High',
        status: 'Pending',
        rawMaterials: [
          { materialCode: 'PET001', materialName: 'PET Resin Clear', requiredQty: 12.5, unit: 'kg', availableStock: 150.0, shortage: 0 },
          { materialCode: 'ADD001', materialName: 'Additive Blue', requiredQty: 0.5, unit: 'kg', availableStock: 3.0, shortage: 0 }
        ],
        children: [],
        hasShortage: false,
        jobId: null
      },
      {
        planId: 'PL20250903-002',
        planType: 'Parent',
        productCode: '2012',
        productName: 'Plastic Container 1L',
        qty: 500,
        unit: 'pcs',
        planner: 'မသန်းတာ (Ma Than Dar)',
        submittedDate: '2025-09-03',
        priority: 'Medium',
        status: 'Draft',
        rawMaterials: [
          { materialCode: 'PP002', materialName: 'PP White Compound', requiredQty: 20.0, unit: 'kg', availableStock: 85.0, shortage: 0 },
          { materialCode: 'ADD002', materialName: 'UV Stabilizer', requiredQty: 1.0, unit: 'kg', availableStock: 2.0, shortage: 0 },
          { materialCode: 'LID002', materialName: 'Container Lid HDPE', requiredQty: 500, unit: 'pcs', availableStock: 2500, shortage: 0 }
        ],
        children: [],
        hasShortage: false,
        jobId: null
      },
      {
        planId: 'PL20250902-001',
        planType: 'Parent',
        productCode: '2013',
        productName: 'Plastic Cup 250ml',
        qty: 2000,
        unit: 'pcs',
        planner: 'ကိုအောင် (Ko Aung)',
        submittedDate: '2025-09-02',
        priority: 'Low',
        status: 'Approved',
        rawMaterials: [
          { materialCode: 'PS001', materialName: 'PS Clear', requiredQty: 15.0, unit: 'kg', availableStock: 65.0, shortage: 0 },
          { materialCode: 'ADD003', materialName: 'Anti-static Agent', requiredQty: 0.5, unit: 'kg', availableStock: 5.0, shortage: 0 }
        ],
        children: ['PL20250902-001-C1'],
        hasShortage: false,
        jobId: 'JOB20250903-001'
      },
      {
        planId: 'PL20250902-001-C1',
        planType: 'Child',
        parentPlan: 'PL20250902-001',
        productCode: '2013',
        productName: 'Plastic Cup 250ml - Single Batch',
        qty: 2000,
        unit: 'pcs',
        planner: 'ကိုအောင် (Ko Aung)',
        submittedDate: '2025-09-02',
        priority: 'Low',
        status: 'Approved',
        rawMaterials: [
          { materialCode: 'PS001', materialName: 'PS Clear', requiredQty: 15.0, unit: 'kg', availableStock: 65.0, shortage: 0 },
          { materialCode: 'ADD003', materialName: 'Anti-static Agent', requiredQty: 0.5, unit: 'kg', availableStock: 5.0, shortage: 0 }
        ],
        children: [],
        hasShortage: false,
        jobId: 'JOB20250903-001'
      },
      {
        planId: 'PL20250902-002',
        planType: 'Parent',
        productCode: '2011',
        productName: 'Plastic Bottle 500ml',
        qty: 1500,
        unit: 'pcs',
        planner: 'မမြင့်မြင့် (Ma Myint Myint)',
        submittedDate: '2025-09-02',
        priority: 'High',
        status: 'Rejected',
        rawMaterials: [
          { materialCode: 'PET001', materialName: 'PET Resin Clear', requiredQty: 37.5, unit: 'kg', availableStock: 150.0, shortage: 0 },
          { materialCode: 'ADD001', materialName: 'Additive Blue', requiredQty: 1.5, unit: 'kg', availableStock: 3.0, shortage: 0 }
        ],
        children: [],
        hasShortage: false,
        jobId: null
      }
    ];

    // State for expanded rows and selected plan details (already defined globally above)

    // Calculate statistics
    const totalPlans = mockProductionPlans.filter(p => p.planType === 'Parent').length;
    const pendingPlans = mockProductionPlans.filter(p => p.status === 'Pending' && p.planType === 'Parent').length;
    const approvedPlans = mockProductionPlans.filter(p => p.status === 'Approved' && p.planType === 'Parent').length;
    const rejectedPlans = mockProductionPlans.filter(p => p.status === 'Rejected' && p.planType === 'Parent').length;
    const draftPlans = mockProductionPlans.filter(p => p.status === 'Draft' && p.planType === 'Parent').length;

    // Get all plans for display in table
    const allPlansForDisplay = mockProductionPlans;

    // Helper functions
    const toggleExpanded = (planId: string) => {
      setExpandedPlans(prev => 
        prev.includes(planId) 
          ? prev.filter(id => id !== planId)
          : [...prev, planId]
      );
    };

    const getChildPlans = (parentId: string) => {
      return mockProductionPlans.filter(p => p.parentPlan === parentId);
    };

    const formatRawMaterialSummary = (materials: any[]) => {
      if (materials.length === 0) return 'No materials';
      if (materials.length === 1) {
        return `${materials[0].materialCode} – ${materials[0].requiredQty} ${materials[0].unit}`;
      }
      const totalWeight = materials.reduce((sum, m) => sum + (m.unit === 'kg' ? m.requiredQty : 0), 0);
      return `${materials.length} types, ${totalWeight.toFixed(1)} kg total`;
    };

    const getStatusColor = (status: string) => {
      switch (status) {
        case 'Pending': return 'bg-yellow-100 text-yellow-800 border-yellow-200';
        case 'Approved': return 'bg-green-100 text-green-800 border-green-200';
        case 'Rejected': return 'bg-red-100 text-red-800 border-red-200';
        case 'Draft': return 'bg-slate-100 text-slate-800 border-slate-200';
        default: return 'bg-slate-100 text-slate-800 border-slate-200';
      }
    };

    const getPriorityColor = (priority: string) => {
      switch (priority) {
        case 'High': return 'bg-red-100 text-red-800 border-red-200';
        case 'Medium': return 'bg-yellow-100 text-yellow-800 border-yellow-200';
        case 'Low': return 'bg-green-100 text-green-800 border-green-200';
        default: return 'bg-slate-100 text-slate-800 border-slate-200';
      }
    };

    const handleApprovePlan = (plan: any) => {
      // Generate Job ID
      const jobId = `JOB${new Date().toISOString().slice(0, 10).replace(/-/g, '')}-${Math.floor(Math.random() * 1000).toString().padStart(3, '0')}`;
      
      // Update the plan status to Approved and assign Job ID
      const updatedPlans = mockProductionPlans.map(p => 
        p.planId === plan.planId 
          ? { ...p, status: 'Approved', jobId: jobId }
          : p
      );
      
      // Also update child plans if this is a parent plan
      if (plan.planType === 'Parent' && plan.children.length > 0) {
        plan.children.forEach((childId: string) => {
          const childIndex = updatedPlans.findIndex(cp => cp.planId === childId);
          if (childIndex !== -1) {
            updatedPlans[childIndex] = {
              ...updatedPlans[childIndex],
              status: 'Approved',
              jobId: `${jobId}-${childId.split('-').pop()}`
            };
          }
        });
      }
      
      toast.success(`✅ Plan ${plan.planId} approved and Job ${jobId} created! | အစီအစဉ် ${plan.planId} ကို အတည်ပြု၍ Job ${jobId} ဖန်တီးပြီးပါပြီ`);
    };

    const handleRejectPlan = () => {
      if (!rejectReason.trim()) {
        toast.error('Please provide a reason for rejection | ငြင်းပယ်ရသည့်အကြောင်းရင်းကို ဖြည့်ပါ');
        return;
      }
      
      if (planToReject) {
        // Update the plan status to Rejected with reason
        const updatedPlans = mockProductionPlans.map(p => 
          p.planId === planToReject.planId 
            ? { ...p, status: 'Rejected', rejectionReason: rejectReason }
            : p
        );
        
        toast.success(`❌ Plan ${planToReject.planId} rejected | အစီအစဉ် ${planToReject.planId} ကို ငြင်းပယ်ပြီးပါပြီ`);
      }
      
      setShowRejectDialog(false);
      setRejectReason('');
      setPlanToReject(null);
    };

    // Render Plan Detail Panel
    const renderPlanDetailPanel = () => {
      if (!selectedPlan) return null;

      const childPlans = getChildPlans(selectedPlan.planId);

      return (
        <Dialog open={showPlanDetail} onOpenChange={setShowPlanDetail}>
          <DialogContent className="max-w-6xl max-h-[90vh] overflow-y-auto">
            <DialogHeader>
              <DialogTitle className="text-xl">
                Plan Detail | အစီအစဉ်အသေးစိတ် - {selectedPlan.planId}
              </DialogTitle>
              <DialogDescription>
                Complete plan information and raw material requirements
              </DialogDescription>
            </DialogHeader>

            <div className="space-y-6">
              {/* Plan Info */}
              <Card>
                <CardHeader>
                  <CardTitle className="text-lg">Plan Information | အစီအစဉ်အချက်အလက်</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                    <div>
                      <Label className="font-medium">Plan ID</Label>
                      <div className="text-slate-700">{selectedPlan.planId}</div>
                    </div>
                    <div>
                      <Label className="font-medium">Product | ထုတ်ကုန်</Label>
                      <div className="text-slate-700">{selectedPlan.productCode} - {selectedPlan.productName}</div>
                    </div>
                    <div>
                      <Label className="font-medium">Quantity | ပမာဏ</Label>
                      <div className="text-slate-700">{selectedPlan.qty.toLocaleString()} {selectedPlan.unit}</div>
                    </div>
                    <div>
                      <Label className="font-medium">Planner | အစီအစဉ်ရေးဆွဲသူ</Label>
                      <div className="text-slate-700">{selectedPlan.planner}</div>
                    </div>
                    <div>
                      <Label className="font-medium">Submitted Date | တင်သွင်းသည့်ရက်</Label>
                      <div className="text-slate-700">{selectedPlan.submittedDate}</div>
                    </div>
                    <div>
                      <Label className="font-medium">Priority | ဦးစားပေးမှု</Label>
                      <Badge className={getPriorityColor(selectedPlan.priority)}>
                        {selectedPlan.priority}
                      </Badge>
                    </div>
                    <div>
                      <Label className="font-medium">Status | အခြေအနေ</Label>
                      <Badge className={getStatusColor(selectedPlan.status)}>
                        {selectedPlan.status}
                      </Badge>
                    </div>
                    <div>
                      <Label className="font-medium">Type | အမျိုးအစား</Label>
                      <Badge variant="outline">
                        {selectedPlan.planType}
                      </Badge>
                    </div>
                  </div>
                </CardContent>
              </Card>

              {/* Parent-Child Tree View */}
              {(selectedPlan.planType === 'Parent' && childPlans.length > 0) && (
                <Card>
                  <CardHeader>
                    <CardTitle className="text-lg flex items-center gap-2">
                      <GitBranch className="h-5 w-5" />
                      Parent-Child Relationship | မူလ-ခွဲအစီအစဉ် ဆက်နွယ်မှု
                    </CardTitle>
                  </CardHeader>
                  <CardContent>
                    <div className="space-y-2">
                      <div className="flex items-center gap-2 font-medium text-lg">
                        <TreePine className="h-4 w-4 text-blue-600" />
                        {selectedPlan.planId} (Parent)
                      </div>
                      <div className="ml-6 space-y-2">
                        {childPlans.map((child) => (
                          <div key={child.planId} className="flex items-center gap-2 text-slate-700">
                            <ChevronRight className="h-4 w-4" />
                            <span className="font-mono">{child.planId}</span>
                            <span>- {child.qty.toLocaleString()} {child.unit}</span>
                            <Badge className={getStatusColor(child.status)} size="sm">
                              {child.status}
                            </Badge>
                          </div>
                        ))}
                      </div>
                    </div>
                  </CardContent>
                </Card>
              )}

              {/* Raw Material Requirements */}
              <Card>
                <CardHeader>
                  <CardTitle className="text-lg flex items-center gap-2">
                    <Package className="h-5 w-5" />
                    Raw Material Requirements | ကုန်ကြမ်းလိုအပ်ချက်များ
                  </CardTitle>
                  <div className="text-sm text-slate-600 mt-2">
                    Complete breakdown of all required materials with weight details and stock information
                  </div>
                </CardHeader>
                <CardContent>
                  <div className="space-y-4">
                    {/* Material Summary */}
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-4 p-4 bg-slate-50 rounded-lg">
                      <div className="text-center">
                        <div className="text-lg font-bold text-slate-700">
                          {selectedPlan.rawMaterials.length}
                        </div>
                        <div className="text-sm text-slate-600">Total Material Types</div>
                      </div>
                      <div className="text-center">
                        <div className="text-lg font-bold text-blue-600">
                          {selectedPlan.rawMaterials.reduce((sum: number, m: any) => sum + (m.unit === 'kg' ? m.requiredQty : 0), 0).toFixed(1)} kg
                        </div>
                        <div className="text-sm text-slate-600">Total Weight Required</div>
                      </div>
                      <div className="text-center">
                        <div className="text-lg font-bold text-green-600">
                          {selectedPlan.rawMaterials.reduce((sum: number, m: any) => sum + (m.unit !== 'kg' ? m.requiredQty : 0), 0).toLocaleString()}
                        </div>
                        <div className="text-sm text-slate-600">Total Pieces Required</div>
                      </div>
                    </div>

                    {/* Detailed Material Table */}
                    <div className="overflow-hidden rounded-lg border">
                      <Table>
                        <TableHeader>
                          <TableRow className="bg-slate-50">
                            <TableHead>Material Code</TableHead>
                            <TableHead>Material Name | ကုန်ကြမ်းအမည်</TableHead>
                            <TableHead>Required Qty | လိုအပ်သောပမာဏ</TableHead>
                            <TableHead>Available Stock | ရနိုင်သောစတော့ခ်</TableHead>
                            <TableHead>Shortage | လိုအပ်နေသောပမာဏ</TableHead>
                          </TableRow>
                        </TableHeader>
                        <TableBody>
                          {selectedPlan.rawMaterials.map((material: any) => (
                            <TableRow key={material.materialCode} className="hover:bg-slate-25">
                              <TableCell className="font-mono font-medium text-blue-600">
                                {material.materialCode}
                              </TableCell>
                              <TableCell>
                                <div className="font-medium">{material.materialName}</div>
                              </TableCell>
                              <TableCell className="text-right font-mono font-medium">
                                {material.requiredQty.toLocaleString()}
                              </TableCell>
                              <TableCell className="text-center">
                                <Badge variant="outline" size="sm">
                                  {material.unit}
                                </Badge>
                              </TableCell>
                              <TableCell className="text-right font-mono">
                                {material.availableStock.toLocaleString()}
                              </TableCell>
                              <TableCell className="text-right font-mono">
                                {material.shortage > 0 ? (
                                  <div className="flex flex-col items-end">
                                    <span className="text-red-600 font-medium bg-red-50 px-2 py-1 rounded text-sm">
                                      ⚠️ {material.shortage.toLocaleString()} {material.unit}
                                    </span>
                                    <span className="text-xs text-red-500 mt-1">Shortage!</span>
                                  </div>
                                ) : (
                                  <div className="flex items-center justify-end gap-1">
                                    <CheckCircle className="h-4 w-4 text-green-600" />
                                    <span className="text-green-600 font-medium">Sufficient</span>
                                  </div>
                                )}
                              </TableCell>

                            </TableRow>
                          ))}
                        </TableBody>
                      </Table>
                    </div>

                    {/* Shortage Alert */}
                    {selectedPlan.hasShortage && (
                      <div className="p-4 bg-red-50 border border-red-200 rounded-lg">
                        <div className="flex items-center gap-2 text-red-700 font-medium mb-2">
                          <AlertTriangle className="h-5 w-5" />
                          Material Shortage Alert | ကုန်ကြမ်းလိုအပ်ချက်သတိပေးချက်
                        </div>
                        <div className="text-sm text-red-600">
                          This plan has material shortages that need to be resolved before production can proceed.
                          Please coordinate with the Raw Material team to ensure adequate stock availability.
                        </div>
                        <div className="text-sm text-red-600 mt-1">
                          ဤအစီအစဉ်တွင် ကုန်ကြမ်းလိုအပ်ချက်ရှိနေသောကြောင့် ထုတ်လုပ်မှုမစမီ ကုန်ကြမ်းအဖွဲ့နှင့် ညှိနှိုင်းပါ။
                        </div>
                      </div>
                    )}
                  </div>
                </CardContent>
              </Card>

              {/* Actions */}
              {selectedPlan.status === 'Pending' && (
                <div className="flex justify-between items-center pt-4">
                  <Button
                    variant="outline"
                    onClick={() => setShowPlanDetail(false)}
                  >
                    <ArrowLeft className="h-4 w-4 mr-2" />
                    Back | နောက်သို့
                  </Button>
                  <div className="flex gap-3">
                    <Button
                      variant="outline"
                      className="text-red-600 border-red-600 hover:bg-red-50"
                      onClick={() => {
                        setPlanToReject(selectedPlan);
                        setShowRejectDialog(true);
                      }}
                    >
                      <X className="h-4 w-4 mr-2" />
                      Reject | ငြင်းပယ်
                    </Button>
                    <Button
                      className="bg-green-600 hover:bg-green-700 text-white"
                      onClick={() => handleApprovePlan(selectedPlan)}
                    >
                      <CheckCircle className="h-4 w-4 mr-2" />
                      Approve | အတည်ပြု
                    </Button>
                  </div>
                </div>
              )}
            </div>
          </DialogContent>
        </Dialog>
      );
    };

    return (
      <div className="space-y-6">
        {/* Page Header */}
        <div className="mb-6">
          <div className="flex items-center gap-3 mb-2">
            <div className="flex items-center justify-center w-8 h-8 bg-green-100 rounded-full">
              <CheckCircle className="h-5 w-5 text-green-600" />
            </div>
            <h1 className="text-xl font-semibold text-slate-900">
              Demand Authorization ေတာင်းဆိုမှုခွင့်ပြုချက်
            </h1>
          </div>
          <div className="flex items-center gap-2 text-sm text-slate-600 ml-11">
            <ClipboardList className="h-4 w-4" />
            Review & approve production plans from Planning team
          </div>
        </div>

        {/* Summary Dashboard */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-6">
          <Card className="border-yellow-200 bg-yellow-50">
            <CardContent className="p-6">
              <div className="flex items-center justify-between">
                <div>
                  <div className="text-2xl font-bold text-yellow-600">{pendingPlans}</div>
                  <div className="text-sm text-yellow-700">🟡 Pending Approval</div>
                </div>
              </div>
            </CardContent>
          </Card>

          <Card className="border-green-200 bg-green-50">
            <CardContent className="p-6">
              <div className="flex items-center justify-between">
                <div>
                  <div className="text-2xl font-bold text-green-600">{approvedPlans}</div>
                  <div className="text-sm text-green-700">🟢 Approved</div>
                </div>
              </div>
            </CardContent>
          </Card>

          <Card className="border-red-200 bg-red-50">
            <CardContent className="p-6">
              <div className="flex items-center justify-between">
                <div>
                  <div className="text-2xl font-bold text-red-600">{rejectedPlans}</div>
                  <div className="text-sm text-red-700">🔴 Rejected</div>
                </div>
              </div>
            </CardContent>
          </Card>

          <Card className="border-slate-200 bg-slate-50">
            <CardContent className="p-6">
              <div className="flex items-center justify-between">
                <div>
                  <div className="text-2xl font-bold text-slate-600">{draftPlans}</div>
                  <div className="text-sm text-slate-700">⚪ Draft</div>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Production Plans Review Table */}
        <Card>
          <CardHeader>
            <div className="flex items-center justify-between">
              <CardTitle className="flex items-center gap-2">
                <ClipboardList className="h-5 w-5 text-blue-600" />
                Production Plans Review | ထုတ်လုပ်မှုအစီအစဉ်စစ်ဆေးခြင်း
              </CardTitle>
              <div className="flex items-center gap-4">
                <Button variant="outline" size="sm">
                  <Filter className="h-4 w-4 mr-2" />
                  Filter
                </Button>
                <Button variant="outline" size="sm">
                  <Download className="h-4 w-4 mr-2" />
                  Export
                </Button>
                <Badge className="bg-blue-100 text-blue-800">
                  {allPlansForDisplay.length} plans
                </Badge>
              </div>
            </div>

          </CardHeader>
          <CardContent>
            <div className="overflow-hidden rounded-lg border">
              <Table>
                <TableHeader>
                  <TableRow className="bg-slate-50">
                    <TableHead>Plan ID</TableHead>
                    <TableHead>Product | ထုတ်ကုန်</TableHead>
                    <TableHead>Quantity | ပမာဏ</TableHead>
                    <TableHead>Planner | စီစဉ်���ူ</TableHead>
                    <TableHead>Priority | ဦးစားပေး</TableHead>
                    <TableHead>Submitted Date | တင်သွင်းသည့်ရက်</TableHead>
                    <TableHead>Status | အခြေအနေ</TableHead>
                    <TableHead>Job ID</TableHead>
                    <TableHead>Actions | လုပ်ဆောင်ချက်</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {allPlansForDisplay.map((plan) => (
                    <TableRow key={plan.planId} className="hover:bg-slate-50">
                      <TableCell className="font-mono font-medium text-blue-600">
                        {plan.planId}
                      </TableCell>
                      <TableCell>
                        <div>
                          <div className="font-medium">{plan.productCode} - {plan.productName}</div>
                        </div>
                      </TableCell>
                      <TableCell className="text-right font-mono">
                        {plan.qty.toLocaleString()} {plan.unit}
                      </TableCell>
                      <TableCell className="font-medium text-slate-700">
                        {plan.planner}
                      </TableCell>
                      <TableCell>
                        <Badge className={getPriorityColor(plan.priority)}>
                          ● {plan.priority}
                        </Badge>
                      </TableCell>
                      <TableCell className="font-mono text-sm">
                        {plan.submittedDate}
                      </TableCell>
                      <TableCell>
                        <Badge className={getStatusColor(plan.status)}>
                          ● {plan.status}
                        </Badge>
                      </TableCell>
                      <TableCell>
                        {plan.jobId ? (
                          <Badge variant="outline" className="font-mono text-blue-700 border-blue-200 bg-blue-50">
                            {plan.jobId}
                          </Badge>
                        ) : (
                          <span className="text-slate-400">-</span>
                        )}
                      </TableCell>
                      <TableCell>
                        <div className="flex gap-2">
                          <Button
                            variant="outline"
                            size="sm"
                            onClick={() => {
                              setSelectedPlan(plan);
                              setShowPlanDetail(true);
                            }}
                          >
                            <Eye className="h-4 w-4" />
                          </Button>
                          {plan.status === 'Pending' && (
                            <>
                              <Button
                                size="sm"
                                className="bg-green-600 hover:bg-green-700"
                                onClick={() => handleApprovePlan(plan)}
                              >
                                <CheckCircle className="h-4 w-4" />
                              </Button>
                              <Button
                                variant="outline"
                                size="sm"
                                className="text-red-600 border-red-600 hover:bg-red-50"
                                onClick={() => {
                                  setPlanToReject(plan);
                                  setShowRejectDialog(true);
                                }}
                              >
                                <X className="h-4 w-4" />
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

        {/* Plan Detail Panel */}
        {renderPlanDetailPanel()}

        {/* Reject Dialog */}
        <Dialog open={showRejectDialog} onOpenChange={setShowRejectDialog}>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>Reject Plan | အစီအစဉ်ငြင်းပယ်ခြင်း</DialogTitle>
              <DialogDescription>
                Please provide a reason for rejecting plan {planToReject?.planId}
              </DialogDescription>
            </DialogHeader>
            <div className="space-y-4">
              <div>
                <Label>Reason for Rejection | ငြင်းပယ်ရသည့်အကြောင်းရင်း</Label>
                <Textarea
                  placeholder="Enter detailed reason for rejection..."
                  value={rejectReason}
                  onChange={(e) => setRejectReason(e.target.value)}
                  rows={4}
                />
              </div>
              <div className="flex justify-end gap-2">
                <Button
                  variant="outline"
                  onClick={() => {
                    setShowRejectDialog(false);
                    setRejectReason('');
                    setPlanToReject(null);
                  }}
                >
                  Cancel | ပယ်ဖျက်
                </Button>
                <Button
                  className="bg-red-600 hover:bg-red-700 text-white"
                  onClick={handleRejectPlan}
                >
                  <X className="h-4 w-4 mr-2" />
                  Reject Plan | အစီအစဉ်ငြင်းပယ်
                </Button>
              </div>
            </div>
          </DialogContent>
        </Dialog>
      </div>
    );
  }

  // Render Mold Change Request section (Production Control specific)
  if (currentPage === 'mold-change-request') {
    return <ProductionControlMCR />;
  }

  // Render Cut/Glue Residue Management section
  if (currentPage === 'cut-glue-residue') {
    return <CutGlueResidueManagement />;
  }

  // Render Mold Change Requests section
  if (currentPage === 'mold-change-requests') {
    return <MoldChangeRequests userRole="production-control" />;
  }

  // Placeholder for other sections
  return (
    <div className="p-6 bg-slate-50 min-h-screen">
      <div className="max-w-4xl mx-auto">
        <div className="bg-white rounded-lg shadow-sm p-8 text-center">
          <h2 className="text-xl font-semibold text-slate-900 mb-2">
            Production Control - {currentPage}
          </h2>
          <p className="text-slate-600 mb-4">
            This section is under development
          </p>
          <p className="text-sm text-slate-500">
            ဤစာမျက်နှာကို ဖွံ့ဖြိုးတိုးတက်နေဆဲဖြစ်သည်
          </p>
        </div>
      </div>
    </div>
  );
}