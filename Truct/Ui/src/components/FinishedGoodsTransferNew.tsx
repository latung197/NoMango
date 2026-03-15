import React, { useState } from 'react';
import {
  Package,
  CheckCircle,
  AlertTriangle,
  Search,
  Filter,
  Download,
  User,
  Calendar,
  Factory,
  AlertCircle,
  TrendingUp,
  Clock,
  FileText,
  Eye
} from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Badge } from './ui/badge';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from './ui/dialog';

// Mock data for finished jobs waiting for transfer
const mockFinishedJobsData = [
  {
    jobId: 'JOB20250907-001',
    parentPlanId: 'PLAN20250907-001',
    product: '2011 - Plastic Bottle 500ml',
    producedQty: 10000,
    goodQty: 9850,
    defectQty: 100,
    scrapQty: 50,
    qcStatus: 'passed',
    transferStatus: 'pending',
    operator: 'Operator A (OP001)',
    machine: 'INJ-M001',
    shift: 'Day Shift',
    completedDate: '2025-09-07 14:30',
    qcOfficer: 'QC Officer 1',
    supervisorApproval: 'Ko Aung (Supervisor)',
    totalReleased: 0,
    transferHistory: [],
    parentPlanComplete: true
  },
  {
    jobId: 'JOB20250907-002',
    parentPlanId: 'PLAN20250907-002',
    product: '2013 - Plastic Cup 250ml',
    producedQty: 8000,
    goodQty: 7800,
    defectQty: 150,
    scrapQty: 50,
    qcStatus: 'passed',
    transferStatus: 'partial',
    operator: 'Operator B (OP002)',
    machine: 'INJ-M002',
    shift: 'Day Shift',
    completedDate: '2025-09-07 15:45',
    qcOfficer: 'QC Officer 2',
    supervisorApproval: 'Ma Thin (Supervisor)',
    totalReleased: 5000,
    transferHistory: [
      { date: '2025-09-07 16:30', qty: 5000, approver: 'Ma Thin (Supervisor)', timestamp: '2025-09-07 16:30:00' }
    ],
    parentPlanComplete: true
  },
  {
    jobId: 'JOB20250907-003',
    parentPlanId: 'PLAN20250907-001',
    product: '2011 - Plastic Bottle 500ml',
    producedQty: 5000,
    goodQty: 4900,
    defectQty: 75,
    scrapQty: 25,
    qcStatus: 'passed',
    transferStatus: 'pending',
    operator: 'Operator C (OP003)',
    machine: 'INJ-M003',
    shift: 'Night Shift',
    completedDate: '2025-09-07 20:00',
    qcOfficer: 'QC Officer 3',
    supervisorApproval: 'Ko Zaw (Supervisor)',
    totalReleased: 0,
    transferHistory: [],
    parentPlanComplete: false // This is a component transfer
  },
  {
    jobId: 'JOB20250906-001',
    parentPlanId: 'PLAN20250906-001',
    product: '2012 - Plastic Container 1L',
    producedQty: 3000,
    goodQty: 2950,
    defectQty: 50,
    scrapQty: 0,
    qcStatus: 'passed',
    transferStatus: 'transferred',
    operator: 'Operator D (OP004)',
    machine: 'INJ-M004',
    shift: 'Day Shift',
    completedDate: '2025-09-06 15:00',
    qcOfficer: 'QC Officer 1',
    supervisorApproval: 'Ko Aung (Supervisor)',
    totalReleased: 2950,
    transferHistory: [
      { date: '2025-09-06 16:30', qty: 2950, approver: 'Ko Aung (Supervisor)', timestamp: '2025-09-06 16:30:00' }
    ],
    parentPlanComplete: true
  },
  {
    jobId: 'JOB20250906-002',
    parentPlanId: null,
    product: '2013 - Plastic Cup 250ml',
    producedQty: 2000,
    goodQty: 1950,
    defectQty: 50,
    scrapQty: 0,
    qcStatus: 'failed',
    transferStatus: 'pending',
    operator: 'Operator E (OP005)',
    machine: 'INJ-M005',
    shift: 'Night Shift',
    completedDate: '2025-09-06 22:30',
    qcOfficer: 'QC Officer 2',
    supervisorApproval: null,
    totalReleased: 0,
    transferHistory: [],
    parentPlanComplete: true
  }
];

