import React, { useState } from 'react';
import { 
  Package, 
  Palette, 
  Link, 
  TreePine,
  Plus,
  Search,
  Filter,
  Download,
  Edit,
  Trash2,
  Eye,
  Upload,
  Save,
  RotateCcw,
  ChevronRight,
  Tag,
  Check,
  Settings
} from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Badge } from './ui/badge';
import { Checkbox } from './ui/checkbox';
import { Tabs, TabsContent, TabsList, TabsTrigger } from './ui/tabs';
import { Switch } from './ui/switch';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from './ui/dialog';
import { toast } from 'sonner@2.0.3';

// Mock data for demonstration
const productTypes = [
  { value: 'parent', label: 'Parent Product' },
  { value: 'sub-part', label: 'Sub-part' },
  { value: 'finished', label: 'Finished Product' }
];

const unitMeasures = [
  { value: 'pcs', label: 'Pieces (pcs)' },
  { value: 'kg', label: 'Kilogram (kg)' },
  { value: 'mt', label: 'Meter (mt)' },
  { value: 'ltr', label: 'Liter (ltr)' }
];



const mockGroups = [
  { id: 'GRP-A', name: 'Group A', color: '#3B82F6', description: 'Standard plastic containers', createdBy: 'Admin', updatedAt: '2024-01-15' },
  { id: 'GRP-B', name: 'Group B', color: '#10B981', description: 'Bottle caps and closures', createdBy: 'Planner', updatedAt: '2024-01-14' },
  { id: 'GRP-C', name: 'Group C', color: '#F59E0B', description: 'Large storage items', createdBy: 'Manager', updatedAt: '2024-01-13' }
];

const mockProducts = [
  {
    id: '2011',
    code: '2011',
    nameEn: 'Plastic Bottle 500ml',
    nameLocal: 'ပလတ်စတစ်ပုလင်း ၅၀၀မီလီ',
    type: 'parent',
    unitMeasure: 'pcs',
    weight: 25.5,
    defaultColor: 'WHITE',
    groupId: 'GRP-A',
    groupName: 'Group A',
    status: 'active'
  },
  {
    id: '2011-01',
    code: '2011-01',
    nameEn: 'Bottle Body',
    nameLocal: 'ပוလင်းကိုယ်ခန္ဓာ',
    type: 'sub-part',
    unitMeasure: 'pcs',
    weight: 20.0,
    defaultColor: 'WHITE',
    groupId: 'GRP-A',
    groupName: 'Group A',
    status: 'active'
  },
  {
    id: '2011-02',
    code: '2011-02',
    nameEn: 'Bottle Cap',
    nameLocal: 'ပုলင်းအဖုံး',
    type: 'sub-part',
    unitMeasure: 'pcs',
    weight: 3.5,
    defaultColor: 'BLUE',
    groupId: 'GRP-B',
    groupName: 'Group B',
    status: 'active'
  }
];

const mockColors = [
  { id: 'WHITE', code: 'WHITE', name: 'White', hexCode: '#FFFFFF', status: 'active', updatedBy: 'Admin', updatedDate: '2024-01-10' },
  { id: 'BLUE', code: 'BLUE', name: 'Blue', hexCode: '#0066CC', status: 'active', updatedBy: 'Admin', updatedDate: '2024-01-10' },
  { id: 'RED', code: 'RED', name: 'Red', hexCode: '#CC0000', status: 'active', updatedBy: 'Planner', updatedDate: '2024-01-09' },
  { id: 'GREEN', code: 'GREEN', name: 'Green', hexCode: '#00AA00', status: 'active', updatedBy: 'Admin', updatedDate: '2024-01-09' },
  { id: 'BLACK', code: 'BLACK', name: 'Black', hexCode: '#000000', status: 'active', updatedBy: 'Admin', updatedDate: '2024-01-08' },
  { id: 'TRANS-CLEAR', code: 'TRANS-CLEAR', name: 'Transparent Clear', hexCode: '#F0F0F0', status: 'active', updatedBy: 'Planner', updatedDate: '2024-01-08' }
];

const mockMolds = [
  { id: 'M-001', code: 'M-001', name: 'Bottle Body Mold A' },
  { id: 'M-002', code: 'M-002', name: 'Bottle Cap Mold B' },
  { id: 'M-003', code: 'M-003', name: 'Container Mold C' },
  { id: 'M-004', code: 'M-004', name: 'Label Die D' }
];

const mockProductColorMappings = [
  { 
    id: 1,
    productId: '2011', 
    productName: 'Plastic Bottle 500ml', 
    colors: ['WHITE', 'BLUE', 'RED'],
    defaultColor: 'WHITE',
    effectiveFrom: '2024-01-01',
    status: 'active',
    updatedBy: 'Admin',
    updatedDate: '2024-01-15'
  },
  { 
    id: 2,
    productId: '2011-01', 
    productName: 'Bottle Body', 
    colors: ['WHITE', 'BLUE'],
    defaultColor: 'WHITE',
    effectiveFrom: '2024-01-01',
    status: 'active',
    updatedBy: 'Planner',
    updatedDate: '2024-01-14'
  },
  { 
    id: 3,
    productId: '2011-02', 
    productName: 'Bottle Cap', 
    colors: ['BLUE', 'RED', 'BLACK'],
    defaultColor: 'BLUE',
    effectiveFrom: '2024-01-01',
    status: 'active',
    updatedBy: 'Admin',
    updatedDate: '2024-01-14'
  }
];

