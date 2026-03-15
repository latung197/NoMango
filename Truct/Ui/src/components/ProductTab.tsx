import React, { useState } from 'react';
import { 
  Plus, 
  Edit, 
  Trash2, 
  Save, 
  X, 
  Search,
  Package,
  Upload,
  Image,
  ArrowLeft,
  Weight,
  Box
} from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Textarea } from './ui/textarea';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from './ui/dialog';
import { Switch } from './ui/switch';

// Mock master data
const productTypes = [
  { value: 'molded', label: 'Molded' },
  { value: 'extrusion', label: 'Extrusion' },
  { value: 'assembly', label: 'Assembly' }
];

const unitsOfMeasure = [
  { value: 'pcs', label: 'Pieces (pcs)' },
  { value: 'kg', label: 'Kilograms (kg)' },
  { value: 'set', label: 'Sets (set)' },
  { value: 'roll', label: 'Rolls (roll)' },
  { value: 'm', label: 'Meters (m)' }
];

const packagingTypes = [
  { value: 'bag', label: 'Bag' },
  { value: 'box', label: 'Box' },
  { value: 'carton', label: 'Carton' },
  { value: 'pallet', label: 'Pallet' },
  { value: 'bulk', label: 'Bulk' }
];

const molds = [
  { code: 'MLD-001', name: 'Bottle Mold 500ml' },
  { code: 'MLD-002', name: 'Container Mold Large' },
  { code: 'MLD-003', name: 'Cup Mold Standard' },
  { code: 'MLD-004', name: 'Box Mold Medium' }
];

const glueTypes = [
  { value: 'hot-melt', label: 'Hot Melt Adhesive' },
  { value: 'water-based', label: 'Water-based Adhesive' },
  { value: 'solvent-based', label: 'Solvent-based Adhesive' },
  { value: 'uv-curable', label: 'UV Curable' },
  { value: 'none', label: 'No Glue Required' }
];

// Initial products data
const initialProducts = [
  {
    id: 1,
    code: 'PRD-001',
    nameEN: 'Plastic Bottle 500ml',
    nameMM: 'ပလပ်စတစ်ပုလင်း ၅၀၀ မီလီ',
    productType: 'molded',
    unit: 'pcs',
    weight: 25,
    packaging: 'box',
    moldCode: 'MLD-001',
    glueType: 'none',
    status: 'active',
    photo: null
  },
  {
    id: 2,
    code: 'PRD-002',
    nameEN: 'Food Container Large',
    nameMM: 'အစားအသောက်သိုလှောင်ပုံး ကြီး',
    productType: 'molded',
    unit: 'pcs',
    weight: 45,
    packaging: 'carton',
    moldCode: 'MLD-002',
    glueType: 'hot-melt',
    status: 'active',
    photo: null
  },
  {
    id: 3,
    code: 'PRD-003',
    nameEN: 'Disposable Cup',
    nameMM: 'တစ်ခါသုံးခွက်',
    productType: 'molded',
    unit: 'pcs',
    weight: 8,
    packaging: 'bag',
    moldCode: 'MLD-003',
    glueType: 'none',
    status: 'active',
    photo: null
  },
  {
    id: 4,
    code: 'PRD-004',
    nameEN: 'Storage Box Medium',
    nameMM: 'သိုလှောင်သေတ္တာ အလတ်',
    productType: 'assembly',
    unit: 'set',
    weight: 120,
    packaging: 'box',
    moldCode: 'MLD-004',
    glueType: 'water-based',
    status: 'inactive',
    photo: null
  }
];

