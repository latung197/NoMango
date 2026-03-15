import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { EngineeringDashboard } from './EngineeringDashboard';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Textarea } from './ui/textarea';
import { Badge } from './ui/badge';
import { Tabs, TabsContent, TabsList, TabsTrigger } from './ui/tabs';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogTrigger } from './ui/dialog';
import { ReasonDropdown } from './ui/reason-dropdown';
import { MoldChangeRequests } from './MoldChangeRequests';
import { toast } from 'sonner@2.0.3';
import { 
  Thermometer, 
  Layers, 
  Wrench, 
  UserCheck,
  Plus,
  Upload,
  BarChart3,
  Settings,
  Play,
  Pause,
  RotateCcw,
  CheckCircle,
  Download,
  Filter,
  Search,
  Edit,
  Trash2,
  Eye,
  Calendar,
  DragDropContext,
  FileText,
  AlertTriangle,
  Zap,
  Droplets,
  Package,
  Bolt
} from 'lucide-react';

interface EngineeringModuleProps {
  currentPage: string;
  onPageChange: (page: string) => void;
}

interface TabConfig {
  id: string;
  name: string;
  nameMM: string;
  color: string;
  bgColor: string;
  borderColor: string;
  textColor: string;
  isActive: boolean;
  isAdminOnly?: boolean;
}

interface RecordData {
  id: string;
  machineNo: string;
  reason: string;
  description: string;
  attachments: string[];
  status: 'Start' | 'Pause' | 'Resume' | 'Done';
  createdAt: string;
  createdBy: string;
  category: string;
}

interface AdminUser {
  role: 'admin' | 'user';
}

