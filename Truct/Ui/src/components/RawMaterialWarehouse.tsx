import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Badge } from './ui/badge';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from './ui/dialog';
import { Textarea } from './ui/textarea';
import { Alert, AlertDescription } from './ui/alert';
import { 
  ArrowUp,
  ArrowDown,
  RefreshCw,
  PackageCheck,
  PackageX,
  RotateCcw,
  AlertTriangle,
  Package,
  TrendingUp,
  TrendingDown,
  Clock,
  User,
  Printer,
  CheckCircle,
  XCircle,
  AlertCircle,
  Building2,
  Truck,
  Factory,
  Eye,
  Weight,
  Info,
  Shield,
  Camera,
  Lock,
  ArrowRightLeft
} from 'lucide-react';
import { format } from 'date-fns';
import { toast } from 'sonner@2.0.3';

// Mock data for today's activities
const todayStats = {
  stockIn: {
    total: 2850,
    transactions: 12,
    supplier: 2200,
    leftover: 650,
    pendingApprovals: 2
  },
  stockOut: {
    total: 1750,
    transactions: 8,
    issued: 1400,
    scrap: 250,
    returned: 100,
    pendingApprovals: 3
  },
  transfers: {
    total: 600,
    transactions: 4,
    warehouse: 400,
    job: 200,
    pendingApprovals: 1
  }
};

// Stock In Requests - All incoming materials waiting for warehouse action
const stockInRequests = [
  {
    id: 'REQ-IN-001',
    time: '08:30',
    refNo: 'SUP-DLV-001',
    source: 'Thai Plastic Co.',
    sourceType: 'Supplier',
    material: 'PP Virgin Grade A',
    color: { name: 'Natural', hex: '#F5F5DC' },
    qty: 1000,
    unit: 'kg',
    status: 'Ready to Accept',
    canProcess: true,
    actionType: 'accept'
  },
  {
    id: 'REQ-IN-002',
    time: '09:15',
    refNo: 'LFT-RET-001',
    source: 'MCH-001 (JOB-2024-001)',
    sourceType: 'Leftover Return',
    material: 'LDPE + TPE Mix',
    color: { name: 'Blue', hex: '#0066CC' },
    qty: 75,
    unit: 'kg',
    status: 'Approved by PC',
    canProcess: true,
    actionType: 'accept'
  },
  {
    id: 'REQ-IN-003',
    time: '10:20',
    refNo: 'LFT-RET-002',
    source: 'MCH-003 (JOB-2024-002)', 
    sourceType: 'Leftover Return',
    material: 'PP Recycled',
    color: { name: 'Green', hex: '#00CC00' },
    qty: 50,
    unit: 'kg',
    status: 'Pending PC Approval',
    canProcess: false,
    actionType: 'pending'
  },
  {
    id: 'REQ-IN-004',
    time: '11:00',
    refNo: 'TRF-IN-001',
    source: 'WIP-WH',
    sourceType: 'Transfer In',
    material: 'PP Grade B',
    color: { name: 'White', hex: '#FFFFFF' },
    qty: 200,
    unit: 'kg',
    status: 'Approved by PC',
    canProcess: true,
    actionType: 'accept'
  },
  {
    id: 'REQ-IN-005',
    time: '14:30',
    refNo: 'SUP-DLV-002',
    source: 'Myanmar Polymer Ltd.',
    sourceType: 'Supplier',
    material: 'HDPE Natural',
    color: { name: 'Natural', hex: '#F5F5DC' },
    qty: 500,
    unit: 'kg',
    status: 'Ready to Accept',
    canProcess: true,
    actionType: 'accept'
  }
];

// Stock Out Requests - All outgoing material requests
const stockOutRequests = [
  {
    id: 'REQ-OUT-001',
    time: '09:00',
    refNo: 'ISS-PROD-001',
    destination: 'MCH-002 (JOB-2024-003)',
    destinationType: 'Issue to Production',
    material: 'PP Virgin Grade A',
    color: { name: 'Natural', hex: '#F5F5DC' },
    qty: 500,
    unit: 'kg',
    status: 'Approved by PC',
    canProcess: true,
    actionType: 'issue'
  },
  {
    id: 'REQ-OUT-002',
    time: '10:30',
    refNo: 'ISS-PROD-002',
    destination: 'MCH-004 (JOB-2024-004)',
    destinationType: 'Issue to Production',
    material: 'LDPE Mix',
    color: { name: 'Red', hex: '#FF0000' },
    qty: 300,
    unit: 'kg',
    status: 'Pending PC Approval',
    canProcess: false,
    actionType: 'pending'
  },
  {
    id: 'REQ-OUT-003',
    time: '11:45',
    refNo: 'SCR-REQ-001',
    destination: 'Scrap Yard',
    destinationType: 'Scrap/Damage',
    material: 'Contaminated LDPE',
    color: { name: 'Mixed', hex: '#808080' },
    qty: 25,
    unit: 'kg',
    status: 'Supervisor Required',
    canProcess: true, // Assuming current user is supervisor
    actionType: 'scrap'
  },
  {
    id: 'REQ-OUT-004',
    time: '13:15',
    refNo: 'RET-SUP-001',
    destination: 'Thai Plastic Co.',
    destinationType: 'Return to Supplier',
    material: 'Defective Pellets',
    color: { name: 'Black', hex: '#000000' },
    qty: 100,
    unit: 'kg',
    status: 'Approved by PC',
    canProcess: true,
    actionType: 'return'
  },
  {
    id: 'REQ-OUT-005',
    time: '15:30',
    refNo: 'RET-HQ-001',
    destination: 'Yangon HQ',
    destinationType: 'Return to HQ',
    material: 'Excess Material',
    color: { name: 'White', hex: '#FFFFFF' },
    qty: 200,
    unit: 'kg',
    status: 'Rejected by PC',
    canProcess: false,
    actionType: 'rejected'
  }
];