export function FinishedGoodsTransferNew() {
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState('all');
  const [qcStatusFilter, setQcStatusFilter] = useState('all');
  const [jobs, setJobs] = useState(mockFinishedJobsData);
  const [transferConfirmDialog, setTransferConfirmDialog] = useState(false);
  const [selectedJob, setSelectedJob] = useState<any>(null);
  const [releaseQty, setReleaseQty] = useState('');

  // Calculate summary metrics
  const jobsWaitingForTransfer = jobs.filter(job => 
    job.transferStatus === 'pending' && job.qcStatus === 'passed'
  ).length;

  const jobsTransferredToday = jobs.filter(job => 
    job.transferHistory.some(transfer => transfer.date.startsWith('2025-09-07'))
  ).length;

  const totalGoodQtyReleased = jobs.reduce((sum, job) => sum + job.totalReleased, 0);
  const totalDefectsAndScrap = jobs.reduce((sum, job) => sum + job.defectQty + job.scrapQty, 0);

  // Filter jobs based on search and filters
  const filteredJobs = jobs.filter(job => {
    const matchesSearch = job.jobId.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         job.product.toLowerCase().includes(searchTerm.toLowerCase());
    
    const matchesStatus = statusFilter === 'all' || job.transferStatus === statusFilter;
    const matchesQCStatus = qcStatusFilter === 'all' || job.qcStatus === qcStatusFilter;
    
    return matchesSearch && matchesStatus && matchesQCStatus;
  });

  const getQCStatusBadge = (status: string) => {
    const statusConfig: any = {
      pending: { color: 'bg-yellow-100 text-yellow-800', label: 'Pending | ဆိုင်းငံ့' },
      passed: { color: 'bg-green-100 text-green-800', label: 'Passed | အောင်မြင်' },
      failed: { color: 'bg-red-100 text-red-800', label: 'Failed | မအောင်မြင်' }
    };
    const config = statusConfig[status] || { color: 'bg-slate-100 text-slate-800', label: status };
    return <Badge className={config.color}>{config.label}</Badge>;
  };

  const getTransferStatusBadge = (job: any) => {
    let status = job.transferStatus;
    let statusConfig: any = {
      pending: { color: 'bg-yellow-100 text-yellow-800', label: 'Pending | ဆိုင်းငံ့' },
      transferred: { color: 'bg-green-100 text-green-800', label: 'Transferred | လွှဲပြောင်းပြီး' },
      partial: { color: 'bg-blue-100 text-blue-800', label: 'Partial | တစ်စိတ်တစ်ပိုင်း' }
    };

    // Check if this is a component transfer
    if (!job.parentPlanComplete && job.transferStatus !== 'pending') {
      statusConfig['component'] = { color: 'bg-orange-100 text-orange-800', label: 'Component Transfer | အစိတ်အပိုင်းလွှဲပြောင်း' };
      status = 'component';
    }

    const config = statusConfig[status] || { color: 'bg-slate-100 text-slate-800', label: status };
    return <Badge className={config.color}>{config.label}</Badge>;
  };

  const handleApproveTransfer = (job: any) => {
    setSelectedJob(job);
    setReleaseQty((job.goodQty - job.totalReleased).toString());
    setTransferConfirmDialog(true);
  };

  const handleConfirmTransfer = () => {
    if (!selectedJob || !releaseQty) return;

    const releaseQtyNum = parseInt(releaseQty);
    const availableQty = selectedJob.goodQty - selectedJob.totalReleased;

    if (releaseQtyNum <= 0 || releaseQtyNum > availableQty) {
      alert('Invalid release quantity');
      return;
    }

    const timestamp = new Date().toISOString().replace('T', ' ').substring(0, 19);
    const newTransfer = {
      date: timestamp.substring(0, 10) + ' ' + timestamp.substring(11, 16),
      qty: releaseQtyNum,
      approver: selectedJob.supervisorApproval || 'Current User',
      timestamp: timestamp
    };

    const newTotalReleased = selectedJob.totalReleased + releaseQtyNum;
    const newTransferStatus = newTotalReleased >= selectedJob.goodQty ? 'transferred' : 'partial';

    // Update jobs state
    const updatedJobs = jobs.map(job => {
      if (job.jobId === selectedJob.jobId) {
        return {
          ...job,
          transferStatus: newTransferStatus,
          totalReleased: newTotalReleased,
          transferHistory: [...job.transferHistory, newTransfer]
        };
      }
      return job;
    });

    setJobs(updatedJobs);
    setTransferConfirmDialog(false);
    setSelectedJob(null);
    setReleaseQty('');

    // Log event
    console.log(`${timestamp} – ${releaseQtyNum} approved for FG transfer for Job ${selectedJob.jobId} by ${selectedJob.supervisorApproval || 'Current User'}`);
  };

  const canApproveTransfer = (job: any) => {
    return job.qcStatus === 'passed' && job.goodQty > job.totalReleased;
  };

  const handleExport = (format: 'excel' | 'pdf') => {
    console.log(`Exporting data as ${format}`);
    alert(`Export feature would generate ${format.toUpperCase()} report with fields: Parent Plan ID, Job ID, Product, Good Qty, Released Qty, Status, Approved By, Timestamp`);
  };

  return (
    <div className="p-6 bg-slate-50 min-h-screen">
      <div className="max-w-7xl mx-auto">
        {/* Header */}
        <div className="mb-6">
          <h1 className="text-2xl font-semibold text-slate-900 mb-2">
            Finished Goods Transfer | ထုတ်ကုန် လွှဲပြောင်းခြင်း
          </h1>
          <p className="text-slate-600">
            Approve and transfer finished goods from production to FG warehouse after QC approval
          </p>
          <p className="text-sm text-slate-500 mt-1">
            QC အတည်ပြုပြီးနောက် ထုတ်လုပ်မှုမှ FG ဂိုဒေါင်သို့ ထုတ်ကုန်များ လွှဲပြောင်းအတည်ပြုရန်
          </p>
        </div>

        {/* Summary Cards */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-6">
          <Card className="border-0 shadow-sm">
            <CardContent className="p-6">
              <div className="flex items-center gap-3">
                <div className="p-3 bg-yellow-50 rounded-lg">
                  <Clock className="h-6 w-6 text-yellow-600" />
                </div>
                <div>
                  <div className="text-2xl font-semibold">{jobsWaitingForTransfer}</div>
                  <div className="text-sm text-slate-600">Jobs Waiting for Transfer</div>
                  <div className="text-xs text-slate-500">လွှဲပြောင်းရန်စောင့်ဆိုင်းနေသောအလုပ်များ</div>
                </div>
              </div>
            </CardContent>
          </Card>
          
          <Card className="border-0 shadow-sm">
            <CardContent className="p-6">
              <div className="flex items-center gap-3">
                <div className="p-3 bg-green-50 rounded-lg">
                  <CheckCircle className="h-6 w-6 text-green-600" />
                </div>
                <div>
                  <div className="text-2xl font-semibold">{jobsTransferredToday}</div>
                  <div className="text-sm text-slate-600">Jobs Transferred Today</div>
                  <div className="text-xs text-slate-500">ယနေ့လွှဲပြောင်းပြီးသောအလုပ်များ</div>
                </div>
              </div>
            </CardContent>
          </Card>
          
          <Card className="border-0 shadow-sm">
            <CardContent className="p-6">
              <div className="flex items-center gap-3">
                <div className="p-3 bg-blue-50 rounded-lg">
                  <TrendingUp className="h-6 w-6 text-blue-600" />
                </div>
                <div>
                  <div className="text-2xl font-semibold">{totalGoodQtyReleased.toLocaleString()}</div>
                  <div className="text-sm text-slate-600">Total Good Qty Released</div>
                  <div className="text-xs text-slate-500">စုစုပေါင်းအရည်အသွေးကောင်းသောပမာဏ</div>
                </div>
              </div>
            </CardContent>
          </Card>
          
          <Card className="border-0 shadow-sm">
            <CardContent className="p-6">
              <div className="flex items-center gap-3">
                <div className="p-3 bg-red-50 rounded-lg">
                  <AlertTriangle className="h-6 w-6 text-red-600" />
                </div>
                <div>
                  <div className="text-2xl font-semibold">{totalDefectsAndScrap.toLocaleString()}</div>
                  <div className="text-sm text-slate-600">Total Defects & Scrap</div>
                  <div className="text-xs text-slate-500">စုစုပေါင်းချွတ်ယွင်းမှုနှင့်အပိုင်းအစများ</div>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Filters and Search */}
        <Card className="border-0 shadow-sm mb-6">
          <CardContent className="p-6">
            <div className="flex flex-col md:flex-row gap-4 items-center">
              <div className="flex-1">
                <div className="relative">
                  <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-slate-400" />
                  <Input
                    placeholder="Search by Job ID, Product..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    className="pl-10"
                  />
                </div>
              </div>
              <div className="flex gap-4">
                <Select value={statusFilter} onValueChange={setStatusFilter}>
                  <SelectTrigger className="w-40">
                    <SelectValue placeholder="Transfer Status" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">All Status</SelectItem>
                    <SelectItem value="pending">Pending</SelectItem>
                    <SelectItem value="transferred">Transferred</SelectItem>
                    <SelectItem value="partial">Partial</SelectItem>
                  </SelectContent>
                </Select>
                
                <Select value={qcStatusFilter} onValueChange={setQcStatusFilter}>
                  <SelectTrigger className="w-40">
                    <SelectValue placeholder="QC Status" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">All QC Status</SelectItem>
                    <SelectItem value="pending">QC Pending</SelectItem>
                    <SelectItem value="passed">QC Passed</SelectItem>
                    <SelectItem value="failed">QC Failed</SelectItem>
                  </SelectContent>
                </Select>
                
                <Button variant="outline" onClick={() => handleExport('excel')}>
                  <Download className="h-4 w-4 mr-2" />
                  Excel
                </Button>
                <Button variant="outline" onClick={() => handleExport('pdf')}>
                  <FileText className="h-4 w-4 mr-2" />
                  PDF
                </Button>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Main Table */}
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Package className="h-5 w-5 text-blue-600" />
              <div>
                <div>Finished Jobs for Transfer</div>
                <div className="text-sm text-slate-500">လွှဲပြောင်းရန်ပြီးမြောက်သောအလုပ်များ</div>
              </div>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Job ID</TableHead>
                    <TableHead>Parent Plan ID</TableHead>
                    <TableHead>Product | ထုတ်ကုန်</TableHead>
                    <TableHead>Produced Qty | ထုတ်လုပ်အရေအတွက်</TableHead>
                    <TableHead>Good Qty | အရည်အသွေးကောင်းသော</TableHead>
                    <TableHead>Defect Qty | ချွတ်ယွင်းမှု</TableHead>
                    <TableHead>Scrap Qty | အပိုင်းအစများ</TableHead>
                    <TableHead>QC Status</TableHead>
                    <TableHead>Transfer Status</TableHead>
                    <TableHead>Actions | လုပ်ဆောင်ချက်များ</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {filteredJobs.map((job) => (
                    <React.Fragment key={job.jobId}>
                      <TableRow>
                        <TableCell className="font-medium">{job.jobId}</TableCell>
                        <TableCell>
                          {job.parentPlanId ? (
                            <div className="font-medium">{job.parentPlanId}</div>
                          ) : (
                            <span className="text-slate-400">-</span>
                          )}
                        </TableCell>
                        <TableCell>
                          <div>
                            <div className="font-medium">{job.product}</div>
                            <div className="text-sm text-slate-500 flex items-center gap-1">
                              <User className="h-3 w-3" />
                              {job.operator} | {job.machine}
                            </div>
                          </div>
                        </TableCell>
                        <TableCell>
                          <div className="font-medium">
                            {job.producedQty.toLocaleString()}
                          </div>
                        </TableCell>
                        <TableCell>
                          <div className="font-medium text-green-600">
                            {job.totalReleased > 0 && job.totalReleased < job.goodQty ? (
                              <>
                                {job.totalReleased.toLocaleString()} / {job.goodQty.toLocaleString()}
                              </>
                            ) : (
                              job.goodQty.toLocaleString()
                            )}
                          </div>
                        </TableCell>
                        <TableCell>
                          <div className="font-medium text-orange-600">
                            {job.defectQty.toLocaleString()}
                          </div>
                        </TableCell>
                        <TableCell>
                          <div className="font-medium text-red-600">
                            {job.scrapQty.toLocaleString()}
                          </div>
                        </TableCell>
                        <TableCell>
                          {getQCStatusBadge(job.qcStatus)}
                        </TableCell>
                        <TableCell>
                          {getTransferStatusBadge(job)}
                        </TableCell>
                        <TableCell>
                          <div className="flex gap-2">
                            <Button
                              size="sm"
                              variant="outline"
                              onClick={() => {/* View details */}}
                            >
                              <Eye className="h-4 w-4 mr-1" />
                              View
                            </Button>
                            {canApproveTransfer(job) && (
                              <Button
                                size="sm"
                                onClick={() => handleApproveTransfer(job)}
                                className="bg-green-600 hover:bg-green-700"
                              >
                                <CheckCircle className="h-4 w-4 mr-1" />
                                Approve Transfer
                              </Button>
                            )}
                          </div>
                        </TableCell>
                      </TableRow>
                      {/* Component Transfer Warning */}
                      {!job.parentPlanComplete && job.transferStatus !== 'pending' && (
                        <TableRow>
                          <TableCell colSpan={10}>
                            <div className="bg-orange-50 border border-orange-200 rounded-lg p-3">
                              <div className="flex items-center gap-2 text-orange-800">
                                <AlertTriangle className="h-4 w-4" />
                                <span className="font-medium">
                                  ⚠ Parent Plan incomplete – this is a component transfer.
                                </span>
                              </div>
                              <p className="text-sm text-orange-700 mt-1">
                                Parent Plan {job.parentPlanId} သည် မပြီးမြောက်သေးသောကြောင့် ဤလွှဲပြောင်းမှုသည် အစိတ်အပိုင်းလွှဲပြောင်းမှုဖြစ်သည်။
                              </p>
                            </div>
                          </TableCell>
                        </TableRow>
                      )}
                    </React.Fragment>
                  ))}
                </TableBody>
              </Table>
            </div>
            
            {filteredJobs.length === 0 && (
              <div className="text-center py-8">
                <div className="text-slate-500">No jobs found matching your criteria</div>
                <div className="text-sm text-slate-400">သင့်ရှာဖွေမှုနှင့်ကိုက်ညီသောအလုပ်များမတွေ့ရှိပါ</div>
              </div>
            )}
          </CardContent>
        </Card>

        {/* Transfer Confirmation Dialog */}
        <Dialog open={transferConfirmDialog} onOpenChange={setTransferConfirmDialog}>
          <DialogContent className="max-w-lg">
            <DialogHeader>
              <DialogTitle>Approve Transfer to FG-WH</DialogTitle>
            </DialogHeader>
            <div className="space-y-4">
              {selectedJob && (
                <>
                  {/* Alert Banner */}
                  <div className="bg-blue-50 border border-blue-200 rounded-lg p-3">
                    <div className="flex items-center gap-2 text-blue-800">
                      <AlertCircle className="h-4 w-4" />
                      <span className="font-medium">
                        ⚠ Please check QC results carefully before transfer.
                      </span>
                    </div>
                  </div>

                  {/* Job Details */}
                  <div className="grid grid-cols-2 gap-4 text-sm">
                    <div>
                      <Label>Job ID:</Label>
                      <div className="font-medium">{selectedJob.jobId}</div>
                    </div>
                    <div>
                      <Label>Parent Plan ID:</Label>
                      <div className="font-medium">{selectedJob.parentPlanId || 'N/A'}</div>
                    </div>
                    <div className="col-span-2">
                      <Label>Product:</Label>
                      <div className="font-medium">{selectedJob.product}</div>
                    </div>
                    <div>
                      <Label>Produced Qty:</Label>
                      <div className="font-medium">{selectedJob.producedQty.toLocaleString()}</div>
                    </div>
                    <div>
                      <Label>Good Qty:</Label>
                      <div className="font-medium text-green-600">{selectedJob.goodQty.toLocaleString()}</div>
                    </div>
                    <div>
                      <Label>Defect Qty:</Label>
                      <div className="font-medium text-orange-600">{selectedJob.defectQty.toLocaleString()}</div>
                    </div>
                    <div>
                      <Label>Scrap Qty:</Label>
                      <div className="font-medium text-red-600">{selectedJob.scrapQty.toLocaleString()}</div>
                    </div>
                  </div>

                  {/* Release Quantity */}
                  <div>
                    <Label htmlFor="release-qty">Release Qty (editable):</Label>
                    <Input
                      id="release-qty"
                      type="number"
                      value={releaseQty}
                      onChange={(e) => setReleaseQty(e.target.value)}
                      max={selectedJob.goodQty - selectedJob.totalReleased}
                      className="mt-1 font-medium"
                    />
                    <p className="text-xs text-slate-600 mt-1">
                      Available: {(selectedJob.goodQty - selectedJob.totalReleased).toLocaleString()}
                    </p>
                  </div>

                  {/* Supervisor Approval */}
                  <div>
                    <Label>Supervisor Approval:</Label>
                    <Input 
                      value={selectedJob.supervisorApproval || 'Current User'} 
                      readOnly 
                      className="bg-slate-100" 
                    />
                  </div>

                  <div className="flex gap-2 justify-end pt-4">
                    <Button 
                      variant="outline" 
                      onClick={() => setTransferConfirmDialog(false)}
                    >
                      Cancel
                    </Button>
                    <Button 
                      onClick={handleConfirmTransfer}
                      className="bg-green-600 hover:bg-green-700"
                    >
                      <CheckCircle className="h-4 w-4 mr-2" />
                      Confirm Transfer
                    </Button>
                  </div>
                </>
              )}
            </div>
          </DialogContent>
        </Dialog>
      </div>
    </div>
  );
}