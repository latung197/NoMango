import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '../ui/card';
import { Button } from '../ui/button';
import { Input } from '../ui/input';
import { Label } from '../ui/label';
import { Badge } from '../ui/badge';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../ui/table';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../ui/select';
import { Switch } from '../ui/switch';
import { Sheet, SheetContent, SheetDescription, SheetHeader, SheetTitle } from '../ui/sheet';
import { Alert, AlertDescription } from '../ui/alert';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogTrigger } from '../ui/dialog';
import { 
  Search, 
  Filter, 
  Download, 
  Plus, 
  Edit, 
  Save,
  Package,
  Calculator,
  Info,
  Settings,
  Clock,
  Zap,
  Scale,
  FileSpreadsheet,
  Wrench,
  Target,
  Eye,
  Layers
} from 'lucide-react';

// Mock data
const mockRawMaterials = [
  {
    id: 1,
    productColorCode: 'CUP-250-RED-001',
    rawMaterialType: 'PP',
    weightPerPiece: 15.5,
    cavities: 4,
    cycleTime: 28,
    outputPerHour: 514, // Auto-calculated
    bagsPerThousand: 1.4, // Auto-calculated
    allowedMolds: ['MLD-001', 'MLD-002'],
    allowedMachines: ['MCH-001', 'MCH-003'],
    status: 'Active',
    createdBy: 'Admin',
    createdOn: '2024-03-15 10:30',
    updatedBy: 'Admin',
    updatedOn: '2024-03-15 10:30',
    version: 1
  },
  {
    id: 2,
    productColorCode: 'CUP-500-BLUE-002',
    rawMaterialType: 'LDPE',
    weightPerPiece: 28.0,
    cavities: 2,
    cycleTime: 45,
    outputPerHour: 160,
    bagsPerThousand: 2.5,
    allowedMolds: ['MLD-004'],
    allowedMachines: ['MCH-002', 'MCH-004'],
    status: 'Active',
    createdBy: 'Planner',
    createdOn: '2024-03-14 14:20',
    updatedBy: 'Planner',
    updatedOn: '2024-03-14 14:20',
    version: 1
  },
  {
    id: 3,
    productColorCode: 'LID-UNIV-WHITE-001',
    rawMaterialType: 'Recycled PP',
    weightPerPiece: 8.2,
    cavities: 8,
    cycleTime: 22,
    outputPerHour: 1309,
    bagsPerThousand: 0.7,
    allowedMolds: ['MLD-003'],
    allowedMachines: ['MCH-001', 'MCH-002', 'MCH-003'],
    status: 'Inactive',
    createdBy: 'Engineer',
    createdOn: '2024-03-13 09:15',
    updatedBy: 'Engineer',
    updatedOn: '2024-03-13 09:15',
    version: 1
  }
];

const mockProductColors = [
  { code: 'CUP-250-RED-001', product: 'CUP-250', color: 'RED-001', description: '250ml Cup - Red' },
  { code: 'CUP-500-BLUE-002', product: 'CUP-500', color: 'BLUE-002', description: '500ml Cup - Blue' },
  { code: 'LID-UNIV-WHITE-001', product: 'LID-UNIV', color: 'WHITE-001', description: 'Universal Lid - White' },
  { code: 'BOX-L-GREEN-001', product: 'BOX-L', color: 'GREEN-001', description: 'Large Box - Green' },
  { code: 'TRAY-MED-CLEAR-001', product: 'TRAY-MED', color: 'CLEAR-001', description: 'Medium Tray - Clear' }
];

const defaultRawMaterialTypes = ['PP', 'LDPE', 'HDPE', 'PET', 'Mix', 'Recycled PP', 'Colored PP'];

const mockMolds = [
  { code: 'MLD-001', name: 'Cup Mold 4-Cavity' },
  { code: 'MLD-002', name: 'Cup Mold 8-Cavity' },
  { code: 'MLD-003', name: 'Lid Mold 8-Cavity' },
  { code: 'MLD-004', name: 'Large Cup Mold 2-Cavity' },
  { code: 'MLD-005', name: 'Box Mold 6-Cavity' }
];

