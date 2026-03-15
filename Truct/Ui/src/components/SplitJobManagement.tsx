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
import { 
  GitBranch,
  Package,
  Plus,
  Eye,
  CheckCircle,
  XCircle,
  Clock,
  ArrowRight,
  Copy,
  Split,
  Users,
  Settings
} from 'lucide-react';
import { toast } from 'sonner@2.0.3';

// Mock data for active jobs
const mockActiveJobs = [
  {
    jobId: 'JOB20250903-001',
    product: '2011 - Plastic Bottle 500ml',
    machine: 'INJ-M001',
    totalQty: 10000,
    completedQty: 3500,
    remainingQty: 6500,
    operator: 'Ko Thant (OP001)',
    shift: 'Day Shift',
    status: 'Running',
    canSplit: true
  },
  {
    jobId: 'JOB20250903-002',
    product: '2012 - Plastic Container 1L',
    machine: 'INJ-M002',
    totalQty: 8000,
    completedQty: 2100,
    remainingQty: 5900,
    operator: 'Ma Hnin (OP002)',
    shift: 'Day Shift',
    status: 'Running',
    canSplit: true
  },
  {
    jobId: 'JOB20250903-003',
    product: '2013 - Plastic Cup 200ml',
    machine: 'INJ-M003',
    totalQty: 15000,
    completedQty: 8200,
    remainingQty: 6800,
    operator: 'Ko Zaw (OP003)',
    shift: 'Day Shift',
    status: 'Running',
    canSplit: true
  }
];

// Mock data for split job requests
const mockSplitRequests = [
  {
    id: 'SPL-REQ-001',
    originalJobId: 'JOB20250903-001',
    product: '2011 - Plastic Bottle 500ml',
    machine: 'INJ-M001',
    totalQty: 10000,
    completedQty: 3500,
    remainingQty: 6500,
    splitInto: 2,
    newJobs: [
      { jobId: 'JOB20250903-001A', qty: 3000 },
      { jobId: 'JOB20250903-001B', qty: 3500 }
    ],
    reason: 'Shift Change - Different Operators',
    requestedBy: 'Production Supervisor',
    requestedTime: '2025-09-08 14:30',
    status: 'Pending Approval',
    approvedBy: null,
    approvedTime: null
  },
  {
    id: 'SPL-REQ-002',
    originalJobId: 'JOB20250903-002',
    product: '2012 - Plastic Container 1L',
    machine: 'INJ-M002',
    totalQty: 8000,
    completedQty: 2100,
    remainingQty: 5900,
    splitInto: 3,
    newJobs: [
      { jobId: 'JOB20250903-002A', qty: 2000 },
      { jobId: 'JOB20250903-002B', qty: 2000 },
      { jobId: 'JOB20250903-002C', qty: 1900 }
    ],
    reason: 'Quality Issue - Different Material Batch',
    requestedBy: 'QC Supervisor',
    requestedTime: '2025-09-08 13:15',
    status: 'Approved',
    approvedBy: 'Production Manager',
    approvedTime: '2025-09-08 13:45'
  }
];

const splitReasons = [
  'Shift Change - Different Operators',
  'Quality Issue - Different Material Batch',
  'Machine Maintenance Required',
  'Urgent Order Priority Change',
  'Material Supply Issue',
  'Operator Skill Requirement',
  'Equipment Capacity Limitation'
];

