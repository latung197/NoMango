import React, { useState } from 'react';
import {
  BarChart3,
  FileText,
  Package,
  Settings,
  CheckCircle,
  AlertTriangle,
  XCircle,
  Clock,
  Camera,
  Upload,
  Search,
  Filter,
  Download,
  Eye,
  Edit,
  Trash2,
  Plus,
  RefreshCw,
  Calendar,
  User,
  Beaker,
  FileCheck,
  ClipboardCheck,
  AlertCircle,
  Truck
} from 'lucide-react';
import { ReasonDropdown } from './ui/reason-dropdown';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Badge } from './ui/badge';
import { Progress } from './ui/progress';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Label } from './ui/label';
import { Textarea } from './ui/textarea';
import { Checkbox } from './ui/checkbox';
import { Tabs, TabsContent, TabsList, TabsTrigger } from './ui/tabs';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogTrigger } from './ui/dialog';
import { Alert, AlertDescription } from './ui/alert';

interface QualityControlProps {
  currentPage: string;
  onPageChange: (page: string) => void;
}

// Types for QC system
interface RawMaterialQC {
  id: string;
  planId: string;
  jobId: string;
  productName: string;
  materialCode: string;
  materialName: string;
  issuedQty: number;
  issuedBy: string;
  issuedDate: string;
  status: 'pending-qc' | 'pass' | 'hold' | 'reject';
  qcInspector?: string;
  qcDate?: string;
  visualChecks: {
    contamination: boolean | null;
    moisture: boolean | null;
    packaging: boolean | null;
    label: boolean | null;
  };
  testParameters: TestParameter[];
  qcDecision?: 'pass' | 'hold' | 'reject';
  qcReason?: string;
  attachments?: string[];
}

interface FinishedGoodsQC {
  id: string;
  jobId: string;
  batchQty: number;
  machine: string;
  operator: string;
  shift: string;
  productionDate: string;
  status: 'pending-qc' | 'pass' | 'rework' | 'reject';
  samplingSize: number;
  visualDefects: DefectCount[];
  measurements: Measurement[];
  goodQty: number;
  defectQty: number;
  scrapQty: number;
  qcDecision?: 'accept' | 'rework' | 'reject';
  qcInspector?: string;
  qcDate?: string;
}

interface TestParameter {
  id: string;
  parameter: string;
  spec: string;
  measured: string;
  result: 'pass' | 'fail' | null;
}

interface DefectCount {
  defectType: string;
  count: number;
}

interface Measurement {
  parameter: string;
  spec: string;
  measured: string;
  result: 'pass' | 'fail';
}

// New interfaces for Finished Goods QC
interface QCTabConfig {
  id: string;
  name: string;
  nameMM: string;
  color: string;
  bgColor: string;
  borderColor: string;
  textColor: string;
  isActive: boolean;
}

interface QCRecord {
  id: string;
  productBatchId: string;
  reason: string;
  quantityChecked: number;
  defectiveQty: number;
  inspectorName: string;
  attachments: string[];
  status: 'Pass' | 'Fail' | 'Recheck';
  createdAt: string;
  category: string;
}

