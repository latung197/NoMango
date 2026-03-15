import React, { useState, useEffect } from 'react';
import {
  Package,
  Plus,
  Search,
  Edit,
  Trash2,
  Save,
  X,
  Palette,
  Building,
  RefreshCw,
  AlertCircle,
  Info,
  Eye,
  EyeOff,
  MapPin
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
import { Switch } from './ui/switch';
import { Alert, AlertDescription } from './ui/alert';
import { toast } from 'sonner@2.0.3';

// Mock data for raw materials
const mockRawMaterials = [
  {
    id: '1',
    stockNo: 100001,
    materialCode: 'RM-PEL-001',
    materialName: 'PET Resin Clear',
    materialType: 'Pellet',
    color: '#FFFFFF',
    colorName: 'Clear',
    defaultUnit: 'kg',
    unitConversion: '1 bag = 25 kg',
    binLocation: 'S1-A01',
    vendor: 'Indorama Ventures',
    warehouse: 'RM-WH',
    expiry: '2025-12-31',
    notes: 'High quality PET for bottles',
    isActive: true,
    createdAt: '2024-01-15'
  },
  {
    id: '2',
    stockNo: 100002,
    materialCode: 'RM-COM-002',
    materialName: 'PP Blue Compound',
    materialType: 'Compound',
    color: '#0066CC',
    colorName: 'Blue',
    defaultUnit: 'kg',
    unitConversion: '1 bag = 25 kg',
    binLocation: 'S1-B07',
    vendor: 'SCG Chemicals',
    warehouse: 'RM-WH',
    expiry: '2026-06-15',
    notes: 'Food grade polypropylene',
    isActive: true,
    createdAt: '2024-01-20'
  },
  {
    id: '3',
    stockNo: 100003,
    materialCode: 'RM-OTH-003',
    materialName: 'Additive Blue',
    materialType: 'Other',
    color: '#0066CC',
    colorName: 'Blue',
    defaultUnit: 'kg',
    unitConversion: '1 bag = 25 kg',
    binLocation: 'S2-A15',
    vendor: 'Clariant',
    warehouse: 'RM-WH',
    expiry: '2025-09-30',
    notes: 'Color masterbatch for blue coloring',
    isActive: true,
    createdAt: '2024-02-01'
  },
  {
    id: '4',
    stockNo: 100004,
    materialCode: 'RM-GLU-004',
    materialName: 'Industrial Adhesive',
    materialType: 'Glue Bag',
    color: '#FFFFE0',
    colorName: 'Light Yellow',
    defaultUnit: 'bag',
    unitConversion: '1 bag = 20 kg',
    binLocation: 'S3-C02',
    vendor: 'Henkel',
    warehouse: 'EXT-WH',
    expiry: '2025-11-20',
    notes: 'Strong bonding adhesive for assembly',
    isActive: true,
    createdAt: '2024-02-10'
  },
  {
    id: '5',
    stockNo: 100005,
    materialCode: 'RM-CUT-005',
    materialName: 'Steel Cutting Insert',
    materialType: 'Cut Piece',
    color: '#C0C0C0',
    colorName: 'Silver',
    defaultUnit: 'pcs',
    unitConversion: '1 roll = 100 pcs',
    binLocation: 'S4-D10',
    vendor: 'Sandvik',
    warehouse: 'RM-WH',
    expiry: null,
    notes: 'Precision cutting tools',
    isActive: false,
    createdAt: '2024-01-05'
  }
];

// Default color options for materials (now dynamically managed)
const defaultColorOptions = [
  { value: 'NO_COLOR', name: 'No color', hex: '#F3F4F6' },
  { value: '#FFFFFF', name: 'Clear/White', hex: '#FFFFFF' },
  { value: '#000000', name: 'Black', hex: '#000000' },
  { value: '#0066CC', name: 'Blue', hex: '#0066CC' },
  { value: '#FF0000', name: 'Red', hex: '#FF0000' },
  { value: '#00AA00', name: 'Green', hex: '#00AA00' },
  { value: '#FFFF00', name: 'Yellow', hex: '#FFFF00' },
  { value: '#FFA500', name: 'Orange', hex: '#FFA500' },
  { value: '#800080', name: 'Purple', hex: '#800080' },
  { value: '#FFC0CB', name: 'Pink', hex: '#FFC0CB' },
  { value: '#A52A2A', name: 'Brown', hex: '#A52A2A' },
  { value: '#808080', name: 'Gray', hex: '#808080' },
  { value: '#C0C0C0', name: 'Silver', hex: '#C0C0C0' },
  { value: '#FFD700', name: 'Gold', hex: '#FFD700' },
  { value: '#FFFFE0', name: 'Light Yellow', hex: '#FFFFE0' },
  { value: '#E6E6FA', name: 'Lavender', hex: '#E6E6FA' },
  { value: '#98FB98', name: 'Pale Green', hex: '#98FB98' }
];

// Vendor options for searchable dropdown
const vendorOptions = [
  'Indorama Ventures',
  'SCG Chemicals',
  'Clariant',
  'Henkel',
  'Sandvik',
  'BASF',
  'DuPont',
  'ExxonMobil',
  'LyondellBasell',
  'SABIC',
  'Dow Chemical',
  'Chevron Phillips'
];

// Bin / Location Code options
const binLocationOptions = [
  'S1-A01', 'S1-A02', 'S1-A03', 'S1-A04', 'S1-A05',
  'S1-B01', 'S1-B02', 'S1-B03', 'S1-B04', 'S1-B05', 'S1-B06', 'S1-B07', 'S1-B08', 'S1-B09', 'S1-B10',
  'S2-A01', 'S2-A02', 'S2-A03', 'S2-A04', 'S2-A05', 'S2-A06', 'S2-A07', 'S2-A08', 'S2-A09', 'S2-A10',
  'S2-A11', 'S2-A12', 'S2-A13', 'S2-A14', 'S2-A15',
  'S3-C01', 'S3-C02', 'S3-C03', 'S3-C04', 'S3-C05',
  'S4-D01', 'S4-D02', 'S4-D03', 'S4-D04', 'S4-D05', 'S4-D06', 'S4-D07', 'S4-D08', 'S4-D09', 'S4-D10'
];

// Material type options (now dynamically managed)
const defaultMaterialTypes = [
  'Pellet',
  'Glue Bag', 
  'Cut Piece',
  'Compound',
  'Other'
];

// Unit conversion helpers
const getUnitConversion = (unit: string) => {
  const conversions: Record<string, string> = {
    'kg': '1 bag = 25 kg',
    'bag': '1 bag = 25 kg', 
    'pcs': '1 roll = 100 pcs',
    'roll': '1 roll = 100 m'
  };
  return conversions[unit] || '1:1 conversion';
};

interface RawMaterialRegistrationProps {
  currentPage: string;
  onPageChange: (page: string) => void;
}

export function RawMaterialRegistration({ currentPage, onPageChange }: RawMaterialRegistrationProps) {
  const [materials, setMaterials] = useState(mockRawMaterials);
  const [searchTerm, setSearchTerm] = useState('');
  const [showInactive, setShowInactive] = useState(false);
  const [isAddDialogOpen, setIsAddDialogOpen] = useState(false);
  const [editingMaterial, setEditingMaterial] = useState<any>(null);
  const [newlyAddedId, setNewlyAddedId] = useState<string | null>(null);
  
  // Material types management
  const [materialTypes, setMaterialTypes] = useState(defaultMaterialTypes);
  const [isAddTypeDialogOpen, setIsAddTypeDialogOpen] = useState(false);
  const [newMaterialType, setNewMaterialType] = useState('');

  // Color management
  const [colorOptions, setColorOptions] = useState(defaultColorOptions);
  const [isAddColorDialogOpen, setIsAddColorDialogOpen] = useState(false);
  const [newColorName, setNewColorName] = useState('');
  const [newColorHex, setNewColorHex] = useState('#000000');
  
  // Form state
  const [formData, setFormData] = useState({
    stockNo: '',
    materialCode: '',
    materialName: '',
    materialType: '',
    color: 'NO_COLOR',
    colorName: 'No color',
    defaultUnit: 'kg',
    unitConversion: '1 bag = 25 kg',
    binLocation: '',
    vendor: 'none',
    warehouse: 'RM-WH',
    expiry: '',
    notes: ''
  });
  
  // Form validation and control state
  const [autoGenerate, setAutoGenerate] = useState(true);
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [isValidating, setIsValidating] = useState(false);
  const [formError, setFormError] = useState('');

  // Clear highlight after 3 seconds
  useEffect(() => {
    if (newlyAddedId) {
      const timer = setTimeout(() => {
        setNewlyAddedId(null);
      }, 3000);
      return () => clearTimeout(timer);
    }
  }, [newlyAddedId]);

  // Auto-generate stock number
  const generateStockNumber = () => {
    const existingStockNos = materials.map(m => m.stockNo).filter(Boolean);
    const maxStockNo = Math.max(...existingStockNos, 100000);
    return maxStockNo + 1;
  };

  // Auto-generate material code based on type
  const generateMaterialCode = (type: string) => {
    if (!type) return '';
    
    const typeMap: Record<string, string> = {
      'Pellet': 'PEL',
      'Glue Bag': 'GLU', 
      'Cut Piece': 'CUT',
      'Compound': 'COM',
      'Other': 'OTH'
    };
    
    // Generate 3-letter code for new types
    const getTypePrefix = (typeName: string) => {
      if (typeMap[typeName]) return typeMap[typeName];
      
      // Generate prefix from first 3 characters (uppercase)
      const cleaned = typeName.replace(/[^A-Za-z]/g, '').toUpperCase();
      return cleaned.length >= 3 ? cleaned.substring(0, 3) : cleaned.padEnd(3, 'X');
    };
    
    const prefix = getTypePrefix(type);
    const existing = materials.filter(m => m.materialCode.includes(`RM-${prefix}-`));
    const nextNumber = String(existing.length + 1).padStart(3, '0');
    
    return `RM-${prefix}-${nextNumber}`;
  };

  // Update material code and unit conversion when type/unit changes and auto-generate is on
  useEffect(() => {
    if (autoGenerate && formData.materialType) {
      const newCode = generateMaterialCode(formData.materialType);
      setFormData(prev => ({ ...prev, materialCode: newCode }));
    }
  }, [formData.materialType, autoGenerate, materials]);

  // Update unit conversion when unit changes
  useEffect(() => {
    const conversion = getUnitConversion(formData.defaultUnit);
    setFormData(prev => ({ ...prev, unitConversion: conversion }));
  }, [formData.defaultUnit]);

  // Generate stock number when dialog opens for new material
  useEffect(() => {
    if (isAddDialogOpen && !editingMaterial) {
      const newStockNo = generateStockNumber();
      setFormData(prev => ({ ...prev, stockNo: newStockNo.toString() }));
    }
  }, [isAddDialogOpen, editingMaterial, materials]);

  // Validation functions
  const validateField = (name: string, value: string) => {
    const newErrors = { ...errors };
    
    switch (name) {
      case 'stockNo':
        if (!value.trim()) {
          newErrors.stockNo = 'Stock number is required';
        } else if (!/^\d+$/.test(value)) {
          newErrors.stockNo = 'Stock number must be numeric';
        } else if (materials.some(m => 
          m.stockNo.toString() === value && (!editingMaterial || m.id !== editingMaterial.id)
        )) {
          newErrors.stockNo = 'Stock number already exists';
        } else {
          delete newErrors.stockNo;
        }
        break;
      case 'materialCode':
        if (!autoGenerate && !value.trim()) {
          newErrors.materialCode = 'Material code is required when not auto-generated';
        } else if (!autoGenerate && materials.some(m => 
          m.materialCode === value && (!editingMaterial || m.id !== editingMaterial.id)
        )) {
          newErrors.materialCode = 'Material code already exists';
        } else {
          delete newErrors.materialCode;
        }
        break;
      case 'materialName':
        if (!value.trim()) {
          newErrors.materialName = 'Material name is required';
        } else {
          delete newErrors.materialName;
        }
        break;
      case 'materialType':
        if (!value) {
          newErrors.materialType = 'Material type is required';
        } else {
          delete newErrors.materialType;
        }
        break;
      case 'binLocation':
        if (!value) {
          newErrors.binLocation = 'Bin/Location code is required';
        } else {
          delete newErrors.binLocation;
        }
        break;
      case 'expiry':
        if (value && new Date(value) < new Date()) {
          newErrors.expiry = 'Expiry date cannot be in the past';
        } else {
          delete newErrors.expiry;
        }
        break;
      case 'notes':
        if (value.length > 200) {
          newErrors.notes = 'Notes cannot exceed 200 characters';
        } else {
          delete newErrors.notes;
        }
        break;
    }
    
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  // Check if form is valid
  const isFormValid = () => {
    const requiredFields = ['stockNo', 'materialName', 'materialType', 'binLocation'];
    if (!autoGenerate) requiredFields.push('materialCode');
    
    return requiredFields.every(field => formData[field as keyof typeof formData]?.toString().trim()) &&
           Object.keys(errors).length === 0;
  };

  // Filter materials based on search term and active status
  const filteredMaterials = materials
    .filter(material => showInactive || material.isActive)
    .filter(material =>
      material.materialCode.toLowerCase().includes(searchTerm.toLowerCase()) ||
      material.materialName.toLowerCase().includes(searchTerm.toLowerCase()) ||
      material.materialType.toLowerCase().includes(searchTerm.toLowerCase()) ||
      (material.vendor && material.vendor.toLowerCase().includes(searchTerm.toLowerCase()))
    );

  // Handle form submission
  const handleSubmit = (saveAndAddAnother = false) => {
    setIsValidating(true);
    setFormError('');
    
    // Final validation
    const isValid = validateField('stockNo', formData.stockNo) &&
                   validateField('materialName', formData.materialName) &&
                   validateField('materialType', formData.materialType) &&
                   validateField('binLocation', formData.binLocation) &&
                   (!autoGenerate ? validateField('materialCode', formData.materialCode) : true) &&
                   validateField('expiry', formData.expiry) &&
                   validateField('notes', formData.notes);
    
    if (!isValid) {
      setFormError('Please fix the errors above before saving');
      setIsValidating(false);
      return;
    }

    // Final duplicate check for material code
    if (materials.some(m => 
      m.materialCode === formData.materialCode && 
      (!editingMaterial || m.id !== editingMaterial.id)
    )) {
      setErrors(prev => ({ ...prev, materialCode: 'Material code already exists' }));
      setFormError('Material code already exists');
      setIsValidating(false);
      return;
    }

    try {
      const selectedColor = colorOptions.find(c => c.value === formData.color);
      const materialData = {
        ...formData,
        stockNo: parseInt(formData.stockNo),
        colorName: selectedColor?.name || 'Custom',
        vendor: formData.vendor === 'none' ? '' : formData.vendor,
        isActive: true,
        createdAt: new Date().toISOString().split('T')[0]
      };

      if (editingMaterial) {
        // Update existing material
        setMaterials(materials.map(m => 
          m.id === editingMaterial.id 
            ? { ...m, ...materialData }
            : m
        ));
        toast.success('✅ Material Updated Successfully!');
        handleDialogClose();
      } else {
        // Add new material
        const newId = Date.now().toString();
        const newMaterial = {
          ...materialData,
          id: newId
        };
        setMaterials([newMaterial, ...materials]); // Add to top
        setNewlyAddedId(newId); // Highlight new row
        
        const colorDisplay = selectedColor?.name !== 'No color' ? ` • ${selectedColor?.name}` : '';
        toast.success(`✅ Material registered (${materialData.materialCode} • ${materialData.materialName}${colorDisplay})`);
        
        if (saveAndAddAnother) {
          resetForm();
        } else {
          handleDialogClose();
        }
      }
    } catch (error) {
      setFormError('An error occurred while saving. Please try again.');
    }
    
    setIsValidating(false);
  };

  // Reset form
  const resetForm = () => {
    const newStockNo = generateStockNumber();
    setFormData({
      stockNo: newStockNo.toString(),
      materialCode: '',
      materialName: '',
      materialType: '',
      color: 'NO_COLOR',
      colorName: 'No color',
      defaultUnit: 'kg',
      unitConversion: '1 bag = 25 kg',
      binLocation: '',
      vendor: 'none',
      warehouse: 'RM-WH',
      expiry: '',
      notes: ''
    });
    setErrors({});
    setFormError('');
    setAutoGenerate(true);
  };

  // Handle dialog open/close
  const handleDialogOpenChange = (open: boolean) => {
    setIsAddDialogOpen(open);
    if (!open) {
      handleDialogClose();
    }
  };

  // Handle dialog close
  const handleDialogClose = () => {
    setIsAddDialogOpen(false);
    setEditingMaterial(null);
    resetForm();
  };

  // Handle edit
  const handleEdit = (material: any) => {
    setEditingMaterial(material);
    setAutoGenerate(false); // Always disable auto-generate when editing
    setFormData({
      stockNo: material.stockNo?.toString() || '',
      materialCode: material.materialCode,
      materialName: material.materialName,
      materialType: material.materialType,
      color: material.color,
      colorName: material.colorName,
      defaultUnit: material.defaultUnit,
      unitConversion: material.unitConversion || getUnitConversion(material.defaultUnit),
      binLocation: material.binLocation || '',
      vendor: material.vendor || 'none',
      warehouse: material.warehouse,
      expiry: material.expiry || '',
      notes: material.notes || ''
    });
    setErrors({});
    setFormError('');
    setIsAddDialogOpen(true);
  };

  // Handle deactivate (soft delete)
  const handleDeactivate = (materialId: string) => {
    const material = materials.find(m => m.id === materialId);
    if (!material) return;
    
    if (window.confirm(`Are you sure you want to deactivate "${material.materialName}"? This will hide it from the default list.`)) {
      setMaterials(materials.map(m => 
        m.id === materialId 
          ? { ...m, isActive: false }
          : m
      ));
      toast.success('Material deactivated successfully');
    }
  };

  // Handle input changes with validation
  const handleInputChange = (name: string, value: string) => {
    setFormData(prev => ({ ...prev, [name]: value }));
    validateField(name, value);
  };

  // Handle color selection
  const handleColorChange = (value: string) => {
    const selectedColor = colorOptions.find(c => c.value === value);
    setFormData(prev => ({
      ...prev,
      color: value,
      colorName: selectedColor?.name || 'Custom'
    }));
  };

  // Handle keyboard shortcuts
  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Escape') {
      e.preventDefault();
      handleDialogClose();
    }
  };

  // Handle adding new material type
  const handleAddMaterialType = () => {
    if (!newMaterialType.trim()) {
      toast.error('Please enter a material type name');
      return;
    }
    
    if (materialTypes.some(type => type.toLowerCase() === newMaterialType.trim().toLowerCase())) {
      toast.error('This material type already exists');
      return;
    }
    
    const trimmedType = newMaterialType.trim();
    setMaterialTypes(prev => [...prev, trimmedType]);
    setFormData(prev => ({ ...prev, materialType: trimmedType }));
    setNewMaterialType('');
    setIsAddTypeDialogOpen(false);
    toast.success(`✅ Material type "${trimmedType}" added successfully`);
  };

  // Handle material type dialog close
  const handleTypeDialogClose = () => {
    setIsAddTypeDialogOpen(false);
    setNewMaterialType('');
  };

  // Handle adding new color
  const handleAddColor = () => {
    if (!newColorName.trim()) {
      toast.error('Please enter a color name');
      return;
    }
    
    if (colorOptions.some(color => color.name.toLowerCase() === newColorName.trim().toLowerCase())) {
      toast.error('This color name already exists');
      return;
    }
    
    if (colorOptions.some(color => color.hex.toLowerCase() === newColorHex.toLowerCase())) {
      toast.error('This color value already exists');
      return;
    }
    
    const trimmedName = newColorName.trim();
    const newColor = {
      value: newColorHex,
      name: trimmedName,
      hex: newColorHex
    };
    
    setColorOptions(prev => [...prev, newColor]);
    setFormData(prev => ({ 
      ...prev, 
      color: newColorHex,
      colorName: trimmedName
    }));
    setNewColorName('');
    setNewColorHex('#000000');
    setIsAddColorDialogOpen(false);
    toast.success(`✅ Color "${trimmedName}" added successfully`);
  };

  // Handle color dialog close
  const handleColorDialogClose = () => {
    setIsAddColorDialogOpen(false);
    setNewColorName('');
    setNewColorHex('#000000');
  };

  return (
    <div className="space-y-6">
      {/* Page Header */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Package className="h-5 w-5 text-green-600" />
            Raw Material Registration
            <span className="text-sm font-normal text-slate-600">ကုန်ကြမ်းအချက်အလက်</span>
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex gap-3 mb-6">
            <div className="flex items-center gap-2 bg-green-50 px-3 py-2 rounded-lg">
              <Package className="h-4 w-4 text-green-600" />
              <span className="text-sm">📦 For warehouse staff to register raw materials into the system</span>
            </div>
          </div>

          {/* Action Bar */}
          <div className="flex gap-4 items-center justify-between mb-6">
            <div className="flex items-center gap-4 flex-1">
              <div className="relative flex-1">
                <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-slate-400 h-4 w-4" />
                <Input
                  placeholder="Search materials by code, name, type, or vendor..."
                  className="pl-10"
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                />
              </div>
              <div className="flex items-center gap-2">
                <Switch
                  id="show-inactive"
                  checked={showInactive}
                  onCheckedChange={setShowInactive}
                />
                <Label htmlFor="show-inactive" className="text-sm whitespace-nowrap">
                  Include inactive
                </Label>
              </div>
            </div>
            <Dialog open={isAddDialogOpen} onOpenChange={handleDialogOpenChange}>
              <DialogTrigger asChild>
                <Button className="bg-green-600 hover:bg-green-700">
                  <Plus className="h-4 w-4 mr-2" />
                  ➕ Add New Raw Material
                </Button>
              </DialogTrigger>
              <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto" onKeyDown={handleKeyDown}>
                <DialogHeader>
                  <DialogTitle className="flex items-center gap-2">
                    <Package className="h-5 w-5 text-green-600" />
                    {editingMaterial ? 'Edit Raw Material' : 'Create Raw Material'}
                  </DialogTitle>
                  <DialogDescription>
                    Enter raw material details. Fields marked with * are required.
                  </DialogDescription>
                </DialogHeader>
                
                {/* Error Banner */}
                {formError && (
                  <Alert className="border-red-200 bg-red-50">
                    <AlertCircle className="h-4 w-4 text-red-600" />
                    <AlertDescription className="text-red-800">
                      {formError}
                    </AlertDescription>
                  </Alert>
                )}

                <div className="space-y-6">
                  {/* Stock Number & Material Code Section */}
                  <div className="space-y-4">
                    <div className="flex items-center gap-4">
                      <div className="flex items-center space-x-2">
                        <Switch
                          id="auto-generate"
                          checked={autoGenerate}
                          onCheckedChange={setAutoGenerate}
                        />
                        <Label htmlFor="auto-generate" className="text-sm font-medium">
                          Auto-generate Material Code
                        </Label>
                      </div>
                      {autoGenerate && (
                        <div className="text-xs text-slate-600 bg-slate-50 px-2 py-1 rounded">
                          Pattern: RM-{'{TYPE}'}-{'{000}'}
                        </div>
                      )}
                    </div>
                    
                    <div className="grid grid-cols-2 gap-4">
                      <div>
                        <Label className="text-sm font-medium">Stock No. *</Label>
                        <Input
                          type="number"
                          placeholder="e.g., 100001"
                          value={formData.stockNo}
                          onChange={(e) => handleInputChange('stockNo', e.target.value)}
                          className={`mt-1 ${errors.stockNo ? 'border-red-500' : ''}`}
                          disabled={editingMaterial} // Stock number should not be changeable after creation
                        />
                        {errors.stockNo && (
                          <div className="text-sm text-red-600 mt-1">{errors.stockNo}</div>
                        )}
                        <div className="text-xs text-slate-500 mt-1">
                          {editingMaterial ? 'Stock numbers cannot be changed after creation' : 'Unique numeric identifier'}
                        </div>
                      </div>
                      <div>
                        <Label className="text-sm font-medium">
                          Material Code {!autoGenerate && '*'}
                        </Label>
                        <Input
                          placeholder="e.g., RM-PEL-001"
                          value={formData.materialCode}
                          onChange={(e) => handleInputChange('materialCode', e.target.value.toUpperCase())}
                          disabled={autoGenerate}
                          className={`mt-1 ${errors.materialCode ? 'border-red-500' : ''}`}
                        />
                        {errors.materialCode && (
                          <div className="text-sm text-red-600 mt-1">{errors.materialCode}</div>
                        )}
                        {!autoGenerate && (
                          <div className="text-xs text-slate-500 mt-1">
                            Uniqueness will be checked when you finish typing
                          </div>
                        )}
                      </div>
                    </div>
                  </div>

                  {/* Material Name */}
                  <div>
                    <Label className="text-sm font-medium">Material Name *</Label>
                    <Input
                      placeholder="e.g., PET Resin Clear"
                      value={formData.materialName}
                      onChange={(e) => handleInputChange('materialName', e.target.value)}
                      className={`mt-1 ${errors.materialName ? 'border-red-500' : ''}`}
                    />
                    {errors.materialName && (
                      <div className="text-sm text-red-600 mt-1">{errors.materialName}</div>
                    )}
                  </div>

                  {/* Material Type and Unit */}
                  <div className="grid grid-cols-2 gap-4">
                    <div>
                      <Label className="text-sm font-medium">Material Type *</Label>
                      <Select value={formData.materialType} onValueChange={(value) => handleInputChange('materialType', value)}>
                        <SelectTrigger className={`mt-1 ${errors.materialType ? 'border-red-500' : ''}`}>
                          <SelectValue placeholder="Select material type" />
                        </SelectTrigger>
                        <SelectContent>
                          {materialTypes.map((type) => (
                            <SelectItem key={type} value={type}>{type}</SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                      {errors.materialType && (
                        <div className="text-sm text-red-600 mt-1">{errors.materialType}</div>
                      )}
                      <Dialog open={isAddTypeDialogOpen} onOpenChange={setIsAddTypeDialogOpen}>
                        <DialogTrigger asChild>
                          <Button 
                            variant="outline" 
                            size="sm" 
                            className="mt-2 text-green-600 border-green-600 hover:bg-green-50"
                          >
                            <Plus className="h-3 w-3 mr-1" />
                            Add
                          </Button>
                        </DialogTrigger>
                        <DialogContent className="max-w-md">
                          <DialogHeader>
                            <DialogTitle>Add Material Type</DialogTitle>
                            <DialogDescription>
                              Enter a new material type to add to the dropdown list.
                            </DialogDescription>
                          </DialogHeader>
                          <div className="space-y-4">
                            <div>
                              <Label className="text-sm font-medium">Material Type Name</Label>
                              <Input
                                placeholder="e.g., Film Roll"
                                value={newMaterialType}
                                onChange={(e) => setNewMaterialType(e.target.value)}
                                onKeyDown={(e) => {
                                  if (e.key === 'Enter') {
                                    e.preventDefault();
                                    handleAddMaterialType();
                                  } else if (e.key === 'Escape') {
                                    handleTypeDialogClose();
                                  }
                                }}
                                className="mt-1"
                                autoFocus
                              />
                            </div>
                            <div className="flex gap-2 justify-end">
                              <Button variant="outline" onClick={handleTypeDialogClose}>
                                Cancel
                              </Button>
                              <Button onClick={handleAddMaterialType} className="bg-green-600 hover:bg-green-700">
                                <Save className="h-4 w-4 mr-2" />
                                Add
                              </Button>
                            </div>
                          </div>
                        </DialogContent>
                      </Dialog>
                    </div>
                    <div>
                      <Label className="text-sm font-medium">Default Unit</Label>
                      <Select value={formData.defaultUnit} onValueChange={(value) => handleInputChange('defaultUnit', value)}>
                        <SelectTrigger className="mt-1">
                          <SelectValue />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="kg">kg (Kilogram)</SelectItem>
                          <SelectItem value="bag">bag</SelectItem>
                          <SelectItem value="pcs">pcs (Pieces)</SelectItem>
                          <SelectItem value="roll">roll</SelectItem>
                          <SelectItem value="liter">liter</SelectItem>
                          <SelectItem value="ton">ton</SelectItem>
                        </SelectContent>
                      </Select>
                      <div className="text-xs text-slate-500 mt-1">
                        Conversion: {formData.unitConversion}
                      </div>
                    </div>
                  </div>

                  {/* Color Selection */}
                  <div>
                    <Label className="text-sm font-medium">Color</Label>
                    <Select value={formData.color} onValueChange={handleColorChange}>
                      <SelectTrigger className="mt-1">
                        <div className="flex items-center gap-2">
                          <div 
                            className="w-4 h-4 rounded border border-slate-300"
                            style={{ backgroundColor: formData.color === 'NO_COLOR' ? '#F3F4F6' : formData.color }}
                          />
                          <SelectValue>
                            {formData.colorName}
                          </SelectValue>
                        </div>
                      </SelectTrigger>
                      <SelectContent>
                        {colorOptions.map((color) => (
                          <SelectItem key={color.value} value={color.value}>
                            <div className="flex items-center gap-2">
                              <div 
                                className="w-4 h-4 rounded border border-slate-300"
                                style={{ backgroundColor: color.hex }}
                              />
                              {color.name}
                            </div>
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                    <Dialog open={isAddColorDialogOpen} onOpenChange={setIsAddColorDialogOpen}>
                      <DialogTrigger asChild>
                        <Button 
                          variant="outline" 
                          size="sm" 
                          className="mt-2 text-green-600 border-green-600 hover:bg-green-50"
                        >
                          <Palette className="h-3 w-3 mr-1" />
                          Add
                        </Button>
                      </DialogTrigger>
                      <DialogContent className="max-w-md">
                        <DialogHeader>
                          <DialogTitle>Add Custom Color</DialogTitle>
                          <DialogDescription>
                            Create a new color option with detailed color selection.
                          </DialogDescription>
                        </DialogHeader>
                        <div className="space-y-4">
                          <div>
                            <Label className="text-sm font-medium">Color Name</Label>
                            <Input
                              placeholder="e.g., Sky Blue"
                              value={newColorName}
                              onChange={(e) => setNewColorName(e.target.value)}
                              onKeyDown={(e) => {
                                if (e.key === 'Enter') {
                                  e.preventDefault();
                                  handleAddColor();
                                } else if (e.key === 'Escape') {
                                  handleColorDialogClose();
                                }
                              }}
                              className="mt-1"
                              autoFocus
                            />
                          </div>
                          <div>
                            <Label className="text-sm font-medium">Color Value</Label>
                            <div className="flex items-center gap-3 mt-1">
                              <input
                                type="color"
                                value={newColorHex}
                                onChange={(e) => setNewColorHex(e.target.value)}
                                className="w-12 h-10 border border-slate-300 rounded cursor-pointer"
                              />
                              <Input
                                placeholder="#000000"
                                value={newColorHex}
                                onChange={(e) => setNewColorHex(e.target.value)}
                                className="flex-1 font-mono text-sm"
                              />
                            </div>
                          </div>
                          <div className="flex items-center gap-2 p-3 bg-slate-50 rounded-lg">
                            <div 
                              className="w-8 h-8 rounded border border-slate-300"
                              style={{ backgroundColor: newColorHex }}
                            />
                            <div className="text-sm">
                              <div className="font-medium">{newColorName || 'Color Preview'}</div>
                              <div className="text-slate-500 font-mono">{newColorHex}</div>
                            </div>
                          </div>
                          <div className="flex gap-2 justify-end">
                            <Button variant="outline" onClick={handleColorDialogClose}>
                              Cancel
                            </Button>
                            <Button onClick={handleAddColor} className="bg-green-600 hover:bg-green-700">
                              <Save className="h-4 w-4 mr-2" />
                              Add
                            </Button>
                          </div>
                        </div>
                      </DialogContent>
                    </Dialog>
                  </div>

                  {/* Vendor Selection */}
                  <div>
                    <Label className="text-sm font-medium">Vendor (Optional)</Label>
                    <Select value={formData.vendor} onValueChange={(value) => handleInputChange('vendor', value === 'none' ? '' : value)}>
                      <SelectTrigger className="mt-1">
                        <SelectValue placeholder="Select vendor" />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="none">No vendor selected</SelectItem>
                        {vendorOptions.map((vendor) => (
                          <SelectItem key={vendor} value={vendor}>{vendor}</SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>

                  {/* Location and Warehouse */}
                  <div className="grid grid-cols-2 gap-4">
                    <div>
                      <Label className="text-sm font-medium">Bin/Location Code *</Label>
                      <Select value={formData.binLocation} onValueChange={(value) => handleInputChange('binLocation', value)}>
                        <SelectTrigger className={`mt-1 ${errors.binLocation ? 'border-red-500' : ''}`}>
                          <SelectValue placeholder="Select location" />
                        </SelectTrigger>
                        <SelectContent>
                          {binLocationOptions.map((location) => (
                            <SelectItem key={location} value={location}>
                              <div className="flex items-center gap-2">
                                <MapPin className="h-3 w-3 text-slate-400" />
                                {location}
                              </div>
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                      {errors.binLocation && (
                        <div className="text-sm text-red-600 mt-1">{errors.binLocation}</div>
                      )}
                    </div>
                    <div>
                      <Label className="text-sm font-medium">Warehouse</Label>
                      <Select value={formData.warehouse} onValueChange={(value) => handleInputChange('warehouse', value)}>
                        <SelectTrigger className="mt-1">
                          <SelectValue />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="RM-WH">
                            <div className="flex items-center gap-2">
                              <Building className="h-3 w-3 text-slate-400" />
                              RM-WH (Raw Material Warehouse)
                            </div>
                          </SelectItem>
                          <SelectItem value="EXT-WH">
                            <div className="flex items-center gap-2">
                              <Building className="h-3 w-3 text-slate-400" />
                              EXT-WH (External Warehouse)
                            </div>
                          </SelectItem>
                        </SelectContent>
                      </Select>
                    </div>
                  </div>

                  {/* Expiry Date */}
                  <div>
                    <Label className="text-sm font-medium">Expiry Date (Optional)</Label>
                    <Input
                      type="date"
                      value={formData.expiry}
                      onChange={(e) => handleInputChange('expiry', e.target.value)}
                      className={`mt-1 ${errors.expiry ? 'border-red-500' : ''}`}
                    />
                    {errors.expiry && (
                      <div className="text-sm text-red-600 mt-1">{errors.expiry}</div>
                    )}
                  </div>

                  {/* Notes */}
                  <div>
                    <Label className="text-sm font-medium">Notes (Optional)</Label>
                    <Textarea
                      placeholder="Additional notes about this raw material..."
                      value={formData.notes}
                      onChange={(e) => handleInputChange('notes', e.target.value)}
                      className={`mt-1 ${errors.notes ? 'border-red-500' : ''}`}
                      rows={3}
                    />
                    {errors.notes && (
                      <div className="text-sm text-red-600 mt-1">{errors.notes}</div>
                    )}
                    <div className="text-xs text-slate-500 mt-1">
                      {formData.notes.length}/200 characters
                    </div>
                  </div>

                  {/* Action Buttons */}
                  <div className="flex gap-3 pt-4 border-t">
                    <Button
                      type="button"
                      variant="outline"
                      onClick={handleDialogClose}
                      className="flex-1"
                    >
                      <X className="h-4 w-4 mr-2" />
                      Cancel
                    </Button>
                    {!editingMaterial && (
                      <Button
                        type="button"
                        variant="outline"
                        onClick={() => handleSubmit(true)}
                        disabled={!isFormValid() || isValidating}
                        className="flex-1"
                      >
                        <Plus className="h-4 w-4 mr-2" />
                        {isValidating ? 'Saving...' : 'Save & Add Another'}
                      </Button>
                    )}
                    <Button
                      type="button"
                      onClick={() => handleSubmit(false)}
                      disabled={!isFormValid() || isValidating}
                      className="flex-1 bg-green-600 hover:bg-green-700"
                    >
                      <Save className="h-4 w-4 mr-2" />
                      {isValidating ? 'Saving...' : editingMaterial ? 'Update Material' : 'Save Material'}
                    </Button>
                  </div>
                </div>
              </DialogContent>
            </Dialog>
          </div>

          {/* Results Summary */}
          <div className="flex items-center justify-between mb-4">
            <div className="text-sm text-slate-600">
              Showing {filteredMaterials.length} of {materials.length} materials
              {searchTerm && ` matching "${searchTerm}"`}
              {!showInactive && materials.filter(m => !m.isActive).length > 0 && 
                ` (${materials.filter(m => !m.isActive).length} hidden inactive)`
              }
            </div>
            <div className="flex items-center gap-2">
              <RefreshCw className="h-4 w-4 text-slate-400" />
              <span className="text-xs text-slate-500">Auto-refresh every 30s</span>
            </div>
          </div>

          {/* Materials Table */}
          <Card>
            <CardContent className="p-0">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Material Code</TableHead>
                    <TableHead>Material Name</TableHead>
                    <TableHead>Type</TableHead>
                    <TableHead>Color</TableHead>
                    <TableHead>Unit</TableHead>
                    <TableHead>Location</TableHead>
                    <TableHead>Vendor</TableHead>
                    <TableHead>Status</TableHead>
                    <TableHead className="w-[120px]">Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {filteredMaterials.map((material) => (
                    <TableRow 
                      key={material.id}
                      className={`${material.id === newlyAddedId ? 'bg-green-50 animate-pulse' : ''} ${!material.isActive ? 'opacity-60' : ''}`}
                    >
                      <TableCell className="font-mono text-sm">
                        {material.materialCode}
                      </TableCell>
                      <TableCell className="font-medium">
                        {material.materialName}
                      </TableCell>
                      <TableCell>
                        <Badge variant="outline">{material.materialType}</Badge>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-2">
                          <div 
                            className="w-4 h-4 rounded border border-slate-300"
                            style={{ backgroundColor: material.color === 'NO_COLOR' ? '#F3F4F6' : material.color }}
                          />
                          <span className="text-sm">{material.colorName}</span>
                        </div>
                      </TableCell>
                      <TableCell>
                        <span className="text-sm">{material.defaultUnit}</span>
                        <div className="text-xs text-slate-500">{material.unitConversion}</div>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-1">
                          <MapPin className="h-3 w-3 text-slate-400" />
                          <span className="text-sm">{material.binLocation}</span>
                        </div>
                        <div className="text-xs text-slate-500">{material.warehouse}</div>
                      </TableCell>
                      <TableCell>
                        <span className="text-sm">{material.vendor || '-'}</span>
                      </TableCell>
                      <TableCell>
                        <Badge variant={material.isActive ? "default" : "secondary"}>
                          {material.isActive ? "Active" : "Inactive"}
                        </Badge>
                        {material.expiry && (
                          <div className="text-xs text-slate-500 mt-1">
                            Exp: {material.expiry}
                          </div>
                        )}
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-1">
                          <Button
                            variant="ghost"
                            size="sm"
                            onClick={() => handleEdit(material)}
                            className="h-8 w-8 p-0"
                          >
                            <Edit className="h-3 w-3" />
                          </Button>
                          {material.isActive && (
                            <Button
                              variant="ghost"
                              size="sm"
                              onClick={() => handleDeactivate(material.id)}
                              className="h-8 w-8 p-0 text-red-600 hover:text-red-700"
                            >
                              <Trash2 className="h-3 w-3" />
                            </Button>
                          )}
                        </div>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
              {filteredMaterials.length === 0 && (
                <div className="text-center py-8 text-slate-500">
                  <Package className="h-12 w-12 mx-auto mb-4 opacity-20" />
                  <div className="text-lg font-medium mb-1">No materials found</div>
                  <div className="text-sm">
                    {searchTerm 
                      ? `No materials match "${searchTerm}"`
                      : 'Click "Add New Raw Material" to get started'
                    }
                  </div>
                </div>
              )}
            </CardContent>
          </Card>
        </CardContent>
      </Card>
    </div>
  );
}