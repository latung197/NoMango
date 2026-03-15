import React, { useState, useEffect } from 'react';
import { 
  Calendar, 
  Package, 
  Settings, 
  AlertTriangle, 
  CheckCircle, 
  XCircle, 
  Save, 
  Lock, 
  Send,
  ArrowLeft,
  Calculator,
  Info
} from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Textarea } from './ui/textarea';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Separator } from './ui/separator';
import { Alert, AlertDescription } from './ui/alert';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from './ui/dialog';
import { initialMappings, products, colors } from './ProductColorMapping';

// Use imported mock master data from ProductColorMapping
// Products, colors, and mappings are imported above

const molds = [
  { code: 'MLD-001', name: 'Bottle Mold A', capacity: 120, unit: 'pcs/hr' },
  { code: 'MLD-002', name: 'Container Mold B', capacity: 80, unit: 'pcs/hr' },
  { code: 'MLD-003', name: 'Cup Mold C', capacity: 200, unit: 'pcs/hr' },
  { code: 'MLD-004', name: 'Box Mold D', capacity: 60, unit: 'pcs/hr' },
  { code: 'MLD-005', name: 'Bag Mold E', capacity: 300, unit: 'pcs/hr' }
];

const machines = [
  { code: 'MCH-001', name: 'Injection Molding 01', type: 'Injection', status: 'available' },
  { code: 'MCH-002', name: 'Injection Molding 02', type: 'Injection', status: 'available' },
  { code: 'MCH-003', name: 'Blow Molding 01', type: 'Blow', status: 'maintenance' },
  { code: 'MCH-004', name: 'Extrusion Line 01', type: 'Extrusion', status: 'available' },
  { code: 'MCH-005', name: 'Extrusion Line 02', type: 'Extrusion', status: 'occupied' }
];

const materials = [
  { name: 'PP Resin Grade A', required: 500, available: 2500, unit: 'kg' },
  { name: 'PE Granules', required: 200, available: 180, unit: 'kg' },
  { name: 'PVC Compound', required: 150, available: 50, unit: 'kg' },
  { name: 'Color Masterbatch Blue', required: 25, available: 100, unit: 'kg' },
  { name: 'Additive Package', required: 10, available: 45, unit: 'kg' }
];

