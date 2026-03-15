import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '../ui/card';
import { Button } from '../ui/button';
import { Input } from '../ui/input';
import { Label } from '../ui/label';
import { Badge } from '../ui/badge';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../ui/table';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../ui/select';
import { Sheet, SheetContent, SheetDescription, SheetHeader, SheetTitle } from '../ui/sheet';
import { Alert, AlertDescription } from '../ui/alert';
import { 
  Search, 
  Filter, 
  Download, 
  Plus, 
  Edit, 
  Save,
  Clock,
  Zap,
  Info,
  AlertTriangle
} from 'lucide-react';

// Mock data
const mockCTRegisters = [
  {
    id: 1,
    productColorCode: 'CUP-250-RED-001',
    mold: 'MLD-001',
    machine: 'MCH-001',
    cycleTime: 28,
    efficiency: 85,
    matchPriority: 1, // Color+Mold+Machine
    notes: 'Optimized settings for this combination'
  },
  {
    id: 2,
    productColorCode: 'CUP-250-RED-001',
    mold: '',
    machine: 'MCH-002',
    cycleTime: 32,
    efficiency: 80,
    matchPriority: 2, // Color+Machine
    notes: 'Alternative machine setting'
  },
  {
    id: 3,
    productColorCode: 'CUP-250-RED-001',
    mold: '',
    machine: '',
    cycleTime: 30,
    efficiency: 82,
    matchPriority: 3, // Color default
    notes: 'Default cycle time for this product color'
  },
  {
    id: 4,
    productColorCode: 'CUP-500-BLUE-002',
    mold: 'MLD-004',
    machine: 'MCH-002',
    cycleTime: 45,
    efficiency: 88,
    matchPriority: 1,
    notes: 'Large cup optimized settings'
  }
];

const mockProductColors = [
  { code: 'CUP-250-RED-001', product: 'CUP-250', color: 'RED-001' },
  { code: 'CUP-500-BLUE-002', product: 'CUP-500', color: 'BLUE-002' },
  { code: 'LID-UNIV-WHITE-001', product: 'LID-UNIV', color: 'WHITE-001' }
];

const mockMolds = [
  { code: 'MLD-001', name: 'Cup Mold 4-Cavity' },
  { code: 'MLD-002', name: 'Cup Mold 8-Cavity' },
  { code: 'MLD-004', name: 'Large Cup Mold 2-Cavity' }
];

const mockMachines = [
  { code: 'MCH-001', name: 'Injection Machine 1' },
  { code: 'MCH-002', name: 'Injection Machine 2' },
  { code: 'MCH-003', name: 'Injection Machine 3' }
];

