import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '../ui/card';
import { Button } from '../ui/button';
import { Input } from '../ui/input';
import { Label } from '../ui/label';
import { Badge } from '../ui/badge';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../ui/table';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../ui/select';
import { Sheet, SheetContent, SheetDescription, SheetHeader, SheetTitle } from '../ui/sheet';
import { 
  Search, 
  Filter, 
  Download, 
  Plus, 
  Edit, 
  Save,
  Scale,
  Calculator
} from 'lucide-react';

// Mock data
const mockWeightRegisters = [
  {
    id: 1,
    productColorCode: 'CUP-250-RED-001',
    partWeight: 15.5,
    runnerWeight: 12.2,
    cavities: 4,
    weightPerShot: 111.2,
    notes: 'Standard 250ml cup in red'
  },
  {
    id: 2,
    productColorCode: 'CUP-500-BLUE-002',
    partWeight: 28.0,
    runnerWeight: 18.5,
    cavities: 2,
    weightPerShot: 93.0,
    notes: 'Large cup variant'
  },
  {
    id: 3,
    productColorCode: 'LID-UNIV-WHITE-001',
    partWeight: 3.2,
    runnerWeight: 8.4,
    cavities: 12,
    weightPerShot: 139.2,
    notes: 'Universal lid design'
  }
];

const mockProductColors = [
  { code: 'CUP-250-RED-001', product: 'CUP-250', color: 'RED-001' },
  { code: 'CUP-500-BLUE-002', product: 'CUP-500', color: 'BLUE-002' },
  { code: 'LID-UNIV-WHITE-001', product: 'LID-UNIV', color: 'WHITE-001' },
  { code: 'CUP-250-BLUE-002', product: 'CUP-250', color: 'BLUE-002' }
];

