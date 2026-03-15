import React, { useState } from 'react';
import { 
  Plus, 
  Edit, 
  Trash2, 
  Save, 
  X, 
  Search,
  Palette,
  ArrowLeft,
  Pipette
} from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Switch } from './ui/switch';

// Predefined color options for quick selection
const presetColors = [
  { name: 'Clear/Transparent', hex: '#FFFFFF', rgba: 'rgba(255,255,255,0.8)' },
  { name: 'White', hex: '#FFFFFF', rgba: 'rgba(255,255,255,1)' },
  { name: 'Black', hex: '#000000', rgba: 'rgba(0,0,0,1)' },
  { name: 'Red', hex: '#EF4444', rgba: 'rgba(239,68,68,1)' },
  { name: 'Blue', hex: '#3B82F6', rgba: 'rgba(59,130,246,1)' },
  { name: 'Green', hex: '#22C55E', rgba: 'rgba(34,197,94,1)' },
  { name: 'Yellow', hex: '#EAB308', rgba: 'rgba(234,179,8,1)' },
  { name: 'Purple', hex: '#A855F7', rgba: 'rgba(168,85,247,1)' },
  { name: 'Orange', hex: '#F97316', rgba: 'rgba(249,115,22,1)' },
  { name: 'Gray', hex: '#6B7280', rgba: 'rgba(107,114,128,1)' }
];

// Initial colors data
const initialColors = [
  {
    id: 1,
    code: 'CLR-001',
    nameEN: 'Clear/Transparent',
    nameMM: 'ရှင်းလင်း/ပွင့်လင်း',
    hex: '#FFFFFF',
    rgba: 'rgba(255,255,255,0.8)',
    status: 'active'
  },
  {
    id: 2,
    code: 'CLR-002',
    nameEN: 'White',
    nameMM: 'အဖြူ',
    hex: '#FFFFFF',
    rgba: 'rgba(255,255,255,1)',
    status: 'active'
  },
  {
    id: 3,
    code: 'CLR-003',
    nameEN: 'Blue',
    nameMM: 'အပြာ',
    hex: '#3B82F6',
    rgba: 'rgba(59,130,246,1)',
    status: 'active'
  },
  {
    id: 4,
    code: 'CLR-004',
    nameEN: 'Red',
    nameMM: 'အနီ',
    hex: '#EF4444',
    rgba: 'rgba(239,68,68,1)',
    status: 'active'
  },
  {
    id: 5,
    code: 'CLR-005',
    nameEN: 'Green',
    nameMM: 'အစိမ်း',
    hex: '#22C55E',
    rgba: 'rgba(34,197,94,1)',
    status: 'active'
  },
  {
    id: 6,
    code: 'CLR-006',
    nameEN: 'Black',
    nameMM: 'အနက်',
    hex: '#000000',
    rgba: 'rgba(0,0,0,1)',
    status: 'inactive'
  }
];

