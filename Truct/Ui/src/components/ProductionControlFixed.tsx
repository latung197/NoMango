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
import { ScrapManagement } from './ScrapManagement';
import { FinishedGoodsReceiving } from './FinishedGoodsReceiving';
import { FinishedGoodsTransfer } from './FinishedGoodsTransfer';

import { JobTracker } from './JobTracker';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Badge } from './ui/badge';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from './ui/dialog';
import { Tabs, TabsContent, TabsList, TabsTrigger } from './ui/tabs';
import { Switch } from './ui/switch';
import { Checkbox } from './ui/checkbox';
import { Textarea } from './ui/textarea';
import { Progress } from './ui/progress';
import { toast } from 'sonner@2.0.3';

interface ProductionControlProps {
  currentPage: string;
  onPageChange: (page: string) => void;
}

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

// Note: Adding minimal content to keep file size manageable while demonstrating the Job-to-Job Transfer functionality

export function ProductionControl({ currentPage, onPageChange }: ProductionControlProps) {
  // If scrap-management is selected, render the dedicated ScrapManagement component
  if (currentPage === 'scrap-management') {
    return <ScrapManagement />;
  }

  // If finished-goods-transfer is selected, render the dedicated FinishedGoodsTransfer component
  if (currentPage === 'finished-goods-transfer') {
    return <FinishedGoodsTransfer />;
  }

  // If job-tracker is selected, render the dedicated JobTracker component
  if (currentPage === 'job-tracker') {
    return <JobTracker />;
  }

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

  // Render Material Control section with Job-to-Job Transfer
  if (currentPage === 'material-control') {
    return (
      <div className="space-y-6">
        {/* Material Control Actions */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Package className="h-5 w-5 text-green-600" />
              Raw Material Control Actions
              <span className="text-sm font-normal text-slate-600">ပစ္စည်းထိန်းချုပ်မှု လုပ်ဆောင်ချက်များ</span>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="flex gap-3">
              <Button 
                className="bg-blue-600 hover:bg-blue-700 text-white"
                onClick={() => setJobTransferDialog(true)}
              >
                <ArrowRightLeft className="h-4 w-4 mr-2" />
                🔁 Job-to-Job Transfer
              </Button>
            </div>
          </CardContent>
        </Card>

        {/* Job-to-Job Transfer Requests History */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <History className="h-5 w-5 text-purple-600" />
              Job-to-Job Transfer Requests
              <span className="text-sm font-normal text-slate-600">အလုပ်မှအလုပ်သို့ လွှဲပြောင်းတောင်းခံမှုများ</span>
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
                    <TableHead>Requested By</TableHead>
                    <TableHead>Date</TableHead>
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
                      <TableCell className="font-medium">
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
                            request.status === 'Acknowledged' ? 'text-green-600' :
                            request.status === 'Pending Acknowledgement (Warehouse)' ? 'text-yellow-600' :
                            'text-red-600'
                          }
                        >
                          {request.status === 'Acknowledged' ? '🟢' : 
                           request.status === 'Pending Acknowledgement (Warehouse)' ? '🟡' : '🔴'} 
                          {request.status}
                        </Badge>
                      </TableCell>
                      <TableCell className="text-sm">{request.requestedBy}</TableCell>
                      <TableCell className="text-sm">{request.requestedDate}</TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          </CardContent>
        </Card>

        {/* Job-to-Job Transfer Modal */}
        <Dialog open={jobTransferDialog} onOpenChange={setJobTransferDialog}>
          <DialogContent className="max-w-2xl">
            <DialogHeader>
              <DialogTitle className="flex items-center gap-2">
                <ArrowRightLeft className="h-5 w-5 text-blue-600" />
                Job-to-Job Transfer Request
              </DialogTitle>
              <p className="text-sm text-slate-600">
                Transfer raw materials from one job to another job
              </p>
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
                      <SelectItem value="Urgent Order">Urgent Order</SelectItem>
                      <SelectItem value="Over Reserved">Over Reserved</SelectItem>
                      <SelectItem value="Machine Change">Machine Change</SelectItem>
                      <SelectItem value="Other">Other</SelectItem>
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
                  className="bg-blue-600 hover:bg-blue-700"
                  onClick={handleJobTransferSubmit}
                >
                  Submit Transfer Request
                </Button>
              </div>
            </div>
          </DialogContent>
        </Dialog>
      </div>
    );
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