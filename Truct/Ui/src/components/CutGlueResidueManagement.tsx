import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Badge } from './ui/badge';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from './ui/dialog';
import { toast } from 'sonner';
import { 
  Search, 
  Filter, 
  Package, 
  Eye, 
  Edit, 
  Send, 
  CheckCircle, 
  XCircle,
  Calendar,
  Clock,
  User,
  AlertTriangle,
  FileText,
  Plus,
  Save,
  RefreshCw,
  Download,
  ArrowUpDown,
  Truck,
  Scale
} from 'lucide-react';

interface ResidueData {
  id: string;
  jobId: string;
  machine: string;
  product: string;
  shift: string;
  operator: string;
  cutSideExtra: number; // kg
  glueSheet: number; // kg
  glueChunk: number; // kg
  destinationWarehouse: 'CUT-WH' | 'CUTPIECE-WH' | 'EXT-WH' | '';
  movementReason: 'Reuse' | 'Cutting' | 'Extrusion' | 'Scrap' | '';
  status: 'Draft' | 'Submitted' | 'Posted';
  createdBy: string;
  createdAt: string;
  submittedAt?: string;
  postedAt?: string;
  notes?: string;
  leftoverTotal?: number;
  batchNumber?: string;
  materialType?: string;
  color?: string;
}

const mockResidueData: ResidueData[] = [
  {
    id: 'RES-20250903-001',
    jobId: 'JOB-20250903-001',
    machine: 'INJ-M001',
    product: 'Plastic Bottle 500ml',
    shift: 'Day Shift',
    operator: 'Ko Thant',
    cutSideExtra: 2.5,
    glueSheet: 1.8,
    glueChunk: 0.7,
    destinationWarehouse: 'CUT-WH',
    movementReason: 'Reuse',
    status: 'Draft',
    createdBy: 'Production Supervisor',
    createdAt: '2025-09-03 14:30',
    leftoverTotal: 8.2,
    batchNumber: 'B-20250903-001',
    materialType: 'PET Clear',
    color: 'Clear',
    notes: 'Quality material suitable for reuse'
  },
  {
    id: 'RES-20250903-002',
    jobId: 'JOB-20250903-002',
    machine: 'INJ-M002',
    product: 'Large Container 2L',
    shift: 'Day Shift',
    operator: 'Ma Phyu',
    cutSideExtra: 3.2,
    glueSheet: 2.1,
    glueChunk: 1.3,
    destinationWarehouse: 'CUTPIECE-WH',
    movementReason: 'Cutting',
    status: 'Submitted',
    createdBy: 'Production Supervisor',
    createdAt: '2025-09-03 13:45',
    submittedAt: '2025-09-03 15:20',
    leftoverTotal: 12.1,
    batchNumber: 'B-20250903-002',
    materialType: 'PP White',
    color: 'White'
  },
  {
    id: 'RES-20250903-003',
    jobId: 'JOB-20250903-003',
    machine: 'INJ-M003',
    product: 'Plastic Cup 250ml',
    shift: 'Night Shift',
    operator: 'Ko Aung',
    cutSideExtra: 1.5,
    glueSheet: 0.9,
    glueChunk: 0.4,
    destinationWarehouse: 'EXT-WH',
    movementReason: 'Extrusion',
    status: 'Posted',
    createdBy: 'Production Supervisor',
    createdAt: '2025-09-02 22:15',
    submittedAt: '2025-09-03 08:30',
    postedAt: '2025-09-03 09:45',
    leftoverTotal: 5.8,
    batchNumber: 'B-20250902-003',
    materialType: 'PP Blue',
    color: 'Blue'
  }
];

const warehouses = [
  { id: 'CUT-WH', name: 'Cut Waste Warehouse', description: 'For cutting and trimming waste' },
  { id: 'CUTPIECE-WH', name: 'Cut Piece Warehouse', description: 'For reusable cut pieces' },
  { id: 'EXT-WH', name: 'Extrusion Warehouse', description: 'For extrusion reprocessing' }
];

const movementReasons = [
  { id: 'Reuse', name: 'Reuse', description: 'Material suitable for direct reuse' },
  { id: 'Cutting', name: 'Cutting', description: 'Requires cutting/trimming processing' },
  { id: 'Extrusion', name: 'Extrusion', description: 'Requires extrusion reprocessing' },
  { id: 'Scrap', name: 'Scrap', description: 'Not suitable for reuse - disposal' }
];