const mockParentChildMappings = [
  {
    id: 1,
    parentId: '2011',
    parentName: 'Plastic Bottle 500ml',
    childId: '2011-01',
    childName: 'Bottle Body',
    notes: 'Main body component',
    status: 'active',
    updatedBy: 'Admin',
    updatedDate: '2024-01-15'
  },
  {
    id: 2,
    parentId: '2011',
    parentName: 'Plastic Bottle 500ml',
    childId: '2011-02',
    childName: 'Bottle Cap',
    notes: 'Cap component',
    status: 'active',
    updatedBy: 'Admin',
    updatedDate: '2024-01-15'
  }
];

const mockProductMoldMappings = [
  {
    id: 1,
    productId: '2011-01',
    productName: 'Bottle Body',
    moldId: 'M-001',
    moldName: 'Bottle Body Mold A',
    cavities: 4,
    cycleTime: 45,
    status: 'active',
    updatedBy: 'Admin',
    updatedDate: '2024-01-15'
  },
  {
    id: 2,
    productId: '2011-02',
    productName: 'Bottle Cap',
    moldId: 'M-002',
    moldName: 'Bottle Cap Mold B',
    cavities: 8,
    cycleTime: 30,
    status: 'active',
    updatedBy: 'Admin',
    updatedDate: '2024-01-14'
  }
];

