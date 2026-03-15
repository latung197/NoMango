import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Textarea } from './ui/textarea';
import { Checkbox } from './ui/checkbox';
import { RadioGroup, RadioGroupItem } from './ui/radio-group';
import { Separator } from './ui/separator';
import { Alert, AlertDescription } from './ui/alert';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { toast } from 'sonner@2.0.3';
import { 
  CheckCircle,
  XCircle,
  QrCode,
  Tag,
  Printer,
  Wifi,
  WifiOff,
  Eye,
  RefreshCw,
  AlertTriangle,
  Settings,
  Clock,
  User,
  Factory,
  Package,
  Shield,
  Lock,
  Unlock,
  ChevronRight
} from 'lucide-react';

// Mock data for jobs list
const mockJobsList = [
  {
    planId: 'PLAN20250907-001',
    jobId: 'JOB20250907-001',
    product: '2011 - Plastic Bottle 500ml',
    machine: 'INJ-M001',
    batchQty: 10000,
    operator: 'Ko Thant (OP001)',
    targetReached: true,
    qcStatus: 'approved', // approved, pending, failed
    productionStatus: 'released', // released, pending, in-progress
    qcOfficer: 'Ma Hnin (QC02)',
    qcTimestamp: '2025-09-07 14:30',
    productionOfficer: 'Ko Zaw (SUP01)',
    productionTimestamp: '2025-09-07 15:00',
    preferredOutput: 'QR',
    completedQty: 10000,
    goodQty: 9850,
    defectQty: 100,
    scrapQty: 50,
    outputStatus: 'pending', // pending, qr-printed, rfid-issued, completed
    qrId: null,
    rfidUid: null
  },
  {
    planId: 'PLAN20250907-002',
    jobId: 'JOB20250907-002',
    product: '2013 - Plastic Cup 250ml',
    machine: 'INJ-M002',
    batchQty: 8000,
    operator: 'Ma Hla (OP002)',
    targetReached: true,
    qcStatus: 'approved',
    productionStatus: 'released',
    qcOfficer: 'Ko Min (QC01)',
    qcTimestamp: '2025-09-07 13:45',
    productionOfficer: 'Ma Thin (SUP02)',
    productionTimestamp: '2025-09-07 14:20',
    preferredOutput: 'RFID',
    completedQty: 8000,
    goodQty: 7850,
    defectQty: 120,
    scrapQty: 30,
    outputStatus: 'qr-printed',
    qrId: 'QR-JOB20250907-002-001',
    rfidUid: null
  },
  {
    planId: 'PLAN20250907-003',
    jobId: 'JOB20250907-003',
    product: '2015 - Plastic Container 1L',
    machine: 'INJ-M003',
    batchQty: 5000,
    operator: 'Ko Aung (OP003)',
    targetReached: true,
    qcStatus: 'pending',
    productionStatus: 'in-progress',
    qcOfficer: null,
    qcTimestamp: null,
    productionOfficer: null,
    productionTimestamp: null,
    preferredOutput: 'QR',
    completedQty: 5000,
    goodQty: 4900,
    defectQty: 80,
    scrapQty: 20,
    outputStatus: 'pending',
    qrId: null,
    rfidUid: null
  },
  {
    planId: 'PLAN20250907-004',
    jobId: 'JOB20250907-004',
    product: '2017 - Plastic Tray 300x200mm',
    machine: 'INJ-M004',
    batchQty: 3000,
    operator: 'Ma Su (OP004)',
    targetReached: true,
    qcStatus: 'approved',
    productionStatus: 'pending',
    qcOfficer: 'Ma Hnin (QC02)',
    qcTimestamp: '2025-09-07 12:30',
    productionOfficer: null,
    productionTimestamp: null,
    preferredOutput: 'RFID',
    completedQty: 3000,
    goodQty: 2950,
    defectQty: 40,
    scrapQty: 10,
    outputStatus: 'pending',
    qrId: null,
    rfidUid: null
  },
  {
    planId: 'PLAN20250907-005',
    jobId: 'JOB20250907-005',
    product: '2019 - Plastic Lid 100mm',
    machine: 'INJ-M005',
    batchQty: 12000,
    operator: 'Ko Htun (OP005)',
    targetReached: true,
    qcStatus: 'approved',
    productionStatus: 'released',
    qcOfficer: 'Ko Min (QC01)',
    qcTimestamp: '2025-09-07 11:15',
    productionOfficer: 'Ko Zaw (SUP01)',
    productionTimestamp: '2025-09-07 11:45',
    preferredOutput: 'QR',
    completedQty: 12000,
    goodQty: 11800,
    defectQty: 150,
    scrapQty: 50,
    outputStatus: 'completed',
    qrId: 'QR-JOB20250907-005-001',
    rfidUid: 'UID-JOB20250907-005-A1B2C3'
  }
];

