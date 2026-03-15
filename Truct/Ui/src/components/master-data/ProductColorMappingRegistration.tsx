import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '../ui/card';
import { Button } from '../ui/button';
import { Input } from '../ui/input';
import { Label } from '../ui/label';
import { Badge } from '../ui/badge';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../ui/table';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../ui/select';
import { Switch } from '../ui/switch';
import { Textarea } from '../ui/textarea';
import { Sheet, SheetContent, SheetDescription, SheetHeader, SheetTitle } from '../ui/sheet';
import { 
  Search, 
  Filter, 
  Download, 
  Plus, 
  Edit, 
  X,
  Save,
  Package,
  Palette,
  Settings,
  AlertCircle
} from 'lucide-react';

// Mock data
const mockProductColorMappings = [
  {
    id: 1,
    productCode: 'CUP-250',
    colorCode: 'RED-001',
    allowedMolds: ['MLD-001', 'MLD-002'],
    allowedMachines: ['MCH-001', 'MCH-003'],
    hasRecipe: true,
    recipe: [
      { materialCode: 'PP-RED-001', percent: 85 },
      { materialCode: 'ADDITIVE-001', percent: 15 }
    ],
    notes: 'Standard red color for 250ml cups',
    status: 'Active'
  },
  {
    id: 2,
    productCode: 'CUP-500',
    colorCode: 'BLUE-002',
    allowedMolds: ['MLD-002', 'MLD-004'],
    allowedMachines: ['MCH-002', 'MCH-004'],
    hasRecipe: false,
    recipe: [],
    notes: 'Blue variant for large cups',
    status: 'Active'
  }
];

const mockProducts = [
  { code: 'CUP-250', name: '250ml Cup' },
  { code: 'CUP-500', name: '500ml Cup' },
  { code: 'LID-UNIV', name: 'Universal Lid' }
];

const mockMolds = [
  { code: 'MLD-001', name: 'Cup Mold 4-Cavity' },
  { code: 'MLD-002', name: 'Cup Mold 8-Cavity' },
  { code: 'MLD-003', name: 'Lid Mold 12-Cavity' },
  { code: 'MLD-004', name: 'Large Cup Mold 2-Cavity' }
];

const mockMachines = [
  { code: 'MCH-001', name: 'Injection Machine 1' },
  { code: 'MCH-002', name: 'Injection Machine 2' },
  { code: 'MCH-003', name: 'Injection Machine 3' },
  { code: 'MCH-004', name: 'Injection Machine 4' }
];

const mockMaterials = [
  { code: 'PP-RED-001', name: 'Red Polypropylene' },
  { code: 'PP-BLUE-002', name: 'Blue Polypropylene' },
  { code: 'ADDITIVE-001', name: 'UV Stabilizer' },
  { code: 'COLORANT-RED', name: 'Red Colorant' }
];

