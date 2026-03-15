import React, { useState } from 'react';
import {
  Package,
  CheckCircle,
  AlertTriangle,
  Factory,
  Search,
  Filter,
  Download,
  Eye,
  QrCode,
  Recycle,
  Trash2,
  ChevronDown,
  ChevronRight,
  Printer,
  AlertCircle,
  TreePine,
  GitBranch,
  User,
  Calendar,
  Settings,
  Clock,
  Shield,
  AlertOctagon,
  Info
} from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Badge } from './ui/badge';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from './ui/dialog';
import { Progress } from './ui/progress';
import { Label } from './ui/label';
import { Textarea } from './ui/textarea';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from './ui/tooltip';

// Enhanced mock data with Parent Plan-based structure
const mockFinishedGoodsData = [
  // Parent Plan 1 - Complete (all child jobs finished)
  {
    parentPlanId: 'PLAN20250907-001',
    childJobId: 'JOB20250907-001-C1',
    product: '2011 - Plastic Bottle 500ml',
    requiredQty: 10000,
    producedQty: 10000,
    goodQty: 9850,
    defectQty: 100,
    scrapQty: 50,
    qcStatus: 'passed',
    operator: 'Operator A (OP001)',
    machine: 'INJ-M001',
    shift: 'Day Shift',
    completedDate: '2025-09-07 14:30',
    qcOfficer: 'QC Officer 1'
  },
  {
    parentPlanId: 'PLAN20250907-001',
    childJobId: 'JOB20250907-001-C2',
    product: '2021 - Plastic Cap Red',
    requiredQty: 10000,
    producedQty: 10000,
    goodQty: 9950,
    defectQty: 50,
    scrapQty: 0,
    qcStatus: 'passed',
    operator: 'Operator B (OP002)',
    machine: 'INJ-M002',
    shift: 'Day Shift',
    completedDate: '2025-09-07 16:00',
    qcOfficer: 'QC Officer 1'
  },
  {
    parentPlanId: 'PLAN20250907-001',
    childJobId: 'JOB20250907-001-C3',
    product: '2031 - Assembly Instructions',
    requiredQty: 10000,
    producedQty: 10000,
    goodQty: 10000,
    defectQty: 0,
    scrapQty: 0,
    qcStatus: 'passed',
    operator: 'Operator G (OP007)',
    machine: 'PRINT-001',
    shift: 'Day Shift',
    completedDate: '2025-09-07 15:30',
    qcOfficer: 'QC Officer 2'
  },

  // Parent Plan 2 - Incomplete (1 of 2 child jobs finished)
  {
    parentPlanId: 'PLAN20250907-002',
    childJobId: 'JOB20250907-002-C1',
    product: '2012 - Plastic Container 1L',
    requiredQty: 5000,
    producedQty: 5000,
    goodQty: 4800,
    defectQty: 150,
    scrapQty: 50,
    qcStatus: 'passed',
    operator: 'Operator C (OP003)',
    machine: 'INJ-M003',
    shift: 'Day Shift',
    completedDate: '2025-09-07 14:20',
    qcOfficer: 'QC Officer 2'
  },
  {
    parentPlanId: 'PLAN20250907-002',
    childJobId: 'JOB20250907-002-C2',
    product: '2022 - Container Lid Blue',
    requiredQty: 5000,
    producedQty: 3200,
    goodQty: 0,
    defectQty: 0,
    scrapQty: 0,
    qcStatus: 'pending',
    operator: 'Operator D (OP004)',
    machine: 'INJ-M004',
    shift: 'Day Shift',
    completedDate: null,
    qcOfficer: null
  },

  // Parent Plan 3 - Complete (all child jobs finished)
  {
    parentPlanId: 'PLAN20250906-001',
    childJobId: 'JOB20250906-001-C1',
    product: '2013 - Plastic Cup 250ml',
    requiredQty: 8000,
    producedQty: 8000,
    goodQty: 7850,
    defectQty: 150,
    scrapQty: 0,
    qcStatus: 'passed',
    operator: 'Operator E (OP005)',
    machine: 'INJ-M005',
    shift: 'Night Shift',
    completedDate: '2025-09-06 22:30',
    qcOfficer: 'QC Officer 3'
  },
  {
    parentPlanId: 'PLAN20250906-001',
    childJobId: 'JOB20250906-001-C2',
    product: '2023 - Cup Sleeve',
    requiredQty: 8000,
    producedQty: 8000,
    goodQty: 7950,
    defectQty: 50,
    scrapQty: 0,
    qcStatus: 'passed',
    operator: 'Operator F (OP006)',
    machine: 'INJ-M006',
    shift: 'Night Shift',
    completedDate: '2025-09-06 23:15',
    qcOfficer: 'QC Officer 3'
  },

  // Parent Plan 4 - Incomplete (QC pending for all)
  {
    parentPlanId: 'PLAN20250905-001',
    childJobId: 'JOB20250905-001-C1',
    product: '2014 - Plastic Plate Round',
    requiredQty: 6000,
    producedQty: 6000,
    goodQty: 0,
    defectQty: 0,
    scrapQty: 0,
    qcStatus: 'pending',
    operator: 'Operator H (OP008)',
    machine: 'INJ-M007',
    shift: 'Day Shift',
    completedDate: '2025-09-05 16:00',
    qcOfficer: null
  },
  {
    parentPlanId: 'PLAN20250905-001',
    childJobId: 'JOB20250905-001-C2',
    product: '2024 - Plate Cover Clear',
    requiredQty: 6000,
    producedQty: 6000,
    goodQty: 0,
    defectQty: 0,
    scrapQty: 0,
    qcStatus: 'pending',
    operator: 'Operator I (OP009)',
    machine: 'INJ-M008',
    shift: 'Day Shift',
    completedDate: '2025-09-05 17:30',
    qcOfficer: null
  }
];

