import React, { useState, useMemo } from 'react';
import {
  Factory,
  Package,
  Users,
  Wrench,
  Briefcase,
  BarChart3,
  Calendar,
  Filter,
  Download,
  FileText,
  FileSpreadsheet,
  Clock,
  TrendingUp,
  TrendingDown,
  AlertTriangle,
  CheckCircle,
  XCircle,
  Target,
  Shield,
  ClipboardList,
  BarChart,
  PieChart,
  LineChart,
  TestTube,
  Cog,
  Building2,
  Printer,
  ArrowUpRight,
  ArrowDownRight,
  User
} from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Progress } from './ui/progress';
import { 
  BarChart as RechartsBarChart, 
  Bar, 
  XAxis, 
  YAxis, 
  CartesianGrid, 
  Tooltip, 
  ResponsiveContainer,
  PieChart as RechartsPieChart,
  Cell,
  Pie,
  LineChart as RechartsLineChart,
  Line,
  Area,
  AreaChart as RechartsAreaChart
} from 'recharts';

interface AdvancedReportingProps {
  currentPage: string;
  onPageChange: (page: string) => void;
}

// Report metadata mapping
const reportMetadata: Record<string, {
  title: string;
  titleMM: string;
  category: string;
  categoryMM: string;
  roleTag: string;
  roleColor: string;
  type: 'analytics' | 'summary';
  icon: any;
}> = {
  // Production Reports
  'daily-production-summary': {
    title: 'Daily Production Summary',
    titleMM: 'နေ့စဉ်ထုတ်လုပ်မှု အကျဉ်းချုပ်',
    category: 'Production Reports',
    categoryMM: 'ထုတ်လုပ်မှုအစီရင်ခံစာများ',
    roleTag: 'Supervisor',
    roleColor: 'bg-green-100 text-green-800',
    type: 'analytics',
    icon: Factory
  },
  'machine-output-log': {
    title: 'Machine Output Log',
    titleMM: 'စက်ထုတ်လုပ်မှု မှတ်တမ်း',
    category: 'Production Reports',
    categoryMM: 'ထုတ်လုပ်မှုအစီရင်ခံစာများ',
    roleTag: 'Planner',
    roleColor: 'bg-blue-100 text-blue-800',
    type: 'summary',
    icon: Factory
  },
  'qr-code-history': {
    title: 'QR Code History',
    titleMM: 'QR ကုဒ်မှတ်တမ်း',
    category: 'Production Reports',
    categoryMM: 'ထုတ်လုပ်မှုအစီရင်ခံစာများ',
    roleTag: 'Operator',
    roleColor: 'bg-purple-100 text-purple-800',
    type: 'summary',
    icon: Factory
  },
  'defect-rejection-report': {
    title: 'Defect & Rejection Report',
    titleMM: 'ချို့တဲ့မှု & ငြင်းပယ်မှု အစီရင်ခံစာ',
    category: 'Production Reports',
    categoryMM: 'ထုတ်လုပ်မှုအစီရင်ခံစာများ',
    roleTag: 'QC Inspector',
    roleColor: 'bg-orange-100 text-orange-800',
    type: 'analytics',
    icon: Factory
  },
  'operator-production-report': {
    title: 'Operator Production Report',
    titleMM: 'အလုပ်သမားထုတ်လုပ်မှု အစီရင်ခံစာ',
    category: 'Production Reports',
    categoryMM: 'ထုတ်လုပ်မှုအစီရင်ခံစာများ',
    roleTag: 'Supervisor',
    roleColor: 'bg-green-100 text-green-800',
    type: 'summary',
    icon: Factory
  },
  'operator-shift-assignment': {
    title: 'Operator Shift Assignment',
    titleMM: 'အလုပ်သမားအလှည့်အချိန် ချမှတ်ချက်',
    category: 'Production Reports',
    categoryMM: 'ထုတ်လုပ်မှုအစီရင်ခံစာများ',
    roleTag: 'HR Manager',
    roleColor: 'bg-indigo-100 text-indigo-800',
    type: 'summary',
    icon: Factory
  },
  'operator-productivity-ranking': {
    title: 'Operator Productivity Ranking',
    titleMM: 'အလုပ်သမား ထုတ်လုပ်မှု အဆင့်ထား',
    category: 'Production Reports',
    categoryMM: 'ထုတ်လုပ်မှုအစီရင်ခံစာများ',
    roleTag: 'Admin Only',
    roleColor: 'bg-red-100 text-red-800',
    type: 'analytics',
    icon: Factory
  },

  // Inventory Reports
  'raw-material-inout': {
    title: 'Raw Material In/Out Report',
    titleMM: 'ကုန်ကြမ်း ဝင်/ထွက် အစီရင်ခံစာ',
    category: 'Inventory Reports',
    categoryMM: 'စတော့အချက်အလက် အစီရင်ခံစာများ',
    roleTag: 'Inventory',
    roleColor: 'bg-teal-100 text-teal-800',
    type: 'summary',
    icon: Package
  },
  'finished-goods-movement': {
    title: 'Finished Goods Movement Report',
    titleMM: 'အပြီးပစ္စည်း အရွေ့အပြောင်း အစီရင်ခံစာ',
    category: 'Inventory Reports',
    categoryMM: 'စတော့အချက်အလက် အစီရင်ခံစာများ',
    roleTag: 'Warehouse',
    roleColor: 'bg-cyan-100 text-cyan-800',
    type: 'summary',
    icon: Package
  },
  'current-stock-summary': {
    title: 'Stock Balance Report',
    titleMM: 'စတော့လက်ကျန် အစီရင်ခံစာ',
    category: 'Inventory Reports',
    categoryMM: 'စတော့အချက်အလက် အစီရင်ခံစာများ',
    roleTag: 'Inventory',
    roleColor: 'bg-teal-100 text-teal-800',
    type: 'analytics',
    icon: Package
  },
  'leftover-raw-material': {
    title: 'Leftover Raw Material Summary',
    titleMM: 'ကျန်ကုန်ကြမ်း အကျဉ်းချုပ်',
    category: 'Inventory Reports',
    categoryMM: 'စတော့အချက်အလက် အစီရင်ခံစာများ',
    roleTag: 'Planner',
    roleColor: 'bg-blue-100 text-blue-800',
    type: 'summary',
    icon: Package
  },
  'expired-unused-material': {
    title: 'Expired/Unused Material Report',
    titleMM: 'သက်တမ်းကုန် & မသုံးစွဲ ကုန်ကြမ်း အစီရင်ခံစာ',
    category: 'Inventory Reports',
    categoryMM: 'စတော့အချက်အလက် အစီရင်ခံစာများ',
    roleTag: 'Inventory',
    roleColor: 'bg-teal-100 text-teal-800',
    type: 'summary',
    icon: Package
  },
  'reserved-available-stock': {
    title: 'Reserved vs Available Stock Report',
    titleMM: 'ကြိုတင်ယူထားမှုနှင့် ရရှိနိုင်မှု အစီရင်ခံစာ',
    category: 'Inventory Reports',
    categoryMM: 'စတော့အချက်အ��က် အစီရင်ခံစာများ',
    roleTag: 'Planner',
    roleColor: 'bg-blue-100 text-blue-800',
    type: 'analytics',
    icon: Package
  },
  'borrowing-reallocation-history': {
    title: 'Borrowing/Reallocation History',
    titleMM: 'ငှားယူ & ပြန်လည်ဖြန့်ဝေ မှတ်တမ်း',
    category: 'Inventory Reports',
    categoryMM: 'စတော့အချက်အလက် အစီရင်ခံစာများ',
    roleTag: 'Supervisor',
    roleColor: 'bg-green-100 text-green-800',
    type: 'summary',
    icon: Package
  },
  'fifo-batch-lot-report': {
    title: 'FIFO/Batch Lot Report',
    titleMM: 'FIFO & အစုလိုက် အစီရင်ခံစာ',
    category: 'Inventory Reports',
    categoryMM: 'စတော့အချက်အလက် အစီရင်ခံစာများ',
    roleTag: 'Inventory',
    roleColor: 'bg-teal-100 text-teal-800',
    type: 'summary',
    icon: Package
  },
  'dispatch-tracking-report': {
    title: 'Dispatch Tracking Report',
    titleMM: 'ပစ္စည်းပို့ဆောင်မှု အစီရင်ခံစာ',
    category: 'Inventory Reports',
    categoryMM: 'စတော့အချက်အလက် အစီရင်ခံစာများ',
    roleTag: 'Logistics',
    roleColor: 'bg-amber-100 text-amber-800',
    type: 'summary',
    icon: Package
  },

  // HR & Operator Reports
  'daily-operator-assignment': {
    title: 'Daily Operator Assignment Sheet',
    titleMM: 'နေ့စဉ် အလုပ်သမား ချမှတ်စာရွက်',
    category: 'HR & Operator Reports',
    categoryMM: 'HR & အလုပ်သမား အစီရင်ခံစာများ',
    roleTag: 'HR Manager',
    roleColor: 'bg-indigo-100 text-indigo-800',
    type: 'summary',
    icon: Users
  },
  'working-hours-rfid-logs': {
    title: 'Working Hours / RFID Logs',
    titleMM: 'အလုပ်ချိန် & RFID မှတ်တမ်း',
    category: 'HR & Operator Reports',
    categoryMM: 'HR & အလုပ်သမား အစီရင်ခံစာများ',
    roleTag: 'HR Manager',
    roleColor: 'bg-indigo-100 text-indigo-800',
    type: 'summary',
    icon: Users
  },
  'operator-productivity-report-hr': {
    title: 'Operator Productivity Report',
    titleMM: 'အလုပ်သမား ထုတ်လုပ်မှု အစီရင်ခံစာ',
    category: 'HR & Operator Reports',
    categoryMM: 'HR & အလုပ်သမား အစီရင်ခံစာများ',
    roleTag: 'Supervisor',
    roleColor: 'bg-green-100 text-green-800',
    type: 'analytics',
    icon: Users
  },
  'operator-downtime-association': {
    title: 'Operator Downtime Association',
    titleMM: 'အလုပ်သမား အချိန်ပျက်မှု ဆက်စပ်မှု',
    category: 'HR & Operator Reports',
    categoryMM: 'HR & အလုပ်သမား အစီရင်ခံစာများ',
    roleTag: 'Admin Only',
    roleColor: 'bg-red-100 text-red-800',
    type: 'analytics',
    icon: Users
  },

  // Maintenance Reports
  'machine-downtime-log': {
    title: 'Machine Downtime Log',
    titleMM: 'စက်ရပ်ဆိုင်းမှု မှတ်တမ်း',
    category: 'Maintenance Reports',
    categoryMM: 'ပြုပြင်ထိန်းသိမ်းမှု အစီရင်ခံစာများ',
    roleTag: 'Maintenance',
    roleColor: 'bg-orange-100 text-orange-800',
    type: 'summary',
    icon: Wrench
  },
  'preventive-maintenance-schedule': {
    title: 'Preventive Maintenance Schedule',
    titleMM: 'ကြိုတင် ပြုပြင်ထိန်းသိမ်းမှု အစီရင်ခံစာ',
    category: 'Maintenance Reports',
    categoryMM: 'ပြုပြင်ထိန်းသိမ်းမှု အစီရင်ခံစာများ',
    roleTag: 'Maintenance',
    roleColor: 'bg-orange-100 text-orange-800',
    type: 'summary',
    icon: Wrench
  },
  'spare-parts-usage-summary': {
    title: 'Spare Parts Usage Summary',
    titleMM: 'အပိုပစ္စည်း အသုံးပြုမှု အကျဉ်းချုပ်',
    category: 'Maintenance Reports',
    categoryMM: 'ပြုပြင်ထိန်းသိမ်းမှု အစီရင်ခံစာများ',
    roleTag: 'Maintenance',
    roleColor: 'bg-orange-100 text-orange-800',
    type: 'analytics',
    icon: Wrench
  },
  'machine-health-history': {
    title: 'Machine Health History',
    titleMM: 'စက်ပစ္စည်း သမိုင်း & အခြေအနေ',
    category: 'Maintenance Reports',
    categoryMM: 'ပြုပြင်ထိန်းသိမ်းမှု အစီရင်ခံစာများ',
    roleTag: 'Engineering',
    roleColor: 'bg-slate-100 text-slate-800',
    type: 'analytics',
    icon: Wrench
  },

  // QC Reports
  'qc-inspection-summary': {
    title: 'QC Inspection Summary',
    titleMM: 'အရည်အသွေးစစ်ဆေးမှု အကျဉ်းချုပ်',
    category: 'QC Reports',
    categoryMM: 'အရည်အသွေးထိန်းချုပ်မှု အစီရင်ခံစာများ',
    roleTag: 'QC Inspector',
    roleColor: 'bg-orange-100 text-orange-800',
    type: 'analytics',
    icon: TestTube
  },
  'defect-analysis-report': {
    title: 'Defect Analysis Report',
    titleMM: 'ချွတ်ယွင်းမှု ခွဲခြမ်းစိတ်ဖြာမှု အစီရင်ခံစာ',
    category: 'QC Reports',
    categoryMM: 'အရည်အသွေးထိန်းချုပ်မှု အစီရင်ခံစာများ',
    roleTag: 'QC Inspector',
    roleColor: 'bg-orange-100 text-orange-800',
    type: 'analytics',
    icon: TestTube
  },
  'batch-quality-log': {
    title: 'Batch Quality Log',
    titleMM: 'အုပ်စု အရည်အသွေး မှတ်တမ်း',
    category: 'QC Reports',
    categoryMM: 'အရည်အသွေးထိန်းချုပ်မှု အစီရင်ခံစာများ',
    roleTag: 'QC Inspector',
    roleColor: 'bg-orange-100 text-orange-800',
    type: 'summary',
    icon: TestTube
  },

  // Engineering Reports
  'machine-efficiency-analysis': {
    title: 'Machine Efficiency Analysis',
    titleMM: 'စက်စွမ်းဆောင်ရည် ခွဲခြမ်းစိတ်ဖြာမှု',
    category: 'Engineering Reports',
    categoryMM: 'အင်ဂျင်နီယာ အစီရင်ခံစာများ',
    roleTag: 'Engineering',
    roleColor: 'bg-slate-100 text-slate-800',
    type: 'analytics',
    icon: Cog
  },
  'process-optimization-report': {
    title: 'Process Optimization Report',
    titleMM: 'လုပ်ငန်းစဉ် အကောင်းဆုံးဖြစ်အောင်လုပ်မှု အစီရင်ခံစာ',
    category: 'Engineering Reports',
    categoryMM: 'အင်ဂျင်နီယာ အစီရင်ခံစာများ',
    roleTag: 'Engineering',
    roleColor: 'bg-slate-100 text-slate-800',
    type: 'analytics',
    icon: Cog
  },
  'technical-documentation': {
    title: 'Technical Documentation',
    titleMM: 'နည်းပညာဆိုင်ရာ စာရွက်စာတမ်းများ',
    category: 'Engineering Reports',
    categoryMM: 'အင်ဂျင်နီယာ အစီရင်ခံစာများ',
    roleTag: 'Engineering',
    roleColor: 'bg-slate-100 text-slate-800',
    type: 'summary',
    icon: Cog
  },

  // Sales & HQ Reports
  'finished-goods-sent-hq': {
    title: 'Finished Goods Sent to HQ',
    titleMM: 'ရုံးချုပ်သို့ ပို့ဆောင်သော ပြီးစီးထုတ်ကုန်များ',
    category: 'Sales & HQ Reports',
    categoryMM: 'ရောင်းအား & ဌာနချုပ် အစီရင်ခံစာများ',
    roleTag: 'Logistics',
    roleColor: 'bg-amber-100 text-amber-800',
    type: 'summary',
    icon: Briefcase
  },
  'dispatch-tracking-report-sales': {
    title: 'Dispatch Tracking Report',
    titleMM: 'ပို့ဆောင်မှု ခြေရာခံ အစီရင်ခံစာ',
    category: 'Sales & HQ Reports',
    categoryMM: 'ရောင်းအား & ဌာနချုပ် အစီရင်ခံစာများ',
    roleTag: 'Logistics',
    roleColor: 'bg-amber-100 text-amber-800',
    type: 'summary',
    icon: Briefcase
  },
  'glue-pellet-sales-report': {
    title: 'Glue Pellet Sales Report',
    titleMM: 'ကော်ပုဆိုး ရောင်းချမှု အစီရင်ခံစာ',
    category: 'Sales & HQ Reports',
    categoryMM: 'ရောင်းအား & ဌာနချုပ် အစီရင်ခံစာများ',
    roleTag: 'Sales Manager',
    roleColor: 'bg-pink-100 text-pink-800',
    type: 'analytics',
    icon: Briefcase
  },

  // Executive / Analytics Reports
  'production-output-chart': {
    title: 'Production Output Chart',
    titleMM: 'ထုတ်လုပ်မှု အကျိုးသက်ရောက်မှု ချတ်',
    category: 'Executive / Analytics Reports',
    categoryMM: 'အထွေထွေခေါင်းဆောင် & အချက်အလက်သုံးသပ်မှု အစီရင်ခံစာများ',
    roleTag: 'Admin Only',
    roleColor: 'bg-red-100 text-red-800',
    type: 'analytics',
    icon: BarChart3
  },
  'machine-downtime-analysis': {
    title: 'Machine Downtime Analysis',
    titleMM: 'စက်ရပ်ဆိုင်းမှု ချက်ခြားစိစစ်မှု',
    category: 'Executive / Analytics Reports',
    categoryMM: 'အထွေထွေခေါင်းဆောင် & အချက်အလက်သုံးသပ်မှု အစီရင်ခံစာများ',
    roleTag: 'Engineering',
    roleColor: 'bg-slate-100 text-slate-800',
    type: 'analytics',
    icon: BarChart3
  },
  'operator-performance-leaderboard': {
    title: 'Operator Performance Leaderboard',
    titleMM: 'အလုပ်သမား စွမ်းဆောင်ရည် စာရင်းဇယား',
    category: 'Executive / Analytics Reports',
    categoryMM: 'အထွေထွေခေါင်းဆောင် & အချက်အလက်သုံးသပ်မှု အစီရင်ခံစာများ',
    roleTag: 'Admin Only',
    roleColor: 'bg-red-100 text-red-800',
    type: 'analytics',
    icon: BarChart3
  },
  'alerts-notifications-report': {
    title: 'Alerts & Notifications Report',
    titleMM: 'သတိပေးချက် & အသိပေးချက် အစီရင်ခံစာ',
    category: 'Executive / Analytics Reports',
    categoryMM: 'အထွေထွေခေါင်းဆောင် & အချက်အလက်သုံးသပ်မှု အစီရင်ခံစာများ',
    roleTag: 'Admin Only',
    roleColor: 'bg-red-100 text-red-800',
    type: 'summary',
    icon: BarChart3
  }
};