export function WeightRegister() {
  const [searchTerm, setSearchTerm] = useState('');
  const [showDrawer, setShowDrawer] = useState(false);
  const [editingItem, setEditingItem] = useState<any>(null);
  const [formData, setFormData] = useState({
    productColorCode: '',
    partWeight: '',
    runnerWeight: '',
    cavities: '',
    notes: ''
  });

  const calculateWeightPerShot = () => {
    const partWeight = parseFloat(formData.partWeight) || 0;
    const runnerWeight = parseFloat(formData.runnerWeight) || 0;
    const cavities = parseInt(formData.cavities) || 1;
    return (partWeight + runnerWeight) * cavities;
  };

  const handleCreate = () => {
    setEditingItem(null);
    setFormData({
      productColorCode: '',
      partWeight: '',
      runnerWeight: '',
      cavities: '',
      notes: ''
    });
    setShowDrawer(true);
  };

  const handleEdit = (item: any) => {
    setEditingItem(item);
    setFormData({
      productColorCode: item.productColorCode,
      partWeight: item.partWeight.toString(),
      runnerWeight: item.runnerWeight.toString(),
      cavities: item.cavities.toString(),
      notes: item.notes || ''
    });
    setShowDrawer(true);
  };

  const handleSave = () => {
    const weightPerShot = calculateWeightPerShot();
    console.log('Saving weight register:', { ...formData, weightPerShot });
    setShowDrawer(false);
  };

  const handleSaveAndNew = () => {
    const weightPerShot = calculateWeightPerShot();
    console.log('Saving weight register:', { ...formData, weightPerShot });
    setFormData({
      productColorCode: '',
      partWeight: '',
      runnerWeight: '',
      cavities: '',
      notes: ''
    });
  };

  const filteredRegisters = mockWeightRegisters.filter(register =>
    register.productColorCode.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="p-6 space-y-6">
      {/* Header */}
      <div>
        <h2 className="text-2xl font-semibold text-slate-900">
          Weight Register | အလေးချိန်မှတ်ပုံတင်
        </h2>
        <p className="text-slate-600 mt-1">
          Register part weights, runner weights, and cavities by product color
        </p>
        <p className="text-sm text-slate-500">
          ထုတ်ကုန်အရောင်အလိုက် အစိတ်အပိုင်းအလေးချိန်၊ ပြေးလမ်းအလေးချိန်နှင့် အပေါက်များကို မှတ်ပုံတင်ခြင်း
        </p>
      </div>

      {/* Toolbar */}
      <div className="flex justify-between items-center">
        <div className="flex items-center gap-4">
          {/* Search */}
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-slate-400 h-4 w-4" />
            <Input
              placeholder="Search by product color code..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="pl-10 w-80"
            />
          </div>
          
          {/* Filters */}
          <Button variant="outline" className="gap-2">
            <Filter className="h-4 w-4" />
            Filters
          </Button>
        </div>

        <div className="flex items-center gap-3">
          <Button variant="outline" className="gap-2">
            <Download className="h-4 w-4" />
            Export CSV
          </Button>
          <Button onClick={handleCreate} className="gap-2">
            <Plus className="h-4 w-4" />
            Add Weight Record | အလေးချိန်မှတ်တမ်းထည့်သွင်းရန်
          </Button>
        </div>
      </div>

      {/* Main Table */}
      <Card className="border-0 shadow-sm">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Scale className="h-5 w-5 text-green-600" />
            Weight Records | အလေးချိန်မှတ်တမ်းများ
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Product Color Code | ထုတ်ကုန်အရောင်ကုဒ်</TableHead>
                  <TableHead>Part Wt (g) | အစိတ်အလေးချိန်</TableHead>
                  <TableHead>Runner Wt (g) | ပြေးလမ်းအလေးချိန်</TableHead>
                  <TableHead>Cavities | အပေါက်များ</TableHead>
                  <TableHead>Wt/Shot (g) | တစ်ခါအလေးချိန်</TableHead>
                  <TableHead>Actions | လုပ်ဆောင်ချက်များ</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {filteredRegisters.map((register) => (
                  <TableRow key={register.id}>
                    <TableCell>
                      <Badge variant="outline" className="font-mono">
                        {register.productColorCode}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-1">
                        <span className="font-medium">{register.partWeight}</span>
                        <span className="text-xs text-slate-500">g</span>
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-1">
                        <span className="font-medium">{register.runnerWeight}</span>
                        <span className="text-xs text-slate-500">g</span>
                      </div>
                    </TableCell>
                    <TableCell>
                      <Badge className="bg-blue-100 text-blue-800">
                        {register.cavities}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-1">
                        <span className="font-semibold text-green-700">{register.weightPerShot}</span>
                        <span className="text-xs text-slate-500">g</span>
                      </div>
                    </TableCell>
                    <TableCell>
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => handleEdit(register)}
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
          
          {/* Summary Footer */}
          <div className="mt-4 p-4 bg-slate-50 rounded-lg">
            <div className="grid grid-cols-4 gap-4 text-center">
              <div>
                <div className="text-sm text-slate-600">Total Records</div>
                <div className="text-lg font-semibold text-slate-900">{filteredRegisters.length}</div>
              </div>
              <div>
                <div className="text-sm text-slate-600">Avg Part Weight</div>
                <div className="text-lg font-semibold text-green-700">
                  {(filteredRegisters.reduce((sum, r) => sum + r.partWeight, 0) / filteredRegisters.length).toFixed(1)}g
                </div>
              </div>
              <div>
                <div className="text-sm text-slate-600">Avg Runner Weight</div>
                <div className="text-lg font-semibold text-blue-700">
                  {(filteredRegisters.reduce((sum, r) => sum + r.runnerWeight, 0) / filteredRegisters.length).toFixed(1)}g
                </div>
              </div>
              <div>
                <div className="text-sm text-slate-600">Avg Weight/Shot</div>
                <div className="text-lg font-semibold text-purple-700">
                  {(filteredRegisters.reduce((sum, r) => sum + r.weightPerShot, 0) / filteredRegisters.length).toFixed(1)}g
                </div>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Right-side Drawer */}
      <Sheet open={showDrawer} onOpenChange={setShowDrawer}>
        <SheetContent className="w-[500px] sm:max-w-[500px] overflow-y-auto">
          <SheetHeader>
            <SheetTitle className="flex items-center gap-2">
              <Scale className="h-5 w-5 text-green-600" />
              {editingItem ? 'Edit Weight Record' : 'Create New Weight Record'}
            </SheetTitle>
            <SheetDescription>
              {editingItem ? 'Update weight information for product color' : 'Add weight information for a product color'}
            </SheetDescription>
          </SheetHeader>

          <div className="space-y-6 py-6">
            {/* Product Color Selection */}
            <div>
              <Label htmlFor="productColorCode">Product Color Code * | ထုတ်ကုန်အရောင်ကုဒ် *</Label>
              <Select 
                value={formData.productColorCode} 
                onValueChange={(value) => setFormData(prev => ({ ...prev, productColorCode: value }))}
              >
                <SelectTrigger className="mt-1">
                  <SelectValue placeholder="Select product color..." />
                </SelectTrigger>
                <SelectContent>
                  {mockProductColors.map((pc) => (
                    <SelectItem key={pc.code} value={pc.code}>
                      <div className="flex items-center gap-2">
                        <Badge variant="outline" className="text-xs">{pc.product}</Badge>
                        <span>-</span>
                        <Badge variant="outline" className="text-xs">{pc.color}</Badge>
                      </div>
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            {/* Weight Inputs */}
            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label htmlFor="partWeight">Part Weight (g) * | အစိတ်အလေးချိန် *</Label>
                <div className="relative mt-1">
                  <Input
                    id="partWeight"
                    type="number"
                    step="0.1"
                    value={formData.partWeight}
                    onChange={(e) => setFormData(prev => ({ ...prev, partWeight: e.target.value }))}
                    placeholder="15.5"
                    className="pr-8"
                  />
                  <span className="absolute right-3 top-1/2 transform -translate-y-1/2 text-xs text-slate-500">g</span>
                </div>
              </div>

              <div>
                <Label htmlFor="runnerWeight">Runner Weight (g) | ပြေးလမ်းအလေးချိန်</Label>
                <div className="relative mt-1">
                  <Input
                    id="runnerWeight"
                    type="number"
                    step="0.1"
                    value={formData.runnerWeight}
                    onChange={(e) => setFormData(prev => ({ ...prev, runnerWeight: e.target.value }))}
                    placeholder="12.2"
                    className="pr-8"
                  />
                  <span className="absolute right-3 top-1/2 transform -translate-y-1/2 text-xs text-slate-500">g</span>
                </div>
              </div>
            </div>

            {/* Cavities */}
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
            </div>

            {/* Calculated Weight per Shot */}
            <div className="p-4 bg-green-50 border border-green-200 rounded-lg">
              <div className="flex items-center gap-2 mb-2">
                <Calculator className="h-4 w-4 text-green-600" />
                <Label className="text-green-800">Weight per Shot (calculated) | တစ်ခါအလေးချိန် (တွက်ချက်ထားသော)</Label>
              </div>
              <div className="text-2xl font-bold text-green-700">
                {calculateWeightPerShot().toFixed(2)} g
              </div>
              <div className="text-sm text-green-600 mt-1">
                Formula: (Part Weight + Runner Weight) × Cavities
              </div>
              <div className="text-xs text-slate-600">
                ဖော်မြူလာ: (အစိတ်အလေးချိန် + ပြေးလမ်းအလေးချိန်) × အပေါက်များ
              </div>
            </div>

            {/* Notes */}
            <div>
              <Label htmlFor="notes">Notes | မှတ်ချက်များ</Label>
              <textarea
                id="notes"
                value={formData.notes}
                onChange={(e) => setFormData(prev => ({ ...prev, notes: e.target.value }))}
                placeholder="Optional notes about this weight configuration..."
                className="mt-1 w-full p-3 border border-slate-200 rounded-lg resize-none"
                rows={3}
              />
            </div>

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