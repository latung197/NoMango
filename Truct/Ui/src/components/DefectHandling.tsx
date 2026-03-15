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

// Mock data for waste
const mockWasteRecords = [
  {
    id: 'WASTE-001',
    jobId: 'JOB20250903-001',
    product: '2011 - Plastic Bottle 500ml',
    machine: 'INJ-M001',
    operator: 'Ko Thant (OP001)',
    wasteType: 'Surface Waste',
    wasteCode: 'SF001',
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
    id: 'WASTE-002',
    jobId: 'JOB20250903-002',
    product: '2012 - Plastic Container 1L',
    machine: 'INJ-M002',
    operator: 'Ma Hnin (OP002)',
    wasteType: 'Dimensional',
    wasteCode: 'DM002',
    quantity: 12,
    unit: 'pcs',
    location: 'Final Inspection',
    detectedBy: 'QC Supervisor',
    reportedTime: '2025-09-08 11:45',
    severity: 'Medium',
    status: 'Root Cause Analysis',
    photos: 1,
    notes: 'Wall thickness out of specification'
  },
  {
    id: 'WASTE-003',
    jobId: 'JOB20250903-003',
    product: '2013 - Plastic Cup 200ml',
    machine: 'INJ-M003',
    operator: 'Ko Zaw (OP003)',
    wasteType: 'Color Variation',
    wasteCode: 'CV001',
    quantity: 8,
    unit: 'pcs',
    location: 'Machine Output',
    detectedBy: 'Operator',
    reportedTime: '2025-09-08 12:15',
    severity: 'Low',
    status: 'Resolved',
    photos: 3,
    notes: 'Color mixing issue resolved by adjusting material ratio'
  },
  {
    id: 'WASTE-004',
    jobId: 'JOB20250903-001',
    product: '2011 - Plastic Bottle 500ml',
    machine: 'INJ-M001',
    operator: 'Ko Thant (OP001)',
    wasteType: 'Flash/Burr',
    wasteCode: 'FB001',
    quantity: 15,
    unit: 'pcs',
    location: 'Machine Output',
    detectedBy: 'Operator',
    reportedTime: '2025-09-08 13:20',
    severity: 'Medium',
    status: 'Corrective Action',
    photos: 1,
    notes: 'Excess material flash, mold pressure adjustment needed'
  }
];

const mockActiveJobs = [
  { jobId: 'JOB20250903-001', product: '2011 - Plastic Bottle 500ml', machine: 'INJ-M001' },
  { jobId: 'JOB20250903-002', product: '2012 - Plastic Container 1L', machine: 'INJ-M002' },
  { jobId: 'JOB20250903-003', product: '2013 - Plastic Cup 200ml', machine: 'INJ-M003' }
];

const wasteTypes = [
  { code: 'SF001', name: 'Surface Waste' },
  { code: 'DM002', name: 'Dimensional' },
  { code: 'CV001', name: 'Color Variation' },
  { code: 'FB001', name: 'Flash/Burr' },
  { code: 'CR001', name: 'Crack/Stress' },
  { code: 'SH001', name: 'Short Shot' },
  { code: 'WL001', name: 'Weld Line' }
];