export function AdvancedReporting({ currentPage, onPageChange }: AdvancedReportingProps) {
  const [languageToggle, setLanguageToggle] = useState<'en' | 'mm'>('en');
  const [dateRange, setDateRange] = useState('today');

  // Sample data for charts and tables
  const productionData = [
    { name: 'MCH-001', target: 1200, actual: 1150, efficiency: 95.8 },
    { name: 'MCH-002', target: 800, actual: 720, efficiency: 90.0 },
    { name: 'MCH-003', target: 1000, actual: 1080, efficiency: 108.0 },
    { name: 'MCH-004', target: 1500, actual: 1350, efficiency: 90.0 },
  ];

  const downtimeData = [
    { reason: 'Material Shortage', minutes: 120, percentage: 35 },
    { reason: 'Machine Breakdown', minutes: 80, percentage: 23 },
    { reason: 'Quality Issues', minutes: 60, percentage: 17 },
    { reason: 'Changeover', minutes: 45, percentage: 13 },
    { reason: 'Other', minutes: 40, percentage: 12 },
  ];

  const operatorProductivity = [
    { name: 'Ko Zaw Min', efficiency: 118, target: 100, actual: 118, rank: 1 },
    { name: 'Ma Thandar', efficiency: 115, target: 100, actual: 115, rank: 2 },
    { name: 'Ko Aung Aung', efficiency: 108, target: 100, actual: 108, rank: 3 },
    { name: 'Ma Phyu Phyu', efficiency: 102, target: 100, actual: 102, rank: 4 },
    { name: 'Ko Min Thu', efficiency: 95, target: 100, actual: 95, rank: 5 },
  ];

  const inventoryData = [
    { item: 'Plastic Pellets Type A', opening: 2800, received: 500, issued: 800, closing: 2500, status: 'Normal' },
    { item: 'Plastic Containers Type A', opening: 1800, received: 475, issued: 275, closing: 2000, status: 'High' },
    { item: 'Cardboard Boxes', opening: 600, received: 200, issued: 300, closing: 500, status: 'Low' },
    { item: 'Adhesive Labels', opening: 1200, received: 0, issued: 150, closing: 1050, status: 'Normal' },
  ];

  const COLORS = ['#0088FE', '#00C49F', '#FFBB28', '#FF8042', '#8884D8'];

  // Get current report metadata
  const currentReport = reportMetadata[currentPage] || {
    title: 'Report',
    titleMM: 'အစီရင်ခံစာ',
    category: 'General',
    categoryMM: 'ယေဘုယျ',
    roleTag: 'User',
    roleColor: 'bg-gray-100 text-gray-800',
    type: 'summary' as const,
    icon: FileText
  };

  const getRoleIcon = (roleTag: string) => {
    if (roleTag.includes('Admin')) return '🔐';
    if (roleTag.includes('Supervisor')) return '✅';
    if (roleTag.includes('Manager')) return '👨‍💼';
    if (roleTag.includes('Inspector')) return '🔍';
    if (roleTag.includes('Engineering')) return '⚙️';
    if (roleTag.includes('Maintenance')) return '🔧';
    if (roleTag.includes('Operator')) return '👷';
    if (roleTag.includes('Planner')) return '📋';
    if (roleTag.includes('Inventory')) return '📦';
    if (roleTag.includes('Warehouse')) return '🏪';
    if (roleTag.includes('Logistics')) return '🚛';
    if (roleTag.includes('Sales')) return '💼';
    return '👤';
  };

  const getRoleTagBadge = (roleTag: string, roleColor: string) => (
    <Badge className={`${roleColor} text-sm`}>
      <span className="mr-1">{getRoleIcon(roleTag)}</span>
      {roleTag}
    </Badge>
  );

  const renderFilters = () => (
    <Card className="mb-6">
      <CardHeader className="pb-3">
        <CardTitle className="flex items-center gap-2">
          <Filter className="h-5 w-5" />
          {languageToggle === 'en' ? 'Report Filters' : 'အစီရင်ခံစာ စစ်ထုတ်မှုများ'}
        </CardTitle>
      </CardHeader>
      <CardContent>
        <div className="grid grid-cols-1 md:grid-cols-5 gap-4">
          <div>
            <label className="text-sm font-medium mb-2 block">
              {languageToggle === 'en' ? 'Date Range' : 'ရက်စွဲအပိုင်းအခြား'}
            </label>
            <Select value={dateRange} onValueChange={setDateRange}>
              <SelectTrigger>
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="today">
                  {languageToggle === 'en' ? 'Today' : 'ဒီနေ့'}
                </SelectItem>
                <SelectItem value="week">
                  {languageToggle === 'en' ? 'This Week' : 'ဒီအပတ်'}
                </SelectItem>
                <SelectItem value="month">
                  {languageToggle === 'en' ? 'This Month' : 'ဒီလ'}
                </SelectItem>
                <SelectItem value="quarter">
                  {languageToggle === 'en' ? 'This Quarter' : 'ဒီသုံးလ'}
                </SelectItem>
                <SelectItem value="custom">
                  {languageToggle === 'en' ? 'Custom Range' : 'စိတ်ကြိုက်သတ်မှတ်မှု'}
                </SelectItem>
              </SelectContent>
            </Select>
          </div>

          <div>
            <label className="text-sm font-medium mb-2 block">
              {languageToggle === 'en' ? 'Machine' : 'စက်'}
            </label>
            <Select>
              <SelectTrigger>
                <SelectValue placeholder={languageToggle === 'en' ? 'All Machines' : 'စက်အားလုံး'} />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">
                  {languageToggle === 'en' ? 'All Machines' : 'စက်အားလုံး'}
                </SelectItem>
                <SelectItem value="MCH-001">MCH-001 - Injection Molding</SelectItem>
                <SelectItem value="MCH-002">MCH-002 - Extrusion Line</SelectItem>
                <SelectItem value="MCH-003">MCH-003 - Cutting Machine</SelectItem>
              </SelectContent>
            </Select>
          </div>

          <div>
            <label className="text-sm font-medium mb-2 block">
              {languageToggle === 'en' ? 'Product' : 'ထုတ်ကုန်'}
            </label>
            <Select>
              <SelectTrigger>
                <SelectValue placeholder={languageToggle === 'en' ? 'All Products' : 'ထုတ်ကုန်အားလုံး'} />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">
                  {languageToggle === 'en' ? 'All Products' : 'ထုတ်ကုန်အားလုံး'}
                </SelectItem>
                <SelectItem value="PRD-001">PRD-001 - Plastic Bottle 500ml</SelectItem>
                <SelectItem value="PRD-002">PRD-002 - Food Container Large</SelectItem>
              </SelectContent>
            </Select>
          </div>

          <div>
            <label className="text-sm font-medium mb-2 block">
              {languageToggle === 'en' ? 'Operator' : 'အော်ပရေတာ'}
            </label>
            <Select>
              <SelectTrigger>
                <SelectValue placeholder={languageToggle === 'en' ? 'All Operators' : 'အော်ပရေတာအားလုံး'} />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">
                  {languageToggle === 'en' ? 'All Operators' : 'အော်ပရေတာအားလုံး'}
                </SelectItem>
                <SelectItem value="OP-001">Ko Zaw Min</SelectItem>
                <SelectItem value="OP-002">Ma Thandar</SelectItem>
                <SelectItem value="OP-003">Ko Aung Aung</SelectItem>
              </SelectContent>
            </Select>
          </div>

          <div>
            <label className="text-sm font-medium mb-2 block">
              {languageToggle === 'en' ? 'Warehouse' : 'ကုန်လှောင်ရုံ'}
            </label>
            <Select>
              <SelectTrigger>
                <SelectValue placeholder={languageToggle === 'en' ? 'All Warehouses' : 'ကုန်လှောင်ရုံအားလုံး'} />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">
                  {languageToggle === 'en' ? 'All Warehouses' : 'ကုန်လှောင်ရုံအားလုံး'}
                </SelectItem>
                <SelectItem value="RM-WH">RM-WH - Raw Material</SelectItem>
                <SelectItem value="FG-WH">FG-WH - Finished Goods</SelectItem>
                <SelectItem value="EXT-WH">EXT-WH - External</SelectItem>
              </SelectContent>
            </Select>
          </div>
        </div>
      </CardContent>
    </Card>
  );

  const renderAnalyticsContent = () => (
    <div className="space-y-6">
      {/* Summary Cards */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-muted-foreground">
                  {languageToggle === 'en' ? 'Total Production' : 'စုစုပေါင်းထုတ်လုပ်မှု'}
                </p>
                <p className="text-2xl font-bold text-green-600">4,300</p>
                <p className="text-xs text-green-600 flex items-center gap-1">
                  <ArrowUpRight className="h-3 w-3" />
                  +12% vs target
                </p>
              </div>
              <Target className="h-8 w-8 text-green-600" />
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-muted-foreground">
                  {languageToggle === 'en' ? 'Efficiency Rate' : 'စွမ်းဆောင်မှုနှုန်း'}
                </p>
                <p className="text-2xl font-bold text-blue-600">95.8%</p>
                <p className="text-xs text-blue-600 flex items-center gap-1">
                  <ArrowUpRight className="h-3 w-3" />
                  +2.3% vs yesterday
                </p>
              </div>
              <TrendingUp className="h-8 w-8 text-blue-600" />
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-muted-foreground">
                  {languageToggle === 'en' ? 'Quality Rate' : 'အရည်အသွေးနှုန်း'}
                </p>
                <p className="text-2xl font-bold text-emerald-600">97.9%</p>
                <p className="text-xs text-emerald-600 flex items-center gap-1">
                  <ArrowUpRight className="h-3 w-3" />
                  +0.5% vs yesterday
                </p>
              </div>
              <CheckCircle className="h-8 w-8 text-emerald-600" />
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-muted-foreground">
                  {languageToggle === 'en' ? 'Downtime' : 'ရပ်နားချိန်'}
                </p>
                <p className="text-2xl font-bold text-orange-600">45min</p>
                <p className="text-xs text-orange-600">3 incidents</p>
              </div>
              <Clock className="h-8 w-8 text-orange-600" />
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Charts */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <BarChart className="h-5 w-5" />
              {languageToggle === 'en' ? 'Machine Performance' : 'စက်စွမ်းဆောင်ရည်'}
            </CardTitle>
          </CardHeader>
          <CardContent>
            <ResponsiveContainer width="100%" height={300}>
              <RechartsBarChart data={productionData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="name" />
                <YAxis />
                <Tooltip />
                <Bar dataKey="target" fill="#e2e8f0" name="Target" />
                <Bar dataKey="actual" fill="#3b82f6" name="Actual" />
              </RechartsBarChart>
            </ResponsiveContainer>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <PieChart className="h-5 w-5" />
              {languageToggle === 'en' ? 'Downtime Reasons' : 'ရပ်နားချိန်အကြောင်းရင်းများ'}
            </CardTitle>
          </CardHeader>
          <CardContent>
            <ResponsiveContainer width="100%" height={300}>
              <RechartsPieChart>
                <Pie
                  data={downtimeData}
                  cx="50%"
                  cy="50%"
                  labelLine={false}
                  label={({ name, percentage }) => `${name} ${percentage}%`}
                  outerRadius={80}
                  fill="#8884d8"
                  dataKey="percentage"
                >
                  {downtimeData.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                  ))}
                </Pie>
                <Tooltip />
              </RechartsPieChart>
            </ResponsiveContainer>
          </CardContent>
        </Card>
      </div>
    </div>
  );

  const renderSummaryContent = () => (
    <div className="space-y-6">
      {/* Summary Table */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <ClipboardList className="h-5 w-5" />
            {languageToggle === 'en' ? 'Detailed Report Data' : 'အသေးစိတ်အစီရင်ခံစာဒေတာ'}
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  {currentReport.category.includes('Inventory') ? (
                    <>
                      <TableHead>{languageToggle === 'en' ? 'Item' : 'ပစ္စည်း'}</TableHead>
                      <TableHead>{languageToggle === 'en' ? 'Opening' : 'အစ'}</TableHead>
                      <TableHead>{languageToggle === 'en' ? 'Received' : 'လက်ခံ'}</TableHead>
                      <TableHead>{languageToggle === 'en' ? 'Issued' : 'ထုတ်ပေး'}</TableHead>
                      <TableHead>{languageToggle === 'en' ? 'Closing' : 'အဆုံး'}</TableHead>
                      <TableHead>{languageToggle === 'en' ? 'Status' : 'အခြေအနေ'}</TableHead>
                    </>
                  ) : (
                    <>
                      <TableHead>{languageToggle === 'en' ? 'Machine' : 'စက်'}</TableHead>
                      <TableHead>{languageToggle === 'en' ? 'Product' : 'ထုတ်ကုန်'}</TableHead>
                      <TableHead>{languageToggle === 'en' ? 'Target' : 'ပစ်မှတ်'}</TableHead>
                      <TableHead>{languageToggle === 'en' ? 'Actual' : 'အမှန်တကယ်'}</TableHead>
                      <TableHead>{languageToggle === 'en' ? 'Efficiency' : 'စွမ်းဆောင်မှု'}</TableHead>
                      <TableHead>{languageToggle === 'en' ? 'Status' : 'အခြေအနေ'}</TableHead>
                    </>
                  )}
                </TableRow>
              </TableHeader>
              <TableBody>
                {currentReport.category.includes('Inventory') ? (
                  inventoryData.map((item, index) => (
                    <TableRow key={index}>
                      <TableCell className="font-medium">{item.item}</TableCell>
                      <TableCell>{item.opening}</TableCell>
                      <TableCell>{item.received}</TableCell>
                      <TableCell>{item.issued}</TableCell>
                      <TableCell>{item.closing}</TableCell>
                      <TableCell>
                        <Badge 
                          className={
                            item.status === 'High' ? 'bg-green-100 text-green-800' :
                            item.status === 'Low' ? 'bg-red-100 text-red-800' :
                            'bg-blue-100 text-blue-800'
                          }
                        >
                          {item.status}
                        </Badge>
                      </TableCell>
                    </TableRow>
                  ))
                ) : (
                  productionData.map((item, index) => (
                    <TableRow key={index}>
                      <TableCell className="font-medium">{item.name}</TableCell>
                      <TableCell>Plastic Bottle 500ml</TableCell>
                      <TableCell>{item.target}</TableCell>
                      <TableCell>{item.actual}</TableCell>
                      <TableCell>
                        <div className="flex items-center gap-2">
                          <Progress value={item.efficiency} className="w-16" />
                          <span className="text-sm">{item.efficiency}%</span>
                        </div>
                      </TableCell>
                      <TableCell>
                        <Badge className={item.efficiency >= 100 ? 'bg-green-100 text-green-800' : 'bg-yellow-100 text-yellow-800'}>
                          {item.efficiency >= 100 ? 'Excellent' : 'Good'}
                        </Badge>
                      </TableCell>
                    </TableRow>
                  ))
                )}
              </TableBody>
            </Table>
          </div>
        </CardContent>
      </Card>
    </div>
  );

  return (
    <div className="h-screen bg-slate-50 p-6 overflow-y-auto">
      <div className="max-w-7xl mx-auto">
        {/* Report Header */}
        <div className="flex justify-between items-start mb-6">
          <div>
            <div className="flex items-center gap-4 mb-2">
              <div className="flex items-center gap-3">
                <currentReport.icon className="h-8 w-8 text-blue-600" />
                <div>
                  <h1 className="text-3xl font-semibold">
                    {languageToggle === 'en' ? currentReport.title : currentReport.titleMM}
                  </h1>
                  <p className="text-muted-foreground">
                    {languageToggle === 'en' ? currentReport.category : currentReport.categoryMM}
                  </p>
                </div>
              </div>
              {getRoleTagBadge(currentReport.roleTag, currentReport.roleColor)}
            </div>
          </div>
          
          <div className="flex items-center gap-3">
            {/* Language Toggle */}
            <Button 
              variant="outline" 
              size="sm"
              onClick={() => setLanguageToggle(languageToggle === 'en' ? 'mm' : 'en')}
            >
              {languageToggle === 'en' ? 'မြန်မာ' : 'EN'}
            </Button>
            
            {/* Export Options */}
            <div className="flex gap-2">
              <Button variant="outline" size="sm">
                <FileSpreadsheet className="h-4 w-4 mr-2" />
                {languageToggle === 'en' ? 'Excel' : 'Excel'}
              </Button>
              <Button variant="outline" size="sm">
                <FileText className="h-4 w-4 mr-2" />
                {languageToggle === 'en' ? 'PDF' : 'PDF'}
              </Button>
              <Button variant="outline" size="sm">
                <Printer className="h-4 w-4 mr-2" />
                {languageToggle === 'en' ? 'Print' : 'ပုံနှိပ်'}
              </Button>
            </div>
          </div>
        </div>

        {/* Filters */}
        {renderFilters()}

        {/* Report Content */}
        {currentReport.type === 'analytics' ? renderAnalyticsContent() : renderSummaryContent()}
      </div>
    </div>
  );
}