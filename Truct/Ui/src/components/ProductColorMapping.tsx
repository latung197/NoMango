import React, { useState } from 'react';
import { 
  Plus, 
  Edit, 
  Trash2, 
  Save, 
  X, 
  Search,
  Package,
  Palette,
  ArrowLeft
} from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from './ui/dialog';

// Mock master data
const products = [
  { code: 'PRD-001', name: 'Plastic Bottle 500ml', weight: 25, unit: 'grams' },
  { code: 'PRD-002', name: 'Food Container Large', weight: 45, unit: 'grams' },
  { code: 'PRD-003', name: 'Disposable Cup', weight: 8, unit: 'grams' },
  { code: 'PRD-004', name: 'Storage Box Medium', weight: 120, unit: 'grams' },
  { code: 'PRD-005', name: 'Plastic Bag Roll', weight: 2, unit: 'grams' }
];

const colors = [
  { code: 'CLR-001', name: 'Clear/Transparent', rgba: 'rgba(255,255,255,0.8)' },
  { code: 'CLR-002', name: 'White', rgba: 'rgba(255,255,255,1)' },
  { code: 'CLR-003', name: 'Blue', rgba: 'rgba(59,130,246,1)' },
  { code: 'CLR-004', name: 'Red', rgba: 'rgba(239,68,68,1)' },
  { code: 'CLR-005', name: 'Green', rgba: 'rgba(34,197,94,1)' },
  { code: 'CLR-006', name: 'Black', rgba: 'rgba(0,0,0,1)' }
];

// Initial product-color mappings
const initialMappings = [
  {
    id: 1,
    mappingCode: 'PRD-001-CLR-002',
    productCode: 'PRD-001',
    colorCode: 'CLR-002',
    fullCode: '2011W',
    status: 'active'
  },
  {
    id: 2,
    mappingCode: 'PRD-001-CLR-003',
    productCode: 'PRD-001',
    colorCode: 'CLR-003',
    fullCode: '2011B',
    status: 'active'
  },
  {
    id: 3,
    mappingCode: 'PRD-002-CLR-001',
    productCode: 'PRD-002',
    colorCode: 'CLR-001',
    fullCode: '2012C',
    status: 'active'
  },
  {
    id: 4,
    mappingCode: 'PRD-003-CLR-004',
    productCode: 'PRD-003',
    colorCode: 'CLR-004',
    fullCode: '2013R',
    status: 'inactive'
  },
  {
    id: 5,
    mappingCode: 'PRD-004-CLR-005',
    productCode: 'PRD-004',
    colorCode: 'CLR-005',
    fullCode: '2014G',
    status: 'active'
  }
];