const mockMachines = [
  { code: 'MCH-001', name: 'Injection Machine 1 (120T)' },
  { code: 'MCH-002', name: 'Injection Machine 2 (150T)' },
  { code: 'MCH-003', name: 'Injection Machine 3 (180T)' },
  { code: 'MCH-004', name: 'Injection Machine 4 (200T)' },
  { code: 'MCH-005', name: 'Injection Machine 5 (250T)' }
];

// Constants for calculations
const CONSTANTS = {
  BAG_WEIGHT_LB: 55,
  GRAMS_PER_POUND: 454.54
};

export function RawMaterialRegistration() {
  const [searchTerm, setSearchTerm] = useState('');
  const [filterType, setFilterType] = useState('all');
  const [filterStatus, setFilterStatus] = useState('all');
  const [showDrawer, setShowDrawer] = useState(false);
  const [editingItem, setEditingItem] = useState<any>(null);
  const [showNewTypeDialog, setShowNewTypeDialog] = useState(false);
  const [newTypeName, setNewTypeName] = useState('');
  const [sampleOrderQty, setSampleOrderQty] = useState('10000');
  
  // Custom raw material types (in real app, this would be from database)
  const [customRawMaterialTypes, setCustomRawMaterialTypes] = useState<string[]>([]);
  const allRawMaterialTypes = [...defaultRawMaterialTypes, ...customRawMaterialTypes];

  const [formData, setFormData] = useState({
    productColorCode: '',
    rawMaterialType: '',
    weightPerPiece: '',
    cavities: '',
    cycleTime: '',
    allowedMolds: [] as string[],
    allowedMachines: [] as string[],
    status: true
  });

  // Auto-calculations
  const getOutputPerHour = (weight?: string, cavities?: string, ct?: string) => {
    const cycleTime = parseFloat(ct || formData.cycleTime) || 0;
    const cavitiesNum = parseInt(cavities || formData.cavities) || 0;
    if (cycleTime === 0) return 0;
    return Math.round((3600 / cycleTime) * cavitiesNum);
  };

  const getBagsPerThousand = (weight?: string) => {
    const weightNum = parseFloat(weight || formData.weightPerPiece) || 0;
    const bagWeightG = CONSTANTS.BAG_WEIGHT_LB * CONSTANTS.GRAMS_PER_POUND;
    if (weightNum === 0) return 0;
    return parseFloat(((1000 * weightNum) / bagWeightG).toFixed(2));
  };

  const getWeightPerShot = (weight?: string, cavities?: string) => {
    const weightNum = parseFloat(weight || formData.weightPerPiece) || 0;
    const cavitiesNum = parseInt(cavities || formData.cavities) || 0;
    return parseFloat((weightNum * cavitiesNum).toFixed(2));
  };

  const getTheoreticalUnitsPerHour = (ct?: string, cavities?: string) => {
    return getOutputPerHour('', cavities, ct);
  };

  const getPlannedUnitsPerHour = (ct?: string, cavities?: string, efficiency = 85) => {
    const theoretical = getTheoreticalUnitsPerHour(ct, cavities);
    return Math.round(theoretical * (efficiency / 100));
  };

  const getBagsForSampleOrder = (weight?: string, qty?: string) => {
    const sampleQty = parseInt(qty || sampleOrderQty) || 0;
    const weightNum = parseFloat(weight || formData.weightPerPiece) || 0;
    const bagWeightG = CONSTANTS.BAG_WEIGHT_LB * CONSTANTS.GRAMS_PER_POUND;
    if (weightNum === 0 || sampleQty === 0) return 0;
    return parseFloat(((sampleQty * weightNum) / bagWeightG).toFixed(2));
  };

  const getDurationEstimate = (ct?: string, cavities?: string, qty?: string) => {
    const outputHour = getOutputPerHour('', cavities, ct);
    const sampleQty = parseInt(qty || sampleOrderQty) || 0;
    if (outputHour === 0 || sampleQty === 0) return 0;
    return parseFloat((sampleQty / outputHour).toFixed(1));
  };

  const handleCreate = () => {
    setEditingItem(null);
    setFormData({
      productColorCode: '',
      rawMaterialType: '',
      weightPerPiece: '',
      cavities: '',
      cycleTime: '',
      allowedMolds: [],
      allowedMachines: [],
      status: true
    });
    setShowDrawer(true);
  };

  const handleEdit = (item: any) => {
    setEditingItem(item);
    setFormData({
      productColorCode: item.productColorCode,
      rawMaterialType: item.rawMaterialType,
      weightPerPiece: item.weightPerPiece.toString(),
      cavities: item.cavities.toString(),
      cycleTime: item.cycleTime.toString(),
      allowedMolds: item.allowedMolds,
      allowedMachines: item.allowedMachines,
      status: item.status === 'Active'
    });
    setShowDrawer(true);
  };

  const handleSave = () => {
    console.log('Saving raw material:', formData);
    setShowDrawer(false);
  };

  const handleSaveAndNew = () => {
    console.log('Saving raw material:', formData);
    setFormData({
      productColorCode: '',
      rawMaterialType: formData.rawMaterialType, // Keep the same type for convenience
      weightPerPiece: '',
      cavities: '',
      cycleTime: '',
      allowedMolds: [],
      allowedMachines: [],
      status: true
    });
  };

  const handleAddNewType = () => {
    if (newTypeName && !allRawMaterialTypes.includes(newTypeName)) {
      setCustomRawMaterialTypes(prev => [...prev, newTypeName]);
      setFormData(prev => ({ ...prev, rawMaterialType: newTypeName }));
      setNewTypeName('');
      setShowNewTypeDialog(false);
    }
  };

  const toggleMoldSelection = (moldCode: string) => {
    setFormData(prev => ({
      ...prev,
      allowedMolds: prev.allowedMolds.includes(moldCode)
        ? prev.allowedMolds.filter(m => m !== moldCode)
        : [...prev.allowedMolds, moldCode]
    }));
  };

  const toggleMachineSelection = (machineCode: string) => {
    setFormData(prev => ({
      ...prev,
      allowedMachines: prev.allowedMachines.includes(machineCode)
        ? prev.allowedMachines.filter(m => m !== machineCode)
        : [...prev.allowedMachines, machineCode]
    }));
  };

  const filteredMaterials = mockRawMaterials.filter(material => {
    const matchesSearch = material.productColorCode.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         material.rawMaterialType.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesType = filterType === 'all' || material.rawMaterialType === filterType;
    const matchesStatus = filterStatus === 'all' || material.status.toLowerCase() === filterStatus.toLowerCase();
    return matchesSearch && matchesType && matchesStatus;
  });

  const exportToCSV = () => {
    console.log('Exporting to CSV...');
  };

  const exportToXLSX = () => {
    console.log('Exporting to XLSX...');
  };

  return (
    <div className="p-6 space-y-6">
      {/* Header */}
      <div>
        <h2 className="text-2xl font-semibold text-slate-900">
          Raw Material Registration | ကုန်ကြမ်းမှတ်ပုံတင်မှု
        </h2>
        <p className="text-slate-600 mt-1">
          All-in-one screen for managing raw material types and configuration with automatic calculations
        </p>
        <p className="text-sm text-slate-500">
          ကုန်ကြမ်းအမျိုးအစားများနှင့် အလိုအလျောက်တွက်ချက်မှုများပါရှိသော စီမံခန့်ခွဲမှုအတွက် အားလုံးပါ၀င်သောမျက်နှာပြင်
        </p>
      </div>

      {/* Helper Info */}
      <Alert>
        <Info className="h-4 w-4" />
        <AlertDescription>
          <div className="flex items-center justify-between">
            <div>
              <strong>System Helper:</strong> CT = seconds per shot | Bag = {CONSTANTS.BAG_WEIGHT_LB} lb = {(CONSTANTS.BAG_WEIGHT_LB * CONSTANTS.GRAMS_PER_POUND / 1000).toFixed(2)} kg | Auto-calculations update live
              <div className="text-xs text-slate-600 mt-1">
                စနစ်အကူအညီ: CT = တစ်ခါလုပ်ရန်စက��ကန့် | အိတ် = {CONSTANTS.BAG_WEIGHT_LB} ပေါင်ဒ် = {(CONSTANTS.BAG_WEIGHT_LB * CONSTANTS.GRAMS_PER_POUND / 1000).toFixed(2)} ကီလိုဂရမ် | အလိုအလျောက်တွက်ချက်မှုများ အမြန်အပ်ဒိတ်
              </div>
            </div>
          </div>
        </AlertDescription>
      </Alert>

      {/* Toolbar */}
      <div className="flex justify-between items-center">
        <div className="flex items-center gap-4">
          {/* Search */}
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-slate-400 h-4 w-4" />
            <Input
              placeholder="Search by product code or material type..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="pl-10 w-80"
            />
          </div>
          
          {/* Filter by Type */}
          <Select value={filterType} onValueChange={setFilterType}>
            <SelectTrigger className="w-40">
              <SelectValue placeholder="Filter by type" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">All Types</SelectItem>
              {allRawMaterialTypes.map((type) => (
                <SelectItem key={type} value={type}>{type}</SelectItem>
              ))}
            </SelectContent>
          </Select>

          {/* Filter by Status */}
          <Select value={filterStatus} onValueChange={setFilterStatus}>
            <SelectTrigger className="w-32">
              <SelectValue placeholder="Status" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">All Status</SelectItem>
              <SelectItem value="active">Active</SelectItem>
              <SelectItem value="inactive">Inactive</SelectItem>
            </SelectContent>
          </Select>

          <Button variant="outline" className="gap-2">
            <Filter className="h-4 w-4" />
            More Filters
          </Button>
        </div>

        <div className="flex items-center gap-3">
          <Button onClick={handleCreate} className="gap-2">
            <Plus className="h-4 w-4" />
            Add New Raw Material | ကုန်ကြမ်းအသစ်ထည့်ရန်
          </Button>
          <Button variant="outline" onClick={exportToCSV} className="gap-2">
            <Download className="h-4 w-4" />
            CSV
          </Button>
          <Button variant="outline" onClick={exportToXLSX} className="gap-2">
            <FileSpreadsheet className="h-4 w-4" />
            XLSX
          </Button>
        </div>
      </div>

      {/* Main Table */}
      <Card className="border-0 shadow-sm">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Package className="h-5 w-5 text-blue-600" />
            Raw Materials Registry | ကုန်ကြမ်းများမှတ်တမ်း
            <Badge variant="outline" className="ml-2">
              {filteredMaterials.length} items
            </Badge>
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Product Color Code | ထုတ်ကုန်အရောင်ကုဒ်</TableHead>
                  <TableHead>Raw Material Type | ကုန်ကြမ်းအမျိုးအစား</TableHead>
                  <TableHead>Weight (g) | အလေးချိန်</TableHead>
                  <TableHead>CT (sec) | စက်ခရီးအချိန်</TableHead>
                  <TableHead>Output/hr (auto) | တစ်နာရီထုတ်လုပ်မှု</TableHead>
                  <TableHead>Bags per 1000 pcs (auto) | ၁၀၀၀လုံးအတွက်အိတ်</TableHead>
                  <TableHead>Allowed Molds (#) | ခွင့်ပြုမှုရပုံစံများ</TableHead>
                  <TableHead>Allowed Machines (#) | ခွင့်ပြုမှုရစက်များ</TableHead>
                  <TableHead>Status | အခြေအနေ</TableHead>
                  <TableHead>Actions | လုပ်ဆောင်ချက်များ</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {filteredMaterials.map((material) => (
                  <TableRow key={material.id}>
                    <TableCell>
                      <Badge variant="outline" className="font-mono text-xs">
                        {material.productColorCode}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <Badge className="bg-purple-100 text-purple-800">
                        {material.rawMaterialType}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-1">
                        <span className="font-medium">{material.weightPerPiece}</span>
                        <span className="text-xs text-slate-500">g</span>
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-1">
                        <Clock className="h-3 w-3 text-blue-500" />
                        <span className="font-medium">{material.cycleTime}</span>
                        <span className="text-xs text-slate-500">sec</span>
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-1">
                        <Zap className="h-3 w-3 text-green-500" />
                        <span className="font-semibold text-green-700">{material.outputPerHour.toLocaleString()}</span>
                        <span className="text-xs text-slate-500">pcs/hr</span>
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-1">
                        <Scale className="h-3 w-3 text-orange-500" />
                        <span className="font-medium text-orange-700">{material.bagsPerThousand}</span>
                        <span className="text-xs text-slate-500">bags</span>
                      </div>
                    </TableCell>
                    <TableCell>
                      <Badge variant="outline" className="bg-blue-50">
                        {material.allowedMolds.length}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <Badge variant="outline" className="bg-green-50">
                        {material.allowedMachines.length}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <Badge className={
                        material.status === 'Active' 
                          ? 'bg-green-100 text-green-800' 
                          : 'bg-red-100 text-red-800'
                      }>
                        {material.status}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => handleEdit(material)}
                        className="gap-1"
                      >
                        <Edit className="h-3 w-3" />
                        Edit
                      </Button>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>

          {/* Pagination */}
          <div className="flex items-center justify-between space-x-2 py-4">
            <div className="text-sm text-slate-600">
              Showing {filteredMaterials.length} of {mockRawMaterials.length} materials
            </div>
            <div className="flex items-center gap-2">
              <Button variant="outline" size="sm" disabled>Previous</Button>
              <Button variant="outline" size="sm" className="bg-blue-50">1</Button>
              <Button variant="outline" size="sm" disabled>Next</Button>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Right-side Drawer */}
      <Sheet open={showDrawer} onOpenChange={setShowDrawer}>
        <SheetContent className="w-[800px] sm:max-w-[800px] overflow-y-auto">
          <SheetHeader>
            <SheetTitle className="flex items-center gap-2">
              <Package className="h-5 w-5 text-blue-600" />
              {editingItem ? 'Edit Raw Material' : 'Create New Raw Material'}
            </SheetTitle>
            <SheetDescription>
              {editingItem ? 'Update raw material configuration and calculations' : 'Add new raw material with automatic calculations'}
            </SheetDescription>
          </SheetHeader>

          <div className="space-y-6 py-6">
            {/* 1) General Info */}
            <div className="space-y-4">
              <div className="flex items-center gap-2">
                <Info className="h-4 w-4 text-slate-600" />
                <h4 className="font-medium text-slate-900">1. General Info | ယေဘုယျအချက်အလက်</h4>
              </div>
              
              <div className="grid grid-cols-2 gap-4 pl-6">
                <div>
                  <Label htmlFor="productColorCode">Product Color Code * | ထုတ်ကုန်အရောင်ကုဒ် *</Label>
                  <Select 
                    value={formData.productColorCode} 
                    onValueChange={(value) => setFormData(prev => ({ ...prev, productColorCode: value }))}
                  >
                    <SelectTrigger className="mt-1">
                      <SelectValue placeholder="Select from planner master..." />
                    </SelectTrigger>
                    <SelectContent>
                      {mockProductColors.map((pc) => (
                        <SelectItem key={pc.code} value={pc.code}>
                          <div className="flex flex-col items-start">
                            <span className="font-medium">{pc.code}</span>
                            <span className="text-xs text-slate-500">{pc.description}</span>
                          </div>
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>

                <div>
                  <Label htmlFor="rawMaterialType">Raw Material Type * | ကုန်ကြမ်းအမျိုးအစား *</Label>
                  <div className="flex gap-2 mt-1">
                    <Select 
                      value={formData.rawMaterialType} 
                      onValueChange={(value) => setFormData(prev => ({ ...prev, rawMaterialType: value }))}
                    >
                      <SelectTrigger>
                        <SelectValue placeholder="Select type..." />
                      </SelectTrigger>
                      <SelectContent>
                        {allRawMaterialTypes.map((type) => (
                          <SelectItem key={type} value={type}>{type}</SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                    <Dialog open={showNewTypeDialog} onOpenChange={setShowNewTypeDialog}>
                      <DialogTrigger asChild>
                        <Button variant="outline" size="sm" className="shrink-0">
                          <Plus className="h-3 w-3" />
                          Add New
                        </Button>
                      </DialogTrigger>
                      <DialogContent className="sm:max-w-[400px]">
                        <DialogHeader>
                          <DialogTitle>Add New Raw Material Type</DialogTitle>
                          <DialogDescription>
                            Create a new raw material type for the system.
                          </DialogDescription>
                        </DialogHeader>
                        <div className="space-y-4 py-4">
                          <div>
                            <Label htmlFor="newType">Type Name</Label>
                            <Input
                              id="newType"
                              value={newTypeName}
                              onChange={(e) => setNewTypeName(e.target.value)}
                              placeholder="e.g., Bio-PP, Special Mix"
                              className="mt-1"
                            />
                          </div>
                          <div className="flex justify-end gap-2">
                            <Button variant="outline" onClick={() => setShowNewTypeDialog(false)}>
                              Cancel
                            </Button>
                            <Button onClick={handleAddNewType} disabled={!newTypeName.trim()}>
                              Add Type
                            </Button>
                          </div>
                        </div>
                      </DialogContent>
                    </Dialog>
                  </div>
                </div>

                <div className="col-span-2">
                  <div className="flex items-center justify-between">
                    <Label htmlFor="status">Status | အခြေအနေ</Label>
                    <div className="flex items-center gap-2">
                      <span className={formData.status ? 'text-green-600' : 'text-red-600'}>
                        {formData.status ? 'Active | အသုံးပြုနေ' : 'Inactive | အသုံးမပြုပါ'}
                      </span>
                      <Switch
                        id="status"
                        checked={formData.status}
                        onCheckedChange={(checked) => setFormData(prev => ({ ...prev, status: checked }))}
                      />
                    </div>
                  </div>
                </div>
              </div>
            </div>

            {/* 2) Production Parameters */}
            <div className="space-y-4">
              <div className="flex items-center gap-2">
                <Settings className="h-4 w-4 text-slate-600" />
                <h4 className="font-medium text-slate-900">2. Production Parameters | ထုတ်လုပ်မှုအရင်းအမြစ်များ</h4>
              </div>
              
              <div className="grid grid-cols-3 gap-4 pl-6">
                <div>
                  <Label htmlFor="weightPerPiece">Weight per piece (g) * | တစ်ခုလျှင်အလေးချိန် *</Label>
                  <Input
                    id="weightPerPiece"
                    type="number"
                    step="0.1"
                    value={formData.weightPerPiece}
                    onChange={(e) => setFormData(prev => ({ ...prev, weightPerPiece: e.target.value }))}
                    placeholder="15.5"
                    className="mt-1"
                  />
                  <div className="text-xs text-slate-500 mt-1">
                    Weight of individual piece
                  </div>
                </div>

                <div>
                  <Label htmlFor="cavities">Cavities * | အပေါက်များ *</Label>
                  <Input
                    id="cavities"
                    type="number"
                    min="1"
                    value={formData.cavities}
                    onChange={(e) => setFormData(prev => ({ ...prev, cavities: e.target.value }))}
                    placeholder="4"
                    className="mt-1"
                  />
                  <div className="text-xs text-slate-500 mt-1">
                    Number of cavities in mold
                  </div>
                </div>

                <div>
                  <Label htmlFor="cycleTime">CT (sec) * | စက်ခရီးအချိန် *</Label>
                  <Input
                    id="cycleTime"
                    type="number"
                    step="0.1"
                    value={formData.cycleTime}
                    onChange={(e) => setFormData(prev => ({ ...prev, cycleTime: e.target.value }))}
                    placeholder="28"
                    className="mt-1"
                  />
                  <div className="text-xs text-slate-500 mt-1">
                    CT = seconds per shot
                  </div>
                </div>
              </div>

              {/* Auto-calculated values display */}
              <div className="grid grid-cols-2 gap-4 pl-6">
                <div className="p-4 bg-green-50 border border-green-200 rounded-lg">
                  <div className="flex items-center gap-2 mb-2">
                    <Zap className="h-4 w-4 text-green-600" />
                    <Label className="text-green-800 text-sm">Output/hr (auto)</Label>
                  </div>
                  <div className="text-2xl font-bold text-green-700">
                    {getOutputPerHour().toLocaleString()} pcs/hr
                  </div>
                  <div className="text-xs text-green-600 mt-1">
                    Formula: (3600 ÷ CT) × Cavities
                  </div>
                </div>

                <div className="p-4 bg-orange-50 border border-orange-200 rounded-lg">
                  <div className="flex items-center gap-2 mb-2">
                    <Scale className="h-4 w-4 text-orange-600" />
                    <Label className="text-orange-800 text-sm">Bags per 1000 pcs (auto)</Label>
                  </div>
                  <div className="text-2xl font-bold text-orange-700">
                    {getBagsPerThousand()} bags
                  </div>
                  <div className="text-xs text-orange-600 mt-1">
                    Using Bag = {(CONSTANTS.BAG_WEIGHT_LB * CONSTANTS.GRAMS_PER_POUND / 1000).toFixed(2)} kg
                  </div>
                </div>
              </div>

              {/* Constants helper */}
              <Alert className="ml-6">
                <Calculator className="h-4 w-4" />
                <AlertDescription className="text-sm">
                  <strong>Constants (read-only helper):</strong> 1 bag = {CONSTANTS.BAG_WEIGHT_LB} lb = {(CONSTANTS.BAG_WEIGHT_LB * CONSTANTS.GRAMS_PER_POUND / 1000).toFixed(2)} kg
                </AlertDescription>
              </Alert>
            </div>

            {/* 3) Compatibility Mapping */}
            <div className="space-y-4">
              <div className="flex items-center gap-2">
                <Wrench className="h-4 w-4 text-slate-600" />
                <h4 className="font-medium text-slate-900">3. Compatibility Mapping | လိုက်ဖက်မှုမြေပုံ</h4>
              </div>
              
              <div className="pl-6 space-y-4">
                <div>
                  <Label>Allowed Mold(s) | ခွင့်ပြုသောပုံစံများ</Label>
                  <div className="grid grid-cols-2 gap-2 mt-2 max-h-40 overflow-y-auto border rounded-lg p-3">
                    {mockMolds.map((mold) => (
                      <label key={mold.code} className="flex items-center gap-2 p-2 border rounded-lg cursor-pointer hover:bg-slate-50">
                        <input
                          type="checkbox"
                          checked={formData.allowedMolds.includes(mold.code)}
                          onChange={() => toggleMoldSelection(mold.code)}
                          className="rounded"
                        />
                        <div className="flex-1">
                          <div className="font-medium text-sm">{mold.code}</div>
                          <div className="text-xs text-slate-500">{mold.name}</div>
                        </div>
                      </label>
                    ))}
                  </div>
                  <div className="text-xs text-slate-500 mt-1">
                    Selected: {formData.allowedMolds.length} mold(s)
                  </div>
                </div>

                <div>
                  <Label>Allowed Machine(s) | ခွင့်ပြုသောစက်များ</Label>
                  <div className="grid grid-cols-2 gap-2 mt-2 max-h-40 overflow-y-auto border rounded-lg p-3">
                    {mockMachines.map((machine) => (
                      <label key={machine.code} className="flex items-center gap-2 p-2 border rounded-lg cursor-pointer hover:bg-slate-50">
                        <input
                          type="checkbox"
                          checked={formData.allowedMachines.includes(machine.code)}
                          onChange={() => toggleMachineSelection(machine.code)}
                          className="rounded"
                        />
                        <div className="flex-1">
                          <div className="font-medium text-sm">{machine.code}</div>
                          <div className="text-xs text-slate-500">{machine.name}</div>
                        </div>
                      </label>
                    ))}
                  </div>
                  <div className="text-xs text-slate-500 mt-1">
                    Selected: {formData.allowedMachines.length} machine(s)
                  </div>
                </div>
              </div>
            </div>

            {/* 4) Auto Preview Panel */}
            <div className="space-y-4">
              <div className="flex items-center gap-2">
                <Eye className="h-4 w-4 text-slate-600" />
                <h4 className="font-medium text-slate-900">4. Auto Preview Panel (read-only) | အလိုအလျောက်ကြည့်ရှုပုံ</h4>
              </div>
              
              <div className="pl-6 space-y-4">
                <div className="grid grid-cols-4 gap-4">
                  <div className="p-3 bg-blue-50 border border-blue-200 rounded-lg">
                    <div className="text-sm text-blue-600 mb-1">Weight per shot (g)</div>
                    <div className="text-lg font-semibold text-blue-700">
                      {getWeightPerShot()} g
                    </div>
                    <div className="text-xs text-blue-500 mt-1">
                      Weight × Cavities
                    </div>
                  </div>

                  <div className="p-3 bg-green-50 border border-green-200 rounded-lg">
                    <div className="text-sm text-green-600 mb-1">Units/hr (theoretical)</div>
                    <div className="text-lg font-semibold text-green-700">
                      {getTheoreticalUnitsPerHour().toLocaleString()}
                    </div>
                    <div className="text-xs text-green-500 mt-1">
                      100% efficiency
                    </div>
                  </div>

                  <div className="p-3 bg-teal-50 border border-teal-200 rounded-lg">
                    <div className="text-sm text-teal-600 mb-1">Units/hr (planned)</div>
                    <div className="text-lg font-semibold text-teal-700">
                      {getPlannedUnitsPerHour().toLocaleString()}
                    </div>
                    <div className="text-xs text-teal-500 mt-1">
                      85% efficiency
                    </div>
                  </div>

                  <div className="p-3 bg-purple-50 border border-purple-200 rounded-lg">
                    <div className="text-sm text-purple-600 mb-1">Bags for sample order</div>
                    <div className="text-lg font-semibold text-purple-700">
                      {getBagsForSampleOrder()} bags
                    </div>
                  </div>
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <Label htmlFor="sampleQty">Sample Order Qty | နမူနာအမှာစား</Label>
                    <Input
                      id="sampleQty"
                      type="number"
                      value={sampleOrderQty}
                      onChange={(e) => setSampleOrderQty(e.target.value)}
                      placeholder="10000"
                      className="mt-1"
                    />
                    <div className="text-xs text-slate-500 mt-1">
                      Enter quantity to see duration estimate
                    </div>
                  </div>
                  
                  <div className="p-4 bg-amber-50 border border-amber-200 rounded-lg">
                    <div className="text-sm text-amber-600 mb-1">Duration estimate</div>
                    <div className="text-2xl font-bold text-amber-700">
                      {getDurationEstimate()} hrs
                    </div>
                    <div className="text-xs text-amber-500 mt-1">
                      For {parseInt(sampleOrderQty || '0').toLocaleString()} pcs
                    </div>
                  </div>
                </div>
              </div>
            </div>

            {/* Audit Info */}
            {editingItem && (
              <div className="p-4 bg-slate-50 border rounded-lg">
                <div className="text-sm text-slate-600">
                  <div className="flex justify-between">
                    <div>
                      <div>Created by: <strong>{editingItem.createdBy}</strong> on {editingItem.createdOn}</div>
                      <div>Updated by: <strong>{editingItem.updatedBy}</strong> on {editingItem.updatedOn}</div>
                    </div>
                    <div>
                      Version: <strong>{editingItem.version}</strong>
                    </div>
                  </div>
                </div>
              </div>
            )}

            {/* Action Buttons */}
            <div className="flex justify-end gap-3 pt-4 border-t">
              <Button variant="outline" onClick={() => setShowDrawer(false)}>
                Cancel | ပယ်ဖျက်ပါ
              </Button>
              <Button variant="outline" onClick={handleSaveAndNew}>
                <Save className="h-4 w-4 mr-2" />
                Save & New | သိမ်းဆည်းပြီးအသစ်
              </Button>
              <Button onClick={handleSave}>
                <Save className="h-4 w-4 mr-2" />
                Save | သိမ်းဆည်းပါ
              </Button>
            </div>
          </div>
        </SheetContent>
      </Sheet>
    </div>
  );
}