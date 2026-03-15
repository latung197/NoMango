import React, { useState } from 'react';
import { 
  BarChart3, 
  Calendar, 
  ClipboardList, 
  Database, 
  Factory, 
  FileText, 
  Home, 
  Layers, 
  Package, 
  Settings, 
  Shield, 
  TrendingUp, 
  Users, 
  Wrench,
  ChevronRight,
  ChevronDown,
  Menu,
  X,
  ArrowRightLeft,
  FormInput,
  Thermometer,
  Bolt,
  AlertTriangle,
  Cog,
  QrCode,

  RefreshCw,
  UserCheck,
  Building2,
  Clock,
  Award,
  UserPlus,
  TestTube,
  Building,
  Briefcase,
  LineChart,
  Target,
  Activity,
  CheckCircle,
  Gauge,

  Cpu,
  Smartphone,
  Scale,
  Droplets,
  Truck
} from 'lucide-react';
import { Button } from './ui/button';
import { ScrollArea } from './ui/scroll-area';
import { Collapsible, CollapsibleContent, CollapsibleTrigger } from './ui/collapsible';

interface LayoutProps {
  children: React.ReactNode;
  currentPage: string;
  onPageChange: (page: string) => void;
}

const navigationItems = [
  {
    id: 'dashboard',
    title: 'Executive Dashboard',
    titleMM: 'အမှုဆောင်ဒက်ရှ်ဘုတ်',
    icon: Home,
    children: []
  },
  {
    id: 'planning',
    title: 'Planning',
    titleMM: 'အသေးစိတ်စီမံခန့်ခွဲမှု',
    icon: Calendar,
    children: [
      { id: 'product-master', title: 'Product Master', titleMM: 'ထုတ်ကုန��စာရင်း' },
      { id: 'production-planning', title: 'Production Planning', titleMM: 'ထုတ်လုပ်မှုအစီအစဉ်ချမှတ်ခြင်း' },
      { id: 'planning-dashboard', title: 'Planning Dashboard', titleMM: 'စီမံခန့်ခွဲမှုဒက်ရှ်ဘုတ်' },
      { id: 'planning-reports', title: 'Planning Reports', titleMM: 'စီမံခန့်ခွဲမှုအစီရင်ခံစာများ' }
    ]
  },
  {
    id: 'production-control',
    title: 'Production Control',
    titleMM: 'ထုတ်လုပ်မှုထိန်းချုပ်မှု',
    icon: ClipboardList,
    children: [
      { id: 'demand-authorization', title: 'Demand Authorization', titleMM: 'လိုအပ်ချက်အတည်ပြုခြင်း' },
      { id: 'production-dashboard', title: 'Production Dashboard', titleMM: 'ထုတ်လုပ်မှုဒက်ရှ်ဘုတ်', icon: Clock },
      { id: 'material-control', title: 'Raw Material Control', titleMM: 'ပစ္စည်းထိန်းချုပ်မှု' },
      { id: 'job-monitoring', title: 'Job Monitoring', titleMM: 'အလုပ်စောင့်ကြည့်မှု' },
      { id: 'mold-change-request', title: 'Mold Change Request', titleMM: 'မို ပြောင်းလဲမှု', icon: Settings },
      { id: 'finished-goods-transfer', title: 'Finished Goods Transfer', titleMM: 'ပြီးသားထုတ်ကုန် လွှဲပြောင်းခြင်း' },
      { id: 'cut-glue-residue', title: 'Crushing, Flash, Nozzle Waste Management', titleMM: 'ခုတ်ဖက် / ကော်စာ / ကော်ခဲ စီမံ', icon: Package },
      { id: 'reject-management', title: 'Reject Management', titleMM: 'ပယ်ဖျက်ပစ္စည်း စီမံ' },
      { id: 'waste-management', title: 'Waste Management', titleMM: 'အပိုင်အချေအစားစီမံခန့်ခွဲမှု' },
      { id: 'split-job-management', title: 'Split Job Management', titleMM: 'ခွဲထုတ်အလုပ်စီမံခန့်ခွဲမှု' },
      { id: 'job-tracker', title: 'Job Tracker', titleMM: 'အလုပ်ခြေရာခံမှု' }
    ]
  },
  {
    id: 'production',
    title: 'Production',
    titleMM: 'ထုတ်လုပ်မှု',
    icon: Factory,
    children: [
      { id: 'operator-kiosk', title: 'Operator Kiosk', titleMM: 'အော်ပရေတာကီယော့စ်' },
      { id: 'glue-fill-operator', title: 'Glue Fill Operator', titleMM: 'ကော်ဖြည့်အလုပ်သမား', icon: Droplets },
      { id: 'supervisor-screen', title: 'Supervisor Screen', titleMM: 'ကြီးကြပ်သူမျက်နှာပြင်' },
      { id: 'live-monitoring', title: 'Live Monitoring', titleMM: 'တိုက်ရိုက်စောင့်ကြည့်ခြင်း' },
      { id: 'operator-dashboard', title: 'Operator Dashboard', titleMM: 'အော်ပရေတာဒက်ရှ်ဘုတ်', icon: Users }
    ]
  },
  {
    id: 'qc',
    title: 'QC (Quality Control)',
    titleMM: 'အရည်အသွေးထိန်းချုပ်မှု',
    icon: Shield,
    children: [
      { id: 'qc-dashboard', title: 'QC Dashboard', titleMM: 'အရည်အသွေးဒက်ရှ်ဘုတ်', icon: BarChart3 },
      { id: 'raw-material-qc', title: 'Raw Material QC', titleMM: 'ကုန်ကြမ်း QC', icon: Package },
      { id: 'in-process-qc', title: 'In-Process QC', titleMM: 'လုပ်ငန်းစဉ် QC', icon: Gauge },
      { id: 'finished-goods-qc', title: 'Finished Goods QC', titleMM: 'ပြီးစီးထုတ်ကုန် QC', icon: CheckCircle },
      { id: 'qc-reports', title: 'QC Reports', titleMM: 'အရည်အသွေးအစီရင်ခံစာများ', icon: FileText },
      { id: 'qc-settings', title: 'QC Settings', titleMM: 'QC ဆက်တင်များ', icon: Settings }
    ]
  },
  {
    id: 'engineering',
    title: 'Engineering',
    titleMM: 'အင်ဂျင်နီယာ',
    icon: Wrench,
    children: [
      { id: 'engineering-dashboard', title: 'Engineering Dashboard', titleMM: 'အင်ဂျင်နီယာဒက်ရှ်ဘုတ်', icon: BarChart3 },
      { id: 'plastic-engineer', title: 'Plastic Technician', titleMM: 'ပလတ်စတစ်နည်းပညာသမား', icon: Thermometer },
      { id: 'mold-engineer', title: 'Mold Technician', titleMM: 'မှိုနည်းပညာသမား', icon: Layers },
      { id: 'mold-change-tasks', title: 'Mold Change Tasks', titleMM: 'မော်လ်ပြောင်းလဲအလုပ်များ', icon: Settings },
      { id: 'maintenance-engineer', title: 'Maintenance Engineer', titleMM: 'ပြုပြင်ထိန်းသိမ်းမှုအင်ဂျင်နီယာ', icon: Wrench },
      { id: 'technician-head', title: 'Technician Head', titleMM: 'နည်းပညာမှူး', icon: UserCheck },
      { id: 'engineering-reports', title: 'Engineering Reports', titleMM: 'အင်ဂျင်နီယာအစီရင်ခံစာများ', icon: FileText }
    ]
  },
  {
    id: 'inventory',
    title: 'Inventory',
    titleMM: 'စတော့ခ်',
    icon: Package,
    children: [
      { id: 'inventory-dashboard', title: 'Inventory Dashboard', titleMM: 'စတော့ဒက်ရှ်ဘုတ်', icon: BarChart3 },
      { id: 'raw-material-registration', title: 'Raw Material Registration', titleMM: 'ကုန်ကြမ်းအချက်အလက်', icon: Package },
      { id: 'raw-material-warehouse', title: 'Raw Material Warehouse', titleMM: 'ကုန်ကြမ်းသိုလှောင်ရုံ', icon: Package },
      { id: 'warehouse-registration', title: 'Warehouse Registration', titleMM: 'ဂိုဒေါင်မှတ်ပုံတင်', icon: Building2 },
      { id: 'warehouse-routes', title: 'Warehouse Routes', titleMM: 'ဂိုဒေါင်လမ်းကြောင်းများ', icon: ArrowRightLeft },
      { id: 'warehouse-dashboard', title: 'Warehouse Dashboard', titleMM: 'ဂိုဒေါင်ဒက်ရှ်ဘုတ်', icon: BarChart3 },
      { id: 'finished-goods-intake', title: 'Finished Goods Intake', titleMM: 'ကုန်ချောလက်ခံ', icon: QrCode },
      { id: 'borrow-center', title: 'Borrow Center', titleMM: 'ပစ္စည်းချေးယူ', icon: RefreshCw },


      { id: 'inventory-reports-main', title: 'Inventory Reports', titleMM: 'စတော့အစီ���င်ခံစာများ', icon: FileText },
      { id: 'user-permissions-matrix', title: 'User Permissions Matrix', titleMM: 'အသုံးပြုသူအခွင့်အရေးဇယား', icon: UserCheck },

    ]
  },

  {
    id: 'hr-management',
    title: 'HR Management',
    titleMM: 'လူ��ာရင်းစီမံခန့်ခွဲမှု',
    icon: Users,
    children: [
      { id: 'staff-registration', title: 'Staff Registration', titleMM: 'ဝန်ထမ်းမှတ်ပုံတင်' },
      { id: 'department-registration', title: 'Department Registration', titleMM: 'ဌာနစာရင်းသွင်း' },
      { id: 'shift-flexible-setup', title: 'Shift & Flexible Setup', titleMM: 'Shift နှင့် အချိန်ပိုင်းအလုပ် စာရင်းသွင���း' },
      { id: 'shift-work-management', title: 'Shift & Work Management', titleMM: 'အလုပ်ချိန်စီမံခန့်ခွဲမှု' },
      { id: 'attendance-record', title: 'Attendance Record', titleMM: 'တက်ရောက်မှုမှတ်တမ်း' },
      { id: 'hr-reports', title: 'Reports', titleMM: 'အစီရင်ခံစာများ' }
    ]
  },
  {
    id: 'master-data',
    title: 'Master Data & Registration',
    titleMM: 'အခြေခံဒေတာနှင့်မှတ်ပုံတင်မှု',
    icon: Database,
    children: [
      { id: 'machines', title: 'Machine Registration', titleMM: 'စက်မှတ်ပုံတင်မှု', icon: Factory },
      { id: 'users', title: 'User Registration', titleMM: 'အသုံးပြုသူမှတ်ပုံတင်မှု', icon: UserPlus },
      { id: 'molds-tooling', title: 'Mold & Tooling Registration', titleMM: 'ပုံစံနှင့်ကိရိယာမှတ်ပုံတင်မှု', icon: Wrench },
      { id: 'raw-material-product-mapping', title: 'Raw Material ↔ Product Mapping', titleMM: 'ကုန်ကြမ်း–ထုတ်ကုန် ချိတ်ဆက်', icon: ArrowRightLeft },
      { id: 'reason-codes', title: 'Reason Code Registration', titleMM: 'အကြောင်းရင်းကုဒ်မှတ်ပုံတင်မှု', icon: AlertTriangle },
      { id: 'devices', title: 'Device Registration', titleMM: 'စက်ပစ္စည်းမှတ်ပုံတင်မှု', icon: Smartphone },
      { id: 'customers-hq', title: 'Customer & HQ Registration', titleMM: 'ဖောက်သည်နှင့်ရုံးချုပ်မှတ်ပုံတင်မှု', icon: Building },
      { id: 'units-measure', title: 'Unit of Measurement Setup', titleMM: 'တိုင်းတာမှုယူနစ်စနစ်', icon: Scale },
      { id: 'role-matrix', title: 'Role Matrix Management', titleMM: 'အခန်းကဏ္ဍဇယားစီမံခန့်ခွဲမှု', icon: Shield }
    ]
  },
  {
    id: 'logistic',
    title: 'Logistic',
    titleMM: 'လိုဂျစ်တစ်စီမံခန့်ခွဲမှု',
    icon: Truck,
    children: [
      { id: 'logistic-control', title: 'Logistic Control', titleMM: 'လိုဂျစ်တစ်ထိန်းချုပ်မှု' }
    ]
  },
  {
    id: 'reporting',
    title: '📊 Reports',
    titleMM: '📊 အစီရင်ခံစာများ',
    icon: BarChart3,
    children: [
      {
        id: 'production-reports',
        title: '🏭 Production Reports (7)',
        titleMM: '🏭 ထုတ်လုပ်မှုအစီရင်ခံစာများ (7)',
        icon: Factory,
        color: 'text-blue-600',
        bgColor: 'hover:bg-blue-50',
        children: [
          { id: 'daily-production-summary', title: 'Daily Production Summary', titleMM: 'နေ့စဉ် ထုတ်လုပ်မှု အကျဉ်းချုပ်' },
          { id: 'machine-output-log', title: 'Machine Output Log', titleMM: 'စက်ထုတ်လုပ်မှု မှတ်တမ်း' },
          { id: 'qr-code-history', title: 'QR Code History', titleMM: 'QR ကုဒ် သမိုင်း' },
          { id: 'defect-rejection-report', title: 'Defect & Rejection Report', titleMM: 'ချွတ်ယွင်းမှုနှင့် ပယ်ချမှု အစီရင်ခံစာ' },
          { id: 'operator-production-report', title: 'Operator Production Report', titleMM: 'အော်ပရေတာ ထုတ်လုပ်မှု အစီရင်ခံစာ' },
          { id: 'operator-shift-assignment', title: 'Operator Shift Assignment', titleMM: 'အော်ပရေတာ အလုပ်ပတ် သတ်မှတ်မှု' },
          { id: 'operator-productivity-ranking', title: 'Operator Productivity Ranking', titleMM: 'အော်ပရေတာ ကုန်ထုတ်စွမ်းအား အဆင့်သတ်မှတ်မှု' }
        ]
      },
      {
        id: 'qc-reports-main',
        title: '🧪 QC Reports (3)',
        titleMM: '🧪 QC အစီရင်ခံစာများ (3)',
        icon: TestTube,
        color: 'text-indigo-600',
        bgColor: 'hover:bg-indigo-50',
        children: [
          { id: 'qc-inspection-summary', title: 'QC Inspection Summary', titleMM: 'အရည်အသွေးစစ်ဆေးမှု အကျဉ်းချုပ်' },
          { id: 'defect-analysis-report', title: 'Defect Analysis Report', titleMM: 'ချွတ်ယွင်းမှု ခွဲခြမ်းစိတ်ဖြာမှု အစီရင်ခံစာ' },
          { id: 'batch-quality-log', title: 'Batch Quality Log', titleMM: 'အုပ်စု အရည်အသွေး မှတ်တမ်း' }
        ]
      },
      {
        id: 'inventory-reports-main',
        title: '📦 Inventory Reports (9)',
        titleMM: '📦 စတော့အကြောင်းအရာအစီရင်ခံစာများ (9)',
        icon: Package,
        color: 'text-green-600',
        bgColor: 'hover:bg-green-50',
        children: [
          { id: 'raw-material-inout', title: 'Raw Material In/Out Report', titleMM: 'ကုန်ကြမ်း ဝင်/ထွက် အစီရင်ခံစာ' },
          { id: 'finished-goods-movement', title: 'Finished Goods Movement Report', titleMM: 'ပြီးစီးထုတ်ကုန် ရွေ့လျားမှု အစီရင်ခံစာ' },
          { id: 'current-stock-summary', title: 'Current Stock Summary', titleMM: 'လက်ရှိစတော့ခ် အကျဉ်းချုပ်' },
          { id: 'leftover-raw-material', title: 'Leftover Raw Material Summary', titleMM: 'ကျန်ရှိကုန်ကြမ်း အကျဉ်းချုပ်' },
          { id: 'expired-unused-material', title: 'Expired/Unused Material Report', titleMM: 'သက်တမ်းကုန်/အသုံးမပြုကုန်ကြမ်း အစီရင်ခံစာ' },
          { id: 'reserved-available-stock', title: 'Reserved vs Available Stock Report', titleMM: 'ကြိုတင်သိမ်းဆည်းထားသော နှင့် ရရှိနိုင်သော စတော့ခ် အစီရင်ခံစာ' },
          { id: 'borrowing-reallocation-history', title: 'Borrowing/Reallocation History', titleMM: 'ငှားယူ/ပြန်လည်ခွဲဝေမှု သမိုင်း' },
          { id: 'fifo-batch-lot-report', title: 'FIFO/Batch Lot Report', titleMM: 'FIFO/အုပ်စု အစီရ��်ခံစာ' },
          { id: 'dispatch-tracking-report', title: 'Dispatch Tracking Report', titleMM: 'ပို့ဆ��ာင်မှု ခြေရာခံ အစီရင်ခံစာ' }
        ]
      },
      {
        id: 'hr-reports-main',
        title: '👷 HR & Operator Reports (4)',
        titleMM: '👷 HR နှင့် အလုပ်သမားအစီရင်��ံစာများ (4)',
        icon: Users,
        color: 'text-purple-600',
        bgColor: 'hover:bg-purple-50',
        children: [
          { id: 'daily-operator-assignment', title: 'Daily Operator Assignment Sheet', titleMM: 'နေ့စဉ် အော်ပရေတာ တာဝန်ခွဲဝေမှု စာရွက်' },
          { id: 'working-hours-rfid-logs', title: 'Working Hours / RFID Logs', titleMM: 'အလုပ်ချိန် / RFID မှတ်တမ်းများ' },
          { id: 'operator-productivity-report-hr', title: 'Operator Productivity Report', titleMM: 'အော်ပရေတာ ကုန်ထုတ်စွမ်းအား အစီရင်ခံစာ' },
          { id: 'operator-downtime-association', title: 'Operator Downtime Association', titleMM: 'အော်ပရေတာ ရပ်နားချိန် ဆက်စပ်မှု' }
        ]
      },
      {
        id: 'engineering-reports-main',
        title: '⚙ Engineering Reports (3)',
        titleMM: '⚙ အင်ဂျင်နီယာအစီရင်ခံစာများ (3)',
        icon: Cog,
        color: 'text-slate-600',
        bgColor: 'hover:bg-slate-50',
        children: [
          { id: 'machine-efficiency-analysis', title: 'Machine Efficiency Analysis', titleMM: 'စက်စွမ်းဆောင်ရည် ခွဲခြမ်းစိတ်ဖြာမှု' },
          { id: 'process-optimization-report', title: 'Process Optimization Report', titleMM: 'လုပ်ငန်းစဉ် အကောင်းဆုံးဖြစ်အောင်လုပ်မှု အစီရင်ခံစာ' },
          { id: 'technical-documentation', title: 'Technical Documentation', titleMM: 'နည်းပညာဆိုင်ရာ စာရွက်စာတမ်းများ' }
        ]
      },
      {
        id: 'maintenance-reports',
        title: '🛠 Maintenance Reports (4)',
        titleMM: '🛠 ပြုပြင်ထိန်းသိမ်းမှုအစီရင်ခံစာများ (4)',
        icon: Wrench,
        color: 'text-orange-600',
        bgColor: 'hover:bg-orange-50',
        children: [
          { id: 'machine-downtime-log', title: 'Machine Downtime Log', titleMM: 'စက် ရပ်နားချိန် မှတ်တမ်း' },
          { id: 'preventive-maintenance-schedule', title: 'Preventive Maintenance Schedule', titleMM: 'ကြိုတင်ကာကွယ် ပြုပြင်ထိန်းသိမ်းမှု အစီအစဉ်' },
          { id: 'spare-parts-usage-summary', title: 'Spare Parts Usage Summary', titleMM: 'အပိုပစ္စည်းများ အသုံးပြုမှု အကျဉ်းချ���ပ်' },
          { id: 'machine-health-history', title: 'Machine Health History', titleMM: 'စက် ကျန်းမာရေး ��မိုင်း' }
        ]
      },
      {
        id: 'hq-sales-reports',
        title: '💼 HQ & Sales Reports (3)',
        titleMM: '💼 ဌာနချုပ်နှင့်ရောင်းအားအစီရင်ခံစာများ (3)',
        icon: Briefcase,
        color: 'text-pink-600',
        bgColor: 'hover:bg-pink-50',
        children: [
          { id: 'finished-goods-sent-hq', title: 'Finished Goods Sent to HQ', titleMM: 'ရုံးချုပ်သို့ ပို့ဆောင်သော ပြီးစီးထုတ်ကုန်များ' },
          { id: 'dispatch-tracking-report-sales', title: 'Dispatch Tracking Report', titleMM: 'ပို့ဆောင်မှု ခြေရာခံ အစီရင်ခံစာ' },
          { id: 'glue-pellet-sales-report', title: 'Glue Pellet Sales Report', titleMM: 'ကော်ပုဆိုး ရောင်းချမှု အစီရင်ခံစာ' }
        ]
      },
      {
        id: 'executive-analytics-reports',
        title: '📈 Executive / Analytics Reports (4)',
        titleMM: '📈 အထွေထွေခေါင်းဆောင်အဆင့်အစီရင်ခံစာများ (4)',
        icon: LineChart,
        color: 'text-violet-600',
        bgColor: 'hover:bg-violet-50',
        children: [
          { id: 'production-output-chart', title: 'Production Output Chart', titleMM: 'ထုတ်လုပ်မှု ဇယား' },
          { id: 'machine-downtime-analysis', title: 'Machine Downtime Analysis', titleMM: 'စက် ရပ်နားချိန် ခွဲခြမ်းစိတ်ဖြာမှု' },
          { id: 'operator-performance-leaderboard', title: 'Operator Performance Leaderboard', titleMM: 'အော်ပရေတာ စွမ်းဆောင်ရည် ဦးဆောင်ဇယား' },
          { id: 'alerts-notifications-report', title: 'Alerts & Notifications Report', titleMM: 'သတိပေးချက်များနှင့် အကြောင်းကြားချက်များ အစီရင်ခံစာ' }
        ]
      }
    ]
  }
];