export function ProductTab() {
  const [view, setView] = useState<'list' | 'form' | 'edit'>('list');
  const [products, setProducts] = useState(initialProducts);
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState('all');
  const [editingProduct, setEditingProduct] = useState<any>(null);
  
  const [formData, setFormData] = useState({
    code: '',
    nameEN: '',
    nameMM: '',
    productType: '',
    unit: '',
    weight: '',
    packaging: '',
    moldCode: '',
    glueType: '',
    status: true,
    photo: null as File | null
  });

  const handleAddNew = () => {
    const newCode = `PRD-${String(Math.max(...products.map(p => p.id)) + 1).padStart(3, '0')}`;
    setFormData({
      code: newCode,
      nameEN: '',
      nameMM: '',
      productType: '',
      unit: '',
      weight: '',
      packaging: '',
      moldCode: '',
      glueType: '',
      status: true,
      photo: null
    });
    setEditingProduct(null);
    setView('form');
  };

  const handleEdit = (product: any) => {
    setFormData({
      code: product.code,
      nameEN: product.nameEN,
      nameMM: product.nameMM,
      productType: product.productType,
      unit: product.unit,
      weight: product.weight.toString(),
      packaging: product.packaging,
      moldCode: product.moldCode,
      glueType: product.glueType,
      status: product.status === 'active',
      photo: null
    });
    setEditingProduct(product);
    setView('edit');
  };

  const handleSave = () => {
    const productData = {
      code: formData.code,
      nameEN: formData.nameEN,
      nameMM: formData.nameMM,
      productType: formData.productType,
      unit: formData.unit,
      weight: parseFloat(formData.weight) || 0,
      packaging: formData.packaging,
      moldCode: formData.moldCode,
      glueType: formData.glueType,
      status: formData.status ? 'active' : 'inactive',
      photo: formData.photo
    };
    
    if (view === 'edit' && editingProduct) {
      // Update existing product
      setProducts(prev => prev.map(p => 
        p.id === editingProduct.id 
          ? { ...p, ...productData }
          : p
      ));
    } else {
      // Add new product
      const newProduct = {
        id: Math.max(...products.map(p => p.id)) + 1,
        ...productData
      };
      setProducts(prev => [...prev, newProduct]);
    }
    
    setView('list');
  };

  const handleDelete = (id: number) => {
    setProducts(prev => prev.filter(p => p.id !== id));
  };

  const handleCancel = () => {
    setView('list');
    setFormData({
      code: '',
      nameEN: '',
      nameMM: '',
      productType: '',
      unit: '',
      weight: '',
      packaging: '',
      moldCode: '',
      glueType: '',
      status: true,
      photo: null
    });
    setEditingProduct(null);
  };

  const handlePhotoUpload = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      setFormData(prev => ({ ...prev, photo: file }));
    }
  };

  const getStatusBadge = (status: string) => {
    return status === 'active' 
      ? <Badge className="bg-green-100 text-green-800 border-green-300">Active</Badge>
      : <Badge className="bg-slate-100 text-slate-800 border-slate-300">Inactive</Badge>;
  };

  const getProductTypeBadge = (type: string) => {
    const colors = {
      molded: 'bg-blue-100 text-blue-800 border-blue-300',
      extrusion: 'bg-purple-100 text-purple-800 border-purple-300',
      assembly: 'bg-orange-100 text-orange-800 border-orange-300'
    };
    return <Badge className={colors[type as keyof typeof colors] || 'bg-slate-100 text-slate-800 border-slate-300'}>
      {productTypes.find(pt => pt.value === type)?.label || type}
    </Badge>;
  };

  const getMoldName = (code: string) => {
    return molds.find(m => m.code === code)?.name || 'Unknown Mold';
  };

  const filteredProducts = products.filter(product => {
    const matchesSearch = searchTerm === '' || 
      product.code.toLowerCase().includes(searchTerm.toLowerCase()) ||
      product.nameEN.toLowerCase().includes(searchTerm.toLowerCase()) ||
      product.nameMM.toLowerCase().includes(searchTerm.toLowerCase());
    
    const matchesStatus = statusFilter === 'all' || product.status === statusFilter;
    
    return matchesSearch && matchesStatus;
  });

  if (view === 'form' || view === 'edit') {
    return (
      <div className="space-y-6">
        {/* Header */}
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-4">
            <Button variant="ghost" size="sm" onClick={handleCancel}>
              <ArrowLeft className="h-4 w-4 mr-2" />
              Back to List
            </Button>
            <div>
              <h3 className="text-lg font-semibold text-slate-900">
                {view === 'edit' ? 'Edit' : 'Add New'} Product
              </h3>
              <p className="text-sm text-slate-600">
                ထုတ်ကုန် {view === 'edit' ? 'ပြင်ဆင်ခြင်း' : 'ထည့်သွင်းခြင်း'}
              </p>
            </div>
          </div>
        </div>

        {/* Form */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Package className="h-5 w-5 text-blue-600" />
              Product Information
              <span className="text-sm font-normal text-slate-600">ထုတ်ကုန်အချက်အလက်</span>
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-6">
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
              {/* Left Column */}
              <div className="space-y-4">
                <div>
                  <Label>Product Code</Label>
                  <Input 
                    value={formData.code}
                    onChange={(e) => setFormData(prev => ({ ...prev, code: e.target.value }))}
                    placeholder="PRD-001"
                  />
                </div>

                <div>
                  <Label>Product Name (English)</Label>
                  <Input 
                    value={formData.nameEN}
                    onChange={(e) => setFormData(prev => ({ ...prev, nameEN: e.target.value }))}
                    placeholder="Product name in English"
                  />
                </div>

                <div>
                  <Label>Product Name (Myanmar)</Label>
                  <Input 
                    value={formData.nameMM}
                    onChange={(e) => setFormData(prev => ({ ...prev, nameMM: e.target.value }))}
                    placeholder="မြန်မာဘာသာဖြင့် ထုတ်ကုန်အမည်"
                  />
                </div>

                <div>
                  <Label>Product Type</Label>
                  <Select value={formData.productType} onValueChange={(value) => setFormData(prev => ({ ...prev, productType: value }))}>
                    <SelectTrigger>
                      <SelectValue placeholder="Select product type" />
                    </SelectTrigger>
                    <SelectContent>
                      {productTypes.map(type => (
                        <SelectItem key={type.value} value={type.value}>
                          {type.label}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>

                <div>
                  <Label>Unit of Measure</Label>
                  <Select value={formData.unit} onValueChange={(value) => setFormData(prev => ({ ...prev, unit: value }))}>
                    <SelectTrigger>
                      <SelectValue placeholder="Select unit" />
                    </SelectTrigger>
                    <SelectContent>
                      {unitsOfMeasure.map(unit => (
                        <SelectItem key={unit.value} value={unit.value}>
                          {unit.label}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
              </div>

              {/* Right Column */}
              <div className="space-y-4">
                <div>
                  <Label>Weight per Unit (grams)</Label>
                  <div className="flex gap-2">
                    <Input 
                      type="number"
                      value={formData.weight}
                      onChange={(e) => setFormData(prev => ({ ...prev, weight: e.target.value }))}
                      placeholder="0"
                      className="flex-1"
                    />
                    <div className="px-3 py-2 bg-slate-50 border rounded-md text-sm text-slate-600 flex items-center">
                      <Weight className="h-4 w-4 mr-1" />
                      grams
                    </div>
                  </div>
                </div>

                <div>
                  <Label>Packaging Type</Label>
                  <Select value={formData.packaging} onValueChange={(value) => setFormData(prev => ({ ...prev, packaging: value }))}>
                    <SelectTrigger>
                      <SelectValue placeholder="Select packaging" />
                    </SelectTrigger>
                    <SelectContent>
                      {packagingTypes.map(pkg => (
                        <SelectItem key={pkg.value} value={pkg.value}>
                          <div className="flex items-center gap-2">
                            <Box className="h-4 w-4" />
                            {pkg.label}
                          </div>
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>

                <div>
                  <Label>Linked Mold</Label>
                  <Select value={formData.moldCode} onValueChange={(value) => setFormData(prev => ({ ...prev, moldCode: value }))}>
                    <SelectTrigger>
                      <SelectValue placeholder="Select mold" />
                    </SelectTrigger>
                    <SelectContent>
                      {molds.map(mold => (
                        <SelectItem key={mold.code} value={mold.code}>
                          {mold.code} - {mold.name}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>

                <div>
                  <Label>Default Glue Type</Label>
                  <Select value={formData.glueType} onValueChange={(value) => setFormData(prev => ({ ...prev, glueType: value }))}>
                    <SelectTrigger>
                      <SelectValue placeholder="Select glue type" />
                    </SelectTrigger>
                    <SelectContent>
                      {glueTypes.map(glue => (
                        <SelectItem key={glue.value} value={glue.value}>
                          {glue.label}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>

                <div className="flex items-center justify-between">
                  <Label>Status</Label>
                  <div className="flex items-center gap-2">
                    <Switch 
                      checked={formData.status}
                      onCheckedChange={(checked) => setFormData(prev => ({ ...prev, status: checked }))}
                    />
                    <span className="text-sm">{formData.status ? 'Active' : 'Inactive'}</span>
                  </div>
                </div>
              </div>
            </div>

            {/* Photo Upload */}
            <div className="border-t pt-6">
              <Label className="text-sm font-medium text-slate-700 mb-3 block">Product Photo</Label>
              <div className="flex items-start gap-4">
                <div className="w-32 h-32 border-2 border-dashed border-slate-300 rounded-lg flex items-center justify-center bg-slate-50">
                  {formData.photo ? (
                    <Image className="h-8 w-8 text-slate-400" />
                  ) : (
                    <div className="text-center">
                      <Image className="h-8 w-8 text-slate-400 mx-auto mb-2" />
                      <p className="text-xs text-slate-500">No image</p>
                    </div>
                  )}
                </div>
                <div className="flex-1">
                  <Button variant="outline" onClick={() => document.getElementById('photo-upload')?.click()}>
                    <Upload className="h-4 w-4 mr-2" />
                    Upload Photo
                  </Button>
                  <input
                    id="photo-upload"
                    type="file"
                    accept="image/*"
                    onChange={handlePhotoUpload}
                    className="hidden"
                  />
                  <p className="text-xs text-slate-500 mt-2">
                    Upload product photo (PNG, JPG up to 5MB)
                  </p>
                  {formData.photo && (
                    <p className="text-xs text-green-600 mt-1">
                      ✓ {formData.photo.name} selected
                    </p>
                  )}
                </div>
              </div>
            </div>
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
            disabled={!formData.code || !formData.nameEN || !formData.productType || !formData.unit || !formData.weight}
            className="bg-green-600 hover:bg-green-700"
          >
            <Save className="h-4 w-4 mr-2" />
            {view === 'edit' ? 'Update' : 'Save'} Product
          </Button>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h3 className="text-lg font-semibold text-slate-900">Product List</h3>
          <p className="text-sm text-slate-600">ထုတ်ကုန်စာရင်း</p>
        </div>
        <Button onClick={handleAddNew} className="bg-blue-600 hover:bg-blue-700">
          <Plus className="h-4 w-4 mr-2" />
          Add Product
        </Button>
      </div>

      {/* Search and Filters */}
      <Card>
        <CardContent className="p-4">
          <div className="flex flex-col sm:flex-row gap-4">
            <div className="relative flex-1">
              <Search className="absolute left-3 top-3 h-4 w-4 text-slate-400" />
              <Input
                placeholder="Search by product code, name..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="pl-10"
              />
            </div>
            <Select value={statusFilter} onValueChange={setStatusFilter}>
              <SelectTrigger className="w-48">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">All Status</SelectItem>
                <SelectItem value="active">Active Only</SelectItem>
                <SelectItem value="inactive">Inactive Only</SelectItem>
              </SelectContent>
            </Select>
          </div>
        </CardContent>
      </Card>

      {/* Products Table */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Package className="h-5 w-5 text-blue-600" />
            Products
            <Badge variant="secondary" className="ml-2">
              {filteredProducts.length} items
            </Badge>
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Product Code</TableHead>
                  <TableHead>Product Name</TableHead>
                  <TableHead>Unit</TableHead>
                  <TableHead>Weight/Unit</TableHead>
                  <TableHead>Packaging</TableHead>
                  <TableHead className="text-center">Status</TableHead>
                  <TableHead className="text-center">Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {filteredProducts.map((product) => (
                  <TableRow key={product.id}>
                    <TableCell>
                      <div>
                        <div className="font-mono font-medium">{product.code}</div>
                        <div className="text-xs">{getProductTypeBadge(product.productType)}</div>
                      </div>
                    </TableCell>
                    <TableCell>
                      <div>
                        <div className="font-medium">{product.nameEN}</div>
                        <div className="text-sm text-slate-500">{product.nameMM}</div>
                      </div>
                    </TableCell>
                    <TableCell>
                      <Badge variant="outline" className="font-mono">
                        {product.unit}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-1">
                        <Weight className="h-4 w-4 text-slate-400" />
                        <span className="font-medium">{product.weight}</span>
                        <span className="text-sm text-slate-500">g</span>
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-1">
                        <Box className="h-4 w-4 text-slate-400" />
                        {packagingTypes.find(p => p.value === product.packaging)?.label}
                      </div>
                    </TableCell>
                    <TableCell className="text-center">
                      {getStatusBadge(product.status)}
                    </TableCell>
                    <TableCell className="text-center">
                      <div className="flex items-center justify-center gap-2">
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={() => handleEdit(product)}
                        >
                          <Edit className="h-4 w-4" />
                        </Button>
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={() => handleDelete(product.id)}
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
          
          {filteredProducts.length === 0 && (
            <div className="text-center py-12">
              <Package className="h-12 w-12 text-slate-300 mx-auto mb-4" />
              <h3 className="text-lg font-medium text-slate-900 mb-2">No products found</h3>
              <p className="text-slate-600 mb-4">
                {searchTerm ? 'Try adjusting your search criteria.' : 'Get started by adding your first product.'}
              </p>
              {!searchTerm && (
                <Button onClick={handleAddNew}>
                  <Plus className="h-4 w-4 mr-2" />
                  Add Product
                </Button>
              )}
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}