export function CreatePlanForm() {
  const [formData, setFormData] = useState({
    planId: '',
    status: 'draft',
    productColorMappingId: '',
    quantity: '',
    moldNo: '',
    machineNo: '',
    duration: '',
    durationOverride: false,
    overrideReason: '',
    remarks: ''
  });

  const [calculatedData, setCalculatedData] = useState({
    productName: '',
    colorName: '',
    fullCode: '',
    weightPerUnit: '',
    unit: '',
    estimatedDuration: 0
  });

  const [showOverrideDialog, setShowOverrideDialog] = useState(false);

  // Generate Plan ID on component mount
  useEffect(() => {
    const today = new Date();
    const dateStr = today.toISOString().slice(0, 10).replace(/-/g, '');
    const serial = String(Math.floor(Math.random() * 1000)).padStart(3, '0');
    setFormData(prev => ({ ...prev, planId: `PL${dateStr}${serial}` }));
  }, []);

  // Auto-fill product + color data from mapping selection
  useEffect(() => {
    if (formData.productColorMappingId) {
      const mapping = initialMappings.find(m => m.id === parseInt(formData.productColorMappingId));
      if (mapping) {
        const product = products.find(p => p.code === mapping.productCode);
        const color = colors.find(c => c.code === mapping.colorCode);
        if (product && color) {
          setCalculatedData(prev => ({
            ...prev,
            productName: product.name,
            colorName: color.name,
            fullCode: mapping.fullCode,
            weightPerUnit: product.weight.toString(),
            unit: product.unit
          }));
        }
      }
    } else {
      // Clear calculated data when no mapping is selected
      setCalculatedData(prev => ({
        ...prev,
        productName: '',
        colorName: '',
        fullCode: '',
        weightPerUnit: '',
        unit: ''
      }));
    }
  }, [formData.productColorMappingId]);

  // Auto-calculate duration
  useEffect(() => {
    if (formData.quantity && formData.moldNo && !formData.durationOverride) {
      const quantity = parseInt(formData.quantity);
      const mold = molds.find(m => m.code === formData.moldNo);
      if (quantity && mold) {
        const hours = quantity / mold.capacity;
        const calculatedDuration = Math.ceil(hours * 10) / 10; // Round to 1 decimal
        setCalculatedData(prev => ({ ...prev, estimatedDuration: calculatedDuration }));
        setFormData(prev => ({ ...prev, duration: calculatedDuration.toString() }));
      }
    }
  }, [formData.quantity, formData.moldNo, formData.durationOverride]);

  const getStatusBadge = (status: string) => {
    switch (status) {
      case 'draft':
        return <Badge className="bg-slate-100 text-slate-800 border-slate-300">📝 Draft</Badge>;
      case 'final':
        return <Badge className="bg-green-100 text-green-800 border-green-300">✅ Final</Badge>;
      default:
        return <Badge variant="secondary">{status}</Badge>;
    }
  };

  const getInventoryStatus = (required: number, available: number) => {
    if (available >= required) {
      return { status: 'ok', badge: <Badge className="bg-green-100 text-green-800 border-green-300">✅ OK</Badge> };
    } else if (available >= required * 0.8) {
      return { status: 'low', badge: <Badge className="bg-yellow-100 text-yellow-800 border-yellow-300">⚠️ Low Stock</Badge> };
    } else {
      return { status: 'insufficient', badge: <Badge className="bg-red-100 text-red-800 border-red-300">❌ Insufficient</Badge> };
    }
  };

  const getMachineStatusBadge = (status: string) => {
    switch (status) {
      case 'available':
        return <Badge className="bg-green-100 text-green-800 border-green-300">Available</Badge>;
      case 'occupied':
        return <Badge className="bg-yellow-100 text-yellow-800 border-yellow-300">Occupied</Badge>;
      case 'maintenance':
        return <Badge className="bg-red-100 text-red-800 border-red-300">Maintenance</Badge>;
      default:
        return <Badge variant="secondary">{status}</Badge>;
    }
  };

  const handleDurationOverride = () => {
    setFormData(prev => ({ ...prev, durationOverride: true }));
    setShowOverrideDialog(true);
  };

  const confirmDurationOverride = () => {
    setShowOverrideDialog(false);
  };

  const handleSaveDraft = () => {
    console.log('Saving as draft:', formData);
    // Handle save draft logic
  };

  const handleReserveMaterials = () => {
    console.log('Reserving materials for plan:', formData);
    // Handle material reservation logic
  };

  const handleSubmitForApproval = () => {
    console.log('Submitting for approval:', formData);
    setFormData(prev => ({ ...prev, status: 'pending_approval' }));
    // Handle submission logic
  };

  const selectedMapping = initialMappings.find(m => m.id === parseInt(formData.productColorMappingId));
  const selectedProduct = selectedMapping ? products.find(p => p.code === selectedMapping.productCode) : null;
  const selectedColor = selectedMapping ? colors.find(c => c.code === selectedMapping.colorCode) : null;
  const selectedMold = molds.find(m => m.code === formData.moldNo);
  const selectedMachine = machines.find(m => m.code === formData.machineNo);

  return (
    <div className="p-6 bg-slate-50 min-h-screen">
      <div className="max-w-6xl mx-auto space-y-6">
        {/* Header */}
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-4">
            <Button variant="ghost" size="sm">
              <ArrowLeft className="h-4 w-4 mr-2" />
              Back to Plans
            </Button>
            <div>
              <h1 className="text-2xl font-semibold text-slate-900">Create Production Plan</h1>
              <p className="text-sm text-slate-600">ထုတ်လုပ်မှုအစီအစဉ်ပြုလုပ်ခြင်း - Plan Creation & Resource Allocation</p>
            </div>
          </div>
          <div className="flex items-center gap-2">
            <div className="text-right">
              <div className="font-medium text-slate-900">{formData.planId}</div>
              <div className="text-sm text-slate-600">Plan ID</div>
            </div>
            {getStatusBadge(formData.status)}
          </div>
        </div>

        {/* Plan Details Section */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Package className="h-5 w-5 text-blue-600" />
              Plan Details
              <span className="text-sm font-normal text-slate-600">အသေးစိတ်အချက်အလက်များ</span>
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-6">
            {/* Two-column form layout */}
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
              {/* Left Column */}
              <div className="space-y-4">
                <div>
                  <Label>Product-Color Combination</Label>
                  <Select value={formData.productColorMappingId} onValueChange={(value) => setFormData(prev => ({ ...prev, productColorMappingId: value }))}>
                    <SelectTrigger>
                      <SelectValue placeholder="Select product-color combination" />
                    </SelectTrigger>
                    <SelectContent>
                      {initialMappings.filter(mapping => mapping.status === 'active').map(mapping => {
                        const product = products.find(p => p.code === mapping.productCode);
                        const color = colors.find(c => c.code === mapping.colorCode);
                        return (
                          <SelectItem key={mapping.id} value={mapping.id.toString()}>
                            <div className="flex items-center gap-2">
                              <div 
                                className="w-4 h-4 rounded border border-slate-300" 
                                style={{ backgroundColor: color?.rgba }}
                              />
                              <span className="font-mono text-blue-600">{mapping.fullCode}</span>
                              <span>- {product?.name} ({color?.name})</span>
                            </div>
                          </SelectItem>
                        );
                      })}
                    </SelectContent>
                  </Select>
                </div>

                <div>
                  <Label>Product Name</Label>
                  <Input 
                    value={calculatedData.productName} 
                    readOnly 
                    className="bg-slate-50" 
                    placeholder="Auto-filled from selection"
                  />
                </div>

                <div>
                  <Label>Color Name</Label>
                  <Input 
                    value={calculatedData.colorName} 
                    readOnly 
                    className="bg-slate-50" 
                    placeholder="Auto-filled from selection"
                  />
                </div>

                <div>
                  <Label>Quantity</Label>
                  <div className="flex gap-2">
                    <Input 
                      type="number" 
                      placeholder="Enter quantity"
                      value={formData.quantity}
                      onChange={(e) => setFormData(prev => ({ ...prev, quantity: e.target.value }))}
                    />
                    <div className="px-3 py-2 bg-slate-50 border rounded-md text-sm text-slate-600 min-w-16 flex items-center justify-center">
                      pcs
                    </div>
                  </div>
                </div>
              </div>

              {/* Right Column */}
              <div className="space-y-4">
                <div>
                  <Label>Full Product+Color Code</Label>
                  <Input 
                    value={calculatedData.fullCode} 
                    readOnly 
                    className="bg-slate-50 font-mono font-medium text-blue-600" 
                    placeholder="Auto-filled from selection"
                  />
                </div>

                <div>
                  <Label>Weight/Unit</Label>
                  <div className="flex gap-2">
                    <Input 
                      value={calculatedData.weightPerUnit} 
                      readOnly 
                      className="bg-slate-50" 
                      placeholder="Auto-filled from product"
                    />
                    <div className="px-3 py-2 bg-slate-50 border rounded-md text-sm text-slate-600 min-w-20 flex items-center justify-center">
                      {calculatedData.unit || 'unit'}
                    </div>
                  </div>
                </div>

                <div>
                  <Label>Mold No.</Label>
                  <Select value={formData.moldNo} onValueChange={(value) => setFormData(prev => ({ ...prev, moldNo: value }))}>
                    <SelectTrigger>
                      <SelectValue placeholder="Select mold" />
                    </SelectTrigger>
                    <SelectContent>
                      {molds.map(mold => (
                        <SelectItem key={mold.code} value={mold.code}>
                          {mold.code} - {mold.name} ({mold.capacity} {mold.unit})
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>

                <div>
                  <Label>Machine No.</Label>
                  <Select value={formData.machineNo} onValueChange={(value) => setFormData(prev => ({ ...prev, machineNo: value }))}>
                    <SelectTrigger>
                      <SelectValue placeholder="Select machine" />
                    </SelectTrigger>
                    <SelectContent>
                      {machines.map(machine => (
                        <SelectItem key={machine.code} value={machine.code} disabled={machine.status !== 'available'}>
                          <div className="flex items-center justify-between w-full">
                            <span>{machine.code} - {machine.name}</span>
                            <div className="ml-2">
                              {getMachineStatusBadge(machine.status)}
                            </div>
                          </div>
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>

                <div>
                  <Label className="flex items-center gap-2">
                    Duration (Hours)
                    {calculatedData.estimatedDuration > 0 && (
                      <Info className="h-4 w-4 text-slate-400" />
                    )}
                  </Label>
                  <div className="flex gap-2">
                    <Input 
                      type="number" 
                      value={formData.duration}
                      onChange={(e) => setFormData(prev => ({ ...prev, duration: e.target.value }))}
                      className={formData.durationOverride ? 'border-orange-300 bg-orange-50' : ''}
                    />
                    <Dialog open={showOverrideDialog} onOpenChange={setShowOverrideDialog}>
                      <DialogTrigger asChild>
                        <Button 
                          variant="outline" 
                          size="sm" 
                          onClick={handleDurationOverride}
                          className="min-w-20"
                        >
                          <Calculator className="h-4 w-4 mr-1" />
                          Override
                        </Button>
                      </DialogTrigger>
                      <DialogContent>
                        <DialogHeader>
                          <DialogTitle>Duration Override</DialogTitle>
                        </DialogHeader>
                        <div className="space-y-4">
                          <div>
                            <Label>New Duration (Hours)</Label>
                            <Input 
                              type="number" 
                              value={formData.duration}
                              onChange={(e) => setFormData(prev => ({ ...prev, duration: e.target.value }))}
                            />
                          </div>
                          <div>
                            <Label>Reason for Override</Label>
                            <Textarea 
                              placeholder="Explain why the duration needs to be overridden"
                              value={formData.overrideReason}
                              onChange={(e) => setFormData(prev => ({ ...prev, overrideReason: e.target.value }))}
                            />
                          </div>
                          <div className="flex justify-end gap-2">
                            <Button variant="outline" onClick={() => setShowOverrideDialog(false)}>
                              Cancel
                            </Button>
                            <Button onClick={confirmDurationOverride}>
                              Confirm Override
                            </Button>
                          </div>
                        </div>
                      </DialogContent>
                    </Dialog>
                  </div>
                  {calculatedData.estimatedDuration > 0 && !formData.durationOverride && (
                    <p className="text-sm text-slate-600 mt-1">
                      Calculated: {calculatedData.estimatedDuration} hours (Qty ÷ Mold capacity)
                    </p>
                  )}
                  {formData.durationOverride && (
                    <Alert className="mt-2">
                      <AlertTriangle className="h-4 w-4" />
                      <AlertDescription>
                        Duration has been overridden. Original estimate: {calculatedData.estimatedDuration} hours
                      </AlertDescription>
                    </Alert>
                  )}
                </div>
              </div>
            </div>

            {/* Additional fields */}
            <Separator />
            <div>
              <Label>Remarks</Label>
              <Textarea 
                placeholder="Additional notes or special instructions"
                value={formData.remarks}
                onChange={(e) => setFormData(prev => ({ ...prev, remarks: e.target.value }))}
              />
            </div>
          </CardContent>
        </Card>

        {/* Inventory Check Section */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Settings className="h-5 w-5 text-green-600" />
              Inventory Check
              <span className="text-sm font-normal text-slate-600">ပစ္စည်းများစစ်ဆေးခြင်း</span>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Material</TableHead>
                    <TableHead className="text-center">Required</TableHead>
                    <TableHead className="text-center">Available</TableHead>
                    <TableHead className="text-center">Status</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {materials.map((material, index) => {
                    const inventoryStatus = getInventoryStatus(material.required, material.available);
                    return (
                      <TableRow key={index}>
                        <TableCell className="font-medium">{material.name}</TableCell>
                        <TableCell className="text-center">
                          {material.required} {material.unit}
                        </TableCell>
                        <TableCell className="text-center">
                          {material.available} {material.unit}
                        </TableCell>
                        <TableCell className="text-center">
                          {inventoryStatus.badge}
                        </TableCell>
                      </TableRow>
                    );
                  })}
                </TableBody>
              </Table>
            </div>
            
            {/* Inventory Summary */}
            <div className="mt-4 p-4 bg-slate-50 rounded-lg">
              <div className="flex items-center gap-4 text-sm">
                <div className="flex items-center gap-2">
                  <CheckCircle className="h-4 w-4 text-green-600" />
                  <span>3 OK</span>
                </div>
                <div className="flex items-center gap-2">
                  <AlertTriangle className="h-4 w-4 text-yellow-600" />
                  <span>1 Low Stock</span>
                </div>
                <div className="flex items-center gap-2">
                  <XCircle className="h-4 w-4 text-red-600" />
                  <span>1 Insufficient</span>
                </div>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Action Buttons */}
        <div className="sticky bottom-0 bg-white border-t border-slate-200 p-4 -mx-6">
          <div className="max-w-6xl mx-auto">
            <div className="flex flex-col sm:flex-row gap-3 justify-end">
              <Button 
                variant="outline" 
                onClick={handleSaveDraft}
                className="min-h-[44px]"
              >
                <Save className="h-4 w-4 mr-2" />
                Save as Draft
              </Button>
              <Button 
                onClick={handleReserveMaterials}
                className="bg-blue-600 hover:bg-blue-700 text-white min-h-[44px]"
                disabled={materials.some(m => getInventoryStatus(m.required, m.available).status === 'insufficient')}
              >
                <Lock className="h-4 w-4 mr-2" />
                Reserve Materials
              </Button>
              <Button 
                onClick={handleSubmitForApproval}
                className="bg-green-600 hover:bg-green-700 text-white min-h-[44px]"
                disabled={!formData.productColorMappingId || !formData.quantity || !formData.moldNo || !formData.machineNo}
              >
                <Send className="h-4 w-4 mr-2" />
                Submit for Approval
              </Button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}