export function Layout({ children, currentPage, onPageChange }: LayoutProps) {
  const [sidebarOpen, setSidebarOpen] = useState(true);
  
  // Auto-open sections based on current page
  const getOpenSections = () => {
    const sections = ['dashboard'];
    
    // Auto-open planning section if any planning page is active
    const planningPages = [
      'product-master', 'production-planning', 
      'planning-dashboard', 'planning-reports'
    ];
    
    // Auto-open inventory section if any inventory page is active
    const inventoryPages = [
      'inventory-dashboard', 'raw-material-registration', 'raw-material-warehouse', 'warehouse-registration', 
      'warehouse-routes', 'warehouse-dashboard', 'finished-goods-intake', 'borrow-center', 'inventory-reports-main', 
      'user-permissions-matrix'
    ];
    
    // Auto-open QC section if any QC page is active
    const qcPages = [
      'qc-dashboard', 'raw-material-qc', 'in-process-qc', 
      'finished-goods-qc', 'qc-reports', 'qc-settings', 'qc-entry-form'
    ];
    
    // Auto-open Engineering section if any engineering page is active
    const engineeringPages = [
      'engineering-dashboard', 'plastic-engineer', 'mold-engineer', 
      'maintenance-engineer', 'technician-head', 'engineering-reports'
    ];
    
    // Auto-open Production Control section
    const productionControlPages = [
      'demand-authorization', 'live-schedule', 'material-control', 'job-monitoring',
      'finished-goods-transfer', 'defect-handling', 'split-job-management',
      'scrap-management', 'cut-glue-residue', 'job-tracker'
    ];

    // Auto-open Production section
    const productionPages = [
      'operator-kiosk', 'glue-fill-operator', 'supervisor-screen', 
      'live-monitoring', 'operator-dashboard'
    ];
    
    // Auto-open Master Data section
    const masterDataPages = [
      'machines', 'users', 'molds-tooling', 'raw-material-product-mapping', 'reason-codes', 
      'devices', 'customers-hq', 'units-measure', 'role-matrix'
    ];

    // Auto-open Logistic section
    const logisticPages = [
      'logistic-control'
    ];
    
    if (planningPages.includes(currentPage)) sections.push('planning');
    if (inventoryPages.includes(currentPage)) sections.push('inventory');
    if (qcPages.includes(currentPage)) sections.push('qc');
    if (engineeringPages.includes(currentPage)) sections.push('engineering');
    if (productionControlPages.includes(currentPage)) sections.push('production-control');
    if (productionPages.includes(currentPage)) sections.push('production');
    if (masterDataPages.includes(currentPage)) sections.push('master-data');
    if (logisticPages.includes(currentPage)) sections.push('logistic');
    
    return sections;
  };
  
  const [openSections, setOpenSections] = useState<string[]>(getOpenSections());

  // Update open sections when currentPage changes
  React.useEffect(() => {
    setOpenSections(getOpenSections());
  }, [currentPage]);

  const toggleSection = (sectionId: string) => {
    setOpenSections(prev => 
      prev.includes(sectionId) 
        ? prev.filter(id => id !== sectionId)
        : [...prev, sectionId]
    );
  };

  // Enhanced function to handle nested children rendering
  const renderNestedChildren = (children: any[], depth: number = 0) => {
    return children.map((child: any) => {
      const hasNestedChildren = child.children && child.children.length > 0;
      const ChildIcon = child.icon;
      const paddingLeft = depth === 0 ? 'pl-6' : 'pl-10';
      
      if (hasNestedChildren) {
        const isOpen = openSections.includes(child.id);
        return (
          <div key={child.id}>
            <Collapsible open={isOpen} onOpenChange={() => toggleSection(child.id)}>
              <CollapsibleTrigger asChild>
                <Button
                  variant="ghost"
                  className={`w-full justify-start p-2 h-auto text-sm ${paddingLeft} ${child.bgColor || 'hover:bg-slate-50'} rounded-lg mb-1`}
                >
                  {ChildIcon && <ChildIcon className={`h-4 w-4 mr-2 shrink-0 ${child.color || ''}`} />}
                  <div className="flex-1 text-left">
                    <div className={`${child.color || ''} font-medium`}>{child.title}</div>
                    <div className="text-xs text-slate-500">{child.titleMM}</div>
                  </div>
                  {isOpen ? (
                    <ChevronDown className="h-4 w-4" />
                  ) : (
                    <ChevronRight className="h-4 w-4" />
                  )}
                </Button>
              </CollapsibleTrigger>
              <CollapsibleContent className="space-y-1">
                {renderNestedChildren(child.children, depth + 1)}
              </CollapsibleContent>
            </Collapsible>
          </div>
        );
      } else {
        return (
          <Button
            key={child.id}
            variant="ghost"
            className={`w-full justify-start p-2 h-auto text-sm ${
              depth === 0 ? 'pl-6' : 'pl-12'
            } ${
              currentPage === child.id 
                ? 'bg-blue-50 text-blue-700 border-l-2 border-blue-500'
                : 'text-slate-600 hover:text-slate-900 hover:bg-slate-50'
            } rounded-lg mb-1`}
            onClick={() => onPageChange(child.id)}
          >
            {ChildIcon && <ChildIcon className="h-4 w-4 mr-2 shrink-0" />}
            <div className="text-left">
              <div>{child.title}</div>
              <div className="text-xs text-slate-500">{child.titleMM}</div>
            </div>
          </Button>
        );
      }
    });
  };

  return (
    <div className="min-h-screen bg-slate-50 flex">
      {/* Sidebar */}
      <div className={`${sidebarOpen ? 'w-80' : 'w-16'} transition-all duration-300 bg-white border-r border-slate-200 flex flex-col`}>
        {/* Header */}
        <div className="p-4 border-b border-slate-200 flex items-center justify-between">
          {sidebarOpen && (
            <div>
              <h1 className="text-lg font-semibold text-slate-900">Smart Plastic Factory</h1>
              <p className="text-sm text-slate-600">Management System</p>
            </div>
          )}
          <Button
            variant="ghost"
            size="sm"
            onClick={() => setSidebarOpen(!sidebarOpen)}
            className="p-2"
          >
            {sidebarOpen ? <X className="h-4 w-4" /> : <Menu className="h-4 w-4" />}
          </Button>
        </div>

        {/* Navigation */}
        <ScrollArea className="flex-1 p-2">
          <nav className="space-y-1">
            {navigationItems.map((item) => {
              const hasChildren = item.children.length > 0;
              const isOpen = openSections.includes(item.id);
              
              return (
                <div key={item.id}>
                  {hasChildren ? (
                    <Collapsible open={isOpen} onOpenChange={() => toggleSection(item.id)}>
                      <CollapsibleTrigger asChild>
                        <Button
                          variant="ghost"
                          className={`w-full justify-start p-3 h-auto ${
                            sidebarOpen ? 'px-3' : 'px-2'
                          }`}
                        >
                          <item.icon className="h-5 w-5 shrink-0" />
                          {sidebarOpen && (
                            <>
                              <div className="ml-3 flex-1 text-left">
                                <div className="text-sm">{item.title}</div>
                                <div className="text-xs text-slate-500">{item.titleMM}</div>
                              </div>
                              {isOpen ? (
                                <ChevronDown className="h-4 w-4" />
                              ) : (
                                <ChevronRight className="h-4 w-4" />
                              )}
                            </>
                          )}
                        </Button>
                      </CollapsibleTrigger>
                      {sidebarOpen && (
                        <CollapsibleContent className="space-y-1">
                          {renderNestedChildren(item.children)}
                        </CollapsibleContent>
                      )}
                    </Collapsible>
                  ) : (
                    <Button
                      variant="ghost"
                      className={`w-full justify-start p-3 h-auto ${
                        sidebarOpen ? 'px-3' : 'px-2'
                      } ${
                        currentPage === item.id 
                          ? 'bg-blue-50 text-blue-700 border-l-2 border-blue-500'
                          : 'text-slate-600 hover:text-slate-900'
                      }`}
                      onClick={() => onPageChange(item.id)}
                    >
                      <item.icon className="h-5 w-5 shrink-0" />
                      {sidebarOpen && (
                        <div className="ml-3 text-left">
                          <div className="text-sm">{item.title}</div>
                          <div className="text-xs text-slate-500">{item.titleMM}</div>
                        </div>
                      )}
                    </Button>
                  )}
                </div>
              );
            })}
          </nav>
        </ScrollArea>
      </div>

      {/* Main Content */}
      <div className="flex-1 overflow-hidden">
        {children}
      </div>
    </div>
  );
}