// State for managing reasons dynamically
const reasonsState = {
  'plastic-technician': {
    'material-side': [
      { id: 'MT001', en: 'Silver mark', mm: 'စိမ်းလက်မှတ်' },
      { id: 'MT002', en: 'Color not consistent', mm: 'အရောင်မရ' },
      { id: 'MT003', en: 'Contamination', mm: 'ညစ်ညမ်းမှု' },
      { id: 'MT004', en: 'Material degradation', mm: 'ပစ္စည်းပျက်စီး' }
    ],
    'injection-side': [
      { id: 'IS001', en: 'Nozzle misalignment', mm: 'Nozzle မတည့်' },
      { id: 'IS002', en: 'Muffler broken', mm: 'Muffler ပြုတ်' },
      { id: 'IS003', en: 'Injection pressure low', mm: 'အင်ဂျက်ရှင်ဖိအားနည်း' },
      { id: 'IS004', en: 'Shot size inconsistent', mm: 'Shot အရွယ်အစားမညီ' }
    ],
    'mold-side': [
      { id: 'MS001', en: 'Mold Runner broken', mm: 'Mold Runner ပျက်' },
      { id: 'MS002', en: 'Mold Ejector Pin Broken', mm: 'မူလ္တီထုတ်ပင်ကျိုး' },
      { id: 'MS003', en: 'Gate mark issue', mm: 'Gate မှတ်သားမှုပြဿနာ' },
      { id: 'MS004', en: 'Flash on product', mm: 'ထုတ်ကုန်ပေါ်တွင် Flash' }
    ],
    'machinery-side': [
      { id: 'MC001', en: 'Hydraulic oil leak', mm: 'Hydraulic ဆီယို' },
      { id: 'MC002', en: 'Air compressor Fail', mm: 'Air compressor ပျက်' },
      { id: 'MC003', en: 'Toggle mechanism stuck', mm: 'Toggle ယန္တရားပိတ်' },
      { id: 'MC004', en: 'Clamp force inconsistent', mm: 'Clamp အားမညီ' }
    ],
    'electrical-side': [
      { id: 'EL001', en: 'Heater Coil Fail', mm: 'ဟီတာကွိုင်းပျက်' },
      { id: 'EL002', en: 'Control Panel Wiring Error', mm: 'Control Panel ဝိုင်ယာချို့ယွင်း' },
      { id: 'EL003', en: 'Temperature sensor fault', mm: 'အပူချိန်အာရုံခံကိရိယာပျက်' },
      { id: 'EL004', en: 'Motor overheating', mm: 'မော်တာအပူလွန်ကဲ' }
    ]
  },
  'maintenance-engineer': {
    'hydraulic-side': [
      { id: 'HY001', en: 'Hydraulic Oil Leak', mm: 'အိုင်ဒရောလစ်ဆီယို' },
      { id: 'HY002', en: 'Seal Change', mm: 'ဆီလ်ပြောင်း' },
      { id: 'HY003', en: 'Pressure valve stuck', mm: 'ဖိအားအဆို့ရှင်ပိတ်' },
      { id: 'HY004', en: 'Pump failure', mm: 'ပန့်ပျက်စီး' }
    ],
    'mechanical-side': [
      { id: 'ME001', en: 'Toggle Pin Repair', mm: 'Toggle Pin ပြုပြင်' },
      { id: 'ME002', en: 'Safety Door Lock Repair', mm: 'လုံခြုံရေးတံခါးသော့ပြုပြင်' },
      { id: 'ME003', en: 'Belt replacement', mm: 'ဘဲ့လ်ပြောင်း' },
      { id: 'ME004', en: 'Bearing maintenance', mm: 'ဘီယာရင်းပြုပြင်' }
    ],
    'electrical-side': [
      { id: 'EL005', en: 'Heater Coil Fail', mm: 'ဟီတာကွိုင်းပျက်' },
      { id: 'EL006', en: 'I/O Control Card Fail', mm: 'I/O Control Card ပျက်' },
      { id: 'EL007', en: 'Circuit breaker trip', mm: 'Circuit breaker လုပ်ကွက်' },
      { id: 'EL008', en: 'Wiring short circuit', mm: 'ဝိုင်ယာတိုတောင်း' }
    ],
    'water-air-side': [
      { id: 'WA001', en: 'Cooling Tower Repair', mm: 'ရေချိုင်းစက်ပြုပြင်' },
      { id: 'WA002', en: 'Screw Air Compressor Fail', mm: 'Screw Air Compressor ပျက်' },
      { id: 'WA003', en: 'Water pump malfunction', mm: 'ရေပန့်ပျက်စီး' },
      { id: 'WA004', en: 'Air filter blocked', mm: 'လေစစ်ကာပိတ်' }
    ]
  },
  'mold-technician': {
    'onsite-repair': [
      { id: 'OR001', en: 'Side Core Piston Seal Repair', mm: 'ဘက်ပိုင်း Core Piston ဆီလ်ပြုပြင်' },
      { id: 'OR002', en: 'Gate adjustment', mm: 'Gate ညှိခြင်း' },
      { id: 'OR003', en: 'Ejector pin alignment', mm: 'Ejector pin တန်းညှိ' },
      { id: 'OR004', en: 'Minor cooling line fix', mm: 'အေးခဲလိုင်းအသေးပြုပြင်' }
    ],
    'heavy-repair': [
      { id: 'HR001', en: 'Mold Hot Runner Coil Fail', mm: 'ဟော့ရနာကွိုင်းပျက်' },
      { id: 'HR002', en: 'Mold Guide Pin Broken', mm: 'မှိုလမ်းညွှန်ပင်ကျိုး' },
      { id: 'HR003', en: 'Complete mold overhaul', mm: 'မှိုအပြည့်အစုံပြုပြင်' },
      { id: 'HR004', en: 'Cavity replacement', mm: 'Cavity အစားထိုး' }
    ],
    'others': [
      { id: 'OT001', en: 'Mold Core Damage', mm: 'မူလ္တီ Core ပျက်စီး' },
      { id: 'OT002', en: 'General Adjustment / Polishing', mm: 'ယေဘုယျညှိခြင်း / ပွတ်တိုက်ခြင်း' },
      { id: 'OT003', en: 'Preventive maintenance', mm: 'ကြိုတင်ပြုပြင်မှု' },
      { id: 'OT004', en: 'Mold inspection', mm: 'မှိုစစ်ဆေးခြင်း' }
    ]
  }
};