// Transfer Requests - All transfers across warehouses or jobs
const transferRequests = [
  {
    id: 'REQ-TRF-001',
    time: '10:00',
    refNo: 'WH-TRF-001',
    from: 'RM-WH',
    to: 'WIP-WH',
    transferType: 'Warehouse Transfer',
    material: 'PP Grade B',
    color: { name: 'White', hex: '#FFFFFF' },
    qty: 300,
    unit: 'kg',
    status: 'Supervisor Required',
    canProcess: true, // Assuming current user is supervisor
    actionType: 'process'
  },
  {
    id: 'REQ-TRF-002',
    time: '12:15',
    refNo: 'JOB-REAL-001',
    from: 'JOB-2024-001',
    to: 'JOB-2024-003',
    transferType: 'Job Reallocation',
    material: 'LDPE Mix',
    color: { name: 'Red', hex: '#FF0000' },
    qty: 150,
    unit: 'kg',
    status: 'Pending PC Approval',
    canProcess: false,
    actionType: 'pending'
  },
  {
    id: 'REQ-TRF-003',
    time: '14:45',
    refNo: 'JOB-REAL-002',
    from: 'JOB-2024-002',
    to: 'JOB-2024-005',
    transferType: 'Job Reallocation',
    material: 'PP Virgin Grade A',
    color: { name: 'Natural', hex: '#F5F5DC' },
    qty: 200,
    unit: 'kg',
    status: 'Approved by PC',
    canProcess: true,
    actionType: 'process'
  }
];

// Job-to-Job Transfer Requests from Production Control
const jobTransferRequests = [
  {
    id: 'JT-REQ-001',
    time: '10:30',
    refNo: 'JT-20250908-001',
    fromJob: 'JOB20250903-001',
    fromMachine: 'INJ-M001',
    toJob: 'JOB20250903-005',
    toMachine: 'INJ-M005',
    transferType: 'Job-to-Job Transfer',
    material: 'PET001 - PET Resin Clear',
    color: { name: 'Clear', hex: '#FFFFFF' },
    qty: 15.0,
    unit: 'kg',
    reason: 'Over Reserved',
    status: 'Pending Acknowledgement (Warehouse)',
    canProcess: true,
    actionType: 'acknowledge',
    requestedBy: 'Production Manager 1',
    requestedDate: '2025-09-08 10:30'
  },
  {
    id: 'JT-REQ-002',
    time: '14:15',
    refNo: 'JT-20250908-002',
    fromJob: 'JOB20250903-002',
    fromMachine: 'INJ-M002',
    toJob: 'JOB20250903-004',
    toMachine: 'INJ-M004',
    transferType: 'Job-to-Job Transfer',
    material: 'PP002 - PP Blue Compound',
    color: { name: 'Blue', hex: '#0066CC' },
    qty: 10.0,
    unit: 'kg',
    reason: 'Urgent Order',
    status: 'Acknowledged',
    canProcess: false,
    actionType: 'completed',
    requestedBy: 'Production Manager 2',
    requestedDate: '2025-09-08 14:15',
    processedDate: '2025-09-08 14:45',
    processedBy: 'Warehouse Staff 1'
  }
];

// Current stock balance
const stockBalance = [
  {
    id: 1,
    materialCode: 'RM-PP-001',
    materialName: 'PP Virgin Grade A',
    color: { name: 'Natural', hex: '#F5F5DC' },
    currentStock: 2500,
    reservedStock: 500,
    minStock: 500,
    isLowStock: false
  },
  {
    id: 2,
    materialCode: 'RM-LDPE-002',
    materialName: 'LDPE + TPE Mix',
    color: { name: 'Blue', hex: '#0066CC' },
    currentStock: 800,
    reservedStock: 600,
    minStock: 1000,
    isLowStock: true
  },
  {
    id: 3,
    materialCode: 'RM-PP-003',
    materialName: 'PP Recycled',
    color: { name: 'Green', hex: '#00CC00' },
    currentStock: 150,
    reservedStock: 0,
    minStock: 200,
    isLowStock: true
  }
];