// Parent Plan definitions with assembly product info
const parentPlanDefinitions = {
  'PLAN20250907-001': {
    assemblyProduct: 'Assembly Set - Bottle & Cap Complete',
    totalRequired: 10000,
    requiredComponents: 3,
    qrCode: 'QR-PLAN20250907-001'
  },
  'PLAN20250907-002': {
    assemblyProduct: 'Container Set - Large & Lid',
    totalRequired: 5000,
    requiredComponents: 2,
    qrCode: null
  },
  'PLAN20250906-001': {
    assemblyProduct: 'Cup Set - Cup & Sleeve',
    totalRequired: 8000,
    requiredComponents: 2,
    qrCode: 'QR-PLAN20250906-001'
  },
  'PLAN20250905-001': {
    assemblyProduct: 'Plate Set - Plate & Cover',
    totalRequired: 6000,
    requiredComponents: 2,
    qrCode: null
  }
};

interface FinishedGoodsReceivingProps {
  mode?: 'receiving' | 'transfer';
}

export function FinishedGoodsReceiving({ mode = 'receiving' }: FinishedGoodsReceivingProps) {
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState('all');
  const [qcStatusFilter, setQcStatusFilter] = useState('all');
  const [approvalStatusFilter, setApprovalStatusFilter] = useState('all');
  const [releaseTypeFilter, setReleaseTypeFilter] = useState('all'); // New filter
  const [expandedPlans, setExpandedPlans] = useState<Set<string>>(new Set());
  const [viewDetailsDialog, setViewDetailsDialog] = useState(false);
  const [selectedJob, setSelectedJob] = useState<any>(null);
  const [qrGenerateDialog, setQrGenerateDialog] = useState(false);
  
  // Force Release Modal State
  const [forceReleaseDialog, setForceReleaseDialog] = useState(false);
  const [selectedPlanForForceRelease, setSelectedPlanForForceRelease] = useState<any>(null);
  const [forceReleaseForm, setForceReleaseForm] = useState({
    reason: '',
    notes: '',
    approver: 'Ko Aung (Supervisor)' // Auto-filled current supervisor
  });
  
  // User role simulation (in real app this would come from auth context)
  const [userRole, setUserRole] = useState('supervisor'); // or 'operator', 'staff'
  
  // Track forced releases and override history
  const [overrideHistory, setOverrideHistory] = useState([
    {
      id: 'OVERRIDE-001',
      parentPlanId: 'PLAN20250906-001',
      timestamp: '2025-09-06 15:30',
      supervisor: 'Ko Aung',
      reason: 'Urgent Sales Order',
      notes: 'Customer requested early shipment for critical order',
      releasedQty: 8000,
      completedComponents: 2,
      totalComponents: 2
    }
  ]);
  
  // Track partially released status (start with demo data)
  const [partiallyReleasedPlans, setPartiallyReleasedPlans] = useState<Set<string>>(new Set(['PLAN20250905-001']));

  // Group data by parent plan and calculate metrics
  const groupedData = mockFinishedGoodsData.reduce((acc: any, job) => {
    if (!acc[job.parentPlanId]) {
      acc[job.parentPlanId] = [];
    }
    acc[job.parentPlanId].push(job);
    return acc;
  }, {});

  // Calculate parent plan completion status
  const parentPlanStatus = Object.keys(groupedData).map(planId => {
    const jobs = groupedData[planId];
    const definition = parentPlanDefinitions[planId as keyof typeof parentPlanDefinitions];
    const completedJobs = jobs.filter((job: any) => job.qcStatus === 'passed').length;
    const isComplete = completedJobs === definition.requiredComponents;
    
    return {
      planId,
      jobs,
      definition,
      completedJobs,
      requiredComponents: definition.requiredComponents,
      isComplete,
      completionPercentage: Math.round((completedJobs / definition.requiredComponents) * 100)
    };
  });

  // Calculate summary metrics
  const completedJobs = mockFinishedGoodsData.filter(job => 
    job.qcStatus === 'passed' || job.qcStatus === 'failed'
  ).length;
  
  const pendingQC = mockFinishedGoodsData.filter(job => 
    job.qcStatus === 'pending'
  ).length;

  const parentPlansIncomplete = parentPlanStatus.filter(plan => !plan.isComplete).length;
  const parentPlansReadyForFG = parentPlanStatus.filter(plan => plan.isComplete).length;

  // Filter data
  const filteredPlanStatus = parentPlanStatus.filter(plan => {
    const matchesSearch = plan.planId.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         plan.definition.assemblyProduct.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         plan.jobs.some((job: any) => 
                           job.childJobId.toLowerCase().includes(searchTerm.toLowerCase()) ||
                           job.product.toLowerCase().includes(searchTerm.toLowerCase())
                         );
    
    const matchesQCStatus = qcStatusFilter === 'all' || 
                           plan.jobs.some((job: any) => job.qcStatus === qcStatusFilter);
    
    const matchesReleaseType = releaseTypeFilter === 'all' ||
                              (releaseTypeFilter === 'normal' && plan.isComplete && !partiallyReleasedPlans.has(plan.planId)) ||
                              (releaseTypeFilter === 'force' && partiallyReleasedPlans.has(plan.planId));
    
    return matchesSearch && matchesQCStatus && matchesReleaseType;
  });

  const toggleExpanded = (planId: string) => {
    const newExpanded = new Set(expandedPlans);
    if (newExpanded.has(planId)) {
      newExpanded.delete(planId);
    } else {
      newExpanded.add(planId);
    }
    setExpandedPlans(newExpanded);
  };

  const getQCStatusBadge = (status: string) => {
    const statusConfig: any = {
      pending: { variant: 'secondary', color: 'bg-yellow-100 text-yellow-800', label: 'Pending | ဆိုင်းငံ့' },
      passed: { variant: 'default', color: 'bg-green-100 text-green-800', label: 'Passed | အောင်မြင်' },
      failed: { variant: 'destructive', color: 'bg-red-100 text-red-800', label: 'Failed | မအောင်မြင်' }
    };
    const config = statusConfig[status] || { variant: 'outline', color: 'bg-slate-100 text-slate-800', label: status };
    return <Badge className={config.color}>{config.label}</Badge>;
  };

  const getApprovalStatusBadge = (status: string) => {
    const statusConfig: any = {
      pending: { variant: 'secondary', color: 'bg-yellow-100 text-yellow-800', label: 'Pending | ဆိုင်းငံ့' },
      approved: { variant: 'default', color: 'bg-green-100 text-green-800', label: 'Approved | အတည်ပြု' },
      'sent-to-wh': { variant: 'outline', color: 'bg-blue-100 text-blue-800', label: 'Sent to FG-WH | ဂိုဒေါင်ပို့' }
    };
    const config = statusConfig[status] || { variant: 'outline', color: 'bg-slate-100 text-slate-800', label: status };
    return <Badge className={config.color}>{config.label}</Badge>;
  };

  const canReleaseToPG = (plan: any) => {
    return plan.isComplete;
  };

  const handleReleaseToFGWarehouse = (plan: any) => {
    if (canReleaseToPG(plan)) {
      console.log(`Releasing parent plan ${plan.planId} to FG Warehouse`);
      // Would generate QR code and update status
    }
  };

  const handleSendToDefectHandling = (job: any) => {
    console.log(`Sending ${job.defectQty} defective items from job ${job.childJobId} to defect handling`);
  };

  const handleLogScrap = (job: any) => {
    console.log(`Logging ${job.scrapQty} items from job ${job.childJobId} as scrap`);
  };

  const handleForceRelease = () => {
    if (!selectedPlanForForceRelease || !forceReleaseForm.reason) return;
    
    const timestamp = new Date().toLocaleString('en-GB', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
      hour12: false
    }).replace(',', '');
    
    // Add to override history
    const newOverride = {
      id: `OVERRIDE-${Date.now()}`,
      parentPlanId: selectedPlanForForceRelease.planId,
      timestamp: timestamp,
      supervisor: forceReleaseForm.approver,
      reason: forceReleaseForm.reason,
      notes: forceReleaseForm.notes,
      releasedQty: selectedPlanForForceRelease.jobs.reduce((sum: number, job: any) => sum + job.goodQty, 0),
      completedComponents: selectedPlanForForceRelease.completedJobs,
      totalComponents: selectedPlanForForceRelease.requiredComponents
    };
    
    setOverrideHistory(prev => [newOverride, ...prev]);
    setPartiallyReleasedPlans(prev => new Set([...prev, selectedPlanForForceRelease.planId]));
    
    // Reset form and close dialog
    setForceReleaseForm({
      reason: '',
      notes: '',
      approver: 'Ko Aung (Supervisor)'
    });
    setForceReleaseDialog(false);
    setSelectedPlanForForceRelease(null);
    
    console.log(`Force released plan ${selectedPlanForForceRelease.planId}`);
  };

  return (
    <TooltipProvider>
      <div className="p-6 bg-slate-50 min-h-screen">
        <div className="max-w-7xl mx-auto">
          {/* Header */}
          <div className="mb-6">
            <div className="flex items-center gap-2 mb-2">
              <h1 className="text-2xl font-semibold text-slate-900">
                {mode === 'transfer' 
                  ? 'Finished Goods Transfer | ထုတ်ကုန် လွှဲပြောင်းခြင်း'
                  : 'Finished Goods Receiving | ထုတ်ကုန်လက်ခံခြင်း'
                }
              </h1>
              {mode === 'transfer' && (
                <Tooltip>
                  <TooltipTrigger asChild>
                    <Info className="h-5 w-5 text-blue-600 cursor-help" />
                  </TooltipTrigger>
                  <TooltipContent className="max-w-sm">
                    <p className="text-sm">
                      This screen is used to <strong>transfer finished batches</strong> from Production Control to Finished Goods Warehouse. Not the HQ receiving step.
                    </p>
                  </TooltipContent>
                </Tooltip>
              )}
            </div>
            <p className="text-slate-600">
              {mode === 'transfer'
                ? 'Transfer finished batches after QC, validate parent plan completeness, and transfer only when all required components are done.'
                : 'Approve finished batches after QC, validate parent plan completeness, and release only when all required components are done.'
              }
            </p>
            <p className="text-sm text-slate-500 mt-1">
              {mode === 'transfer'
                ? 'QC ပြီးနောက် ပြီးသားအုပ်စုများကို လွှဲပြောင်းပြီး parent plan ပြီးမြောက်မှုကို အတည်ပြုပါ၊ လိုအပ်သောအစိတ်အပိုင်းများ အားလုံးပြီးမှသာ လွှဲပြောင်းပါ။'
                : 'QC ပြီးနောက် ပြီးသားအုပ်စုများကို အတည်ပြုပြီး parent plan ပြီးမြောက်မှုကို အတည်ပြုပါ၊ လိုအပ်သောအစိတ်အပိုင်းများ အားလုံးပြီးမှသာ ထုတ်ပါ။'
              }
            </p>
          </div>

        {/* Warning Banner for Incomplete Parent Plans */}
        {parentPlansIncomplete > 0 && (
          <div className="mb-6 p-4 bg-red-50 border border-red-200 rounded-lg">
            <div className="flex items-center gap-2">
              <AlertTriangle className="h-5 w-5 text-red-600" />
              <p className="text-red-800 font-medium">
                ⚠ {parentPlansIncomplete} parent plan(s) incomplete – cannot {mode === 'transfer' ? 'transfer' : 'release'} until all child jobs finish. Supervisor may override.
              </p>
            </div>
            <p className="text-sm text-red-700 mt-1">
              {parentPlansIncomplete} parent plan(s) မပြီးသေး - child job များ အားလုံးမပြီးမချင်း မ{mode === 'transfer' ? 'လွှဲပြောင်း' : 'ထုတ်'}နိုင်ပါ။ ကြီးကြပ်ရေးမှူးမှ လွှမ်းမိုးနိုင်သည်။
            </p>
            {userRole === 'supervisor' && (
              <div className="flex items-center gap-2 mt-2">
                <Shield className="h-4 w-4 text-blue-600" />
                <p className="text-sm text-blue-700 font-medium">
                  You have supervisor privileges to force {mode === 'transfer' ? 'transfer' : 'release'} incomplete parent plans if needed.
                </p>
              </div>
            )}
          </div>
        )}

        {/* Summary Cards */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
          <Card>
            <CardContent className="pt-6">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm font-medium text-slate-600">Completed Jobs</p>
                  <p className="text-sm text-slate-500">ပြီးသားအလုပ်များ</p>
                  <p className="text-2xl font-bold text-green-600">{completedJobs}</p>
                </div>
                <Factory className="h-8 w-8 text-green-600" />
              </div>
            </CardContent>
          </Card>

          <Card>
            <CardContent className="pt-6">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm font-medium text-slate-600">Pending QC</p>
                  <p className="text-sm text-slate-500">QC ဆိုင်းငံ့</p>
                  <p className="text-2xl font-bold text-yellow-600">{pendingQC}</p>
                </div>
                <AlertTriangle className="h-8 w-8 text-yellow-600" />
              </div>
            </CardContent>
          </Card>

          <Card>
            <CardContent className="pt-6">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm font-medium text-slate-600">Parent Plans Incomplete</p>
                  <p className="text-sm text-slate-500">မပြီးသေးသော Parent Plan များ</p>
                  <p className="text-2xl font-bold text-red-600">{parentPlansIncomplete}</p>
                </div>
                <AlertCircle className="h-8 w-8 text-red-600" />
              </div>
            </CardContent>
          </Card>

          <Card>
            <CardContent className="pt-6">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm font-medium text-slate-600">
                    Parent Plans Ready for FG {mode === 'transfer' ? 'Transfer' : 'Release'}
                  </p>
                  <p className="text-sm text-slate-500">
                    FG {mode === 'transfer' ? 'လွှဲပြောင်း' : 'ထုတ်'}ရန်အဆင်သင့် Parent Plan များ
                  </p>
                  <p className="text-2xl font-bold text-green-600">{parentPlansReadyForFG}</p>
                </div>
                <CheckCircle className="h-8 w-8 text-green-600" />
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Filters and Controls */}
        <Card className="mb-6">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Filter className="h-5 w-5" />
              Filters & Search | စစ်ထုတ်မှုနှင့် ရှာဖွေမှု
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
              <div>
                <Label htmlFor="search">Search Plans & Jobs</Label>
                <div className="relative">
                  <Search className="absolute left-3 top-3 h-4 w-4 text-slate-400" />
                  <Input
                    id="search"
                    placeholder="Parent Plan ID, Child Job ID, Product..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    className="pl-10"
                  />
                </div>
              </div>

              <div>
                <Label htmlFor="qc-status">QC Status</Label>
                <Select value={qcStatusFilter} onValueChange={setQcStatusFilter}>
                  <SelectTrigger>
                    <SelectValue placeholder="Select QC Status" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">All QC Status</SelectItem>
                    <SelectItem value="pending">Pending</SelectItem>
                    <SelectItem value="passed">Passed</SelectItem>
                    <SelectItem value="failed">Failed</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div>
                <Label htmlFor="release-type">Release Type Filter</Label>
                <Select value={releaseTypeFilter} onValueChange={setReleaseTypeFilter}>
                  <SelectTrigger>
                    <SelectValue placeholder="Show" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">All {mode === 'transfer' ? 'Transfers' : 'Releases'}</SelectItem>
                    <SelectItem value="normal">Normal {mode === 'transfer' ? 'Transfer' : 'Release'}</SelectItem>
                    <SelectItem value="force">Force {mode === 'transfer' ? 'Transfer' : 'Release'}</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div className="flex items-end gap-2">
                <Button 
                  variant="outline" 
                  className="flex-1"
                  title={`Export includes override reason and supervisor name for forced ${mode === 'transfer' ? 'transfers' : 'releases'}`}
                >
                  <Download className="h-4 w-4 mr-2" />
                  Export Finished Goods {mode === 'transfer' ? 'Transfer' : 'Receiving'} Report (Excel)
                </Button>
                <Button 
                  variant="outline"
                  title={`Export includes override reason and supervisor name for forced ${mode === 'transfer' ? 'transfers' : 'releases'}`}
                >
                  <Download className="h-4 w-4 mr-2" />
                  Export Finished Goods {mode === 'transfer' ? 'Transfer' : 'Receiving'} Report (PDF)
                </Button>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Main Table */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Package className="h-5 w-5" />
              Finished Goods Jobs | ထုတ်ကုန် အလုပ်များ
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead className="w-12"></TableHead>
                    <TableHead>Parent Plan ID</TableHead>
                    <TableHead>Child Job ID</TableHead>
                    <TableHead>Product (Child Item)</TableHead>
                    <TableHead>Required Qty</TableHead>
                    <TableHead>Produced Qty</TableHead>
                    <TableHead>Good Qty</TableHead>
                    <TableHead>Defect Qty</TableHead>
                    <TableHead>Scrap Qty</TableHead>
                    <TableHead>QC Status</TableHead>
                    <TableHead>Completion %</TableHead>
                    <TableHead>Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {filteredPlanStatus.map((plan: any) => (
                    <React.Fragment key={plan.planId}>
                      {/* Parent Plan Row */}
                      <TableRow 
                        className={
                          !plan.isComplete 
                            ? 'bg-red-50 border-red-200' 
                            : 'bg-green-50 border-green-200'
                        }
                      >
                        <TableCell>
                          <div className="flex items-center gap-2">
                            <Button
                              variant="ghost"
                              size="sm"
                              onClick={() => toggleExpanded(plan.planId)}
                            >
                              {expandedPlans.has(plan.planId) ? (
                                <ChevronDown className="h-4 w-4" />
                              ) : (
                                <ChevronRight className="h-4 w-4" />
                              )}
                            </Button>
                            <TreePine className="h-4 w-4 text-blue-600" />
                          </div>
                        </TableCell>
                        <TableCell className="font-bold">
                          {plan.planId}
                          <div className="text-sm font-normal text-slate-600 mt-1">
                            {plan.definition.assemblyProduct}
                          </div>
                          <div className="flex items-center gap-2 mt-2">
                            <Progress value={plan.completionPercentage} className="w-20 h-2" />
                            <span className="text-xs text-slate-500">
                              {plan.completedJobs}/{plan.requiredComponents} complete
                            </span>
                          </div>
                        </TableCell>
                        <TableCell className="text-slate-500">-</TableCell>
                        <TableCell className="font-medium">
                          Assembly Set ({plan.requiredComponents} components)
                          {!plan.isComplete && (
                            <div className="text-xs text-red-600 font-medium mt-1">
                              ⚠ Parent Plan Incomplete
                            </div>
                          )}
                          {plan.isComplete && (
                            <div className="text-xs text-green-600 font-medium mt-1">
                              ✓ Parent Plan Complete
                            </div>
                          )}
                        </TableCell>
                        <TableCell>{plan.definition.totalRequired.toLocaleString()}</TableCell>
                        <TableCell>
                          {plan.jobs.reduce((sum: number, job: any) => sum + job.producedQty, 0).toLocaleString()}
                        </TableCell>
                        <TableCell className="text-green-600 font-medium">
                          {plan.jobs.reduce((sum: number, job: any) => sum + job.goodQty, 0).toLocaleString()}
                        </TableCell>
                        <TableCell className="text-yellow-600">
                          {plan.jobs.reduce((sum: number, job: any) => sum + job.defectQty, 0).toLocaleString()}
                        </TableCell>
                        <TableCell className="text-red-600 font-medium">
                          {plan.jobs.reduce((sum: number, job: any) => sum + job.scrapQty, 0).toLocaleString()}
                        </TableCell>
                        <TableCell>
                          {partiallyReleasedPlans.has(plan.planId) ? (
                            <Badge className="bg-yellow-100 text-yellow-800">Partially Released | တစ်စိတ်တစ်ပိုင်းထုတ်ပြီး</Badge>
                          ) : plan.isComplete ? (
                            <Badge className="bg-green-100 text-green-800">All Passed | အားလုံးအောင်မြင်</Badge>
                          ) : (
                            <Badge className="bg-yellow-100 text-yellow-800">In Progress | ဆောင်ရွက်နေ</Badge>
                          )}
                        </TableCell>
                        <TableCell>
                          <div className="flex items-center gap-2">
                            <span className={`font-medium ${plan.isComplete ? 'text-green-600' : 'text-red-600'}`}>
                              {plan.completionPercentage}%
                            </span>
                          </div>
                        </TableCell>
                        <TableCell>
                          <div className="flex gap-1 flex-wrap">
                            {plan.isComplete ? (
                              <Button
                                size="sm"
                                onClick={() => handleReleaseToFGWarehouse(plan)}
                                className="bg-green-600 hover:bg-green-700"
                                title={`Parent Plan complete. Ready for FG ${mode === 'transfer' ? 'transfer' : 'release'}.`}
                              >
                                <QrCode className="h-3 w-3 mr-1" />
                                {mode === 'transfer' ? 'Transfer to FG-WH' : 'Release to FG-WH'}
                              </Button>
                            ) : partiallyReleasedPlans.has(plan.planId) ? (
                              <div className="flex gap-1">
                                <Badge className="bg-yellow-100 text-yellow-800">
                                  Force {mode === 'transfer' ? 'Transferred' : 'Released'}
                                </Badge>
                              </div>
                            ) : (
                              <div className="flex gap-1">
                                <Button
                                  size="sm"
                                  variant="outline"
                                  disabled
                                  title="Parent Plan incomplete. Waiting for remaining child jobs."
                                  className="opacity-50"
                                >
                                  <QrCode className="h-3 w-3 mr-1" />
                                  {mode === 'transfer' ? 'Transfer to FG-WH' : 'Release to FG-WH'}
                                </Button>
                                {userRole === 'supervisor' && (
                                  <Button
                                    size="sm"
                                    variant="outline"
                                    className="bg-red-50 hover:bg-red-100 text-red-600 border-red-200"
                                    onClick={() => {
                                      setSelectedPlanForForceRelease(plan);
                                      setForceReleaseDialog(true);
                                    }}
                                    title={`Supervisor only: Force ${mode === 'transfer' ? 'transfer' : 'release'} incomplete parent plan`}
                                  >
                                    <Shield className="h-3 w-3 mr-1" />
                                    Force {mode === 'transfer' ? 'Transfer' : 'Release'}
                                  </Button>
                                )}
                              </div>
                            )}
                          </div>
                        </TableCell>
                      </TableRow>

                      {/* Child Jobs */}
                      {expandedPlans.has(plan.planId) && plan.jobs.map((job: any) => (
                        <TableRow key={job.childJobId} className="bg-slate-50">
                          <TableCell className="pl-8">
                            <GitBranch className="h-4 w-4 text-slate-400" />
                          </TableCell>
                          <TableCell className="text-slate-600">{plan.planId}</TableCell>
                          <TableCell className="text-slate-600 font-medium">{job.childJobId}</TableCell>
                          <TableCell className="text-slate-600">{job.product}</TableCell>
                          <TableCell>{job.requiredQty.toLocaleString()}</TableCell>
                          <TableCell>{job.producedQty.toLocaleString()}</TableCell>
                          <TableCell className="text-green-600">{job.goodQty.toLocaleString()}</TableCell>
                          <TableCell className="text-yellow-600">{job.defectQty.toLocaleString()}</TableCell>
                          <TableCell className="text-red-600">{job.scrapQty.toLocaleString()}</TableCell>
                          <TableCell>{getQCStatusBadge(job.qcStatus)}</TableCell>
                          <TableCell>
                            {job.qcStatus === 'passed' ? (
                              <Badge className="bg-green-100 text-green-800">100%</Badge>
                            ) : job.qcStatus === 'pending' ? (
                              <Badge className="bg-yellow-100 text-yellow-800">
                                {Math.round((job.producedQty / job.requiredQty) * 100)}%
                              </Badge>
                            ) : (
                              <Badge className="bg-red-100 text-red-800">0%</Badge>
                            )}
                          </TableCell>
                          <TableCell>
                            <div className="flex gap-1 flex-wrap">
                              {job.defectQty > 0 && (
                                <Button
                                  size="sm"
                                  variant="outline"
                                  onClick={() => handleSendToDefectHandling(job)}
                                  className="text-yellow-600 hover:bg-yellow-50"
                                >
                                  <Recycle className="h-3 w-3 mr-1" />
                                  Defect
                                </Button>
                              )}
                              {job.scrapQty > 0 && (
                                <Button
                                  size="sm"
                                  variant="outline"
                                  onClick={() => handleLogScrap(job)}
                                  className="text-red-600 hover:bg-red-50"
                                >
                                  <Trash2 className="h-3 w-3 mr-1" />
                                  Log Scrap
                                </Button>
                              )}
                              <Button
                                size="sm"
                                variant="ghost"
                                onClick={() => {
                                  setSelectedJob(job);
                                  setViewDetailsDialog(true);
                                }}
                              >
                                <Eye className="h-3 w-3" />
                              </Button>
                            </div>
                          </TableCell>
                        </TableRow>
                      ))}
                    </React.Fragment>
                  ))}
                </TableBody>
              </Table>
            </div>
          </CardContent>
        </Card>

        {/* View Details Dialog */}
        <Dialog open={viewDetailsDialog} onOpenChange={setViewDetailsDialog}>
          <DialogContent className="max-w-2xl">
            <DialogHeader>
              <DialogTitle>Job Details | အလုပ်အသေးစိတ်</DialogTitle>
            </DialogHeader>
            {selectedJob && (
              <div className="space-y-4">
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <Label>Child Job ID</Label>
                    <p className="font-medium">{selectedJob.childJobId}</p>
                  </div>
                  <div>
                    <Label>Parent Plan ID</Label>
                    <p className="font-medium">{selectedJob.parentPlanId}</p>
                  </div>
                  <div>
                    <Label>Product</Label>
                    <p className="font-medium">{selectedJob.product}</p>
                  </div>
                  <div>
                    <Label>Machine</Label>
                    <p className="font-medium">{selectedJob.machine}</p>
                  </div>
                  <div>
                    <Label>Operator</Label>
                    <p className="font-medium flex items-center gap-2">
                      <User className="h-4 w-4" />
                      {selectedJob.operator}
                    </p>
                  </div>
                  <div>
                    <Label>Shift</Label>
                    <p className="font-medium flex items-center gap-2">
                      <Clock className="h-4 w-4" />
                      {selectedJob.shift}
                    </p>
                  </div>
                  <div>
                    <Label>Completed Date</Label>
                    <p className="font-medium flex items-center gap-2">
                      <Calendar className="h-4 w-4" />
                      {selectedJob.completedDate || 'Not completed'}
                    </p>
                  </div>
                  <div>
                    <Label>QC Officer</Label>
                    <p className="font-medium">{selectedJob.qcOfficer || 'Not assigned'}</p>
                  </div>
                </div>
                
                <div className="grid grid-cols-4 gap-4 pt-4 border-t">
                  <div className="text-center">
                    <Label>Required Qty</Label>
                    <p className="text-lg font-bold">{selectedJob.requiredQty?.toLocaleString() || selectedJob.producedQty?.toLocaleString()}</p>
                  </div>
                  <div className="text-center">
                    <Label>Produced Qty</Label>
                    <p className="text-lg font-bold">{selectedJob.producedQty?.toLocaleString()}</p>
                  </div>
                  <div className="text-center">
                    <Label>Good Qty</Label>
                    <p className="text-lg font-bold text-green-600">{selectedJob.goodQty?.toLocaleString()}</p>
                  </div>
                  <div className="text-center">
                    <Label>Defect Qty</Label>
                    <p className="text-lg font-bold text-yellow-600">{selectedJob.defectQty?.toLocaleString()}</p>
                  </div>
                </div>
              </div>
            )}
          </DialogContent>
        </Dialog>

        {/* Force Release Confirmation Modal */}
        <Dialog open={forceReleaseDialog} onOpenChange={setForceReleaseDialog}>
          <DialogContent className="max-w-2xl">
            <DialogHeader>
              <DialogTitle className="flex items-center gap-2">
                <AlertOctagon className="h-5 w-5 text-red-600" />
                Early {mode === 'transfer' ? 'Transfer' : 'Release'} Confirmation | အစောပိုင်း{mode === 'transfer' ? 'လွှဲပြောင်း' : 'ထုတ်ပေး'}မှုအတည်ပြုချက်
              </DialogTitle>
            </DialogHeader>
            {selectedPlanForForceRelease && (
              <div className="space-y-6">
                {/* Warning Banner */}
                <div className="p-4 bg-red-50 border border-red-200 rounded-lg">
                  <div className="flex items-center gap-2 mb-2">
                    <AlertTriangle className="h-5 w-5 text-red-600" />
                    <p className="font-medium text-red-800">
                      ⚠ Warning: You are about to force {mode === 'transfer' ? 'transfer' : 'release'} an incomplete parent plan
                    </p>
                  </div>
                  <p className="text-sm text-red-700">
                    This action will override the normal completion requirement and should only be used in exceptional circumstances.
                  </p>
                </div>

                {/* Plan Details */}
                <div className="grid grid-cols-2 gap-4 p-4 bg-slate-50 rounded-lg">
                  <div>
                    <Label>Parent Plan ID</Label>
                    <p className="font-medium">{selectedPlanForForceRelease.planId}</p>
                  </div>
                  <div>
                    <Label>Assembly Product</Label>
                    <p className="font-medium">{selectedPlanForForceRelease.definition.assemblyProduct}</p>
                  </div>
                  <div>
                    <Label>Child Jobs Completed</Label>
                    <p className="font-medium">
                      <span className={selectedPlanForForceRelease.completedJobs === selectedPlanForForceRelease.requiredComponents ? 'text-green-600' : 'text-red-600'}>
                        {selectedPlanForForceRelease.completedJobs}/{selectedPlanForForceRelease.requiredComponents}
                      </span>
                    </p>
                  </div>
                  <div>
                    <Label>Completion Progress</Label>
                    <div className="flex items-center gap-2">
                      <Progress value={selectedPlanForForceRelease.completionPercentage} className="flex-1 h-3" />
                      <span className="text-sm font-medium">{selectedPlanForForceRelease.completionPercentage}%</span>
                    </div>
                  </div>
                </div>

                {/* Form Fields */}
                <div className="space-y-4">
                  <div>
                    <Label htmlFor="reason">Select Reason (Required)</Label>
                    <Select 
                      value={forceReleaseForm.reason} 
                      onValueChange={(value) => setForceReleaseForm(prev => ({ ...prev, reason: value }))}
                    >
                      <SelectTrigger>
                        <SelectValue placeholder={`Choose reason for early ${mode === 'transfer' ? 'transfer' : 'release'}`} />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="urgent-sales-order">Urgent Sales Order</SelectItem>
                        <SelectItem value="customer-request">Customer Request</SelectItem>
                        <SelectItem value="space-issue">Space Issue</SelectItem>
                        <SelectItem value="other">Other</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>

                  <div>
                    <Label htmlFor="notes">Notes (Optional)</Label>
                    <Textarea
                      id="notes"
                      placeholder={`Additional details about this force ${mode === 'transfer' ? 'transfer' : 'release'}...`}
                      value={forceReleaseForm.notes}
                      onChange={(e) => setForceReleaseForm(prev => ({ ...prev, notes: e.target.value }))}
                      rows={3}
                    />
                  </div>

                  <div>
                    <Label htmlFor="approver">Approver</Label>
                    <Input
                      id="approver"
                      value={forceReleaseForm.approver}
                      readOnly
                      className="bg-slate-50"
                    />
                    <p className="text-xs text-slate-500 mt-1">Auto-filled current supervisor user</p>
                  </div>
                </div>

                {/* Action Buttons */}
                <div className="flex gap-3 pt-4 border-t">
                  <Button
                    variant="outline"
                    onClick={() => setForceReleaseDialog(false)}
                    className="flex-1"
                  >
                    Cancel | ပယ်ဖျက်
                  </Button>
                  <Button
                    onClick={handleForceRelease}
                    disabled={!forceReleaseForm.reason}
                    className="flex-1 bg-red-600 hover:bg-red-700"
                  >
                    <Shield className="h-4 w-4 mr-2" />
                    Confirm Force {mode === 'transfer' ? 'Transfer' : 'Release'} | အတင်းအကြပ်{mode === 'transfer' ? 'လွှဲပြောင်း' : 'ထုတ်ပေး'}မှုအတည်ပြု
                  </Button>
                </div>

                {/* Override History */}
                {overrideHistory.length > 0 && (
                  <div className="mt-6 p-4 bg-slate-50 rounded-lg">
                    <h4 className="font-medium mb-3 flex items-center gap-2">
                      <Clock className="h-4 w-4" />
                      Recent Override History | မကြာသေးမီ လွှမ်းမိုးမှုမှတ်တမ်း
                    </h4>
                    <div className="space-y-2 max-h-32 overflow-y-auto">
                      {overrideHistory.slice(0, 3).map((override) => (
                        <div key={override.id} className="text-sm p-2 bg-white rounded border">
                          <p className="font-medium">
                            {override.timestamp} – Supervisor {override.supervisor} early released {override.parentPlanId}
                          </p>
                          <p className="text-slate-600">
                            Reason: {override.reason} | Components: {override.completedComponents}/{override.totalComponents}
                          </p>
                        </div>
                      ))}
                    </div>
                  </div>
                )}
              </div>
            )}
          </DialogContent>
        </Dialog>
      </div>
    </div>
    </TooltipProvider>
  );
}