export function SplitJobManagement() {
  const [splitJobDialog, setSplitJobDialog] = useState(false);
  const [splitFormData, setSplitFormData] = useState({
    jobId: '',
    splitInto: '2',
    reason: '',
    notes: '',
    splitJobs: [
      { qty: '', operator: '' },
      { qty: '', operator: '' }
    ]
  });

  const getStatusBadge = (status: string) => {
    switch (status) {
      case 'Pending Approval':
        return <Badge className="bg-yellow-100 text-yellow-800">⏳ Pending</Badge>;
      case 'Approved':
        return <Badge className="bg-green-100 text-green-800">✅ Approved</Badge>;
      case 'Rejected':
        return <Badge className="bg-red-100 text-red-800">❌ Rejected</Badge>;
      case 'Completed':
        return <Badge className="bg-blue-100 text-blue-800">🏁 Completed</Badge>;
      default:
        return <Badge variant="outline">{status}</Badge>;
    }
  };

  const getJobStatusBadge = (status: string) => {
    switch (status) {
      case 'Running':
        return <Badge className="bg-green-100 text-green-800">🟢 Running</Badge>;
      case 'Paused':
        return <Badge className="bg-yellow-100 text-yellow-800">⏸️ Paused</Badge>;
      case 'Completed':
        return <Badge className="bg-blue-100 text-blue-800">✅ Completed</Badge>;
      default:
        return <Badge variant="outline">{status}</Badge>;
    }
  };

  const handleSplitCountChange = (count: string) => {
    const numSplits = parseInt(count) || 2;
    const newSplitJobs = Array(numSplits).fill(null).map(() => ({ qty: '', operator: '' }));
    setSplitFormData(prev => ({
      ...prev,
      splitInto: count,
      splitJobs: newSplitJobs
    }));
  };

  const updateSplitJob = (index: number, field: 'qty' | 'operator', value: string) => {
    setSplitFormData(prev => {
      const newSplitJobs = [...prev.splitJobs];
      newSplitJobs[index] = { ...newSplitJobs[index], [field]: value };
      return { ...prev, splitJobs: newSplitJobs };
    });
  };

  const getSelectedJob = () => {
    return mockActiveJobs.find(job => job.jobId === splitFormData.jobId);
  };

  const getTotalSplitQty = () => {
    return splitFormData.splitJobs.reduce((total, job) => {
      return total + (parseInt(job.qty) || 0);
    }, 0);
  };

  const handleSplitSubmit = () => {
    const { jobId, splitInto, reason, splitJobs } = splitFormData;
    const selectedJob = getSelectedJob();
    
    if (!jobId || !reason) {
      toast.error('Please fill in all required fields');
      return;
    }

    if (!selectedJob) {
      toast.error('Selected job not found');
      return;
    }

    // Validate split quantities
    const totalSplitQty = getTotalSplitQty();
    if (totalSplitQty === 0) {
      toast.error('Please enter quantities for all split jobs');
      return;
    }

    if (totalSplitQty > selectedJob.remainingQty) {
      toast.error(`Total split quantity (${totalSplitQty}) cannot exceed remaining quantity (${selectedJob.remainingQty})`);
      return;
    }

    const newRequest = {
      id: `SPL-REQ-${String(mockSplitRequests.length + 1).padStart(3, '0')}`,
      originalJobId: jobId,
      product: selectedJob.product,
      machine: selectedJob.machine,
      totalQty: selectedJob.totalQty,
      completedQty: selectedJob.completedQty,
      remainingQty: selectedJob.remainingQty,
      splitInto: parseInt(splitInto),
      newJobs: splitJobs.map((job, index) => ({
        jobId: `${jobId}${String.fromCharCode(65 + index)}`, // A, B, C, etc.
        qty: parseInt(job.qty) || 0
      })),
      reason,
      requestedBy: 'Production Supervisor',
      requestedTime: new Date().toISOString().slice(0, 16).replace('T', ' '),
      status: 'Pending Approval',
      approvedBy: null,
      approvedTime: null
    };

    mockSplitRequests.unshift(newRequest);
    toast.success('✅ Split Job Request Created Successfully');
    setSplitJobDialog(false);
    setSplitFormData({
      jobId: '',
      splitInto: '2',
      reason: '',
      notes: '',
      splitJobs: [
        { qty: '', operator: '' },
        { qty: '', operator: '' }
      ]
    });
  };

  return (
    <div className="space-y-6">
      {/* Header with Action Buttons */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <GitBranch className="h-5 w-5 text-purple-600" />
            Split Job Management
            <span className="text-sm font-normal text-slate-600">အလုပ် ခွဲခြမ်းခြင်း</span>
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex gap-3">
            <Button 
              onClick={() => setSplitJobDialog(true)}
              className="bg-purple-600 hover:bg-purple-700 text-white"
            >
              <Split className="h-4 w-4 mr-2" />
              🔀 Create Split Request
            </Button>
            <Button variant="outline" className="text-blue-600 border-blue-600 hover:bg-blue-50">
              <Eye className="h-4 w-4 mr-2" />
              📊 Split Analytics
            </Button>
            <Button variant="outline" className="text-green-600 border-green-600 hover:bg-green-50">
              <Settings className="h-4 w-4 mr-2" />
              ⚙️ Split Settings
            </Button>
          </div>
        </CardContent>
      </Card>

      {/* Active Jobs Available for Splitting */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Package className="h-5 w-5 text-green-600" />
            Active Jobs (Available for Splitting) | အလုပ်များ (ခွဲရန်အဆင်သင့်)
            <Badge className="bg-green-100 text-green-800 ml-auto">
              {mockActiveJobs.filter(job => job.canSplit).length} jobs
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
                  <TableHead>Remaining Qty</TableHead>
                  <TableHead>Operator</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {mockActiveJobs.filter(job => job.canSplit).map((job) => (
                  <TableRow key={job.jobId}>
                    <TableCell>
                      <Badge variant="outline" className="font-mono text-xs">
                        {job.jobId}
                      </Badge>
                    </TableCell>
                    <TableCell className="max-w-[200px]">
                      <div className="truncate" title={job.product}>
                        {job.product}
                      </div>
                    </TableCell>
                    <TableCell>
                      <Badge variant="outline" className="bg-blue-50 text-blue-700">
                        {job.machine}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <div>
                        <div className="flex items-center gap-2">
                          <div className="w-16 bg-slate-200 rounded-full h-2">
                            <div 
                              className="bg-green-500 h-2 rounded-full"
                              style={{ width: `${(job.completedQty / job.totalQty) * 100}%` }}
                            />
                          </div>
                          <span className="text-xs text-slate-600">
                            {Math.round((job.completedQty / job.totalQty) * 100)}%
                          </span>
                        </div>
                        <div className="text-xs text-slate-500 mt-1">
                          {job.completedQty.toLocaleString()} / {job.totalQty.toLocaleString()}
                        </div>
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-1 text-blue-600 font-semibold">
                        <Package className="h-3 w-3" />
                        {job.remainingQty.toLocaleString()} pcs
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="text-sm">
                        <div className="font-medium">{job.operator}</div>
                        <div className="text-xs text-slate-500">{job.shift}</div>
                      </div>
                    </TableCell>
                    <TableCell>
                      {getJobStatusBadge(job.status)}
                    </TableCell>
                    <TableCell>
                      <Button 
                        size="sm" 
                        variant="outline"
                        className="text-purple-600 border-purple-600 hover:bg-purple-50"
                        onClick={() => {
                          setSplitFormData(prev => ({ ...prev, jobId: job.jobId }));
                          setSplitJobDialog(true);
                        }}
                      >
                        <Split className="h-3 w-3 mr-1" />
                        Split
                      </Button>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        </CardContent>
      </Card>

      {/* Split Requests Table */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <GitBranch className="h-5 w-5 text-orange-600" />
            Split Job Requests | အလုပ်ခွဲ တောင်းခံမှုများ
            <Badge className="bg-orange-100 text-orange-800 ml-auto">
              {mockSplitRequests.length} requests
            </Badge>
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Request ID</TableHead>
                  <TableHead>Original Job</TableHead>
                  <TableHead>Product</TableHead>
                  <TableHead>Split Into</TableHead>
                  <TableHead>Reason</TableHead>
                  <TableHead>Requested By</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {mockSplitRequests.map((request) => (
                  <TableRow key={request.id}>
                    <TableCell>
                      <Badge variant="outline" className="font-mono text-xs">
                        {request.id}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <div>
                        <div className="font-medium text-sm">{request.originalJobId}</div>
                        <div className="text-xs text-slate-500">{request.machine}</div>
                      </div>
                    </TableCell>
                    <TableCell className="max-w-[150px]">
                      <div className="truncate" title={request.product}>
                        {request.product}
                      </div>
                    </TableCell>
                    <TableCell>
                      <div>
                        <div className="flex items-center gap-1 text-purple-600 font-semibold">
                          <GitBranch className="h-3 w-3" />
                          {request.splitInto} jobs
                        </div>
                        <div className="text-xs text-slate-500 mt-1">
                          {request.newJobs.map(job => `${job.qty}`).join(' + ')} = {request.newJobs.reduce((total, job) => total + job.qty, 0)} pcs
                        </div>
                      </div>
                    </TableCell>
                    <TableCell className="max-w-[120px]">
                      <div className="text-xs truncate" title={request.reason}>
                        {request.reason}
                      </div>
                    </TableCell>
                    <TableCell className="text-sm">
                      <div className="font-medium">{request.requestedBy}</div>
                      <div className="text-xs text-slate-500 font-mono">{request.requestedTime}</div>
                    </TableCell>
                    <TableCell>
                      {getStatusBadge(request.status)}
                    </TableCell>
                    <TableCell>
                      <div className="flex gap-1">
                        <Button size="sm" variant="ghost" title="View Details">
                          <Eye className="h-3 w-3" />
                        </Button>
                        {request.status === 'Pending Approval' && (
                          <Button 
                            size="sm" 
                            variant="outline"
                            className="text-green-600 border-green-600 hover:bg-green-50"
                            title="Approve"
                          >
                            <CheckCircle className="h-3 w-3" />
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

      {/* Split Job Request Dialog */}
      <Dialog open={splitJobDialog} onOpenChange={setSplitJobDialog}>
        <DialogContent className="max-w-4xl">
          <DialogHeader>
            <DialogTitle className="flex items-center gap-2">
              <Split className="h-5 w-5 text-purple-600" />
              Create Split Job Request
            </DialogTitle>
            <DialogDescription>
              Split an active job into multiple smaller jobs for better management
            </DialogDescription>
          </DialogHeader>
          <div className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label>Select Job to Split</Label>
                <Select
                  value={splitFormData.jobId}
                  onValueChange={(value) => setSplitFormData(prev => ({ ...prev, jobId: value }))}
                >
                  <SelectTrigger>
                    <SelectValue placeholder="Select active job" />
                  </SelectTrigger>
                  <SelectContent>
                    {mockActiveJobs.filter(job => job.canSplit).map((job) => (
                      <SelectItem key={job.jobId} value={job.jobId}>
                        {job.jobId} - {job.product} (Remaining: {job.remainingQty.toLocaleString()})
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>

              <div>
                <Label>Split Into</Label>
                <Select
                  value={splitFormData.splitInto}
                  onValueChange={handleSplitCountChange}
                >
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="2">2 Jobs</SelectItem>
                    <SelectItem value="3">3 Jobs</SelectItem>
                    <SelectItem value="4">4 Jobs</SelectItem>
                    <SelectItem value="5">5 Jobs</SelectItem>
                  </SelectContent>
                </Select>
              </div>
            </div>

            {splitFormData.jobId && getSelectedJob() && (
              <div className="p-4 bg-slate-50 rounded-lg">
                <h4 className="font-medium text-sm mb-2">Job Information</h4>
                <div className="grid grid-cols-4 gap-4 text-sm">
                  <div>
                    <span className="text-slate-600">Product:</span>
                    <div className="font-medium">{getSelectedJob()?.product}</div>
                  </div>
                  <div>
                    <span className="text-slate-600">Machine:</span>
                    <div className="font-medium">{getSelectedJob()?.machine}</div>
                  </div>
                  <div>
                    <span className="text-slate-600">Completed:</span>
                    <div className="font-medium">{getSelectedJob()?.completedQty.toLocaleString()} pcs</div>
                  </div>
                  <div>
                    <span className="text-slate-600">Remaining:</span>
                    <div className="font-medium text-blue-600">{getSelectedJob()?.remainingQty.toLocaleString()} pcs</div>
                  </div>
                </div>
              </div>
            )}

            <div>
              <Label>Split Job Quantities</Label>
              <div className="grid grid-cols-2 gap-4 mt-2">
                {splitFormData.splitJobs.map((job, index) => (
                  <div key={index} className="p-3 border rounded-lg">
                    <div className="text-sm font-medium mb-2">
                      Job {splitFormData.jobId}{String.fromCharCode(65 + index)}
                    </div>
                    <div className="space-y-2">
                      <div>
                        <Label className="text-xs">Quantity</Label>
                        <Input
                          type="number"
                          placeholder="Enter quantity"
                          value={job.qty}
                          onChange={(e) => updateSplitJob(index, 'qty', e.target.value)}
                        />
                      </div>
                      <div>
                        <Label className="text-xs">Assigned Operator (Optional)</Label>
                        <Input
                          placeholder="Enter operator ID"
                          value={job.operator}
                          onChange={(e) => updateSplitJob(index, 'operator', e.target.value)}
                        />
                      </div>
                    </div>
                  </div>
                ))}
              </div>
              
              {getSelectedJob() && (
                <div className="text-sm mt-2">
                  <span className="text-slate-600">Total Split Quantity: </span>
                  <span className={`font-medium ${getTotalSplitQty() > getSelectedJob()?.remainingQty! ? 'text-red-600' : 'text-green-600'}`}>
                    {getTotalSplitQty().toLocaleString()} / {getSelectedJob()?.remainingQty.toLocaleString()} pcs
                  </span>
                </div>
              )}
            </div>

            <div>
              <Label>Reason for Split</Label>
              <Select
                value={splitFormData.reason}
                onValueChange={(value) => setSplitFormData(prev => ({ ...prev, reason: value }))}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Select reason for split" />
                </SelectTrigger>
                <SelectContent>
                  {splitReasons.map((reason) => (
                    <SelectItem key={reason} value={reason}>
                      {reason}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div>
              <Label>Additional Notes</Label>
              <Textarea
                placeholder="Add any additional notes or special instructions..."
                value={splitFormData.notes}
                onChange={(e) => setSplitFormData(prev => ({ ...prev, notes: e.target.value }))}
                rows={2}
              />
            </div>

            <div className="flex justify-end gap-3">
              <Button
                variant="outline"
                onClick={() => setSplitJobDialog(false)}
              >
                Cancel
              </Button>
              <Button
                onClick={handleSplitSubmit}
                className="bg-purple-600 hover:bg-purple-700 text-white"
              >
                <GitBranch className="h-4 w-4 mr-2" />
                Create Split Request
              </Button>
            </div>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );
}