// Recent movement history
const movementHistory = [
  {
    id: 'MOV-JT-001',
    dateTime: '2024-03-15 14:45',
    refNo: 'JT-20250908-002',
    type: 'Job-to-Job Transfer',
    material: 'PP002 - PP Blue Compound',
    color: { name: 'Blue', hex: '#0066CC' },
    fromJob: 'JOB20250903-002',
    toJob: 'JOB20250903-004',
    qty: -10, // Negative from source job perspective
    unit: 'kg',
    status: 'Completed',
    performedBy: 'Warehouse Staff 1'
  },
  {
    id: 'MOV-001',
    dateTime: '2024-03-15 15:45',
    refNo: 'RET-SUP-001',
    type: 'Return to Supplier',
    material: 'Defective Pellets',
    color: { name: 'Black', hex: '#000000' },
    qty: -100,
    unit: 'kg',
    status: 'Completed',
    performedBy: 'Ma Hnin (INV01)'
  },
  {
    id: 'MOV-002',
    dateTime: '2024-03-15 14:20',
    refNo: 'TRF-IN-001',
    type: 'Transfer In',
    material: 'PP Grade B',
    color: { name: 'White', hex: '#FFFFFF' },
    qty: +200,
    unit: 'kg',
    status: 'Completed',
    performedBy: 'Ko Thant (INV01)'
  },
  {
    id: 'MOV-003',
    dateTime: '2024-03-15 13:15',
    refNo: 'JOB-REAL-002',
    type: 'Job Reallocation',
    material: 'PP Virgin Grade A',
    color: { name: 'Natural', hex: '#F5F5DC' },
    qty: -200,
    unit: 'kg',
    status: 'Completed',
    performedBy: 'Ma Hnin (INV01)'
  },
  {
    id: 'MOV-004',
    dateTime: '2024-03-15 11:30',
    refNo: 'SCR-REQ-001',
    type: 'Scrap',
    material: 'Contaminated LDPE',
    color: { name: 'Mixed', hex: '#808080' },
    qty: -25,
    unit: 'kg',
    status: 'Completed',
    performedBy: 'Ko Thant (INV02)'
  },
  {
    id: 'MOV-005',
    dateTime: '2024-03-15 10:00',
    refNo: 'WH-TRF-001',
    type: 'Warehouse Transfer',
    material: 'PP Grade B',
    color: { name: 'White', hex: '#FFFFFF' },
    qty: -300,
    unit: 'kg',
    status: 'Completed',
    performedBy: 'Ko Thant (INV01)'
  },
  {
    id: 'MOV-006',
    dateTime: '2024-03-15 09:00',
    refNo: 'ISS-PROD-001',
    type: 'Issue to Production',
    material: 'PP Virgin Grade A',
    color: { name: 'Natural', hex: '#F5F5DC' },
    qty: -500,
    unit: 'kg',
    status: 'Completed',
    performedBy: 'Ma Hnin (INV01)'
  },
  {
    id: 'MOV-007',
    dateTime: '2024-03-15 08:30',
    refNo: 'SUP-DLV-001',
    type: 'Supplier Receive',
    material: 'PP Virgin Grade A',
    color: { name: 'Natural', hex: '#F5F5DC' },
    qty: +1000,
    unit: 'kg',
    status: 'Completed',
    performedBy: 'Ko Min (INV02)'
  },
  {
    id: 'MOV-008',
    dateTime: '2024-03-14 16:45',
    refNo: 'LFT-RET-001',
    type: 'Leftover Receive',
    material: 'LDPE + TPE Mix',
    color: { name: 'Blue', hex: '#0066CC' },
    qty: +75,
    unit: 'kg',
    status: 'Completed',
    performedBy: 'Ma Hnin (INV01)'
  }
];