export function QualityControl({ currentPage, onPageChange }: QualityControlProps) {
  const [selectedRMQC, setSelectedRMQC] = useState<RawMaterialQC | null>(null);
  const [selectedFGQC, setSelectedFGQC] = useState<FinishedGoodsQC | null>(null);
  const [showRMQCDialog, setShowRMQCDialog] = useState(false);
  const [showFGQCDialog, setShowFGQCDialog] = useState(false);

  // In-Process QC states
  const [dateFilter, setDateFilter] = useState('today');
  const [machineFilter, setMachineFilter] = useState('all');
  const [jobIdSearch, setJobIdSearch] = useState('');
  const [viewMode, setViewMode] = useState<'cards' | 'table'>('cards');

  // Mock data - Raw Material QC Queue
  const [rmQCQueue, setRmQCQueue] = useState<RawMaterialQC[]>([
    {
      id: 'RMQC-001',
      planId: 'PLAN-2024-001',
      jobId: 'JOB-2024-001-C1',
      productName: 'Plastic Bottle 500ml',
      materialCode: 'PE-001',
      materialName: 'Polyethylene Resin Grade A',
      issuedQty: 500,
      issuedBy: 'Inventory Staff 1',
      issuedDate: '2024-01-15 08:30',
      status: 'pending-qc',
      visualChecks: {
        contamination: null,
        moisture: null,
        packaging: null,
        label: null
      },
      testParameters: [
        { id: '1', parameter: 'Melt Flow Index', spec: '0.3-0.7 g/10min', measured: '', result: null },
        { id: '2', parameter: 'Density', spec: '0.915-0.925 g/cm³', measured: '', result: null },
        { id: '3', parameter: 'Moisture Content', spec: '<0.05%', measured: '', result: null }
      ]
    },
    {
      id: 'RMQC-002',
      planId: 'PLAN-2024-002',
      jobId: 'JOB-2024-002-C1',
      productName: 'Plastic Cap Red',
      materialCode: 'PP-002',
      materialName: 'Polypropylene Red Masterbatch',
      issuedQty: 200,
      issuedBy: 'Inventory Staff 2',
      issuedDate: '2024-01-15 09:15',
      status: 'pending-qc',
      visualChecks: {
        contamination: null,
        moisture: null,
        packaging: null,
        label: null
      },
      testParameters: [
        { id: '1', parameter: 'Color Strength', spec: '95-105%', measured: '', result: null },
        { id: '2', parameter: 'Dispersion', spec: 'Grade 3 max', measured: '', result: null }
      ]
    }
  ]);

  // Mock data - Finished Goods QC Queue
  const [fgQCQueue, setFgQCQueue] = useState<FinishedGoodsQC[]>([
    {
      id: 'FGQC-001',
      jobId: 'JOB-2024-001-C1',
      batchQty: 10000,
      machine: 'INJ-001',
      operator: 'OP-001',
      shift: 'A',
      productionDate: '2024-01-15 14:30',
      status: 'pending-qc',
      samplingSize: 50,
      visualDefects: [
        { defectType: 'Flash', count: 0 },
        { defectType: 'Burn', count: 0 },
        { defectType: 'Short Shot', count: 0 },
        { defectType: 'Sink Mark', count: 0 }
      ],
      measurements: [
        { parameter: 'Weight', spec: '25±2g', measured: '', result: 'pass' },
        { parameter: 'Height', spec: '180±3mm', measured: '', result: 'pass' },
        { parameter: 'Diameter', spec: '65±1mm', measured: '', result: 'pass' }
      ],
      goodQty: 0,
      wasteQty: 0,
      rejectQty: 0
    }
  ]);

  // Dashboard statistics
  const dashboardStats = {
    todayRMQC: {
      issued: rmQCQueue.length + 8, // Total issued lots today
      approved: rmQCQueue.filter(item => item.status === 'pass').length + 6,
      pending: rmQCQueue.filter(item => item.status === 'pending-qc').length,
      rejected: rmQCQueue.filter(item => item.status === 'reject').length + 1
    },
    todayFGQC: {
      totalQty: 15000,
      goodQty: 14200,
      defectQty: 600,
      scrapQty: 200,
      wasteQty: 2200,
      rejectQty: 800,
      passRate: 94.7
    },
    todayInProcess: {
      checks: 24,
      passed: 22,
      failed: 2
    },
    blockedJobs: [
      { jobId: 'JOB-2024-001-C1', reason: 'Raw Material QC Pending', since: '08:30' },
      { jobId: 'JOB-2024-002-C1', reason: 'Raw Material QC Pending', since: '09:15' }
    ]
  };

  // Status color configurations
  const getQCStatusConfig = (status: string) => {
    switch (status) {
      case 'pass': return { color: 'bg-green-100 text-green-800', icon: '🟢', text: 'Pass', textMM: 'အောင်မြင်' };
      case 'hold': return { color: 'bg-yellow-100 text-yellow-800', icon: '🟡', text: 'Hold', textMM: 'ရပ်နားထား' };
      case 'reject': return { color: 'bg-red-100 text-red-800', icon: '🔴', text: 'Reject', textMM: 'ငြင်းပယ်' };
      case 'pending-qc': return { color: 'bg-blue-100 text-blue-800', icon: '⏳', text: 'Pending QC', textMM: 'QC စောင့်ဆိုင်း' };
      case 'rework': return { color: 'bg-orange-100 text-orange-800', icon: '🔄', text: 'Rework', textMM: 'ပြန်လုပ်' };
      default: return { color: 'bg-slate-100 text-slate-800', icon: '❓', text: 'Unknown', textMM: 'မသိ' };
    }
  };

  const getQCStatusBadge = (status: string) => {
    const config = getQCStatusConfig(status);
    return (
      <Badge className={config.color}>
        <span className="mr-1">{config.icon}</span>
        {config.text} / {config.textMM}
      </Badge>
    );
  };

  // Render QC Dashboard
  const renderQCDashboard = () => (
    <div className="space-y-6">
      {/* Today's Summary Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm">RM-QC Issued Lots</CardTitle>
            <Package className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{dashboardStats.todayRMQC.issued}</div>
            <p className="text-xs text-muted-foreground">
              ကုန်ကြမ်း QC အတွက် ထုတ်ပေးပြီး
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm">RM-QC Approved</CardTitle>
            <CheckCircle className="h-4 w-4 text-green-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-green-600">{dashboardStats.todayRMQC.approved}</div>
            <p className="text-xs text-muted-foreground">
              ကုန်ကြမ်း QC အတည်ပြုပြီး
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm">RM-QC Pending</CardTitle>
            <Clock className="h-4 w-4 text-yellow-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-yellow-600">{dashboardStats.todayRMQC.pending}</div>
            <p className="text-xs text-muted-foreground">
              ကုန်ကြမ်း QC စောင့်ဆိုင်းနေ
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm">RM-QC Rejected</CardTitle>
            <XCircle className="h-4 w-4 text-red-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-red-600">{dashboardStats.todayRMQC.rejected}</div>
            <p className="text-xs text-muted-foreground">
              ကုန်ကြမ်း QC ငြင်းပယ်ပြီး
            </p>
          </CardContent>
        </Card>
      </div>

      {/* FG-QC Results */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm">FG-QC Good Qty</CardTitle>
            <CheckCircle className="h-4 w-4 text-green-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-green-600">{dashboardStats.todayFGQC.goodQty.toLocaleString()}</div>
            <p className="text-xs text-muted-foreground">
              ပြီးစီးထုတ်ကုန် အရည်အသွေးကောင်း
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm">FG-QC Defect Qty</CardTitle>
            <AlertTriangle className="h-4 w-4 text-yellow-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-yellow-600">{dashboardStats.todayFGQC.defectQty.toLocaleString()}</div>
            <p className="text-xs text-muted-foreground">
              ပြီးစီးထုတ်ကုန် ချွတ်ယွင်းမှု
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm">FG-QC Scrap Qty</CardTitle>
            <XCircle className="h-4 w-4 text-red-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-red-600">{dashboardStats.todayFGQC.scrapQty.toLocaleString()}</div>
            <p className="text-xs text-muted-foreground">
              ပြီးစီးထုတ်ကုန် အမှိုက်
            </p>
          </CardContent>
        </Card>
      </div>

      {/* Blocked Jobs Alert */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2 text-red-600">
            <AlertCircle className="h-5 w-5" />
            Jobs Blocked Due to Pending QC / QC စောင့်ဆိုင်းနေသောကြောင့် ပိတ်ဆို့ထားသော လုပ်ငန်းများ
          </CardTitle>
        </CardHeader>
        <CardContent>
          {dashboardStats.blockedJobs.length > 0 ? (
            <div className="space-y-3">
              {dashboardStats.blockedJobs.map((job, index) => (
                <Alert key={index} className="border-red-200 bg-red-50">
                  <AlertCircle className="h-4 w-4 text-red-600" />
                  <AlertDescription>
                    <div className="flex justify-between items-center">
                      <div>
                        <span className="font-medium">{job.jobId}</span> - {job.reason}
                      </div>
                      <div className="text-sm text-red-600">Since: {job.since}</div>
                    </div>
                  </AlertDescription>
                </Alert>
              ))}
            </div>
          ) : (
            <div className="text-center text-slate-500 py-4">
              No jobs currently blocked by QC / လောလောဆယ် QC ကြောင့် ပိတ်ဆို့ထားသော လုပ်ငန်းမရှိပါ
            </div>
          )}
        </CardContent>
      </Card>

      {/* Quick Actions */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <Card>
          <CardHeader>
            <CardTitle>Pending Raw Material QC / စောင့်ဆိုင်းနေသော ကုန်ကြမ်း QC</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              {rmQCQueue.filter(item => item.status === 'pending-qc').map((item) => (
                <div key={item.id} className="flex items-center justify-between p-3 bg-slate-50 rounded-lg">
                  <div>
                    <div className="font-medium">{item.materialName}</div>
                    <div className="text-sm text-slate-600">{item.jobId} • {item.issuedQty}kg</div>
                  </div>
                  <Button size="sm" onClick={() => onPageChange('raw-material-qc')}>
                    Start QC
                  </Button>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Pending Finished Goods QC / စောင့်ဆိုင်းနေသော ပြီးစီးထုတ်ကုန် QC</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              {fgQCQueue.filter(item => item.status === 'pending-qc').map((item) => (
                <div key={item.id} className="flex items-center justify-between p-3 bg-slate-50 rounded-lg">
                  <div>
                    <div className="font-medium">{item.jobId}</div>
                    <div className="text-sm text-slate-600">{item.machine} • {item.batchQty.toLocaleString()} pcs</div>
                  </div>
                  <Button size="sm" onClick={() => onPageChange('finished-goods-qc')}>
                    Start QC
                  </Button>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );

  // Render Raw Material QC
  const renderRawMaterialQC = () => (
    <div className="space-y-6">
      <div className="flex items-center justify-end mb-4">
        <div className="flex gap-2">
          <Button variant="outline" size="sm">
            <Filter className="h-4 w-4 mr-2" />
            Filter
          </Button>
          <Button variant="outline" size="sm">
            <RefreshCw className="h-4 w-4 mr-2" />
            Refresh
          </Button>
        </div>
      </div>

      {/* RM QC Queue */}
      <Card>
        <CardHeader>
          <CardTitle>Raw Material QC Queue / ကုန်ကြမ်း QC တန်းစီ</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-hidden rounded-lg border">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>QC ID</TableHead>
                  <TableHead>Job ID</TableHead>
                  <TableHead>Material</TableHead>
                  <TableHead>Issued Qty</TableHead>
                  <TableHead>Issued By</TableHead>
                  <TableHead>Issue Date</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {rmQCQueue.map((item) => (
                  <TableRow key={item.id}>
                    <TableCell className="font-medium">{item.id}</TableCell>
                    <TableCell>{item.jobId}</TableCell>
                    <TableCell>
                      <div>
                        <div className="font-medium">{item.materialCode}</div>
                        <div className="text-sm text-slate-600">{item.materialName}</div>
                      </div>
                    </TableCell>
                    <TableCell>{item.issuedQty}kg</TableCell>
                    <TableCell>{item.issuedBy}</TableCell>
                    <TableCell>{item.issuedDate}</TableCell>
                    <TableCell>{getQCStatusBadge(item.status)}</TableCell>
                    <TableCell>
                      <div className="flex gap-2">
                        <Button 
                          size="sm" 
                          onClick={() => {
                            setSelectedRMQC(item);
                            setShowRMQCDialog(true);
                          }}
                          disabled={item.status !== 'pending-qc'}
                        >
                          <Eye className="h-4 w-4 mr-1" />
                          Inspect
                        </Button>
                      </div>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        </CardContent>
      </Card>
    </div>
  );

  // Sample QC reasons data - moved before usage
  const qcReasons = {
    'appearance': [
      { id: 'AP001', en: 'Color Defect', mm: 'ပုံပျက်အရောင်မဟုတ်' },
      { id: 'AP002', en: 'Flashing defect', mm: 'ဘောအပြားပါ' },
      { id: 'AP003', en: 'Oil contamination', mm: 'ဆီရပါ' },
      { id: 'AP004', en: 'Shrink mark', mm: 'ဘောမူရာပျက်' },
      { id: 'AP005', en: 'Burn mark', mm: 'အမှတ်ပါ' }
    ],
    'deforming': [
      { id: 'DF001', en: 'Deformed shape', mm: 'ပုံကွာ' },
      { id: 'DF002', en: 'Size deviation', mm: 'ပစွည်းကွာ' },
      { id: 'DF003', en: 'Misalignment', mm: 'အမှတ် + အမှားကွာ' },
      { id: 'DF004', en: 'Warpage', mm: 'အတုကွာ' },
      { id: 'DF005', en: 'Incomplete engraving/numbering', mm: 'နံပါတ်မလုံမပြည့်' }
    ],
    'others': [
      { id: 'OT001', en: 'Cracked', mm: 'ကွဲသွားမှု' },
      { id: 'OT002', en: 'Wrong Date code', mm: 'Date လုံး' },
      { id: 'OT003', en: 'Foreign particle inside', mm: 'မိုဃ်းရာပါ' },
      { id: 'OT004', en: 'Stone mixed', mm: 'မိုဃ်းကျောက်ပါ' },
      { id: 'OT005', en: 'Moisture defect', mm: 'မိုးချိုးပါ' }
    ],
    'reject-codes': [
      { id: 'RC001', en: 'Major defect - Reject immediately', mm: 'အဓိကချွတ်ယွင်းမှု - ချက်ချင်းပယ်ဖျက်' },
      { id: 'RC002', en: 'Critical safety issue', mm: 'အရေးကြီးသောလုံခြုံရေးပြဿနာ' },
      { id: 'RC003', en: 'Non-repairable damage', mm: 'ပြုပြင်၍မရသောပျက်စီးမှု' }
    ]
  };

  // Add new state variables for Finished Goods QC
  const [currentUser] = useState<{ role: 'admin' | 'user' }>({ role: 'admin' });
  const [qcRecords, setQcRecords] = useState<QCRecord[]>([]);
  const [showNewQCDialog, setShowNewQCDialog] = useState(false);
  const [activeQCTab, setActiveQCTab] = useState('');
  const [wizardStep, setWizardStep] = useState(1);

  // QC reasons state management
  const [qcReasonsState, setQcReasonsState] = useState(qcReasons);

  // Form state for new QC records
  const [qcFormData, setQcFormData] = useState({
    productBatchId: '',
    reason: '',
    quantityChecked: '',
    wasteQty: '',
    inspectorName: '',
    attachments: [] as File[],
    status: 'Pass' as 'Pass' | 'Fail' | 'Recheck'
  });

  // QC Tab configurations
  const getQCTabConfigs = (): QCTabConfig[] => [
    { id: 'appearance', name: 'Appearance', nameMM: 'အသွင်အပြင်', color: 'orange', bgColor: 'bg-orange-100', borderColor: 'border-orange-300', textColor: 'text-orange-800', isActive: true },
    { id: 'deforming', name: 'Deforming', nameMM: 'ပုံပျက်/ပုံမူရာပျက်', color: 'green', bgColor: 'bg-green-100', borderColor: 'border-green-300', textColor: 'text-green-800', isActive: true },
    { id: 'others', name: 'Others', nameMM: 'အခြားများ', color: 'purple', bgColor: 'bg-purple-100', borderColor: 'border-purple-300', textColor: 'text-purple-800', isActive: true },
    { id: 'reject-codes', name: 'Reject Codes', nameMM: 'ပယ်ဖျက်အကြောင်းအရင်း', color: 'blue', bgColor: 'bg-blue-100', borderColor: 'border-blue-300', textColor: 'text-blue-800', isActive: false }
  ];



  // Helper function to get QC tab icon
  const getQCTabIcon = (tabId: string) => {
    switch (tabId) {
      case 'appearance': return <Eye className="h-8 w-8 text-orange-600" />;
      case 'deforming': return <AlertTriangle className="h-8 w-8 text-green-600" />;
      case 'others': return <Package className="h-8 w-8 text-purple-600" />;
      case 'reject-codes': return <XCircle className="h-8 w-8 text-blue-600" />;
      default: return <ClipboardCheck className="h-8 w-8 text-slate-600" />;
    }
  };

  const getQCStatusBadgeColor = (status: string) => {
    switch (status) {
      case 'Pass': return 'bg-green-100 text-green-800';
      case 'Fail': return 'bg-red-100 text-red-800';
      case 'Recheck': return 'bg-yellow-100 text-yellow-800';
      default: return 'bg-slate-100 text-slate-800';
    }
  };

  const getQCStatusIcon = (status: string) => {
    switch (status) {
      case 'Pass': return <CheckCircle className="h-4 w-4" />;
      case 'Fail': return <XCircle className="h-4 w-4" />;
      case 'Recheck': return <RefreshCw className="h-4 w-4" />;
      default: return <Clock className="h-4 w-4" />;
    }
  };

  const handleNewQCRecord = () => {
    if (!qcFormData.productBatchId || !qcFormData.reason || !qcFormData.quantityChecked) {
      // Use alert instead of toast for now
      alert('Please fill in all required fields | လိုအပ်သော အကွက်များကို ဖြည့်ပါ');
      return;
    }

    const newRecord: QCRecord = {
      id: `QC-${Date.now()}`,
      productBatchId: qcFormData.productBatchId,
      reason: qcFormData.reason,
      quantityChecked: parseInt(qcFormData.quantityChecked),
      defectiveQty: parseInt(qcFormData.defectiveQty || '0'),
      inspectorName: qcFormData.inspectorName,
      attachments: qcFormData.attachments.map(f => f.name),
      status: qcFormData.status,
      createdAt: new Date().toLocaleString(),
      category: activeQCTab
    };

    setQcRecords(prev => [newRecord, ...prev]);
    
    // Reset form
    setQcFormData({
      productBatchId: '',
      reason: '',
      quantityChecked: '',
      defectiveQty: '',
      inspectorName: '',
      attachments: [],
      status: 'Pass'
    });
    
    setWizardStep(1);
    setShowNewQCDialog(false);
    alert('New QC record created successfully | QC မှတ်တမ်းအသစ် ဖန်တီးပြီးပါပြီ');
  };

  const handleQCFileUpload = (event: React.ChangeEvent<HTMLInputElement>) => {
    const files = Array.from(event.target.files || []);
    setQcFormData(prev => ({
      ...prev,
      attachments: [...prev.attachments, ...files]
    }));
  };

  const getQCReasonOptions = (tabId: string) => {
    return qcReasonsState[tabId as keyof typeof qcReasonsState] || [];
  };

  const handleAddQCReason = (newReason: { id: string; en: string; mm: string; description?: string }) => {
    const currentTab = activeQCTab || getQCTabConfigs()[0]?.id;
    
    setQcReasonsState(prev => ({
      ...prev,
      [currentTab]: [...(prev[currentTab as keyof typeof prev] || []), newReason]
    }));
  };

  const renderQCNewRecordWizard = (tabConfigs: QCTabConfig[], currentTab: string) => {
    return (
      <div className="space-y-6">
        {/* Step indicator */}
        <div className="flex items-center justify-center space-x-4 mb-8">
          {[1, 2, 3].map((step) => (
            <div key={step} className="flex items-center">
              <div className={`w-10 h-10 rounded-full flex items-center justify-center font-semibold ${
                wizardStep >= step 
                  ? 'bg-blue-600 text-white' 
                  : 'bg-slate-200 text-slate-500'
              }`}>
                {step}
              </div>
              {step < 3 && (
                <div className={`w-16 h-1 ml-4 ${
                  wizardStep > step ? 'bg-blue-600' : 'bg-slate-200'
                }`} />
              )}
            </div>
          ))}
        </div>

        {wizardStep === 1 && (
          <div className="space-y-6">
            <h3 className="text-xl font-semibold text-center">Step 1: Basic Information | အခြေခံအချက်အလက်</h3>
            
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <Label htmlFor="productBatchId">Product / Batch ID (ထုတ်ကုန် / အထုပ်အမှတ်) *</Label>
                <Select value={qcFormData.productBatchId} onValueChange={(value) => setQcFormData(prev => ({ ...prev, productBatchId: value }))}>
                  <SelectTrigger className="h-12 text-lg">
                    <SelectValue placeholder="Select product/batch | ထုတ်ကုန်/အထုပ်ရွေးပါ" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="BATCH-001">BATCH-001 - Plastic Bottle 500ml</SelectItem>
                    <SelectItem value="BATCH-002">BATCH-002 - Plastic Cap Red</SelectItem>
                    <SelectItem value="BATCH-003">BATCH-003 - Container Blue</SelectItem>
                    <SelectItem value="BATCH-004">BATCH-004 - Food Container</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div>
                <Label htmlFor="category">QC Category (QC အမျိုးအစား)</Label>
                <div className="p-3 bg-blue-50 rounded-lg border border-blue-200">
                  <div className="font-medium text-blue-800">
                    {tabConfigs.find(t => t.id === currentTab)?.name || 'Selected Category'}
                  </div>
                  <div className="text-sm text-blue-600">
                    {tabConfigs.find(t => t.id === currentTab)?.nameMM || 'ရွေးချယ်ထားသော အမျိုးအစား'}
                  </div>
                </div>
              </div>
            </div>

            <div>
              <Label htmlFor="reason">Reason (အကြောင်းရင်း) *</Label>
              <ReasonDropdown
                value={qcFormData.reason}
                onValueChange={(value) => setQcFormData(prev => ({ ...prev, reason: value }))}
                category={currentTab}
                categoryDisplayName={tabConfigs.find(t => t.id === currentTab)?.name || 'Category'}
                categoryDisplayNameMM={tabConfigs.find(t => t.id === currentTab)?.nameMM || 'အမျိုးအစား'}
                reasons={getQCReasonOptions(currentTab)}
                onAddReason={handleAddQCReason}
                currentUser={currentUser}
                className="mt-2"
              />
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <Label htmlFor="quantityChecked">Quantity Checked (စစ်ဆေးပမာဏ) *</Label>
                <Input
                  type="number"
                  value={qcFormData.quantityChecked}
                  onChange={(e) => setQcFormData(prev => ({ ...prev, quantityChecked: e.target.value }))}
                  placeholder="Enter quantity checked"
                  className="h-12 text-lg"
                />
              </div>

              <div>
                <Label htmlFor="defectiveQty">Defective Qty (ချို့တဲ့ပမာဏ)</Label>
                <Input
                  type="number"
                  value={qcFormData.defectiveQty}
                  onChange={(e) => setQcFormData(prev => ({ ...prev, defectiveQty: e.target.value }))}
                  placeholder="Enter defective quantity"
                  className="h-12 text-lg"
                />
              </div>
            </div>

            <div>
              <Label htmlFor="inspectorName">Inspector Name (စစ်ဆေးသူအမည်) *</Label>
              <Select value={qcFormData.inspectorName} onValueChange={(value) => setQcFormData(prev => ({ ...prev, inspectorName: value }))}>
                <SelectTrigger className="h-12 text-lg">
                  <SelectValue placeholder="Select inspector | စစ်ဆေးသူရွေးပါ" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="QC-001">John Doe - Senior QC Inspector</SelectItem>
                  <SelectItem value="QC-002">Jane Smith - QC Inspector</SelectItem>
                  <SelectItem value="QC-003">Myanmar User - QC စစ်ဆေးသူ</SelectItem>
                </SelectContent>
              </Select>
            </div>

            <div className="flex justify-end">
              <Button 
                onClick={() => setWizardStep(2)} 
                disabled={!qcFormData.productBatchId || !qcFormData.reason || !qcFormData.quantityChecked}
                className="bg-blue-600 hover:bg-blue-700 px-8"
              >
                Next | ရှေ့သို့
              </Button>
            </div>
          </div>
        )}

        {wizardStep === 2 && (
          <div className="space-y-6">
            <h3 className="text-xl font-semibold text-center">Step 2: Attachments | ဓာတ်ပုံများ</h3>
            
            <div>
              <Label htmlFor="attachments">Attachments (ဓာတ်ပုံ / ဗီဒီယို)</Label>
              <div className="border-2 border-dashed border-slate-300 rounded-lg p-8 text-center hover:border-slate-400 transition-colors">
                <input
                  type="file"
                  id="qc-attachments"
                  multiple
                  accept="image/*,video/*"
                  onChange={handleQCFileUpload}
                  className="hidden"
                />
                <label htmlFor="qc-attachments" className="cursor-pointer">
                  <Camera className="h-12 w-12 mx-auto mb-4 text-slate-400" />
                  <p className="text-lg font-medium text-slate-600 mb-2">
                    Upload photos/videos or click to browse
                  </p>
                  <p className="text-sm text-slate-500">
                    ဓာတ်ပုံများ/ဗီဒီယိုများ ထည့်ပါ သို့မဟုတ် နှိပ်၍ ရွေးပါ
                  </p>
                </label>
              </div>
              
              {qcFormData.attachments.length > 0 && (
                <div className="mt-4 space-y-2">
                  {qcFormData.attachments.map((file, index) => (
                    <div key={index} className="flex items-center gap-3 p-3 bg-slate-50 rounded-lg">
                      <Upload className="h-5 w-5 text-slate-400" />
                      <span className="flex-1 font-medium">{file.name}</span>
                      <span className="text-sm text-slate-500">{(file.size / 1024 / 1024).toFixed(2)} MB</span>
                    </div>
                  ))}
                </div>
              )}
            </div>

            <div className="flex justify-between">
              <Button variant="outline" onClick={() => setWizardStep(1)} className="px-8">
                Back | နောက်သို့
              </Button>
              <Button 
                onClick={() => setWizardStep(3)}
                className="bg-blue-600 hover:bg-blue-700 px-8"
              >
                Next | ရှေ့သို့
              </Button>
            </div>
          </div>
        )}

        {wizardStep === 3 && (
          <div className="space-y-6">
            <h3 className="text-xl font-semibold text-center">Step 3: QC Decision | QC ဆုံးဖြတ်ချက်</h3>
            
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              {['Pass', 'Fail', 'Recheck'].map((status) => (
                <Button
                  key={status}
                  variant={qcFormData.status === status ? "default" : "outline"}
                  className={`h-20 flex-col text-lg font-semibold ${
                    qcFormData.status === status 
                      ? status === 'Pass' ? 'bg-green-600 hover:bg-green-700 text-white'
                        : status === 'Fail' ? 'bg-red-600 hover:bg-red-700 text-white'
                        : 'bg-yellow-600 hover:bg-yellow-700 text-white'
                      : 'border-2 hover:border-blue-300'
                  }`}
                  onClick={() => setQcFormData(prev => ({ ...prev, status: status as any }))}
                >
                  {getQCStatusIcon(status)}
                  <span className="mt-2">{status}</span>
                  <span className="text-sm opacity-75">
                    {status === 'Pass' && '✅'}
                    {status === 'Fail' && '❌'}
                    {status === 'Recheck' && '🔄'}
                  </span>
                </Button>
              ))}
            </div>

            <div className="bg-slate-50 p-6 rounded-lg">
              <h4 className="font-semibold mb-4">QC Record Summary | QC မှတ်တမ်း အကျဉ်းချုပ်</h4>
              <div className="space-y-2 text-sm">
                <div><span className="font-medium">Product/Batch:</span> {qcFormData.productBatchId}</div>
                <div><span className="font-medium">Reason:</span> {qcFormData.reason}</div>
                <div><span className="font-medium">Quantity Checked:</span> {qcFormData.quantityChecked}</div>
                <div><span className="font-medium">Waste Qty:</span> {qcFormData.wasteQty || '0'}</div>
                <div><span className="font-medium">Inspector:</span> {qcFormData.inspectorName}</div>
                <div><span className="font-medium">Status:</span> {qcFormData.status}</div>
                <div><span className="font-medium">Attachments:</span> {qcFormData.attachments.length} files</div>
              </div>
            </div>

            <div className="flex justify-between">
              <Button variant="outline" onClick={() => setWizardStep(2)} className="px-8">
                Back | နောက်သို့
              </Button>
              <Button onClick={handleNewQCRecord} className="bg-green-600 hover:bg-green-700 px-8 text-lg font-semibold">
                <CheckCircle className="h-5 w-5 mr-2" />
                Save QC Record | QC မှတ်တမ်းသိမ်း
              </Button>
            </div>
          </div>
        )}
      </div>
    );
  };

  // Additional states for the new FG QC design
  const [selectedJob, setSelectedJob] = useState<any>(null);
  const [showQCEntryModal, setShowQCEntryModal] = useState(false);
  const [qcEntries, setQcEntries] = useState<any[]>([]);
  const [qcSaved, setQcSaved] = useState(false);
  const [adminPassword, setAdminPassword] = useState('');
  const [showAddReasonDialog, setShowAddReasonDialog] = useState(false);

  // In-Process QC states
  const [selectedInProcessJob, setSelectedInProcessJob] = useState<any>(null);
  const [showInProcessQCModal, setShowInProcessQCModal] = useState(false);
  const [inProcessQCEntries, setInProcessQCEntries] = useState<any[]>([]);
  const [inProcessQCSaved, setInProcessQCSaved] = useState(false);

  // QC Checkpoints data per job
  const [qcCheckpoints, setQcCheckpoints] = useState<Record<string, any[]>>({
    'JOB-2024-001-C1': [
      {
        id: 1,
        timestamp: '2024-01-15 09:30',
        operator: 'Ma Thida',
        rejectQty: 25,
        wasteQty: 15,
        reason: 'Color Waste | ပုံပျက်အရောင်မဟုတ်',
        mainReason: 'appearance',
        subReason: 'AP001'
      },
      {
        id: 2,
        timestamp: '2024-01-15 12:15',
        operator: 'Ma Thida',
        rejectQty: 30,
        wasteQty: 10,
        reason: 'Deformed shape | ပုံကွာ',
        mainReason: 'deforming',
        subReason: 'DF001'
      }
    ],
    'JOB-2024-002-C1': [
      {
        id: 1,
        timestamp: '2024-01-15 11:00',
        operator: 'Ko Zaw',
        rejectQty: 20,
        wasteQty: 8,
        reason: 'Flashing waste | ဘောအပြားပါ',
        mainReason: 'appearance',
        subReason: 'AP002'
      }
    ]
  });

  // Mock jobs pending QC data (completed jobs with QC summaries)
  const jobsPendingQC = [
    {
      jobId: 'JOB-2024-001-C1',
      product: 'Plastic Bottle 500ml Clear',
      machine: 'IM-001',
      shift: 'Day Shift',
      operator: 'Ko Thant',
      qtyProduced: 5000,
      goodQty: 4500,
      rejectQty: 300,
      wasteQty: 200,
      lastQCUpdate: '2024-01-15 14:30',
      qcSummary: {
        totalCheckpoints: 2,
        totalReject: 55,
        totalWaste: 25,
        reasonBreakdown: [
          { reason: 'Color Waste | ပုံပျက်အရောင်မဟုတ်', reject: 25, waste: 15 },
          { reason: 'Deformed shape | ပုံကွာ', reject: 30, waste: 10 }
        ]
      }
    },
    {
      jobId: 'JOB-2024-002-C1',
      product: 'Food Container 1L White',
      machine: 'IM-002',
      shift: 'Day Shift',
      operator: 'Ma Su',
      qtyProduced: 3000,
      goodQty: 2800,
      rejectQty: 150,
      wasteQty: 50,
      lastQCUpdate: '2024-01-15 13:45',
      qcSummary: {
        totalCheckpoints: 1,
        totalReject: 20,
        totalWaste: 8,
        reasonBreakdown: [
          { reason: 'Flashing waste | ဘောအပြားပါ', reject: 20, waste: 8 }
        ]
      }
    },
    {
      jobId: 'JOB-2024-003-C1',
      product: 'Disposable Cup 200ml',
      machine: 'IM-003',
      shift: 'Night Shift',
      operator: 'Ko Aung',
      qtyProduced: 8000,
      goodQty: 7600,
      rejectQty: 350,
      wasteQty: 50,
      lastQCUpdate: '2024-01-15 12:00',
      qcSummary: {
        totalCheckpoints: 0,
        totalReject: 0,
        totalWaste: 0,
        reasonBreakdown: []
      }
    }
  ];

  // QC Entry Form State
  const [qcEntryForm, setQcEntryForm] = useState({
    rejectQty: '',
    wasteQty: '',
    mainReason: '',
    subReason: '',
    entries: [] as any[]
  });

  // In-Process QC Entry Form State
  const [inProcessQCForm, setInProcessQCForm] = useState({
    rejectQty: '',
    wasteQty: '',
    mainReason: '',
    subReason: '',
    entries: [] as any[]
  });

  // Main reasons (fixed 4 options)
  const mainReasons = [
    { id: 'appearance', en: 'Appearance', mm: 'အသွင်အပြင်' },
    { id: 'deforming', en: 'Deforming', mm: 'ပုံပျက်/ပုံမူရာပျက်' },
    { id: 'others', en: 'Others', mm: 'အခြားများ' },
    { id: 'reject-codes', en: 'Reject Codes', mm: 'ပယ်ဖျက်အကြောင်းအရင်း' }
  ];

  // Get sub reasons based on main reason
  const getSubReasons = (mainReasonId: string) => {
    return qcReasonsState[mainReasonId as keyof typeof qcReasonsState] || [];
  };

  // Handle QC Entry Save
  const handleSaveQC = () => {
    if (!qcEntryForm.rejectQty && !qcEntryForm.wasteQty) {
      alert('Please enter at least Reject Qty or Waste Qty | ပယ်ဖျက်မည့်ပမာဏ သို့မဟုတ် အပိုင်အချေအစားပမာဏ ထည့်ပါ');
      return;
    }

    if (qcEntryForm.mainReason && qcEntryForm.subReason) {
      const newEntry = {
        id: Date.now(),
        rejectQty: parseInt(qcEntryForm.rejectQty || '0'),
        wasteQty: parseInt(qcEntryForm.wasteQty || '0'),  
        mainReason: qcEntryForm.mainReason,
        subReason: qcEntryForm.subReason,
        timestamp: new Date().toLocaleString()
      };

      setQcEntries(prev => [...prev, newEntry]);
      
      // Reset form for next entry
      setQcEntryForm({
        rejectQty: '',
        wasteQty: '',
        mainReason: '',
        subReason: '',
        entries: []
      });
    }

    setQcSaved(true);
    alert('QC Entry saved successfully | QC မှတ်တမ်းသိမ်းပြီးပါပြီ');
  };

  // Handle FG Transfer
  const handleFGTransfer = () => {
    if (!qcSaved) {
      alert('Please save QC first | ပထမ QC ကိုသိမ်းပါ');
      return;
    }
    alert('FG Transfer initiated | FG လွှဲပြောင်းမှုစတင်ပြီးပါပြီ');
    setShowQCEntryModal(false);
    setQcSaved(false);
    setQcEntries([]);
  };

  // Handle Add New Reason
  const handleAddNewReason = () => {
    if (adminPassword !== 'admin123') {
      alert('Invalid admin password | အက်ဒမင်စကားဝှက်မှားပါသည်');
      return;
    }
    
    const newReason = prompt('Enter new reason (EN | MM format):');
    if (newReason && newReason.includes('|')) {
      const [en, mm] = newReason.split('|').map(s => s.trim());
      const newReasonObj = {
        id: `CUSTOM-${Date.now()}`,
        en,
        mm
      };
      
      // Add to current main reason category
      const currentMainReason = qcEntryForm.mainReason || inProcessQCForm.mainReason;
      if (currentMainReason) {
        setQcReasonsState(prev => ({
          ...prev,
          [currentMainReason]: [...(prev[currentMainReason as keyof typeof prev] || []), newReasonObj]
        }));
      }
      alert('New reason added successfully | အကြောင်းရင်းအသစ်ထည့်ပြီးပါပြီ');
    }
    setShowAddReasonDialog(false);
    setAdminPassword('');
  };

  // Handle In-Process QC Save
  const handleSaveInProcessQC = () => {
    if (!inProcessQCForm.rejectQty && !inProcessQCForm.wasteQty) {
      alert('Please enter at least Reject Qty or Waste Qty | ဖယ်ရှားမည့်ပမာဏ သို့မဟုတ် အပိုင်အချေအစားပမာဏ ထည့်ပါ');
      return;
    }

    if (inProcessQCForm.mainReason && inProcessQCForm.subReason && selectedInProcessJob) {
      const newCheckpoint = {
        id: Date.now(),
        timestamp: new Date().toLocaleString(),
        operator: 'Ma Thida', // Current QC operator
        rejectQty: parseInt(inProcessQCForm.rejectQty || '0'),
        wasteQty: parseInt(inProcessQCForm.wasteQty || '0'),
        reason: `${getSubReasons(inProcessQCForm.mainReason).find(r => r.id === inProcessQCForm.subReason)?.en} | ${getSubReasons(inProcessQCForm.mainReason).find(r => r.id === inProcessQCForm.subReason)?.mm}`,
        mainReason: inProcessQCForm.mainReason,
        subReason: inProcessQCForm.subReason
      };

      // Add to QC checkpoints for this job
      setQcCheckpoints(prev => ({
        ...prev,
        [selectedInProcessJob.jobId]: [...(prev[selectedInProcessJob.jobId] || []), newCheckpoint]
      }));
      
      // Reset form
      setInProcessQCForm({
        rejectQty: '',
        wasteQty: '',
        mainReason: '',
        subReason: '',
        entries: []
      });
    }

    alert('QC Checkpoint saved successfully | QC စစ်ဆေးမှုအမှတ်သိမ်းပြီးပါပြီ');
  };

  // Render QC Entry Modal
  const renderQCEntryModal = () => {
    if (!selectedJob) return null;

    return (
      <Dialog open={showQCEntryModal} onOpenChange={setShowQCEntryModal}>
        <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle className="text-xl">
              QC Entry | QC မှတ်တမ်းတင်ခြင်း
            </DialogTitle>
            <DialogDescription>
              Job ID: {selectedJob.jobId} | လုပ်ငန်းအမှတ်: {selectedJob.jobId}
            </DialogDescription>
          </DialogHeader>

          <div className="space-y-6">
            {/* Job Information */}
            <Card>
              <CardHeader>
                <CardTitle className="text-lg">Job Information | လုပ်ငန်းအချက်အလက်</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="grid grid-cols-2 md:grid-cols-5 gap-4 text-sm">
                  <div>
                    <Label className="font-medium">Job ID</Label>
                    <div className="text-slate-700">{selectedJob.jobId}</div>
                  </div>
                  <div>
                    <Label className="font-medium">Product | ထုတ်ကုန်</Label>
                    <div className="text-slate-700">{selectedJob.product}</div>
                  </div>
                  <div>
                    <Label className="font-medium">Machine | စက်</Label>
                    <div className="text-slate-700">{selectedJob.machine}</div>
                  </div>
                  <div>
                    <Label className="font-medium">Shift | အလုပ်ပတ်</Label>
                    <div className="text-slate-700">{selectedJob.shift}</div>
                  </div>
                  <div>
                    <Label className="font-medium">Operator | အော်ပရေတာ</Label>
                    <div className="text-slate-700">{selectedJob.operator}</div>
                  </div>
                </div>
              </CardContent>
            </Card>

            {/* QC Entry Form */}
            <Card>
              <CardHeader>
                <CardTitle className="text-lg">QC Entry Form | QC မှတ်တမ်းပုံစံ</CardTitle>
              </CardHeader>
              <CardContent className="space-y-6">
                {/* Quantities */}
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <Label className="text-base font-medium">Reject Qty | ဖယ်ရှားမည့်ပမာဏ</Label>
                    <Input
                      type="number"
                      placeholder="0"
                      className="text-lg h-12"
                      value={qcEntryForm.rejectQty}
                      onChange={(e) => setQcEntryForm(prev => ({ ...prev, rejectQty: e.target.value }))}
                    />
                  </div>
                  <div>
                    <Label className="text-base font-medium">Waste Qty | အပိုင်အချေအစားပမာဏ</Label>
                    <Input
                      type="number"
                      placeholder="0"
                      className="text-lg h-12"
                      value={qcEntryForm.wasteQty}
                      onChange={(e) => setQcEntryForm(prev => ({ ...prev, wasteQty: e.target.value }))}
                    />
                  </div>
                </div>

                {/* Reason Selection */}
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <Label className="text-base font-medium">Main Reason | အဓိကအကြောင်းရင်း</Label>
                    <Select 
                      value={qcEntryForm.mainReason} 
                      onValueChange={(value) => setQcEntryForm(prev => ({ ...prev, mainReason: value, subReason: '' }))}
                    >
                      <SelectTrigger className="text-lg h-12">
                        <SelectValue placeholder="Select main reason | အဓိကအကြောင်းရင်းရွေးပါ" />
                      </SelectTrigger>
                      <SelectContent>
                        {mainReasons.map(reason => (
                          <SelectItem key={reason.id} value={reason.id}>
                            {reason.en} | {reason.mm}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>
                  <div>
                    <div className="flex items-center justify-between mb-2">
                      <Label className="text-base font-medium">Sub Reason | အကြောင်းရင်းခွဲ</Label>
                      <Button
                        type="button"
                        variant="outline"
                        size="sm"
                        onClick={() => setShowAddReasonDialog(true)}
                        disabled={!qcEntryForm.mainReason}
                      >
                        <Plus className="h-4 w-4 mr-1" />
                        Add New | အသစ်ထည့်
                      </Button>
                    </div>
                    <Select 
                      value={qcEntryForm.subReason} 
                      onValueChange={(value) => setQcEntryForm(prev => ({ ...prev, subReason: value }))}
                      disabled={!qcEntryForm.mainReason}
                    >
                      <SelectTrigger className="text-lg h-12">
                        <SelectValue placeholder="Select sub reason | အကြောင်းရင်းခွဲရွေးပါ" />
                      </SelectTrigger>
                      <SelectContent>
                        {getSubReasons(qcEntryForm.mainReason).map(reason => (
                          <SelectItem key={reason.id} value={reason.id}>
                            {reason.en} | {reason.mm}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>
                </div>
              </CardContent>
            </Card>

            {/* In-Process QC Summary */}
            {selectedJob?.qcSummary && selectedJob.qcSummary.totalCheckpoints > 0 && (
              <Card>
                <CardHeader>
                  <CardTitle className="text-lg">In-Process QC Summary | လုပ်ငန်းစဉ်အတွင်း QC အကျဉ်းချုပ်</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="space-y-4">
                    {/* Summary Stats */}
                    <div className="grid grid-cols-3 gap-4 p-3 bg-slate-50 rounded-lg">
                      <div className="text-center">
                        <div className="text-lg font-medium text-blue-600">{selectedJob.qcSummary.totalCheckpoints}</div>
                        <div className="text-sm text-slate-600">QC Checkpoints</div>
                      </div>
                      <div className="text-center">
                        <div className="text-lg font-medium text-red-600">{selectedJob.qcSummary.totalReject}</div>
                        <div className="text-sm text-slate-600">Total Reject | စုစုပေါင်းဖယ်ရှား</div>
                      </div>
                      <div className="text-center">
                        <div className="text-lg font-medium text-yellow-600">{selectedJob.qcSummary.totalWaste}</div>
                        <div className="text-sm text-slate-600">Total Waste | စုစုပေါင်းအပိုင်အချေအစား</div>
                      </div>
                    </div>

                    {/* Reason Breakdown */}
                    <div>
                      <div className="font-medium mb-2">Reason Breakdown | အကြောင်းရင်းခွဲခြမ်းစိတ်ဖြာမှု</div>
                      <div className="space-y-2">
                        {selectedJob.qcSummary.reasonBreakdown.map((item, index) => (
                          <div key={index} className="flex items-center justify-between p-2 bg-white rounded border">
                            <div className="text-sm text-slate-700">{item.reason}</div>
                            <div className="flex gap-4 text-sm">
                              <span className="text-red-600">🗑 {item.reject}</span>
                              <span className="text-yellow-600">❌ {item.waste}</span>
                            </div>
                          </div>
                        ))}
                      </div>
                    </div>
                  </div>
                </CardContent>
              </Card>
            )}

            {/* Current Session QC Entries */}
            {qcEntries.length > 0 && (
              <Card>
                <CardHeader>
                  <CardTitle className="text-lg">Final QC Entries | နောက်ဆုံး QC မှတ်တမ်းများ</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="space-y-3">
                    {qcEntries.map(entry => (
                      <div key={entry.id} className="flex items-center justify-between p-3 bg-slate-50 rounded-lg">
                        <div>
                          <div className="font-medium">
                            Reject: {entry.rejectQty} | Waste: {entry.wasteQty}
                          </div>
                          <div className="text-sm text-slate-600">
                            {getSubReasons(entry.mainReason).find(r => r.id === entry.subReason)?.en} | 
                            {getSubReasons(entry.mainReason).find(r => r.id === entry.subReason)?.mm}
                          </div>
                        </div>
                        <div className="text-xs text-slate-500">{entry.timestamp}</div>
                      </div>
                    ))}
                  </div>
                </CardContent>
              </Card>
            )}

            {/* Action Buttons */}
            <div className="flex justify-between">
              <Button
                variant="outline"
                onClick={() => setShowQCEntryModal(false)}
              >
                Cancel | ပယ်ဖျက်
              </Button>
              <div className="flex gap-3">
                <Button
                  onClick={handleSaveQC}
                  className="bg-green-600 hover:bg-green-700 text-white"
                >
                  <CheckCircle className="h-4 w-4 mr-2" />
                  Save QC | QC သိမ်း
                </Button>
                <Button
                  onClick={handleFGTransfer}
                  variant="secondary"
                  disabled={!qcSaved}
                  className={qcSaved ? 'bg-slate-600 hover:bg-slate-700 text-white' : ''}
                >
                  <Truck className="h-4 w-4 mr-2" />
                  FG Transfer | FG လွှဲပြောင်း
                </Button>
              </div>
            </div>
          </div>
        </DialogContent>
      </Dialog>
    );
  };

  // Render Add New Reason Dialog
  const renderAddReasonDialog = () => (
    <Dialog open={showAddReasonDialog} onOpenChange={setShowAddReasonDialog}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Add New Reason | အကြောင်းရင်းအသစ်ထည့်</DialogTitle>
          <DialogDescription>
            Admin password required | အက်ဒမင်စကားဝှက်လိုအပ်
          </DialogDescription>
        </DialogHeader>
        <div className="space-y-4">
          <div>
            <Label>Admin Password | အက်ဒမင်စကားဝှက်</Label>
            <Input
              type="password"
              value={adminPassword}
              onChange={(e) => setAdminPassword(e.target.value)}
              placeholder="Enter admin password"
            />
          </div>
          <div className="flex justify-end gap-2">
            <Button variant="outline" onClick={() => setShowAddReasonDialog(false)}>
              Cancel | ပယ်ဖျက်
            </Button>
            <Button onClick={handleAddNewReason}>
              Add Reason | အကြောင်းရင်းထည့်
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );

  // Render In-Process QC Entry Modal
  const renderInProcessQCModal = () => {
    if (!selectedInProcessJob) return null;

    return (
      <Dialog open={showInProcessQCModal} onOpenChange={setShowInProcessQCModal}>
        <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle className="text-xl">
              New QC Checkpoint | QC စစ်ဆေးမှုအမှတ်အသစ်
            </DialogTitle>
            <DialogDescription>
              Job ID: {selectedInProcessJob.jobId} | လုပ်ငန်းအမှတ်: {selectedInProcessJob.jobId}
            </DialogDescription>
          </DialogHeader>

          <div className="space-y-6">
            {/* Job Information */}
            <Card>
              <CardHeader>
                <CardTitle className="text-lg">Job Information | လုပ်ငန်းအချက်အလက်</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="grid grid-cols-2 md:grid-cols-5 gap-4 text-sm">
                  <div>
                    <Label className="font-medium">Job ID</Label>
                    <div className="text-slate-700">{selectedInProcessJob.jobId}</div>
                  </div>
                  <div>
                    <Label className="font-medium">Product | ထုတ်ကုန်</Label>
                    <div className="text-slate-700">{selectedInProcessJob.productName}</div>
                  </div>
                  <div>
                    <Label className="font-medium">Machine | စက်</Label>
                    <div className="text-slate-700">{selectedInProcessJob.machineNo}</div>
                  </div>
                  <div>
                    <Label className="font-medium">Shift | အလုပ်ပတ်</Label>
                    <div className="text-slate-700">{selectedInProcessJob.shift}</div>
                  </div>
                  <div>
                    <Label className="font-medium">Operator | အော်ပရေတာ</Label>
                    <div className="text-slate-700">{selectedInProcessJob.operatorName}</div>
                  </div>
                </div>
              </CardContent>
            </Card>

            {/* QC Entry Form */}
            <Card>
              <CardHeader>
                <CardTitle className="text-lg">QC Checkpoint Entry | QC စစ်ဆေးမှုအမှတ်တင်ခြင်း</CardTitle>
              </CardHeader>
              <CardContent className="space-y-6">
                {/* Quantities */}
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <Label className="text-base font-medium">Reject Qty | ဖယ်ရှားမည့်ပမာဏ</Label>
                    <Input
                      type="number"
                      placeholder="0"
                      className="text-lg h-12"
                      value={inProcessQCForm.rejectQty}
                      onChange={(e) => setInProcessQCForm(prev => ({ ...prev, rejectQty: e.target.value }))}
                    />
                  </div>
                  <div>
                    <Label className="text-base font-medium">Waste Qty | အပိုင်အချေအစားပမာဏ</Label>
                    <Input
                      type="number"
                      placeholder="0"
                      className="text-lg h-12"
                      value={inProcessQCForm.wasteQty}
                      onChange={(e) => setInProcessQCForm(prev => ({ ...prev, wasteQty: e.target.value }))}
                    />
                  </div>
                </div>

                {/* Reason Selection */}
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <Label className="text-base font-medium">Main Reason | အဓိကအကြောင်းရင်း</Label>
                    <Select 
                      value={inProcessQCForm.mainReason} 
                      onValueChange={(value) => setInProcessQCForm(prev => ({ ...prev, mainReason: value, subReason: '' }))}
                    >
                      <SelectTrigger className="text-lg h-12">
                        <SelectValue placeholder="Select main reason | အဓိကအကြောင်းရင်းရွေးပါ" />
                      </SelectTrigger>
                      <SelectContent>
                        {mainReasons.map(reason => (
                          <SelectItem key={reason.id} value={reason.id}>
                            {reason.en} | {reason.mm}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>
                  <div>
                    <div className="flex items-center justify-between mb-2">
                      <Label className="text-base font-medium">Sub Reason | အကြောင်းရင်းခွဲ</Label>
                      <Button
                        type="button"
                        variant="outline"
                        size="sm"
                        onClick={() => setShowAddReasonDialog(true)}
                        disabled={!inProcessQCForm.mainReason}
                      >
                        <Plus className="h-4 w-4 mr-1" />
                        Add New | အသစ်ထည့်
                      </Button>
                    </div>
                    <Select 
                      value={inProcessQCForm.subReason} 
                      onValueChange={(value) => setInProcessQCForm(prev => ({ ...prev, subReason: value }))}
                      disabled={!inProcessQCForm.mainReason}
                    >
                      <SelectTrigger className="text-lg h-12">
                        <SelectValue placeholder="Select sub reason | အကြောင်းရင်းခွဲရွေးပါ" />
                      </SelectTrigger>
                      <SelectContent>
                        {getSubReasons(inProcessQCForm.mainReason).map(reason => (
                          <SelectItem key={reason.id} value={reason.id}>
                            {reason.en} | {reason.mm}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>
                </div>
              </CardContent>
            </Card>

            {/* Action Buttons */}
            <div className="flex justify-between">
              <Button
                variant="outline"
                onClick={() => setShowInProcessQCModal(false)}
              >
                Cancel | ပယ်ဖျက်
              </Button>
              <Button
                onClick={handleSaveInProcessQC}
                className="bg-green-600 hover:bg-green-700 text-white"
              >
                <CheckCircle className="h-4 w-4 mr-2" />
                Save QC Checkpoint | QC စစ်ဆေးမှုအမှတ်သိမ်း
              </Button>
            </div>
          </div>
        </DialogContent>
      </Dialog>
    );
  };

  // Render Finished Goods QC
  const renderFinishedGoodsQC = () => {
    return (
      <div className="space-y-6">
        {/* Header with filters */}
        <div className="flex items-center justify-between">
          <div>
            <h2 className="text-xl font-semibold text-slate-900">
              Jobs Pending QC | QC စောင့်ဆိုင်းနေသော လုပ်ငန်းများ
            </h2>
            <p className="text-slate-600 mt-1">
              Click "QC Entry" to inspect and record quality control results
            </p>
            <p className="text-sm text-slate-500">
              "QC Entry" ကိုနှိပ်၍ အရည်အသွေးထိန်းချုပ်မှုရလဒ်များကို စစ်ဆေးမှတ်တမ်းတင်ပါ
            </p>
          </div>
          <div className="flex gap-2">
            <Button variant="outline" size="sm">
              <Filter className="h-4 w-4 mr-2" />
              Filter | စစ်ထုတ်
            </Button>
            <Button variant="outline" size="sm">
              <RefreshCw className="h-4 w-4 mr-2" />
              Refresh | ပြန်တင်
            </Button>
          </div>
        </div>

        {/* Jobs Table */}
        <Card>
          <CardContent className="p-0">
            <div className="overflow-hidden rounded-lg border">
              <Table>
                <TableHeader>
                  <TableRow className="bg-slate-50">
                    <TableHead className="font-semibold">Job ID</TableHead>
                    <TableHead className="font-semibold">Product | ထုတ်ကုန်</TableHead>
                    <TableHead className="font-semibold">Machine | စက်</TableHead>
                    <TableHead className="font-semibold">Shift | အလုပ်ပတ်</TableHead>
                    <TableHead className="font-semibold">Qty Produced | ထုတ်လုပ်ပမာណ</TableHead>
                    <TableHead className="font-semibold">Good Qty | ကောင်းသောပမာဏ</TableHead>
                    <TableHead className="font-semibold">Reject Qty | ဖယ်ရှားပမာဏ</TableHead>
                    <TableHead className="font-semibold">Waste Qty | အပိုင်အချေအစားပမာဏ</TableHead>
                    <TableHead className="font-semibold">Last QC Update</TableHead>
                    <TableHead className="font-semibold">Actions | လုပ်ဆောင်ချက်</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {jobsPendingQC.map((job) => (
                    <TableRow key={job.jobId} className="hover:bg-slate-50">
                      <TableCell className="font-mono font-medium">{job.jobId}</TableCell>
                      <TableCell>
                        <div className="font-medium">{job.product}</div>
                      </TableCell>
                      <TableCell className="font-medium">{job.machine}</TableCell>
                      <TableCell>{job.shift}</TableCell>
                      <TableCell className="text-right font-mono">{job.qtyProduced.toLocaleString()}</TableCell>
                      <TableCell className="text-right font-mono text-green-600 font-medium">{job.goodQty.toLocaleString()}</TableCell>
                      <TableCell className="text-right font-mono text-red-600 font-medium">{job.rejectQty.toLocaleString()}</TableCell>
                      <TableCell className="text-right font-mono text-yellow-600 font-medium">{job.wasteQty.toLocaleString()}</TableCell>
                      <TableCell className="text-xs font-mono text-slate-600">{job.lastQCUpdate}</TableCell>
                      <TableCell>
                        <Button
                          size="sm"
                          onClick={() => {
                            setSelectedJob(job);
                            setShowQCEntryModal(true);
                            setQcSaved(false);
                            setQcEntries([]);
                          }}
                          className="bg-blue-600 hover:bg-blue-700 text-white"
                        >
                          <ClipboardCheck className="h-4 w-4 mr-1" />
                          QC Entry
                        </Button>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          </CardContent>
        </Card>

        {/* QC Entry Modal */}
        {renderQCEntryModal()}
        
        {/* Add New Reason Dialog */}
        {renderAddReasonDialog()}
      </div>
    );
  };

  // Render QC Reports
  const renderQCReports = () => (
    <div className="space-y-6">
      <div className="flex items-center justify-end mb-4">
        <div className="flex gap-2">
          <Button variant="outline" size="sm">
            <Download className="h-4 w-4 mr-2" />
            Export Excel
          </Button>
          <Button variant="outline" size="sm">
            <Download className="h-4 w-4 mr-2" />
            Export PDF
          </Button>
        </div>
      </div>

      {/* Report Categories */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <Card className="cursor-pointer hover:shadow-lg transition-shadow">
          <CardHeader>
            <CardTitle className="flex items-center gap-2 text-sm">
              <Package className="h-5 w-5 text-blue-600" />
              Raw Material QC Report
            </CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-xs text-slate-600">Per Job ID, Supplier, Status</p>
            <p className="text-xs text-slate-500">လုပ်ငန်းအမှတ်၊ ပေးသွင်းသူ၊ အခြေအနေအလိုက်</p>
          </CardContent>
        </Card>

        <Card className="cursor-pointer hover:shadow-lg transition-shadow">
          <CardHeader>
            <CardTitle className="flex items-center gap-2 text-sm">
              <CheckCircle className="h-5 w-5 text-green-600" />
              Finished Goods QC Report
            </CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-xs text-slate-600">Per Job ID, Machine, Good vs Defect %</p>
            <p className="text-xs text-slate-500">လုပ်ငန်းအမှတ်၊ စက်၊ ကောင်းနှင့်ညံ့ရာခိုင်နှုန်း</p>
          </CardContent>
        </Card>

        <Card className="cursor-pointer hover:shadow-lg transition-shadow">
          <CardHeader>
            <CardTitle className="flex items-center gap-2 text-sm">
              <AlertTriangle className="h-5 w-5 text-red-600" />
              NCR Summary
            </CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-xs text-slate-600">Non-Conformity Report</p>
            <p className="text-xs text-slate-500">စံနှုန်းမမီမှု အစီရင်ခံစာ</p>
          </CardContent>
        </Card>

        <Card className="cursor-pointer hover:shadow-lg transition-shadow">
          <CardHeader>
            <CardTitle className="flex items-center gap-2 text-sm">
              <FileCheck className="h-5 w-5 text-purple-600" />
              Traceability Report
            </CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-xs text-slate-600">Material Issue → QC → Job ID → FG Result</p>
            <p className="text-xs text-slate-500">ခြေရာခံနိုင်မှု အစီရင်ခံစာ</p>
          </CardContent>
        </Card>
      </div>
    </div>
  );

  // Render QC Settings
  const renderQCSettings = () => (
    <div className="space-y-6">

      <Tabs defaultValue="templates" className="space-y-4">
        <TabsList>
          <TabsTrigger value="templates">QC Templates</TabsTrigger>
          <TabsTrigger value="defects">Defect Codes</TabsTrigger>
          <TabsTrigger value="sampling">Sampling Plans</TabsTrigger>
          <TabsTrigger value="roles">Roles & Permissions</TabsTrigger>
          <TabsTrigger value="notifications">Notifications</TabsTrigger>
        </TabsList>

        <TabsContent value="templates">
          <Card>
            <CardHeader>
              <CardTitle>QC Test Templates / QC စစ်ဆေးမှုပုံစံများ</CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-slate-600">Configure QC test templates per material and product</p>
              <p className="text-sm text-slate-500">ပစ္စည်းနှင့်ထုတ်ကုန်အလိုက် QC စစ်ဆေးမှုပုံစံများ သတ်မှတ်ပါ</p>
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="defects">
          <Card>
            <CardHeader>
              <CardTitle>Defect / Reject Codes / ချွတ်ယွင်းမှု/ငြင်းပယ်မှုကုဒ်များ</CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-slate-600">Manage defect and rejection reason codes (EN/MM)</p>
              <p className="text-sm text-slate-500">ချွတ်ယွင်းမှုနှင့် ငြင်းပယ်မှုအကြောင်းရင်းကုဒ်များ (အင်္ဂလိပ်/မြန်မာ)</p>
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="sampling">
          <Card>
            <CardHeader>
              <CardTitle>Sampling Plans / နမူနာယူမှုအစီအစဉ်များ</CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-slate-600">Configure AQL levels and sampling percentages</p>
              <p className="text-sm text-slate-500">AQL အဆင့်များနှင့် နမူနာယူရာခိုင်နှုန်းများ သတ်မှတ်ပါ</p>
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="roles">
          <Card>
            <CardHeader>
              <CardTitle>Roles & Permissions / အခန်းကဏ္ဍများနှင့် ခွင့်ပြုချက်များ</CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-slate-600">Manage QC Inspector vs QC Lead permissions</p>
              <p className="text-sm text-slate-500">QC စစ်ဆေးသူနှင့် QC ဦးဆောင်သူ ခွင့်ပြုချက်များ</p>
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="notifications">
          <Card>
            <CardHeader>
              <CardTitle>Notifications / အကြောင်းကြားစာများ</CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-slate-600">Configure pending QC aging alerts and reject notifications</p>
              <p className="text-sm text-slate-500">စောင့်ဆိုင်းနေသော QC အချိန်လွန်သတိပေးချက်များနှင့် ငြင်းပယ်မှုအကြောင်းကြားချက်များ</p>
            </CardContent>
          </Card>
        </TabsContent>
      </Tabs>
    </div>
  );

  // Raw Material QC Dialog
  const renderRMQCDialog = () => {
    if (!selectedRMQC) return null;

    return (
      <Dialog open={showRMQCDialog} onOpenChange={setShowRMQCDialog}>
        <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>Raw Material QC Inspection / ကုန်ကြမ်း QC စစ်ဆေးမှု</DialogTitle>
            <DialogDescription>
              {selectedRMQC.id} - {selectedRMQC.materialName}
            </DialogDescription>
          </DialogHeader>

          <Tabs defaultValue="context" className="w-full">
            <TabsList>
              <TabsTrigger value="context">Context</TabsTrigger>
              <TabsTrigger value="inspection">Inspection</TabsTrigger>
              <TabsTrigger value="decision">QC Decision</TabsTrigger>
            </TabsList>

            <TabsContent value="context" className="space-y-4">
              <Card>
                <CardHeader>
                  <CardTitle>Context Panel / အခြေအနေအကြောင်းအရာ</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                    <div>
                      <Label>Plan ID</Label>
                      <div className="font-medium">{selectedRMQC.planId}</div>
                    </div>
                    <div>
                      <Label>Job ID</Label>
                      <div className="font-medium">{selectedRMQC.jobId}</div>
                    </div>
                    <div>
                      <Label>Product Name</Label>
                      <div className="font-medium">{selectedRMQC.productName}</div>
                    </div>
                    <div>
                      <Label>Material Code</Label>
                      <div className="font-medium">{selectedRMQC.materialCode}</div>
                    </div>
                    <div>
                      <Label>Material Name</Label>
                      <div className="font-medium">{selectedRMQC.materialName}</div>
                    </div>
                    <div>
                      <Label>Issued Qty</Label>
                      <div className="font-medium">{selectedRMQC.issuedQty}kg</div>
                    </div>
                    <div>
                      <Label>Issued By</Label>
                      <div className="font-medium">{selectedRMQC.issuedBy}</div>
                    </div>
                    <div>
                      <Label>Status</Label>
                      <div>{getQCStatusBadge(selectedRMQC.status)}</div>
                    </div>
                  </div>
                </CardContent>
              </Card>
            </TabsContent>

            <TabsContent value="inspection" className="space-y-4">
              <Card>
                <CardHeader>
                  <CardTitle>Visual Checks / မျက်လုံးကြည့်စစ်ဆေးမှု</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="grid grid-cols-2 gap-4">
                    <div className="flex items-center space-x-2">
                      <Checkbox id="contamination" />
                      <Label htmlFor="contamination">Contamination / ညစ်ညမ်းမှု</Label>
                    </div>
                    <div className="flex items-center space-x-2">
                      <Checkbox id="moisture" />
                      <Label htmlFor="moisture">Moisture / စိုထိုင်းမှု</Label>
                    </div>
                    <div className="flex items-center space-x-2">
                      <Checkbox id="packaging" />
                      <Label htmlFor="packaging">Packaging / ထုပ်ပိုးမှု</Label>
                    </div>
                    <div className="flex items-center space-x-2">
                      <Checkbox id="label" />
                      <Label htmlFor="label">Label / တံဆိပ်</Label>
                    </div>
                  </div>
                </CardContent>
              </Card>

              <Card>
                <CardHeader>
                  <CardTitle>Test Parameters / စစ်ဆေးမှုအတိုင်းအတာများ</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="space-y-4">
                    {selectedRMQC.testParameters.map((param) => (
                      <div key={param.id} className="grid grid-cols-4 gap-4 items-center">
                        <div>
                          <Label>{param.parameter}</Label>
                        </div>
                        <div>
                          <Label>Spec: {param.spec}</Label>
                        </div>
                        <div>
                          <Input placeholder="Measured value" />
                        </div>
                        <div>
                          <Select>
                            <SelectTrigger>
                              <SelectValue placeholder="Result" />
                            </SelectTrigger>
                            <SelectContent>
                              <SelectItem value="pass">Pass / အောင်မြင်</SelectItem>
                              <SelectItem value="fail">Fail / မအောင်မြင်</SelectItem>
                            </SelectContent>
                          </Select>
                        </div>
                      </div>
                    ))}
                  </div>
                </CardContent>
              </Card>

              <Card>
                <CardHeader>
                  <CardTitle>Attachments / ပူးတွဲစာရွက်စာတမ်းများ</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="space-y-4">
                    <div className="border-2 border-dashed border-slate-300 rounded-lg p-6 text-center">
                      <Upload className="h-8 w-8 mx-auto mb-2 text-slate-400" />
                      <p className="text-slate-600 mb-2">Upload COA (Certificate of Analysis)</p>
                      <p className="text-xs text-slate-500">PDF, JPG, PNG up to 10MB</p>
                      <Button className="mt-2" size="sm">
                        <Upload className="h-4 w-4 mr-2" />
                        Upload COA
                      </Button>
                    </div>
                    <div className="border-2 border-dashed border-slate-300 rounded-lg p-6 text-center">
                      <Camera className="h-8 w-8 mx-auto mb-2 text-slate-400" />
                      <p className="text-slate-600 mb-2">Upload Photos</p>
                      <p className="text-xs text-slate-500">JPG, PNG up to 5MB each</p>
                      <Button className="mt-2" size="sm">
                        <Camera className="h-4 w-4 mr-2" />
                        Take Photo
                      </Button>
                    </div>
                  </div>
                </CardContent>
              </Card>
            </TabsContent>

            <TabsContent value="decision" className="space-y-4">
              <Card>
                <CardHeader>
                  <CardTitle>QC Decision / QC အဖြေ</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  <div>
                    <Label>QC Decision / အရည်အသွေး စိစစ်အဖြေ</Label>
                    <Select>
                      <SelectTrigger>
                        <SelectValue placeholder="Select QC decision..." />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="pass">🟢 Pass / အောင်မြင် (Release to Production)</SelectItem>
                        <SelectItem value="hold">🟡 Hold / ရပ်နားထား (Quarantine)</SelectItem>
                        <SelectItem value="reject">🔴 Reject / ငြင်းပယ် (Return to RM-WH)</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>

                  <div>
                    <Label>Reason / အကြောင်းရင်း</Label>
                    <Textarea 
                      placeholder="Enter reason for QC decision..."
                      rows={3}
                    />
                  </div>

                  <div>
                    <Label>QC Inspector / QC စစ်ဆေးသူ</Label>
                    <Select>
                      <SelectTrigger>
                        <SelectValue placeholder="Select inspector..." />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="qc1">QC Inspector 1</SelectItem>
                        <SelectItem value="qc2">QC Inspector 2</SelectItem>
                        <SelectItem value="qc3">QC Inspector 3</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>

                  <Alert>
                    <AlertCircle className="h-4 w-4" />
                    <AlertDescription>
                      <strong>System Behavior:</strong><br />
                      • Pass = 🟢 Material released to production (operator can start)<br />
                      • Hold = 🟡 Material quarantined (blocked until re-test)<br />
                      • Reject = 🔴 Material returned to RM-WH (Planner + Inventory notified)
                    </AlertDescription>
                  </Alert>
                </CardContent>
              </Card>
            </TabsContent>
          </Tabs>

          <div className="flex gap-3 pt-4 border-t">
            <Button className="bg-green-600 hover:bg-green-700">
              <ClipboardCheck className="h-4 w-4 mr-2" />
              Submit QC Result
            </Button>
            <Button variant="outline">
              <FileText className="h-4 w-4 mr-2" />
              Save Draft
            </Button>
            <Button variant="outline" onClick={() => setShowRMQCDialog(false)}>
              Cancel
            </Button>
          </div>
        </DialogContent>
      </Dialog>
    );
  };

  // Finished Goods QC Dialog  
  const renderFGQCDialog = () => {
    if (!selectedFGQC) return null;

    return (
      <Dialog open={showFGQCDialog} onOpenChange={setShowFGQCDialog}>
        <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>Finished Goods QC Inspection / ပြီးစီးထုတ်ကုန် QC စစ်ဆေးမှု</DialogTitle>
            <DialogDescription>
              {selectedFGQC.id} - {selectedFGQC.jobId}
            </DialogDescription>
          </DialogHeader>

          <Tabs defaultValue="context" className="w-full">
            <TabsList>
              <TabsTrigger value="context">Context</TabsTrigger>
              <TabsTrigger value="inspection">Inspection</TabsTrigger>
              <TabsTrigger value="decision">QC Decision</TabsTrigger>
            </TabsList>

            <TabsContent value="context" className="space-y-4">
              <Card>
                <CardHeader>
                  <CardTitle>Context Panel / အခြေအနေအကြောင်းအရာ</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                    <div>
                      <Label>Job ID</Label>
                      <div className="font-medium">{selectedFGQC.jobId}</div>
                    </div>
                    <div>
                      <Label>Batch Qty</Label>
                      <div className="font-medium">{selectedFGQC.batchQty.toLocaleString()}</div>
                    </div>
                    <div>
                      <Label>Machine</Label>
                      <div className="font-medium">{selectedFGQC.machine}</div>
                    </div>
                    <div>
                      <Label>Operator</Label>
                      <div className="font-medium">{selectedFGQC.operator}</div>
                    </div>
                    <div>
                      <Label>Shift</Label>
                      <div className="font-medium">{selectedFGQC.shift}</div>
                    </div>
                    <div>
                      <Label>Production Date</Label>
                      <div className="font-medium">{selectedFGQC.productionDate}</div>
                    </div>
                    <div>
                      <Label>Status</Label>
                      <div>{getQCStatusBadge(selectedFGQC.status)}</div>
                    </div>
                    <div>
                      <Label>Sampling Size</Label>
                      <div className="font-medium">{selectedFGQC.samplingSize} pcs</div>
                    </div>
                  </div>
                </CardContent>
              </Card>
            </TabsContent>

            <TabsContent value="inspection" className="space-y-4">
              <Card>
                <CardHeader>
                  <CardTitle>Visual Defects / မျက်လုံးကြည့်ချွတ်ယွင်းမှုများ</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="grid grid-cols-2 gap-4">
                    {selectedFGQC.visualDefects.map((defect) => (
                      <div key={defect.defectType} className="flex items-center gap-4">
                        <Label className="w-24">{defect.defectType}</Label>
                        <Input 
                          type="number" 
                          placeholder="0" 
                          className="w-20"
                          defaultValue={defect.count}
                        />
                        <span className="text-sm text-slate-500">pcs</span>
                      </div>
                    ))}
                  </div>
                </CardContent>
              </Card>

              <Card>
                <CardHeader>
                  <CardTitle>Measurements / တိုင်းတာမှုများ</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="space-y-4">
                    {selectedFGQC.measurements.map((measurement, index) => (
                      <div key={index} className="grid grid-cols-4 gap-4 items-center">
                        <div>
                          <Label>{measurement.parameter}</Label>
                        </div>
                        <div>
                          <Label>Spec: {measurement.spec}</Label>
                        </div>
                        <div>
                          <Input placeholder="Measured value" />
                        </div>
                        <div>
                          <Select>
                            <SelectTrigger>
                              <SelectValue placeholder="Result" />
                            </SelectTrigger>
                            <SelectContent>
                              <SelectItem value="pass">Pass / အောင်မြင်</SelectItem>
                              <SelectItem value="fail">Fail / မအောင်မြင်</SelectItem>
                            </SelectContent>
                          </Select>
                        </div>
                      </div>
                    ))}
                  </div>
                </CardContent>
              </Card>

              <Card>
                <CardHeader>
                  <CardTitle>Defect Entry / ချွတ်ယွင်းမှုမှတ်တမ်းတင်ခြင်း</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="grid grid-cols-3 gap-4">
                    <div>
                      <Label>Good Qty / ကောင်းသောပမာណ</Label>
                      <Input type="number" placeholder="0" />
                    </div>
                    <div>
                      <Label>Defect Qty / ချွတ်ယွင်းသောပမာণ</Label>
                      <Input type="number" placeholder="0" />
                    </div>
                    <div>
                      <Label>Scrap Qty / အမှိုက်ပမာণ</Label>
                      <Input type="number" placeholder="0" />
                    </div>
                  </div>
                </CardContent>
              </Card>
            </TabsContent>

            <TabsContent value="decision" className="space-y-4">
              <Card>
                <CardHeader>
                  <CardTitle>QC Decision / QC အဖြေ</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  <div>
                    <Label>QC Decision / အရည်အသွေး စိစစ်အဖြေ</Label>
                    <Select>
                      <SelectTrigger>
                        <SelectValue placeholder="Select QC decision..." />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="accept">🟢 Accept / လက်ခံ (Allow QR Print)</SelectItem>
                        <SelectItem value="rework">🟡 Rework / ပြန်လုပ် (Send to Cutting WH)</SelectItem>
                        <SelectItem value="reject">🔴 Reject / ငြင်းပယ် (Send to Scrap WH)</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>

                  <div>
                    <Label>Comments / မှတ်ချက်များ</Label>
                    <Textarea 
                      placeholder="Enter comments and recommendations..."
                      rows={3}
                    />
                  </div>

                  <div>
                    <Label>QC Inspector / QC စစ်ဆေးသူ</Label>
                    <Select>
                      <SelectTrigger>
                        <SelectValue placeholder="Select inspector..." />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="qc1">QC Inspector 1</SelectItem>
                        <SelectItem value="qc2">QC Inspector 2</SelectItem>
                        <SelectItem value="qc3">QC Inspector 3</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>

                  <Alert>
                    <AlertCircle className="h-4 w-4" />
                    <AlertDescription>
                      <strong>System Behavior:</strong><br />
                      • Accept = 🟢 Enable QR Print for batch<br />
                      • Rework = 🟡 Send items to Cutting WH<br />
                      • Reject = 🔴 Block QR print, move items to Scrap WH<br />
                      • If Defect &gt; Threshold &rarr; Alert Production Manager
                    </AlertDescription>
                  </Alert>
                </CardContent>
              </Card>
            </TabsContent>
          </Tabs>

          <div className="flex gap-3 pt-4 border-t">
            <Button className="bg-green-600 hover:bg-green-700">
              <ClipboardCheck className="h-4 w-4 mr-2" />
              Submit QC Result
            </Button>
            <Button variant="outline">
              <FileText className="h-4 w-4 mr-2" />
              Save Draft
            </Button>
            <Button variant="outline" onClick={() => setShowFGQCDialog(false)}>
              Cancel
            </Button>
          </div>
        </DialogContent>
      </Dialog>
    );
  };

  // Render In-Process QC
  const renderInProcessQC = () => {

    // Mock running jobs data
    const runningJobs = [
      {
        id: 'JOB-001',
        jobId: 'JOB-2024-001-C1',
        machineNo: 'IM-001',
        productName: 'Plastic Bottle 500ml Clear',
        productImage: 'https://images.unsplash.com/photo-1571068316344-75bc76f77890?w=100&h=100&fit=crop',
        operatorName: 'Ko Thant',
        shift: 'Day Shift',
        targetQty: 5000,
        completedQty: 3200,
        operatorInputQty: 3200,
        status: 'running' as const,
        startTime: '08:00',
        estimatedEndTime: '16:00',
        goodQty: 2800,
        defectQty: 300,
        scrapQty: 100,
        lastQCUpdate: '2024-01-15 14:30',
        qcStaff: 'Ma Thida'
      },
      {
        id: 'JOB-002', 
        jobId: 'JOB-2024-002-C1',
        machineNo: 'IM-002',
        productName: 'Food Container 1L White',
        productImage: 'https://images.unsplash.com/photo-1584464491033-06628f3a6b7b?w=100&h=100&fit=crop',
        operatorName: 'Ma Su',
        shift: 'Day Shift',
        targetQty: 3000,
        completedQty: 2100,
        operatorInputQty: 2100,
        status: 'running' as const,
        startTime: '09:00',
        estimatedEndTime: '17:00',
        goodQty: 1950,
        defectQty: 120,
        scrapQty: 30,
        lastQCUpdate: '2024-01-15 14:45',
        qcStaff: 'Ko Zaw'
      },
      {
        id: 'JOB-003',
        jobId: 'JOB-2024-003-C1', 
        machineNo: 'IM-003',
        productName: 'Disposable Cup 200ml',
        productImage: 'https://images.unsplash.com/photo-1558618047-3c8c76ca7d13?w=100&h=100&fit=crop',
        operatorName: 'Ko Aung',
        shift: 'Day Shift',
        targetQty: 8000,
        completedQty: 8000,
        operatorInputQty: 8000,
        status: 'completed' as const,
        startTime: '06:00',
        estimatedEndTime: '14:00',
        goodQty: 7600,
        defectQty: 350,
        scrapQty: 50,
        lastQCUpdate: '2024-01-15 14:00',
        qcStaff: 'Ma Thida'
      }
    ];

    // Mock operator logs
    const operatorLogs = [
      { timestamp: '2024-01-15 14:30', operator: 'Ko Thant', qtyEntered: 100 },
      { timestamp: '2024-01-15 14:00', operator: 'Ko Thant', qtyEntered: 150 },
      { timestamp: '2024-01-15 13:30', operator: 'Ko Thant', qtyEntered: 200 }
    ];

    // Mock QC logs
    const qcLogs = [
      { timestamp: '2024-01-15 14:30', qcStaff: 'Ma Thida', good: 90, defect: 8, scrap: 2 },
      { timestamp: '2024-01-15 14:00', qcStaff: 'Ma Thida', good: 140, defect: 8, scrap: 2 },
      { timestamp: '2024-01-15 13:30', qcStaff: 'Ma Thida', good: 185, defect: 12, scrap: 3 }
    ];

    const filteredJobs = runningJobs.filter(job => {
      const matchesSearch = jobIdSearch === '' || job.jobId.toLowerCase().includes(jobIdSearch.toLowerCase());
      const matchesMachine = machineFilter === 'all' || job.machineNo === machineFilter;
      return matchesSearch && matchesMachine;
    });

    const getStatusColor = (status: string) => {
      switch (status) {
        case 'running': return 'bg-green-500';
        case 'completed': return 'bg-blue-500';
        case 'idle': return 'bg-yellow-500';
        default: return 'bg-slate-500';
      }
    };

    const getStatusIcon = (status: string) => {
      switch (status) {
        case 'running': return '🟢';
        case 'completed': return '🔵';
        case 'idle': return '🟡';
        default: return '⚪';
      }
    };

    const handleQCUpdate = (jobId: string, qcData: any) => {
      console.log('QC Update for', jobId, qcData);
      // Implementation for updating QC data
    };

    const handleTransferToFG = (jobId: string) => {
      console.log('Transfer to FG Warehouse:', jobId);
      // Implementation for transferring to FG warehouse
    };

    return (
      <div className="space-y-6">
        {/* Layout: Left Sidebar + Main Panel */}
        <div className="grid grid-cols-1 lg:grid-cols-4 gap-6">
          {/* Left Sidebar - Filters */}
          <div className="lg:col-span-1">
            <Card>
              <CardHeader>
                <CardTitle className="text-lg">Filters | စစ်ထုတ်မှု</CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                {/* Date Filter */}
                <div>
                  <Label>Date Filter</Label>
                  <Select value={dateFilter} onValueChange={setDateFilter}>
                    <SelectTrigger>
                      <SelectValue placeholder="Select date range" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="today">Today | ယနေ့</SelectItem>
                      <SelectItem value="week">This Week | ဤအပတ်</SelectItem>
                      <SelectItem value="month">This Month | ဤလ</SelectItem>
                    </SelectContent>
                  </Select>
                </div>

                {/* Machine Filter */}
                <div>
                  <Label>Machine Filter</Label>
                  <Select value={machineFilter} onValueChange={setMachineFilter}>
                    <SelectTrigger>
                      <SelectValue placeholder="Select machine" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="all">All Machines | စက်အားလုံး</SelectItem>
                      <SelectItem value="IM-001">IM-001</SelectItem>
                      <SelectItem value="IM-002">IM-002</SelectItem>
                      <SelectItem value="IM-003">IM-003</SelectItem>
                      <SelectItem value="IM-004">IM-004</SelectItem>
                    </SelectContent>
                  </Select>
                </div>

                {/* Job ID Search */}
                <div>
                  <Label>Job ID Search</Label>
                  <div className="relative">
                    <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-slate-400" />
                    <Input
                      placeholder="Search by Job ID..."
                      value={jobIdSearch}
                      onChange={(e) => setJobIdSearch(e.target.value)}
                      className="pl-10"
                    />
                  </div>
                </div>

                {/* View Mode Toggle */}
                <div>
                  <Label>View Mode</Label>
                  <div className="flex gap-2">
                    <Button
                      variant={viewMode === 'cards' ? 'default' : 'outline'}
                      onClick={() => setViewMode('cards')}
                      size="sm"
                      className="flex-1"
                    >
                      Cards
                    </Button>
                    <Button
                      variant={viewMode === 'table' ? 'default' : 'outline'}
                      onClick={() => setViewMode('table')}
                      size="sm"
                      className="flex-1"
                    >
                      Table
                    </Button>
                  </div>
                </div>
              </CardContent>
            </Card>
          </div>

          {/* Main Panel */}
          <div className="lg:col-span-3">
            <div className="space-y-6">
              {/* Header Actions */}
              <div className="flex items-center justify-between">
                <h3 className="text-lg font-semibold">Running Jobs | လုပ်ငန်းများ ({filteredJobs.length})</h3>
                <Button variant="outline" size="sm">
                  <RefreshCw className="h-4 w-4 mr-2" />
                  Refresh
                </Button>
              </div>

              {/* Jobs Display */}
              {viewMode === 'cards' ? (
                <div className="grid grid-cols-1 xl:grid-cols-2 gap-6">
                  {filteredJobs.map((job) => (
                    <Card key={job.id} className={`border-l-4 ${
                      job.status === 'running' ? 'border-l-green-500' : 
                      job.status === 'completed' ? 'border-l-blue-500' : 'border-l-yellow-500'
                    }`}>
                      <CardHeader className={`${getStatusColor(job.status)} text-white rounded-t-lg`}>
                        <div className="flex items-center justify-between">
                          <div>
                            <CardTitle className="text-lg">{job.jobId}</CardTitle>
                            <p className="text-sm opacity-90">Machine: {job.machineNo}</p>
                          </div>
                          <Badge className="bg-white/20 text-white">
                            {getStatusIcon(job.status)} {job.status.toUpperCase()}
                          </Badge>
                        </div>
                      </CardHeader>
                      
                      <CardContent className="p-6 space-y-4">
                        {/* Product Info */}
                        <div className="flex items-center gap-3">
                          <img 
                            src={job.productImage} 
                            alt={job.productName}
                            className="w-12 h-12 rounded-lg object-cover"
                          />
                          <div>
                            <h4 className="font-medium">{job.productName}</h4>
                            <p className="text-sm text-slate-600">
                              {job.operatorName} • {job.shift}
                            </p>
                          </div>
                        </div>

                        {/* Progress */}
                        <div>
                          <div className="flex justify-between text-sm mb-2">
                            <span>Progress</span>
                            <span>{job.completedQty.toLocaleString()} / {job.targetQty.toLocaleString()}</span>
                          </div>
                          <Progress value={(job.completedQty / job.targetQty) * 100} className="h-2" />
                          <p className="text-xs text-slate-500 mt-1">
                            {Math.round((job.completedQty / job.targetQty) * 100)}% completed
                          </p>
                        </div>

                        {/* QC Checkpoints */}
                        <div className="bg-blue-50 p-4 rounded-lg border border-blue-200">
                          <div className="flex items-center justify-between mb-3">
                            <h5 className="font-medium text-blue-800">QC Checkpoints | QC စစ်ဆေးမှုအမှတ်များ</h5>
                            <Button
                              size="sm"
                              onClick={() => {
                                setSelectedInProcessJob(job);
                                setShowInProcessQCModal(true);
                                setInProcessQCForm({
                                  scrapQty: '',
                                  defectQty: '',
                                  mainReason: '',
                                  subReason: '',
                                  entries: []
                                });
                              }}
                              className="bg-blue-600 hover:bg-blue-700 text-white"
                            >
                              <Plus className="h-4 w-4 mr-1" />
                              New QC Checkpoint
                            </Button>
                          </div>

                          {/* QC Checkpoints List */}
                          <div className="space-y-2 mb-4">
                            {(qcCheckpoints[job.jobId] || []).length === 0 ? (
                              <div className="text-center text-slate-500 py-4 text-sm">
                                No QC checkpoints yet | QC စစ်ဆေးမှုအမှတ်များ မရှိသေးပါ
                              </div>
                            ) : (
                              (qcCheckpoints[job.jobId] || []).map((checkpoint) => (
                                <div key={checkpoint.id} className="bg-white p-3 rounded-lg border border-slate-200">
                                  <div className="flex items-start justify-between">
                                    <div className="flex-1">
                                      <div className="flex items-center gap-2 text-sm">
                                        <span className="font-medium text-slate-700">{checkpoint.timestamp}</span>
                                        <span className="text-slate-500">by {checkpoint.operator}</span>
                                      </div>
                                      <div className="flex items-center gap-4 mt-1 text-sm">
                                        <span className="text-red-600 font-medium">🗑 Scrap: {checkpoint.scrapQty}</span>
                                        <span className="text-yellow-600 font-medium">❌ Defect: {checkpoint.defectQty}</span>
                                      </div>
                                      <div className="text-xs text-slate-600 mt-1">
                                        {checkpoint.reason}
                                      </div>
                                    </div>
                                  </div>
                                </div>
                              ))
                            )}
                          </div>

                          {/* Summary */}
                          {(qcCheckpoints[job.jobId] || []).length > 0 && (
                            <div className="bg-slate-100 p-3 rounded-lg">
                              <div className="text-sm font-medium text-slate-700 mb-1">Total Summary | စုစုပေါင်းအကျဉ်းချုပ်</div>
                              <div className="flex items-center gap-4 text-sm">
                                <span className="text-red-600">
                                  🗑 Total Scrap: {(qcCheckpoints[job.jobId] || []).reduce((sum, cp) => sum + cp.scrapQty, 0)}
                                </span>
                                <span className="text-yellow-600">
                                  ❌ Total Defect: {(qcCheckpoints[job.jobId] || []).reduce((sum, cp) => sum + cp.defectQty, 0)}
                                </span>
                              </div>
                            </div>
                          )}

                          {/* Transfer Button */}
                          {job.status === 'completed' && (
                            <div className="mt-3">
                              <Button
                                onClick={() => handleTransferToFG(job.jobId)}
                                className="w-full bg-green-600 hover:bg-green-700 text-white"
                              >
                                <Package className="h-4 w-4 mr-2" />
                                Transfer to FG QC | FG QC သို့လွှဲပြောင်း
                              </Button>
                            </div>
                          )}
                        </div>
                      </CardContent>
                    </Card>
                  ))}
                </div>
              ) : (
                // Table View
                <Card>
                  <CardContent className="p-0">
                    <Table>
                      <TableHeader>
                        <TableRow>
                          <TableHead>Job ID</TableHead>
                          <TableHead>Machine</TableHead>
                          <TableHead>Product</TableHead>
                          <TableHead>Operator</TableHead>
                          <TableHead>Progress</TableHead>
                          <TableHead>Status</TableHead>
                          <TableHead>QC Data</TableHead>
                          <TableHead>Actions</TableHead>
                        </TableRow>
                      </TableHeader>
                      <TableBody>
                        {filteredJobs.map((job) => (
                          <TableRow key={job.id}>
                            <TableCell className="font-mono">{job.jobId}</TableCell>
                            <TableCell>{job.machineNo}</TableCell>
                            <TableCell>
                              <div className="flex items-center gap-2">
                                <img src={job.productImage} alt="" className="w-8 h-8 rounded object-cover" />
                                <span className="truncate max-w-32">{job.productName}</span>
                              </div>
                            </TableCell>
                            <TableCell>{job.operatorName}</TableCell>
                            <TableCell>
                              <div className="w-24">
                                <Progress value={(job.completedQty / job.targetQty) * 100} className="h-2" />
                                <p className="text-xs text-slate-500 mt-1">
                                  {Math.round((job.completedQty / job.targetQty) * 100)}%
                                </p>
                              </div>
                            </TableCell>
                            <TableCell>
                              <Badge className={`${getStatusColor(job.status)} text-white`}>
                                {getStatusIcon(job.status)} {job.status}
                              </Badge>
                            </TableCell>
                            <TableCell>
                              <div className="text-xs">
                                <div>✅ {job.goodQty}</div>
                                <div>❌ {job.defectQty}</div>
                                <div>🗑 {job.scrapQty}</div>
                              </div>
                            </TableCell>
                            <TableCell>
                              <div className="flex gap-1">
                                <Button size="sm" variant="outline">
                                  <Edit className="h-4 w-4" />
                                </Button>
                                <Button 
                                  size="sm" 
                                  disabled={job.status !== 'completed'}
                                  className={job.status === 'completed' ? 'bg-blue-600 hover:bg-blue-700' : ''}
                                >
                                  <Package className="h-4 w-4" />
                                </Button>
                              </div>
                            </TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>
                  </CardContent>
                </Card>
              )}

              {/* Batch Log Tables */}
              <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                {/* Operator Log */}
                <Card>
                  <CardHeader>
                    <CardTitle className="text-lg">Operator Log History | အော်ပရေတာမှတ်တမ်း</CardTitle>
                  </CardHeader>
                  <CardContent>
                    <Table>
                      <TableHeader>
                        <TableRow>
                          <TableHead>Timestamp</TableHead>
                          <TableHead>Operator</TableHead>
                          <TableHead>Qty Entered</TableHead>
                        </TableRow>
                      </TableHeader>
                      <TableBody>
                        {operatorLogs.map((log, index) => (
                          <TableRow key={index}>
                            <TableCell className="text-xs font-mono">{log.timestamp}</TableCell>
                            <TableCell>{log.operator}</TableCell>
                            <TableCell className="font-mono">{log.qtyEntered}</TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>
                  </CardContent>
                </Card>

                {/* QC Log */}
                <Card>
                  <CardHeader>
                    <CardTitle className="text-lg">QC Log History | QC မှတ်တမ်း</CardTitle>
                  </CardHeader>
                  <CardContent>
                    <Table>
                      <TableHeader>
                        <TableRow>
                          <TableHead>Timestamp</TableHead>
                          <TableHead>QC Staff</TableHead>
                          <TableHead>Good/Defect/Scrap</TableHead>
                        </TableRow>
                      </TableHeader>
                      <TableBody>
                        {qcLogs.map((log, index) => (
                          <TableRow key={index}>
                            <TableCell className="text-xs font-mono">{log.timestamp}</TableCell>
                            <TableCell>{log.qcStaff}</TableCell>
                            <TableCell className="font-mono text-xs">
                              <div className="flex gap-2">
                                <span className="text-green-600">✅{log.good}</span>
                                <span className="text-red-600">❌{log.defect}</span>
                                <span className="text-slate-600">🗑{log.scrap}</span>
                              </div>
                            </TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>
                  </CardContent>
                </Card>
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  };

  // Main render function
  const renderCurrentView = () => {
    switch (currentPage) {
      case 'qc-dashboard':
        return renderQCDashboard();
      case 'raw-material-qc':
        return renderRawMaterialQC();
      case 'in-process-qc':
        return renderInProcessQC();
      case 'finished-goods-qc':
        return renderFinishedGoodsQC();
      case 'qc-reports':
        return renderQCReports();
      case 'qc-settings':
        return renderQCSettings();
      default:
        return renderQCDashboard();
    }
  };

  // Get page title and subtitle
  const getPageInfo = () => {
    switch (currentPage) {
      case 'qc-dashboard':
        return {
          title: 'QC Dashboard | အရည်အသွေးဒက်ရှ်ဘုတ်',
          subtitle: 'Real-time quality control monitoring and inspection queue management',
          subtitleMM: 'အချိန်နှင့်တပြေးညီ အရည်အသွေးထိန်းချုပ်မှုစောင့်ကြည့်ခြင်းနှင့် စစ်ဆေးမှုတန်းစီစီမံခန့်ခွဲမှု'
        };
      case 'raw-material-qc':
        return {
          title: 'Raw Material QC (After Issue) | ကုန်ကြမ်း QC (ထုတ်ယူမှုနောက်)',
          subtitle: 'Quality control inspection for raw materials after inventory issue, before production',
          subtitleMM: 'ကုန်ကြမ်းများအတွက် စတော့ခ်ထုတ်ယူမှုနောက်၊ ထုတ်လုပ်မှုမတိုင်မီ အရည်အသွေးစစ်ဆေးမှု'
        };
      case 'in-process-qc':
        return {
          title: 'In-Process QC | လုပ်ငန်းစဉ်အတွင်း QC',
          subtitle: 'Quality control monitoring and inspection for running jobs during production',
          subtitleMM: 'ထုတ်လုပ်မှုအတွင်း လုပ်ငန်းများအတွက် အရည်အသွေးထိန်းချုပ်မှုစောင့်ကြည့်ခြင်းနှင့် စစ်ဆေးမှု'
        };
      case 'finished-goods-qc':
        return {
          title: 'Finished Goods QC (Before QR Print) | ပြီးစီးထုတ်ကုန် QC (QR ပုံနှိပ်ခြင်းမတိုင်မီ)',
          subtitle: 'Quality control inspection for finished goods after production, before QR code printing',
          subtitleMM: 'ပြီးစီးထုတ်ကုန်များအတွက် ထုတ်လုပ်မှုနောက်၊ QR ကုဒ်ပုံနှိပ်ခြင်းမတိုင်မီ အရည်အသွေးစစ်ဆေးမှု'
        };
      case 'qc-reports':
        return {
          title: 'QC Reports | အရည်အသွေးအစီရင်ခံစာများ',
          subtitle: 'Comprehensive quality control reports and traceability analytics',
          subtitleMM: 'ပြည့်စုံသော အရည်အသွေးထိန်းချုပ်မှုအစီရင်ခံစာများနှင့် ခြေရာခံနိုင်မှုခွဲခြမ်းစိတ်ဖြာမှု'
        };
      case 'qc-settings':
        return {
          title: 'QC Settings | အရည်အသွေးထိန်းချုပ်မှု ဆက်တင်များ',
          subtitle: 'Quality control master data and system configuration',
          subtitleMM: 'အရည်အသွေးထိန်းချုပ်မှု မူလဒေတာနှင့် စနစ်ဖွဲ့စည်းပုံ သတ်မှတ်ခြင်း'
        };
      default:
        return {
          title: 'QC (Quality Control) | အရည်အသွေးထိန်းချုပ်မှု',
          subtitle: 'Comprehensive quality control management system',
          subtitleMM: 'ပြည့်စုံသော အရည်အသွေးထိန်းချုပ်မှုစနစ်'
        };
    }
  };

  const pageInfo = getPageInfo();

  return (
    <div className="p-6 bg-slate-50 min-h-screen">
      <div className="max-w-7xl mx-auto">
        {/* Header */}
        <div className="mb-6">
          <h1 className="text-2xl font-semibold text-slate-900 mb-2">
            {pageInfo.title}
          </h1>
          <p className="text-slate-600">
            {pageInfo.subtitle}
          </p>
          <p className="text-sm text-slate-500 mt-1">
            {pageInfo.subtitleMM}
          </p>
        </div>

        {/* Current Page Content */}
        {renderCurrentView()}

        {/* Dialogs */}
        {renderRMQCDialog()}
        {renderFGQCDialog()}
        {renderInProcessQCModal()}
        {renderAddReasonDialog()}
      </div>
    </div>
  );
}