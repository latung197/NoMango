import React, { useState, useEffect } from 'react';
import { 
  ClipboardList, 
  Plus,
  Search,
  Filter,
  Download,
  Edit,
  Trash2,
  Eye,
  Save,
  RotateCcw,
  ChevronRight,
  ChevronDown,
  Package,
  Clock,
  User,
  Settings,
  FileText,
  CheckCircle,
  AlertCircle,
  Calendar
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
import { Textarea } from './ui/textarea';

// Mock data
const mockProducts = [
  {
    id: '2011',
    code: '2011',
    name: 'Plastic Bottle 500ml',
    nameLocal: 'ပလတ်စတစ်ပုလင်း ၅၀၀မီလီ',
    type: 'parent',
    children: [
      { id: '2011-01', code: '2011-01', name: 'Bottle Body', nameLocal: 'ပုလင်းကိုယ်ခန္ဓာ', defaultColor: 'WHITE', autoSelect: true },
      { id: '2011-02', code: '2011-02', name: 'Bottle Cap', nameLocal: 'ပုလင်းအဖုံး', defaultColor: 'BLUE', autoSelect: true },
      { id: '2011-03', code: '2011-03', name: 'Product Label', nameLocal: 'ထုတ်ကုန်တံဆိပ်', defaultColor: 'WHITE', autoSelect: false }
    ]
  },
  {
    id: '2012',
    code: '2012',
    name: 'Plastic Container 1L',
    nameLocal: 'ပလတ်စတစ်ပုံးကြီး ၁လီတာ',
    type: 'parent',
    children: [
      { id: '2012-01', code: '2012-01', name: 'Container Body', nameLocal: 'ပုံးကိုယ်ခန္ဓာ', defaultColor: 'WHITE', autoSelect: true },
      { id: '2012-02', code: '2012-02', name: 'Container Lid', nameLocal: 'ပုံးအဖုံး', defaultColor: 'RED', autoSelect: true }
    ]
  }
];

const mockMachines = [
  { id: 'M001', name: 'Injection Machine 1', nameLocal: 'ထိုးစက် ၁' },
  { id: 'M002', name: 'Injection Machine 2', nameLocal: 'ထိုးစက် ၂' },
  { id: 'M003', name: 'Blow Molding Machine', nameLocal: 'မှုတ်ထုတ်စက်' }
];

const mockMolds = [
  { id: 'MD001', name: 'Bottle Body Mold', nameLocal: 'ပုလင်းကိုယ်ခန္ဓာပုံစံ' },
  { id: 'MD002', name: 'Bottle Cap Mold', nameLocal: 'ပုလင်းအဖုံးပုံစံ' },
  { id: 'MD003', name: 'Label Mold', nameLocal: 'တံဆိပ်ပုံစံ' }
];

const mockPlans = [
  {
    id: 'PL20250903-001',
    parentProductCode: '2011',
    parentProductName: 'Plastic Bottle 500ml',
    quantity: 1000,
    plannerName: 'John Doe',
    status: 'draft',
    createdDate: '2025-09-03',
    jobs: [
      {
        jobId: 'PL20250903-001-J1',
        childCode: '2011-01',
        childName: 'Bottle Body',
        defaultColor: 'WHITE',
        plannedQty: 1000,
        machineId: 'M001',
        moldNo: 'MD001',
        duration: '8 hours',
        status: 'pending'
      },
      {
        jobId: 'PL20250903-001-J2',
        childCode: '2011-02',
        childName: 'Bottle Cap',
        defaultColor: 'BLUE',
        plannedQty: 1000,
        machineId: 'M002',
        moldNo: 'MD002',
        duration: '4 hours',
        status: 'pending'
      }
    ]
  },
  {
    id: 'PL20250903-002',
    parentProductCode: '2012',
    parentProductName: 'Plastic Container 1L',
    quantity: 500,
    plannerName: 'Jane Smith',
    status: 'final',
    createdDate: '2025-09-03',
    jobs: [
      {
        jobId: 'PL20250903-002-J1',
        childCode: '2012-01',
        childName: 'Container Body',
        defaultColor: 'WHITE',
        plannedQty: 500,
        machineId: 'M001',
        moldNo: 'MD001',
        duration: '10 hours',
        status: 'in-progress'
      },
      {
        jobId: 'PL20250903-002-J2',
        childCode: '2012-02',
        childName: 'Container Lid',
        defaultColor: 'RED',
        plannedQty: 500,
        machineId: 'M003',
        moldNo: 'MD003',
        duration: '3 hours',
        status: 'completed'
      }
    ]
  }
];

export function ProductionPlanning() {
  const [activeTab, setActiveTab] = useState('create-plan');
  const [expandedPlans, setExpandedPlans] = useState<string[]>([]);
  
  // Create Plan Form State
  const [planForm, setPlanForm] = useState({
    planId: '',
    parentProductCode: '',
    parentProductName: '',
    quantity: '',
    plannerName: 'Current User',
    status: 'draft',
    notes: ''
  });

  const [selectedChildren, setSelectedChildren] = useState<any[]>([]);
  const [autoExpandedChildren, setAutoExpandedChildren] = useState<any[]>([]);

  // Generate Plan ID
  const generatePlanId = () => {
    const today = new Date();
    const dateStr = today.toISOString().slice(0, 10).replace(/-/g, '');
    const serial = '001'; // In real app, this would be auto-incremented
    return `PL${dateStr}-${serial}`;
  };

  useEffect(() => {
    setPlanForm(prev => ({ ...prev, planId: generatePlanId() }));
  }, []);

  // Handle parent product selection
  const handleParentProductChange = (productId: string) => {
    const selectedProduct = mockProducts.find(p => p.id === productId);
    if (selectedProduct) {
      setPlanForm(prev => ({
        ...prev,
        parentProductCode: selectedProduct.code,
        parentProductName: selectedProduct.name
      }));

      // Auto-expand children with default selections
      const childrenWithDefaults = selectedProduct.children.map(child => ({
        ...child,
        selected: child.autoSelect,
        plannedQty: child.autoSelect ? planForm.quantity || 0 : 0,
        machineId: '',
        moldNo: '',
        duration: ''
      }));

      setAutoExpandedChildren(childrenWithDefaults);
      setSelectedChildren(childrenWithDefaults.filter(child => child.selected));
    }
  };

  // Handle child selection change
  const handleChildSelectionChange = (childId: string, selected: boolean) => {
    setAutoExpandedChildren(prev => 
      prev.map(child => 
        child.id === childId 
          ? { ...child, selected, plannedQty: selected ? parseInt(planForm.quantity) || 0 : 0 }
          : child
      )
    );

    if (selected) {
      const child = autoExpandedChildren.find(c => c.id === childId);
      if (child) {
        setSelectedChildren(prev => [...prev.filter(c => c.id !== childId), { ...child, selected: true }]);
      }
    } else {
      setSelectedChildren(prev => prev.filter(c => c.id !== childId));
    }
  };

  // Handle quantity change
  const handleQuantityChange = (quantity: string) => {
    setPlanForm(prev => ({ ...prev, quantity }));
    
    // Update all selected children quantities
    const numQty = parseInt(quantity) || 0;
    setAutoExpandedChildren(prev => 
      prev.map(child => ({
        ...child,
        plannedQty: child.selected ? numQty : 0
      }))
    );
    
    setSelectedChildren(prev => 
      prev.map(child => ({
        ...child,
        plannedQty: numQty
      }))
    );
  };

  // Toggle plan expansion in dashboard
  const togglePlanExpansion = (planId: string) => {
    setExpandedPlans(prev => 
      prev.includes(planId) 
        ? prev.filter(id => id !== planId)
        : [...prev, planId]
    );
  };

  const renderCreatePlan = () => (
    <div className="space-y-6">
      {/* Plan Creation Form */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <ClipboardList className="h-5 w-5 text-blue-600" />
            Create New Production Plan
            <span className="text-sm font-normal text-slate-600">ထုတ်လုပ်မှုအစီအစဉ်အသစ်ဖန်တီးခြင်း</span>
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-6">
          {/* Basic Plan Information */}
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <div>
              <Label htmlFor="planId">Plan ID (Auto-generated)</Label>
              <Input
                id="planId"
                value={planForm.planId}
                disabled
                className="bg-slate-50"
              />
            </div>
            
            <div>
              <Label htmlFor="parentProduct">Parent Product Code *</Label>
              <Select 
                value={planForm.parentProductCode} 
                onValueChange={handleParentProductChange}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Select parent product" />
                </SelectTrigger>
                <SelectContent>
                  {mockProducts.filter(p => p.type === 'parent').map((product) => (
                    <SelectItem key={product.id} value={product.id}>
                      <div className="flex items-center gap-2">
                        <Package className="h-4 w-4" />
                        {product.code} - {product.name}
                      </div>
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            
            <div>
              <Label htmlFor="parentProductName">Parent Product Name</Label>
              <Input
                id="parentProductName"
                value={planForm.parentProductName}
                disabled
                className="bg-slate-50"
              />
            </div>
            
            <div>
              <Label htmlFor="quantity">Quantity (Total Set Qty) *</Label>
              <Input
                id="quantity"
                type="number"
                placeholder="1000"
                value={planForm.quantity}
                onChange={(e) => handleQuantityChange(e.target.value)}
              />
            </div>
            
            <div>
              <Label htmlFor="plannerName">Planner Name</Label>
              <Input
                id="plannerName"
                value={planForm.plannerName}
                onChange={(e) => setPlanForm(prev => ({ ...prev, plannerName: e.target.value }))}
              />
            </div>
            
            <div>
              <Label htmlFor="status">Status</Label>
              <Select 
                value={planForm.status} 
                onValueChange={(value) => setPlanForm(prev => ({ ...prev, status: value }))}
              >
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="draft">Draft</SelectItem>
                  <SelectItem value="final">Final</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>

          <div>
            <Label htmlFor="notes">Notes</Label>
            <Textarea
              id="notes"
              placeholder="Additional notes for this production plan..."
              value={planForm.notes}
              onChange={(e) => setPlanForm(prev => ({ ...prev, notes: e.target.value }))}
            />
          </div>
        </CardContent>
      </Card>

      {/* Child Product Auto-Expansion */}
      {autoExpandedChildren.length > 0 && (
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Package className="h-5 w-5 text-green-600" />
              Child Product Auto-Expansion
              <span className="text-sm font-normal text-slate-600">ခွဲထုတ်ကုန်များအလိုအလျောက်ဖွင့်ခြင်း</span>
            </CardTitle>
            <p className="text-sm text-slate-600">
              System auto-loaded mapped sub-products. All items are ticked by default, you can untick as needed.
            </p>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead className="w-12">Select</TableHead>
                    <TableHead>Child Code</TableHead>
                    <TableHead>Child Name</TableHead>
                    <TableHead>Default Color</TableHead>
                    <TableHead>Planned Qty</TableHead>
                    <TableHead>Machine Assignment</TableHead>
                    <TableHead>Mold No.</TableHead>
                    <TableHead>Duration (Est.)</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {autoExpandedChildren.map((child) => (
                    <TableRow key={child.id}>
                      <TableCell>
                        <Checkbox
                          checked={child.selected}
                          onCheckedChange={(checked) => handleChildSelectionChange(child.id, checked as boolean)}
                        />
                      </TableCell>
                      <TableCell className="font-mono">{child.code}</TableCell>
                      <TableCell>
                        <div>
                          <div className="font-medium">{child.name}</div>
                          <div className="text-sm text-slate-600">{child.nameLocal}</div>
                        </div>
                      </TableCell>
                      <TableCell>
                        <Badge variant="outline">{child.defaultColor}</Badge>
                      </TableCell>
                      <TableCell>
                        <Input
                          type="number"
                          value={child.plannedQty}
                          disabled={!child.selected}
                          className="w-24"
                          onChange={(e) => {
                            const newQty = parseInt(e.target.value) || 0;
                            setAutoExpandedChildren(prev => 
                              prev.map(c => c.id === child.id ? { ...c, plannedQty: newQty } : c)
                            );
                          }}
                        />
                      </TableCell>
                      <TableCell>
                        <Select disabled={!child.selected}>
                          <SelectTrigger className="w-40">
                            <SelectValue placeholder="Select machine" />
                          </SelectTrigger>
                          <SelectContent>
                            {mockMachines.map((machine) => (
                              <SelectItem key={machine.id} value={machine.id}>
                                {machine.name}
                              </SelectItem>
                            ))}
                          </SelectContent>
                        </Select>
                      </TableCell>
                      <TableCell>
                        <Select disabled={!child.selected}>
                          <SelectTrigger className="w-40">
                            <SelectValue placeholder="Select mold" />
                          </SelectTrigger>
                          <SelectContent>
                            {mockMolds.map((mold) => (
                              <SelectItem key={mold.id} value={mold.id}>
                                {mold.name}
                              </SelectItem>
                            ))}
                          </SelectContent>
                        </Select>
                      </TableCell>
                      <TableCell>
                        <Input
                          placeholder="8 hours"
                          disabled={!child.selected}
                          className="w-24"
                        />
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          </CardContent>
        </Card>
      )}

      {/* Action Buttons */}
      <div className="flex gap-2 justify-end">
        <Button variant="outline">
          <RotateCcw className="h-4 w-4 mr-2" />
          Reset Form
        </Button>
        <Button 
          disabled={!planForm.parentProductCode || !planForm.quantity || selectedChildren.length === 0}
          className="bg-blue-600 hover:bg-blue-700"
        >
          <Save className="h-4 w-4 mr-2" />
          Create Plan
        </Button>
      </div>
    </div>
  );

  const renderPlanningDashboard = () => (
    <div className="space-y-6">
      {/* Dashboard Header */}
      <div className="flex items-center justify-between">
        <div>
          <h3 className="text-lg font-semibold text-slate-900">Production Plans Dashboard</h3>
          <p className="text-sm text-slate-600">ထုတ်လုပ်မှုအစီအစဉ်များဒက်ရှ်ဘုတ်</p>
        </div>
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

      {/* Plans Table */}
      <Card>
        <CardHeader>
          <CardTitle>Production Plans</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-2">
            {mockPlans.map((plan) => (
              <div key={plan.id} className="border rounded-lg">
                {/* Plan Summary Row */}
                <div 
                  className="flex items-center justify-between p-4 cursor-pointer hover:bg-slate-50"
                  onClick={() => togglePlanExpansion(plan.id)}
                >
                  <div className="flex items-center gap-4">
                    {expandedPlans.includes(plan.id) ? (
                      <ChevronDown className="h-4 w-4 text-slate-400" />
                    ) : (
                      <ChevronRight className="h-4 w-4 text-slate-400" />
                    )}
                    <div>
                      <div className="flex items-center gap-2">
                        <span className="font-mono text-sm bg-blue-50 px-2 py-1 rounded">
                          {plan.id}
                        </span>
                        <Badge variant={plan.status === 'final' ? 'default' : 'secondary'}>
                          {plan.status}
                        </Badge>
                      </div>
                      <div className="text-sm text-slate-600 mt-1">
                        {plan.parentProductCode} - {plan.parentProductName}
                      </div>
                    </div>
                  </div>
                  
                  <div className="flex items-center gap-4">
                    <div className="text-right">
                      <div className="font-medium">{plan.quantity.toLocaleString()} sets</div>
                      <div className="text-sm text-slate-600">{plan.plannerName}</div>
                    </div>
                    <div className="text-right">
                      <div className="text-sm text-slate-600">Created</div>
                      <div className="text-sm">{plan.createdDate}</div>
                    </div>
                    <div className="flex gap-1">
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
                  </div>
                </div>

                {/* Expanded Job Details */}
                {expandedPlans.includes(plan.id) && (
                  <div className="border-t bg-slate-50 p-4">
                    <h4 className="font-medium mb-3 flex items-center gap-2">
                      <Settings className="h-4 w-4" />
                      Job IDs & Details
                    </h4>
                    <div className="overflow-x-auto">
                      <Table>
                        <TableHeader>
                          <TableRow>
                            <TableHead>Job ID</TableHead>
                            <TableHead>Child Product</TableHead>
                            <TableHead>Color</TableHead>
                            <TableHead>Qty</TableHead>
                            <TableHead>Machine</TableHead>
                            <TableHead>Mold</TableHead>
                            <TableHead>Duration</TableHead>
                            <TableHead>Status</TableHead>
                            <TableHead>Progress</TableHead>
                          </TableRow>
                        </TableHeader>
                        <TableBody>
                          {plan.jobs.map((job) => (
                            <TableRow key={job.jobId}>
                              <TableCell className="font-mono text-sm">
                                {job.jobId}
                              </TableCell>
                              <TableCell>
                                <div>
                                  <div className="font-medium">{job.childCode}</div>
                                  <div className="text-sm text-slate-600">{job.childName}</div>
                                </div>
                              </TableCell>
                              <TableCell>
                                <Badge variant="outline">{job.defaultColor}</Badge>
                              </TableCell>
                              <TableCell>{job.plannedQty.toLocaleString()}</TableCell>
                              <TableCell>{job.machineId}</TableCell>
                              <TableCell>{job.moldNo}</TableCell>
                              <TableCell>{job.duration}</TableCell>
                              <TableCell>
                                <Badge 
                                  variant={
                                    job.status === 'completed' ? 'default' : 
                                    job.status === 'in-progress' ? 'secondary' : 
                                    'outline'
                                  }
                                >
                                  {job.status}
                                </Badge>
                              </TableCell>
                              <TableCell>
                                <div className="flex items-center gap-2">
                                  {job.status === 'completed' ? (
                                    <CheckCircle className="h-4 w-4 text-green-600" />
                                  ) : job.status === 'in-progress' ? (
                                    <Clock className="h-4 w-4 text-yellow-600" />
                                  ) : (
                                    <AlertCircle className="h-4 w-4 text-slate-400" />
                                  )}
                                  <span className="text-sm">
                                    {job.status === 'completed' ? '100%' : 
                                     job.status === 'in-progress' ? '65%' : '0%'}
                                  </span>
                                </div>
                              </TableCell>
                            </TableRow>
                          ))}
                        </TableBody>
                      </Table>
                    </div>
                  </div>
                )}
              </div>
            ))}
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
            <h1 className="text-2xl font-semibold text-slate-900">Production Planning</h1>
            <p className="text-sm text-slate-600">ထုတ်လုပ်မှုအစီအစဉ်ချမှတ်ခြင်း</p>
            <div className="text-xs text-slate-500 mt-1">Under Planning Module • စီမံခန့်ခွဲမှုမော်ဂျူးအောက်တွင်</div>
          </div>
          <Button className="bg-blue-600 hover:bg-blue-700">
            <Plus className="h-4 w-4 mr-2" />
            Create New Plan
          </Button>
        </div>

        {/* Tabs */}
        <Card>
          <CardContent className="pt-6">
            <Tabs value={activeTab} onValueChange={setActiveTab}>
              <TabsList className="grid w-full grid-cols-2">
                <TabsTrigger value="create-plan" className="flex items-center gap-2">
                  <ClipboardList className="h-4 w-4" />
                  Create New Plan
                </TabsTrigger>
                <TabsTrigger value="planning-dashboard" className="flex items-center gap-2">
                  <FileText className="h-4 w-4" />
                  Planning Dashboard
                </TabsTrigger>
              </TabsList>

              <div className="mt-6">
                <TabsContent value="create-plan" className="space-y-4">
                  {renderCreatePlan()}
                </TabsContent>

                <TabsContent value="planning-dashboard" className="space-y-4">
                  {renderPlanningDashboard()}
                </TabsContent>
              </div>
            </Tabs>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}