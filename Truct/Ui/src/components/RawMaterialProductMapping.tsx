import React, { useState } from 'react';
import {
  ArrowRightLeft,
  Plus,
  Search,
  Edit,
  Trash2,
  Save,
  X,
  Calculator,
  Package,
  Factory,
  Clock,
  TrendingUp,
  Settings,
  Info
} from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Badge } from './ui/badge';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogTrigger } from './ui/dialog';
import { Textarea } from './ui/textarea';
import { Checkbox } from './ui/checkbox';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from './ui/tooltip';
import { toast } from 'sonner@2.0.3';

// Mock data for products (from Product Master)
const mockProducts = [
  {
    code: 'PB500',
    name: '2011 - Plastic Bottle 500ml',
    photo: '/api/placeholder/60/60'
  },
  {
    code: 'PC1000',
    name: '2012 - Plastic Container 1L',
    photo: '/api/placeholder/60/60'
  },
  {
    code: 'PC250',
    name: '2013 - Plastic Cup 250ml',
    photo: '/api/placeholder/60/60'
  },
  {
    code: 'PL100',
    name: '2014 - Plastic Lid 100ml',
    photo: '/api/placeholder/60/60'
  },
  {
    code: 'PT-L',
    name: '2015 - Plastic Tray Large',
    photo: '/api/placeholder/60/60'
  }
];

// Mock data for raw materials (from Raw Material Registration)
const mockRawMaterials = [
  { code: 'PET001', name: 'PET Resin Clear', type: 'Pellet' },
  { code: 'PP002', name: 'PP Blue Compound', type: 'Compound' },
  { code: 'PS001', name: 'PS Clear', type: 'Pellet' },
  { code: 'ABS001', name: 'ABS Black', type: 'Pellet' },
  { code: 'ADD001', name: 'Additive Blue', type: 'Other' },
  { code: 'ADD002', name: 'UV Stabilizer', type: 'Other' },
  { code: 'ADD003', name: 'Flame Retardant', type: 'Other' }
];

// Mock data for molds
const mockMolds = [
  'INJ-M001', 'INJ-M002', 'INJ-M003', 'INJ-M004', 'INJ-M005',
  'INJ-M006', 'INJ-M007', 'INJ-M008', 'INJ-M009', 'INJ-M010'
];

// Mock data for existing mappings
const mockMappings = [
  {
    id: '1',
    productCode: 'PB500',
    productName: '2011 - Plastic Bottle 500ml',
    rawMaterialCode: 'PET001',
    rawMaterialName: 'PET Resin Clear',
    weightPerPiece: 25,
    cavitiesPerMold: 4,
    cycleTime: 45,
    outputPerHour: 320,
    bagsPerThousandPcs: 1.0,
    allowedMolds: ['INJ-M001', 'INJ-M002', 'INJ-M003'],
    notes: 'Standard bottle production setup'
  },
  {
    id: '2',
    productCode: 'PC1000',
    productName: '2012 - Plastic Container 1L',
    rawMaterialCode: 'PP002',
    rawMaterialName: 'PP Blue Compound',
    weightPerPiece: 50,
    cavitiesPerMold: 2,
    cycleTime: 38,
    outputPerHour: 189,
    bagsPerThousandPcs: 2.0,
    allowedMolds: ['INJ-M002', 'INJ-M004'],
    notes: 'Large container requiring precise temperature control'
  },
  {
    id: '3',
    productCode: 'PC250',
    productName: '2013 - Plastic Cup 250ml',
    rawMaterialCode: 'PS001',
    rawMaterialName: 'PS Clear',
    weightPerPiece: 15,
    cavitiesPerMold: 8,
    cycleTime: 30,
    outputPerHour: 960,
    bagsPerThousandPcs: 0.6,
    allowedMolds: ['INJ-M003', 'INJ-M005', 'INJ-M006'],
    notes: 'High volume production with 8-cavity mold'
  },
  {
    id: '4',
    productCode: 'PL100',
    productName: '2014 - Plastic Lid 100ml',
    rawMaterialCode: 'PP002',
    rawMaterialName: 'PP Blue Compound',
    weightPerPiece: 8,
    cavitiesPerMold: 16,
    cycleTime: 15,
    outputPerHour: 3840,
    bagsPerThousandPcs: 0.32,
    allowedMolds: ['INJ-M004', 'INJ-M007', 'INJ-M008'],
    notes: 'Ultra high speed production setup'
  }
];

