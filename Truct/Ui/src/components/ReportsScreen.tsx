import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Tabs, TabsContent, TabsList, TabsTrigger } from './ui/tabs';
import { 
  FileText, 
  Download, 
  Calendar, 
  Search,
  BarChart3,
  Activity,
  Package,
  Users
} from 'lucide-react';
import { ProductionReportTable } from './reports/ProductionReportTable';
import { QCReportTable } from './reports/QCReportTable';
import { InventoryReportTable } from './reports/InventoryReportTable';
import { productionReports, qcReports, inventoryReports } from './data/reportsData';
import { handleExport } from './utils/reportUtils';

export function ReportsScreen() {
  const [selectedTab, setSelectedTab] = useState('production');
  const [dateFilter, setDateFilter] = useState('today');
  const [searchTerm, setSearchTerm] = useState('');

  return (
    <div className="p-6 space-y-6 bg-slate-50 min-h-screen">
      {/* Header */}
      <div className="mb-6">
        <h1 className="text-2xl font-semibold text-slate-900">Reports & Analytics</h1>
        <p className="text-sm text-slate-600 mt-1">အစီရင်ခံစာများနှင့်ခွဲခြမ်းစိတ်ဖြာမှု</p>
      </div>

      {/* Filters */}
      <Card className="border-0 shadow-sm">
        <CardContent className="p-6">
          <div className="flex flex-col md:flex-row gap-4">
            <div className="flex-1">
              <div className="relative">
                <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-slate-400" />
                <Input
                  placeholder="Search reports... / အစီရင်ခံစာများရှာဖွေပါ..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="pl-10"
                />
              </div>
            </div>
            <div className="flex gap-4">
              <Select value={dateFilter} onValueChange={setDateFilter}>
                <SelectTrigger className="w-40">
                  <SelectValue placeholder="Date Range" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="today">Today / ယနေ့</SelectItem>
                  <SelectItem value="yesterday">Yesterday / မနေ့က</SelectItem>
                  <SelectItem value="week">This Week / ဒီအပတ်</SelectItem>
                  <SelectItem value="month">This Month / ဒီလ</SelectItem>
                  <SelectItem value="quarter">This Quarter / ဒီလေးပုံတစ်ပုံ</SelectItem>
                </SelectContent>
              </Select>
              <Button variant="outline">
                <Calendar className="h-4 w-4 mr-2" />
                Custom Range
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Main Content Tabs */}
      <Tabs value={selectedTab} onValueChange={setSelectedTab}>
        <TabsList className="grid w-full grid-cols-4">
          <TabsTrigger value="production">Production / ထုတ်လုပ်မှု</TabsTrigger>
          <TabsTrigger value="qc">QC Reports / QC အစီရင်ခံစာများ</TabsTrigger>
          <TabsTrigger value="inventory">Inventory / စတော့ခ်</TabsTrigger>
          <TabsTrigger value="hr">HR Reports / HR အစီရင်ခံစာများ</TabsTrigger>
        </TabsList>

        <TabsContent value="production" className="space-y-6">
          <Card className="border-0 shadow-sm">
            <CardHeader>
              <div className="flex items-center justify-between">
                <CardTitle className="flex items-center gap-2">
                  <BarChart3 className="h-5 w-5 text-blue-600" />
                  <div>
                    <div>Production Reports</div>
                    <div className="text-sm text-slate-500">ထုတ်လုပ်မှုအစီရင်ခံစာများ</div>
                  </div>
                </CardTitle>
                <div className="flex gap-2">
                  <Button 
                    variant="outline" 
                    size="sm"
                    onClick={() => handleExport('Production', 'Excel')}
                  >
                    <Download className="h-4 w-4 mr-1" />
                    Excel
                  </Button>
                  <Button 
                    variant="outline" 
                    size="sm"
                    onClick={() => handleExport('Production', 'PDF')}
                  >
                    <Download className="h-4 w-4 mr-1" />
                    PDF
                  </Button>
                </div>
              </div>
            </CardHeader>
            <CardContent>
              <ProductionReportTable reports={productionReports} />
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="qc" className="space-y-6">
          <Card className="border-0 shadow-sm">
            <CardHeader>
              <div className="flex items-center justify-between">
                <CardTitle className="flex items-center gap-2">
                  <Activity className="h-5 w-5 text-green-600" />
                  <div>
                    <div>Quality Control Reports</div>
                    <div className="text-sm text-slate-500">အရည်အသွေးထိန်းချုပ်မှုအစီရင်ခံစာများ</div>
                  </div>
                </CardTitle>
                <div className="flex gap-2">
                  <Button 
                    variant="outline" 
                    size="sm"
                    onClick={() => handleExport('QC', 'Excel')}
                  >
                    <Download className="h-4 w-4 mr-1" />
                    Excel
                  </Button>
                  <Button 
                    variant="outline" 
                    size="sm"
                    onClick={() => handleExport('QC', 'PDF')}
                  >
                    <Download className="h-4 w-4 mr-1" />
                    PDF
                  </Button>
                </div>
              </div>
            </CardHeader>
            <CardContent>
              <QCReportTable reports={qcReports} />
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="inventory" className="space-y-6">
          <Card className="border-0 shadow-sm">
            <CardHeader>
              <div className="flex items-center justify-between">
                <CardTitle className="flex items-center gap-2">
                  <Package className="h-5 w-5 text-purple-600" />
                  <div>
                    <div>Inventory Reports</div>
                    <div className="text-sm text-slate-500">စတော့ခ်အစီရင်ခံစာများ</div>
                  </div>
                </CardTitle>
                <div className="flex gap-2">
                  <Button 
                    variant="outline" 
                    size="sm"
                    onClick={() => handleExport('Inventory', 'Excel')}
                  >
                    <Download className="h-4 w-4 mr-1" />
                    Excel
                  </Button>
                  <Button 
                    variant="outline" 
                    size="sm"
                    onClick={() => handleExport('Inventory', 'PDF')}
                  >
                    <Download className="h-4 w-4 mr-1" />
                    PDF
                  </Button>
                </div>
              </div>
            </CardHeader>
            <CardContent>
              <InventoryReportTable reports={inventoryReports} />
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="hr" className="space-y-6">
          <Card className="border-0 shadow-sm">
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Users className="h-5 w-5 text-orange-600" />
                <div>
                  <div>HR Reports</div>
                  <div className="text-sm text-slate-500">HR အစီရင်ခံစာများ</div>
                </div>
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="text-center py-8">
                <FileText className="h-12 w-12 text-slate-400 mx-auto mb-4" />
                <div className="text-slate-500">HR Reports Coming Soon</div>
                <div className="text-sm text-slate-400">HR အစီရင်ခံစာများမကြာမီလာမည်</div>
              </div>
            </CardContent>
          </Card>
        </TabsContent>
      </Tabs>
    </div>
  );
}