export function EngineeringModule({ currentPage, onPageChange }: EngineeringModuleProps) {
  const [currentUser] = useState<AdminUser>({ role: 'admin' }); // Mock user for demo
  const [records, setRecords] = useState<RecordData[]>([]);
  const [showNewRecordDialog, setShowNewRecordDialog] = useState(false);
  const [activeTab, setActiveTab] = useState('');
  const [searchTerm, setSearchTerm] = useState('');
  const [filterStatus, setFilterStatus] = useState('all');
  const [wizardStep, setWizardStep] = useState(1);

  // Form state
  const [formData, setFormData] = useState({
    machineNo: '',
    reason: '',
    description: '',
    attachments: [] as File[],
    status: 'Start' as 'Start' | 'Pause' | 'Resume' | 'Done'
  });

  // State for managing reasons dynamically
  const [reasonsDynamicState, setReasonsDynamicState] = useState(reasonsState);

  // Tab configurations for each role
  const getTabConfigs = (role: string): TabConfig[] => {
    switch (role) {
      case 'plastic-engineer':
        return [
          { id: 'material-side', name: 'Material Side', nameMM: 'ကုန်ကြမ်းဘက်', color: 'orange', bgColor: 'bg-orange-100', borderColor: 'border-orange-300', textColor: 'text-orange-800', isActive: true },
          { id: 'injection-side', name: 'Injection Side', nameMM: 'အင်ဂျက်ရှင်ဘက်', color: 'green', bgColor: 'bg-green-100', borderColor: 'border-green-300', textColor: 'text-green-800', isActive: true },
          { id: 'mold-side', name: 'Mold Side', nameMM: 'မူလ္တီဘက်', color: 'amber', bgColor: 'bg-amber-100', borderColor: 'border-amber-300', textColor: 'text-amber-800', isActive: true },
          { id: 'machinery-side', name: 'Machinery Side', nameMM: 'စက်ဘက်', color: 'purple', bgColor: 'bg-purple-100', borderColor: 'border-purple-300', textColor: 'text-purple-800', isActive: true },
          { id: 'electrical-side', name: 'Electrical Side', nameMM: 'လျှပ်စစ်ဘက်', color: 'green', bgColor: 'bg-green-100', borderColor: 'border-green-300', textColor: 'text-green-800', isActive: true }
        ];
      
      case 'maintenance-engineer':
        return [
          { id: 'hydraulic-side', name: 'Hydraulic Side', nameMM: 'အိုင်ဒရောလစ်ဘက်', color: 'orange', bgColor: 'bg-orange-100', borderColor: 'border-orange-300', textColor: 'text-orange-800', isActive: true },
          { id: 'mechanical-side', name: 'Mechanical Side', nameMM: 'မက်ကနေရှင်နယ်ဘက်', color: 'green', bgColor: 'bg-green-100', borderColor: 'border-green-300', textColor: 'text-green-800', isActive: true },
          { id: 'electrical-side', name: 'Electrical Side', nameMM: 'လျှပ်စစ်ဘက်', color: 'amber', bgColor: 'bg-amber-100', borderColor: 'border-amber-300', textColor: 'text-amber-800', isActive: true },
          { id: 'water-air-side', name: 'Water & Air Side', nameMM: 'ရေ & လေဘက်', color: 'purple', bgColor: 'bg-purple-100', borderColor: 'border-purple-300', textColor: 'text-purple-800', isActive: true }
        ];
      
      case 'mold-engineer':
        return [
          { id: 'onsite-repair', name: 'Onsite Repair', nameMM: 'နေရာပေါ်မှာ ပြုပြင်ခြင်း', color: 'green', bgColor: 'bg-green-100', borderColor: 'border-green-300', textColor: 'text-green-800', isActive: true },
          { id: 'heavy-repair', name: 'Heavy Repair', nameMM: 'အလေးအနက်ပြုပြင်ခြင်း', color: 'purple', bgColor: 'bg-purple-100', borderColor: 'border-purple-300', textColor: 'text-purple-800', isActive: true },
          { id: 'others', name: 'Others', nameMM: 'အခြားများ', color: 'green', bgColor: 'bg-green-100', borderColor: 'border-green-300', textColor: 'text-green-800', isActive: true }
        ];
      
      default:
        return [];
    }
  };

  const getCurrentRoleKey = (page: string) => {
    switch (page) {
      case 'plastic-engineer': return 'plastic-technician';
      case 'maintenance-engineer': return 'maintenance-engineer';
      case 'mold-engineer': return 'mold-technician';
      default: return 'plastic-technician';
    }
  };

  const getReasonOptions = (roleKey: string, tabId: string) => {
    const roleReasons = reasonsDynamicState[roleKey as keyof typeof reasonsDynamicState];
    if (!roleReasons) return [];
    return roleReasons[tabId as keyof typeof roleReasons] || [];
  };

  const handleAddReason = (newReason: { id: string; en: string; mm: string; description?: string }) => {
    const roleKey = getCurrentRoleKey(currentPage);
    const currentTab = activeTab || getTabConfigs(currentPage)[0]?.id;
    
    setReasonsDynamicState(prev => ({
      ...prev,
      [roleKey]: {
        ...prev[roleKey as keyof typeof prev],
        [currentTab]: [...(prev[roleKey as keyof typeof prev]?.[currentTab as keyof any] || []), newReason]
      }
    }));
  };

  const handleNewRecord = () => {
    if (!formData.machineNo || !formData.reason || !formData.description) {
      toast.error('Please fill in all required fields | လိုအပ်သော အကွက်များကို ဖြည့်ပါ');
      return;
    }

    const newRecord: RecordData = {
      id: `REC-${Date.now()}`,
      machineNo: formData.machineNo,
      reason: formData.reason,
      description: formData.description,
      attachments: formData.attachments.map(f => f.name),
      status: formData.status,
      createdAt: new Date().toLocaleString(),
      createdBy: 'Current User',
      category: activeTab
    };

    setRecords(prev => [newRecord, ...prev]);
    
    // Reset form
    setFormData({
      machineNo: '',
      reason: '',
      description: '',
      attachments: [],
      status: 'Start'
    });
    
    setWizardStep(1);
    setShowNewRecordDialog(false);
    toast.success('New record created successfully | မှတ်တမ်းအသစ် ဖန်တီးပြီးပါပြီ');
  };

  const handleFileUpload = (event: React.ChangeEvent<HTMLInputElement>) => {
    const files = Array.from(event.target.files || []);
    setFormData(prev => ({
      ...prev,
      attachments: [...prev.attachments, ...files]
    }));
  };

  const updateRecordStatus = (recordId: string, newStatus: 'Start' | 'Pause' | 'Resume' | 'Done') => {
    setRecords(prev => prev.map(record => 
      record.id === recordId ? { ...record, status: newStatus } : record
    ));
    toast.success(`Status updated to ${newStatus} | အခြေအနေကို ${newStatus} သို့ ပြောင်းလဲပြီး`);
  };

  const getStatusBadgeColor = (status: string) => {
    switch (status) {
      case 'Start': return 'bg-blue-100 text-blue-800';
      case 'Pause': return 'bg-yellow-100 text-yellow-800';
      case 'Resume': return 'bg-green-100 text-green-800';
      case 'Done': return 'bg-gray-100 text-gray-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  const getStatusIcon = (status: string) => {
    switch (status) {
      case 'Start': return <Play className="h-4 w-4" />;
      case 'Pause': return <Pause className="h-4 w-4" />;
      case 'Resume': return <Play className="h-4 w-4" />;
      case 'Done': return <CheckCircle className="h-4 w-4" />;
      default: return <Play className="h-4 w-4" />;
    }
  };

  const getTabIcon = (tabId: string) => {
    switch (tabId) {
      case 'material-side': return <Package className="h-8 w-8 text-orange-600" />;
      case 'injection-side': return <Zap className="h-8 w-8 text-green-600" />;
      case 'mold-side': return <Layers className="h-8 w-8 text-amber-600" />;
      case 'machinery-side': return <Wrench className="h-8 w-8 text-purple-600" />;
      case 'electrical-side': return <Bolt className="h-8 w-8 text-green-600" />;
      case 'hydraulic-side': return <Droplets className="h-8 w-8 text-orange-600" />;
      case 'mechanical-side': return <Wrench className="h-8 w-8 text-green-600" />;
      case 'water-air-side': return <Droplets className="h-8 w-8 text-purple-600" />;
      case 'onsite-repair': return <Wrench className="h-8 w-8 text-green-600" />;
      case 'heavy-repair': return <AlertTriangle className="h-8 w-8 text-purple-600" />;
      case 'others': return <Settings className="h-8 w-8 text-green-600" />;
      default: return <Settings className="h-8 w-8 text-slate-600" />;
    }
  };

  const renderNewRecordWizard = (tabConfigs: TabConfig[], roleKey: string, currentTab: string) => {
    return (
      <div className="space-y-6">
        {/* Step indicator */}
        <div className="flex items-center justify-center space-x-4 mb-8">
          {[1, 2, 3].map((step) => (
            <div key={step} className="flex items-center">
              <div className={`w-10 h-10 rounded-full flex items-center justify-center font-semibold ${
                wizardStep >= step 
                  ? 'bg-blue-600 text-white' 
                  : 'bg-slate-200 text-slate-500'
              }`}>
                {step}
              </div>
              {step < 3 && (
                <div className={`w-16 h-1 ml-4 ${
                  wizardStep > step ? 'bg-blue-600' : 'bg-slate-200'
                }`} />
              )}
            </div>
          ))}
        </div>

        {wizardStep === 1 && (
          <div className="space-y-6">
            <h3 className="text-xl font-semibold text-center">Step 1: Select Information | အချက်အလက်ရွေးချယ်ရန်</h3>
            
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <Label htmlFor="machineNo">Machine No. (စက်နံပါတ်) *</Label>
                <Select value={formData.machineNo} onValueChange={(value) => setFormData(prev => ({ ...prev, machineNo: value }))}>
                  <SelectTrigger className="h-12 text-lg">
                    <SelectValue placeholder="Select machine | စက်ရွေးပါ" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="INJ-001">INJ-001 - Injection Machine 1</SelectItem>
                    <SelectItem value="INJ-002">INJ-002 - Injection Machine 2</SelectItem>
                    <SelectItem value="INJ-003">INJ-003 - Injection Machine 3</SelectItem>
                    <SelectItem value="INJ-004">INJ-004 - Injection Machine 4</SelectItem>
                    <SelectItem value="INJ-005">INJ-005 - Injection Machine 5</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div>
                <Label htmlFor="category">Category (အမျိုးအစား)</Label>
                <div className="p-3 bg-blue-50 rounded-lg border border-blue-200">
                  <div className="font-medium text-blue-800">
                    {tabConfigs.find(t => t.id === currentTab)?.name || 'Selected Category'}
                  </div>
                  <div className="text-sm text-blue-600">
                    {tabConfigs.find(t => t.id === currentTab)?.nameMM || 'ရွေးချယ်ထားသော အမျိုးအစား'}
                  </div>
                </div>
              </div>
            </div>

            <div>
              <Label htmlFor="reason">Reason (အကြောင်းရင်း) *</Label>
              <ReasonDropdown
                value={formData.reason}
                onValueChange={(value) => setFormData(prev => ({ ...prev, reason: value }))}
                category={currentTab}
                categoryDisplayName={tabConfigs.find(t => t.id === currentTab)?.name || 'Category'}
                categoryDisplayNameMM={tabConfigs.find(t => t.id === currentTab)?.nameMM || 'အမျိုးအစား'}
                reasons={getReasonOptions(roleKey, currentTab)}
                onAddReason={handleAddReason}
                currentUser={currentUser}
                className="mt-2"
              />
            </div>

            <div className="flex justify-end">
              <Button 
                onClick={() => setWizardStep(2)} 
                disabled={!formData.machineNo || !formData.reason}
                className="bg-blue-600 hover:bg-blue-700 px-8"
              >
                Next | ရှေ့သို့
              </Button>
            </div>
          </div>
        )}

        {wizardStep === 2 && (
          <div className="space-y-6">
            <h3 className="text-xl font-semibold text-center">Step 2: Problem Details | ပြဿနာအသေးစိတ်</h3>
            
            <div>
              <Label htmlFor="description">Description (ဖော်ပြချက်) *</Label>
              <Textarea
                value={formData.description}
                onChange={(e) => setFormData(prev => ({ ...prev, description: e.target.value }))}
                placeholder="Enter problem details in both English and Myanmar... | ပြဿနာအသေးစိတ်ကို အင်္ဂလိပ်နှင့် မြန်မာ နှစ်ဘာသာဖြင့် ရေးပါ..."
                rows={6}
                className="text-lg"
              />
            </div>

            <div>
              <Label htmlFor="attachments">Attachments (ဓာတ်ပုံ/ဗီဒီယို)</Label>
              <div className="border-2 border-dashed border-slate-300 rounded-lg p-8 text-center hover:border-slate-400 transition-colors">
                <input
                  type="file"
                  id="attachments"
                  multiple
                  accept="image/*,video/*"
                  onChange={handleFileUpload}
                  className="hidden"
                />
                <label htmlFor="attachments" className="cursor-pointer">
                  <Upload className="h-12 w-12 mx-auto mb-4 text-slate-400" />
                  <p className="text-lg font-medium text-slate-600 mb-2">
                    Drag & drop files here or click to browse
                  </p>
                  <p className="text-sm text-slate-500">
                    ဖိုင်များကို ဆွဲယူ၍ ထည့်ပါ သို့မဟုတ် နှိပ်၍ ရွေးပါ
                  </p>
                </label>
              </div>
              
              {formData.attachments.length > 0 && (
                <div className="mt-4 space-y-2">
                  {formData.attachments.map((file, index) => (
                    <div key={index} className="flex items-center gap-3 p-3 bg-slate-50 rounded-lg">
                      <Upload className="h-5 w-5 text-slate-400" />
                      <span className="flex-1 font-medium">{file.name}</span>
                      <span className="text-sm text-slate-500">{(file.size / 1024 / 1024).toFixed(2)} MB</span>
                    </div>
                  ))}
                </div>
              )}
            </div>

            <div className="flex justify-between">
              <Button variant="outline" onClick={() => setWizardStep(1)} className="px-8">
                Back | နောက်သို့
              </Button>
              <Button 
                onClick={() => setWizardStep(3)} 
                disabled={!formData.description}
                className="bg-blue-600 hover:bg-blue-700 px-8"
              >
                Next | ရှေ့သို့
              </Button>
            </div>
          </div>
        )}

        {wizardStep === 3 && (
          <div className="space-y-6">
            <h3 className="text-xl font-semibold text-center">Step 3: Set Status | အခြေအနေသတ်မှတ်ရန်</h3>
            
            <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
              {['Start', 'Pause', 'Resume', 'Done'].map((status) => (
                <Button
                  key={status}
                  variant={formData.status === status ? "default" : "outline"}
                  className={`h-20 flex-col text-lg font-semibold ${
                    formData.status === status 
                      ? 'bg-blue-600 hover:bg-blue-700 text-white' 
                      : 'border-2 hover:border-blue-300'
                  }`}
                  onClick={() => setFormData(prev => ({ ...prev, status: status as any }))}
                >
                  {getStatusIcon(status)}
                  <span className="mt-2">{status}</span>
                  <span className="text-sm opacity-75">
                    {status === 'Start' && 'စတင်'}
                    {status === 'Pause' && 'ရပ်ဆိုင်း'}
                    {status === 'Resume' && 'ပြန်လည်စတင်'}
                    {status === 'Done' && 'ပြီးစီး'}
                  </span>
                </Button>
              ))}
            </div>

            <div className="bg-slate-50 p-6 rounded-lg">
              <h4 className="font-semibold mb-4">Record Summary | မှတ်တမ်း အကျဉ်းချုပ်</h4>
              <div className="space-y-2 text-sm">
                <div><span className="font-medium">Machine:</span> {formData.machineNo}</div>
                <div><span className="font-medium">Reason:</span> {formData.reason}</div>
                <div><span className="font-medium">Status:</span> {formData.status}</div>
                <div><span className="font-medium">Attachments:</span> {formData.attachments.length} files</div>
              </div>
            </div>

            <div className="flex justify-between">
              <Button variant="outline" onClick={() => setWizardStep(2)} className="px-8">
                Back | နောက်သို့
              </Button>
              <Button onClick={handleNewRecord} className="bg-green-600 hover:bg-green-700 px-8 text-lg font-semibold">
                <CheckCircle className="h-5 w-5 mr-2" />
                Save Record | မှတ်တမ်းသိမ်း
              </Button>
            </div>
          </div>
        )}
      </div>
    );
  };

  const renderEngineeringDashboard = () => {
    return <EngineeringDashboard />;
  };

  const renderRoleSpecificContent = () => {
    const tabConfigs = getTabConfigs(currentPage);
    if (tabConfigs.length === 0) {
      return renderEngineeringDashboard();
    }

    const roleKey = getCurrentRoleKey(currentPage);
    const currentTab = activeTab || tabConfigs[0]?.id;

    return (
      <div className="min-h-screen bg-slate-50">
        {/* Header Bar - Sticky Top */}
        <div className="sticky top-0 z-50 bg-white border-b border-slate-200 shadow-sm">
          <div className="max-w-7xl mx-auto px-6 py-4">
            <div className="flex items-center justify-between">
              <div>
                <h1 className="text-2xl font-semibold text-slate-900">
                  {currentPage === 'plastic-engineer' ? 'Plastic Technician | ပလတ်စတစ်နည်းပြ' : 
                   currentPage === 'mold-engineer' ? 'Mold Technician | မှိုနည်းပညာသမား' : 
                   'Maintenance Engineer | ပြုပြင်ထိန်းသိမ်းမှုအင်ဂျင်နီယာ'}
                </h1>
              </div>
              <div className="flex items-center gap-3">
                <Button variant="outline" size="sm" className="text-slate-600">
                  <Search className="h-4 w-4 mr-2" />
                  Filter | စစ်ထုတ်ရန်
                </Button>
                <Button variant="outline" size="sm" className="text-slate-600">
                  <Download className="h-4 w-4 mr-2" />
                  Export | တင်ယူရန်
                </Button>
                <Dialog open={showNewRecordDialog} onOpenChange={setShowNewRecordDialog}>
                  <DialogTrigger asChild>
                    <Button className="bg-blue-600 hover:bg-blue-700 text-white font-medium px-6">
                      <Plus className="h-4 w-4 mr-2" />
                      New Record | ပြုပြင်အသစ်
                    </Button>
                  </DialogTrigger>
                  <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
                    <DialogHeader>
                      <DialogTitle>New Record | မှတ်တမ်းအသစ်</DialogTitle>
                      <DialogDescription>
                        Create a new maintenance record with step-by-step process. Fill in all required information to track the issue and resolution.
                      </DialogDescription>
                    </DialogHeader>
                    {renderNewRecordWizard(tabConfigs, roleKey, currentTab)}
                  </DialogContent>
                </Dialog>
              </div>
            </div>
          </div>
        </div>

        {/* Tab Navigation - Large Rounded Cards */}
        <div className="max-w-7xl mx-auto px-6 py-8">
          <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-5 gap-4 mb-8">
            {tabConfigs.map((tab) => {
              const isActive = currentTab === tab.id;
              const tabIcon = getTabIcon(tab.id);
              return (
                <Card 
                  key={tab.id}
                  className={`cursor-pointer transition-all duration-200 hover:shadow-lg border-2 ${
                    isActive 
                      ? `${tab.borderColor} shadow-lg scale-105` 
                      : 'border-slate-200 hover:border-slate-300'
                  }`}
                  onClick={() => setActiveTab(tab.id)}
                >
                  <CardContent className="p-6 text-center">
                    <div className={`w-16 h-16 rounded-full ${tab.bgColor} flex items-center justify-center mx-auto mb-4`}>
                      {tabIcon}
                    </div>
                    <h3 className={`font-semibold text-lg mb-1 ${isActive ? tab.textColor : 'text-slate-700'}`}>
                      {tab.name}
                    </h3>
                    <p className="text-sm text-slate-500">{tab.nameMM}</p>
                  </CardContent>
                </Card>
              );
            })}
            
            {currentUser.role === 'admin' && (
              <Card className="cursor-pointer border-2 border-dashed border-slate-300 hover:border-slate-400 transition-colors">
                <CardContent className="p-6 text-center">
                  <div className="w-16 h-16 rounded-full bg-slate-100 flex items-center justify-center mx-auto mb-4">
                    <Plus className="h-8 w-8 text-slate-400" />
                  </div>
                  <h3 className="font-semibold text-lg mb-1 text-slate-600">Add Tab</h3>
                  <p className="text-sm text-slate-400">ဖြည့်သွင်းရန်</p>
                </CardContent>
              </Card>
            )}
          </div>

          {/* Main Content Panel */}
          <div className="bg-white rounded-lg shadow-sm border border-slate-200">
            {tabConfigs.filter(tab => tab.id === currentTab).map((tab) => (
              <div key={tab.id} className="p-8">
                <h2 className="text-2xl font-semibold mb-6 flex items-center gap-3">
                  {getTabIcon(tab.id)}
                  <span>{tab.name} Records | {tab.nameMM} မှတ်တမ်းများ</span>
                </h2>

                {/* Records Grid */}
                {records.filter(record => record.category === tab.id).length === 0 ? (
                  <Card className="border-dashed border-2 border-slate-300">
                    <CardContent className="p-16 text-center">
                      <div className="text-slate-300 mb-6">
                        <div className="w-24 h-24 mx-auto rounded-full bg-slate-100 flex items-center justify-center mb-4">
                          <FileText className="h-12 w-12 text-slate-400" />
                        </div>
                      </div>
                      <h3 className="text-xl font-semibold text-slate-600 mb-2">
                        No records yet for {tab.name}
                      </h3>
                      <p className="text-lg text-slate-500 mb-6">
                        {tab.nameMM} အတွက် မှတ်တမ်းများ မရှိသေးပါ
                      </p>
                      <Dialog open={showNewRecordDialog} onOpenChange={setShowNewRecordDialog}>
                        <DialogTrigger asChild>
                          <Button size="lg" className="bg-blue-600 hover:bg-blue-700 text-lg px-8 py-3">
                            <Plus className="h-5 w-5 mr-2" />
                            New Record | မှတ်တမ်းအသစ်
                          </Button>
                        </DialogTrigger>
                      </Dialog>
                    </CardContent>
                  </Card>
                ) : (
                  <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                    {records
                      .filter(record => record.category === tab.id)
                      .map((record) => (
                        <Card key={record.id} className="border-0 shadow-md hover:shadow-lg transition-all duration-200">
                          <CardContent className="p-6">
                            <div className="space-y-4">
                              {/* Header */}
                              <div className="flex items-center justify-between">
                                <Badge variant="outline" className="font-mono text-lg px-3 py-1">
                                  {record.machineNo}
                                </Badge>
                                <Badge className={`${getStatusBadgeColor(record.status)} text-sm px-3 py-1`}>
                                  {getStatusIcon(record.status)}
                                  <span className="ml-2">{record.status}</span>
                                </Badge>
                              </div>

                              {/* Reason */}
                              <div>
                                <h4 className="font-semibold text-lg text-slate-800 mb-2">
                                  {record.reason.split(' | ')[0]}
                                </h4>
                                <p className="text-sm text-slate-500">
                                  {record.reason.split(' | ')[1]}
                                </p>
                              </div>

                              {/* Description */}
                              <div>
                                <p className="text-sm text-slate-600 line-clamp-3">
                                  {record.description}
                                </p>
                              </div>

                              {/* Footer */}
                              <div className="flex items-center justify-between text-sm text-slate-500">
                                <span className="flex items-center gap-1">
                                  <Calendar className="h-4 w-4" />
                                  {record.createdAt}
                                </span>
                                <div className="flex gap-2">
                                  <Button size="sm" variant="outline">
                                    <Eye className="h-4 w-4" />
                                  </Button>
                                  <Button size="sm" variant="outline">
                                    <Edit className="h-4 w-4" />
                                  </Button>
                                </div>
                              </div>
                            </div>
                          </CardContent>
                        </Card>
                      ))}
                  </div>
                )}
              </div>
            ))}
          </div>
        </div>
      </div>
    );
  };

  // Handle Engineering Dashboard
  if (currentPage === 'engineering-dashboard') {
    return renderEngineeringDashboard();
  }

  // Handle Mold Change Tasks for Mold Technicians
  if (currentPage === 'mold-change-tasks') {
    return <MoldChangeRequests userRole="mold-technician" />;
  }

  return renderRoleSpecificContent();
}