interface RawMaterialProductMappingProps {
  currentPage: string;
  onPageChange: (page: string) => void;
}

export function RawMaterialProductMapping({ currentPage, onPageChange }: RawMaterialProductMappingProps) {
  const [mappings, setMappings] = useState(mockMappings);
  const [searchTerm, setSearchTerm] = useState('');
  const [isAddDialogOpen, setIsAddDialogOpen] = useState(false);
  const [editingMapping, setEditingMapping] = useState<any>(null);
  const [formData, setFormData] = useState({
    productCode: '',
    rawMaterialCode: '',
    weightPerPiece: '',
    cavitiesPerMold: '',
    cycleTime: '',
    allowedMolds: [] as string[],
    notes: ''
  });

  // Calculate auto-calculations
  const outputPerHour = formData.cycleTime && formData.cavitiesPerMold 
    ? Math.round((3600 / parseFloat(formData.cycleTime)) * parseFloat(formData.cavitiesPerMold))
    : 0;

  const bagsPerThousandPcs = formData.weightPerPiece 
    ? parseFloat(((parseFloat(formData.weightPerPiece) * 1000) / 25000).toFixed(2))
    : 0;

  // Filter mappings based on search term
  const filteredMappings = mappings.filter(mapping =>
    mapping.productCode.toLowerCase().includes(searchTerm.toLowerCase()) ||
    mapping.productName.toLowerCase().includes(searchTerm.toLowerCase()) ||
    mapping.rawMaterialCode.toLowerCase().includes(searchTerm.toLowerCase()) ||
    mapping.rawMaterialName.toLowerCase().includes(searchTerm.toLowerCase())
  );

  // Handle form submission
  const handleSubmit = () => {
    // Validate required fields
    if (!formData.productCode || !formData.rawMaterialCode || !formData.weightPerPiece || 
        !formData.cavitiesPerMold || !formData.cycleTime) {
      toast.error('Please fill in all required fields');
      return;
    }

    // Check for duplicate mapping
    const isDuplicate = mappings.some(
      m => m.productCode === formData.productCode && 
           m.rawMaterialCode === formData.rawMaterialCode && 
           (!editingMapping || m.id !== editingMapping.id)
    );
    
    if (isDuplicate) {
      toast.error('This Product-Material mapping already exists');
      return;
    }

    const product = mockProducts.find(p => p.code === formData.productCode);
    const rawMaterial = mockRawMaterials.find(r => r.code === formData.rawMaterialCode);

    const mappingData = {
      productCode: formData.productCode,
      productName: product?.name || '',
      rawMaterialCode: formData.rawMaterialCode,
      rawMaterialName: rawMaterial?.name || '',
      weightPerPiece: parseFloat(formData.weightPerPiece),
      cavitiesPerMold: parseFloat(formData.cavitiesPerMold),
      cycleTime: parseFloat(formData.cycleTime),
      outputPerHour,
      bagsPerThousandPcs,
      allowedMolds: formData.allowedMolds,
      notes: formData.notes
    };

    if (editingMapping) {
      // Update existing mapping
      setMappings(mappings.map(m => 
        m.id === editingMapping.id 
          ? { ...m, ...mappingData }
          : m
      ));
      toast.success('✅ Product Mapping Updated!');
      setEditingMapping(null);
    } else {
      // Add new mapping
      const newMapping = {
        ...mappingData,
        id: (mappings.length + 1).toString()
      };
      setMappings([...mappings, newMapping]);
      toast.success('✅ Product Mapping Saved!');
    }

    // Reset form and close dialog
    handleDialogClose();
  };

  // Handle edit
  const handleEdit = (mapping: any) => {
    setEditingMapping(mapping);
    setFormData({
      productCode: mapping.productCode,
      rawMaterialCode: mapping.rawMaterialCode,
      weightPerPiece: mapping.weightPerPiece.toString(),
      cavitiesPerMold: mapping.cavitiesPerMold.toString(),
      cycleTime: mapping.cycleTime.toString(),
      allowedMolds: mapping.allowedMolds || [],
      notes: mapping.notes || ''
    });
    setIsAddDialogOpen(true);
  };

  // Handle delete
  const handleDelete = (mappingId: string) => {
    setMappings(mappings.filter(m => m.id !== mappingId));
    toast.success('Mapping deleted successfully');
  };

  // Handle mold selection
  const handleMoldToggle = (moldCode: string) => {
    setFormData(prev => ({
      ...prev,
      allowedMolds: prev.allowedMolds.includes(moldCode)
        ? prev.allowedMolds.filter(m => m !== moldCode)
        : [...prev.allowedMolds, moldCode]
    }));
  };

  // Handle dialog open/close
  const handleDialogOpenChange = (open: boolean) => {
    setIsAddDialogOpen(open);
    if (!open) {
      setEditingMapping(null);
      setFormData({
        productCode: '',
        rawMaterialCode: '',
        weightPerPiece: '',
        cavitiesPerMold: '',
        cycleTime: '',
        allowedMolds: [],
        notes: ''
      });
    }
  };

  // Reset form when dialog closes
  const handleDialogClose = () => {
    setIsAddDialogOpen(false);
    setEditingMapping(null);
    setFormData({
      productCode: '',
      rawMaterialCode: '',
      weightPerPiece: '',
      cavitiesPerMold: '',
      cycleTime: '',
      allowedMolds: [],
      notes: ''
    });
  };

  return (
    <TooltipProvider>
      <div className="space-y-6">
        {/* Page Header */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <ArrowRightLeft className="h-5 w-5 text-blue-600" />
              Raw Material ↔ Product Mapping
              <span className="text-sm font-normal text-slate-600">ကုန်ကြမ်း–ထုတ်ကုန် ချိတ်ဆက်</span>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="flex gap-3 mb-6">
              <div className="flex items-center gap-2 bg-blue-50 px-3 py-2 rounded-lg">
                <ArrowRightLeft className="h-4 w-4 text-blue-600" />
                <span className="text-sm">⚙️ For planners/engineers to define raw material consumption per product</span>
              </div>
            </div>

            {/* Action Bar */}
            <div className="flex gap-4 items-center justify-between mb-6">
              <div className="flex-1">
                <div className="relative">
                  <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-slate-400 h-4 w-4" />
                  <Input
                    placeholder="Search mappings by product or material..."
                    className="pl-10"
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                  />
                </div>
              </div>
              <Dialog open={isAddDialogOpen} onOpenChange={handleDialogOpenChange}>
                <DialogTrigger asChild>
                  <Button className="bg-blue-600 hover:bg-blue-700">
                    <Plus className="h-4 w-4 mr-2" />
                    ➕ Add New Mapping
                  </Button>
                </DialogTrigger>
                <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
                  <DialogHeader>
                    <DialogTitle className="flex items-center gap-2">
                      <ArrowRightLeft className="h-5 w-5 text-blue-600" />
                      {editingMapping ? 'Edit Product Mapping' : 'Add New Product Mapping'}
                    </DialogTitle>
                    <DialogDescription>
                      Define how raw materials are consumed per product. All calculations are automated.
                    </DialogDescription>
                  </DialogHeader>
                  <div className="space-y-6">
                    {/* Product and Material Selection */}
                    <div className="grid grid-cols-2 gap-4">
                      <div>
                        <Label className="text-sm font-medium">Product Code *</Label>
                        <Select value={formData.productCode} onValueChange={(value) => setFormData({ ...formData, productCode: value })}>
                          <SelectTrigger className="mt-1">
                            <SelectValue placeholder="Select product" />
                          </SelectTrigger>
                          <SelectContent>
                            {mockProducts.map((product) => (
                              <SelectItem key={product.code} value={product.code}>
                                <div className="flex items-center gap-2">
                                  <div className="w-6 h-6 bg-slate-200 rounded"></div>
                                  <div>
                                    <div className="font-medium">{product.code}</div>
                                    <div className="text-xs text-slate-600">{product.name}</div>
                                  </div>
                                </div>
                              </SelectItem>
                            ))}
                          </SelectContent>
                        </Select>
                      </div>
                      <div>
                        <Label className="text-sm font-medium">Raw Material Code *</Label>
                        <Select value={formData.rawMaterialCode} onValueChange={(value) => setFormData({ ...formData, rawMaterialCode: value })}>
                          <SelectTrigger className="mt-1">
                            <SelectValue placeholder="Select raw material" />
                          </SelectTrigger>
                          <SelectContent>
                            {mockRawMaterials.map((material) => (
                              <SelectItem key={material.code} value={material.code}>
                                <div className="flex items-center gap-2">
                                  <Package className="h-4 w-4 text-slate-500" />
                                  <div>
                                    <div className="font-medium">{material.code}</div>
                                    <div className="text-xs text-slate-600">{material.name} ({material.type})</div>
                                  </div>
                                </div>
                              </SelectItem>
                            ))}
                          </SelectContent>
                        </Select>
                      </div>
                    </div>

                    {/* Production Parameters */}
                    <div className="grid grid-cols-3 gap-4">
                      <div>
                        <Label className="text-sm font-medium flex items-center gap-1">
                          Weight per Piece (g) *
                          <Tooltip>
                            <TooltipTrigger>
                              <Info className="h-3 w-3 text-slate-400" />
                            </TooltipTrigger>
                            <TooltipContent>
                              <p>Weight of raw material needed per single product piece</p>
                            </TooltipContent>
                          </Tooltip>
                        </Label>
                        <Input
                          type="number"
                          placeholder="e.g., 25"
                          value={formData.weightPerPiece}
                          onChange={(e) => setFormData({ ...formData, weightPerPiece: e.target.value })}
                          className="mt-1"
                        />
                      </div>
                      <div>
                        <Label className="text-sm font-medium flex items-center gap-1">
                          Cavities per Mold *
                          <Tooltip>
                            <TooltipTrigger>
                              <Info className="h-3 w-3 text-slate-400" />
                            </TooltipTrigger>
                            <TooltipContent>
                              <p>Number of product pieces produced per mold cycle</p>
                            </TooltipContent>
                          </Tooltip>
                        </Label>
                        <Input
                          type="number"
                          placeholder="e.g., 4"
                          value={formData.cavitiesPerMold}
                          onChange={(e) => setFormData({ ...formData, cavitiesPerMold: e.target.value })}
                          className="mt-1"
                        />
                      </div>
                      <div>
                        <Label className="text-sm font-medium flex items-center gap-1">
                          Cycle Time (sec) *
                          <Tooltip>
                            <TooltipTrigger>
                              <Info className="h-3 w-3 text-slate-400" />
                            </TooltipTrigger>
                            <TooltipContent>
                              <p>Time taken for one complete molding cycle</p>
                            </TooltipContent>
                          </Tooltip>
                        </Label>
                        <Input
                          type="number"
                          placeholder="e.g., 45"
                          value={formData.cycleTime}
                          onChange={(e) => setFormData({ ...formData, cycleTime: e.target.value })}
                          className="mt-1"
                        />
                      </div>
                    </div>

                    {/* Auto-calculation Cards */}
                    <div className="grid grid-cols-2 gap-4">
                      <Card className="bg-green-50 border-green-200">
                        <CardContent className="p-4">
                          <div className="flex items-center gap-2 mb-2">
                            <TrendingUp className="h-4 w-4 text-green-600" />
                            <Label className="text-sm font-medium text-green-800">Output/hr (Auto-calculated)</Label>
                            <Tooltip>
                              <TooltipTrigger>
                                <Info className="h-3 w-3 text-green-600" />
                              </TooltipTrigger>
                              <TooltipContent>
                                <p>Formula: (3600 ÷ Cycle Time) × Cavities</p>
                              </TooltipContent>
                            </Tooltip>
                          </div>
                          <div className="text-2xl font-bold text-green-700">
                            {outputPerHour.toLocaleString()} pcs/hr
                          </div>
                        </CardContent>
                      </Card>
                      <Card className="bg-orange-50 border-orange-200">
                        <CardContent className="p-4">
                          <div className="flex items-center gap-2 mb-2">
                            <Calculator className="h-4 w-4 text-orange-600" />
                            <Label className="text-sm font-medium text-orange-800">Bags/1000 pcs (Auto-calculated)</Label>
                            <Tooltip>
                              <TooltipTrigger>
                                <Info className="h-3 w-3 text-orange-600" />
                              </TooltipTrigger>
                              <TooltipContent className="max-w-xs">
                                <p className="font-medium mb-1">Formula: (Weight × 1000) ÷ 25,000g</p>
                                <p className="text-xs text-slate-200">Unit Conversions (defined in Raw Material Registration):</p>
                                <div className="text-xs mt-1 space-y-1">
                                  <div>• 1 bag = 25 kg (for kg/bag units)</div>
                                  <div>• 1 roll = 100 m (for roll units)</div>
                                  <div>• 1 roll = 100 pcs (for piece units)</div>
                                </div>
                              </TooltipContent>
                            </Tooltip>
                          </div>
                          <div className="text-2xl font-bold text-orange-700">
                            {bagsPerThousandPcs} bags
                          </div>
                        </CardContent>
                      </Card>
                    </div>

                    {/* Allowed Molds */}
                    <div>
                      <Label className="text-sm font-medium mb-3 block">Allowed Molds (Select applicable machines)</Label>
                      <div className="grid grid-cols-5 gap-2">
                        {mockMolds.map((mold) => (
                          <div key={mold} className="flex items-center space-x-2">
                            <Checkbox
                              id={mold}
                              checked={formData.allowedMolds.includes(mold)}
                              onCheckedChange={() => handleMoldToggle(mold)}
                            />
                            <Label htmlFor={mold} className="text-sm font-mono cursor-pointer">
                              {mold}
                            </Label>
                          </div>
                        ))}
                      </div>
                    </div>

                    {/* Notes */}
                    <div>
                      <Label className="text-sm font-medium">Notes (Optional)</Label>
                      <Textarea
                        placeholder="Additional notes about this mapping..."
                        value={formData.notes}
                        onChange={(e) => setFormData({ ...formData, notes: e.target.value })}
                        className="mt-1"
                        rows={3}
                      />
                    </div>

                    {/* Action Buttons */}
                    <div className="flex gap-2 pt-4">
                      <Button onClick={handleSubmit} className="bg-blue-600 hover:bg-blue-700">
                        <Save className="h-4 w-4 mr-2" />
                        {editingMapping ? 'Update Mapping' : 'Save Mapping'}
                      </Button>
                      <Button variant="outline" onClick={handleDialogClose}>
                        <X className="h-4 w-4 mr-2" />
                        Cancel
                      </Button>
                    </div>
                  </div>
                </DialogContent>
              </Dialog>
            </div>
          </CardContent>
        </Card>

        {/* Mappings Table */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Settings className="h-5 w-5 text-slate-700" />
              Product-Material Mappings
              <Badge className="bg-slate-100 text-slate-800 ml-auto">
                {filteredMappings.length} mappings
              </Badge>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Product Code</TableHead>
                    <TableHead>Raw Material Code</TableHead>
                    <TableHead>Weight (g)</TableHead>
                    <TableHead>CT (sec)</TableHead>
                    <TableHead>Output/hr</TableHead>
                    <TableHead>Bags/1000 pcs</TableHead>
                    <TableHead>Allowed Molds</TableHead>
                    <TableHead>Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {filteredMappings.map((mapping) => (
                    <TableRow key={mapping.id}>
                      <TableCell>
                        <div className="font-medium text-blue-600">{mapping.productCode}</div>
                        <div className="text-sm text-slate-600 max-w-[150px] truncate" title={mapping.productName}>
                          {mapping.productName}
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="font-medium font-mono">{mapping.rawMaterialCode}</div>
                        <div className="text-sm text-slate-600 max-w-[150px] truncate" title={mapping.rawMaterialName}>
                          {mapping.rawMaterialName}
                        </div>
                      </TableCell>
                      <TableCell className="font-mono">{mapping.weightPerPiece}g</TableCell>
                      <TableCell className="font-mono">{mapping.cycleTime}s</TableCell>
                      <TableCell>
                        <div className="font-medium text-green-600">{mapping.outputPerHour.toLocaleString()}</div>
                        <div className="text-xs text-slate-600">pcs/hr</div>
                      </TableCell>
                      <TableCell>
                        <div className="font-medium text-orange-600">{mapping.bagsPerThousandPcs}</div>
                        <div className="text-xs text-slate-600">bags</div>
                      </TableCell>
                      <TableCell>
                        <div className="flex flex-wrap gap-1">
                          {mapping.allowedMolds.slice(0, 2).map(mold => (
                            <Badge key={mold} variant="outline" className="text-xs font-mono">
                              {mold}
                            </Badge>
                          ))}
                          {mapping.allowedMolds.length > 2 && (
                            <Badge variant="outline" className="text-xs">
                              +{mapping.allowedMolds.length - 2}
                            </Badge>
                          )}
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="flex gap-1">
                          <Button
                            size="sm"
                            variant="ghost"
                            onClick={() => handleEdit(mapping)}
                            className="h-8 w-8 p-0"
                          >
                            <Edit className="h-4 w-4" />
                          </Button>
                          <Button
                            size="sm"
                            variant="ghost"
                            onClick={() => handleDelete(mapping.id)}
                            className="h-8 w-8 p-0 text-red-600 hover:text-red-700 hover:bg-red-50"
                          >
                            <Trash2 className="h-4 w-4" />
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
    </TooltipProvider>
  );
}