export function ProductColorMappingRegistration() {
  const [searchTerm, setSearchTerm] = useState('');
  const [showDrawer, setShowDrawer] = useState(false);
  const [editingItem, setEditingItem] = useState<any>(null);
  const [formData, setFormData] = useState({
    productCode: '',
    colorCode: '',
    allowedMolds: [] as string[],
    allowedMachines: [] as string[],
    recipe: [] as Array<{ materialCode: string; percent: number }>,
    notes: '',
    status: true
  });

  const handleCreate = () => {
    setEditingItem(null);
    setFormData({
      productCode: '',
      colorCode: '',
      allowedMolds: [],
      allowedMachines: [],
      recipe: [],
      notes: '',
      status: true
    });
    setShowDrawer(true);
  };

  const handleEdit = (item: any) => {
    setEditingItem(item);
    setFormData({
      productCode: item.productCode,
      colorCode: item.colorCode,
      allowedMolds: item.allowedMolds,
      allowedMachines: item.allowedMachines,
      recipe: item.recipe,
      notes: item.notes,
      status: item.status === 'Active'
    });
    setShowDrawer(true);
  };

  const handleSave = () => {
    console.log('Saving:', formData);
    setShowDrawer(false);
  };

  const handleSaveAndNew = () => {
    console.log('Saving:', formData);
    setFormData({
      productCode: '',
      colorCode: '',
      allowedMolds: [],
      allowedMachines: [],
      recipe: [],
      notes: '',
      status: true
    });
  };

  const addRecipeRow = () => {
    setFormData(prev => ({
      ...prev,
      recipe: [...prev.recipe, { materialCode: '', percent: 0 }]
    }));
  };

  const removeRecipeRow = (index: number) => {
    setFormData(prev => ({
      ...prev,
      recipe: prev.recipe.filter((_, i) => i !== index)
    }));
  };

  const updateRecipeRow = (index: number, field: string, value: any) => {
    setFormData(prev => ({
      ...prev,
      recipe: prev.recipe.map((row, i) => 
        i === index ? { ...row, [field]: value } : row
      )
    }));
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

  const filteredMappings = mockProductColorMappings.filter(mapping =>
    mapping.productCode.toLowerCase().includes(searchTerm.toLowerCase()) ||
    mapping.colorCode.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="p-6 space-y-6">
      {/* Header */}
      <div>
        <h2 className="text-2xl font-semibold text-slate-900">
          Product–Color Mapping | ထုတ်ကုန်-အရောင်ကွက်လပ်
        </h2>
        <p className="text-slate-600 mt-1">
          Map products to colors with allowed molds, machines, and material recipes
        </p>
        <p className="text-sm text-slate-500">
          ထုတ်ကုန်များကို အရောင်များနှင့် ခွင့်ပြုသောပုံစံများ၊ စက်များနှင့် ပစ္စည်းများအတွက် ချိတ်ဆက်ခြင်း
        </p>
      </div>

      {/* Toolbar */}
      <div className="flex justify-between items-center">
        <div className="flex items-center gap-4">
          {/* Search */}
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-slate-400 h-4 w-4" />
            <Input
              placeholder="Search by product or color code..."
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
            Add Mapping | ကွက်လပ်ထည့်သွင်းရန်
          </Button>
        </div>
      </div>

      {/* Main Table */}
      <Card className="border-0 shadow-sm">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Package className="h-5 w-5 text-blue-600" />
            Product–Color Mappings | ထုတ်ကုန်-အရောင်ကွက်လပ်များ
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Product Code | ထုတ်ကုန်ကုဒ်</TableHead>
                  <TableHead>Color Code | အရောင်ကုဒ်</TableHead>
                  <TableHead>Allowed Molds | ခွင့်ပြုသောပုံစံများ</TableHead>
                  <TableHead>Allowed Machines | ခွင့်ပြုသောစက်များ</TableHead>
                  <TableHead>Recipe | ချက်နည်း</TableHead>
                  <TableHead>Status | အခြေအနေ</TableHead>
                  <TableHead>Actions | လုပ်ဆောင်ချက်များ</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {filteredMappings.map((mapping) => (
                  <TableRow key={mapping.id}>
                    <TableCell>
                      <Badge variant="outline" className="font-mono">
                        {mapping.productCode}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-2">
                        <div className="w-4 h-4 bg-red-500 rounded-full border"></div>
                        <Badge variant="outline">
                          {mapping.colorCode}
                        </Badge>
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex flex-wrap gap-1">
                        {mapping.allowedMolds.map((mold) => (
                          <Badge key={mold} variant="secondary" className="text-xs">
                            {mold}
                          </Badge>
                        ))}
                        <Badge variant="outline" className="text-xs text-slate-500">
                          ({mapping.allowedMolds.length})
                        </Badge>
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex flex-wrap gap-1">
                        {mapping.allowedMachines.map((machine) => (
                          <Badge key={machine} variant="secondary" className="text-xs">
                            {machine}
                          </Badge>
                        ))}
                        <Badge variant="outline" className="text-xs text-slate-500">
                          ({mapping.allowedMachines.length})
                        </Badge>
                      </div>
                    </TableCell>
                    <TableCell>
                      {mapping.hasRecipe ? (
                        <Badge className="bg-green-100 text-green-800">
                          ✓ Recipe
                        </Badge>
                      ) : (
                        <Badge variant="outline" className="text-slate-500">
                          No Recipe
                        </Badge>
                      )}
                    </TableCell>
                    <TableCell>
                      <Badge className={
                        mapping.status === 'Active' 
                          ? 'bg-green-100 text-green-800' 
                          : 'bg-red-100 text-red-800'
                      }>
                        {mapping.status}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => handleEdit(mapping)}
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
        </CardContent>
      </Card>

      {/* Right-side Drawer */}
      <Sheet open={showDrawer} onOpenChange={setShowDrawer}>
        <SheetContent className="w-[600px] sm:max-w-[600px] overflow-y-auto">
          <SheetHeader>
            <SheetTitle className="flex items-center gap-2">
              <Palette className="h-5 w-5 text-blue-600" />
              {editingItem ? 'Edit Mapping' : 'Create New Mapping'}
            </SheetTitle>
            <SheetDescription>
              {editingItem ? 'Update product-color mapping details' : 'Create a new product-color mapping'}
            </SheetDescription>
          </SheetHeader>

          <div className="space-y-6 py-6">
            {/* Basic Info */}
            <div className="space-y-4">
              <h4 className="font-medium text-slate-900">Basic Information | အခြေခံအချက်အလက်</h4>
              
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label htmlFor="productCode">Product Code * | ထုတ်ကုန်ကုဒ် *</Label>
                  <Select value={formData.productCode} onValueChange={(value) => setFormData(prev => ({ ...prev, productCode: value }))}>
                    <SelectTrigger className="mt-1">
                      <SelectValue placeholder="Select product..." />
                    </SelectTrigger>
                    <SelectContent>
                      {mockProducts.map((product) => (
                        <SelectItem key={product.code} value={product.code}>
                          {product.code} - {product.name}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>

                <div>
                  <Label htmlFor="colorCode">Color Code * | အရောင်ကုဒ် *</Label>
                  <Input
                    id="colorCode"
                    value={formData.colorCode}
                    onChange={(e) => setFormData(prev => ({ ...prev, colorCode: e.target.value }))}
                    placeholder="e.g., RED-001"
                    className="mt-1"
                  />
                </div>
              </div>
            </div>

            {/* Allowed Molds */}
            <div className="space-y-4">
              <h4 className="font-medium text-slate-900">Allowed Molds | ခွင့်ပြုသောပုံစံများ</h4>
              <div className="grid grid-cols-2 gap-2">
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
            </div>

            {/* Allowed Machines */}
            <div className="space-y-4">
              <h4 className="font-medium text-slate-900">Allowed Machines | ခွင့်ပြုသောစက်များ</h4>
              <div className="grid grid-cols-2 gap-2">
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
            </div>

            {/* Material Recipe */}
            <div className="space-y-4">
              <div className="flex items-center justify-between">
                <h4 className="font-medium text-slate-900">Material Recipe | ပစ္စည်းချက်နည်း</h4>
                <Button variant="outline" size="sm" onClick={addRecipeRow} className="gap-1">
                  <Plus className="h-3 w-3" />
                  Add Material
                </Button>
              </div>
              
              {formData.recipe.length > 0 ? (
                <div className="space-y-2">
                  {formData.recipe.map((row, index) => (
                    <div key={index} className="flex items-center gap-2 p-3 border rounded-lg">
                      <div className="flex-1">
                        <Select 
                          value={row.materialCode} 
                          onValueChange={(value) => updateRecipeRow(index, 'materialCode', value)}
                        >
                          <SelectTrigger>
                            <SelectValue placeholder="Select material..." />
                          </SelectTrigger>
                          <SelectContent>
                            {mockMaterials.map((material) => (
                              <SelectItem key={material.code} value={material.code}>
                                {material.code} - {material.name}
                              </SelectItem>
                            ))}
                          </SelectContent>
                        </Select>
                      </div>
                      <div className="w-20">
                        <Input
                          type="number"
                          value={row.percent}
                          onChange={(e) => updateRecipeRow(index, 'percent', parseInt(e.target.value) || 0)}
                          placeholder="%"
                          min="0"
                          max="100"
                        />
                      </div>
                      <div className="text-sm text-slate-500">%</div>
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => removeRecipeRow(index)}
                        className="p-1"
                      >
                        <X className="h-3 w-3" />
                      </Button>
                    </div>
                  ))}
                  
                  {/* Recipe Total */}
                  <div className="flex justify-end p-2 bg-slate-50 rounded">
                    <div className="text-sm">
                      Total: <strong>{formData.recipe.reduce((sum, row) => sum + (row.percent || 0), 0)}%</strong>
                      {formData.recipe.reduce((sum, row) => sum + (row.percent || 0), 0) !== 100 && (
                        <AlertCircle className="inline h-4 w-4 ml-1 text-orange-500" />
                      )}
                    </div>
                  </div>
                </div>
              ) : (
                <div className="text-center py-6 text-slate-500 border-2 border-dashed rounded-lg">
                  <Settings className="h-8 w-8 mx-auto mb-2 text-slate-400" />
                  <p>No recipe defined</p>
                  <p className="text-xs">ချက်နည်းမရှိပါ</p>
                </div>
              )}
            </div>

            {/* Notes and Status */}
            <div className="space-y-4">
              <div>
                <Label htmlFor="notes">Notes | မှတ်ချက်များ</Label>
                <Textarea
                  id="notes"
                  value={formData.notes}
                  onChange={(e) => setFormData(prev => ({ ...prev, notes: e.target.value }))}
                  placeholder="Optional notes..."
                  className="mt-1"
                  rows={3}
                />
              </div>

              <div className="flex items-center justify-between">
                <Label htmlFor="status">Status | အခြေအနေ</Label>
                <div className="flex items-center gap-2">
                  <span className={formData.status ? 'text-green-600' : 'text-red-600'}>
                    {formData.status ? 'Active' : 'Inactive'}
                  </span>
                  <Switch
                    id="status"
                    checked={formData.status}
                    onCheckedChange={(checked) => setFormData(prev => ({ ...prev, status: checked }))}
                  />
                </div>
              </div>
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