export function RawMaterialWarehouse() {
  const [showProcessModal, setShowProcessModal] = useState(false);
  const [processingItem, setProcessingItem] = useState<any>(null);
  const [modalType, setModalType] = useState('');

  // Current user role (can be changed based on actual user)
  const userRole = 'Inventory Supervisor'; // Could be: 'Inventory Supervisor', 'Warehouse Staff', etc.

  const getStatusBadge = (status: string) => {
    switch (status) {
      case 'Ready to Accept':
      case 'Approved by PC':
      case 'Supervisor Required':
        return <Badge className="bg-green-100 text-green-800"><CheckCircle className="h-3 w-3 mr-1" />Ready</Badge>;
      case 'Pending PC Approval':
        return <Badge className="bg-amber-100 text-amber-800"><AlertCircle className="h-3 w-3 mr-1" />Pending</Badge>;
      case 'Rejected by PC':
        return <Badge className="bg-red-100 text-red-800"><XCircle className="h-3 w-3 mr-1" />Rejected</Badge>;
      default:
        return <Badge variant="outline">{status}</Badge>;
    }
  };

  const getActionButton = (request: any, listType: string) => {
    if (request.actionType === 'rejected') {
      return (
        <Button 
          size="sm" 
          variant="outline"
          className="border-red-500 text-red-600 cursor-not-allowed opacity-50"
          disabled
        >
          <XCircle className="h-3 w-3 mr-1" />
          ❌ Rejected
        </Button>
      );
    }

    if (request.actionType === 'pending' || !request.canProcess) {
      return (
        <Button 
          size="sm" 
          variant="outline"
          className="cursor-not-allowed opacity-50"
          disabled
          title="Waiting for Production Control Approval"
        >
          <Lock className="h-3 w-3 mr-1" />
          🔒 Pending
        </Button>
      );
    }

    // Supervisor-only actions
    if ((request.status === 'Supervisor Required' || request.destinationType === 'Scrap/Damage' || request.transferType === 'Warehouse Transfer') && userRole !== 'Inventory Supervisor') {
      return (
        <Button 
          size="sm" 
          variant="outline"
          className="cursor-not-allowed opacity-50"
          disabled
          title="Supervisor Only"
        >
          <Shield className="h-3 w-3 mr-1" />
          🔒 Supervisor Only
        </Button>
      );
    }

    // Action buttons based on type
    switch (request.actionType) {
      case 'accept':
        return (
          <Button 
            size="sm" 
            className="bg-green-600 hover:bg-green-700 text-white"
            onClick={() => handleRowAction(request, 'accept')}
          >
            <CheckCircle className="h-3 w-3 mr-1" />
            ✅ Accept
          </Button>
        );
      case 'issue':
        return (
          <Button 
            size="sm" 
            className="bg-purple-600 hover:bg-purple-700 text-white"
            onClick={() => handleRowAction(request, 'issue')}
            title="Confirm this approved request and issue material."
          >
            <CheckCircle className="h-3 w-3 mr-1" />
            ✅ Acknowledge & Issue
          </Button>
        );
      case 'scrap':
        return (
          <Button 
            size="sm" 
            className="bg-red-600 hover:bg-red-700 text-white"
            onClick={() => handleRowAction(request, 'scrap')}
          >
            <AlertTriangle className="h-3 w-3 mr-1" />
            🚮 Scrap
          </Button>
        );
      case 'return':
        return (
          <Button 
            size="sm" 
            className="bg-yellow-700 hover:bg-yellow-800 text-white"
            onClick={() => handleRowAction(request, 'return')}
          >
            <Truck className="h-3 w-3 mr-1" />
            ↩ Return
          </Button>
        );
      case 'process':
        return (
          <Button 
            size="sm" 
            className="bg-blue-600 hover:bg-blue-700 text-white"
            onClick={() => handleRowAction(request, 'process')}
          >
            <RefreshCw className="h-3 w-3 mr-1" />
            🔁 Process
          </Button>
        );
      default:
        return null;
    }
  };

  const handleRowAction = (request: any, actionType: string) => {
    setProcessingItem(request);
    setModalType(actionType);
    setShowProcessModal(true);
  };

  const confirmAction = () => {
    if (!processingItem) return;

    let toastMessage = "";
    switch (modalType) {
      case 'accept':
        toastMessage = "✅ Material Accepted Successfully";
        break;
      case 'issue':
        toastMessage = "✅ Material Acknowledged & Issued Successfully";
        break;
      case 'scrap':
        toastMessage = "🚮 Scrap Recorded Successfully";
        break;
      case 'return':
        toastMessage = "↩ Return Processed Successfully";
        break;
      case 'process':
        toastMessage = "🔁 Transfer Processed Successfully";
        break;
      default:
        toastMessage = "✅ Action Completed Successfully";
    }

    toast.success(toastMessage);
    
    // Simulate printing slip for relevant actions
    if (['accept', 'issue', 'return', 'process'].includes(modalType)) {
      setTimeout(() => {
        toast.success(`📄 Slip ${processingItem.refNo} sent to printer | လွှာပုံနှိပ်ပြီးပါပြီ!`);
      }, 1000);
    }

    setShowProcessModal(false);
    setProcessingItem(null);
    setModalType('');
  };

  // Handle Job-to-Job Transfer actions
  const handleJobTransferAction = (request: any, action: 'acknowledge' | 'reject') => {
    const materialInfo = `${request.qty} ${request.unit} ${request.material}`;
    const jobInfo = `Job ${request.fromJob} to Job ${request.toJob}`;
    
    if (action === 'acknowledge') {
      // Show confirmation modal
      if (window.confirm(`Confirm transfer of ${materialInfo} from ${jobInfo}?`)) {
        toast.success(`✅ Job-to-Job Transfer Acknowledged: ${materialInfo} transferred from ${jobInfo}`);
        
        // Update status in mock data (in real app, this would be an API call)
        const requestIndex = jobTransferRequests.findIndex(r => r.id === request.id);
        if (requestIndex !== -1) {
          jobTransferRequests[requestIndex].status = 'Acknowledged';
          jobTransferRequests[requestIndex].processedDate = new Date().toISOString().slice(0, 16).replace('T', ' ');
          jobTransferRequests[requestIndex].processedBy = 'Warehouse Staff';
        }
        
        // Show movement history update
        setTimeout(() => {
          toast.success(`📋 Movement History Updated: Job-to-Job Transfer recorded`);
        }, 1000);
      }
    } else if (action === 'reject') {
      const reason = prompt('Please enter reason for rejection:');
      if (reason) {
        toast.error(`❌ Job-to-Job Transfer Rejected: ${materialInfo} from ${jobInfo}. Reason: ${reason}`);
        
        // Update status in mock data
        const requestIndex = jobTransferRequests.findIndex(r => r.id === request.id);
        if (requestIndex !== -1) {
          jobTransferRequests[requestIndex].status = 'Rejected';
          jobTransferRequests[requestIndex].processedDate = new Date().toISOString().slice(0, 16).replace('T', ' ');
          jobTransferRequests[requestIndex].processedBy = 'Warehouse Staff';
          jobTransferRequests[requestIndex].rejectionReason = reason;
        }
      }
    }
  };

  const getRowClassName = (request: any) => {
    if (request.actionType === 'rejected') {
      return "bg-red-50 opacity-60";
    }
    if (request.actionType === 'pending' || !request.canProcess) {
      return "bg-gray-50";
    }
    return "hover:bg-blue-50";
  };

  return (
    <div className="p-6 space-y-6">
      {/* Header */}
      <div>
        <h2 className="text-2xl font-semibold text-slate-900">
          Raw Material Warehouse | ကုန်ကြမ်းဂိုဒေါင်
        </h2>
        <p className="text-slate-600 mt-1">
          Simple request-driven workflow - process requests one by one
        </p>
        <p className="text-sm text-slate-500">
          တောင်းဆိုမှုများကို တစ်ခုချင်းစီ လုပ်ဆောင်ရန် ရိုးရှင်းသောစနစ်
        </p>
      </div>

      {/* Top Section - Summary Cards */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        {/* Stock In Today - Green */}
        <Card className="border-0 shadow-lg bg-gradient-to-br from-green-50 to-green-100 border-l-4 border-l-green-500">
          <CardHeader className="pb-3">
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-3">
                <div className="p-3 bg-green-600 rounded-full">
                  <ArrowUp className="h-6 w-6 text-white" />
                </div>
                <div>
                  <CardTitle className="text-green-800">Stock In Today</CardTitle>
                  <p className="text-sm text-green-600">စာရင်းဝင် (ယနေ့)</p>
                </div>
              </div>
              {todayStats.stockIn.pendingApprovals > 0 && (
                <Badge className="bg-amber-100 text-amber-800">
                  <AlertCircle className="h-3 w-3 mr-1" />
                  {todayStats.stockIn.pendingApprovals} pending
                </Badge>
              )}
            </div>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              <div className="text-3xl font-bold text-green-800">
                {todayStats.stockIn.total.toLocaleString()} kg
              </div>
              <div className="text-sm text-green-700">
                {todayStats.stockIn.transactions} transactions today
              </div>
              <div className="grid grid-cols-2 gap-3 text-sm">
                <div className="flex items-center gap-2">
                  <Truck className="h-4 w-4 text-green-600" />
                  <div>
                    <div className="font-semibold">{todayStats.stockIn.supplier.toLocaleString()}</div>
                    <div className="text-xs text-green-600">Supplier</div>
                  </div>
                </div>
                <div className="flex items-center gap-2">
                  <RotateCcw className="h-4 w-4 text-green-600" />
                  <div>
                    <div className="font-semibold">{todayStats.stockIn.leftover.toLocaleString()}</div>
                    <div className="text-xs text-green-600">Leftover</div>
                  </div>
                </div>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Stock Out Today - Red */}
        <Card className="border-0 shadow-lg bg-gradient-to-br from-red-50 to-red-100 border-l-4 border-l-red-500">
          <CardHeader className="pb-3">
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-3">
                <div className="p-3 bg-red-600 rounded-full">
                  <ArrowDown className="h-6 w-6 text-white" />
                </div>
                <div>
                  <CardTitle className="text-red-800">Stock Out Today</CardTitle>
                  <p className="text-sm text-red-600">စာရင်းထွက် (ယနေ့)</p>
                </div>
              </div>
              {todayStats.stockOut.pendingApprovals > 0 && (
                <Badge className="bg-amber-100 text-amber-800">
                  <AlertCircle className="h-3 w-3 mr-1" />
                  {todayStats.stockOut.pendingApprovals} pending
                </Badge>
              )}
            </div>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              <div className="text-3xl font-bold text-red-800">
                {todayStats.stockOut.total.toLocaleString()} kg
              </div>
              <div className="text-sm text-red-700">
                {todayStats.stockOut.transactions} transactions today
              </div>
              <div className="grid grid-cols-3 gap-2 text-xs">
                <div className="flex items-center gap-1">
                  <Factory className="h-3 w-3 text-red-600" />
                  <div>
                    <div className="font-semibold">{todayStats.stockOut.issued.toLocaleString()}</div>
                    <div className="text-red-600">Issued</div>
                  </div>
                </div>
                <div className="flex items-center gap-1">
                  <AlertTriangle className="h-3 w-3 text-red-600" />
                  <div>
                    <div className="font-semibold">{todayStats.stockOut.scrap.toLocaleString()}</div>
                    <div className="text-red-600">Scrap</div>
                  </div>
                </div>
                <div className="flex items-center gap-1">
                  <Truck className="h-3 w-3 text-red-600" />
                  <div>
                    <div className="font-semibold">{todayStats.stockOut.returned.toLocaleString()}</div>
                    <div className="text-red-600">Return</div>
                  </div>
                </div>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Transfers - Blue */}
        <Card className="border-0 shadow-lg bg-gradient-to-br from-blue-50 to-blue-100 border-l-4 border-l-blue-500">
          <CardHeader className="pb-3">
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-3">
                <div className="p-3 bg-blue-600 rounded-full">
                  <RefreshCw className="h-6 w-6 text-white" />
                </div>
                <div>
                  <CardTitle className="text-blue-800">Transfers</CardTitle>
                  <p className="text-sm text-blue-600">လွှဲပြောင်းမှုများ</p>
                </div>
              </div>
              {todayStats.transfers.pendingApprovals > 0 && (
                <Badge className="bg-amber-100 text-amber-800">
                  <AlertCircle className="h-3 w-3 mr-1" />
                  {todayStats.transfers.pendingApprovals} pending
                </Badge>
              )}
            </div>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              <div className="text-3xl font-bold text-blue-800">
                {todayStats.transfers.total.toLocaleString()} kg
              </div>
              <div className="text-sm text-blue-700">
                {todayStats.transfers.transactions} transactions today
              </div>
              <div className="grid grid-cols-2 gap-3 text-sm">
                <div className="flex items-center gap-2">
                  <Building2 className="h-4 w-4 text-blue-600" />
                  <div>
                    <div className="font-semibold">{todayStats.transfers.warehouse.toLocaleString()}</div>
                    <div className="text-xs text-blue-600">Warehouse</div>
                  </div>
                </div>
                <div className="flex items-center gap-2">
                  <RefreshCw className="h-4 w-4 text-blue-600" />
                  <div>
                    <div className="font-semibold">{todayStats.transfers.job.toLocaleString()}</div>
                    <div className="text-xs text-blue-600">Job</div>
                  </div>
                </div>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Middle Section - Request Lists */}
      <div className="space-y-6">
        {/* Stock In Requests */}
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <ArrowUp className="h-5 w-5 text-green-600" />
              Stock In Requests | ဝင်ရန်တောင်းခံမှုများ
              <Badge className="bg-green-100 text-green-800 ml-auto">
                {stockInRequests.length} requests
              </Badge>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Time | အချိန်</TableHead>
                    <TableHead>Ref No. | ကိုးကားနံပါတ်</TableHead>
                    <TableHead>Source | အရင်းအမြစ်</TableHead>
                    <TableHead>Material + Color | ပစ္စည်း + အရောင်</TableHead>
                    <TableHead>Qty | ပမာဏ</TableHead>
                    <TableHead>Status | အခြေအနေ</TableHead>
                    <TableHead>Action | လုပ်ဆောင်ချက်</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {stockInRequests.map((request) => (
                    <TableRow key={request.id} className={getRowClassName(request)}>
                      <TableCell className="font-mono text-sm">{request.time}</TableCell>
                      <TableCell>
                        <Badge variant="outline" className="font-mono text-xs">
                          {request.refNo}
                        </Badge>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-2">
                          {request.sourceType === 'Supplier' && <Truck className="h-3 w-3 text-green-600" />}
                          {request.sourceType === 'Leftover Return' && <RotateCcw className="h-3 w-3 text-orange-600" />}
                          {request.sourceType === 'Transfer In' && <RefreshCw className="h-3 w-3 text-blue-600" />}
                          <div>
                            <div className="font-medium">{request.source}</div>
                            <div className="text-xs text-slate-500">{request.sourceType}</div>
                          </div>
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-2">
                          <div 
                            className="w-4 h-4 rounded-full border border-gray-300"
                            style={{ backgroundColor: request.color.hex }}
                          />
                          <div>
                            <div className="font-medium">{request.material}</div>
                            <div className="text-sm text-slate-500">{request.color.name}</div>
                          </div>
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-1 text-green-600 font-semibold">
                          <TrendingUp className="h-3 w-3" />
                          +{request.qty.toLocaleString()} {request.unit}
                        </div>
                      </TableCell>
                      <TableCell>
                        {getStatusBadge(request.status)}
                      </TableCell>
                      <TableCell>
                        {getActionButton(request, 'stockIn')}
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          </CardContent>
        </Card>

        {/* Stock Out Requests */}
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <ArrowDown className="h-5 w-5 text-red-600" />
              Stock Out Requests | ထွက်ရန်တောင်းခံမှုများ
              <Badge className="bg-red-100 text-red-800 ml-auto">
                {stockOutRequests.length} requests
              </Badge>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Time | အချိန်</TableHead>
                    <TableHead>Ref No. | ကိုးကားနံပါတ်</TableHead>
                    <TableHead>Destination | သွားရောက်ရာ</TableHead>
                    <TableHead>Material + Color | ပစ္စည်း + အရောင်</TableHead>
                    <TableHead>Qty | ပမာဏ</TableHead>
                    <TableHead>Status | အခြေအနေ</TableHead>
                    <TableHead>Action | လုပ်ဆောင်ချက်</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {stockOutRequests.map((request) => (
                    <TableRow key={request.id} className={getRowClassName(request)}>
                      <TableCell className="font-mono text-sm">{request.time}</TableCell>
                      <TableCell>
                        <Badge variant="outline" className="font-mono text-xs">
                          {request.refNo}
                        </Badge>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-2">
                          {request.destinationType === 'Issue to Production' && <Factory className="h-3 w-3 text-purple-600" />}
                          {request.destinationType === 'Scrap/Damage' && <AlertTriangle className="h-3 w-3 text-red-600" />}
                          {(request.destinationType === 'Return to Supplier' || request.destinationType === 'Return to HQ') && <Truck className="h-3 w-3 text-yellow-600" />}
                          <div>
                            <div className="font-medium">{request.destination}</div>
                            <div className="text-xs text-slate-500">{request.destinationType}</div>
                          </div>
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-2">
                          <div 
                            className="w-4 h-4 rounded-full border border-gray-300"
                            style={{ backgroundColor: request.color.hex }}
                          />
                          <div>
                            <div className="font-medium">{request.material}</div>
                            <div className="text-sm text-slate-500">{request.color.name}</div>
                          </div>
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-1 text-red-600 font-semibold">
                          <TrendingDown className="h-3 w-3" />
                          -{request.qty.toLocaleString()} {request.unit}
                        </div>
                      </TableCell>
                      <TableCell>
                        {getStatusBadge(request.status)}
                      </TableCell>
                      <TableCell>
                        {getActionButton(request, 'stockOut')}
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          </CardContent>
        </Card>

        {/* Transfer Requests */}
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <RefreshCw className="h-5 w-5 text-blue-600" />
              Transfer Requests | လွှဲပြောင်းတောင်းခံမှုများ
              <Badge className="bg-blue-100 text-blue-800 ml-auto">
                {transferRequests.length} requests
              </Badge>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Time | အချိန်</TableHead>
                    <TableHead>Ref No. | ကိုးကားနံပါတ်</TableHead>
                    <TableHead>From → To | မှ → သို့</TableHead>
                    <TableHead>Material + Color | ပစ္စည်း + အရောင်</TableHead>
                    <TableHead>Qty | ပမာဏ</TableHead>
                    <TableHead>Status | အခြေအနေ</TableHead>
                    <TableHead>Action | လုပ်ဆောင်ချက်</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {transferRequests.map((request) => (
                    <TableRow key={request.id} className={getRowClassName(request)}>
                      <TableCell className="font-mono text-sm">{request.time}</TableCell>
                      <TableCell>
                        <Badge variant="outline" className="font-mono text-xs">
                          {request.refNo}
                        </Badge>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-2">
                          {request.transferType === 'Warehouse Transfer' && <Building2 className="h-3 w-3 text-blue-600" />}
                          {request.transferType === 'Job Reallocation' && <RefreshCw className="h-3 w-3 text-teal-600" />}
                          <div>
                            <div className="font-medium text-sm">{request.from} → {request.to}</div>
                            <div className="text-xs text-slate-500">{request.transferType}</div>
                          </div>
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-2">
                          <div 
                            className="w-4 h-4 rounded-full border border-gray-300"
                            style={{ backgroundColor: request.color.hex }}
                          />
                          <div>
                            <div className="font-medium">{request.material}</div>
                            <div className="text-sm text-slate-500">{request.color.name}</div>
                          </div>
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-1 font-semibold">
                          <RefreshCw className="h-3 w-3 text-blue-600" />
                          {request.qty.toLocaleString()} {request.unit}
                        </div>
                      </TableCell>
                      <TableCell>
                        {getStatusBadge(request.status)}
                      </TableCell>
                      <TableCell>
                        {getActionButton(request, 'transfer')}
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          </CardContent>
        </Card>

        {/* Job-to-Job Transfer Requests */}
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <ArrowRightLeft className="h-5 w-5 text-purple-600" />
              Job-to-Job Transfer Requests | အလုပ်မှအလုပ်သို့ လွှဲပြောင်းတောင်းခံမှုများ
              <Badge className="bg-purple-100 text-purple-800 ml-auto">
                {jobTransferRequests.length} requests
              </Badge>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Time | အချိန်</TableHead>
                    <TableHead>Ref No. | ကိုးကားနံပါတ်</TableHead>
                    <TableHead>From Job (Machine)</TableHead>
                    <TableHead>To Job (Machine)</TableHead>
                    <TableHead>Material + Color</TableHead>
                    <TableHead>Qty | ပမာဏ</TableHead>
                    <TableHead>Reason</TableHead>
                    <TableHead>Status</TableHead>
                    <TableHead>Action</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {jobTransferRequests.map((request) => (
                    <TableRow key={request.id} className={getRowClassName(request)}>
                      <TableCell className="font-mono text-sm">{request.time}</TableCell>
                      <TableCell>
                        <Badge variant="outline" className="font-mono text-xs">
                          {request.refNo}
                        </Badge>
                      </TableCell>
                      <TableCell>
                        <div>
                          <div className="font-medium text-sm">{request.fromJob}</div>
                          <div className="text-xs text-slate-500">({request.fromMachine})</div>
                        </div>
                      </TableCell>
                      <TableCell>
                        <div>
                          <div className="font-medium text-sm">{request.toJob}</div>
                          <div className="text-xs text-slate-500">({request.toMachine})</div>
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-2">
                          <div 
                            className="w-4 h-4 rounded-full border border-gray-300"
                            style={{ backgroundColor: request.color.hex }}
                          />
                          <div>
                            <div className="font-medium text-sm">{request.material}</div>
                            <div className="text-xs text-slate-500">{request.color.name}</div>
                          </div>
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-1 font-semibold text-purple-600">
                          <ArrowRightLeft className="h-3 w-3" />
                          {request.qty.toLocaleString()} {request.unit}
                        </div>
                      </TableCell>
                      <TableCell>
                        <Badge variant="outline" className="text-xs">
                          {request.reason}
                        </Badge>
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
                        {request.status === 'Pending Acknowledgement (Warehouse)' ? (
                          <div className="flex gap-1">
                            <Button 
                              size="sm" 
                              className="bg-green-600 hover:bg-green-700 text-white text-xs px-2 py-1"
                              onClick={() => handleJobTransferAction(request, 'acknowledge')}
                            >
                              ✅ Acknowledge
                            </Button>
                            <Button 
                              size="sm" 
                              variant="outline"
                              className="border-red-500 text-red-600 text-xs px-2 py-1"
                              onClick={() => handleJobTransferAction(request, 'reject')}
                            >
                              ❌ Reject
                            </Button>
                          </div>
                        ) : request.status === 'Acknowledged' ? (
                          <Badge className="bg-green-100 text-green-800">
                            ✅ Completed
                          </Badge>
                        ) : (
                          <Badge className="bg-red-100 text-red-800">
                            ❌ Rejected
                          </Badge>
                        )}
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Bottom Section - Stock Balance & Movement */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Stock Balance */}
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Package className="h-5 w-5 text-green-600" />
              Stock Balance | လက်ရှိစတော့ခ်
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Material Code</TableHead>
                    <TableHead>Material Name</TableHead>
                    <TableHead>Color</TableHead>
                    <TableHead>Current</TableHead>
                    <TableHead>Reserved</TableHead>
                    <TableHead>Min Stock</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {stockBalance.map((item) => (
                    <TableRow key={item.id} className="hover:bg-slate-50">
                      <TableCell>
                        <Badge variant="outline" className="font-mono text-xs">
                          {item.materialCode}
                        </Badge>
                      </TableCell>
                      <TableCell className="font-medium">{item.materialName}</TableCell>
                      <TableCell>
                        <div className="flex items-center gap-2">
                          <div 
                            className="w-4 h-4 rounded-full border border-gray-300"
                            style={{ backgroundColor: item.color.hex }}
                          />
                          <span className="text-sm">{item.color.name}</span>
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-1">
                          <Weight className="h-3 w-3 text-blue-500" />
                          <span className="font-semibold">{item.currentStock.toLocaleString()}</span>
                        </div>
                      </TableCell>
                      <TableCell>
                        <span className="text-yellow-600 font-medium">
                          {item.reservedStock.toLocaleString()}
                        </span>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-1">
                          <span>{item.minStock.toLocaleString()}</span>
                          {item.isLowStock && (
                            <AlertTriangle className="h-3 w-3 text-red-500" title="⚠️ Below safe level" />
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

        {/* Movement History */}
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Clock className="h-5 w-5 text-purple-600" />
              Movement History (Last 10) | လှုပ်ရှားမှုသမိုင်း
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-3 max-h-96 overflow-y-auto">
              {movementHistory.map((movement) => (
                <div key={movement.id} className="flex items-center justify-between p-3 bg-slate-50 rounded-lg hover:bg-slate-100">
                  <div className="flex items-center gap-3">
                    <div className="flex items-center gap-2">
                      {movement.qty > 0 ? (
                        <TrendingUp className="h-4 w-4 text-green-500" />
                      ) : (
                        <TrendingDown className="h-4 w-4 text-red-500" />
                      )}
                      <div 
                        className="w-4 h-4 rounded-full border border-gray-300"
                        style={{ backgroundColor: movement.color.hex }}
                      />
                    </div>
                    <div>
                      <div className="font-medium text-sm">{movement.material}</div>
                      <div className="text-xs text-slate-500 flex items-center gap-2">
                        <span>{movement.type}</span>
                        <Badge variant="outline" className="text-xs">{movement.refNo}</Badge>
                        <Badge className="bg-green-100 text-green-800 text-xs">
                          <CheckCircle className="h-2 w-2 mr-1" />
                          {movement.status}
                        </Badge>
                      </div>
                    </div>
                  </div>
                  <div className="text-right">
                    <div className={`font-semibold text-sm ${movement.qty > 0 ? 'text-green-600' : 'text-red-600'}`}>
                      {movement.qty > 0 ? '+' : ''}{movement.qty.toLocaleString()} {movement.unit}
                    </div>
                    <div className="text-xs text-slate-500">
                      {format(new Date(movement.dateTime), 'MMM dd, HH:mm')}
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Process Modal */}
      <Dialog open={showProcessModal} onOpenChange={setShowProcessModal}>
        <DialogContent className="max-w-md">
          <DialogHeader>
            <DialogTitle className="flex items-center gap-2">
              {modalType === 'accept' && <CheckCircle className="h-5 w-5 text-green-600" />}
              {modalType === 'issue' && <PackageX className="h-5 w-5 text-purple-600" />}
              {modalType === 'scrap' && <AlertTriangle className="h-5 w-5 text-red-600" />}
              {modalType === 'return' && <Truck className="h-5 w-5 text-yellow-700" />}
              {modalType === 'process' && <RefreshCw className="h-5 w-5 text-blue-600" />}
              Confirm Action | လုပ်ဆောင်ချက်အတည်ပြု
            </DialogTitle>
            <DialogDescription>
              Confirm this action for the selected request
            </DialogDescription>
          </DialogHeader>
          
          {processingItem && (
            <div className="space-y-4 py-4">
              <div className="bg-blue-50 rounded-lg p-3 border border-blue-200">
                <div className="grid grid-cols-2 gap-3 text-sm">
                  <div>
                    <span className="text-slate-500">Ref No:</span>
                    <div className="font-medium">{processingItem.refNo}</div>
                  </div>
                  <div>
                    <span className="text-slate-500">Material:</span>
                    <div className="font-medium">{processingItem.material}</div>
                  </div>
                  <div>
                    <span className="text-slate-500">Color:</span>
                    <div className="flex items-center gap-1">
                      <div 
                        className="w-3 h-3 rounded-full border"
                        style={{ backgroundColor: processingItem.color.hex }}
                      />
                      {processingItem.color.name}
                    </div>
                  </div>
                  <div>
                    <span className="text-slate-500">Quantity:</span>
                    <div className="font-medium text-blue-600">
                      {processingItem.qty.toLocaleString()} {processingItem.unit}
                    </div>
                  </div>
                </div>
              </div>
              
              <div className="text-center">
                <p className="text-sm text-slate-600">
                  Are you sure you want to{' '}
                  <strong>
                    {modalType === 'accept' && 'accept this material into stock'}
                    {modalType === 'issue' && 'issue this material to production'}
                    {modalType === 'scrap' && 'record this material as scrap'}
                    {modalType === 'return' && 'process this return'}
                    {modalType === 'process' && 'process this transfer'}
                  </strong>
                  ?
                </p>
              </div>
            </div>
          )}
          
          <div className="flex justify-end gap-3 pt-4 border-t">
            <Button variant="outline" onClick={() => setShowProcessModal(false)}>
              Cancel | ပယ်ဖျက်ပါ
            </Button>
            <Button 
              onClick={confirmAction} 
              className={`${
                modalType === 'accept' ? 'bg-green-600 hover:bg-green-700' :
                modalType === 'issue' ? 'bg-purple-600 hover:bg-purple-700' :
                modalType === 'scrap' ? 'bg-red-600 hover:bg-red-700' :
                modalType === 'return' ? 'bg-yellow-700 hover:bg-yellow-800' :
                'bg-blue-600 hover:bg-blue-700'
              }`}
            >
              <Printer className="h-4 w-4 mr-2" />
              Confirm | အတည်ပြု
            </Button>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );
}