// Mock audit log data
const mockAuditLog = [
  { timestamp: '2025-09-07 14:30', event: 'QC approved by Ma Hnin (QC02)', type: 'success' },
  { timestamp: '2025-09-07 14:25', event: 'Target quantity reached: 10,000 units', type: 'info' },
  { timestamp: '2025-09-07 14:20', event: 'Batch production completed', type: 'info' },
];

export function SupervisorScreen() {
  const [jobsList, setJobsList] = useState(mockJobsList);
  const [selectedJobId, setSelectedJobId] = useState<string | null>(null);
  const [selectedMethod, setSelectedMethod] = useState<'QR' | 'RFID' | null>(null);
  const [overrideReason, setOverrideReason] = useState('');
  const [showOverrideInput, setShowOverrideInput] = useState(false);
  const [auditLog, setAuditLog] = useState(mockAuditLog);
  
  // QR Label states
  const [labelTemplate, setLabelTemplate] = useState('standard-70x50');
  const [selectedPrinter, setSelectedPrinter] = useState('printer-01');
  const [copies, setCopies] = useState(1);
  const [qrPrinted, setQrPrinted] = useState(false);
  const [qrId, setQrId] = useState('');

  // RFID states
  const [selectedEncoder, setSelectedEncoder] = useState('encoder-01');
  const [lockTag, setLockTag] = useState(false);
  const [lockPassword, setLockPassword] = useState('');
  const [rfidEncoded, setRfidEncoded] = useState(false);
  const [rfidUid, setRfidUid] = useState('');
  const [verificationResult, setVerificationResult] = useState<any>(null);

  // Device status
  const [printerStatus] = useState('online'); // online, offline
  const [encoderStatus] = useState('connected'); // connected, not-connected

  // Get selected job data
  const selectedJob = jobsList.find(job => job.jobId === selectedJobId);
  const isReady = selectedJob && selectedJob.targetReached && selectedJob.qcStatus === 'approved' && selectedJob.productionStatus === 'released';

  const handleJobSelection = (jobId: string) => {
    setSelectedJobId(jobId);
    setSelectedMethod(null);
    setShowOverrideInput(false);
    setOverrideReason('');
    setQrPrinted(false);
    setRfidEncoded(false);
    setVerificationResult(null);
    
    // Add to audit log
    const timestamp = new Date().toISOString().replace('T', ' ').substring(0, 19);
    const logEntry = {
      timestamp,
      event: `Job selected: ${jobId}`,
      type: 'info' as const
    };
    setAuditLog(prev => [logEntry, ...prev]);
  };

  const handleMethodSelection = (method: 'QR' | 'RFID') => {
    if (!selectedJob) return;
    
    setSelectedMethod(method);
    setShowOverrideInput(method !== selectedJob.preferredOutput);
    
    // Add to audit log
    const timestamp = new Date().toISOString().replace('T', ' ').substring(0, 19);
    const logEntry = {
      timestamp,
      event: `Output method selected: ${method}${method !== selectedJob.preferredOutput ? ' (Override)' : ''} for ${selectedJob.jobId}`,
      type: 'info' as const
    };
    setAuditLog(prev => [logEntry, ...prev]);
  };

  const handlePrintQR = () => {
    if (!selectedMethod || selectedMethod !== 'QR' || !selectedJob) return;
    
    const newQrId = `QR-${selectedJob.jobId}-${Date.now()}`;
    setQrId(newQrId);
    setQrPrinted(true);
    
    // Update jobs list
    setJobsList(prev => prev.map(job => 
      job.jobId === selectedJob.jobId 
        ? { ...job, outputStatus: 'qr-printed', qrId: newQrId }
        : job
    ));
    
    // Add to audit log
    const timestamp = new Date().toISOString().replace('T', ' ').substring(0, 19);
    const logEntry = {
      timestamp,
      event: `QR Label printed successfully. QR ID: ${newQrId} for ${selectedJob.jobId}`,
      type: 'success' as const
    };
    setAuditLog(prev => [logEntry, ...prev]);
    
    toast.success(`Printed successfully. QR ID: ${newQrId} recorded.`);
  };

  const handleReprintQR = () => {
    // Add to audit log
    const timestamp = new Date().toISOString().replace('T', ' ').substring(0, 19);
    const logEntry = {
      timestamp,
      event: `QR Label reprinted. QR ID: ${qrId}`,
      type: 'info' as const
    };
    setAuditLog(prev => [logEntry, ...prev]);
    
    toast.success(`Reprinted QR ID: ${qrId}`);
  };

  const handleWriteRFID = () => {
    if (!selectedMethod || selectedMethod !== 'RFID' || !selectedJob) return;
    
    const newUid = `UID-${selectedJob.jobId}-${Date.now().toString(16).toUpperCase()}`;
    setRfidUid(newUid);
    setRfidEncoded(true);
    
    // Update jobs list
    const newStatus = selectedJob.outputStatus === 'qr-printed' ? 'completed' : 'rfid-issued';
    setJobsList(prev => prev.map(job => 
      job.jobId === selectedJob.jobId 
        ? { ...job, outputStatus: newStatus, rfidUid: newUid }
        : job
    ));
    
    // Mock verification result
    setVerificationResult({
      uid: newUid,
      crc: 'CRC-VALID',
      success: true
    });
    
    // Add to audit log
    const timestamp = new Date().toISOString().replace('T', ' ').substring(0, 19);
    const logEntry = {
      timestamp,
      event: `RFID encoded & verified. Tag UID: ${newUid} for ${selectedJob.jobId}`,
      type: 'success' as const
    };
    setAuditLog(prev => [logEntry, ...prev]);
    
    toast.success(`RFID encoded & verified. Tag UID: ${newUid} recorded.`);
  };

  const handleFinalizeBatch = () => {
    if (!selectedMethod || (!qrPrinted && !rfidEncoded) || !selectedJob) return;
    
    // Update jobs list to completed status
    setJobsList(prev => prev.map(job => 
      job.jobId === selectedJob.jobId 
        ? { ...job, outputStatus: 'completed' }
        : job
    ));
    
    // Add to audit log
    const timestamp = new Date().toISOString().replace('T', ' ').substring(0, 19);
    const logEntry = {
      timestamp,
      event: `Batch finalized with ${selectedMethod} output for ${selectedJob.jobId}`,
      type: 'success' as const
    };
    setAuditLog(prev => [logEntry, ...prev]);
    
    toast.success(`Batch ${selectedJob.jobId} finalized successfully!`);
    
    // Clear selection to allow next job
    setSelectedJobId(null);
    setSelectedMethod(null);
    setQrPrinted(false);
    setRfidEncoded(false);
    setVerificationResult(null);
  };

  const canFinalize = selectedMethod && ((selectedMethod === 'QR' && qrPrinted) || (selectedMethod === 'RFID' && rfidEncoded));

  return (
    <div className="p-6 bg-slate-50 min-h-screen">
      <div className="max-w-7xl mx-auto space-y-6">
        {/* Header */}
        <div className="mb-6">
          <h1 className="text-3xl font-semibold text-slate-900 mb-2">
            Supervisor – Finalize Batch | ကြီးကြပ်ရေးမှူး – အသုံးအစုံအတည်ပြုခြင်း
          </h1>
          <p className="text-slate-600">
            Choose output method and finalize batch after QC approval
          </p>
          <p className="text-sm text-slate-500 mt-1">
            QC အတည်ပြုပြီးနောက် အထွက်နည်းလမ်းရွေးချယ်ပြီး အသုံးအစုံကို အတည်ပြုပါ
          </p>
        </div>

        {/* Jobs List Table */}
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Package className="h-5 w-5 text-blue-600" />
              <div>
                <div>Jobs List - Select Job to Process | အလုပ်စာရင်း - လုပ်ဆောင်ရန်အလုပ်ရွေးပါ</div>
              </div>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <table className="w-full border-collapse">
                <thead>
                  <tr className="border-b border-slate-200">
                    <th className="text-left p-3 font-medium text-slate-700">Select</th>
                    <th className="text-left p-3 font-medium text-slate-700">Job ID</th>
                    <th className="text-left p-3 font-medium text-slate-700">Product | ထုတ်ကုန်</th>
                    <th className="text-left p-3 font-medium text-slate-700">Machine</th>
                    <th className="text-left p-3 font-medium text-slate-700">Operator</th>
                    <th className="text-left p-3 font-medium text-slate-700">QC Status</th>
                    <th className="text-left p-3 font-medium text-slate-700">Production Status</th>
                    <th className="text-left p-3 font-medium text-slate-700">Output Status</th>
                    <th className="text-left p-3 font-medium text-slate-700">Preferred Method</th>
                  </tr>
                </thead>
                <tbody>
                  {jobsList.map((job) => {
                    const canProcess = job.qcStatus === 'approved' && job.productionStatus === 'released';
                    const isSelected = selectedJobId === job.jobId;
                    
                    return (
                      <tr 
                        key={job.jobId} 
                        className={`border-b border-slate-100 hover:bg-slate-50 ${isSelected ? 'bg-blue-50 border-blue-200' : ''}`}
                      >
                        <td className="p-3">
                          <Button
                            variant={isSelected ? "default" : "outline"}
                            size="sm"
                            onClick={() => handleJobSelection(job.jobId)}
                            disabled={!canProcess}
                            className={isSelected ? 'bg-blue-600 hover:bg-blue-700' : ''}
                          >
                            {isSelected ? 'Selected' : 'Select'}
                          </Button>
                        </td>
                        <td className="p-3 font-medium">{job.jobId}</td>
                        <td className="p-3">
                          <div>
                            <div className="font-medium">{job.product}</div>
                            <div className="text-sm text-slate-500">Qty: {job.goodQty.toLocaleString()}</div>
                          </div>
                        </td>
                        <td className="p-3">
                          <div className="flex items-center gap-1">
                            <Factory className="h-3 w-3 text-slate-400" />
                            {job.machine}
                          </div>
                        </td>
                        <td className="p-3">
                          <div className="flex items-center gap-1">
                            <User className="h-3 w-3 text-slate-400" />
                            {job.operator}
                          </div>
                        </td>
                        <td className="p-3">
                          <Badge className={
                            job.qcStatus === 'approved' ? 'bg-green-100 text-green-800' :
                            job.qcStatus === 'pending' ? 'bg-yellow-100 text-yellow-800' :
                            'bg-red-100 text-red-800'
                          }>
                            {job.qcStatus === 'approved' ? (
                              <>
                                <CheckCircle className="h-3 w-3 mr-1" />
                                Approved | အတည်ပြုပြီး
                              </>
                            ) : job.qcStatus === 'pending' ? (
                              <>
                                <Clock className="h-3 w-3 mr-1" />
                                Pending | ဆိုင်းငံ့
                              </>
                            ) : (
                              <>
                                <XCircle className="h-3 w-3 mr-1" />
                                Failed | ကျရှုံး
                              </>
                            )}
                          </Badge>
                          {job.qcOfficer && (
                            <div className="text-xs text-slate-500 mt-1">{job.qcOfficer}</div>
                          )}
                        </td>
                        <td className="p-3">
                          <Badge className={
                            job.productionStatus === 'released' ? 'bg-green-100 text-green-800' :
                            job.productionStatus === 'pending' ? 'bg-yellow-100 text-yellow-800' :
                            'bg-blue-100 text-blue-800'
                          }>
                            {job.productionStatus === 'released' ? (
                              <>
                                <CheckCircle className="h-3 w-3 mr-1" />
                                Released | ထုတ်ပေး
                              </>
                            ) : job.productionStatus === 'pending' ? (
                              <>
                                <Clock className="h-3 w-3 mr-1" />
                                Pending | ဆိုင်းငံ့
                              </>
                            ) : (
                              <>
                                <Package className="h-3 w-3 mr-1" />
                                In Progress | လုပ်ဆောင်နေ
                              </>
                            )}
                          </Badge>
                          {job.productionOfficer && (
                            <div className="text-xs text-slate-500 mt-1">{job.productionOfficer}</div>
                          )}
                        </td>
                        <td className="p-3">
                          <Badge className={
                            job.outputStatus === 'completed' ? 'bg-green-100 text-green-800' :
                            job.outputStatus === 'qr-printed' ? 'bg-blue-100 text-blue-800' :
                            job.outputStatus === 'rfid-issued' ? 'bg-purple-100 text-purple-800' :
                            'bg-slate-100 text-slate-800'
                          }>
                            {job.outputStatus === 'completed' ? (
                              <>
                                <CheckCircle className="h-3 w-3 mr-1" />
                                Completed | ပြီးစီး
                              </>
                            ) : job.outputStatus === 'qr-printed' ? (
                              <>
                                <QrCode className="h-3 w-3 mr-1" />
                                QR Printed | QR ပုံနှိပ်ပြီး
                              </>
                            ) : job.outputStatus === 'rfid-issued' ? (
                              <>
                                <Tag className="h-3 w-3 mr-1" />
                                RFID Issued | RFID ထုတ်ပေးပြီး
                              </>
                            ) : (
                              <>
                                <Clock className="h-3 w-3 mr-1" />
                                Pending | ဆိုင်းငံ့
                              </>
                            )}
                          </Badge>
                        </td>
                        <td className="p-3">
                          <div className="flex items-center gap-1">
                            {job.preferredOutput === 'QR' ? (
                              <QrCode className="h-4 w-4 text-blue-600" />
                            ) : (
                              <Tag className="h-4 w-4 text-green-600" />
                            )}
                            <span className="font-medium">{job.preferredOutput}</span>
                          </div>
                        </td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            </div>
          </CardContent>
        </Card>

        {/* Pre-condition Warning */}
        {!selectedJob && (
          <Alert className="border-blue-200 bg-blue-50">
            <Package className="h-4 w-4 text-blue-600" />
            <AlertDescription className="text-blue-800">
              <strong>Select a job to proceed</strong> - Please select a job from the list above to begin the finalization process.
              <br />
              <span className="text-sm">ဆက်လက်လုပ်ဆောင်ရန် အလုပ်တစ်ခုရွေးပါ - အတည်ပြုခြင်းလုပ်ငန်းစဉ်စတင်ရန် အထက်စာရင်းမှ အလုပ်တစ်ခုရွေးချယ်ပါ</span>
            </AlertDescription>
          </Alert>
        )}
        
        {selectedJob && !isReady && (
          <Alert className="border-yellow-200 bg-yellow-50">
            <AlertTriangle className="h-4 w-4 text-yellow-600" />
            <AlertDescription className="text-yellow-800">
              <strong>Job not ready for processing</strong> - This job requires both QC approval and Production release before proceeding.
              <br />
              <span className="text-sm">အလုပ်ဆောင်ရန်အဆင်သင့်မဖြစ်သေး - ဤအလုပ်အတွက် QC အတည်ပြုချက်နှင့် ထုတ်လုပ်မှုလွှတ်ပေးမှုနှစ်ခုလုံးလိုအပ်သည်</span>
            </AlertDescription>
          </Alert>
        )}

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          {/* Left Column - Job Summary & Method Selection */}
          <div className="lg:col-span-1 space-y-6">
            {/* Job Summary */}
            <Card className="border-0 shadow-sm">
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Package className="h-5 w-5 text-blue-600" />
                  <div>
                    <div>Job Summary</div>
                    <div className="text-sm text-slate-500">အလုပ်အနှစ်ချုပ်</div>
                  </div>
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                {selectedJob ? (
                  <>
                    <div className="grid grid-cols-2 gap-4 text-sm">
                      <div>
                        <Label className="text-slate-600">Plan ID:</Label>
                        <p className="font-medium">{selectedJob.planId}</p>
                      </div>
                      <div>
                        <Label className="text-slate-600">Job ID:</Label>
                        <p className="font-medium">{selectedJob.jobId}</p>
                      </div>
                      <div className="col-span-2">
                        <Label className="text-slate-600">Product:</Label>
                        <p className="font-medium">{selectedJob.product}</p>
                      </div>
                      <div>
                        <Label className="text-slate-600">Machine:</Label>
                        <div className="flex items-center gap-1">
                          <Factory className="h-3 w-3 text-slate-400" />
                          <span className="font-medium">{selectedJob.machine}</span>
                        </div>
                      </div>
                      <div>
                        <Label className="text-slate-600">Batch Qty:</Label>
                        <p className="font-medium text-blue-600">{selectedJob.batchQty.toLocaleString()}</p>
                      </div>
                      <div className="col-span-2">
                        <Label className="text-slate-600">Operator:</Label>
                        <div className="flex items-center gap-1">
                          <User className="h-3 w-3 text-slate-400" />
                          <span className="font-medium">{selectedJob.operator}</span>
                        </div>
                      </div>
                    </div>

                    <Separator />

                    <div className="space-y-2">
                      <div className="flex items-center justify-between">
                        <Label className="text-slate-600">QC Status:</Label>
                        <div className="flex items-center gap-2">
                          {selectedJob.qcStatus === 'approved' ? (
                            <>
                              <CheckCircle className="h-4 w-4 text-green-600" />
                              <Badge className="bg-green-100 text-green-800">Approved | အတည်ပြုပြီး</Badge>
                            </>
                          ) : (
                            <>
                              <XCircle className="h-4 w-4 text-red-600" />
                              <Badge className="bg-red-100 text-red-800">Pending | ဆိုင်းငံ့</Badge>
                            </>
                          )}
                        </div>
                      </div>
                      
                      <div className="flex items-center justify-between">
                        <Label className="text-slate-600">Production Status:</Label>
                        <div className="flex items-center gap-2">
                          {selectedJob.productionStatus === 'released' ? (
                            <>
                              <CheckCircle className="h-4 w-4 text-green-600" />
                              <Badge className="bg-green-100 text-green-800">Released | ထုတ်ပေး</Badge>
                            </>
                          ) : (
                            <>
                              <Clock className="h-4 w-4 text-yellow-600" />
                              <Badge className="bg-yellow-100 text-yellow-800">Pending | ဆိုင်းငံ့</Badge>
                            </>
                          )}
                        </div>
                      </div>
                    </div>
                  </>
                ) : (
                  <div className="text-center py-8 text-slate-500">
                    <Package className="h-12 w-12 mx-auto mb-4 text-slate-300" />
                    <p>No job selected</p>
                    <p className="text-sm">အလုပ်မရွေးထားပါ</p>
                  </div>
                )}
              </CardContent>
            </Card>

            {/* Step 1 - Output Method Selection */}
            <Card className="border-0 shadow-sm">
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Settings className="h-5 w-5 text-blue-600" />
                  <div>
                    <div>Step 1 — Output Method</div>
                    <div className="text-sm text-slate-500">အဆင့် ၁ — အထွက်နည်းလမ်း</div>
                  </div>
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                {/* Suggestion Chip */}
                {selectedJob && (
                  <div className="bg-blue-50 border border-blue-200 rounded-lg p-3">
                    <div className="flex items-center gap-2 text-blue-800 text-sm">
                      <div className="w-2 h-2 rounded-full bg-blue-400"></div>
                      <span>Suggested by Product Master: <strong>{selectedJob.preferredOutput}</strong></span>
                    </div>
                  </div>
                )}

                {/* Method Selection */}
                <RadioGroup 
                  value={selectedMethod || ''} 
                  onValueChange={(value) => handleMethodSelection(value as 'QR' | 'RFID')}
                  disabled={!isReady}
                  className="space-y-3"
                >
                  <div className="flex items-center space-x-3 p-4 border-2 border-slate-200 rounded-lg hover:border-blue-300 transition-colors">
                    <RadioGroupItem value="QR" id="qr" className="text-blue-600" />
                    <Label htmlFor="qr" className="flex items-center gap-2 cursor-pointer flex-1">
                      <QrCode className="h-5 w-5 text-blue-600" />
                      <div>
                        <div className="font-medium">QR Label</div>
                        <div className="text-sm text-slate-500">QR တံဆိပ်</div>
                      </div>
                    </Label>
                  </div>
                  
                  <div className="flex items-center space-x-3 p-4 border-2 border-slate-200 rounded-lg hover:border-green-300 transition-colors">
                    <RadioGroupItem value="RFID" id="rfid" className="text-green-600" />
                    <Label htmlFor="rfid" className="flex items-center gap-2 cursor-pointer flex-1">
                      <Tag className="h-5 w-5 text-green-600" />
                      <div>
                        <div className="font-medium">RFID Tag</div>
                        <div className="text-sm text-slate-500">RFID တံဆိပ်</div>
                      </div>
                    </Label>
                  </div>
                </RadioGroup>

                {/* Override Reason */}
                {showOverrideInput && selectedMethod && (
                  <div className="bg-orange-50 border border-orange-200 rounded-lg p-3 space-y-2">
                    <Label className="text-orange-800">Override Reason | အစားထိုးအကြောင်းရင်း:</Label>
                    <Textarea
                      placeholder="Explain why you're overriding the suggested method..."
                      value={overrideReason}
                      onChange={(e) => setOverrideReason(e.target.value)}
                      className="bg-white"
                    />
                  </div>
                )}
              </CardContent>
            </Card>
          </div>

          {/* Center Column - Main Controls */}
          <div className="lg:col-span-1 space-y-6">
            {selectedMethod === 'QR' && (
              <Card className="border-0 shadow-sm">
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <QrCode className="h-5 w-5 text-blue-600" />
                    <div>
                      <div>QR Label Configuration</div>
                      <div className="text-sm text-slate-500">QR တံဆိပ်ပြင်ဆင်မှု</div>
                    </div>
                  </CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  {/* Label Template */}
                  <div>
                    <Label>Label Template | တံဆိပ်ပုံစံ:</Label>
                    <Select value={labelTemplate} onValueChange={setLabelTemplate}>
                      <SelectTrigger className="mt-1">
                        <SelectValue />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="standard-70x50">Standard 70×50mm</SelectItem>
                        <SelectItem value="small-40x30">Small 40×30mm</SelectItem>
                        <SelectItem value="large-100x60">Large 100×60mm</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>

                  {/* Auto-filled Fields */}
                  {selectedJob && (
                    <div className="bg-slate-50 rounded-lg p-3 space-y-2">
                      <Label className="text-sm text-slate-600">Auto-filled Fields | အလိုအလျောက်ဖြည့်သွင်းသောနေရာများ:</Label>
                      <div className="grid grid-cols-2 gap-2 text-xs">
                        <div><strong>Product:</strong> {selectedJob.product}</div>
                        <div><strong>Machine:</strong> {selectedJob.machine}</div>
                        <div><strong>Operator:</strong> {selectedJob.operator}</div>
                        <div><strong>Date/Time:</strong> {new Date().toLocaleString()}</div>
                        <div><strong>Quantity:</strong> {selectedJob.goodQty.toLocaleString()}</div>
                        <div><strong>Job ID:</strong> {selectedJob.jobId}</div>
                      </div>
                    </div>
                  )}

                  {/* Printer Selection */}
                  <div>
                    <Label>Printer | ပုံနှိပ်စက်:</Label>
                    <div className="flex items-center gap-2 mt-1">
                      <Select value={selectedPrinter} onValueChange={setSelectedPrinter}>
                        <SelectTrigger className="flex-1">
                          <SelectValue />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="printer-01">Production Line Printer 01</SelectItem>
                          <SelectItem value="printer-02">Production Line Printer 02</SelectItem>
                          <SelectItem value="printer-03">Office Printer 03</SelectItem>
                        </SelectContent>
                      </Select>
                      <Badge className={printerStatus === 'online' ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}>
                        {printerStatus === 'online' ? (
                          <>
                            <Wifi className="h-3 w-3 mr-1" />
                            Online
                          </>
                        ) : (
                          <>
                            <WifiOff className="h-3 w-3 mr-1" />
                            Offline
                          </>
                        )}
                      </Badge>
                    </div>
                  </div>

                  {/* Copies */}
                  <div>
                    <Label>Copies (labels per bag/box/pallet) | ကူးယူရေ:</Label>
                    <div className="flex items-center gap-2 mt-1">
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => setCopies(Math.max(1, copies - 1))}
                        disabled={copies <= 1}
                      >
                        -
                      </Button>
                      <Input
                        type="number"
                        value={copies}
                        onChange={(e) => setCopies(Math.max(1, parseInt(e.target.value) || 1))}
                        className="w-20 text-center"
                        min="1"
                      />
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => setCopies(copies + 1)}
                      >
                        +
                      </Button>
                    </div>
                  </div>

                  {/* Action Buttons */}
                  <div className="space-y-2 pt-4">
                    <Button
                      onClick={handlePrintQR}
                      disabled={printerStatus === 'offline' || !isReady}
                      className="w-full bg-blue-600 hover:bg-blue-700 h-12 text-lg"
                    >
                      <Printer className="h-5 w-5 mr-2" />
                      Print QR Label | QR တံဆိပ်ပုံနှိပ်ပါ
                    </Button>
                    
                    {qrPrinted && (
                      <Button
                        variant="outline"
                        onClick={handleReprintQR}
                        className="w-full h-10"
                      >
                        <RefreshCw className="h-4 w-4 mr-2" />
                        Reprint | ပြန်ပုံနှိပ်ပါ
                      </Button>
                    )}
                  </div>

                  {/* Success Message */}
                  {qrPrinted && (
                    <Alert className="border-green-200 bg-green-50">
                      <CheckCircle className="h-4 w-4 text-green-600" />
                      <AlertDescription className="text-green-800">
                        <strong>QR Label printed successfully!</strong>
                        <br />QR ID: <code className="bg-white px-1 rounded">{qrId}</code>
                      </AlertDescription>
                    </Alert>
                  )}
                </CardContent>
              </Card>
            )}

            {selectedMethod === 'RFID' && (
              <Card className="border-0 shadow-sm">
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <Tag className="h-5 w-5 text-green-600" />
                    <div>
                      <div>RFID Tag Configuration</div>
                      <div className="text-sm text-slate-500">RFID တံဆိပ်ပြင်ဆင်မှု</div>
                    </div>
                  </CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  {/* Encoder Device */}
                  <div>
                    <Label>Encoder Device | စာဝှက်စက်:</Label>
                    <div className="flex items-center gap-2 mt-1">
                      <Select value={selectedEncoder} onValueChange={setSelectedEncoder}>
                        <SelectTrigger className="flex-1">
                          <SelectValue />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="encoder-01">RFID Writer Station 01</SelectItem>
                          <SelectItem value="encoder-02">RFID Writer Station 02</SelectItem>
                          <SelectItem value="encoder-03">Mobile RFID Writer</SelectItem>
                        </SelectContent>
                      </Select>
                      <Badge className={encoderStatus === 'connected' ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}>
                        {encoderStatus === 'connected' ? (
                          <>
                            <Wifi className="h-3 w-3 mr-1" />
                            Connected
                          </>
                        ) : (
                          <>
                            <WifiOff className="h-3 w-3 mr-1" />
                            Not Connected
                          </>
                        )}
                      </Badge>
                    </div>
                  </div>

                  {/* Encoding Scheme */}
                  {selectedJob && (
                    <div>
                      <Label>Encoding Scheme | စာဝှက်ပုံစံ:</Label>
                      <div className="bg-slate-50 rounded-lg p-3 mt-1">
                        <div className="text-sm space-y-1">
                          <div><strong>EPC Format:</strong> {selectedJob.jobId} + Batch + {selectedJob.goodQty}</div>
                          <div className="text-slate-600">Job ID + Batch Info + Quantity</div>
                        </div>
                        
                        <Separator className="my-2" />
                        
                        <div className="space-y-2">
                          <Label className="text-sm">Optional User Memory | နောက်ထပ်မှတ်ဉာဏ်:</Label>
                          <div className="grid grid-cols-2 gap-2">
                            <Input placeholder="Lot Number" className="text-sm" />
                            <Input placeholder="Expiry Date" className="text-sm" />
                          </div>
                        </div>
                      </div>
                    </div>
                  )}

                  {/* Lock Tag Option */}
                  <div className="flex items-center space-x-2 p-3 bg-slate-50 rounded-lg">
                    <Checkbox
                      id="lock-tag"
                      checked={lockTag}
                      onCheckedChange={(checked) => setLockTag(checked as boolean)}
                    />
                    <Label htmlFor="lock-tag" className="flex items-center gap-2">
                      {lockTag ? <Lock className="h-4 w-4" /> : <Unlock className="h-4 w-4" />}
                      Lock Tag | တံဆိပ်သော့ခတ်ပါ
                    </Label>
                  </div>

                  {lockTag && (
                    <div>
                      <Label>Lock Password | သော့ခတ်စကားဝှက်:</Label>
                      <Input
                        type="password"
                        value={lockPassword}
                        onChange={(e) => setLockPassword(e.target.value)}
                        placeholder="Enter lock password"
                        className="mt-1"
                      />
                    </div>
                  )}

                  {/* Action Button */}
                  <div className="pt-4">
                    <Button
                      onClick={handleWriteRFID}
                      disabled={encoderStatus === 'not-connected' || !isReady}
                      className="w-full bg-green-600 hover:bg-green-700 h-12 text-lg"
                    >
                      <Shield className="h-5 w-5 mr-2" />
                      Write & Verify Tag | တံဆိပ်ရေးပြီးစစ်ဆေးပါ
                    </Button>
                  </div>

                  {/* Verification Result */}
                  {verificationResult && (
                    <Alert className="border-green-200 bg-green-50">
                      <CheckCircle className="h-4 w-4 text-green-600" />
                      <AlertDescription className="text-green-800">
                        <strong>RFID Tag Verification Result:</strong>
                        <div className="mt-1 space-y-1">
                          <div>✅ UID: <code className="bg-white px-1 rounded">{verificationResult.uid}</code></div>
                          <div>✅ CRC: <code className="bg-white px-1 rounded">{verificationResult.crc}</code></div>
                        </div>
                      </AlertDescription>
                    </Alert>
                  )}
                </CardContent>
              </Card>
            )}
          </div>

          {/* Right Column - Preview & Audit Log */}
          <div className="lg:col-span-1 space-y-6">
            {/* Preview Panel */}
            {selectedMethod && (
              <Card className="border-0 shadow-sm">
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <Eye className="h-5 w-5 text-slate-600" />
                    <div>
                      <div>Preview | အစမ်းကြည့်ရှုမှု</div>
                    </div>
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  {selectedMethod === 'QR' ? (
                    <div className="bg-white border-2 border-dashed border-slate-300 rounded-lg p-6 text-center">
                      <QrCode className="h-16 w-16 mx-auto text-slate-400 mb-4" />
                      <div className="text-sm text-slate-600 space-y-1">
                        <div><strong>QR Label Preview</strong></div>
                        {selectedJob ? (
                          <>
                            <div>{selectedJob.product}</div>
                            <div>{selectedJob.machine} | {selectedJob.operator}</div>
                            <div>Qty: {selectedJob.goodQty.toLocaleString()}</div>
                            <div className="text-xs">{selectedJob.jobId}</div>
                          </>
                        ) : (
                          <div className="text-slate-400">No job selected</div>
                        )}
                      </div>
                    </div>
                  ) : (
                    <div className="bg-white border-2 border-dashed border-slate-300 rounded-lg p-6 text-center">
                      <Tag className="h-16 w-16 mx-auto text-slate-400 mb-4" />
                      <div className="text-sm text-slate-600 space-y-1">
                        <div><strong>RFID Tag Preview</strong></div>
                        {selectedJob ? (
                          <>
                            <div>EPC: {selectedJob.jobId}-BATCH-{selectedJob.goodQty}</div>
                            <div>User Memory: Available</div>
                            <div className="text-xs">125kHz / 13.56MHz Compatible</div>
                          </>
                        ) : (
                          <div className="text-slate-400">No job selected</div>
                        )}
                      </div>
                    </div>
                  )}
                </CardContent>
              </Card>
            )}

            {/* Audit Log */}
            <Card className="border-0 shadow-sm">
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Clock className="h-5 w-5 text-slate-600" />
                  <div>
                    <div>Audit Log | စာရင်းစစ်မှတ်တမ်း</div>
                  </div>
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="space-y-3 max-h-64 overflow-y-auto">
                  {auditLog.map((entry, index) => (
                    <div key={index} className="flex items-start gap-3 text-sm">
                      <div className={`w-2 h-2 rounded-full mt-2 flex-shrink-0 ${
                        entry.type === 'success' ? 'bg-green-500' :
                        entry.type === 'error' ? 'bg-red-500' : 'bg-blue-500'
                      }`}></div>
                      <div className="flex-1">
                        <div className="text-slate-600 text-xs">{entry.timestamp}</div>
                        <div className="text-slate-900">{entry.event}</div>
                      </div>
                    </div>
                  ))}
                </div>
              </CardContent>
            </Card>
          </div>
        </div>

        {/* Footer Actions */}
        <Card className="border-0 shadow-sm bg-slate-100">
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div className="text-sm text-slate-600">
                {selectedJob ? (
                  <>Job Status: {canFinalize ? 'Ready to Finalize' : 'Waiting for Output Configuration'}</>
                ) : (
                  <>No job selected - Please select a job from the list above</>
                )}
              </div>
              <div className="flex items-center gap-4">
                <Button
                  variant="outline"
                  className="h-12 px-6"
                  onClick={() => {
                    // Clear current selection to allow selecting next job
                    setSelectedJobId(null);
                    setSelectedMethod(null);
                    setQrPrinted(false);
                    setRfidEncoded(false);
                    setVerificationResult(null);
                  }}
                  disabled={!selectedJob}
                >
                  Select Next Job | နောက်အလုပ်ရွေးပါ
                  <ChevronRight className="h-4 w-4 ml-2" />
                </Button>
                <Button
                  onClick={handleFinalizeBatch}
                  disabled={!canFinalize}
                  className="bg-green-600 hover:bg-green-700 h-12 px-8 text-lg"
                >
                  <CheckCircle className="h-5 w-5 mr-2" />
                  Finalize Job | အလုပ်အတည်ပြုပါ
                </Button>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}