export function CutGlueResidueManagement() {
  const [selectedResidue, setSelectedResidue] = useState<ResidueData | null>(null);
  const [showDetailModal, setShowDetailModal] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState('all');
  const [machineFilter, setMachineFilter] = useState('all');
  const [dateFilter, setDateFilter] = useState('today');

  // Edit form state
  const [editForm, setEditForm] = useState({
    cutSideExtra: 0,
    glueSheet: 0,
    glueChunk: 0,
    destinationWarehouse: '',
    movementReason: '',
    notes: ''
  });

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Draft': return 'bg-gray-100 text-gray-800 border-gray-200';
      case 'Submitted': return 'bg-blue-100 text-blue-800 border-blue-200';
      case 'Posted': return 'bg-green-100 text-green-800 border-green-200';
      default: return 'bg-gray-100 text-gray-800 border-gray-200';
    }
  };

  const getMovementReasonColor = (reason: string) => {
    switch (reason) {
      case 'Reuse': return 'bg-green-100 text-green-800';
      case 'Cutting': return 'bg-blue-100 text-blue-800';
      case 'Extrusion': return 'bg-orange-100 text-orange-800';
      case 'Scrap': return 'bg-red-100 text-red-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  const filteredResidues = mockResidueData.filter(residue => {
    const matchesSearch = residue.id.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         residue.jobId.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         residue.machine.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         residue.product.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesStatus = statusFilter === 'all' || residue.status === statusFilter;
    const matchesMachine = machineFilter === 'all' || residue.machine === machineFilter;
    
    return matchesSearch && matchesStatus && matchesMachine;
  });

  const handleViewDetails = (residue: ResidueData) => {
    setSelectedResidue(residue);
    setEditForm({
      cutSideExtra: residue.cutSideExtra,
      glueSheet: residue.glueSheet,
      glueChunk: residue.glueChunk,
      destinationWarehouse: residue.destinationWarehouse,
      movementReason: residue.movementReason,
      notes: residue.notes || ''
    });
    setShowDetailModal(true);
  };

  const handleSaveDraft = () => {
    // Validation
    const totalResidue = editForm.cutSideExtra + editForm.glueSheet + editForm.glueChunk;
    const leftoverTotal = selectedResidue?.leftoverTotal || 0;

    if (totalResidue > leftoverTotal) {
      toast.error(
        `Total residue (${totalResidue} kg) cannot exceed leftover total (${leftoverTotal} kg) | စုစုပေါင်းကျန်ရစ်ပမာဏ (${totalResidue} kg) သည် ကျန်သောစုစုပေါင်း (${leftoverTotal} kg) ထက် ကျော်လွန်၍မရပါ`
      );
      return;
    }

    if (editForm.cutSideExtra < 0 || editForm.glueSheet < 0 || editForm.glueChunk < 0) {
      toast.error('Negative values are not allowed | အနုတ်တန်ဖိုးများကို ခွင့်မပြုပါ');
      return;
    }

    if (!editForm.destinationWarehouse) {
      toast.error('Destination warehouse is mandatory | ပစ္စည်းပေးပို့မည့်ဂေါဒေါင်ကို ဖြည့်စွက်ရန်လိုအပ်သည်');
      return;
    }

    if (!editForm.movementReason) {
      toast.error('Movement reason is mandatory | ပြောင်းရွှေ့ရသည့်အကြောင်းရင်းကို ဖြည့်စွက်ရန်လိုအပ်သည်');
      return;
    }

    toast.success('Draft saved successfully | မူကြမ်းအောင်မြင်စွာသိမ်းပြီး', {
      description: 'All residue data has been saved as draft'
    });
    setShowDetailModal(false);
  };

  const handleSubmit = () => {
    // Same validation as save draft
    const totalResidue = editForm.cutSideExtra + editForm.glueSheet + editForm.glueChunk;
    const leftoverTotal = selectedResidue?.leftoverTotal || 0;

    if (totalResidue > leftoverTotal) {
      toast.error(
        `Total residue (${totalResidue} kg) cannot exceed leftover total (${leftoverTotal} kg) | စုစုပေါင်းကျန်ရစ်ပမာဏ (${totalResidue} kg) သည် ကျန်သောစုစုပေါင်း (${leftoverTotal} kg) ထက် ကျော်လွန်၍မရပါ`
      );
      return;
    }

    if (!editForm.destinationWarehouse || !editForm.movementReason) {
      toast.error('All required fields must be completed | လိုအပ်သောနေရာများအားလုံးကို ဖြည့်စွက်ရန်လိုအပ်သည်');
      return;
    }

    toast.success('Residue record submitted successfully | ကျန်ရစ်ပမာဏမှတ်တမ်းကို အောင်မြင်စွာတင်ပြပြီး', {
      description: 'Status changed to Submitted. Ready for warehouse posting.'
    });
    setShowDetailModal(false);
  };

  const handlePost = (residueId: string) => {
    toast.success('Posted to warehouse successfully | ဂေါဒေါင်သို့ အောင်မြင်စွာပို့ပြီး', {
      description: 'Inventory movement created and stock levels updated. Inventory team has been notified.',
      action: {
        label: 'View Movement',
        onClick: () => console.log('View inventory movement')
      }
    });
    // Update residue status to Posted
    // Create inventory movement (Prod-WH → selected warehouse)
    // Notify inventory team
  };

  const handleCancel = (residueId: string) => {
    const reason = prompt('Please provide a reason for cancellation | ပယ်ဖျက်ရသည့်အကြောင်းရင်းကို ဖော်ပြပါ:');
    if (reason) {
      toast.error(`Residue record cancelled: ${reason} | ကျန်ရစ်ပမာဏမှတ်တမ်းကို ပယ်ဖျက်ခဲ့သည်: ${reason}`);
    }
  };

  const getTotalResidue = (residue: ResidueData) => {
    return residue.cutSideExtra + residue.glueSheet + residue.glueChunk;
  };

  return (
    <div className="p-6 bg-slate-50 min-h-screen">
      <div className="max-w-7xl mx-auto space-y-6">
        {/* Header */}
        <div className="bg-white rounded-lg shadow-sm p-6">
          <div className="flex items-center justify-between mb-6">
            <div>
              <h1 className="text-2xl font-semibold text-slate-900 mb-2">
                Cut/Glue Residue Management | ခုတ်ဖက် / ကော်စာ / ကော်ခဲ စီမံ
              </h1>
              <p className="text-slate-600">
                Manage production residues and waste materials | ထုတ်လုပ်မှုကျန်ရစ်ပစ္စည်းများနှင့် အပိုင်းအစများကို စီမံခန့်ခွဲပါ
              </p>
            </div>
            <div className="flex gap-2">
              <Button variant="outline" className="text-slate-600">
                <RefreshCw className="h-4 w-4 mr-2" />
                Refresh | ပြန်လည်ပြင်ဆင်
              </Button>
              <Button variant="outline" className="text-slate-600">
                <Download className="h-4 w-4 mr-2" />
                Export | ထုတ်ယူ
              </Button>
              <Button className="bg-blue-600 hover:bg-blue-700 text-white">
                <Plus className="h-4 w-4 mr-2" />
                New Entry | အသစ်ထည့်
              </Button>
            </div>
          </div>

          {/* Filters */}
          <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
            <div>
              <Label>Search Job ID / Product</Label>
              <div className="relative">
                <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-slate-400" />
                <Input
                  placeholder="Search..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="pl-10"
                />
              </div>
            </div>
            <div>
              <Label>Machine | စက်</Label>
              <Select value={machineFilter} onValueChange={setMachineFilter}>
                <SelectTrigger>
                  <SelectValue placeholder="All Machines" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All Machines | စက်အားလုံး</SelectItem>
                  <SelectItem value="INJ-M001">INJ-M001</SelectItem>
                  <SelectItem value="INJ-M002">INJ-M002</SelectItem>
                  <SelectItem value="INJ-M003">INJ-M003</SelectItem>
                  <SelectItem value="INJ-M004">INJ-M004</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div>
              <Label>Status | အခြေအနေ</Label>
              <Select value={statusFilter} onValueChange={setStatusFilter}>
                <SelectTrigger>
                  <SelectValue placeholder="All Status" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All Status | အားလုံး</SelectItem>
                  <SelectItem value="Draft">Draft | မူကြမ်း</SelectItem>
                  <SelectItem value="Submitted">Submitted | တင်ပြပြီး</SelectItem>
                  <SelectItem value="Posted">Posted | ပို့ပြီး</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div>
              <Label>Date Range | ရက်စွဲအပိုင်းအခြား</Label>
              <Select value={dateFilter} onValueChange={setDateFilter}>
                <SelectTrigger>
                  <SelectValue placeholder="Select date range" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="today">Today | ယနေ့</SelectItem>
                  <SelectItem value="week">This Week | ဤအပတ်</SelectItem>
                  <SelectItem value="month">This Month | ဤလ</SelectItem>
                  <SelectItem value="custom">Custom Range | စိတ်ကြိုက်</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>
        </div>

        {/* Summary Cards */}
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
          <Card className="p-4">
            <div className="text-center">
              <div className="text-2xl font-bold text-gray-600">
                {mockResidueData.filter(r => r.status === 'Draft').length}
              </div>
              <div className="text-sm text-slate-600">Draft | မူကြမ်း</div>
            </div>
          </Card>
          <Card className="p-4">
            <div className="text-center">
              <div className="text-2xl font-bold text-blue-600">
                {mockResidueData.filter(r => r.status === 'Submitted').length}
              </div>
              <div className="text-sm text-slate-600">Submitted | တင်ပြပြီး</div>
            </div>
          </Card>
          <Card className="p-4">
            <div className="text-center">
              <div className="text-2xl font-bold text-green-600">
                {mockResidueData.filter(r => r.status === 'Posted').length}
              </div>
              <div className="text-sm text-slate-600">Posted | ပို့ပြီး</div>
            </div>
          </Card>
          <Card className="p-4">
            <div className="text-center">
              <div className="text-2xl font-bold text-orange-600">
                {mockResidueData.reduce((sum, r) => sum + getTotalResidue(r), 0).toFixed(1)} kg
              </div>
              <div className="text-sm text-slate-600">Total Residue | စုစုပေါင်းကျန်ရစ်</div>
            </div>
          </Card>
        </div>

        {/* Data Table */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Package className="h-5 w-5" />
              Residue Records | ကျန်ရစ်ပမာဏမှတ်တမ်းများ
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead className="cursor-pointer hover:bg-slate-50">
                      <div className="flex items-center gap-1">
                        Job ID
                        <ArrowUpDown className="h-3 w-3" />
                      </div>
                    </TableHead>
                    <TableHead>Machine | စက်</TableHead>
                    <TableHead>Product | ထုတ်ကုန်</TableHead>
                    <TableHead>Shift | ရှစ်</TableHead>
                    <TableHead>Operator | အော်ပရေတာ</TableHead>
                    <TableHead className="text-center">Cut Side Extra (kg)</TableHead>
                    <TableHead className="text-center">Glue Sheet (kg)</TableHead>
                    <TableHead className="text-center">Glue Chunk (kg)</TableHead>
                    <TableHead>Destination WH | ပစ္စည်းပေးပို့မည့်ဂေါဒေါင်</TableHead>
                    <TableHead>Status | အခြေအနေ</TableHead>
                    <TableHead className="text-center">Actions | လုပ်ဆောင်ချက်များ</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {filteredResidues.map((residue) => (
                    <TableRow 
                      key={residue.id} 
                      className="cursor-pointer hover:bg-slate-50"
                      onClick={() => handleViewDetails(residue)}
                    >
                      <TableCell className="font-medium">{residue.jobId}</TableCell>
                      <TableCell>{residue.machine}</TableCell>
                      <TableCell className="max-w-32 truncate">{residue.product}</TableCell>
                      <TableCell>{residue.shift}</TableCell>
                      <TableCell>{residue.operator}</TableCell>
                      <TableCell className="text-center">
                        <div className="flex items-center justify-center gap-1">
                          <Scale className="h-3 w-3 text-slate-400" />
                          {residue.cutSideExtra}
                        </div>
                      </TableCell>
                      <TableCell className="text-center">
                        <div className="flex items-center justify-center gap-1">
                          <Scale className="h-3 w-3 text-slate-400" />
                          {residue.glueSheet}
                        </div>
                      </TableCell>
                      <TableCell className="text-center">
                        <div className="flex items-center justify-center gap-1">
                          <Scale className="h-3 w-3 text-slate-400" />
                          {residue.glueChunk}
                        </div>
                      </TableCell>
                      <TableCell>
                        {residue.destinationWarehouse && (
                          <div className="flex items-center gap-1">
                            <Truck className="h-3 w-3 text-slate-400" />
                            {residue.destinationWarehouse}
                          </div>
                        )}
                      </TableCell>
                      <TableCell>
                        <Badge className={getStatusColor(residue.status)}>
                          {residue.status}
                        </Badge>
                      </TableCell>
                      <TableCell>
                        <div className="flex gap-1 justify-center">
                          <Button
                            size="sm"
                            variant="outline"
                            onClick={(e) => {
                              e.stopPropagation();
                              handleViewDetails(residue);
                            }}
                          >
                            <Eye className="h-3 w-3" />
                          </Button>
                          {residue.status === 'Draft' && (
                            <Button
                              size="sm"
                              variant="outline"
                              className="text-blue-600 border-blue-600 hover:bg-blue-50"
                              onClick={(e) => {
                                e.stopPropagation();
                                handleSubmit();
                              }}
                            >
                              <Send className="h-3 w-3" />
                            </Button>
                          )}
                          {residue.status === 'Submitted' && (
                            <Button
                              size="sm"
                              variant="outline"
                              className="text-green-600 border-green-600 hover:bg-green-50"
                              onClick={(e) => {
                                e.stopPropagation();
                                handlePost(residue.id);
                              }}
                            >
                              <CheckCircle className="h-3 w-3" />
                            </Button>
                          )}
                          {residue.status !== 'Posted' && (
                            <Button
                              size="sm"
                              variant="outline"
                              className="text-red-600 border-red-600 hover:bg-red-50"
                              onClick={(e) => {
                                e.stopPropagation();
                                handleCancel(residue.id);
                              }}
                            >
                              <XCircle className="h-3 w-3" />
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

        {/* Detail Modal */}
        <Dialog open={showDetailModal} onOpenChange={setShowDetailModal}>
          <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
            <DialogHeader>
              <DialogTitle className="flex items-center gap-2">
                <Package className="h-5 w-5" />
                Residue Details: {selectedResidue?.jobId} | ကျန်ရစ်ပမာဏအသေးစိတ်
              </DialogTitle>
              <DialogDescription>
                Production residue entry and warehouse movement | ထုတ်လုပ်မှုကျန်ရစ်ပမာဏထည့်ခြင်းနှင့် ဂေါဒေါင်ပြောင်းရွှေ့ခြင်း
              </DialogDescription>
            </DialogHeader>

            {selectedResidue && (
              <div className="space-y-6">
                {/* Job Information */}
                <Card>
                  <CardHeader>
                    <CardTitle className="text-lg">Job Information | အလုပ်အချက်အလက်</CardTitle>
                  </CardHeader>
                  <CardContent className="space-y-4">
                    <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
                      <div>
                        <Label className="text-sm text-slate-600">Job ID</Label>
                        <p className="font-medium">{selectedResidue.jobId}</p>
                      </div>
                      <div>
                        <Label className="text-sm text-slate-600">Machine | စက်</Label>
                        <p className="font-medium">{selectedResidue.machine}</p>
                      </div>
                      <div>
                        <Label className="text-sm text-slate-600">Product | ထုတ်ကုန်</Label>
                        <p className="font-medium">{selectedResidue.product}</p>
                      </div>
                      <div>
                        <Label className="text-sm text-slate-600">Shift | ရှစ်</Label>
                        <p className="font-medium">{selectedResidue.shift}</p>
                      </div>
                      <div>
                        <Label className="text-sm text-slate-600">Operator | အော်ပရေတာ</Label>
                        <p className="font-medium">{selectedResidue.operator}</p>
                      </div>
                      <div>
                        <Label className="text-sm text-slate-600">Batch Number | အစုအဝေးနံပါတ်</Label>
                        <p className="font-medium">{selectedResidue.batchNumber}</p>
                      </div>
                    </div>
                    <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
                      <div>
                        <Label className="text-sm text-slate-600">Material Type | ပစ္စည်းအမျိုးအစား</Label>
                        <p className="font-medium">{selectedResidue.materialType}</p>
                      </div>
                      <div>
                        <Label className="text-sm text-slate-600">Color | အရောင်</Label>
                        <p className="font-medium">{selectedResidue.color}</p>
                      </div>
                      <div>
                        <Label className="text-sm text-slate-600">Leftover Total | စုစုပေါင်းကျန်ရစ်</Label>
                        <p className="font-medium text-blue-600">{selectedResidue.leftoverTotal} kg</p>
                      </div>
                    </div>
                  </CardContent>
                </Card>

                {/* Residue Entry */}
                <Card>
                  <CardHeader>
                    <CardTitle className="text-lg">Residue Entry | ကျန်ရစ်ပမာဏထည့်ခြင်း</CardTitle>
                  </CardHeader>
                  <CardContent className="space-y-4">
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                      <div>
                        <Label>Cut Side Extra Qty (kg) | ခုတ်ဖက်အပိုပမာဏ *</Label>
                        <Input
                          type="number"
                          step="0.1"
                          min="0"
                          value={editForm.cutSideExtra}
                          onChange={(e) => setEditForm(prev => ({...prev, cutSideExtra: parseFloat(e.target.value) || 0}))}
                          placeholder="0.0"
                          disabled={selectedResidue.status === 'Posted'}
                        />
                      </div>
                      <div>
                        <Label>Glue Sheet Qty (kg) | ကော်စာပမာဏ *</Label>
                        <Input
                          type="number"
                          step="0.1"
                          min="0"
                          value={editForm.glueSheet}
                          onChange={(e) => setEditForm(prev => ({...prev, glueSheet: parseFloat(e.target.value) || 0}))}
                          placeholder="0.0"
                          disabled={selectedResidue.status === 'Posted'}
                        />
                      </div>
                      <div>
                        <Label>Glue Chunk Qty (kg) | ကော်ခဲပမာဏ *</Label>
                        <Input
                          type="number"
                          step="0.1"
                          min="0"
                          value={editForm.glueChunk}
                          onChange={(e) => setEditForm(prev => ({...prev, glueChunk: parseFloat(e.target.value) || 0}))}
                          placeholder="0.0"
                          disabled={selectedResidue.status === 'Posted'}
                        />
                      </div>
                    </div>

                    <div className="p-4 bg-blue-50 rounded-lg">
                      <div className="flex justify-between items-center">
                        <span className="font-medium">Total Residue | စုစုပေါင်းကျန်ရစ်:</span>
                        <span className="text-xl font-bold text-blue-600">
                          {(editForm.cutSideExtra + editForm.glueSheet + editForm.glueChunk).toFixed(1)} kg
                        </span>
                      </div>
                      <div className="text-sm text-slate-600 mt-1">
                        Available: {selectedResidue.leftoverTotal} kg | ရရှိသော: {selectedResidue.leftoverTotal} kg
                      </div>
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                      <div>
                        <Label>Destination Warehouse | ပစ္စည်းပေးပို့မည့်ဂေါဒေါင် *</Label>
                        <Select 
                          value={editForm.destinationWarehouse} 
                          onValueChange={(value) => setEditForm(prev => ({...prev, destinationWarehouse: value}))}
                          disabled={selectedResidue.status === 'Posted'}
                        >
                          <SelectTrigger>
                            <SelectValue placeholder="Select warehouse" />
                          </SelectTrigger>
                          <SelectContent>
                            {warehouses.map((warehouse) => (
                              <SelectItem key={warehouse.id} value={warehouse.id}>
                                {warehouse.id} - {warehouse.name}
                              </SelectItem>
                            ))}
                          </SelectContent>
                        </Select>
                        {editForm.destinationWarehouse && (
                          <p className="text-sm text-slate-600 mt-1">
                            {warehouses.find(w => w.id === editForm.destinationWarehouse)?.description}
                          </p>
                        )}
                      </div>
                      <div>
                        <Label>Movement Reason | ပြောင်းရွှေ့ရသည့်အကြောင်းရင်း *</Label>
                        <Select 
                          value={editForm.movementReason} 
                          onValueChange={(value) => setEditForm(prev => ({...prev, movementReason: value}))}
                          disabled={selectedResidue.status === 'Posted'}
                        >
                          <SelectTrigger>
                            <SelectValue placeholder="Select reason" />
                          </SelectTrigger>
                          <SelectContent>
                            {movementReasons.map((reason) => (
                              <SelectItem key={reason.id} value={reason.id}>
                                <div className="flex items-center gap-2">
                                  <Badge className={getMovementReasonColor(reason.id)}>
                                    {reason.name}
                                  </Badge>
                                  <span className="text-xs">{reason.description}</span>
                                </div>
                              </SelectItem>
                            ))}
                          </SelectContent>
                        </Select>
                      </div>
                    </div>

                    <div>
                      <Label>Notes | မှတ်ချက်များ</Label>
                      <Input
                        value={editForm.notes}
                        onChange={(e) => setEditForm(prev => ({...prev, notes: e.target.value}))}
                        placeholder="Add any additional notes..."
                        disabled={selectedResidue.status === 'Posted'}
                      />
                    </div>
                  </CardContent>
                </Card>

                {/* Status Information */}
                {selectedResidue.status !== 'Draft' && (
                  <Card>
                    <CardHeader>
                      <CardTitle className="text-lg">Status History | အခြေအနေမှတ်တမ်း</CardTitle>
                    </CardHeader>
                    <CardContent className="space-y-3">
                      <div className="flex items-center gap-3">
                        <div className="w-3 h-3 bg-gray-400 rounded-full"></div>
                        <div>
                          <p className="font-medium">Created | ဖန်တီးခဲ့သည်</p>
                          <p className="text-sm text-slate-600">{selectedResidue.createdAt} by {selectedResidue.createdBy}</p>
                        </div>
                      </div>
                      {selectedResidue.submittedAt && (
                        <div className="flex items-center gap-3">
                          <div className="w-3 h-3 bg-blue-500 rounded-full"></div>
                          <div>
                            <p className="font-medium">Submitted | တင်ပြခဲ့သည်</p>
                            <p className="text-sm text-slate-600">{selectedResidue.submittedAt}</p>
                          </div>
                        </div>
                      )}
                      {selectedResidue.postedAt && (
                        <div className="flex items-center gap-3">
                          <div className="w-3 h-3 bg-green-500 rounded-full"></div>
                          <div>
                            <p className="font-medium">Posted to Warehouse | ဂေါဒေါင်သို့ပို့ခဲ့သည်</p>
                            <p className="text-sm text-slate-600">{selectedResidue.postedAt}</p>
                          </div>
                        </div>
                      )}
                    </CardContent>
                  </Card>
                )}

                {/* Footer Actions */}
                <div className="flex justify-end gap-3 pt-4 border-t">
                  <Button
                    variant="outline"
                    onClick={() => setShowDetailModal(false)}
                  >
                    <XCircle className="h-4 w-4 mr-2" />
                    Close | ပိတ်
                  </Button>
                  
                  {selectedResidue.status !== 'Posted' && (
                    <>
                      <Button
                        variant="outline"
                        onClick={handleSaveDraft}
                      >
                        <Save className="h-4 w-4 mr-2" />
                        Save Draft | မူကြမ်းသိမ်း
                      </Button>
                      
                      {selectedResidue.status === 'Draft' && (
                        <Button
                          className="bg-blue-600 hover:bg-blue-700 text-white"
                          onClick={handleSubmit}
                        >
                          <Send className="h-4 w-4 mr-2" />
                          Submit | တင်ပြ
                        </Button>
                      )}
                      
                      {selectedResidue.status === 'Submitted' && (
                        <Button
                          className="bg-green-600 hover:bg-green-700 text-white"
                          onClick={() => handlePost(selectedResidue.id)}
                        >
                          <CheckCircle className="h-4 w-4 mr-2" />
                          Post to Warehouse | ဂေါဒေါင်သို့ပို့
                        </Button>
                      )}
                    </>
                  )}
                </div>
              </div>
            )}
          </DialogContent>
        </Dialog>
      </div>
    </div>
  );
}