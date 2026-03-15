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
  AlertTriangle,
  Package,
  FileX,
  Plus,
  Eye,
  CheckCircle,
  XCircle,
  Clock,
  RotateCcw,
  Truck,
  PackageX,
  ArrowRight
} from 'lucide-react';
import { toast } from 'sonner@2.0.3';

// Mock data for waste materials
const mockWasteRecords = [
  {
    id: 'WST-001',
    jobId: 'JOB20250903-001',
    product: '2011 - Plastic Bottle 500ml',
    machine: 'INJ-M001',
    operator: 'Ko Thant (OP001)',
    wasteType: 'Surface Waste',
    wasteCode: 'SW001',
    quantity: 25,
    unit: 'pcs',
    location: 'Machine Output',
    detectedBy: 'QC Inspector',
    reportedTime: '2025-09-08 10:30',
    severity: 'High',
    status: 'Under Investigation',
    photos: 2,
    notes: 'Multiple scratches on bottle surface, possibly due to mold wear'
  },
  {
    id: 'WST-002',
    jobId: 'JOB20250903-002',
    product: '2012 - Plastic Container 1L',
    machine: 'INJ-M002',
    operator: 'Ma Thandar (OP002)',
    wasteType: 'Dimensional Waste',
    wasteCode: 'DW001',
    quantity: 18,
    unit: 'pcs',
    location: 'QC Station',
    detectedBy: 'QC Inspector',
    reportedTime: '2025-09-08 14:15',
    severity: 'Medium',
    status: 'Confirmed',
    photos: 1,
    notes: 'Container height exceeded tolerance limit'
  },
  {
    id: 'WST-003',
    jobId: 'JOB20250903-003',
    product: '2013 - Plastic Cup 250ml',
    machine: 'INJ-M003',
    operator: 'Ko Aung (OP003)',
    wasteType: 'Material Waste',
    wasteCode: 'MW001',
    quantity: 35,
    unit: 'pcs',
    location: 'Production Line',
    detectedBy: 'Operator',
    reportedTime: '2025-09-08 16:45',
    severity: 'Low',
    status: 'Resolved',
    photos: 0,
    notes: 'Color variation due to material contamination - batch replaced'
  }
];

// Waste handling actions
const wasteActions = [
  { value: 'rework', label: 'Send for Rework', color: 'blue' },
  { value: 'scrap', label: 'Mark as Reject', color: 'red' },
  { value: 'quarantine', label: 'Quarantine', color: 'yellow' },
  { value: 'investigate', label: 'Further Investigation', color: 'purple' }
];