export function DefectHandling() {
  const [newWasteDialog, setNewWasteDialog] = useState(false);
  const [wasteFormData, setWasteFormData] = useState({
    jobId: '',
    wasteType: '',
    quantity: '',
    location: '',
    severity: '',
    notes: ''
  });

  const getSeverityBadge = (severity: string) => {
    switch (severity) {
      case 'High':
        return <Badge className="bg-red-100 text-red-800">🔴 High</Badge>;
      case 'Medium':
        return <Badge className="bg-yellow-100 text-yellow-800">🟡 Medium</Badge>;
      case 'Low':
        return <Badge className="bg-green-100 text-green-800">🟢 Low</Badge>;
      default:
        return <Badge variant="outline">{severity}</Badge>;
    }
  };

  const getStatusBadge = (status: string) => {
    switch (status) {
      case 'Under Investigation':
        return <Badge className="bg-orange-100 text-orange-800">🔍 Investigating</Badge>;
      case 'Root Cause Analysis':
        return <Badge className="bg-blue-100 text-blue-800">🧪 RCA</Badge>;
      case 'Corrective Action':
        return <Badge className="bg-purple-100 text-purple-800">🔧 Correcting</Badge>;
      case 'Resolved':
        return <Badge className="bg-green-100 text-green-800">✅ Resolved</Badge>;
      default:
        return <Badge variant="outline">{status}</Badge>;
    }
  };

  const handleWasteSubmit = () => {
    const { jobId, wasteType, quantity, location, severity, notes } = wasteFormData;
    
    if (!jobId || !wasteType || !quantity || !location || !severity) {
      toast.error('Please fill in all required fields');
      return;
    }

    const selectedJob = mockActiveJobs.find(job => job.jobId === jobId);
    const selectedWasteType = wasteTypes.find(type => type.code === wasteType);

    const newWaste = {
      id: `WASTE-${String(mockWasteRecords.length + 1).padStart(3, '0')}`,
      jobId,
      product: selectedJob?.product || '',
      machine: selectedJob?.machine || '',
      operator: 'Current User',
      wasteType: selectedWasteType?.name || '',
      wasteCode: wasteType,
      quantity: parseInt(quantity),
      unit: 'pcs',
      location,
      detectedBy: 'Production Operator',
      reportedTime: new Date().toISOString().slice(0, 16).replace('T', ' '),
      severity,
      status: 'Under Investigation',
      photos: 0,
      notes: notes || ''
    };

    mockWasteRecords.unshift(newWaste);
    toast.success('✅ Waste Record Created Successfully');
    setNewWasteDialog(false);
    setWasteFormData({
      jobId: '',
      wasteType: '',
      quantity: '',
      location: '',
      severity: '',
      notes: ''
    });
  };

  return (
    <div className="space-y-6">
      {/* Header with Action Buttons */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <AlertTriangle className="h-5 w-5 text-red-600" />
            Waste Handling
            <span className="text-sm font-normal text-slate-600">အပိုင်အချေအစား ကိုင်တွယ်ခြင်း</span>
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex gap-3">
            <Button 
              onClick={() => setNewWasteDialog(true)}
              className="bg-red-600 hover:bg-red-700 text-white"
            >
              <Plus className="h-4 w-4 mr-2" />
              📝 Report New Waste
            </Button>
            <Button variant="outline" className="text-blue-600 border-blue-600 hover:bg-blue-50">
              <Eye className="h-4 w-4 mr-2" />
              📊 Waste Analytics
            </Button>
            <Button variant="outline" className="text-green-600 border-green-600 hover:bg-green-50">
              <FileX className="h-4 w-4 mr-2" />
              📋 Waste Reports
            </Button>
          </div>
        </CardContent>
      </Card>

      {/* Waste Records Table */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Package className="h-5 w-5 text-orange-600" />
            Waste Records | အပိုင်အချေအစား မှတ်တမ်းများ
            <Badge className="bg-orange-100 text-orange-800 ml-auto">
              {mockWasteRecords.length} records
            </Badge>
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Waste ID</TableHead>
                  <TableHead>Job & Product</TableHead>
                  <TableHead>Machine</TableHead>
                  <TableHead>Waste Type</TableHead>
                  <TableHead>Quantity</TableHead>
                  <TableHead>Severity</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Detected By</TableHead>
                  <TableHead>Time</TableHead>
                  <TableHead>Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {mockWasteRecords.map((waste) => (
                  <TableRow key={waste.id}>
                    <TableCell>
                      <Badge variant="outline" className="font-mono text-xs">
                        {waste.id}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <div>
                        <div className="font-medium text-sm">{waste.jobId}</div>
                        <div className="text-xs text-slate-600 max-w-[150px] truncate">
                          {waste.product}
                        </div>
                      </div>
                    </TableCell>
                    <TableCell>
                      <Badge variant="outline" className="bg-blue-50 text-blue-700">
                        {waste.machine}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <div>
                        <div className="font-medium text-sm">{waste.wasteType}</div>
                        <div className="text-xs text-slate-500">{waste.wasteCode}</div>
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-1 text-red-600 font-semibold">
                        <AlertTriangle className="h-3 w-3" />
                        {waste.quantity} {waste.unit}
                      </div>
                    </TableCell>
                    <TableCell>
                      {getSeverityBadge(waste.severity)}
                    </TableCell>
                    <TableCell>
                      {getStatusBadge(waste.status)}
                    </TableCell>
                    <TableCell className="text-sm">{waste.detectedBy}</TableCell>
                    <TableCell className="font-mono text-xs">{waste.reportedTime}</TableCell>
                    <TableCell>
                      <div className="flex gap-1">
                        <Button size="sm" variant="ghost" title="View Details">
                          <Eye className="h-3 w-3" />
                        </Button>
                        {waste.status !== 'Resolved' && (
                          <Button 
                            size="sm" 
                            variant="outline"
                            className="text-green-600 border-green-600 hover:bg-green-50"
                            title="Take Action"
                          >
                            <ArrowRight className="h-3 w-3" />
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

      {/* New Waste Report Dialog */}
      <Dialog open={newWasteDialog} onOpenChange={setNewWasteDialog}>
        <DialogContent className="max-w-2xl">
          <DialogHeader>
            <DialogTitle className="flex items-center gap-2">
              <AlertTriangle className="h-5 w-5 text-red-600" />
              Report New Waste
            </DialogTitle>
            <DialogDescription>
              Create a new waste record for quality tracking and analysis
            </DialogDescription>
          </DialogHeader>
          <div className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label>Job ID</Label>
                <Select
                  value={wasteFormData.jobId}
                  onValueChange={(value) => setWasteFormData(prev => ({ ...prev, jobId: value }))}
                >
                  <SelectTrigger>
                    <SelectValue placeholder="Select active job" />
                  </SelectTrigger>
                  <SelectContent>
                    {mockActiveJobs.map((job) => (
                      <SelectItem key={job.jobId} value={job.jobId}>
                        {job.jobId} - {job.machine}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>

              <div>
                <Label>Waste Type</Label>
                <Select
                  value={wasteFormData.wasteType}
                  onValueChange={(value) => setWasteFormData(prev => ({ ...prev, wasteType: value }))}
                >
                  <SelectTrigger>
                    <SelectValue placeholder="Select waste type" />
                  </SelectTrigger>
                  <SelectContent>
                    {wasteTypes.map((type) => (
                      <SelectItem key={type.code} value={type.code}>
                        {type.code} - {type.name}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
            </div>

            <div className="grid grid-cols-3 gap-4">
              <div>
                <Label>Waste Quantity</Label>
                <Input
                  type="number"
                  placeholder="Enter quantity"
                  value={wasteFormData.quantity}
                  onChange={(e) => setWasteFormData(prev => ({ ...prev, quantity: e.target.value }))}
                />
              </div>

              <div>
                <Label>Detection Location</Label>
                <Select
                  value={wasteFormData.location}
                  onValueChange={(value) => setWasteFormData(prev => ({ ...prev, location: value }))}
                >
                  <SelectTrigger>
                    <SelectValue placeholder="Select location" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="Machine Output">Machine Output</SelectItem>
                    <SelectItem value="In-Process QC">In-Process QC</SelectItem>
                    <SelectItem value="Final Inspection">Final Inspection</SelectItem>
                    <SelectItem value="Packaging Area">Packaging Area</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div>
                <Label>Severity Level</Label>
                <Select
                  value={wasteFormData.severity}
                  onValueChange={(value) => setWasteFormData(prev => ({ ...prev, severity: value }))}
                >
                  <SelectTrigger>
                    <SelectValue placeholder="Select severity" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="High">🔴 High</SelectItem>
                    <SelectItem value="Medium">🟡 Medium</SelectItem>
                    <SelectItem value="Low">🟢 Low</SelectItem>
                  </SelectContent>
                </Select>
              </div>
            </div>

            <div>
              <Label>Notes & Details</Label>
              <Textarea
                placeholder="Describe the waste in detail..."
                value={wasteFormData.notes}
                onChange={(e) => setWasteFormData(prev => ({ ...prev, notes: e.target.value }))}
                rows={3}
              />
            </div>

            <div className="flex justify-end gap-3">
              <Button
                variant="outline"
                onClick={() => setNewWasteDialog(false)}
              >
                Cancel
              </Button>
              <Button
                onClick={handleWasteSubmit}
                className="bg-red-600 hover:bg-red-700 text-white"
              >
                <AlertTriangle className="h-4 w-4 mr-2" />
                Report Waste
              </Button>
            </div>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );
}