export function ColorTab() {
  const [view, setView] = useState<'list' | 'form' | 'edit'>('list');
  const [colors, setColors] = useState(initialColors);
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState('all');
  const [editingColor, setEditingColor] = useState<any>(null);
  
  const [formData, setFormData] = useState({
    code: '',
    nameEN: '',
    nameMM: '',
    hex: '#3B82F6',
    rgba: 'rgba(59,130,246,1)',
    status: true
  });

  const handleAddNew = () => {
    const newCode = `CLR-${String(Math.max(...colors.map(c => c.id)) + 1).padStart(3, '0')}`;
    setFormData({
      code: newCode,
      nameEN: '',
      nameMM: '',
      hex: '#3B82F6',
      rgba: 'rgba(59,130,246,1)',
      status: true
    });
    setEditingColor(null);
    setView('form');
  };

  const handleEdit = (color: any) => {
    setFormData({
      code: color.code,
      nameEN: color.nameEN,
      nameMM: color.nameMM,
      hex: color.hex,
      rgba: color.rgba,
      status: color.status === 'active'
    });
    setEditingColor(color);
    setView('edit');
  };

  const handleSave = () => {
    const colorData = {
      code: formData.code,
      nameEN: formData.nameEN,
      nameMM: formData.nameMM,
      hex: formData.hex,
      rgba: formData.rgba,
      status: formData.status ? 'active' : 'inactive'
    };
    
    if (view === 'edit' && editingColor) {
      // Update existing color
      setColors(prev => prev.map(c => 
        c.id === editingColor.id 
          ? { ...c, ...colorData }
          : c
      ));
    } else {
      // Add new color
      const newColor = {
        id: Math.max(...colors.map(c => c.id)) + 1,
        ...colorData
      };
      setColors(prev => [...prev, newColor]);
    }
    
    setView('list');
  };

  const handleDelete = (id: number) => {
    setColors(prev => prev.filter(c => c.id !== id));
  };

  const handleCancel = () => {
    setView('list');
    setFormData({
      code: '',
      nameEN: '',
      nameMM: '',
      hex: '#3B82F6',
      rgba: 'rgba(59,130,246,1)',
      status: true
    });
    setEditingColor(null);
  };

  const handleColorChange = (hex: string) => {
    // Convert hex to rgba (assuming full opacity)
    const r = parseInt(hex.slice(1, 3), 16);
    const g = parseInt(hex.slice(3, 5), 16);
    const b = parseInt(hex.slice(5, 7), 16);
    const rgba = `rgba(${r},${g},${b},1)`;
    
    setFormData(prev => ({ ...prev, hex, rgba }));
  };

  const handlePresetColorSelect = (preset: typeof presetColors[0]) => {
    setFormData(prev => ({ 
      ...prev, 
      hex: preset.hex, 
      rgba: preset.rgba,
      nameEN: prev.nameEN || preset.name 
    }));
  };

  const getStatusBadge = (status: string) => {
    return status === 'active' 
      ? <Badge className="bg-green-100 text-green-800 border-green-300">Active</Badge>
      : <Badge className="bg-slate-100 text-slate-800 border-slate-300">Inactive</Badge>;
  };

  const getColorPreview = (hex: string, rgba: string) => {
    return (
      <div className="flex items-center gap-2">
        <div 
          className="w-6 h-6 rounded border-2 border-slate-300" 
          style={{ backgroundColor: rgba }}
        />
        <span className="font-mono text-xs text-slate-600">{hex}</span>
      </div>
    );
  };

  const filteredColors = colors.filter(color => {
    const matchesSearch = searchTerm === '' || 
      color.code.toLowerCase().includes(searchTerm.toLowerCase()) ||
      color.nameEN.toLowerCase().includes(searchTerm.toLowerCase()) ||
      color.nameMM.toLowerCase().includes(searchTerm.toLowerCase());
    
    const matchesStatus = statusFilter === 'all' || color.status === statusFilter;
    
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
                {view === 'edit' ? 'Edit' : 'Add New'} Color
              </h3>
              <p className="text-sm text-slate-600">
                အရောင် {view === 'edit' ? 'ပြင်ဆင်ခြင်း' : 'ထည့်သွင်းခြင်း'}
              </p>
            </div>
          </div>
        </div>

        {/* Form */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Palette className="h-5 w-5 text-purple-600" />
              Color Information
              <span className="text-sm font-normal text-slate-600">အရောင်အချက်အလက်</span>
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-6">
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
              {/* Left Column */}
              <div className="space-y-4">
                <div>
                  <Label>Color Code</Label>
                  <Input 
                    value={formData.code}
                    onChange={(e) => setFormData(prev => ({ ...prev, code: e.target.value }))}
                    placeholder="CLR-001"
                  />
                </div>

                <div>
                  <Label>Color Name (English)</Label>
                  <Input 
                    value={formData.nameEN}
                    onChange={(e) => setFormData(prev => ({ ...prev, nameEN: e.target.value }))}
                    placeholder="Color name in English"
                  />
                </div>

                <div>
                  <Label>Color Name (Myanmar)</Label>
                  <Input 
                    value={formData.nameMM}
                    onChange={(e) => setFormData(prev => ({ ...prev, nameMM: e.target.value }))}
                    placeholder="မြန်မာဘာသာဖြင့် အရောင်အမည်"
                  />
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

              {/* Right Column */}
              <div className="space-y-4">
                <div>
                  <Label>Color Picker</Label>
                  <div className="flex items-center gap-3">
                    <input
                      type="color"
                      value={formData.hex}
                      onChange={(e) => handleColorChange(e.target.value)}
                      className="w-12 h-12 border border-slate-300 rounded-lg cursor-pointer"
                    />
                    <div className="flex-1">
                      <Input 
                        value={formData.hex}
                        onChange={(e) => handleColorChange(e.target.value)}
                        placeholder="#3B82F6"
                        className="font-mono"
                      />
                    </div>
                  </div>
                </div>

                <div>
                  <Label>RGBA Value</Label>
                  <Input 
                    value={formData.rgba}
                    onChange={(e) => setFormData(prev => ({ ...prev, rgba: e.target.value }))}
                    placeholder="rgba(59,130,246,1)"
                    className="font-mono"
                  />
                </div>

                {/* Color Preview */}
                <div>
                  <Label>Preview</Label>
                  <div className="p-4 border rounded-lg bg-slate-50">
                    <div className="flex items-center gap-3">
                      <div 
                        className="w-12 h-12 rounded-lg border-2 border-slate-300 shadow-sm" 
                        style={{ backgroundColor: formData.rgba }}
                      />
                      <div>
                        <div className="font-medium">{formData.nameEN || 'Color Name'}</div>
                        <div className="text-sm text-slate-600">{formData.nameMM || 'အရောင်အမည်'}</div>
                        <div className="font-mono text-xs text-slate-500">{formData.hex}</div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            {/* Preset Colors */}
            <div className="border-t pt-6">
              <Label className="text-sm font-medium text-slate-700 mb-3 block">Quick Color Selection</Label>
              <div className="grid grid-cols-5 lg:grid-cols-10 gap-2">
                {presetColors.map((preset, index) => (
                  <button
                    key={index}
                    type="button"
                    onClick={() => handlePresetColorSelect(preset)}
                    className="w-10 h-10 rounded-lg border-2 border-slate-300 hover:border-slate-400 focus:border-blue-500 focus:outline-none transition-colors"
                    style={{ backgroundColor: preset.rgba }}
                    title={preset.name}
                  />
                ))}
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
            disabled={!formData.code || !formData.nameEN}
            className="bg-purple-600 hover:bg-purple-700"
          >
            <Save className="h-4 w-4 mr-2" />
            {view === 'edit' ? 'Update' : 'Save'} Color
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
          <h3 className="text-lg font-semibold text-slate-900">Color Master</h3>
          <p className="text-sm text-slate-600">အရောင်စာရင်း</p>
        </div>
        <Button onClick={handleAddNew} className="bg-purple-600 hover:bg-purple-700">
          <Plus className="h-4 w-4 mr-2" />
          Add Color
        </Button>
      </div>

      {/* Search and Filters */}
      <Card>
        <CardContent className="p-4">
          <div className="flex flex-col sm:flex-row gap-4">
            <div className="relative flex-1">
              <Search className="absolute left-3 top-3 h-4 w-4 text-slate-400" />
              <Input
                placeholder="Search by color code, name..."
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

      {/* Colors Table */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Palette className="h-5 w-5 text-purple-600" />
            Colors
            <Badge variant="secondary" className="ml-2">
              {filteredColors.length} items
            </Badge>
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Color Code</TableHead>
                  <TableHead>Color Name (EN + MM)</TableHead>
                  <TableHead>Color Preview</TableHead>
                  <TableHead className="text-center">Status</TableHead>
                  <TableHead className="text-center">Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {filteredColors.map((color) => (
                  <TableRow key={color.id}>
                    <TableCell>
                      <div className="font-mono font-medium">{color.code}</div>
                    </TableCell>
                    <TableCell>
                      <div>
                        <div className="font-medium">{color.nameEN}</div>
                        <div className="text-sm text-slate-500">{color.nameMM}</div>
                      </div>
                    </TableCell>
                    <TableCell>
                      {getColorPreview(color.hex, color.rgba)}
                    </TableCell>
                    <TableCell className="text-center">
                      {getStatusBadge(color.status)}
                    </TableCell>
                    <TableCell className="text-center">
                      <div className="flex items-center justify-center gap-2">
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={() => handleEdit(color)}
                        >
                          <Edit className="h-4 w-4" />
                        </Button>
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={() => handleDelete(color.id)}
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
          
          {filteredColors.length === 0 && (
            <div className="text-center py-12">
              <Palette className="h-12 w-12 text-slate-300 mx-auto mb-4" />
              <h3 className="text-lg font-medium text-slate-900 mb-2">No colors found</h3>
              <p className="text-slate-600 mb-4">
                {searchTerm ? 'Try adjusting your search criteria.' : 'Get started by adding your first color.'}
              </p>
              {!searchTerm && (
                <Button onClick={handleAddNew}>
                  <Plus className="h-4 w-4 mr-2" />
                  Add Color
                </Button>
              )}
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}