export function WasteManagement() {
  const [wasteRecords, setWasteRecords] = useState(mockWasteRecords);
  const [selectedRecord, setSelectedRecord] = useState<typeof mockWasteRecords[0] | null>(null);
  const [showDetails, setShowDetails] = useState(false);
  const [showNewWasteDialog, setShowNewWasteDialog] = useState(false);
  const [filterStatus, setFilterStatus] = useState('all');
  const [searchTerm, setSearchTerm] = useState('');

  // Filter waste records
  const filteredRecords = wasteRecords.filter(record => {
    const matchesStatus = filterStatus === 'all' || record.status.toLowerCase().includes(filterStatus);
    const matchesSearch = record.jobId.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         record.product.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         record.wasteType.toLowerCase().includes(searchTerm.toLowerCase());
    return matchesStatus && matchesSearch;
  });

  // Statistics
  const totalWaste = wasteRecords.length;
  const underInvestigation = wasteRecords.filter(r => r.status === 'Under Investigation').length;
  const confirmed = wasteRecords.filter(r => r.status === 'Confirmed').length;
  const resolved = wasteRecords.filter(r => r.status === 'Resolved').length;

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Under Investigation': return 'bg-yellow-100 text-yellow-800 border-yellow-200';
      case 'Confirmed': return 'bg-red-100 text-red-800 border-red-200';
      case 'Resolved': return 'bg-green-100 text-green-800 border-green-200';
      default: return 'bg-slate-100 text-slate-800 border-slate-200';
    }
  };

  const getSeverityColor = (severity: string) => {
    switch (severity) {
      case 'High': return 'bg-red-100 text-red-800 border-red-200';
      case 'Medium': return 'bg-yellow-100 text-yellow-800 border-yellow-200';
      case 'Low': return 'bg-green-100 text-green-800 border-green-200';
      default: return 'bg-slate-100 text-slate-800 border-slate-200';
    }
  };

  const handleActionSelect = (recordId: string, action: string) => {
    const record = wasteRecords.find(r => r.id === recordId);
    if (!record) return;

    let newStatus = record.status;
    let actionText = '';

    switch (action) {
      case 'rework':
        newStatus = 'Sent for Rework';
        actionText = 'sent for rework';
        break;
      case 'scrap':
        newStatus = 'Marked as Reject';
        actionText = 'marked as reject';
        break;
      case 'quarantine':
        newStatus = 'Quarantined';
        actionText = 'quarantined';
        break;
      case 'investigate':
        newStatus = 'Under Investigation';
        actionText = 'flagged for investigation';
        break;
    }

    setWasteRecords(prev => prev.map(r => 
      r.id === recordId ? { ...r, status: newStatus } : r
    ));

    toast.success(`✅ Waste record ${recordId} has been ${actionText}.`);
  };

  return (
    <div className="space-y-6">
      {/* Page Header */}
      <div className="mb-6">
        <div className="flex items-center gap-3 mb-2">
          <div className="flex items-center justify-center w-8 h-8 bg-orange-100 rounded-full">
            <PackageX className="h-5 w-5 text-orange-600" />
          </div>
          <h1 className="text-xl font-semibold text-slate-900">
            Waste Management | အပိုင်အချေအစားစီမံခန့်ခွဲမှု
          </h1>
        </div>
        <div className="flex items-center gap-2 text-sm text-slate-600 ml-11">
          <PackageX className="h-4 w-4" />
          Track and manage production waste, quality issues, and corrective actions
        </div>
      </div>

      {/* Summary Dashboard */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
        <Card>
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <div className="text-2xl font-bold text-slate-700">{totalWaste}</div>
                <div className="text-sm text-slate-600">Total Waste Records</div>
              </div>
              <PackageX className="h-8 w-8 text-slate-600" />
            </div>
          </CardContent>
        </Card>

        <Card className="border-yellow-200 bg-yellow-50">
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <div className="text-2xl font-bold text-yellow-600">{underInvestigation}</div>
                <div className="text-sm text-yellow-700">Under Investigation</div>
              </div>
              <Clock className="h-8 w-8 text-yellow-600" />
            </div>
          </CardContent>
        </Card>

        <Card className="border-red-200 bg-red-50">
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <div className="text-2xl font-bold text-red-600">{confirmed}</div>
                <div className="text-sm text-red-700">Confirmed Waste</div>
              </div>
              <XCircle className="h-8 w-8 text-red-600" />
            </div>
          </CardContent>
        </Card>

        <Card className="border-green-200 bg-green-50">
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <div className="text-2xl font-bold text-green-600">{resolved}</div>
                <div className="text-sm text-green-700">Resolved</div>
              </div>
              <CheckCircle className="h-8 w-8 text-green-600" />
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Waste Records Table */}
      <Card>
        <CardHeader>
          <div className="flex items-center justify-between">
            <CardTitle className="flex items-center gap-2">
              <FileX className="h-5 w-5 text-orange-600" />
              Waste Records | အပိုင်အချေအစားမှတ်တမ်းများ
              <Badge className="bg-orange-100 text-orange-800 ml-auto">
                {filteredRecords.length} records
              </Badge>
            </CardTitle>
            <div className="flex items-center gap-4">
              <div className="flex items-center gap-2">
                <Input
                  placeholder="Search by Job ID, Product..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="w-64"
                />
              </div>
              <Select value={filterStatus} onValueChange={setFilterStatus}>
                <SelectTrigger className="w-48">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All Status</SelectItem>
                  <SelectItem value="investigation">Under Investigation</SelectItem>
                  <SelectItem value="confirmed">Confirmed</SelectItem>
                  <SelectItem value="resolved">Resolved</SelectItem>
                </SelectContent>
              </Select>
              <Button 
                onClick={() => setShowNewWasteDialog(true)}
                className="bg-orange-600 hover:bg-orange-700"
              >
                <Plus className="h-4 w-4 mr-2" />
                New Waste Report
              </Button>
            </div>
          </div>
        </CardHeader>
        <CardContent>
          <div className="overflow-hidden rounded-lg border">
            <Table>
              <TableHeader>
                <TableRow className="bg-slate-50">
                  <TableHead>Waste ID</TableHead>
                  <TableHead>Job ID</TableHead>
                  <TableHead>Product</TableHead>
                  <TableHead>Waste Type</TableHead>
                  <TableHead>Quantity</TableHead>
                  <TableHead>Severity</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Reported Time</TableHead>
                  <TableHead>Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {filteredRecords.map((record) => (
                  <TableRow key={record.id} className="hover:bg-slate-50">
                    <TableCell className="font-mono font-medium text-orange-600">
                      {record.id}
                    </TableCell>
                    <TableCell className="font-mono font-medium text-blue-600">
                      {record.jobId}
                    </TableCell>
                    <TableCell>
                      <div className="font-medium">{record.product}</div>
                      <div className="text-sm text-slate-600">{record.machine}</div>
                    </TableCell>
                    <TableCell>
                      <div className="font-medium">{record.wasteType}</div>
                      <div className="text-sm text-slate-600">{record.wasteCode}</div>
                    </TableCell>
                    <TableCell className="text-right font-mono">
                      <span className="text-red-600 font-medium">
                        {record.quantity} {record.unit}
                      </span>
                    </TableCell>
                    <TableCell>
                      <Badge className={getSeverityColor(record.severity)}>
                        {record.severity}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <Badge className={getStatusColor(record.status)}>
                        {record.status}
                      </Badge>
                    </TableCell>
                    <TableCell className="font-mono text-sm">
                      {record.reportedTime}
                    </TableCell>
                    <TableCell>
                      <div className="flex gap-2">
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => {
                            setSelectedRecord(record);
                            setShowDetails(true);
                          }}
                        >
                          <Eye className="h-4 w-4" />
                        </Button>
                        <Select onValueChange={(value) => handleActionSelect(record.id, value)}>
                          <SelectTrigger className="w-32">
                            <SelectValue placeholder="Action" />
                          </SelectTrigger>
                          <SelectContent>
                            {wasteActions.map((action) => (
                              <SelectItem key={action.value} value={action.value}>
                                {action.label}
                              </SelectItem>
                            ))}
                          </SelectContent>
                        </Select>
                      </div>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        </CardContent>
      </Card>

      {/* Waste Details Dialog */}
      <Dialog open={showDetails} onOpenChange={setShowDetails}>
        <DialogContent className="max-w-2xl">
          <DialogHeader>
            <DialogTitle className="flex items-center gap-2">
              <FileX className="h-5 w-5 text-orange-600" />
              Waste Details | အပိုင်အချေအစားအသေးစိတ်
            </DialogTitle>
            <DialogDescription>
              Review waste record details and take appropriate action
            </DialogDescription>
          </DialogHeader>
          {selectedRecord && (
            <div className="space-y-4">
              <div className="grid grid-cols-2 gap-4 p-4 bg-slate-50 rounded-lg">
                <div>
                  <Label className="text-sm font-medium text-slate-600">Waste ID</Label>
                  <div className="font-mono text-sm">{selectedRecord.id}</div>
                </div>
                <div>
                  <Label className="text-sm font-medium text-slate-600">Job ID</Label>
                  <div className="font-mono text-sm text-blue-600">{selectedRecord.jobId}</div>
                </div>
                <div>
                  <Label className="text-sm font-medium text-slate-600">Product</Label>
                  <div className="text-sm">{selectedRecord.product}</div>
                </div>
                <div>
                  <Label className="text-sm font-medium text-slate-600">Machine</Label>
                  <div className="text-sm">{selectedRecord.machine}</div>
                </div>
                <div>
                  <Label className="text-sm font-medium text-slate-600">Operator</Label>
                  <div className="text-sm">{selectedRecord.operator}</div>
                </div>
                <div>
                  <Label className="text-sm font-medium text-slate-600">Waste Type</Label>
                  <div className="text-sm">{selectedRecord.wasteType} ({selectedRecord.wasteCode})</div>
                </div>
                <div>
                  <Label className="text-sm font-medium text-slate-600">Quantity</Label>
                  <div className="font-medium text-red-600">{selectedRecord.quantity} {selectedRecord.unit}</div>
                </div>
                <div>
                  <Label className="text-sm font-medium text-slate-600">Severity</Label>
                  <Badge className={getSeverityColor(selectedRecord.severity)}>
                    {selectedRecord.severity}
                  </Badge>
                </div>
                <div>
                  <Label className="text-sm font-medium text-slate-600">Status</Label>
                  <Badge className={getStatusColor(selectedRecord.status)}>
                    {selectedRecord.status}
                  </Badge>
                </div>
                <div>
                  <Label className="text-sm font-medium text-slate-600">Detected By</Label>
                  <div className="text-sm">{selectedRecord.detectedBy}</div>
                </div>
                <div>
                  <Label className="text-sm font-medium text-slate-600">Location</Label>
                  <div className="text-sm">{selectedRecord.location}</div>
                </div>
                <div>
                  <Label className="text-sm font-medium text-slate-600">Photos</Label>
                  <div className="text-sm">{selectedRecord.photos} attachments</div>
                </div>
              </div>
              
              {selectedRecord.notes && (
                <div className="p-4 border border-slate-200 rounded-lg">
                  <Label className="text-sm font-medium text-slate-600">Notes</Label>
                  <div className="text-sm text-slate-700 mt-1">{selectedRecord.notes}</div>
                </div>
              )}

              <div className="flex justify-end gap-2 pt-4">
                <Button variant="outline" onClick={() => setShowDetails(false)}>
                  Close
                </Button>
                <Button className="bg-orange-600 hover:bg-orange-700">
                  <ArrowRight className="h-4 w-4 mr-2" />
                  Take Action
                </Button>
              </div>
            </div>
          )}
        </DialogContent>
      </Dialog>
    </div>
  );
}