export function ProductMaster() {
  const [activeTab, setActiveTab] = useState('product-registration');
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedProductType, setSelectedProductType] = useState('all');
  const [selectedStatus, setSelectedStatus] = useState('all');

  // Form states
  const [productForm, setProductForm] = useState({
    code: '',
    nameEn: '',
    nameLocal: '',
    type: 'finished',
    unitMeasure: 'pcs',
    weight: '',
    defaultColor: '',
    groupId: '',
    status: 'active'
  });

  const [colorForm, setColorForm] = useState({
    code: '',
    name: '',
    hexCode: '#FFFFFF',
    status: true
  });

  const [productColorMappingForm, setProductColorMappingForm] = useState({
    productId: '',
    colors: [] as string[],
    defaultColor: '',
    effectiveFrom: '',
    status: true
  });

  const [parentChildMappingForm, setParentChildMappingForm] = useState({
    parentId: '',
    childId: '',
    notes: '',
    status: true
  });

  const [productMoldMappingForm, setProductMoldMappingForm] = useState({
    productId: '',
    moldId: '',
    cavities: '',
    cycleTime: '',
    status: true
  });

  const [groupForm, setGroupForm] = useState({
    name: '',
    color: '#3B82F6',
    description: ''
  });

  const [groups, setGroups] = useState(mockGroups);
  const [showAddGroupDialog, setShowAddGroupDialog] = useState(false);
  const [editingGroup, setEditingGroup] = useState<any>(null);

  const handleAddGroup = () => {
    if (!groupForm.name.trim()) {
      toast.error('Group name is required');
      return;
    }

    const existingGroup = groups.find(g => g.name.toLowerCase() === groupForm.name.toLowerCase());
    if (existingGroup) {
      toast.error('Group name already exists');
      return;
    }

    const newGroup = {
      id: `GRP-${Date.now()}`,
      name: groupForm.name,
      color: groupForm.color,
      description: groupForm.description,
      createdBy: 'Current User',
      updatedAt: new Date().toISOString().split('T')[0]
    };

    setGroups(prev => [...prev, newGroup]);
    setGroupForm({ name: '', color: '#3B82F6', description: '' });
    setShowAddGroupDialog(false);
    toast.success('Group added successfully | အုပ်စုအသစ်ထည့်ပြီးပါပြီး');
  };

  const handleEditGroup = (group: any) => {
    setEditingGroup(group);
    setGroupForm({
      name: group.name,
      color: group.color,
      description: group.description
    });
    setShowAddGroupDialog(true);
  };

  const handleUpdateGroup = () => {
    if (!groupForm.name.trim()) {
      toast.error('Group name is required');
      return;
    }

    const existingGroup = groups.find(g => g.name.toLowerCase() === groupForm.name.toLowerCase() && g.id !== editingGroup.id);
    if (existingGroup) {
      toast.error('Group name already exists');
      return;
    }

    setGroups(prev => prev.map(g => 
      g.id === editingGroup.id 
        ? { ...g, name: groupForm.name, color: groupForm.color, description: groupForm.description, updatedAt: new Date().toISOString().split('T')[0] }
        : g
    ));

    setEditingGroup(null);
    setGroupForm({ name: '', color: '#3B82F6', description: '' });
    setShowAddGroupDialog(false);
    toast.success('Group updated successfully');
  };

  const handleDeleteGroup = (groupId: string) => {
    const hasProducts = mockProducts.some(p => p.groupId === groupId);
    if (hasProducts) {
      toast.error('Cannot delete group that has assigned products');
      return;
    }

    setGroups(prev => prev.filter(g => g.id !== groupId));
    toast.success('Group deleted successfully');
  };

  const renderGroupManagement = () => (
    <div className="space-y-6">
      {/* Group Management Form */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Tag className="h-5 w-5 text-orange-600" />
            Group Management
            <span className="text-sm font-normal text-slate-600">အုပ်စုစီမံခန့်ခွဲမှု</span>
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex items-center justify-between mb-4">
            <p className="text-slate-600">
              Manage product groups for delivery order organization
            </p>
            <Button onClick={() => setShowAddGroupDialog(true)} className="bg-orange-600 hover:bg-orange-700">
              <Plus className="h-4 w-4 mr-2" />
              Add Group | အုပ်စုအသစ်ထည့်ရန်
            </Button>
          </div>

          {/* Groups Table */}
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Group Name | အုပ်စုအမည်</TableHead>
                  <TableHead>Description | ဖော်ပြချက်</TableHead>
                  <TableHead>Created By | ဖန်တီးသူ</TableHead>
                  <TableHead>Last Updated | နောက်ဆုံးပြင်ဆင်</TableHead>
                  <TableHead className="text-center">Actions | လုပ်ဆောင်ချက်များ</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {groups.map((group) => (
                  <TableRow key={group.id}>
                    <TableCell>
                      <div className="flex items-center gap-2">
                        <div 
                          className="w-4 h-4 rounded-full" 
                          style={{ backgroundColor: group.color }}
                        />
                        <span className="font-medium">{group.name}</span>
                      </div>
                    </TableCell>
                    <TableCell>
                      <span className="text-slate-600">{group.description}</span>
                    </TableCell>
                    <TableCell>{group.createdBy}</TableCell>
                    <TableCell>{group.updatedAt}</TableCell>
                    <TableCell className="text-center">
                      <div className="flex items-center justify-center gap-1">
                        <Button 
                          variant="ghost" 
                          size="sm"
                          onClick={() => handleEditGroup(group)}
                        >
                          <Edit className="h-4 w-4" />
                        </Button>
                        <Button 
                          variant="ghost" 
                          size="sm" 
                          className="text-red-600 hover:text-red-700"
                          onClick={() => handleDeleteGroup(group.id)}
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
  );

  const renderProductRegistration = () => (
    <div className="space-y-6">
      {/* Registration Form */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Package className="h-5 w-5 text-blue-600" />
            Product Code Registration
            <span className="text-sm font-normal text-slate-600">ထုတ်ကုန်ကုဒ်မှတ်ပုံတင်ခြင်း</span>
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <div>
              <Label htmlFor="productCode">Product Code *</Label>
              <Input
                id="productCode"
                placeholder="e.g., 2011"
                value={productForm.code}
                onChange={(e) => setProductForm(prev => ({ ...prev, code: e.target.value }))}
              />
            </div>
            
            <div>
              <Label htmlFor="productNameEn">Product Name (EN) *</Label>
              <Input
                id="productNameEn"
                placeholder="English name"
                value={productForm.nameEn}
                onChange={(e) => setProductForm(prev => ({ ...prev, nameEn: e.target.value }))}
              />
            </div>
            
            <div>
              <Label htmlFor="productNameLocal">Product Name (Local) *</Label>
              <Input
                id="productNameLocal"
                placeholder="မြန်မာအမည်"
                value={productForm.nameLocal}
                onChange={(e) => setProductForm(prev => ({ ...prev, nameLocal: e.target.value }))}
              />
            </div>
            
            <div>
              <Label htmlFor="productType">Product Type *</Label>
              <Select value={productForm.type} onValueChange={(value) => setProductForm(prev => ({ ...prev, type: value }))}>
                <SelectTrigger>
                  <SelectValue placeholder="Select type" />
                </SelectTrigger>
                <SelectContent>
                  {productTypes.map((type) => (
                    <SelectItem key={type.value} value={type.value}>
                      {type.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            
            <div>
              <Label htmlFor="unitMeasure">Unit of Measure *</Label>
              <Select value={productForm.unitMeasure} onValueChange={(value) => setProductForm(prev => ({ ...prev, unitMeasure: value }))}>
                <SelectTrigger>
                  <SelectValue placeholder="Select unit" />
                </SelectTrigger>
                <SelectContent>
                  {unitMeasures.map((unit) => (
                    <SelectItem key={unit.value} value={unit.value}>
                      {unit.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            
            <div>
              <Label htmlFor="weight">Weight per Unit (g)</Label>
              <Input
                id="weight"
                type="number"
                placeholder="25.5"
                value={productForm.weight}
                onChange={(e) => setProductForm(prev => ({ ...prev, weight: e.target.value }))}
              />
            </div>
            
            <div>
              <Label htmlFor="defaultColor">Default Color</Label>
              <Select value={productForm.defaultColor} onValueChange={(value) => setProductForm(prev => ({ ...prev, defaultColor: value }))}>
                <SelectTrigger>
                  <SelectValue placeholder="Select color" />
                </SelectTrigger>
                <SelectContent>
                  {mockColors.map((color) => (
                    <SelectItem key={color.id} value={color.id}>
                      <div className="flex items-center gap-2">
                        <div className="w-4 h-4 rounded border" style={{ backgroundColor: color.hexCode }}></div>
                        {color.name}
                      </div>
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div>
              <Label htmlFor="productGroup">Assign Group | အုပ်စုသတ်မှတ်ရန်</Label>
              <div className="flex gap-2">
                <Select value={productForm.groupId} onValueChange={(value) => setProductForm(prev => ({ ...prev, groupId: value }))}>
                  <SelectTrigger className="flex-1">
                    <SelectValue placeholder="Select group..." />
                  </SelectTrigger>
                  <SelectContent>
                    {groups.map((group) => (
                      <SelectItem key={group.id} value={group.id}>
                        <div className="flex items-center gap-2">
                          <div className="w-3 h-3 rounded-full" style={{ backgroundColor: group.color }}></div>
                          {group.name}
                        </div>
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
                <Button 
                  type="button" 
                  variant="outline" 
                  size="sm"
                  onClick={() => setShowAddGroupDialog(true)}
                  className="px-3"
                >
                  <Plus className="h-4 w-4" />
                  Add Group
                </Button>
              </div>
              <p className="text-xs text-slate-500 mt-1">
                This product belongs to Group. မြန်မာ: ဤထုတ်ကုန်သည် အုပ်စု အောက်တွင် ပါဝင်သည်။
              </p>
            </div>
            
            <div className="flex items-center space-x-2">
              <Switch
                id="status"
                checked={productForm.status === 'active'}
                onCheckedChange={(checked) => setProductForm(prev => ({ ...prev, status: checked ? 'active' : 'inactive' }))}
              />
              <Label htmlFor="status">Active Status</Label>
            </div>
          </div>
          
          <div className="flex gap-2 pt-4">
            <Button>
              <Save className="h-4 w-4 mr-2" />
              Save Product
            </Button>
            <Button variant="outline">
              <RotateCcw className="h-4 w-4 mr-2" />
              Reset
            </Button>
          </div>
        </CardContent>
      </Card>

      {/* Product Table */}
      <Card>
        <CardHeader>
          <div className="flex items-center justify-between">
            <CardTitle>Registered Products</CardTitle>
            <div className="flex gap-2">
              <Button variant="outline" size="sm">
                <Filter className="h-4 w-4 mr-2" />
                Filter
              </Button>
              <Button variant="outline" size="sm">
                <Download className="h-4 w-4 mr-2" />
                Export
              </Button>
            </div>
          </div>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Product Code</TableHead>
                  <TableHead>Product Name</TableHead>
                  <TableHead>Type</TableHead>
                  <TableHead>Group | အုပ်စု</TableHead>
                  <TableHead>Unit</TableHead>
                  <TableHead>Weight (g)</TableHead>
                  <TableHead>Default Color</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead className="text-center">Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {mockProducts.map((product) => (
                  <TableRow key={product.id}>
                    <TableCell className="font-mono">{product.code}</TableCell>
                    <TableCell>
                      <div>
                        <div className="font-medium">{product.nameEn}</div>
                        <div className="text-sm text-slate-600">{product.nameLocal}</div>
                      </div>
                    </TableCell>
                    <TableCell>
                      <Badge variant={product.type === 'parent' ? 'default' : product.type === 'sub-part' ? 'secondary' : 'outline'}>
                        {product.type}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-2">
                        <div 
                          className="w-3 h-3 rounded-full" 
                          style={{ backgroundColor: groups.find(g => g.id === product.groupId)?.color || '#6B7280' }}
                        />
                        <span className="font-medium">{product.groupName}</span>
                      </div>
                    </TableCell>
                    <TableCell>{product.unitMeasure}</TableCell>
                    <TableCell>{product.weight}</TableCell>
                    <TableCell>
                      <div className="flex items-center gap-2">
                        <div className="w-4 h-4 rounded border bg-white"></div>
                        {product.defaultColor}
                      </div>
                    </TableCell>
                    <TableCell>
                      <Badge variant={product.status === 'active' ? 'default' : 'secondary'}>
                        {product.status}
                      </Badge>
                    </TableCell>
                    <TableCell className="text-center">
                      <div className="flex items-center justify-center gap-1">
                        <Button variant="ghost" size="sm">
                          <Eye className="h-4 w-4" />
                        </Button>
                        <Button variant="ghost" size="sm">
                          <Edit className="h-4 w-4" />
                        </Button>
                        <Button variant="ghost" size="sm" className="text-red-600 hover:text-red-700">
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
  );

  const handleReset = (formType: string) => {
    switch(formType) {
      case 'color':
        setColorForm({
          code: '',
          name: '',
          hexCode: '#FFFFFF',
          status: true
        });
        break;
      case 'productColor':
        setProductColorMappingForm({
          productId: '',
          colors: [],
          defaultColor: '',
          effectiveFrom: '',
          status: true
        });
        break;
      case 'parentChild':
        setParentChildMappingForm({
          parentId: '',
          childId: '',
          notes: '',
          status: true
        });
        break;
      case 'productMold':
        setProductMoldMappingForm({
          productId: '',
          moldId: '',
          cavities: '',
          cycleTime: '',
          status: true
        });
        break;
    }
  };

  const renderColorRegistration = () => (
    <div className="space-y-6">
      {/* Color Registration Form */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Palette className="h-5 w-5 text-purple-600" />
            Color Code Registration
            <span className="text-sm font-normal text-slate-600">အရောင်ကုဒ်မှတ်ပုံတင်ခြင်း</span>
          </CardTitle>
          <p className="text-sm text-slate-500 mt-2">Color data used for planner and reporting only.</p>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <Label htmlFor="colorCode">Color Code *</Label>
              <Input
                id="colorCode"
                placeholder="e.g., BLUE"
                value={colorForm.code}
                onChange={(e) => setColorForm(prev => ({ ...prev, code: e.target.value.toUpperCase() }))}
              />
            </div>
            
            <div>
              <Label htmlFor="colorName">Color Name *</Label>
              <Input
                id="colorName"
                placeholder="Blue"
                value={colorForm.name}
                onChange={(e) => setColorForm(prev => ({ ...prev, name: e.target.value }))}
              />
            </div>
            
            <div>
              <Label htmlFor="colorHex">Color Picker / Hex Code</Label>
              <div className="flex gap-2">
                <input
                  type="color"
                  id="colorPicker"
                  value={colorForm.hexCode}
                  onChange={(e) => setColorForm(prev => ({ ...prev, hexCode: e.target.value }))}
                  className="w-12 h-10 rounded border cursor-pointer"
                />
                <Input
                  id="colorHex"
                  placeholder="#FFFFFF"
                  value={colorForm.hexCode}
                  onChange={(e) => setColorForm(prev => ({ ...prev, hexCode: e.target.value }))}
                  className="flex-1"
                />
                <div 
                  className="w-12 h-10 rounded border shadow-sm"
                  style={{ backgroundColor: colorForm.hexCode }}
                  title="Live preview"
                ></div>
              </div>
            </div>
            
            <div className="flex items-center space-x-2">
              <Switch
                id="colorStatus"
                checked={colorForm.status}
                onCheckedChange={(checked) => setColorForm(prev => ({ ...prev, status: checked }))}
              />
              <Label htmlFor="colorStatus">Active Status</Label>
            </div>
          </div>
          
          <div className="flex gap-2 pt-4 justify-end">
            <Button className="bg-blue-600 hover:bg-blue-700">
              <Save className="h-4 w-4 mr-2" />
              Save
            </Button>
            <Button variant="outline" onClick={() => handleReset('color')}>
              <RotateCcw className="h-4 w-4 mr-2" />
              Reset
            </Button>
          </div>
        </CardContent>
      </Card>

      {/* Colors Table */}
      <Card>
        <CardHeader>
          <div className="flex items-center justify-between">
            <CardTitle>Registered Colors</CardTitle>
            <div className="flex gap-2">
              <Button variant="outline" size="sm">
                <Download className="h-4 w-4 mr-2" />
                Export
              </Button>
            </div>
          </div>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Color Code</TableHead>
                  <TableHead>Color Name</TableHead>
                  <TableHead>Hex Preview</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Updated By</TableHead>
                  <TableHead>Updated Date</TableHead>
                  <TableHead className="text-center">Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {mockColors.map((color) => (
                  <TableRow key={color.id}>
                    <TableCell className="font-mono">{color.code}</TableCell>
                    <TableCell>
                      <div className="flex items-center gap-2">
                        <div 
                          className="w-5 h-5 rounded border shadow-sm" 
                          style={{ backgroundColor: color.hexCode }}
                        ></div>
                        <span className="font-medium">{color.name}</span>
                      </div>
                    </TableCell>
                    <TableCell className="font-mono text-sm">{color.hexCode}</TableCell>
                    <TableCell>
                      <Badge variant={color.status === 'active' ? 'default' : 'secondary'}>
                        {color.status}
                      </Badge>
                    </TableCell>
                    <TableCell>{color.updatedBy}</TableCell>
                    <TableCell>{color.updatedDate}</TableCell>
                    <TableCell className="text-center">
                      <div className="flex items-center justify-center gap-1">
                        <Button variant="ghost" size="sm">
                          <Edit className="h-4 w-4" />
                        </Button>
                        <Button variant="ghost" size="sm" className="text-red-600 hover:text-red-700">
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
  );

  const renderProductColorMapping = () => (
    <div className="space-y-6">
      {/* Mapping Form */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Link className="h-5 w-5 text-green-600" />
            Product-Color Mapping
            <span className="text-sm font-normal text-slate-600">ထုတ်ကုန်-အရောင်ပေါင်းစပ်မှု</span>
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <Label htmlFor="selectProduct">Product *</Label>
              <Select 
                value={productColorMappingForm.productId} 
                onValueChange={(value) => setProductColorMappingForm(prev => ({ ...prev, productId: value }))}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Choose product" />
                </SelectTrigger>
                <SelectContent>
                  {mockProducts.map((product) => (
                    <SelectItem key={product.id} value={product.id}>
                      {product.code} - {product.nameEn}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            
            <div>
              <Label htmlFor="selectColors">Available Colors * (Multi-select)</Label>
              <div className="border rounded-md p-2 bg-white min-h-10">
                <div className="flex flex-wrap gap-2">
                  {mockColors.map((color) => (
                    <div key={color.id} className="flex items-center gap-1">
                      <Checkbox 
                        id={`color-${color.id}`}
                        checked={productColorMappingForm.colors.includes(color.id)}
                        onCheckedChange={(checked) => {
                          setProductColorMappingForm(prev => ({
                            ...prev,
                            colors: checked 
                              ? [...prev.colors, color.id]
                              : prev.colors.filter(c => c !== color.id)
                          }));
                        }}
                      />
                      <Label htmlFor={`color-${color.id}`} className="cursor-pointer flex items-center gap-1">
                        <div 
                          className="w-4 h-4 rounded border" 
                          style={{ backgroundColor: color.hexCode }}
                        ></div>
                        <span className="text-sm">{color.name}</span>
                      </Label>
                    </div>
                  ))}
                </div>
              </div>
            </div>

            <div>
              <Label htmlFor="defaultColor">Default Color</Label>
              <Select 
                value={productColorMappingForm.defaultColor}
                onValueChange={(value) => setProductColorMappingForm(prev => ({ ...prev, defaultColor: value }))}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Select default color" />
                </SelectTrigger>
                <SelectContent>
                  {mockColors
                    .filter(color => productColorMappingForm.colors.includes(color.id))
                    .map((color) => (
                      <SelectItem key={color.id} value={color.id}>
                        <div className="flex items-center gap-2">
                          <div className="w-4 h-4 rounded border" style={{ backgroundColor: color.hexCode }}></div>
                          {color.name}
                        </div>
                      </SelectItem>
                    ))}
                </SelectContent>
              </Select>
            </div>

            <div>
              <Label htmlFor="effectiveFrom">Effective From</Label>
              <Input
                id="effectiveFrom"
                type="date"
                value={productColorMappingForm.effectiveFrom}
                onChange={(e) => setProductColorMappingForm(prev => ({ ...prev, effectiveFrom: e.target.value }))}
              />
            </div>
            
            <div className="flex items-center space-x-2">
              <Switch
                id="mappingStatus"
                checked={productColorMappingForm.status}
                onCheckedChange={(checked) => setProductColorMappingForm(prev => ({ ...prev, status: checked }))}
              />
              <Label htmlFor="mappingStatus">Active Status</Label>
            </div>
          </div>
          
          <div className="flex gap-2 pt-4 justify-end">
            <Button className="bg-blue-600 hover:bg-blue-700">
              <Save className="h-4 w-4 mr-2" />
              Save
            </Button>
            <Button variant="outline" onClick={() => handleReset('productColor')}>
              <RotateCcw className="h-4 w-4 mr-2" />
              Reset
            </Button>
          </div>
        </CardContent>
      </Card>

      {/* Mapping Table */}
      <Card>
        <CardHeader>
          <div className="flex items-center justify-between">
            <CardTitle>Product-Color Mapping</CardTitle>
            <div className="flex gap-2">
              <Button variant="outline" size="sm">
                <Download className="h-4 w-4 mr-2" />
                Export
              </Button>
            </div>
          </div>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Product</TableHead>
                  <TableHead>Assigned Colors</TableHead>
                  <TableHead>Default Color</TableHead>
                  <TableHead>Effective From</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Updated By</TableHead>
                  <TableHead>Updated Date</TableHead>
                  <TableHead className="text-center">Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {mockProductColorMappings.map((mapping) => (
                  <TableRow key={mapping.id}>
                    <TableCell>
                      <div>
                        <div className="font-mono text-sm">{mapping.productId}</div>
                        <div className="text-sm text-slate-600">{mapping.productName}</div>
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex flex-wrap gap-1">
                        {mapping.colors.map(colorId => {
                          const color = mockColors.find(c => c.id === colorId);
                          return color ? (
                            <div key={colorId} className="flex items-center gap-1 bg-slate-100 rounded px-2 py-1">
                              <div 
                                className="w-3 h-3 rounded border" 
                                style={{ backgroundColor: color.hexCode }}
                              ></div>
                              <span className="text-xs">{color.name}</span>
                            </div>
                          ) : null;
                        })}
                      </div>
                    </TableCell>
                    <TableCell>
                      {mockColors.find(c => c.id === mapping.defaultColor)?.name}
                    </TableCell>
                    <TableCell>{mapping.effectiveFrom}</TableCell>
                    <TableCell>
                      <Badge variant={mapping.status === 'active' ? 'default' : 'secondary'}>
                        {mapping.status}
                      </Badge>
                    </TableCell>
                    <TableCell>{mapping.updatedBy}</TableCell>
                    <TableCell>{mapping.updatedDate}</TableCell>
                    <TableCell className="text-center">
                      <div className="flex items-center justify-center gap-1">
                        <Button variant="ghost" size="sm">
                          <Edit className="h-4 w-4" />
                        </Button>
                        <Button variant="ghost" size="sm" className="text-red-600 hover:text-red-700">
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
  );

  const renderParentChildMapping = () => (
    <div className="space-y-6">
      {/* Parent-Child Mapping Form */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <TreePine className="h-5 w-5 text-orange-600" />
            Parent-Child Product Mapping
            <span className="text-sm font-normal text-slate-600">မူလ-ခွဲထုတ်ကုန်ပေါင်းစပ်မှု</span>
          </CardTitle>
          <p className="text-sm text-slate-500 mt-2">
            Parent–Child သတ်မှတ်ခြင်းသည် Planner အတွက်သာ ဖြစ်ပြီး Inventory မထိပါ။
          </p>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <Label htmlFor="parentProduct">Parent Product *</Label>
              <Select 
                value={parentChildMappingForm.parentId}
                onValueChange={(value) => setParentChildMappingForm(prev => ({ ...prev, parentId: value }))}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Select parent product" />
                </SelectTrigger>
                <SelectContent>
                  {mockProducts.map((product) => (
                    <SelectItem key={product.id} value={product.id}>
                      {product.code} - {product.nameEn}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            
            <div>
              <Label htmlFor="childProduct">Child Product *</Label>
              <Select 
                value={parentChildMappingForm.childId}
                onValueChange={(value) => setParentChildMappingForm(prev => ({ ...prev, childId: value }))}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Select child product" />
                </SelectTrigger>
                <SelectContent>
                  {mockProducts
                    .filter(p => p.id !== parentChildMappingForm.parentId)
                    .map((product) => (
                      <SelectItem key={product.id} value={product.id}>
                        {product.code} - {product.nameEn}
                      </SelectItem>
                    ))}
                </SelectContent>
              </Select>
            </div>

            <div>
              <Label htmlFor="notes">Notes</Label>
              <Input
                id="notes"
                placeholder="Additional notes"
                value={parentChildMappingForm.notes}
                onChange={(e) => setParentChildMappingForm(prev => ({ ...prev, notes: e.target.value }))}
              />
            </div>
            
            <div className="flex items-center space-x-2">
              <Switch
                id="parentChildStatus"
                checked={parentChildMappingForm.status}
                onCheckedChange={(checked) => setParentChildMappingForm(prev => ({ ...prev, status: checked }))}
              />
              <Label htmlFor="parentChildStatus">Active Status</Label>
            </div>
          </div>
          
          <div className="flex gap-2 pt-4 justify-end">
            <Button className="bg-blue-600 hover:bg-blue-700">
              <Save className="h-4 w-4 mr-2" />
              Save
            </Button>
            <Button variant="outline" onClick={() => handleReset('parentChild')}>
              <RotateCcw className="h-4 w-4 mr-2" />
              Reset
            </Button>
          </div>
        </CardContent>
      </Card>

      {/* Parent-Child Mapping Table */}
      <Card>
        <CardHeader>
          <div className="flex items-center justify-between">
            <CardTitle>Parent-Child Mappings</CardTitle>
            <div className="flex gap-2">
              <Button variant="outline" size="sm">
                <Download className="h-4 w-4 mr-2" />
                Export
              </Button>
            </div>
          </div>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Parent</TableHead>
                  <TableHead>Child</TableHead>
                  <TableHead>Notes</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Updated By</TableHead>
                  <TableHead>Updated Date</TableHead>
                  <TableHead className="text-center">Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {mockParentChildMappings.map((mapping) => (
                  <TableRow key={mapping.id}>
                    <TableCell>
                      <div>
                        <div className="font-mono text-sm">{mapping.parentId}</div>
                        <div className="text-sm text-slate-600">{mapping.parentName}</div>
                      </div>
                    </TableCell>
                    <TableCell>
                      <div>
                        <div className="font-mono text-sm">{mapping.childId}</div>
                        <div className="text-sm text-slate-600">{mapping.childName}</div>
                      </div>
                    </TableCell>
                    <TableCell className="text-sm text-slate-600">{mapping.notes}</TableCell>
                    <TableCell>
                      <Badge variant={mapping.status === 'active' ? 'default' : 'secondary'}>
                        {mapping.status}
                      </Badge>
                    </TableCell>
                    <TableCell>{mapping.updatedBy}</TableCell>
                    <TableCell>{mapping.updatedDate}</TableCell>
                    <TableCell className="text-center">
                      <div className="flex items-center justify-center gap-1">
                        <Button variant="ghost" size="sm">
                          <Edit className="h-4 w-4" />
                        </Button>
                        <Button variant="ghost" size="sm" className="text-red-600 hover:text-red-700">
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
  );

  const renderProductMoldMapping = () => (
    <div className="space-y-6">
      {/* Product-Mold Mapping Form */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Settings className="h-5 w-5 text-indigo-600" />
            Product-Mold Mapping
            <span className="text-sm font-normal text-slate-600">ထုတ်ကုန်-မော်ဒယ်ပေါင်းစပ်မှု</span>
          </CardTitle>
          <p className="text-sm text-slate-500 mt-2">
            Planner မှာ Product ရွေးချယ်သည့်အခါ ဤ Mapping အပေါ် မူတည်၍ မော်ဒယ်များကို ပြသမည်။ Machine များသည် Mold–Machine Mapping မှ ပေါ်လာမည်။
          </p>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <Label htmlFor="productMold">Product *</Label>
              <Select 
                value={productMoldMappingForm.productId}
                onValueChange={(value) => setProductMoldMappingForm(prev => ({ ...prev, productId: value }))}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Select product" />
                </SelectTrigger>
                <SelectContent>
                  {mockProducts.map((product) => (
                    <SelectItem key={product.id} value={product.id}>
                      {product.code} - {product.nameEn}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            
            <div>
              <Label htmlFor="moldSelect">Mold *</Label>
              <Select 
                value={productMoldMappingForm.moldId}
                onValueChange={(value) => setProductMoldMappingForm(prev => ({ ...prev, moldId: value }))}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Select mold (from Master Data)" />
                </SelectTrigger>
                <SelectContent>
                  {mockMolds.map((mold) => (
                    <SelectItem key={mold.id} value={mold.id}>
                      {mold.code} - {mold.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div>
              <Label htmlFor="cavities">Cavities</Label>
              <Input
                id="cavities"
                type="number"
                placeholder="4"
                value={productMoldMappingForm.cavities}
                onChange={(e) => setProductMoldMappingForm(prev => ({ ...prev, cavities: e.target.value }))}
              />
            </div>

            <div>
              <Label htmlFor="cycleTime">Cycle Time (sec)</Label>
              <Input
                id="cycleTime"
                type="number"
                placeholder="45"
                value={productMoldMappingForm.cycleTime}
                onChange={(e) => setProductMoldMappingForm(prev => ({ ...prev, cycleTime: e.target.value }))}
              />
            </div>
            
            <div className="flex items-center space-x-2">
              <Switch
                id="productMoldStatus"
                checked={productMoldMappingForm.status}
                onCheckedChange={(checked) => setProductMoldMappingForm(prev => ({ ...prev, status: checked }))}
              />
              <Label htmlFor="productMoldStatus">Active Status</Label>
            </div>
          </div>
          
          <div className="flex gap-2 pt-4 justify-end">
            <Button className="bg-blue-600 hover:bg-blue-700">
              <Save className="h-4 w-4 mr-2" />
              Save
            </Button>
            <Button variant="outline" onClick={() => handleReset('productMold')}>
              <RotateCcw className="h-4 w-4 mr-2" />
              Reset
            </Button>
          </div>
        </CardContent>
      </Card>

      {/* Product-Mold Mapping Table */}
      <Card>
        <CardHeader>
          <div className="flex items-center justify-between">
            <CardTitle>Product-Mold Mappings</CardTitle>
            <div className="flex gap-2">
              <Button variant="outline" size="sm">
                <Download className="h-4 w-4 mr-2" />
                Export
              </Button>
            </div>
          </div>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Product</TableHead>
                  <TableHead>Mold</TableHead>
                  <TableHead>Cavities</TableHead>
                  <TableHead>Cycle Time</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Updated By</TableHead>
                  <TableHead>Updated Date</TableHead>
                  <TableHead className="text-center">Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {mockProductMoldMappings.map((mapping) => (
                  <TableRow key={mapping.id}>
                    <TableCell>
                      <div>
                        <div className="font-mono text-sm">{mapping.productId}</div>
                        <div className="text-sm text-slate-600">{mapping.productName}</div>
                      </div>
                    </TableCell>
                    <TableCell>
                      <div>
                        <div className="font-mono text-sm">{mapping.moldId}</div>
                        <div className="text-sm text-slate-600">{mapping.moldName}</div>
                      </div>
                    </TableCell>
                    <TableCell>{mapping.cavities}</TableCell>
                    <TableCell>{mapping.cycleTime} sec</TableCell>
                    <TableCell>
                      <Badge variant={mapping.status === 'active' ? 'default' : 'secondary'}>
                        {mapping.status}
                      </Badge>
                    </TableCell>
                    <TableCell>{mapping.updatedBy}</TableCell>
                    <TableCell>{mapping.updatedDate}</TableCell>
                    <TableCell className="text-center">
                      <div className="flex items-center justify-center gap-1">
                        <Button variant="ghost" size="sm">
                          <Edit className="h-4 w-4" />
                        </Button>
                        <Button variant="ghost" size="sm" className="text-red-600 hover:text-red-700">
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
  );

  return (
    <div className="p-6 bg-slate-50 min-h-screen">
      <div className="max-w-7xl mx-auto space-y-6">
        {/* Header */}
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-semibold text-slate-900">Product Master</h1>
            <p className="text-sm text-slate-600">ထုတ်ကုန်စာရင်း အဓိကစီမံခန့်ခွဲမှု</p>
            <div className="text-xs text-slate-500 mt-1">Under Planning Module • စီမံခန့်ခွဲမှုမော်ဂျူးအောက်တွင်</div>
          </div>
          <Button className="bg-blue-600 hover:bg-blue-700">
            <Plus className="h-4 w-4 mr-2" />
            Add New Product
          </Button>
        </div>

        {/* Tabs */}
        <Card>
          <CardContent className="pt-6">
            <Tabs value={activeTab} onValueChange={setActiveTab}>
              <TabsList className="grid w-full grid-cols-6">
                <TabsTrigger value="product-registration" className="flex items-center gap-2">
                  <Package className="h-4 w-4" />
                  Product
                </TabsTrigger>
                <TabsTrigger value="group-management" className="flex items-center gap-2">
                  <Tag className="h-4 w-4" />
                  Group
                </TabsTrigger>
                <TabsTrigger value="color-registration" className="flex items-center gap-2">
                  <Palette className="h-4 w-4" />
                  Color
                </TabsTrigger>
                <TabsTrigger value="product-color-mapping" className="flex items-center gap-2">
                  <Link className="h-4 w-4" />
                  Product-Color
                </TabsTrigger>
                <TabsTrigger value="parent-child-mapping" className="flex items-center gap-2">
                  <TreePine className="h-4 w-4" />
                  Parent-Child
                </TabsTrigger>
                <TabsTrigger value="product-mold-mapping" className="flex items-center gap-2">
                  <Settings className="h-4 w-4" />
                  Product-Mold
                </TabsTrigger>
              </TabsList>

              <div className="mt-6">
                <TabsContent value="product-registration" className="space-y-4">
                  {renderProductRegistration()}
                </TabsContent>

                <TabsContent value="group-management" className="space-y-4">
                  {renderGroupManagement()}
                </TabsContent>

                <TabsContent value="color-registration" className="space-y-4">
                  {renderColorRegistration()}
                </TabsContent>

                <TabsContent value="product-color-mapping" className="space-y-4">
                  {renderProductColorMapping()}
                </TabsContent>

                <TabsContent value="parent-child-mapping" className="space-y-4">
                  {renderParentChildMapping()}
                </TabsContent>

                <TabsContent value="product-mold-mapping" className="space-y-4">
                  {renderProductMoldMapping()}
                </TabsContent>
              </div>
            </Tabs>
          </CardContent>
        </Card>

        {/* Add/Edit Group Dialog */}
        <Dialog open={showAddGroupDialog} onOpenChange={setShowAddGroupDialog}>
          <DialogContent className="max-w-md">
            <DialogHeader>
              <DialogTitle>
                {editingGroup ? 'Edit Group | အုပ်စုတည်းဖြတ်ရန်' : 'Add New Group | အုပ်စုအသစ်ထည့်ရန်'}
              </DialogTitle>
            </DialogHeader>
            <div className="space-y-4">
              <div>
                <Label>Group Name | အုပ်စုအမည်</Label>
                <Input
                  placeholder="Enter group name..."
                  value={groupForm.name}
                  onChange={(e) => setGroupForm(prev => ({ ...prev, name: e.target.value }))}
                />
              </div>

              <div>
                <Label>Color | အရောင်</Label>
                <div className="flex gap-2">
                  <Input
                    type="color"
                    value={groupForm.color}
                    onChange={(e) => setGroupForm(prev => ({ ...prev, color: e.target.value }))}
                    className="w-16 h-10 p-1 border rounded"
                  />
                  <Input
                    value={groupForm.color}
                    onChange={(e) => setGroupForm(prev => ({ ...prev, color: e.target.value }))}
                    placeholder="#3B82F6"
                    className="flex-1"
                  />
                </div>
              </div>

              <div>
                <Label>Description | ဖော်ပြချက်</Label>
                <Input
                  placeholder="Enter description..."
                  value={groupForm.description}
                  onChange={(e) => setGroupForm(prev => ({ ...prev, description: e.target.value }))}
                />
              </div>

              <div className="flex gap-2 pt-4">
                <Button 
                  onClick={editingGroup ? handleUpdateGroup : handleAddGroup}
                  className="flex-1"
                >
                  {editingGroup ? 'Update Group' : 'Add Group'}
                </Button>
                <Button 
                  variant="outline" 
                  onClick={() => {
                    setShowAddGroupDialog(false);
                    setEditingGroup(null);
                    setGroupForm({ name: '', color: '#3B82F6', description: '' });
                  }}
                >
                  Cancel
                </Button>
              </div>
            </div>
          </DialogContent>
        </Dialog>
      </div>
    </div>
  );
}