export function CTRegister() {
  const [searchTerm, setSearchTerm] = useState('');
  const [showDrawer, setShowDrawer] = useState(false);
  const [editingItem, setEditingItem] = useState<any>(null);
  const [formData, setFormData] = useState({
    productColorCode: '',
    mold: '',
    machine: '',
    cycleTime: '',
    efficiency: '85',
    notes: ''
  });

  const getMatchPriority = (mold: string, machine: string) => {
    if (mold && machine) return 1; // Color+Mold+Machine
    if (machine && !mold) return 2; // Color+Machine
    if (!mold && !machine) return 3; // Color default
    return 4; // Invalid combination
  };

  const getPriorityBadge = (priority: number) => {
    switch (priority) {
      case 1:
        return <Badge className="bg-green-100 text-green-800">Priority 1: Color+Mold+Machine</Badge>;
      case 2:
        return <Badge className="bg-blue-100 text-blue-800">Priority 2: Color+Machine</Badge>;
      case 3:
        return <Badge className="bg-orange-100 text-orange-800">Priority 3: Color Default</Badge>;
      default:
        return <Badge variant="outline">Unknown Priority</Badge>;
    }
  };

  const handleCreate = () => {
    setEditingItem(null);
    setFormData({
      productColorCode: '',
      mold: '',
      machine: '',
      cycleTime: '',
      efficiency: '85',
      notes: ''
    });
    setShowDrawer(true);
  };

  const handleEdit = (item: any) => {
    setEditingItem(item);
    setFormData({
      productColorCode: item.productColorCode,
      mold: item.mold || '',
      machine: item.machine || '',
      cycleTime: item.cycleTime.toString(),
      efficiency: item.efficiency.toString(),
      notes: item.notes || ''
    });
    setShowDrawer(true);
  };

  const handleSave = () => {
    const priority = getMatchPriority(formData.mold, formData.machine);
    console.log('Saving CT register:', { ...formData, matchPriority: priority });
    setShowDrawer(false);
  };

  const handleSaveAndNew = () => {
    const priority = getMatchPriority(formData.mold, formData.machine);
    console.log('Saving CT register:', { ...formData, matchPriority: priority });
    setFormData({
      productColorCode: '',
      mold: '',
      machine: '',
      cycleTime: '',
      efficiency: '85',
      notes: ''
    });
  };

  const filteredRegisters = mockCTRegisters.filter(register =>
    register.productColorCode.toLowerCase().includes(searchTerm.toLowerCase()) ||
    (register.mold && register.mold.toLowerCase().includes(searchTerm.toLowerCase())) ||
    (register.machine && register.machine.toLowerCase().includes(searchTerm.toLowerCase()))
  );

  const currentPriority = getMatchPriority(formData.mold, formData.machine);

  return (
    <div className="p-6 space-y-6">
      {/* Header */}
      <div>
        <h2 className="text-2xl font-semibold text-slate-900">
          CT Register | CT မှတ်ပုံတင်
        </h2>
        <p className="text-slate-600 mt-1">
          Register cycle times and efficiency by product color, mold, and machine combinations
        </p>
        <p className="text-sm text-slate-500">
          ထုတ်ကုန်အရောင်၊ ပုံစံနှင့် စက်ပေါင်းစပ်မှုအလိုက် စက်ခရီးအချိန်နှင့် စွမ်းရည်ကို မှတ်ပုံတင်ခြင်း
        </p>
      </div>

      {/* Match Priority Info */}
      <Alert>
        <Info className="h-4 w-4" />
        <AlertDescription>
          <strong>Match Priority:</strong> 1) Color+Mold+Machine → 2) Color+Machine → 3) Color Default
          <br />
          <span className="text-xs text-slate-600">
            ကွက်ညီမှုဦးစားပေးအစီအစဉ်: ၁) အရောင်+ပုံစံ+စက် → ၂) အရောင်+စက် → ၃) အရောင်ပုံမှန်
          </span>
        </AlertDescription>
      </Alert>

      {/* Toolbar */}
      <div className="flex justify-between items-center">
        <div className="flex items-center gap-4">
          {/* Search */}
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-slate-400 h-4 w-4" />
            <Input
              placeholder="Search by product, mold, or machine..."
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
            Add CT Record | CT မှတ်တမ်းထည့်သွင်းရန်
          </Button>
        </div>
      </div>

      {/* Main Table */}
      <Card className="border-0 shadow-sm">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Clock className="h-5 w-5 text-blue-600" />
            Cycle Time Records | စက်ခရီးအချိန်မှတ်တမ်းများ
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Product Color Code | ထုတ်ကုန်အရောင်ကုဒ်</TableHead>
                  <TableHead>Mold | ပုံစံ</TableHead>
                  <TableHead>Machine | စက��</TableHead>
                  <TableHead>CT (sec) | စက်ခရီးအချိန်</TableHead>
                  <TableHead>Eff % | စွမ်းရည်</TableHead>
                  <TableHead>Priority | ဦးစားပေး</TableHead>
                  <TableHead>Actions | လုပ်ဆောင်ချက်များ</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {filteredRegisters
                  .sort((a, b) => a.matchPriority - b.matchPriority)
                  .map((register) => (
                  <TableRow key={register.id}>
                    <TableCell>
                      <Badge variant="outline" className="font-mono">
                        {register.productColorCode}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      {register.mold ? (
                        <Badge variant="secondary">{register.mold}</Badge>
                      ) : (
                        <span className="text-slate-400 text-sm">Any mold</span>
                      )}
                    </TableCell>
                    <TableCell>
                      {register.machine ? (
                        <Badge variant="secondary">{register.machine}</Badge>
                      ) : (
                        <span className="text-slate-400 text-sm">Any machine</span>
                      )}
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-1">
                        <span className="font-semibold text-blue-700">{register.cycleTime}</span>
                        <span className="text-xs text-slate-500">sec</span>
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-1">
                        <Zap className="h-3 w-3 text-green-500" />
                        <span className="font-medium">{register.efficiency}%</span>
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-2">
                        <div className={`w-2 h-2 rounded-full ${
                          register.matchPriority === 1 ? 'bg-green-500' :
                          register.matchPriority === 2 ? 'bg-blue-500' : 'bg-orange-500'
                        }`}></div>
                        <span className="text-sm">Priority {register.matchPriority}</span>
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
                <div className="text-sm text-slate-600">Avg Cycle Time</div>
                <div className="text-lg font-semibold text-blue-700">
                  {(filteredRegisters.reduce((sum, r) => sum + r.cycleTime, 0) / filteredRegisters.length).toFixed(1)}s
                </div>
              </div>
              <div>
                <div className="text-sm text-slate-600">Avg Efficiency</div>
                <div className="text-lg font-semibold text-green-700">
                  {(filteredRegisters.reduce((sum, r) => sum + r.efficiency, 0) / filteredRegisters.length).toFixed(1)}%
                </div>
              </div>
              <div>
                <div className="text-sm text-slate-600">Priority 1 Records</div>
                <div className="text-lg font-semibold text-purple-700">
                  {filteredRegisters.filter(r => r.matchPriority === 1).length}
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
              <Clock className="h-5 w-5 text-blue-600" />
              {editingItem ? 'Edit CT Record' : 'Create New CT Record'}
            </SheetTitle>
            <SheetDescription>
              {editingItem ? 'Update cycle time and efficiency information' : 'Add cycle time and efficiency information'}
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

            {/* Mold and Machine Selection */}
            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label htmlFor="mold">Mold (Optional) | ပုံစံ (ရွေးချယ်နိုင်သော)</Label>
                <Select 
                  value={formData.mold} 
                  onValueChange={(value) => setFormData(prev => ({ ...prev, mold: value || '' }))}
                >
                  <SelectTrigger className="mt-1">
                    <SelectValue placeholder="Any mold" />
                  </SelectTrigger>
                  <SelectContent>
                    {mockMolds.map((mold) => (
                      <SelectItem key={mold.code} value={mold.code}>
                        {mold.code} - {mold.name}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>

              <div>
                <Label htmlFor="machine">Machine (Optional) | စက် (ရွေးချယ်နိုင်သော)</Label>
                <Select 
                  value={formData.machine} 
                  onValueChange={(value) => setFormData(prev => ({ ...prev, machine: value || '' }))}
                >
                  <SelectTrigger className="mt-1">
                    <SelectValue placeholder="Any machine" />
                  </SelectTrigger>
                  <SelectContent>
                    {mockMachines.map((machine) => (
                      <SelectItem key={machine.code} value={machine.code}>
                        {machine.code} - {machine.name}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
            </div>

            {/* Match Priority Display */}
            <div className="p-4 bg-slate-50 border rounded-lg">
              <div className="flex items-center gap-2 mb-2">
                <AlertTriangle className="h-4 w-4 text-slate-600" />
                <Label className="text-slate-800">Match Priority | ကွက်ညီမှုဦးစားပေး</Label>
              </div>
              {getPriorityBadge(currentPriority)}
              <div className="text-xs text-slate-600 mt-2">
                Higher priority records will be used first in calculations
              </div>
            </div>

            {/* Cycle Time and Efficiency */}
            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label htmlFor="cycleTime">Cycle Time (sec) * | စက်ခရီးအချိန် *</Label>
                <div className="relative mt-1">
                  <Input
                    id="cycleTime"
                    type="number"
                    step="0.1"
                    value={formData.cycleTime}
                    onChange={(e) => setFormData(prev => ({ ...prev, cycleTime: e.target.value }))}
                    placeholder="30"
                    className="pr-8"
                  />
                  <span className="absolute right-3 top-1/2 transform -translate-y-1/2 text-xs text-slate-500">sec</span>
                </div>
                <div className="text-xs text-slate-500 mt-1">
                  Time for one complete cycle
                </div>
              </div>

              <div>
                <Label htmlFor="efficiency">Planning Efficiency (%) | စီမံကိန်းစွမ်းရည်</Label>
                <div className="relative mt-1">
                  <Input
                    id="efficiency"
                    type="number"
                    min="1"
                    max="100"
                    value={formData.efficiency}
                    onChange={(e) => setFormData(prev => ({ ...prev, efficiency: e.target.value }))}
                    placeholder="85"
                    className="pr-8"
                  />
                  <span className="absolute right-3 top-1/2 transform -translate-y-1/2 text-xs text-slate-500">%</span>
                </div>
                <div className="text-xs text-slate-500 mt-1">
                  Default: 85%
                </div>
              </div>
            </div>

            {/* Notes */}
            <div>
              <Label htmlFor="notes">Notes | မှတ်ချက်များ</Label>
              <textarea
                id="notes"
                value={formData.notes}
                onChange={(e) => setFormData(prev => ({ ...prev, notes: e.target.value }))}
                placeholder="Optional notes about this CT configuration..."
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