export function ProductColorMapping() {
  const [view, setView] = useState<'list' | 'form' | 'edit'>('list');
  const [mappings, setMappings] = useState(initialMappings);
  const [searchTerm, setSearchTerm] = useState('');
  const [editingMapping, setEditingMapping] = useState<any>(null);
  
  const [formData, setFormData] = useState({
    productCode: '',
    colorCode: '',
    fullCode: '',
    status: 'active'
  });

  // Auto-generate mapping code and full code when product/color changes
  React.useEffect(() => {
    if (formData.productCode && formData.colorCode && view !== 'list') {
      const mappingCode = `${formData.productCode}-${formData.colorCode}`;
      
      // Generate a simple full code (this would be more sophisticated in real app)
      const productNum = formData.productCode.split('-')[1];
      const colorInitial = colors.find(c => c.code === formData.colorCode)?.name.charAt(0) || 'X';
      const fullCode = `20${productNum}${colorInitial}`;
      
      setFormData(prev => ({ ...prev, fullCode }));
    }
  }, [formData.productCode, formData.colorCode, view]);

  const handleAddNew = () => {
    setFormData({
      productCode: '',
      colorCode: '',
      fullCode: '',
      status: 'active'
    });
    setEditingMapping(null);
    setView('form');
  };

  const handleEdit = (mapping: any) => {
    setFormData({
      productCode: mapping.productCode,
      colorCode: mapping.colorCode,
      fullCode: mapping.fullCode,
      status: mapping.status
    });
    setEditingMapping(mapping);
    setView('edit');
  };

  const handleSave = () => {
    const mappingCode = `${formData.productCode}-${formData.colorCode}`;
    
    if (view === 'edit' && editingMapping) {
      // Update existing mapping
      setMappings(prev => prev.map(m => 
        m.id === editingMapping.id 
          ? { ...m, ...formData, mappingCode }
          : m
      ));
    } else {
      // Add new mapping
      const newMapping = {
        id: Math.max(...mappings.map(m => m.id)) + 1,
        mappingCode,
        productCode: formData.productCode,
        colorCode: formData.colorCode,
        fullCode: formData.fullCode,
        status: formData.status
      };
      setMappings(prev => [...prev, newMapping]);
    }
    
    setView('list');
  };

  const handleDelete = (id: number) => {
    setMappings(prev => prev.filter(m => m.id !== id));
  };

  const handleCancel = () => {
    setView('list');
    setFormData({
      productCode: '',
      colorCode: '',
      fullCode: '',
      status: 'active'
    });
    setEditingMapping(null);
  };

  const getStatusBadge = (status: string) => {
    return status === 'active' 
      ? <Badge className="bg-green-100 text-green-800 border-green-300">Active</Badge>
      : <Badge className="bg-slate-100 text-slate-800 border-slate-300">Inactive</Badge>;
  };

  const getProductName = (code: string) => {
    return products.find(p => p.code === code)?.name || 'Unknown Product';
  };

  const getColorName = (code: string) => {
    return colors.find(c => c.code === code)?.name || 'Unknown Color';
  };

  const getColorDisplay = (code: string) => {
    const color = colors.find(c => c.code === code);
    if (!color) return null;
    return (
      <div className="flex items-center gap-2">
        <div 
          className="w-4 h-4 rounded border border-slate-300" 
          style={{ backgroundColor: color.rgba }}
        />
        {color.name}
      </div>
    );
  };

  const filteredMappings = mappings.filter(mapping => {
    const searchLower = searchTerm.toLowerCase();
    const productName = getProductName(mapping.productCode).toLowerCase();
    const colorName = getColorName(mapping.colorCode).toLowerCase();
    return (
      mapping.mappingCode.toLowerCase().includes(searchLower) ||
      mapping.fullCode.toLowerCase().includes(searchLower) ||
      productName.includes(searchLower) ||
      colorName.includes(searchLower)
    );
  });

  if (view === 'form' || view === 'edit') {
    return (
      <div className="p-6 bg-slate-50 min-h-screen">
        <div className="max-w-4xl mx-auto space-y-6">
          {/* Header */}
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-4">
              <Button variant="ghost" size="sm" onClick={handleCancel}>
                <ArrowLeft className="h-4 w-4 mr-2" />
                Back to List
              </Button>
              <div>
                <h1 className="text-2xl font-semibold text-slate-900">
                  {view === 'edit' ? 'Edit' : 'Add New'} Product-Color Mapping
                </h1>
                <p className="text-sm text-slate-600">
                  ထုတ်ကုန်နှင့်အရောင် ပေါင်းစပ်မှု {view === 'edit' ? 'ပြင်ဆင်ခြင်း' : 'ထည့်သွင်းခြင်း'}
                </p>
              </div>
            </div>
          </div>

          {/* Form Card */}
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Package className="h-5 w-5 text-blue-600" />
                Mapping Details
                <span className="text-sm font-normal text-slate-600">ပေါင်းစပ်မှု အသေးစိတ်</span>
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-6">
              <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                {/* Left Column */}
                <div className="space-y-4">
                  <div>
                    <Label>Product</Label>
                    <Select value={formData.productCode} onValueChange={(value) => setFormData(prev => ({ ...prev, productCode: value }))}>
                      <SelectTrigger>
                        <SelectValue placeholder="Select product" />
                      </SelectTrigger>
                      <SelectContent>
                        {products.map(product => (
                          <SelectItem key={product.code} value={product.code}>
                            {product.code} - {product.name}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>

                  <div>
                    <Label>Color</Label>
                    <Select value={formData.colorCode} onValueChange={(value) => setFormData(prev => ({ ...prev, colorCode: value }))}>
                      <SelectTrigger>
                        <SelectValue placeholder="Select color" />
                      </SelectTrigger>
                      <SelectContent>
                        {colors.map(color => (
                          <SelectItem key={color.code} value={color.code}>
                            <div className="flex items-center gap-2">
                              <div 
                                className="w-4 h-4 rounded border border-slate-300" 
                                style={{ backgroundColor: color.rgba }}
                              />
                              {color.code} - {color.name}
                            </div>
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>
                </div>

                {/* Right Column */}
                <div className="space-y-4">
                  <div>
                    <Label>Mapping Code</Label>
                    <Input 
                      value={formData.productCode && formData.colorCode ? `${formData.productCode}-${formData.colorCode}` : ''}
                      readOnly 
                      className="bg-slate-50" 
                      placeholder="Auto-generated from selections"
                    />
                  </div>

                  <div>
                    <Label>Full Product+Color Code</Label>
                    <Input 
                      value={formData.fullCode}
                      onChange={(e) => setFormData(prev => ({ ...prev, fullCode: e.target.value }))}
                      placeholder="e.g., 2011W, 2012B"
                    />
                  </div>

                  <div>
                    <Label>Status</Label>
                    <Select value={formData.status} onValueChange={(value) => setFormData(prev => ({ ...prev, status: value }))}>
                      <SelectTrigger>
                        <SelectValue />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="active">Active</SelectItem>
                        <SelectItem value="inactive">Inactive</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>
                </div>
              </div>

              {/* Preview */}
              {formData.productCode && formData.colorCode && (
                <div className="border-t pt-6">
                  <Label className="text-sm font-medium text-slate-700 mb-3 block">Preview</Label>
                  <div className="p-4 bg-slate-50 rounded-lg">
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-4 text-sm">
                      <div>
                        <div className="font-medium text-slate-900">Product</div>
                        <div className="text-slate-600">{getProductName(formData.productCode)}</div>
                      </div>
                      <div>
                        <div className="font-medium text-slate-900">Color</div>
                        <div className="text-slate-600">{getColorDisplay(formData.colorCode)}</div>
                      </div>
                      <div>
                        <div className="font-medium text-slate-900">Full Code</div>
                        <div className="text-slate-600 font-mono">{formData.fullCode}</div>
                      </div>
                    </div>
                  </div>
                </div>
              )}
            </CardContent>
          </Card>

          {/* Action Buttons */}
          <div className="flex justify-end gap-3">
            <Button variant="outline" onClick={handleCancel}>
              <X className="h-4 w-4 mr-2" />
              Cancel
            </Button>
            <Button 
              onClick={handleSave}
              disabled={!formData.productCode || !formData.colorCode || !formData.fullCode}
              className="bg-green-600 hover:bg-green-700"
            >
              <Save className="h-4 w-4 mr-2" />
              {view === 'edit' ? 'Update' : 'Save'} Mapping
            </Button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="p-6 bg-slate-50 min-h-screen">
      <div className="max-w-7xl mx-auto space-y-6">
        {/* Header */}
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-semibold text-slate-900">Product-Color Mapping</h1>
            <p className="text-sm text-slate-600">ထုတ်ကုန်နှင့်အရောင် ပေါင်းစပ်မှု စီမံခန့်ခွဲမှု</p>
          </div>
          <Button onClick={handleAddNew} className="bg-blue-600 hover:bg-blue-700">
            <Plus className="h-4 w-4 mr-2" />
            Add New Mapping
          </Button>
        </div>

        {/* Search and Filters */}
        <Card>
          <CardContent className="p-4">
            <div className="flex flex-col sm:flex-row gap-4">
              <div className="relative flex-1">
                <Search className="absolute left-3 top-3 h-4 w-4 text-slate-400" />
                <Input
                  placeholder="Search by mapping code, product, color..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="pl-10"
                />
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Mappings Table */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Palette className="h-5 w-5 text-purple-600" />
              Product-Color Mappings
              <Badge variant="secondary" className="ml-2">
                {filteredMappings.length} items
              </Badge>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Mapping Code</TableHead>
                    <TableHead>Product Name</TableHead>
                    <TableHead>Color Name</TableHead>
                    <TableHead>Full Product+Color Code</TableHead>
                    <TableHead className="text-center">Status</TableHead>
                    <TableHead className="text-center">Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {filteredMappings.map((mapping) => (
                    <TableRow key={mapping.id}>
                      <TableCell className="font-mono text-sm">
                        {mapping.mappingCode}
                      </TableCell>
                      <TableCell>
                        <div>
                          <div className="font-medium">{getProductName(mapping.productCode)}</div>
                          <div className="text-sm text-slate-500">{mapping.productCode}</div>
                        </div>
                      </TableCell>
                      <TableCell>
                        {getColorDisplay(mapping.colorCode)}
                      </TableCell>
                      <TableCell>
                        <div className="font-mono font-medium text-blue-600">
                          {mapping.fullCode}
                        </div>
                      </TableCell>
                      <TableCell className="text-center">
                        {getStatusBadge(mapping.status)}
                      </TableCell>
                      <TableCell className="text-center">
                        <div className="flex items-center justify-center gap-2">
                          <Button
                            variant="ghost"
                            size="sm"
                            onClick={() => handleEdit(mapping)}
                          >
                            <Edit className="h-4 w-4" />
                          </Button>
                          <Button
                            variant="ghost"
                            size="sm"
                            onClick={() => handleDelete(mapping.id)}
                            className="text-red-600 hover:text-red-700 hover:bg-red-50"
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
            
            {filteredMappings.length === 0 && (
              <div className="text-center py-12">
                <Palette className="h-12 w-12 text-slate-300 mx-auto mb-4" />
                <h3 className="text-lg font-medium text-slate-900 mb-2">No mappings found</h3>
                <p className="text-slate-600 mb-4">
                  {searchTerm ? 'Try adjusting your search criteria.' : 'Get started by adding your first product-color mapping.'}
                </p>
                {!searchTerm && (
                  <Button onClick={handleAddNew}>
                    <Plus className="h-4 w-4 mr-2" />
                    Add New Mapping
                  </Button>
                )}
              </div>
            )}
          </CardContent>
        </Card>
      </div>
    </div>
  );
}

// Export the mappings data for use in other components
export { initialMappings, products, colors };